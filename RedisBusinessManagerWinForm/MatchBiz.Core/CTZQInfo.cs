using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatchBiz.Core
{
    /// <summary>
    /// 传统足球奖期数据
    /// </summary>
    public class CTZQ_IssuseInfo
    {
        public string Id { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string IssuseNumber { get; set; }
        public string StopBettingTime { get; set; }
        public string WinNumber { get; set; }
        public string CreateTime { get; set; }
    }

    /// <summary>
    /// 传统足球队伍信息
    /// </summary>
    public class CTZQ_MatchInfo
    {
        /// <summary>
        /// 编号 = GameCode|GameType|IssuseNumber|OrderNumber
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string GameCode { get; set; }
        public string GameType { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int OrderNumber { get; set; }
        /// <summary>
        /// 比赛开始时间
        /// </summary>
        public string MatchStartTime { get; set; }
        public string Color { get; set; }
        /// <summary>
        /// 数据中心Id
        /// </summary>
        public int Mid { get; set; }
        public int FXId { get; set; }
        /// <summary>
        /// 赛事Id
        /// </summary>
        public int MatchId { get; set; }
        /// <summary>
        /// 赛事名称
        /// </summary>
        public string MatchName { get; set; }
        /// <summary>
        /// 主队编号
        /// </summary>
        public string HomeTeamId { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        public string HomeTeamName { get; set; }
        /// <summary>
        /// 客队编号
        /// </summary>
        public string GuestTeamId { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        public string GuestTeamName { get; set; }
        /// <summary>
        /// 赛事状态
        /// </summary>
        public CTZQMatchState MatchState { get; set; }
        /// <summary>
        /// 主队排名
        /// </summary>
        public string HomeTeamStanding { get; set; }
        /// <summary>
        /// 客队排名
        /// </summary>
        public string GuestTeamStanding { get; set; }
        /// <summary>
        /// 主队半场得分
        /// </summary>
        public int HomeTeamHalfScore { get; set; }
        /// <summary>
        /// 主队全场得分
        /// </summary>
        public int HomeTeamScore { get; set; }
        /// <summary>
        /// 客队半场得分
        /// </summary>
        public int GuestTeamHalfScore { get; set; }
        /// <summary>
        /// 客队全场得分
        /// </summary>
        public int GuestTeamScore { get; set; }
        /// <summary>
        /// 比赛结果
        /// </summary>
        public string MatchResult { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public string UpdateTime { get; set; }
    }

    /// <summary>
    /// 传统足球赔率信息
    /// </summary>
    public class CTZQ_OddInfo
    {
        /// <summary>
        /// 编号 = GameCode|GameType|IssuseNumber|OrderNumber
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 离散指数-胜
        /// </summary>
        public decimal LSWin { get; set; }
        /// <summary>
        /// 离散指数-负
        /// </summary>
        public decimal LSLose { get; set; }
        /// <summary>
        /// 离散指数-平
        /// </summary>
        public decimal LSFlat { get; set; }
        /// <summary>
        /// 凯利指数-胜
        /// </summary>
        public decimal KLWin { get; set; }
        /// <summary>
        /// 凯利指数-负
        /// </summary>
        public decimal KLLose { get; set; }
        /// <summary>
        /// 凯利指数-平
        /// </summary>
        public decimal KLFlat { get; set; }
        /// <summary>
        /// 亚盘水位
        /// </summary>
        public string YPSW { get; set; }
        /// <summary>
        /// 平均赔率 （胜、平、负以 | 分隔）
        /// </summary>
        public string AverageOdds { get; set; }
        /// <summary>
        /// 半场平均赔率 （胜、平、负以 | 分隔）
        /// </summary>
        public string HalfAverageOdds { get; set; }
        /// <summary>
        /// 全场平均赔率 （胜、平、负以 | 分隔）
        /// </summary>
        public string FullAverageOdds { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public string UpdateTime { get; set; }
    }

    /// <summary>
    /// 传统足球奖级信息
    /// </summary>
    public class CTZQ_BonusLevelInfo
    {
        /// <summary>
        /// GameCode|GameType|IssuseNumber|BonusLevel
        /// </summary>
        public string Id { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string IssuseNumber { get; set; }
        public int BonusLevel { get; set; }
        public int BonusCount { get; set; }
        public string BonusLevelDisplayName { get; set; }
        public decimal BonusMoney { get; set; }
        public string MatchResult { get; set; }
        public decimal BonusBalance { get; set; }
        public decimal TotalSaleMoney { get; set; }
        public string CreateTime { get; set; }
    }
}
