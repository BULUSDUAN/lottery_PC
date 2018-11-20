using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBiz.Domain.Entities
{
    public class SFGG_Match
    {
        public virtual int Id { get; set; }
        public virtual string MatchId { get; set; }
        public virtual int MatchOrderId { get; set; }
        public virtual int MatchState { get; set; }
        public virtual string Category { get; set; }
        public virtual string IssuseNumber { get; set; }
        public virtual string MatchName { get; set; }
        public virtual string HomeTeamName { get; set; }
        public virtual string GuestTeamName { get; set; }
        public virtual string LetBall { get; set; }
        public virtual decimal LoseOdds { get; set; }
        public virtual decimal WinOdds { get; set; }
        public virtual DateTime? MatchStartTime { get; set; }
        public virtual DateTime? BetStopTime { get; set; }
        public virtual DateTime? CreateTime { get; set; }
        public virtual string HomeFull_Result { get; set; }
        public virtual string GuestFull_Result { get; set; }
        public virtual DateTime? MatchResultTime { get; set; }
        public virtual string MatchResultState { get; set; }
        public virtual string SF_Result { get; set; }
        public virtual decimal SF_SP { get; set; }
        public virtual string PrivilegesType { get; set; }
    }
}
