using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using Common.Lottery;
using Common.XmlAnalyzer;
using Common;

namespace GameBiz.Core
{
    /// <summary>
    /// 普通投注对象
    /// </summary>
    [CommunicationObject]
    public class Sports_BetingInfo
    {
        public Sports_BetingInfo()
        {
            AnteCodeList = new Sports_AnteCodeInfoCollection();
        }
        public string SchemeId { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string PlayType { get; set; }
        public string IssuseNumber { get; set; }
        public SchemeSource SchemeSource { get; set; }
        public TogetherSchemeSecurity Security { get; set; }
        public SchemeBettingCategory BettingCategory { get; set; }
        public int Amount { get; set; }
        public int TotalMatchCount { get; set; }
        public decimal TotalMoney { get; set; }
        public TogetherSchemeProgress SchemeProgress { get; set; }
        public int SoldCount { get; set; }
        public Sports_AnteCodeInfoCollection AnteCodeList { get; set; }
        /// <summary>
        /// 活动选择
        /// </summary>
        public ActivityType ActivityType { get; set; }
        /// <summary>
        /// 附加数据
        /// </summary>
        public string Attach { get; set; }
        /// <summary>
        /// 记录当前投注时间，用于判断频繁投注
        /// </summary>
        public DateTime CurrentBetTime { get; set; }
        /// <summary>
        /// 是否重复投注
        /// </summary>
        public bool IsRepeat { get; set; }
        /// <summary>
        /// 是否正在执行
        /// </summary>
        public bool IsSubmit { get; set; }
        /// <summary>
        /// 宝单分享宣言
        /// </summary>
        public string SingleTreasureDeclaration { get; set; }
        /// <summary>
        /// 宝单分享提成
        /// </summary>
        public decimal BDFXCommission { get; set; }
        /// <summary>
        /// 分享订单号
        /// </summary>
        public string BDFXSchemeId { get; set; }
        public override bool Equals(object obj)
        {
            var currObj = obj as Sports_BetingInfo;
            if (currObj == null)
                return false;
            if (currObj.AnteCodeList.Count != AnteCodeList.Count)
                return false;
            foreach (var item in currObj.AnteCodeList)
            {
                var currAnteCode = AnteCodeList.FirstOrDefault(s => s.MatchId == item.MatchId && s.AnteCode == item.AnteCode && s.GameType == item.GameType && s.IsDan == item.IsDan && s.PlayType == item.PlayType);
                if (currAnteCode == null)
                    return false;
            }
            return true;
        }
    }

    /// <summary>
    /// 投注号码
    /// </summary>
    [CommunicationObject]
    public class Sports_AnteCodeInfo : ISportAnteCode
    {
        public string GameType { get; set; }
        public string PlayType { get; set; }
        public string AnteCode { get; set; }
        public bool IsDan { get; set; }
        public string Odds { get { return ""; } }
        public string MatchId { get; set; }
        public int Length { get { return AnteCode.Split(',', '|').Length; } }

        public string GetMatchResult(string gameCode, string gameType, string score)
        {
            return "";
        }
    }
    [CommunicationObject]
    public class Sports_AnteCodeInfoCollection : List<Sports_AnteCodeInfo>
    {
    }

    /// <summary>
    /// 追号投注期号
    /// </summary>
    [CommunicationObject]
    public class LotteryBettingIssuseInfo
    {
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 倍数
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 当期总金额
        /// </summary>
        public decimal IssuseTotalMoney { get; set; }
    }
    [CommunicationObject]
    public class LotteryBettingIssuseInfoCollection : List<LotteryBettingIssuseInfo>
    {
    }

    /// <summary>
    /// 投注号码
    /// </summary>
    [CommunicationObject]
    public class LotteryAnteCodeInfo
    {
        /// <summary>
        /// 玩法编码
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 号码
        /// </summary>
        public string AnteCode { get; set; }
        public bool IsDan { get; set; }
    }
    [CommunicationObject]
    public class LotteryAnteCodeInfoCollection : List<LotteryAnteCodeInfo>
    {
    }

    /// <summary>
    /// 彩票投注对象
    /// </summary>
    [CommunicationObject]
    public class LotteryBettingInfo
    {
        public LotteryBettingInfo()
        {
            AnteCodeList = new LotteryAnteCodeInfoCollection();
            IssuseNumberList = new LotteryBettingIssuseInfoCollection();
        }
        public string SchemeId { get; set; }
        public string UserId { get; set; }
        public string GameCode { get; set; }
        /// <summary>
        /// 方案来源
        /// </summary>
        public SchemeSource SchemeSource { get; set; }
        public TogetherSchemeSecurity Security { get; set; }
        public SchemeBettingCategory BettingCategory { get; set; }
        /// <summary>
        /// 投注总金额
        /// </summary>
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 投注号
        /// </summary>
        public LotteryAnteCodeInfoCollection AnteCodeList { get; set; }
        /// <summary>
        /// 投注期号
        /// </summary>
        public LotteryBettingIssuseInfoCollection IssuseNumberList { get; set; }
        /// <summary>
        /// 中奖后停止
        /// </summary>
        public bool StopAfterBonus { get; set; }
        public ActivityType ActivityType { get; set; }
        /// <summary>
        /// 是否追号投注
        /// </summary>
        public bool IsAppend { get; set; }
        /// <summary>
        /// 出票时间
        /// </summary>
        public DateTime? TicketTime { get; set; }
        /// <summary>
        /// 记录当前投注时间，用于判断频繁投注
        /// </summary>
        public DateTime CurrentBetTime { get; set; }
        /// <summary>
        /// 是否重复投注
        /// </summary>
        public bool IsRepeat { get; set; }
        /// <summary>
        ///是否正在执行 
        /// </summary>
        public bool IsSubmit { get; set; }
        /// <summary>
        /// 方案类别
        /// </summary>
        //public SchemeType SchemeType { get; set; }

