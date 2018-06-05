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
    [Entity("E_Login_Local",Type = EntityType.Table)]
    public class E_Login_Local
    { 
        public E_Login_Local()
        {
        
        }
            /// <summary>
            // 用户编号
            ///</summary>
            [ProtoMember(1)]
            [Field("UserId", IsIdenty = false, IsPrimaryKey = true)]
            public string UserId{ get; set; }
            /// <summary>
            // 用户注册信息编号
            ///</summary>
            [ProtoMember(2)]
            [Field("RegisterId")]
            public string RegisterId{ get; set; }
            /// <summary>
            // 登录名
            ///</summary>
            [ProtoMember(3)]
            [Field("LoginName")]
            public string LoginName{ get; set; }
            /// <summary>
            // 登录密码
            ///</summary>
            [ProtoMember(4)]
            [Field("Password")]
            public string Password{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(5)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
            /// <summary>
            // 手机号
            ///</summary>
            [ProtoMember(6)]
            [Field("mobile")]
            public string mobile{ get; set; }
    }
}