using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBiz.Domain.Entities
{
    /// <summary>
    /// 最新动态
    /// </summary>
    public class Blog_Dynamic
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public virtual string UserDisplayName { get; set; }
        /// <summary>
        /// 被参与用户
        /// </summary>
        public virtual string UserId2 { get; set; }
        /// <summary>
        /// UserId2显示名称
        /// </summary>
        public virtual string User2DisplayName { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        public virtual string GameCode { get; set; }
        /// <summary>
        /// 玩法
        /// </summary>
        public virtual string GameType { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public virtual string IssuseNumber { get; set; }
        /// <summary>
        /// 用户动作
        /// </summary>
        public virtual string DynamicType { get; set; }
        /// <summary>
        /// 方案号
        /// </summary>
        public virtual string SchemeId { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public virtual decimal TotalMonery { get; set; }
        /// <summary>
        /// 没份单价
        /// </summary>
        public virtual decimal Price { get; set; }
        /// <summary>
        /// 发起人保底份数
        /// </summary>
        public virtual int Guarantees { get; set; }
        /// <summary>
        /// 发起人认购份数
        /// </summary>
        public virtual int Subscription { get; set; }
        /// <summary>
        /// 方案进度百分比
        /// </summary>
        public virtual decimal Progress { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 用户数据统计
    /// </summary>
    public class Blog_DataReport
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 发起合买次数
        /// </summary>
        public virtual int CreateSchemeCount { get; set; }
        /// <summary>
        /// 参与合买次数
        /// </summary>
        public virtual int JoinSchemeCount { get; set; }
        /// <summary>
        /// 总中奖次数
        /// </summary>
        public virtual int TotalBonusCount { get; set; }
        /// <summary>
        /// 总中奖金额
        /// </summary>
        public virtual decimal TotalBonusMoney { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public virtual DateTime UpdateTime { get; set; }
    }

    /// <summary>
    /// 用户获奖记录
    /// </summary>
    public class Blog_ProfileBonusLevel
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 最高中奖等级
        /// </summary>
        public virtual int MaxLevelValue { get; set; }
        /// <summary>
        /// 最高中奖中文
        /// </summary>
        public virtual string MaxLevelName { get; set; }
        /// <summary>
        /// 中奖一百元次数
        /// </summary>
        public virtual int WinOneHundredCount { get; set; }
        /// <summary>
        /// 中一千元次数
        /// </summary>
        public virtual int WinOneThousandCount { get; set; }
        /// <summary>
        /// 中一万元次数
        /// </summary>
        public virtual int WinTenThousandCount { get; set; }
        /// <summary>
        /// 中十万元次数
        /// </summary>
        public virtual int WinOneHundredThousandCount { get; set; }
        /// <summary>
        /// 中百万元次数
        /// </summary>
        public virtual int WinOneMillionCount { get; set; }
        /// <summary>
        /// 中一千万次数
        /// </summary>
        public virtual int WinTenMillionCount { get; set; }
        /// <summary>
        /// 中一亿元次数
        /// </summary>
        public virtual int WinHundredMillionCount { get; set; }
        /// <summary>
        /// 总中奖金额
        /// </summary>
        public virtual decimal TotalBonusMoney { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public virtual DateTime UpdateTime { get; set; }
    }

    /// <summary>
    /// 用户最新中奖
    /// </summary>
    public class Blog_NewProfileLastBonus
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 方案号
        /// </summary>
        public virtual string SchemeId { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        public virtual string GameCode { get; set; }
        /// <summary>
        /// 玩法
        /// </summary>
        public virtual string GameType { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public virtual string IssuseNumber { get; set; }
        /// <summary>
        /// 中奖金额
        /// </summary>
        public virtual decimal BonusMoney { get; set; }
        /// <summary>
        /// 中奖时间
        /// </summary>
        public virtual DateTime BonusTime { get; set; }
    }

    /// <summary>
    /// 用户登陆历史
    /// </summary>
    public class Blog_UserLoginHistory
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 登陆来至那个通道
        /// </summary>
        public virtual string LoginFrom { get; set; }
        /// <summary>
        /// IP
        /// </summary>
        public virtual string LoginIp { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public virtual string IpDisplayName { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public virtual DateTime LoginTime { get; set; }
    }

    /// <summary>
    /// 访客历史记录
    /// </summary>
    public class Blog_UserVisitHistory
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 访客用户Id
        /// </summary>
        public virtual string VisitUserId { get; set; }
        /// <summary>
        /// 用户显示名
        /// </summary>
        public virtual string VisitorUserDisplayName { get; set; }
        /// <summary>
        /// 用户名匿名位数
        /// </summary>
        public virtual string VisitorHideNameCount { get; set; }
        /// <summary>
        /// 登陆IP
        /// </summary>
        public virtual string VisitorIp { get; set; }
        /// <summary>
        /// IP显示地址
        /// </summary>
        public virtual string IpDisplayName { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// yqid普通用户推广
    /// </summary>
    public class Blog_UserSpread
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// userName
        /// </summary>
        public virtual string userName { get; set; }
        /// <summary>
        /// AgentId
        /// </summary>
        public virtual string AgentId { get; set; }
        /// <summary>
        /// CrateTime
        /// </summary>
        public virtual DateTime CrateTime { get; set; }
        /// <summary>
        /// CTZQ
        /// </summary>
        public virtual decimal CTZQ { get; set; }
        /// <summary>
        /// BJDC
        /// </summary>
        public virtual decimal BJDC { get; set; }
        /// <summary>
        /// JCZQ
        /// </summary>
        public virtual decimal JCZQ { get; set; }
        /// <summary>
        /// JCLQ
        /// </summary>
        public virtual decimal JCLQ { get; set; }
        /// <summary>
        /// SZC
        /// </summary>
        public virtual decimal SZC { get; set; }
        /// <summary>
        /// GPC
        /// </summary>
        public virtual decimal GPC { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public virtual DateTime UpdateTime { get; set; }
    }
    /// <summary>
    /// yqid普通用户推广 领取红包
    /// </summary>
    public class Blog_UserSpreadGiveRedBag
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// UserId
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 红包
        /// </summary>
        public virtual decimal giveRedBagMoney { get; set; }
        /// <summary>
        /// 现金(预留字段)
        /// </summary>
        public virtual decimal GiveBonusMoney { get; set; }
        /// <summary>
        /// 满足条件的会员个数
        /// </summary>
        public virtual int userCount { get; set; }
        /// <summary>
        /// 领取的红包个数
        /// </summary>
        public virtual int redBagCount { get; set; }
        /// <summary>
        /// 当前领取了红包的会员数 比如10个满足条件的会员领取了一次 20个满足条件的也可以领取一次(这里值10 20 30 ...)
        /// </summary>
        public virtual int userGiveCount { get; set; }
        /// <summary>
        /// CrateTime
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public virtual DateTime UpdateTime { get; set; }
    }

    /// <summary>
    /// fxid分享推广
    /// </summary>
    public class Blog_UserShareSpread
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// UserId
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string userName { get; set; }
        /// <summary>
        /// 分享人id
        /// </summary>
        public virtual string AgentId { get; set; }
        /// <summary>
        /// 是否领取注册(要求绑定银行卡)红包
        /// </summary>
        public virtual bool isGiveRegisterRedBag { get; set; }
        /// <summary>
        /// 是否领取首次购彩红包
        /// </summary>
        public virtual bool isGiveLotteryRedBag { get; set; }
        /// <summary>
        /// CrateTime
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public virtual DateTime UpdateTime { get; set; }
        /// <summary>
        /// 本次推广 奖励 红包
        /// </summary>
        public virtual decimal giveRedBagMoney { get; set; }
        /// <summary>
        /// 是否已领取首充红包
        /// </summary>
        public virtual bool isGiveRechargeRedBag { get; set; }
    }

    public class BlogOrderShareRegisterRedBag
    {
        public virtual int Id { get; set; }
        public virtual string SchemeId { get; set; }
        public virtual string UserId { get; set; }
        public virtual bool IsGiveRegisterRedBag { get; set; }
        public virtual int RegisterCount { get; set; }
        public virtual DateTime CreateTime { get; set; }
        public virtual DateTime UpdateTime { get; set; }
        public virtual decimal RedBagPre { get; set; }
        public virtual decimal RedBagMoney { get; set; }
    }
}
