using System;
using System.Collections.Generic;
using System.Text;
using EntityModel.Enum;
namespace EntityModel.CoreModel
{
    public class TotalSingleTreasure_Collection : Page
    {
        public TotalSingleTreasure_Collection()
        { }
        public int AllTotalBuyCount { get; set; }
        public int AllTotalBonusMoney { get; set; }
        public IList<TotalSingleTreasureInfo> TotalSingleTreasureList { get; set; }
        public List<AnteCodeInfo> AnteCodeList { get; set; }
        public string FileCreateTime { get; set; }
    }
    public class TotalSingleTreasureInfo
    {
        public TotalSingleTreasureInfo()
        { }

        public decimal CurrProfitRate { get; set; }
        public decimal CurrentBetMoney { get; set; }
        public bool IsComplate { get; set; }
        public int TotalMatchCount { get; set; }
        public int BetCount { get; set; }
        public decimal ExpectedBonusMoney { get; set; }
        public decimal TotalBonusMoney { get; set; }
        public decimal ProfitRate { get; set; }
        public DateTime LastMatchStopTime { get; set; }
        public DateTime FirstMatchStopTime { get; set; }
        public decimal AfterTaxBonusMoney { get; set; }
        public decimal TotalBuyMoney { get; set; }
        public int TotalBuyCount { get; set; }
        public TogetherSchemeSecurity Security { get; set; }
        public decimal Commission { get; set; }
        public decimal ExpectedReturnRate { get; set; }
        public string IssuseNumber { get; set; }
        public string GameType { get; set; }
        public string GameCode { get; set; }
        public string SingleTreasureDeclaration { get; set; }
        public string SchemeId { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public decimal LastweekProfitRate { get; set; }
        public DateTime BDFXCreateTime { get; set; }
    }
    public class AnteCodeInfo
    {
        public AnteCodeInfo()
        { }
        public string SchemeId { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string PlayType { get; set; }
        public string IssuseNumber { get; set; }
        public string MatchId { get; set; }
        public string AnteCode { get; set; }
        public bool IsDan { get; set; }
    }
}
