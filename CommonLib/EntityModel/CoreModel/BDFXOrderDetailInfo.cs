using System;
using System.Collections.Generic;
using System.Text;
using EntityModel.Enum;
namespace EntityModel.CoreModel
{
   public class BDFXOrderDetailInfo
    {
        public BDFXOrderDetailInfo() { }

        public NearTimeProfitRate_Collection NearTimeProfitRateCollection { get; set; }
        public TicketStatus TicketStatus { get; set; }
        public SchemeBettingCategory SchemeBettingCategory { get; set; }
        public TogetherSchemeSecurity Security { get; set; }
        public decimal Commission { get; set; }
        public decimal CurrProfitRate { get; set; }
        public decimal CurrentBetMoney { get; set; }
        public bool IsComplate { get; set; }
        public decimal ExpectedBonusMoney { get; set; }
        public DateTime LastMatchStopTime { get; set; }
        public DateTime FirstMatchStopTime { get; set; }
        public int TotalMatchCount { get; set; }
        public string PlayType { get; set; }
        public int BetCount { get; set; }
        public int Amount { get; set; }
        public int RankNumber { get; set; }
        public string SingleTreasureDeclaration { get; set; }
        public decimal ExpectedReturnRate { get; set; }
        public string IssuseNumber { get; set; }
        public string GameType { get; set; }
        public string GameCode { get; set; }
        public decimal TotalBonusMoney { get; set; }
        public decimal ProfitRate { get; set; }
        public decimal AfterTaxBonusMoney { get; set; }
        public decimal TotalBuyMoney { get; set; }
        public int TotalBuyCount { get; set; }
        public string SchemeId { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public Sports_AnteCodeQueryInfoCollection AnteCodeCollection { get; set; }
        public List<AnteCodeInfo> AnteCodeList { get; set; }
    }
}
