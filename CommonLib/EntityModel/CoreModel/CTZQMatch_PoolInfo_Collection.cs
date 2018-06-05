using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
   public class CTZQMatch_PoolInfo_Collection : Page
    {
        public CTZQMatch_PoolInfo_Collection() { }
        public List<CTZQMatch_PoolInfo> ListInfo { get; set; }
    }
    public class CTZQMatch_PoolInfo:CTZQMatchInfo
    {
        public CTZQMatch_PoolInfo() { }

        public decimal BonusBalance { get; set; }
        public int BonusCount { get; set; }
        public int BonusLevel { get; set; }
        public string BonusLevelDisplayName { get; set; }
        public decimal BonusMoney { get; set; }
        public string M_Result { get; set; }
        public decimal TotalSaleMoney { get; set; }
        public DateTime BounsTime { get; set; }
    }

}
