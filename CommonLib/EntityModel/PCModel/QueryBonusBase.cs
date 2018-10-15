using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel
{
    [ProtoContract]
    public class QueryBonusBase
    {
        [ProtoMember(1)]
        public DateTime fromDate { get; set; }
        [ProtoMember(2)]
        public DateTime toDate { get; set; }
        [ProtoMember(3)]
        public string gameCode { get; set; }
        [ProtoMember(4)]
        public string gameType { get; set; }
        [ProtoMember(5)]
        public int pageIndex { get; set; }
        [ProtoMember(6)]
        public int pageSize { get; set; }
    }
}
