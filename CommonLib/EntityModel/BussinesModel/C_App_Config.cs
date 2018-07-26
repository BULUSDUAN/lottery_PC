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
    [Entity("C_App_Config",Type = EntityType.Table)]
    public class C_App_Config
    { 
        public C_App_Config()
        {
        
        }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(1)]
            [Field("AppAgentId", IsIdenty = false, IsPrimaryKey = true)]
            public string AppAgentId{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(2)]
            [Field("AgentName")]
            public string AgentName{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(3)]
            [Field("ConfigName")]
            public string ConfigName{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(4)]
            [Field("ConfigVersion")]
            public string ConfigVersion{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(5)]
            [Field("ConfigUpdateContent")]
            public string ConfigUpdateContent{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(6)]
            [Field("ConfigDownloadUrl")]
            public string ConfigDownloadUrl{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(7)]
            [Field("ConfigCode")]
            public string ConfigCode{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(8)]
            [Field("IsForcedUpgrade")]
            public bool? IsForcedUpgrade{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(9)]
            [Field("ConfigExtended")]
            public string ConfigExtended{ get; set; }
    }
}