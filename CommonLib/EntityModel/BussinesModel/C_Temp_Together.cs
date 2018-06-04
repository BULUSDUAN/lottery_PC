using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace EntityModel
{
    //// <summary>
    // 
    ////</summary>
    [ProtoContract]
    [Entity("C_Temp_Together",Type = EntityType.Table)]
    public class C_Temp_Together
    { 
        public C_Temp_Together()
        {
        
        }
            //// <summary>
            // 方案编号
            ////</summary>
            [ProtoMember(1)]
            [Field("SchemeId", IsIdenty = false, IsPrimaryKey = true)]
            public string SchemeId{ get; set; }
            //// <summary>
            // 彩种代码
            ////</summary>
            [ProtoMember(2)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            //// <summary>
            // 停止时间
            ////</summary>
            [ProtoMember(3)]
            [Field("StopTime")]
            public string StopTime{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(4)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}