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
    // 代理商结算历史
    ///</summary>
    [ProtoContract]
    [Entity("P_Agent_CaculateHistory",Type = EntityType.Table)]
    public class P_Agent_CaculateHistory
    { 
        public P_Agent_CaculateHistory()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("ID", IsIdenty = true, IsPrimaryKey = true)]
            public int ID{ get; set; }
            /// <summary>
            // 结算时间开始
            ///</summary>
            [ProtoMember(2)]
            [Field("CaculateTimeFrom")]
            public DateTime CaculateTimeFrom{ get; set; }
            /// <summary>
            // 结算时间结束
            ///</summary>
            [ProtoMember(3)]
            [Field("CaculateTimeTo")]
            public DateTime CaculateTimeTo{ get; set; }
            /// <summary>
            // 总代理商数
            ///</summary>
            [ProtoMember(4)]
            [Field("TotalAgentCount")]
            public int TotalAgentCount{ get; set; }
            /// <summary>
            // 总订单数
            ///</summary>
            [ProtoMember(5)]
            [Field("TotalOrderCount")]
            public int TotalOrderCount{ get; set; }
            /// <summary>
            // 总订单金额
            ///</summary>
            [ProtoMember(6)]
            [Field("TotalOrderMoney")]
            public decimal TotalOrderMoney{ get; set; }
            /// <summary>
            // 总购买金额
            ///</summary>
            [ProtoMember(7)]
            [Field("TotalBuyMoney")]
            public decimal TotalBuyMoney{ get; set; }
            /// <summary>
            // 总提成金额
            ///</summary>
            [ProtoMember(8)]
            [Field("TotalCommisionMoney")]
            public decimal TotalCommisionMoney{ get; set; }
            /// <summary>
            // 错误数
            ///</summary>
            [ProtoMember(9)]
            [Field("ErrorCount")]
            public int ErrorCount{ get; set; }
            /// <summary>
            // 错误订单金额
            ///</summary>
            [ProtoMember(10)]
            [Field("ErrorOrderMoney")]
            public decimal ErrorOrderMoney{ get; set; }
            /// <summary>
            // 错误购买金额
            ///</summary>
            [ProtoMember(11)]
            [Field("ErrorBuyMoney")]
            public decimal ErrorBuyMoney{ get; set; }
            /// <summary>
            // 错误方案编号列表
            ///</summary>
            [ProtoMember(12)]
            [Field("ErrorSchemeIdList")]
            public string ErrorSchemeIdList{ get; set; }
            /// <summary>
            // 毫秒跨度
            ///</summary>
            [ProtoMember(13)]
            [Field("MillisecondSpan")]
            public int MillisecondSpan{ get; set; }
            /// <summary>
            // 创建者
            ///</summary>
            [ProtoMember(14)]
            [Field("CreateBy")]
            public string CreateBy{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(15)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}