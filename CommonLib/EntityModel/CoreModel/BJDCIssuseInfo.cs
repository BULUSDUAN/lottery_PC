using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
    [ProtoContract]
    public class BJDCIssuseInfo
    {
        public BJDCIssuseInfo()
        { }
        [ProtoMember(1)]
        public string IssuseNumber { get; set; }
        [ProtoMember(2)]
        public string MinLocalStopTime { get; set; }
        [ProtoMember(3)]
        public string MinMatchStartTime { get; set; }
    }
}
