using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core
{
    [CommunicationObject]
    public class LocalIssuse_AddInfo
    {
        public string GameCode { get; set; }
        public string IssuseNumber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime BettingStopTime { get; set; }
        public DateTime OfficialStopTime { get; set; }
    }
    [CommunicationObject]
    public class LocalIssuse_AddInfoCollection : List<LocalIssuse_AddInfo>
    {
    }

    [CommunicationObject]
    public class Issuse_AddInfo
    {
        /// <summary>
        /// 游戏
        /// </summary>
        public GameInfo Game { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 开启时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 本地截至时间
        /// </summary>
        public DateTime LocalStopTime { get; set; }
        /// <summary>
        /// 外部接口截至时间
        /// </summary>
        public DateTime GatewayStopTime { get; set; }
        /// <summary>
        /// 官方截至时间
        /// </summary>
        public DateTime OfficialStopTime { get; set; }
        /// <summary>
        /// 奖期状态
        /// </summary>
        public IssuseStatus Status { get; set; }
    }

    [CommunicationObject]
    public class Issuse_AddCollection : List<Issuse_AddInfo>
    {
    }

    [CommunicationObject]
    public class LotteryIssuse_QueryInfo
    {
        /// <summary>
        /// 彩种编码
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 本地截至投注时间
        /// </summary>
        public DateTime LocalStopTime { get; set; }
        /// <summary>
        /// 官方截至时间
        /// </summary>
        public DateTime OfficialStopTime { get; set; }
        /// <summary>
        /// 提前结束秒数
        /// </summary>
        public int GameDelaySecond { get; set; }
    }

    [CommunicationObject]
    public class LotteryIssuse_QueryInfoCollection : List<LotteryIssuse_QueryInfo>
    {
    }

    [CommunicationObject]
    public class Issuse_QueryInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string GameCode_IssuseNumber { get; set; }
        /// <summary>
        /// 游戏名称
        /// </summary>
        public GameInfo Game { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 开启时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 本地截至投注时间
        /// </summary>
        public DateTime LocalStopTime { get; set; }
        /// <summary>
        /// 外部接口截至投注时间
        /// </summary>
        public DateTime GatewayStopTime { get; set; }
        /// <summary>
        /// 官方截至时间
        /// </summary>
        public DateTime OfficialStopTime { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public IssuseStatus Status { get; set; }
        /// <summary>
        /// 中奖号码
        /// </summary>
        public string WinNumber { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }

    [CommunicationObject]
    public class Issuse_QueryCollection
    {
        public Issuse_QueryCollection()
        {
            IssuseList = new List<Issuse_QueryInfo>();
        }
        public int TotalCount { get; set; }
        public List<Issuse_QueryInfo> IssuseList { get; set; }
    }

    [CommunicationObject]
    public class CoreJCZQMatchInfo
    {
        /// <summary>
        /// 比赛Id : 120813001
        /// </summary>
        public string MatchId { get; set; }
        /// <summary>
        /// 比赛日期 : 120813
        /// </summary>
        public string MatchData { get; set; }
        /// <summary>
        /// 比赛编号 ：001
        /// </summary>
        public string MatchNumber { get; set; }
        /// <summary>
        /// 比赛编号名称
        /// </summary>
        public string MatchIdName { get; set; }
        public int LeagueId { get; set; }
        /// <summary>
        /// 联赛名称
        /// </summary>
        public string LeagueName { get; set; }
        /// <summary>
        /// 联赛颜色
        /// </summary>
        public string LeagueColor { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        public string HomeTeamName { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        public string GuestTeamName { get; set; }
        /// <summary>
        /// 比赛开始时间
        /// </summary>
        public DateTime StartDateTime { get; set; }
        /// <summary>
        /// 复式停止投注时间
        /// </summary>
        public DateTime FSStopBettingTime { get; set; }
        /// <summary>
        /// 限玩法列表
        /// </summary>
        public string PrivilegesType { get; set; }
    }

    [CommunicationObject]
    public class CoreJCZQMatchInfoCollection : List<CoreJCZQMatchInfo>
    {
    }

    [CommunicationObject]
    public class JCZQMatchResult
    {
        public string MatchId { get; set; }
        public string MatchIdName { get; set; }
        public DateTime StartTime { get; set; }
        public int LeagueId { get; set; }
        public string LeagueName { get; set; }
        public string LeagueColor { get; set; }
        public int HomeTeamId { get; set; }
        public string HomeTeamName { get; set; }
        public int GuestTeamId { get; set; }
        public string GuestTeamName { get; set; }
        public int HalfHomeTeamScore { get; set; }
        public int HalfGuestTeamScore { get; set; }
        public int FullHomeTeamScore { get; set; }
        public int FullGuestTeamScore { get; set; }
        public string MatchState { get; set; }
        public int LetBall { get; set; }
        public decimal WinOdds { get; set; }
        public decimal FlatOdds { get; set; }
        public decimal LoseOdds { get; set; }
        public string SPF_Result { get; set; }
        public decimal SPF_SP { get; set; }
        public string BRQSPF_Result { get; set; }
        public decimal BRQSPF_SP { get; set; }
        public string ZJQ_Result { get; set; }
        public decimal ZJQ_SP { get; set; }
        public string BF_Result { get; set; }
        public decimal BF_SP { get; set; }
        public string BQC_Result { get; set; }
        public decimal BQC_SP { get; set; }
        public DateTime CreateTime { get; set; }
    }

    [CommunicationObject]
    public class JCZQMatchResult_Collection
    {
        public JCZQMatchResult_Collection()
        {
            MatchResultList = new List<JCZQMatchResult>();
        }
        public int TotalCount { get; set; }
        public List<JCZQMatchResult> MatchResultList { get; set; }
    }


    [CommunicationObject]
    public class CoreJCLQMatchInfo
    {
        /// <summary>
        /// 比赛Id : 120813001
        /// </summary>
        public string MatchId { get; set; }
        /// <summary>
        /// 比赛日期 : 120813
        /// </summary>
        public string MatchData { get; set; }
        /// <summary>
        /// 比赛编号 ：001
        /// </summary>
        public string MatchNumber { get; set; }
        /// <summary>
        /// 比赛编号名称
        /// </summary>
        public string MatchIdName { get; set; }
        /// <summary>
        /// 联赛名称
        /// </summary>
        public int LeagueId { get; set; }
        public string LeagueName { get; set; }
        /// <summary>
        /// 联赛颜色
        /// </summary>
        public string LeagueColor { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        public string HomeTeamName { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        public string GuestTeamName { get; set; }
        /// <summary>
        /// 比赛开始时间
        /// </summary>
        public DateTime StartDateTime { get; set; }
        /// <summary>
        /// 复式停止投注时间
        /// </summary>
        public DateTime FSStopBettingTime { get; set; }
        /// <summary>
        /// 限玩法列表
        /// </summary>
        public string PrivilegesType { get; set; }
    }

    [CommunicationObject]
    public class CoreJCLQMatchInfoCollection : List<CoreJCLQMatchInfo>
    {
    }

    [CommunicationObject]
    public class JCLQMatchResult
    {
        public string MatchId { get; set; }
        public string MatchIdName { get; set; }
        public DateTime StartTime { get; set; }
        public int LeagueId { get; set; }
        public string LeagueName { get; set; }
        public string LeagueColor { get; set; }
        public int HomeTeamId { get; set; }
        public string HomeTeamName { get; set; }
        public int GuestTeamId { get; set; }
        public string GuestTeamName { get; set; }
        public int HomeTeamScore { get; set; }
        public int GuestTeamScore { get; set; }
        public string SF_Result { get; set; }
        public decimal SF_SP { get; set; }
        public string RFSF_Result { get; set; }
        public decimal RFSF_SP { get; set; }
        public string SFC_Result { get; set; }
        public decimal SFC_SP { get; set; }
        public string DXF_Result { get; set; }
        public decimal DXF_SP { get; set; }
        public string MatchState { get; set; }
        public string RFSF_Trend { get; set; }
        public string DXF_Trend { get; set; }
        public DateTime CreateTime { get; set; }
    }

    [CommunicationObject]
    public class JCLQMatchResult_Collection
    {
        public JCLQMatchResult_Collection()
        {
            MatchResultList = new List<JCLQMatchResult>();
        }
        public int TotalCount { get; set; }
        public List<JCLQMatchResult> MatchResultList { get; set; }
    }

    [CommunicationObject]
    public class CoreBJDCMatchInfo
    {
        /// <summary>
        /// IssuseNumber|MatchOrderId
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 联赛编排号
        /// </summary>
        public string MatchOrderId { get; set; }
        /// <summary>
        /// 比赛Id : 120813001
        /// </summary>
        public string MatchId { get; set; }
        /// <summary>
        /// 比赛日期 : 120813
        /// </summary>
        public string MatchData { get; set; }
        /// <summary>
        /// 比赛编号 ：001
        /// </summary>
        public string MatchNumber { get; set; }
        /// <summary>
        /// 比赛编号名称
        /// </summary>
        public string MatchIdName { get; set; }
        /// <summary>
        /// 联赛名称
        /// </summary>
        public int LeagueId { get; set; }
        public string LeagueName { get; set; }
        /// <summary>
        /// 联赛颜色
        /// </summary>
        public string LeagueColor { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        public string HomeTeamName { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        public string GuestTeamName { get; set; }
        /// <summary>
        /// 比赛开始时间
        /// </summary>
        public DateTime StartDateTime { get; set; }
        /// <summary>
        /// 复式停止投注时间
        /// </summary>
        public DateTime FSStopBettingTime { get; set; }
        /// <summary>
        /// 限玩法列表
        /// </summary>
        public string PrivilegesType { get; set; }
    }

    [CommunicationObject]
    public class CoreBJDCMatchInfoCollection : List<CoreBJDCMatchInfo>
    {
    }
    [CommunicationObject]
    public class HitMatchInfo
    {
        public string IssuseNumber { get; set; }
        public int HitMatch_R9 { get; set; }
        public int HitMatch_14 { get; set; }
    }
}
