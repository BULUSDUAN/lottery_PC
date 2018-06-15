using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.RequestModel
{
    public class QueryFillMoneyListParam : Page
    {
        public QueryFillMoneyListParam()
        {
            this.OrderId = string.Empty;
        }
        public string statusList { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public string userToken { get; set; }
        public string agentTypeList { get; set; }
        public string sourceList { get; set; }
        public string OrderId { get; set; }
    }
}
