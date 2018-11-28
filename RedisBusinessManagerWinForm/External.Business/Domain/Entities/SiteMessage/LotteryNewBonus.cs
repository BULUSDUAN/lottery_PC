using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace External.Domain.Entities.SiteMessage
{
    /// <summary>
    /// 彩票最新中奖
    /// </summary>
    public class LotteryNewBonus
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 方案号
        /// </summary>
        public virtual string SchemeId { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        public virtual string PlayType { get; set; }
        public virtual string IssuseNumber { get; set; }
        public virtual int Amount { get; set; }
        public virtual string UserDisplayName { get; set; }
        public virtual int HideUserDisplayNameCount { get; set; }
        public virtual decimal TotalMoney { get; set; }
        public virtual decimal PreTaxBonusMoney { get; set; }
        public virtual decimal AfterTaxBonusMoney { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
