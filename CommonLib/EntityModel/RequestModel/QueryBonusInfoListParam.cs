using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.RequestModel
{
   public class QueryBonusInfoListParam:Page
    {
        public string userId { get; set; }
        public string gameCode { get; set; }
        public string gameType { get; set; }
        public string issuseNumber { get; set; }
        public int completeData { get; set; }
        public string key { get; set; }
    }
}
