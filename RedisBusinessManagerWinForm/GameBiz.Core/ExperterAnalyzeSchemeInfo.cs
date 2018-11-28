using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using Common;

namespace GameBiz.Core
{
    /// <summary>
    /// 分析列表
    /// </summary>
    [CommunicationObject]
    public class ExperterAnalyzeSchemeInfo
    {
        public string AnalyzeId { get; set; }
        /// <summary>
        /// 专家编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 名家显示名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 文章标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 分析价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 处理类别
        /// </summary>
        public DealWithType DealWithType { get; set; }
        /// <summary>
        /// 是否购买
        /// </summary>
        public bool IsBuy { get; set; }
        /// <summary>
        /// 处理意见
        /// </summary>
        public string DisposeOpinion { get; set; }
        /// <summary>
        /// 购买时间戳
        /// </summary>
        public string CurrentTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class ExperterAnalyzeSchemeInfoCollection
    {
        public ExperterAnalyzeSchemeInfoCollection()
        {
            List = new List<ExperterAnalyzeSchemeInfo>();
        }
        public List<ExperterAnalyzeSchemeInfo> List { get; set; }
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// 某专家的分析发布历史
    /// </summary>
    [CommunicationObject]
    public class ExperterAnalyzeInfo
    {
        public string AnalyzeId { get; set; }
        /// <summary>
        /// 文章标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 售出次数
        /// </summary>
        public int SellCount { get; set; }
        /// <summary>
        /// 分析价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class ExperterAnalyzeInfoCollection
    {
        public ExperterAnalyzeInfoCollection()
        {
            List = new List<ExperterAnalyzeInfo>();
        }
        public List<ExperterAnalyzeInfo> List { get; set; }
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// 修改买分析信息
    /// </summary>
    [CommunicationObject]
    public class ExperterAnalyzeUpdateInfo
    {
        /// <summary>
        /// 分析ID
        /// </summary>
        public string AnalyzeId { get; set; }
        /// <summary>
        /// 处理类别
        /// </summary>
        public DealWithType DealWithType { get; set; }
        /// <summary>
        /// 处理意见
        /// </summary>
        public string DisposeOpinion { get; set; }
    }

    /// <summary>
    /// 专家分析交易信息
    /// </summary>
    [CommunicationObject]
    public class ExperterAnalyzeTransactionInfo
    {
        /// <summary>
        /// 分析Id
        /// </summary>
        public string AnalyzeId { get; set; }
        /// <summary>
        /// 购买用户Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 专家Id
        /// </summary>
        public string ExperterId { get; set; }
        /// <summary>
        /// 分析单价
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 当前发布时间
        /// </summary>
        public string CurrentTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class ExperterAnalyzeTransactionInfoCollection
    {
        public ExperterAnalyzeTransactionInfoCollection()
        {
            List = new List<ExperterAnalyzeTransactionInfo>();
        }
        public List<ExperterAnalyzeTransactionInfo> List { get; set; }
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// 吐槽内容
    /// </summary>
    [CommunicationObject]
    public class ExperterCommentsInfo
    {
        public int Id { get; set; }
        /// <summary>
        /// 专家编号
        /// </summary>
        public string ExperterId { get; set; }
        /// <summary>
        /// 发言用户编号
        /// </summary>
        public string SendUserId { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 用户名隐藏长度
        /// </summary>
        public int HideDisplayNameCount { get; set; }
        /// <summary>
        /// 推荐方案
        /// </summary>
        public string RecommendSchemeId { get; set; }
        /// <summary>
        /// 分析方案
        /// </summary>
        public string AnalyzeSchemeId { get; set; }
        /// <summary>
        /// 吐槽内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 处理类别
        /// </summary>
        public DealWithType DealWithType { get; set; }
        /// <summary>
        /// 吐槽类别
        /// </summary>
        public CommentsTpye CommentsTpye { get; set; }
        /// <summary>
        /// 处理意见
        /// </summary>
        public string DisposeOpinion { get; set; }
        /// <summary>
        /// 当前发布时间
        /// </summary>
        public string CurrentTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class ExperterCommentsCollection
    {
        public ExperterCommentsCollection()
        {
            List = new List<ExperterCommentsInfo>();
        }
        public List<ExperterCommentsInfo> List { get; set; }
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// 更新吐槽
    /// </summary>
    [CommunicationObject]
    public class UpdateCommentsInfo
    {
        /// <summary>
        /// 处理类别
        /// </summary>
        public DealWithType DealWithType { get; set; }
        /// <summary>
        /// 吐槽ID
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 处理意见
        /// </summary>
        public string DisposeOpinion { get; set; }
    }

    [CommunicationObject]
    public class ExperterInfo
    {
        /// <summary>
        /// 专家编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 显示名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 专家描述
        /// </summary>
        public string ExperterSummary { get; set; }
        /// <summary>
        /// 专家头像
        /// </summary>
        public string ExperterHeadImage { get; set; }
        /// <summary>
        /// 擅长彩种
        /// </summary>
        public string AdeptGameCode { get; set; }
        /// <summary>
        /// 最近发单数
        /// </summary>
        public int RecentlyOrderCount { get; set; }
        /// <summary>
        /// 专家类别
        /// </summary>
        public ExperterType ExperterType { get; set; }
        /// <summary>
        /// 周命中率
        /// </summary>
        public decimal WeekShooting { get; set; }
        /// <summary>
        /// 月命中率
        /// </summary>
        public decimal MonthShooting { get; set; }
        /// <summary>
        /// 总和命中率
        /// </summary>
        public decimal TotalShooting { get; set; }
        /// <summary>
        /// 周回报率
        /// </summary>
        public decimal WeekRate { get; set; }
        /// <summary>
        /// 月回报率
        /// </summary>
        public decimal MonthRate { get; set; }
        /// <summary>
        /// 总回报率
        /// </summary>
        public decimal TotalRate { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 被关注人数
        /// </summary>
        public int Attention { get; set; }
        public DateTime CreateTime { get; set; }
    }

    [CommunicationObject]
    public class ExperterInfoCollection
    {
        public ExperterInfoCollection()
        {
            List = new List<ExperterInfo>();
        }
        public List<ExperterInfo> List { get; set; }
        public int TotalCount { get; set; }
    }

    [CommunicationObject]
    public class ExperterAuditInfo
    {
        /// <summary>
        /// 专家编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 显示名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 专家描述
        /// </summary>
        public string ExperterSummary { get; set; }
        /// <summary>
        /// 专家头像
        /// </summary>
        public string ExperterHeadImage { get; set; }
        /// <summary>
        /// 擅长彩种
        /// </summary>
        public string AdeptGameCode { get; set; }
        public DateTime UpdateTime { get; set; }
    }

    [CommunicationObject]
    public class ExperterAuditInfoCollection
    {
        public ExperterAuditInfoCollection()
        {
            List = new List<ExperterAuditInfo>();
        }
        public List<ExperterAuditInfo> List { get; set; }
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// 专家修改历史
    /// </summary>
    [CommunicationObject]
    public class ExperterUpdateInfo
    {
        /// <summary>
        /// 专家编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 显示名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 专家描述
        /// </summary>
        public string ExperterSummary { get; set; }
        /// <summary>
        /// 专家头像
        /// </summary>
        public string ExperterHeadImage { get; set; }
        /// <summary>
        /// 擅长彩种
        /// </summary>
        public string AdeptGameCode { get; set; }
        /// <summary>
        /// 处理类别
        /// </summary>
        public DealWithType DealWithType { get; set; }
        /// <summary>
        /// 处理意见
        /// </summary>
        public string DisposeOpinion { get; set; }
        public DateTime UpdateTime { get; set; }
    }

    [CommunicationObject]
    public class ExperterUpdateInfoCollection
    {
        public ExperterUpdateInfoCollection()
        {
            List = new List<ExperterUpdateInfo>();
        }
        public List<ExperterUpdateInfo> List { get; set; }
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// 专家命中率排行
    /// </summary>
    [CommunicationObject]
    public class ExperterShootingInfo
    {
        /// <summary>
        /// 专家编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 显示名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 专家描述
        /// </summary>
        public string ExperterSummary { get; set; }
        /// <summary>
        /// 命中率
        /// </summary>
        public decimal Shooting { get; set; }
    }

    [CommunicationObject]
    public class ExperterShootingInfoCollection
    {
        public ExperterShootingInfoCollection()
        {
            List = new List<ExperterShootingInfo>();
        }
        public List<ExperterShootingInfo> List { get; set; }
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// 查询名家发单记录
    /// </summary>
    [CommunicationObject]
    public class ExperterPublishedInfo
    {
        /// <summary>
        /// 专家编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 显示名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 专家描述
        /// </summary>
        public string ExperterSummary { get; set; }
        /// <summary>
        /// 方案号
        /// </summary>
        public string SchemeId { get; set; }
        /// <summary>
        /// 发起金额
        /// </summary>
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }

    [CommunicationObject]
    public class ExperterPublishedInfoCollection
    {
        public ExperterPublishedInfoCollection()
        {
            List = new List<ExperterPublishedInfo>();
        }
        public List<ExperterPublishedInfo> List { get; set; }
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// 专家方案查询对象
    /// </summary>
    [CommunicationObject]
    public class ExperterQuerySchemeInfo
    {
        public int Id { get; set; }
        /// <summary>
        /// 方案ID
        /// </summary>
        public string SchemeId { get; set; }
        /// <summary>
        /// 名家ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 专家类别
        /// </summary>
        public ExperterType ExperterType { get; set; }
        /// <summary>
        /// 中奖状态
        /// </summary>
        public BonusStatus BonusStatus { get; set; }
        /// <summary>
        /// 支持
        /// </summary>
        public int Support { get; set; }
        /// <summary>
        /// 反对
        /// </summary>
        public int Against { get; set; }
        /// <summary>
        /// 主队的评论
        /// </summary>
        public string HomeTeamComments { get; set; }
        /// <summary>
        /// 主队的评论
        /// </summary>
        public string GuestTeamComments { get; set; }
        /// <summary>
        /// 方案截止时间
        /// </summary>
        public DateTime StopTime { get; set; }
        /// <summary>
        /// 当前发布时间
        /// </summary>
        public string CurrentTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class ExperterSchemeInfoCollection
    {
        public ExperterSchemeInfoCollection()
        {
            List = new List<ExperterQuerySchemeInfo>();
        }
        public List<ExperterQuerySchemeInfo> List { get; set; }
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// 名家排行
    /// </summary>
    [CommunicationObject]
    public class ExperterRankingInfo
    {
        /// <summary>
        /// 名家ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户显示名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 显示名匿名位数
        /// </summary>
        public int HideDisplayNameCount { get; set; }
        /// <summary>
        /// 中奖注数
        /// </summary>
        public int BonusCount { get; set; }
        /// <summary>
        /// 订单注数
        /// </summary>
        public int OrderCount { get; set; }
        /// <summary>
        /// 被关注人数
        /// </summary>
        public int FollowerUserCount { get; set; }
        /// <summary>
        /// 盈利率
        /// </summary>
        public decimal Yingli { get; set; }
        /// <summary>
        /// 命中率
        /// </summary>
        public decimal Mizhong { get; set; }
    }
    [CommunicationObject]
    public class ExperterRankingInfoCollection
    {
        public ExperterRankingInfoCollection()
        {
            List = new List<ExperterRankingInfo>();
        }
        public List<ExperterRankingInfo> List { get; set; }
    }

    /// <summary>
    /// 专家发布历史
    /// </summary>
    [CommunicationObject]
    public class ExperterHistorySchemeInfo
    {
        /// <summary>
        /// 方案ID
        /// </summary>
        public string SchemeId { get; set; }
        /// <summary>
        /// 比赛Id
        /// </summary>
        public string MatchId { get; set; }
        /// <summary>
        /// 当前发布时间
        /// </summary>
        public string CurrentTime { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 中奖金额
        /// </summary>
        public decimal BonusMoney { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class ExperterHistorySchemeInfoCollection
    {
        public ExperterHistorySchemeInfoCollection()
        {
            List = new List<ExperterHistorySchemeInfo>();
        }
        public List<ExperterHistorySchemeInfo> List { get; set; }
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// 名家发布方案
    /// </summary>
    [CommunicationObject]
    public class ExperterPublishSchemeInfo
    {
        /// <summary>
        /// 方案ID
        /// </summary>
        public string SchemeId { get; set; }
        /// <summary>
        /// 名家ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 专家类别
        /// </summary>
        public ExperterType ExperterType { get; set; }
    }

    /// <summary>
    /// 专家支持率对象
    /// </summary>
    [CommunicationObject]
    public class ExperterSchemeSupportInfo
    {
        public virtual int Id { get; set; }
        /// <summary>
        /// 方案ID
        /// </summary>
        public virtual string SchemeId { get; set; }
        /// <summary>
        /// 支持者用户Id
        /// </summary>
        public virtual string SupportUserId { get; set; }
        /// <summary>
        /// 反对者用户Id 
        /// </summary>
        public virtual string AgainstUserId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }


}
