﻿using KaSon.FrameWork.Services.Attribute;
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
    [Entity("HD15X5_CH",Type = EntityType.Table)]
    public class HD15X5_CH
    { 
        public HD15X5_CH()
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
            [Field("JO_Proportion")]
            public string JO_Proportion{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(5)]
            [Field("DX_Proportion")]
            public string DX_Proportion{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(6)]
            [Field("HeZhi")]
            public int HeZhi{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(7)]
            [Field("Red_01")]
            public int Red_01{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(8)]
            [Field("Red_02")]
            public int Red_02{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(9)]
            [Field("Red_03")]
            public int Red_03{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(10)]
            [Field("Red_04")]
            public int Red_04{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(11)]
            [Field("Red_05")]
            public int Red_05{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(12)]
            [Field("Red_06")]
            public int Red_06{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(13)]
            [Field("Red_07")]
            public int Red_07{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(14)]
            [Field("Red_08")]
            public int Red_08{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(15)]
            [Field("Red_09")]
            public int Red_09{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(16)]
            [Field("Red_10")]
            public int Red_10{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(17)]
            [Field("Red_11")]
            public int Red_11{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(18)]
            [Field("Red_12")]
            public int Red_12{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(19)]
            [Field("Red_13")]
            public int Red_13{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(20)]
            [Field("Red_14")]
            public int Red_14{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(21)]
            [Field("Red_15")]
            public int Red_15{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(22)]
            [Field("Duplicate")]
            public int Duplicate{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(23)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}