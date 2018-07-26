using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace EntityModel
{
    /// <summary>
    // 
    ///</summary>
    [ProtoContract]
    [Entity("C_JCLQ_Match",Type = EntityType.Table)]
    public class C_JCLQ_Match
    { 
        public C_JCLQ_Match()
        {
        
        }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(1)]
            [Field("MatchId", IsIdenty = false, IsPrimaryKey = true)]
            public string MatchId{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(2)]
            [Field("MatchIdName")]
            public string MatchIdName{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(3)]
            [Field("MatchData")]
            public string MatchData{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(4)]
            [Field("MatchNumber")]
            public string MatchNumber{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(5)]
            [Field("Mid")]
            public int? Mid{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(6)]
            [Field("LeagueId")]
            public int? LeagueId{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(7)]
            [Field("LeagueName")]
            public string LeagueName{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(8)]
            [Field("LeagueColor")]
            public string LeagueColor{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(9)]
            [Field("HomeTeamId")]
            public int? HomeTeamId{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(10)]
            [Field("HomeTeamName")]
            public string HomeTeamName{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(11)]
            [Field("GuestTeamId")]
            public int? GuestTeamId{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(12)]
            [Field("GuestTeamName")]
            public string GuestTeamName{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(13)]
            [Field("MatchState")]
            public int? MatchState{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(14)]
            [Field("AverageWin")]
            public decimal? AverageWin{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(15)]
            [Field("AverageLose")]
            public decimal? AverageLose{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(16)]
            [Field("StartDateTime")]
            public DateTime? StartDateTime{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(17)]
            [Field("DSStopBettingTime")]
            public DateTime? DSStopBettingTime{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(18)]
            [Field("FSStopBettingTime")]
            public DateTime? FSStopBettingTime{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(19)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(20)]
            [Field("PrivilegesType")]
            public string PrivilegesType{ get; set; }
    }
}