using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
   public class JCLQMatchResult_Collection:Page
    {
        public JCLQMatchResult_Collection() { }

        public List<JCLQMatchResult> MatchResultList { get; set; }
    }
    public class JCLQMatchResult
    {
        public JCLQMatchResult() { }

        public string RFSF_Trend { get; set; }
        public string MatchState { get; set; }
        public decimal DXF_SP { get; set; }
        public string DXF_Result { get; set; }
        public decimal SFC_SP { get; set; }
        public string SFC_Result { get; set; }
        public decimal RFSF_SP { get; set; }
        public string RFSF_Result { get; set; }
        public decimal SF_SP { get; set; }
        public string SF_Result { get; set; }
        public string DXF_Trend { get; set; }
        public int GuestTeamScore { get; set; }
        public string GuestTeamName { get; set; }
        public int GuestTeamId { get; set; }
        public string HomeTeamName { get; set; }
        public int HomeTeamId { get; set; }
        public string LeagueColor { get; set; }
        public string LeagueName { get; set; }
        public int LeagueId { get; set; }
        public DateTime StartTime { get; set; }
        public string MatchIdName { get; set; }
        public string MatchId { get; set; }
        public int HomeTeamScore { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
