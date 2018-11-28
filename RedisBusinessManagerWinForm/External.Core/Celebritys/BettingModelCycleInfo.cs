using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace External.Core.Celebritys
{
    [CommunicationObject]
    public class BettingModelCycleInfo
    {
        /// <summary>
        /// 模型Id
        /// </summary>
        public string ModelId { get; set; }
        /// <summary>
        /// 模型作者编号
        /// </summary>
        public string CreaterUserId { get; set; }
        /// <summary>
        /// 当前登录用户编号
        /// </summary>
        public string CurrUserId { get; set; }
        /// <summary>
        /// 追号期数
        /// </summary>
        public int TotalChaseIssuseCount { get; set; }
        /// <summary>
        /// 盈利后停止
        /// </summary>
        public bool IsProfitedStop { get; set; }
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
        /// 每期方案Id
        /// </summary>
        public int ModelCycleId { get; set; }
        /// <summary>
        /// 投注倍数
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 总投注金额
        /// </summary>
        public decimal TotalBettingMoney { get; set; }
        /// <summary>
        /// 当前投注金额
        /// </summary>
        public decimal CurrBettingMoney { get; set; }
        /// <summary>
        /// 资金密码
        /// </summary>
        public string BalancePassword { get; set; }
        /// <summary>
        /// 模型名称(模型名+用户名)
        /// </summary>
        //public string ModelName { get; set; }
        /// <summary>
        /// 投注模式
        /// 主要包括盈利计划里面：推荐计划：（低风险、适中）；高级设置：（低风险翻倍、固定翻倍、固定盈利率）
        /// </summary>
        public ProfitBettingCategory ProfitBettingCategory { get; set; }
        /// <summary>
        /// 固定翻倍设置
        /// </summary>
        public int SetDoubleAmount { get; set; }
    }
}
