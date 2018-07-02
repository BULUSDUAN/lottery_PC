using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
    /// <summary>
    /// 检查提现申请结果
    /// </summary>
    public class CheckWithdrawResult
    {
        /// <summary>
        /// 申请提现金额
        /// </summary>
        public decimal RequestMoney { get; set; }
        /// <summary>
        /// 到账金额
        /// </summary>
        public decimal ResponseMoney { get; set; }
        /// <summary>
        /// 提现类别
        /// </summary>
        public WithdrawCategory WithdrawCategory { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Summary { get; set; }

    }
}
