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
    // 访客历史记录
    ////</summary>
    [ProtoContract]
    [Entity("E_Blog_UserVisitHistory",Type = EntityType.Table)]
    public class E_Blog_UserVisitHistory
    { 
        public E_Blog_UserVisitHistory()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 用户编号
            ////</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            //// <summary>
            // 访客用户编号
            ////</summary>
            [ProtoMember(3)]
            [Field("VisitUserId")]
            public string VisitUserId{ get; set; }
            //// <summary>
            // 访客用户名称
            ////</summary>
            [ProtoMember(4)]
            [Field("VisitorUserDisplayName")]
            public string VisitorUserDisplayName{ get; set; }
            //// <summary>
            // 用户名匿名位数
            ////</summary>
            [ProtoMember(5)]
            [Field("VisitorHideNameCount")]
            public string VisitorHideNameCount{ get; set; }
            //// <summary>
            // 登陆IP
            ////</summary>
            [ProtoMember(6)]
            [Field("VisitorIp")]
            public string VisitorIp{ get; set; }
            //// <summary>
            // IP显示地址
            ////</summary>
            [ProtoMember(7)]
            [Field("IpDisplayName")]
            public string IpDisplayName{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(8)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}