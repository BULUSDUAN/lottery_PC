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
    // 注册信息
    ///</summary>
    [ProtoContract]
    [Entity("C_User_Register",Type = EntityType.Table)]
    public class C_User_Register
    { 
        public C_User_Register()
        {
        
        }
            /// <summary>
            // 用户编号
            ///</summary>
            [ProtoMember(1)]
            [Field("UserId", IsIdenty = false, IsPrimaryKey = true)]
            public string UserId{ get; set; }
            /// <summary>
            // VIP等级
            ///</summary>
            [ProtoMember(2)]
            [Field("VipLevel")]
            public int VipLevel{ get; set; }
            /// <summary>
            // 显示名称
            ///</summary>
            [ProtoMember(3)]
            [Field("DisplayName")]
            public string DisplayName{ get; set; }
            /// <summary>
            // 用户名称来源
            ///</summary>
            [ProtoMember(4)]
            [Field("ComeFrom")]
            public string ComeFrom{ get; set; }
            /// <summary>
            // 注册IP
            ///</summary>
            [ProtoMember(5)]
            [Field("RegisterIp")]
            public string RegisterIp{ get; set; }
            /// <summary>
            // 注册引用页面
            ///</summary>
            [ProtoMember(6)]
            [Field("ReferrerUrl")]
            public string ReferrerUrl{ get; set; }
            /// <summary>
            // 注册引用
            ///</summary>
            [ProtoMember(7)]
            [Field("Referrer")]
            public string Referrer{ get; set; }
            /// <summary>
            // 注册类型
            ///</summary>
            [ProtoMember(8)]
            [Field("RegType")]
            public string RegType{ get; set; }
            /// <summary>
            // 是否充值
            ///</summary>
            [ProtoMember(9)]
            [Field("IsFillMoney")]
            public bool IsFillMoney{ get; set; }
            /// <summary>
            // 是否启用
            ///</summary>
            [ProtoMember(10)]
            [Field("IsEnable")]
            public bool IsEnable{ get; set; }
            /// <summary>
            // 是否代理商
            ///</summary>
            [ProtoMember(11)]
            [Field("IsAgent")]
            public bool IsAgent{ get; set; }
            /// <summary>
            // 隐藏用户名数
            ///</summary>
            [ProtoMember(12)]
            [Field("HideDisplayNameCount")]
            public int HideDisplayNameCount{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(13)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
            /// <summary>
            // 代理商编号
            ///</summary>
            [ProtoMember(14)]
            [Field("AgentId")]
            public string AgentId{ get; set; }
            /// <summary>
            // 是否同意报告
            ///</summary>
            [ProtoMember(15)]
            [Field("IsIgnoreReport")]
            public bool IsIgnoreReport{ get; set; }
            /// <summary>
            // 父级路径
            ///</summary>
            [ProtoMember(16)]
            [Field("ParentPath")]
            public string ParentPath{ get; set; }
            /// <summary>
            // 用户类别:0:网站普通用户；1：内部员工用户；
            ///</summary>
            [ProtoMember(17)]
            [Field("UserType")]
            public int UserType{ get; set; }
           [ProtoMember(18)]
           [Field("UserCreditType")]
           public int UserCreditType { get; set; }
    }
}