        public override bool Equals(object obj)
        {
            var currObj = obj as LotteryBettingInfo;
            if (currObj == null)
                return false;
            if (currObj.AnteCodeList.Count != AnteCodeList.Count)
                return false;
            foreach (var item in currObj.AnteCodeList)
            {
                var currAnteCode = AnteCodeList.FirstOrDefault(s => s.AnteCode == item.AnteCode && s.GameType == item.GameType && s.IsDan == item.IsDan);
                if (currAnteCode == null)
                    return false;
            }
            return true;
        }
    }

    /// <summary>
    /// 合买基类
    /// </summary>
    [CommunicationObject]
    public class TogetherSchemeBase
    {
        #region 合买信息

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 参与密码
        /// </summary>
        public string JoinPwd { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 总份数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 每份单价
        /// </summary>
        public decimal Price { get; set; }
        private int _bonusDeduct;
        /// <summary>
        /// 中奖提成 0-10
        /// </summary>
        public int BonusDeduct {
            get { return _bonusDeduct; }
            set {
                if (value < 0 || value > 10)
                { throw new LogicException("提成无效"); }
                _bonusDeduct=value;
            }
        }
        /// <summary>
        /// 发起人认购份数
        /// </summary>
        public int Subscription { get; set; }
        /// <summary>
        /// 发起人保底份数
        /// </summary>
        public int Guarantees { get; set; }

        #endregion
    }

    /// <summary>
    /// 创建合买对象
    /// </summary>
    [CommunicationObject]
    public class Sports_TogetherSchemeInfo : TogetherSchemeBase
    {
        #region 投注信息

        public string SchemeId { get; set; }
        /// <summary>
        /// 彩种编号
        /// </summary>
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string PlayType { get; set; }
        public int Amount { get; set; }
        public int TotalMatchCount { get; set; }
        /// <summary>
        /// 方案保密性
        /// </summary>
        public TogetherSchemeSecurity Security { get; set; }
        public SchemeBettingCategory BettingCategory { get; set; }
        /// <summary>
        /// 方案来源
        /// </summary>
        public SchemeSource SchemeSource { get; set; }
        /// <summary>
        /// 投注号
        /// </summary>
        public Sports_AnteCodeInfoCollection AnteCodeList { get; set; }
        /// <summary>
        /// 投注期号
        /// </summary>
        public string IssuseNumber { get; set; }
        public ActivityType ActivityType { get; set; }
        public string Attach { get; set; }
        /// <summary>
        /// 是否追号
        /// </summary>
        public bool IsAppend { get; set; }
        /// <summary>
        /// 出票时间
        /// </summary>
        public DateTime? TicketTime { get; set; }
        /// <summary>
        /// 记录当前投注时间，用于判断频繁投注
        /// </summary>
        public DateTime CurrentBetTime { get; set; }
        /// <summary>
        /// 是否重复投注
        /// </summary>
        public bool IsRepeat { get; set; }
        /// <summary>
        /// 是否正在执行
        /// </summary>
        public bool IsSubmit { get; set; }

        public override bool Equals(object obj)
        {
            var currObj = obj as Sports_TogetherSchemeInfo;
            if (currObj == null)
                return false;
            if (currObj.AnteCodeList.Count != AnteCodeList.Count)
                return false;
            foreach (var item in currObj.AnteCodeList)
            {
                var currAnteCode = AnteCodeList.FirstOrDefault(s => s.MatchId == item.MatchId && s.AnteCode == item.AnteCode && s.GameType == item.GameType && s.IsDan == item.IsDan && s.PlayType == item.PlayType);
                if (currAnteCode == null)
                    return false;
            }
            return true;
        }

        #endregion
    }

