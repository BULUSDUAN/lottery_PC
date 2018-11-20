using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBiz.Domain.Entities
{
    /// <summary>
    /// 用户积分明细
    /// </summary>
    public class UserIntegralDetail
    {
        public virtual long Id { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public virtual string OrderId { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public virtual string Summary { get; set; }
        /// <summary>
        /// 消费积分
        /// </summary>
        public virtual int PayIntegral { get; set; }
        /// <summary>
        /// 消费前
        /// </summary>
        public virtual int BeforeIntegral { get; set; }
        /// <summary>
        /// 消费后
        /// </summary>
        public virtual int AfterIntegral { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
