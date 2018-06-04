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
    // 
    ////</summary>
    [ProtoContract]
    [Entity("E_SiteMessage_UserIdea_List",Type = EntityType.Table)]
    public class E_SiteMessage_UserIdea_List
    { 
        public E_SiteMessage_UserIdea_List()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 描述
            ////</summary>
            [ProtoMember(2)]
            [Field("Description")]
            public string Description{ get; set; }
            //// <summary>
            // 分类
            ////</summary>
            [ProtoMember(3)]
            [Field("Category")]
            public string Category{ get; set; }
            //// <summary>
            // 状态
            ////</summary>
            [ProtoMember(4)]
            [Field("Status")]
            public string Status{ get; set; }
            //// <summary>
            // 是否匿名用户
            ////</summary>
            [ProtoMember(5)]
            [Field("IsAnonymous")]
            public bool? IsAnonymous{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(6)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
            //// <summary>
            // 创建者编号
            ////</summary>
            [ProtoMember(7)]
            [Field("CreateUserId")]
            public string CreateUserId{ get; set; }
            //// <summary>
            // 创建者显示名称
            ////</summary>
            [ProtoMember(8)]
            [Field("CreateUserDisplayName")]
            public string CreateUserDisplayName{ get; set; }
            //// <summary>
            // 创建者手机
            ////</summary>
            [ProtoMember(9)]
            [Field("CreateUserMoibile")]
            public string CreateUserMoibile{ get; set; }
            //// <summary>
            // 更新时间
            ////</summary>
            [ProtoMember(10)]
            [Field("UpdateTime")]
            public DateTime? UpdateTime{ get; set; }
            //// <summary>
            // 更新者编号
            ////</summary>
            [ProtoMember(11)]
            [Field("UpdateUserId")]
            public string UpdateUserId{ get; set; }
            //// <summary>
            // 更新者显示名称
            ////</summary>
            [ProtoMember(12)]
            [Field("UpdateUserDisplayName")]
            public string UpdateUserDisplayName{ get; set; }
            //// <summary>
            // 页面打开速度
            ////</summary>
            [ProtoMember(13)]
            [Field("PageOpenSpeed")]
            public decimal? PageOpenSpeed{ get; set; }
            //// <summary>
            // 界面设计美观
            ////</summary>
            [ProtoMember(14)]
            [Field("InterfaceBeautiful")]
            public decimal? InterfaceBeautiful{ get; set; }
            //// <summary>
            // 界面设计美观
            ////</summary>
            [ProtoMember(15)]
            [Field("ComposingReasonable")]
            public decimal? ComposingReasonable{ get; set; }
            //// <summary>
            // 操作过程合理
            ////</summary>
            [ProtoMember(16)]
            [Field("OperationReasonable")]
            public decimal? OperationReasonable{ get; set; }
            //// <summary>
            // 内容传达清晰
            ////</summary>
            [ProtoMember(17)]
            [Field("ContentConveyDistinct")]
            public decimal? ContentConveyDistinct{ get; set; }
            //// <summary>
            // 管理回复
            ////</summary>
            [ProtoMember(18)]
            [Field("ManageReply")]
            public string ManageReply{ get; set; }
    }
}