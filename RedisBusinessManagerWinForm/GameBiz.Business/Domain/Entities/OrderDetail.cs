using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Auth.Domain.Entities;
using GameBiz.Domain.Entities;
using GameBiz.Core;

namespace GameBiz.Domain.Entities
{
    public class OrderDetail
    {
        public virtual string SchemeId { get; set; }
        public virtual DateTime CreateTime { get; set; }
        public virtual DateTime BetTime { get; set; }
        public virtual string UserId { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        public virtual string GameTypeName { get; set; }
        public virtual string PlayType { get; set; }
        public virtual int Amount { get; set; }
        public virtual SchemeType SchemeType { get; set; }
        public virtual SchemeSource SchemeSource { get; set; }
        public virtual SchemeBettingCategory SchemeBettingCategory { get; set; }
        public virtual decimal CurrentBettingMoney { get; set; }
        /// <summary>
        /// 红包金额
        /// </summary>
        public virtual decimal RedBagMoney { get; set; }
        public virtual decimal TotalMoney { get; set; }
        public virtual ProgressStatus ProgressStatus { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }
        public virtual int TotalIssuseCount { get; set; }
        public virtual string StartIssuseNumber { get; set; }
        public virtual string CurrentIssuseNumber { get; set; }
        public virtual BonusStatus BonusStatus { get; set; }
        public virtual decimal PreTaxBonusMoney { get; set; }
        public virtual decimal AfterTaxBonusMoney { get; set; }
        public virtual decimal AddMoney { get; set; }
        public virtual bool IsVirtualOrder { get; set; }
        public virtual bool StopAfterBonus { get; set; }
        public virtual string AgentId { get; set; }
        public virtual DateTime? ComplateTime { get; set; }
        public virtual bool IsAppend { get; set; }
        public virtual DateTime? TicketTime { get; set; }
        public virtual decimal TotalPayRebateMoney { get; set; }
        public virtual decimal RealPayRebateMoney { get; set; }
        public virtual decimal RedBagAwardsMoney { get; set; }
        public virtual decimal BonusAwardsMoney { get; set; }
    }
}
