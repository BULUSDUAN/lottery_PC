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
    [Entity("SDKLPK3_JBZS",Type = EntityType.Table)]
    public class SDKLPK3_JBZS
    { 
        public SDKLPK3_JBZS()
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
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(5)]
            [Field("D1_1")]
            public int D1_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(6)]
            [Field("D1_2")]
            public int D1_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(7)]
            [Field("D1_3")]
            public int D1_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(8)]
            [Field("D1_4")]
            public int D1_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(9)]
            [Field("D1_5")]
            public int D1_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(10)]
            [Field("D1_6")]
            public int D1_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(11)]
            [Field("D1_7")]
            public int D1_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(12)]
            [Field("D1_8")]
            public int D1_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(13)]
            [Field("D1_9")]
            public int D1_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(14)]
            [Field("D1_10")]
            public int D1_10{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(15)]
            [Field("D1_11")]
            public int D1_11{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(16)]
            [Field("D1_12")]
            public int D1_12{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(17)]
            [Field("D1_13")]
            public int D1_13{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(18)]
            [Field("D2_1")]
            public int D2_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(19)]
            [Field("D2_2")]
            public int D2_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(20)]
            [Field("D2_3")]
            public int D2_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(21)]
            [Field("D2_4")]
            public int D2_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(22)]
            [Field("D2_5")]
            public int D2_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(23)]
            [Field("D2_6")]
            public int D2_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(24)]
            [Field("D2_7")]
            public int D2_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(25)]
            [Field("D2_8")]
            public int D2_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(26)]
            [Field("D2_9")]
            public int D2_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(27)]
            [Field("D2_10")]
            public int D2_10{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(28)]
            [Field("D2_11")]
            public int D2_11{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(29)]
            [Field("D2_12")]
            public int D2_12{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(30)]
            [Field("D2_13")]
            public int D2_13{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(31)]
            [Field("D3_1")]
            public int D3_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(32)]
            [Field("D3_2")]
            public int D3_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(33)]
            [Field("D3_3")]
            public int D3_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(34)]
            [Field("D3_4")]
            public int D3_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(35)]
            [Field("D3_5")]
            public int D3_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(36)]
            [Field("D3_6")]
            public int D3_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(37)]
            [Field("D3_7")]
            public int D3_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(38)]
            [Field("D3_8")]
            public int D3_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(39)]
            [Field("D3_9")]
            public int D3_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(40)]
            [Field("D3_10")]
            public int D3_10{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(41)]
            [Field("D3_11")]
            public int D3_11{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(42)]
            [Field("D3_12")]
            public int D3_12{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(43)]
            [Field("D3_13")]
            public int D3_13{ get; set; }
    }
}