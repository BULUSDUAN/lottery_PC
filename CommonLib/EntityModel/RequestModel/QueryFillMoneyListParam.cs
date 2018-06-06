using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.RequestModel
{
    public class QueryFillMoneyListParam
    {
        public BonusStatus? bonusStatus { get; set; }
        public string gameCode { get; set; }
        public DateTime? startTime { get; set; }
        public DateTime? endTime { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public string userToken { get; set; }
    }
}
