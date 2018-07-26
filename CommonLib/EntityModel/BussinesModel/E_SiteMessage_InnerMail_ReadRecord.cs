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
    // 站内信阅读
    ///</summary>
    [ProtoContract]
    [Entity("E_SiteMessage_InnerMail_ReadRecord",Type = EntityType.Table)]
    public class E_SiteMessage_InnerMail_ReadRecord
    { 
        public E_SiteMessage_InnerMail_ReadRecord()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("LId", IsIdenty = true, IsPrimaryKey = true)]
            public int LId{ get; set; }
            /// <summary>
            // 站内信编号
            ///</summary>
            [ProtoMember(2)]
            [Field("MailId")]
            public string MailId{ get; set; }
            /// <summary>
            // 接收者编号
            ///</summary>
            [ProtoMember(3)]
            [Field("ReceiverId")]
            public string ReceiverId{ get; set; }
            /// <summary>
            // 站内信处理类型
            ///</summary>
            [ProtoMember(4)]
            [Field("HandleType")]
            public int HandleType{ get; set; }
            /// <summary>
            // 更新时间
            ///</summary>
            [ProtoMember(5)]
            [Field("UpdateTime")]
            public DateTime UpdateTime{ get; set; }
    }
}