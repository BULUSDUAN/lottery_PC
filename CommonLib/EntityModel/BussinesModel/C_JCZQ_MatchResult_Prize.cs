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
    [Entity("C_JCZQ_MatchResult_Prize",Type = EntityType.Table)]
    public class C_JCZQ_MatchResult_Prize
    { 
        public C_JCZQ_MatchResult_Prize()
        {
        
        }
            /// <summary>
            // 比赛Id
            ///</summary>
            [ProtoMember(1)]
            [Field("MatchId", IsIdenty = false, IsPrimaryKey = true)]
            public string MatchId{ get; set; }
            /// <summary>
            // 比赛日期
            ///</summary>
            [ProtoMember(2)]
            [Field("MatchData")]
            public string MatchData{ get; set; }
            /// <summary>
            // 比赛编号
            ///</summary>
            [ProtoMember(3)]
            [Field("MatchNumber")]
            public string MatchNumber{ get; set; }
            /// <summary>
            // 主队半场得分
            ///</summary>
            [ProtoMember(4)]
            [Field("HalfHomeTeamScore")]
            public int HalfHomeTeamScore{ get; set; }
            /// <summary>
            // 客队半场得分
            ///</summary>
            [ProtoMember(5)]
            [Field("HalfGuestTeamScore")]
            public int HalfGuestTeamScore{ get; set; }
            /// <summary>
            // 主队全场得分
            ///</summary>
            [ProtoMember(6)]
            [Field("FullHomeTeamScore")]
            public int FullHomeTeamScore{ get; set; }
            /// <summary>
            // 客队全场得分
            ///</summary>
            [ProtoMember(7)]
            [Field("FullGuestTeamScore")]
            public int FullGuestTeamScore{ get; set; }
            /// <summary>
            // 比赛状态
            ///</summary>
            [ProtoMember(8)]
            [Field("MatchState")]
            public string MatchState{ get; set; }
            /// <summary>
            // 胜平负彩果
            ///</summary>
            [ProtoMember(9)]
            [Field("SPF_Result")]
            public string SPF_Result{ get; set; }
            /// <summary>
            // 胜平负开奖sp
            ///</summary>
            [ProtoMember(10)]
            [Field("SPF_SP")]
            public decimal SPF_SP{ get; set; }
            /// <summary>
            // 不让球胜平负彩果
            ///</summary>
            [ProtoMember(11)]
            [Field("BRQSPF_Result")]
            public string BRQSPF_Result{ get; set; }
            /// <summary>
            // 不让球胜平负 SP
            ///</summary>
            [ProtoMember(12)]
            [Field("BRQSPF_SP")]
            public decimal BRQSPF_SP{ get; set; }
            /// <summary>
            // 总进球彩果
            ///</summary>
            [ProtoMember(13)]
            [Field("ZJQ_Result")]
            public string ZJQ_Result{ get; set; }
            /// <summary>
            // 总进球开奖 SP
            ///</summary>
            [ProtoMember(14)]
            [Field("ZJQ_SP")]
            public decimal ZJQ_SP{ get; set; }
            /// <summary>
            // 比分彩果
            ///</summary>
            [ProtoMember(15)]
            [Field("BF_Result")]
            public string BF_Result{ get; set; }
            /// <summary>
            // 比分开奖sp
            ///</summary>
            [ProtoMember(16)]
            [Field("BF_SP")]
            public decimal BF_SP{ get; set; }
            /// <summary>
            // 半全场彩果
            ///</summary>
            [ProtoMember(17)]
            [Field("BQC_Result")]
            public string BQC_Result{ get; set; }
            /// <summary>
            // 半全场开奖sp
            ///</summary>
            [ProtoMember(18)]
            [Field("BQC_SP")]
            public decimal BQC_SP{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(19)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}