using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
   public class JCZQMatchResult_Collection:Page
    {
        public JCZQMatchResult_Collection() { }
       
        public List<JCZQMatchResult> MatchResultList { get; set; }
    }
    public class JCZQMatchResult
    {
        public JCZQMatchResult() { }

        public string BQC_Result { get; set; }
        public decimal BF_SP { get; set; }
        public string BF_Result { get; set; }
        public decimal ZJQ_SP { get; set; }
        public string ZJQ_Result { get; set; }
        public decimal BRQSPF_SP { get; set; }
        public string BRQSPF_Result { get; set; }
        public decimal SPF_SP { get; set; }
        public string SPF_Result { get; set; }
        public decimal LoseOdds { get; set; }
        public decimal FlatOdds { get; set; }
        public decimal WinOdds { get; set; }
        public int LetBall { get; set; }
        public decimal BQC_SP { get; set; }
        public string MatchState { get; set; }
        public int FullHomeTeamScore { get; set; }
        public int HalfGuestTeamScore { get; set; }
        public int HalfHomeTeamScore { get; set; }
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
        public int FullGuestTeamScore { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
