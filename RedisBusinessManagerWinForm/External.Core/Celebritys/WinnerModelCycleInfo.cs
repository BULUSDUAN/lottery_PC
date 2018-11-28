using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace External.Core.Celebritys
{
    [CommunicationObject]
    public class WinnerModelCycleInfo
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public int ModelCycleId { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string SchemeId { get; set; }
        /// <summary>
        /// 模型Id
        /// </summary>
        public string ModelId { get; set; }
        //public WinnerModelInfo WinnerModelInfo { get; set; }
        /// <summary>
        /// 当前期号
        /// </summary>
        public string CurrModelIssuse { get; set; }
        /// <summary>
        /// 当前发起投注金额
        /// </summary>
        public decimal CurrBettingMoney { get; set; }
        /// <summary>
        /// 当前中奖金额
        /// </summary>
        public decimal CurrBonusMoney { get; set; }
        /// <summary>
        /// 当前状态
        /// </summary>
        public ModelProgressStatus ModelProgressStatus { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 当前期是否完成
        /// </summary>
        public bool IsComplete { get; set; }
        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime CompleteTime { get; set; }
        /// <summary>
        /// 当前档案停止投注时间
        /// </summary>
        public DateTime StopBettingTime { get; set; }
    }
    [CommunicationObject]
    public class WinnerModelCycleInfo_Collection
    {
        public WinnerModelCycleInfo_Collection()
        {
            ModelCycleList = new List<WinnerModelCycleInfo>();
            ModelList = new List<WinnerModelInfo>();
        }
        public int TotalCount { get; set; }
        public List<WinnerModelCycleInfo> ModelCycleList { get; set; }
        public List<WinnerModelInfo> ModelList { get; set; }
    }
}
