using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;

namespace GameBiz.Auth.Domain.Entities
{
    /// <summary>
    /// 系统角色
    /// </summary>
    public class SystemRole
    {
        public virtual string RoleId { get; set; }
        public virtual string RoleName { get; set; }
        public virtual SystemRole ParentRole { get; set; }
        public virtual RoleType RoleType { get; set; }
        public virtual bool IsInner { get; set; }
        public virtual bool IsAdmin { get; set; }
        /// <summary>
        /// 角色包含功能列表
        /// </summary>
        public virtual IList<RoleFunction> FunctionList { get; set; }
    }
}
