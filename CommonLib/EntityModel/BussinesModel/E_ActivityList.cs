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
    // 活动列表
    ///</summary>
    [ProtoContract]
    [Entity("E_ActivityList",Type = EntityType.Table)]
    public class E_ActivityList
    { 
        public E_ActivityList()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 活动Id
            ///</summary>
            [ProtoMember(2)]
            [Field("ActivityIndex")]
            public int ActivityIndex{ get; set; }
            /// <summary>
            // 活动名称
            ///</summary>
            [ProtoMember(3)]
            [Field("ActiveName")]
            public string ActiveName{ get; set; }
            /// <summary>
            // 活动标题
            ///</summary>
            [ProtoMember(4)]
            [Field("Title")]
            public string Title{ get; set; }
            /// <summary>
            // 活动描述
            ///</summary>
            [ProtoMember(5)]
            [Field("Summary")]
            public string Summary{ get; set; }
            /// <summary>
            // 活动地址
            ///</summary>
            [ProtoMember(6)]
            [Field("LinkUrl")]
            public string LinkUrl{ get; set; }
            /// <summary>
            // 图片地址
            ///</summary>
            [ProtoMember(7)]
            [Field("ImageUrl")]
            public string ImageUrl{ get; set; }
            /// <summary>
            // 是否显示
            ///</summary>
            [ProtoMember(8)]
            [Field("IsShow")]
            public bool IsShow{ get; set; }
            /// <summary>
            // 活动开始时间
            ///</summary>
            [ProtoMember(9)]
            [Field("BeginTime")]
            public DateTime BeginTime{ get; set; }
            /// <summary>
            // 活动结束时间
            ///</summary>
            [ProtoMember(10)]
            [Field("EndTime")]
            public DateTime EndTime{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(11)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}