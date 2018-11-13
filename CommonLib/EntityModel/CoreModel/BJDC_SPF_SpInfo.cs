using EntityModel.Enum;
using EntityModel.LotteryJsonInfo;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
 
    
    public class BJDC_SPF_SpInfoCollection : List<BJDC_SPF_SpInfo>
    {
    }

   
    
    public class BJDC_ZJQ_SpInfoCollection : List<BJDC_ZJQ_SpInfo>
    {
    }

   

    
    public class BJDC_SXDS_SpInfoCollection : List<BJDC_SXDS_SpInfo>
    {
    }

  
    
    public class BJDC_BF_SpInfoCollection : List<BJDC_BF_SpInfo>
    {
    }

   
    public class BJDC_BQC_SpInfoCollection : List<BJDC_BQC_SpInfo>
    {
    }

    /// <summary>
    /// 北京单场期号信息
    /// </summary>
    public class BJDC_IssuseInfo
    {
        public string IssuseNumber { get; set; }
        public string MinMatchStartTime { get; set; }
        public string MinLocalStopTime { get; set; }
    }

 
    

    public class BJDC_SPF_SP_Trend
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

        public string issuseNumber { get; set; }
    }


    /// <summary>
    /// 平均欧赔
    /// </summary>
    public class BJDC_SPF_OZ_SPInfo : BJDC_SPF_SpInfo
    {
        public string OddsMid { get; set; }
        public string Flag { get; set; }
    }
    /// <summary>
    /// 威廉希尔
    /// </summary>
    public class BJDC_SPF_WLXE_SPInfo : BJDC_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// 澳门
    /// </summary>
    public class BJDC_SPF_AM_SPInfo : BJDC_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// 立博
    /// </summary>
    public class BJDC_SPF_LB_SPInfo : BJDC_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// Bet365
    /// </summary>
    public class BJDC_SPF_Bet365_SPInfo : BJDC_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// SNAI
    /// </summary>
    public class BJDC_SPF_SNAI_SPInfo : BJDC_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// 易德胜
    /// </summary>
    public class BJDC_SPF_YDS_SPInfo : BJDC_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// 韦德
    /// </summary>
    public class BJDC_SPF_WD_SPInfo : BJDC_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// Bwin
    /// </summary>
    public class BJDC_SPF_Bwin_SPInfo : BJDC_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// Coral
    /// </summary>
    public class BJDC_SPF_Coral_SPInfo : BJDC_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// Oddset
    /// </summary>
    public class BJDC_SPF_Oddset_SPInfo : BJDC_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// 投注比例
    /// </summary>
    public class BJDC_SPF_TZBL_SPInfo : BJDC_SPF_OZ_SPInfo
    {
    }


    /// <summary>
    /// 队伍对阵历史
    /// </summary>
    public class BJDC_Team_History
    {
        //<r ln="亚洲预" hteam="澳大利亚" ateam="阿曼" mtime="1364286600" hscore="2" ascore="2" bc="0:1" bet="1.75" binfo="输" htid="632" atid="933" cl="393465"></r>
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
