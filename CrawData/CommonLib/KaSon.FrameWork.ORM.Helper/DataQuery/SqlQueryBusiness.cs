using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text;
using System.Linq;
using EntityModel.CoreModel;

namespace KaSon.FrameWork.ORM.Helper
{
    public class SqlQueryBusiness
    {
        //private static string _baseDir = "";
        //public static void SetReportXmlBaseDir(string dir)
        //{
        //    _baseDir = dir;
        //}

        //#region 订单查询

        //public MyOrderListInfo QueryMyOrderDetailInfo(string schemeId)
        //{
        //    return new SqlQueryManager().QueryMyOrderDetailInfo(schemeId);
        //}

        //public MyOrderListInfoCollection QueryMyOrderListInfo(string userId, string gameCode, BonusStatus? bonusStatus, SchemeType? schemeType,
        //   DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        //{
        //    var collection = new MyOrderListInfoCollection();
        //    collection.AddRange(new SqlQueryManager().QueryMyOrderListInfo(userId, gameCode, bonusStatus, schemeType, startTime, endTime, pageIndex, pageSize));
        //    return collection;
        //}

        //public MyBettingOrderInfoCollection QueryMyBettingOrderList(string userId, BonusStatus? bonusStatus, string gameCode
        //    , DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        //{
        //    var totalCount = 0;
        //    var totalBuyMoney = 0M;
        //    var totalBonusMoney = 0M;
        //    var collection = new MyBettingOrderInfoCollection();

        //    collection.OrderList = new SqlQueryManager().QueryMyBettingOrderList(userId, bonusStatus, gameCode, startTime, endTime, pageIndex, pageSize
        //        , out totalCount, out totalBuyMoney, out totalBonusMoney);
        //    collection.TotalCount = totalCount;
        //    collection.TotalBuyMoney = totalBuyMoney;
        //    collection.TotalBonusMoney = totalBonusMoney;
        //    return collection;
        //}
        //public BettingOrderInfoCollection QueryMyChaseOrderList(string userId, string gameCode
        //    , DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        //{
        //    var totalCount = 0;
        //    var totalBuyMoney = 0M;
        //    var totalOrderMoney = 0M;
        //    var totalPreTaxBonusMoney = 0M;
        //    var totalAfterTaxBonusMoney = 0M;
        //    var totalAddMoney = 0M;
        //    var collection = new BettingOrderInfoCollection();

        //    collection.OrderList = new SqlQueryManager().QueryMyChaseOrderList(userId, gameCode, startTime, endTime, pageIndex, pageSize, out totalCount);
        //    collection.TotalCount = totalCount;
        //    collection.TotalBuyMoney = totalBuyMoney;
        //    collection.TotalOrderMoney = totalOrderMoney;
        //    collection.TotalPreTaxBonusMoney = totalPreTaxBonusMoney;
        //    collection.TotalAfterTaxBonusMoney = totalAfterTaxBonusMoney;
        //    collection.TotalAddMoney = totalAddMoney;
        //    return collection;
        //}
        //public BettingOrderInfoCollection QueryBettingOrderList(string userIdOrName, SchemeType? schemeType, ProgressStatus? progressStatus, BonusStatus? bonusStatus, SchemeBettingCategory? betCategory, bool? isVirtual, string gameCode
        //    , DateTime startTime, DateTime endTime, int sortType, string agentId, int pageIndex, int pageSize, string fieldName, TicketStatus? ticketStatus = null, SchemeSource? schemeSource = null)
        //{
        //    var totalCount = 0;
        //    var totalUserCount = 0;
        //    var totalBuyMoney = 0M;
        //    var totalOrderMoney = 0M;
        //    var totalPreTaxBonusMoney = 0M;
        //    var totalAfterTaxBonusMoney = 0M;
        //    var totalAddMoney = 0M;
        //    var totalRedbagMoney = 0M;
        //    var totalRealPayRebateMoney = 0M;
        //    var totalBonusAwardsMoney = 0M;
        //    var totalRedBagAwardsMoney = 0M;
        //    var collection = new BettingOrderInfoCollection();

        //    collection.OrderList = new SqlQueryManager().QueryBettingOrderList(userIdOrName, schemeType, progressStatus, bonusStatus, betCategory, isVirtual, gameCode, startTime, endTime, sortType, agentId, pageIndex, pageSize
        //         , fieldName, out totalCount, out totalUserCount, out totalBuyMoney, out totalPreTaxBonusMoney, out totalAfterTaxBonusMoney, out totalAddMoney, out totalRedbagMoney, out totalRealPayRebateMoney, out totalBonusAwardsMoney, out totalRedBagAwardsMoney, ticketStatus, schemeSource);
        //    collection.TotalCount = totalCount;
        //    collection.TotalUserCount = totalUserCount;
        //    collection.TotalBuyMoney = totalBuyMoney;
        //    collection.TotalOrderMoney = totalOrderMoney;
        //    collection.TotalPreTaxBonusMoney = totalPreTaxBonusMoney;
        //    collection.TotalAfterTaxBonusMoney = totalAfterTaxBonusMoney;
        //    collection.TotalAddMoney = totalAddMoney;
        //    collection.TotalRedbagMoney = totalRedbagMoney;
        //    collection.TotalBonusAwardsMoney = totalBonusAwardsMoney;
        //    collection.TotalRedBagAwardsMoney = totalRedBagAwardsMoney;
        //    collection.TotalRealPayRebateMoney = totalRealPayRebateMoney;
        //    return collection;
        //}


      
        //public BettingOrderInfoCollection QueryBettingOrderListByChaseKeyLine(string keyLine)
        //{
        //    var result = new BettingOrderInfoCollection();
        //    result.OrderList = new SqlQueryManager().QueryBettingOrderListByChaseKeyLine(keyLine);
        //    return result;
        //}
        //public BettingAnteCodeInfoCollection QueryAnteCodeListBySchemeId(string schemeId)
        //{
        //    var result = new BettingAnteCodeInfoCollection();
        //    result.AnteCodeList = new SqlQueryManager().QueryAnteCodeListBySchemeId(schemeId);
        //    return result;
        //}
        //// 查询指定用户创建的合买订单列表
        //public TogetherOrderInfoCollection QueryCreateTogetherOrderListByUserId(string userId, BonusStatus? bonus, string gameCode, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        //{
        //    var totalCount = 0;
        //    var totalBuyMoney = 0M;
        //    var totalOrderMoney = 0M;

