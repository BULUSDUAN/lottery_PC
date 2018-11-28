using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Common.Database.DbAccess
{
    /// <summary>
    /// 数据库访问异常类定义
    /// </summary>
    [Serializable]
    public class DbAccessException : Exception
    {
        public DbAccessException(string message)
            : base(message)
        {
        }
        public DbAccessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        protected DbAccessException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
