using System;
using System.ServiceModel;

namespace Common.Communication
{
    /// <summary>
    /// Wcf�ͻ��˻���
    /// </summary>
    public abstract class WcfClient : IDisposable
    {
        /// <summary>
        /// Ҫ���ӵķ�������Uri
        /// </summary>
        public string Uri { get; set; }
        /// <summary>
        /// �������ݳ�ʱʱ��
        /// </summary>
        public TimeSpan ReceiveTimeout { get; set; }
        /// <summary>
        /// �������ݳ�ʱʱ��
        /// </summary>
        public TimeSpan SendTimeout { get; set; }
        /// <summary>
        /// �����ӳ�ʱʱ��
        /// </summary>
        public TimeSpan OpenTimeout { get; set; }
        /// <summary>
        /// �ر����ӳ�ʱʱ��
        /// </summary>
        public TimeSpan CloseTimeout { get; set; }

        /// <summary>
        /// ������ӳػ����С
        /// </summary>
        public long MaxBufferPoolSize { get; set; }
        /// <summary>
        /// ��󻺴��С
        /// </summary>
        public int MaxBufferSize { get; set; }
        /// <summary>
        /// ���������ݴ�С
        /// </summary>
        public long MaxReceivedMessageSize { get; set; }
        /// <summary>
        /// ������鳤��
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
        /// ���÷�������һ��������������һ��ֵ
        /// </summary>
        /// <typeparam name="T">����ֵ����</typeparam>
        /// <param name="invoke">����</param>
        /// <returns>����ֵ</returns>
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
                    writer.Write("WCF�����쳣", "Invoke_WCF�����쳣", Common.Log.LogType.Error, "WCF�����쳣", ex.ToString());
                    throw new Exception("���ã�Ŀǰ��������æ�����Ժ����ԣ�");
                }
                else throw new Exception(ex.Message);
            }
            return default(T);
        }
        /// <summary>
        /// ���÷�������һ���޷���ֵ�ĺ���
        /// </summary>
        /// <param name="invoke">����</param>
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

        #region IDisposable ��Ա

        private bool _isDisposed;
        /// <summary>
        /// �ͷ���Դ���ر�ͨ��ͨ��
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