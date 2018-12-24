using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace EntityModel
{
    /// <summary>
    // 
    ///</summary>
    [ProtoContract]
    [Entity("blast_data_time",Type = EntityType.Table)]
    public class xingblast_data_time
    { 
        public xingblast_data_time()
        {
        
        }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(1)]
            [Field("id", IsIdenty = true, IsPrimaryKey = true)]
            public int id{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(2)]
            [Field("type")]
            public sbyte typeid{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(3)]
            [Field("actionNo")]
            public string actionNo{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(4)]
            [Field("actionTime")]
            public string actionTime { get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(5)]
            [Field("stopTime")]
            public string stopTime { get; set; }

    
    }
}