        //    var collection = new TogetherOrderInfoCollection();
        //    collection.OrderList = new SqlQueryManager().QueryCreateTogetherOrderListByUserId(userId, bonus, gameCode, startTime, endTime, pageIndex, pageSize, out totalCount, out totalBuyMoney, out totalOrderMoney);
        //    collection.TotalCount = totalCount;
        //    collection.TotalBuyMoney = totalBuyMoney;
        //    collection.TotalOrderMoney = totalOrderMoney;
        //    return collection;
        //}
        //// 查询指定用户参与的合买订单列表
        //public TogetherOrderInfoCollection QueryJoinTogetherOrderListByUserId(string userId, BonusStatus? bonus, string gameCode, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        //{
        //    var totalCount = 0;
        //    var collection = new TogetherOrderInfoCollection();

        //    collection.OrderList = new SqlQueryManager().QueryJoinTogetherOrderListByUserId(userId, bonus, gameCode, startTime, endTime, pageIndex, pageSize, out totalCount);
        //    collection.TotalCount = totalCount;
        //    return collection;
        //}
        //public TogetherReportInfoGroupByUserCollection QueryCreateTogetherReportGroupByUser(string userIdList, DateTime dateFrom, DateTime dateTo)
        //{
        //    var collection = new TogetherReportInfoGroupByUserCollection();

        //    collection.ReportList = new SqlQueryManager().QueryCreateTogetherReportGroupByUser(userIdList, dateFrom, dateTo);
        //    return collection;
        //}

        //#endregion

        //#region 资金明细

        //public UserFundDetailCollection QueryUserFundDetail(string userId, string keyLine, DateTime fromDate, DateTime toDate, string accountTypeList, string categoryList, int pageIndex, int pageSize)
        //{
        //    int totalPayinCount;
        //    decimal totalPayinMoney;
        //    int totalPayoutCount;
        //    decimal totalPayoutMoney;
        //    var collection = new UserFundDetailCollection();

        //    //List<AccountType> acList = new List<AccountType>();
        //    //if (!string.IsNullOrEmpty(accountTypeList))
        //    //{
        //    //    foreach (var item in accountTypeList.Split('|'))
        //    //    {
        //    //        acList.Add((AccountType)int.Parse(item));
        //    //    }
        //    //}
        //    //List<string> cayList = new List<string>();
        //    //if (!string.IsNullOrEmpty(categoryList))
        //    //{
        //    //    foreach (var item in categoryList.Split('|'))
        //    //    {
        //    //        cayList.Add(item);
        //    //    }
        //    //}
        //    collection.FundDetailList = new SqlQueryManager().QueryUserFundDetail(userId, keyLine, fromDate, toDate, accountTypeList, categoryList, pageIndex, pageSize
        //        , out totalPayinCount, out totalPayinMoney, out totalPayoutCount, out totalPayoutMoney);
        //    collection.TotalPayinCount = totalPayinCount;
        //    collection.TotalPayinMoney = totalPayinMoney;
        //    collection.TotalPayoutCount = totalPayoutCount;
        //    collection.TotalPayoutMoney = totalPayoutMoney;
        //    //collection.TotalBalanceMoney = new SqlQueryManager().GetAllUserBalanceMoney(userId);
        //    return collection;
        //}

        //public UserFundDetailCollection QueryUserFundDetailListReport(string userId, string keyLine, DateTime fromDate, DateTime toDate, string accountTypeList, string categoryList, int pageIndex, int pageSize)
        //{
        //    int totalPayinCount;
        //    decimal totalPayinMoney;
        //    int totalPayoutCount;
        //    decimal totalPayoutMoney;
        //    var collection = new UserFundDetailCollection();

        //    //List<AccountType> acList = new List<AccountType>();
        //    //if (!string.IsNullOrEmpty(accountTypeList))
        //    //{
        //    //    foreach (var item in accountTypeList.Split('|'))
        //    //    {
        //    //        acList.Add((AccountType)int.Parse(item));
        //    //    }
        //    //}
        //    //List<string> cayList = new List<string>();
        //    //if (!string.IsNullOrEmpty(categoryList))
        //    //{
        //    //    foreach (var item in categoryList.Split('|'))
        //    //    {
        //    //        cayList.Add(item);
        //    //    }
        //    //}
        //    collection.FundDetailList = new SqlQueryManager().QueryUserFundDetailListReport(userId, keyLine, fromDate, toDate, accountTypeList, categoryList, pageIndex, pageSize
        //        , out totalPayinCount, out totalPayinMoney, out totalPayoutCount, out totalPayoutMoney);
        //    collection.TotalPayinCount = totalPayinCount;
        //    collection.TotalPayinMoney = totalPayinMoney;
        //    collection.TotalPayoutCount = totalPayoutCount;
        //    collection.TotalPayoutMoney = totalPayoutMoney;
        //    //collection.TotalBalanceMoney = new SqlQueryManager().GetAllUserBalanceMoney(userId);
        //    return collection;
        //}
        //public UserFundDetailCollection QueryUserFundDetail_Commission(string userId, DateTime fromDate, DateTime toDate, int pageIndex, int pageSize)
        //{
        //    int totalPayinCount;
        //    decimal totalPayinMoney;
        //    var collection = new UserFundDetailCollection();
        //    collection.FundDetailList = new SqlQueryManager().QueryUserFundDetail_Commission(userId, fromDate, toDate, pageIndex, pageSize
        //        , out totalPayinCount, out totalPayinMoney);
        //    collection.TotalPayinCount = totalPayinCount;
        //    collection.TotalPayinMoney = totalPayinMoney;
        //    return collection;
        //}


        //public UserFundDetailCollection QueryUserFundDetail_CPS(string userId, string keyLine, DateTime fromDate, DateTime toDate, string accountTypeList, int pageIndex, int pageSize)
        //{
        //    int totalPayinCount;
        //    decimal totalPayinMoney;
        //    int totalPayoutCount;
        //    decimal totalPayoutMoney;
        //    var collection = new UserFundDetailCollection();
        //    collection.FundDetailList = new SqlQueryManager().QueryUserFundDetail_CPS(userId, keyLine, fromDate, toDate, accountTypeList, pageIndex, pageSize
        //        , out totalPayinCount, out totalPayinMoney, out totalPayoutCount, out totalPayoutMoney);
        //    collection.TotalPayinCount = totalPayinCount;
        //    collection.TotalPayinMoney = totalPayinMoney;
        //    collection.TotalPayoutCount = totalPayoutCount;
        //    collection.TotalPayoutMoney = totalPayoutMoney;
        //    return collection;
        //}

        //public UserFundDetailCollection QueryUserFundDetail_ContainCommission(string userId, DateTime fromDate, DateTime toDate, string accountTypeList, string categoryList, int pageIndex, int pageSize)
        //{
        //    int totalPayinCount;
        //    decimal totalPayinMoney;
        //    int totalPayoutCount;
        //    decimal totalPayoutMoney;
        //    var collection = new UserFundDetailCollection();

