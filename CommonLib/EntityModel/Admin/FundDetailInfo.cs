using System;
using System.Collections.Generic;
using EntityModel.Enum;

namespace EntityModel
{
    /// <summary>
    /// 资金明细
    /// </summary>
    public class FundDetailInfo
    {
        public long Id { get; set; }
        public string KeyLine { get; set; }
        public string OrderId { get; set; }
        public string UserId { get; set; }
        /// 收支类型
        public PayType PayType { get; set; }
        /// 账户类型
        public AccountType AccountType { get; set; }
        public string Category { get; set; }
        public string Summary { get; set; }
        /// 收入金额
        public decimal PayMoney { get; set; }
        /// 交易前余额
        public decimal BeforeBalance { get; set; }
        /// 交易后余额
        public decimal AfterBalance { get; set; }
        public DateTime CreateTime { get; set; }
        public string OperatorId { get; set; }
    }
    public class UserFillMoneyAddInfo
    {
        public string CustomerOrderId { get; set; }
        /// <summary>
        /// 充值代理商。如：支付宝
        /// </summary>
        public FillMoneyAgentType FillMoneyAgent { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public SchemeSource SchemeSource { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }
        /// <summary>
        /// 商品类型
        /// </summary>
        public string GoodsType { get; set; }
        /// <summary>
        /// 商品描述
        /// </summary>
        public string GoodsDescription { get; set; }
        /// <summary>
        /// 是否需要快递
        /// </summary>
        public string IsNeedDelivery { get; set; }
        /// <summary>
        /// 请求充值金额
        /// </summary>
        public decimal RequestMoney { get; set; }
        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal PayMoney { get; set; }
        /// <summary>
        /// 充值完成后跳转的页面
        /// </summary>
        public string ReturnUrl { get; set; }
        /// <summary>
        /// 交易过程中服务器通知的页面。用于充值后立马关闭页面
        /// </summary>
        public string NotifyUrl { get; set; }
        /// <summary>
        /// 商品展示的页面
        /// </summary>
        public string ShowUrl { get; set; }
        /// <summary>
        /// 附加数据 
        /// </summary>
        public string RequestExtensionInfo { get; set; }
    }
    
    /// <summary>
    /// 澳彩豆豆明细
    /// </summary>
    
    public class OCDouDouDetailInfo
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 收支类型
        /// </summary>
        public PayType PayType { get; set; }
        /// <summary>
        /// 分类编号
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 收入金额
        /// </summary>
        public int PayMoney { get; set; }
        /// <summary>
        /// 交易前余额
        /// </summary>
        public int BeforeBalance { get; set; }
        /// <summary>
        /// 交易后余额
        /// </summary>
        public int AfterBalance { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateTime { get; set; }
    }

    
    public class OCDouDouDetailInfoCollection
    {
        public OCDouDouDetailInfoCollection()
        {
            List = new List<OCDouDouDetailInfo>();
        }
        public int TotalCount { get; set; }
        public List<OCDouDouDetailInfo> List { get; set; }
    }

    /// <summary>
    /// 成长值明细
    /// </summary>
    
    public class UserGrowthDetailInfo
    {
        /// <summary>
        /// 分类编号
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 收入成长值
        /// </summary>
        public int PayMoney { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 支出类型
        /// </summary>
        public PayType PayType { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 交易后余额
        /// </summary>
        public int AfterBalance { get; set; }
    }
    
    public class UserGrowthDetailInfoCollection
    {
        public UserGrowthDetailInfoCollection()
        {
            List = new List<UserGrowthDetailInfo>();
        }
        public List<UserGrowthDetailInfo> List { get; set; }
        public int TotalPayInMoney { get; set; }
        public int TotalPayOutMoney { get; set; }
        public int TotalCount { get; set; }
    }

}
