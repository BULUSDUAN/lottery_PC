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
        public string desc { get; set; }

        public string name { get; set; }

        public string flag { get; set; }
    }
}