        //    List<AccountType> acList = new List<AccountType>();
        //    if (!string.IsNullOrEmpty(accountTypeList))
        //    {
        //        foreach (var item in accountTypeList.Split('|'))
        //        {
        //            acList.Add((AccountType)int.Parse(item));
        //        }
        //    }
        //    List<string> cayList = new List<string>();
        //    if (!string.IsNullOrEmpty(categoryList))
        //    {
        //        foreach (var item in categoryList.Split('|'))
        //        {
        //            cayList.Add(item);
        //        }
        //    }
        //    collection.FundDetailList = new SqlQueryManager().QueryUserFundDetail_ContainCommission(userId, fromDate, toDate, acList, cayList, pageIndex, pageSize
        //        , out totalPayinCount, out totalPayinMoney, out totalPayoutCount, out totalPayoutMoney);
        //    collection.TotalPayinCount = totalPayinCount;
        //    collection.TotalPayinMoney = totalPayinMoney;
        //    collection.TotalPayoutCount = totalPayoutCount;
        //    collection.TotalPayoutMoney = totalPayoutMoney;
        //    return collection;
        //}

        //public OCDouDouDetailInfoCollection QueryUserOCDouDouDetail(string userId, DateTime fromDate, DateTime toDate, int pageIndex, int pageSize)
        //{
        //    var result = new OCDouDouDetailInfoCollection();
        //    var totalCount = 0;
        //    var list = new SqlQueryManager().QueryUserOCDouDouDetail(userId, fromDate, toDate, pageIndex, pageSize, out totalCount);
        //    result.TotalCount = totalCount;
        //    result.List.AddRange(list);
        //    return result;
        //}

        //public UserFundDetailCollection QueryFundDetailInfo(string keyLine)
        //{
        //    var collection = new UserFundDetailCollection();
        //    var list = new SqlQueryManager().QueryFundDetailInfo(keyLine);
        //    foreach (var item in list)
        //    {
        //        collection.FundDetailList.Add(item);
        //    }
        //    return collection;
        //}

        //#endregion

        //#region 充值提现
        //public FillMoneyQueryInfoCollection QueryFillMoneyList(string userId, string agentTypeList, string statusList, string sourceList, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string orderId = "")
        //{
        //    var collection = new FillMoneyQueryInfoCollection();
        //    int totalCount;
        //    decimal totalRequestMoney;
        //    decimal totalResponseMoney;
        //    collection.FillMoneyList = new SqlQueryManager().QueryFillMoneyList(userId, agentTypeList, statusList, sourceList, startTime, endTime
        //    , pageIndex, pageSize, orderId, out totalCount, out totalRequestMoney, out totalResponseMoney);
        //    collection.TotalCount = totalCount;
        //    collection.TotalRequestMoney = totalRequestMoney;
        //    collection.TotalResponseMoney = totalResponseMoney;
        //    return collection;
        //}
        //public FillMoneyQueryInfoCollection QueryMyFillMoneyList(string userId, string statusList, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        //{
        //    return this.QueryFillMoneyList(userId, "", statusList, "", startTime, endTime, pageIndex, pageSize);
        //}
        //public FillMoneyQueryInfoCollection QueryManualFillMoneyList(string userKey, string statusList, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        //{
        //    return this.QueryFillMoneyList(userKey, ((int)FillMoneyAgentType.ManualAdd).ToString(), statusList, "", startTime, endTime, pageIndex, pageSize);
        //}
        //public FillMoneyQueryInfoCollection QueryManualDeductMoneyList(string userKey, string statusList, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        //{
        //    return this.QueryFillMoneyList(userKey, ((int)FillMoneyAgentType.ManualDeduct).ToString(), statusList, "", startTime, endTime, pageIndex, pageSize);
        //}

        ///// <summary>
        ///// 查询充值专员给用户的充值记录记录
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <param name="agentTypeList"></param>
        ///// <param name="statusList"></param>
        ///// <param name="sourceList"></param>
        ///// <param name="startTime"></param>
        ///// <param name="endTime"></param>
        ///// <param name="pageIndex"></param>
        ///// <param name="pageSize"></param>
        ///// <param name="orderId"></param>
        ///// <returns></returns>
        //public FillMoneyQueryInfoCollection QueryMyFillMoneyListByCzzy(string userId, string agentTypeList, string statusList, string sourceList, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string orderId = "")
        //{
        //    var collection = new FillMoneyQueryInfoCollection();
        //    int totalCount;
        //    decimal totalRequestMoney;
        //    decimal totalResponseMoney;
        //    collection.FillMoneyList = new SqlQueryManager().QueryMyFillMoneyListByCzzy(userId, agentTypeList, statusList, sourceList, startTime, endTime
        //    , pageIndex, pageSize, orderId, out totalCount, out totalRequestMoney, out totalResponseMoney);
        //    collection.TotalCount = totalCount;
        //    collection.TotalRequestMoney = totalRequestMoney;
        //    collection.TotalResponseMoney = totalResponseMoney;
        //    return collection;
        //}
        ///// <summary>
        ///// 查询充值专员给用户的充值记录记录
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <param name="statusList"></param>
        ///// <param name="startTime"></param>
        ///// <param name="endTime"></param>
        ///// <param name="pageIndex"></param>
        ///// <param name="pageSize"></param>
        ///// <returns></returns>
        //public FillMoneyQueryInfoCollection QueryMyFillMoneyListByCzzy(string userId, string statusList, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        //{
        //    return this.QueryMyFillMoneyListByCzzy(userId, "", statusList, "", startTime, endTime, pageIndex, pageSize);
        //}
        //#endregion

        #region 中奖查询

        //public BonusOrderInfoCollection QueryBonusInfoList(string userId, string gameCode, string gameType, string issuseNumber, string completeData, string key, int pageIndex, int pageSize)
        //{
        //    var result = new BonusOrderInfoCollection();
        //    var totalCount = 0;
        //    result.BonusOrderList.AddRange(new DataQuery().QueryBonusInfoList(userId, gameCode, gameType, issuseNumber, completeData, key, pageIndex, pageSize, out totalCount));
        //    result.TotalCount = totalCount;
        //    return result;
        //}

        #endregion

        //#region 排行榜查询


        ///// <summary>
        ///// 发单盈利排行榜
        ///// </summary>
        //public RankReportCollection_BettingProfit_Sport QueryRankInfoList_BettingProfit_Sport(DateTime fromDate, DateTime toDate, string gameCode, string gameType, int pageIndex, int pageSize)
        //{
        //    var result = new RankReportCollection_BettingProfit_Sport();
        //    int totalCount;
        //    result.RankInfoList.AddRange(new SqlQueryManager().QueryRankInfoList_BettingProfit_Sport(fromDate, toDate, gameCode, gameType, pageIndex, pageSize, out totalCount));
        //    result.TotalCount = totalCount;
        //    return result;
        //}

