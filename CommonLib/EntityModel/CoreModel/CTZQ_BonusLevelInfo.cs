﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
    [BsonIgnoreExtraElements]
    public class CTZQ_BonusLevelInfo
    {
        [BsonId]
        public ObjectId _id { get; set; }

        public string Id { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string IssuseNumber { get; set; }
        public int BonusLevel { get; set; }
        public int BonusCount { get; set; }
        public string BonusLevelDisplayName { get; set; }
        public decimal BonusMoney { get; set; }
        public string MatchResult { get; set; }
        public decimal BonusBalance { get; set; }
        public decimal TotalSaleMoney { get; set; }
        public string CreateTime { get; set; }

        public override bool Equals(object obj)
        {
            var t = obj as CTZQ_BonusLevelInfo;
            if (t.Id != this.Id
                || t.BonusCount != this.BonusCount
                || t.BonusMoney != this.BonusMoney
                || t.MatchResult != this.MatchResult)
                return false;
            return true;
        }
    }
}
