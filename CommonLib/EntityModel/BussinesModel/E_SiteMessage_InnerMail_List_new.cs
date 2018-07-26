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
    [Entity("E_SiteMessage_InnerMail_List_new",Type = EntityType.Table)]
    public class E_SiteMessage_InnerMail_List_new
    { 
        public E_SiteMessage_InnerMail_List_new()
        {
        
        }
            /// <summary>
            // 站内信编码
            ///</summary>
            [ProtoMember(1)]
            [Field("MailId", IsIdenty = false, IsPrimaryKey = true)]
            public string MailId{ get; set; }
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
            [Field("MsgContent")]
            public string MsgContent{ get; set; }
            /// <summary>
            // 发送时间
            ///</summary>
            [ProtoMember(4)]
            [Field("SendTime")]
            public DateTime SendTime{ get; set; }
            /// <summary>
            // 发送者编号
            ///</summary>
            [ProtoMember(5)]
            [Field("SenderId")]
            public string SenderId{ get; set; }
            /// <summary>
            // 接收者编号
            ///</summary>
            [ProtoMember(6)]
            [Field("ReceiverId")]
            public string ReceiverId{ get; set; }
            /// <summary>
            // 站内信处理类型
            ///</summary>
            [ProtoMember(7)]
            [Field("HandleType")]
            public int HandleType{ get; set; }
            /// <summary>
            // 阅读时间
            ///</summary>
            [ProtoMember(8)]
            [Field("ReadTime")]
            public DateTime ReadTime{ get; set; }
    }
}