using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.XmlAnalyzer;
using Common;
using Common.Communication;

namespace GameBiz.Core
{
    #region 超级发起人
    /// <summary>
    /// 超级发起人信息
    /// </summary>
    [CommunicationObject]
    [XmlMapping("item", 0)]
    public class SupperCreatorInfo : XmlMappingObject
    {
        [XmlMapping("userId", 0, MappingType = MappingType.Attribute)]
        public string UserId { get; set; }
        [XmlMapping("displayName", 1, MappingType = MappingType.Attribute)]
        public string DisplayName { get; set; }
        [XmlMapping("isSupper", 2, MappingType = MappingType.Attribute)]
        public bool IsSupper { get; set; }
        [XmlMapping("isStar", 3, MappingType = MappingType.Attribute)]
        public bool IsStar { get; set; }
        [XmlMapping("hasTogetherScheme", 4, MappingType = MappingType.Attribute)]
        public bool HasTogetherScheme { get; set; }
        [XmlMapping("lastStopTime", 5, MappingType = MappingType.Attribute)]
        public DateTime LastStopTime { get; set; }
    }
    [CommunicationObject]
    public class SupperCreatorCollection : XmlMappingList<SupperCreatorInfo>
    {
    }
    #endregion

    #region 用户登录日志
    [CommunicationObject]
    [Serializable]
    public class UserLoginHistoryInfo
    {
        public int Id { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public string LoginFrom { get; set; }
        /// <summary>
        /// IP
        /// </summary>
        public string LoginIp { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string IpDisplayName { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime LoginTime { get; set; }
    }
    [CommunicationObject]
    public class UserLoginHistoryCollection : List<UserLoginHistoryInfo>
    {
    }
    #endregion

    #region 个人主页

    #region 基础信息
    [CommunicationObject]
    public class ProfileUserInfo
    {
        public int Id { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户显示名
        /// </summary>
        public string UserDisplayName { get; set; }
        /// <summary>
        /// 隐藏匿名位数
        /// </summary>
        public int HideNameCount { get; set; }
        /// <summary>
        /// 关注人数
        /// </summary>
        public int AttentionCount { get; set; }
        /// <summary>
        /// 被关注人数
        /// </summary>
        public int AttentionedCount { get; set; }
        /// <summary>
        /// 用户最高得奖等级
        /// </summary>
        public string MaxLevelName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
    #endregion

    #region 数据统计
    [CommunicationObject]
    public class ProfileDataReport
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 发起合买次数
        /// </summary>
        public int CreateSchemeCount { get; set; }
        /// <summary>
        /// 参与合买次数
        /// </summary>
        public int JoinSchemeCount { get; set; }
        /// <summary>
        /// 总中奖次数
        /// </summary>
        public int TotalBonusCount { get; set; }
        /// <summary>
        /// 总中奖金额
        /// </summary>
        public decimal TotalBonusMoney { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
    #endregion

    #region 访客记录
    [CommunicationObject]
    public class ProfileVisitHistoryInfo
    {
        public int Id { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 最高中奖中文
        /// </summary>
        public string MaxLevelName { get; set; }
        /// <summary>
        /// 访客用户Id
        /// </summary>
        public string VisitUserId { get; set; }
        /// <summary>
        /// 用户显示名
        /// </summary>
        public string VisitorUserDisplayName { get; set; }
        /// <summary>
        /// 用户名匿名位数
        /// </summary>
        public string VisitorHideNameCount { get; set; }
        /// <summary>
        /// 登陆IP
        /// </summary>
        public string VisitorIp { get; set; }
        /// <summary>
        /// IP显示地址
        /// </summary>
        public string IpDisplayName { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class ProfileVisitHistoryCollection : List<ProfileVisitHistoryInfo>
    {
    }
    #endregion

    #region 获奖级别统计
    [CommunicationObject]
    public class ProfileBonusLevelInfo
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 最高中奖等级
        /// </summary>
        public int MaxLevelValue { get; set; }
        /// <summary>
        /// 最高中奖中文
        /// </summary>
        public string MaxLevelName { get; set; }
        /// <summary>
        /// 中奖一百元次数
        /// </summary>
        public int WinOneHundredCount { get; set; }
        /// <summary>
        /// 中一千元次数
        /// </summary>
        public int WinOneThousandCount { get; set; }
        /// <summary>
        /// 中一万元次数
        /// </summary>
        public int WinTenThousandCount { get; set; }
        /// <summary>
        /// 中十万元次数
        /// </summary>
        public int WinOneHundredThousandCount { get; set; }
        /// <summary>
        /// 中百万元次数
        /// </summary>
        public int WinOneMillionCount { get; set; }
        /// <summary>
        /// 中一千万次数
        /// </summary>
        public int WinTenMillionCount { get; set; }
        /// <summary>
        /// 中一亿元次数
        /// </summary>
        public int WinHundredMillionCount { get; set; }
    }

    [CommunicationObject]
    public class ProfileBonusLevelCollection
    {
        public ProfileBonusLevelCollection()
        {
            List = new List<ProfileBonusLevelInfo>();
        }
        public List<ProfileBonusLevelInfo> List { get; set; }
    }



    #endregion

    #region 最新动态
    [CommunicationObject]
    public class ProfileDynamicInfo
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string UserDisplayName { get; set; }
        public int HideDisplayNameCount { get; set; }
        /// <summary>
        /// 被参与用户
        /// </summary>
        public string UserId2 { get; set; }
        /// <summary>
        /// UserId2显示名称
        /// </summary>
        public string User2DisplayName { get; set; }
        public int User2HideDisplayNameCount { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 玩法
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 用户动作
        /// </summary>
        public string DynamicType { get; set; }
        /// <summary>
        /// 方案号
        /// </summary>
        public string SchemeId { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalMonery { get; set; }
        /// <summary>
        /// 没份单价
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 发起人保底份数
        /// </summary>
        public int Guarantees { get; set; }
        /// <summary>
        /// 发起人认购份数
        /// </summary>
        public int Subscription { get; set; }
        /// <summary>
        /// 方案进度百分比
        /// </summary>
        public decimal Progress { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class ProfileDynamicCollection
    {
        public ProfileDynamicCollection()
        {
            List = new List<ProfileDynamicInfo>();
        }
        public List<ProfileDynamicInfo> List { get; set; }
        public int TotalCount { get; set; }
    }
    #endregion

    #region 最新中奖
    [CommunicationObject]
    public class ProfileLastBonusInfo
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 方案号
        /// </summary>
        public string SchemeId { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 玩法
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public decimal BonusMoney { get; set; }
        /// <summary>
        /// 中奖时间
        /// </summary>
        public DateTime BonusTime { get; set; }
    }

    [CommunicationObject]
    public class ProfileLastBonusCollection
    {
        public ProfileLastBonusCollection()
        {
            List = new List<ProfileLastBonusInfo>();
        }
        public List<ProfileLastBonusInfo> List { get; set; }
        public int TotalCount { get; set; }
    }


    #endregion

    #region 历史战绩
    [CommunicationObject]
    public class ProfileBeedingsInfo
    {
        /// <summary>
        /// 用户Id
        /// </summary>
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
        /// 总中奖次数
        /// </summary>
        public int TotalBonusCount { get; set; }
        /// <summary>
        /// 总中奖金额
        /// </summary>
        public decimal TotalBonusMoney { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class ProfileBeedingsCollection : List<ProfileBeedingsInfo>
    {
    }
    #endregion

    #region 定制跟单
    [CommunicationObject]
    [XmlMapping("item", 0)]
    public class ProfileFollowerInfo : XmlMappingObject
    {
        [XmlMapping("gameCode", 1, MappingType = MappingType.Attribute)]
        public string GameCode { get; set; }
        [XmlMapping("gameType", 2, MappingType = MappingType.Attribute)]
        public string GameType { get; set; }
        [XmlMapping("gameDisplayName", 3, MappingType = MappingType.Attribute)]
        public string GameDisplayName { get; set; }
        [XmlMapping("followerCount", 4, MappingType = MappingType.Attribute)]
        public int FollowerCount { get; set; }
        [XmlMapping("updateTime", 10, MappingType = MappingType.Attribute)]
        public DateTime UpdateTime { get; set; }
    }
    [CommunicationObject]
    public class ProfileFollowerCollection : XmlMappingList<ProfileFollowerInfo>
    {
    }
    #endregion

    #region 用户关注
    [CommunicationObject]
    public class ProfileAttentionInfo
    {
        /// <summary>
        /// 关注人Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 关注人显示名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 隐藏用户名数
        /// </summary>
        public int HideDisplayNameCount { get; set; }
        /// <summary>
        /// 用户中奖显示名
        /// </summary>
        public string MaxLevelName { get; set; }

    }
    [CommunicationObject]
    public class ProfileAttentionCollection
    {
        public ProfileAttentionCollection()
        {
            List = new List<ProfileAttentionInfo>();
        }
        public List<ProfileAttentionInfo> List { get; set; }
        public int TotalCount { get; set; }
    }
    #endregion

    /// <summary>
    /// 用户博客全部信息
    /// </summary>
    [CommunicationObject]
    public class UserBlogInfo
    {
        public ProfileUserInfo ProfileUserInfo { get; set; }
        public ProfileBonusLevelInfo ProfileBonusLevelInfo { get; set; }
        public ProfileLastBonusCollection ProfileLastBonusCollection { get; set; }
        public ProfileDataReport ProfileDataReport { get; set; }
        public BonusOrderInfoCollection BonusOrderInfoCollection { get; set; }
        public int FollowerCount { get; set; }
        public UserCurrentOrderInfoCollection UserCurrentOrderInfoCollection { get; set; }
    }

    #endregion


    #region 竞彩比赛缓存对象

    /// <summary>
    /// 北京单场比赛缓存对象
    /// </summary>
    public class Cache_BJDC_MatchInfo
    {
        /// <summary>
        /// IssuseNumber|MatchOrderId
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 联赛排序编号
        /// </summary>
        public int MatchOrderId { get; set; }
        /// <summary>
        /// 联赛名字
        /// </summary>
        public string MatchName { get; set; }
        /// <summary>
        /// 比赛开始时间
        /// </summary>
        public DateTime MatchStartTime { get; set; }
        /// <summary>
        /// 本地结束时间
        /// </summary>
        public DateTime LocalStopTime { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        public string HomeTeamName { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        public string GuestTeamName { get; set; }
        /// <summary>
        /// 限制玩法列表
        /// </summary>
        public string PrivilegesType { get; set; }
    }

    public class Cache_JCZQ_MatchInfo
    {
        /// <summary>
        /// 比赛Id : 120813001
        /// </summary>
        public string MatchId { get; set; }
        /// <summary>
        /// 比赛编号名称
        /// </summary>
        public string MatchIdName { get; set; }
        /// <summary>
        /// 联赛名称
        /// </summary>
        public string LeagueName { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        public string HomeTeamName { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        public string GuestTeamName { get; set; }
        /// <summary>
        /// 单式停止投注时间
        /// </summary>
        public DateTime DSStopBettingTime { get; set; }
        /// <summary>
        /// 复式停止投注时间
        /// </summary>
        public DateTime FSStopBettingTime { get; set; }
        /// <summary>
        /// 限玩法列表
        /// </summary>
        public string PrivilegesType { get; set; }
        /// <summary>
        /// 比赛停售说明
        /// </summary>
        public string MatchStopDesc { get; set; }
    }

    public class Cache_JCLQ_MatchInfo
    {
        /// <summary>
        /// 比赛Id : 120813001
        /// </summary>
        public string MatchId { get; set; }
        /// <summary>
        /// 比赛编号名称
        /// </summary>
        public string MatchIdName { get; set; }
        /// <summary>
        /// 联赛名称
        /// </summary>
        public string LeagueName { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        public string HomeTeamName { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        public string GuestTeamName { get; set; }
        /// <summary>
        /// 单式停止投注时间
        /// </summary>
        public DateTime DSStopBettingTime { get; set; }
        /// <summary>
        /// 复式停止投注时间
        /// </summary>
        public DateTime FSStopBettingTime { get; set; }
        /// <summary>
        /// 限玩法列表
        /// </summary>
        public string PrivilegesType { get; set; }
    }

    #endregion
}
