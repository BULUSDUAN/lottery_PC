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
    [Entity("C_Auth_Function_List",Type = EntityType.Table)]
    public class C_Auth_Function_List
    { 
        public C_Auth_Function_List()
        {
        
        }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(1)]
            [Field("FunctionId", IsIdenty = false, IsPrimaryKey = true)]
            public string FunctionId{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(2)]
            [Field("IsWebBasic")]
            public bool? IsWebBasic{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(3)]
            [Field("IsBackBasic")]
            public bool? IsBackBasic{ get; set; }
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
            [Field("ParentId")]
            public string ParentId{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(6)]
            [Field("ParentPath")]
            public string ParentPath{ get; set; }
    }
}