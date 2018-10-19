using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
    [ProtoContract]
    [Serializable]
    public class LoginInfo
    {
        /// <summary>
        /// 是否登录成功
        /// </summary>
        [ProtoMember(1)]
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        [ProtoMember(2)]
        public string Message { get; set; }
        /// <summary>
        /// 登录号
        /// </summary>
        [ProtoMember(3)]
        public string UserId { get; set; }
        /// <summary>
        /// VIP 级别
        /// </summary>
        [ProtoMember(4)]
        public int VipLevel { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        [ProtoMember(5)]
        public string LoginName { get; set; }
        /// <summary>
        /// 用户显示名称
        /// </summary>
        [ProtoMember(6)]
        public string DisplayName { get; set; }
        /// <summary>
        /// 登录方式
        /// </summary>
        [ProtoMember(7)]
        public string LoginFrom { get; set; }
        /// <summary>
        /// 用户口令
        /// </summary>
        [ProtoMember(8)]
        public string UserToken { get; set; }
        [ProtoMember(9)]
        public string Referrer { get; set; }
        [ProtoMember(10)]
        public string RegType { get; set; }
        [ProtoMember(11)]
        public string AgentId { get; set; }
        [ProtoMember(12)]
        public bool IsAgent { get; set; }
        [ProtoMember(13)]
        public int HideDisplayNameCount { get; set; }
        [ProtoMember(14)]
        public DateTime CreateTime { get; set; }
        [ProtoMember(15)]
        public List<string> FunctionList { get; set; }
        [ProtoMember(16)]
        public bool IsAdmin { get; set; }
        [ProtoMember(17)]
        public decimal CommissionBalance { get; set; }
        [ProtoMember(18)]
        public decimal FreezeBalance { get; set; }
        [ProtoMember(19)]
        public bool IsGeneralUser { get; set; }
        [ProtoMember(20)]
        public string MaxLevelName { get; set; }
        [ProtoMember(21)]
        public bool IsRebate { get; set; }
        /// <summary>
        /// 是否内部员工 0:网站普通用户；1：内部员工用户
        /// </summary>
        [ProtoMember(22)]
        public bool IsUserType { get; set; }
    }
}


