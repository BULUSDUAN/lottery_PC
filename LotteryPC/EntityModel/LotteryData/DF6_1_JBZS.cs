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
    [Entity("DF6_1_JBZS",Type = EntityType.Table)]
    public class DF6_1_JBZS
    { 
        public DF6_1_JBZS()
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
            [Field("Red_0")]
            public int Red_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(5)]
            [Field("Red_1")]
            public int Red_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(6)]
            [Field("Red_2")]
            public int Red_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(7)]
            [Field("Red_3")]
            public int Red_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(8)]
            [Field("Red_4")]
            public int Red_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(9)]
            [Field("Red_5")]
            public int Red_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(10)]
            [Field("Red_6")]
            public int Red_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(11)]
            [Field("Red_7")]
            public int Red_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(12)]
            [Field("Red_8")]
            public int Red_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(13)]
            [Field("Red_9")]
            public int Red_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(14)]
            [Field("Red1_0")]
            public int Red1_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(15)]
            [Field("Red1_1")]
            public int Red1_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(16)]
            [Field("Red1_2")]
            public int Red1_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(17)]
            [Field("Red1_3")]
            public int Red1_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(18)]
            [Field("Red1_4")]
            public int Red1_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(19)]
            [Field("Red1_5")]
            public int Red1_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(20)]
            [Field("Red1_6")]
            public int Red1_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(21)]
            [Field("Red1_7")]
            public int Red1_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(22)]
            [Field("Red1_8")]
            public int Red1_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(23)]
            [Field("Red1_9")]
            public int Red1_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(24)]
            [Field("Red2_0")]
            public int Red2_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(25)]
            [Field("Red2_1")]
            public int Red2_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(26)]
            [Field("Red2_2")]
            public int Red2_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(27)]
            [Field("Red2_3")]
            public int Red2_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(28)]
            [Field("Red2_4")]
            public int Red2_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(29)]
            [Field("Red2_5")]
            public int Red2_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(30)]
            [Field("Red2_6")]
            public int Red2_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(31)]
            [Field("Red2_7")]
            public int Red2_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(32)]
            [Field("Red2_8")]
            public int Red2_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(33)]
            [Field("Red2_9")]
            public int Red2_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(34)]
            [Field("Red3_0")]
            public int Red3_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(35)]
            [Field("Red3_1")]
            public int Red3_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(36)]
            [Field("Red3_2")]
            public int Red3_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(37)]
            [Field("Red3_3")]
            public int Red3_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(38)]
            [Field("Red3_4")]
            public int Red3_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(39)]
            [Field("Red3_5")]
            public int Red3_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(40)]
            [Field("Red3_6")]
            public int Red3_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(41)]
            [Field("Red3_7")]
            public int Red3_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(42)]
            [Field("Red3_8")]
            public int Red3_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(43)]
            [Field("Red3_9")]
            public int Red3_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(44)]
            [Field("Red4_0")]
            public int Red4_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(45)]
            [Field("Red4_1")]
            public int Red4_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(46)]
            [Field("Red4_2")]
            public int Red4_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(47)]
            [Field("Red4_3")]
            public int Red4_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(48)]
            [Field("Red4_4")]
            public int Red4_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(49)]
            [Field("Red4_5")]
            public int Red4_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(50)]
            [Field("Red4_6")]
            public int Red4_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(51)]
            [Field("Red4_7")]
            public int Red4_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(52)]
            [Field("Red4_8")]
            public int Red4_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(53)]
            [Field("Red4_9")]
            public int Red4_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(54)]
            [Field("Red5_0")]
            public int Red5_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(55)]
            [Field("Red5_1")]
            public int Red5_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(56)]
            [Field("Red5_2")]
            public int Red5_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(57)]
            [Field("Red5_3")]
            public int Red5_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(58)]
            [Field("Red5_4")]
            public int Red5_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(59)]
            [Field("Red5_5")]
            public int Red5_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(60)]
            [Field("Red5_6")]
            public int Red5_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(61)]
            [Field("Red5_7")]
            public int Red5_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(62)]
            [Field("Red5_8")]
            public int Red5_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(63)]
            [Field("Red5_9")]
            public int Red5_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(64)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}