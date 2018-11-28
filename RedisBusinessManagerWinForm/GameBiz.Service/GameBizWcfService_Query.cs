using System;
using System.Linq;
using System.Transactions;
using Common.Communication;
using GameBiz.Business;
using GameBiz.Core;
using Common.Business;
using System.Configuration;
using GameBiz.Auth.Business;
using System.Collections.Generic;
using Common.Utilities;
using Common.Net;
using System.Data;

namespace GameBiz.Service
{
    public partial class GameBizWcfService_Query : WcfService
    {
        public GameBizWcfService_Query()
        {
            KnownTypeRegister.RegisterKnownTypes(CommunicationObjectGetter.GetCommunicationObjectTypes());
        }
        #region 订单查询

        public MyOrderListInfo QueryMyOrderDetailInfo(string schemeId)
        {
            return new SqlQueryBusiness().QueryMyOrderDetailInfo(schemeId);
        }

        public MyOrderListInfoCollection QueryMyOrderListInfo(string gameCode, BonusStatus? bonusStatus, SchemeType? schemeType,
          DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string userToken)
        {
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new SqlQueryBusiness().QueryMyOrderListInfo(userId, gameCode, bonusStatus, schemeType, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询我的投注记录出错 - " + ex.Message, ex);
            }
        }
        // 查询我的投注记录
        public MyBettingOrderInfoCollection QueryMyBettingOrderList(BonusStatus? bonusStatus, string gameCode
            , DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                return new SqlQueryBusiness().QueryMyBettingOrderList(userId, bonusStatus, gameCode, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询我的投注记录出错 - " + ex.Message, ex);
            }
        }
        // 查询我的追号订单列表
        public BettingOrderInfoCollection QueryMyChaseOrderList(string gameCode
            , DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                return new SqlQueryBusiness().QueryMyChaseOrderList(userId, gameCode, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询投注订单列表出错 - " + ex.Message, ex);
            }
        }
        // 后台查询订单列表
        public BettingOrderInfoCollection QueryBettingOrderList(string userIdOrName, SchemeType? schemeType, ProgressStatus? progressStatus, BonusStatus? bonusStatus, SchemeBettingCategory? betCategory, bool? isVirtual, string gameCode
            , DateTime startTime, DateTime endTime, int sortType, int pageIndex, int pageSize, string userToken, string fieldName,TicketStatus? ticketStatus = null, SchemeSource? schemeSource = null)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                var agentId = "";
                //var userBiz = new UserBusiness();
                //var regInfo = userBiz.GetRegisterById(userId);
                //if (regInfo.IsAgent)
                //{
                //    agentId = userId;
                //}
                return new SqlQueryBusiness().QueryBettingOrderList(userIdOrName, schemeType, progressStatus, bonusStatus, betCategory, isVirtual, gameCode, startTime, endTime, sortType, agentId, pageIndex, pageSize, fieldName, ticketStatus, schemeSource);
            }
            catch (Exception ex)
            {
                throw new Exception("查询投注订单列表出错 - " + ex.Message, ex);
            }
        }



      
        //按keyline查询追号列表
        public BettingOrderInfoCollection QueryBettingOrderListByChaseKeyLine(string keyLine, string userToken)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                var collection = new SqlQueryBusiness().QueryBettingOrderListByChaseKeyLine(keyLine);
                if (collection != null && collection.OrderList != null && collection.OrderList.Count > 0)
                {
                    collection.TotalCount = collection.OrderList.Count;
                    collection.TotalOrderMoney = collection.OrderList.Sum(o => o.TotalMoney);
                    collection.TotalBuyMoney = collection.OrderList.Sum(o => o.CurrentBettingMoney);
                    collection.TotalPreTaxBonusMoney = collection.OrderList.Sum(o => o.PreTaxBonusMoney);
                    collection.TotalAfterTaxBonusMoney = collection.OrderList.Sum(o => o.AfterTaxBonusMoney);
                    collection.TotalAddMoney = collection.OrderList.Sum(o => o.AddMoney);
                    collection.TotalUserCount = 1;
                }
                return collection;
            }
            catch (Exception ex)
            {
                throw new Exception("查询追号列表失败 - " + ex.Message, ex);
            }
        }
        //查询指定订单的投注号码列表
        public BettingAnteCodeInfoCollection QueryAnteCodeListBySchemeId(string schemeId, string userToken)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new SqlQueryBusiness().QueryAnteCodeListBySchemeId(schemeId);
            }
            catch (Exception ex)
            {
                throw new Exception("查询投注号码列表失败 - " + ex.Message, ex);
            }
        }
        // 查询指定用户创建的合买订单列表
        public TogetherOrderInfoCollection QueryCreateTogetherOrderListByUserId(string userId, BonusStatus? bonus, string gameCode, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            try
            {
                return new SqlQueryBusiness().QueryCreateTogetherOrderListByUserId(userId, bonus, gameCode, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询合买订单列表出错 - " + ex.Message, ex);
            }
        }
        // 查询指定用户参与的合买订单列表
        public TogetherOrderInfoCollection QueryJoinTogetherOrderListByUserId(string userId, BonusStatus? bonus, string gameCode, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            try
            {
                return new SqlQueryBusiness().QueryJoinTogetherOrderListByUserId(userId, bonus, gameCode, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询合买订单列表出错 - " + ex.Message, ex);
            }
        }
        // 查询合买创建统计报表
        public TogetherReportInfoGroupByUserCollection QueryCreateTogetherReportGroupByUser(string userIdList, DateTime dateFrom, DateTime dateTo, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                return new SqlQueryBusiness().QueryCreateTogetherReportGroupByUser(userIdList, dateFrom, dateTo);
            }
            catch (Exception ex)
            {
                throw new Exception("查询合买创建统计报表出错 - " + ex.Message, ex);
            }
        }

        #endregion

        #region 充值提现

        /// <summary>
        /// 查询充值记录
        /// </summary>
        public FillMoneyQueryInfoCollection QueryFillMoneyList(string userKey, string agentTypeList, string statusList, string sourceList, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string orderId, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                return new SqlQueryBusiness().QueryFillMoneyList(userKey, agentTypeList, statusList, sourceList, startTime, endTime, pageIndex, pageSize, orderId);
            }
            catch (Exception ex)
            {
                throw new Exception("查询充值记录出错", ex);
            }
        }
        /// <summary>
        /// 查询我的充值记录
        /// </summary>
        public FillMoneyQueryInfoCollection QueryMyFillMoneyList(string statusList, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var tokenUserId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                return new SqlQueryBusiness().QueryMyFillMoneyList(tokenUserId, statusList, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询充值记录出错", ex);
            }
        }

        /// <summary>
        /// 查询充值专员给用户的充值记录记录
        /// </summary>
        /// <param name="statusList"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public FillMoneyQueryInfoCollection QueryMyFillMoneyListByCzzy(string statusList, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            //var tokenUserId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                return new SqlQueryBusiness().QueryMyFillMoneyListByCzzy(userToken, statusList, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询充值记录出错", ex);
            }
        }

        /// <summary>
        /// 查询手工充值明细
        /// </summary>
        public FillMoneyQueryInfoCollection QueryManualFillMoneyList(string userKey, string statusList, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var tokenUserId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new SqlQueryBusiness().QueryManualFillMoneyList(userKey, statusList, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询手工充值明细出错 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询手工扣款明细
        /// </summary>
        public FillMoneyQueryInfoCollection QueryManualDeductMoneyList(string userKey, string statusList, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var tokenUserId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new SqlQueryBusiness().QueryManualDeductMoneyList(userKey, statusList, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询手工扣款明细出错 - " + ex.Message, ex);
            }
        }
        #endregion

        #region 查询用户资金明细

        /// <summary>
        /// 刷新我的资金明细
        /// </summary>
        public UserFundDetailCollection RefreshMyFundDetailList(DateTime fromDate, DateTime toDate, string accountTypeList, string categoryList, int pageIndex, int pageSize, string userId)
        {
            return new UserFundDetailCollection();
            //try
            //{
            //    var biz = new FundBusiness();
            //    biz.BuildTodayFundDetailCache(userId);
            //    return new FundBusiness().QueryUserFundDetailFromCache(userId, "", fromDate, toDate, accountTypeList, categoryList, pageIndex, pageSize);
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("刷新我的资金明细出错 - " + ex.Message, ex);
            //}
        }

        /// <summary>
        /// 查询我的资金明细
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="accountTypeList"> AccountType ,这个枚举类型用| 分隔传入字符串（奖金：10，冻结：20，佣金：30，充值：50，名家：60，红包：70）</param>
        /// <param name="categoryList"> 分类编号用|分隔传入字符串,BusinessHelper.资金分类</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public UserFundDetailCollection QueryMyFundDetailList(DateTime fromDate, DateTime toDate, string accountTypeList, string categoryList, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                var v = ConfigurationManager.AppSettings["QueryUserFundDetailFromCache"];
                if (!string.IsNullOrEmpty(v))
                {
                    if (bool.Parse(v))
                    {
                        return new FundBusiness().QueryUserFundDetailFromCache(userId, "", fromDate, toDate, accountTypeList, categoryList, pageIndex, pageSize);
                    }
                }
                return new SqlQueryBusiness().QueryUserFundDetail(userId, "", fromDate, toDate, accountTypeList, categoryList, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询我的资金明细出错 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询用户资金明细
        /// </summary>
        public UserFundDetailCollection QueryUserFundDetailList(string userId, string keyLine, DateTime fromDate, DateTime toDate, string accountTypeList, string categoryList, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                return new SqlQueryBusiness().QueryUserFundDetail(userId, keyLine, fromDate, toDate, accountTypeList, categoryList, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询用户资金明细出错 - " + ex.Message, ex);
            }
        }
        


        /// <summary>
        /// 查询用户资金明细报表定制
        /// </summary>
        public UserFundDetailCollection QueryUserFundDetailListReport(string userId, string keyLine, DateTime fromDate, DateTime toDate, string accountTypeList, string categoryList,  int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                return new SqlQueryBusiness().QueryUserFundDetailListReport(userId, keyLine, fromDate, toDate, accountTypeList, categoryList, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询用户资金明细报表出错 - " + ex.Message, ex);
            }
        }
        public UserFundDetailCollection QueryUserFundDetail_Commission(string userId, DateTime fromDate, DateTime toDate, int pageIndex, int pageSize)
        {
            try
            {
                return new SqlQueryBusiness().QueryUserFundDetail_Commission(userId, fromDate, toDate, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询用户资金明细出错 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询用户资金明细
        /// </summary>
        public UserFundDetailCollection QueryUserFundDetailList_CPS(string userId, string keyLine, DateTime fromDate, DateTime toDate, string accountTypeList, int pageIndex, int pageSize)
        {
            //// 验证用户身份及权限
            //var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                return new SqlQueryBusiness().QueryUserFundDetail_CPS(userId, keyLine, fromDate, toDate, accountTypeList, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询用户资金明细出错 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询用户豆豆明细
        /// </summary>
        public OCDouDouDetailInfoCollection QueryUserOCDouDouDetail(DateTime fromDate, DateTime toDate, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                return new SqlQueryBusiness().QueryUserOCDouDouDetail(userId, fromDate, toDate, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询用户豆豆明细出错 - " + ex.Message, ex);
            }
        }

        public UserFundDetailCollection QueryFundDetailInfo(string keyLine)
        {
            return new SqlQueryBusiness().QueryFundDetailInfo(keyLine);
        }

        #endregion

        #region 中奖查询，公共数据

        /// <summary>
        /// 中奖查询，公共数据
        /// </summary>
        public BonusOrderInfoCollection QueryBonusInfoList(string userId, string gameCode, string gameType, string issuseNumber, string completeData, string key, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            //var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                return new SqlQueryBusiness().QueryBonusInfoList(userId, gameCode, gameType, issuseNumber, completeData, key, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("中奖查询出错 - " + ex.Message, ex);
            }
        }
        /// <summary>
        ///  中奖查询
        /// </summary>
        /// <param name="gameCode"></param>
        /// <param name="gameType"></param>
        /// <param name="key"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public BonusOrderInfoCollection QueryUserBonusLifoList(string gameCode, string gameType, string key, int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            //var tokenId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                return new SqlQueryBusiness().QueryBonusInfoList(string.Empty, gameCode, gameType, string.Empty, string.Empty, key, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("中奖查询出错 - " + ex.Message, ex);
            }
        }

        #endregion

        #region 排行榜查询

        /// <summary>
        /// 发单盈利排行榜_竞彩类
        /// </summary>
        public RankReportCollection_BettingProfit_Sport QueryRankReport_BettingProfit_Sport(DateTime fromDate, DateTime toDate, string gameCode, string gameType, int pageIndex, int pageSize)
        {
            try
            {
                return new SqlQueryBusiness().QueryRankInfoList_BettingProfit_Sport(fromDate, toDate, gameCode, gameType, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("中奖查询出错 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 大奖排行榜_竞彩类
        /// </summary>
        public RankReportCollection_BettingProfit_Sport QueryRankReport_BigBonus_Sport(DateTime fromDate, DateTime toDate, string gameCode, string gameType, int pageIndex, int pageSize)
        {
            try
            {
                return new SqlQueryBusiness().QueryRankInfoList_BigBonus_Sport(fromDate, toDate, gameCode, gameType, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("中奖查询出错 - " + ex.Message, ex);
            }
        }


        /// <summary>
        /// 跟单盈利排行榜_竞彩类
        /// </summary>
        public RankReportCollection_BettingProfit_Sport QueryRankReport_JoinProfit_Sport(DateTime fromDate, DateTime toDate, string gameCode, string gameType, int pageIndex, int pageSize)
        {
            try
            {
                return new SqlQueryBusiness().QueryRankInfoList_JoinProfit_Sport(fromDate, toDate, gameCode, gameType, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("中奖查询出错 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 成功的战绩排行_竞彩类
        /// </summary>
        public RankReportCollection_BettingProfit_Sport QueryRankInfoList_SuccessOrder_Sport(DateTime fromDate, DateTime toDate, string gameCode, string gameType, int pageIndex, int pageSize)
        {
            try
            {
                return new SqlQueryBusiness().QueryRankInfoList_SuccessOrder_Sport(fromDate, toDate, gameCode, gameType, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("中奖查询出错 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 自动跟单排行
        /// </summary>
        public RankReportCollection_RankInfo_BeFollower QueryRankInfoList_BeFollowerCount(DateTime fromDate, DateTime toDate, string gameCode, string gameType, int pageIndex, int pageSize)
        {
            try
            {
                return new SqlQueryBusiness().QueryRankInfoList_BeFollowerCount(fromDate, toDate, gameCode, gameType, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("中奖查询出错 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 合买人气排行
        /// </summary>
        public RankReportCollection_RankInfo_HotTogether QueryRankInfoList_HotTogether(DateTime fromDate, DateTime toDate, string gameCode, string gameType, int pageIndex, int pageSize)
        {
            try
            {
                return new SqlQueryBusiness().QueryRankInfoList_HotTogether(fromDate, toDate, gameCode, gameType, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("中奖查询出错 - " + ex.Message, ex);
            }
        }

        #region 累积中奖排行榜 - 竞彩类

        public RankReportCollection_TotalBonus_Sport QueryRankReport_TotalBonus_Sport(DateTime fromDate, DateTime toDate, string gameCode, string gameType, int pageIndex, int pageSize)
        {
            try
            {
                return new SqlQueryBusiness().QueryRankInfoList_TotalBonus_Sport(fromDate, toDate, gameCode, gameType, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("中奖查询出错 - " + ex.Message, ex);
            }
        }

        #endregion

        #region 中奖排行榜 - 按彩种查

        /// <summary>
        /// 中奖排行榜_按彩种查
        /// </summary>
        public RankReportCollection_TotalBonus_Sport QueryRankReport_BonusByGameCode_All(DateTime fromDate, DateTime toDate, int topCount, string gameCode)
        {
            try
            {
                return new SqlQueryBusiness().QueryRankReport_BonusByGameCode_All(fromDate, toDate, topCount, gameCode);
            }
            catch (Exception ex)
            {
                throw new Exception("中奖查询出错 - " + ex.Message, ex);
            }
        }

        #endregion


        #endregion

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

        #region 自定义报表

        /// <summary>
        /// 查询自定义报表
        /// </summary>
        //public ReportInfoCollection GetCustomerReportList(string userToken)
        //{
        //    // 验证用户身份及权限
        //    var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

        //    try
        //    {
        //        var list = new SqlQueryBusiness().GetCustomerReportList();
        //        var collection = new ReportInfoCollection();
        //        collection.AddRange(list);
        //        return collection;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("查询自定义报表列表出错 - " + ex.Message, ex);
        //    }
        //}
        /// <summary>
        /// 获取自定义报表
        /// </summary>
        //public ReportInfo_Customer GetCustomerReportInfo(string uuid, string userToken)
        //{
        //    // 验证用户身份及权限
        //    var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

        //    try
        //    {
        //        return new SqlQueryBusiness().GetCustomerReportInfo(uuid);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("获取自定义报表出错 - " + uuid + " - " + ex.Message, ex);
        //    }
        //}
        /// <summary>
        /// 保存自定义报表
        /// </summary>
        //public CommonActionResult SaveCustomerReportXml(ReportInfo_Customer report, string userToken)
        //{
        //    // 验证用户身份及权限
        //    var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

        //    try
        //    {
        //        new SqlQueryBusiness().SaveCustomerReportXml(report);

        //        return new CommonActionResult(true, "保存自定义报表成功 - " + report.DisplayName);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("保存自定义报表出错 - " + report.DisplayName + " - " + ex.Message, ex);
        //    }
        //}
        /// <summary>
        /// 自定义报表查询
        /// </summary>
        /// <param name="report"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        //public DataTable GetCustomerReportDataTable(ReportInfo_Customer report)
        //{
        //    try
        //    {
        //        return new SqlQueryBusiness().GetDataTable(report);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("自定义报表查询出错 - " + report.DisplayName + " - " + ex.Message, ex);
        //    }
        //}
        /// <summary>
        /// 自定义报表查询
        /// </summary>
        //public DataSet GetCustomerReportDataSet(ReportInfo_Customer report)
        //{
        //    try
        //    {
        //        return new SqlQueryBusiness().GetDataSet(report);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("自定义报表查询出错 - " + report.DisplayName + " - " + ex.Message, ex);
        //    }
        //}

        #endregion

        #region 后台首页统计

        /// <summary>
        /// 后台首页统计
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public BackgroundIndexReportInfo_Collection BackgroundIndexReport(DateTime startTime, DateTime endTime, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new SqlQueryBusiness().BackgroundIndexReport(startTime, endTime);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region 比赛查询

        public CoreJCZQMatchInfoCollection QueryJCZQCanBetMatch()
        {
            try
            {
                return new SqlQueryBusiness().QueryJCZQCanBetMatch();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CoreJCLQMatchInfoCollection QueryJCLQCanBetMatch()
        {
            try
            {
                return new SqlQueryBusiness().QueryJCLQCanBetMatch();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Issuse_QueryCollection QueryCTZQCanBetIssuse(string gameType)
        {
            try
            {
                return new SqlQueryBusiness().QueryCTZQCanBetIssuse(gameType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CTZQMatchInfo_Collection QueryCTZQCanBetMatch(string gameType, string issuseNumber)
        {
            try
            {
                return new SqlQueryBusiness().QueryCTZQCanBetMatch(gameType, issuseNumber);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 查询单式上传方案详情
        /// </summary>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public OrderSingleSchemeCollection QuerySingSchemeDetail(string schemeId)
        {
            try
            {
                return new SqlQueryBusiness().QuerySingSchemeDetail(schemeId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// APP查询传统足球开奖结果
        /// </summary>
        public CTZQMatch_PoolInfo_Collection QueryCTZQMatch_PoolCollection(string gameType, string issuseNumber)
        {
            try
            {
                return new SqlQueryBusiness().QueryCTZQMatch_PoolCollection(gameType, issuseNumber);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 获取传统足球期号
        /// </summary>
        public CTZQMatch_PoolInfo_Collection GetCTZQIssuse(string gameType, int count)
        {
            try
            {
                return new SqlQueryBusiness().GetCTZQIssuse(gameType, count);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 根据订单号查询，订单信息
        /// </summary>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public BettingOrderInfo QueryOrderDetailBySchemeId(string schemeId)
        {
            try
            {
                return new SqlQueryBusiness().QueryOrderDetailBySchemeId(schemeId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询订单票数据
        /// </summary>
        public Sports_TicketQueryInfoCollection QuerySchemeTicketList(string schemeId, int pageIndex, int pageSize, DateTime startTime, DateTime endTime, string gameCode)
        {
            try
            {
                return new SqlQueryBusiness().QuerySchemeTicketList(schemeId, pageIndex, pageSize, startTime, endTime, gameCode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region 后台报表导出查询

        /// <summary>
        /// 查询用户购彩统计
        /// </summary>
        /// <returns></returns>
        public UserBetStatistics_Collection QueryUserBetStatistiscList()
        {
            try
            {
                return new SqlQueryBusiness().QueryUserBetStatistiscList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询代理返点明细
        /// </summary>
        public AgentRebateStatistics_Collection QueryAgentRebateStatisticsList(string viewType, string gs_AgentId, DateTime startTime, DateTime endTime)
        {
            try
            {
                return new SqlQueryBusiness().QueryAgentRebateStatisticsList(viewType, gs_AgentId, startTime, endTime);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion


        /// <summary>
        /// 查询虚拟订单
        /// </summary>
        public BettingOrderInfoCollection QueryVirtualOrderList(string userIdOrName, SchemeType? schemeType, ProgressStatus? progressStatus, BonusStatus? bonusStatus, SchemeBettingCategory? betCategory, string gameCode
             , DateTime startTime, DateTime endTime, int pageIndex, int pageSize
             , TicketStatus? ticketStatus = null, SchemeSource? schemeSource = null)
        {
            try
            {
                return new SqlQueryBusiness().QueryVirtualOrderList(userIdOrName, schemeType, progressStatus, bonusStatus, betCategory, gameCode, startTime, endTime, pageIndex, pageSize, ticketStatus, schemeSource);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        #region 站内信

        /// <summary>
        /// 发送站内信
        /// todo:后台权限
        /// </summary>
        public CommonActionResult SendInnerMail(InnerMailInfo_Send innerMail, string userId)
        {
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            var siteBiz = new SiteMessageControllBusiness();
            siteBiz.SendInnerMail(innerMail, userId);

            return new CommonActionResult { IsSuccess = true, Message = "发送站内信成功", };
        }
        /// <summary>
        /// 阅读站内信
        /// </summary>
        public InnerMailInfo_Query ReadInnerMail(string innerMailId, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();

                var siteBiz = new SiteMessageControllBusiness();
                if (!siteBiz.IsMyInnerMail(innerMailId, userId))
                {
                    throw new SiteMessageException(string.Format("此站内信不属于指定用户。站内信：{0}；用户：{1}。", innerMailId, userId));
                }
                siteBiz.ReadInnerMail(innerMailId, userId);
                var info = siteBiz.QueryInnerMailDetailById(innerMailId);

                biz.CommitTran();

                return info;
            }
        }
        /// <summary>
        /// 删除站内信
        /// </summary>
        public CommonActionResult DeleteInnerMail(string innerMailId, string userId)
        {
            using (var biz = new GameBiz.Business.GameBizBusinessManagement())
            {
                biz.BeginTran();

                var siteBiz = new SiteMessageControllBusiness();
                if (!siteBiz.IsMyInnerMail(innerMailId, userId))
                {
                    throw new SiteMessageException(string.Format("此站内信不属于指定用户。站内信：{0}；用户：{1}。", innerMailId, userId));
                }
                siteBiz.DeleteInnerMail(innerMailId, userId);

                biz.CommitTran();
            }
            return new CommonActionResult(true, "删除站内信完成。");
        }
        /// <summary>
        /// 是否选择我的站内信
        /// </summary>
        /// <param name="innerMailId"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public bool CheckIsMyInnerMail(string innerMailId, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            var siteBiz = new SiteMessageControllBusiness();
            return siteBiz.IsMyInnerMail(innerMailId, userId);
        }
        #region 20151226

        ///// <summary>
        ///// 根据接受者查询站内信列表
        ///// </summary>
        //public InnerMailInfo_QueryCollection QueryInnerMailListByReceiver(string receiverId, int pageIndex, int pageSize)
        //{
        //    var siteBiz = new SiteMessageControllBusiness();
        //    int totalCount;
        //    var list = siteBiz.QueryInnerMailListByReceiver(receiverId, pageIndex, pageSize, out totalCount);

        //    var result = new InnerMailInfo_QueryCollection();
        //    result.TotalCount = totalCount;
        //    result.LoadList(list);
        //    return result;
        //} 
        /// <summary>
        /// 根据接受者查询站内信列表
        /// </summary>
        public SiteMessageInnerMailListNew_Collection QueryInnerMailListByReceiver(string receiverId, int pageIndex, int pageSize)
        {
            var siteBiz = new SiteMessageControllBusiness();
            return siteBiz.QueryInnerMailListByReceiver(receiverId, pageIndex, pageSize);
        }

        #endregion

        #region 20151226

        ///// <summary>
        ///// 查询我的站内信
        ///// </summary>
        //public InnerMailInfo_QueryCollection QueryMyInnerMailList(int pageIndex, int pageSize, string userToken)
        //{
        //    // 验证用户身份及权限
        //    var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

        //    var siteBiz = new SiteMessageControllBusiness();
        //    int totalCount;
        //    var list = siteBiz.QueryInnerMailListByReceiver(userId, pageIndex, pageSize, out totalCount);

        //    var result = new InnerMailInfo_QueryCollection();
        //    result.TotalCount = totalCount;
        //    result.LoadList(list);
        //    return result;
        //} 

        ///// <summary>
        ///// 查询我的站内信
        ///// </summary>
        public SiteMessageInnerMailListNew_Collection QueryMyInnerMailList(int pageIndex, int pageSize, string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            var siteBiz = new SiteMessageControllBusiness();
            return siteBiz.QueryInnerMailListByReceiver(userId, pageIndex, pageSize);
        }

        #endregion
        /// <summary>
        /// 获取我的未读站内信条数
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public int GetMyUnreadInnerMailCount(string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            var siteBiz = new SiteMessageControllBusiness();
            return siteBiz.GetUnreadMailCountByUser(userId);
        }
        #region 20151226
        ///// <summary>
        ///// 查询已读和未读站内信
        ///// </summary>
        //public InnerMailInfo_QueryCollection QueryUnReadInnerMailListByReceiver(string userId, int pageIndex, int pageSize, InnerMailHandleType handleType)
        //{
        //    try
        //    {
        //        return new SiteMessageControllBusiness().QueryUnReadInnerMailListByReceiver(userId, pageIndex, pageSize, handleType);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message, ex);
        //    }
        //} 
        /// <summary>
        /// 查询已读和未读站内信
        /// </summary>
        public SiteMessageInnerMailListNew_Collection QueryUnReadInnerMailListByReceiver(string userId, int pageIndex, int pageSize, InnerMailHandleType handleType)
        {
            try
            {
                return new SiteMessageControllBusiness().QueryUnReadInnerMailListByReceiver(userId, pageIndex, pageSize, handleType);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion


        /// <summary>
        /// 某场景触发的发送站内消息
        /// </summary>
        public CommonActionResult DoSendSiteMessage(string userId, string mobile, string sceneKey, string msgTemplateParams)
        {
            try
            {
                var siteBiz = new SiteMessageControllBusiness();
                siteBiz.DoSendSiteMessage(userId, mobile, sceneKey, msgTemplateParams.Split('|'));
                return new CommonActionResult(true, "发送完成");

            }
            catch (Exception ex)
            {
                return new CommonActionResult(false, ex.Message);
            }
        }

        /// <summary>
        /// 查询站点信息参数
        /// </summary>
        public string QuerySiteMessageTags()
        {
            return new SiteMessageControllBusiness().QuerySiteMessageTags();
        }

        /// <summary>
        /// 查询网站通知配置
        /// </summary>
        public SiteMessageSceneInfoCollection QuerySiteNoticeConfig()
        {
            return new SiteMessageControllBusiness().QuerySiteNoticeConfig();
        }

        public CommonActionResult UpdateSiteNotice(string key, SiteMessageCategory category, string title, string content)
        {
            try
            {
                new SiteMessageControllBusiness().UpdateSiteNotice(key, category, title, content);
                return new CommonActionResult(true, "更新完成");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(false, ex.Message);
            }
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        public CommonActionResult SendSMS(string mobile, string content, string userId)
        {
            try
            {
                new SiteMessageControllBusiness().SendSMS(mobile, content, userId);
                return new CommonActionResult(true, "发送完成");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(false, ex.Message);
            }
        }

        /// <summary>
        /// 查询发送短信记录
        /// </summary>
        public MoibleSMSSendRecordInfoCollection QuerySMSSendRecordList(string userId, string mobileNumber, DateTime startTime, DateTime endTime, string status, int pageIndex, int pageSize)
        {
            return new SiteMessageControllBusiness().QuerySMSSendRecordList(userId, mobileNumber, startTime, endTime, status, pageIndex, pageSize);
        }

        #endregion

        /// <summary>
        /// 查询发起人被跟单总人数
        /// </summary>
        /// <param name="createUserId"></param>
        /// <returns></returns>
        public int QueryTogetherFollowerCount(string createUserId)
        {
            try
            {
                return new SqlQueryBusiness().QueryTogetherFollowerCount(createUserId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 根据用户编号，查询总的站内信条数
        /// </summary>
        public int GetUserInnerMailCount(string userId)
        {
            try
            {
                return new SiteMessageControllBusiness().GetUserInnerMailCount(userId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 根据角色编号查询用户编号
        /// </summary>
        public string QueryUserIdByRoleId(string roleId)
        {
            try
            {
                return new SiteMessageControllBusiness().QueryUserIdByRoleId(roleId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #region 网站结余

        /// <summary>
        /// 查询网站结余明细
        /// </summary>
        public UserBalanceHistoryInfoCollection QueryUserBalanceHistoryList(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            try
            {
                return new UserBusiness().QueryUserBalanceHistoryList(userId, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询网站结余报表
        /// </summary>
        public UserBalanceReportInfoCollection QueryUserBalanceReportList(DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            try
            {
                return new UserBusiness().QueryUserBalanceReportList(startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        // KPI报表查询
        public NewKPIDetailInfoCollection NewKPIDetailList(DateTime startTime, DateTime endTime)
        {
           

            try
            {               
                return new UserBusiness().NewKPIDetailList(startTime, endTime);
            }
            catch (Exception ex)
            {
                throw new Exception("KPI报表查询 - " + ex.Message, ex);
            }
        }
        // 资金日汇总报表查询
        public NewSummaryReportInfoCollection NewSummaryReport(DateTime startTime, DateTime endTime)
        {


            try
            {
                return new UserBusiness().NewSummaryReport(startTime, endTime);
            }
            catch (Exception ex)
            {
                throw new Exception("资金日汇总报表查询 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询某个yqid下面的 能满足领红包条件的用户个数
        /// </summary>
        /// <param name="AgentId">普通用户代理 邀请注册的会员</param>
        /// <returns></returns>
        public string QueryYqidRegisterByAgentId(string AgentId)
        {
            try
            {
                return new SqlQueryBusiness().QueryYqidRegisterByAgentId(AgentId);
            }
            catch (Exception ex)
            {
                throw new Exception("查询某个yqid下面的能满足领红包条件的用户个数出错 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// QueryYqidRegisterByAgentId方法的手机接口
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public string QueryYqidRegisterByAgentIdToApp(string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            return QueryYqidRegisterByAgentId(userId);
        }

        #endregion
    }
}
