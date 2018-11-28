using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Activity.Domain.Entities
{
    public class A20150120_JoinLuckyDraw
    {
        public virtual int Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual decimal PrizeMoney { get; set; }
        public virtual string OrderId { get; set; }
        public virtual string AgentId { get; set; }
        public virtual string PrizeType { get; set; }
        public virtual bool IsTestUser { get; set; }
        public virtual int ClientType { get; set; }
        public virtual string Description { get; set; }
        public virtual string LotteryNumber { get; set; }
        public virtual DateTime CreateTime { get; set; }
        public virtual string IdCardNumber { get; set; }
    }
}
