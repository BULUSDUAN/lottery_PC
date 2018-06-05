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
    // 站内信息标签
    ///</summary>
    [ProtoContract]
    [Entity("E_SiteMessage_SiteMessageTags",Type = EntityType.Table)]
    public class E_SiteMessage_SiteMessageTags
    { 
        public E_SiteMessage_SiteMessageTags()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 标签KEY
            ///</summary>
            [ProtoMember(2)]
            [Field("TagKey")]
            public string TagKey{ get; set; }
            /// <summary>
            // 标签名称
            ///</summary>
            [ProtoMember(3)]
            [Field("TagName")]
            public string TagName{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(4)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}