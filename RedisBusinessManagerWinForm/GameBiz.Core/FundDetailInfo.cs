using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using Common;
using Common.Utilities;
using Common.Mappings;

namespace GameBiz.Core
{
    /// <summary>
    /// 资金明细
    /// </summary>
    [CommunicationObject]
    public class FundDetailInfo
    {
        [EntityMappingField("Id")]
        public long Id { get; set; }
        [EntityMappingField("KeyLine")]
        public string KeyLine { get; set; }
        [EntityMappingField("OrderId")]
        public string OrderId { get; set; }
        [EntityMappingField("UserId")]
        public string UserId { get; set; }
        /// 收支类型
        [EntityMappingField("PayType")]
        public PayType PayType { get; set; }
        /// 账户类型
        [EntityMappingField("AccountType")]
        public AccountType AccountType { get; set; }
        [EntityMappingField("Category")]
        public string Category { get; set; }
        [EntityMappingField("Summary")]
        public string Summary { get; set; }
        /// 收入金额
        [EntityMappingField("PayMoney")]
        public decimal PayMoney { get; set; }
        /// 交易前余额
        [EntityMappingField("BeforeBalance")]
        public decimal BeforeBalance { get; set; }
        /// 交易后余额
        [EntityMappingField("AfterBalance")]
        public decimal AfterBalance { get; set; }
        [EntityMappingField("CreateTime")]
        public DateTime CreateTime { get; set; }
        [EntityMappingField("OperatorId")]
        public string OperatorId { get; set; }
    }
    [CommunicationObject]
    public class UserFundDetailCollection
    {
        public int TotalPayinCount { get; set; }
        public decimal TotalPayinMoney { get; set; }
        public int TotalPayoutCount { get; set; }
        public decimal TotalPayoutMoney { get; set; }
        public decimal TotalBalanceMoney { get; set; }
        public IList<FundDetailInfo> FundDetailList { get; set; }
    }

    [CommunicationObject]
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

