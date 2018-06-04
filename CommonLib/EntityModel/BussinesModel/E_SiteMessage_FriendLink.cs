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
    // 友情链接,热点链接
    ////</summary>
    [ProtoContract]
    [Entity("E_SiteMessage_FriendLink",Type = EntityType.Table)]
    public class E_SiteMessage_FriendLink
    { 
        public E_SiteMessage_FriendLink()
        {
        
        }
            //// <summary>
            // 编号
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 友情链接排序
            ////</summary>
            [ProtoMember(2)]
            [Field("IndexLink")]
            public int? IndexLink{ get; set; }
            //// <summary>
            // 网站名称
            ////</summary>
            [ProtoMember(3)]
            [Field("InnerText")]
            public string InnerText{ get; set; }
            //// <summary>
            // 链接地址
            ////</summary>
            [ProtoMember(4)]
            [Field("LinkUrl")]
            public string LinkUrl{ get; set; }
            //// <summary>
            // 是否是友情链接
            ////</summary>
            [ProtoMember(5)]
            [Field("Isfriendship")]
            public bool? Isfriendship{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(6)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}