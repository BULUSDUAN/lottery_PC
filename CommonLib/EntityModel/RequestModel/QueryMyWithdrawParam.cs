using System;
using System.Collections.Generic;
using System.Text;
using EntityModel.Enum;
namespace EntityModel.RequestModel
{
    public class QueryMyWithdrawParam : Page
    {
        public WithdrawStatus? status { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public string userToken { get; set; }
    }
}
