﻿using EntityModel.CoreModel;
using EntityModel.RequestModel;
using Kason.Sg.Core.CPlatform.Ioc;
using OrderLottery.Service.IModuleServices;
using OrderLottery.Service.ModuleBaseServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel;
using EntityModel.Enum;
using EntityModel.Communication;
using System.Threading.Tasks;
using KaSon.FrameWork.ORM.Helper;
using KaSon.FrameWork.Common;
using EntityModel.ExceptionExtend;
using static EntityModel.CoreModel.ReportInfo;

namespace OrderLottery.Service.ModuleServices
{
    [ModuleName("Order")]
    public class OrderService :KgBaseService, IOrderService
    {
        IKgLog log = null;
        private OrderQuery _order = null;
        public OrderService()
        {
            _order = new OrderQuery();
        }

        /// <summary>
        /// 中奖查询
        /// </summary>
        /// <param name="Model">请求实体</param>
        /// <returns></returns>
        public Task<BonusOrderInfoCollection> QueryBonusInfoList(QueryBonusInfoListParam Model)
        {
            return Task.FromResult(_order.QueryBonusInfoList(Model));
        }

        /// <summary>
        /// 北京单场查询开奖结果
        /// </summary>
        /// <param name="issuseNumber">期号</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页数</param>
        /// <returns></returns>
        public Task<BJDCMatchResultInfo_Collection> QueryBJDC_MatchResultCollection(string issuseNumber, int pageIndex, int pageSize)
        {
            return Task.FromResult(_order.QueryBJDC_MatchResultCollection(issuseNumber, pageIndex, pageSize));
        }