        ///// <summary>
        ///// 跟单盈利排行榜
        ///// </summary>
        //public RankReportCollection_BettingProfit_Sport QueryRankInfoList_JoinProfit_Sport(DateTime fromDate, DateTime toDate, string gameCode, string gameType, int pageIndex, int pageSize)
        //{
        //    var result = new RankReportCollection_BettingProfit_Sport();
        //    int totalCount;
        //    result.RankInfoList.AddRange(new SqlQueryManager().QueryRankInfoList_JoinProfit_Sport(fromDate, toDate, gameCode, gameType, pageIndex, pageSize, out totalCount));
        //    result.TotalCount = totalCount;
        //    return result;
        //}

        ///// <summary>
        ///// 大奖排行榜
        ///// </summary>
        //public RankReportCollection_BettingProfit_Sport QueryRankInfoList_BigBonus_Sport(DateTime fromDate, DateTime toDate, string gameCode, string gameType, int pageIndex, int pageSize)
        //{
        //    var result = new RankReportCollection_BettingProfit_Sport();
        //    int totalCount;
        //    result.RankInfoList.AddRange(new SqlQueryManager().QueryRankInfoList_BigBonus_Sport(fromDate, toDate, gameCode, gameType, pageIndex, pageSize, out totalCount));
        //    result.TotalCount = totalCount;
        //    return result;
        //}


        ///// <summary>
        ///// 成功的战绩排行
        ///// </summary>
        //public RankReportCollection_BettingProfit_Sport QueryRankInfoList_SuccessOrder_Sport(DateTime fromDate, DateTime toDate, string gameCode, string gameType, int pageIndex, int pageSize)
        //{
        //    var result = new RankReportCollection_BettingProfit_Sport();
        //    int totalCount;
        //    result.RankInfoList.AddRange(new SqlQueryManager().QueryRankInfoList_SuccessOrder_Sport(fromDate, toDate, gameCode, gameType, pageIndex, pageSize, out totalCount));
        //    result.TotalCount = totalCount;
        //    return result;
        //}

        ///// <summary>
        ///// 跟单排行榜
        ///// </summary>
        //public RankReportCollection_RankInfo_BeFollower QueryRankInfoList_BeFollowerCount(DateTime fromDate, DateTime toDate, string gameCode, string gameType, int pageIndex, int pageSize)
        //{
        //    var result = new RankReportCollection_RankInfo_BeFollower();
        //    int totalCount;
        //    if (BusinessHelper.CheckSQLCondition(gameCode) || BusinessHelper.CheckSQLCondition(gameType))
        //        throw new Exception("传入彩种或玩法含有特殊字符");
        //    result.RankInfoList.AddRange(new SqlQueryManager().QueryRankInfoList_BeFollowerCount(fromDate, toDate, gameCode, gameType, pageIndex, pageSize, out totalCount));
        //    result.TotalCount = totalCount;
        //    return result;
        //}

        ///// <summary>
        ///// 合买人气排行
        ///// </summary>
        //public RankReportCollection_RankInfo_HotTogether QueryRankInfoList_HotTogether(DateTime fromDate, DateTime toDate, string gameCode, string gameType, int pageIndex, int pageSize)
        //{
        //    var result = new RankReportCollection_RankInfo_HotTogether();
        //    int totalCount;
        //    result.RankInfoList.AddRange(new SqlQueryManager().QueryRankInfoList_HotTogether(fromDate, toDate, gameCode, gameType, pageIndex, pageSize, out totalCount));
        //    result.TotalCount = totalCount;
        //    return result;
        //}


        //#region 累积中奖排行榜 - 竞彩类

        //public RankReportCollection_TotalBonus_Sport QueryRankInfoList_TotalBonus_Sport(DateTime fromDate, DateTime toDate, string gameCode, string gameType, int pageIndex, int pageSize)
        //{
        //    var result = new RankReportCollection_TotalBonus_Sport();
        //    int totalCount;
        //    result.RankInfoList = new SqlQueryManager().QueryRankInfoList_TotalBonus_Sport(fromDate, toDate, gameCode, gameType, pageIndex, pageSize, out totalCount);
        //    result.TotalCount = totalCount;
        //    return result;
        //}

        //#endregion

        //#region 中奖排行榜 - 按彩种查

        //public RankReportCollection_TotalBonus_Sport QueryRankReport_BonusByGameCode_All(DateTime fromDate, DateTime toDate, int topCount, string gameCode)
        //{
        //    var result = new RankReportCollection_TotalBonus_Sport();
        //    result.RankInfoList = new SqlQueryManager().QueryRankReport_BonusByGameCode_All(fromDate, toDate, topCount, gameCode);
        //    return result;
        //}

        //#endregion

        //#endregion

        //#region 过关统计

        ///// <summary>
        ///// 查询过关统计
        ///// </summary>
        //public SportsOrder_GuoGuanInfoCollection QueryReportInfoList_GuoGuan(bool? isVirtualOrder, SchemeBettingCategory? category, string key, string gameCode, string gameType, string issuseNumber, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        //{
        //    var result = new SportsOrder_GuoGuanInfoCollection();
        //    var totalCount = 0;
        //    result.ReportItemList.AddRange(new SqlQueryManager().QueryReportInfoList_GuoGuan(isVirtualOrder, category, key, gameCode, gameType, issuseNumber, startTime, endTime, pageIndex, pageSize, out totalCount));
        //    result.TotalCount = totalCount;
        //    return result;
        //}

        //#endregion

        //#region 统计报表

        //// 获取营销概况数据
        //public DataTable GetSaleSummaryDataTable(string userIdList, string dateType, DateTime dateFrom, DateTime dateTo)
        //{
        //    var hourOffset = -5;
        //    var sql = "EXEC P_Report_QuerySaleSummary @userIdList,@dateType,@dateFrom,@dateTo,@hourOffset";
        //    var paramList = new Dictionary<string, object>();
        //    paramList.Add("userIdList", userIdList);
        //    paramList.Add("dateType", dateType);
        //    paramList.Add("dateFrom", dateFrom);
        //    paramList.Add("dateTo", dateTo);
        //    paramList.Add("hourOffset", hourOffset);
        //    return new SqlQueryManager().GetDataTable(sql, paramList);
        //}

        //#endregion

        //#region 自定义报表

