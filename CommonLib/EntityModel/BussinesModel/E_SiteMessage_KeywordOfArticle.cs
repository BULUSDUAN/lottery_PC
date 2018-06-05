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
    // 内容关键字
    ///</summary>
    [ProtoContract]
    [Entity("E_SiteMessage_KeywordOfArticle",Type = EntityType.Table)]
    public class E_SiteMessage_KeywordOfArticle
    { 
        public E_SiteMessage_KeywordOfArticle()
        {
        
        }
            /// <summary>
            // 编号
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 关键字
            ///</summary>
            [ProtoMember(2)]
            [Field("KeyWords")]
            public string KeyWords{ get; set; }
            /// <summary>
            // 链接
            ///</summary>
            [ProtoMember(3)]
            [Field("Link")]
            public string Link{ get; set; }
            /// <summary>
            // 是否启用
            ///</summary>
            [ProtoMember(4)]
            [Field("IsEnable")]
            public bool IsEnable{ get; set; }
    }
}