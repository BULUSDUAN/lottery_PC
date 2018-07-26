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
    [Entity("C_ReceiveNotice_Log_Complate",Type = EntityType.Table)]
    public class C_ReceiveNotice_Log_Complate
    { 
        public C_ReceiveNotice_Log_Complate()
        {
        
        }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(1)]
            [Field("ReceiveNoticeId", IsIdenty = true, IsPrimaryKey = true)]
            public int ReceiveNoticeId{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(2)]
            [Field("AgentId")]
            public string AgentId{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(3)]
            [Field("NoticeType")]
            public int NoticeType{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(4)]
            [Field("ReceiveUrlRoot")]
            public string ReceiveUrlRoot{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(5)]
            [Field("ReceiveDataString")]
            public string ReceiveDataString{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(6)]
            [Field("Sign")]
            public string Sign{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(7)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(8)]
            [Field("Remark")]
            public string Remark{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(9)]
            [Field("SendTimes")]
            public int? SendTimes{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(10)]
            [Field("ComplateTime")]
            public DateTime ComplateTime{ get; set; }
    }
}