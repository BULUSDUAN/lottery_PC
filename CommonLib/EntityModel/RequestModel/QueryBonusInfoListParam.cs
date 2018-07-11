using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.RequestModel
{
    [ProtoContract]
    [Serializable]
   public class QueryBonusInfoListParam:Page
    {
        [ProtoMember(1)]
        public string userId { get; set; }
        [ProtoMember(2)]
        public string gameCode { get; set; }
        [ProtoMember(3)]
        public string gameType { get; set; }
        [ProtoMember(4)]
        public string issuseNumber { get; set; }
        [ProtoMember(5)]
        public int completeData { get; set; }
        [ProtoMember(6)]
        public string key { get; set; }
        [ProtoMember(7)]
        public string UserToken { get; set; }
    }
}
