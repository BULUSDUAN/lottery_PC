using System;
using System.Collections.Generic;
using System.Text;
using EntityModel.Enum;
using ProtoBuf;
namespace EntityModel.CoreModel
{
    [ProtoContract]
    public class TotalSingleTreasure_Collection : Page
    {
        public TotalSingleTreasure_Collection()
        {}
        [ProtoMember(1)]
        public int AllTotalBuyCount { get; set; }
        [ProtoMember(2)]
        public int AllTotalBonusMoney { get; set; }
        [ProtoMember(3)]
        public IList<TotalSingleTreasureInfo> TotalSingleTreasureList { get; set; }
        [ProtoMember(4)]
        public List<AnteCodeInfo> AnteCodeList { get; set; }
        [ProtoMember(5)]
        public string FileCreateTime { get; set; }
    }
    [ProtoContract]
    public class TotalSingleTreasureInfo
    {
        public TotalSingleTreasureInfo()
        { }
        [ProtoMember(1)]
        public decimal CurrProfitRate { get; set; }
        [ProtoMember(2)]
        public decimal CurrentBetMoney { get; set; }
        [ProtoMember(3)]
        public bool IsComplate { get; set; }
        [ProtoMember(4)]
        public int TotalMatchCount { get; set; }
        [ProtoMember(5)]
        public int BetCount { get; set; }
        [ProtoMember(6)]
        public decimal ExpectedBonusMoney { get; set; }
        [ProtoMember(7)]
        public decimal TotalBonusMoney { get; set; }
        [ProtoMember(8)]
        public decimal ProfitRate { get; set; }
        [ProtoMember(9)]
        public DateTime LastMatchStopTime { get; set; }
        [ProtoMember(10)]
        public DateTime FirstMatchStopTime { get; set; }
        [ProtoMember(11)]
        public decimal AfterTaxBonusMoney { get; set; }
        [ProtoMember(12)]
        public decimal TotalBuyMoney { get; set; }
        [ProtoMember(13)]
        public int TotalBuyCount { get; set; }
        [ProtoMember(14)]
        public TogetherSchemeSecurity Security { get; set; }
        [ProtoMember(15)]
        public decimal Commission { get; set; }
        [ProtoMember(16)]
        public decimal ExpectedReturnRate { get; set; }
        [ProtoMember(17)]
        public string IssuseNumber { get; set; }
        [ProtoMember(18)]
        public string GameType { get; set; }
        [ProtoMember(19)]
        public string GameCode { get; set; }
        [ProtoMember(20)]
        public string SingleTreasureDeclaration { get; set; }
        [ProtoMember(21)]
        public string SchemeId { get; set; }
        [ProtoMember(22)]
        public string UserName { get; set; }
        [ProtoMember(23)]
        public string UserId { get; set; }
        [ProtoMember(24)]
        public decimal LastweekProfitRate { get; set; }
        [ProtoMember(25)]
        public DateTime BDFXCreateTime { get; set; }
    }
    [ProtoContract]
    public class AnteCodeInfo
    {
        public AnteCodeInfo()
        { }
        [ProtoMember(1)]
        public string SchemeId { get; set; }
        [ProtoMember(2)]
        public string GameCode { get; set; }
        [ProtoMember(3)]
        public string GameType { get; set; }
        [ProtoMember(4)]
        public string PlayType { get; set; }
        [ProtoMember(5)]
        public string IssuseNumber { get; set; }
        [ProtoMember(6)]
        public string MatchId { get; set; }
        [ProtoMember(7)]
        public string AnteCode { get; set; }
        [ProtoMember(8)]
        public bool IsDan { get; set; }
    }
}
