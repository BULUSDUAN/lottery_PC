using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel.CoreModel
{
    /// <summary>
    /// 数字彩奖池：双色球、大乐透
    /// </summary>
    public class SZC_BonusPoolInfo
    {
        /// <summary>
        /// 彩种
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 奖等
        /// </summary>
        public List<GradeList> GradeList { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }
        public int TotalBonusCount { get; set; }
        public decimal TotalBonusMoney { get; set; }
        public decimal TotalPrizePoolMoney { get; set; }
        public decimal TotalSellMoney { get; set; }
        public string WinNumber { get; set; }
    }

    public class GradeList
    {
        public string Attr { get; set; }
        public int BonusCount { get; set; }
        public decimal BonusMoney { get; set; }
        public string Grade { get; set; }
        public string GradeIndex { get; set; }
        public string GradeName { get; set; }
    }

}
