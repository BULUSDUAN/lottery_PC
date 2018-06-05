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
    [Entity("E_Sitemessage_Banner",Type = EntityType.Table)]
    public class E_Sitemessage_Banner
    { 
        public E_Sitemessage_Banner()
        {
        
        }
            /// <summary>
            // 轮播图编号
            ///</summary>
            [ProtoMember(1)]
            [Field("BannerId", IsIdenty = true, IsPrimaryKey = true)]
            public int BannerId{ get; set; }
            /// <summary>
            // 轮播图序号
            ///</summary>
            [ProtoMember(2)]
            [Field("BannerIndex")]
            public int BannerIndex{ get; set; }
            /// <summary>
            // 轮播图标题
            ///</summary>
            [ProtoMember(3)]
            [Field("BannerTitle")]
            public string BannerTitle{ get; set; }
            /// <summary>
            // 图片路径
            ///</summary>
            [ProtoMember(4)]
            [Field("ImageUrl")]
            public string ImageUrl{ get; set; }
            /// <summary>
            // 轮播图类型
            ///</summary>
            [ProtoMember(5)]
            [Field("BannerType")]
            public int BannerType{ get; set; }
            /// <summary>
            // 跳转路径
            ///</summary>
            [ProtoMember(6)]
            [Field("JumpUrl")]
            public string JumpUrl{ get; set; }
            /// <summary>
            // 是否启动
            ///</summary>
            [ProtoMember(7)]
            [Field("IsEnable")]
            public bool IsEnable{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(8)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}