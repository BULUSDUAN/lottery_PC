
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
        Task<CommonActionResult> FreezeGameRecharge(string userId, decimal money, string userDisplayName, int gameType);

        [Service(Date = "2018-9-27", Director = "lidi", Name = "游戏充值完成后操作")]
        Task<CommonActionResult> EndFreezeGameRecharge(string orderId, bool isSuccess, string providerSerialNo);

        [Service(Date = "2018-9-27", Director = "lidi", Name = "游戏提现前存入交易表")]
        Task<CommonActionResult> AddGameWithdraw(string userId, decimal money, string userDisplayName, string orderId, string providerSerialNo);

        //[Service(Date = "2018-9-27", Director = "lidi", Name = "提款完成，修改交易表")]
        //Task<CommonActionResult> EndAddGameWithdraw(string OrderId, bool IsSuccess);
        [Service(Date = "2018-10-25", Director = "lidi", Name = "提款步骤1")]
        Task<CommonActionResult> AddGameWithdraw_Step1(string userId, decimal money, string userDisplayName, int gameType);

        //[Service(Date = "2018-10-25", Director = "lidi", Name = "提款步骤2")]
        //Task<CommonActionResult> AddGameWithdraw_Step2(string userId, string orderId, string providerSerialNo);

        [Service(Date = "2018-10-25", Director = "lidi", Name = "提款步骤3")]
        Task<CommonActionResult> EndAddGameWithdraw(string OrderId, bool IsSuccess, string providerSerialNo);

        //[Service(Date = "2018-10-25", Director = "lidi", Name = "提款步骤3")]
        //Task<CommonActionResult> GameRecharge_Step2(string orderId, string providerSerialNo);

        [Service(Date = "2018-9-29", Director = "renjun", Name = "查询开奖号码")]
        Task<GameWinNumber_Info> QueryNewWinNumber(string gameCode);

        [Service(Date = "2018-9-29", Director = "renjun", Name = "查询开奖号码")]
        Task<GameWinNumber_InfoCollection> QueryGameWinNumber(string gameCode, int pageIndex, int pageSize);

        #region 时时彩走势
        [Service(Date = "2018-10-08", Director = "renjun", Name = "基本走势")]
        Task<List<SSQ_JiBenZouSi_Info>> QueryCache_SSQ_JiBenZouSi_Info(int index);
        [Service(Date = "2018-10-08", Director = "renjun", Name = "大小走势")]
        Task<List<SSQ_DX_Info>> QueryCache_SSQ_DX_Info(int index);

        [Service(Date = "2018-10-08", Director = "renjun", Name = "除3走势")]
        Task<List<SSQ_C3_Info>> QueryCache_SSQ_C3_Info(int index);
        [Service(Date = "2018-10-08", Director = "renjun", Name = "奇偶走势")]
        Task<List<SSQ_JiOu_Info>> QueryCache_SSQ_JiOu_Info(int index);

        [Service(Date = "2018-10-08", Director = "renjun", Name = "跨度SW走势")]
        Task<List<SSQ_KuaDu_SW_Info>> QueryCache_SSQ_KuaDu_SW_Info(int index);

        [Service(Date = "2018-10-08", Director = "renjun", Name = "和值走势")]
        Task<List<SSQ_HeZhi_Info>> QueryCache_SSQ_HeZhi_Info(int index);
        [Service(Date = "2018-10-08", Director = "renjun", Name = "质合走势")]
        Task<List<SSQ_ZhiHe_Info>> QueryCache_SSQ_ZhiHe_Info(int index);        
        [Service(Date = "2018-10-08", Director = "renjun", Name = "跨度1_6走势")]
        Task<List<SSQ_KuaDu_1_6_Info>> QueryCache_SSQ_KuaDu_1_6_Info(int index);
        #endregion

        #region 大乐透走势
        [Service(Date = "2018-10-08", Director = "renjun", Name = "查询除3走势")]
        Task<List<DLT_Chu3_Info>> QueryDLT_Chu3_Info(int length);

        [Service(Date = "2018-10-08",  Director = "renjun", Name = "查询大小走势")]
        Task<List<DLT_DX_Info>> QueryDLT_DX_Info(int length);
        [Service(Date = "2018-10-08", Director = "renjun", Name = "查询和值走势")]
        Task<List<DLT_HeZhi_Info>> QueryDLT_HeZhi_Info(int length);
        [Service(Date = "2018-10-08", Director = "renjun", Name = "查询基本走势")]
        Task<List<DLT_JiBenZouSi_Info>> QueryDLT_JiBenZouSi_Info(int length);
        [Service(Date = "2018-10-08", Director = "renjun", Name = "查询奇偶走势")]
        Task<List<DLT_JiOu_Info>> QueryDLT_JiOu_Info(int length);

        [Service(Date = "2018-10-08", Director = "renjun", Name = "查询12走势列表按时间倒叙")]
        Task<List<DLT_KuaDu_12_Info>> QueryDLT_KuaDu_12_Info(int length);

        [Service(Date = "2018-10-08", Director = "renjun", Name = "查询23走势列表按时间倒叙")]
        Task<List<DLT_KuaDu_23_Info>> QueryDLT_KuaDu_23_Info(int length);

        [Service(Date = "2018-10-08", Director = "renjun", Name = "查询34走势列表按时间倒叙")]
        Task<List<DLT_KuaDu_34_Info>> QueryDLT_KuaDu_34_Info(int length);
        [Service(Date = "2018-10-08", Director = "renjun", Name = "查询45走势列表按时间倒叙")]
        Task<List<DLT_KuaDu_45_Info>> QueryDLT_KuaDu_45_Info(int length);
        [Service(Date = "2018-10-08", Director = "renjun", Name = "查询首尾走势列表按时间倒叙")]
        Task<List<DLT_KuaDu_SW_Info>> QueryDLT_KuaDu_SW_Info(int length);
        [Service(Date = "2018-10-08", Director = "renjun", Name = "查询质和走势")]
        Task<List<DLT_ZhiHe_Info>> QueryDLT_ZhiHe_Info(int length);
        #endregion

        #region 福彩3D走势
        [Service(Date = "2018-10-09", Director = "renjun", Name = "直选走势")]
        Task<List<FC3D_ZhiXuanZouSi_Info>> QueryCache_FC3D_ZhiXuanZouSi_Info(int index);

        [Service(Date = "2018-10-09", Director = "renjun", Name = "总跨度")]
        Task<List<FC3D_KuaDu_Z_Info>> QueryCache_FC3D_KuaDu_Z_Info(int index);

        [Service(Date = "2018-10-09", Director = "renjun", Name = "和值走势")]
        Task<List<FC3D_HZZS_Info>> QueryCache_FC3D_HZZS_Info(int index);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "奇偶号码")]
        Task<List<FC3D_JOHM_Info>> QueryCache_FC3D_JOHM_Info(int index);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "除3_3")]
        Task<List<FC3D_Chu33_Info>> QueryCache_FC3D_Chu33_Info(int index);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "大小号码")]
        Task<List<FC3D_DXHM_Info>> QueryCache_FC3D_DXHM_Info(int index);   
        [Service(Date = "2018-10-09", Director = "renjun", Name = "组选走势")]
        Task<List<FC3D_ZuXuanZouSi_Info>> QueryCache_FC3D_ZuXuanZouSi_Info(int index);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "除3_1")]
        Task<List<FC3D_Chu31_Info>> QueryCache_FC3D_Chu31_Info(int index);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "除3_2")]
        Task<List<FC3D_Chu32_Info>> QueryCache_FC3D_Chu32_Info(int index);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "跨度百位、十位")]
        Task<List<FC3D_KuaDu_12_Info>> QueryCache_FC3D_KuaDu_12_Info(int index);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "跨度百位、个位")]
        Task<List<FC3D_KuaDu_13_Info>> QueryCache_FC3D_KuaDu_13_Info(int index);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "跨度十位、个位")]
        Task<List<FC3D_KuaDu_23_Info>> QueryCache_FC3D_KuaDu_23_Info(int index);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "大小形态走势")]
        Task<List<FC3D_DXXT_Info>> QueryCache_FC3D_DXXT_Info(int index);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "和值分布")]
        Task<List<FC3D_HZFB_Info>> QueryCache_FC3D_HZFB_Info(int index);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "和值特征")]
        Task<List<FC3D_HZTZ_Info>> QueryCache_FC3D_HZTZ_Info(int index);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "奇偶形态")]
        Task<List<FC3D_JOXT_Info>> QueryCache_FC3D_JOXT_Info(int index);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "质合号码")]
        Task<List<FC3D_ZHHM_Info>> QueryCache_FC3D_ZHHM_Info(int index);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "质合形态")]
        Task<List<FC3D_ZHXT_Info>> QueryCache_FC3D_ZHXT_Info(int index);
        #endregion

        #region 排列3走势
        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询基本走势")]
        Task<List<PL3_JiBenZouSi_Info>> QueryPL3_JiBenZouSi_Info(int length);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询组选走势")]
        Task<List<PL3_ZuXuanZouSi_Info>> QueryPL3_ZuXuanZouSi_info(int length);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询大小走势")]
        Task<List<PL3_DX_Info>> QueryPL3_DX_info(int length);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询大小号码走势")]
        Task<List<PL3_DXHM_Info>> QueryPL3_DXHM_info(int length);

        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询奇偶走势")]
        Task<List<PL3_JIOU_Info>> QueryPL3_JIOU_info(int length);

        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询奇偶号码走势")]
        Task<List<PL3_JOHM_Info>> QueryPL3_JOHM_info(int length);

        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询质和走势")]
        Task<List<PL3_ZhiHe_Info>> QueryPL3_ZhiHe_info(int length);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询质和号码走势")]
        Task<List<PL3_ZHHM_Info>> QueryPL3_ZHHM_info(int length);

        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询和值走势")]
        Task<List<PL3_HeiZhi_Info>> QueryPL3_HeiZhi_Info(int length);

        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询跨度百位、十位走势")]
        Task<List<PL3_KuaDu_12_Info>> QueryPL3_KuaDu_12_Info(int length);

        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询跨度百位、个位走势")]
        Task<List<PL3_KuaDu_13_Info>> QueryPL3_KuaDu_13_Info(int length);

        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询跨度十位、个位走势")]
        Task<List<PL3_KuaDu_23_Info>> QueryPL3_KuaDu_23_Info(int length);

        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询除3 走势1走势")]
        Task<List<PL3_Chu31_Info>> QueryPL3_PL3_Chu31_Info(int length);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询除3 走势2走势")]
        Task<List<PL3_Chu32_Info>> QueryPL3_PL3_Chu32_Info(int length);

        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询除3 走势3走势")]
        Task<List<PL3_Chu33_Info>> QueryPL3_PL3_Chu33_Info(int length);

        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询和值特征 走势3走势")]
        Task<List<PL3_HZTZ_Info>> QueryPL3_PL3_HZTZ_Info(int length);
        [Service(Date = "2018-10-09", Director = "renjun", Name = "查询和值合尾 走势3走势")]
        Task<List<PL3_HZHW_Info>> QueryPL3_PL3_HZHW_Info(int length);
        #endregion

        #region 重新时时彩走势
        [Service(Date = "2018-10-10", Director = "renjun", Name = "大小单双")]
        Task<List<CQSSC_DXDS_Info>> QueryCache_CQSSC_DXDS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "重庆1星走势")]
        Task<List<CQSSC_1X_ZS_Info>> QueryCache_CQSSC_1X_ZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "重庆2星和值走势")]
        Task<List<CQSSC_2X_HZZS_Info>> QueryCache_CQSSC_2X_HZZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "重庆2星组选走势")]
        Task<List<CQSSC_2X_ZuXZS_Info>> QueryCache_CQSSC_2X_ZuXZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "重庆2星直选走势")]
        Task<List<CQSSC_2X_ZXZS_Info>> QueryCache_CQSSC_2X_ZXZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "除3")]
        Task<List<CQSSC_3X_C3YS_Info>> QueryCache_CQSSC_3X_C3YS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "大小走势")]
        Task<List<CQSSC_3X_DXZS_Info>> QueryCache_CQSSC_3X_DXZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "和值走势")]
        Task<List<CQSSC_3X_HZZS_Info>> QueryCache_CQSSC_3X_HZZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "奇偶走势")]
        Task<List<CQSSC_3X_JOZS_Info>> QueryCache_CQSSC_3X_JOZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "跨度")]
        Task<List<CQSSC_3X_KD_Info>> QueryCache_CQSSC_3X_KD_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "质合走势")]
        Task<List<CQSSC_3X_ZHZS_Info>> QueryCache_CQSSC_3X_ZHZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "重庆3星组选走势")]
        Task<List<CQSSC_3X_ZuXZS_Info>> QueryCache_CQSSC_3X_ZuXZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "重庆3星直选走势")]
        Task<List<CQSSC_3X_ZXZS_Info>> QueryCache_CQSSC_3X_ZXZS_Info(int index);

        [Service(Date = "2018-10-10", Director = "renjun", Name = "和值走势")]
        Task<List<CQSSC_5X_HZZS_Info>> QueryCache_CQSSC_5X_HZZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "重庆5星基本走势")]
        Task<List<CQSSC_5X_JBZS_Info>> QueryCache_CQSSC_5X_JBZS_Info(int index);
        #endregion

        #region  江西11选5
        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询基本走势")]
        Task<List<JX11X5_RXJBZS_Info>> QueryJX11X5_RXJBZS_Info(int length);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "任选大小")]
        Task<List<JX11X5_RXDX_Info>> QueryJX11X5_RXDX_Info(int length);

        [Service(Date = "2018-10-10", Director = "renjun", Name = "任选奇偶")]
        Task<List<JX11X5_RXJO_Info>> QueryJX11X5_RXJO_Info(int length);

        [Service(Date = "2018-10-10", Director = "renjun", Name = "任选质和")]
        Task<List<JX11X5_RXZH_Info>> QueryJX11X5_RXZH_Info(int length);

        [Service(Date = "2018-10-10", Director = "renjun", Name = "任选和值")]
        Task<List<JX11X5_RXHZ_Info>> QueryJX11X5_RXHZ_Info(int length);

        [Service(Date = "2018-10-10", Director = "renjun", Name = "任选除3")]
        Task<List<JX11X5_Chu3_Info>> QueryJX11X5_Chu3_Info(int length);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "任选第一位")]
        Task<List<JX11X5_RX1_Info>> QueryJX11X5_RX1_Info(int length);

        [Service(Date = "2018-10-10", Director = "renjun", Name = "任选第二位")]
        Task<List<JX11X5_RX2_Info>> QueryJX11X5_RX2_Info(int length);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "任选第三位")]
        Task<List<JX11X5_RX3_Info>> QueryJX11X5_RX3_Info(int length);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "任选第四位")]
        Task<List<JX11X5_RX4_Info>> QueryJX11X5_RX4_Info(int length);

        [Service(Date = "2018-10-10", Director = "renjun", Name = "任选第五位")]
        Task<List<JX11X5_RX5_Info>> QueryJX11X5_RX5_Info(int length);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "前三直选")]
        Task<List<JX11X5_Q3ZS_Info>> QueryJX11X5_Q3ZS_Info(int length);

        [Service(Date = "2018-10-10", Director = "renjun", Name = "前三组选")]
        Task<List<JX11X5_Q3ZUS_Info>> QueryJX11X5_Q3ZUS_Info(int length);

        [Service(Date = "2018-10-10", Director = "renjun", Name = "前三大小")]
        Task<List<JX11X5_Q3DX_Info>> QueryJX11X5_Q3DX_Info(int length);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "前三基偶")]
        Task<List<JX11X5_Q3JO_Info>> QueryJX11X5_Q3JO_Info(int length);

        [Service(Date = "2018-10-10", Director = "renjun", Name = "前三质和")]
        Task<List<JX11X5_Q3ZH_Info>> QueryJX11X5_Q3ZH_Info(int length);

        [Service(Date = "2018-10-10", Director = "renjun", Name = "前三除3")]
        Task<List<JX11X5_Q3Chu3_Info>> QueryJX11X5_Q3Chu3_Info(int length);

        [Service(Date = "2018-10-10", Director = "renjun", Name = "前三和值")]
        Task<List<JX11X5_Q3HZ_Info>> QueryJX11X5_Q3HZ_Info(int length);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "前2直选")]
        Task<List<JX11X5_Q2ZS_Info>> QueryJX11X5_Q2ZS_Info(int length);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "前2组选")]
        Task<List<JX11X5_Q2ZUS_Info>> QueryJX11X5_Q2ZUS_Info(int length);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "前2和值")]
        Task<List<JX11X5_Q2HZ_Info>> QueryJX11X5_Q2HZ_Info(int length);


        #endregion

        #region 山东11选5

        [Service(Date = "2018-10-10", Director = "renjun", Name = "012定位走势")]
        Task<List<YDJ11_012DWZS_Info>> QueryCache_YDJ11_012DWZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "路比值走势")]
        Task<List<YDJ11_012LZZS_Info>> QueryCache_YDJ11_012LZZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询2连走势")]
        Task<List<YDJ11_2LZS_Info>> QueryCache_YDJ11_2LZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询重号走势")]
        Task<List<YDJ11_CHZS_Info>> QueryCache_YDJ11_CHZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询多连走势")]
        Task<List<YDJ11_DLZS_Info>> QueryCache_YDJ11_DLZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询隔号走势")]
        Task<List<YDJ11_GHZS_Info>> QueryCache_YDJ11_GHZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询和值走势")]
        Task<List<YDJ11_HZZS_Info>> QueryCache_YDJ11_HZZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询基本走势")]
        Task<List<YDJ11_JBZS_Info>> QueryCache_YDJ11_JBZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询跨度走势")]
        Task<List<YDJ11_KDZS_Info>> QueryCache_YDJ11_KDZS_Info(int index);


        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询前1基本走势")]
        Task<List<YDJ11_Q1JBZS_Info>> QueryCache_YDJ11_Q1JBZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询前1形态走势")]
        Task<List<YDJ11_Q1XTZS_Info>> QueryCache_YDJ11_Q1XTZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询前2基本走势")]
        Task<List<YDJ11_Q2JBZS_Info>> QueryCache_YDJ11_Q2JBZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询前2形态走势")]
        Task<List<YDJ11_Q2XTZS_Info>> QueryCache_YDJ11_Q2XTZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询前3基本走势")]
        Task<List<YDJ11_Q3JBZS_Info>> QueryCache_YDJ11_Q3JBZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询前3形态走势")]
        Task<List<YDJ11_Q3XTZS_Info>> QueryCache_YDJ11_Q3XTZS_Info(int index);
        [Service(Date = "2018-10-10", Director = "renjun", Name = "查询形态走势")]
        Task<List<YDJ11_XTZS_Info>> QueryCache_YDJ11_XTZS_Info(int index);
        #endregion

        #region 广东11选5

        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询012定位走势")]
        Task<List<GD11X5_012DWZS_Info>> QueryCache_GD11X5_012DWZS_Info(int index);

        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询路比值走势")]
        Task<List<GD11X5_012LZZS_Info>> QueryCache_GD11X5_012LZZS_Info(int index);

        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询2连走势")]
        Task<List<GD11X5_2LZS_Info>> QueryCache_GD11X5_2LZS_Info(int index);

        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询重号走势")]
        Task<List<GD11X5_CHZS_Info>> QueryCache_GD11X5_CHZS_Info(int index);

        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询多连走势")]
        Task<List<GD11X5_DLZS_Info>> QueryCache_GD11X5_DLZS_Info(int index);

        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询隔号走势")]
        Task<List<GD11X5_GHZS_Info>> QueryCache_GD11X5_GHZS_Info(int index);

        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询和值走势")]
        Task<List<GD11X5_HZZS_Info>> QueryCache_GD11X5_HZZS_Info(int index);

        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询基本走势")]
        Task<List<GD11X5_JBZS_Info>> QueryCache_GD11X5_JBZS_Info(int index);

        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询跨度走势")]
        Task<List<GD11X5_KDZS_Info>> QueryCache_GD11X5_KDZS_Info(int index);

        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询前1基本走势")]
        Task<List<GD11X5_Q1JBZS_Info>> QueryCache_GD11X5_Q1JBZS_Info(int index);

        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询前1形态走势")]
        Task<List<GD11X5_Q1XTZS_Info>> QueryCache_GD11X5_Q1XTZS_Info(int index);

        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询前2基本走势")]
        Task<List<GD11X5_Q2JBZS_Info>> QueryCache_GD11X5_Q2JBZS_Info(int index);

        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询前2形态走势")]
        Task<List<GD11X5_Q2XTZS_Info>> QueryCache_GD11X5_Q2XTZS_Info(int index);

        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询前3基本走势")]
        Task<List<GD11X5_Q3JBZS_Info>> QueryCache_GD11X5_Q3JBZS_Info(int index);

        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询前3形态走势")]
        Task<List<GD11X5_Q3XTZS_Info>> QueryCache_GD11X5_Q3XTZS_Info(int index);

        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询形态走势")]
        Task<List<GD11X5_XTZS_Info>> QueryCache_GD11X5_XTZS_Info(int index);
        #endregion

        #region  广东快乐10分
        [Service(Date = "2018-10-11", Director = "renjun", Name = "基本走势")]
        Task<List<GDKLSF_JBZS_Info>> QueryGDKLSF_JBZS(int index);
        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询定位第一位走势")]
        Task<List<GDKLSF_DW1_Info>> QueryGDKLSF_DW1(int index);

        [Service(Date = "2018-10-11", Director = "renjun", Name = "定位第二位走势")]
        Task<List<GDKLSF_DW2_Info>> QueryGDKLSF_DW2(int index);
        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询定位第三位走势")]
        Task<List<GDKLSF_DW3_Info>> QueryGDKLSF_DW3(int index);
        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询大小走势")]
        Task<List<GDKLSF_DX_Info>> QueryGDKLSF_DX(int index);
        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询基偶走势")]
        Task<List<GDKLSF_JO_Info>> QueryGDKLSF_JO(int index);

        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询质和走势")]
        Task<List<GDKLSF_ZH_Info>> QueryGDKLSF_ZH(int index);
        #endregion

        #region 江苏快3走势图
        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询质和走势")]
        Task<List<JSK3_JBZS_Info>> QueryJSK3_JBZS_Info(int length);
        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询和值走势")]
        Task<List<JSK3_HZ_Info>> QueryJSK3_HZ_Info(int length);
        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询江苏快3形态走势")]
        Task<List<JSK3_XT_Info>> QueryJSK3_XT_Info(int length);

        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询江苏快3组合走势")]
        Task<List<JSK3_ZH_Info>> QueryJSK3_ZH_Info(int length);
        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询江苏快3综合走势")]
        Task<List<JSK3_ZHZS_Info>> QueryJSK3_ZHZS_Info(int length);
        #endregion

        #region  山东快乐扑克3走势图
        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询基本走势")]
        Task<SDKLPK3_JBZS_InfoCollection> QueryCache_SDKLPK3_JBZS_Info(int length);
        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询组选走势")]
        Task<SDKLPK3_ZHXZS_InfoCollection> QueryCache_SDKLPK3_ZHXZS_Info(int length);

        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询花色走势")]
        Task<SDKLPK3_HSZS_InfoCollection> QueryCache_SDKLPK3_HSZS_Info(int length);
        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询大小走势")]
        Task<SDKLPK3_DXZS_InfoCollection> QueryCache_SDKLPK3_DXZS_Info(int length);

        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询奇偶走势")]
        Task<SDKLPK3_JOZS_InfoCollection> QueryCache_SDKLPK3_JOZS_Info(int length);
        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询质合走势")]
        Task<SDKLPK3_ZHZS_InfoCollection> QueryCache_SDKLPK3_ZHZS_Info(int length);

        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询除3余走势")]
        Task<SDKLPK3_C3YZS_InfoCollection> QueryCache_SDKLPK3_C3YZS_Info(int length);
        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询和值走势")]
        Task<SDKLPK3_HZZS_InfoCollection> QueryCache_SDKLPK3_HZZS_Info(int length);

        [Service(Date = "2018-10-11", Director = "renjun", Name = "查询类型走势")]
        Task<SDKLPK3_LXZS_InfoCollection> QueryCache_SDKLPK3_LXZS_Info(int length);
        #endregion

        #region 七乐彩走势图
        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询七乐彩基本走势")]
        Task<QLC_JBZS_InfoCollection> QueryQLC_JBZS(int length);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询七乐彩大小走势")]
        Task<QLC_DX_InfoCollection> QueryQLC_DX(int length);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询七乐彩奇偶走势")]
        Task<QLC_JO_InfoCollection> QueryQLC_JO(int length);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询七乐彩质和走势")]
        Task<QLC_ZH_InfoCollection> QueryQLC_ZH(int length);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询七乐彩除3走势")]
        Task<QLC_Chu3_InfoCollection> QueryQLC_Chu3(int length);
        #endregion

        #region 江苏快3
        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询江苏快3基本走势")]
        Task<JLK3_JBZS_InfoCollection> QueryJLK3_JBZS_Info(int length);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询江苏快3和值走势")]
        Task<JLK3_HZ_InfoCollection> QueryJLK3_HZ_Info(int length);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询江苏快3形态走势")]
        Task<JLK3_XT_InfoCollection> QueryJLK3_XT_Info(int length);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询江苏快3组合走势")]
        Task<JLK3_ZH_InfoCollection> QueryJLK3_ZH_Info(int length);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询江苏快3综合走势")]
        Task<JLK3_ZHZS_InfoCollection> QueryJLK3_ZHZS_Info(int length);
        #endregion

        #region 湖北快3
        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询湖北快3基本走势")]
        Task<HBK3_JBZS_InfoCollection> QueryHBK3_JBZS_Info(int length);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询湖北快3和值走势")]
        Task<HBK3_HZ_InfoCollection> QueryHBK3_HZ_Info(int length);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询湖北快3形态走势")]
        Task<HBK3_XT_InfoCollection> QueryHBK3_XT_Info(int length);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询湖北快3组合走势")]
        Task<HBK3_ZH_InfoCollection> QueryHBK3_ZH_Info(int length);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询湖北快3综合走势")]
        Task<HBK3_ZHZS_InfoCollection> QueryHBK3_ZHZS_Info(int length);

        #endregion

        #region 山东群英会走势图
        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询山东群英会奇偶走势")]
        Task<SDQYH_RXJO_InfoCollection> QuerySDQYH_RXJO_Info(int index);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "山东群英会质和走势")]
        Task<SDQYH_RXZH_InfoCollection> QuerySDQYH_RXZH_Info(int index);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询山东群英会大小走势")]
        Task<SDQYH_RXDX_InfoCollection> QuerySDQYH_RXDX_Info(int index);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询山东群英会除3走势")]
        Task<SDQYH_Chu3_InfoCollection> QuerySDQYH_Chu3_Info(int index);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询山东群英会顺选1走势")]
        Task<SDQYH_SX1_InfoCollection> QuerySDQYH_SX1_Info(int index);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询山东群英会除3走势")]
        Task<SDQYH_SX2_InfoCollection> QuerySDQYH_SX2_Info(int index);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询山东群英会顺选1走势")]
        Task<SDQYH_SX3_InfoCollection> QuerySDQYH_SX3_Info(int index);
        #endregion

        #region 好彩1走势图
        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询基本走势")]
        Task<HC1_JBZS_InfoCollection> QueryCache_HC1_JBZS_Info(int index);
        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询生肖季节方位")]
        Task<HC1_SXJJFWZS_InfoCollection> QueryCache_HC1_SXJJFWZS_Info(int index);
        #endregion

        #region 华东15选5走势图
        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询华东15选5和值走势")]
        Task<HD15X5_HZ_InfoCollection> QueryHD15X5_HZ(int length);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询华东15选5基本走势")]
        Task<HD15X5_JBZS_InfoCollection> QueryHD15X5_JBZS(int length);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询华东15选5重号走势")]
        Task<HD15X5_CH_InfoCollection> QueryCache_HD15X5_CH_Info(int index);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询华东15选5大小走势")]
        Task<HD15X5_DX_InfoCollection> QueryHD15X5_DX(int length);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询华东15选5奇偶走势")]
        Task<HD15X5_JO_InfoCollection> QueryHD15X5_JO(int length);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询华东15选5质和走势")]
        Task<HD15X5_ZH_InfoCollection> QueryHD15X5_ZH(int length);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询华东15选5连号走势")]
        Task<HD15X5_LH_InfoCollection> QueryCache_HD15X5_LH_Info(int index);
        #endregion

        #region 东方6+1走势图
        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询大小走势")]
        Task<DF6_1_DXZS_InfoCollection> QueryCache_DF6_1_DXZS_Info(int index);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询和值走势")]
        Task<DF6_1_HZZS_InfoCollection> QueryCache_DF6_1_HZZS_Info(int index);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询基本走势")]
        Task<DF6_1_JBZS_InfoCollection> QueryCache_DF6_1_JBZS_Info(int index);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询奇偶走势")]
        Task<DF6_1_JOZS_InfoCollection> QueryCache_DF6_1_JOZS_Info(int index);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询跨度走势")]
        Task<DF6_1_KDZS_InfoCollection> QueryCache_DF6_1_KDZS_Info(int index);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询质和走势")]
        Task<DF6_1_ZHZS_InfoCollection> QueryCache_DF6_1_ZHZS_Info(int index);

        #endregion

        #region 排列5
        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询排列5基本走势")]
        Task<PL5_JBZS_InfoCollection> QueryPL5_JBZS(int length);


        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询排列5大小走势")]
        Task<PL5_DX_InfoCollection> QueryPL5_DX(int length);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询排列5奇偶走势")]
        Task<PL5_JO_InfoCollection> QueryPL5_JO(int length);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询排列5质和走势")]
        Task<PL5_ZH_InfoCollection> QueryPL5_ZH(int length);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询排列5除3走势")]
        Task<PL5_Chu3_InfoCollection> QueryPL5_Chu3(int length);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询排列5和值走势")]
        Task<PL5_HZ_InfoCollection> QueryPL5_HZ(int length);
        #endregion

        #region 七星彩走势

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询七星彩基本走势")]
        Task<QXC_JBZS_InfoCollection> QueryQXC_JBZS(int length);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询七星彩大小走势")]
        Task<QXC_DX_InfoCollection> QueryQXC_DX(int length);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询七星彩奇偶走势")]
        Task<QXC_JO_InfoCollection> QueryQXC_JO(int length);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询七星彩质和走势")]
        Task<QXC_ZH_InfoCollection> QueryQXC_ZH(int length);

        [Service(Date = "2018-10-12", Director = "renjun", Name = "查询七星彩除3走势")]
        Task<QXC_Chu3_InfoCollection> QueryQXC_Chu3(int length);

        #endregion

        #region 重庆11选5
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询012定位走势")]
        Task<CQ11X5_012DWZS_InfoCollection> QueryCache_CQ11X5_012DWZS_Info(int index);
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询路比值走势")]
        Task<CQ11X5_012LZZS_InfoCollection> QueryCache_CQ11X5_012LZZS_Info(int index);

        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询2连走势")]
        Task<CQ11X5_2LZS_InfoCollection> QueryCache_CQ11X5_2LZS_Info(int index);
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询重号走势")]
        Task<CQ11X5_CHZS_InfoCollection> QueryCache_CQ11X5_CHZS_Info(int index);
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询多连走势")]
        Task<CQ11X5_DLZS_InfoCollection> QueryCache_CQ11X5_DLZS_Info(int index);
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询隔号走势")]
        Task<CQ11X5_GHZS_InfoCollection> QueryCache_CQ11X5_GHZS_Info(int index);

        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询和值走势")]
        Task<CQ11X5_HZZS_InfoCollection> QueryCache_CQ11X5_HZZS_Info(int index);
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询基本走势")]
        Task<CQ11X5_JBZS_InfoCollection> QueryCache_CQ11X5_JBZS_Info(int index);

        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询跨度走势")]
        Task<CQ11X5_KDZS_InfoCollection> QueryCache_CQ11X5_KDZS_Info(int index);
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询前1基本走势")]
        Task<CQ11X5_Q1JBZS_InfoCollection> QueryCache_CQ11X5_Q1JBZS_Info(int index);

        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询前1形态走势")]
        Task<CQ11X5_Q1XTZS_InfoCollection> QueryCache_CQ11X5_Q1XTZS_Info(int index);
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询前2基本走势")]
        Task<CQ11X5_Q2JBZS_InfoCollection> QueryCache_CQ11X5_Q2JBZS_Info(int index);
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询前2形态走势")]
        Task<CQ11X5_Q2XTZS_InfoCollection> QueryCache_CQ11X5_Q2XTZS_Info(int index);
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询前3基本走势")]
        Task<CQ11X5_Q3JBZS_InfoCollection> QueryCache_CQ11X5_Q3JBZS_Info(int index);

        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询前3形态走势")]
        Task<CQ11X5_Q3XTZS_InfoCollection> QueryCache_CQ11X5_Q3XTZS_Info(int index);
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询形态走势")]
        Task<CQ11X5_XTZS_InfoCollection> QueryCache_CQ11X5_XTZS_Info(int index);
        #endregion

        #region 辽宁11选5
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询2连走势")]
        Task<LN11X5_2LZS_InfoCollection> QueryCache_LN11X5_2LZS_Info(int index);
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询重号走势")]
        Task<LN11X5_CHZS_InfoCollection> QueryCache_LN11X5_CHZS_Info(int index);
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询多练走势")]
        Task<LN11X5_DLZS_InfoCollection> QueryCache_LN11X5_DLZS_Info(int index);
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询大小走势")]
        Task<LN11X5_DXZS_InfoCollection> QueryCache_LN11X5_DXZS_Info(int index);
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询隔号走势")]
        Task<LN11X5_GHZS_InfoCollection> QueryCache_LN11X5_GHZS_Info(int index);

        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询和值走势")]
        Task<LN11X5_HZZS_InfoCollection> QueryCache_LN11X5_HZZS_Info(int index);
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询基本走势")]
        Task<LN11X5_JBZS_InfoCollection> QueryCache_LN11X5_JBZS_Info(int index);

        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询奇偶走势")]
        Task<LN11X5_JOZS_InfoCollection> QueryCache_LN11X5_JOZS_Info(int index);
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询前1走势")]
        Task<LN11X5_Q1ZS_InfoCollection> QueryCache_LN11X5_Q1ZS_Info(int index);

        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询前2走势")]
        Task<LN11X5_Q2ZS_InfoCollection> QueryCache_LN11X5_Q2ZS_Info(int index);
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询前3走势")]
        Task<LN11X5_Q3ZS_InfoCollection> QueryCache_LN11X5_Q3ZS_Info(int index);
        #endregion

        #region 江西时时彩走势图
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询江西1星走势")]
        Task<JXSSC_1X_ZS_InfoCollection> QueryCache_JXSSC_1X_ZS_Info(int index);
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询江西2星和值走势")]
        Task<JXSSC_2X_HZZS_InfoCollection> QueryCache_JXSSC_2X_HZZS_Info(int index);
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询江西2星组选走势")]
        Task<JXSSC_2X_ZuXZS_InfoCollection> QueryCache_JXSSC_2X_ZuXZS_Info(int index);
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询江西2星直选走势")]
        Task<JXSSC_2X_ZXZS_InfoCollection> QueryCache_JXSSC_2X_ZXZS_Info(int index);
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询除3")]
        Task<JXSSC_3X_C3YS_InfoCollection> QueryCache_JXSSC_3X_C3YS_Info(int index);

        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询大小走势")]
        Task<JXSSC_3X_DXZS_InfoCollection> QueryCache_JXSSC_3X_DXZS_Info(int index);
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询和值走势")]
        Task<JXSSC_3X_HZZS_InfoCollection> QueryCache_JXSSC_3X_HZZS_Info(int index);

        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询奇偶走势")]
        Task<JXSSC_3X_JOZS_InfoCollection> QueryCache_JXSSC_3X_JOZS_Info(int index);
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询跨度")]
        Task<JXSSC_3X_KD_InfoCollection> QueryCache_JXSSC_3X_KD_Info(int index);

        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询质合走势")]
        Task<JXSSC_3X_ZHZS_InfoCollection> QueryCache_JXSSC_3X_ZHZS_Info(int index);
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询江西3星组选走势")]
        Task<JXSSC_3X_ZuXZS_InfoCollection> QueryCache_JXSSC_3X_ZuXZS_Info(int index);

        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询江西3星直选走势")]
        Task<JXSSC_3X_ZXZS_InfoCollection> QueryCache_JXSSC_3X_ZXZS_Info(int index);
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询和值走势")]
        Task<JXSSC_5X_HZZS_InfoCollection> QueryCache_JXSSC_5X_HZZS_Info(int index);
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询江西5星基本走势")]
        Task<JXSSC_5X_JBZS_InfoCollection> QueryCache_JXSSC_5X_JBZS_Info(int index);
        [Service(Date = "2018-10-16", Director = "renjun", Name = "查询大小单双")]
        Task<JXSSC_DXDS_InfoCollection> QueryCache_JXSSC_DXDS_Info(int index);

        #endregion

        #region 快速购买
        [Service(Date = "2018-11-16", Director = "lili", Name = "快速购买")]
        Task<LotteryIssuse_QueryInfo> QueryCurrentIssuseByOfficialStopTime(string gameCode);
        #endregion

        #region 获取合买名人列表
        [Service(Date = "2018-11-16", Director = "lili", Name = "获取合买名人列表")]
        Task<List<TogetherHotUserInfo>> QueryHotTogetherUserListFromRedis(); 
        #endregion
    }
}
