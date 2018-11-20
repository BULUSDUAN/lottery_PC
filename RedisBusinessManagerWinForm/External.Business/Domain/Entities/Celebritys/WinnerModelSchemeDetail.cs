using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using External.Core;

namespace External.Domain.Entities.Celebritys
{
   public class WinnerModelSchemeDetail
    {
        /// <summary>
        /// 主键
        /// </summary>
        public virtual int ModelSchemeDetailId { get; set; }
        /// <summary>
        /// 每期方案Id
        /// </summary>
        public virtual int ModelCycleId { get; set; }
        /// <summary>
        /// 追号计划订单编号
        /// </summary>
        public virtual string ModelKeyLine { get; set; }
        /// <summary>
        /// 方案编号
        /// </summary>
        public virtual string SchemeId { get; set; }
        /// <summary>
        /// 当前对应期号
        /// </summary>
        public virtual string CurrIssuseNumber { get; set; }
        /// <summary>
        /// 倍数
        /// </summary>
        public virtual int Amount { get; set; }
        /// <summary>
        /// 当前投注金额
        /// </summary>
        public virtual decimal CurrBettingMoney { get; set; }
        /// <summary>
        /// 税后金额
        /// </summary>
        public virtual decimal AfterTaxBonusMoney { get; set; }
        /// <summary>
        /// 支付状态
        /// </summary>
        public virtual PayStatus PayStatus { get; set; }
        /// <summary>
        /// 是否完成
        /// </summary>
        public virtual bool IsComplete { get; set; }
        /// <summary>
        /// 完成时间
        /// </summary>
        public virtual DateTime? CreateTime { get; set; }
        /// <summary>
        /// 索引
        /// </summary>
        public virtual int OrderIndex { get; set; }
    }
}
