using EntityModel.Enum;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel.CoreModel
{
    [ProtoContract]
    public class LocalIssuse_AddInfo
    {
        [ProtoMember(1)]
        public string GameCode { get; set; }
        [ProtoMember(2)]
        public string IssuseNumber { get; set; }
        [ProtoMember(3)]
        public DateTime StartTime { get; set; }
        [ProtoMember(4)]
        public DateTime BettingStopTime { get; set; }
        [ProtoMember(5)]
        public DateTime OfficialStopTime { get; set; }
    }
    [ProtoContract]
    public class LocalIssuse_AddInfoCollection : List<LocalIssuse_AddInfo>
    {
    }

    [ProtoContract]
    public class Issuse_AddInfo
    {
        /// <summary>
        /// 游戏
        /// </summary>
        [ProtoMember(1)]
        public GameInfo Game { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        [ProtoMember(2)]
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 开启时间
        /// </summary>
        [ProtoMember(3)]
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 本地截至时间
        /// </summary>
        [ProtoMember(5)]
        public DateTime LocalStopTime { get; set; }
        /// <summary>
        /// 外部接口截至时间
        /// </summary>
        [ProtoMember(6)]
        public DateTime GatewayStopTime { get; set; }
        /// <summary>
        /// 官方截至时间
        /// </summary>
        [ProtoMember(7)]
        public DateTime OfficialStopTime { get; set; }
        /// <summary>
        /// 奖期状态
        /// </summary>
        [ProtoMember(8)]
        public IssuseStatus Status { get; set; }
    }

    [ProtoContract]
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

    [ProtoContract]
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

    [ProtoContract]
    [Serializable]
    public class Issuse_QueryInfoEX
    {

        /// <summary>
        /// 主键
        /// </summary>
        /// 
        [ProtoMember(1)]
        public IList<Issuse_QueryInfo> CTZQ_IssuseNumber { get; set; }
        /// <summary>
        /// 游戏名称
        /// </summary>
        /// 
        [ProtoMember(2)]      
        public BJDCIssuseInfo BJDC_IssuseNumber { get; set; }
      
       
    }

    [ProtoContract]
    public class Issuse_QueryCollection
    {
        public Issuse_QueryCollection()
        {
            IssuseList = new List<Issuse_QueryInfo>();
        }
        [ProtoMember(1)]
        public int TotalCount { get; set; }
        [ProtoMember(2)]
        public List<Issuse_QueryInfo> IssuseList { get; set; }
    }
    [ProtoContract]
    public class CoreJCZQMatchInfo
    {
        /// <summary>
        /// 比赛Id : 120813001
        /// </summary>
        [ProtoMember(1)]
        public string MatchId { get; set; }
        /// <summary>
        /// 比赛日期 : 120813
        /// </summary>
        [ProtoMember(2)]
        public string MatchData { get; set; }
        /// <summary>
        /// 比赛编号 ：001
        /// </summary>
        [ProtoMember(3)]
        public string MatchNumber { get; set; }
        /// <summary>
        /// 比赛编号名称
        /// </summary>
        [ProtoMember(4)]
        public string MatchIdName { get; set; }
        [ProtoMember(5)]
        public int LeagueId { get; set; }
        /// <summary>
        /// 联赛名称
        /// </summary>
        [ProtoMember(6)]
        public string LeagueName { get; set; }
        /// <summary>
        /// 联赛颜色
        /// </summary>
        [ProtoMember(7)]
        public string LeagueColor { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        [ProtoMember(8)]
        public string HomeTeamName { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        [ProtoMember(9)]
        public string GuestTeamName { get; set; }
        /// <summary>
        /// 比赛开始时间
        /// </summary>
        [ProtoMember(10)]
        public DateTime StartDateTime { get; set; }
        /// <summary>
        /// 复式停止投注时间
        /// </summary>
        [ProtoMember(11)]
        public DateTime FSStopBettingTime { get; set; }
        /// <summary>
        /// 限玩法列表
        /// </summary>
        [ProtoMember(12)]
        public string PrivilegesType { get; set; }
    }

    [ProtoContract]
    public class CoreJCZQMatchInfoCollection : List<CoreJCZQMatchInfo>
    {
    }

    [ProtoContract]
    public class CoreJCLQMatchInfo
    {
        /// <summary>
        /// 比赛Id : 120813001
        /// </summary>
        [ProtoMember(1)]
        public string MatchId { get; set; }
        /// <summary>
        /// 比赛日期 : 120813
        /// </summary>
        [ProtoMember(2)]
        public string MatchData { get; set; }
        /// <summary>
        /// 比赛编号 ：001
        /// </summary>
        [ProtoMember(3)]
        public string MatchNumber { get; set; }
        /// <summary>
        /// 比赛编号名称
        /// </summary>
        [ProtoMember(4)]
        public string MatchIdName { get; set; }
        /// <summary>
        /// 联赛名称
        /// </summary>
        [ProtoMember(5)]
        public int LeagueId { get; set; }
        [ProtoMember(6)]
        public string LeagueName { get; set; }
        /// <summary>
        /// 联赛颜色
        /// </summary>
        [ProtoMember(7)]
        public string LeagueColor { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        [ProtoMember(8)]
        public string HomeTeamName { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        [ProtoMember(9)]
        public string GuestTeamName { get; set; }
        /// <summary>
        /// 比赛开始时间
        /// </summary>
        [ProtoMember(10)]
        public DateTime StartDateTime { get; set; }
        /// <summary>
        /// 复式停止投注时间
        /// </summary>
        [ProtoMember(11)]
        public DateTime FSStopBettingTime { get; set; }
        /// <summary>
        /// 限玩法列表
        /// </summary>
        [ProtoMember(12)]
        public string PrivilegesType { get; set; }
    }

    [ProtoContract]
    public class CoreJCLQMatchInfoCollection : List<CoreJCLQMatchInfo>
    {
    }


    

    [ProtoContract]
    public class CoreBJDCMatchInfo
    {
        /// <summary>
        /// IssuseNumber|MatchOrderId
        /// </summary>
        [ProtoMember(1)]
        public string Id { get; set; }
        /// <summary>
        /// 联赛编排号
        /// </summary>
        [ProtoMember(2)]
        public string MatchOrderId { get; set; }
        /// <summary>
        /// 比赛Id : 120813001
        /// </summary>
        [ProtoMember(3)]
        public string MatchId { get; set; }
        /// <summary>
        /// 比赛日期 : 120813
        /// </summary>
        [ProtoMember(4)]
        public string MatchData { get; set; }
        /// <summary>
        /// 比赛编号 ：001
        /// </summary>
        [ProtoMember(5)]
        public string MatchNumber { get; set; }
        /// <summary>
        /// 比赛编号名称
        /// </summary>
        [ProtoMember(6)]
        public string MatchIdName { get; set; }
        /// <summary>
        /// 联赛名称
        /// </summary>
        [ProtoMember(7)]
        public int LeagueId { get; set; }
        [ProtoMember(8)]
        public string LeagueName { get; set; }
        /// <summary>
        /// 联赛颜色
        /// </summary>
        [ProtoMember(9)]
        public string LeagueColor { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        [ProtoMember(10)]
        public string HomeTeamName { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        [ProtoMember(11)]
        public string GuestTeamName { get; set; }
        /// <summary>
        /// 比赛开始时间
        /// </summary>
        [ProtoMember(12)]
        public DateTime StartDateTime { get; set; }
        /// <summary>
        /// 复式停止投注时间
        /// </summary>
        [ProtoMember(13)]
        public DateTime FSStopBettingTime { get; set; }
        /// <summary>
        /// 限玩法列表
        /// </summary>
        [ProtoMember(14)]
        public string PrivilegesType { get; set; }
    }

    [ProtoContract]
    public class CoreBJDCMatchInfoCollection : List<CoreBJDCMatchInfo>
    {
    }
    [ProtoContract]
    public class HitMatchInfo
    {
        [ProtoMember(1)]
        public string IssuseNumber { get; set; }
        [ProtoMember(2)]
        public int HitMatch_R9 { get; set; }
         [ProtoMember(3)]
        public int HitMatch_14 { get; set; }
    }
}
