using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBiz.Domain.Entities
{
    public class JCZQ_Base
    {
        /// <summary>
        /// 比赛Id : 120813001
        /// </summary>
        public virtual string MatchId { get; set; }
        /// <summary>
        /// 比赛日期 : 120813
        /// </summary>
        public virtual string MatchData { get; set; }
        /// <summary>
        /// 比赛编号 ：001
        /// </summary>
        public virtual string MatchNumber { get; set; }
    }
    public class JCZQ_Match : JCZQ_Base
    {
        /// <summary>
        /// 比赛编号名称
        /// </summary>
        public virtual string MatchIdName { get; set; }
        /// <summary>
        /// 数据中心Id
        /// </summary>
        public virtual int Mid { get; set; }
        /// <summary>
        /// 联赛Id : 140
        /// </summary>
        public virtual int LeagueId { get; set; }
        /// <summary>
        /// 联赛名称
        /// </summary>
        public virtual string LeagueName { get; set; }
        /// <summary>
        /// 主队编号
        /// </summary>
        public virtual int HomeTeamId { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        public virtual string HomeTeamName { get; set; }
        /// <summary>
        /// 客队编号
        /// </summary>
        public virtual int GuestTeamId { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        public virtual string GuestTeamName { get; set; }
        /// <summary>
        /// 联赛颜色
        /// </summary>
        public virtual string LeagueColor { get; set; }
        /// <summary>
        /// 让球数
        /// </summary>
        public virtual int LetBall { get; set; }
        /// <summary>
        /// 胜 平均赔率
        /// </summary>
        public virtual decimal WinOdds { get; set; }
        /// <summary>
        /// 平 平均赔率
        /// </summary>
        public virtual decimal FlatOdds { get; set; }
        /// <summary>
        /// 负 平均赔率
        /// </summary>
        public virtual decimal LoseOdds { get; set; }
        /// <summary>
        /// 比赛开始时间
        /// </summary>
        public virtual DateTime StartDateTime { get; set; }
        /// <summary>
        /// 单式停止投注时间
        /// </summary>
        public virtual DateTime DSStopBettingTime { get; set; }
        /// <summary>
        /// 复式停止投注时间
        /// </summary>
        public virtual DateTime FSStopBettingTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }

        /// <summary>
        /// 限玩法列表
        /// </summary>
        public virtual string PrivilegesType { get; set; }
        /// <summary>
        /// 比赛停售说明
        /// </summary>
        public virtual string MatchStopDesc { get; set; }
    }

    public class JCZQ_MatchResult : JCZQ_Base, Common.Lottery.ISportResult
    {
        public virtual int HalfHomeTeamScore { get; set; }
        public virtual int HalfGuestTeamScore { get; set; }
        public virtual int FullHomeTeamScore { get; set; }
        public virtual int FullGuestTeamScore { get; set; }
        public virtual string MatchState { get; set; }
        public virtual string SPF_Result { get; set; }
        public virtual decimal SPF_SP { get; set; }
        public virtual string BRQSPF_Result { get; set; }
        public virtual decimal BRQSPF_SP { get; set; }
        public virtual string ZJQ_Result { get; set; }
        public virtual decimal ZJQ_SP { get; set; }
        public virtual string BF_Result { get; set; }
        public virtual decimal BF_SP { get; set; }
        public virtual string BQC_Result { get; set; }
        public virtual decimal BQC_SP { get; set; }
        public virtual DateTime CreateTime { get; set; }

        public virtual string GetMatchId(string gameCode)
        {
            return MatchId;
        }
        public virtual string GetFullMatchScore(string gameCode)
        {
            return FullHomeTeamScore + "" + FullGuestTeamScore;
        }
        public virtual string GetMatchResult(string gameCode, string gameType, decimal offset = -1)
        {
            switch (gameType)
            {
                case "SPF":
                    return SPF_Result;
                case "BRQSPF":
                    return BRQSPF_Result;
                case "ZJQ":
                    return ZJQ_Result;
                case "BF":
                    return BF_Result;
                case "BQC":
                    return BQC_Result;
            }
            return string.Empty;
        }
    }

    public class JCZQ_MatchResult_Prize : JCZQ_Base, Common.Lottery.ISportResult
    {
        public virtual int HalfHomeTeamScore { get; set; }
        public virtual int HalfGuestTeamScore { get; set; }
        public virtual int FullHomeTeamScore { get; set; }
        public virtual int FullGuestTeamScore { get; set; }
        public virtual string MatchState { get; set; }
        public virtual string SPF_Result { get; set; }
        public virtual decimal SPF_SP { get; set; }
        public virtual string BRQSPF_Result { get; set; }
        public virtual decimal BRQSPF_SP { get; set; }
        public virtual string ZJQ_Result { get; set; }
        public virtual decimal ZJQ_SP { get; set; }
        public virtual string BF_Result { get; set; }
        public virtual decimal BF_SP { get; set; }
        public virtual string BQC_Result { get; set; }
        public virtual decimal BQC_SP { get; set; }
        public virtual DateTime CreateTime { get; set; }

        public virtual string GetMatchId(string gameCode)
        {
            return MatchId;
        }
        public virtual string GetFullMatchScore(string gameCode)
        {
            return FullHomeTeamScore + "" + FullGuestTeamScore;
        }
        public virtual string GetMatchResult(string gameCode, string gameType, decimal offset = -1)
        {
            switch (gameType)
            {
                case "SPF":
                    return SPF_Result;
                case "BRQSPF":
                    return BRQSPF_Result;
                case "ZJQ":
                    return ZJQ_Result;
                case "BF":
                    return BF_Result;
                case "BQC":
                    return BQC_Result;
            }
            return string.Empty;
        }
    }

    /// <summary>
    /// 足球欧洲杯比赛-冠军
    /// </summary>
    public class JCZQ_OZBMatch
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public virtual string MatchId { get; set; }
        /// <summary>
        /// 彩种 OZB
        /// </summary>
        public virtual string GameCode { get; set; }
        /// <summary>
        /// 球队
        /// </summary>
        public virtual string TeamName { get; set; }
        /// <summary>
        /// 投注状态
        /// </summary>
        public virtual string BetState { get; set; }
        /// <summary>
        /// 玩法类型，冠军 或 冠亚军
        /// </summary>
        public virtual string GameType { get; set; }
        /// <summary>
        /// 奖金金额
        /// </summary>
        public virtual decimal BonusMoney { get; set; }
        /// <summary>
        /// 支持率
        /// </summary>
        public virtual decimal SupportRate { get; set; }
        /// <summary>
        /// 概率
        /// </summary>
        public virtual decimal Probadbility { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public virtual string IssuseNumber { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public virtual DateTime UpdateDateTime { get; set; }
    }


    /// <summary>
    /// 足球世界杯比赛-冠军
    /// </summary>
    public class JCZQ_SJBMatch
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public virtual string MatchId { get; set; }
        /// <summary>
        /// 彩种 OZB
        /// </summary>
        public virtual string GameCode { get; set; }
        /// <summary>
        /// 球队
        /// </summary>
        public virtual string TeamName { get; set; }
        /// <summary>
        /// 投注状态
        /// </summary>
        public virtual string BetState { get; set; }
        /// <summary>
        /// 玩法类型，冠军 或 冠亚军
        /// </summary>
        public virtual string GameType { get; set; }
        /// <summary>
        /// 奖金金额
        /// </summary>
        public virtual decimal BonusMoney { get; set; }
        /// <summary>
        /// 支持率
        /// </summary>
        public virtual decimal SupportRate { get; set; }
        /// <summary>
        /// 概率
        /// </summary>
        public virtual decimal Probadbility { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public virtual string IssuseNumber { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public virtual DateTime UpdateDateTime { get; set; }
    }
}
