using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
    [ProtoContract]
    [Serializable]
    public class APP_Advertising
    {
        [ProtoMember(1)]
        public string desc { get; set; }
        [ProtoMember(2)]
        public string name { get; set; }
        [ProtoMember(3)]
        public string flag { get; set; }
    }
}