    /// <summary>
    /// 足彩查询对象
    /// </summary>
    [CommunicationObject]
    [XmlMapping("info", 0)]
    public class Sports_SchemeQueryInfo : XmlMappingObject
    {
        [XmlMapping("UserId", 0)]
        public string UserId { get; set; }
        [XmlMapping("UserDisplayName", 1)]
        public string UserDisplayName { get; set; }
        [XmlMapping("SchemeId", 2)]
        public string SchemeId { get; set; }
        [XmlMapping("GameCode", 3)]
        public string GameCode { get; set; }
        [XmlMapping("GameDisplayName", 4)]
        public string GameDisplayName { get; set; }
        [XmlMapping("GameType", 5)]
        public string GameType { get; set; }
        [XmlMapping("GameTypeDisplayName", 6)]
        public string GameTypeDisplayName { get; set; }
        [XmlMapping("PlayType", 7)]
        public string PlayType { get; set; }
        [XmlMapping("SchemeType", 8)]
        public SchemeType SchemeType { get; set; }
        [XmlMapping("IssuseNumber", 9)]
        public string IssuseNumber { get; set; }
        [XmlMapping("Amount", 10)]
        public int Amount { get; set; }
        [XmlMapping("BetCount", 11)]
        public int BetCount { get; set; }
        [XmlMapping("TotalMatchCount", 12)]
        public int TotalMatchCount { get; set; }
        [XmlMapping("TotalMoney", 13)]
        public decimal TotalMoney { get; set; }
        [XmlMapping("TicketStatus", 14)]
        public TicketStatus TicketStatus { get; set; }
        [XmlMapping("TicketId", 15)]
        public string TicketId { get; set; }
        [XmlMapping("TicketLog", 16)]
        public string TicketLog { get; set; }
        [XmlMapping("ProgressStatus", 17)]
        public ProgressStatus ProgressStatus { get; set; }
        [XmlMapping("BonusStatus", 18)]
        public BonusStatus BonusStatus { get; set; }
        [XmlMapping("PreTaxBonusMoney", 19)]
        public decimal PreTaxBonusMoney { get; set; }
        [XmlMapping("AfterTaxBonusMoney", 19)]
        public decimal AfterTaxBonusMoney { get; set; }
        [XmlMapping("BonusCount", 20)]
        public int BonusCount { get; set; }
        [XmlMapping("WinNumber", 50)]
        public string WinNumber { get; set; }
        [XmlMapping("IsPrizeMoney", 21)]
        public bool IsPrizeMoney { get; set; }
        [XmlMapping("IsVirtualOrder", 22)]
        public bool IsVirtualOrder { get; set; }
        [XmlMapping("CreateTime", 23)]
        public DateTime CreateTime { get; set; }
        [XmlMapping("Security", 24)]
        public TogetherSchemeSecurity Security { get; set; }
        [XmlMapping("StopTime", 25)]
        public DateTime StopTime { get; set; }
        [XmlMapping("HitMatchCount", 26)]
        public int HitMatchCount { get; set; }
        [XmlMapping("AddMoney", 27)]
        public decimal AddMoney { get; set; }
        [XmlMapping("AddMoneyDescription", 28)]
        public string AddMoneyDescription { get; set; }
        [XmlMapping("SchemeBettingCategory", 29)]
        public SchemeBettingCategory SchemeBettingCategory { get; set; }
        [XmlMapping("SchemeBettingCategory", 30)]
        public int HideDisplayNameCount { get; set; }
        [XmlMapping("TicketProgress", 31)]
        public decimal TicketProgress { get; set; }
        [XmlMapping("DistributionWay", 32)]
        public AddMoneyDistributionWay DistributionWay { get; set; }
        [XmlMapping("Attach", 33)]
        public string Attach { get; set; }
        [XmlMapping("MinBonusMoney", 34)]
        public decimal MinBonusMoney { get; set; }
        [XmlMapping("MaxBonusMoney", 35)]
        public decimal MaxBonusMoney { get; set; }
        [XmlMapping("ExtensionOne", 36)]
        public string ExtensionOne { get; set; }
        [XmlMapping("IsAppend", 37)]
        public bool IsAppend { get; set; }
        [XmlMapping("ComplateDateTime", 38)]
        public DateTime ComplateDateTime { get; set; }
        [XmlMapping("BetTime", 39)]
        public DateTime BetTime { get; set; }
        [XmlMapping("SchemeSource", 40)]
        public SchemeSource SchemeSource { get; set; }
        [XmlMapping("TicketTime", 41)]
        public DateTime? TicketTime { get; set; }
        [XmlMapping("RedBagMoney", 42)]
        public decimal RedBagMoney { get; set; }
        [XmlMapping("RedBagAwardsMoney", 43)]
        public decimal RedBagAwardsMoney { get; set; }
        [XmlMapping("BonusAwardsMoney", 44)]
        public decimal BonusAwardsMoney { get; set; }
    }



    [CommunicationObject]
    public class Sports_ComplateInfo
    {
        public Sports_ComplateInfo()
        {
            SingleDetailList = new Sports_ComplateInfoCollection();
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { set; get; }
        /// <summary>
        /// 发起人佣金
        /// </summary>
        public decimal PromoterCommission { get; set; }
        /// <summary>
        /// 总分配金额
        /// </summary>
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 加奖金额
        /// </summary>
        public decimal AwardMoney { get; set; }
        /// <summary>
        /// 认购份数
        /// </summary>
        public int BuyCount { set; get; }
        /// <summary>
        /// 单份金额
        /// </summary>
        public decimal SingleMoney { get; set; }
        /// <summary>
        /// 分配方式
        /// </summary>
        public AddMoneyDistributionWay Allocation { get; set; }
        /// <summary>
        /// 参与用户详细
        /// </summary>
        public Sports_ComplateInfoCollection SingleDetailList { set; get; }

    }
    [CommunicationObject]
    public class Sports_ComplateInfoCollection : List<Sports_ComplateInfoList>
    {
    }

