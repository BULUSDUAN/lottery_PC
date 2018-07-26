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
    [Entity("C_Auth_BeautyKey",Type = EntityType.Table)]
    public class C_Auth_BeautyKey
    { 
        public C_Auth_BeautyKey()
        {
        
        }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(1)]
            [Field("BeautyKey", IsIdenty = false, IsPrimaryKey = true)]
            public string BeautyKey{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(2)]
            [Field("PrevUserKey")]
            public string PrevUserKey{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(3)]
            [Field("NextUserKey")]
            public string NextUserKey{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(4)]
            [Field("Status")]
            public string Status{ get; set; }
    }
}