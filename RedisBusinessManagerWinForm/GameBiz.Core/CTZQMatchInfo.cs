using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core
{
    [CommunicationObject]
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
    [CommunicationObject]
    public class CTZQMatchInfo_Collection
    {
        public CTZQMatchInfo_Collection()
        {
            ListInfo = new List<CTZQMatchInfo>();
        }
        public int TotalCount { get; set; }
        public List<CTZQMatchInfo> ListInfo { get; set; }
    }
    [CommunicationObject]
    public class CTZQMatch_PoolInfo : CTZQMatchInfo
    {
        public decimal BonusBalance { get; set; }
        public int BonusCount { get; set; }
        public int BonusLevel { get; set; }
        public string BonusLevelDisplayName { get; set; }
        public decimal BonusMoney { get; set; }
        public string M_Result { get; set; }
        public decimal TotalSaleMoney { get; set; }
        public DateTime BounsTime { get; set; }
    }
    [CommunicationObject]
    public class CTZQMatch_PoolInfo_Collection
    {
        public CTZQMatch_PoolInfo_Collection()
        {
            ListInfo = new List<CTZQMatch_PoolInfo>();
        }
        public int TotalCount { get; set; }
        public List<CTZQMatch_PoolInfo> ListInfo { get; set; }
    }
}
