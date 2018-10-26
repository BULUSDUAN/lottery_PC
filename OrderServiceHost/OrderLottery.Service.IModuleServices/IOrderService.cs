using EntityModel.Communication;
using EntityModel.CoreModel;
using EntityModel.Enum;
using EntityModel.RequestModel;
using Kason.Sg.Core.CPlatform.Ioc;
using Kason.Sg.Core.CPlatform.Runtime.Server.Implementation.ServiceDiscovery.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static EntityModel.CoreModel.ReportInfo;

namespace OrderLottery.Service.IModuleServices
{
    [ServiceBundle("api/{Service}")]
   public interface IOrderService:IServiceKey
    {
        [Service(Date = "2018-06-04", Director = "Debug", Name = "中奖查询")]
        Task<BonusOrderInfoCollection> QueryBonusInfoList(QueryBonusInfoListParam Model);

        [Service(Date = "2018-06-05", Director = "Debug", Name = "北京单场查询开奖结果")]
        Task<BJDCMatchResultInfo_Collection> QueryBJDC_MatchResultCollection(string issuseNumber, int pageIndex, int pageSize);

        [Service(Date = "2018-06-05", Director = "Debug", Name = "查询我的资金明细")]
        Task<UserFundDetailCollection> QueryMyFundDetailList(QueryUserFundDetailParam Model);

        [Service(Date = "2018-06-06", Director = "Debug", Name = "查询我的充值记录")]
        Task<FillMoneyQueryInfoCollection> QueryMyFillMoneyList(QueryFillMoneyListParam Model);

        [Service(Date = "2018-06-07", Director = "Debug", Name = "查询我的投注记录")]
        Task<MyBettingOrderInfoCollection> QueryMyBettingOrderList(QueryMyBettingOrderParam Model);

        [Service(Date = "2018-06-07", Director = "Debug", Name = "查询提款记录")]
        Task<Withdraw_QueryInfoCollection> QueryMyWithdrawList(QueryMyWithdrawParam Model);

        [Service(Date = "2018-06-07", Director = "Debug", Name = "查询指定用户创建的合买订单列表")]
        Task<TogetherOrderInfoCollection> QueryCreateTogetherOrderListByUserId(QueryCreateTogetherOrderParam Model);

        [Service(Date = "2018-06-08", Director = "Debug", Name = "查询指定用户参与的合买订单列表")]
        Task<TogetherOrderInfoCollection> QueryJoinTogetherOrderListByUserId(QueryCreateTogetherOrderParam Model);

        [Service(Date = "2018-06-08", Director = "Debug", Name = "从Redis查询出合买订单数据")]
        Task<Sports_TogetherSchemeQueryInfoCollection> QuerySportsTogetherListFromRedis(QuerySportsTogetherListFromRedisParam Model);

        [Service(Date = "2018-06-11", Director = "Debug", Name = "按keyline查询追号列表")]
        Task<BettingOrderInfoCollection> QueryBettingOrderListByChaseKeyLine(string keyLine);

        [Service(Date = "2018-06-11", Director = "Debug", Name = "查询指定订单的投注号码列表")]
        Task<BettingAnteCodeInfoCollection> QueryAnteCodeListBySchemeId(string schemeId );

        [Service(Date = "2018-06-11", Director = "Debug", Name = "查询足彩合买明细")]
        Task<Sports_TogetherSchemeQueryInfo> QuerySportsTogetherDetail(string schemeId);

        [Service(Date = "2018-06-12", Director = "Debug", Name = "查询用户是否已经参与了合买")]
        Task<bool> IsUserJoinSportsTogether(string schemeId, string userId);

        [Service(Date = "2018-06-12", Director = "Debug", Name = "查询投注号码信息")]
        Task<Sports_AnteCodeQueryInfoCollection> QuerySportsOrderAnteCodeList(string schemeId);

        [Service(Date = "2018-06-12", Director = "Debug", Name = "查询奖期")]
        Task<Issuse_QueryInfo> QueryIssuseInfo(string gameCode, string gameType, string issuseNumber);

