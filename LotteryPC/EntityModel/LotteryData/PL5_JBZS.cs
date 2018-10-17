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
    [Entity("PL5_JBZS",Type = EntityType.Table)]
    public class PL5_JBZS
    { 
        public PL5_JBZS()
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
            [Field("W_0")]
            public int W_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(5)]
            [Field("W_1")]
            public int W_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(6)]
            [Field("W_2")]
            public int W_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(7)]
            [Field("W_3")]
            public int W_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(8)]
            [Field("W_4")]
            public int W_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(9)]
            [Field("W_5")]
            public int W_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(10)]
            [Field("W_6")]
            public int W_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(11)]
            [Field("W_7")]
            public int W_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(12)]
            [Field("W_8")]
            public int W_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(13)]
            [Field("W_9")]
            public int W_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(14)]
            [Field("Q_0")]
            public int Q_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(15)]
            [Field("Q_1")]
            public int Q_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(16)]
            [Field("Q_2")]
            public int Q_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(17)]
            [Field("Q_3")]
            public int Q_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(18)]
            [Field("Q_4")]
            public int Q_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(19)]
            [Field("Q_5")]
            public int Q_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(20)]
            [Field("Q_6")]
            public int Q_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(21)]
            [Field("Q_7")]
            public int Q_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(22)]
            [Field("Q_8")]
            public int Q_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(23)]
            [Field("Q_9")]
            public int Q_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(24)]
            [Field("B_0")]
            public int B_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(25)]
            [Field("B_1")]
            public int B_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(26)]
            [Field("B_2")]
            public int B_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(27)]
            [Field("B_3")]
            public int B_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(28)]
            [Field("B_4")]
            public int B_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(29)]
            [Field("B_5")]
            public int B_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(30)]
            [Field("B_6")]
            public int B_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(31)]
            [Field("B_7")]
            public int B_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(32)]
            [Field("B_8")]
            public int B_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(33)]
            [Field("B_9")]
            public int B_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(34)]
            [Field("S_0")]
            public int S_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(35)]
            [Field("S_1")]
            public int S_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(36)]
            [Field("S_2")]
            public int S_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(37)]
            [Field("S_3")]
            public int S_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(38)]
            [Field("S_4")]
            public int S_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(39)]
            [Field("S_5")]
            public int S_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(40)]
            [Field("S_6")]
            public int S_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(41)]
            [Field("S_7")]
            public int S_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(42)]
            [Field("S_8")]
            public int S_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(43)]
            [Field("S_9")]
            public int S_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(44)]
            [Field("G_0")]
            public int G_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(45)]
            [Field("G_1")]
            public int G_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(46)]
            [Field("G_2")]
            public int G_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(47)]
            [Field("G_3")]
            public int G_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(48)]
            [Field("G_4")]
            public int G_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(49)]
            [Field("G_5")]
            public int G_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(50)]
            [Field("G_6")]
            public int G_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(51)]
            [Field("G_7")]
            public int G_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(52)]
            [Field("G_8")]
            public int G_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(53)]
            [Field("G_9")]
            public int G_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(54)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}