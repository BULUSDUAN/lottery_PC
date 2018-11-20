using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using GameBiz.Core;

namespace External.Core.Agnet
{
    /// <summary>
    /// 代理返点
    /// </summary>
    [CommunicationObject]
    public class OCAgentRebateInfo
    {
        public string UserId { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        /// <summary>
        /// 自身返点
        /// </summary>
        public decimal Rebate { get; set; }
        /// <summary>
        /// 返点类型:0:串关返点；1:单关返点；
        /// </summary>
        public int RebateType { get; set; }
        /// <summary>
        /// 下级用户默认返点
        /// </summary>
        public decimal SubUserRebate { get; set; }
        public DateTime CreateTime { get; set; }
    }

    [CommunicationObject]
    public class OCAgentRebateInfoCollection : List<OCAgentRebateInfo>
    {
    }

    /// <summary>
    /// 店面代理
    /// </summary>
    [CommunicationObject]
    public class StoreMessageInfo
    {
        public string UserId { get; set; }
        public string StoreId { get; set; }
        public string UserName { get; set; }
        public OCAgentCategory OCAgentCategory { get; set; }
        public string ParentUserId { get; set; }
        public string CustomerDomain { get; set; }
        public DateTime CreateTime { get; set; }
    }

    [CommunicationObject]
    public class StoreMessageInfoCollection : List<StoreMessageInfo>
    {
    }

