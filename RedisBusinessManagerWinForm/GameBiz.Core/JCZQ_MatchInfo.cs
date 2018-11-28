using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core
{
    [CommunicationObject]
    public class JCZQ_MatchInfo
    {
        public string CreateTime { get; set; }
        public string DSStopBettingTime { get; set; }
        public decimal FlatOdds { get; set; }
        public string FSStopBettingTime { get; set; }
        public int FXId { get; set; }
        public string Gi { get; set; }
        public int GuestTeamId { get; set; }
        public string GuestTeamName { get; set; }
        public string Hi { get; set; }
        public int HomeTeamId { get; set; }
        public string HomeTeamName { get; set; }
        public string LeagueColor { get; set; }
        public int LeagueId { get; set; }
        public string LeagueName { get; set; }
        public int LetBall { get; set; }
        public decimal LoseOdds { get; set; }
        public string MatchIdName { get; set; }
        public int Mid { get; set; }
        public string PrivilegesType { get; set; }
        public string ShortGuestTeamName { get; set; }
        public string ShortHomeTeamName { get; set; }
        public string ShortLeagueName { get; set; }
        public string StartDateTime { get; set; }
        public decimal WinOdds { get; set; }
        public string MatchData { get; set; }
        public string MatchId { get; set; }
        public string MatchNumber { get; set; }
        public string State { get; set; }
        public string MatchStopDesc { get; set; }
    }
    [CommunicationObject]
    public class JCZQ_MatchInfo_Collection
    {
        public JCZQ_MatchInfo_Collection()
        {
            MatchList = new List<JCZQ_MatchInfo>();
        }
        public List<JCZQ_MatchInfo> MatchList { get; set; }
    }

    [CommunicationObject]
    public class JCZQ_OZBMatchInfo
    {
        public string MatchId { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 球队
        /// </summary>
        public string Team { get; set; }
        /// <summary>
        /// 投注状态
        /// </summary>
        public string BetState { get; set; }
        /// <summary>
        /// 世界杯类型
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 奖金金额
        /// </summary>
        public decimal BonusMoney { get; set; }
        /// <summary>
        /// 支持率
        /// </summary>
        public decimal SupportRate { get; set; }
        /// <summary>
        /// 概率
        /// </summary>
        public decimal Probadbility { get; set; }
    }

    [CommunicationObject]
    public class JCZQ_OZBMatchInfo_Collection
    {
        public JCZQ_OZBMatchInfo_Collection()
        {
            MatchList = new List<JCZQ_OZBMatchInfo>();
        }
        public List<JCZQ_OZBMatchInfo> MatchList { get; set; }
    }

    [CommunicationObject]
    public class JCZQ_SJBMatchInfo
    {
        public string MatchId { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 球队
        /// </summary>
        public string Team { get; set; }
        /// <summary>
        /// 投注状态
        /// </summary>
        public string BetState { get; set; }
        /// <summary>
        /// 世界杯类型
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 奖金金额
        /// </summary>
        public decimal BonusMoney { get; set; }
        /// <summary>
        /// 支持率
        /// </summary>
        public decimal SupportRate { get; set; }
        /// <summary>
        /// 概率
        /// </summary>
        public decimal Probadbility { get; set; }
    }

    [CommunicationObject]
    public class JCZQ_SJBMatchInfo_Collection
    {
        public JCZQ_SJBMatchInfo_Collection()
        {
            MatchList = new List<JCZQ_SJBMatchInfo>();
        }
        public List<JCZQ_SJBMatchInfo> MatchList { get; set; }
    }

}