        [Service(Date = "2018-06-12", Director = "Debug", Name = "查询足彩方案信息")]
        Task<Sports_SchemeQueryInfo> QuerySportsSchemeInfo(string schemeId);
        [Service(Date ="2018-06-12",Director ="Debug",Name = "查询我的定制  或 定制我的")]
        Task<TogetherFollowerRuleQueryInfoCollection> QueryUserFollowRule(QueryUserFollowRuleParam Model);
        [Service(Date = "2018-06-12", Director = "Debug", Name = "查询跟单信息")]
        Task<TogetherFollowerRuleQueryInfo> QueryTogetherFollowerRuleInfo(string createrUserId, string followerUserId, string gameCode, string gameType);
        [Service(Date = "2018-06-12", Director = "Debug", Name = "查询今日宝单")]
        Task<TotalSingleTreasure_Collection> QueryTodayBDFXList(QueryTodayBDFXList Model);
        [Service(Date = "2018-06-13", Director = "Debug", Name = "查询昨日牛人")]
        Task<string> QueryYesterdayNR(DateTime startTime, DateTime endTime, int count);
        [Service(Date = "2018-06-13", Director = "Debug", Name = "查询宝单作者主页")]
        Task<TotalSingleTreasure_Collection> QueryBDFXAutherHomePage(string userId, string strIsBonus, string currentTime, int pageIndex, int pageSize);
        [Service(Date = "2018-06-13", Director = "Debug", Name = "查询关注(关注总数、被关注总数、晒单总数等)")]
        Task<ConcernedInfo> QueryConcernedByUserId(string bdfxUserId, string currUserId, string startTime, string endTime);
        [Service(Date = "2018-06-13", Director = "Debug", Name = "关注")]
        Task<CommonActionResult> BDFXAttention(string currUserId, string bgzUserId);
        [Service(Date = "2018-06-13", Director = "Debug", Name = "取消关注")]
        Task<CommonActionResult> BDFXCancelAttention(string currUserId, string bgzUserId);
        [Service(Date = "2018-06-13", Director = "Debug", Name = "查询高手排行/我的关注")]
        Task<BDFXGSRank_Collection> QueryGSRankList(string startTime, string endTime, string currUserId, string isMyGZ);
        [Service(Date = "2018-06-14", Director = "Debug", Name = "查询宝单详情")]
        Task<BDFXOrderDetailInfo> QueryBDFXOrderDetailBySchemeId(string schemeId);
        [Service(Date = "2018-06-14", Director = "Debug", Name = "查询我的追号订单列表")]
        Task<BettingOrderInfoCollection> QueryMyChaseOrderList(string gameCode, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string userId, int ProgressStatusType);
        [Service(Date = "2018-06-14", Director = "Debug", Name = "查询我的订单列表信息")]
        Task<MyOrderListInfoCollection> QueryMyOrderListInfo(QueryMyOrderListInfoParam Model);
        [Service(Date = "2018-06-14", Director = "Debug", Name = "查询我的订单详细信息")]
        Task<MyOrderListInfo> QueryMyOrderDetailInfo(string schemeId);
        [Service(Date = "2018-06-14", Director = "Debug", Name = "查询最新中奖")]
        Task<List<LotteryNewBonusInfo>> QueryLotteryNewBonusInfoList(int count);
        [Service(Date = "2018-06-15", Director = "Debug", Name = "查询开奖历史")]
        Task<GameWinNumber_InfoCollection> QueryGameWinNumberByDate(DateTime startTime, DateTime endTime, string gameCode, int pageIndex, int pageSize);
        [Service(Date = "2018-06-15", Director = "Debug", Name = "查询开奖历史倒序")]
        Task<GameWinNumber_InfoCollection> QueryGameWinNumberByDateDesc(DateTime startTime, DateTime endTime, string gameCode, int pageIndex, int pageSize);
        [Service(Date = "2018-06-15", Director = "Debug", Name = "查询奖期")]
        Task<List<LotteryIssuse_QueryInfo>> QueryAllGameCurrentIssuse(bool byOfficial);
        [Service(Date = "2018-06-15", Director = "Debug", Name = "查询北京单场信息")]
        Task<BJDCIssuseInfo> QueryBJDCCurrentIssuseInfo();
        [Service(Date = "2018-06-15", Director = "Debug", Name = "传统足球开奖比赛内容")]
        Task<CTZQMatchInfo_Collection> QueryCTZQMatchListByIssuseNumber(string gameType, string issuseNumber);
        [Service(Date ="2018-06-15",Director ="Debug",Name ="查询所有猜中最新开奖号码")]
        Task<GameWinNumber_InfoCollection> QueryAllGameNewWinNumber(string gameString);
        [Service(Date = "2018-08-17", Director = "Debug", Name = "查看合买详情")]
        Task<Sports_TogetherJoinInfoCollection> QuerySportsTogetherJoinList(string schemeId, int pageIndex, int pageSize, string userId);
        [Service(Date = "2018-08-17", Director = "Debug", Name = "查询本人的所有合买记录")]
        Task<List<Sports_TogetherJoinInfo>> QueryMySportsTogetherListBySchemeId(string schemeId, string userId);

