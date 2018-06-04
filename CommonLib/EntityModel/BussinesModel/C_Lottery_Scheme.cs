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
    [Entity("C_Lottery_Scheme",Type = EntityType.Table)]
    public class C_Lottery_Scheme
    { 
        public C_Lottery_Scheme()
        {
        
        }
            //// <summary>
            // 编号
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 关联值
            ////</summary>
            [ProtoMember(2)]
            [Field("KeyLine")]
            public string KeyLine{ get; set; }
            //// <summary>
            // 方案号
            ////</summary>
            [ProtoMember(3)]
            [Field("SchemeId")]
            public string SchemeId{ get; set; }
            //// <summary>
            // 期号
            ////</summary>
            [ProtoMember(4)]
            [Field("IssuseNumber")]
            public string IssuseNumber{ get; set; }
            //// <summary>
            // 序号
            ////</summary>
            [ProtoMember(5)]
            [Field("OrderIndex")]
            public int? OrderIndex{ get; set; }
            //// <summary>
            // 是否完成
            ////</summary>
            [ProtoMember(6)]
            [Field("IsComplate")]
            public bool? IsComplate{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(7)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}