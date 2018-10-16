using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kason.Net.Common.Utilities
{
    //KaSon.FrameWork.Kason.Net.Common.
    /// <summary>
    /// 前置条件验证异常。一旦出现此异常，表明传入参数不正确
    /// </summary>
    public class PreconditionException : Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常信息</param>
        public PreconditionException(string message)
            : base(message)
        {
        }
    }
}
