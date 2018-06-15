using EntityModel.Communication;
using EntityModel.CoreModel;
using EntityModel.Enum;
using EntityModel.RequestModel;
using Kason.Sg.Core.CPlatform.Ioc;
using Kason.Sg.Core.CPlatform.Runtime.Server.Implementation.ServiceDiscovery.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderLottery.Service.IModuleServices
{
    [ServiceBundle("Order/{Service}")]
   public interface IOrderService
    {
        [Service(Date = "2018-06-04", Director = "Debug", Name = "中奖查询")]
        BonusOrderInfoCollection QueryBonusInfoList(QueryBonusInfoListParam Model);

        [Service(Date = "2018-06-05", Director = "Debug", Name = "北京单场查询开奖结果")]
        BJDCMatchResultInfo_Collection QueryBJDC_MatchResultCollection(string issuseNumber, int pageIndex, int pageSize);

        [Service(Date = "2018-06-05", Director = "Debug", Name = "查询我的资金明细")]
        UserFundDetailCollection QueryMyFundDetailList(QueryUserFundDetailParam Model);

        [Service(Date = "2018-06-06", Director = "Debug", Name = "查询我的充值记录")]
        FillMoneyQueryInfoCollection QueryFillMoneyList(QueryFillMoneyListParam Model);

        [Service(Date = "2018-06-07", Director = "Debug", Name = "查询我的投注记录")]
        MyBettingOrderInfoCollection QueryMyBettingOrderList(QueryMyBettingOrderParam Model);

        [Service(Date = "2018-06-07", Director = "Debug", Name = "查询提款记录")]
        Withdraw_QueryInfoCollection QueryMyWithdrawList(QueryMyWithdrawParam Model);

        [Service(Date = "2018-06-07", Director = "Debug", Name = "查询指定用户创建的合买订单列表")]
        TogetherOrderInfoCollection QueryCreateTogetherOrderListByUserId(QueryCreateTogetherOrderParam Model);

        [Service(Date = "2018-06-08", Director = "Debug", Name = "查询指定用户参与的合买订单列表")]
        TogetherOrderInfoCollection QueryJoinTogetherOrderListByUserId(QueryCreateTogetherOrderParam Model);

        [Service(Date = "2018-06-08", Director = "Debug", Name = "从Redis查询出合买订单数据")]
        Sports_TogetherSchemeQueryInfoCollection QuerySportsTogetherListFromRedis(QuerySportsTogetherListFromRedisParam Model);

        [Service(Date = "2018-06-11", Director = "Debug", Name = "按keyline查询追号列表")]
        BettingOrderInfoCollection QueryBettingOrderListByChaseKeyLine(string keyLine, string userToken);

        [Service(Date = "2018-06-11", Director = "Debug", Name = "查询指定订单的投注号码列表")]
        BettingAnteCodeInfoCollection QueryAnteCodeListBySchemeId(string schemeId );

        [Service(Date = "2018-06-11", Director = "Debug", Name = "查询足彩合买明细")]
        Sports_TogetherSchemeQueryInfo QuerySportsTogetherDetail(string schemeId);

        [Service(Date = "2018-06-12", Director = "Debug", Name = "查询用户是否已经参与了合买")]
        bool IsUserJoinSportsTogether(string schemeId, string userToken);

        [Service(Date = "2018-06-12", Director = "Debug", Name = "查询投注号码信息")]
        Sports_AnteCodeQueryInfoCollection QuerySportsOrderAnteCodeList(string schemeId);

        [Service(Date = "2018-06-12", Director = "Debug", Name = "查询奖期")]
        Issuse_QueryInfo QueryIssuseInfo(string gameCode, string gameType, string issuseNumber);

        [Service(Date = "2018-06-12", Director = "Debug", Name = "查询足彩方案信息")]
        Sports_SchemeQueryInfo QuerySportsSchemeInfo(string schemeId);
        [Service(Date ="2018-06-12",Director ="Debug",Name = "查询我的定制  或 定制我的")]
        TogetherFollowerRuleQueryInfoCollection QueryUserFollowRule(QueryUserFollowRuleParam Model);
        [Service(Date = "2018-06-12", Director = "Debug", Name = "查询跟单信息")]
        TogetherFollowerRuleQueryInfo QueryTogetherFollowerRuleInfo(string createrUserId, string followerUserId, string gameCode, string gameType);
        [Service(Date = "2018-06-12", Director = "Debug", Name = "查询今日宝单")]
        TotalSingleTreasure_Collection QueryTodayBDFXList(QueryTodayBDFXList Model);
        [Service(Date = "2018-06-13", Director = "Debug", Name = "查询昨日牛人")]
        string QueryYesterdayNR(DateTime startTime, DateTime endTime, int count);
        [Service(Date = "2018-06-13", Director = "Debug", Name = "查询宝单作者主页")]
        TotalSingleTreasure_Collection QueryBDFXAutherHomePage(string userId, string strIsBonus, string currentTime, int pageIndex, int pageSize);
        [Service(Date = "2018-06-13", Director = "Debug", Name = "查询关注(关注总数、被关注总数、晒单总数等)")]
        ConcernedInfo QueryConcernedByUserId(string bdfxUserId, string currUserId, string startTime, string endTime);
        [Service(Date = "2018-06-13", Director = "Debug", Name = "关注")]
        CommonActionResult BDFXAttention(string currUserId, string bgzUserId);
        [Service(Date = "2018-06-13", Director = "Debug", Name = "取消关注")]
        CommonActionResult BDFXCancelAttention(string currUserId, string bgzUserId);
        [Service(Date = "2018-06-13", Director = "Debug", Name = "查询高手排行/我的关注")]
        BDFXGSRank_Collection QueryGSRankList(string startTime, string endTime, string currUserId, string isMyGZ);
        [Service(Date = "2018-06-14", Director = "Debug", Name = "查询宝单详情")]
        BDFXOrderDetailInfo QueryBDFXOrderDetailBySchemeId(string schemeId);
        [Service(Date = "2018-06-14", Director = "Debug", Name = "查询我的追号订单列表")]
        BettingOrderInfoCollection QueryMyChaseOrderList(string gameCode, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string userToken);
        [Service(Date = "2018-06-14", Director = "Debug", Name = "查询我的订单列表信息")]
        MyOrderListInfoCollection QueryMyOrderListInfo(QueryMyOrderListInfoParam Model);
        [Service(Date = "2018-06-14", Director = "Debug", Name = "查询我的订单详细信息")]
        MyOrderListInfo QueryMyOrderDetailInfo(string schemeId);
        [Service(Date = "2018-06-14", Director = "Debug", Name = "查询最新中奖")]
        List<LotteryNewBonusInfo> QueryLotteryNewBonusInfoList(int count);
        [Service(Date = "2018-06-15", Director = "Debug", Name = "查询开奖历史")]
        GameWinNumber_InfoCollection QueryGameWinNumberByDate(DateTime startTime, DateTime endTime, string gameCode, int pageIndex, int pageSize);
        [Service(Date = "2018-06-15", Director = "Debug", Name = "查询开奖历史倒序")]
        GameWinNumber_InfoCollection QueryGameWinNumberByDateDesc(DateTime startTime, DateTime endTime, string gameCode, int pageIndex, int pageSize);
        [Service(Date = "2018-06-15", Director = "Debug", Name = "查询奖期")]
        List<LotteryIssuse_QueryInfo> QueryAllGameCurrentIssuse(bool byOfficial);
        [Service(Date = "2018-06-15", Director = "Debug", Name = "查询北京单场信息")]
        BJDCIssuseInfo QueryBJDCCurrentIssuseInfo();
        [Service(Date = "2018-06-15", Director = "Debug", Name = "传统足球开奖比赛内容")]
        CTZQMatchInfo_Collection QueryCTZQMatchListByIssuseNumber(string gameType, string issuseNumber, string userToken);
    }
}
