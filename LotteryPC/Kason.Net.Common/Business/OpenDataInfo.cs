using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kason.Net.Common.Business
{
    /// <summary>
    /// 开奖数据
    /// </summary>
    
    public class OpenDataInfo
    {
        public OpenDataInfo()
        {
            GradeList = new OpenGradeInfoCollection();
        }
        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 彩种编码
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 开奖号码
        /// </summary>
        public string WinNumber { get; set; }
        /// <summary>
        /// 奖池总金额
        /// </summary>
        public decimal TotalPrizePoolMoney { get; set; }
        /// <summary>
        /// 总销售金额
        /// </summary>
        public decimal TotalSellMoney { get; set; }
        /// <summary>
        /// 总中奖个数
        /// </summary>
        public int TotalBonusCount { get; set; }
        /// <summary>
        /// 总中奖金额
        /// </summary>
        public decimal TotalBonusMoney { get; set; }
        /// <summary>
        /// 奖等明细
        /// </summary>
        public OpenGradeInfoCollection GradeList { get; set; }
    }

    public class OpenDataInfoCollection
    {
        public OpenDataInfoCollection()
        {
            List = new List<OpenDataInfo>();
        }
        public List<OpenDataInfo> List { get; set; }
    }
    /// <summary>
    /// 开奖奖等
    /// </summary>
 
    public class OpenGradeInfo
    {
        /// <summary>
        /// 奖等索引
        /// </summary>
        public int GradeIndex { get; set; }
        /// <summary>
        /// 奖等
        /// </summary>
        public int Grade { get; set; }
        /// <summary>
        /// 奖等名称
        /// </summary>
        public string GradeName { get; set; }
        /// <summary>
        /// 属性
        /// </summary>
        public string Attr { get; set; }
        /// <summary>
        /// 中奖个数
        /// </summary>
        public int BonusCount { get; set; }
        /// <summary>
        /// 单注奖金
        /// </summary>
        public decimal BonusMoney { get; set; }
    }

  
    public class OpenGradeInfoCollection : List<OpenGradeInfo>
    {

    }
    public class OpenGradeAttr
    {
        /// <summary>
        /// 普通
        /// </summary>
        public const string general = "general";
        /// <summary>
        /// 追加
        /// </summary>
        public const string append = "append";
        /// <summary>
        /// 钻石
        /// </summary>
        public const string diamond = "diamond";
        /// <summary>
        /// 宝石
        /// </summary>
        public const string stone = "stone";

    }



}
