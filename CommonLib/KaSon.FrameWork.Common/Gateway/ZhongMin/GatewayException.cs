using System;

namespace KaSon.FrameWork.Common.Gateway
{
    [Serializable]
    public class GatewayException : Exception
    {
        public GatewayException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public GatewayException(string message)
            : base(message)
        {
        }
    }
}