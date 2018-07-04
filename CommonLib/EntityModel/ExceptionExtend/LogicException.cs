using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace EntityModel.ExceptionExtend
{
    public class LogicException : System.Exception
    {
        public LogicException(string message)
            : base(message)
        {
        }
        public LogicException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
        protected LogicException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
