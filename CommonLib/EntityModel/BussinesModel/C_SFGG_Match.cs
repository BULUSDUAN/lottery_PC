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
    [Entity("C_SFGG_Match", Type = EntityType.Table)]
    public class C_SFGG_Match
    { 
        public C_SFGG_Match()
        {
        
        }
        [Field("Id", IsIdenty = false, IsPrimaryKey = true)]
        public int Id { get; set; }
        /// <summary>
        // 比赛Id
        ///</summary>
        [ProtoMember(1)]
            [Field("MatchId")]
            public string MatchId{ get; set; }

        [Field("MatchOrderId")]
        public int MatchOrderId { get; set; }
        [Field("MatchState")]
        public int MatchState { get; set; }


        [Field("Category")]
        public string Category { get; set; }

        [Field("IssuseNumber")]
        public string IssuseNumber { get; set; }

        [Field("MatchName")]
        public string MatchName { get; set; }
        [Field("HomeTeamName")]
        public string HomeTeamName { get; set; }
        [Field("GuestTeamName")]
        public string GuestTeamName { get; set; }
        [Field("LetBall")]
        public string LetBall { get; set; }
        [Field("LoseOdds")]
        public decimal LoseOdds { get; set; }
        [Field("WinOdds")]
        public decimal WinOdds { get; set; }
        [Field("MatchStartTime")]
        public DateTime MatchStartTime { get; set; }
        [Field("BetStopTime")]
        public DateTime BetStopTime { get; set; }
        [Field("CreateTime")]
        public DateTime CreateTime { get; set; }

        [Field("HomeFull_Result")]
        public string HomeFull_Result { get; set; }

        [Field("GuestFull_Result")]
        public string GuestFull_Result { get; set; }
        [Field("MatchResultTime")]
        public DateTime MatchResultTime { get; set; }

        [Field("MatchResultState")]
        public string MatchResultState { get; set; }

        [Field("SF_Result")]
        public string SF_Result { get; set; }


        [Field("SF_SP")]
        public decimal SF_SP { get; set; }
        [Field("PrivilegesType")]
        public string PrivilegesType { get; set; }

     
    }
}