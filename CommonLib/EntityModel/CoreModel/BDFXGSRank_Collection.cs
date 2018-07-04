using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
   public class BDFXGSRank_Collection:Page
    {
        public BDFXGSRank_Collection() { }
        public IList<BDFXGSRankInfo> RankList { get; set; }
    }
    public class BDFXGSRankInfo
    {
        public BDFXGSRankInfo() { }

        public int RankNumber { get; set; }
        public int LastweekRank { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public int BeConcernedUserCount { get; set; }
        public int SingleTreasureCount { get; set; }
        public decimal CurrProfitRate { get; set; }
        public bool IsGZ { get; set; }
        public string SchemeId { get; set; }
    }
}
