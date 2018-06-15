using EntityModel.CoreModel;
using EntityModel.RequestModel;
using Kason.Sg.Core.CPlatform.Ioc;
using KaSon.FrameWork.Helper;
using Lottery.Kg.ORM.Helper;
using OrderLottery.Service.IModuleServices;
using OrderLottery.Service.ModuleBaseServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel;
using EntityModel.Enum;
using Lottery.Kg.ORM.Helper.OrderQuery;
using EntityModel.Communication;

namespace OrderLottery.Service.ModuleServices
{
    [ModuleName("Order")]
    public class OrderService : DBbase, IOrderService
    {
        IKgLog log = null;
        readonly OrderQuery _order = null;
        public OrderService()
        {
            _order = new OrderQuery();
            log = new Log4Log();
        }

        /// <summary>
        /// 中奖查询
        /// </summary>
        /// <param name="Model">请求实体</param>
        /// <returns></returns>
        public BonusOrderInfoCollection QueryBonusInfoList(QueryBonusInfoListParam Model)
        {
            return _order.QueryBonusInfoList(Model);
        }

        /// <summary>
        /// 北京单场查询开奖结果
        /// </summary>
        /// <param name="issuseNumber">期号</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页数</param>
        /// <returns></returns>
        public BJDCMatchResultInfo_Collection QueryBJDC_MatchResultCollection(string issuseNumber, int pageIndex, int pageSize)
        {
            return _order.QueryBJDC_MatchResultCollection(issuseNumber, pageIndex, pageSize);
        }

