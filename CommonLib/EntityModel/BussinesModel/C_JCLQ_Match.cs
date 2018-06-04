using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace EntityModel
{
    //// <summary>
    // 
    ////</summary>
    [ProtoContract]
    [Entity("C_JCLQ_Match",Type = EntityType.Table)]
    public class C_JCLQ_Match
    { 
        public C_JCLQ_Match()
        {
        
        }
            //// <summary>
            // 比赛Id
            ////</summary>
            [ProtoMember(1)]
            [Field("MatchId", IsIdenty = false, IsPrimaryKey = true)]
            public string MatchId{ get; set; }
            //// <summary>
            // 比赛编号名称
            ////</summary>
            [ProtoMember(2)]
            [Field("MatchIdName")]
            public string MatchIdName{ get; set; }
            //// <summary>
            // 比赛日期
            ////</summary>
            [ProtoMember(3)]
            [Field("MatchData")]
            public string MatchData{ get; set; }
            //// <summary>
            // 比赛编号
            ////</summary>
            [ProtoMember(4)]
            [Field("MatchNumber")]
            public string MatchNumber{ get; set; }
            //// <summary>
            // 数据中心Id
            ////</summary>
            [ProtoMember(5)]
            [Field("Mid")]
            public int? Mid{ get; set; }
            //// <summary>
            // 联赛Id
            ////</summary>
            [ProtoMember(6)]
            [Field("LeagueId")]
            public int? LeagueId{ get; set; }
            //// <summary>
            // 联赛名称
            ////</summary>
            [ProtoMember(7)]
            [Field("LeagueName")]
            public string LeagueName{ get; set; }
            //// <summary>
            // 联赛颜色
            ////</summary>
            [ProtoMember(8)]
            [Field("LeagueColor")]
            public string LeagueColor{ get; set; }
            //// <summary>
            // 主队编号
            ////</summary>
            [ProtoMember(9)]
            [Field("HomeTeamId")]
            public int? HomeTeamId{ get; set; }
            //// <summary>
            // 主队名称
            ////</summary>
            [ProtoMember(10)]
            [Field("HomeTeamName")]
            public string HomeTeamName{ get; set; }
            //// <summary>
            // 客队编号
            ////</summary>
            [ProtoMember(11)]
            [Field("GuestTeamId")]
            public int? GuestTeamId{ get; set; }
            //// <summary>
            // 客队名称
            ////</summary>
            [ProtoMember(12)]
            [Field("GuestTeamName")]
            public string GuestTeamName{ get; set; }
            //// <summary>
            // 状态
            ////</summary>
            [ProtoMember(13)]
            [Field("MatchState")]
            public int? MatchState{ get; set; }
            //// <summary>
            // 平均欧指 - 胜
            ////</summary>
            [ProtoMember(14)]
            [Field("AverageWin")]
            public decimal? AverageWin{ get; set; }
            //// <summary>
            // 平均欧指 - 负
            ////</summary>
            [ProtoMember(15)]
            [Field("AverageLose")]
            public decimal? AverageLose{ get; set; }
            //// <summary>
            // 比赛开始时间
            ////</summary>
            [ProtoMember(16)]
            [Field("StartDateTime")]
            public DateTime? StartDateTime{ get; set; }
            //// <summary>
            // 单式停止投注时间
            ////</summary>
            [ProtoMember(17)]
            [Field("DSStopBettingTime")]
            public DateTime? DSStopBettingTime{ get; set; }
            //// <summary>
            // 复式停止投注时间
            ////</summary>
            [ProtoMember(18)]
            [Field("FSStopBettingTime")]
            public DateTime? FSStopBettingTime{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(19)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
            //// <summary>
            // 限玩法列表
            ////</summary>
            [ProtoMember(20)]
            [Field("PrivilegesType")]
            public string PrivilegesType{ get; set; }
    }
}