using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace EntityModel.CoreModel
{
   public class RankReportCollection_TotalBonus_Sport:Page
    {
        public IList<RankInfo_TotalBonus_Sport> RankInfoList { get; set; }
    }
    public class RankInfo_TotalBonus_Sport
    {
        public RankInfo_TotalBonus_Sport()
        {

        }

        public int TotalOrderCount { get; set; }
        public decimal TotalOrderMoney { get; set; }
        public decimal BonusMoney { get; set; }
        public decimal ProfitMoney { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public int UserHideDisplayNameCount { get; set; }
    }
}
