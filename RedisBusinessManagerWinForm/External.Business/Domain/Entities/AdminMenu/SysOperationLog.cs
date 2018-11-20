using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace External.Domain.Entities.AdminMenu
{
    public class SysOperationLog
    {
        public virtual int Id { get; set; }
        public virtual string MenuName { get; set; }
        public virtual string Description { get; set; }
        public virtual string UserId { get; set; }
        public virtual string OperUserId { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
