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
    // 用户编号规则
    ///</summary>
    [ProtoContract]
    [Entity("C_Auth_KeyRule",Type = EntityType.Table)]
    public class C_Auth_KeyRule
    { 
        public C_Auth_KeyRule()
        {
        
        }
            /// <summary>
            // 规则Key
            ///</summary>
            [ProtoMember(1)]
            [Field("RuleKey", IsIdenty = false, IsPrimaryKey = true)]
            public string RuleKey{ get; set; }
            /// <summary>
            // 规则Value
            ///</summary>
            [ProtoMember(2)]
            [Field("RuleValue")]
            public string RuleValue{ get; set; }
    }
}