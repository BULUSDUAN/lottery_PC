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
    [Entity("E_SiteMessage_InnerMail_List",Type = EntityType.Table)]
    public class E_SiteMessage_InnerMail_List
    { 
        public E_SiteMessage_InnerMail_List()
        {
        
        }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(1)]
            [Field("MailId", IsIdenty = false, IsPrimaryKey = true)]
            public string MailId{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(2)]
            [Field("Title")]
            public string Title{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(3)]
            [Field("Content")]
            public string Content{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(4)]
            [Field("SendTime")]
            public DateTime? SendTime{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(5)]
            [Field("ActionTime")]
            public DateTime? ActionTime{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(6)]
            [Field("SenderId")]
            public string SenderId{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(7)]
            [Field("UpdateTime")]
            public DateTime? UpdateTime{ get; set; }
    }
}