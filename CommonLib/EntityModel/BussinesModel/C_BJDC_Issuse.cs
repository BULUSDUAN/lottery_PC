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
    [Entity("C_BJDC_Issuse",Type = EntityType.Table)]
    public class C_BJDC_Issuse
    { 
        public C_BJDC_Issuse()
        {
        
        }
            /// <summary>
            // 编号
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 期号
            ///</summary>
            [ProtoMember(2)]
            [Field("IssuseNumber")]
            public string IssuseNumber{ get; set; }
            /// <summary>
            // 本地最小结束时间
            ///</summary>
            [ProtoMember(3)]
            [Field("MinLocalStopTime")]
            public DateTime MinLocalStopTime{ get; set; }
            /// <summary>
            // 最小开始时间
            ///</summary>
            [ProtoMember(4)]
            [Field("MinMatchStartTime")]
            public DateTime MinMatchStartTime{ get; set; }
    }
}