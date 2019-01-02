﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace External.Core.Authentication
{
    /// <summary>
    /// 用户手机认证请求信息
    /// </summary>
    [CommunicationObject]
    [Serializable]
    public class UserMobileRequestInfo
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 已请求次数
        /// </summary>
        public int RequestTimes { get; set; }
        /// <summary>
        /// 是否可以换
        /// </summary>
        public bool CanBeChanged { get; set; }
        /// <summary>
        /// 剩余秒数
        /// </summary>
        public int Seconds { get; set; }
    }
}