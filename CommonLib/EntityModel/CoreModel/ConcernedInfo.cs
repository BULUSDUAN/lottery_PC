using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
   public class ConcernedInfo
    {
        public ConcernedInfo()
        { }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public int BeConcernedUserCount { get; set; }
        public int ConcernedUserCount { get; set; }
        public int SingleTreasureCount { get; set; }
        public bool IsGZ { get; set; }
        public int RankNumber { get; set; }
        public NearTimeProfitRate_Collection NearTimeProfitRateCollection { get; set; }
        public string FileCreateTime { get; set; }
    }
}
