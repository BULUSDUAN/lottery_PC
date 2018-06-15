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
    [Entity("QXC_JBZS",Type = EntityType.Table)]
    public class QXC_JBZS
    { 
        public QXC_JBZS()
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
            [Field("NO1_0")]
            public int NO1_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(5)]
            [Field("NO1_1")]
            public int NO1_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(6)]
            [Field("NO1_2")]
            public int NO1_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(7)]
            [Field("NO1_3")]
            public int NO1_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(8)]
            [Field("NO1_4")]
            public int NO1_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(9)]
            [Field("NO1_5")]
            public int NO1_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(10)]
            [Field("NO1_6")]
            public int NO1_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(11)]
            [Field("NO1_7")]
            public int NO1_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(12)]
            [Field("NO1_8")]
            public int NO1_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(13)]
            [Field("NO1_9")]
            public int NO1_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(14)]
            [Field("NO2_0")]
            public int NO2_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(15)]
            [Field("NO2_1")]
            public int NO2_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(16)]
            [Field("NO2_2")]
            public int NO2_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(17)]
            [Field("NO2_3")]
            public int NO2_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(18)]
            [Field("NO2_4")]
            public int NO2_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(19)]
            [Field("NO2_5")]
            public int NO2_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(20)]
            [Field("NO2_6")]
            public int NO2_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(21)]
            [Field("NO2_7")]
            public int NO2_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(22)]
            [Field("NO2_8")]
            public int NO2_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(23)]
            [Field("NO2_9")]
            public int NO2_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(24)]
            [Field("NO3_0")]
            public int NO3_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(25)]
            [Field("NO3_1")]
            public int NO3_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(26)]
            [Field("NO3_2")]
            public int NO3_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(27)]
            [Field("NO3_3")]
            public int NO3_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(28)]
            [Field("NO3_4")]
            public int NO3_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(29)]
            [Field("NO3_5")]
            public int NO3_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(30)]
            [Field("NO3_6")]
            public int NO3_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(31)]
            [Field("NO3_7")]
            public int NO3_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(32)]
            [Field("NO3_8")]
            public int NO3_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(33)]
            [Field("NO3_9")]
            public int NO3_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(34)]
            [Field("NO4_0")]
            public int NO4_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(35)]
            [Field("NO4_1")]
            public int NO4_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(36)]
            [Field("NO4_2")]
            public int NO4_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(37)]
            [Field("NO4_3")]
            public int NO4_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(38)]
            [Field("NO4_4")]
            public int NO4_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(39)]
            [Field("NO4_5")]
            public int NO4_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(40)]
            [Field("NO4_6")]
            public int NO4_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(41)]
            [Field("NO4_7")]
            public int NO4_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(42)]
            [Field("NO4_8")]
            public int NO4_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(43)]
            [Field("NO4_9")]
            public int NO4_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(44)]
            [Field("NO5_0")]
            public int NO5_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(45)]
            [Field("NO5_1")]
            public int NO5_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(46)]
            [Field("NO5_2")]
            public int NO5_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(47)]
            [Field("NO5_3")]
            public int NO5_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(48)]
            [Field("NO5_4")]
            public int NO5_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(49)]
            [Field("NO5_5")]
            public int NO5_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(50)]
            [Field("NO5_6")]
            public int NO5_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(51)]
            [Field("NO5_7")]
            public int NO5_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(52)]
            [Field("NO5_8")]
            public int NO5_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(53)]
            [Field("NO5_9")]
            public int NO5_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(54)]
            [Field("NO6_0")]
            public int NO6_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(55)]
            [Field("NO6_1")]
            public int NO6_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(56)]
            [Field("NO6_2")]
            public int NO6_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(57)]
            [Field("NO6_3")]
            public int NO6_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(58)]
            [Field("NO6_4")]
            public int NO6_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(59)]
            [Field("NO6_5")]
            public int NO6_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(60)]
            [Field("NO6_6")]
            public int NO6_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(61)]
            [Field("NO6_7")]
            public int NO6_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(62)]
            [Field("NO6_8")]
            public int NO6_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(63)]
            [Field("NO6_9")]
            public int NO6_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(64)]
            [Field("NO7_0")]
            public int NO7_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(65)]
            [Field("NO7_1")]
            public int NO7_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(66)]
            [Field("NO7_2")]
            public int NO7_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(67)]
            [Field("NO7_3")]
            public int NO7_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(68)]
            [Field("NO7_4")]
            public int NO7_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(69)]
            [Field("NO7_5")]
            public int NO7_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(70)]
            [Field("NO7_6")]
            public int NO7_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(71)]
            [Field("NO7_7")]
            public int NO7_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(72)]
            [Field("NO7_8")]
            public int NO7_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(73)]
            [Field("NO7_9")]
            public int NO7_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(74)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}