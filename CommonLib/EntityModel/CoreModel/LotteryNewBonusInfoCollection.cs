using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
   public class LotteryNewBonusInfoCollection:Page
    {
        public List<LotteryNewBonusInfo> List { get; set; }
    }
    public class LotteryNewBonusInfo
    {
        public string SchemeId { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string PlayType { get; set; }
        public string IssuseNumber { get; set; }
        public int Amount { get; set; }
        public string UserDisplayName { get; set; }
        public int HideUserDisplayNameCount { get; set; }
        public decimal TotalMoney { get; set; }
        public decimal PreTaxBonusMoney { get; set; }
        public decimal AfterTaxBonusMoney { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
