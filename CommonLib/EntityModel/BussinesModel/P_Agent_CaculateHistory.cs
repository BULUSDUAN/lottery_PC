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
    [Entity("P_Agent_CaculateHistory",Type = EntityType.Table)]
    public class P_Agent_CaculateHistory
    { 
        public P_Agent_CaculateHistory()
        {
        
        }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(1)]
            [Field("ID", IsIdenty = true, IsPrimaryKey = true)]
            public int ID{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(2)]
            [Field("CaculateTimeFrom")]
            public DateTime? CaculateTimeFrom{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(3)]
            [Field("CaculateTimeTo")]
            public DateTime? CaculateTimeTo{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(4)]
            [Field("TotalAgentCount")]
            public int? TotalAgentCount{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(5)]
            [Field("TotalOrderCount")]
            public int? TotalOrderCount{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(6)]
            [Field("TotalOrderMoney")]
            public decimal? TotalOrderMoney{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(7)]
            [Field("TotalBuyMoney")]
            public decimal? TotalBuyMoney{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(8)]
            [Field("TotalCommisionMoney")]
            public decimal? TotalCommisionMoney{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(9)]
            [Field("ErrorCount")]
            public int? ErrorCount{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(10)]
            [Field("ErrorOrderMoney")]
            public decimal? ErrorOrderMoney{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(11)]
            [Field("ErrorBuyMoney")]
            public decimal? ErrorBuyMoney{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(12)]
            [Field("ErrorSchemeIdList")]
            public string ErrorSchemeIdList{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(13)]
            [Field("MillisecondSpan")]
            public int? MillisecondSpan{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(14)]
            [Field("CreateBy")]
            public string CreateBy{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(15)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}