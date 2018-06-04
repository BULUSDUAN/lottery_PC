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
    // 疑问
    ////</summary>
    [ProtoContract]
    [Entity("E_SiteMessage_Doubt_List",Type = EntityType.Table)]
    public class E_SiteMessage_Doubt_List
    { 
        public E_SiteMessage_Doubt_List()
        {
        
        }
            //// <summary>
            // 编号
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = false, IsPrimaryKey = true)]
            public string Id{ get; set; }
            //// <summary>
            // 标题
            ////</summary>
            [ProtoMember(2)]
            [Field("Title")]
            public string Title{ get; set; }
            //// <summary>
            // 描述
            ////</summary>
            [ProtoMember(3)]
            [Field("Description")]
            public string Description{ get; set; }
            //// <summary>
            // 分类
            ////</summary>
            [ProtoMember(4)]
            [Field("Category")]
            public string Category{ get; set; }
            //// <summary>
            // 排序次序
            ////</summary>
            [ProtoMember(5)]
            [Field("ShowIndex")]
            public int? ShowIndex{ get; set; }
            //// <summary>
            // 顶的次数统计
            ////</summary>
            [ProtoMember(6)]
            [Field("UpCount")]
            public int? UpCount{ get; set; }
            //// <summary>
            // 踩的次数统计
            ////</summary>
            [ProtoMember(7)]
            [Field("DownCount")]
            public int? DownCount{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(8)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
            //// <summary>
            // 创建者编号
            ////</summary>
            [ProtoMember(9)]
            [Field("CreateUserKey")]
            public string CreateUserKey{ get; set; }
            //// <summary>
            // 创建者显示名称
            ////</summary>
            [ProtoMember(10)]
            [Field("CreateUserDisplayName")]
            public string CreateUserDisplayName{ get; set; }
            //// <summary>
            // 更新时间
            ////</summary>
            [ProtoMember(11)]
            [Field("UpdateTime")]
            public DateTime? UpdateTime{ get; set; }
            //// <summary>
            // 更新者编号
            ////</summary>
            [ProtoMember(12)]
            [Field("UpdateUserKey")]
            public string UpdateUserKey{ get; set; }
            //// <summary>
            // 更新者显示名称
            ////</summary>
            [ProtoMember(13)]
            [Field("UpdateUserDisplayName")]
            public string UpdateUserDisplayName{ get; set; }
    }
}