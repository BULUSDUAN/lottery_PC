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
    // 活动价配置
    ////</summary>
    [ProtoContract]
    [Entity("C_Activity_PrizeConfig",Type = EntityType.Table)]
    public class C_Activity_PrizeConfig
    { 
        public C_Activity_PrizeConfig()
        {
        
        }
            //// <summary>
            // 活动编号
            ////</summary>
            [ProtoMember(1)]
            [Field("ActivityId", IsIdenty = true, IsPrimaryKey = true)]
            public int ActivityId{ get; set; }
            //// <summary>
            // 活动标题
            ////</summary>
            [ProtoMember(2)]
            [Field("ActivityTitle")]
            public string ActivityTitle{ get; set; }
            //// <summary>
            // 活动内容
            ////</summary>
            [ProtoMember(3)]
            [Field("ActivityContent")]
            public string ActivityContent{ get; set; }
            //// <summary>
            // 是否启用
            ////</summary>
            [ProtoMember(4)]
            [Field("IsEnabled")]
            public bool? IsEnabled{ get; set; }
            //// <summary>
            // 创建编号
            ////</summary>
            [ProtoMember(5)]
            [Field("CreatorId")]
            public string CreatorId{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(6)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}