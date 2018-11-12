using EntityModel.Enum;
using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Domain.Entities
{
    /// <summary>
    /// 竞彩足球赛事信息
    /// </summary>
    [Entity("C_JCZQ_Match", Type = EntityType.Table)]
    [BsonIgnoreExtraElements]
    public class C_JCZQ_Match_ZJQ
    {
        [BsonId]

        public ObjectId _id { get; set; }
        /// <summary>
        /// 比赛Id : 120813001
        /// </summary>
        [Field("MatchId")]
        public string MatchId { get; set; }
        /// <summary>
        /// 比赛日期 : 120813
        /// </summary>
        [Field("MatchData")]
        public string MatchData { get; set; }
        /// <summary>
        /// 比赛编号 ：001
        /// </summary>
        [Field("MatchNumber")]
        public string MatchNumber { get; set; }
        [Field("Id", IsPrimaryKey = true)]
        public long Id { get; set; }
        /// <summary>
        /// 比赛编号名称
        /// </summary>
        [Field("MatchIdName")]
        public string MatchIdName { get; set; }
        /// <summary>
        /// 数据中心Id
        /// </summary>
        [Field("Mid")]
        public int Mid { get; set; }
        //[Field("FXId")]
        public int FXId { get; set; }
        /// <summary>
        /// 联赛Id : 140
        /// </summary>
        [Field("LeagueId")]
        public int LeagueId { get; set; }
        public string State { get; set; }
        /// <summary>
        /// 简写联赛名称
        /// </summary>
        [Field("ShortLeagueName")]
        public string ShortLeagueName { get; set; }
        /// <summary>
        /// 联赛名称
        /// </summary>
        [Field("LeagueName")]
        public string LeagueName { get; set; }
        /// <summary>
        /// 主队编号
        /// </summary>
        [Field("HomeTeamId")]
        public int HomeTeamId { get; set; }
        /// <summary>
        /// 简写主队名称
        /// </summary>
        [Field("ShortHomeTeamName")]
        public string ShortHomeTeamName { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        [Field("HomeTeamName")]
        public string HomeTeamName { get; set; }
        /// <summary>
        /// 客队编号
        /// </summary>
        [Field("GuestTeamId")]
        public int GuestTeamId { get; set; }
        /// <summary>
        /// 简写客队名称
        /// </summary>
        [Field("ShortGuestTeamName")]
        public string ShortGuestTeamName { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        [Field("GuestTeamName")]
        public string GuestTeamName { get; set; }
        /// <summary>
        /// 联赛颜色
        /// </summary>
        [Field("LeagueColor")]
        public string LeagueColor { get; set; }
        /// <summary>
        /// 让球数
        /// </summary>
        [Field("LetBall")]
        public int LetBall { get; set; }
        /// <summary>
        /// 胜 平均赔率
        /// </summary>
        [Field("WinOdds")]
        public decimal WinOdds { get; set; }
        /// <summary>
        /// 平 平均赔率
        /// </summary>
        [Field("FlatOdds")]
        public decimal FlatOdds { get; set; }
        /// <summary>
        /// 负 平均赔率
        /// </summary>
        [Field("LoseOdds")]
        public decimal LoseOdds { get; set; }
        /// <summary>
        /// 比赛开始时间
        /// </summary>
        [Field("StartDateTime")]
        public string StartDateTime { get; set; }
        /// <summary>
        /// 单式停止投注时间
        /// </summary>
        [Field("DSStopBettingTime")]
        public string DSStopBettingTime { get; set; }
        /// <summary>
        /// 复式停止投注时间
        /// </summary>
        [Field("FSStopBettingTime")]
        public string FSStopBettingTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Field("CreateTime")]
        public string CreateTime { get; set; }
        /// <summary>
        /// 限玩法列表
        /// </summary>
        public string PrivilegesType { get; set; }

        public string ZJQ { get; set; }

        public string Hi { get; set; }
        public string Gi { get; set; }

        public string HRank { get; set; }
        public string GRank { get; set; }

        public string HLg { get; set; }
        public string GLg { get; set; }

        public string MatchStopDesc { get; set; }

        public override bool Equals(object obj)
        {
            var t = obj as C_JCZQ_Match_ZJQ;
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
