using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
   public class CTZQMatchInfo
    {
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public int GuestTeamHalfScore { get; set; }
        public string GuestTeamId { get; set; }
        public string GuestTeamName { get; set; }
        public int GuestTeamScore { get; set; }
        public string GuestTeamStanding { get; set; }
        public int HomeTeamHalfScore { get; set; }
        public string HomeTeamId { get; set; }
        public string HomeTeamName { get; set; }
        public int HomeTeamScore { get; set; }
        public string HomeTeamStanding { get; set; }
        public string Id { get; set; }
        public string IssuseNumber { get; set; }
        public int MatchId { get; set; }
        public string MatchName { get; set; }
        public string MatchResult { get; set; }
        public DateTime MatchStartTime { get; set; }
        public int Mid { get; set; }
        public int OrderNumber { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
