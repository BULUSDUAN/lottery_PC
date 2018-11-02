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
    [Entity("JXSSC_2X_ZuXZS",Type = EntityType.Table)]
    public class JXSSC_2X_ZuXZS
    { 
        public JXSSC_2X_ZuXZS()
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
            [Field("DX_Proportion")]
            public string DX_Proportion{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(7)]
            [Field("JO_Proportion")]
            public string JO_Proportion{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(8)]
            [Field("ZH_Proportion")]
            public string ZH_Proportion{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(9)]
            [Field("Type")]
            public string Type{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(10)]
            [Field("Red_0")]
            public int Red_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(11)]
            [Field("Red_1")]
            public int Red_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(12)]
            [Field("Red_2")]
            public int Red_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(13)]
            [Field("Red_3")]
            public int Red_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(14)]
            [Field("Red_4")]
            public int Red_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(15)]
            [Field("Red_5")]
            public int Red_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(16)]
            [Field("Red_6")]
            public int Red_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(17)]
            [Field("Red_7")]
            public int Red_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(18)]
            [Field("Red_8")]
            public int Red_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(19)]
            [Field("Red_9")]
            public int Red_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(20)]
            [Field("RedCan_0")]
            public int RedCan_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(21)]
            [Field("RedCan_1")]
            public int RedCan_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(22)]
            [Field("RedCan_2")]
            public int RedCan_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(23)]
            [Field("RedCan_3")]
            public int RedCan_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(24)]
            [Field("RedCan_4")]
            public int RedCan_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(25)]
            [Field("RedCan_5")]
            public int RedCan_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(26)]
            [Field("RedCan_6")]
            public int RedCan_6{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(27)]
            [Field("RedCan_7")]
            public int RedCan_7{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(28)]
            [Field("RedCan_8")]
            public int RedCan_8{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(29)]
            [Field("RedCan_9")]
            public int RedCan_9{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(30)]
            [Field("Type_DZ")]
            public int Type_DZ{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(31)]
            [Field("O_DX_Proportion20")]
            public int O_DX_Proportion20{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(32)]
            [Field("O_DX_Proportion11")]
            public int O_DX_Proportion11{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(33)]
            [Field("O_DX_Proportion02")]
            public int O_DX_Proportion02{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(34)]
            [Field("O_JO_Proportion20")]
            public int O_JO_Proportion20{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(35)]
            [Field("O_JO_Proportion11")]
            public int O_JO_Proportion11{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(36)]
            [Field("O_JO_Proportion02")]
            public int O_JO_Proportion02{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(37)]
            [Field("O_ZH_Proportion20")]
            public int O_ZH_Proportion20{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(38)]
            [Field("O_ZH_Proportion11")]
            public int O_ZH_Proportion11{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(39)]
            [Field("O_ZH_Proportion02")]
            public int O_ZH_Proportion02{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(40)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}