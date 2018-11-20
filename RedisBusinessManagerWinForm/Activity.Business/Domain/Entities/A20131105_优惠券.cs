using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Activity.Domain.Entities
{
    public class A20131105_优惠券
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public virtual decimal Money { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public virtual string Summary { get; set; }
        /// <summary>
        /// 号码
        /// </summary>
        public virtual string Number { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        public virtual bool CanUsable { get; set; }
        /// <summary>
        /// 使用用户
        /// </summary>
        public virtual string BelongUserId { get; set; }
        /// <summary>
        /// 生成时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }
}
