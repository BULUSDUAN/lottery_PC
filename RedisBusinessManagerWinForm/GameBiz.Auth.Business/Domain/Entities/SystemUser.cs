using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace GameBiz.Auth.Domain.Entities
{
    /// <summary>
    /// 登录帐号
    /// </summary>
    public class SystemUser
    {
        /// <summary>
        /// 帐号唯一标识
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 注册来源
        /// </summary>
        public virtual string RegFrom { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 代理商
        /// </summary>
        public virtual string AgentId { get; set; }
        /// <summary>
        /// 所属角色（可多种）
        /// </summary>
        public virtual IList<SystemRole> RoleList { get; set; }
        /// <summary>
        /// 用户包含功能列表
        /// </summary>
        public virtual IList<UserFunction> FunctionList { get; set; }
    }
    /// <summary>
    /// 用户编号规则
    /// </summary>
    public class UserKeyRule
    {
        public virtual string RuleKey { get; set; }
        public virtual string RuleValue { get; set; }
    }
    /// <summary>
    /// 靓号
    /// </summary>
    public class BeautyUserKey
    {
        public virtual string BeautyKey { get; set; }
        public virtual string PrevUserKey { get; set; }
        public virtual string NextUserKey { get; set; }
        public virtual string Status { get; set; }
    }
}
