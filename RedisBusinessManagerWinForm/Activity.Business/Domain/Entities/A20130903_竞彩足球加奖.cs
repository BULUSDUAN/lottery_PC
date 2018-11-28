using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;

namespace Activity.Domain.Entities
{
    public class A20130903_JCZQ加奖
    {
        public virtual long Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual SchemeType SchemeType { get; set; }
        public virtual string SchemeId { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        public virtual string PlayType { get; set; }
        public virtual string IssuseNumber { get; set; }
        public virtual decimal AddMoney { get; set; }
        public virtual decimal OrderMoney { get; set; }
        public virtual decimal afterTaxBonusMoney { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

    public class A20140318_竞彩北单奖上奖
    {
        public virtual long Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual SchemeType SchemeType { get; set; }
        public virtual string SchemeId { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        public virtual string PlayType { get; set; }
        public virtual string IssuseNumber { get; set; }
        public virtual decimal AddMoney { get; set; }
        public virtual decimal OrderMoney { get; set; }
        public virtual decimal AfterTaxBonusMoney { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
