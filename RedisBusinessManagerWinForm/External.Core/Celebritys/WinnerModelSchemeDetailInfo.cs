using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace External.Core.Celebritys
{
    [CommunicationObject]
    public class WinnerModelSchemeDetailInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int ModelSchemeDetailId { get; set; }
        /// <summary>
        /// 每期方案Id
        /// </summary>
        public int ModelCycleId { get; set; }
        /// <summary>
        /// 追号计划订单编号
        /// </summary>
        public string ModelKeyLine { get; set; }
        /// <summary>
        /// 方案编号
        /// </summary>
        public string SchemeId { get; set; }
        /// <summary>
        /// 当前对应期号
        /// </summary>
        public string CurrIssuseNumber { get; set; }
        /// <summary>
        /// 倍数
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 当前投注金额
        /// </summary>
        public decimal CurrBettingMoney { get; set; }
        /// <summary>
        /// 税后金额
        /// </summary>
        public decimal AfterTaxBonusMoney { get; set; }
        /// <summary>
        /// 支付状态
        /// </summary>
        public PayStatus PayStatus { get; set; }
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsComplete { get; set; }
        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 索引
        /// </summary>
        public int OrderIndex { get; set; }
    }
    [CommunicationObject]
    public class WinnerModelSchemeDetailInfo_Collection
    {
        public WinnerModelSchemeDetailInfo_Collection()
        {
            ModelSchemeDetailList = new List<WinnerModelSchemeDetailInfo>();
        }
        public int TotalCount { get; set; }
        public List<WinnerModelSchemeDetailInfo> ModelSchemeDetailList { get; set; }
    }
}
