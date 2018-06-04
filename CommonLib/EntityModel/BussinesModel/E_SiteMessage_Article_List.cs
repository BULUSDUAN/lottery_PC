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
    // 文章
    ////</summary>
    [ProtoContract]
    [Entity("E_SiteMessage_Article_List",Type = EntityType.Table)]
    public class E_SiteMessage_Article_List
    { 
        public E_SiteMessage_Article_List()
        {
        
        }
            //// <summary>
            // 编号
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = false, IsPrimaryKey = true)]
            public string Id{ get; set; }
            //// <summary>
            // 彩种
            ////</summary>
            [ProtoMember(2)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            //// <summary>
            // 标题
            ////</summary>
            [ProtoMember(3)]
            [Field("Title")]
            public string Title{ get; set; }
            //// <summary>
            // 内容
            ////</summary>
            [ProtoMember(4)]
            [Field("Description")]
            public string Description{ get; set; }
            //// <summary>
            // 关键字
            ////</summary>
            [ProtoMember(5)]
            [Field("KeyWords")]
            public string KeyWords{ get; set; }
            //// <summary>
            // 描述
            ////</summary>
            [ProtoMember(6)]
            [Field("DescContent")]
            public string DescContent{ get; set; }
            //// <summary>
            // 是否标红
            ////</summary>
            [ProtoMember(7)]
            [Field("IsRedTitle")]
            public bool? IsRedTitle{ get; set; }
            //// <summary>
            // 分类
            ////</summary>
            [ProtoMember(8)]
            [Field("Category")]
            public string Category{ get; set; }
            //// <summary>
            // 排序次序
            ////</summary>
            [ProtoMember(9)]
            [Field("ShowIndex")]
            public int? ShowIndex{ get; set; }
            //// <summary>
            // 阅读次数统计
            ////</summary>
            [ProtoMember(10)]
            [Field("ReadCount")]
            public int? ReadCount{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(11)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
            //// <summary>
            // 创建者编号
            ////</summary>
            [ProtoMember(12)]
            [Field("CreateUserKey")]
            public string CreateUserKey{ get; set; }
            //// <summary>
            // 创建者显示名称
            ////</summary>
            [ProtoMember(13)]
            [Field("CreateUserDisplayName")]
            public string CreateUserDisplayName{ get; set; }
            //// <summary>
            // 更新时间
            ////</summary>
            [ProtoMember(14)]
            [Field("UpdateTime")]
            public DateTime? UpdateTime{ get; set; }
            //// <summary>
            // 更新者编号
            ////</summary>
            [ProtoMember(15)]
            [Field("UpdateUserKey")]
            public string UpdateUserKey{ get; set; }
            //// <summary>
            // 更新者显示名称
            ////</summary>
            [ProtoMember(16)]
            [Field("UpdateUserDisplayName")]
            public string UpdateUserDisplayName{ get; set; }
            //// <summary>
            // 静态文件地址
            ////</summary>
            [ProtoMember(17)]
            [Field("StaticPath")]
            public string StaticPath{ get; set; }
            //// <summary>
            // 上一条编号
            ////</summary>
            [ProtoMember(18)]
            [Field("PreId")]
            public string PreId{ get; set; }
            //// <summary>
            // 上一条标题
            ////</summary>
            [ProtoMember(19)]
            [Field("PreTitle")]
            public string PreTitle{ get; set; }
            //// <summary>
            // 上一条静态路径
            ////</summary>
            [ProtoMember(20)]
            [Field("PreStaticPath")]
            public string PreStaticPath{ get; set; }
            //// <summary>
            // 下一条编号
            ////</summary>
            [ProtoMember(21)]
            [Field("NextId")]
            public string NextId{ get; set; }
            //// <summary>
            // 下一条标题
            ////</summary>
            [ProtoMember(22)]
            [Field("NextTitle")]
            public string NextTitle{ get; set; }
            //// <summary>
            // 下一条静态路径
            ////</summary>
            [ProtoMember(23)]
            [Field("NextStaticPath")]
            public string NextStaticPath{ get; set; }
    }
}