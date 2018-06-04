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
    // APP配置
    ////</summary>
    [ProtoContract]
    [Entity("C_App_Config",Type = EntityType.Table)]
    public class C_App_Config
    { 
        public C_App_Config()
        {
        
        }
            //// <summary>
            // 代理商编码
            ////</summary>
            [ProtoMember(1)]
            [Field("AppAgentId", IsIdenty = false, IsPrimaryKey = true)]
            public string AppAgentId{ get; set; }
            //// <summary>
            // 代理商名称
            ////</summary>
            [ProtoMember(2)]
            [Field("AgentName")]
            public string AgentName{ get; set; }
            //// <summary>
            // 配置名称
            ////</summary>
            [ProtoMember(3)]
            [Field("ConfigName")]
            public string ConfigName{ get; set; }
            //// <summary>
            // 版本号
            ////</summary>
            [ProtoMember(4)]
            [Field("ConfigVersion")]
            public string ConfigVersion{ get; set; }
            //// <summary>
            // 升级内容
            ////</summary>
            [ProtoMember(5)]
            [Field("ConfigUpdateContent")]
            public string ConfigUpdateContent{ get; set; }
            //// <summary>
            // 升级地址
            ////</summary>
            [ProtoMember(6)]
            [Field("ConfigDownloadUrl")]
            public string ConfigDownloadUrl{ get; set; }
            //// <summary>
            // 升级编码
            ////</summary>
            [ProtoMember(7)]
            [Field("ConfigCode")]
            public string ConfigCode{ get; set; }
            //// <summary>
            // 是否自动升级
            ////</summary>
            [ProtoMember(8)]
            [Field("IsForcedUpgrade")]
            public bool? IsForcedUpgrade{ get; set; }
            //// <summary>
            // 扩展字段
            ////</summary>
            [ProtoMember(9)]
            [Field("ConfigExtended")]
            public string ConfigExtended{ get; set; }
    }
}