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
    // 最新动态
    ////</summary>
    [ProtoContract]
    [Entity("E_Blog_Dynamic",Type = EntityType.Table)]
    public class E_Blog_Dynamic
    { 
        public E_Blog_Dynamic()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 用户编号
            ////</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            //// <summary>
            // 显示名称
            ////</summary>
            [ProtoMember(3)]
            [Field("UserDisplayName")]
            public string UserDisplayName{ get; set; }
            //// <summary>
            // 被参与用户
            ////</summary>
            [ProtoMember(4)]
            [Field("UserId2")]
            public string UserId2{ get; set; }
            //// <summary>
            // UserId2显示名称
            ////</summary>
            [ProtoMember(5)]
            [Field("User2DisplayName")]
            public string User2DisplayName{ get; set; }
            //// <summary>
            // 彩种
            ////</summary>
            [ProtoMember(6)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            //// <summary>
            // 玩法
            ////</summary>
            [ProtoMember(7)]
            [Field("GameType")]
            public string GameType{ get; set; }
            //// <summary>
            // 期号
            ////</summary>
            [ProtoMember(8)]
            [Field("IssuseNumber")]
            public string IssuseNumber{ get; set; }
            //// <summary>
            // 用户动作
            ////</summary>
            [ProtoMember(9)]
            [Field("DynamicType")]
            public string DynamicType{ get; set; }
            //// <summary>
            // 方案号
            ////</summary>
            [ProtoMember(10)]
            [Field("SchemeId")]
            public string SchemeId{ get; set; }
            //// <summary>
            // 总金额
            ////</summary>
            [ProtoMember(11)]
            [Field("TotalMonery")]
            public decimal? TotalMonery{ get; set; }
            //// <summary>
            // 每份单价
            ////</summary>
            [ProtoMember(12)]
            [Field("Price")]
            public decimal? Price{ get; set; }
            //// <summary>
            // 发起人保底份数
            ////</summary>
            [ProtoMember(13)]
            [Field("Guarantees")]
            public int? Guarantees{ get; set; }
            //// <summary>
            // 发起人认购份数
            ////</summary>
            [ProtoMember(14)]
            [Field("Subscription")]
            public int? Subscription{ get; set; }
            //// <summary>
            // 方案进度百分比
            ////</summary>
            [ProtoMember(15)]
            [Field("Progress")]
            public decimal? Progress{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(16)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}