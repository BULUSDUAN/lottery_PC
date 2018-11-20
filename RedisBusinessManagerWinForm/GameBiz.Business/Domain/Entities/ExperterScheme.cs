using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;

namespace GameBiz.Domain.Entities
{
    /// <summary>
    /// 专家方案
    /// </summary>
    public class ExperterScheme
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 方案ID
        /// </summary>
        public virtual string SchemeId { get; set; }
        /// <summary>
        /// 名家ID
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 专家类别
        /// </summary>
        public virtual ExperterType ExperterType { get; set; }
        /// <summary>
        /// 投注金额
        /// </summary>
        public virtual decimal BetMoney { get; set; }
        /// <summary>
        /// 中奖金额
        /// </summary>
        public virtual decimal BonusMoney { get; set; }
        /// <summary>
        /// 中奖状态
        /// </summary>
        public virtual BonusStatus BonusStatus { get; set; }
        /// <summary>
        /// 支持
        /// </summary>
        public virtual int Support { get; set; }
        /// <summary>
        /// 反对
        /// </summary>
        public virtual int Against { get; set; }
        /// <summary>
        /// 主队的评论
        /// </summary>
        public virtual string HomeTeamComments { get; set; }
        /// <summary>
        /// 主队的评论
        /// </summary>
        public virtual string GuestTeamComments { get; set; }
        /// <summary>
        /// 方案截止时间
        /// </summary>
        public virtual DateTime StopTime { get; set; }
        /// <summary>
        /// 当前发布时间
        /// </summary>
        public virtual string CurrentTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 专家方案支持率
    /// </summary>
    public class ExperterSchemeSupport
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 方案ID
        /// </summary>
        public virtual string SchemeId { get; set; }
        /// <summary>
        /// 支持者用户Id
        /// </summary>
        public virtual string SupportUserId { get; set; }
        /// <summary>
        /// 反对者用户Id 
        /// </summary>
        public virtual string AgainstUserId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }
}
