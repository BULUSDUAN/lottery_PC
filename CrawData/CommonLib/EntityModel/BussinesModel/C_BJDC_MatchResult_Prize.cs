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
    [Entity("C_BJDC_MatchResult_Prize",Type = EntityType.Table)]
    public class C_BJDC_MatchResult_Prize
    { 
        public C_BJDC_MatchResult_Prize()
        {
        
        }
            /// <summary>
            // IssuseNumber|MatchOrderId
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = false, IsPrimaryKey = true)]
            public string Id{ get; set; }
            /// <summary>
            // 期号
            ///</summary>
            [ProtoMember(2)]
            [Field("IssuseNumber")]
            public string IssuseNumber{ get; set; }
            /// <summary>
            // 联赛排序编号
            ///</summary>
            [ProtoMember(3)]
            [Field("MatchOrderId")]
            public int MatchOrderId{ get; set; }
            /// <summary>
            // 主队半场比分
            ///</summary>
            [ProtoMember(4)]
            [Field("HomeHalf_Result")]
            public string HomeHalf_Result{ get; set; }
            /// <summary>
            // 主队全场比分
            ///</summary>
            [ProtoMember(5)]
            [Field("HomeFull_Result")]
            public string HomeFull_Result{ get; set; }
            /// <summary>
            // 客队半场比分
            ///</summary>
            [ProtoMember(6)]
            [Field("GuestHalf_Result")]
            public string GuestHalf_Result{ get; set; }
            /// <summary>
            // 客队全场比分
            ///</summary>
            [ProtoMember(7)]
            [Field("GuestFull_Result")]
            public string GuestFull_Result{ get; set; }
            /// <summary>
            // 胜平负彩果
            ///</summary>
            [ProtoMember(8)]
            [Field("SPF_Result")]
            public string SPF_Result{ get; set; }
            /// <summary>
            // 胜平负开奖sp
            ///</summary>
            [ProtoMember(9)]
            [Field("SPF_SP")]
            public decimal SPF_SP{ get; set; }
            /// <summary>
            // 总进球彩果
            ///</summary>
            [ProtoMember(10)]
            [Field("ZJQ_Result")]
            public string ZJQ_Result{ get; set; }
            /// <summary>
            // 总进球开奖sp
            ///</summary>
            [ProtoMember(11)]
            [Field("ZJQ_SP")]
            public decimal ZJQ_SP{ get; set; }
            /// <summary>
            // 上下单双彩果
            ///</summary>
            [ProtoMember(12)]
            [Field("SXDS_Result")]
            public string SXDS_Result{ get; set; }
            /// <summary>
            // 上下单双开奖sp
            ///</summary>
            [ProtoMember(13)]
            [Field("SXDS_SP")]
            public decimal SXDS_SP{ get; set; }
            /// <summary>
            // 比分彩果
            ///</summary>
            [ProtoMember(14)]
            [Field("BF_Result")]
            public string BF_Result{ get; set; }
            /// <summary>
            // 比分开奖sp
            ///</summary>
            [ProtoMember(15)]
            [Field("BF_SP")]
            public decimal BF_SP{ get; set; }
            /// <summary>
            // 半全场彩果
            ///</summary>
            [ProtoMember(16)]
            [Field("BQC_Result")]
            public string BQC_Result{ get; set; }
            /// <summary>
            // 半全场开奖sp
            ///</summary>
            [ProtoMember(17)]
            [Field("BQC_SP")]
            public decimal BQC_SP{ get; set; }
            /// <summary>
            // 状态
            ///</summary>
            [ProtoMember(18)]
            [Field("MatchState")]
            public string MatchState{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(19)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}