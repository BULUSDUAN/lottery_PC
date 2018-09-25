
using EntityModel;
using EntityModel.Communication;
using EntityModel.CoreModel;
using EntityModel.Enum;
using EntityModel.LotteryJsonInfo;
using Kason.Sg.Core.Caching;
using Kason.Sg.Core.CPlatform;
using Kason.Sg.Core.CPlatform.EventBus.Events;
using Kason.Sg.Core.CPlatform.Filters.Implementation;
using Kason.Sg.Core.CPlatform.Ioc;
using Kason.Sg.Core.CPlatform.Routing.Implementation;
using Kason.Sg.Core.CPlatform.Runtime.Client.Address.Resolvers.Implementation.Selectors.Implementation;
using Kason.Sg.Core.CPlatform.Runtime.Server.Implementation.ServiceDiscovery.Attributes;
using Kason.Sg.Core.CPlatform.Support;
using Kason.Sg.Core.CPlatform.Support.Attributes;
using Kason.Sg.Core.ProxyGenerator.Implementation;
using Kason.Sg.Core.System.Intercept;
using Lottery.Service.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Service.IModuleServices
{
    [ServiceBundle("api/{Service}")]
    public interface IDataService: IServiceKey
    {

        [Service(Date = "2018-07-05", Director = "lidi", Name = "获取当前场次数据")]
        Task<Issuse_QueryInfo> QueryCurrentIssuseInfo(string gameCode);
        //GetUserListBy

        [Service(Date = "2018-07-05", Director = "lidi", Name = "查找C_Core_Config表中的配置信息")]
        Task<C_Core_Config> QueryCoreConfigByKey(string key);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "从Redis中查询当前奖期")]
        Task<LotteryIssuse_QueryInfo> QueryNextIssuseListByLocalStopTime(string gameCode);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "前台查询广告信息")]
        Task<SiteMessageBannerInfo_Collection> QuerySitemessageBanngerList_Web(int bannerType, int returnRecord = 10);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "查询文章列表")]
        Task<ArticleInfo_QueryCollection> QueryArticleList(string key, string gameCode, string category, int pageIndex, int pageSize);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "根据编号查询文章信息_后台")]
        Task<ArticleInfo_Query> QueryArticleById_Web(string articleId);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "查询公告")]
        Task<BulletinInfo_Collection> QueryDisplayBulletinCollection(int agent, int pageIndex, int pageSize);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "提交用户意见")]
        Task<CommonActionResult> SubmitUserIdea(UserIdeaInfo_Add userIdea);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "查询活动列表查询")]
        Task<ActivityListInfoCollection> QueryActivInfoList(int pageIndex, int pageSize);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "查询最新期号")]
        Task<Issuse_QueryInfo> QueryCurretNewIssuseInfo(string gameCode, string gameType);

        [Service(Date = "2018-07-05", Director = "Kason", Name = "查询最新期号")]
        Task<Issuse_QueryInfoEX> QueryCurretNewIssuseInfoList();

        [Service(Date = "2018-07-05", Director = "lidi", Name = "根据公告编号查询公告")]
        Task<BulletinInfo_Query> QueryDisplayBulletinDetailById(long bulletinId);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "根据代理商编码查询APP配置")]
        Task<APPConfigInfo> QueryAppConfigByAgentId(string appAgentId);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "根据UrlType查询所有APP嵌套配置")]
        Task<NestedUrlConfig_Collection> QueryNestedUrlConfigListByUrlType(int urlType);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "查询我的站内信")]
        Task<SiteMessageInnerMailListNew_Collection> QueryMyInnerMailList(int pageIndex, int pageSize, string userId);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "阅读站内信")]
        Task<InnerMailInfo_Query> ReadInnerMail(string innerMailId, string userId);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "查询红包使用规则")]
        Task<string> QueryRedBagUseConfig();

        [Service(Date = "2018-07-05", Director = "lidi", Name = "查询文章列表")]
        //[Command(Strategy = StrategyType.Failover, ShuntStrategy = AddressSelectorMode.Random, ExecutionTimeoutInMilliseconds = 60000, BreakerRequestVolumeThreshold = 0, Injection =null, RequestCacheEnabled = false)]
        Task<ArticleInfo_QueryCollection> QueryArticleList_YouHua(string category, string gameCode, int pageIndex, int pageSize);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "查询fxid活动下所有邀请")]
        Task<ShareSpreadCollection> QueryShareSpreadUsers(string agentId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "查询一星单选遗漏")]
        Task<CQSSC_1X_ZS> QueryCQSSCCurrNumberOmission_1XDX(string key, int index);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "查询重庆时时彩当前遗漏_二星直选")]
        Task<CQSSC_2X_ZXZS> QueryCQSSCCurrNumberOmission_2XZX(string key, int index);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "查询重庆时时彩当前遗漏_三星直选")]
        Task<CQSSC_3X_ZXZS> QueryCQSSCCurrNumberOmission_3XZX(string key, int index);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "查询重庆时时彩当前遗漏_二星组选")]
        Task<CQSSC_2X_ZuXZS> QueryCQSSCCurrNumberOmission_2XZuX(string key, int index);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "查询重庆时时彩当前遗漏_组三，组六")]
        Task<CQSSC_3X_ZuXZS> QueryCQSSCCurrNumberOmission_ZX3_ZX6(string key, int index);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "查询重庆时时彩当前遗漏_大小单双")]
        Task<CQSSC_DXDS> QueryCQSSCCurrNumberOmission_DXDS(string key, int index);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "查询重庆时时彩当前遗漏_五星基本走势")]
        Task<CQSSC_5X_JBZS> QueryCQSSCCurrNumberOmission_5XJBZS(string key, int index);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "获取游客token")]
        Task<CommonActionResult> GetGuestToken();

        [Service(Date = "2018-07-05", Director = "lidi", Name = "查询北京单场最新期号")]
        Task<BJDCIssuseInfo> QueryBJDCCurrentIssuseInfo();

        [Service(Date = "2018-07-26", Director = "lidi", Name = "获取银行列表")]
        Task<List<C_Bank_Info>> GetBankList();

        [Service(Date = "2018-07-26", Director = "lidi", Name = "获取当年/当日最大场次数")]
        Task<List<string>> GetMaxIssueByGameCode(string gameCode, string currIssueNumber,int issueCount);

        
        [Service(Date = "2018-07-26", Director = "lidi", Name = "查询已读和未读站内信")]
        Task<SiteMessageInnerMailListNew_Collection> QueryUnReadInnerMailListByReceiver(string userId, int pageIndex, int pageSize, int handleType);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "日志")]
        Task<string> ReadSqlTimeLog(string FileName);

        [Service(Date = "2018-9-7", Director = "lidi", Name = "根据Key获取传统足球场次redis")]
        Task<List<CtzqIssuesWeb>> GetCTZQIssuseList_ByRedis(string Key);

        [Service(Date = "2018-9-7", Director = "lidi", Name = "根据Key获取传统足球比赛redis")]
        Task<List<CTZQ_MatchInfo_WEB>> GetCTZQMatchOddsList_ByRedis(string Key);

        [Service(Date = "2018-9-7", Director = "lidi", Name = "根据Key获取北京单场比赛redis")]
        Task<List<BJDC_MatchInfo_WEB>> GetBJDCMatchOddsLis_ByRedis(string Key);

        [Service(Date = "2018-9-7", Director = "lidi", Name = "根据Key获取竞彩足球比赛redis")]
        Task<List<JCZQ_MatchInfo_WEB>> GetJCZQMatchOddsList_ByRedis(string Key);

        [Service(Date = "2018-9-7", Director = "lidi", Name = "根据Key获取竞彩篮球比赛redis")]
        Task<List<JCLQ_MatchInfo_WEB>> GetJCLQMatchOddsList_ByRedis(string Key);

        [Service(Date = "2018-9-7", Director = "lidi", Name = "根据Key获取开奖结果列表")]
        Task<List<KaiJiang>> GetKaiJiangList_ByRedis();

        [Service(Date = "2018-9-13", Director = "lidi", Name = "获取首页按钮广告列表")]
        Task<List<APP_Advertising>> GetGameInfoIndex();

        [Service(Date = "2018-9-13", Director = "lidi", Name = "获取此期的开奖号")]
        Task<WinNumber_QueryInfo> GetWinNumber(string gameCode, string gameType, string issuseNumber);

        [Service(Date = "2018-9-13", Director = "lidi", Name = "首页快速购买彩种奖期数据")]
        Task<QuickBuyModel> GetQuickBuy_PC(List<string> GameCodeList);

        [Service(Date = "2018-9-21", Director = "lidi", Name = "大奖排行榜")]
        Task<RankReportCollection_BettingProfit_Sport> QueryRankReport_BigBonus_Sport(QueryBonusBase QueryBase);

        [Service(Date = "2018-9-21", Director = "lidi", Name = "发单盈利排行榜 - 竞彩类")]
        Task<RankReportCollection_BettingProfit_Sport> QueryRankReport_BettingProfit_Sport(QueryBonusBase QueryBase);

        [Service(Date = "2018-9-21", Director = "lidi", Name = "跟单盈利排行榜 - 竞彩类")]
        Task<RankReportCollection_BettingProfit_Sport> QueryRankReport_JoinProfit_Sport(QueryBonusBase QueryBase);

        [Service(Date = "2018-9-21", Director = "lidi", Name = "合买人气排行")]
        Task<RankReportCollection_RankInfo_HotTogether> QueryRankInfoList_HotTogether(QueryBonusBase QueryBase);

        [Service(Date = "2018-9-21", Director = "lidi", Name = "成功的战绩排行_竞彩类")]
        Task<RankReportCollection_BettingProfit_Sport> QueryRankInfoList_SuccessOrder_Sport(QueryBonusBase QueryBase);

        [Service(Date = "2018-9-21", Director = "lidi", Name = "跟单排行榜")]
        Task<RankReportCollection_RankInfo_BeFollower> QueryRankInfoList_BeFollowerCount(QueryBonusBase QueryBase);

        [Service(Date = "2018-9-21", Director = "lidi", Name = "累积中奖排行榜 - 竞彩类")]
        Task<RankReportCollection_TotalBonus_Sport> QueryRankReport_TotalBonus_Sport(QueryBonusBase QueryBase);

        [Service(Date = "2018-9-21", Director = "lidi", Name = "查询成功派奖列表")]
        Task<string> QueryPrizedIssuseList(string gameCode, string gameType, int length);

        [Service(Date = "2018-9-21", Director = "lidi", Name = "中奖查询，公共数据")]
        Task<BonusOrderInfoCollection> QueryBonusInfoList(string userId, string gameCode, string gameType, string issuseNumber, string completeData, string key, int pageIndex, int pageSize);

        [Service(Date = "2018-9-25", Director = "lidi", Name = "中奖排行榜_按彩种查")]
        Task<RankReportCollection_TotalBonus_Sport> QueryRankReport_BonusByGameCode_All(DateTime fromDate, DateTime toDate, int topCount, string gameCode);

        [Service(Date = "2018-9-25", Director = "lidi", Name = "查询开奖记录")]
        Task<WinNumber_QueryInfoCollection> QueryWinNumberHistory(string gameCode, DateTime startTime, DateTime endTime, int pageIndex, int pageSize);

        [Service(Date = "2018-9-25", Director = "lidi", Name = "根据条数查询开奖记录")]
        Task<WinNumber_QueryInfoCollection> QueryWinNumberHistoryByCount(string gameCode, int count);
    }
}
