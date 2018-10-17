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
    [Entity("PL3_JOHM",Type = EntityType.Table)]
    public class PL3_JOHM
    { 
        public PL3_JOHM()
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
            [Field("BW_0")]
            public int BW_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(5)]
            [Field("BW_1")]
            public int BW_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(6)]
            [Field("BW_2")]
            public int BW_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(7)]
            [Field("BW_3")]
            public int BW_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(8)]
            [Field("BW_4")]
            public int BW_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(9)]
            [Field("BW_5")]
            public int BW_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(10)]
            [Field("BW_6")]
            public int BW_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(11)]
            [Field("BW_7")]
            public int BW_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(12)]
            [Field("BW_8")]
            public int BW_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(13)]
            [Field("BW_9")]
            public int BW_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(14)]
            [Field("SW_0")]
            public int SW_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(15)]
            [Field("SW_1")]
            public int SW_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(16)]
            [Field("SW_2")]
            public int SW_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(17)]
            [Field("SW_3")]
            public int SW_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(18)]
            [Field("SW_4")]
            public int SW_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(19)]
            [Field("SW_5")]
            public int SW_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(20)]
            [Field("SW_6")]
            public int SW_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(21)]
            [Field("SW_7")]
            public int SW_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(22)]
            [Field("SW_8")]
            public int SW_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(23)]
            [Field("SW_9")]
            public int SW_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(24)]
            [Field("GW_0")]
            public int GW_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(25)]
            [Field("GW_1")]
            public int GW_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(26)]
            [Field("GW_2")]
            public int GW_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(27)]
            [Field("GW_3")]
            public int GW_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(28)]
            [Field("GW_4")]
            public int GW_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(29)]
            [Field("GW_5")]
            public int GW_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(30)]
            [Field("GW_6")]
            public int GW_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(31)]
            [Field("GW_7")]
            public int GW_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(32)]
            [Field("GW_8")]
            public int GW_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(33)]
            [Field("GW_9")]
            public int GW_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(34)]
            [Field("JOBi")]
            public string JOBi{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(35)]
            [Field("Bi3_0")]
            public int Bi3_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(36)]
            [Field("Bi2_1")]
            public int Bi2_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(37)]
            [Field("Bi1_2")]
            public int Bi1_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(38)]
            [Field("Bi0_3")]
            public int Bi0_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(39)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}