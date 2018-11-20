using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace External.Core.Celebritys
{
    [CommunicationObject]
    public class WinnerModelInfo
    {
        /// <summary>
        /// 模型Id
        /// </summary>
        public string ModelId { get; set; }
        /// <summary>
        /// 模型类别
        /// </summary>
        public ModelType ModelType { get; set; }
        /// <summary>
        /// 模型名称
        /// </summary>
        public string ModelName { get; set; }
        /// <summary>
        /// 模型描述
        /// </summary>
        public string ModelDescription { get; set; }
        /// <summary>
        /// 方案编号
        /// </summary>
        public string SchemeId { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 玩法
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 过关方式
        /// </summary>
        public string PlayType { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 投注总期数
        /// </summary>
        public int TotalBettingIssuseCount { get; set; }
        /// <summary>
        /// 中奖总期数
        /// </summary>
        public int TotalBonusIssuseCount { get; set; }
        /// <summary>
        /// 方案金额(单倍金额)
        /// </summary>
        public decimal SchemeMoney { get; set; }
        /// <summary>
        /// 累计购买人数
        /// </summary>
        public int TotalBuyCount { get; set; }
        /// <summary>
        /// 累计中奖金额
        /// </summary>
        public decimal TotalBonusMoney { get; set; }
        /// <summary>
        /// 累计盈利金额
        /// </summary>
        public decimal TotalProfitMoney { get; set; }
        /// <summary>
        /// 累计销售金额
        /// </summary>
        public decimal TotalSaleMoney { get; set; }
        /// <summary>
        /// 实战盈利比率=中奖频率
        /// </summary>
        public decimal ActulProfitRatio { get; set; }
        /// <summary>
        /// 总回报率=历史中奖总额/历史投注总额
        /// </summary>
        public decimal TotalReportRatio { get; set; }
        /// <summary>
        /// 中奖频率=中奖期数/投注期数
        /// </summary>
        public decimal BonusFrequency { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }
        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime DeleteTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 是否为名家方案
        /// </summary>
        //public bool IsExperter { get; set; }
        /// <summary>
        /// 分享
        /// </summary>
        public bool IsShare { get; set; }
        /// <summary>
        /// 保密性
        /// </summary>
        public ModelSecurity ModelSecurity { get; set; }
        /// <summary>
        /// 是否先行赔付
        /// </summary>
        public bool IsFirstPayment { get; set; }
        /// <summary>
        /// 先行赔付_盈利计划期数
        /// </summary>
        public int ProfitIssuseCount { get; set; }
        /// <summary>
        /// 先行赔付_风险程度
        /// </summary>
        public RiskType RiskType { get; set; }
        /// <summary>
        /// 先行赔付_亏损百分比
        /// </summary>
        public decimal LossRatio { get; set; }
        /// <summary>
        /// 先行赔付_佣金百分比
        /// </summary>
        public decimal CommissionRitio { get; set; }
        /// <summary>
        /// 是否盈利
        /// </summary>
        //public bool IsProfit { get; set; }
        /// <summary>
        /// 总的赔付金额
        /// </summary>
        public decimal TotalLoseMoney { get; set; }
        /// <summary>
        /// 已赔付金额
        /// </summary>
        public decimal TotalHaveMoney { get; set; }
        /// <summary>
        /// 分享奖励金额
        /// </summary>
        public decimal TotalShareBonusMoney { get; set; }
        /// <summary>
        /// 先行赔付佣金
        /// </summary>
        public decimal TotalCommissionMoney { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserDisplayName { get; set; }
        /// <summary>
        /// 是否已开启先行赔付
        /// </summary>
        public bool IsEnableFirstPayment { get; set; }
        /// <summary>
        /// 每期方案集合
        /// </summary>
        public IList<WinnerModelCycleInfo> ModelCycleInfoList { get; set; }
        /// <summary>
        /// 最新期方案停止投注时间
        /// </summary>
        public DateTime CurrSchemeStopTime { get; set; }
        /// <summary>
        /// 模型每期方案历史中奖金额
        /// </summary>
        public decimal TotalModelBonusMoney { get; set; }
        /// <summary>
        /// 模型每期方案历史投注金额
        /// </summary>
        public decimal TotalModelBettingMoney { get; set; }
        /// <summary>
        /// 累计未中奖期数
        /// </summary>
        //public int TotalNotBonusIssuseCount { get; set; }
        /// <summary>
        /// 模型被收藏次数
        /// </summary>
        public int TotalModelCollection { get; set; }
    }
    [CommunicationObject]
    public class WinnerModelInfo_Collection
    {
        public WinnerModelInfo_Collection()
        {
            ModelListInfo = new List<WinnerModelInfo>();
        }
        public int TotalCount { get; set; }
        public List<WinnerModelInfo> ModelListInfo { get; set; }
    }
}
