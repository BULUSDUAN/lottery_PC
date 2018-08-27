using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.RequestModel
{
    public class QueryMyOrderListInfoParam:Page
    {
        public string gameCode { get; set; }
        public BonusStatus? bonusStatus { get; set; }
        public SchemeType? schemeType { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public string userId { get; set; }
    }
}
