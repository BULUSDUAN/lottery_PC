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
    // 充值
    ////</summary>
    [ProtoContract]
    [Entity("C_FillMoney",Type = EntityType.Table)]
    public class C_FillMoney
    { 
        public C_FillMoney()
        {
        
        }
            //// <summary>
            // 充值订单号
            ////</summary>
            [ProtoMember(1)]
            [Field("OrderId", IsIdenty = false, IsPrimaryKey = true)]
            public string OrderId{ get; set; }
            //// <summary>
            // 充值代理商。如：支付宝
            ////</summary>
            [ProtoMember(2)]
            [Field("FillMoneyAgent")]
            public int FillMoneyAgent{ get; set; }
            //// <summary>
            // 用户
            ////</summary>
            [ProtoMember(3)]
            [Field("UserId")]
            public string UserId{ get; set; }
            //// <summary>
            // 商品名称
            ////</summary>
            [ProtoMember(4)]
            [Field("GoodsName")]
            public string GoodsName{ get; set; }
            //// <summary>
            // 商品类型
            ////</summary>
            [ProtoMember(5)]
            [Field("GoodsType")]
            public string GoodsType{ get; set; }
            //// <summary>
            // 商品描述
            ////</summary>
            [ProtoMember(6)]
            [Field("GoodsDescription")]
            public string GoodsDescription{ get; set; }
            //// <summary>
            // 是否需要快递
            ////</summary>
            [ProtoMember(7)]
            [Field("IsNeedDelivery")]
            public string IsNeedDelivery{ get; set; }
            //// <summary>
            // 快递地址
            ////</summary>
            [ProtoMember(8)]
            [Field("DeliveryAddress")]
            public string DeliveryAddress{ get; set; }
            //// <summary>
            // 发起请求者
            ////</summary>
            [ProtoMember(9)]
            [Field("RequestBy")]
            public string RequestBy{ get; set; }
            //// <summary>
            // 请求附加信息
            ////</summary>
            [ProtoMember(10)]
            [Field("RequestExtensionInfo")]
            public string RequestExtensionInfo{ get; set; }
            //// <summary>
            // 请求充值金额
            ////</summary>
            [ProtoMember(11)]
            [Field("RequestMoney")]
            public decimal? RequestMoney{ get; set; }
            //// <summary>
            // 支付金额
            ////</summary>
            [ProtoMember(12)]
            [Field("PayMoney")]
            public decimal? PayMoney{ get; set; }
            //// <summary>
            // 请求充值时间
            ////</summary>
            [ProtoMember(13)]
            [Field("RequestTime")]
            public DateTime? RequestTime{ get; set; }
            //// <summary>
            // 充值完成后跳转的页面
            ////</summary>
            [ProtoMember(14)]
            [Field("ReturnUrl")]
            public string ReturnUrl{ get; set; }
            //// <summary>
            // 交易过程中服务器通知的页面。用于充值后立马关闭页面
            ////</summary>
            [ProtoMember(15)]
            [Field("NotifyUrl")]
            public string NotifyUrl{ get; set; }
            //// <summary>
            // 商品展示的页面
            ////</summary>
            [ProtoMember(16)]
            [Field("ShowUrl")]
            public string ShowUrl{ get; set; }
            //// <summary>
            // 充值状态
            ////</summary>
            [ProtoMember(17)]
            [Field("Status")]
            public int? Status{ get; set; }
            //// <summary>
            // 响应者
            ////</summary>
            [ProtoMember(18)]
            [Field("ResponseBy")]
            public string ResponseBy{ get; set; }
            //// <summary>
            // 响应编码
            ////</summary>
            [ProtoMember(19)]
            [Field("ResponseCode")]
            public string ResponseCode{ get; set; }
            //// <summary>
            // 响应消息
            ////</summary>
            [ProtoMember(20)]
            [Field("ResponseMessage")]
            public string ResponseMessage{ get; set; }
            //// <summary>
            // 响应金额
            ////</summary>
            [ProtoMember(21)]
            [Field("ResponseMoney")]
            public decimal? ResponseMoney{ get; set; }
            //// <summary>
            // 响应时间
            ////</summary>
            [ProtoMember(22)]
            [Field("ResponseTime")]
            public DateTime? ResponseTime{ get; set; }
            //// <summary>
            // 外部订单流水号
            ////</summary>
            [ProtoMember(23)]
            [Field("OuterFlowId")]
            public string OuterFlowId{ get; set; }
            //// <summary>
            // 方案来源
            ////</summary>
            [ProtoMember(24)]
            [Field("SchemeSource")]
            public int? SchemeSource{ get; set; }
            //// <summary>
            // 充值接口商户号
            ////</summary>
            [ProtoMember(25)]
            [Field("AgentId")]
            public string AgentId{ get; set; }
    }
}