        /// <summary>
        /// 查询我的资金明细
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public UserFundDetailCollection QueryMyFundDetailList(QueryUserFundDetailParam Model)
        {
            return _order.QueryMyFundDetailList(Model);
        }
        /// <summary>
        /// 查询我的资金明细
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserFundDetailCollection QueryUserFundDetail(QueryUserFundDetailParam Model, string userId)
        {
            return _order.QueryUserFundDetail(Model, userId);
        }
        /// <summary>
        /// 查询我的充值提现
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public FillMoneyQueryInfoCollection QueryFillMoneyList(QueryFillMoneyListParam Model)
        {
            return _order.QueryFillMoneyList(Model);
        }
        /// <summary>
        /// 查询我的投注记录
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public MyBettingOrderInfoCollection QueryMyBettingOrderList(QueryMyBettingOrderParam Model)
        {
            return _order.QueryMyBettingOrderList(Model);
        }
        /// <summary>
        /// 查询提现记录
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public Withdraw_QueryInfoCollection QueryMyWithdrawList(QueryMyWithdrawParam Model)
        {
            return _order.QueryMyWithdrawList(Model);
        }
        /// <summary>
        /// 查询指定用户创建的合买订单列表
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public TogetherOrderInfoCollection QueryCreateTogetherOrderListByUserId(QueryCreateTogetherOrderParam Model)
        {
            return _order.QueryCreateTogetherOrderListByUserId(Model);
        }
        /// <summary>
        /// 查询指定用户参与的合买订单
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public TogetherOrderInfoCollection QueryJoinTogetherOrderListByUserId(QueryCreateTogetherOrderParam Model)
        {
            return _order.QueryJoinTogetherOrderListByUserId(Model);
        }
        /// <summary>
        /// 从Redis查询出合买订单数据
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public Sports_TogetherSchemeQueryInfoCollection QuerySportsTogetherListFromRedis(QuerySportsTogetherListFromRedisParam Model)
        {
            return _order.QuerySportsTogetherListFromRedis(Model);
        }
        /// <summary>
        /// 按keyline查询追号列表
        /// </summary>
        /// <param name="keyLine"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public BettingOrderInfoCollection QueryBettingOrderListByChaseKeyLine(string keyLine, string userToken)
        {
            return _order.QueryBettingOrderListByChaseKeyLine(keyLine, userToken);
        }
        /// <summary>
        /// 查询指定订单的投注号码列表
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public BettingAnteCodeInfoCollection QueryAnteCodeListBySchemeId(string schemeId)
        {
            return _order.QueryAnteCodeListBySchemeId(schemeId);
        }
        /// <summary>
        /// 查询足彩合买明细
        /// </summary>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public Sports_TogetherSchemeQueryInfo QuerySportsTogetherDetail(string schemeId)
        {
            return _order.QuerySportsTogetherDetail(schemeId);
        }
        /// <summary>
        /// 用户是否已经参与了合买
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public bool IsUserJoinSportsTogether(string schemeId, string userToken)
        {
            return _order.IsUserJoinSportsTogether(schemeId, userToken);
        }
        public Sports_AnteCodeQueryInfoCollection QuerySportsOrderAnteCodeList(string schemeId)
        {
            return _order.QuerySportsOrderAnteCodeList(schemeId);
        }
        public Issuse_QueryInfo QueryIssuseInfo(string gameCode, string gameType, string issuseNumber)
        {
            return _order.QueryIssuseInfo(gameCode, gameType, issuseNumber);
        }
        public Sports_SchemeQueryInfo QuerySportsSchemeInfo(string schemeId)
        {
            return _order.QuerySportsSchemeInfo(schemeId);
        }
        /// <summary>
        /// 查询我的定制  或 定制我的
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public TogetherFollowerRuleQueryInfoCollection QueryUserFollowRule(QueryUserFollowRuleParam Model)
        {
            return _order.QueryUserFollowRule(Model);
        }
        /// <summary>
        ///  查询跟单信息
        /// </summary>
        /// <param name="createrUserId"></param>
        /// <param name="followerUserId"></param>
        /// <param name="gameCode"></param>
        /// <param name="gameType"></param>
        /// <returns></returns>
        public TogetherFollowerRuleQueryInfo QueryTogetherFollowerRuleInfo(string createrUserId, string followerUserId, string gameCode, string gameType)
        {
            return _order.QueryTogetherFollowerRuleInfo(createrUserId, followerUserId, gameCode, gameType);
        }
        /// <summary>
        /// 查询今日宝单
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public TotalSingleTreasure_Collection QueryTodayBDFXList(QueryTodayBDFXList Model)
        {
            return _order.QueryTodayBDFXList(Model);
        }
        /// <summary>
        /// 查询昨日牛人
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public string QueryYesterdayNR(DateTime startTime, DateTime endTime, int count)
        {
            return _order.QueryYesterdayNR(startTime, endTime, count);
        }
        /// <summary>
        /// 查询宝单作者主页
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="strIsBonus"></param>
        /// <param name="currentTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public TotalSingleTreasure_Collection QueryBDFXAutherHomePage(string userId, string strIsBonus, string currentTime, int pageIndex, int pageSize)
        {
            return _order.QueryBDFXAutherHomePage(userId,strIsBonus,currentTime,pageIndex,pageSize);
        }
        /// <summary>
        /// 查询关注(关注总数、被关注总数、晒单总数等)
        /// </summary>
        /// <param name="bdfxUserId"></param>
        /// <param name="currUserId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ConcernedInfo QueryConcernedByUserId(string bdfxUserId, string currUserId, string startTime, string endTime)
        {
            return _order.QueryConcernedByUserId(bdfxUserId, currUserId, startTime, endTime);
        }
        /// <summary>
        /// 关注
        /// </summary>
        /// <param name="currUserId"></param>
        /// <param name="bgzUserId"></param>
        public CommonActionResult BDFXAttention(string currUserId, string bgzUserId)
        {
          return  _order.BDFXAttention(currUserId,bgzUserId);
        }
        /// <summary>
        /// 取消关注
        /// </summary>
        /// <param name="currUserId"></param>
        /// <param name="bgzUserId"></param>
        public CommonActionResult BDFXCancelAttention(string currUserId, string bgzUserId)
        {
          return  _order.BDFXCancelAttention(currUserId, bgzUserId);
        }
        /// <summary>
        /// 查询高手排行/我的关注
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="currUserId"></param>
        /// <param name="isMyGZ"></param>
        /// <returns></returns>
        public BDFXGSRank_Collection QueryGSRankList(string startTime, string endTime, string currUserId, string isMyGZ)
        {
            return _order.QueryGSRankList(startTime, endTime, currUserId, isMyGZ);
        }
        public BDFXOrderDetailInfo QueryBDFXOrderDetailBySchemeId(string schemeId)
        {
            return _order.QueryBDFXOrderDetailBySchemeId(schemeId);
        }
        public BettingOrderInfoCollection QueryMyChaseOrderList(string gameCode, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string userToken)
        {
            return _order.QueryMyChaseOrderList(gameCode, startTime, endTime, pageIndex, pageSize, userToken);
        }
        public MyOrderListInfoCollection QueryMyOrderListInfo(QueryMyOrderListInfoParam Model)
        {
            return _order.QueryMyOrderListInfo(Model);
        }
        public MyOrderListInfo QueryMyOrderDetailInfo(string schemeId)
        {
            return _order.QueryMyOrderDetailInfo(schemeId);
        }
        public List<LotteryNewBonusInfo> QueryLotteryNewBonusInfoList(int count)
        {
            return _order.QueryLotteryNewBonusInfoList(count);
        }
        public GameWinNumber_InfoCollection QueryGameWinNumberByDate(DateTime startTime, DateTime endTime, string gameCode, int pageIndex, int pageSize)
        {
            return _order.QueryGameWinNumberByDate(startTime, endTime, gameCode, pageIndex, pageSize);
        }
        public GameWinNumber_InfoCollection QueryGameWinNumberByDateDesc(DateTime startTime, DateTime endTime, string gameCode, int pageIndex, int pageSize)
        {
            return _order.QueryGameWinNumberByDateDesc(startTime, endTime, gameCode, pageIndex, pageSize);
        }
        public List<LotteryIssuse_QueryInfo> QueryAllGameCurrentIssuse(bool byOfficial)
        {
            return _order.QueryAllGameCurrentIssuse(byOfficial);
        }
        public BJDCIssuseInfo QueryBJDCCurrentIssuseInfo()
        {
            return _order.QueryBJDCCurrentIssuseInfo();
        }
        public CTZQMatchInfo_Collection QueryCTZQMatchListByIssuseNumber(string gameType, string issuseNumber, string userToken)
        {
            return _order.QueryCTZQMatchListByIssuseNumber(gameType, issuseNumber, userToken);
        }
    }
}
