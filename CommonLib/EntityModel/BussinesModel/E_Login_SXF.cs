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
    [Entity("E_Login_SXF",Type = EntityType.Table)]
    public class E_Login_SXF
    { 
        public E_Login_SXF()
        {
        
        }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(1)]
            [Field("UserId", IsIdenty = false, IsPrimaryKey = true)]
            public string UserId{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(2)]
            [Field("RegisterId")]
            public string RegisterId{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(3)]
            [Field("SXFUserNo")]
            public string SXFUserNo{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(4)]
            [Field("DisplayName")]
            public string DisplayName{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(5)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}