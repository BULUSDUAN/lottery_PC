using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.RequestModel
{
    public class QueryTodayBDFXList : Page
    {
        public string userId { get; set; }
        public string userName { get; set; }
        public string gameCode { get; set; }
        public string strOrderBy { get; set; }
        public string currentUserId { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public string isMyBD { get; set; }
    }
}
