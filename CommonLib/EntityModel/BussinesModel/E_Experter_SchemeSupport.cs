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
    // 专家方案支持率
    ////</summary>
    [ProtoContract]
    [Entity("E_Experter_SchemeSupport",Type = EntityType.Table)]
    public class E_Experter_SchemeSupport
    { 
        public E_Experter_SchemeSupport()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 方案ID
            ////</summary>
            [ProtoMember(2)]
            [Field("SchemeId")]
            public string SchemeId{ get; set; }
            //// <summary>
            // 支持者用户Id
            ////</summary>
            [ProtoMember(3)]
            [Field("SupportUserId")]
            public string SupportUserId{ get; set; }
            //// <summary>
            // 反对者用户Id
            ////</summary>
            [ProtoMember(4)]
            [Field("AgainstUserId")]
            public string AgainstUserId{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(5)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}