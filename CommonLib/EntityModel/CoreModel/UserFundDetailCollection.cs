using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
   public class UserFundDetailCollection
    {
        public int TotalPayinCount { get; set; }
        public decimal TotalPayinMoney { get; set; }
        public int TotalPayoutCount { get; set; }
        public decimal TotalPayoutMoney { get; set; }
        public decimal TotalBalanceMoney { get; set; }
        public IList<FundDetailInfo> FundDetailList { get; set; }
    }
    public class FundDetailInfo
    {
       
        public long Id { get; set; }
        public string KeyLine { get; set; }
        public string OrderId { get; set; }
        public string UserId { get; set; }
        /// 收支类型
        public PayType PayType { get; set; }
        /// 账户类型
        public AccountType AccountType { get; set; }
        public string Category { get; set; }
        public string Summary { get; set; }
        /// 收入金额
        public decimal PayMoney { get; set; }
        /// 交易前余额
        public decimal BeforeBalance { get; set; }
        /// 交易后余额
        public decimal AfterBalance { get; set; }
        public DateTime CreateTime { get; set; }
        public string OperatorId { get; set; }
    }
    
}