        ////public IList<ReportInfo_Customer> GetCustomerReportList()
        ////{
        ////    var list = new List<ReportInfo_Customer>();
        ////    var fileName = GetReportConfigFullName();
        ////    if (File.Exists(fileName))
        ////    {
        ////        var doc = new XmlDocument();
        ////        doc.Load(fileName);
        ////        foreach (XmlNode item in doc.SelectNodes("reports/report"))
        ////        {
        ////            var report = new ReportInfo_Customer();
        ////            report.UUID = item.Attributes["id"].Value;
        ////            report.DisplayName = item.Attributes["displayName"].Value;
        ////            report.Tag = item.Attributes["tag"].Value;
        ////            report.IsShow = bool.Parse(item.Attributes["isShow"].Value);
        ////            report.TopOnMenu = bool.Parse(item.Attributes["isTop"].Value);
        ////            list.Add(report);
        ////        }
        ////    }
        ////    return list;
        ////}
        ////public ReportInfo_Customer GetCustomerReportInfo(string uuid)
        ////{
        ////    var fileName = GetReportFileFullName(uuid);
        ////    if (!File.Exists(fileName))
        ////    {
        ////        return null;
        ////    }
        ////    var report = new ReportInfo_Customer();
        ////    report.Parameters = new List<ReportParameterInfo>();
        ////    var doc = new XmlDocument();
        ////    doc.Load(fileName);

        ////    var e_uuid = doc.SelectSingleNode("report/uuid");
        ////    if (e_uuid != null) report.UUID = e_uuid.InnerText;

        ////    var e_displayName = doc.SelectSingleNode("report/displayName");
        ////    if (e_displayName != null) report.DisplayName = e_displayName.InnerText;

        ////    var e_tag = doc.SelectSingleNode("report/tag");
        ////    if (e_tag != null) report.Tag = e_tag.InnerText;

        ////    var e_isShow = doc.SelectSingleNode("report/isShow");
        ////    if (e_isShow != null) report.IsShow = bool.Parse(e_isShow.InnerText);

        ////    var e_topOnMenu = doc.SelectSingleNode("report/topOnMenu");
        ////    if (e_topOnMenu != null) report.TopOnMenu = bool.Parse(e_topOnMenu.InnerText);

        ////    var e_sql = doc.SelectSingleNode("report/sql");
        ////    if (e_sql != null) report.Sql = e_sql.InnerText;

        ////    var e_p = doc.SelectSingleNode("report/params");
        ////    if (e_p != null)
        ////    {
        ////        foreach (XmlElement item in e_p.ChildNodes)
        ////        {
        ////            var p = new ReportParameterInfo();
        ////            p.Id = item.Attributes["id"].Value;
        ////            p.Name = item.Attributes["name"].Value;
        ////            p.Type = item.Attributes["type"].Value;
        ////            p.Default = item.Attributes["default"].Value;
        ////            report.Parameters.Add(p);
        ////        }
        ////    }

        ////    return report;
        ////}
        ////public void SaveCustomerReportXml(ReportInfo_Customer report)
        ////{
        ////    var doc = new XmlDocument();
        ////    if (string.IsNullOrEmpty(report.UUID))
        ////    {
        ////        report.UUID = Common.Utilities.UsefullHelper.UUID();
        ////        var fileName = GetReportFileFullName(report.UUID);
        ////        if (!Directory.Exists(Path.GetDirectoryName(fileName)))
        ////        {
        ////            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
        ////        }
        ////        XmlNode root;
        ////        doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));
        ////        root = doc.AppendChild(doc.CreateElement("report"));

        ////        var e_uuid = doc.CreateElement("uuid");
        ////        e_uuid.InnerText = report.UUID;
        ////        root.AppendChild(e_uuid);
        ////        var e_displayName = doc.CreateElement("displayName");
        ////        e_displayName.InnerText = report.DisplayName;
        ////        root.AppendChild(e_displayName);
        ////        var e_tag = doc.CreateElement("tag");
        ////        e_tag.InnerText = report.Tag;
        ////        root.AppendChild(e_tag);
        ////        var e_isShow = doc.CreateElement("isShow");
        ////        e_isShow.InnerText = report.IsShow.ToString();
        ////        root.AppendChild(e_isShow);
        ////        var e_isTop = doc.CreateElement("isTop");
        ////        e_isTop.InnerText = report.TopOnMenu.ToString();
        ////        root.AppendChild(e_isTop);
        ////        var e_sql = doc.CreateElement("sql");
        ////        e_sql.AppendChild(doc.CreateCDataSection(report.Sql));
        ////        root.AppendChild(e_sql);

        ////        var e_params = doc.CreateElement("params");
        ////        foreach (var p in report.Parameters)
        ////        {
        ////            var e_p = doc.CreateElement("p");
        ////            var a_id = doc.CreateAttribute("id"); a_id.Value = p.Id; e_p.Attributes.Append(a_id);
        ////            var a_name = doc.CreateAttribute("name"); a_name.Value = p.Name; e_p.Attributes.Append(a_name);
        ////            var a_type = doc.CreateAttribute("type"); a_type.Value = p.Type; e_p.Attributes.Append(a_type);
        ////            var a_default = doc.CreateAttribute("default"); a_default.Value = p.Default; e_p.Attributes.Append(a_default);
        ////            e_params.AppendChild(e_p);
        ////        }
        ////        root.AppendChild(e_params);
        ////        doc.Save(fileName);
        ////    }
        ////    else
        ////    {
        ////        var fileName = GetReportFileFullName(report.UUID);
        ////        doc.Load(fileName);
        ////        doc.SelectSingleNode("report/displayName").InnerText = report.DisplayName;
        ////        doc.SelectSingleNode("report/tag").InnerText = report.Tag;
        ////        doc.SelectSingleNode("report/isShow").InnerText = report.IsShow.ToString();
        ////        doc.SelectSingleNode("report/isTop").InnerText = report.TopOnMenu.ToString();
        ////        ((XmlCDataSection)doc.SelectSingleNode("report/sql").FirstChild).Data = report.Sql;
        ////        var e_params = doc.SelectSingleNode("report/params");
        ////        e_params.RemoveAll();
        ////        foreach (var p in report.Parameters)
        ////        {
        ////            var e_p = doc.CreateElement("p");
        ////            var a_id = doc.CreateAttribute("id"); a_id.Value = p.Id; e_p.Attributes.Append(a_id);
        ////            var a_name = doc.CreateAttribute("name"); a_name.Value = p.Name; e_p.Attributes.Append(a_name);
        ////            var a_type = doc.CreateAttribute("type"); a_type.Value = p.Type; e_p.Attributes.Append(a_type);
        ////            var a_default = doc.CreateAttribute("default"); a_default.Value = p.Default; e_p.Attributes.Append(a_default);
        ////            e_params.AppendChild(e_p);
        ////        }
        ////        doc.Save(fileName);
        ////    }
        ////    var configName = GetReportConfigFullName();
        ////    doc.Load(configName);
        ////    var listRoot = doc.SelectSingleNode("reports");
        ////    var isExsit = false;
        ////    foreach (XmlNode item in listRoot.ChildNodes)
        ////    {
        ////        var id = item.Attributes["id"].Value;
        ////        if (id.Equals(report.UUID))
        ////        {
        ////            item.Attributes["displayName"].Value = report.DisplayName;
        ////            item.Attributes["tag"].Value = report.Tag;
        ////            item.Attributes["isShow"].Value = report.IsShow.ToString();
        ////            item.Attributes["isTop"].Value = report.TopOnMenu.ToString();
        ////            isExsit = true;
        ////        }
        ////    }
        ////    if (!isExsit)
        ////    {
        ////        var creat = doc.CreateElement("report");
        ////        creat.Attributes["id"].Value = report.UUID;
        ////        creat.Attributes["displayName"].Value = report.DisplayName;
        ////        creat.Attributes["tag"].Value = report.Tag;
        ////        creat.Attributes["isShow"].Value = report.IsShow.ToString();
        ////        creat.Attributes["isTop"].Value = report.TopOnMenu.ToString();
        ////        listRoot.AppendChild(creat);
        ////    }
        ////    doc.Save(configName);
        ////}

