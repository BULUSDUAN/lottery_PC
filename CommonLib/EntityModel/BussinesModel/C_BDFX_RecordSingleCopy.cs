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
    [Entity("C_BDFX_RecordSingleCopy",Type = EntityType.Table)]
    public class C_BDFX_RecordSingleCopy
    { 
        public C_BDFX_RecordSingleCopy()
        {
        
        }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(2)]
            [Field("BDXFSchemeId")]
            public string BDXFSchemeId{ get; set; }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(3)]
            [Field("SingleCopySchemeId")]
            public string SingleCopySchemeId{ get; set; }
            //// <summary>
            // 
            ////</summary>
            [ProtoMember(4)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}