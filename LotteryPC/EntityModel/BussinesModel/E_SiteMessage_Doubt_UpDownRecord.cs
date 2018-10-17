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
    // 顶踩记录
    ///</summary>
    [ProtoContract]
    [Entity("E_SiteMessage_Doubt_UpDownRecord",Type = EntityType.Table)]
    public class E_SiteMessage_Doubt_UpDownRecord
    { 
        public E_SiteMessage_Doubt_UpDownRecord()
        {
        
        }
            /// <summary>
            // 主键
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
            // 问题编号
            ///</summary>
            [ProtoMember(3)]
            [Field("DoubtId")]
            public string DoubtId{ get; set; }
            /// <summary>
            // 顶/踩
            ///</summary>
            [ProtoMember(4)]
            [Field("UpDown")]
            public string UpDown{ get; set; }
            /// <summary>
            // 操作时间
            ///</summary>
            [ProtoMember(5)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}