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
    // 资金明细
    ///</summary>
    [ProtoContract]
    [Entity("C_Fund_Detail",Type = EntityType.Table)]
    public class C_Fund_Detail
    { 
        public C_Fund_Detail()
        {
        
        }
            /// <summary>
            // 自增编号
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 线索编号
            ///</summary>
            [ProtoMember(2)]
            [Field("KeyLine")]
            public string KeyLine{ get; set; }
            /// <summary>
            // 内部订单号
            ///</summary>
            [ProtoMember(3)]
            [Field("OrderId")]
            public string OrderId{ get; set; }
            /// <summary>
            // 用户Id
            ///</summary>
            [ProtoMember(4)]
            [Field("UserId")]
            public string UserId{ get; set; }
            /// <summary>
            // 收支类型
            ///</summary>
            [ProtoMember(5)]
            [Field("PayType")]
            public int PayType{ get; set; }
            /// <summary>
            // 账户类型
            ///</summary>
            [ProtoMember(6)]
            [Field("AccountType")]
            public int AccountType{ get; set; }
            /// <summary>
            // 分类编号
            ///</summary>
            [ProtoMember(7)]
            [Field("Category")]
            public string Category{ get; set; }
            /// <summary>
            // 描述
            ///</summary>
            [ProtoMember(8)]
            [Field("Summary")]
            public string Summary{ get; set; }
            /// <summary>
            // 收入金额
            ///</summary>
            [ProtoMember(9)]
            [Field("PayMoney")]
            public decimal PayMoney{ get; set; }
            /// <summary>
            // 交易前余额
            ///</summary>
            [ProtoMember(10)]
            [Field("BeforeBalance")]
            public decimal BeforeBalance{ get; set; }
            /// <summary>
            // 交易后余额
            ///</summary>
            [ProtoMember(11)]
            [Field("AfterBalance")]
            public decimal AfterBalance{ get; set; }
            /// <summary>
            // 创建日期
            ///</summary>
            [ProtoMember(12)]
            [Field("OperatorId")]
            public string OperatorId{ get; set; }
            /// <summary>
            // 代理商
            ///</summary>
            [ProtoMember(13)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
            /// <summary>
            // 操作员Id
            ///</summary>
            [ProtoMember(14)]
            [Field("AgentId")]
            public string AgentId{ get; set; }
    }
}