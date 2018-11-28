using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Activity.Domain.Entities
{
    public class A20131101_新用户首充送钱
    {
        public virtual long Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual string OrderId { get; set; }
        public virtual decimal FillMoney { get; set; }
        public virtual decimal CurrentGiveMoney { get; set; }
        public virtual int NextMonth { get; set; }
        public virtual decimal NextMonthGiveMoney { get; set; }
        public virtual bool IsGiveComplate { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

    public class A20131101_用户返点
    {
        public virtual long Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual int Month { get; set; }
        public virtual decimal TotalMoney { get; set; }
        public virtual decimal GiveMoney { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

    public class A20140318_充值最高送1000
    {
        public virtual long Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual string OrderId { get; set; }
        public virtual decimal FillMoney { get; set; }
        public virtual decimal CurrentGiveMoney { get; set; }
        public virtual int NextMonth { get; set; }
        public virtual decimal NextMonthGiveMoney { get; set; }
        public virtual bool IsGiveComplete { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