        //public DataTable GetDataTable(ReportInfo_Customer report)
        //{
        //    var sql = report.Sql;
        //    var paramList = new Dictionary<string, object>();
        //    foreach (var p in report.Parameters)
        //    {
        //        paramList.Add(p.Id, p.Value);
        //    }
        //    return new SqlQueryManager().GetDataTable(sql, paramList);
        //}
        //public DataSet GetDataSet(ReportInfo_Customer report)
        //{
        //    var sql = report.Sql;
        //    var paramList = new Dictionary<string, object>();
        //    foreach (var p in report.Parameters)
        //    {
        //        sql = sql.Replace("{p:" + p.Id + "}", ":" + p.Id);
        //        paramList.Add(p.Id, p.Value);
        //    }
        //    return new SqlQueryManager().GetDataSet(sql, paramList);
        //}
        ////private string GetReportFileFullName(string uuid)
        ////{
        ////    var fullName = Path.Combine(_baseDir, "Reports", uuid + ".xml");
        ////    return fullName;
        ////}
        ////private string GetReportConfigFullName()
        ////{
        ////    var fullName = Path.Combine(_baseDir, "Report.Config.xml");
        ////    return fullName;
        ////}

        //#endregion

        //#region 后台首页统计

        //public BackgroundIndexReportInfo_Collection BackgroundIndexReport(DateTime startTime, DateTime endTime)
        //{
        //    using (var manage = new SqlQueryManager())
        //    {
        //        return manage.BackgroundIndexReport(startTime, endTime);
        //    }
        //}

        //#endregion

        //#region 比赛查询

        //public CoreJCZQMatchInfoCollection QueryJCZQCanBetMatch()
        //{
        //    var list = new CoreJCZQMatchInfoCollection();
        //    list.AddRange(new SqlQueryManager().QueryJCZQCanBetMatch());
        //    return list;
        //}

        //public CoreJCLQMatchInfoCollection QueryJCLQCanBetMatch()
        //{
        //    var list = new CoreJCLQMatchInfoCollection();
        //    list.AddRange(new SqlQueryManager().QueryJCLQCanBetMatch());
        //    return list;
        //}

        //public Issuse_QueryCollection QueryCTZQCanBetIssuse(string gameType)
        //{
        //    var list = new Issuse_QueryCollection();
        //    list.IssuseList.AddRange(new CTZQMatchManager().QueryCTZQCanBetIssuse(gameType));
        //    return list;
        //}

        //public CTZQMatchInfo_Collection QueryCTZQCanBetMatch(string gameType, string issuseNumber)
        //{
        //    var list = new CTZQMatchInfo_Collection();
        //    list.ListInfo.AddRange(new CTZQMatchManager().QueryCTZQMatchListByIssuseNumber(gameType, issuseNumber));
        //    return list;
        //}
        //public OrderSingleSchemeCollection QuerySingSchemeDetail(string schemeId)
        //{
        //    var sportsManager = new Sports_Manager();
        //    var sqlQueryManager = new SqlQueryManager();
        //    var result = new OrderSingleSchemeCollection();
        //    var singCodeList = sportsManager.QuerySingleScheme_AnteCode(schemeId);
        //    if (singCodeList == null || string.IsNullOrEmpty(singCodeList.SelectMatchId))
        //        throw new Exception("未查询到投注内容");
        //    var arrayMatchId = singCodeList.SelectMatchId.Split(',');
        //    foreach (var item in arrayMatchId)
        //    {

