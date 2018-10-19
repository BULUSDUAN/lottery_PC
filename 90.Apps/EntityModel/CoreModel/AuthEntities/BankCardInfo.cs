using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModel.CoreModel.AuthEntities
{
    public class BankCardInfo
    {
        /// <summary>
        /// id
        /// </summary>
        public long BId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户真实姓名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 银行编号
        /// </summary>
        public string BankCode { get; set; }
        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// 省份名称
        /// </summary>
        public string ProvinceName { get; set; }
        /// <summary>
        /// 城市名称
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 开户支行名称
        /// </summary>
        public string BankSubName { get; set; }
        /// <summary>
        /// 银行卡卡号
        /// </summary>
        public string BankCardNumber { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}
