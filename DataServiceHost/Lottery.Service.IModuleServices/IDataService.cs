
using EntityModel;
using EntityModel.Communication;
using EntityModel.CoreModel;
using EntityModel.Enum;
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
        ////[Authorization(AuthType = AuthorizationType.JWT)]
        //[Service(Date = "2018-06-05", Director = "lidi", Name = "根据key获取配置")]
        //// [Command(Strategy = StrategyType.Injection, ShuntStrategy = AddressSelectorMode.HashAlgorithm, ExecutionTimeoutInMilliseconds = 1500, BreakerRequestVolumeThreshold = 3, Injection = @"return 1;", RequestCacheEnabled = false)]
        //Task<CoreConfigInfo> QueryCoreConfigByKey(string key);

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
        Task<ArticleInfo_QueryCollection> QueryArticleList(string key, string gameCode, string category, int pageIndex, int pageSize, string userToken);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "根据编号查询文章信息_后台")]
        Task<ArticleInfo_Query> QueryArticleById_Web(string articleId);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "查询公告")]
        Task<BulletinInfo_Collection> QueryDisplayBulletinCollection(int agent, int pageIndex, int pageSize, string userToken);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "提交用户意见")]
        Task<CommonActionResult> SubmitUserIdea(UserIdeaInfo_Add userIdea);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "查询活动列表查询")]
        Task<ActivityListInfoCollection> QueryActivInfoList(int pageIndex, int pageSize);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "查询最新期号")]
        Task<Issuse_QueryInfo> QueryCurretNewIssuseInfo(string gameCode, string gameType);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "根据公告编号查询公告")]
        Task<BulletinInfo_Query> QueryDisplayBulletinDetailById(long bulletinId);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "根据代理商编码查询APP配置")]
        Task<APPConfigInfo> QueryAppConfigByAgentId(string appAgentId);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "根据UrlType查询所有APP嵌套配置")]
        Task<NestedUrlConfig_Collection> QueryNestedUrlConfigListByUrlType(int urlType);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "查询我的站内信")]
        Task<SiteMessageInnerMailListNew_Collection> QueryMyInnerMailList(int pageIndex, int pageSize, string userToken);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "阅读站内信")]
        Task<InnerMailInfo_Query> ReadInnerMail(string innerMailId, string userToken);

        [Service(Date = "2018-07-05", Director = "lidi", Name = "查询红包使用规则")]
        Task<string> QueryRedBagUseConfig();

        [Service(Date = "2018-07-05", Director = "lidi", Name = "查询文章列表")]
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
    }
}
