using EntityModel.Enum;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
    [Serializable]
    [ProtoContract]
   public class Sports_AnteCodeQueryInfoCollection
    {
        public Sports_AnteCodeQueryInfoCollection()
        {
            List = new List<Sports_AnteCodeQueryInfo>();
        }
        [ProtoMember(1)]
        public List<Sports_AnteCodeQueryInfo> List { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public class Sports_AnteCodeQueryInfo
    {

        public Sports_AnteCodeQueryInfo()
        {

        }
        [ProtoMember(1)]
        public string LeagueId { get; set; }
        [ProtoMember(2)]
        public string LeagueName { get; set; }
        [ProtoMember(3)]
        public string LeagueColor { get; set; }
        [ProtoMember(4)]
        public DateTime StartTime { get; set; }
        [ProtoMember(5)]
        public string MatchId { get; set; }
        [ProtoMember(6)]
        public string MatchIdName { get; set; }
        [ProtoMember(7)]
        public string HomeTeamId { get; set; }
        [ProtoMember(8)]
        public string HomeTeamName { get; set; }
        [ProtoMember(9)]
        public string GuestTeamId { get; set; }
        [ProtoMember(10)]
        public string GuestTeamName { get; set; }
        [ProtoMember(11)]
        public string IssuseNumber { get; set; }
        [ProtoMember(12)]
        public string AnteCode { get; set; }
        [ProtoMember(13)]
        public bool IsDan { get; set; }
        [ProtoMember(14)]
        public int LetBall { get; set; }
        [ProtoMember(15)]
        public string CurrentSp { get; set; }
        [ProtoMember(16)]
        public string HalfResult { get; set; }
        [ProtoMember(17)]
        public string FullResult { get; set; }
        [ProtoMember(18)]
        public string MatchResult { get; set; }
        [ProtoMember(19)]
        public decimal MatchResultSp { get; set; }
        [ProtoMember(20)]
        public BonusStatus BonusStatus { get; set; }
        [ProtoMember(21)]
        public string GameType { get; set; }
        [ProtoMember(22)]
        public string MatchState { get; set; }
        [ProtoMember(23)]
        public string WinNumber { get; set; }
    }
}
