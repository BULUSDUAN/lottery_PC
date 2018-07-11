using EntityModel.Enum;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel.CoreModel
{
    public class LocalIssuse_AddInfo
    {
        public string GameCode { get; set; }
        public string IssuseNumber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime BettingStopTime { get; set; }
        public DateTime OfficialStopTime { get; set; }
    }

    public class LocalIssuse_AddInfoCollection : List<LocalIssuse_AddInfo>
    {
    }


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


    public class Issuse_AddCollection : List<Issuse_AddInfo>
    {
    }

    [ProtoContract]
    [Serializable]
    public class LotteryIssuse_QueryInfo
    {
        /// <summary>
        /// 彩种编码
        /// </summary>
        /// 
        [ProtoMember(1)]
        public string GameCode { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        /// 
        [ProtoMember(2)]
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 本地截至投注时间
        /// </summary>
        /// 
        [ProtoMember(3)]
        public DateTime LocalStopTime { get; set; }
        /// <summary>
        /// 官方截至时间
        /// </summary>
        /// 
        [ProtoMember(4)]
        public DateTime OfficialStopTime { get; set; }
        /// <summary>
        /// 提前结束秒数
        /// </summary>
        /// 
        [ProtoMember(5)]
        public int GameDelaySecond { get; set; }
    }


    public class LotteryIssuse_QueryInfoCollection : List<LotteryIssuse_QueryInfo>
    {
    }

    [ProtoContract]
    [Serializable]
    public class Issuse_QueryInfo
    {

        /// <summary>
        /// 主键
        /// </summary>
        /// 
        [ProtoMember(1)]
        public string GameCode_IssuseNumber { get; set; }
        /// <summary>
        /// 游戏名称
        /// </summary>
        /// 
        [ProtoMember(2)]
        public GameInfo Game { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        /// 
        [ProtoMember(3)]
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 开启时间
        /// </summary>
        /// 
        [ProtoMember(4)]
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 本地截至投注时间
        /// </summary>
        /// 
        [ProtoMember(5)]
        public DateTime LocalStopTime { get; set; }
        /// <summary>
        /// 外部接口截至投注时间
        /// </summary>
        /// 
        [ProtoMember(6)]
        public DateTime GatewayStopTime { get; set; }
        /// <summary>
        /// 官方截至时间
        /// </summary>
        /// 
        [ProtoMember(7)]
        public DateTime OfficialStopTime { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        /// 
        [ProtoMember(8)]
        public IssuseStatus Status { get; set; }
        /// <summary>
        /// 中奖号码
        /// </summary>
        /// 
        [ProtoMember(9)]
        public string WinNumber { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        /// 
        [ProtoMember(10)]
        public DateTime CreateTime { get; set; }
    }


    public class Issuse_QueryCollection
    {
        public Issuse_QueryCollection()
        {
            IssuseList = new List<Issuse_QueryInfo>();
        }
        public int TotalCount { get; set; }
        public List<Issuse_QueryInfo> IssuseList { get; set; }
    }

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


    public class CoreJCZQMatchInfoCollection : List<CoreJCZQMatchInfo>
    {
    }


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


    public class CoreJCLQMatchInfoCollection : List<CoreJCLQMatchInfo>
    {
    }


    


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


    public class CoreBJDCMatchInfoCollection : List<CoreBJDCMatchInfo>
    {
    }

    public class HitMatchInfo
    {
        public string IssuseNumber { get; set; }
        public int HitMatch_R9 { get; set; }
        public int HitMatch_14 { get; set; }
    }
}
