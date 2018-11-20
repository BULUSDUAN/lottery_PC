using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Auth.Domain.Entities;
using GameBiz.Core;

namespace GameBiz.Domain.Entities
{
    /// <summary>
    /// 提现
    /// </summary>
    public class Withdraw
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public virtual string OrderId { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 提现渠道
        /// </summary>
        public virtual WithdrawAgentType WithdrawAgent { get; set; }
        /// <summary>
        /// 提现类别
        /// </summary>
        public virtual WithdrawCategory WithdrawCategory { get; set; }
        /// <summary>
        /// 省份名称
        /// </summary>
        public virtual string ProvinceName { get; set; }
        /// <summary>
        /// 城市名称
        /// </summary>
        public virtual string CityName { get; set; }
        /// <summary>
        /// 银行编号。如果不是银行卡支付，则为对应子类型的编码。如：支付宝为ALYPAY
        /// </summary>
        public virtual string BankCode { get; set; }
        /// <summary>
        /// 银行名称
        /// </summary>
        public virtual string BankName { get; set; }
        /// <summary>
        /// 开户支行名称
        /// </summary>
        public virtual string BankSubName { get; set; }
        /// <summary>
        /// 银行卡卡号。如果不是银行卡提款，则为对应的帐号。
        /// </summary>
        public virtual string BankCardNumber { get; set; }
        /// <summary>
        /// 申请提款金额
        /// </summary>
        public virtual decimal RequestMoney { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public virtual DateTime RequestTime { get; set; }
        /// <summary>
        /// 提款状态
        /// </summary>
        public virtual WithdrawStatus Status { get; set; }
        /// <summary>
        /// 响应金额。即已提款金额
        /// </summary>
        public virtual decimal? ResponseMoney { get; set; }
        /// <summary>
        /// 响应消息
        /// </summary>
        public virtual string ResponseMessage { get; set; }
        /// <summary>
        /// 处理人
        /// </summary>
        public virtual string ResponseUserId { get; set; }
        /// <summary>
        /// 回复时间
        /// </summary>
        public virtual DateTime? ResponseTime { get; set; }
        /// <summary>
        /// 代理商
        /// </summary>
        public virtual string AgentId { get; set; }
    }
}
