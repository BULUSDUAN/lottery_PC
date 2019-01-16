using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel.AuthEntities
{
    public class UserMobile
    {
        public virtual string UserId { get; set; }
        /// <summary>
        /// 系统用户
        /// </summary>
        public virtual SystemUser User { get; set; }
        public virtual bool IsSettedMobile { get; set; }
        public virtual string AuthFrom { get; set; }
        public virtual string Mobile { get; set; }
        public virtual int RequestTimes { get; set; }
        public virtual DateTime CreateTime { get; set; }
        public virtual string CreateBy { get; set; }
        public virtual DateTime UpdateTime { get; set; }
        public virtual string UpdateBy { get; set; }
    }
}
