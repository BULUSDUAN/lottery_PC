using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace External.Core.Popularity
{

    /// <summary>
    /// 单场人气
    /// </summary>
    [CommunicationObject]
    public class PopularitysInfo
    {
        public string MatchId { get; set; }
        public string HomeTeamName { get; set; }
        public string GuestTeamName { get; set; }
        public string GameType { get; set; }
        public int Win { get; set; }
        public int Flat { get; set; }
        public int Negative { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime DSStopBettingTime { get; set; }
        public DateTime FSStopBettingTime { get; set; }
    }

    [CommunicationObject]
    public class PopularitysInfoCollection
    {
        public PopularitysInfoCollection()
        {
            List = new List<PopularitysInfo>();
        }
        public int TotalCount { get; set; }
        public List<PopularitysInfo> List { get; set; }
    }
}
