﻿using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using EntityModel.ExceptionExtend;
using EntityModel.Xml;
using ProtoBuf;

namespace EntityModel.CoreModel
{
    [Serializable]
    public class HK6Sports_OrderInfo
    {
        public string content { get; set; }

        /// <summary>
        /// 玩法标签
        /// </summary>
        public string playedTag { get; set; }

        /// <summary>
        /// 玩法编号
        /// </summary>
        public int playedId { get; set; }

        /// <summary>
        /// 投注号码
        /// </summary>
        public string betingCode { get; set; }

        /// <summary>
        /// 每一注总价
        /// </summary>
        public decimal totalPrice { get; set; }

        public decimal unitPrice { get; set; }

    }
    
    /// <summary>
    ///  追号和加倍实体
    /// </summary>
    [Serializable]
    public class HK6Sports_PlanInfo
    {
        /// <summary>
        /// 倍数
        /// </summary>
        public int multiple { get; set; }

        /// <summary>
        /// 期号
        /// </summary>
        public int issueNo { get; set; }

       

    }

    public class HK6Sports_BetingInfo
    {
        /// <summary>
        /// 期号
        /// </summary>
        public int issueNo { get; set; }

        /// <summary>
        /// 彩种
        /// </summary>
        public string gameCode { get; set; } = "HK6";

        public IList<HK6Sports_OrderInfo> orderList { get; set; }
        public IList<HK6Sports_PlanInfo> planList { get; set; }

        /// <summary>
        /// 追期赢停止追期
        /// </summary>
        public int winStop { get; set; } = 0;

        public DateTime CurrentBetTime { get; set; }
    }

        /// <summary>
        /// 普通投注对象
        /// </summary>
        [Serializable]
    [ProtoContract]
    public class Sports_BetingInfo
    {
        public Sports_BetingInfo()
        {
            AnteCodeList = new Sports_AnteCodeInfoCollection();
        }
        [ProtoMember(1)]
        public string SchemeId { get; set; }
        [ProtoMember(2)]
        public string GameCode { get; set; }
        [ProtoMember(3)]
        public string GameType { get; set; }
        [ProtoMember(4)]
        public string PlayType { get; set; }
        [ProtoMember(5)]
        public string IssuseNumber { get; set; }
        [ProtoMember(6)]
        public SchemeSource SchemeSource { get; set; }
        [ProtoMember(7)]
        public TogetherSchemeSecurity Security { get; set; }
        [ProtoMember(8)]
        public SchemeBettingCategory BettingCategory { get; set; }
        [ProtoMember(9)]
        public int Amount { get; set; }
        [ProtoMember(10)]
        public int TotalMatchCount { get; set; }
        [ProtoMember(11)]
        public decimal TotalMoney { get; set; }
        [ProtoMember(12)]
        public TogetherSchemeProgress SchemeProgress { get; set; }
        [ProtoMember(13)]
        public int SoldCount { get; set; }
        [ProtoMember(14)]
        public List<Sports_AnteCodeInfo> AnteCodeList { get; set; }
        /// <summary>
        /// 活动选择
        /// </summary>
        [ProtoMember(15)]
        public ActivityType ActivityType { get; set; }
        /// <summary>
        /// 附加数据
        /// </summary>
        [ProtoMember(16)]
        public string Attach { get; set; }
        /// <summary>
        /// 记录当前投注时间，用于判断频繁投注
        /// </summary>
        [ProtoMember(17)]
        public DateTime CurrentBetTime { get; set; }
        /// <summary>
        /// 是否重复投注
        /// </summary>
        [ProtoMember(18)]
        public bool IsRepeat { get; set; }
        /// <summary>
        /// 是否正在执行
        /// </summary>
        [ProtoMember(19)]
        public bool IsSubmit { get; set; }
        /// <summary>
        /// 宝单分享宣言
        /// </summary>
        [ProtoMember(20)]
        public string SingleTreasureDeclaration { get; set; }
        /// <summary>
        /// 宝单分享提成
        /// </summary>
        [ProtoMember(21)]
        public decimal BDFXCommission { get; set; }
        /// <summary>
        /// 分享订单号
        /// </summary>
        [ProtoMember(22)]
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
    [Serializable]
    [ProtoContract]
    public class Sports_AnteCodeInfo : Interface.ISportAnteCode
    {
        [ProtoMember(1)]
        public string GameType { get; set; }
        [ProtoMember(2)]
        public string PlayType { get; set; }
        [ProtoMember(3)]
        public string AnteCode { get; set; }
        [ProtoMember(4)]
        public bool IsDan { get; set; }
        [ProtoMember(5)]
        public string Odds { get { return ""; } }
        [ProtoMember(6)]
        public string MatchId { get; set; }
        [ProtoMember(7)]
        public int Length { get { return AnteCode.Split(',', '|').Length; } }
        public string GetMatchResult(string gameCode, string gameType, string score)
        {
            return "";
        }
    }
    [Serializable]
    [ProtoContract]
    public class Sports_AnteCodeInfoCollection : List<Sports_AnteCodeInfo>
    {
    }

