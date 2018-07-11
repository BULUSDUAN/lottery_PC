﻿using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Communication
{
    /// <summary>
    /// 操作结果
    /// </summary>
    [ProtoContract]
    [Serializable]
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
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 返回消息
        /// </summary>
        [ProtoMember(2)]
        public string Message { get; set; }
        /// <summary>
        /// 返回值
        /// </summary>
        [ProtoMember(3)]
        public string ReturnValue { get; set; }
    }
}
