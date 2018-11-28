using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Activity.Domain.Entities
{
    public class A20140318_现场送彩票
    {
        public virtual int Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual string MobileNumber { get; set; }
        public virtual string SchemeId { get; set; }
        public virtual int BetCount { get; set; }
        public virtual decimal BetMoney { get; set; }
        public virtual decimal BonusMoney { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