        /// <summary>
        /// 查询我的资金明细
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public Task<UserFundDetailCollection> QueryMyFundDetailList(QueryUserFundDetailParam Model)
        {
            return Task.FromResult(_order.QueryMyFundDetailList(Model));
        }
        /// <summary>
        /// 查询我的资金明细
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<UserFundDetailCollection> QueryUserFundDetail(QueryUserFundDetailParam Model, string userId)
        {
            return Task.FromResult(_order.QueryUserFundDetail(Model, userId));
        }
        /// <summary>
        /// 查询我的充值提现
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public Task<FillMoneyQueryInfoCollection> QueryMyFillMoneyList(QueryFillMoneyListParam Model)
        {
            return Task.FromResult(_order.QueryMyFillMoneyList(Model));
        }
        /// <summary>
        /// 查询我的投注记录
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public Task<MyBettingOrderInfoCollection> QueryMyBettingOrderList(QueryMyBettingOrderParam Model)
        {
            return Task.FromResult(_order.QueryMyBettingOrderList(Model));
        }
        /// <summary>
        /// 查询提现记录
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public Task<Withdraw_QueryInfoCollection> QueryMyWithdrawList(QueryMyWithdrawParam Model)
        {
            return Task.FromResult(_order.QueryMyWithdrawList(Model));
        }
        /// <summary>
        /// 查询指定用户创建的合买订单列表
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public Task<TogetherOrderInfoCollection> QueryCreateTogetherOrderListByUserId(QueryCreateTogetherOrderParam Model)
        {
            return Task.FromResult(_order.QueryCreateTogetherOrderListByUserId(Model));
        }
        /// <summary>
        /// 查询指定用户参与的合买订单
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public Task<TogetherOrderInfoCollection> QueryJoinTogetherOrderListByUserId(QueryCreateTogetherOrderParam Model)
        {
            return Task.FromResult(_order.QueryJoinTogetherOrderListByUserId(Model));
        }
        /// <summary>
        /// 从Redis查询出合买订单数据
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public Task<Sports_TogetherSchemeQueryInfoCollection> QuerySportsTogetherListFromRedis(QuerySportsTogetherListFromRedisParam Model)
        {
            return Task.FromResult(_order.QuerySportsTogetherListFromRedis(Model));
        }
        /// <summary>
        /// 按keyline查询追号列表
        /// </summary>
        /// <param name="keyLine"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Task<BettingOrderInfoCollection> QueryBettingOrderListByChaseKeyLine(string keyLine)
        {
            return Task.FromResult(_order.QueryBettingOrderListByChaseKeyLine(keyLine));
        }
        /// <summary>
        /// 查询指定订单的投注号码列表
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Task<BettingAnteCodeInfoCollection> QueryAnteCodeListBySchemeId(string schemeId)
        {
            return Task.FromResult(_order.QueryAnteCodeListBySchemeId(schemeId));
        }
        /// <summary>
        /// 查询足彩合买明细
        /// </summary>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public Task<Sports_TogetherSchemeQueryInfo> QuerySportsTogetherDetail(string schemeId)
        {
            try
            {
                return Task.FromResult(_order.QuerySportsTogetherDetail(schemeId));
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception("查询失败 - " + ex.Message, ex);
            }

        }
        /// <summary>
        /// 用户是否已经参与了合买
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Task<bool> IsUserJoinSportsTogether(string schemeId, string userId)
        {
            return Task.FromResult(_order.IsUserJoinSportsTogether(schemeId, userId));
        }
        public Task<Sports_AnteCodeQueryInfoCollection> QuerySportsOrderAnteCodeList(string schemeId)
        {
            return Task.FromResult(_order.QuerySportsOrderAnteCodeList(schemeId));
        }
        public Task<Issuse_QueryInfo> QueryIssuseInfo(string gameCode, string gameType, string issuseNumber)
        {
            return Task.FromResult(_order.QueryIssuseInfo(gameCode, gameType, issuseNumber));
        }
        public Task<Sports_SchemeQueryInfo> QuerySportsSchemeInfo(string schemeId)
        {
            try
            {
                return Task.FromResult(_order.QuerySportsSchemeInfo(schemeId));
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception("查询出错 - " + ex.Message, ex);
            }

        }
        /// <summary>
        /// 查询我的定制  或 定制我的
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public Task<TogetherFollowerRuleQueryInfoCollection> QueryUserFollowRule(QueryUserFollowRuleParam Model)
        {
            return Task.FromResult(_order.QueryUserFollowRule(Model));
        }

    
        /// <summary>
        ///  查询跟单信息
        /// </summary>
        /// <param name="createrUserId"></param>
        /// <param name="followerUserId"></param>
        /// <param name="gameCode"></param>
        /// <param name="gameType"></param>
        /// <returns></returns>
        public Task<TogetherFollowerRuleQueryInfo> QueryTogetherFollowerRuleInfo(string createrUserId, string followerUserId, string gameCode, string gameType)
        {
            return Task.FromResult(_order.QueryTogetherFollowerRuleInfo(createrUserId, followerUserId, gameCode, gameType));
        }
        /// <summary>
        /// 查询今日宝单
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public Task<TotalSingleTreasure_Collection> QueryTodayBDFXList(QueryTodayBDFXList Model)
        {
            return Task.FromResult(_order.QueryTodayBDFXList(Model));
        }
        /// <summary>
        /// 查询昨日牛人
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public Task<string> QueryYesterdayNR(DateTime startTime, DateTime endTime, int count)
        {
            return Task.FromResult(_order.QueryYesterdayNR(startTime, endTime, count));
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
        public Task<TotalSingleTreasure_Collection> QueryBDFXAutherHomePage(string userId, string strIsBonus, string currentTime, int pageIndex, int pageSize)
        {
            return Task.FromResult(_order.QueryBDFXAutherHomePage(userId,strIsBonus,currentTime,pageIndex,pageSize));
        }
        /// <summary>
        /// 查询关注(关注总数、被关注总数、晒单总数等)
        /// </summary>
        /// <param name="bdfxUserId"></param>
        /// <param name="currUserId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public Task<ConcernedInfo> QueryConcernedByUserId(string bdfxUserId, string currUserId, string startTime, string endTime)
        {
            return Task.FromResult(_order.QueryConcernedByUserId(bdfxUserId, currUserId, startTime, endTime));
        }
        /// <summary>
        /// 关注
        /// </summary>
        /// <param name="currUserId"></param>
        /// <param name="bgzUserId"></param>
        public Task<CommonActionResult> BDFXAttention(string currUserId, string bgzUserId)
        {
            try
            {
                return Task.FromResult(_order.BDFXAttention(currUserId, bgzUserId));
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception("查询出错 - " + ex.Message, ex);
            }
         
        }
        /// <summary>
        /// 取消关注
        /// </summary>
        /// <param name="currUserId"></param>
        /// <param name="bgzUserId"></param>
        public Task<CommonActionResult> BDFXCancelAttention(string currUserId, string bgzUserId)
        {
            try
            {
                return Task.FromResult(_order.BDFXCancelAttention(currUserId, bgzUserId));
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception("系统异常 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询高手排行/我的关注
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="currUserId"></param>
        /// <param name="isMyGZ"></param>
        /// <returns></returns>
        public Task<BDFXGSRank_Collection> QueryGSRankList(string startTime, string endTime, string currUserId, string isMyGZ)
        {
            return Task.FromResult(_order.QueryGSRankList(startTime, endTime, currUserId, isMyGZ));
        }
        public Task<BDFXOrderDetailInfo> QueryBDFXOrderDetailBySchemeId(string schemeId)
        {
            return Task.FromResult(_order.QueryBDFXOrderDetailBySchemeId(schemeId));
        }
        public Task<BettingOrderInfoCollection> QueryMyChaseOrderList(string gameCode, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string userId, int ProgressStatusType)
        {
            return Task.FromResult(_order.QueryMyChaseOrderList(gameCode, startTime, endTime, pageIndex, pageSize, userId, ProgressStatusType));
        }
        public Task<MyOrderListInfoCollection> QueryMyOrderListInfo(QueryMyOrderListInfoParam Model)
        {
            return Task.FromResult(_order.QueryMyOrderListInfo(Model));
        }
        public Task<MyOrderListInfo> QueryMyOrderDetailInfo(string schemeId)
        {
            return Task.FromResult(_order.QueryMyOrderDetailInfo(schemeId));
        }
        public Task<List<LotteryNewBonusInfo>> QueryLotteryNewBonusInfoList(int count)
        {
            return Task.FromResult(_order.QueryLotteryNewBonusInfoList(count));
        }
        public Task<GameWinNumber_InfoCollection> QueryGameWinNumberByDate(DateTime startTime, DateTime endTime, string gameCode, int pageIndex, int pageSize)
        {
            try
            {
                return Task.FromResult(_order.QueryGameWinNumberByDate(startTime, endTime, gameCode, pageIndex, pageSize));
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception("系统异常 - " + ex.Message, ex);
            }

        }
        public Task<GameWinNumber_InfoCollection> QueryGameWinNumberByDateDesc(DateTime startTime, DateTime endTime, string gameCode, int pageIndex, int pageSize)
        {
            try
            {
                return Task.FromResult(_order.QueryGameWinNumberByDateDesc(startTime, endTime, gameCode, pageIndex, pageSize));
            }
            catch (LogicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception("系统异常 - " + ex.Message, ex);
            }
        }
        public Task<List<LotteryIssuse_QueryInfo>> QueryAllGameCurrentIssuse(bool byOfficial)
        {
            return Task.FromResult(_order.QueryAllGameCurrentIssuse(byOfficial));
        }
        public Task<BJDCIssuseInfo> QueryBJDCCurrentIssuseInfo()
        {
            return Task.FromResult(_order.QueryBJDCCurrentIssuseInfo());
        }
        public Task<CTZQMatchInfo_Collection> QueryCTZQMatchListByIssuseNumber(string gameType, string issuseNumber)
        {
            return Task.FromResult(_order.QueryCTZQMatchListByIssuseNumber(gameType, issuseNumber));
        }
        public Task<GameWinNumber_InfoCollection> QueryAllGameNewWinNumber(string gameString)
        {
            return Task.FromResult(_order.QueryAllGameNewWinNumber(gameString));
        }

        public Task<Sports_TogetherJoinInfoCollection> QuerySportsTogetherJoinList(string schemeId, int pageIndex, int pageSize, string userId)
        {
            return Task.FromResult(_order.QuerySportsTogetherJoinList(schemeId, pageIndex, pageSize, userId));
        }

        public Task<List<Sports_TogetherJoinInfo>> QueryMySportsTogetherListBySchemeId(string schemeId,string userId)
        {
            return Task.FromResult(_order.QueryMySportsTogetherListBySchemeId(schemeId, userId));
        }

        public Task<string> ReadSqlTimeLog(string FileName)
        {
            return Task.FromResult(KaSon.FrameWork.Common.Utilities.FileHelper.GetLogInfo("Log_Log\\SQLInfo", "LogTime_"));
        }

        /// <summary>
        /// 根据订单号查询，订单信息
        /// </summary>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public Task<BettingOrderInfo> QueryOrderDetailBySchemeId(string schemeId)
        {
            try
            {
                return Task.FromResult(new SqlQueryBusiness().QueryOrderDetailBySchemeId(schemeId));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #region 定制跟单

        /// <summary>
        /// 查询已跟单数
        /// </summary>
        public Task<int> QueryProfileFollowedCount(string userId, string gameCode, string gameType)
        {
            var biz = new Sports_Manager();
            return Task.FromResult(biz.QueryTogetherFollowerRecord(userId, gameCode, gameType));
        }

        /// <summary>
        /// 查询 定制我的 跟单规则
        /// </summary>
        public Task<TogetherFollowerRuleQueryInfoCollection> QueryUserFollowRule(string gameCode, string gameType, int pageIndex, int pageSize, string UserId)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return Task.FromResult(new Sports_Business().QueryUserFollowRule(false, UserId, gameCode, gameType, pageIndex, pageSize));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public Task<TogetherFollowRecordInfoCollection> QuerySucessFolloweRecord(string gameCode, long ruleId, int pageIndex, int pageSize, string UserId)
        {
            // 验证用户身份及权限
            
            try
            {
                return Task.FromResult(new Sports_Business().QuerySucessFolloweRecord(UserId, ruleId, gameCode, pageIndex, pageSize));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// 查询订单票数据
        /// </summary>
        public Task<Sports_TicketQueryInfoCollection> QuerySportsTicketList(string schemeId, int pageIndex, int pageSize)
        {
           
            try
            {
                var collection = new Sports_Business().QuerySchemeTicketList(schemeId, pageIndex, pageSize);

                return Task.FromResult(collection);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询单式上传全路径名
        /// </summary>
        public Task<SingleScheme_AnteCodeQueryInfo> QuerySingleSchemeFullFileName(string schemeId)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return Task.FromResult(new Sports_Business().QuerySingleSchemeFullFileName(schemeId));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询宝单奖金提成信息
        /// </summary>
        public Task<BDFXCommisionInfo> QueryBDFXCommision(string schemeId)
        {
            try
            {
                return Task.FromResult(new BDFXManager().QueryBDFXCommision(schemeId));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        /// <summary>
        /// 查询派奖失败列表
        /// </summary>
        /// <param name="gameCode"></param>
        /// <param name="gameType"></param>
        /// <param name="length"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Task<string> QueryStopIssuseList(string gameCode, string gameType, int length)
        {
           
            try
            {
                return Task.FromResult(new Sports_Business().QueryStopIssuseList(gameCode, gameType, length));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public Task<JCZQMatchResult_Collection> QueryJCZQMatchResultByTime(DateTime time)
        {
            try
            {
                return Task.FromResult(new IssuseBusiness().QueryJCZQMatchResult(time));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public Task<JCLQMatchResult_Collection> QueryJCLQMatchResultByTime(DateTime time)
        {
            try
            {
                return Task.FromResult(new IssuseBusiness().QueryJCLQMatchResult(time));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public Task<string> QueryBJDCLastIssuseNumber(int count)
        {
            return Task.FromResult(new IssuseBusiness().QueryBJDCLastIssuseNumber(count));
        }

        /// <summary>
        /// 北京单场查询开奖结果
        /// </summary>
        /// <param name="issuseNumber"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Task<BJDCMatchResultInfo_Collection> QueryBJDC_MatchResultList(string issuseNumber)
        {
            try
            {
                return Task.FromResult(new IssuseBusiness().QueryBJDC_MatchResultList(issuseNumber));
            }
            catch (Exception ex)
            {
                throw new Exception("北京单场查询开奖结果异常 - " + ex.Message, ex);
            }
        }

        public Task<ArticleInfo_QueryCollection> QueryNoStaticPathArticleList(int pageIndex, int pageSize)
        {
            var siteBiz = new SiteMessageBusiness();
            return Task.FromResult(siteBiz.QueryNoStaticPathArticleList(pageIndex, pageSize));
        }

        public Task<CommonActionResult> UpdateArticleStaticPath(string articleId, string staticPath, string preId, string nextId)
        {
            var siteBiz = new SiteMessageBusiness();
            siteBiz.UpdateArticleStaticPath(articleId, staticPath, preId, nextId);
            return Task.FromResult(new CommonActionResult(true, "提交文章成功") { });
        }


        #region 过关统计

        /// <summary>
        /// 查询过关统计
        /// </summary>
        public SportsOrder_GuoGuanInfoCollection QueryReportInfoList_GuoGuan(bool isVirtualOrder, SchemeBettingCategory? category, string key, string gameCode, string gameType, string issuseNumber, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            try
            {
                return new SqlQueryBusiness().QueryReportInfoList_GuoGuan(isVirtualOrder, category, key, gameCode, gameType, issuseNumber, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
