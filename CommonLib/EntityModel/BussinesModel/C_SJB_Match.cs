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
    // 传统足球14场赛事信息
    ///</summary>
    [ProtoContract]
    [Entity("C_SJB_Match",Type = EntityType.Table)]
    public class C_SJB_Match
    { 
        public C_SJB_Match()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 比赛编号
            ///</summary>
            [ProtoMember(2)]
            [Field("MatchId")]
            public int MatchId{ get; set; }
            /// <summary>
            // 期号
            ///</summary>
            [ProtoMember(3)]
            [Field("IssuseNumber")]
            public string IssuseNumber{ get; set; }
            /// <summary>
            // 球队
            ///</summary>
            [ProtoMember(4)]
            [Field("Team")]
            public string Team{ get; set; }
            /// <summary>
            // 世界杯类型
            ///</summary>
            [ProtoMember(5)]
            [Field("GameType")]
            public string GameType{ get; set; }
            /// <summary>
            // 状态
            ///</summary>
            [ProtoMember(6)]
            [Field("BetState")]
            public string BetState{ get; set; }
            /// <summary>
            // 奖金
            ///</summary>
            [ProtoMember(7)]
            [Field("BonusMoney")]
            public decimal BonusMoney{ get; set; }
            /// <summary>
            // 支持率
            ///</summary>
            [ProtoMember(8)]
            [Field("SupportRate")]
            public decimal SupportRate{ get; set; }
            /// <summary>
            // 概率
            ///</summary>
            [ProtoMember(9)]
            [Field("Probadbility")]
            public decimal Probadbility{ get; set; }
    }
}