    /// <summary>
    /// 追号投注期号
    /// </summary>
    [Serializable]
    [ProtoContract]
    public class LotteryBettingIssuseInfo
    {
        /// <summary>
        /// 期号
        /// </summary>
        [ProtoMember(1)]
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 倍数
        /// </summary>
        [ProtoMember(2)]
        public int Amount { get; set; }
        /// <summary>
        /// 当期总金额
        /// </summary>
        [ProtoMember(3)]
        public decimal IssuseTotalMoney { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public class LotteryBettingIssuseInfoCollection : List<LotteryBettingIssuseInfo>
    {
    }

    /// <summary>
    /// 投注号码
    /// </summary>
    [Serializable]
    [ProtoContract]
    public class LotteryAnteCodeInfo
    {
        /// <summary>
        /// 玩法编码
        /// </summary>
        [ProtoMember(1)]
        public string GameType { get; set; }
        /// <summary>
        /// 号码
        /// </summary>
        [ProtoMember(2)]
        public string AnteCode { get; set; }
        [ProtoMember(3)]
        public bool IsDan { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public class LotteryAnteCodeInfoCollection : List<LotteryAnteCodeInfo>
    {
    }

    /// <summary>
    /// 彩票投注对象
    /// </summary>
    [Serializable]
    [ProtoContract]
    public class LotteryBettingInfo
    {
        public LotteryBettingInfo()
        {
            AnteCodeList = new LotteryAnteCodeInfoCollection();
            IssuseNumberList = new LotteryBettingIssuseInfoCollection();
        }
        [ProtoMember(1)]
        public string SchemeId { get; set; }
        [ProtoMember(2)]
        public string UserId { get; set; }
        [ProtoMember(3)]
        public string GameCode { get; set; }
        /// <summary>
        /// 方案来源
        /// </summary>
        [ProtoMember(4)]
        public SchemeSource SchemeSource { get; set; }
        [ProtoMember(5)]
        public TogetherSchemeSecurity Security { get; set; }
        [ProtoMember(6)]
        public SchemeBettingCategory BettingCategory { get; set; }
        /// <summary>
        /// 投注总金额
        /// </summary>
        [ProtoMember(7)]
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 投注号
        /// </summary>
        [ProtoMember(8)]
        public List<LotteryAnteCodeInfo> AnteCodeList { get; set; }
        /// <summary>
        /// 投注期号
        /// </summary>
        [ProtoMember(9)]
        public List<LotteryBettingIssuseInfo> IssuseNumberList { get; set; }
        /// <summary>
        /// 中奖后停止
        /// </summary>
        [ProtoMember(10)]
        public bool StopAfterBonus { get; set; }
        [ProtoMember(11)]
        public ActivityType ActivityType { get; set; }
        /// <summary>
        /// 是否追号投注
        /// </summary>
        [ProtoMember(12)]
        public bool IsAppend { get; set; }
        /// <summary>
        /// 出票时间
        /// </summary>
        [ProtoMember(13)]
        public DateTime? TicketTime { get; set; }
        /// <summary>
        /// 记录当前投注时间，用于判断频繁投注
        /// </summary>
        [ProtoMember(14)]
        public DateTime CurrentBetTime { get; set; }
        /// <summary>
        /// 是否重复投注
        /// </summary>
        [ProtoMember(15)]
        public bool IsRepeat { get; set; }
        /// <summary>
        ///是否正在执行 
        /// </summary>
        [ProtoMember(16)]
        public bool IsSubmit { get; set; }
        /// <summary>
        /// 方案类别
        /// </summary>
        //public SchemeType SchemeType { get; set; }

        public bool Equals(object obj)
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
    [Serializable]
    [ProtoContract]
    public class TogetherSchemeBase
    {
        #region 合买信息
        [ProtoMember(1)]
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        [ProtoMember(2)]
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        [ProtoMember(3)]
        /// <summary>
        /// 参与密码
        /// </summary>
        public string JoinPwd { get; set; }
        [ProtoMember(4)]
        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalMoney { get; set; }
        [ProtoMember(5)]
        /// <summary>
        /// 总份数
        /// </summary>
        public int TotalCount { get; set; }
        [ProtoMember(6)]
        /// <summary>
        /// 每份单价
        /// </summary>
        public decimal Price { get; set; }
        [ProtoMember(7)]
        private int _bonusDeduct;
        /// <summary>
        /// 中奖提成 0-10
        /// </summary>
        public int BonusDeduct
        {
            get { return _bonusDeduct; }
            set
            {
                if (value < 0 || value > 10)
                { throw new LogicException("提成无效"); }
                _bonusDeduct = value;
            }
        }
        [ProtoMember(8)]
        /// <summary>
        /// 发起人认购份数
        /// </summary>
        public int Subscription { get; set; }
        [ProtoMember(9)]
        /// <summary>
        /// 发起人保底份数
        /// </summary>
        public int Guarantees { get; set; }

        #endregion
    }

    /// <summary>
    /// 创建合买对象
    /// </summary>
    [Serializable]
    [ProtoContract]
    public class Sports_TogetherSchemeInfo : TogetherSchemeBase
    {
        #region 投注信息
        [ProtoMember(1)]
        public string SchemeId { get; set; }
        [ProtoMember(2)]
        /// <summary>
        /// 彩种编号
        /// </summary>
        public string GameCode { get; set; }
        [ProtoMember(3)]
        public string GameType { get; set; }
        [ProtoMember(4)]
        public string PlayType { get; set; }
        [ProtoMember(5)]
        public int Amount { get; set; }
        [ProtoMember(6)]
        public int TotalMatchCount { get; set; }
        [ProtoMember(7)]
        /// <summary>
        /// 方案保密性
        /// </summary>
        public TogetherSchemeSecurity Security { get; set; }
        [ProtoMember(8)]
        public SchemeBettingCategory BettingCategory { get; set; }
        [ProtoMember(9)]
        /// <summary>
        /// 方案来源
        /// </summary>
        public SchemeSource SchemeSource { get; set; }
        [ProtoMember(10)]
        /// <summary>
        /// 投注号
        /// </summary>
        public List<Sports_AnteCodeInfo> AnteCodeList { get; set; }
        [ProtoMember(11)]
        /// <summary>
        /// 投注期号
        /// </summary>
        public string IssuseNumber { get; set; }
        [ProtoMember(12)]
        public ActivityType ActivityType { get; set; }
        [ProtoMember(13)]
        public string Attach { get; set; }
        [ProtoMember(14)]
        /// <summary>
        /// 是否追号
        /// </summary>
        public bool IsAppend { get; set; }
        [ProtoMember(15)]
        /// <summary>
        /// 出票时间
        /// </summary>
        public DateTime? TicketTime { get; set; }
        [ProtoMember(16)]
        /// <summary>
        /// 记录当前投注时间，用于判断频繁投注
        /// </summary>
        public DateTime CurrentBetTime { get; set; }
        [ProtoMember(17)]
        /// <summary>
        /// 是否重复投注
        /// </summary>
        public bool IsRepeat { get; set; }
        [ProtoMember(18)]
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

    [Serializable]
    [ProtoContract]
    public class Sports_SchemeQueryInfo : XmlMappingObject
    {
        [ProtoMember(1)]
        public string UserId { get; set; }
        [ProtoMember(2)]
        public string UserDisplayName { get; set; }
        [ProtoMember(3)]
        public string SchemeId { get; set; }
        [ProtoMember(4)]
        public string GameCode { get; set; }
        [ProtoMember(5)]
        public string GameDisplayName { get; set; }
        [ProtoMember(6)]
        public string GameType { get; set; }
        [ProtoMember(7)]
        public string GameTypeDisplayName { get; set; }
        [ProtoMember(8)]
        public string PlayType { get; set; }
        [ProtoMember(9)]
        public SchemeType SchemeType { get; set; }
        [ProtoMember(10)]
        public string IssuseNumber { get; set; }
        [ProtoMember(11)]
        public int Amount { get; set; }
        [ProtoMember(12)]
        public int BetCount { get; set; }
        [ProtoMember(13)]
        public int TotalMatchCount { get; set; }
        [ProtoMember(14)]
        public decimal TotalMoney { get; set; }
        [ProtoMember(15)]
        public TicketStatus TicketStatus { get; set; }
        [ProtoMember(16)]
        public string TicketId { get; set; }
        [ProtoMember(17)]
        public string TicketLog { get; set; }
        [ProtoMember(18)]
        public ProgressStatus ProgressStatus { get; set; }
        [ProtoMember(19)]
        public BonusStatus BonusStatus { get; set; }
        [ProtoMember(20)]
        public decimal PreTaxBonusMoney { get; set; }
        [ProtoMember(21)]
        public decimal AfterTaxBonusMoney { get; set; }
        [ProtoMember(22)]
        public int BonusCount { get; set; }
        [ProtoMember(23)]
        public string WinNumber { get; set; }
        [ProtoMember(24)]
        public bool IsPrizeMoney { get; set; }
        [ProtoMember(25)]
        public bool IsVirtualOrder { get; set; }
        [ProtoMember(26)]
        public DateTime CreateTime { get; set; }
        [ProtoMember(27)]
        public TogetherSchemeSecurity Security { get; set; }
        [ProtoMember(28)]
        public DateTime StopTime { get; set; }
        [ProtoMember(29)]
        public int HitMatchCount { get; set; }
        [ProtoMember(30)]
        public decimal AddMoney { get; set; }
        [ProtoMember(31)]
        public string AddMoneyDescription { get; set; }
        [ProtoMember(32)]
        public SchemeBettingCategory SchemeBettingCategory { get; set; }
        [ProtoMember(33)]
        public int HideDisplayNameCount { get; set; }
        [ProtoMember(34)]
        public decimal TicketProgress { get; set; }
        [ProtoMember(35)]
        public AddMoneyDistributionWay DistributionWay { get; set; }
        [ProtoMember(36)]
        public string Attach { get; set; }
        [ProtoMember(37)]
        // [XmlMapping("MinBonusMoney", 34)]
        public decimal MinBonusMoney { get; set; }
        [ProtoMember(38)]
        // [XmlMapping("MaxBonusMoney", 35)]
        public decimal MaxBonusMoney { get; set; }
        [ProtoMember(39)]
        // [XmlMapping("ExtensionOne", 36)]
        public string ExtensionOne { get; set; }
        [ProtoMember(40)]
        //[XmlMapping("IsAppend", 37)]
        public bool IsAppend { get; set; }
        [ProtoMember(41)]
        // [XmlMapping("ComplateDateTime", 38)]
        public DateTime ComplateDateTime { get; set; }
        [ProtoMember(42)]
        // [XmlMapping("BetTime", 39)]
        public DateTime BetTime { get; set; }
        [ProtoMember(43)]
        //  [XmlMapping("SchemeSource", 40)]
        public SchemeSource SchemeSource { get; set; }
        [ProtoMember(44)]
        // [XmlMapping("TicketTime", 41)]
        public DateTime? TicketTime { get; set; }
        [ProtoMember(45)]
        // [XmlMapping("RedBagMoney", 42)]
        public decimal RedBagMoney { get; set; }
        [ProtoMember(46)]
        // [XmlMapping("RedBagAwardsMoney", 43)]
        public decimal RedBagAwardsMoney { get; set; }
        [ProtoMember(47)]
        //  [XmlMapping("BonusAwardsMoney", 44)]
        public decimal BonusAwardsMoney { get; set; }
    }




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

    public class Sports_ComplateInfoCollection : List<Sports_ComplateInfoList>
    {
    }


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

    //[XmlMapping("antecode", 0)]
    //public class Sports_AnteCodeQueryInfo : XmlMappingObject
    //{
    //    [XmlMapping("LeagueId", 0, MappingType = MappingType.Attribute)]
    //    public string LeagueId { get; set; }
    //    [XmlMapping("LeagueName", 1, MappingType = MappingType.Attribute)]
    //    public string LeagueName { get; set; }
    //    [XmlMapping("LeagueColor", 2, MappingType = MappingType.Attribute)]
    //    public string LeagueColor { get; set; }
    //    [XmlMapping("StartTime", 3, MappingType = MappingType.Attribute)]
    //    public DateTime StartTime { get; set; }
    //    [XmlMapping("MatchId", 4, MappingType = MappingType.Attribute)]
    //    public string MatchId { get; set; }
    //    [XmlMapping("MatchIdName", 5, MappingType = MappingType.Attribute)]
    //    public string MatchIdName { get; set; }
    //    [XmlMapping("HomeTeamId", 6, MappingType = MappingType.Attribute)]
    //    public string HomeTeamId { get; set; }
    //    [XmlMapping("HomeTeamName", 7, MappingType = MappingType.Attribute)]
    //    public string HomeTeamName { get; set; }
    //    [XmlMapping("GuestTeamId", 8, MappingType = MappingType.Attribute)]
    //    public string GuestTeamId { get; set; }
    //    [XmlMapping("GuestTeamName", 9, MappingType = MappingType.Attribute)]
    //    public string GuestTeamName { get; set; }
    //    [XmlMapping("IssuseNumber", 10, MappingType = MappingType.Attribute)]
    //    public string IssuseNumber { get; set; }
    //    [XmlMapping("AnteCode", 11, MappingType = MappingType.Attribute)]
    //    public string AnteCode { get; set; }
    //    [XmlMapping("IsDan", 12, MappingType = MappingType.Attribute)]
    //    public bool IsDan { get; set; }
    //    [XmlMapping("LetBall", 13, MappingType = MappingType.Attribute)]
    //    public int LetBall { get; set; }
    //    [XmlMapping("CurrentSp", 14, MappingType = MappingType.Attribute)]
    //    public string CurrentSp { get; set; }
    //    [XmlMapping("HalfResult", 15, MappingType = MappingType.Attribute)]
    //    public string HalfResult { get; set; }
    //    [XmlMapping("FullResult", 16, MappingType = MappingType.Attribute)]
    //    public string FullResult { get; set; }
    //    [XmlMapping("MatchResult", 17, MappingType = MappingType.Attribute)]
    //    public string MatchResult { get; set; }
    //    [XmlMapping("MatchResultSp", 18, MappingType = MappingType.Attribute)]
    //    public decimal MatchResultSp { get; set; }
    //    [XmlMapping("BonusStatus", 19, MappingType = MappingType.Attribute)]
    //    public BonusStatus BonusStatus { get; set; }
    //    [XmlMapping("GameType", 20, MappingType = MappingType.Attribute)]
    //    public string GameType { get; set; }
    //    [XmlMapping("MatchState", 21, MappingType = MappingType.Attribute)]
    //    public string MatchState { get; set; }
    //    [XmlMapping("WinNumber", 22, MappingType = MappingType.Attribute)]
    //    public string WinNumber { get; set; }
    //}

    //public class Sports_AnteCodeQueryInfoCollection : XmlMappingList<Sports_AnteCodeQueryInfo>
    //{
    //}

    /// <summary>
    /// 合买 订制跟单 规则
    /// </summary>

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
    [ProtoContract]
    public class TogetherFollowerRuleQueryInfo
    {
        [ProtoMember(1)]
        public long RuleId { get; set; }
        [ProtoMember(2)]
        public string UserId { get; set; }
        [ProtoMember(3)]
        public string UserDisplayName { get; set; }
        [ProtoMember(4)]
        public int HideDisplayNameCount { get; set; }
        /// <summary>
        /// 跟单投注金额
        /// </summary>
        [ProtoMember(5)]
        public decimal BuyMoney { get; set; }
        /// <summary>
        /// 中奖金额
        /// </summary>
        [ProtoMember(6)]
        public decimal BonusMoney { get; set; }
        [ProtoMember(7)]
        public int FollowerIndex { get; set; }
        [ProtoMember(8)]
        public DateTime CreateTime { get; set; }
        [ProtoMember(9)]
        public bool IsEnable { get; set; }
        [ProtoMember(10)]
        public string CreaterUserId { get; set; }
        [ProtoMember(11)]
        public string FollowerUserId { get; set; }
        [ProtoMember(12)]
        public string GameCode { get; set; }
        [ProtoMember(13)]
        public string GameType { get; set; }
        [ProtoMember(14)]
        public int SchemeCount { get; set; }
        [ProtoMember(15)]
        public decimal MinSchemeMoney { get; set; }
        [ProtoMember(16)]
        public decimal MaxSchemeMoney { get; set; }
        [ProtoMember(17)]
        public int FollowerCount { get; set; }
        [ProtoMember(18)]
        public decimal FollowerPercent { get; set; }
        [ProtoMember(19)]
        public bool CancelWhenSurplusNotMatch { get; set; }
        [ProtoMember(20)]
        public int CancelNoBonusSchemeCount { get; set; }
        [ProtoMember(21)]
        public decimal StopFollowerMinBalance { get; set; }
    }

    [Serializable]
    [ProtoContract]
    public class TogetherFollowerRuleQueryInfoCollection
    {
        public TogetherFollowerRuleQueryInfoCollection()
        {
            List = new List<TogetherFollowerRuleQueryInfo>();
        }
        [ProtoMember(1)]
        public List<TogetherFollowerRuleQueryInfo> List { get; set; }
        [ProtoMember(2)]
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// 定制跟单 跟我的对象
    /// </summary>

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
    [Serializable]
    [ProtoContract]
    public class Sports_TogetherSchemeQueryInfo
    {
        [ProtoMember(1)]
        public string SchemeId { get; set; }
        /// <summary>
        /// 方案来源
        /// </summary>
        [ProtoMember(2)]
        public SchemeSource SchemeSource { get; set; }
        [ProtoMember(3)]
        public bool IsTop { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [ProtoMember(4)]
        public string Title { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [ProtoMember(5)]
        public string Description { get; set; }
        /// <summary>
        /// 发起人编号
        /// </summary>
        [ProtoMember(6)]
        public string CreateUserId { get; set; }
        /// <summary>
        /// 发起人名称
        /// </summary>
        [ProtoMember(7)]
        public string CreaterDisplayName { get; set; }
        [ProtoMember(8)]
        public int CreaterHideDisplayNameCount { get; set; }
        /// <summary>
        /// 参与密码
        /// </summary>
        [ProtoMember(9)]
        public string JoinPwd { get; set; }

        /// <summary>
        /// 方案保密性
        /// </summary>
        [ProtoMember(10)]
        public TogetherSchemeSecurity Security { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        [ProtoMember(11)]
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 总份数
        /// </summary>
        [ProtoMember(12)]
        public int TotalCount { get; set; }
        [ProtoMember(13)]
        public int SoldCount { get; set; }
        [ProtoMember(14)]
        public int SurplusCount { get; set; }
        [ProtoMember(15)]
        public int JoinUserCount { get; set; }
        /// <summary>
        /// 每份单价
        /// </summary>
        [ProtoMember(16)]
        public decimal Price { get; set; }
        /// <summary>
        /// 中奖提成 0-10
        /// </summary>
        [ProtoMember(17)]
        public int BonusDeduct { get; set; }
        /// <summary>
        /// 方案提成
        /// </summary>
        [ProtoMember(18)]
        public decimal SchemeDeduct { get; set; }
        /// <summary>
        /// 发起人认购份数
        /// </summary>
        [ProtoMember(19)]
        public int Subscription { get; set; }
        /// <summary>
        /// 发起人保底份数
        /// </summary>
        [ProtoMember(20)]
        public int Guarantees { get; set; }
        /// <summary>
        /// 系统保底份数
        /// </summary>
        [ProtoMember(21)]
        public int SystemGuarantees { get; set; }
        /// <summary>
        /// 进度面分比
        /// </summary>
        [ProtoMember(22)]
        public decimal Progress { get; set; }
        /// <summary>
        /// 进度状态
        /// </summary>
        [ProtoMember(23)]
        public TogetherSchemeProgress ProgressStatus { get; set; }
        [ProtoMember(24)]
        public string GameCode { get; set; }
        [ProtoMember(25)]
        public string GameType { get; set; }
        [ProtoMember(26)]
        public string GameDisplayName { get; set; }
        [ProtoMember(27)]
        public string GameTypeDisplayName { get; set; }
        [ProtoMember(28)]
        public string PlayType { get; set; }
        [ProtoMember(29)]
        public string IssuseNumber { get; set; }
        [ProtoMember(30)]
        public int TotalMatchCount { get; set; }
        [ProtoMember(31)]
        public int Amount { get; set; }
        [ProtoMember(32)]
        public int BetCount { get; set; }
        [ProtoMember(33)]
        public DateTime StopTime { get; set; }
        [ProtoMember(34)]
        public decimal PreTaxBonusMoney { get; set; }
        [ProtoMember(35)]
        public decimal AfterTaxBonusMoney { get; set; }
        [ProtoMember(36)]
        public string WinNumber { get; set; }
        [ProtoMember(37)]
        public int BonusCount { get; set; }
        [ProtoMember(38)]
        public int HitMatchCount { get; set; }
        [ProtoMember(39)]
        public TicketStatus TicketStatus { get; set; }
        [ProtoMember(40)]
        public SchemeBettingCategory SchemeBettingCategory { get; set; }
        [ProtoMember(41)]
        public DateTime CreateTime { get; set; }
        [ProtoMember(42)]
        public BonusStatus BonusStatus { get; set; }
        [ProtoMember(43)]
        public bool IsPrizeMoney { get; set; }
        [ProtoMember(44)]
        public decimal AddMoney { get; set; }
        [ProtoMember(45)]
        public string AddMoneyDescription { get; set; }
        [ProtoMember(46)]
        public bool IsVirtualOrder { get; set; }
        [ProtoMember(47)]
        public string Attach { get; set; }
        [ProtoMember(48)]
        public decimal MinBonusMoney { get; set; }
        [ProtoMember(49)]
        public decimal MaxBonusMoney { get; set; }
        [ProtoMember(50)]
        public string ExtensionOne { get; set; }
        [ProtoMember(51)]
        public int GoldCrownCount { get; set; }
        [ProtoMember(52)]
        public int GoldCupCount { get; set; }
        [ProtoMember(53)]
        public int GoldDiamondsCount { get; set; }
        [ProtoMember(54)]
        public int GoldStarCount { get; set; }
        [ProtoMember(55)]
        public int SilverCrownCount { get; set; }
        [ProtoMember(56)]
        public int SilverCupCount { get; set; }
        [ProtoMember(57)]
        public int SilverDiamondsCount { get; set; }
        [ProtoMember(58)]
        public int SilverStarCount { get; set; }
        [ProtoMember(59)]
        public bool IsAppend { get; set; }
        [ProtoMember(60)]
        public DateTime? TicketTime { get; set; }

    }

    [Serializable]
    [ProtoContract]
    public class Sports_TogetherSchemeQueryInfoCollection
    {
        public Sports_TogetherSchemeQueryInfoCollection()
        {
            List = new List<Sports_TogetherSchemeQueryInfo>();
        }
        [ProtoMember(1)]
        public List<Sports_TogetherSchemeQueryInfo> List { get; set; }
        [ProtoMember(2)]
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// 合买参与用户信息
    /// </summary>

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

    public class SingleScheme_TogetherSchemeInfo : TogetherSchemeBase
    {
        public SingleScheme_TogetherSchemeInfo()
        {
            BettingInfo = new SingleSchemeInfo();
        }

        public SingleSchemeInfo BettingInfo { get; set; }
    }


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


    public class UserCurrentOrderInfoCollection
    {
        public UserCurrentOrderInfoCollection()
        {
            List = new List<UserCurrentOrderInfo>();
        }
        public List<UserCurrentOrderInfo> List { get; set; }
        public int TotalCount { get; set; }
    }


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


    public class SaveOrder_LotteryBettingInfoCollection : List<SaveOrder_LotteryBettingInfo>
    {
    }


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

    public class YouHuaMatchInfo
    {
        public string Sign { get; set; }
        public string MatchId { get; set; }
        public string GameType { get; set; }
        public string BetContent { get; set; }
    }

    public class YouHuaMatchInfoCollection : List<YouHuaMatchInfo>
    {
    }

    /// <summary>
    /// 优化投注号码
    /// </summary>

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

    public class YouHuaBetCodeInfoCollection : List<YouHuaBetCodeInfo>
    {
    }

    /// <summary>
    /// 奖金优化投注对象
    /// </summary>

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

    public class YouHua_TogetherSchemeInfo : TogetherSchemeBase
    {
        public YouHua_TogetherSchemeInfo()
        {
            BettingInfo = new YouHuaBetInfo();
        }

        public YouHuaBetInfo BettingInfo { get; set; }
    }

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