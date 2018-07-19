using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;
namespace EntityModel.CoreModel
{
    [ProtoContract]
   public class ConcernedInfo
    {
        public ConcernedInfo()
        {
            NearTimeProfitRateList = new List<NearTimeProfitRateInfo>();
        }
        [ProtoMember(1)]
        public string UserId { get; set; }
        [ProtoMember(2)]
        public string UserName { get; set; }
        [ProtoMember(3)]
        public int BeConcernedUserCount { get; set; }
        [ProtoMember(4)]
        public int ConcernedUserCount { get; set; }
        [ProtoMember(5)]
        public int SingleTreasureCount { get; set; }
        [ProtoMember(6)]
        public bool IsGZ { get; set; }
        [ProtoMember(7)]
        public int RankNumber { get; set; }
        [ProtoMember(8)]
        public List<NearTimeProfitRateInfo> NearTimeProfitRateList { get; set; }
        [ProtoMember(9)]
        public string FileCreateTime { get; set; }
    }
}
