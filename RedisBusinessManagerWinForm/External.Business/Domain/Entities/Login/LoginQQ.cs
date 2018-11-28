using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Auth.Domain.Entities;
using GameBiz.Domain.Entities;

namespace External.Domain.Entities.Login
{
    public class LoginQQ
    {
        public virtual string UserId { get; set; }
        /// <summary>
        /// 系统用户
        /// </summary>
        public virtual SystemUser User { get; set; }
        /// <summary>
        /// 用户注册信息
        /// </summary>
        public virtual UserRegister Register { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public virtual string LoginName { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public virtual string DisplayName { get; set; }
        /// <summary>
        /// 开放编号
        /// </summary>
        public virtual string OpenId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }
}
