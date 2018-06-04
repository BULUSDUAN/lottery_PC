using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace EntityModel
{
    //// <summary>
    // 
    ////</summary>
    [ProtoContract]
    [Entity("C_Auth_RoleFunction",Type = EntityType.Table)]
    public class C_Auth_RoleFunction
    { 
        public C_Auth_RoleFunction()
        {
        
        }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(1)]
            [Field("IId", IsIdenty = true, IsPrimaryKey = true)]
            public int IId{ get; set; }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(2)]
            [Field("RoleId")]
            public string RoleId{ get; set; }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(3)]
            [Field("FunctionId")]
            public string FunctionId{ get; set; }
            //// <summary>
            // 允许/禁止
            ////</summary>
            [ProtoMember(4)]
            [Field("Status")]
            public int? Status{ get; set; }
            //// <summary>
            // R:读；W:写
            ////</summary>
            [ProtoMember(5)]
            [Field("Mode")]
            public string Mode{ get; set; }
    }
}