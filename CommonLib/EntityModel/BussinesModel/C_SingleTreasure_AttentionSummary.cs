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
    [Entity("C_SingleTreasure_AttentionSummary",Type = EntityType.Table)]
    public class C_SingleTreasure_AttentionSummary
    { 
        public C_SingleTreasure_AttentionSummary()
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
            // 被关注总数
            ///</summary>
            [ProtoMember(3)]
            [Field("ConcernedUserCount")]
            public int ConcernedUserCount{ get; set; }
            /// <summary>
            // 关注总数
            ///</summary>
            [ProtoMember(4)]
            [Field("BeConcernedUserCount")]
            public int BeConcernedUserCount{ get; set; }
            /// <summary>
            // 晒单总数
            ///</summary>
            [ProtoMember(5)]
            [Field("SingleTreasureCount")]
            public int SingleTreasureCount{ get; set; }
            /// <summary>
            // 修改时间
            ///</summary>
            [ProtoMember(6)]
            [Field("UpdateTime")]
            public DateTime UpdateTime{ get; set; }
    }
}