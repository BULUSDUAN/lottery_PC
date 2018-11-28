using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;

namespace GameBiz.Domain.Entities
{
    public class TotalSingleTreasure
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public virtual string SchemeId { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 累计购买人数
        /// </summary>
        public virtual int TotalBuyCount { get; set; }
        /// <summary>
        /// 累计购买金额
        /// </summary>
        public virtual decimal TotalBuyMoney { get; set; }
        /// <summary>
        /// 累计中奖金额
        /// </summary>
        public virtual decimal TotalBonusMoney { get; set; }
        /// <summary>
        /// 所有抄单盈利率
        /// </summary>
        public virtual decimal ProfitRate { get; set; }
        /// <summary>
        /// 当前投注金额
        /// </summary>
        public virtual decimal CurrentBetMoney { get; set; }
        /// <summary>
        /// 当前中奖金额
        /// </summary>
        public virtual decimal CurrBonusMoney { get; set; }
        /// <summary>
        /// 当前宝单的盈利率
        /// </summary>
        public virtual decimal CurrProfitRate { get; set; }
        /// <summary>
        /// 预计中奖奖金
        /// </summary>
        public virtual decimal ExpectedBonusMoney { get; set; }
        /// <summary>
        /// 预计回报率
        /// </summary>
        public virtual decimal ExpectedReturnRate { get; set; }
        /// <summary>
        /// 提成
        /// </summary>
        public virtual decimal Commission { get; set; }
        /// <summary>
        /// 总的提成金额
        /// </summary>
        public virtual decimal TotalCommissionMoney { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 所选比赛第一场结束时间
        /// </summary>
        public virtual DateTime FirstMatchStopTime { get; set; }
        /// <summary>
        /// 所选比赛最后一场结束时间
        /// </summary>
        public virtual DateTime LastMatchStopTime { get; set; }
        /// <summary>
        /// 是否中奖
        /// </summary>
        public virtual bool IsBonus { get; set; }
        /// <summary>
        /// 是否完成
        /// </summary>
        public virtual bool IsComplate { get; set; }
        /// <summary>
        /// 晒单宣言
        /// </summary>
        public virtual string SingleTreasureDeclaration { get; set; }
        /// <summary>
        /// 方案保密性
        /// </summary>
        public virtual TogetherSchemeSecurity Security { get; set; }
    }

    /// <summary>
    /// 宝单分享数据统计
    /// </summary>
    public class BDFXReportStatisticsData
    {
        /// <summary>
        /// 主键编号
        /// </summary>
        public virtual int Id { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 总盈利率(注：目前暂定最近三个月统计数据)
        /// </summary>
        public virtual decimal TotalProfit { get; set; }
        /// <summary>
        /// 月盈利率(注：目前暂定最近一个月统计数据)
        /// </summary>
        public virtual decimal MonthProfit { get; set; }
        /// <summary>
        /// 周盈利率(注：指最近七天统计数据)
        /// </summary>
        public virtual decimal WeekProfit { get; set; }

        /// <summary>
        /// 总粉丝数(注：目前暂定最近三个月统计数据)
        /// </summary>
        public virtual int TotalFansCount { get; set; }
        /// <summary>
        /// 月粉丝数(注：目前暂定最近一个月统计数据)
        /// </summary>
        public virtual int MonthFansCount { get; set; }
        /// <summary>
        /// 周粉丝数(注：指最近七天统计数据)
        /// </summary>
        public virtual int WeekFansCount { get; set; }

        /// <summary>
        /// 被抄单总数
        /// </summary>
        public virtual int TotalSingleCopyCount { get; set; }
        /// <summary>
        /// 月-被抄单数
        /// </summary>
        public virtual int MonthSingleCopyCount { get; set; }
        /// <summary>
        /// 周-被抄单数
        /// </summary>
        public virtual int WeekSingleCopyCount { get; set; }


        /// <summary>
        /// 总战绩(注：目前暂定最近三个月统计数据)
        /// </summary>
        public virtual string TotalRecord { get; set; }
        /// <summary>
        /// 月战绩(注：目前暂定最近一个月统计数据)
        /// </summary>
        public virtual string MonthRecord { get; set; }
        /// <summary>
        /// 周战绩(注：指最近七天统计数据)
        /// </summary>
        public virtual string WeekRecord { get; set; }
        /// <summary>
        /// 总战绩比例(注：目前暂定最近三个月统计数据)
        /// </summary>
        public virtual decimal TotalRecordRatio { get; set; }
        /// <summary>
        /// 月战绩比例(注：目前暂定最近一个月统计数据)
        /// </summary>
        public virtual decimal MonthRecordRatio { get; set; }
        /// <summary>
        /// 周战绩比例(注：指最近七天统计数据)
        /// </summary>
        public virtual decimal WeekRecordRatio { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public virtual DateTime UpdateTime { get; set; }
    }


    /// <summary>
    /// 宝单、大单推荐专家
    /// </summary>
    public class UserSchemeShareExpert
    {
        /// <summary>
        /// 主键编号
        /// </summary>
        public virtual Int64 Id { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 专家类别：分为宝单专家和大单专家
        /// </summary>
        public virtual CopyOrderSource ExpertType { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public virtual bool IsEnable { get; set; }
        /// <summary>
        /// 显示排序号
        /// </summary>
        public virtual int ShowSort { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }
}
