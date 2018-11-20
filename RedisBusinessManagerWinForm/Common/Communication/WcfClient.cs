using System;
using System.ServiceModel;

namespace Common.Communication
{
    /// <summary>
    /// Wcf客户端基类
    /// </summary>
    public abstract class WcfClient : IDisposable
    {
        /// <summary>
        /// 要连接的服务器端Uri
        /// </summary>
        public string Uri { get; set; }
        /// <summary>
        /// 接收数据超时时间
        /// </summary>
        public TimeSpan ReceiveTimeout { get; set; }
        /// <summary>
        /// 发送数据超时时间
        /// </summary>
        public TimeSpan SendTimeout { get; set; }
        /// <summary>
        /// 打开连接超时时间
        /// </summary>
        public TimeSpan OpenTimeout { get; set; }
        /// <summary>
        /// 关闭连接超时时间
        /// </summary>
        public TimeSpan CloseTimeout { get; set; }

        /// <summary>
        /// 最大连接池缓存大小
        /// </summary>
        public long MaxBufferPoolSize { get; set; }
        /// <summary>
        /// 最大缓存大小
        /// </summary>
        public int MaxBufferSize { get; set; }
        /// <summary>
        /// 最大接收数据大小
        /// </summary>
        public long MaxReceivedMessageSize { get; set; }
        /// <summary>
        /// 最大数组长度
        /// </summary>
        public int MaxArrayLength { get; set; }

        protected WcfClient(string uri)
        {
            Uri = uri;
            ReceiveTimeout = new TimeSpan(1, 0, 0);
            SendTimeout = new TimeSpan(1, 0, 0);
            OpenTimeout = new TimeSpan(0, 20, 0);
            CloseTimeout = new TimeSpan(0, 1, 0);

            MaxBufferPoolSize = 52428800;
            MaxBufferSize = 2147483647;
            MaxReceivedMessageSize = 2147483647;
            MaxArrayLength = 2147483647;
        }

        protected abstract System.ServiceModel.Channels.Binding CreateBinding();

        private ChannelFactory<IWcfService> _channel;
        private void CreateServiceChannel()
        {
            if (_channel == null)
            {
                var bind = CreateBinding();
                var address = new EndpointAddress(Uri);
                _channel = new ChannelFactory<IWcfService>(bind, address);
            }
        }

        private IWcfService _service;
        private IWcfService GetService()
        {
            if (_service == null)
            {
                CreateServiceChannel();
                _service = _channel.CreateChannel();
            }
            return _service;
        }
        private static byte[] Process(byte[] buffer, Func<byte[], byte[]> action)
        {
            buffer = WcfByteHandler.CompressData(buffer);
            buffer = WcfByteHandler.EncryptData(buffer);
            buffer = action(buffer);
            if (buffer != null)
            {
                buffer = WcfByteHandler.DecryptData(buffer);
                buffer = WcfByteHandler.DecompressData(buffer);
            }
            return buffer;
        }
        /// <summary>
        /// 调用服务器端一个函数，并返回一个值
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="invoke">参数</param>
        /// <returns>返回值</returns>
        protected T Invoke<T>(WcfInvokeInfo invoke)
        {
            try
            {
                byte[] bs = CommunicationSerializer.SerializeDataContract(invoke, "Communication", "www.qcw.com", KnownTypeRegister.GetKnownTypes());
                bs = Process(bs, (buffer) =>
                {
                    var service = GetService();
                    return service.Process(buffer);
                });
                var result = CommunicationSerializer.DeserializeDataContract(typeof(WcfInvokeResult), "Communication", "www.qcw.com", KnownTypeRegister.GetKnownTypes(), bs) as WcfInvokeResult;
                if (result != null)
                {
                    if (!result.IsSuccess)
                    {
                        throw new WcfException(result.ErrorMessage);
                    }
                    return (T)result.Data;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("http:") > 0)
                {
                    var writer = Common.Log.LogWriterGetter.GetLogWriter();
                    writer.Write("WCF连接异常", "Invoke_WCF连接异常", Common.Log.LogType.Error, "WCF连接异常", ex.ToString());
                    throw new Exception("您好，目前服务器繁忙，请稍后再试！");
                }
                else throw new Exception(ex.Message);
            }
            return default(T);
        }
        /// <summary>
        /// 调用服务器端一个无返回值的函数
        /// </summary>
        /// <param name="invoke">参数</param>
        protected void InvokeViaOneWay(WcfInvokeInfo invoke)
        {
            var bs = CommunicationSerializer.SerializeDataContract(invoke, "Communication", "www.qcw.com", KnownTypeRegister.GetKnownTypes());
            Process(bs, buffer =>
            {
                var service = GetService();
                service.ProcessViaOneWay(buffer);
                return null;
            });
        }

        #region IDisposable 成员

        private bool _isDisposed;
        /// <summary>
        /// 释放资源，关闭通信通道
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                try
                {
                    if (_channel != null)
                    {
                        _channel.Close();
                    }
                }
                finally
                {
                    _isDisposed = true;
                }
            }
        }

        #endregion
    }
}