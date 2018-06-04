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
    // 站内信息场景
    ////</summary>
    [ProtoContract]
    [Entity("E_SiteMessage_SiteMessageScene",Type = EntityType.Table)]
    public class E_SiteMessage_SiteMessageScene
    { 
        public E_SiteMessage_SiteMessageScene()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 场景Key
            ////</summary>
            [ProtoMember(2)]
            [Field("SceneKey")]
            public string SceneKey{ get; set; }
            //// <summary>
            // 场景名称
            ////</summary>
            [ProtoMember(3)]
            [Field("SceneName")]
            public string SceneName{ get; set; }
            //// <summary>
            // 信息类别
            ////</summary>
            [ProtoMember(4)]
            [Field("MsgCategory")]
            public int? MsgCategory{ get; set; }
            //// <summary>
            // 消息模板标题
            ////</summary>
            [ProtoMember(5)]
            [Field("MsgTemplateTitle")]
            public string MsgTemplateTitle{ get; set; }
            //// <summary>
            // 消息模板内容
            ////</summary>
            [ProtoMember(6)]
            [Field("MsgTemplateContent")]
            public string MsgTemplateContent{ get; set; }
            //// <summary>
            // 消息模板支持的参数(程序主动代入的参数)
            ////</summary>
            [ProtoMember(7)]
            [Field("MsgTemplateParams")]
            public string MsgTemplateParams{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(8)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}