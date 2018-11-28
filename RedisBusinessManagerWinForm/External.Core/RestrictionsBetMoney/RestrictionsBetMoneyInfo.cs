using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace External.Core.RestrictionsBetMoney
{
    [CommunicationObject]
    public class RestrictionsBetMoneyInfo
    {
        /// <summary>
        /// 限制投注编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 今天投注金额
        /// </summary>
        public decimal TodayBetMoney { get; set; }
        /// <summary>
        /// 最大限制金额
        /// </summary>
        public decimal MaxRestrictionsMoney { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class RestrictionsUsersInfo
    {
        /// <summary>
        /// 限制编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 最大限制金额
        /// </summary>
        public decimal MaxMoney { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime RegistTime { get; set; }
        /// <summary>
        /// 用户显示名
        /// </summary>
        public string UserDisplayName { get; set; }

    }
    [CommunicationObject]
    public class RestrictionsUsersInfoCollection
    {
        public RestrictionsUsersInfoCollection()
        {
            List = new List<RestrictionsUsersInfo>();
        }
        public int TotalCount { get; set; }
        public List<RestrictionsUsersInfo> List { get; set; }

    }
    [CommunicationObject]
    public class RestrictionsBetMoneyInfoCollection
    {
        public RestrictionsBetMoneyInfoCollection()
        {
            List = new List<RestrictionsBetMoneyInfo>();
        }
        public int TotalCount { get; set; }
        public List<RestrictionsBetMoneyInfo> List { get; set; }

    }
}
