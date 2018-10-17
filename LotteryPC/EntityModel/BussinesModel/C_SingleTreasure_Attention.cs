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
    [Entity("C_SingleTreasure_Attention",Type = EntityType.Table)]
    public class C_SingleTreasure_Attention
    { 
        public C_SingleTreasure_Attention()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 被关注用户编号
            ///</summary>
            [ProtoMember(2)]
            [Field("ConcernedUserId")]
            public string ConcernedUserId{ get; set; }
            /// <summary>
            // 关注者用户编号
            ///</summary>
            [ProtoMember(3)]
            [Field("BeConcernedUserId")]
            public string BeConcernedUserId{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(4)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}