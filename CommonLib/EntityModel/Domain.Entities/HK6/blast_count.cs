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
    [Entity("blast_count",Type = EntityType.Table)]
    public class blast_count
    { 
        public blast_count()
        {
        
        }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(1)]
            [Field("id", IsIdenty = true, IsPrimaryKey = true)]
            public int id{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(2)]
            [Field("typeid")]
            public byte typeid{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(3)]
            [Field("playedId")]
            public short playedId{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(4)]
            [Field("createdate")]
            public DateTime createdate{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(5)]
            [Field("betCount")]
            public int betCount{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(6)]
            [Field("betAmount")]
            public decimal betAmount{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(7)]
            [Field("zjAmount")]
            public decimal zjAmount{ get; set; }
    }
}