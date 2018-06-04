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
    // 用户登陆历史
    ////</summary>
    [ProtoContract]
    [Entity("E_Blog_UserLoginHistory",Type = EntityType.Table)]
    public class E_Blog_UserLoginHistory
    { 
        public E_Blog_UserLoginHistory()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 用户Id
            ////</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            //// <summary>
            // 登陆来至那个通道
            ////</summary>
            [ProtoMember(3)]
            [Field("LoginFrom")]
            public string LoginFrom{ get; set; }
            //// <summary>
            // IP
            ////</summary>
            [ProtoMember(4)]
            [Field("LoginIp")]
            public string LoginIp{ get; set; }
            //// <summary>
            // 地址
            ////</summary>
            [ProtoMember(5)]
            [Field("IpDisplayName")]
            public string IpDisplayName{ get; set; }
            //// <summary>
            // 时间
            ////</summary>
            [ProtoMember(6)]
            [Field("LoginTime")]
            public DateTime? LoginTime{ get; set; }
    }
}