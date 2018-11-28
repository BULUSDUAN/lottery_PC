using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Activity.Domain.Entities
{
    public class A20121009
    {
        public virtual Int64 Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual int SchemeType { get; set; }
        public virtual int SchemeId { get; set; }
        public virtual string GameType { get; set; }
        public virtual string IssuseNumber { get; set; }
        public virtual int HitMatchCount { get; set; }
        public virtual decimal AddMoney { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
