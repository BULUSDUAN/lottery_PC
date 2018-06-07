using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Communication
{
    /// <summary>
    /// 操作结果
    /// </summary>

    public class CommonActionResult
    {
        public CommonActionResult()
            : this(false, "")
        {
        }
        public CommonActionResult(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
        /// <summary>
        /// 是否操作成功
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 返回消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 返回值
        /// </summary>
        public string ReturnValue { get; set; }
    }
}
