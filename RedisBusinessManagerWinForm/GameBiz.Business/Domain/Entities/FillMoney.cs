using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;

namespace GameBiz.Domain.Entities
{
    /// <summary>
    /// 充值
    /// </summary>
    public class FillMoney
    {
        /// <summary>
        /// 充值订单号
        /// </summary>
        public virtual string OrderId { get; set; }
        /// <summary>
        /// 充值代理商。如：支付宝
        /// </summary>
        public virtual FillMoneyAgentType FillMoneyAgent { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public virtual string GoodsName { get; set; }
        /// <summary>
        /// 商品类型
        /// </summary>
        public virtual string GoodsType { get; set; }
        /// <summary>
        /// 商品描述
        /// </summary>
        public virtual string GoodsDescription { get; set; }
        /// <summary>
        /// 是否需要快递
        /// </summary>
        public virtual string IsNeedDelivery { get; set; }
        /// <summary>
        /// 快递地址
        /// </summary>
        public virtual string DeliveryAddress { get; set; }
        /// <summary>
        /// 发起请求者
        /// </summary>
        public virtual string RequestBy { get; set; }
        /// <summary>
        /// 请求附加信息
        /// </summary>
        public virtual string RequestExtensionInfo { get; set; }
        /// <summary>
        /// 请求充值金额
        /// </summary>
        public virtual decimal RequestMoney { get; set; }
        /// <summary>
        /// 支付金额
        /// </summary>
        public virtual decimal PayMoney { get; set; }
        /// <summary>
        /// 请求充值时间
        /// </summary>
        public virtual DateTime RequestTime { get; set; }
        /// <summary>
        /// 充值完成后跳转的页面
        /// </summary>
        public virtual string ReturnUrl { get; set; }
        /// <summary>
        /// 交易过程中服务器通知的页面。用于充值后立马关闭页面
        /// </summary>
        public virtual string NotifyUrl { get; set; }
        /// <summary>
        /// 商品展示的页面
        /// </summary>
        public virtual string ShowUrl { get; set; }
        /// <summary>
        /// 充值状态
        /// </summary>
        public virtual FillMoneyStatus Status { get; set; }
        /// <summary>
        /// 响应者
        /// </summary>
        public virtual string ResponseBy { get; set; }
        /// <summary>
        /// 响应编码
        /// </summary>
        public virtual string ResponseCode { get; set; }
        /// <summary>
        /// 响应消息
        /// </summary>
        public virtual string ResponseMessage { get; set; }
        /// <summary>
        /// 响应金额
        /// </summary>
        public virtual decimal? ResponseMoney { get; set; }
        /// <summary>
        /// 响应时间
        /// </summary>
        public virtual DateTime? ResponseTime { get; set; }
        /// <summary>
        /// 外部订单流水号
        /// </summary>
        public virtual string OuterFlowId { get; set; }
        /// <summary>
        /// 方案来源
        /// </summary>
        public virtual SchemeSource SchemeSource { get; set; }
        /// <summary>
        /// 充值接口商户号
        /// </summary>
        public virtual string AgentId { get; set; }

    }
}
