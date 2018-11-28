using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Activity.Domain.Entities
{
    public class A20130114_加奖统计
    {
        public virtual long Id { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        public virtual string IssuseNumber { get; set; }
        public virtual string UserId { get; set; }
        public virtual string SchemeId { get; set; }
        public virtual decimal OrderMoney { get; set; }
        public virtual decimal BonusMoney { get; set; }
        public virtual decimal AddMoney { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
