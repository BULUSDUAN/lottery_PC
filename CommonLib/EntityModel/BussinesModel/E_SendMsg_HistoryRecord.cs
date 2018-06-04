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
    [Entity("E_SendMsg_HistoryRecord",Type = EntityType.Table)]
    public class E_SendMsg_HistoryRecord
    { 
        public E_SendMsg_HistoryRecord()
        {
        
        }
            //// <summary>
            // 消息主键Id
            ////</summary>
            [ProtoMember(1)]
            [Field("MsgId", IsIdenty = true, IsPrimaryKey = true)]
            public int MsgId{ get; set; }
            //// <summary>
            // 手机号码
            ////</summary>
            [ProtoMember(2)]
            [Field("PhoneNumber")]
            public string PhoneNumber{ get; set; }
            //// <summary>
            // IP地址
            ////</summary>
            [ProtoMember(3)]
            [Field("IP")]
            public string IP{ get; set; }
            //// <summary>
            // 消息内容
            ////</summary>
            [ProtoMember(4)]
            [Field("MsgType")]
            public int? MsgType{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(5)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}