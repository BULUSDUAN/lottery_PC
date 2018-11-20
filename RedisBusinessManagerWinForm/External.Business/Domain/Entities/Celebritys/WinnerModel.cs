using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using External.Core;

namespace External.Domain.Entities.Celebritys
{
    public class WinnerModel
    {
        /// <summary>
        /// 模型Id
        /// </summary>
        public virtual string ModelId { get; set; }
        /// <summary>
        /// 模型类别
        /// </summary>
        public virtual ModelType ModelType { get; set; }
        /// <summary>
        /// 模型名称
        /// </summary>
        public virtual string ModelName { get; set; }
        /// <summary>
        /// 模型描述
        /// </summary>
        public virtual string ModelDescription { get; set; }
        /// <summary>
        /// 方案编号
        /// </summary>
        public virtual string SchemeId { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        public virtual string GameCode { get; set; }
        /// <summary>
        /// 玩法
        /// </summary>
        public virtual string GameType { get; set; }
        /// <summary>
        /// 过关方式
        /// </summary>
        public virtual string PlayType { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 投注总期数
        /// </summary>
        public virtual int TotalBettingIssuseCount { get; set; }
        /// <summary>
        /// 中奖总期数
        /// </summary>
        public virtual int TotalBonusIssuseCount { get; set; }
        /// <summary>
        /// 方案金额(单倍金额)
        /// </summary>
        public virtual decimal SchemeMoney { get; set; }
        /// <summary>
        /// 累计购买人数
        /// </summary>
        public virtual int TotalBuyCount { get; set; }
        /// <summary>
        /// 累计中奖金额
        /// </summary>
        public virtual decimal TotalBonusMoney { get; set; }
        /// <summary>
        /// 累计盈利金额
        /// </summary>
        public virtual decimal TotalProfitMoney { get; set; }
        /// <summary>
        /// 累计销售金额
        /// </summary>
        public virtual decimal TotalSaleMoney { get; set; }
        /// <summary>
        /// 实战盈利比率=中奖频率
        /// </summary>
        public virtual decimal ActulProfitRatio { get; set; }
        /// <summary>
        /// 总回报率=历史中奖总额/历史投注总额
        /// </summary>
        public virtual decimal TotalReportRatio { get; set; }
        /// <summary>
        /// 中奖频率=中奖期数/投注期数
        /// </summary>
        public virtual decimal BonusFrequency { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public virtual bool IsDelete { get; set; }
        /// <summary>
        /// 删除时间
        /// </summary>
        public virtual DateTime? DeleteTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime? CreateTime { get; set; }
        /// <summary>
        /// 是否为名家方案
        /// </summary>
        //public virtual bool IsExperter { get; set; }
        /// <summary>
        /// 分享
        /// </summary>
        public virtual bool IsShare { get; set; }
        /// <summary>
        /// 保密性
        /// </summary>
        public virtual ModelSecurity ModelSecurity { get; set; }
        /// <summary>
        /// 是否先行赔付
        /// </summary>
        public virtual bool IsFirstPayment { get; set; }
        /// <summary>
        /// 先行赔付_盈利计划期数
        /// </summary>
        public virtual int ProfitIssuseCount { get; set; }
        /// <summary>
        /// 先行赔付_风险程度
        /// </summary>
        public virtual RiskType RiskType { get; set; }
        /// <summary>
        /// 亏损百分比
        /// </summary>
        public virtual decimal LossRatio { get; set; }
        /// <summary>
        /// 佣金百分比
        /// </summary>
        public virtual decimal CommissionRitio { get; set; }
        /// <summary>
        /// 是否盈利
        /// </summary>
        //public virtual bool IsProfit { get; set; }
        /// <summary>
        /// 总的赔付金额
        /// </summary>
        public virtual decimal TotalLoseMoney { get; set; }
        /// <summary>
        /// 已赔付金额
        /// </summary>
        public virtual decimal TotalHaveMoney { get; set; }
        /// <summary>
        /// 分享奖励金额
        /// </summary>
        public virtual decimal TotalShareBonusMoney { get; set; }
        /// <summary>
        /// 先行赔付佣金
        /// </summary>
        public virtual decimal TotalCommissionMoney { get; set; }
        /// <summary>
        /// 用户显示名
        /// </summary>
        public virtual string UserDisplayName { get; set; }
        /// <summary>
        /// 是否已开启先行赔付
        /// </summary>
        public virtual bool IsEnableFirstPayment { get; set; }
        /// <summary>
        /// 最新期方案停止投注时间
        /// </summary>
        public virtual DateTime CurrSchemeStopTime { get; set; }
        /// <summary>
        /// 模型每期方案历史中奖金额
        /// </summary>
        public virtual decimal TotalModelBonusMoney { get; set; }
        /// <summary>
        /// 模型每期方案历史投注金额
        /// </summary>
        public virtual decimal TotalModelBettingMoney { get; set; }
        /// <summary>
        /// 累计未中奖期数
        /// </summary>
        //public virtual int TotalNotBonusIssuseCount { get; set; }
        /// <summary>
        /// 每期方案集合
        /// </summary>
        public virtual  IList<WinnerModelCycle> ModelCycleList { get; set; }
        /// <summary>
        /// 模型被收藏次数
        /// </summary>
        public virtual int TotalModelCollection { get; set; }
    }
}
