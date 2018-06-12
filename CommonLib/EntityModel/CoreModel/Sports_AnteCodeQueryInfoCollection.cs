using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
   public class Sports_AnteCodeQueryInfoCollection
    {
        public Sports_AnteCodeQueryInfoCollection()
        {

        }
        public List<Sports_AnteCodeQueryInfo> List { get; set; }
    }
    public class Sports_AnteCodeQueryInfo
    {

        public Sports_AnteCodeQueryInfo()
        {

        }
        public string LeagueId { get; set; }
     
        public string LeagueName { get; set; }
        
        public string LeagueColor { get; set; }
     
        public DateTime StartTime { get; set; }
        
        public string MatchId { get; set; }
      
        public string MatchIdName { get; set; }
        
        public string HomeTeamId { get; set; }
        
        public string HomeTeamName { get; set; }
       
        public string GuestTeamId { get; set; }
       
        public string GuestTeamName { get; set; }
      
        public string IssuseNumber { get; set; }
        
        public string AnteCode { get; set; }
       
        public bool IsDan { get; set; }
        
        public int LetBall { get; set; }
        
        public string CurrentSp { get; set; }
       
        public string HalfResult { get; set; }
     
        public string FullResult { get; set; }
      
        public string MatchResult { get; set; }
       
        public decimal MatchResultSp { get; set; }
        
        public BonusStatus BonusStatus { get; set; }
        
        public string GameType { get; set; }
      
        public string MatchState { get; set; }
     
        public string WinNumber { get; set; }
    }
}
