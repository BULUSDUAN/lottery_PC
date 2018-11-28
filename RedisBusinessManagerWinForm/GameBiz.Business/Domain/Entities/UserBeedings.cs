using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;

namespace GameBiz.Domain.Entities
{
    /// <summary>
    /// 用户战绩
    /// </summary>
    public class UserBeedings
    {
        /// <summary>
        /// 标识
        /// </summary>
        public virtual long Id { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public virtual string UserId { get; set; }
        //public virtual string UserDisplayName { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        /// <summary>
        /// 金星个数
        /// </summary>
        public virtual int GoldStarCount { get; set; }
        /// <summary>
        /// 金钻个数
        /// </summary>
        public virtual int GoldDiamondsCount { get; set; }
        /// <summary>
        /// 金杯个数
        /// </summary>
        public virtual int GoldCupCount { get; set; }
        /// <summary>
        /// 金冠个数
        /// </summary>
        public virtual int GoldCrownCount { get; set; }


        /// <summary>
        /// 银星个数
        /// </summary>
        public virtual int SilverStarCount { get; set; }
        /// <summary>
        /// 银钻个数
        /// </summary>
        public virtual int SilverDiamondsCount { get; set; }
        /// <summary>
        /// 银杯个数
        /// </summary>
        public virtual int SilverCupCount { get; set; }
        /// <summary>
        /// 银冠个数
        /// </summary>
        public virtual int SilverCrownCount { get; set; }


        /// <summary>
        /// 被订制跟单人数
        /// </summary>
        public virtual int BeFollowerUserCount { get; set; }
        /// <summary>
        /// 已被跟单总金额
        /// </summary>
        public virtual decimal BeFollowedTotalMoney { get; set; }

        /// <summary>
        /// 总订单数
        /// </summary>
        public virtual int TotalOrderCount { get; set; }
        /// <summary>
        /// 总投注金额
        /// </summary>
        public virtual decimal TotalBetMoney { get; set; }
        /// <summary>
        /// 总中奖次数
        /// </summary>
        public virtual int TotalBonusTimes { get; set; }
        /// <summary>
        /// 总中奖金额
        /// </summary>
        public virtual decimal TotalBonusMoney { get; set; }

        /// <summary>
        /// 战绩更新时间
        /// </summary>
        public virtual DateTime UpdateTime { get; set; }
    }

    /// <summary>
    /// 用户关注汇总
    /// </summary>
    public class UserAttentionSummary
    {
        public virtual long Id { get; set; }
        public virtual string UserId { get; set; }
        /// <summary>
        /// 被关注人数
        /// </summary>
        public virtual int BeAttentionUserCount { get; set; }
        /// <summary>
        /// 已关注人数
        /// </summary>
        public virtual int FollowerUserCount { get; set; }
        public virtual DateTime UpdateTime { get; set; }
    }

    /// <summary>
    /// 用户关注
    /// </summary>
    public class UserAttention
    {
        public virtual long Id { get; set; }
        /// <summary>
        /// 被关注人编号
        /// </summary>
        public virtual string BeAttentionUserId { get; set; }
        /// <summary>
        /// 关注人编号(粉丝)
        /// </summary>
        public virtual string FollowerUserId { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 用户中奖概率
    /// </summary>
    public class UserBonusPercent
    {
        public virtual long Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        public virtual int TotalOrderCount { get; set; }
        public virtual int BonusOrderCount { get; set; }
        public virtual decimal BonusPercent { get; set; }
        public virtual string CurrentDate { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
