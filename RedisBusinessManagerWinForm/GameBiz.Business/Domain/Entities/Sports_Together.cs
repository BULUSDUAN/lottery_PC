using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;

namespace GameBiz.Domain.Entities
{
    public class Sports_Together
    {
        public virtual string SchemeId { get; set; }
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
        /// 参与密码
        /// </summary>
        public virtual string JoinPwd { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public virtual decimal TotalMoney { get; set; }
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
        /// 中奖提成 0-10
        /// </summary>
        public virtual int BonusDeduct { get; set; }
        /// <summary>
        /// 方案提成
        /// </summary>
        public virtual decimal SchemeDeduct { get; set; }
        /// <summary>
        /// 发起人认购份数
        /// </summary>
        public virtual int Subscription { get; set; }
        /// <summary>
        /// 发起人保底份数
        /// </summary>
        public virtual int Guarantees { get; set; }
        /// <summary>
        /// 是否已退还用户保底
        /// </summary>
        public virtual bool IsPayBackGuarantees { get; set; }
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
        /// <summary>
        /// 是否已上传号码
        /// </summary>
        public virtual bool IsUploadAnteCode { get; set; }
        public virtual DateTime StopTime { get; set; }

        public virtual SchemeSource SchemeSource { get; set; }
        public virtual SchemeBettingCategory SchemeBettingCategory { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        public virtual string PlayType { get; set; }
        public virtual int TotalMatchCount { get; set; }

        public virtual string CreateUserId { get; set; }
        public virtual string AgentId { get; set; }
        public virtual string CreateTimeOrIssuseNumber { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

    public class Temp_Together
    {
        public virtual string SchemeId { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string StopTime { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

    public class Sports_TogetherJoin
    {
        public virtual long Id { get; set; }
        public virtual string SchemeId { get; set; }
        public virtual string JoinUserId { get; set; }
        public virtual int BuyCount { get; set; }
        public virtual int RealBuyCount { get; set; }
        /// <summary>
        /// 每份单价
        /// </summary>
        public virtual decimal Price { get; set; }
        /// <summary>
        /// 购买总金额
        /// </summary>
        public virtual decimal TotalMoney { get; set; }
        /// <summary>
        /// 参与类别
        /// </summary>
        public virtual TogetherJoinType JoinType { get; set; }
        /// <summary>
        /// 参与成功，退出合买时为false
        /// </summary>
        public virtual bool JoinSucess { get; set; }
        /// <summary>
        /// 参与日志
        /// </summary>
        public virtual string JoinLog { get; set; }
        public virtual decimal PreTaxBonusMoney { get; set; }
        public virtual decimal AfterTaxBonusMoney { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }


}
