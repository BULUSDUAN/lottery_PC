using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBiz.Domain.Entities
{
    public class JCLQ_Base
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

    public class JCLQ_Match : JCLQ_Base
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
        /// 联赛颜色
        /// </summary>
        public virtual string LeagueColor { get; set; }
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
        /// 状态
        /// </summary>
        public virtual int MatchState { get; set; }
        /// <summary>
        /// 平均欧指 - 胜
        /// </summary>
        public virtual decimal AverageWin { get; set; }
        /// <summary>
        /// 平均欧指 - 负
        /// </summary>
        public virtual decimal AverageLose { get; set; }
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
    }

    public class JCLQ_MatchResult : JCLQ_Base, Common.Lottery.ISportResult
    {
        /// <summary>
        /// 主队得分
        /// </summary>
        public virtual int HomeScore { get; set; }
        /// <summary>
        /// 客队得分
        /// </summary>
        public virtual int GuestScore { get; set; }
        public virtual string MatchState { get; set; }
        /// <summary>
        /// 胜负结果
        /// </summary>
        public virtual string SF_Result { get; set; }
        /// <summary>
        /// 胜负 SP
        /// </summary>
        public virtual decimal SF_SP { get; set; }
        /// <summary>
        /// 让分胜负结果
        /// </summary>
        public virtual string RFSF_Result { get; set; }
        /// <summary>
        /// 让分胜负 SP
        /// </summary>
        public virtual decimal RFSF_SP { get; set; }
        /// <summary>
        /// 胜分差结果
        /// </summary>
        public virtual string SFC_Result { get; set; }
        /// <summary>
        /// 胜分差 SP
        /// </summary>
        public virtual decimal SFC_SP { get; set; }
        /// <summary>
        /// 大小分结果
        /// </summary>
        public virtual string DXF_Result { get; set; }
        /// <summary>
        /// 大小分 SP
        /// </summary>
        public virtual decimal DXF_SP { get; set; }
        /// <summary>
        /// 让分胜负走势  如 让分主胜|-7.5;让分主胜|-8.5;
        /// </summary>
        public virtual string RFSF_Trend { get; set; }
        /// <summary>
        /// 大小分走势   如 小|152.5;小|151.5;小|150.5;
        /// </summary>
        public virtual string DXF_Trend { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }

        public virtual string GetMatchId(string gameCode)
        {
            return MatchId;
        }
        public virtual string GetFullMatchScore(string gameCode)
        {
            return HomeScore + ":" + GuestScore;
        }
        public virtual string GetMatchResult(string gameCode, string gameType, decimal offset = -1)
        {
            switch (gameType)
            {
                case "SF":
                    return SF_Result;
                case "RFSF":
                    if (offset != -1)
                    {
                        var host1 = (decimal)HomeScore;
                        var guest1 = (decimal)GuestScore;
                        if (host1 + offset > guest1)
                        {
                            return "3";
                        }
                        else if (host1 + offset < guest1)
                        {
                            return "0";
                        }
                    }
                    return RFSF_Result;
                case "SFC":
                    return SFC_Result;
                case "DXF":
                    if (offset != -1)
                    {
                        var host2 = (decimal)HomeScore;
                        var guest2 = (decimal)GuestScore;
                        if (host2 + guest2 > offset)
                        {
                            return "3";
                        }
                        else if (host2 + guest2 < offset)
                        {
                            return "0";
                        }
                    }
                    return DXF_Result;
            }
            return string.Empty;
        }
    }

    public class JCLQ_MatchResult_Prize : JCLQ_Base, Common.Lottery.ISportResult
    {
        /// <summary>
        /// 主队得分
        /// </summary>
        public virtual int HomeScore { get; set; }
        /// <summary>
        /// 客队得分
        /// </summary>
        public virtual int GuestScore { get; set; }
        public virtual string MatchState { get; set; }
        /// <summary>
        /// 胜负结果
        /// </summary>
        public virtual string SF_Result { get; set; }
        /// <summary>
        /// 胜负 SP
        /// </summary>
        public virtual decimal SF_SP { get; set; }
        /// <summary>
        /// 让分胜负结果
        /// </summary>
        public virtual string RFSF_Result { get; set; }
        /// <summary>
        /// 让分胜负 SP
        /// </summary>
        public virtual decimal RFSF_SP { get; set; }
        /// <summary>
        /// 胜分差结果
        /// </summary>
        public virtual string SFC_Result { get; set; }
        /// <summary>
        /// 胜分差 SP
        /// </summary>
        public virtual decimal SFC_SP { get; set; }
        /// <summary>
        /// 大小分结果
        /// </summary>
        public virtual string DXF_Result { get; set; }
        /// <summary>
        /// 大小分 SP
        /// </summary>
        public virtual decimal DXF_SP { get; set; }
        /// <summary>
        /// 让分胜负走势  如 让分主胜|-7.5;让分主胜|-8.5;
        /// </summary>
        public virtual string RFSF_Trend { get; set; }
        /// <summary>
        /// 大小分走势   如 小|152.5;小|151.5;小|150.5;
        /// </summary>
        public virtual string DXF_Trend { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }

        public virtual string GetMatchId(string gameCode)
        {
            return MatchId;
        }
        public virtual string GetFullMatchScore(string gameCode)
        {
            return HomeScore + ":" + GuestScore;
        }
        public virtual string GetMatchResult(string gameCode, string gameType, decimal offset = -1)
        {
            switch (gameType)
            {
                case "SF":
                    return SF_Result;
                case "RFSF":
                    if (offset != -1)
                    {
                        var host1 = (decimal)HomeScore;
                        var guest1 = (decimal)GuestScore;
                        if (host1 + offset > guest1)
                        {
                            return "3";
                        }
                        else if (host1 + offset < guest1)
                        {
                            return "0";
                        }
                    }
                    return RFSF_Result;
                case "SFC":
                    return SFC_Result;
                case "DXF":
                    if (offset != -1)
                    {
                        var host2 = (decimal)HomeScore;
                        var guest2 = (decimal)GuestScore;
                        if (host2 + guest2 > offset)
                        {
                            return "3";
                        }
                        else if (host2 + guest2 < offset)
                        {
                            return "0";
                        }
                    }
                    return DXF_Result;
            }
            return string.Empty;
        }
    }
}
