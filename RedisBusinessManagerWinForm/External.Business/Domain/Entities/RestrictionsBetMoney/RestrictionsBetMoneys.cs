using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace External.Business.Domain.Entities.RestrictionsBetMoney
{
    public class RestrictionsBetMoneys
    {
        /// <summary>
        /// 限制投注编号
        /// </summary>
        public virtual int Id { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 今天投注金额
        /// </summary>
        public virtual decimal TodayBetMoney { get; set; }
        /// <summary>
        /// 最大限制金额
        /// </summary>
        public virtual decimal MaxRestrictionsMoney { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }
    public class RestrictionsUsers
    {
        /// <summary>
        /// 限制编号
        /// </summary>
        public virtual int Id { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 最大限制金额
        /// </summary>
        public virtual decimal MaxMoney { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }
}
