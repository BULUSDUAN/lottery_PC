using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core
{
    /// <summary>
    /// 宝单信息
    /// </summary>
    [CommunicationObject]
    public class TotalSingleTreasureInfo
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string SchemeId { get; set; }
        /// <summary>
        /// 宣言
        /// </summary>
        public string SingleTreasureDeclaration { get; set; }
        /// <summary>
        /// 彩种编码
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 玩法编码
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 预期回报率
        /// </summary>
        public decimal ExpectedReturnRate { get; set; }
        /// <summary>
        /// 中奖提成
        /// </summary>
        public decimal Commission { get; set; }
        /// <summary>
        /// 方案保密性
        /// </summary>
        public TogetherSchemeSecurity Security { get; set; }
        /// <summary>
        /// 抄单总人数
        /// </summary>
        public int TotalBuyCount { get; set; }
        /// <summary>
        /// 抄单金额
        /// </summary>
        public decimal TotalBuyMoney { get; set; }
        /// <summary>
        /// 宝单中奖金额
        /// </summary>
        public decimal AfterTaxBonusMoney { get; set; }
        /// <summary>
        /// 第一场比赛结束时间
        /// </summary>
        public DateTime FirstMatchStopTime { get; set; }
        /// <summary>
        /// 最后一场比赛结束时间
        /// </summary>
        public DateTime LastMatchStopTime { get; set; }
        /// <summary>
        /// 抄单盈利率
        /// </summary>
        public decimal ProfitRate { get; set; }
        /// <summary>
        /// 总的中奖金额
        /// </summary>
        public decimal TotalBonusMoney { get; set; }
        /// <summary>
        /// 预计中奖金额
        /// </summary>
        public decimal ExpectedBonusMoney { get; set; }
        /// <summary>
        /// 投注注数
        /// </summary>
        public int BetCount { get; set; }
        /// <summary>
        /// 投注场次
        /// </summary>
        public int TotalMatchCount { get; set; }
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsComplate { get; set; }
        /// <summary>
        /// 当前订单投注金额
        /// </summary>
        public decimal CurrentBetMoney { get; set; }
        /// <summary>
        /// 当前宝单的盈利率
        /// </summary>
        public decimal CurrProfitRate { get; set; }
        /// <summary>
        /// 上周盈利率
        /// </summary>
        public decimal LastweekProfitRate { get; set; }
        /// <summary>
        /// 宝单创建时间
        /// </summary>
        public DateTime BDFXCreateTime { get; set; }

    }
    [CommunicationObject]
    public class TotalSingleTreasure_Collection
    {
        public TotalSingleTreasure_Collection()
        {
            TotalSingleTreasureList = new List<TotalSingleTreasureInfo>();
            AnteCodeList = new List<AnteCodeInfo>();
        }
        public int TotalCount { get; set; }
        /// <summary>
        /// 所有购买人数
        /// </summary>
        public int AllTotalBuyCount { get; set; }
        /// <summary>
        /// 所有中奖金额
        /// </summary>
        public int AllTotalBonusMoney { get; set; }
        public List<TotalSingleTreasureInfo> TotalSingleTreasureList { get; set; }
        public List<AnteCodeInfo> AnteCodeList { get; set; }
        /// <summary>
        /// 文件生成时间
        /// </summary>
        public string FileCreateTime { get; set; } 
    }
    /// <summary>
    /// 投注号码信息
    /// </summary>
    [CommunicationObject]
    public class AnteCodeInfo
    {
        public string SchemeId { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string PlayType { get; set; }
        public string IssuseNumber { get; set; }
        public string MatchId { get; set; }
        public string AnteCode { get; set; }
        public bool IsDan { get; set; }
    }
    /// <summary>
    /// 关注信息
    /// </summary>
    [CommunicationObject]
    public class ConcernedInfo
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 被关注人数
        /// </summary>
        public int BeConcernedUserCount { get; set; }
        /// <summary>
        /// 关注人数
        /// </summary>
        public int ConcernedUserCount { get; set; }
        /// <summary>
        /// 晒单数
        /// </summary>
        public int SingleTreasureCount { get; set; }
        /// <summary>
        /// 是否关注
        /// </summary>
        public bool IsGZ { get; set; }
        //public decimal OneDayProfitRate { get; set; }
        //public decimal TwoDayProfitRate { get; set; }
        //public decimal ThreeDayProfitRate { get; set; }
        //public decimal FourDayProfitRate { get; set; }
        //public decimal FiveDayProfitRate { get; set; }
        //public decimal SixDayProfitRate { get; set; }
        //public decimal SevenDayProfitRate { get; set; }
        /// <summary>
        /// 排行
        /// </summary>
        public int RankNumber { get; set; }
        /// <summary>
        /// 最近时间段盈利率
        /// </summary>
        public NearTimeProfitRate_Collection NearTimeProfitRateCollection { get; set; }
        /// <summary>
        /// 文件生成时间
        /// </summary>
        public string FileCreateTime { get; set; }
    }
    /// <summary>
    /// 最近时间盈利率
    /// </summary>
    [CommunicationObject]
    public class NearTimeProfitRateInfo
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int RowNumber { get; set; }
        /// <summary>
        /// 当前日期
        /// </summary>
        public string CurrDate { get; set; }
        /// <summary>
        /// 当前宝单盈利率
        /// </summary>
        public decimal CurrProfitRate { get; set; }
    }
    [CommunicationObject]
    public class NearTimeProfitRate_Collection
    {
        public NearTimeProfitRate_Collection()
        {
            NearTimeProfitRateList = new List<NearTimeProfitRateInfo>();
        }
        public List<NearTimeProfitRateInfo> NearTimeProfitRateList { get; set; }
    }
    /// <summary>
    /// 宝单分享-订单详情
    /// </summary>
    [CommunicationObject]
    public class BDFXOrderDetailInfo
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string SchemeId { get; set; }
        /// <summary>
        /// 抄单总人数
        /// </summary>
        public int TotalBuyCount { get; set; }
        /// <summary>
        /// 抄单金额
        /// </summary>
        public decimal TotalBuyMoney { get; set; }
        /// <summary>
        /// 宝单中奖金额
        /// </summary>
        public decimal AfterTaxBonusMoney { get; set; }
        /// <summary>
        /// 盈利率
        /// </summary>
        public decimal ProfitRate { get; set; }
        /// <summary>
        /// 总的中奖金额
        /// </summary>
        public decimal TotalBonusMoney { get; set; }
        /// <summary>
        /// 彩种编码
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 玩法编码
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 预期回报率
        /// </summary>
        public decimal ExpectedReturnRate { get; set; }
        /// <summary>
        /// 宣言
        /// </summary>
        public string SingleTreasureDeclaration { get; set; }
        /// <summary>
        /// 排行
        /// </summary>
        public int RankNumber { get; set; }
        /// <summary>
        /// 倍数
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 注数
        /// </summary>
        public int BetCount { get; set; }
        /// <summary>
        /// 过关方式
        /// </summary>
        public string PlayType { get; set; }
        /// <summary>
        /// 投注场次
        /// </summary>
        public int TotalMatchCount { get; set; }
        /// <summary>
        /// 第一场比赛结束时间
        /// </summary>
        public DateTime FirstMatchStopTime { get; set; }
        /// <summary>
        /// 最后一场比赛结束时间
        /// </summary>
        public DateTime LastMatchStopTime { get; set; }
        /// <summary>
        /// 预计中奖金额
        /// </summary>
        public decimal ExpectedBonusMoney { get; set; }
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsComplate { get; set; }
        /// <summary>
        /// 当前订单投注金额
        /// </summary>
        public decimal CurrentBetMoney { get; set; }
        /// <summary>
        /// 当前宝单的盈利率
        /// </summary>
        public decimal CurrProfitRate { get; set; }
        /// <summary>
        /// 中奖提成
        /// </summary>
        public decimal Commission { get; set; }
        /// <summary>
        /// 方案保密性
        /// </summary>
        public TogetherSchemeSecurity Security { get; set; }
        /// <summary>
        /// 方案投注类别
        /// </summary>
        public SchemeBettingCategory SchemeBettingCategory { get; set; }
        /// <summary>
        /// 票状态
        /// </summary>
        public TicketStatus TicketStatus { get; set; }
        /// <summary>
        /// 最近时间段盈利率
        /// </summary>
        public NearTimeProfitRate_Collection NearTimeProfitRateCollection { get; set; }
        /// <summary>
        /// 投注内容信息(包含比赛结果)
        /// </summary>
        public Sports_AnteCodeQueryInfoCollection AnteCodeCollection { get; set; }
        /// <summary>
        /// 投注内容
        /// </summary>
        public List<AnteCodeInfo> AnteCodeList { get; set; }
    }

    /// <summary>
    /// 高手排行
    /// </summary>
    [CommunicationObject]
    public class BDFXGSRankInfo
    {
        /// <summary>
        /// 排行
        /// </summary>
        public int RankNumber { get; set; }
        /// <summary>
        /// 上周排行
        /// </summary>
        public int LastweekRank { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 粉丝数
        /// </summary>
        public int BeConcernedUserCount { get; set; }
        /// <summary>
        /// 晒单数
        /// </summary>
        public int SingleTreasureCount { get; set; }
        /// <summary>
        /// 当前宝单盈利率
        /// </summary>
        public decimal CurrProfitRate { get; set; }
        /// <summary>
        /// 是否关注
        /// </summary>
        public bool IsGZ { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string SchemeId { get; set; }
    }
    [CommunicationObject]
    public class BDFXGSRank_Collection
    {
        public BDFXGSRank_Collection()
        {
            RankList = new List<BDFXGSRankInfo>();
        }
        public int TotalCount { get; set; }
        public List<BDFXGSRankInfo> RankList { get; set; }
    }

    [CommunicationObject]
    public class BDFXCommisionInfo
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 中奖提成
        /// </summary>
        public decimal Commission { get; set; }
    }
    /// <summary>
    /// 牛人排行
    /// </summary>
    [CommunicationObject]
    public class BDFXNRRankListInfo
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public decimal CurrProfitRate { get; set; }
        public int RankNumber { get; set; }
    }

    [CommunicationObject]
    public class BDFXNRRankList_Collection
    {
        public BDFXNRRankList_Collection()
        {
            RanList = new List<BDFXNRRankListInfo>();
        }
        public int TotalCount { get; set; }
        public List<BDFXNRRankListInfo> RanList { get; set; }
    }

    [CommunicationObject]
    public class SingleTreasureAttentionInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 被关注用户编号
        /// </summary>
        public string BeConcernedUserId { get; set; }
        /// <summary>
        /// 关注者用户编号
        /// </summary>
        public string ConcernedUserId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class SingleTreasureAttention_Collection : List<SingleTreasureAttentionInfo>
    {

    }

    /// <summary>
    /// 网页宝单或大单推荐专家
    /// </summary>
    [CommunicationObject]
    public class WebUserSchemeShareExpertInfo
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int HideDisplayNameCount { get; set; }
        public decimal TotalProfit { get; set; }
        public int TotalFansCount { get; set; }
        /// <summary>
        /// 万元户等级
        /// </summary>
        public string MaxLevelName { get; set; }
        /// <summary>
        /// 用户自定义头像
        /// </summary>
        public string UserCustomerImgUrl { get; set; }
    }
    [CommunicationObject]
    public class WebUserSchemeShareExpert_Collection
    {
        public WebUserSchemeShareExpert_Collection()
        {
            UserSchemeShareExpertList = new List<WebUserSchemeShareExpertInfo>();
        }
        public int TotalCount { get; set; }
        public List<WebUserSchemeShareExpertInfo> UserSchemeShareExpertList { get; set; }
    }

    /// <summary>
    /// 宝单、大单推荐专家
    /// </summary>
    [CommunicationObject]
    public class UserSchemeShareExpertInfo
    {
        /// <summary>
        /// 主键编号
        /// </summary>
        public Int64 Id { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 专家类别：分为宝单专家和大单专家
        /// </summary>
        public CopyOrderSource ExpertType { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 显示排序号
        /// </summary>
        public int ShowSort { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
    }
    /// <summary>
    /// 宝单、大单推荐专家
    /// </summary>
    [CommunicationObject]
    public class UserSchemeShareExpert_Collection
    {
        public UserSchemeShareExpert_Collection()
        {
            SchemeShareExpertList = new List<UserSchemeShareExpertInfo>();
        }
        public int TotalCount { get; set; }
        public List<UserSchemeShareExpertInfo> SchemeShareExpertList { get; set; }
    }
}