    [CommunicationObject]
    public class FillMoneyQueryInfo
    {
        /// <summary>
        /// 充值订单号
        /// </summary>
        [EntityMappingField("OrderId")]
        public string OrderId { get; set; }
        /// <summary>
        /// 充值代理商。如：支付宝
        /// </summary>
        [EntityMappingField("FillMoneyAgent")]
        public FillMoneyAgentType FillMoneyAgent { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        [EntityMappingField("UserId")]
        public string UserId { get; set; }
        /// <summary>
        /// 用户显示名
        /// </summary>
        [EntityMappingField("DisplayName")]
        public string UserDisplayName { get; set; }
        /// <summary>
        /// 用户名称来源
        /// </summary>
        [EntityMappingField("ComeFrom")]
        public string UserComeFrom { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [EntityMappingField("GoodsName")]
        public string GoodsName { get; set; }
        /// <summary>
        /// 商品类型
        /// </summary>
        [EntityMappingField("GoodsType")]
        public string GoodsType { get; set; }
        /// <summary>
        /// 商品描述
        /// </summary>
        [EntityMappingField("GoodsDescription")]
        public string GoodsDescription { get; set; }
        /// <summary>
        /// 是否需要快递
        /// </summary>
        [EntityMappingField("IsNeedDelivery")]
        public string IsNeedDelivery { get; set; }
        /// <summary>
        /// 快递地址
        /// </summary>
        [EntityMappingField("DeliveryAddress")]
        public string DeliveryAddress { get; set; }
        /// <summary>
        /// 发起请求者
        /// </summary>
        [EntityMappingField("RequestBy")]
        public string RequestBy { get; set; }
        /// <summary>
        /// 请求附加信息
        /// </summary>
        [EntityMappingField("RequestExtensionInfo")]
        public string RequestExtensionInfo { get; set; }
        /// <summary>
        /// 请求充值金额
        /// </summary>
        [EntityMappingField("RequestMoney")]
        public decimal RequestMoney { get; set; }
        /// <summary>
        /// 支付金额
        /// </summary>
        [EntityMappingField("PayMoney")]
        public decimal PayMoney { get; set; }
        /// <summary>
        /// 请求充值时间
        /// </summary>
        [EntityMappingField("RequestTime")]
        public DateTime RequestTime { get; set; }
        /// <summary>
        /// 充值完成后跳转的页面
        /// </summary>
        [EntityMappingField("ReturnUrl")]
        public string ReturnUrl { get; set; }
        /// <summary>
        /// 交易过程中服务器通知的页面。用于充值后立马关闭页面
        /// </summary>
        [EntityMappingField("NotifyUrl")]
        public string NotifyUrl { get; set; }
        /// <summary>
        /// 商品展示的页面
        /// </summary>
        [EntityMappingField("ShowUrl")]
        public string ShowUrl { get; set; }
        /// <summary>
        /// 充值状态
        /// </summary>
        [EntityMappingField("Status")]
        public FillMoneyStatus Status { get; set; }
        /// <summary>
        /// 响应者
        /// </summary>
        [EntityMappingField("ResponseBy")]
        public string ResponseBy { get; set; }
        /// <summary>
        /// 响应编码
        /// </summary>
        [EntityMappingField("ResponseCode")]
        public string ResponseCode { get; set; }
        /// <summary>
        /// 响应消息
        /// </summary>
        [EntityMappingField("ResponseMessage")]
        public string ResponseMessage { get; set; }
        /// <summary>
        /// 响应金额
        /// </summary>
        [EntityMappingField("ResponseMoney")]
        public decimal? ResponseMoney { get; set; }
        /// <summary>
        /// 响应时间
        /// </summary>
        [EntityMappingField("ResponseTime")]
        public DateTime? ResponseTime { get; set; }
        /// <summary>
        /// 外部订单流水号
        /// </summary>
        [EntityMappingField("OuterFlowId")]
        public string OuterFlowId { get; set; }
        /// <summary>
        /// 方案来源
        /// </summary>
        [EntityMappingField("SchemeSource")]
        public SchemeSource SchemeSource { get; set; }
        /// <summary>
        /// 充值接口商户号
        /// </summary>
        [EntityMappingField("AgentId")]
        public string AgentId { get; set; }
    }

    [CommunicationObject]
    public class FillMoneyQueryInfoCollection
    {
        public FillMoneyQueryInfoCollection()
        {
            FillMoneyList = new List<FillMoneyQueryInfo>();
        }
        public int TotalCount { get; set; }
        public decimal TotalRequestMoney { get; set; }
        public decimal TotalResponseMoney { get; set; }
        public IList<FillMoneyQueryInfo> FillMoneyList { get; set; }
    }

    /// <summary>
    /// 检查提现申请结果
    /// </summary>
    [CommunicationObject]
    public class CheckWithdrawResult
    {
        /// <summary>
        /// 申请提现金额
        /// </summary>
        public decimal RequestMoney { get; set; }
        /// <summary>
        /// 到账金额
        /// </summary>
        public decimal ResponseMoney { get; set; }
        /// <summary>
        /// 提现类别
        /// </summary>
        public WithdrawCategory WithdrawCategory { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Summary { get; set; }

    }

    /// <summary>
    /// 用户提现申请
    /// </summary>
    [CommunicationObject]
    public class Withdraw_RequestInfo
    {
        /// <summary>
        /// 提现渠道
        /// </summary>
        public WithdrawAgentType WithdrawAgent { get; set; }
        /// <summary>
        /// 省份名称
        /// </summary>
        public string ProvinceName { get; set; }
        /// <summary>
        /// 城市名称
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 银行编号。如果不是银行卡支付，则为对应子类型的编码。如：支付宝为ALYPAY
        /// </summary>
        public string BankCode { get; set; }
        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// 开户支行名称
        /// </summary>
        public string BankSubName { get; set; }
        /// <summary>
        /// 银行卡卡号。如果不是银行卡提款，则为对应的帐号。
        /// </summary>
        public string BankCardNumber { get; set; }
        /// <summary>
        /// 申请提款金额
        /// </summary>
        public decimal RequestMoney { get; set; }
        /// <summary>
        /// 用户真实姓名
        /// </summary>
        public String userRealName { get; set; }
    }

    [CommunicationObject]
    public class Withdraw_QueryInfo
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 申请人真实姓名
        /// </summary>
        public string RequesterRealName { get; set; }
        /// <summary>
        /// 申情人手机
        /// </summary>
        public string RequesterMobile { get; set; }
        /// <summary>
        /// 申请人名称
        /// </summary>
        public string RequesterDisplayName { get; set; }
        /// <summary>
        /// 申请人名称编号
        /// </summary>
        public string RequesterUserKey { get; set; }
        /// <summary>
        /// 申请人名称来源
        /// </summary>
        public string RequesterComeFrom { get; set; }
        /// <summary>
        /// 处理人名称
        /// </summary>
        public string ProcessorsDisplayName { get; set; }
        /// <summary>
        /// 处理人名称编号
        /// </summary>
        public string ProcessorsUserKey { get; set; }
        /// <summary>
        /// 提现类别
        /// </summary>
        public WithdrawAgentType WithdrawAgent { get; set; }
        /// <summary>
        /// 省份名称
        /// </summary>
        public string ProvinceName { get; set; }
        /// <summary>
        /// 城市名称
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 银行编号。如果不是银行卡支付，则为对应子类型的编码。如：支付宝为ALYPAY
        /// </summary>
        public string BankCode { get; set; }
        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// 开户支行名称
        /// </summary>
        public string BankSubName { get; set; }
        /// <summary>
        /// 银行卡卡号。如果不是银行卡提款，则为对应的帐号。
        /// </summary>
        public string BankCardNumber { get; set; }
        /// <summary>
        /// 申请提款金额
        /// </summary>
        public decimal RequestMoney { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime RequestTime { get; set; }
        /// <summary>
        /// 提款状态
        /// </summary>
        public WithdrawStatus Status { get; set; }
        /// <summary>
        /// 响应金额。即已提款金额
        /// </summary>
        public decimal? ResponseMoney { get; set; }
        /// <summary>
        /// 响应消息
        /// </summary>
        public string ResponseMessage { get; set; }
        /// <summary>
        /// 回复时间
        /// </summary>
        public DateTime? ResponseTime { get; set; }

        public void LoadArray(object[] dataArray)
        {
            if (dataArray.Length == 21)
            {
                OrderId = UsefullHelper.GetDbValue<string>(dataArray[0]);
                RequesterDisplayName = UsefullHelper.GetDbValue<string>(dataArray[1]);
                RequesterUserKey = UsefullHelper.GetDbValue<string>(dataArray[2]);
                RequesterComeFrom = UsefullHelper.GetDbValue<string>(dataArray[3]);
                ProcessorsDisplayName = UsefullHelper.GetDbValue<string>(dataArray[4]);
                ProcessorsUserKey = UsefullHelper.GetDbValue<string>(dataArray[5]);
                WithdrawAgent = UsefullHelper.GetDbValue<WithdrawAgentType>(dataArray[6]);
                ProvinceName = UsefullHelper.GetDbValue<string>(dataArray[7]);
                CityName = UsefullHelper.GetDbValue<string>(dataArray[8]);
                BankCode = UsefullHelper.GetDbValue<string>(dataArray[9]);
                BankName = UsefullHelper.GetDbValue<string>(dataArray[10]);
                BankSubName = UsefullHelper.GetDbValue<string>(dataArray[11]);
                BankCardNumber = UsefullHelper.GetDbValue<string>(dataArray[12]);
                RequestMoney = UsefullHelper.GetDbValue<decimal>(dataArray[13]);
                RequestTime = UsefullHelper.GetDbValue<DateTime>(dataArray[14]);
                Status = UsefullHelper.GetDbValue<WithdrawStatus>(dataArray[15]);
                ResponseMoney = UsefullHelper.GetDbValue<decimal?>(dataArray[16]);
                ResponseMessage = UsefullHelper.GetDbValue<string>(dataArray[17]);
                ResponseTime = UsefullHelper.GetDbValue<DateTime?>(dataArray[18]);
                RequesterRealName = UsefullHelper.GetDbValue<string>(dataArray[19]);
                RequesterMobile = UsefullHelper.GetDbValue<string>(dataArray[20]);
            }
            else
            {
                throw new ArgumentException("数据数组长度不满足要求，不能转换成此Withdraw_QueryInfo对象，传入数组为" + dataArray.Length + "。" + string.Join(", ", dataArray), "dataArray");
            }
        }
    }

    [CommunicationObject]
    public class Withdraw_QueryInfoCollection
    {
        public Withdraw_QueryInfoCollection()
        {
            WithdrawList = new List<Withdraw_QueryInfo>();
        }
        public int TotalCount { get; set; }

        public int WinCount { get; set; }
        public int RefusedCount { get; set; }
        public decimal TotalWinMoney { get; set; }
        public decimal TotalResponseMoney { get; set; }
        public decimal TotalRefusedMoney { get; set; }
        public decimal TotalMoney { get; set; }

        public List<Withdraw_QueryInfo> WithdrawList { get; set; }
    }

    /// <summary>
    /// 澳彩豆豆明细
    /// </summary>
    [CommunicationObject]
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

    [CommunicationObject]
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
    [CommunicationObject]
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
    [CommunicationObject]
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
