using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Activity.Domain.Entities
{
    public class A20121128_首次中奖超过100送5元
    {
        public virtual long Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        public virtual string IssuseNumber { get; set; }
        public virtual string SchemeId { get; set; }
        public virtual decimal GiveMoney { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
