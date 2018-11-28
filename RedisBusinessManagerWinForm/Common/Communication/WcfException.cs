using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Common.Communication
{
    /// <summary>
    /// Wcf调用过程的错误异常
    /// </summary>
    public class WcfException : Exception
    {
        public WcfException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        protected WcfException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        public WcfException(string message)
            : base(message)
        {
        }
    }
}
