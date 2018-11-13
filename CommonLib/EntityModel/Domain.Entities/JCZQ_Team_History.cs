using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class JCZQ_Team_History
    {
       

        public string Ln { get; set; }
        public string HTeam { get; set; }
        public string ATeam { get; set; }
        public string MTime { get; set; }
        public int HScore { get; set; }
        public int AScore { get; set; }
        public string Bc { get; set; }
        public string Bet { get; set; }
        public string BInfo { get; set; }
        public string HTId { get; set; }
        public string ATId { get; set; }
        public string Cl { get; set; }
    }
}