    /// <summary>
    /// 代理类型
    /// </summary>
    [CommunicationObject]
    public class OCAgentType
    {
        public string UserId { get; set; }
        public string StoreId { get; set; }
        public OCAgentCategory OCAgentCategory { get; set; }
        public string ParentUserId { get; set; }
        public string CustomerDomain { get; set; }
        public DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 代理返点明细
    /// </summary>
    [CommunicationObject]
    public class OCAgentPayDetailInfo
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string SchemeId { get; set; }
        /// <summary>
        /// 订单用户（投注订单的用户）
        /// </summary>
        public string OrderUser { get; set; }
        /// <summary>
        /// 收入代理Id
        /// </summary>
        public string PayInUserId { get; set; }
        /// <summary>
        /// 订单用户显示名
        /// </summary>
        public string OrderUserDisplayName { get; set; }
        /// <summary>
        /// 方案类型
        /// </summary>
        public SchemeType SchemeType { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 玩法
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 返点
        /// </summary>
        public decimal Rebate { get; set; }
        /// <summary>
        /// 返利金额
        /// </summary>
        public decimal PayMoney { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// CPS模式
        /// </summary>
        public CPSMode CPSMode { get; set; }
        /// <summary>
        /// 处理人（用于提现返点，结算分红）
        /// </summary>
        public string HandlPeople { get; set; }
        public DateTime CreateTime { get; set; }
    }

    [CommunicationObject]
    public class OCAgentPayDetailInfoCollection
    {
        public OCAgentPayDetailInfoCollection()
        {
            DetailList = new List<OCAgentPayDetailInfo>();
        }

        public int TotalCount { get; set; }
        public List<OCAgentPayDetailInfo> DetailList { get; set; }
    }

    /// <summary>
    /// 结算报表
    /// </summary>
    [CommunicationObject]
    public class AgentPayDetailReportInfo
    {
        public string UserId { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 玩法
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 佣金金额
        /// </summary>
        public decimal CommissionMoney { get; set; }
    }

    [CommunicationObject]
    public class AgentPayDetailReportInfoCollection
    {
        public AgentPayDetailReportInfoCollection()
        {
            List = new List<AgentPayDetailReportInfo>();
        }

        public int TotalCount { get; set; }
        public IList<AgentPayDetailReportInfo> List { get; set; }
    }

    /// <summary>
    /// 代理对象
    /// </summary>
    [CommunicationObject]
    public class OCAgentInfo
    {
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public int SubUserCount { get; set; }
        public string RealName { get; set; }
        public string IdCard { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string RegTime { get; set; }
        public decimal CTZQMaxRebate { get; set; }
        public decimal BJDCMaxRebate { get; set; }
        public decimal JCZQMaxRebate { get; set; }
        public decimal JCLQMaxRebate { get; set; }
        public decimal SZCMaxRebate { get; set; }
    }
    [CommunicationObject]
    public class OCAgentInfoCollection
    {
        public OCAgentInfoCollection()
        {
            DetailList = new List<OCAgentInfo>();
        }

        public int TotalCount { get; set; }
        public List<OCAgentInfo> DetailList { get; set; }
    }

    /// <summary>
    /// 查询下线用户 未返点订单
    /// </summary>
    [CommunicationObject]
    public class SubUserNoPayRebateOrderInfo
    {
        public string UserId { get; set; }
        public string SchemeId { get; set; }
        public string DisplayName { get; set; }
        public string GameCode { get; set; }
        /// <summary>
        /// 方案进度百分比
        /// </summary>
        public decimal Progress { get; set; }
        /// <summary>
        /// 订单总金额
        /// </summary>
        public decimal TotalMonery { get; set; }
        public DateTime CreateTime { get; set; }
        public int HideDisplayNameCount { get; set; }
        public TicketStatus TicketStatus { get; set; }
    }
    [CommunicationObject]
    public class SubUserNoPayRebateOrderInfoCollection
    {
        public SubUserNoPayRebateOrderInfoCollection()
        {
            List = new List<SubUserNoPayRebateOrderInfo>();
        }

        public int TotalCount { get; set; }
        public int UserCount { get; set; }
        public decimal TotalMoney { get; set; }
        public List<SubUserNoPayRebateOrderInfo> List { get; set; }
    }

    /// <summary>
    /// 查询用户下线的合买信息
    /// </summary>
    [CommunicationObject]
    public class SubUserPayRebateOrderInfo
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户显示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string SchemeId { get; set; }
        /// <summary>
        /// 彩种编码
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 订单总金额
        /// </summary>
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 实际应执行返点金额（享受返点金额）
        /// </summary>
        public decimal RealPayRebateMoney { get; set; }
        public int HideDisplayNameCount { get; set; }
    }
    [CommunicationObject]
    public class SubUserPayRebateOrderInfoCollection
    {
        public SubUserPayRebateOrderInfoCollection()
        {
            List = new List<SubUserPayRebateOrderInfo>();
        }
        /// <summary>
        /// 数据总条数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 总用户数
        /// </summary>
        public int UserCount { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 总实际应执行返点金额
        /// </summary>
        public decimal TotalRealPayRebateMoney { get; set; }
        public List<SubUserPayRebateOrderInfo> List { get; set; }
    }

    [CommunicationObject]
    public class OCAagentDetailInfo
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 是否是代理
        /// </summary>
        public bool IsAgent { get; set; }
        /// <summary>
        /// 是否充值
        /// </summary>
        public bool IsFillMoney { get; set; }
        /// <summary>
        /// 上级代理
        /// </summary>
        public string AgentId { get; set; }
        /// <summary>
        /// 是否验证手机
        /// </summary>
        public bool IsSettedMobile { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 是否实名认证
        /// </summary>
        public bool IsSettedRealName { get; set; }
        /// <summary>
        /// 实名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// CPS模式
        /// </summary>
        public CPSMode CPSMode { get; set; }
    }

    [CommunicationObject]
    public class OCAagentDetailInfoCollection
    {
        public OCAagentDetailInfoCollection()
        {
            List = new List<OCAagentDetailInfo>();
        }
        /// <summary>
        /// 用户总条数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 总销量
        /// </summary>
        public decimal TotalBuyMoney { get; set; }
        /// <summary>
        /// 总中奖金额
        /// </summary>
        public decimal TotalBounsMoney { get; set; }
        /// <summary>
        /// 总加奖分红金额
        /// </summary>
        public decimal TotalRedBagAwardsMoney { get; set; }
        /// <summary>
        /// 总加奖现金金额
        /// </summary>
        public decimal TotalBonusAwardsMoney { get; set; }
        /// <summary>
        /// 总提现金额
        /// </summary>
        public decimal TotalWithdrawalsMoney { get; set; }
        /// <summary>
        /// 总充值
        /// </summary>
        public decimal TotalFillMoney { get; set; }
        public List<OCAagentDetailInfo> List { get; set; }
    }
}
