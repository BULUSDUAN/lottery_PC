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
    #region 我的投注记录信息
    [CommunicationObject]
    public class MyBettingOrderInfo
    {
        // 行号
        [EntityMappingField("RowNumber")]
        public long RowNumber { get; set; }
        // 方案号
        [EntityMappingField("SchemeId")]
        public string SchemeId { get; set; }
        // 方案创建者编号
        [EntityMappingField("UserId")]
        public string UserId { get; set; }
        // 方案创建者VIP级别
        [EntityMappingField("VipLevel")]
        public int VipLevel { get; set; }
        // 方案创建者显示名称
        [EntityMappingField("CreatorDisplayName")]
        public string CreatorDisplayName { get; set; }
        // 方案创建者是否隐藏显示名称
        [EntityMappingField("HideDisplayNameCount")]
        public int HideDisplayNameCount { get; set; }
        // 参与者编号
        [EntityMappingField("JoinUserId")]
        public string JoinUserId { get; set; }
        // 是否参与成功
        [EntityMappingField("JoinSucess")]
        public string JoinSucessString { get; set; }
        public bool JoinSucess
        {
            get { return JoinSucessString == "1"; }
            set { JoinSucessString = value ? "1" : "0"; }
        }
        // 彩种
        [EntityMappingField("GameCode")]
        public string GameCode { get; set; }
        // 玩法名称
        [EntityMappingField("GameTypeName")]
        public string GameTypeName { get; set; }
        // 方案类型
        [EntityMappingField("SchemeType")]
        public SchemeType SchemeType { get; set; }
        // 方案来源
        [EntityMappingField("SchemeSource")]
        public SchemeSource SchemeSource { get; set; }
        // 方案投注方案
        [EntityMappingField("SchemeBettingCategory")]
        public SchemeBettingCategory SchemeBettingCategory { get; set; }
        // 认购时间
        [EntityMappingField("BuyTime")]
        public DateTime BuyTime { get; set; }
        // 认购金额
        [EntityMappingField("BuyMoney")]
        public decimal BuyMoney { get; set; }
        // 方案总金额
        [EntityMappingField("TotalMoney")]
        public decimal TotalMoney { get; set; }
        // 方案进度
        [EntityMappingField("ProgressStatus")]
        public ProgressStatus ProgressStatus { get; set; }
        // 方案出票状态
        [EntityMappingField("TicketStatus")]
        public TicketStatus TicketStatus { get; set; }
        // 购买期数
        [EntityMappingField("TotalIssuseCount")]
        public int TotalIssuseCount { get; set; }
        // 购买期号
        [EntityMappingField("IssuseNumber")]
        public string IssuseNumber { get; set; }
        // 是否虚拟订单
        [EntityMappingField("IsVirtualOrder")]
        public bool IsVirtualOrder { get; set; }
        // 中奖状态
        [EntityMappingField("BonusStatus")]
        public BonusStatus BonusStatus { get; set; }
        // 税前奖金
        [EntityMappingField("PreTaxBonusMoney")]
        public decimal PreTaxBonusMoney { get; set; }
        // 税后奖金
        [EntityMappingField("AfterTaxBonusMoney")]
        public decimal AfterTaxBonusMoney { get; set; }
        // 中奖后停止
        [EntityMappingField("StopAfterBonus")]
        public bool StopAfterBonus { get; set; }
        /// <summary>
        /// 投注时间
        /// </summary>
        [EntityMappingField("BetTime")]
        public DateTime BetTime { get; set; }
        /// <summary>
        /// 彩种玩法
        /// </summary>
        [EntityMappingField("GameType")]
        public string GameType { get; set; }
        [EntityMappingField("AddMoney")]
        public decimal AddMoney { get; set; }
        [EntityMappingField("RedBagAwardsMoney")]
        public decimal RedBagAwardsMoney { get; set; }
        [EntityMappingField("BonusAwardsMoney")]
        public decimal BonusAwardsMoney { get; set; }
    }
    [CommunicationObject]
    public class MyBettingOrderInfoCollection
    {
        public int TotalCount { get; set; }
        public decimal TotalBuyMoney { get; set; }
        public decimal TotalBonusMoney { get; set; }

        public IList<MyBettingOrderInfo> OrderList { get; set; }
    }
    #endregion

    #region 投注订单信息
    [CommunicationObject]
    public class BettingOrderInfo
    {
        // 行号
        [EntityMappingField("RowNumber")]
        public long RowNumber { get; set; }
        // 方案号
        [EntityMappingField("SchemeId")]
        public string SchemeId { get; set; }
        // 方案创建者编号
        [EntityMappingField("UserId")]
        public string UserId { get; set; }
        // 方案创建者VIP级别
        [EntityMappingField("VipLevel")]
        public int VipLevel { get; set; }
        // 方案创建者显示名称
        [EntityMappingField("CreatorDisplayName")]
        public string CreatorDisplayName { get; set; }
        // 方案创建者是否隐藏显示名称
        [EntityMappingField("HideDisplayNameCount")]
        public int HideDisplayNameCount { get; set; }
        // 彩种
        [EntityMappingField("GameCode")]
        public string GameCode { get; set; }
        // 彩种名称
        [EntityMappingField("GameName")]
        public string GameName { get; set; }
        // 玩法名称
        [EntityMappingField("GameTypeName")]
        public string GameTypeName { get; set; }
        //过关方式
        [EntityMappingField("PlayType")]
        public string PlayType { get; set; }
        //倍数
        [EntityMappingField("Amount")]
        public int Amount { get; set; }
        // 方案类型
        [EntityMappingField("SchemeType")]
        public SchemeType SchemeType { get; set; }
        // 方案来源
        [EntityMappingField("SchemeSource")]
        public SchemeSource SchemeSource { get; set; }
        // 方案投注方案
        [EntityMappingField("SchemeBettingCategory")]
        public SchemeBettingCategory SchemeBettingCategory { get; set; }
        // 方案投注方案
        [EntityMappingField("Security")]
        public TogetherSchemeSecurity Security { get; set; }
        // 当前投注金额
        [EntityMappingField("CurrentBettingMoney")]
        public decimal CurrentBettingMoney { get; set; }
        // 方案总金额
        [EntityMappingField("TotalMoney")]
        public decimal TotalMoney { get; set; }
        // 中奖号码
        [EntityMappingField("WinNumber")]
        public string WinNumber { get; set; }
        // 方案进度
        [EntityMappingField("ProgressStatus")]
        public ProgressStatus ProgressStatus { get; set; }
        // 方案出票状态
        [EntityMappingField("TicketStatus")]
        public TicketStatus TicketStatus { get; set; }
        // 购买期数
        [EntityMappingField("TotalIssuseCount")]
        public int TotalIssuseCount { get; set; }
        // 购买期号
        [EntityMappingField("CurrentIssuseNumber")]
        public string IssuseNumber { get; set; }
        // 中奖状态
        [EntityMappingField("BonusStatus")]
        public BonusStatus BonusStatus { get; set; }
        // 中奖后停止
        [EntityMappingField("StopAfterBonus")]
        public bool StopAfterBonus { get; set; }
        // 税前奖金
        [EntityMappingField("PreTaxBonusMoney")]
        public decimal PreTaxBonusMoney { get; set; }
        // 税后奖金
        [EntityMappingField("AfterTaxBonusMoney")]
        public decimal AfterTaxBonusMoney { get; set; }
        // 加奖奖金
        [EntityMappingField("AddMoney")]
        public decimal AddMoney { get; set; }
        // 创建时间
        [EntityMappingField("CreateTime")]
        public DateTime CreateTime { get; set; }
        // 所属经销商
        [EntityMappingField("AgentId")]
        public string AgentId { get; set; }
        [EntityMappingField("IsVirtualOrder")]
        public bool IsVirtualOrder { get; set; }
        [EntityMappingField("BetTime")]
        public DateTime BetTime { get; set; }
        [EntityMappingField("RedBagMoney")]
        public decimal RedBagMoney { get; set; }
        [EntityMappingField("RealPayRebateMoney")]
        public decimal RealPayRebateMoney { get; set; }
        [EntityMappingField("RedBagAwardsMoney")]
        public decimal RedBagAwardsMoney { get; set; }
        [EntityMappingField("BonusAwardsMoney")]
        public decimal BonusAwardsMoney { get; set; }
    }
    [CommunicationObject]
    public class BettingOrderInfoCollection
    {
        public int TotalCount { get; set; }
        public int TotalUserCount { get; set; }
        public decimal TotalBuyMoney { get; set; }
        public decimal TotalOrderMoney { get; set; }
        public decimal TotalPreTaxBonusMoney { get; set; }
        public decimal TotalAfterTaxBonusMoney { get; set; }
        public decimal TotalAddMoney { get; set; }
        public decimal TotalRedbagMoney { get; set; }
        public decimal TotalRealPayRebateMoney { get; set; }
        public decimal TotalRedBagAwardsMoney { get; set; }
        public decimal TotalBonusAwardsMoney { get; set; }

        public IList<BettingOrderInfo> OrderList { get; set; }
    }
    #endregion

    #region 投注号码
    [CommunicationObject]
    public class BettingAnteCodeInfo
    {
        [EntityMappingField("SchemeId")]
        public string SchemeId { get; set; }
        [EntityMappingField("GameCode")]
        public string GameCode { get; set; }
        [EntityMappingField("GameName")]
        public string GameName { get; set; }
        [EntityMappingField("GameType")]
        public string GameType { get; set; }
        [EntityMappingField("GameTypeName")]
        public string GameTypeName { get; set; }
        [EntityMappingField("IssuseNumber")]
        public string IssuseNumber { get; set; }
        [EntityMappingField("AnteCode")]
        public string AnteCode { get; set; }
        [EntityMappingField("BonusStatus")]
        public BonusStatus BonusStatus { get; set; }
        [EntityMappingField("CreateTime")]
        public DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class BettingAnteCodeInfoCollection
    {
        public IList<BettingAnteCodeInfo> AnteCodeList { get; set; }
    }
    #endregion

    #region 合买订单信息
    [CommunicationObject]
    public class TogetherOrderInfo
    {
        // 行号
        [EntityMappingField("RowIndex")]
        public long RowIndex { get; set; }
        // 方案号
        [EntityMappingField("SchemeId")]
        public string SchemeId { get; set; }
        // 方案创建者编号
        [EntityMappingField("UserId")]
        public string UserId { get; set; }
        // 方案创建者VIP级别
        [EntityMappingField("VipLevel")]
        public int VipLevel { get; set; }
        // 方案创建者显示名称
        [EntityMappingField("DisplayName")]
        public string CreatorDisplayName { get; set; }
        // 方案创建者是否隐藏显示名称
        [EntityMappingField("HideDisplayNameCount")]
        public int HideDisplayNameCount { get; set; }
        // 彩种
        [EntityMappingField("GameCode")]
        public string GameCode { get; set; }
        // 彩种
        [EntityMappingField("GameType")]
        public string GameType { get; set; }
        // 玩法名称
        [EntityMappingField("GameTypeName")]
        public string GameTypeName { get; set; }
        // 方案类型
        [EntityMappingField("SchemeType")]
        public SchemeType SchemeType { get; set; }
        // 方案总金额
        [EntityMappingField("TotalMoney")]
        public decimal TotalMoney { get; set; }
        // 参与合买金额
        [EntityMappingField("JoinMoney")]
        public decimal JoinMoney { get; set; }
        // 合买进度
        [EntityMappingField("TogetherSchemeProgress")]
        public TogetherSchemeProgress TogetherSchemeProgress { get; set; }
        // 合买进度金额
        [EntityMappingField("Progress")]
        public decimal Progress { get; set; }
        // 方案进度
        [EntityMappingField("ProgressStatus")]
        public ProgressStatus ProgressStatus { get; set; }
        // 方案出票状态
        [EntityMappingField("TicketStatus")]
        public TicketStatus TicketStatus { get; set; }
        // 购买期号
        [EntityMappingField("StartIssuseNumber")]
        public string IssuseNumber { get; set; }
        // 购买期号
        [EntityMappingField("IsVirtualOrder")]
        public bool IsVirtualOrder { get; set; }
        // 是否已派奖
        [EntityMappingField("IsPrizeMoney")]
        public bool IsPrizeMoney { get; set; }
        // 中奖状态
        [EntityMappingField("BonusStatus")]
        public BonusStatus BonusStatus { get; set; }
        // 税前奖金
        [EntityMappingField("PreTaxBonusMoney")]
        public decimal PreTaxBonusMoney { get; set; }
        // 税后奖金
        [EntityMappingField("AfterTaxBonusMoney")]
        public decimal AfterTaxBonusMoney { get; set; }
        // 创建时间
        [EntityMappingField("CreateTime")]
        public DateTime CreateTime { get; set; }
        //投注类别
        [EntityMappingField("SchemeBettingCategory")]
        public SchemeBettingCategory SchemeBettingCategory { get; set; }
        //嘉奖
        [EntityMappingField("AddMoney")]
        public decimal AddMoney { get; set; }

    }
    [CommunicationObject]
    public class TogetherOrderInfoCollection
    {
        public int TotalCount { get; set; }
        public decimal TotalBuyMoney { get; set; }
        public decimal TotalOrderMoney { get; set; }

        public IList<TogetherOrderInfo> OrderList { get; set; }
    }
    [CommunicationObject]
    public class TogetherReportInfoGroupByUserInfo
    {
        // 方案创建者编号
        [EntityMappingField("UserId")]
        public string UserId { get; set; }
        // 方案创建者VIP级别
        [EntityMappingField("VipLevel")]
        public int VipLevel { get; set; }
        // 方案创建者显示名称
        [EntityMappingField("CreatorDisplayName")]
        public string CreatorDisplayName { get; set; }
        // 方案创建者是否隐藏显示名称
        [EntityMappingField("HideDisplayNameCount")]
        public int HideDisplayNameCount { get; set; }

        [EntityMappingField("TotalOrderCount_Success")]
        public int TotalOrderCount_Success { get; set; }
        [EntityMappingField("TotalBuyMoney_Success")]
        public decimal TotalBuyMoney_Success { get; set; }
        [EntityMappingField("TotalOrderMoney_Success")]
        public decimal TotalOrderMoney_Success { get; set; }

        [EntityMappingField("TotalOrderCount_Fail")]
        public int TotalOrderCount_Fail { get; set; }
        [EntityMappingField("TotalBuyMoney_Fail")]
        public decimal TotalBuyMoney_Fail { get; set; }
        [EntityMappingField("TotalOrderMoney_Fail")]
        public decimal TotalOrderMoney_Fail { get; set; }
    }
    [CommunicationObject]
    public class TogetherReportInfoGroupByUserCollection
    {
        public IList<TogetherReportInfoGroupByUserInfo> ReportList { get; set; }
    }
    #endregion

    #region 票信息

    [CommunicationObject]
    public class Sports_TicketQueryInfo
    {
        public string SchemeId { get; set; }
        public string TicketId { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string PlayType { get; set; }
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 注数
        /// </summary>
        public int BetUnits { get; set; }
        /// <summary>
        /// 倍数
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 票金额
        /// </summary>
        public decimal BetMoney { get; set; }
        public string BetContent { get; set; }
        public string LocOdds { get; set; }
        public TicketStatus TicketStatus { get; set; }
        /// <summary>
        /// 票号 1
        /// </summary>
        public string PrintNumber1 { get; set; }
        /// <summary>
        /// 票号 2
        /// </summary>
        public string PrintNumber2 { get; set; }
        /// <summary>
        /// 票号 3
        /// </summary>
        public string PrintNumber3 { get; set; }
        /// <summary>
        /// 条形码
        /// </summary>
        public string BarCode { get; set; }

        public BonusStatus BonusStatus { get; set; }

        public decimal PreTaxBonusMoney { get; set; }
        public decimal AfterTaxBonusMoney { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime? PrintDateTime { get; set; }
    }

    [CommunicationObject]
    public class Sports_TicketQueryInfoCollection
    {
        public int TotalCount { get; set; }
        public List<Sports_TicketQueryInfo> TicketList { get; set; }
    }
    #endregion

    /// <summary>
    /// 票查询结果
    /// </summary>
    [CommunicationObject]
    public class QueryTicketInfo
    {
        public string SchemeId { get; set; }
        public string TicketId { get; set; }
        public string UserId { get; set; }
        public bool IsAllFail { get; set; }
        public decimal TotalMoney { get; set; }
        public decimal TotalErrorMoney { get; set; }
        public bool SaveComplate { get; set; }
    }

    #region 延误开奖订单列表

    /// <summary>
    /// 延误开奖订单列表
    /// </summary>
    [CommunicationObject]
    public class DelayPrizeOrderInfo
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string SchemeId { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public decimal TotalMoney { get; set; }
        public decimal SuccessMoney { get; set; }
        public string PlayType { get; set; }
        public string IssuseNumber { get; set; }
        public int Amount { get; set; }
        public int BetCount { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime BetTime { get; set; }
        public SchemeType SchemeType { get; set; }
        public SchemeSource SchemeSource { get; set; }
        public SchemeBettingCategory SchemeBettingCategory { get; set; }
        public TicketStatus TicketStatus { get; set; }
        public ProgressStatus ProgressStatus { get; set; }
        public BonusStatus BonusStatus { get; set; }
    }
    [CommunicationObject]
    public class DelayPrizeOrder_Collection
    {
        public DelayPrizeOrder_Collection()
        {
            DelayPrizeOrderList = new List<DelayPrizeOrderInfo>();
        }
        public int TotalCount { get; set; }
        public List<DelayPrizeOrderInfo> DelayPrizeOrderList { get; set; }
    }

    #endregion


    #region 2017手机接口订单对象

    /// <summary>
    /// 订单列表对象
    /// </summary>
    [CommunicationObject]
    public class MyOrderListInfo
    {
        // 方案号
        public string SchemeId { get; set; }
        // 彩种
        public string GameCode { get; set; }
        // 玩法名称
        public string GameTypeName { get; set; }
        // 方案类型
        public SchemeType SchemeType { get; set; }
        // 方案来源
        public SchemeSource SchemeSource { get; set; }
        // 方案投注方案
        public SchemeBettingCategory SchemeBettingCategory { get; set; }
        // 方案总金额
        public decimal TotalMoney { get; set; }
        public bool StopAfterBonus { get; set; }
        /// <summary>
        /// 倍数
        /// </summary>
        public int Amount { get; set; }
        // 方案进度
        public ProgressStatus ProgressStatus { get; set; }
        // 方案出票状态
        public TicketStatus TicketStatus { get; set; }
        // 购买期号
        public string IssuseNumber { get; set; }
        // 中奖状态
        public BonusStatus BonusStatus { get; set; }
        // 税前奖金
        public decimal PreTaxBonusMoney { get; set; }
        // 税后奖金
        public decimal AfterTaxBonusMoney { get; set; }
        /// <summary>
        /// 投注时间
        /// </summary>
        public string BetTime { get; set; }
        /// <summary>
        /// 彩种玩法
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 加奖总金额
        /// </summary>
        public decimal AddMoney { get; set; }
        /// <summary>
        /// 红包加奖金额
        /// </summary>
        public decimal RedBagAwardsMoney { get; set; }
        /// <summary>
        /// 奖金加奖金额
        /// </summary>
        public decimal BonusAwardsMoney { get; set; }
    }
    [CommunicationObject]
    public class MyOrderListInfoCollection : List<MyOrderListInfo>
    {
    }

    #endregion
}
