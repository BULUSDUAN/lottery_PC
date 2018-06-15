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
    [Entity("JXSSC_5X_JBZS",Type = EntityType.Table)]
    public class JXSSC_5X_JBZS
    { 
        public JXSSC_5X_JBZS()
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
            [Field("RedBall1")]
            public string RedBall1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(5)]
            [Field("RedBall2")]
            public string RedBall2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(6)]
            [Field("RedBall3")]
            public string RedBall3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(7)]
            [Field("RedBall4")]
            public string RedBall4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(8)]
            [Field("RedBall5")]
            public string RedBall5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(9)]
            [Field("DX_Proportion")]
            public string DX_Proportion{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(10)]
            [Field("JO_Proportion")]
            public string JO_Proportion{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(11)]
            [Field("Red_W0")]
            public int Red_W0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(12)]
            [Field("Red_W1")]
            public int Red_W1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(13)]
            [Field("Red_W2")]
            public int Red_W2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(14)]
            [Field("Red_W3")]
            public int Red_W3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(15)]
            [Field("Red_W4")]
            public int Red_W4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(16)]
            [Field("Red_W5")]
            public int Red_W5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(17)]
            [Field("Red_W6")]
            public int Red_W6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(18)]
            [Field("Red_W7")]
            public int Red_W7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(19)]
            [Field("Red_W8")]
            public int Red_W8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(20)]
            [Field("Red_W9")]
            public int Red_W9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(21)]
            [Field("Red_Q0")]
            public int Red_Q0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(22)]
            [Field("Red_Q1")]
            public int Red_Q1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(23)]
            [Field("Red_Q2")]
            public int Red_Q2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(24)]
            [Field("Red_Q3")]
            public int Red_Q3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(25)]
            [Field("Red_Q4")]
            public int Red_Q4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(26)]
            [Field("Red_Q5")]
            public int Red_Q5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(27)]
            [Field("Red_Q6")]
            public int Red_Q6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(28)]
            [Field("Red_Q7")]
            public int Red_Q7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(29)]
            [Field("Red_Q8")]
            public int Red_Q8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(30)]
            [Field("Red_Q9")]
            public int Red_Q9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(31)]
            [Field("Red_B0")]
            public int Red_B0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(32)]
            [Field("Red_B1")]
            public int Red_B1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(33)]
            [Field("Red_B2")]
            public int Red_B2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(34)]
            [Field("Red_B3")]
            public int Red_B3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(35)]
            [Field("Red_B4")]
            public int Red_B4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(36)]
            [Field("Red_B5")]
            public int Red_B5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(37)]
            [Field("Red_B6")]
            public int Red_B6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(38)]
            [Field("Red_B7")]
            public int Red_B7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(39)]
            [Field("Red_B8")]
            public int Red_B8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(40)]
            [Field("Red_B9")]
            public int Red_B9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(41)]
            [Field("Red_S0")]
            public int Red_S0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(42)]
            [Field("Red_S1")]
            public int Red_S1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(43)]
            [Field("Red_S2")]
            public int Red_S2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(44)]
            [Field("Red_S3")]
            public int Red_S3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(45)]
            [Field("Red_S4")]
            public int Red_S4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(46)]
            [Field("Red_S5")]
            public int Red_S5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(47)]
            [Field("Red_S6")]
            public int Red_S6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(48)]
            [Field("Red_S7")]
            public int Red_S7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(49)]
            [Field("Red_S8")]
            public int Red_S8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(50)]
            [Field("Red_S9")]
            public int Red_S9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(51)]
            [Field("Red_G0")]
            public int Red_G0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(52)]
            [Field("Red_G1")]
            public int Red_G1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(53)]
            [Field("Red_G2")]
            public int Red_G2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(54)]
            [Field("Red_G3")]
            public int Red_G3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(55)]
            [Field("Red_G4")]
            public int Red_G4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(56)]
            [Field("Red_G5")]
            public int Red_G5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(57)]
            [Field("Red_G6")]
            public int Red_G6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(58)]
            [Field("Red_G7")]
            public int Red_G7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(59)]
            [Field("Red_G8")]
            public int Red_G8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(60)]
            [Field("Red_G9")]
            public int Red_G9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(61)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}