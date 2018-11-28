using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using Common;

namespace GameBiz.Core
{
    /// <summary>
    /// 合买红人
    /// </summary>
    [CommunicationObject]
    public class TogetherHotUserInfo
    {
        public TogetherHotUserInfo()
        {
            OrderList = new TogetherHotUserOrderInfoCollection();
        }

        /// <summary>
        /// 近期本周中奖
        /// </summary>
        public decimal WeeksWinMoney { get; set; }
        /// <summary>
        /// 关注此专家的人数
        /// </summary>
        public int AttentionUserCount { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 创建者显示姓名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 隐藏用户名数
        /// </summary>
        public int HideDisplayNameCount { get; set; }
        /// <summary>
        /// 最高中奖等级
        /// </summary>
        public string MaxLevelName { get; set; }
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 订单
        /// </summary>
        public TogetherHotUserOrderInfoCollection OrderList { get; set; }
    }
    [CommunicationObject]
    public class TogetherHotUserInfoCollection : List<TogetherHotUserInfo>
    {
    }

    /// <summary>
    /// 合买红人订单
    /// </summary>
    [CommunicationObject]
    public class TogetherHotUserOrderInfo
    {
        /// <summary>
        /// 方案号
        /// </summary>
        public string SchemeId { get; set; }
        public string CreateUserId { get; set; }
        /// <summary>
        /// 方案进度状态
        /// </summary>
        public TogetherSchemeProgress ProgressStatus { get; set; }
        /// <summary>
        /// 方案进度百分比
        /// </summary>
        public decimal Progress { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalMoney { get; set; }
        public DateTime StopTime { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string PlayType { get; set; }
        public DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class TogetherHotUserOrderInfoCollection : List<TogetherHotUserOrderInfo>
    {
    }

}
