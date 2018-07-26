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
    // 足球世界杯比赛-冠军
    ///</summary>
    [ProtoContract]
    [Entity("C_JCZQ_SJBMatch",Type = EntityType.Table)]
    public class C_JCZQ_SJBMatch
    { 
        public C_JCZQ_SJBMatch()
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
            public string MatchId{ get; set; }
            /// <summary>
            // 彩种 SJB
            ///</summary>
            [ProtoMember(3)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            /// <summary>
            // 球队
            ///</summary>
            [ProtoMember(4)]
            [Field("TeamName")]
            public string TeamName{ get; set; }
            /// <summary>
            // 投注状态
            ///</summary>
            [ProtoMember(5)]
            [Field("BetState")]
            public string BetState{ get; set; }
            /// <summary>
            // 玩法类型，冠军 或 冠亚军
            ///</summary>
            [ProtoMember(6)]
            [Field("GameType")]
            public string GameType{ get; set; }
            /// <summary>
            // 期号
            ///</summary>
            [ProtoMember(7)]
            [Field("IssuseNumber")]
            public string IssuseNumber{ get; set; }
            /// <summary>
            // 奖金金额
            ///</summary>
            [ProtoMember(8)]
            [Field("BonusMoney")]
            public decimal BonusMoney{ get; set; }
            /// <summary>
            // 支持率
            ///</summary>
            [ProtoMember(9)]
            [Field("SupportRate")]
            public decimal SupportRate{ get; set; }
            /// <summary>
            // 概率
            ///</summary>
            [ProtoMember(10)]
            [Field("Probadbility")]
            public decimal Probadbility{ get; set; }
            /// <summary>
            // 更新时间
            ///</summary>
            [ProtoMember(11)]
            [Field("UpdateDateTime")]
            public DateTime UpdateDateTime{ get; set; }
    }
}