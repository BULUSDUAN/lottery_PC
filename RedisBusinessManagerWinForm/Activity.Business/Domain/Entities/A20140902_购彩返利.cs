using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;

namespace Activity.Domain.Entities
{
    /// <summary>
    /// A20140902_购彩返利
    /// </summary>
    public class A20140902_购彩返利
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
