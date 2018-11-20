using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatchBiz.Core
{
    public class JCLQBase
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
    }

    public class JCLQ_MatchInfo : JCLQBase
    {
        /// <summary>
        /// 比赛编号名称
        /// </summary>
        public string MatchIdName { get; set; }
        /// <summary>
        /// 数据中心Id
        /// </summary>
        public int Mid { get; set; }
        public int FXId { get; set; }
        /// <summary>
        /// 联赛Id : 140
        /// </summary>
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
        /// 主队编号
        /// </summary>
        public int HomeTeamId { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        public string HomeTeamName { get; set; }
        /// <summary>
        /// 客队编号
        /// </summary>
        public int GuestTeamId { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        public string GuestTeamName { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int MatchState { get; set; }
        /// <summary>
        /// 平均欧指 - 胜
        /// </summary>
        public decimal AverageWin { get; set; }
        /// <summary>
        /// 平均欧指 - 负
        /// </summary>
        public decimal AverageLose { get; set; }
        /// <summary>
        /// 比赛开始时间
        /// </summary>
        public string StartDateTime { get; set; }
        /// <summary>
        /// 单式停止投注时间
        /// </summary>
        public string DSStopBettingTime { get; set; }
        /// <summary>
        /// 复式停止投注时间
        /// </summary>
        public string FSStopBettingTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }

        /// <summary>
        /// 限玩法列表
        /// </summary>
        public string PrivilegesType { get; set; }
    }

    public class JCLQ_MatchResultInfo : JCLQBase
    {
        /// <summary>
        /// 主队得分
        /// </summary>
        public int HomeScore { get; set; }
        /// <summary>
        /// 客队得分
        /// </summary>
        public int GuestScore { get; set; }
        public string MatchState { get; set; }
        /// <summary>
        /// 胜负结果
        /// </summary>
        public string SF_Result { get; set; }
        /// <summary>
        /// 胜负 SP
        /// </summary>
        public decimal SF_SP { get; set; }
        /// <summary>
        /// 让分胜负结果
        /// </summary>
        public string RFSF_Result { get; set; }
        /// <summary>
        /// 让分胜负 SP
        /// </summary>
        public decimal RFSF_SP { get; set; }
        /// <summary>
        /// 胜分差结果
        /// </summary>
        public string SFC_Result { get; set; }
        /// <summary>
        /// 胜分差 SP
        /// </summary>
        public decimal SFC_SP { get; set; }
        /// <summary>
        /// 大小分结果
        /// </summary>
        public string DXF_Result { get; set; }
        /// <summary>
        /// 大小分 SP
        /// </summary>
        public decimal DXF_SP { get; set; }
        /// <summary>
        /// 让分胜负走势  如 让分主胜|-7.5;让分主胜|-8.5;
        /// </summary>
        public string RFSF_Trend { get; set; }
        /// <summary>
        /// 大小分走势   如 小|152.5;小|151.5;小|150.5;
        /// </summary>
        public string DXF_Trend { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
    }

    public class JCLQ_SF_SPInfo : JCLQBase
    {
        public decimal WinSP { get; set; }
        public decimal LoseSP { get; set; }
        public string CreateTime { get; set; }
    }

    public class JCLQ_RFSF_SPInfo : JCLQBase
    {
        public decimal WinSP { get; set; }
        public decimal LoseSP { get; set; }
        public decimal RF { get; set; }
        public string CreateTime { get; set; }
    }

    public class JCLQ_SFC_SPInfo : JCLQBase
    {
        public decimal HomeWin1_5 { get; set; }
        public decimal HomeWin6_10 { get; set; }
        public decimal HomeWin11_15 { get; set; }
        public decimal HomeWin16_20 { get; set; }
        public decimal HomeWin21_25 { get; set; }
        public decimal HomeWin26 { get; set; }

        public decimal GuestWin1_5 { get; set; }
        public decimal GuestWin6_10 { get; set; }
        public decimal GuestWin11_15 { get; set; }
        public decimal GuestWin16_20 { get; set; }
        public decimal GuestWin21_25 { get; set; }
        public decimal GuestWin26 { get; set; }
        public string CreateTime { get; set; }
    }

    public class JCLQ_DXF_SPInfo : JCLQBase
    {
        /// <summary>
        /// 预设总分
        /// </summary>
        public decimal YSZF { get; set; }
        /// <summary>
        /// 大分
        /// </summary>
        public decimal DF { get; set; }
        /// <summary>
        /// 小分
        /// </summary>
        public decimal XF { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
    }
}
