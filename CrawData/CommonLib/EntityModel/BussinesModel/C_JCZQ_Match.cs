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
    [Entity("C_JCZQ_Match",Type = EntityType.Table)]
    public class C_JCZQ_Match
    { 
        public C_JCZQ_Match()
        {
        
        }
            /// <summary>
            // 比赛Id
            ///</summary>
            [ProtoMember(1)]
            [Field("MatchId", IsIdenty = false, IsPrimaryKey = true)]
            public string MatchId{ get; set; }
            /// <summary>
            // 比赛编号名称
            ///</summary>
            [ProtoMember(2)]
            [Field("MatchIdName")]
            public string MatchIdName{ get; set; }
            /// <summary>
            // 比赛日期
            ///</summary>
            [ProtoMember(3)]
            [Field("MatchData")]
            public string MatchData{ get; set; }
            /// <summary>
            // 比赛编号
            ///</summary>
            [ProtoMember(4)]
            [Field("MatchNumber")]
            public string MatchNumber{ get; set; }
            /// <summary>
            // 数据中心Id
            ///</summary>
            [ProtoMember(5)]
            [Field("Mid")]
            public int Mid{ get; set; }
            /// <summary>
            // 联赛Id
            ///</summary>
            [ProtoMember(6)]
            [Field("LeagueId")]
            public int LeagueId{ get; set; }
            /// <summary>
            // 联赛名称
            ///</summary>
            [ProtoMember(7)]
            [Field("LeagueName")]
            public string LeagueName{ get; set; }
            /// <summary>
            // 主队编号
            ///</summary>
            [ProtoMember(8)]
            [Field("HomeTeamId")]
            public int HomeTeamId{ get; set; }
            /// <summary>
            // 主队名称
            ///</summary>
            [ProtoMember(9)]
            [Field("HomeTeamName")]
            public string HomeTeamName{ get; set; }
            /// <summary>
            // 客队编号
            ///</summary>
            [ProtoMember(10)]
            [Field("GuestTeamId")]
            public int GuestTeamId{ get; set; }
            /// <summary>
            // 客队名称
            ///</summary>
            [ProtoMember(11)]
            [Field("GuestTeamName")]
            public string GuestTeamName{ get; set; }
            /// <summary>
            // 联赛颜色
            ///</summary>
            [ProtoMember(12)]
            [Field("LeagueColor")]
            public string LeagueColor{ get; set; }
            /// <summary>
            // 让球数
            ///</summary>
            [ProtoMember(13)]
            [Field("LetBall")]
            public int LetBall{ get; set; }
            /// <summary>
            // 胜 平均赔率
            ///</summary>
            [ProtoMember(14)]
            [Field("WinOdds")]
            public decimal WinOdds{ get; set; }
            /// <summary>
            // 平 平均赔率
            ///</summary>
            [ProtoMember(15)]
            [Field("FlatOdds")]
            public decimal FlatOdds{ get; set; }
            /// <summary>
            // 负 平均赔率间
            ///</summary>
            [ProtoMember(16)]
            [Field("LoseOdds")]
            public decimal LoseOdds{ get; set; }
            /// <summary>
            // 比赛开始时间
            ///</summary>
            [ProtoMember(17)]
            [Field("StartDateTime")]
            public DateTime StartDateTime{ get; set; }
            /// <summary>
            // 单式停止投注时间
            ///</summary>
            [ProtoMember(18)]
            [Field("DSStopBettingTime")]
            public DateTime DSStopBettingTime{ get; set; }
            /// <summary>
            // 复式停止投注时间
            ///</summary>
            [ProtoMember(19)]
            [Field("FSStopBettingTime")]
            public DateTime FSStopBettingTime{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(20)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
            /// <summary>
            // 限玩法列表
            ///</summary>
            [ProtoMember(21)]
            [Field("PrivilegesType")]
            public string PrivilegesType{ get; set; }
            /// <summary>
            // 比赛停售说明
            ///</summary>
            [ProtoMember(22)]
            [Field("MatchStopDesc")]
            public string MatchStopDesc{ get; set; }

        /// <summary>
        /// 限玩法列表
        /// </summary>
        // public string PrivilegesType { get; set; }
        public string State { get; set; }
        public int FXId { get; set; }
        public string BRQSPF { get; set; }
        public string SPF { get; set; }
        public string BF { get; set; }
        public string BQC { get; set; }
        public string ZJQ { get; set; }

        public string Hi { get; set; }
        public string Gi { get; set; }

        public string HRank { get; set; }
        public string GRank { get; set; }

        public string HLg { get; set; }
        public string GLg { get; set; }

        //public string MatchStopDesc { get; set; }

        public override bool Equals(object obj)
        {
            var t = obj as C_JCZQ_Match;
            if (t.StartDateTime != this.StartDateTime
                || t.DSStopBettingTime != this.DSStopBettingTime
                || t.FSStopBettingTime != this.FSStopBettingTime
                || t.LeagueColor != this.LeagueColor
                || t.Mid != this.Mid
                || t.State != this.State
                || t.LeagueId != this.LeagueId
                || t.HomeTeamId != this.HomeTeamId
                || t.HomeTeamName != this.HomeTeamName
                || t.GuestTeamId != this.GuestTeamId
                || t.GuestTeamName != this.GuestTeamName
                || t.WinOdds != this.WinOdds
                || t.FlatOdds != this.FlatOdds
                || t.LoseOdds != this.LoseOdds
                || t.MatchId != this.MatchId
                || t.MatchData != this.MatchData
                || t.MatchNumber != this.MatchNumber
                || t.PrivilegesType != this.PrivilegesType
                )
                return false;
            return true;
        }
    }
}