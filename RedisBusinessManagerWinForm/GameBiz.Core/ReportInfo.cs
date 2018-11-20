using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Mappings;
using Common.Communication;

namespace GameBiz.Core
{
    #region 过关统计

    /// <summary>
    /// 过关统计订单
    /// </summary>
    [CommunicationObject]
    public class SportsOrder_GuoGuanInfo
    {
        public string SchemeId { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 订单类型
        /// </summary>
        public SchemeType SchemeType { get; set; }
        /// <summary>
        /// 投注注数
        /// </summary>
        public int BetCount { get; set; }
        /// <summary>
        /// 正确注数
        /// </summary>
        public int RightCount { get; set; }
        /// <summary>
        /// 错一注数
        /// </summary>
        public int Error1Count { get; set; }
        /// <summary>
        /// 错二注数
        /// </summary>
        public int Error2Count { get; set; }
        /// <summary>
        /// 订单总注数
        /// </summary>
        public int TotalMatchCount { get; set; }
        /// <summary>
        /// 命中场数
        /// </summary>
        public int HitMatchCount { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 中奖状态
        /// </summary>
        public BonusStatus BonusStatus { get; set; }
        /// <summary>
        /// 中奖金额
        /// </summary>
        public decimal BonusMoney { get; set; }


        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public int UserHideDisplayNameCount { get; set; }

        /// <summary>
        /// 金星个数
        /// </summary>
        public int GoldStarCount { get; set; }
        /// <summary>
        /// 金钻个数
        /// </summary>
        public int GoldDiamondsCount { get; set; }
        /// <summary>
        /// 金杯个数
        /// </summary>
        public int GoldCupCount { get; set; }
        /// <summary>
        /// 金冠个数
        /// </summary>
        public int GoldCrownCount { get; set; }


        /// <summary>
        /// 银星个数
        /// </summary>
        public int SilverStarCount { get; set; }
        /// <summary>
        /// 银钻个数
        /// </summary>
        public int SilverDiamondsCount { get; set; }
        /// <summary>
        /// 银杯个数
        /// </summary>
        public int SilverCupCount { get; set; }
        /// <summary>
        /// 银冠个数
        /// </summary>
        public int SilverCrownCount { get; set; }
        /// <summary>
        /// 是否为虚拟订单
        /// </summary>
        public bool IsVirtualOrder { get; set; }
        /// <summary>
        /// 方案投注类别
        /// </summary>
        public SchemeBettingCategory SchemeBettingCategory { get; set; }
        /// <summary>
        /// 投注时间
        /// </summary>
        public DateTime BetTime { get; set; }

    }
    [CommunicationObject]
    public class SportsOrder_GuoGuanInfoCollection
    {
        public SportsOrder_GuoGuanInfoCollection()
        {
            ReportItemList = new List<SportsOrder_GuoGuanInfo>();
        }

        public int TotalCount { get; set; }
        public List<SportsOrder_GuoGuanInfo> ReportItemList { get; set; }
    }


    #endregion

    #region 自定义报表

    [CommunicationObject]
    public class ReportInfo_Customer
    {
        public string UUID { get; set; }
        public string DisplayName { get; set; }
        public string Tag { get; set; }
        public bool IsShow { get; set; }
        public bool TopOnMenu { get; set; }
        public string Sql { get; set; }
        public IList<ReportParameterInfo> Parameters { get; set; }
    }
    [CommunicationObject]
    public class ReportParameterInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Default { get; set; }
        public object Value { get; set; }
    }
    [CommunicationObject]
    public class ReportInfoCollection : List<ReportInfo_Customer>
    {
    }

    #endregion

    #region 后台首页统计

    [CommunicationObject]
    public class BackgroundIndexReportInfo
    {
        /// <summary>
        /// 按天统计金额
        /// </summary>
        public decimal ReportMoney_Day { get; set; }
        /// <summary>
        /// 按月统计金额
        /// </summary>
        public decimal ReportMoney_Month { get; set; }
        /// <summary>
        /// 报表类型
        /// </summary>
        public string ReportType { get; set; }
        /// <summary>
        /// 按天查询列表地址
        /// </summary>
        public string ListUrl_Day { get; set; }
        /// <summary>
        /// 按月查询列表地址
        /// </summary>
        public string ListUrl_Month { get; set; }
    }
    [CommunicationObject]
    public class BackgroundIndexReportInfo_Collection
    {
        public BackgroundIndexReportInfo_Collection()
        {
            ReportList = new List<BackgroundIndexReportInfo>();
        }
        public List<BackgroundIndexReportInfo> ReportList { get; set; }
    }

    #endregion

    /// <summary>
    /// 用户购彩信息统计
    /// </summary>
    [CommunicationObject]
    public class UserBetStatisticsInfo
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 显示名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 实名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 账户总余额=充值+奖金+佣金+红包+名家
        /// </summary>
        public decimal TotalBalance { get; set; }
        /// <summary>
        /// 总投注额
        /// </summary>
        public decimal TotalBettingMoney { get; set; }
        /// <summary>
        /// 上级编号
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// 返点
        /// </summary>
        public string StrRebate { get; set; }
        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime LastLoginTime { get; set; }
        /// <summary>
        /// 最后投注时间
        /// </summary>
        public DateTime LastBetTime { get; set; }
    }
    [CommunicationObject]
    public class UserBetStatistics_Collection
    {
        public UserBetStatistics_Collection()
        {
            BetList = new List<UserBetStatisticsInfo>();
        }
        public int TotalCount { get; set; }
        public List<UserBetStatisticsInfo> BetList { get; set; }
    }
    /// <summary>
    /// 导出代理返点金额
    /// </summary>
    [CommunicationObject]
    public class AgentRebateStatisticsInfo
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 显示名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 玩法
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string SchemeId { get; set; }
        /// <summary>
        /// 订单投注金额
        /// </summary>
        public decimal OrderTotalMoney { get; set; }
        /// <summary>
        /// 返点
        /// </summary>
        public decimal Rebate { get; set; }
        /// <summary>
        /// 返点金额
        /// </summary>
        public decimal PayMoney { get; set; }
        /// <summary>
        /// 返点时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class AgentRebateStatistics_Collection
    {
        public AgentRebateStatistics_Collection()
        {
            AgentRebateList = new List<AgentRebateStatisticsInfo>();
        }
        public int TotalCount { get; set; }
        public List<AgentRebateStatisticsInfo> AgentRebateList { get; set; }
    }
}
