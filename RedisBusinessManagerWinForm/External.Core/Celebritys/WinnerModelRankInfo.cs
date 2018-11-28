using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using GameBiz.Core;

namespace External.Core.Celebritys
{
    [CommunicationObject]
    public class WinnerModelRankInfo
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
        /// 模型作者
        /// </summary>
        public string UserDisplayName { get; set; }
        /// <summary>
        /// 被关注人数
        /// </summary>
        public int TotalAttentionCount { get; set; }
        /// <summary>
        /// 已关注人数
        /// </summary>
        public int TotalFollowerCount { get; set; }
        /// <summary>
        /// 模型数量
        /// </summary>
        public int TotalModelCount { get; set; }
        /// <summary>
        /// 被购买人次
        /// </summary>
        public int TotalBuyCount { get; set; }
        /// <summary>
        /// 被购买金额
        /// </summary>
        public decimal TotalBuyMoney { get; set; }
        /// <summary>
        /// 中奖金额
        /// </summary>
        public decimal TotalBonusMoney { get; set; }
        /// <summary>
        /// 最高总回报率
        /// </summary>
        public decimal TotalRateReturn { get; set; }
        /// <summary>
        /// 是否为名家
        /// </summary>
        public bool IsExperter { get; set; }
        /// <summary>
        /// 是否关注
        /// </summary>
        public bool IsAttention { get; set; }

    }
    [CommunicationObject]
    public class WinnerModelRank_Collection
    {
        public WinnerModelRank_Collection()
        {
            ModelRankList = new List<WinnerModelRankInfo>();
        }
        public int TotalCount { get; set; }
        public List<WinnerModelRankInfo> ModelRankList { get; set; }
    }

    [CommunicationObject]
    public class HistoryRecordsInfo
    {

        /// <summary>
        /// 最高总回报率
        /// </summary>
        public decimal TotalRateReturn { get; set; }

    }
    [CommunicationObject]
    public class WinnerModelCenterInfo
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户显示名
        /// </summary>
        public string UserDisplayName { get; set; }
        /// <summary>
        /// 被关注人数
        /// </summary>
        public int BeAttentionUserCount { get; set; }
        /// <summary>
        /// 关注人数
        /// </summary>
        public int FollowerUserCount { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 名家标识
        /// </summary>
        public DealWithType? DealWithType { get; set; }
        /// <summary>
        /// 名家头像地址
        /// </summary>
        public string ImgUrl { get; set; }
        /// <summary>
        /// 拥有模型个数
        /// </summary>
        public int TotalModelCount { get; set; }
        /// <summary>
        /// 累计购买人次
        /// </summary>
        public int TotalBuyCount { get; set; }
        /// <summary>
        /// 累计奖金
        /// </summary>
        public decimal TotalBonusMoney { get; set; }
        /// <summary>
        /// 最早创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 盈利模型个数
        /// </summary>
        public int ProfiteModelCount { get; set; }
        /// <summary>
        /// 最高总回报率
        /// </summary>
        public decimal TotalRateReturn { get; set; }
        /// <summary>
        /// 最高中奖频率
        /// </summary>
        public decimal TotalBonusFrequency { get; set; }
    }
    
}
