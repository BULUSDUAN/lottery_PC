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
    [Entity("CQKLSF_QJZS",Type = EntityType.Table)]
    public class CQKLSF_QJZS
    { 
        public CQKLSF_QJZS()
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
            [Field("QJ_1_5_0")]
            public int QJ_1_5_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(5)]
            [Field("QJ_1_5_1")]
            public int QJ_1_5_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(6)]
            [Field("QJ_1_5_2")]
            public int QJ_1_5_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(7)]
            [Field("QJ_1_5_3")]
            public int QJ_1_5_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(8)]
            [Field("QJ_1_5_4")]
            public int QJ_1_5_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(9)]
            [Field("QJ_1_5_5")]
            public int QJ_1_5_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(10)]
            [Field("QJ_6_10_0")]
            public int QJ_6_10_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(11)]
            [Field("QJ_6_10_1")]
            public int QJ_6_10_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(12)]
            [Field("QJ_6_10_2")]
            public int QJ_6_10_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(13)]
            [Field("QJ_6_10_3")]
            public int QJ_6_10_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(14)]
            [Field("QJ_6_10_4")]
            public int QJ_6_10_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(15)]
            [Field("QJ_6_10_5")]
            public int QJ_6_10_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(16)]
            [Field("QJ_11_15_0")]
            public int QJ_11_15_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(17)]
            [Field("QJ_11_15_1")]
            public int QJ_11_15_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(18)]
            [Field("QJ_11_15_2")]
            public int QJ_11_15_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(19)]
            [Field("QJ_11_15_3")]
            public int QJ_11_15_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(20)]
            [Field("QJ_11_15_4")]
            public int QJ_11_15_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(21)]
            [Field("QJ_11_15_5")]
            public int QJ_11_15_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(22)]
            [Field("QJ_16_20_0")]
            public int QJ_16_20_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(23)]
            [Field("QJ_16_20_1")]
            public int QJ_16_20_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(24)]
            [Field("QJ_16_20_2")]
            public int QJ_16_20_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(25)]
            [Field("QJ_16_20_3")]
            public int QJ_16_20_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(26)]
            [Field("QJ_16_20_4")]
            public int QJ_16_20_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(27)]
            [Field("QJ_16_20_5")]
            public int QJ_16_20_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(28)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}