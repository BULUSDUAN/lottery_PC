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
    // 提现
    ///</summary>
    [ProtoContract]
    [Entity("C_Withdraw",Type = EntityType.Table)]
    public class C_Withdraw
    { 
        public C_Withdraw()
        {
        
        }
            /// <summary>
            // 订单号
            ///</summary>
            [ProtoMember(1)]
            [Field("OrderId", IsIdenty = false, IsPrimaryKey = true)]
            public string OrderId{ get; set; }
            /// <summary>
            // 申请人
            ///</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            /// <summary>
            // 提现渠道
            ///</summary>
            [ProtoMember(3)]
            [Field("WithdrawAgent")]
            public int WithdrawAgent { get; set; }
            /// <summary>
            // 提现类别
            ///</summary>
            [ProtoMember(4)]
            [Field("WithdrawCategory")]
            public int WithdrawCategory{ get; set; }
            /// <summary>
            // 省份名称
            ///</summary>
            [ProtoMember(5)]
            [Field("ProvinceName")]
            public string ProvinceName{ get; set; }
            /// <summary>
            // 城市名称
            ///</summary>
            [ProtoMember(6)]
            [Field("CityName")]
            public string CityName{ get; set; }
            /// <summary>
            //  银行编号。如果不是银行卡支付，则为对应子类型的编码。如：支付宝为ALYPAY
            ///</summary>
            [ProtoMember(7)]
            [Field("BankCode")]
            public string BankCode{ get; set; }
            /// <summary>
            // 银行名称
            ///</summary>
            [ProtoMember(8)]
            [Field("BankName")]
            public string BankName{ get; set; }
            /// <summary>
            // 开户支行名称
            ///</summary>
            [ProtoMember(9)]
            [Field("BankSubName")]
            public string BankSubName{ get; set; }
            /// <summary>
            // 银行卡卡号。如果不是银行卡提款，则为对应的帐号。
            ///</summary>
            [ProtoMember(10)]
            [Field("BankCardNumber")]
            public string BankCardNumber{ get; set; }
            /// <summary>
            // 申请提款金额
            ///</summary>
            [ProtoMember(11)]
            [Field("RequestMoney")]
            public decimal RequestMoney{ get; set; }
            /// <summary>
            // 申请时间
            ///</summary>
            [ProtoMember(12)]
            [Field("RequestTime")]
            public DateTime? RequestTime{ get; set; }
            /// <summary>
            // 提款状态
            ///</summary>
            [ProtoMember(13)]
            [Field("Status")]
            public int Status{ get; set; }
            /// <summary>
            // 处理人
            ///</summary>
            [ProtoMember(14)]
            [Field("ResponseUserId")]
            public string ResponseUserId{ get; set; }
            /// <summary>
            // 响应金额。即已提款金额
            ///</summary>
            [ProtoMember(15)]
            [Field("ResponseMoney")]
            public decimal ResponseMoney{ get; set; }
            /// <summary>
            // 响应消息
            ///</summary>
            [ProtoMember(16)]
            [Field("ResponseMessage")]
            public string ResponseMessage{ get; set; }
            /// <summary>
            // 回复时间
            ///</summary>
            [ProtoMember(17)]
            [Field("ResponseTime")]
            public DateTime? ResponseTime{ get; set; }
            /// <summary>
            // 代理商
            ///</summary>
            [ProtoMember(18)]
            [Field("AgentId")]
            public string AgentId{ get; set; }
    }
}