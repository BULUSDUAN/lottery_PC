using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBiz.Domain.Entities
{
    /// <summary>
    /// 合买 订制跟单 规则
    /// </summary>
    public class TogetherFollowerRule
    {
        /// <summary>
        /// 自增编号
        /// </summary>
        public virtual long Id { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public virtual bool IsEnable { get; set; }
        /// <summary>
        /// 合买发起人用户编号
        /// </summary>
        public virtual string CreaterUserId { get; set; }
        /// <summary>
        /// 跟单人用户编号
        /// </summary>
        public virtual string FollowerUserId { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public virtual int FollowerIndex { get; set; }
        /// <summary>
        /// 彩种编码
        /// </summary>
        public virtual string GameCode { get; set; }
        /// <summary>
        /// 玩法编码
        /// </summary>
        public virtual string GameType { get; set; }
        /// <summary>
        /// 跟单方案个数(每认购一个就 -1)
        /// </summary>
        public virtual int SchemeCount { get; set; }
        /// <summary>
        /// 方案最小金额 
        /// </summary>
        public virtual decimal MinSchemeMoney { get; set; }
        /// <summary>
        /// 方案最大金额
        /// </summary>
        public virtual decimal MaxSchemeMoney { get; set; }
        /// <summary>
        /// 跟单份数
        /// </summary>
        public virtual int FollowerCount { get; set; }
        /// <summary>
        /// 跟单百分比
        /// </summary>
        public virtual decimal FollowerPercent { get; set; }
        /// <summary>
        /// 当方案剩余份数/百分比不足时 是否跟单
        /// </summary>
        public virtual bool CancelWhenSurplusNotMatch { get; set; }
        /// <summary>
        /// 连续未中奖方案数
        /// </summary>
        public virtual int NotBonusSchemeCount { get; set; }
        /// <summary>
        /// 连续X个方案未中奖则停止跟单
        /// </summary>
        public virtual int CancelNoBonusSchemeCount { get; set; }
        /// <summary>
        /// 当用户金额小于X时停止跟单
        /// </summary>
        public virtual decimal StopFollowerMinBalance { get; set; }

        /// <summary>
        /// 已跟单订单数
        /// </summary>
        public virtual int TotalBetOrderCount { get; set; }
        /// <summary>
        /// 已跟单且中奖订单数
        /// </summary>
        public virtual int TotalBonusOrderCount { get; set; }
        /// <summary>
        /// 已跟单总金额
        /// </summary>
        public virtual decimal TotalBetMoney { get; set; }
        /// <summary>
        /// 已跟单中奖金额
        /// </summary>
        public virtual decimal TotalBonusMoney { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 订制跟单记录
    /// </summary>
    public class TogetherFollowerRecord
    {
        /// <summary>
        /// 自增编号
        /// </summary>
        public virtual long Id { get; set; }
        public virtual long RuleId { get; set; }
        /// <summary>
        /// 用于查询
        ///  string RecordKey = string.Format("{0}_{1}_{2}_{3}", createrUserId, follower.FollowerUserId, gameCode,GameType);
        /// </summary>
        public virtual string RecordKey { get; set; }
        /// <summary>
        /// 合买发起人用户编号
        /// </summary>
        public virtual string CreaterUserId { get; set; }
        /// <summary>
        /// 跟单人用户编号
        /// </summary>
        public virtual string FollowerUserId { get; set; }
        /// <summary>
        /// 彩种编码
        /// </summary>
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        /// <summary>
        /// 方案号
        /// </summary>
        public virtual string SchemeId { get; set; }
        /// <summary>
        /// 每份单价
        /// </summary>
        public virtual decimal Price { get; set; }
        /// <summary>
        /// 购买份数
        /// </summary>
        public virtual int BuyCount { get; set; }
        /// <summary>
        /// 购买金额
        /// </summary>
        public virtual decimal BuyMoney { get; set; }
        /// <summary>
        /// 中奖金额
        /// </summary>
        public virtual decimal BonusMoney { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }
}
