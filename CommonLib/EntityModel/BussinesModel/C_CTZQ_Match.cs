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
    [Entity("C_CTZQ_Match",Type = EntityType.Table)]
    public class C_CTZQ_Match
    { 
        public C_CTZQ_Match()
        {
        
        }
            /// <summary>
            //  主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = false, IsPrimaryKey = true)]
            public string Id{ get; set; }
            /// <summary>
            // 编号
            ///</summary>
            [ProtoMember(2)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            /// <summary>
            // 游戏类型
            ///</summary>
            [ProtoMember(3)]
            [Field("GameType")]
            public string GameType{ get; set; }
            /// <summary>
            // 期号
            ///</summary>
            [ProtoMember(4)]
            [Field("IssuseNumber")]
            public string IssuseNumber{ get; set; }
            /// <summary>
            // 序号
            ///</summary>
            [ProtoMember(5)]
            [Field("OrderNumber")]
            public int OrderNumber{ get; set; }
            /// <summary>
            // 比赛开始时间
            ///</summary>
            [ProtoMember(6)]
            [Field("MatchStartTime")]
            public DateTime MatchStartTime{ get; set; }
            /// <summary>
            // 背景色
            ///</summary>
            [ProtoMember(7)]
            [Field("Color")]
            public string Color{ get; set; }
            /// <summary>
            // 数据中心Id
            ///</summary>
            [ProtoMember(8)]
            [Field("Mid")]
            public int Mid{ get; set; }
            /// <summary>
            // 赛事编号
            ///</summary>
            [ProtoMember(9)]
            [Field("MatchId")]
            public int MatchId{ get; set; }
            /// <summary>
            // 赛事名称
            ///</summary>
            [ProtoMember(10)]
            [Field("MatchName")]
            public string MatchName{ get; set; }
            /// <summary>
            // 主队编号
            ///</summary>
            [ProtoMember(11)]
            [Field("HomeTeamId")]
            public string HomeTeamId{ get; set; }
            /// <summary>
            // 主队名称
            ///</summary>
            [ProtoMember(12)]
            [Field("HomeTeamName")]
            public string HomeTeamName{ get; set; }
            /// <summary>
            // 客队编号
            ///</summary>
            [ProtoMember(13)]
            [Field("GuestTeamId")]
            public string GuestTeamId{ get; set; }
            /// <summary>
            // 客队名称
            ///</summary>
            [ProtoMember(14)]
            [Field("GuestTeamName")]
            public string GuestTeamName{ get; set; }
            /// <summary>
            // 赛事状态
            ///</summary>
            [ProtoMember(15)]
            [Field("MatchState")]
            public int MatchState{ get; set; }
            /// <summary>
            // 主队排名
            ///</summary>
            [ProtoMember(16)]
            [Field("HomeTeamStanding")]
            public int HomeTeamStanding{ get; set; }
            /// <summary>
            // 客队排名
            ///</summary>
            [ProtoMember(17)]
            [Field("GuestTeamStanding")]
            public int GuestTeamStanding{ get; set; }
            /// <summary>
            // 主队半场得分
            ///</summary>
            [ProtoMember(18)]
            [Field("HomeTeamHalfScore")]
            public int HomeTeamHalfScore{ get; set; }
            /// <summary>
            // 主队全场得分
            ///</summary>
            [ProtoMember(19)]
            [Field("HomeTeamScore")]
            public int HomeTeamScore{ get; set; }
            /// <summary>
            // 客队半场得分
            ///</summary>
            [ProtoMember(20)]
            [Field("GuestTeamHalfScore")]
            public int GuestTeamHalfScore{ get; set; }
            /// <summary>
            // 客队全场得分
            ///</summary>
            [ProtoMember(21)]
            [Field("GuestTeamScore")]
            public int GuestTeamScore{ get; set; }
            /// <summary>
            // 比赛结果
            ///</summary>
            [ProtoMember(22)]
            [Field("MatchResult")]
            public string MatchResult{ get; set; }
            /// <summary>
            // 更新时间
            ///</summary>
            [ProtoMember(23)]
            [Field("UpdateTime")]
            public DateTime UpdateTime{ get; set; }
    }
}