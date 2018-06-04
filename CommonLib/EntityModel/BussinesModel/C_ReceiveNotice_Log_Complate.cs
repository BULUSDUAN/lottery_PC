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
    [Entity("C_ReceiveNotice_Log_Complate",Type = EntityType.Table)]
    public class C_ReceiveNotice_Log_Complate
    { 
        public C_ReceiveNotice_Log_Complate()
        {
        
        }
            //// <summary>
            // 通知编号
            ////</summary>
            [ProtoMember(1)]
            [Field("ReceiveNoticeId", IsIdenty = true, IsPrimaryKey = true)]
            public int ReceiveNoticeId{ get; set; }
            //// <summary>
            // 代理商编号
            ////</summary>
            [ProtoMember(2)]
            [Field("AgentId")]
            public string AgentId{ get; set; }
            //// <summary>
            // 通知类型
            ////</summary>
            [ProtoMember(3)]
            [Field("NoticeType")]
            public int NoticeType{ get; set; }
            //// <summary>
            // 通知根URL
            ////</summary>
            [ProtoMember(4)]
            [Field("ReceiveUrlRoot")]
            public string ReceiveUrlRoot{ get; set; }
            //// <summary>
            // 通知数据字符串
            ////</summary>
            [ProtoMember(5)]
            [Field("ReceiveDataString")]
            public string ReceiveDataString{ get; set; }
            //// <summary>
            // 签名
            ////</summary>
            [ProtoMember(6)]
            [Field("Sign")]
            public string Sign{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(7)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
            //// <summary>
            // 备注
            ////</summary>
            [ProtoMember(8)]
            [Field("Remark")]
            public string Remark{ get; set; }
            //// <summary>
            // 发送时间
            ////</summary>
            [ProtoMember(9)]
            [Field("SendTimes")]
            public int? SendTimes{ get; set; }
            //// <summary>
            // 完成时间
            ////</summary>
            [ProtoMember(10)]
            [Field("ComplateTime")]
            public DateTime ComplateTime{ get; set; }
    }
}