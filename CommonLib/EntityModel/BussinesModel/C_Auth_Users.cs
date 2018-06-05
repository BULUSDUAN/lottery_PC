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
    [Entity("C_Auth_Users",Type = EntityType.Table)]
    public class C_Auth_Users
    { 
        public C_Auth_Users()
        {
        
        }
            /// <summary>
            // 帐号唯一标识
            ///</summary>
            [ProtoMember(1)]
            [Field("UserId", IsIdenty = false, IsPrimaryKey = true)]
            public string UserId{ get; set; }
            /// <summary>
            // 注册来源
            ///</summary>
            [ProtoMember(2)]
            [Field("RegFrom")]
            public string RegFrom{ get; set; }
            /// <summary>
            // 状态
            ///</summary>
            [ProtoMember(3)]
            [Field("Status")]
            public int Status{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(4)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
            /// <summary>
            // 代理商编号
            ///</summary>
            [ProtoMember(5)]
            [Field("AgentId")]
            public string AgentId{ get; set; }
    }
}