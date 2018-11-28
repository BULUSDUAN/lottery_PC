using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Activity.Domain.Entities
{
    public class A20140307_美家颂
    {
        public virtual int Id { get; set; }
        public virtual string MJSNumber { get; set; }
        public virtual string BelongUserId { get; set; }
        public virtual DateTime UsedTime { get; set; }
        public virtual DateTime CreateTime { get; set; }

    }
}