    [CommunicationObject]
    public class Sports_ComplateInfoList
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { set; get; }
        /// <summary>
        /// 购买金额
        /// </summary>
        public decimal BuyMoney { set; get; }
        /// <summary>
        /// 比例
        /// </summary>
        public string Proportion { set; get; }
        /// <summary>
        /// 中奖奖金
        /// </summary>
        public decimal WinMoney { set; get; }
        /// <summary>
        /// 加奖奖金
        /// </summary>
        public decimal AddMoney { set; get; }
    }


    [CommunicationObject]
    public class Sports_SchemeQueryInfoCollection
    {
        public Sports_SchemeQueryInfoCollection()
        {
            List = new List<Sports_SchemeQueryInfo>();
        }
        public List<Sports_SchemeQueryInfo> List { get; set; }
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// 足彩投注号码查询对象
    /// </summary>
    [CommunicationObject]
    [XmlMapping("antecode", 0)]
    public class Sports_AnteCodeQueryInfo : XmlMappingObject
    {
        [XmlMapping("LeagueId", 0, MappingType = MappingType.Attribute)]
        public string LeagueId { get; set; }
        [XmlMapping("LeagueName", 1, MappingType = MappingType.Attribute)]
        public string LeagueName { get; set; }
        [XmlMapping("LeagueColor", 2, MappingType = MappingType.Attribute)]
        public string LeagueColor { get; set; }
        [XmlMapping("StartTime", 3, MappingType = MappingType.Attribute)]
        public DateTime StartTime { get; set; }
        [XmlMapping("MatchId", 4, MappingType = MappingType.Attribute)]
        public string MatchId { get; set; }
        [XmlMapping("MatchIdName", 5, MappingType = MappingType.Attribute)]
        public string MatchIdName { get; set; }
        [XmlMapping("HomeTeamId", 6, MappingType = MappingType.Attribute)]
        public string HomeTeamId { get; set; }
        [XmlMapping("HomeTeamName", 7, MappingType = MappingType.Attribute)]
        public string HomeTeamName { get; set; }
        [XmlMapping("GuestTeamId", 8, MappingType = MappingType.Attribute)]
        public string GuestTeamId { get; set; }
        [XmlMapping("GuestTeamName", 9, MappingType = MappingType.Attribute)]
        public string GuestTeamName { get; set; }
        [XmlMapping("IssuseNumber", 10, MappingType = MappingType.Attribute)]
        public string IssuseNumber { get; set; }
        [XmlMapping("AnteCode", 11, MappingType = MappingType.Attribute)]
        public string AnteCode { get; set; }
        [XmlMapping("IsDan", 12, MappingType = MappingType.Attribute)]
        public bool IsDan { get; set; }
        [XmlMapping("LetBall", 13, MappingType = MappingType.Attribute)]
        public int LetBall { get; set; }
        [XmlMapping("CurrentSp", 14, MappingType = MappingType.Attribute)]
        public string CurrentSp { get; set; }
        [XmlMapping("HalfResult", 15, MappingType = MappingType.Attribute)]
        public string HalfResult { get; set; }
        [XmlMapping("FullResult", 16, MappingType = MappingType.Attribute)]
        public string FullResult { get; set; }
        [XmlMapping("MatchResult", 17, MappingType = MappingType.Attribute)]
        public string MatchResult { get; set; }
        [XmlMapping("MatchResultSp", 18, MappingType = MappingType.Attribute)]
        public decimal MatchResultSp { get; set; }
        [XmlMapping("BonusStatus", 19, MappingType = MappingType.Attribute)]
        public BonusStatus BonusStatus { get; set; }
        [XmlMapping("GameType", 20, MappingType = MappingType.Attribute)]
        public string GameType { get; set; }
        [XmlMapping("MatchState", 21, MappingType = MappingType.Attribute)]
        public string MatchState { get; set; }
        [XmlMapping("WinNumber", 22, MappingType = MappingType.Attribute)]
        public string WinNumber { get; set; }
    }
    [CommunicationObject]
    public class Sports_AnteCodeQueryInfoCollection : XmlMappingList<Sports_AnteCodeQueryInfo>
    {
    }

