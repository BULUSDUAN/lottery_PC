using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace Activity.Core
{
    [CommunicationObject]
    public class A20140731_DouDouInfo
    {
        public Int64 Id { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 扣除豆豆
        /// </summary>
        public int DouDou { get; set; }
        /// <summary>
        /// 扣除金额
        /// </summary>
        public decimal Money { get; set; }
        /// <summary>
        /// 活动赠送金额
        /// </summary>
        public decimal ActivityMoney { get; set; }
        /// <summary>
        /// 兑换奖品
        /// </summary>
        public string Prize { get; set; }
        /// <summary>
        /// 奖品价值
        /// </summary>
        public decimal PrizeMoney { get; set; }
        /// <summary>
        /// 是否领取
        /// </summary>
        public bool IsGive { get; set; }
        /// <summary>
        /// 兑换时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class A20140731_DouDouInfoCollection : List<A20140731_DouDouInfo>
    {
        public A20140731_DouDouInfoCollection()
        {
            List = new List<A20140731_DouDouInfo>();
        }
        public List<A20140731_DouDouInfo> List { get; set; }
        public int TotalCount { get; set; }
    }

}
