using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Activity.Domain.Entities
{
    public class A20150120_LuckyDraw
    {
        public virtual int Id { get; set; }
        public virtual string LotteryNumber { get; set; }
        public virtual bool IsUse { get; set; }
        public virtual string BelongUserId { get; set; }
        public virtual string BelongUserName { get; set; }
        public virtual string LotteryType { get; set; }
        public virtual DateTime CreateTime { get; set; }
        public virtual string AgentId { get; set; }
    }
}
