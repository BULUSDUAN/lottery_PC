using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
   public class NearTimeProfitRate_Collection:Page
    {
        public NearTimeProfitRate_Collection() { }

        public List<NearTimeProfitRateInfo> NearTimeProfitRateList { get; set; }
    }
   public class NearTimeProfitRateInfo
    {
        public NearTimeProfitRateInfo() { }

        public int RowNumber { get; set; }
        public string CurrDate { get; set; }
        public decimal CurrProfitRate { get; set; }
    }
}
