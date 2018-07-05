using EntityModel.Enum;
using System;
using System.Collections.Generic;

namespace EntityModel.CoreModel
{
    /// <summary>
    /// 经销商
    /// </summary>
    public class AgentUserInfo
    {
        /// <summary>
        /// 用户
        /// </summary>
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public string ComeFrom { get; set; }
        public string RegisterIp { get; set; }
        /// <summary>
        /// 经销商代理启用状态
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 是否经销商
        /// </summary>
        public bool IsAgent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 上级经销商代理编号
        /// </summary>
        public string AgentId { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Mobile
        /// </summary>
        public string Mobile { get; set; }
        public string RealName { get; set; }
        public string CardType { get; set; }
        public string IdCardNumber { get; set; }

        public decimal BJDC { get; set; }
        public decimal CTZQ { get; set; }
        public decimal GPC { get; set; }
        public decimal JCLQ { get; set; }
        public decimal JCZQ { get; set; }
        public decimal SZC { get; set; }

        public int UserCount { get; set; }

        public bool IsSettedMobile { get; set; }
        public decimal TotalBalanceMoney { get; set; }
        public bool IsSettedEmail { get; set; }
        public bool IsSettedRealName { get; set; }
    }

    /// <summary>
    /// 经销商用户
    /// </summary>
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
    public class AgentReturnPointInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 设置的人
        /// </summary>
        public string AgentIdFrom { get; set; }
        /// <summary>
        /// 被设置的人
        /// </summary>
        public string AgentIdTo { get; set; }
        /// <summary>
        /// 设置等级
        /// </summary>
        public int SetLevel { get; set; }
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
        public decimal ReturnPoint { get; set; }
    }
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
    public class AgentReturnPointRealityInfo
    {
        public string UserId { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public decimal UpPoint { get; set; }
        public decimal MyPoint { get; set; }
        public decimal LowerPoint { get; set; }
    }
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
    public class AgentReturnPointInitialInfo
    {
        public int ID { get; set; }
        public string UserId { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public decimal? MyPoint { get; set; }
        public decimal? LowerPoint { get; set; }
        public DateTime? LowerUpTime { get; set; }
        public DateTime? MyUpTime { get; set; }
    }
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
    public class AgentCommissionApplyInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime RequestTime { get; set; }
        /// <summary>
        /// 周期开始时间
        /// </summary>
        public DateTime FromTime { get; set; }
        /// <summary>
        /// 周期停止时间
        /// </summary>
        public DateTime ToTime { get; set; }
        /// <summary>
        /// 申请人ID
        /// </summary>
        public string RequestUserId { get; set; }
        /// <summary>
        /// 申请的佣金
        /// </summary>
        public decimal RequestCommission { get; set; }
        /// <summary>
        /// 响应的佣金
        /// </summary>
        public decimal ResponseCommission { get; set; }
        /// <summary>
        /// 结算人
        /// </summary>
        public string ResponseUserId { get; set; }
        /// <summary>
        /// 结算销量
        /// </summary>
        public decimal DealSale { get; set; }
        /// <summary>
        /// 结算时间
        /// </summary>
        public DateTime ResponseTime { get; set; }
        /// <summary>
        /// 申请状态，1：处理中，2：已处理，3：已拒绝
        /// </summary>
        public int ApplyState { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remark { get; set; }

        public string DisplayName { get; set; }

        public string AgentId { get; set; }

    }
    /// <summary>
    /// 佣金申请记录集合
    /// </summary>
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
    public class AgentCommissionDetailInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 玩法
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 申请状态 0：未申请，1：已申请
        /// </summary>
        public int ApplyState { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 上级用户Id
        /// </summary>
        public string PAgentId { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 分类 1：投注，2，申请
        /// </summary>
        public int Category { get; set; }
        /// <summary>
        /// 销量
        /// </summary>
        public decimal Sale { get; set; }
        /// <summary>
        /// 初始返点
        /// </summary>
        public decimal InitialPoint { get; set; }
        /// <summary>
        /// 下级的返点
        /// </summary>
        public decimal LowerPoint { get; set; }
        /// <summary>
        /// 实际返点
        /// </summary>
        public decimal ActualPoint { get; set; }
        /// <summary>
        /// 扣量
        /// </summary>
        public decimal Deduction { get; set; }
        /// <summary>
        /// 扣量前佣金
        /// </summary>
        public decimal BeforeCommission { get; set; }
        /// <summary>
        /// 实际佣金
        /// </summary>
        public decimal ActualCommission { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        public string DetailKeyword { get; set; }

        public DateTime ComplateDateTime { get; set; }

        public string DisplayName { get; set; }

    }
    /// <summary>
    /// 代理佣金明细集合
    /// </summary>
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


    public class AgentCloseReturnPointInfo
    {
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string AgentId { get; set; }
        public int SetLevel { get; set; }
        public decimal ReturnPoint { get; set; }
        public string PAgentId { get; set; }
        public int PSetLevel { get; set; }
        public decimal PReturnPoint { get; set; }
    }

    public class AgentWaitingCommissionOrderInfo
    {
        public string SchemeId { get; set; }
        public string UserId { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public SchemeType SchemeType { get; set; }
        public string IssuseNumber { get; set; }
        public decimal TotalMoney { get; set; }
        public decimal TotalBuyMoney { get; set; }
        public string AgentId { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ComplateTime { get; set; }
    }

    public class AgentLottoTopInfo
    {
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public string AgentId { get; set; }
        public bool IsAgent { get; set; }
        public decimal BJDC { get; set; }
        public decimal CTZQ { get; set; }
        public decimal GPC { get; set; }
        public decimal JCLQ { get; set; }
        public decimal JCZQ { get; set; }
        public decimal SZC { get; set; }
        public decimal TotalMoney { get; set; }
    }
    public class AgentLottoTopCollection
    {
        public AgentLottoTopCollection()
        {
            AgentLottoTopList = new List<AgentLottoTopInfo>();
        }

        public IList<AgentLottoTopInfo> AgentLottoTopList { get; set; }

        public int TotalCount { get; set; }
    }

    public class AgentFillMoneyTopInfo
    {
        public string UserId { get; set; }
        public string AgentId { get; set; }
        public bool IsAgent { get; set; }
        public string DisplayName { get; set; }
        public int FillTotalCount { get; set; }
        public decimal TotalMoney { get; set; }
    }
    public class AgentFillMoneyTopCollection
    {
        public AgentFillMoneyTopCollection()
        {
            AgentFillMoneyTopList = new List<AgentFillMoneyTopInfo>();
        }

        public IList<AgentFillMoneyTopInfo> AgentFillMoneyTopList { get; set; }

        public int TotalCount { get; set; }
    }

    public class AgentSchemeInfo
    {
        public string SchemeId { get; set; }
        public DateTime CreateTime { get; set; }
        public string UserId { get; set; }
        public string GameCode { get; set; }
        public string GameTypeName { get; set; }
        public int SchemeType { get; set; }
        public int SchemeSource { get; set; }
        public int SchemeBettingCategory { get; set; }
        public decimal CurrentBettingMoney { get; set; }
        public decimal TotalMoney { get; set; }
        public int ProgressStatus { get; set; }
        public int TicketStatus { get; set; }
        public int TotalIssuseCount { get; set; }
        public string StartIssuseNumber { get; set; }
        public string CurrentIssuseNumber { get; set; }
        public int BonusStatus { get; set; }
        public decimal PreTaxBonusMoney { get; set; }
        public decimal AfterTaxBonusMoney { get; set; }
        public bool StopAfterBonus { get; set; }
        public bool IsVirtualOrder { get; set; }
        public string AgentId { get; set; }
        public DateTime ComplateTime { get; set; }
        public string DisplayName { get; set; }
    }

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

    public class AgentWithdrawRecordInfo
    {
        public string KeyLine { get; set; }
        public string OrderId { get; set; }
        public string UserId { get; set; }
        public PayType PayType { get; set; }
        public AccountType AccountType { get; set; }
        public string Category { get; set; }
        public string Summary { get; set; }
        public decimal PayMoney { get; set; }
        public decimal BeforeBalance { get; set; }
        public decimal AfterBalance { get; set; }
        public DateTime CreateTime { get; set; }
        public WithdrawAgentType WithdrawAgent { get; set; }
        public WithdrawStatus Status { get; set; }
        public string ResponseMessage { get; set; }
    }

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

    public class AgentCommDetailByTotalSaleInfo
    {
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public decimal TotalSale { get; set; }
    }

    public class AgentCommDetailByTotalSaleCollection
    {
        public AgentCommDetailByTotalSaleCollection()
        {
            AgentCommDetailByTotalSaleList = new List<AgentCommDetailByTotalSaleInfo>();
        }
        public IList<AgentCommDetailByTotalSaleInfo> AgentCommDetailByTotalSaleList { get; set; }
    }

    public class SporeadUsersCollection
    {
        public SporeadUsersCollection()
        {
            BlogUserSpreadList = new List<BlogUserSpread>();
        }

        public IList<BlogUserSpread> BlogUserSpreadList { get; set; }

        public int TotalCount { get; set; }
    }

    public class BlogUserSpread
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string userName { get; set; }
        public string AgentId { get; set; }
        public DateTime CrateTime { get; set; }
        public decimal CTZQ { get; set; }
        public decimal BJDC { get; set; }
        public decimal JCZQ { get; set; }
        public decimal JCLQ { get; set; }
        public decimal SZC { get; set; }
        public decimal GPC { get; set; }
        public DateTime UpdateTime { get; set; }
    }


    // <summary>
    /// fxid 
    /// </summary>
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
    public class BlogUserShareSpread
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string AgentId { get; set; }
        public DateTime CreateTime { get; set; }
        public bool isGiveRegisterRedBag { get; set; }
        public bool isGiveLotteryRedBag { get; set; }
        public decimal giveRedBagMoney { get; set; }
        public DateTime UpdateTime { get; set; }
    }











}
