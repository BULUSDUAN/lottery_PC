using EntityModel.CoreModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
     public class LoginLocal
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
        /// 登录名
        /// </summary>
        public virtual string LoginName { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public virtual string Password { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public virtual string mobile { get; set; }
    }
}
