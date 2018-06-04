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
    [Entity("E_Login_Local_test",Type = EntityType.Table)]
    public class E_Login_Local_test
    { 
        public E_Login_Local_test()
        {
        
        }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(1)]
            [Field("UserId", IsIdenty = false, IsPrimaryKey = true)]
            public string UserId{ get; set; }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(2)]
            [Field("RegisterId")]
            public string RegisterId{ get; set; }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(3)]
            [Field("LoginName")]
            public string LoginName{ get; set; }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(4)]
            [Field("Password")]
            public string Password{ get; set; }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(5)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(6)]
            [Field("mobile")]
            public string mobile{ get; set; }
    }
}