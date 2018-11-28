using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Common.CommonWCF
{
    public abstract class HttpWcfClient_JSON : WcfClient_JSON
    {
        protected HttpWcfClient_JSON(string uri)
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
