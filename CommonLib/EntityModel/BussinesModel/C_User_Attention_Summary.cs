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
    // 用户关注汇总
    ///</summary>
    [ProtoContract]
    [Entity("C_User_Attention_Summary",Type = EntityType.Table)]
    public class C_User_Attention_Summary
    { 
        public C_User_Attention_Summary()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 用户编号
            ///</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            /// <summary>
            // 被关注人数
            ///</summary>
            [ProtoMember(3)]
            [Field("BeAttentionUserCount")]
            public int BeAttentionUserCount{ get; set; }
            /// <summary>
            // 已关注人数
            ///</summary>
            [ProtoMember(4)]
            [Field("FollowerUserCount")]
            public int FollowerUserCount{ get; set; }
            /// <summary>
            // 修改时间
            ///</summary>
            [ProtoMember(5)]
            [Field("UpdateTime")]
            public DateTime UpdateTime{ get; set; }
    }
}