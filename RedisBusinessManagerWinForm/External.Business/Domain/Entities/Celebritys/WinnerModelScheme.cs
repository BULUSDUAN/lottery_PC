using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using External.Core;

namespace External.Domain.Entities.Celebritys
{
    public class WinnerModelScheme
    {
        /// <summary>
        /// 主键
        /// </summary>
        public virtual int ModelSchemeId { get; set; }
        /// <summary>
        /// 模型Id
        /// </summary>
        public virtual string ModelId { get; set; }
        /// <summary>
        /// 追号计划订单号
        /// </summary>
        public virtual string ModelKeyLine { get; set; }
        /// <summary>
        /// 模型名称
        /// </summary>
        public virtual string ModelName { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 追号计划状态
        /// </summary>
        public virtual SchemeProgressStatus SchemeProgressStatus { get; set; }
        /// <summary>
        /// 追号期数
        /// </summary>
        public virtual int TotalChaseIssuseCount { get; set; }
        /// <summary>
        /// 投注总额
        /// </summary>
        public virtual decimal TotalMoney { get; set; }
        /// <summary>
        /// 累计奖金
        /// </summary>
        public virtual decimal TotalBonusMoney { get; set; }
        /// <summary>
        /// 盈利后停止
        /// </summary>
        public virtual bool IsProfitedStop { get; set; }
        /// <summary>
        /// 已完成期数
        /// </summary>
        public virtual int CompleteIssuseCount { get; set; }
        /// <summary>
        /// 已完成期数总金额
        /// </summary>
        public virtual decimal CompleteIssuseMoney { get; set; }
        /// <summary>
        /// 是否停止
        /// </summary>
        public virtual bool IsStop { get; set; }
        /// <summary>
        /// 停止时间
        /// </summary>
        public virtual DateTime? StopTime { get; set; }
        /// <summary>
        /// 追号计划停止描述[已停止(用户停止);已停止(模型删除停止)]
        /// </summary>
        public virtual string StopDesc { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime? CreateTime { get; set; }
        /// <summary>
        /// 盈利达到百分比后停止
        /// </summary>
        public virtual decimal ProfiteRatio { get; set; }
        /// <summary>
        /// 投注类型
        /// </summary>
        public virtual BettingType BettingType { get; set; }
        /// <summary>
        /// 购买类型
        /// </summary>
        public virtual BuyPayType BuyType { get; set; }
        /// <summary>
        /// 当前追号计划先行赔付金额
        /// </summary>
        public virtual decimal CurrLossMoney { get; set; }
    }
}
