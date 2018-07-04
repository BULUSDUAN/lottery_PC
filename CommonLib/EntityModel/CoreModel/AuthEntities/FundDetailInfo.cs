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

    /// <summary>
    /// 用户提现申请
    /// </summary>
    public class Withdraw_RequestInfo
    {
        /// <summary>
        /// 提现渠道
        /// </summary>
        public WithdrawAgentType WithdrawAgent { get; set; }
        /// <summary>
        /// 省份名称
        /// </summary>
        public string ProvinceName { get; set; }
        /// <summary>
        /// 城市名称
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 银行编号。如果不是银行卡支付，则为对应子类型的编码。如：支付宝为ALYPAY
        /// </summary>
        public string BankCode { get; set; }
        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// 开户支行名称
        /// </summary>
        public string BankSubName { get; set; }
        /// <summary>
        /// 银行卡卡号。如果不是银行卡提款，则为对应的帐号。
        /// </summary>
        public string BankCardNumber { get; set; }
        /// <summary>
        /// 申请提款金额
        /// </summary>
        public decimal RequestMoney { get; set; }
        /// <summary>
        /// 用户真实姓名
        /// </summary>
        public String userRealName { get; set; }
    }


}
