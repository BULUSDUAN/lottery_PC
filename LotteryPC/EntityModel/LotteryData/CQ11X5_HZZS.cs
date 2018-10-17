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
    [Entity("CQ11X5_HZZS",Type = EntityType.Table)]
    public class CQ11X5_HZZS
    { 
        public CQ11X5_HZZS()
        {
        
        }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(2)]
            [Field("IssuseNumber")]
            public string IssuseNumber{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(3)]
            [Field("WinNumber")]
            public string WinNumber{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(4)]
            [Field("HeZhi")]
            public int HeZhi{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(5)]
            [Field("HZ_15")]
            public int HZ_15{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(6)]
            [Field("HZ_16")]
            public int HZ_16{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(7)]
            [Field("HZ_17")]
            public int HZ_17{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(8)]
            [Field("HZ_18")]
            public int HZ_18{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(9)]
            [Field("HZ_19")]
            public int HZ_19{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(10)]
            [Field("HZ_20")]
            public int HZ_20{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(11)]
            [Field("HZ_21")]
            public int HZ_21{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(12)]
            [Field("HZ_22")]
            public int HZ_22{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(13)]
            [Field("HZ_23")]
            public int HZ_23{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(14)]
            [Field("HZ_24")]
            public int HZ_24{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(15)]
            [Field("HZ_25")]
            public int HZ_25{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(16)]
            [Field("HZ_26")]
            public int HZ_26{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(17)]
            [Field("HZ_27")]
            public int HZ_27{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(18)]
            [Field("HZ_28")]
            public int HZ_28{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(19)]
            [Field("HZ_29")]
            public int HZ_29{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(20)]
            [Field("HZ_30")]
            public int HZ_30{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(21)]
            [Field("HZ_31")]
            public int HZ_31{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(22)]
            [Field("HZ_32")]
            public int HZ_32{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(23)]
            [Field("HZ_33")]
            public int HZ_33{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(24)]
            [Field("HZ_34")]
            public int HZ_34{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(25)]
            [Field("HZ_35")]
            public int HZ_35{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(26)]
            [Field("HZ_36")]
            public int HZ_36{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(27)]
            [Field("HZ_37")]
            public int HZ_37{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(28)]
            [Field("HZ_38")]
            public int HZ_38{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(29)]
            [Field("HZ_39")]
            public int HZ_39{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(30)]
            [Field("HZ_40")]
            public int HZ_40{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(31)]
            [Field("HZ_41")]
            public int HZ_41{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(32)]
            [Field("HZ_42")]
            public int HZ_42{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(33)]
            [Field("HZ_43")]
            public int HZ_43{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(34)]
            [Field("HZ_44")]
            public int HZ_44{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(35)]
            [Field("HZ_45")]
            public int HZ_45{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(36)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}