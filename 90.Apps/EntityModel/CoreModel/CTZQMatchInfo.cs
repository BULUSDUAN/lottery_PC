using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;
namespace EntityModel.CoreModel
{
    [ProtoContract]
   public class CTZQMatchInfo
    {
        [ProtoMember(1)]
        public string GameCode { get; set; }
        [ProtoMember(2)]
        public string GameType { get; set; }
        [ProtoMember(3)]
        public int GuestTeamHalfScore { get; set; }
        [ProtoMember(4)]
        public string GuestTeamId { get; set; }
        [ProtoMember(5)]
        public string GuestTeamName { get; set; }
        [ProtoMember(6)]
        public int GuestTeamScore { get; set; }
        [ProtoMember(7)]
        public string GuestTeamStanding { get; set; }
        [ProtoMember(8)]
        public int HomeTeamHalfScore { get; set; }
        [ProtoMember(9)]
        public string HomeTeamId { get; set; }
        [ProtoMember(10)]
        public string HomeTeamName { get; set; }
        [ProtoMember(11)]
        public int HomeTeamScore { get; set; }
        [ProtoMember(12)]
        public string HomeTeamStanding { get; set; }
        [ProtoMember(13)]
        public string Id { get; set; }
        [ProtoMember(14)]
        public string IssuseNumber { get; set; }
        [ProtoMember(15)]
        public int MatchId { get; set; }
        [ProtoMember(16)]
        public string MatchName { get; set; }
        [ProtoMember(17)]
        public string MatchResult { get; set; }
        [ProtoMember(18)]
        public DateTime MatchStartTime { get; set; }
        [ProtoMember(19)]
        public int Mid { get; set; }
        [ProtoMember(20)]
        public int OrderNumber { get; set; }
        [ProtoMember(21)]
        public DateTime UpdateTime { get; set; }
    }
}
