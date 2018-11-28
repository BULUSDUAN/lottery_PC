using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;

namespace Activity.Domain.Entities
{
    public class A20141203GiveLottery
    {
        public virtual string UserId { get; set; }
        public virtual string AnteCode { get; set; }
        public virtual string SchemeId { get; set; }
        public virtual SchemeSource ActivityType { get; set; }
        public virtual DateTime CreateTime { get; set; }
        public virtual string IdCardNumber { get; set; }
    }
}
