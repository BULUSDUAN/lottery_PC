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
    // 用户关注
    ///</summary>
    [ProtoContract]
    [Entity("C_User_Attention",Type = EntityType.Table)]
    public class C_User_Attention
    { 
        public C_User_Attention()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 被关注人编号
            ///</summary>
            [ProtoMember(2)]
            [Field("BeAttentionUserId")]
            public string BeAttentionUserId{ get; set; }
            /// <summary>
            // 关注人编号(粉丝)
            ///</summary>
            [ProtoMember(3)]
            [Field("FollowerUserId")]
            public string FollowerUserId{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(4)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}