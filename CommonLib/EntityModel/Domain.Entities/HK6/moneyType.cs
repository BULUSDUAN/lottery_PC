using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Domain.Entities.HK6
{
  public  class moneyType
    {
        /// <summary>
        /// 充值
        /// </summary>
        public const string HK6_Recharge = "普通充值";

        /// <summary>
        /// 提取
        /// </summary>
        public const string HK6_Withdraw = "金币提取";

        /// <summary>
        /// 购彩
        /// </summary>
        public const string HK6_Buy = "金币购彩";

        /// <summary>
        /// 中奖
        /// </summary>
        public const string HK6_Bonus = "中奖";

        /// <summary>
        /// 手动充值
        /// </summary>
        public const string HK6_ManualRecharge = "手动充值";

        /// <summary>
        /// 手动充值
        /// </summary>
        public const string HK6_ManualDeduct = "手动扣除";
    }
}
