using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Activity.Domain.Entities
{
    /// <summary>
    /// VIP充值积分总数
    /// </summary>
    public class A20130226VipScoreSummary
    {
        public virtual long Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual int VipLeavel { get; set; }
        public virtual int TotalScore { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// VIP充值积分明细
    /// </summary>
    public class A20130226VipScoreDetail
    {
        public virtual long Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual decimal FillMoney { get; set; }
        public virtual int Score { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

    //    2、	每个等级的VIP都可以额外获得一次充值送彩金的机会：
    //VIP1充值1000送58
    //VIP2充值2000送108
    //VIP3充值3000送188
    //VIP4充值5000送588
    //VIP5充值10000送888
    //VIP6充值50000送3888 
    public class A20130226VipFillMoneyGiveRecord
    {
        public virtual long Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual int VipLeavel { get; set; }
        public virtual decimal GiveMoney { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

}
