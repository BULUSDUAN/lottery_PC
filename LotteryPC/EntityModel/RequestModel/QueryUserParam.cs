using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.RequestModel
{

    [ProtoContract]
    public class QueryUserParam
    {
        [ProtoMember(1)]
        public string loginName { get; set; }

        [ProtoMember(2)]
        public string password { get; set; }
        [ProtoMember(3)]
        public string IPAddress { get; set; }
    }
}
