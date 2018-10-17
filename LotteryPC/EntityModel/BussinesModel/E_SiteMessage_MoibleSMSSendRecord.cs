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
    // 手机短信发送记录
    ///</summary>
    [ProtoContract]
    [Entity("E_SiteMessage_MoibleSMSSendRecord",Type = EntityType.Table)]
    public class E_SiteMessage_MoibleSMSSendRecord
    { 
        public E_SiteMessage_MoibleSMSSendRecord()
        {
        
        }
            /// <summary>
            // 编号
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 用户编号
            ///</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            /// <summary>
            // 手机号码
            ///</summary>
            [ProtoMember(3)]
            [Field("Mobile")]
            public string Mobile{ get; set; }
            /// <summary>
            // 短信内容
            ///</summary>
            [ProtoMember(4)]
            [Field("SMSContent")]
            public string SMSContent{ get; set; }
            /// <summary>
            // 发送状态
            ///</summary>
            [ProtoMember(5)]
            [Field("SendStatus")]
            public string SendStatus{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(6)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}