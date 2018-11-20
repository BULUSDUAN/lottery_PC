using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using External.Core;

namespace External.Domain.Entities.Celebritys
{
   public class WinnerModelCycle
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public virtual int ModelCycleId { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public virtual string SchemeId { get; set; }
        /// <summary>
        /// 模型Id
        /// </summary>
        public virtual string ModelId { get; set; }
        //public virtual WinnerModel WinnerModel { get; set; }
        /// <summary>
        /// 当前期号
        /// </summary>
        public virtual string CurrModelIssuse { get; set; }
        /// <summary>
        /// 当前发起投注金额
        /// </summary>
        public virtual decimal CurrBettingMoney { get; set; }
        /// <summary>
        /// 当前中奖金额
        /// </summary>
        public virtual decimal CurrBonusMoney { get; set; }
        /// <summary>
        /// 当前状态
        /// </summary>
        public virtual ModelProgressStatus ModelProgressStatus { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime? CreateTime { get; set; }
        /// <summary>
        /// 当前期是否完成
        /// </summary>
        public virtual bool IsComplete { get; set; }
        /// <summary>
        /// 完成时间
        /// </summary>
        public virtual DateTime? CompleteTime { get; set; }
        /// <summary>
        /// 停止时间
        /// </summary>
        public virtual DateTime? StopBettingTime { get; set; }
    }
}
