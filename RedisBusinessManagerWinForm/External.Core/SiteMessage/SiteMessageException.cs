using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace External.Core.SiteMessage
{
    /// <summary>
    /// 站点消息异常
    /// </summary>
    public class SiteMessageException : Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常消息</param>
        public SiteMessageException(string message)
            : base(message)
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="innerException">引起此异常的异常</param>
        public SiteMessageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