        //        var anteCode = sportsManager.QueryAnteCode(singCodeList.SchemeId, item, singCodeList.GameType);
        //        if (singCodeList.GameCode.ToUpper() == "BJDC")
        //        {
        //            var match = sportsManager.QueryBJDC_Match(string.Format("{0}|{1}", singCodeList.IssuseNumber, item));
        //            var matchResult = sportsManager.QueryBJDC_MatchResult(string.Format("{0}|{1}", singCodeList.IssuseNumber, item));
        //            var halfResult = string.Empty;
        //            var fullResult = string.Empty;
        //            var caiguo = string.Empty;
        //            var matchResultSp = 0M;
        //            var matchState = string.Empty;
        //            if (matchResult != null)
        //            {
        //                halfResult = string.Format("{0}:{1}", matchResult.HomeHalf_Result, matchResult.GuestHalf_Result);
        //                fullResult = string.Format("{0}:{1}", matchResult.HomeFull_Result, matchResult.GuestFull_Result);
        //                matchState = matchResult.MatchState;
        //                switch (singCodeList.GameType)
        //                {
        //                    case "SPF":
        //                        caiguo = matchResult.SPF_Result;
        //                        matchResultSp = matchResult.SPF_SP;
        //                        break;
        //                    case "ZJQ":
        //                        caiguo = matchResult.ZJQ_Result;
        //                        matchResultSp = matchResult.ZJQ_SP;
        //                        break;
        //                    case "SXDS":
        //                        caiguo = matchResult.SXDS_Result;
        //                        matchResultSp = matchResult.SXDS_SP;
        //                        break;
        //                    case "BF":
        //                        caiguo = matchResult.BF_Result;
        //                        matchResultSp = matchResult.BF_SP;
        //                        break;
        //                    case "BQC":
        //                        caiguo = matchResult.BQC_Result;
        //                        matchResultSp = matchResult.BQC_SP;
        //                        break;
        //                }
        //            }
        //            result.AnteCodeList.Add(new OrderSingleScheme
        //            {
        //                IssuseNumber = match.IssuseNumber,
        //                LeagueId = string.Empty,
        //                LeagueName = match.MatchName,
        //                LeagueColor = match.MatchColor,
        //                MatchId = match.MatchOrderId.ToString(),
        //                MatchIdName = string.Empty,
        //                HomeTeamId = string.Empty,
        //                HomeTeamName = match.HomeTeamName,
        //                GuestTeamId = string.Empty,
        //                GuestTeamName = match.GuestTeamName,
        //                IsDan = anteCode == null ? false : anteCode.IsDan,
        //                StartTime = match.MatchStartTime,
        //                HalfResult = halfResult,
        //                FullResult = fullResult,
        //                MatchResult = caiguo,
        //                MatchResultSp = matchResultSp,
        //                CurrentSp = anteCode == null ? "0" : anteCode.Odds,
        //                LetBall = match.LetBall,
        //                BonusStatus = anteCode == null ? BonusStatus.Waitting : anteCode.BonusStatus,
        //                GameType = singCodeList.GameType,
        //                MatchState = matchState,
        //                FileBuffer = singCodeList.FileBuffer,
        //                PlayType = singCodeList.PlayType,
        //                ContainsMatchId = singCodeList.ContainsMatchId,
        //            });
        //            continue;
        //        }
        //        if (singCodeList.GameCode.ToUpper() == "JCZQ")
        //        {
        //            var match = sportsManager.QueryJCZQ_Match(item);
        //            var matchResult = sportsManager.QueryJCZQ_MatchResult(item);
        //            if (match == null)
        //                throw new Exception("未查询到比赛数据");
        //            var halfResult = string.Empty;
        //            var fullResult = string.Empty;
        //            var caiguo = string.Empty;
        //            var matchResultSp = 0M;
        //            var matchState = string.Empty;
        //            if (matchResult != null)
        //            {
        //                halfResult = string.Format("{0}:{1}", matchResult.HalfHomeTeamScore, matchResult.HalfGuestTeamScore);
        //                fullResult = string.Format("{0}:{1}", matchResult.FullHomeTeamScore, matchResult.FullGuestTeamScore);
        //                matchState = matchResult.MatchState;
        //                switch (singCodeList.GameType.ToUpper())
        //                {
        //                    case "SPF":
        //                        caiguo = matchResult.SPF_Result;
        //                        matchResultSp = matchResult.SPF_SP;
        //                        break;
        //                    case "BRQSPF":
        //                        caiguo = matchResult.BRQSPF_Result;
        //                        matchResultSp = matchResult.BRQSPF_SP;
        //                        break;
        //                    case "ZJQ":
        //                        caiguo = matchResult.ZJQ_Result;
        //                        matchResultSp = matchResult.ZJQ_SP;
        //                        break;
        //                    case "BF":
        //                        caiguo = matchResult.BF_Result;
        //                        matchResultSp = matchResult.BF_SP;
        //                        break;
        //                    case "BQC":
        //                        caiguo = matchResult.BQC_Result;
        //                        matchResultSp = matchResult.BQC_SP;
        //                        break;
        //                }
        //            }
        //            result.AnteCodeList.Add(new OrderSingleScheme
        //            {
        //                IssuseNumber = string.Empty,
        //                LeagueId = match.LeagueId.ToString(),
        //                LeagueName = match.LeagueName,
        //                LeagueColor = match.LeagueColor,
        //                MatchId = match.MatchId,
        //                MatchIdName = match.MatchIdName,
        //                HomeTeamId = match.HomeTeamId.ToString(),
        //                HomeTeamName = match.HomeTeamName,
        //                GuestTeamId = match.GuestTeamId.ToString(),
        //                GuestTeamName = match.GuestTeamName,
        //                IsDan = anteCode == null ? false : anteCode.IsDan,
        //                StartTime = match.StartDateTime,
        //                HalfResult = halfResult,
        //                FullResult = fullResult,
        //                MatchResult = caiguo,
        //                MatchResultSp = matchResultSp,
        //                CurrentSp = anteCode == null ? "0" : anteCode.Odds,
        //                LetBall = match.LetBall,
        //                BonusStatus = anteCode == null ? BonusStatus.Waitting : anteCode.BonusStatus,
        //                GameType = singCodeList.GameType,
        //                MatchState = matchState,
        //                FileBuffer = singCodeList.FileBuffer,
        //                PlayType = singCodeList.PlayType,
        //                ContainsMatchId = singCodeList.ContainsMatchId,
        //            });
        //            continue;
        //        }
        //        if (singCodeList.GameCode.ToUpper() == "JCLQ")
        //        {
        //            var match = sportsManager.QueryJCLQ_Match(item);
        //            var matchResult = sportsManager.QueryJCLQ_MatchResult(item);
        //            var halfResult = string.Empty;
        //            var fullResult = string.Empty;
        //            var caiguo = string.Empty;
        //            var matchResultSp = 0M;
        //            var matchState = string.Empty;
        //            if (matchResult != null)
        //            {
        //                //halfResult = string.Format("{0}:{1}", matchResult.HomeScore, matchResult.GuestHalf_Result);
        //                fullResult = string.Format("{0}:{1}", matchResult.HomeScore, matchResult.GuestScore);
        //                matchState = matchResult.MatchState;
        //                switch (singCodeList.GameType.ToUpper())
        //                {
        //                    case "SF":
        //                        caiguo = matchResult.SF_Result;
        //                        matchResultSp = matchResult.SF_SP;
        //                        break;
        //                    case "RFSF":
        //                        caiguo = matchResult.RFSF_Result;
        //                        matchResultSp = matchResult.RFSF_SP;
        //                        break;
        //                    case "SFC":
        //                        caiguo = matchResult.SFC_Result;
        //                        matchResultSp = matchResult.SFC_SP;
        //                        break;
        //                    case "DXF":
        //                        caiguo = matchResult.DXF_Result;
        //                        matchResultSp = matchResult.DXF_SP;
        //                        break;
        //                }
        //            }
        //            result.AnteCodeList.Add(new OrderSingleScheme
        //            {
        //                IssuseNumber = string.Empty,
        //                LeagueId = match.LeagueId.ToString(),
        //                LeagueName = match.LeagueName,
        //                LeagueColor = match.LeagueColor,
        //                MatchId = match.MatchId,
        //                MatchIdName = match.MatchIdName,
        //                HomeTeamId = string.Empty,
        //                HomeTeamName = match.HomeTeamName,
        //                GuestTeamId = string.Empty,
        //                GuestTeamName = match.GuestTeamName,
        //                IsDan = anteCode == null ? false : anteCode.IsDan,
        //                StartTime = match.StartDateTime,
        //                HalfResult = halfResult,
        //                FullResult = fullResult,
        //                MatchResult = caiguo,
        //                MatchResultSp = matchResultSp,
        //                CurrentSp = anteCode == null ? "0" : anteCode.Odds,
        //                BonusStatus = anteCode == null ? BonusStatus.Waitting : anteCode.BonusStatus,
        //                GameType = singCodeList.GameType,
        //                MatchState = matchState,
        //                FileBuffer = singCodeList.FileBuffer,
        //                PlayType = singCodeList.PlayType,
        //                ContainsMatchId = singCodeList.ContainsMatchId,
        //            });
        //            continue;
        //        }
        //        if (singCodeList.GameCode.ToUpper() == "CTZQ")
        //        {
        //            result.AnteCodeList.Add(new OrderSingleScheme
        //            {
        //                IssuseNumber = singCodeList.IssuseNumber,
        //                LeagueId = string.Empty,
        //                LeagueName = string.Empty,
        //                LeagueColor = string.Empty,
        //                MatchId = string.Empty,
        //                MatchIdName = string.Empty,
        //                HomeTeamId = string.Empty,
        //                HomeTeamName = string.Empty,
        //                GuestTeamId = string.Empty,
        //                GuestTeamName = string.Empty,
        //                IsDan = anteCode == null ? false : anteCode.IsDan,
        //                StartTime = DateTime.Now,
        //                HalfResult = string.Empty,
        //                FullResult = string.Empty,
        //                MatchResult = string.Empty,
        //                MatchResultSp = 0M,
        //                CurrentSp = anteCode == null ? "0" : anteCode.Odds,
        //                BonusStatus = anteCode == null ? BonusStatus.Waitting : anteCode.BonusStatus,
        //                GameType = singCodeList.GameType,
        //                FileBuffer = singCodeList.FileBuffer,
        //                PlayType = singCodeList.PlayType,
        //                ContainsMatchId = singCodeList.ContainsMatchId,
        //            });
        //        }
        //    }
        //    return result;
        //}

        

