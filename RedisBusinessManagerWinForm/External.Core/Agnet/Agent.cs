using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using System.Collections;
using External.Core.Login;
using External.Core.Authentication;
using Common.Mappings;
using GameBiz.Core;
using Common;

namespace External.Core.Agnet
{
    /// <summary>
    /// 经销商
    /// </summary>
    [CommunicationObject]
    public class AgentUserInfo
    {
        /// <summary>
        /// 用户
        /// </summary>
        [EntityMappingField("UserId")]
        public string UserId { get; set; }
        [EntityMappingField("DisplayName")]
        public string DisplayName { get; set; }
        [EntityMappingField("ComeFrom")]
        public string ComeFrom { get; set; }
        [EntityMappingField("RegisterIp")]
        public string RegisterIp { get; set; }
        /// <summary>
        /// 经销商代理启用状态
        /// </summary>
        [EntityMappingField("IsEnable")]
        public bool IsEnable { get; set; }
        /// <summary>
        /// 是否经销商
        /// </summary>
        [EntityMappingField("IsAgent")]
        public bool IsAgent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [EntityMappingField("CreateTime")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 上级经销商代理编号
        /// </summary>
        [EntityMappingField("AgentId")]
        public string AgentId { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        [EntityMappingField("Email")]
        public string Email { get; set; }
        /// <summary>
        /// Mobile
        /// </summary>
        [EntityMappingField("Mobile")]
        public string Mobile { get; set; }
        [EntityMappingField("RealName")]
        public string RealName { get; set; }
        [EntityMappingField("CardType")]
        public string CardType { get; set; }
        [EntityMappingField("IdCardNumber")]
        public string IdCardNumber { get; set; }

        [EntityMappingField("BJDC")]
        public decimal BJDC { get; set; }
        [EntityMappingField("CTZQ")]
        public decimal CTZQ { get; set; }
        [EntityMappingField("GPC")]
        public decimal GPC { get; set; }
        [EntityMappingField("JCLQ")]
        public decimal JCLQ { get; set; }
        [EntityMappingField("JCZQ")]
        public decimal JCZQ { get; set; }
        [EntityMappingField("SZC")]
        public decimal SZC { get; set; }

        [EntityMappingField("UserCount")]
        public int UserCount { get; set; }

        [EntityMappingField("IsSettedMobile")]
        public bool IsSettedMobile { get; set; }
        [EntityMappingField("TotalBalanceMoney")]
        public decimal TotalBalanceMoney { get; set; }
        [EntityMappingField("IsSettedEmail")]
        public bool IsSettedEmail { get; set; }
        [EntityMappingField("IsSettedRealName")]
        public bool IsSettedRealName { get; set; }
    }

    /// <summary>
    /// 经销商用户
    /// </summary>
    [CommunicationObject]
    public class AgentUserInfoCollection
    {
        public AgentUserInfoCollection()
        {
            AgentUserList = new List<AgentUserInfo>();
        }

        public int TotalCount { get; set; }
        public IList<AgentUserInfo> AgentUserList { get; set; }
    }

    /// <summary>
    /// 设置返点
    /// </summary>
    [CommunicationObject]
    public class AgentReturnPointInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        [EntityMappingField("ID")]
        public int ID { get; set; }
        /// <summary>
        /// 设置的人
        /// </summary>
        [EntityMappingField("AgentIdFrom")]
        public string AgentIdFrom { get; set; }
        /// <summary>
        /// 被设置的人
        /// </summary>
        [EntityMappingField("AgentIdTo")]
        public string AgentIdTo { get; set; }
        /// <summary>
        /// 设置等级
        /// </summary>
        [EntityMappingField("SetLevel")]
        public int SetLevel { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        [EntityMappingField("GameCode")]
        public string GameCode { get; set; }
        /// <summary>
        /// 玩法
        /// </summary>
        [EntityMappingField("GameType")]
        public string GameType { get; set; }
        /// <summary>
        /// 返点
        /// </summary>
        [EntityMappingField("ReturnPoint")]
        public decimal ReturnPoint { get; set; }
    }
    [CommunicationObject]
    public class AgentReturnPointCollection
    {
        public AgentReturnPointCollection()
        {
            AgentReturnPointList = new List<AgentReturnPointInfo>();
            AgentReturnPointListByUp = new List<AgentReturnPointInfo>();
            AgentReturnPointListByLower = new List<AgentReturnPointInfo>();
        }

        public IList<AgentReturnPointInfo> AgentReturnPointList { get; set; }
        public bool IsAdmin { get; set; }

        public IList<AgentReturnPointInfo> AgentReturnPointListByUp { get; set; }
        public IList<AgentReturnPointInfo> AgentReturnPointListByLower { get; set; }
    }


    /// <summary>
    /// 设置返点（后面加上的）
    /// </summary>
    [CommunicationObject]
    public class AgentReturnPointRealityInfo
    {
        [EntityMappingField("UserId")]
        public string UserId { get; set; }
        [EntityMappingField("GameCode")]
        public string GameCode { get; set; }
        [EntityMappingField("GameType")]
        public string GameType { get; set; }
        [EntityMappingField("UpPoint")]
        public decimal UpPoint { get; set; }
        [EntityMappingField("MyPoint")]
        public decimal MyPoint { get; set; }
        [EntityMappingField("LowerPoint")]
        public decimal LowerPoint { get; set; }
    }
    [CommunicationObject]
    public class AgentReturnPointRealityCollection
    {
        public AgentReturnPointRealityCollection()
        {
            AgentReturnPointRealityList = new List<AgentReturnPointRealityInfo>();
        }

        public IList<AgentReturnPointRealityInfo> AgentReturnPointRealityList { get; set; }
    }
    /// <summary>
    /// 设置返点（后面加上的）
    /// </summary>
    [CommunicationObject]
    public class AgentReturnPointInitialInfo
    {
        [EntityMappingField("ID")]
        public int ID { get; set; }
        [EntityMappingField("UserId")]
        public string UserId { get; set; }
        [EntityMappingField("GameCode")]
        public string GameCode { get; set; }
        [EntityMappingField("GameType")]
        public string GameType { get; set; }
        [EntityMappingField("MyPoint")]
        public decimal? MyPoint { get; set; }
        [EntityMappingField("LowerPoint")]
        public decimal? LowerPoint { get; set; }
        [EntityMappingField("LowerUpTime")]
        public DateTime? LowerUpTime { get; set; }
        [EntityMappingField("MyUpTime")]
        public DateTime? MyUpTime { get; set; }
    }
    [CommunicationObject]
    public class AgentReturnPointInitialCollection
    {
        public AgentReturnPointInitialCollection()
        {
            AgentReturnPointInitialList = new List<AgentReturnPointInitialInfo>();
        }

        public IList<AgentReturnPointInitialInfo> AgentReturnPointInitialList { get; set; }
    }


    /// <summary>
    /// 佣金申请记录
    /// </summary>
    [CommunicationObject]
    public class AgentCommissionApplyInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        [EntityMappingField("ID")]
        public string ID { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        [EntityMappingField("RequestTime")]
        public DateTime RequestTime { get; set; }
        /// <summary>
        /// 周期开始时间
        /// </summary>
        [EntityMappingField("FromTime")]
        public DateTime FromTime { get; set; }
        /// <summary>
        /// 周期停止时间
        /// </summary>
        [EntityMappingField("ToTime")]
        public DateTime ToTime { get; set; }
        /// <summary>
        /// 申请人ID
        /// </summary>
        [EntityMappingField("RequestUserId")]
        public string RequestUserId { get; set; }
        /// <summary>
        /// 申请的佣金
        /// </summary>
        [EntityMappingField("RequestCommission")]
        public decimal RequestCommission { get; set; }
        /// <summary>
        /// 响应的佣金
        /// </summary>
        [EntityMappingField("ResponseCommission")]
        public decimal ResponseCommission { get; set; }
        /// <summary>
        /// 结算人
        /// </summary>
        [EntityMappingField("ResponseUserId")]
        public string ResponseUserId { get; set; }
        /// <summary>
        /// 结算销量
        /// </summary>
        [EntityMappingField("DealSale")]
        public decimal DealSale { get; set; }
        /// <summary>
        /// 结算时间
        /// </summary>
        [EntityMappingField("ResponseTime")]
        public DateTime ResponseTime { get; set; }
        /// <summary>
        /// 申请状态，1：处理中，2：已处理，3：已拒绝
        /// </summary>
        [EntityMappingField("ApplyState")]
        public int ApplyState { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        [EntityMappingField("Remark")]
        public string Remark { get; set; }

        [EntityMappingField("DisplayName")]
        public string DisplayName { get; set; }

        [EntityMappingField("AgentId")]
        public string AgentId { get; set; }

    }
    /// <summary>
    /// 佣金申请记录集合
    /// </summary>
    [CommunicationObject]
    public class AgentCommissionApplyCollection
    {
        public AgentCommissionApplyCollection()
        {
            AgentCommissionApplyList = new List<AgentCommissionApplyInfo>();
        }
        public IList<AgentCommissionApplyInfo> AgentCommissionApplyList { get; set; }
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// 代理佣金明细
    /// </summary>
    [CommunicationObject]
    public class AgentCommissionDetailInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        [EntityMappingField("ID")]
        public int ID { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        [EntityMappingField("GameCode")]
        public string GameCode { get; set; }
        /// <summary>
        /// 玩法
        /// </summary>
        [EntityMappingField("GameType")]
        public string GameType { get; set; }
        /// <summary>
        /// 申请状态 0：未申请，1：已申请
        /// </summary>
        [EntityMappingField("ApplyState")]
        public int ApplyState { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [EntityMappingField("CreateTime")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 上级用户Id
        /// </summary>
        [EntityMappingField("PAgentId")]
        public string PAgentId { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        [EntityMappingField("UserId")]
        public string UserId { get; set; }
        /// <summary>
        /// 分类 1：投注，2，申请
        /// </summary>
        [EntityMappingField("Category")]
        public int Category { get; set; }
        /// <summary>
        /// 销量
        /// </summary>
        [EntityMappingField("Sale")]
        public decimal Sale { get; set; }
        /// <summary>
        /// 初始返点
        /// </summary>
        [EntityMappingField("InitialPoint")]
        public decimal InitialPoint { get; set; }
        /// <summary>
        /// 下级的返点
        /// </summary>
        [EntityMappingField("LowerPoint")]
        public decimal LowerPoint { get; set; }
        /// <summary>
        /// 实际返点
        /// </summary>
        [EntityMappingField("ActualPoint")]
        public decimal ActualPoint { get; set; }
        /// <summary>
        /// 扣量
        /// </summary>
        [EntityMappingField("Deduction")]
        public decimal Deduction { get; set; }
        /// <summary>
        /// 扣量前佣金
        /// </summary>
        [EntityMappingField("BeforeCommission")]
        public decimal BeforeCommission { get; set; }
        /// <summary>
        /// 实际佣金
        /// </summary>
        [EntityMappingField("ActualCommission")]
        public decimal ActualCommission { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        [EntityMappingField("Remark")]
        public string Remark { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        [EntityMappingField("DetailKeyword")]
        public string DetailKeyword { get; set; }

        [EntityMappingField("ComplateDateTime")]
        public DateTime ComplateDateTime { get; set; }

        [EntityMappingField("DisplayName")]
        public string DisplayName { get; set; }

    }
    /// <summary>
    /// 代理佣金明细集合
    /// </summary>
    [CommunicationObject]
    public class AgentCommissionDetailCollection
    {
        public AgentCommissionDetailCollection()
        {
            AgentCommissionDetailList = new List<AgentCommissionDetailInfo>();
            AgentCommissionReport = new List<AgentCommissionDetailInfo>();
        }

        public IList<AgentCommissionDetailInfo> AgentCommissionDetailList { get; set; }
        public IList<AgentCommissionDetailInfo> AgentCommissionReport { get; set; }

        public int TotalCount { get; set; }

        public decimal SaleTotal { get; set; }
        public decimal ActualCommissionTotal { get; set; }
    }

    ///// <summary>
    ///// 佣金申请结算记录
    ///// </summary>
    //public class AgentApplyCloseInfo
    //{
    //    /// <summary>
    //    /// ID
    //    /// </summary>
    //    public string ID { get; set; }
    //    /// <summary>
    //    /// 申请Id号
    //    /// </summary>
    //    public string ApplyId { get; set; }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string GameCode { get; set; }
    //    public string GameType { get; set; }
    //    public string Sale { get; set; }
    //    public string Commission { get; set; }
    //}


    [CommunicationObject]
    public class AgentCloseReturnPointInfo
    {
        [EntityMappingField("GameCode")]
        public string GameCode { get; set; }
        [EntityMappingField("GameType")]
        public string GameType { get; set; }
        [EntityMappingField("AgentId")]
        public string AgentId { get; set; }
        [EntityMappingField("SetLevel")]
        public int SetLevel { get; set; }
        [EntityMappingField("ReturnPoint")]
        public decimal ReturnPoint { get; set; }
        [EntityMappingField("PAgentId")]
        public string PAgentId { get; set; }
        [EntityMappingField("PSetLevel")]
        public int PSetLevel { get; set; }
        [EntityMappingField("PReturnPoint")]
        public decimal PReturnPoint { get; set; }
    }

    [CommunicationObject]
    public class AgentWaitingCommissionOrderInfo
    {
        [EntityMappingField("SchemeId")]
        public string SchemeId { get; set; }
        [EntityMappingField("UserId")]
        public string UserId { get; set; }
        [EntityMappingField("GameCode")]
        public string GameCode { get; set; }
        [EntityMappingField("GameType")]
        public string GameType { get; set; }
        [EntityMappingField("SchemeType")]
        public SchemeType SchemeType { get; set; }
        [EntityMappingField("IssuseNumber")]
        public string IssuseNumber { get; set; }
        [EntityMappingField("TotalMoney")]
        public decimal TotalMoney { get; set; }
        [EntityMappingField("TotalBuyMoney")]
        public decimal TotalBuyMoney { get; set; }
        [EntityMappingField("AgentId")]
        public string AgentId { get; set; }
        [EntityMappingField("CreateTime")]
        public DateTime CreateTime { get; set; }
        [EntityMappingField("ComplateTime")]
        public DateTime ComplateTime { get; set; }
    }

    [CommunicationObject]
    public class AgentLottoTopInfo
    {
        [EntityMappingField("UserId")]
        public string UserId { get; set; }
        [EntityMappingField("DisplayName")]
        public string DisplayName { get; set; }
        [EntityMappingField("AgentId")]
        public string AgentId { get; set; }
        [EntityMappingField("IsAgent")]
        public bool IsAgent { get; set; }
        [EntityMappingField("BJDC")]
        public decimal BJDC { get; set; }
        [EntityMappingField("CTZQ")]
        public decimal CTZQ { get; set; }
        [EntityMappingField("GPC")]
        public decimal GPC { get; set; }
        [EntityMappingField("JCLQ")]
        public decimal JCLQ { get; set; }
        [EntityMappingField("JCZQ")]
        public decimal JCZQ { get; set; }
        [EntityMappingField("SZC")]
        public decimal SZC { get; set; }
        [EntityMappingField("TotalMoney")]
        public decimal TotalMoney { get; set; }
    }
    [CommunicationObject]
    public class AgentLottoTopCollection
    {
        public AgentLottoTopCollection()
        {
            AgentLottoTopList = new List<AgentLottoTopInfo>();
        }

        public IList<AgentLottoTopInfo> AgentLottoTopList { get; set; }

        public int TotalCount { get; set; }
    }

    [CommunicationObject]
    public class AgentFillMoneyTopInfo
    {
        [EntityMappingField("UserId")]
        public string UserId { get; set; }
        [EntityMappingField("AgentId")]
        public string AgentId { get; set; }
        [EntityMappingField("IsAgent")]
        public bool IsAgent { get; set; }
        [EntityMappingField("DisplayName")]
        public string DisplayName { get; set; }
        [EntityMappingField("FillTotalCount")]
        public int FillTotalCount { get; set; }
        [EntityMappingField("TotalMoney")]
        public decimal TotalMoney { get; set; }
    }
    [CommunicationObject]
    public class AgentFillMoneyTopCollection
    {
        public AgentFillMoneyTopCollection()
        {
            AgentFillMoneyTopList = new List<AgentFillMoneyTopInfo>();
        }

        public IList<AgentFillMoneyTopInfo> AgentFillMoneyTopList { get; set; }

        public int TotalCount { get; set; }
    }

    [CommunicationObject]
    public class AgentSchemeInfo
    {
        [EntityMappingField("SchemeId")]
        public string SchemeId { get; set; }
        [EntityMappingField("CreateTime")]
        public DateTime CreateTime { get; set; }
        [EntityMappingField("UserId")]
        public string UserId { get; set; }
        [EntityMappingField("GameCode")]
        public string GameCode { get; set; }
        [EntityMappingField("GameTypeName")]
        public string GameTypeName { get; set; }
        [EntityMappingField("SchemeType")]
        public int SchemeType { get; set; }
        [EntityMappingField("SchemeSource")]
        public int SchemeSource { get; set; }
        [EntityMappingField("SchemeBettingCategory")]
        public int SchemeBettingCategory { get; set; }
        [EntityMappingField("CurrentBettingMoney")]
        public decimal CurrentBettingMoney { get; set; }
        [EntityMappingField("TotalMoney")]
        public decimal TotalMoney { get; set; }
        [EntityMappingField("ProgressStatus")]
        public int ProgressStatus { get; set; }
        [EntityMappingField("TicketStatus")]
        public int TicketStatus { get; set; }
        [EntityMappingField("TotalIssuseCount")]
        public int TotalIssuseCount { get; set; }
        [EntityMappingField("StartIssuseNumber")]
        public string StartIssuseNumber { get; set; }
        [EntityMappingField("CurrentIssuseNumber")]
        public string CurrentIssuseNumber { get; set; }
        [EntityMappingField("BonusStatus")]
        public int BonusStatus { get; set; }
        [EntityMappingField("PreTaxBonusMoney")]
        public decimal PreTaxBonusMoney { get; set; }
        [EntityMappingField("AfterTaxBonusMoney")]
        public decimal AfterTaxBonusMoney { get; set; }
        [EntityMappingField("StopAfterBonus")]
        public bool StopAfterBonus { get; set; }
        [EntityMappingField("IsVirtualOrder")]
        public bool IsVirtualOrder { get; set; }
        [EntityMappingField("AgentId")]
        public string AgentId { get; set; }
        [EntityMappingField("ComplateTime")]
        public DateTime ComplateTime { get; set; }
        [EntityMappingField("DisplayName")]
        public string DisplayName { get; set; }
    }

    [CommunicationObject]
    public class AgentSchemeCollection
    {
        public AgentSchemeCollection()
        {
            AgentSchemeList = new List<AgentSchemeInfo>();
        }

        public IList<AgentSchemeInfo> AgentSchemeList { get; set; }

        public int TotalCount { get; set; }
        public int TotalUser { get; set; }
        public int TotalScheme { get; set; }
        public decimal TotalMoney1 { get; set; }

    }

    [CommunicationObject]
    public class AgentWithdrawRecordInfo
    {
        [EntityMappingField("KeyLine")]
        public string KeyLine { get; set; }
        [EntityMappingField("OrderId")]
        public string OrderId { get; set; }
        [EntityMappingField("UserId")]
        public string UserId { get; set; }
        [EntityMappingField("PayType")]
        public PayType PayType { get; set; }
        [EntityMappingField("AccountType")]
        public AccountType AccountType { get; set; }
        [EntityMappingField("Category")]
        public string Category { get; set; }
        [EntityMappingField("Summary")]
        public string Summary { get; set; }
        [EntityMappingField("PayMoney")]
        public decimal PayMoney { get; set; }
        [EntityMappingField("BeforeBalance")]
        public decimal BeforeBalance { get; set; }
        [EntityMappingField("AfterBalance")]
        public decimal AfterBalance { get; set; }
        [EntityMappingField("CreateTime")]
        public DateTime CreateTime { get; set; }
        [EntityMappingField("WithdrawAgent")]
        public WithdrawAgentType WithdrawAgent { get; set; }
        [EntityMappingField("Status")]
        public WithdrawStatus Status { get; set; }
        [EntityMappingField("ResponseMessage")]
        public string ResponseMessage { get; set; }
    }

    [CommunicationObject]
    public class AgentWithdrawRecordCollection
    {
        public AgentWithdrawRecordCollection()
        {
            AgentWithdrawRecordList = new List<AgentWithdrawRecordInfo>();
        }

        public IList<AgentWithdrawRecordInfo> AgentWithdrawRecordList { get; set; }
        public int TotalCount { get; set; }

        /// <summary>
        /// 请求中
        /// </summary>
        public decimal RequestingMoney { get; set; }
        /// <summary>
        /// 已结算
        /// </summary>
        public decimal SuccessMoney { get; set; }
        /// <summary>
        /// 被拒绝
        /// </summary>
        public decimal RefusedMoney { get; set; }
    }

    [CommunicationObject]
    public class AgentCommDetailByTotalSaleInfo
    {
        [EntityMappingField("UserId")]
        public string UserId { get; set; }
        [EntityMappingField("DisplayName")]
        public string DisplayName { get; set; }
        [EntityMappingField("TotalSale")]
        public decimal TotalSale { get; set; }
    }

    [CommunicationObject]
    public class AgentCommDetailByTotalSaleCollection
    {
        public AgentCommDetailByTotalSaleCollection()
        {
            AgentCommDetailByTotalSaleList = new List<AgentCommDetailByTotalSaleInfo>();
        }
        public IList<AgentCommDetailByTotalSaleInfo> AgentCommDetailByTotalSaleList { get; set; }
    }

    [CommunicationObject]
    public class SporeadUsersCollection
    {
        public SporeadUsersCollection()
        {
            BlogUserSpreadList = new List<BlogUserSpread>();
        }

        public IList<BlogUserSpread> BlogUserSpreadList { get; set; }

        public int TotalCount { get; set; }
    }

    [CommunicationObject]
    public class BlogUserSpread
    {
        [EntityMappingField("Id")]
        public int Id { get; set; }
        [EntityMappingField("UserId")]
        public string UserId { get; set; }
        [EntityMappingField("userName")]
        public string userName { get; set; }
        [EntityMappingField("AgentId")]
        public string AgentId { get; set; }
        [EntityMappingField("CrateTime")]
        public DateTime CrateTime { get; set; }
        [EntityMappingField("CTZQ")]
        public decimal CTZQ { get; set; }
        [EntityMappingField("BJDC")]
        public decimal BJDC { get; set; }
        [EntityMappingField("JCZQ")]
        public decimal JCZQ { get; set; }
        [EntityMappingField("JCLQ")]
        public decimal JCLQ { get; set; }
        [EntityMappingField("SZC")]
        public decimal SZC { get; set; }
        [EntityMappingField("GPC")]
        public decimal GPC { get; set; }
        [EntityMappingField("UpdateTime")]
        public DateTime UpdateTime { get; set; }
    }


    // <summary>
    /// fxid 
    /// </summary>
    [CommunicationObject]
    public class ShareSpreadCollection
    {
        public ShareSpreadCollection()
        {
            ShareSpreadList = new List<BlogUserShareSpread>();
        }

        public IList<BlogUserShareSpread> ShareSpreadList { get; set; }

        /// <summary>
        /// 总红包
        /// </summary>
        public decimal RedBagMoneyTotal { get; set; }

        /// <summary>
        /// 总人数
        /// </summary>
        public int UserTotal { get; set; }
    }
    /// <summary>
    /// fxid 
    /// </summary>
    [CommunicationObject]
    public class BlogUserShareSpread
    {
        [EntityMappingField("Id")]
        public int Id { get; set; }
        [EntityMappingField("UserId")]
        public string UserId { get; set; }
        [EntityMappingField("UserName")]
        public string UserName { get; set; }
        [EntityMappingField("AgentId")]
        public string AgentId { get; set; }
        [EntityMappingField("CreateTime")]
        public DateTime CreateTime { get; set; }
        [EntityMappingField("isGiveRegisterRedBag")]
        public bool isGiveRegisterRedBag { get; set; }
        [EntityMappingField("isGiveLotteryRedBag")]
        public bool isGiveLotteryRedBag { get; set; }
        [EntityMappingField("giveRedBagMoney")]
        public decimal giveRedBagMoney { get; set; }
        [EntityMappingField("UpdateTime")]
        public DateTime UpdateTime { get; set; }
    }











}
