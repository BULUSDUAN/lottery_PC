using System.ServiceModel;

namespace Common.Communication
{
    /// <summary>
    /// Http协议的Wcf客户端基类
    /// </summary>
    public abstract class HttpWcfClient : WcfClient
    {
        protected HttpWcfClient(string uri)
            : base(uri)
        {
            HttpSecurityMode = BasicHttpSecurityMode.None;
        }
        /// <summary>
        /// Http传输安全模式
        /// </summary>
        public BasicHttpSecurityMode HttpSecurityMode { get; set; }

        protected override System.ServiceModel.Channels.Binding CreateBinding()
        {
            var bind = new BasicHttpBinding(HttpSecurityMode)
            {
                MaxBufferPoolSize = MaxBufferPoolSize,
                MaxBufferSize = MaxBufferSize,
                MaxReceivedMessageSize = MaxReceivedMessageSize,
                ReaderQuotas = { MaxArrayLength = MaxArrayLength },
                ReceiveTimeout = ReceiveTimeout,
                SendTimeout = SendTimeout,
                OpenTimeout = OpenTimeout,
                CloseTimeout = CloseTimeout
            };
            return bind;
        }
    }
}