
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
using KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection;
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
        [Service(Date = "2018-9-27", Director = "lidi", Name = "冻结需要充值的金额")]
        Task<CommonActionResult> FreezeGameRecharge(string userId, decimal money, string userDisplayName);

        [Service(Date = "2018-9-27", Director = "lidi", Name = "游戏充值完成后操作")]
        Task<CommonActionResult> EndFreezeGameRecharge(string orderId, bool isSuccess, string providerSerialNo);

        [Service(Date = "2018-9-27", Director = "lidi", Name = "游戏提现前存入交易表")]
        Task<CommonActionResult> AddGameWithdraw(string userId, decimal money, string userDisplayName, string orderId, string providerSerialNo);

        [Service(Date = "2018-9-29", Director = "renjun", Name = "查询开奖号码")]
        Task<GameWinNumber_Info> QueryNewWinNumber(string gameCode);

        [Service(Date = "2018-9-29", Director = "renjun", Name = "查询开奖号码")]
        Task<GameWinNumber_InfoCollection> QueryGameWinNumber(string gameCode, int pageIndex, int pageSize);

        #region 时时彩走势
        [Service(Date = "2018-10-08", Director = "renjun", Name = "基本走势")]
        Task<SSQ_JiBenZouSi_InfoCollection> QueryCache_SSQ_JiBenZouSi_Info(int index);
        [Service(Date = "2018-10-08", Director = "renjun", Name = "大小走势")]
        Task<SSQ_DX_InfoCollection> QueryCache_SSQ_DX_Info(int index);

        [Service(Date = "2018-10-08", Director = "renjun", Name = "除3走势")]
        Task<SSQ_C3_InfoCollection> QueryCache_SSQ_C3_Info(int index);
        [Service(Date = "2018-10-08", Director = "renjun", Name = "奇偶走势")]
        Task<SSQ_JiOu_InfoCollection> QueryCache_SSQ_JiOu_Info(int index);

        [Service(Date = "2018-10-08", Director = "renjun", Name = "跨度SW走势")]
        Task<SSQ_KuaDu_SW_InfoCollection> QueryCache_SSQ_KuaDu_SW_Info(int index);

        [Service(Date = "2018-10-08", Director = "renjun", Name = "和值走势")]
        Task<SSQ_HeZhi_InfoCollection> QueryCache_SSQ_HeZhi_Info(int index);
        [Service(Date = "2018-10-08", Director = "renjun", Name = "质合走势")]
        Task<SSQ_ZhiHe_InfoCollection> QueryCache_SSQ_ZhiHe_Info(int index);        
        [Service(Date = "2018-10-08", Director = "renjun", Name = "跨度1_6走势")]
        Task<SSQ_KuaDu_1_6_InfoCollection> QueryCache_SSQ_KuaDu_1_6_Info(int index);
        #endregion

        #region 大乐透走势
        [Service(Date = "2018-10-08", Director = "renjun", Name = "查询除3走势")]
        Task<DLT_Chu3_InfoCollection> QueryDLT_Chu3_Info(int length);

        [Service(Date = "2018-10-08",  Director = "renjun", Name = "查询大小走势")]
        Task<DLT_DX_InfoCollection> QueryDLT_DX_Info(int length);
        [Service(Date = "2018-10-08", Director = "renjun", Name = "查询和值走势")]
        Task<DLT_HeZhi_InfoCollection> QueryDLT_HeZhi_Info(int length);
        [Service(Date = "2018-10-08", Director = "renjun", Name = "查询基本走势")]
        Task<DLT_JiBenZouSi_InfoCollection> QueryDLT_JiBenZouSi_Info(int length);
        [Service(Date = "2018-10-08", Director = "renjun", Name = "查询奇偶走势")]
        Task<DLT_JiOu_InfoCollection> QueryDLT_JiOu_Info(int length);

        [Service(Date = "2018-10-08", Director = "renjun", Name = "查询12走势列表按时间倒叙")]
        Task<DLT_KuaDu_12_InfoCollection> QueryDLT_KuaDu_12_Info(int length);

        [Service(Date = "2018-10-08", Director = "renjun", Name = "查询23走势列表按时间倒叙")]
        Task<DLT_KuaDu_23_InfoCollection> QueryDLT_KuaDu_23_Info(int length);

        [Service(Date = "2018-10-08", Director = "renjun", Name = "查询34走势列表按时间倒叙")]
        Task<DLT_KuaDu_34_InfoCollection> QueryDLT_KuaDu_34_Info(int length);
        [Service(Date = "2018-10-08", Director = "renjun", Name = "查询45走势列表按时间倒叙")]
        Task<DLT_KuaDu_45_InfoCollection> QueryDLT_KuaDu_45_Info(int length);
        [Service(Date = "2018-10-08", Director = "renjun", Name = "查询首尾走势列表按时间倒叙")]
        Task<DLT_KuaDu_SW_InfoCollection> QueryDLT_KuaDu_SW_Info(int length);
        [Service(Date = "2018-10-08", Director = "renjun", Name = "查询质和走势")]
        Task<DLT_ZhiHe_InfoCollection> QueryDLT_ZhiHe_Info(int length);
        #endregion

        #region 福彩3D走势
        [Service(Date = "2018-10-09", Director = "renjun", Name = "直选走势")]
        Task<FC3D_ZhiXuanZouSi_InfoCollection> QueryCache_FC3D_ZhiXuanZouSi_Info(int index);

        [Service(Date = "2018-10-09", Director = "renjun", Name = "总跨度")]
        Task<FC3D_KuaDu_Z_InfoCollection> QueryCache_FC3D_KuaDu_Z_Info(int index);

        [Service(Date = "2018-10-09", Director = "renjun", Name = "和值走势")]
        Task<FC3D_HZZS_InfoCollection> QueryCache_FC3D_HZZS_Info(int index);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "奇偶号码")]
        Task<FC3D_JOHM_InfoCollection> QueryCache_FC3D_JOHM_Info(int index);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "除3_3")]
        Task<FC3D_Chu33_InfoCollection> QueryCache_FC3D_Chu33_Info(int index);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "大小号码")]
        Task<FC3D_DXHM_InfoCollection> QueryCache_FC3D_DXHM_Info(int index);   
        [Service(Date = "2018-10-09", Director = "renjun", Name = "组选走势")]
        Task<FC3D_ZuXuanZouSi_InfoCollection> QueryCache_FC3D_ZuXuanZouSi_Info(int index);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "除3_1")]
        Task<FC3D_Chu31_InfoCollection> QueryCache_FC3D_Chu31_Info(int index);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "除3_2")]
        Task<FC3D_Chu32_InfoCollection> QueryCache_FC3D_Chu32_Info(int index);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "跨度百位、十位")]
        Task<FC3D_KuaDu_12_InfoCollection> QueryCache_FC3D_KuaDu_12_Info(int index);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "跨度百位、个位")]
        Task<FC3D_KuaDu_13_InfoCollection> QueryCache_FC3D_KuaDu_13_Info(int index);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "跨度十位、个位")]
        Task<FC3D_KuaDu_23_InfoCollection> QueryCache_FC3D_KuaDu_23_Info(int index);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "大小形态走势")]
        Task<FC3D_DXXT_InfoCollection> QueryCache_FC3D_DXXT_Info(int index);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "和值分布")]
        Task<FC3D_HZFB_InfoCollection> QueryCache_FC3D_HZFB_Info(int index);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "和值特征")]
        Task<FC3D_HZTZ_InfoCollection> QueryCache_FC3D_HZTZ_Info(int index);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "奇偶形态")]
        Task<FC3D_JOXT_InfoCollection> QueryCache_FC3D_JOXT_Info(int index);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "质合号码")]
        Task<FC3D_ZHHM_InfoCollection> QueryCache_FC3D_ZHHM_Info(int index);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "质合形态")]
        Task<FC3D_ZHXT_InfoCollection> QueryCache_FC3D_ZHXT_Info(int index);
        #endregion

        #region 排列3走势
        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询基本走势")]
        Task<PL3_JiBenZouSi_InfoCollection> QueryPL3_JiBenZouSi_Info(int length);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询组选走势")]
        Task<PL3_ZuXuanZouSi_InfoCollection> QueryPL3_ZuXuanZouSi_info(int length);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询大小走势")]
        Task<PL3_DX_InfoCollection> QueryPL3_DX_info(int length);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询大小号码走势")]
        Task<PL3_DXHM_InfoCollection> QueryPL3_DXHM_info(int length);

        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询奇偶走势")]
        Task<PL3_JIOU_InfoCollection> QueryPL3_JIOU_info(int length);

        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询奇偶号码走势")]
        Task<PL3_JOHM_InfoCollection> QueryPL3_JOHM_info(int length);

        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询质和走势")]
        Task<PL3_ZhiHe_InfoCollection> QueryPL3_ZhiHe_info(int length);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询质和号码走势")]
        Task<PL3_ZHHM_InfoCollection> QueryPL3_ZHHM_info(int length);

        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询和值走势")]
        Task<PL3_HeiZhi_InfoCollection> QueryPL3_HeiZhi_Info(int length);

        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询跨度百位、十位走势")]
        Task<PL3_KuaDu_12_InfoCollection> QueryPL3_KuaDu_12_Info(int length);

        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询跨度百位、个位走势")]
        Task<PL3_KuaDu_13_InfoCollection> QueryPL3_KuaDu_13_Info(int length);

        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询跨度十位、个位走势")]
        Task<PL3_KuaDu_23_InfoCollection> QueryPL3_KuaDu_23_Info(int length);

        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询除3 走势1走势")]
        Task<PL3_Chu31_InfoCollection> QueryPL3_PL3_Chu31_Info(int length);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询除3 走势2走势")]
        Task<PL3_Chu32_InfoCollection> QueryPL3_PL3_Chu32_Info(int length);

        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询除3 走势3走势")]
        Task<PL3_Chu33_InfoCollection> QueryPL3_PL3_Chu33_Info(int length);

        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询和值特征 走势3走势")]
        Task<PL3_HZTZ_InfoCollection> QueryPL3_PL3_HZTZ_Info(int length);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询和值合尾 走势3走势")]
        Task<PL3_HZHW_InfoCollection> QueryPL3_PL3_HZHW_Info(int length);
        #endregion

        #region 重新时时彩走势
        [Service(Date = "2018-10-10", Director = "renjun", Name = "大小单双")]
        Task<CQSSC_DXDS_InfoCollection> QueryCache_CQSSC_DXDS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "重庆1星走势")]
        Task<CQSSC_1X_ZS_InfoCollection> QueryCache_CQSSC_1X_ZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "重庆2星和值走势")]
        Task<CQSSC_2X_HZZS_InfoCollection> QueryCache_CQSSC_2X_HZZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "重庆2星组选走势")]
        Task<CQSSC_2X_ZuXZS_InfoCollection> QueryCache_CQSSC_2X_ZuXZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "重庆2星直选走势")]
        Task<CQSSC_2X_ZXZS_InfoCollection> QueryCache_CQSSC_2X_ZXZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "除3")]
        Task<CQSSC_3X_C3YS_InfoCollection> QueryCache_CQSSC_3X_C3YS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "大小走势")]
        Task<CQSSC_3X_DXZS_InfoCollection> QueryCache_CQSSC_3X_DXZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "和值走势")]
        Task<CQSSC_3X_HZZS_InfoCollection> QueryCache_CQSSC_3X_HZZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "奇偶走势")]
        Task<CQSSC_3X_JOZS_InfoCollection> QueryCache_CQSSC_3X_JOZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "跨度")]
        Task<CQSSC_3X_KD_InfoCollection> QueryCache_CQSSC_3X_KD_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "质合走势")]
        Task<CQSSC_3X_ZHZS_InfoCollection> QueryCache_CQSSC_3X_ZHZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "重庆3星组选走势")]
        Task<CQSSC_3X_ZuXZS_InfoCollection> QueryCache_CQSSC_3X_ZuXZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "重庆3星直选走势")]
        Task<CQSSC_3X_ZXZS_InfoCollection> QueryCache_CQSSC_3X_ZXZS_Info(int index);

        [Service(Date = "2018-10-10", Director = "renjun", Name = "和值走势")]
        Task<CQSSC_5X_HZZS_InfoCollection> QueryCache_CQSSC_5X_HZZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "重庆5星基本走势")]
        Task<CQSSC_5X_JBZS_InfoCollection> QueryCache_CQSSC_5X_JBZS_Info(int index);
        #endregion
        #region  江西11选5
        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询基本走势")]
        Task<JX11X5_RXJBZS_InfoCollection> QueryJX11X5_RXJBZS_Info(int length);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "任选大小")]
        Task<JX11X5_RXDX_InfoCollection> QueryJX11X5_RXDX_Info(int length);

        [Service(Date = "2018-10-10", Director = "renjun", Name = "任选奇偶")]
        Task<JX11X5_RXJO_InfoCollection> QueryJX11X5_RXJO_Info(int length);

        [Service(Date = "2018-10-10", Director = "renjun", Name = "任选质和")]
        Task<JX11X5_RXZH_InfoCollection> QueryJX11X5_RXZH_Info(int length);

        [Service(Date = "2018-10-10", Director = "renjun", Name = "任选和值")]
        Task<JX11X5_RXHZ_InfoCollection> QueryJX11X5_RXHZ_Info(int length);

        [Service(Date = "2018-10-10", Director = "renjun", Name = "任选除3")]
        Task<JX11X5_Chu3_InfoCollection> QueryJX11X5_Chu3_Info(int length);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "任选第一位")]
        Task<JX11X5_RX1_InfoCollection> QueryJX11X5_RX1_Info(int length);

        [Service(Date = "2018-10-10", Director = "renjun", Name = "任选第二位")]
        Task<JX11X5_RX2_InfoCollection> QueryJX11X5_RX2_Info(int length);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "任选第三位")]
        Task<JX11X5_RX3_InfoCollection> QueryJX11X5_RX3_Info(int length);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "任选第四位")]
        Task<JX11X5_RX4_InfoCollection> QueryJX11X5_RX4_Info(int length);

        [Service(Date = "2018-10-10", Director = "renjun", Name = "任选第五位")]
        Task<JX11X5_RX5_InfoCollection> QueryJX11X5_RX5_Info(int length);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "前三直选")]
        Task<JX11X5_Q3ZS_InfoCollection> QueryJX11X5_Q3ZS_Info(int length);

        [Service(Date = "2018-10-10", Director = "renjun", Name = "前三组选")]
        Task<JX11X5_Q3ZUS_InfoCollection> QueryJX11X5_Q3ZUS_Info(int length);

        [Service(Date = "2018-10-10", Director = "renjun", Name = "前三大小")]
        Task<JX11X5_Q3DX_InfoCollection> QueryJX11X5_Q3DX_Info(int length);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "前三基偶")]
        Task<JX11X5_Q3JO_InfoCollection> QueryJX11X5_Q3JO_Info(int length);

        [Service(Date = "2018-10-10", Director = "renjun", Name = "前三质和")]
        Task<JX11X5_Q3ZH_InfoCollection> QueryJX11X5_Q3ZH_Info(int length);

        [Service(Date = "2018-10-10", Director = "renjun", Name = "前三除3")]
        Task<JX11X5_Q3Chu3_InfoCollection> QueryJX11X5_Q3Chu3_Info(int length);

        [Service(Date = "2018-10-10", Director = "renjun", Name = "前三和值")]
        Task<JX11X5_Q3HZ_InfoCollection> QueryJX11X5_Q3HZ_Info(int length);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "前2直选")]
        Task<JX11X5_Q2ZS_InfoCollection> QueryJX11X5_Q2ZS_Info(int length);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "前2组选")]
        Task<JX11X5_Q2ZUS_InfoCollection> QueryJX11X5_Q2ZUS_Info(int length);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "前2和值")]
        Task<JX11X5_Q2HZ_InfoCollection> QueryJX11X5_Q2HZ_Info(int length);


        #endregion

        #region 山东11选5

        [Service(Date = "2018-10-10", Director = "renjun", Name = "012定位走势")]
        Task<YDJ11_012DWZS_InfoCollection> QueryCache_YDJ11_012DWZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "路比值走势")]
        Task<YDJ11_012LZZS_InfoCollection> QueryCache_YDJ11_012LZZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询2连走势")]
        Task<YDJ11_2LZS_InfoCollection> QueryCache_YDJ11_2LZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询重号走势")]
        Task<YDJ11_CHZS_InfoCollection> QueryCache_YDJ11_CHZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询多连走势")]
        Task<YDJ11_DLZS_InfoCollection> QueryCache_YDJ11_DLZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询隔号走势")]
        Task<YDJ11_GHZS_InfoCollection> QueryCache_YDJ11_GHZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询和值走势")]
        Task<YDJ11_HZZS_InfoCollection> QueryCache_YDJ11_HZZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询基本走势")]
        Task<YDJ11_JBZS_InfoCollection> QueryCache_YDJ11_JBZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询跨度走势")]
        Task<YDJ11_KDZS_InfoCollection> QueryCache_YDJ11_KDZS_Info(int index);


        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询前1基本走势")]
        Task<YDJ11_Q1JBZS_InfoCollection> QueryCache_YDJ11_Q1JBZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询前1形态走势")]
        Task<YDJ11_Q1XTZS_InfoCollection> QueryCache_YDJ11_Q1XTZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询前2基本走势")]
        Task<YDJ11_Q2JBZS_InfoCollection> QueryCache_YDJ11_Q2JBZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询前2形态走势")]
        Task<YDJ11_Q2XTZS_InfoCollection> QueryCache_YDJ11_Q2XTZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询前3基本走势")]
        Task<YDJ11_Q3JBZS_InfoCollection> QueryCache_YDJ11_Q3JBZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询前3形态走势")]
        Task<YDJ11_Q3XTZS_InfoCollection> QueryCache_YDJ11_Q3XTZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询形态走势")]
        Task<YDJ11_XTZS_InfoCollection> QueryCache_YDJ11_XTZS_Info(int index);
        #endregion
    }
}
