using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Activity.Domain.Entities
{
    public class A20140731_DouDou
    {
        public virtual Int64 Id { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 扣除豆豆
        /// </summary>
        public virtual int DouDou { get; set; }
        /// <summary>
        /// 扣除金额
        /// </summary>
        public virtual decimal Money { get; set; }
        /// <summary>
        /// 活动赠送金额
        /// </summary>
        public virtual decimal ActivityMoney { get; set; }
        /// <summary>
        /// 兑换奖品
        /// </summary>
        public virtual string Prize { get; set; }
        /// <summary>
        /// 奖品价值
        /// </summary>
        public virtual decimal PrizeMoney { get; set; }
        /// <summary>
        /// 是否领取
        /// </summary>
        public virtual bool IsGive { get; set; }
        /// <summary>
        /// 兑换时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }
}
