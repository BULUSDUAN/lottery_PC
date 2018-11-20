using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Common.Communication
{
    /// <summary>
    /// 逻辑错误异常，系统约定此异常不会写日志
    /// </summary>
    public class LogicException : Exception
    {
        public LogicException(string message)
            : base(message)
        {
        }
        public LogicException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        protected LogicException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
