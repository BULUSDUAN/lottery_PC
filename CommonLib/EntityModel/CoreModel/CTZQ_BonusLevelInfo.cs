using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
    public class CTZQ_BonusLevelInfo
    {
       

        public string Id { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string IssuseNumber { get; set; }
        public int BonusLevel { get; set; }
        public int BonusCount { get; set; }
        public string BonusLevelDisplayName { get; set; }
        public decimal BonusMoney { get; set; }
        public string MatchResult { get; set; }
        public decimal BonusBalance { get; set; }
        public decimal TotalSaleMoney { get; set; }
        public string CreateTime { get; set; }
    }
}
