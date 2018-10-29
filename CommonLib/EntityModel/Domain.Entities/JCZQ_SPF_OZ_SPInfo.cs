using EntityModel.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Domain.Entities
{
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

    public class JCZQ_2X1SP : JCZQBase
    {
        public string BRQSPF { get; set; }
        public string SPF { get; set; }
    }
    public class JCZQ_Match_SPF_SP : JCZQBase
    {
        public string BF { get; set; }
        public string BQC { get; set; }
        public string ZJQ { get; set; }
    }
    public class JCZQ_SPF_OZ_SPInfo: JCZQ_SPF_SPInfo
    {
        public string OddsMid { get; set; }
        public string Flag { get; set; }
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
    public class JCZQ_SP : JCZQBase
    {
        public string BRQSPF { get; set; }
        public string SPF { get; set; }
        public string BF { get; set; }
        public string BQC { get; set; }
        public string ZJQ { get; set; }
    }
    public class JCZQBase : IBallBaseInfo
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
}
