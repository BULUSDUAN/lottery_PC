using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBiz.Domain.Entities
{
    public class BankCard
    {
        /// <summary>
        /// id
        /// </summary>
        public virtual long BId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 用户真实姓名
        /// </summary>
        public virtual string RealName { get; set; }
        /// <summary>
        /// 银行编号
        /// </summary>
        public virtual string BankCode { get; set; }
        /// <summary>
        /// 银行名称
        /// </summary>
        public virtual string BankName { get; set; }
        /// <summary>
        /// 省份名称
        /// </summary>
        public virtual string ProvinceName { get; set; }
        /// <summary>
        /// 城市名称
        /// </summary>
        public virtual string CityName { get; set; }
        /// <summary>
        /// 开户支行名称
        /// </summary>
        public virtual string BankSubName { get; set; }
        /// <summary>
        /// 银行卡卡号
        /// </summary>
        public virtual string BankCardNumber { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime UpdateTime { get; set; }
    }
}
