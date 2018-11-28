using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;

namespace Activity.Domain.Entities
{
    public class A20130808_注册送3元
    {
        public virtual long Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual string RealName { get; set; }
        public virtual string Mobile { get; set; }
        public virtual decimal GiveMoney { get; set; }
        public virtual DateTime UpdateTime { get; set; }
    }

}
