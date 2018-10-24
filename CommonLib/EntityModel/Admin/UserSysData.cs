using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel
{
   public class UserSysData
    {
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public string ComeFrom { get; set; }
        public string RegisterIp { get; set; }
        public string CreateTime { get; set; }
        public string IsEnable { get; set; }
        public string IsAgent { get; set; }
        public string IsFillMoney { get; set; }
        public string AgentId { get; set; }
        public string FillMoneyBalance { get; set; }
        public string BonusBalance { get; set; }
        public string FreezeBalance { get; set; }
        public string CommissionBalance { get; set; }
        public string RedBagBalance { get; set; }
        public string ExpertsBalance { get; set; }
        public string IsSettedMobile { get; set; }
        public string Mobile { get; set; }
        public string IsSettedRealName { get; set; }
        public string RealName { get; set; }
        public string CardType { get; set; }
        public string IdCardNumber { get; set; }
        public string IsSettedEmail { get; set; }
        public string Email { get; set; }
        public string VipLevel { get; set; }
        public string UserType { get; set; }
        public string AlipayAccount { get; set; }
        public string QQ { get; set; }
        public string OCAgentCategory { get; set; }
        public string CPSBalance { get; set; }
        public string CPSMode { get; set; }
        public string UserCreditType { get; set; }
        public string UpdateBy { get; set; }
    }
    public class UserSysCount
    {
        public int TotalCount { get; set; }
        public decimal FillMoneyBalance { get; set; }
        public decimal BonusBalance { get; set; }
        public decimal CommissionBalance { get; set; }
        public decimal ExpertsBalance { get; set; }
        public decimal FreezeBalance { get; set; }
        public decimal RedBagBalance { get; set; }
        public int DouDou { get; set; }
        public decimal CPSBalance { get; set; }
    }
}
