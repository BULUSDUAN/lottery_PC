using System;
using System.Collections.Generic;

namespace EntityModel.LotteryJsonInfo
{
    public class CTZQ_JSON
    {
        public CTZQ_MatchInfo_New ListMatch { get; set; }
    }

    public class CTZQ_MatchInfo_New
    {
        public string stop_sale_time { get; set; }
        public List<Match> match { get; set; }
        public string term_no { get; set; }
    }

    public class Match
    {
        public string odd_home { get; set; }
        public string color { get; set; }
        public string match_name { get; set; }
        public string away_team { get; set; }
        public string odd_away { get; set; }
        public string odd_draw { get; set; }
        public string home_team { get; set; }
        public string bout_index { get; set; }
        public string match_time { get; set; }
        public string url { get; set; }
    }
}