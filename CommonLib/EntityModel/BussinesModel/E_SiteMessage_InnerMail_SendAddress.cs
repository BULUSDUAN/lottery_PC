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
    [Entity("E_SiteMessage_InnerMail_SendAddress",Type = EntityType.Table)]
    public class E_SiteMessage_InnerMail_SendAddress
    { 
        public E_SiteMessage_InnerMail_SendAddress()
        {
        
        }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(1)]
            [Field("LId", IsIdenty = true, IsPrimaryKey = true)]
            public int LId{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(2)]
            [Field("MailId")]
            public string MailId{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(3)]
            [Field("ReceiverType")]
            public int? ReceiverType{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(4)]
            [Field("ReceiverId")]
            public string ReceiverId{ get; set; }
    }
}