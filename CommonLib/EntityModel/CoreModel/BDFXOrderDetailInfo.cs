using System;
using System.Collections.Generic;
using System.Text;
using EntityModel.Enum;
using ProtoBuf;
namespace EntityModel.CoreModel
{
    [ProtoContract]
   public class BDFXOrderDetailInfo
    {
        public BDFXOrderDetailInfo()
        {
            NearTimeProfitRateCollection = new NearTimeProfitRate_Collection();
            AnteCodeCollection = new Sports_AnteCodeQueryInfoCollection();
            AnteCodeList = new List<AnteCodeInfo>();
        }
        [ProtoMember(1)]
        public NearTimeProfitRate_Collection NearTimeProfitRateCollection { get; set; }
        [ProtoMember(2)]
        public TicketStatus TicketStatus { get; set; }
        [ProtoMember(3)]
        public SchemeBettingCategory SchemeBettingCategory { get; set; }
        [ProtoMember(4)]
        public TogetherSchemeSecurity Security { get; set; }
        [ProtoMember(5)]
        public decimal Commission { get; set; }
        [ProtoMember(6)]
        public decimal CurrProfitRate { get; set; }
        [ProtoMember(7)]
        public decimal CurrentBetMoney { get; set; }
        [ProtoMember(8)]
        public bool IsComplate { get; set; }
        [ProtoMember(9)]
        public decimal ExpectedBonusMoney { get; set; }
        [ProtoMember(10)]
        public DateTime LastMatchStopTime { get; set; }
        [ProtoMember(11)]
        public DateTime FirstMatchStopTime { get; set; }
        [ProtoMember(12)]
        public int TotalMatchCount { get; set; }
        [ProtoMember(13)]
        public string PlayType { get; set; }
        [ProtoMember(14)]
        public int BetCount { get; set; }
        [ProtoMember(15)]
        public int Amount { get; set; }
        [ProtoMember(16)]
        public int RankNumber { get; set; }
        [ProtoMember(17)]
        public string SingleTreasureDeclaration { get; set; }
        [ProtoMember(18)]
        public decimal ExpectedReturnRate { get; set; }
        [ProtoMember(19)]
        public string IssuseNumber { get; set; }
        [ProtoMember(20)]
        public string GameType { get; set; }
        [ProtoMember(21)]
        public string GameCode { get; set; }
        [ProtoMember(22)]
        public decimal TotalBonusMoney { get; set; }
        [ProtoMember(23)]
        public decimal ProfitRate { get; set; }
        [ProtoMember(24)]
        public decimal AfterTaxBonusMoney { get; set; }
        [ProtoMember(25)]
        public decimal TotalBuyMoney { get; set; }
        [ProtoMember(26)]
        public int TotalBuyCount { get; set; }
        [ProtoMember(27)]
        public string SchemeId { get; set; }
        [ProtoMember(28)]
        public string UserName { get; set; }
        [ProtoMember(29)]
        public string UserId { get; set; }
        [ProtoMember(30)]
        public Sports_AnteCodeQueryInfoCollection AnteCodeCollection { get; set; }
        [ProtoMember(31)]
        public List<AnteCodeInfo> AnteCodeList { get; set; }
    }
}
