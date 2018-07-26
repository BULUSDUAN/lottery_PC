using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
    public class Withdraw_QueryInfoCollection : Page
    {
        public Withdraw_QueryInfoCollection()
        {
            WithdrawList = new List<Withdraw_QueryInfo>();
        }

        public int WinCount { get; set; }
        public int RefusedCount { get; set; }
        public decimal TotalWinMoney { get; set; }
        public decimal TotalResponseMoney { get; set; }
        public decimal TotalRefusedMoney { get; set; }
        public decimal TotalMoney { get; set; }

        public List<Withdraw_QueryInfo> WithdrawList { get; set; }
    }
    public class Withdraw_QueryInfo
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 申请人真实姓名
        /// </summary>
        public string RequesterRealName { get; set; }
        /// <summary>
        /// 申情人手机
        /// </summary>
        public string RequesterMobile { get; set; }
        /// <summary>
        /// 申请人名称
        /// </summary>
        public string RequesterDisplayName { get; set; }
        /// <summary>
        /// 申请人名称编号
        /// </summary>
        public string RequesterUserKey { get; set; }
        /// <summary>
        /// 申请人名称来源
        /// </summary>
        public string RequesterComeFrom { get; set; }
        /// <summary>
        /// 处理人名称
        /// </summary>
        public string ProcessorsDisplayName { get; set; }
        /// <summary>
        /// 处理人名称编号
        /// </summary>
        public string ProcessorsUserKey { get; set; }
        /// <summary>
        /// 提现类别
        /// </summary>
        public int WithdrawAgent { get; set; }
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
        /// 申请时间
        /// </summary>
        public DateTime? RequestTime { get; set; }
        /// <summary>
        /// 提款状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 响应金额。即已提款金额
        /// </summary>
        public decimal? ResponseMoney { get; set; }
        /// <summary>
        /// 响应消息
        /// </summary>
        public string ResponseMessage { get; set; }
        /// <summary>
        /// 回复时间
        /// </summary>
        public DateTime? ResponseTime { get; set; }
  
    }

}
