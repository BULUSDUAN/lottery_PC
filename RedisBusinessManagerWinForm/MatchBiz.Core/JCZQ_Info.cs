using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatchBiz.Core
{
    public class JCZQBase
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

    public class JCZQ_MatchInfo : JCZQBase
    {
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
        public string ShortLeagueName { get; set; }
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
        public string ShortHomeTeamName { get; set; }
        /// <summary>
        /// 客队编号
        /// </summary>
        public int GuestTeamId { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        public string GuestTeamName { get; set; }
        public string ShortGuestTeamName { get; set; }
        /// <summary>
        /// 让球数
        /// </summary>
        public int LetBall { get; set; }
        /// <summary>
        /// 胜 平均赔率
        /// </summary>
        public decimal WinOdds { get; set; }
        /// <summary>
        /// 平 平均赔率
        /// </summary>
        public decimal FlatOdds { get; set; }
        /// <summary>
        /// 负 平均赔率
        /// </summary>
        public decimal LoseOdds { get; set; }
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


        public string Hi { get; set; }
        public string Gi { get; set; }
    }

    public class JCZQ_MatchResultInfo : JCZQBase
    {
        public int HalfHomeTeamScore { get; set; }
        public int HalfGuestTeamScore { get; set; }
        public int FullHomeTeamScore { get; set; }
        public int FullGuestTeamScore { get; set; }
        public string MatchState { get; set; }
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
        public string CreateTime { get; set; }
    }

    public class JCZQ_SPF_SPInfo : JCZQBase
    {
        /// <summary>
        /// 胜 平均赔率
        /// </summary>
        public decimal WinOdds { get; set; }
        /// <summary>
        /// 平 平均赔率
        /// </summary>
        public decimal FlatOdds { get; set; }
        /// <summary>
        /// 负 平均赔率
        /// </summary>
        public decimal LoseOdds { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
    }

    public class JCZQ_BF_SPInfo : JCZQBase
    {
        /// <summary>
        /// 胜-其它
        /// </summary>
        public decimal S_QT { get; set; }
        /// <summary>
        /// 胜-10
        /// </summary>
        public decimal S_10 { get; set; }
        /// <summary>
        /// 胜-20
        /// </summary>
        public decimal S_20 { get; set; }
        /// <summary>
        /// 胜-21
        /// </summary>
        public decimal S_21 { get; set; }
        /// <summary>
        /// 胜-30
        /// </summary>
        public decimal S_30 { get; set; }
        /// <summary>
        /// 胜-31
        /// </summary>
        public decimal S_31 { get; set; }
        /// <summary>
        /// 胜-32
        /// </summary>
        public decimal S_32 { get; set; }
        /// <summary>
        /// 胜-40
        /// </summary>
        public decimal S_40 { get; set; }
        /// <summary>
        /// 胜-41
        /// </summary>
        public decimal S_41 { get; set; }
        /// <summary>
        /// 胜-42
        /// </summary>
        public decimal S_42 { get; set; }
        /// <summary>
        /// 胜-50
        /// </summary>
        public decimal S_50 { get; set; }
        /// <summary>
        /// 胜-51
        /// </summary>
        public decimal S_51 { get; set; }
        /// <summary>
        /// 胜-52
        /// </summary>
        public decimal S_52 { get; set; }
        /// <summary>
        /// 平-其它
        /// </summary>
        public decimal P_QT { get; set; }
        /// <summary>
        /// 平-00
        /// </summary>
        public decimal P_00 { get; set; }
        /// <summary>
        /// 平-11
        /// </summary>
        public decimal P_11 { get; set; }
        /// <summary>
        /// 平-22
        /// </summary>
        public decimal P_22 { get; set; }
        /// <summary>
        /// 平-33
        /// </summary>
        public decimal P_33 { get; set; }
        /// <summary>
        /// 负-其它
        /// </summary>
        public decimal F_QT { get; set; }
        /// <summary>
        /// 负-01
        /// </summary>
        public decimal F_01 { get; set; }
        /// <summary>
        /// 负-02
        /// </summary>
        public decimal F_02 { get; set; }
        /// <summary>
        /// 负-12
        /// </summary>
        public decimal F_12 { get; set; }
        /// <summary>
        /// 负-03
        /// </summary>
        public decimal F_03 { get; set; }
        /// <summary>
        /// 负-13
        /// </summary>
        public decimal F_13 { get; set; }
        /// <summary>
        /// 负-23
        /// </summary>
        public decimal F_23 { get; set; }
        /// <summary>
        /// 负-04
        /// </summary>
        public decimal F_04 { get; set; }
        /// <summary>
        /// 负-14
        /// </summary>
        public decimal F_14 { get; set; }
        /// <summary>
        /// 负-24
        /// </summary>
        public decimal F_24 { get; set; }
        /// <summary>
        /// 负-05
        /// </summary>
        public decimal F_05 { get; set; }
        /// <summary>
        /// 负-15
        /// </summary>
        public decimal F_15 { get; set; }
        /// <summary>
        /// 负-25
        /// </summary>
        public decimal F_25 { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
    }

    public class JCZQ_ZJQ_SPInfo : JCZQBase
    {
        /// <summary>
        /// 进球数 0
        /// </summary>
        public decimal JinQiu_0_Odds { get; set; }
        /// <summary>
        /// 进球数 1
        /// </summary>
        public decimal JinQiu_1_Odds { get; set; }
        /// <summary>
        /// 进球数 2
        /// </summary>
        public decimal JinQiu_2_Odds { get; set; }
        /// <summary>
        /// 进球数 3
        /// </summary>
        public decimal JinQiu_3_Odds { get; set; }
        /// <summary>
        /// 进球数 4
        /// </summary>
        public decimal JinQiu_4_Odds { get; set; }
        /// <summary>
        /// 进球数 5
        /// </summary>
        public decimal JinQiu_5_Odds { get; set; }
        /// <summary>
        /// 进球数 6
        /// </summary>
        public decimal JinQiu_6_Odds { get; set; }
        /// <summary>
        /// 进球数 7
        /// </summary>
        public decimal JinQiu_7_Odds { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
    }

    public class JCZQ_BQC_SPInfo : JCZQBase
    {
        /// <summary>
        /// 胜-胜
        /// </summary>
        public decimal SH_SH_Odds { get; set; }
        /// <summary>
        /// 胜-平
        /// </summary>
        public decimal SH_P_Odds { get; set; }
        /// <summary>
        /// 胜-负
        /// </summary>
        public decimal SH_F_Odds { get; set; }
        /// <summary>
        /// 平-胜
        /// </summary>
        public decimal P_SH_Odds { get; set; }
        /// <summary>
        /// 平-平
        /// </summary>
        public decimal P_P_Odds { get; set; }
        /// <summary>
        /// 平-负
        /// </summary>
        public decimal P_F_Odds { get; set; }
        /// <summary>
        /// 负-胜
        /// </summary>
        public decimal F_SH_Odds { get; set; }
        /// <summary>
        /// 负-平
        /// </summary>
        public decimal F_P_Odds { get; set; }
        /// <summary>
        /// 负-负
        /// </summary>
        public decimal F_F_Odds { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
    }


    public class JCZQ_SPF_SP_Trend
    {
        /// <summary>
        /// 赔率编号
        /// </summary>
        public string OddsMid { get; set; }

        /// <summary>
        /// 胜 平均赔率
        /// </summary>
        public decimal WinOdds { get; set; }
        /// <summary>
        /// 平 平均赔率
        /// </summary>
        public decimal FlatOdds { get; set; }
        /// <summary>
        /// 负 平均赔率
        /// </summary>
        public decimal LoseOdds { get; set; }
        /// <summary>
        /// 初盘为 0 其它为1
        /// </summary>
        public int TP { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
    }

    /// <summary>
    /// 平均欧赔
    /// </summary>
    public class JCZQ_SPF_OZ_SPInfo : JCZQ_SPF_SPInfo
    {
        public string OddsMid { get; set; }
        public string Flag { get; set; }
    }
    /// <summary>
    /// 威廉希尔
    /// </summary>
    public class JCZQ_SPF_WLXE_SPInfo : JCZQ_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// 澳门
    /// </summary>
    public class JCZQ_SPF_AM_SPInfo : JCZQ_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// 立博
    /// </summary>
    public class JCZQ_SPF_LB_SPInfo : JCZQ_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// Bet365
    /// </summary>
    public class JCZQ_SPF_Bet365_SPInfo : JCZQ_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// SNAI
    /// </summary>
    public class JCZQ_SPF_SNAI_SPInfo : JCZQ_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// 易德胜
    /// </summary>
    public class JCZQ_SPF_YDS_SPInfo : JCZQ_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// 韦德
    /// </summary>
    public class JCZQ_SPF_WD_SPInfo : JCZQ_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// Bwin
    /// </summary>
    public class JCZQ_SPF_Bwin_SPInfo : JCZQ_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// Coral
    /// </summary>
    public class JCZQ_SPF_Coral_SPInfo : JCZQ_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// Oddset
    /// </summary>
    public class JCZQ_SPF_Oddset_SPInfo : JCZQ_SPF_OZ_SPInfo
    {
    }

    /// <summary>
    /// 队伍对阵历史
    /// </summary>
    public class JCZQ_Team_History
    {
        public string Ln { get; set; }
        public string HTeam { get; set; }
        public string ATeam { get; set; }
        public string MTime { get; set; }
        public int HScore { get; set; }
        public int AScore { get; set; }
        public string Bc { get; set; }
        public string Bet { get; set; }
        public string BInfo { get; set; }
        public string HTId { get; set; }
        public string ATId { get; set; }
        public string Cl { get; set; }
    }
}
