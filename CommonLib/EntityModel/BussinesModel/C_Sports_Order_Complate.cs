using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace EntityModel
{
    /// <summary>
    // 
    ///</summary>
    [ProtoContract]
    [Entity("C_Sports_Order_Complate",Type = EntityType.Table)]
    public class C_Sports_Order_Complate
    { 
        public C_Sports_Order_Complate()
        {
        
        }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(1)]
            [Field("SchemeId", IsIdenty = false, IsPrimaryKey = true)]
            public string SchemeId{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(3)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(4)]
            [Field("GameType")]
            public string GameType{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(5)]
            [Field("PlayType")]
            public string PlayType{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(6)]
            [Field("SchemeType")]
            public int? SchemeType{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(7)]
            [Field("Security")]
            public int? Security{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(8)]
            [Field("SchemeSource")]
            public int? SchemeSource{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(9)]
            [Field("SchemeBettingCategory")]
            public int? SchemeBettingCategory{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(10)]
            [Field("IssuseNumber")]
            public string IssuseNumber{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(11)]
            [Field("Amount")]
            public int? Amount{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(12)]
            [Field("BetCount")]
            public int? BetCount{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(13)]
            [Field("TotalMatchCount")]
            public int? TotalMatchCount{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(14)]
            [Field("TotalMoney")]
            public decimal? TotalMoney{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(15)]
            [Field("RedBagMoney")]
            public decimal? RedBagMoney{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(16)]
            [Field("SchemeDeduct")]
            public decimal? SchemeDeduct{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(17)]
            [Field("StopTime")]
            public DateTime? StopTime{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(18)]
            [Field("TicketStatus")]
            public int? TicketStatus{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(19)]
            [Field("TicketGateway")]
            public string TicketGateway{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(20)]
            [Field("TicketProgress")]
            public decimal? TicketProgress{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(21)]
            [Field("TicketId")]
            public string TicketId{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(22)]
            [Field("TicketLog")]
            public string TicketLog{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(23)]
            [Field("ProgressStatus")]
            public int? ProgressStatus{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(24)]
            [Field("BonusStatus")]
            public int? BonusStatus{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(25)]
            [Field("HitMatchCount")]
            public int? HitMatchCount{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(26)]
            [Field("RightCount")]
            public int? RightCount{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(27)]
            [Field("Error1Count")]
            public int? Error1Count{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(28)]
            [Field("Error2Count")]
            public int? Error2Count{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(29)]
            [Field("MinBonusMoney")]
            public decimal? MinBonusMoney{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(30)]
            [Field("MaxBonusMoney")]
            public decimal? MaxBonusMoney{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(31)]
            [Field("PreTaxBonusMoney")]
            public decimal? PreTaxBonusMoney{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(32)]
            [Field("AfterTaxBonusMoney")]
            public decimal? AfterTaxBonusMoney{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(33)]
            [Field("CanChase")]
            public bool? CanChase{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(34)]
            [Field("IsVirtualOrder")]
            public bool? IsVirtualOrder{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(35)]
            [Field("IsPayRebate")]
            public bool? IsPayRebate{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(36)]
            [Field("TotalPayRebateMoney")]
            public decimal? TotalPayRebateMoney{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(37)]
            [Field("RealPayRebateMoney")]
            public decimal? RealPayRebateMoney{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(38)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(39)]
            [Field("BetTime")]
            public DateTime? BetTime{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(40)]
            [Field("BonusCount")]
            public int? BonusCount{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(41)]
            [Field("BonusCountDescription")]
            public string BonusCountDescription{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(42)]
            [Field("BonusCountDisplayName")]
            public string BonusCountDisplayName{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(43)]
            [Field("ComplateDateTime")]
            public DateTime? ComplateDateTime{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(44)]
            [Field("ComplateDate")]
            public string ComplateDate{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(45)]
            [Field("IsPrizeMoney")]
            public bool? IsPrizeMoney{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(46)]
            [Field("AddMoney")]
            public decimal? AddMoney{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(47)]
            [Field("DistributionWay")]
            public int? DistributionWay{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(48)]
            [Field("AddMoneyDescription")]
            public string AddMoneyDescription{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(49)]
            [Field("AgentId")]
            public string AgentId{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(50)]
            [Field("SuccessMoney")]
            public decimal? SuccessMoney{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(51)]
            [Field("ExtensionOne")]
            public string ExtensionOne{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(52)]
            [Field("Attach")]
            public string Attach{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(53)]
            [Field("IsAppend")]
            public bool? IsAppend{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(54)]
            [Field("TicketTime")]
            public DateTime? TicketTime{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(55)]
            [Field("IsSplitTickets")]
            public bool? IsSplitTickets{ get; set; }
    }
}