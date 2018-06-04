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
    // 配置APP嵌套地址
    ////</summary>
    [ProtoContract]
    [Entity("C_APP_NestedUrlConfig",Type = EntityType.Table)]
    public class C_APP_NestedUrlConfig
    { 
        public C_APP_NestedUrlConfig()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // Key值
            ////</summary>
            [ProtoMember(2)]
            [Field("ConfigKey")]
            public string ConfigKey{ get; set; }
            //// <summary>
            // 嵌套地址
            ////</summary>
            [ProtoMember(3)]
            [Field("Url")]
            public string Url{ get; set; }
            //// <summary>
            // 备注
            ////</summary>
            [ProtoMember(4)]
            [Field("Remarks")]
            public string Remarks{ get; set; }
            //// <summary>
            // 类型
            ////</summary>
            [ProtoMember(5)]
            [Field("UrlType")]
            public int? UrlType{ get; set; }
            //// <summary>
            // 是否启用
            ////</summary>
            [ProtoMember(6)]
            [Field("IsEnable")]
            public bool? IsEnable{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(7)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}