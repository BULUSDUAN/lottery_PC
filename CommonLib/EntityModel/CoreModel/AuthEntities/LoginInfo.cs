using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
    [Serializable]
    public class LoginInfo
    {
        /// <summary>
        /// 是否登录成功
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 登录号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// VIP 级别
        /// </summary>
        public int VipLevel { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 用户显示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 登录方式
        /// </summary>
        public string LoginFrom { get; set; }
        /// <summary>
        /// 用户口令
        /// </summary>
        public string UserToken { get; set; }

        public string Referrer { get; set; }
        public string RegType { get; set; }

        public string AgentId { get; set; }
        public bool IsAgent { get; set; }
        public int HideDisplayNameCount { get; set; }
        public DateTime CreateTime { get; set; }
        public List<string> FunctionList { get; set; }
        public bool IsAdmin { get; set; }
        public decimal CommissionBalance { get; set; }
        public decimal FreezeBalance { get; set; }
        public bool IsGeneralUser { get; set; }
        public string MaxLevelName { get; set; }
        public bool IsRebate { get; set; }
        /// <summary>
        /// 是否内部员工 0:网站普通用户；1：内部员工用户
        /// </summary>
        public bool IsUserType { get; set; }
    }
}

