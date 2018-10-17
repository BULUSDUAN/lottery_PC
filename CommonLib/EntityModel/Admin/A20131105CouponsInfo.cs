using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel
{
    /// <summary>
    /// 优惠券
    /// </summary>
    [Serializable]
    public class A20131105CouponsInfo
    {
        public int Id { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Money { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 号码
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool CanUsable { get; set; }
        /// <summary>
        /// 使用用户
        /// </summary>
        public string BelongUserId { get; set; }
        /// <summary>
        /// 生成时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
    [Serializable]
    public class A20131105CouponsInfoCollection
    {
        public A20131105CouponsInfoCollection()
        {
            List = new List<A20131105CouponsInfo>();
        }

        public int TotalCount { get; set; }
        public List<A20131105CouponsInfo> List { get; set; }
    }

}
