using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.BonusPool
{
    public class OpenDataInfo
    {
        public OpenDataInfo()
        {
            GradeList = new List<OpenGradeInfo>();
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
        public IList<OpenGradeInfo> GradeList { get; set; }
    }
}
