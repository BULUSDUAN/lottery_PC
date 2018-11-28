using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace External.Core.Celebritys
{
    [CommunicationObject]
    public class WinnerModelSchemeInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int ModelSchemeId { get; set; }
        /// <summary>
        /// 模型Id
        /// </summary>
        public string ModelId { get; set; }
        /// <summary>
        /// 追号计划订单号
        /// </summary>
        public string ModelKeyLine { get; set; }
        /// <summary>
        /// 模型名称
        /// </summary>
        public string ModelName { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 追号计划状态
        /// </summary>
        public SchemeProgressStatus SchemeProgressStatus { get; set; }
        /// <summary>
        /// 追号期数
        /// </summary>
        public int TotalChaseIssuseCount { get; set; }
        /// <summary>
        /// 投注总额
        /// </summary>
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 累计奖金
        /// </summary>
        public decimal TotalBonusMoney { get; set; }
        /// <summary>
        /// 盈利后停止
        /// </summary>
        public bool IsProfitedStop { get; set; }
        /// <summary>
        /// 已完成期数
        /// </summary>
        public int CompleteIssuseCount { get; set; }
        /// <summary>
        /// 已完成期数总金额
        /// </summary>
        public decimal CompleteIssuseMoney { get; set; }
        /// <summary>
        /// 是否停止
        /// </summary>
        public bool IsStop { get; set; }
        /// <summary>
        /// 停止时间
        /// </summary>
        public DateTime StopTime { get; set; }
        /// <summary>
        /// 追号计划停止描述[已停止(用户停止);已停止(模型删除停止)]
        /// </summary>
        public string StopDesc { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 盈利达到百分比后停止
        /// </summary>
        public decimal ProfiteRatio { get; set; }
        /// <summary>
        /// 投注类型
        /// </summary>
        public BettingType BettingType { get; set; }
        /// <summary>
        /// 购买类型
        /// </summary>
        public BuyPayType BuyType { get; set; }
        /// <summary>
        /// 当前追号计划先行赔付金额
        /// </summary>
        public decimal CurrLossMoney { get; set; }
        /// <summary>
        /// 用户显示名
        /// </summary>
        public string UserDisplayName { get; set; }

    }
    [CommunicationObject]
    public class WinnerModelSchemeInfo_Collection
    {
        public WinnerModelSchemeInfo_Collection()
        {
            ModelSchemeList = new List<WinnerModelSchemeInfo>();
        }
        public int TotalCount { get; set; }
        public List<WinnerModelSchemeInfo> ModelSchemeList { get; set; }
    }
}