    /// <summary>
    /// 合买 订制跟单 规则
    /// </summary>
    [CommunicationObject]
    public class TogetherFollowerRuleInfo
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 合买发起人用户编号
        /// </summary>
        public string CreaterUserId { get; set; }
        /// <summary>
        /// 跟单人用户编号
        /// </summary>
        public string FollowerUserId { get; set; }
        /// <summary>
        /// 彩种编码
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 玩法编码
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 跟单方案个数
        /// </summary>
        public int SchemeCount { get; set; }
        /// <summary>
        /// 方案最小金额 
        /// </summary>
        public decimal MinSchemeMoney { get; set; }
        /// <summary>
        /// 方案最大金额
        /// </summary>
        public decimal MaxSchemeMoney { get; set; }
        /// <summary>
        /// 跟单份数
        /// </summary>
        public int FollowerCount { get; set; }
        /// <summary>
        /// 跟单百分比
        /// </summary>
        public decimal FollowerPercent { get; set; }
        /// <summary>
        /// 当方案剩余份数/百分比不足时 是否跟单
        /// </summary>
        public bool CancelWhenSurplusNotMatch { get; set; }
        /// <summary>
        /// 连续X个方案未中奖则停止跟单
        /// </summary>
        public int CancelNoBonusSchemeCount { get; set; }
        /// <summary>
        /// 当用户金额小于X时停止跟单
        /// </summary>
        public decimal StopFollowerMinBalance { get; set; }
    }

