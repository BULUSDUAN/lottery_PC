using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core
{
    [CommunicationObject]
    public class SFGG_MatchInfo
    {
        //public int Id { get; set; }
        //public string MatchId { get; set; }
        public string Id { get; set; }
        public int MatchOrderId { get; set; }
        public int MatchState { get; set; }
        public string Category { get; set; }
        public string IssuseNumber { get; set; }
        public string MatchName { get; set; }
        public string HomeTeamName { get; set; }
        public string GuestTeamName { get; set; }
        public string LetBall { get; set; }
        public decimal LoseOdds { get; set; }
        public decimal WinOdds { get; set; }
        public string MatchStartTime { get; set; }
        public string BetStopTime { get; set; }
        public string CreateTime { get; set; }
        public string HomeFull_Result { get; set; }
        public string GuestFull_Result { get; set; }
        public string MatchResultTime { get; set; }
        public string MatchResultState { get; set; }
        public string SF_Result { get; set; }
        public decimal SF_SP { get; set; }
        public string PrivilegesType { get; set; }

    }
    [CommunicationObject]
    public class SFGG_Match_Collection
    {
        public SFGG_Match_Collection()
        {
            SFGGListInfo = new List<SFGG_MatchInfo>();
        }
        public int TotalCount { get; set; }
        public List<SFGG_MatchInfo> SFGGListInfo { get; set; }
    }

    [CommunicationObject]
    public class SFGG_MatchResultInfo
    {
        //public int Id { get; set; }
        //public string MatchId { get; set; }
        public string Id { get; set; }
        public int MatchOrderId { get; set; }
        public string MatchState { get; set; }
        public string Category { get; set; }
        public string IssuseNumber { get; set; }
        public string MatchName { get; set; }
        public string HomeTeamName { get; set; }
        public string GuestTeamName { get; set; }
        public string LetBall { get; set; }
        public decimal LoseOdds { get; set; }
        public decimal WinOdds { get; set; }
        public string MatchStartTime { get; set; }
        public string BetStopTime { get; set; }
        public string CreateTime { get; set; }
        public string HomeFull_Result { get; set; }
        public string GuestFull_Result { get; set; }
        public string MatchResultTime { get; set; }
        public string MatchResultState { get; set; }
        public string SF_Result { get; set; }
        public decimal SF_SP { get; set; }
        public string PrivilegesType { get; set; }

    }
}
