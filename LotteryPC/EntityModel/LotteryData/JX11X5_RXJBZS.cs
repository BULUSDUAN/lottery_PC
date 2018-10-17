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
    [Entity("JX11X5_RXJBZS",Type = EntityType.Table)]
    public class JX11X5_RXJBZS
    { 
        public JX11X5_RXJBZS()
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
            [Field("RXJB_01")]
            public int RXJB_01{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(5)]
            [Field("RXJB_02")]
            public int RXJB_02{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(6)]
            [Field("RXJB_03")]
            public int RXJB_03{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(7)]
            [Field("RXJB_04")]
            public int RXJB_04{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(8)]
            [Field("RXJB_05")]
            public int RXJB_05{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(9)]
            [Field("RXJB_06")]
            public int RXJB_06{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(10)]
            [Field("RXJB_07")]
            public int RXJB_07{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(11)]
            [Field("RXJB_08")]
            public int RXJB_08{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(12)]
            [Field("RXJB_09")]
            public int RXJB_09{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(13)]
            [Field("RXJB_10")]
            public int RXJB_10{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(14)]
            [Field("RXJB_11")]
            public int RXJB_11{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(15)]
            [Field("J_0")]
            public int J_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(16)]
            [Field("J_1")]
            public int J_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(17)]
            [Field("J_2")]
            public int J_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(18)]
            [Field("J_3")]
            public int J_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(19)]
            [Field("J_4")]
            public int J_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(20)]
            [Field("J_5")]
            public int J_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(21)]
            [Field("X_0")]
            public int X_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(22)]
            [Field("X_1")]
            public int X_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(23)]
            [Field("X_2")]
            public int X_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(24)]
            [Field("X_3")]
            public int X_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(25)]
            [Field("X_4")]
            public int X_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(26)]
            [Field("X_5")]
            public int X_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(27)]
            [Field("Z_0")]
            public int Z_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(28)]
            [Field("Z_1")]
            public int Z_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(29)]
            [Field("Z_2")]
            public int Z_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(30)]
            [Field("Z_3")]
            public int Z_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(31)]
            [Field("Z_4")]
            public int Z_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(32)]
            [Field("Z_5")]
            public int Z_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(33)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}