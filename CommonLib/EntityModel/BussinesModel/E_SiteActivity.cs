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
    [Entity("E_SiteActivity",Type = EntityType.Table)]
    public class E_SiteActivity
    { 
        public E_SiteActivity()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 图片Url
            ///</summary>
            [ProtoMember(2)]
            [Field("ImageUrl")]
            public string ImageUrl{ get; set; }
            /// <summary>
            // 文章Url
            ///</summary>
            [ProtoMember(3)]
            [Field("ArticleUrl")]
            public string ArticleUrl{ get; set; }
            /// <summary>
            // 活动标题
            ///</summary>
            [ProtoMember(4)]
            [Field("Titile")]
            public string Titile{ get; set; }
            /// <summary>
            // 活动开始时间
            ///</summary>
            [ProtoMember(5)]
            [Field("StartTime")]
            public DateTime StartTime{ get; set; }
            /// <summary>
            // 活动结束时间
            ///</summary>
            [ProtoMember(6)]
            [Field("EndTime")]
            public DateTime EndTime{ get; set; }
    }
}