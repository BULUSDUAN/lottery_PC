using System;
using GameBiz.Core;
using GameBiz.Auth.Domain.Entities;
using System.Collections.Generic;

namespace GameBiz.Domain.Entities
{
    /// <summary>
    /// 合买方案主表
    /// </summary>
    public class TogetherSchemeMain
    {
        public virtual string SchemeId { get; set; }
        public virtual SystemUser CreateUser { get; set; }
        public virtual LotteryGame Game { get; set; }
        public virtual SchemeSource SchemeSource { get; set; }
        public virtual string GameTypeList { get; set; }
        public virtual string GameTypeListDisplayName { get; set; }
        public virtual decimal TotalMoney { get; set; }
        public virtual BonusStatus BonusStatus { get; set; }
        public virtual decimal PreTaxBonusMoney { get; set; }
        public virtual decimal AfterTaxBonusMoney { get; set; }
        public virtual bool StopAfterBonus { get; set; }
        public virtual bool CancelAfterAward { get; set; }
        public virtual DateTime CreateTime { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public virtual string Title { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public virtual string Description { get; set; }
        /// <summary>
        /// 方案保密性
        /// </summary>
        public virtual TogetherSchemeSecurity Security { get; set; }
        /// <summary>
        /// 起始期号
        /// </summary>
        public virtual string StartIssuseNumber { get; set; }
        /// <summary>
        /// 总份数
        /// </summary>
        public virtual int TotalCount { get; set; }
        /// <summary>
        /// 每份单价
        /// </summary>
        public virtual decimal Price { get; set; }
        /// <summary>
        /// 售出份数
        /// </summary>
        public virtual int SoldCount { get; set; }
        /// <summary>
        /// 参与人数
        /// </summary>
        public virtual int JoinUserCount { get; set; }
        /// <summary>
        /// 提成 0-10
        /// </summary>
        public virtual int DeductPercentage { get; set; }
        /// <summary>
        /// 发起人认购份数
        /// </summary>
        public virtual int Subscription { get; set; }
        /// <summary>
        /// 发起人保底份数
        /// </summary>
        public virtual int Guarantees { get; set; }
        /// <summary>
        /// 系统保底份数
        /// </summary>
        public virtual int SystemGuarantees { get; set; }
        /// <summary>
        /// 方案进度状态
        /// </summary>
        public virtual TogetherSchemeProgress ProgressStatus { get; set; }
        /// <summary>
        /// 方案进度百分比
        /// </summary>
        public virtual decimal Progress { get; set; }
        /// <summary>
        /// 是否置顶
        /// </summary>
        public virtual bool IsTop { get; set; }
    }
}
