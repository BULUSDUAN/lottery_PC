using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace External.Core.Report
{
    [CommunicationObject]
    public class TodaySummaryInfo
    {
        public int TodayRegistUserCount { get; set; }
        public int TodayFirstFillMoneyUserCount { get; set; }
        public decimal TodayFillMoney { get; set; }
        public int TodayOrderCount { get; set; }
        public decimal TodayOrderMoney { get; set; }
        public decimal TodayPayMoney { get; set; }
        public int TodayPayCount { get; set; }
    }
}
