using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
    public class Data_QueryBonusInfo
    {
        public long RowNumber { get; set; }

        public string SchemeId { get; set; }

        public string GameCode { get; set; }

        public string GameType { get; set; }

        public string PlayType { get; set; }

        public int SchemeType { get; set; }

        public string IssuseNumber { get; set; }

        public int Amount { get; set; }

        public int BetCount { get; set; }

        public int TotalMatchCount { get; set; }

        public decimal TotalMoney { get; set; }

        public int TicketStatus { get; set; }

        public int ProgressStatus { get; set; }

        public int BonusStatus { get; set; }

        public decimal PreTaxBonusMoney { get; set; }

        public decimal AfterTaxBonusMoney { get; set; }

        public bool IsVirtualOrder { get; set; }

        public DateTime CreateTime { get; set; }

        public int BonusCount { get; set; }

        public string BonusCountDescription { get; set; }

        public string BonusCountDisplayName { get; set; }

        public DateTime? ComplateDateTime { get; set; }

        public string ComplateDate { get; set; }

        public bool IsPrizeMoney { get; set; }

        public string AgentId { get; set; }

        public string UserId { get; set; }

        public string DisplayName { get; set; }

        public bool IsAgent { get; set; }

        public int HideDisplayNameCount { get; set; }
    }
}
