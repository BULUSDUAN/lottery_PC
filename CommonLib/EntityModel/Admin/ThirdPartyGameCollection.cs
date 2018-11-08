using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel
{
    public class ThirdPartyGameCollection
    {
        public int TotalCount { get; set; }
        //public decimal TotalRechargeMoney { get; set; }
        //public decimal TotalWithdrawMoney { get; set; }

        public List<GameTransfer_ShowModel> List {get;set;}
    }
}