        //public CTZQMatch_PoolInfo_Collection QueryCTZQMatch_PoolCollection(string gameType, string issuseNumber)
        //{
        //    using (var manager = new CTZQMatchManager())
        //    {
        //        return manager.QueryCTZQMatch_PoolCollection(gameType, issuseNumber);
        //    }
        //}
        //public CTZQMatch_PoolInfo_Collection GetCTZQIssuse(string gameType, int count)
        //{
        //    using (var manager = new CTZQMatchManager())
        //    {
        //        return manager.GetCTZQIssuse(gameType, count);
        //    }
        //}

        //#endregion

        //public BettingOrderInfo QueryOrderDetailBySchemeId(string schemeId)
        //{
        //    using (var manager = new SqlQueryManager())
        //    {
        //        return manager.QueryOrderDetailBySchemeId(schemeId);
        //    }
        //}

        ///// <summary>
        ///// 查询订单票数据
        ///// </summary>
        //public Sports_TicketQueryInfoCollection QuerySchemeTicketList(string schemeId, int pageIndex, int pageSize, DateTime startTime, DateTime endTime, string gameCode)
        //{
        //    var result = new Sports_TicketQueryInfoCollection();

        //    var totalCount = 0;
        //    var sportManager = new SqlQueryManager();
        //    var list = sportManager.QueryTicketInfoList(schemeId, pageIndex, pageSize, startTime, endTime, gameCode, out totalCount);

        //    result.TotalCount = totalCount;
        //    result.TicketList = list;

        //    return result;
        //}

        ///// <summary>
        ///// 查询用户购彩统计
        ///// </summary>
        ///// <returns></returns>
        //public UserBetStatistics_Collection QueryUserBetStatistiscList()
        //{
        //    using (var manager = new SqlQueryManager())
        //    {
        //        return manager.QueryUserBetStatistiscList();
        //    }
        //}
        ///// <summary>
        ///// 查询代理返点明细
        ///// </summary>
        //public AgentRebateStatistics_Collection QueryAgentRebateStatisticsList(string viewType, string gs_AgentId, DateTime startTime, DateTime endTime)
        //{
        //    using (var manager = new SqlQueryManager())
        //    {
        //        return manager.QueryAgentRebateStatisticsList(viewType, gs_AgentId, startTime, endTime);
        //    }
        //}
        ///// <summary>
        ///// 查询状态为撤单和部分出票失败并且短信没有发送成功
        ///// </summary>
        ///// <returns></returns>
        //public SendMsgHistoryRecord_Collection QueryFailMsgList()
        //{
        //    using (var manager = new SqlQueryManager())
        //    {
        //        return manager.QueryFailMsgList();
        //    }
        //}

        ///// <summary>
        ///// 查询虚拟订单
        ///// </summary>
        //public BettingOrderInfoCollection QueryVirtualOrderList(string userIdOrName, SchemeType? schemeType, ProgressStatus? progressStatus, BonusStatus? bonusStatus, SchemeBettingCategory? betCategory, string gameCode
        //     , DateTime startTime, DateTime endTime, int pageIndex, int pageSize
        //     , TicketStatus? ticketStatus = null, SchemeSource? schemeSource = null)
        //{
        //    using (var manager = new SqlQueryManager())
        //    {
        //        var totalCount = 0;
        //        var totalUserCount = 0;
        //        var totalBuyMoney = 0M;
        //        var totalOrderMoney = 0M;
        //        var totalPreTaxBonusMoney = 0M;
        //        var totalAfterTaxBonusMoney = 0M;
        //        var totalAddMoney = 0M;
        //        BettingOrderInfoCollection collection = new BettingOrderInfoCollection();
        //        collection.OrderList = manager.QueryVirtualOrderList(userIdOrName, schemeType, progressStatus, bonusStatus, betCategory, gameCode, startTime, endTime, pageIndex, pageSize, out totalCount, out totalUserCount, out totalBuyMoney, out totalOrderMoney, out totalPreTaxBonusMoney, out totalAfterTaxBonusMoney, out totalAddMoney, ticketStatus, schemeSource);
        //        collection.TotalCount = totalCount;
        //        collection.TotalUserCount = totalUserCount;
        //        collection.TotalBuyMoney = totalBuyMoney;
        //        collection.TotalOrderMoney = totalOrderMoney;
        //        collection.TotalPreTaxBonusMoney = totalPreTaxBonusMoney;
        //        collection.TotalAfterTaxBonusMoney = totalAfterTaxBonusMoney;
        //        collection.TotalAddMoney = totalAddMoney;
        //        return collection;
        //    }
        //}

        //public int QueryTogetherFollowerCount(string createUserId)
        //{
        //    using (var manager = new SqlQueryManager())
        //    {
        //        return manager.QueryTogetherFollowerCount(createUserId);
        //    }
        //}

        ///// <summary>
        ///// 查询某个yqid下面的 能满足领红包条件的用户个数
        ///// </summary>
        ///// <param name="AgentId">普通用户代理 邀请注册的会员</param>
        ///// <returns></returns>
        //public string QueryYqidRegisterByAgentId(string AgentId)
        //{
        //    using (var manager = new SqlQueryManager())
        //    {
        //        return manager.QueryYqidRegisterByAgentId(AgentId);
        //    }
        //}
    }
}