    /// <summary>
    /// 合买 订制跟单规则查询对象
    /// </summary>
    [CommunicationObject]
    public class TogetherFollowerRuleQueryInfo
    {
        public long RuleId { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public int HideDisplayNameCount { get; set; }
        /// <summary>
        /// 跟单投注金额
        /// </summary>
        public decimal BuyMoney { get; set; }
        /// <summary>
        /// 中奖金额
        /// </summary>
        public decimal BonusMoney { get; set; }
        public int FollowerIndex { get; set; }
        public DateTime CreateTime { get; set; }
        public bool IsEnable { get; set; }
        public string CreaterUserId { get; set; }
        public string FollowerUserId { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public int SchemeCount { get; set; }
        public decimal MinSchemeMoney { get; set; }
        public decimal MaxSchemeMoney { get; set; }
        public int FollowerCount { get; set; }
        public decimal FollowerPercent { get; set; }
        public bool CancelWhenSurplusNotMatch { get; set; }
        public int CancelNoBonusSchemeCount { get; set; }
        public decimal StopFollowerMinBalance { get; set; }
    }

    [CommunicationObject]
    public class TogetherFollowerRuleQueryInfoCollection
    {
        public TogetherFollowerRuleQueryInfoCollection()
        {
            List = new List<TogetherFollowerRuleQueryInfo>();
        }

        public List<TogetherFollowerRuleQueryInfo> List { get; set; }
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// 定制跟单 跟我的对象
    /// </summary>
    [CommunicationObject]
    public class TogetherFollowMeInfo
    {
        public string CreaterUserId { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        /// <summary>
        /// 已跟单订单数
        /// </summary>
        public int TotalOrderCount { get; set; }
        /// <summary>
        /// 已跟单且中奖订单数
        /// </summary>
        public int TotalOrderBonusCount { get; set; }
        /// <summary>
        /// 已跟单投注金额
        /// </summary>
        public decimal TotalBetMoney { get; set; }
        /// <summary>
        /// 已跟单且中奖金额
        /// </summary>
        public decimal TotalBonusMoney { get; set; }
    }
    [CommunicationObject]
    public class TogetherFollowMeInfoCollection
    {
        public TogetherFollowMeInfoCollection()
        {
            List = new List<TogetherFollowMeInfo>();
        }

        public List<TogetherFollowMeInfo> List { get; set; }
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// 订制跟单记录
    /// </summary>
    [CommunicationObject]
    public class TogetherFollowRecordInfo
    {
        public string SchemeId { get; set; }
        public string CreaterUserId { get; set; }
        public string CreaterDisplayName { get; set; }
        public int CreaterHideDisplayNameCount { get; set; }
        public string IssuseNumber { get; set; }
        public string GameCode { get; set; }
        public string GameCodeDisplayName { get; set; }
        public string GameType { get; set; }
        public string GameTypeDisplayName { get; set; }
        public decimal SchemeMoney { get; set; }
        public decimal SchemeBonusMoney { get; set; }
        public decimal FollowMoney { get; set; }
        public decimal FollowBonusMoney { get; set; }
        public DateTime CreateTime { get; set; }
        public ProgressStatus ProgressStatus { get; set; }
    }

    [CommunicationObject]
    public class TogetherFollowRecordInfoCollection
    {
        public TogetherFollowRecordInfoCollection()
        {
            List = new List<TogetherFollowRecordInfo>();
        }

        public List<TogetherFollowRecordInfo> List { get; set; }
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// 合买查询对象
    /// </summary>
    [CommunicationObject]
    public class Sports_TogetherSchemeQueryInfo
    {
        public string SchemeId { get; set; }
        /// <summary>
        /// 方案来源
        /// </summary>
        public SchemeSource SchemeSource { get; set; }
        public bool IsTop { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 发起人编号
        /// </summary>
        public string CreateUserId { get; set; }
        /// <summary>
        /// 发起人名称
        /// </summary>
        public string CreaterDisplayName { get; set; }
        public int CreaterHideDisplayNameCount { get; set; }
        /// <summary>
        /// 参与密码
        /// </summary>
        public string JoinPwd { get; set; }

        /// <summary>
        /// 方案保密性
        /// </summary>
        public TogetherSchemeSecurity Security { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 总份数
        /// </summary>
        public int TotalCount { get; set; }
        public int SoldCount { get; set; }
        public int SurplusCount { get; set; }
        public int JoinUserCount { get; set; }
        /// <summary>
        /// 每份单价
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 中奖提成 0-10
        /// </summary>
        public int BonusDeduct { get; set; }
        /// <summary>
        /// 方案提成
        /// </summary>
        public decimal SchemeDeduct { get; set; }
        /// <summary>
        /// 发起人认购份数
        /// </summary>
        public int Subscription { get; set; }
        /// <summary>
        /// 发起人保底份数
        /// </summary>
        public int Guarantees { get; set; }
        /// <summary>
        /// 系统保底份数
        /// </summary>
        public int SystemGuarantees { get; set; }
        /// <summary>
        /// 进度面分比
        /// </summary>
        public decimal Progress { get; set; }
        /// <summary>
        /// 进度状态
        /// </summary>
        public TogetherSchemeProgress ProgressStatus { get; set; }

        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string GameDisplayName { get; set; }
        public string GameTypeDisplayName { get; set; }
        public string PlayType { get; set; }
        public string IssuseNumber { get; set; }
        public int TotalMatchCount { get; set; }
        public int Amount { get; set; }
        public int BetCount { get; set; }
        public DateTime StopTime { get; set; }

        public decimal PreTaxBonusMoney { get; set; }
        public decimal AfterTaxBonusMoney { get; set; }
        public string WinNumber { get; set; }
        public int BonusCount { get; set; }
        public int HitMatchCount { get; set; }

        public TicketStatus TicketStatus { get; set; }
        public SchemeBettingCategory SchemeBettingCategory { get; set; }
        public DateTime CreateTime { get; set; }
        public BonusStatus BonusStatus { get; set; }
        public bool IsPrizeMoney { get; set; }
        public decimal AddMoney { get; set; }
        public string AddMoneyDescription { get; set; }
        public bool IsVirtualOrder { get; set; }
        public string Attach { get; set; }
        public decimal MinBonusMoney { get; set; }
        public decimal MaxBonusMoney { get; set; }
        public string ExtensionOne { get; set; }

        public int GoldCrownCount { get; set; }
        public int GoldCupCount { get; set; }
        public int GoldDiamondsCount { get; set; }
        public int GoldStarCount { get; set; }
        public int SilverCrownCount { get; set; }
        public int SilverCupCount { get; set; }
        public int SilverDiamondsCount { get; set; }
        public int SilverStarCount { get; set; }
        public bool IsAppend { get; set; }
        public DateTime? TicketTime { get; set; }

    }

    [CommunicationObject]
    public class Sports_TogetherSchemeQueryInfoCollection
    {
        public Sports_TogetherSchemeQueryInfoCollection()
        {
            List = new List<Sports_TogetherSchemeQueryInfo>();
        }
        public List<Sports_TogetherSchemeQueryInfo> List { get; set; }
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// 合买参与用户信息
    /// </summary>
    [CommunicationObject]
    public class Sports_TogetherJoinInfo
    {
        public long JoinId { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public int HideDisplayNameCount { get; set; }
        public string SchemeId { get; set; }
        public int BuyCount { get; set; }
        public int RealBuyCount { get; set; }
        public bool IsSucess { get; set; }
        public decimal Price { get; set; }
        public decimal BonusMoney { get; set; }
        public TogetherJoinType JoinType { get; set; }
        public DateTime JoinDateTime { get; set; }
    }

    [CommunicationObject]
    public class Sports_TogetherJoinInfoCollection
    {
        public Sports_TogetherJoinInfoCollection()
        {
            List = new List<Sports_TogetherJoinInfo>();
        }

        public List<Sports_TogetherJoinInfo> List { get; set; }
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// 单式上传投注
    /// </summary>
    [CommunicationObject]
    public class SingleSchemeInfo
    {
        public string SchemeId { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string PlayType { get; set; }
        public string IssuseNumber { get; set; }
        public string SelectMatchId { get; set; }
        /// <summary>
        /// 允许投注的号，如BJDC 胜平负 只能投 3 1 0
        /// </summary>
        public string AllowCodes { get; set; }
        /// <summary>
        /// 是否包括场次编号
        /// </summary>
        public bool ContainsMatchId { get; set; }
        public SchemeSource SchemeSource { get; set; }
        public TogetherSchemeSecurity Security { get; set; }
        public SchemeBettingCategory BettingCategory { get; set; }
        public Sports_AnteCodeInfoCollection AnteCodeList { get; set; }
        public int Amount { get; set; }
        public decimal TotalMoney { get; set; }
        public byte[] FileBuffer { get; set; }
        //public string AnteCodeFullFileName { get; set; }
        public ActivityType ActivityType { get; set; }
        /// <summary>
        /// 记录当前投注时间，用于判断频繁投注
        /// </summary>
        public DateTime CurrentBetTime { get; set; }
        /// <summary>
        /// 是否重复投注
        /// </summary>
        public bool IsRepeat { get; set; }
        /// <summary>
        /// 是否正在执行
        /// </summary>
        public bool IsSubmit { get; set; }
        public override bool Equals(object obj)
        {
            var currObj = obj as SingleSchemeInfo;
            if (currObj.AnteCodeList == null || AnteCodeList == null)//判断先发起后上传
                return false;
            if (currObj == null)
                return false;
            if (AnteCodeList != null && currObj.AnteCodeList.Count != AnteCodeList.Count)
                return false;
            foreach (var item in currObj.AnteCodeList)
            {
                var currAnteCode = AnteCodeList.FirstOrDefault(s => s.MatchId == item.MatchId && s.AnteCode == item.AnteCode && s.GameType == item.GameType && s.IsDan == item.IsDan && s.PlayType == item.PlayType);
                if (currAnteCode == null)
                    return false;
            }
            return true;
        }
    }
    /// <summary>
    /// 单式上传号码查询对象
    /// </summary>
    [CommunicationObject]
    public class SingleScheme_AnteCodeQueryInfo
    {
        public string SchemeId { get; set; }
        //public string AnteCodeFullFileName { get; set; }
        public string PlayType { get; set; }
        public string AllowCodes { get; set; }
        public string SelectMatchId { get; set; }
        public bool ContainsMatchId { get; set; }
        public byte[] FileBuffer { get; set; }
        public DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 单式上传方案创建合买对象
    /// </summary>
    [CommunicationObject]
    public class SingleScheme_TogetherSchemeInfo : TogetherSchemeBase
    {
        public SingleScheme_TogetherSchemeInfo()
        {
            BettingInfo = new SingleSchemeInfo();
        }

        public SingleSchemeInfo BettingInfo { get; set; }
    }

    [CommunicationObject]
    public class UserCurrentOrderInfo
    {
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public int HideDisplayNameCount { get; set; }
        public string SchemeId { get; set; }
        public string IssuseNumber { get; set; }
        public string GameCode { get; set; }
        public string GameCodeName { get; set; }
        public string GameTypeName { get; set; }
        public SchemeType SchemeType { get; set; }
        public TogetherJoinType JoinType { get; set; }
        public decimal TotalMoney { get; set; }
        public decimal Progress { get; set; }
    }

    [CommunicationObject]
    public class UserCurrentOrderInfoCollection
    {
        public UserCurrentOrderInfoCollection()
        {
            List = new List<UserCurrentOrderInfo>();
        }
        public List<UserCurrentOrderInfo> List { get; set; }
        public int TotalCount { get; set; }
    }

    [CommunicationObject]
    public class SaveOrder_LotteryBettingInfo
    {
        public string SchemeId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string DisplayName { get; set; }
        public string UserId { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string StrStopTime { get; set; }

        public string PlayType { get; set; }
        public SchemeType SchemeType { get; set; }
        public SchemeSource SchemeSource { get; set; }
        public SchemeBettingCategory SchemeBettingCategory { get; set; }
        public ProgressStatus ProgressStatus { get; set; }
        public string IssuseNumber { get; set; }
        public int Amount { get; set; }
        public int BetCount { get; set; }
        public decimal TotalMoney { get; set; }
        public DateTime StopTime { get; set; }
        public DateTime CreateTime { get; set; }
    }

    [CommunicationObject]
    public class SaveOrder_LotteryBettingInfoCollection : List<SaveOrder_LotteryBettingInfo>
    {
    }

    [CommunicationObject]
    public class TestInfo
    {
        public string OrderId { get; set; }
        public DateTime CreateTime { get; set; }
        public int Age { get; set; }
        public decimal Money { get; set; }
        public byte[] Buffer { get; set; }
    }

    /// <summary>
    /// 优化投注比赛
    /// </summary>
    [CommunicationObject]
    public class YouHuaMatchInfo
    {
        public string Sign { get; set; }
        public string MatchId { get; set; }
        public string GameType { get; set; }
        public string BetContent { get; set; }
    }
    [CommunicationObject]
    public class YouHuaMatchInfoCollection : List<YouHuaMatchInfo>
    {
    }

    /// <summary>
    /// 优化投注号码
    /// </summary>
    [CommunicationObject]
    public class YouHuaBetCodeInfo
    {
        public YouHuaBetCodeInfo()
        {
            MatchList = new YouHuaMatchInfoCollection();
        }
        public YouHuaMatchInfoCollection MatchList { get; set; }
        public string PlayType { get; set; }
        public int Amount { get; set; }
    }
    [CommunicationObject]
    public class YouHuaBetCodeInfoCollection : List<YouHuaBetCodeInfo>
    {
    }

    /// <summary>
    /// 奖金优化投注对象
    /// </summary>
    [CommunicationObject]
    public class YouHuaBetInfo
    {
        public YouHuaBetInfo()
        {
            AnteCodeList = new YouHuaBetCodeInfoCollection();
        }
        public string GameCode { get; set; }
        public string PlayType { get; set; }
        public string IssuseNumber { get; set; }
        public SchemeSource SchemeSource { get; set; }
        public YouHuaCategory YouHuaCategory { get; set; }
        public TogetherSchemeSecurity Security { get; set; }
        public int TotalMatchCount { get; set; }
        public decimal TotalMoney { get; set; }
        public int Amount { get; set; }
        public YouHuaBetCodeInfoCollection AnteCodeList { get; set; }
        public ActivityType ActivityType { get; set; }
    }

    /// <summary>
    /// 奖金优化合买
    /// </summary>
    [CommunicationObject]
    public class YouHua_TogetherSchemeInfo : TogetherSchemeBase
    {
        public YouHua_TogetherSchemeInfo()
        {
            BettingInfo = new YouHuaBetInfo();
        }

        public YouHuaBetInfo BettingInfo { get; set; }
    }
    [CommunicationObject]
    public class OrderSingleScheme
    {
        public string MatchId { get; set; }
        public string SchemeId { get; set; }
        public string IssuseNumber { get; set; }
        public string LeagueId { get; set; }
        public string LeagueName { get; set; }
        public string LeagueColor { get; set; }
        public string MatchIdName { get; set; }
        public string HomeTeamId { get; set; }
        public string HomeTeamName { get; set; }
        public string GuestTeamId { get; set; }
        public string GuestTeamName { get; set; }
        public bool IsDan { get; set; }
        public DateTime StartTime { get; set; }
        public string HalfResult { get; set; }
        public string FullResult { get; set; }
        public string MatchResult { get; set; }
        public decimal MatchResultSp { get; set; }
        public string CurrentSp { get; set; }
        public int LetBall { get; set; }
        public BonusStatus BonusStatus { get; set; }
        public string GameType { get; set; }
        public string MatchState { get; set; }
        public string PlayType { get; set; }
        public byte[] FileBuffer { get; set; }
        public bool ContainsMatchId { get; set; }
    }
    [CommunicationObject]
    public class OrderSingleSchemeCollection
    {
        public OrderSingleSchemeCollection()
        {
            AnteCodeList = new List<OrderSingleScheme>();
        }
        public int TotalCount { get; set; }
        public List<OrderSingleScheme> AnteCodeList { get; set; }
    }

    /// <summary>
    /// 票派奖对象，新流程
    /// </summary>
    [CommunicationObject]
    public class TicketPrizeInfo
    {
        public long Id { get; set; }
        public string TicketId { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string BetContent { get; set; }
        public int Amount { get; set; }
        public bool IsAppend { get; set; }
        public string IssuseNumber { get; set; }
        public string WinNumber { get; set; }
    }


    /// <summary>
    /// Redis投注队伍对象
    /// </summary>
    public class RedisBet_LotteryBetting
    {
        public LotteryBettingInfo BetInfo { get; set; }
        public string BalancePassword { get; set; }
        public decimal RedBagMoney { get; set; }
        public string UserToken { get; set; }
        public string BetDataTime { get; set; }
    }

    public class RedisBet_SportsBetting
    {
        public Sports_BetingInfo BetInfo { get; set; } 
        public string BalancePassword { get; set; }
        public decimal RedBagMoney { get; set; }
        public string UserToken { get; set; }
        public string BetDataTime { get; set; }
    }

    public class RedisBet_SingleScheme
    {
        public SingleSchemeInfo BetInfo { get; set; }  
        public string BalancePassword { get; set; }
        public decimal RedBagMoney { get; set; }
        public string UserToken { get; set; }
        public string BetDataTime { get; set; }
    }

    public class RedisBet_YouHuaBet
    {
        public Sports_BetingInfo BetInfo { get; set; }
        public string BalancePassword { get; set; }
        public decimal RealTotalMoney { get; set; }
        public decimal RedBagMoney { get; set; }
        public string UserToken { get; set; }
        public string BetDataTime { get; set; }
    }

    public class RedisBet_CreateSingleSchemeTogether
    {
        public SingleScheme_TogetherSchemeInfo BetInfo { get; set; }
        public string BalancePassword { get; set; }
        public string UserToken { get; set; }
        public string BetDataTime { get; set; }
    }

    public class RedisBet_CreateYouHuaSchemeTogether
    {
        public Sports_TogetherSchemeInfo BetInfo { get; set; }
        public string BalancePassword { get; set; }
        public decimal RealTotalMoney { get; set; }
        public string UserToken { get; set; }
        public string BetDataTime { get; set; }
    }

    public class RedisBet_CreateSportsTogether
    {
        public Sports_TogetherSchemeInfo BetInfo { get; set; }
        public string BalancePassword { get; set; }
        public string UserToken { get; set; }
        public string BetDataTime { get; set; }
    }

}
