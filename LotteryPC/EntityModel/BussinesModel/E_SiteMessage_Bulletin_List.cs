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
    // 公告
    ///</summary>
    [ProtoContract]
    [Entity("E_SiteMessage_Bulletin_List",Type = EntityType.Table)]
    public class E_SiteMessage_Bulletin_List
    { 
        public E_SiteMessage_Bulletin_List()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 标题
            ///</summary>
            [ProtoMember(2)]
            [Field("Title")]
            public string Title{ get; set; }
            /// <summary>
            // 内容
            ///</summary>
            [ProtoMember(3)]
            [Field("Content")]
            public string Content{ get; set; }
            /// <summary>
            // 公告代理商
            ///</summary>
            [ProtoMember(4)]
            [Field("BulletinAgent")]
            public int BulletinAgent{ get; set; }
            /// <summary>
            // 公告状态：正常，取消
            ///</summary>
            [ProtoMember(5)]
            [Field("Status")]
            public int Status{ get; set; }
            /// <summary>
            // 有效期从。如为null表示及时启用
            ///</summary>
            [ProtoMember(6)]
            [Field("EffectiveFrom")]
            public DateTime EffectiveFrom{ get; set; }
            /// <summary>
            // 有效期至。如为null表示不过期
            ///</summary>
            [ProtoMember(7)]
            [Field("EffectiveTo")]
            public DateTime EffectiveTo{ get; set; }
            /// <summary>
            // 优先级别（数字越小，表示级别越高）
            ///</summary>
            [ProtoMember(8)]
            [Field("Priority")]
            public int Priority{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(9)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
            /// <summary>
            // 创建者
            ///</summary>
            [ProtoMember(10)]
            [Field("CreateBy")]
            public string CreateBy{ get; set; }
            /// <summary>
            // 最后修改时间
            ///</summary>
            [ProtoMember(11)]
            [Field("UpdateTime")]
            public DateTime UpdateTime{ get; set; }
            /// <summary>
            // 最后修改者
            ///</summary>
            [ProtoMember(12)]
            [Field("UpdateBy")]
            public string UpdateBy{ get; set; }
            /// <summary>
            // 是否置顶
            ///</summary>
            [ProtoMember(13)]
            [Field("IsPutTop")]
            public int IsPutTop{ get; set; }
    }
}