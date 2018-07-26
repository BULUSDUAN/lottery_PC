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
    [Entity("E_Activity_Config",Type = EntityType.Table)]
    public class E_Activity_Config
    { 
        public E_Activity_Config()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 配置KEY
            ///</summary>
            [ProtoMember(2)]
            [Field("ConfigKey")]
            public string ConfigKey{ get; set; }
            /// <summary>
            // 配置名称
            ///</summary>
            [ProtoMember(3)]
            [Field("ConfigName")]
            public string ConfigName{ get; set; }
            /// <summary>
            // 配置值
            ///</summary>
            [ProtoMember(4)]
            [Field("ConfigValue")]
            public string ConfigValue{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(5)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}