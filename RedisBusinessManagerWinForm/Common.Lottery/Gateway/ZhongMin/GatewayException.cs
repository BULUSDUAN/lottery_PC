using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Lottery.Gateway.ZhongMin
{
    public class GatewayException: Exception
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
