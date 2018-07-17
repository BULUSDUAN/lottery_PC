using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;
namespace EntityModel.CoreModel
{
    [ProtoContract]
   public class BDFXGSRank_Collection:Page
    {
        public BDFXGSRank_Collection() { }
        [ProtoMember(1)]
        public IList<BDFXGSRankInfo> RankList { get; set; }
    }
    [ProtoContract]
    public class BDFXGSRankInfo
    {
        public BDFXGSRankInfo() { }
        [ProtoMember(1)]
        public int RankNumber { get; set; }
        [ProtoMember(2)]
        public int LastweekRank { get; set; }
        [ProtoMember(3)]
        public string UserName { get; set; }
        [ProtoMember(4)]
        public string UserId { get; set; }
        [ProtoMember(5)]
        public int BeConcernedUserCount { get; set; }
        [ProtoMember(6)]
        public int SingleTreasureCount { get; set; }
        [ProtoMember(7)]
        public decimal CurrProfitRate { get; set; }
        [ProtoMember(8)]
        public bool IsGZ { get; set; }
        [ProtoMember(9)]
        public string SchemeId { get; set; }
    }
}
