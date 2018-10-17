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
    [Entity("C_BJDC_Match",Type = EntityType.Table)]
    public class C_BJDC_Match
    { 
        public C_BJDC_Match()
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
            // 数据中心Id
            ///</summary>
            [ProtoMember(4)]
            [Field("Mid")]
            public int Mid{ get; set; }
            /// <summary>
            // 赛事编号
            ///</summary>
            [ProtoMember(5)]
            [Field("MatchId")]
            public int MatchId{ get; set; }
            /// <summary>
            // 联赛名字
            ///</summary>
            [ProtoMember(6)]
            [Field("MatchName")]
            public string MatchName{ get; set; }
            /// <summary>
            // 比赛开始时间
            ///</summary>
            [ProtoMember(7)]
            [Field("MatchStartTime")]
            public DateTime MatchStartTime{ get; set; }
            /// <summary>
            // 本地结束时间
            ///</summary>
            [ProtoMember(8)]
            [Field("LocalStopTime")]
            public DateTime LocalStopTime{ get; set; }
            /// <summary>
            // 赛事状态
            ///</summary>
            [ProtoMember(9)]
            [Field("MatchState")]
            public int MatchState{ get; set; }
            /// <summary>
            // 联赛背景色
            ///</summary>
            [ProtoMember(10)]
            [Field("MatchColor")]
            public string MatchColor{ get; set; }
            /// <summary>
            // 主队编号
            ///</summary>
            [ProtoMember(11)]
            [Field("HomeTeamId")]
            public int HomeTeamId{ get; set; }
            /// <summary>
            // 主队排名 有可能是字符串
            ///</summary>
            [ProtoMember(12)]
            [Field("HomeTeamSort")]
            public string HomeTeamSort{ get; set; }
            /// <summary>
            // 主队名称
            ///</summary>
            [ProtoMember(13)]
            [Field("HomeTeamName")]
            public string HomeTeamName{ get; set; }
            /// <summary>
            // 客队编号
            ///</summary>
            [ProtoMember(14)]
            [Field("GuestTeamId")]
            public int GuestTeamId{ get; set; }
            /// <summary>
            // 客队名称
            ///</summary>
            [ProtoMember(15)]
            [Field("GuestTeamName")]
            public string GuestTeamName{ get; set; }
            /// <summary>
            // 客队排名 有可能是字符串
            ///</summary>
            [ProtoMember(16)]
            [Field("GuestTeamSort")]
            public string GuestTeamSort{ get; set; }
            /// <summary>
            // 让球数
            ///</summary>
            [ProtoMember(17)]
            [Field("LetBall")]
            public int LetBall{ get; set; }
            /// <summary>
            // 胜 平均赔率
            ///</summary>
            [ProtoMember(18)]
            [Field("WinOdds")]
            public decimal WinOdds{ get; set; }
            /// <summary>
            // 平 平均赔率
            ///</summary>
            [ProtoMember(19)]
            [Field("FlatOdds")]
            public decimal FlatOdds{ get; set; }
            /// <summary>
            // 负 平均赔率
            ///</summary>
            [ProtoMember(20)]
            [Field("LoseOdds")]
            public decimal LoseOdds{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(21)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
            /// <summary>
            // 限制玩法列表
            ///</summary>
            [ProtoMember(22)]
            [Field("PrivilegesType")]
            public string PrivilegesType{ get; set; }
    }
}