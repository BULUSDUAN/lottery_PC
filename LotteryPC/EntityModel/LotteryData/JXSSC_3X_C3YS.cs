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
    [Entity("JXSSC_3X_C3YS",Type = EntityType.Table)]
    public class JXSSC_3X_C3YS
    { 
        public JXSSC_3X_C3YS()
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
            [Field("O_Red1_0")]
            public int O_Red1_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(8)]
            [Field("O_Red1_1")]
            public int O_Red1_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(9)]
            [Field("O_Red1_2")]
            public int O_Red1_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(10)]
            [Field("O_Red2_0")]
            public int O_Red2_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(11)]
            [Field("O_Red2_1")]
            public int O_Red2_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(12)]
            [Field("O_Red2_2")]
            public int O_Red2_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(13)]
            [Field("O_Red3_0")]
            public int O_Red3_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(14)]
            [Field("O_Red3_1")]
            public int O_Red3_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(15)]
            [Field("O_Red3_2")]
            public int O_Red3_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(16)]
            [Field("Y0_Number")]
            public int Y0_Number{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(17)]
            [Field("Y1_Number")]
            public int Y1_Number{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(18)]
            [Field("Y2_Number")]
            public int Y2_Number{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(19)]
            [Field("Y0_Number0")]
            public int Y0_Number0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(20)]
            [Field("Y0_Number1")]
            public int Y0_Number1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(21)]
            [Field("Y0_Number2")]
            public int Y0_Number2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(22)]
            [Field("Y0_Number3")]
            public int Y0_Number3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(23)]
            [Field("Y1_Number0")]
            public int Y1_Number0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(24)]
            [Field("Y1_Number1")]
            public int Y1_Number1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(25)]
            [Field("Y1_Number2")]
            public int Y1_Number2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(26)]
            [Field("Y1_Number3")]
            public int Y1_Number3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(27)]
            [Field("Y2_Number0")]
            public int Y2_Number0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(28)]
            [Field("Y2_Number1")]
            public int Y2_Number1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(29)]
            [Field("Y2_Number2")]
            public int Y2_Number2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(30)]
            [Field("Y2_Number3")]
            public int Y2_Number3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(31)]
            [Field("YS_Proportion")]
            public string YS_Proportion{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(32)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}