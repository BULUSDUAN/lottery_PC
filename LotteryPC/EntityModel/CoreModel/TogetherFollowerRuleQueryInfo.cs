using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
   //public class TogetherFollowerRuleQueryInfoCollection : Page
   // {
   //     public List<TogetherFollowerRuleQueryInfo> List { get; set; }
   // }
    public class TogetherFollowerRuleQueryInfo
    {
        public TogetherFollowerRuleQueryInfo()
        { }
        public bool CancelWhenSurplusNotMatch { get; set; }
        public decimal FollowerPercent { get; set; }
        public int FollowerCount { get; set; }
        public decimal MaxSchemeMoney { get; set; }
        public decimal MinSchemeMoney { get; set; }
        public int SchemeCount { get; set; }
        public string GameType { get; set; }
        public string GameCode { get; set; }
        public string FollowerUserId { get; set; }
        public string CreaterUserId { get; set; }
        public bool IsEnable { get; set; }
        public DateTime CreateTime { get; set; }
        public int FollowerIndex { get; set; }
        public decimal BonusMoney { get; set; }
        public decimal BuyMoney { get; set; }
        public int HideDisplayNameCount { get; set; }
        public string UserDisplayName { get; set; }
        public string UserId { get; set; }
        public long RuleId { get; set; }
        public int CancelNoBonusSchemeCount { get; set; }
        public decimal StopFollowerMinBalance { get; set; }
    }
}
