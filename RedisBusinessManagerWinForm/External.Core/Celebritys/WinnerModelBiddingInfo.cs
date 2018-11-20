using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace External.Core.Celebritys
{
    [CommunicationObject]
    public class WinnerModelBiddingInfo
    {
        /// <summary>
        /// 模型编号
        /// </summary>
        public string ModelId { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 被点击次数
        /// </summary>
        public int ClickNumber { get; set; }
        /// <summary>
        /// 已调整出价
        /// </summary>
        public decimal BidDouDou { get; set; }
        /// <summary>
        /// 当前出价
        /// </summary>
        public decimal CurrBidDouDou { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 调整竞价时间
        /// </summary>
        public DateTime ModifyTime { get; set; }
        /// <summary>
        /// 模型名称
        /// </summary>
        public string ModelName { get; set; }
        /// <summary>
        /// 用户显示名
        /// </summary>
        public string UserDisplayName { get; set; }
        /// <summary>
        /// 是否系统推荐
        /// </summary>
        public bool IsSystemRecom { get; set; }
    }
    [CommunicationObject]
    public class WinnerModelBidding_Collection
    {
        public WinnerModelBidding_Collection()
        {
            ModelBidList = new List<WinnerModelBiddingInfo>();
        }
        public int TotalCount { get; set; }
        public List<WinnerModelBiddingInfo> ModelBidList { get; set; }
    }
}
