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
    // 站内信息模板
    ///</summary>
    [ProtoContract]
    [Entity("E_SiteMessage_SiteMessageTemplate",Type = EntityType.Table)]
    public class E_SiteMessage_SiteMessageTemplate
    { 
        public E_SiteMessage_SiteMessageTemplate()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 模板标题
            ///</summary>
            [ProtoMember(2)]
            [Field("MsgTitle")]
            public string MsgTitle{ get; set; }
            /// <summary>
            // 模板内容
            ///</summary>
            [ProtoMember(3)]
            [Field("MsgContent")]
            public string MsgContent{ get; set; }
            /// <summary>
            // 模板参数,参数间以|分隔
            ///</summary>
            [ProtoMember(4)]
            [Field("MsgParams")]
            public string MsgParams{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(5)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}