        [Service(Date = "2018-7-15", Director = "lidi", Name = "日志")]
        Task<string> ReadSqlTimeLog(string FileName);

        [Service(Date = "2018-9-20", Director = "renjun", Name = "根据订单号查询订单信息")]
        Task<BettingOrderInfo> QueryOrderDetailBySchemeId(string schemeId);

        [Service(Date = "2018-9-20", Director = "renjun", Name = "查询已跟单数")]
        Task<int> QueryProfileFollowedCount(string userId, string gameCode, string gameType);
        [Service(Date = "2018-9-20", Director = "renjun", Name = "查询订单票数据")]
        Task<Sports_TicketQueryInfoCollection> QuerySportsTicketList(string schemeId, int pageIndex, int pageSize);
           [Service(Date = "2018-9-21", Director = "renjun", Name = "查询单式上传全路径名")]
        Task<SingleScheme_AnteCodeQueryInfo> QuerySingleSchemeFullFileName(string schemeId);
        [Service(Date = "2018-9-21", Director = "renjun", Name = "查询宝单奖金提成信息")]
        Task<BDFXCommisionInfo> QueryBDFXCommision(string schemeId);
        [Service(Date = "2018-9-25", Director = "renjun", Name = "查询我的定制跟单规则")]
        Task<TogetherFollowerRuleQueryInfoCollection> QueryUserFollowRule(string gameCode, string gameType, int pageIndex, int pageSize, string UserId);
        [Service(Date = "2018-10-25", Director = "renjun", Name = "查询定制我的跟单规则")]
        Task<TogetherFollowerRuleQueryInfoCollection> QueryUserFollowMeRule(string gameCode, string gameType, int pageIndex, int pageSize, string UserId);
        [Service(Date = "2018-9-26", Director = "renjun", Name = "成功定制跟单记录")]
        Task<TogetherFollowRecordInfoCollection> QuerySucessFolloweRecord(string gameCode, long ruleId, int pageIndex, int pageSize, string UserId);
        [Service(Date = "2018-9-27", Director = "renjun", Name = "查询已停止的期号信息")]
        Task<string> QueryStopIssuseList(string gameCode, string gameType, int length);
        [Service(Date = "2018-9-28", Director = "renjun", Name = "竞彩足球开奖明细")]
        Task<JCZQMatchResult_Collection> QueryJCZQMatchResultByTime(DateTime time);
        [Service(Date = "2018-9-28", Director = "renjun", Name = "竞彩篮球开奖明细")]
        Task<JCLQMatchResult_Collection> QueryJCLQMatchResultByTime(DateTime time);

        [Service(Date = "2018-9-28", Director = "renjun", Name = "北京单场期号")]
        Task<string> QueryBJDCLastIssuseNumber(int count);
        [Service(Date = "2018-9-28", Director = "renjun", Name = "北京单场查询开奖结果")]
        Task<BJDCMatchResultInfo_Collection> QueryBJDC_MatchResultList(string issuseNumber);
        [Service(Date = "2018-9-29", Director = "renjun", Name = "查询文章")]
        Task<ArticleInfo_QueryCollection> QueryNoStaticPathArticleList(int pageIndex, int pageSize);
        [Service(Date = "2018-9-29", Director = "renjun", Name = "文章资讯")]
        Task<CommonActionResult> UpdateArticleStaticPath(string articleId, string staticPath, string preId, string nextId);
        [Service(Date = "2018-10-19", Director = "renjun", Name = "过关统计")]
        Task<SportsOrder_GuoGuanInfoCollection> QueryReportInfoList_GuoGuan(bool isVirtualOrder, SchemeBettingCategory? category, string key, string gameCode, string gameType, string issuseNumber, DateTime startTime, DateTime endTime, int pageIndex, int pageSize);

        [Service(Date = "2018-10-22", Director = "renjun", Name = "查询单式上传方案详情")]
        Task<OrderSingleSchemeCollection> QuerySingSchemeDetail(string schemeId);
    }
}
