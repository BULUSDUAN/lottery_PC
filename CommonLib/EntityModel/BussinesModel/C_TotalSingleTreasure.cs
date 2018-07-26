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
    [Entity("C_TotalSingleTreasure",Type = EntityType.Table)]
    public class C_TotalSingleTreasure
    { 
        public C_TotalSingleTreasure()
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
            [Field("TotalBuyCount")]
            public int? TotalBuyCount{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(4)]
            [Field("TotalBuyMoney")]
            public decimal? TotalBuyMoney{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(5)]
            [Field("TotalBonusMoney")]
            public decimal? TotalBonusMoney{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(6)]
            [Field("ProfitRate")]
            public decimal? ProfitRate{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(7)]
            [Field("CurrentBetMoney")]
            public decimal? CurrentBetMoney{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(8)]
            [Field("CurrBonusMoney")]
            public decimal? CurrBonusMoney{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(9)]
            [Field("ExpectedBonusMoney")]
            public decimal? ExpectedBonusMoney{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(10)]
            [Field("ExpectedReturnRate")]
            public decimal? ExpectedReturnRate{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(11)]
            [Field("Commission")]
            public decimal? Commission{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(12)]
            [Field("TotalCommissionMoney")]
            public decimal? TotalCommissionMoney{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(13)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(14)]
            [Field("FirstMatchStopTime")]
            public DateTime? FirstMatchStopTime{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(15)]
            [Field("LastMatchStopTime")]
            public DateTime? LastMatchStopTime{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(16)]
            [Field("IsBonus")]
            public bool? IsBonus{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(17)]
            [Field("IsComplate")]
            public bool? IsComplate{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(18)]
            [Field("SingleTreasureDeclaration")]
            public string SingleTreasureDeclaration{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(19)]
            [Field("Security")]
            public int? Security{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(20)]
            [Field("CurrProfitRate")]
            public decimal? CurrProfitRate{ get; set; }
    }
}