using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Communication
{
    /// <summary>
    /// 操作结果
    /// </summary>
    [ProtoContract]
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
        [ProtoMember(1)]
        public bool IsSuccess { get; set; } = true;
        /// <summary>
        /// 返回消息
        /// </summary>
        [ProtoMember(2)]
        public string Message { get; set; } = "";
        /// <summary>
        /// 返回值
        /// </summary>
        [ProtoMember(3)]
        public string ReturnValue { get; set; } = "";

        public object Value { get; set; }

        /// <summary>
        /// 200 正确，502 系统错误，403
        /// </summary>
        public int StatuCode { get; set; } = 200;
        public int Code { get; set; } = 101;
        public string MsgId { get; set; } = "";

        public string Sign { get; set; } = "";
    }
    
}
