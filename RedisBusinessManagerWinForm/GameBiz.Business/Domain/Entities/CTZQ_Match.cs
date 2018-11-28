using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MatchBiz.Core;
using GameBiz.Core;

namespace GameBiz.Domain.Entities
{
    public class CTZQ_GameIssuse
    {
        /// <summary>
        /// GameCode|GameType|IssuseNumber
        /// </summary>
        public virtual string Id { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        public virtual string IssuseNumber { get; set; }
        public virtual DateTime StopBettingTime { get; set; }
        public virtual string WinNumber { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 传统足球14场赛事信息
    /// </summary>
    public class CTZQ_Match
    {
        public virtual string Id { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public virtual string IssuseNumber { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public virtual int OrderNumber { get; set; }
        /// <summary>
        /// 比赛开始时间
        /// </summary>
        public virtual DateTime MatchStartTime { get; set; }
        public virtual string Color { get; set; }
        /// <summary>
        /// 数据中心Id
        /// </summary>
        public virtual int Mid { get; set; }
        /// <summary>
        /// 赛事编号
        /// </summary>
        public virtual int MatchId { get; set; }
        /// <summary>
        /// 赛事名称
        /// </summary>
        public virtual string MatchName { get; set; }
        /// <summary>
        /// 主队编号
        /// </summary>
        public virtual string HomeTeamId { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        public virtual string HomeTeamName { get; set; }
        /// <summary>
        /// 客队编号
        /// </summary>
        public virtual string GuestTeamId { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        public virtual string GuestTeamName { get; set; }
        /// <summary>
        /// 赛事状态
        /// </summary>
        public virtual CTZQMatchState MatchState { get; set; }
        /// <summary>
        /// 主队排名
        /// </summary>
        public virtual string HomeTeamStanding { get; set; }
        /// <summary>
        /// 客队排名
        /// </summary>
        public virtual string GuestTeamStanding { get; set; }
        /// <summary>
        /// 主队半场得分
        /// </summary>
        public virtual int HomeTeamHalfScore { get; set; }
        /// <summary>
        /// 主队全场得分
        /// </summary>
        public virtual int HomeTeamScore { get; set; }
        /// <summary>
        /// 客队半场得分
        /// </summary>
        public virtual int GuestTeamHalfScore { get; set; }
        /// <summary>
        /// 客队全场得分
        /// </summary>
        public virtual int GuestTeamScore { get; set; }
        /// <summary>
        /// 比赛结果
        /// </summary>
        public virtual string MatchResult { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public virtual DateTime UpdateTime { get; set; }
    }

    /// <summary>
    /// 传统足球奖池信息
    /// </summary>
    public class CTZQ_MatchPool
    {
        /// <summary>
        /// GameCode|GameType|IssuseNumber|BonusLevel
        /// </summary>
        public virtual string Id { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        public virtual string IssuseNumber { get; set; }
        public virtual int BonusLevel { get; set; }
        public virtual int BonusCount { get; set; }
        public virtual string BonusLevelDisplayName { get; set; }
        public virtual decimal BonusMoney { get; set; }
        public virtual string MatchResult { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
