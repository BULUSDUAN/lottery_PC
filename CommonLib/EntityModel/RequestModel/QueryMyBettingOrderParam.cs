using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.RequestModel
{
    public class QueryMyBettingOrderParam : Page
    {
        public BonusStatus? bonusStatus { get; set; }
        public string gameCode { get; set; }
        public DateTime? startTime { get; set; }
        public DateTime? endTime { get; set; }
        public string UserID { get; set; }
    }
}
