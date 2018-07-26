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
    [Entity("C_Auth_KeyRule",Type = EntityType.Table)]
    public class C_Auth_KeyRule
    { 
        public C_Auth_KeyRule()
        {
        
        }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(1)]
            [Field("RuleKey", IsIdenty = false, IsPrimaryKey = true)]
            public string RuleKey{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(2)]
            [Field("RuleValue")]
            public string RuleValue{ get; set; }
    }
}