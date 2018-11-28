using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Auth.Domain.Entities;

namespace External.Domain.Entities.Authentication
{
    public class UserRealName
    {
        public virtual string UserId { get; set; }
        /// <summary>
        /// 系统用户
        /// </summary>
        public virtual SystemUser User { get; set; }
        public virtual bool IsSettedRealName { get; set; }
        public virtual string AuthFrom { get; set; }
        public virtual string RealName { get; set; }
        public virtual string CardType { get; set; }
        public virtual string IdCardNumber { get; set; }
        public virtual DateTime CreateTime { get; set; }
        public virtual string CreateBy { get; set; }
        public virtual DateTime UpdateTime { get; set; }
        public virtual string UpdateBy { get; set; }
    }
}
