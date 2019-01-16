using System;
using System.Collections.Generic;
using System.Text;
using EntityModel.Enum;
namespace EntityModel.RequestModel
{
    public class QueryCreateTogetherOrderParam : Page
    {
        public string userId { get; set; }
        public BonusStatus? bonus { get; set; }
        public string gameCode { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
    }
}
