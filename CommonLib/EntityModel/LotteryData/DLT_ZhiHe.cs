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
    [Entity("DLT_ZhiHe",Type = EntityType.Table)]
    public class DLT_ZhiHe
    { 
        public DLT_ZhiHe()
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
            [Field("QianQu")]
            public string QianQu{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(5)]
            [Field("ZhiHeQualifying")]
            public string ZhiHeQualifying{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(6)]
            [Field("ZhiHeBi")]
            public string ZhiHeBi{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(7)]
            [Field("NO1Z")]
            public int NO1Z{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(8)]
            [Field("NO1H")]
            public int NO1H{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(9)]
            [Field("NO2Z")]
            public int NO2Z{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(10)]
            [Field("NO2H")]
            public int NO2H{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(11)]
            [Field("NO3Z")]
            public int NO3Z{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(12)]
            [Field("NO3H")]
            public int NO3H{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(13)]
            [Field("NO4Z")]
            public int NO4Z{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(14)]
            [Field("NO4H")]
            public int NO4H{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(15)]
            [Field("NO5Z")]
            public int NO5Z{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(16)]
            [Field("NO5H")]
            public int NO5H{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(17)]
            [Field("Bi0_5")]
            public int Bi0_5{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(18)]
            [Field("Bi1_4")]
            public int Bi1_4{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(19)]
            [Field("Bi2_3")]
            public int Bi2_3{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(20)]
            [Field("Bi3_2")]
            public int Bi3_2{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(21)]
            [Field("Bi4_1")]
            public int Bi4_1{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(22)]
            [Field("Bi5_0")]
            public int Bi5_0{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(23)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}