using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Common.Database.ORM
{
    /// <summary>
    /// ORM 异常类定义
    /// </summary>
    [Serializable]
    public class ORMException : Exception
    {
        public ORMException(string message)
            : base(message)
        {
        }
        public ORMException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        protected ORMException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
