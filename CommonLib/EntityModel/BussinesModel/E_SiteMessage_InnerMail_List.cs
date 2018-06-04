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
    // 站内信
    ////</summary>
    [ProtoContract]
    [Entity("E_SiteMessage_InnerMail_List",Type = EntityType.Table)]
    public class E_SiteMessage_InnerMail_List
    { 
        public E_SiteMessage_InnerMail_List()
        {
        
        }
            //// <summary>
            // 站内信编号
            ////</summary>
            [ProtoMember(1)]
            [Field("MailId", IsIdenty = false, IsPrimaryKey = true)]
            public string MailId{ get; set; }
            //// <summary>
            // 标题
            ////</summary>
            [ProtoMember(2)]
            [Field("Title")]
            public string Title{ get; set; }
            //// <summary>
            // 内容
            ////</summary>
            [ProtoMember(3)]
            [Field("Content")]
            public string Content{ get; set; }
            //// <summary>
            // 发送时间
            ////</summary>
            [ProtoMember(4)]
            [Field("SendTime")]
            public DateTime? SendTime{ get; set; }
            //// <summary>
            // 编辑时间
            ////</summary>
            [ProtoMember(5)]
            [Field("ActionTime")]
            public DateTime? ActionTime{ get; set; }
            //// <summary>
            // 发送者编号
            ////</summary>
            [ProtoMember(6)]
            [Field("SenderId")]
            public string SenderId{ get; set; }
            //// <summary>
            // 更新时间
            ////</summary>
            [ProtoMember(7)]
            [Field("UpdateTime")]
            public DateTime? UpdateTime{ get; set; }
    }
}