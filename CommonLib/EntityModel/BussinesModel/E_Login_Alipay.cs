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
    [Entity("E_Login_Alipay",Type = EntityType.Table)]
    public class E_Login_Alipay
    { 
        public E_Login_Alipay()
        {
        
        }
            //// <summary>
            // 用户编号
            ////</summary>
            [ProtoMember(1)]
            [Field("UserId", IsIdenty = false, IsPrimaryKey = true)]
            public string UserId{ get; set; }
            //// <summary>
            // 用户注册信息编号
            ////</summary>
            [ProtoMember(2)]
            [Field("RegisterId")]
            public string RegisterId{ get; set; }
            //// <summary>
            // 用户状态
            ////</summary>
            [ProtoMember(3)]
            [Field("UserStatus")]
            public string UserStatus{ get; set; }
            //// <summary>
            // 用户编号
            ////</summary>
            [ProtoMember(4)]
            [Field("LoginName")]
            public string LoginName{ get; set; }
            //// <summary>
            // 开放编号
            ////</summary>
            [ProtoMember(5)]
            [Field("OpenId")]
            public string OpenId{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(6)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}