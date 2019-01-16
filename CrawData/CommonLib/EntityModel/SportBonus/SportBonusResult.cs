using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel
{
    public class SportBonusResult
    {
        public bool IsWin { get; set; }
        public int BaseCount { get; set; }

        public int AnteDanCount { get; set; }
        public int AnteTuoCount { get; set; }
        public int AnteTotalCount { get; set; }

        public int HitDanCount { get; set; }
        public int HitTuoCount { get; set; }
        public int HitTotalCount { get; set; }

        // 中奖注数
        public List<string> HitDanMatchIdList { get; set; }
        public List<string> HitTuoMatchIdList { get; set; }
        public List<string> HitTotalMatchIdList { get; set; }

        public int BonusCount { get; set; }
        public List<string[]> BonusHitMatchIdListCollection { get; set; }
    }
}
