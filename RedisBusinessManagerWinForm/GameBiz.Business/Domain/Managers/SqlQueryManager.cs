using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Domain.Entities;
using Common.Database.NHibernate;
using Common.Business;
using GameBiz.Core;
using NHibernate.Linq;
using Common.Expansion;
using GameBiz.Business;
using Common.Database.ORM;
using Common.Utilities;
using Common;
using System.Data;

namespace GameBiz.Domain.Managers
{
    public class SqlQueryManager : GameBizEntityManagement
    {
        #region 订单查询

        public MyOrderListInfo QueryMyOrderDetailInfo(string schemeId)
        {
            Session.Clear();
            var query = from d in this.Session.Query<OrderDetail>()
                        where d.SchemeId == schemeId
                        select new MyOrderListInfo
                        {
                            AddMoney = d.AddMoney,
                            Amount = d.Amount,
                            AfterTaxBonusMoney = d.AfterTaxBonusMoney,
                            BetTime = d.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            BonusAwardsMoney = d.BonusAwardsMoney,
                            BonusStatus = d.BonusStatus,
                            GameCode = d.GameCode,
                            GameType = d.GameType,
                            GameTypeName = d.GameTypeName,
                            IssuseNumber = d.CurrentIssuseNumber,
                            PreTaxBonusMoney = d.PreTaxBonusMoney,
                            ProgressStatus = d.ProgressStatus,
                            RedBagAwardsMoney = d.RedBagAwardsMoney,
                            SchemeBettingCategory = d.SchemeBettingCategory,
                            SchemeId = d.SchemeId,
                            SchemeSource = d.SchemeSource,
                            SchemeType = d.SchemeType,
                            TicketStatus = d.TicketStatus,
                            TotalMoney = d.TotalMoney,
                            StopAfterBonus = d.StopAfterBonus,
                        };
            return query.FirstOrDefault();
        }

        public List<MyOrderListInfo> QueryMyOrderListInfo(string userId, string gameCode, BonusStatus? bonusStatus, SchemeType? schemeType,
            DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from d in this.Session.Query<OrderDetail>()
                        where d.UserId == userId
                        && (gameCode == string.Empty || d.GameCode == gameCode)
                        && (bonusStatus == null || d.BonusStatus == bonusStatus)
                        && (schemeType == null || d.SchemeType == schemeType)
                        && (d.CreateTime >= startTime && d.CreateTime < endTime)
                        select new MyOrderListInfo
                        {
                            AddMoney = d.AddMoney,
                            Amount = d.Amount,
                            AfterTaxBonusMoney = d.AfterTaxBonusMoney,
                            BetTime = d.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            BonusAwardsMoney = d.BonusAwardsMoney,
                            BonusStatus = d.BonusStatus,
                            GameCode = d.GameCode,
                            GameType = d.GameType,
                            GameTypeName = d.GameTypeName,
                            IssuseNumber = d.CurrentIssuseNumber,
                            PreTaxBonusMoney = d.PreTaxBonusMoney,
                            ProgressStatus = d.ProgressStatus,
                            RedBagAwardsMoney = d.RedBagAwardsMoney,
                            SchemeBettingCategory = d.SchemeBettingCategory,
                            SchemeId = d.SchemeId,
                            SchemeSource = d.SchemeSource,
                            SchemeType = d.SchemeType,
                            TicketStatus = d.TicketStatus,
                            TotalMoney = d.TotalMoney,
                        };
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public IList<MyBettingOrderInfo> QueryMyBettingOrderList(string userId, BonusStatus? bonusStatus, string gameCode
            , DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize
            , out int totalCount, out decimal totalBuyMoney, out decimal totalBonusMoney)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            // 通过数据库存储过程进行查询
            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Order_QueryMyOrderList"))
                .AddInParameter("userId", userId)
                .AddInParameter("bonusStatus", bonusStatus.HasValue ? (int)bonusStatus.Value : -1)
                .AddInParameter("gameCode", gameCode)
                .AddInParameter("fromDate", startTime.HasValue ? startTime.Value.ToString("yyyy-MM-dd") : "")
                .AddInParameter("toDate", endTime.HasValue ? endTime.Value.AddDays(1).ToString("yyyy-MM-dd") : "")
                .AddInParameter("pageIndex", pageIndex)
                .AddInParameter("pageSize", pageSize)
                .AddOutParameter("totalCount", "Int32")
                .AddOutParameter("totalBuyMoney", "Decimal")
                .AddOutParameter("totalBonusMoney", "Decimal");

            var dt = query.GetDataTable(out outputs);
            totalCount = UsefullHelper.GetDbValue<int>(outputs["totalCount"]);
            totalBuyMoney = UsefullHelper.GetDbValue<decimal>(outputs["totalBuyMoney"]);
            totalBonusMoney = UsefullHelper.GetDbValue<decimal>(outputs["totalBonusMoney"]);

            return ORMHelper.DataTableToList<MyBettingOrderInfo>(dt);
        }

        public IList<BettingOrderInfo> QueryMyChaseOrderList(string userId, string gameCode, DateTime startTime, DateTime endTime, int pageIndex, int pageSize
            , out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            // 通过数据库存储过程进行查询
            Dictionary<string, object> outputs = new Dictionary<string, object>();
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Order_QueryMyChaseOrderList"))
                .AddInParameter("userId", userId)
                .AddInParameter("gameCode", gameCode)
                .AddInParameter("fromDate", startTime.ToString("yyyy-MM-dd"))
                .AddInParameter("toDate", endTime.AddDays(1).ToString("yyyy-MM-dd"))
                .AddInParameter("pageIndex", pageIndex)
                .AddInParameter("pageSize", pageSize);

            var ds = query.GetDataSet();
            if (ds.Tables[0].Rows.Count == 0)
            {
                totalCount = 0;
            }
            else
            {
                totalCount = UsefullHelper.GetDbValue<int>(ds.Tables[0].Rows[0]["TotalCount"]);
            }
            return ORMHelper.DataTableToList<BettingOrderInfo>(ds.Tables[1]);
        }

        public IList<BettingOrderInfo> QueryBettingOrderList(string userIdOrName, SchemeType? schemeType, ProgressStatus? progressStatus, BonusStatus? bonusStatus, SchemeBettingCategory? betCategory, bool? isVirtual, string gameCode
             , DateTime startTime, DateTime endTime, int sortType, string agentId, int pageIndex, int pageSize
            ,string fieldName , out int totalCount, out int totalUserCount, out decimal totalBuyMoney, out decimal totalPreTaxBonusMoney,
            out decimal totalAfterTaxBonusMoney, out decimal totalAddMoney, out decimal totalRedbagMoney, out decimal totalRealPayRebateMoney,
            out decimal totalBonusAwardsMoney, out decimal totalRedBagAwardsMoney, TicketStatus? ticketStatus = null, SchemeSource? schemeSource = null)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            // 通过数据库存储过程进行查询
            Dictionary<string, object> outputs = new Dictionary<string, object>();
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Order_QueryOrderList"))
                .AddInParameter("userId", userIdOrName)
                .AddInParameter("schemeType", schemeType.HasValue ? (int)schemeType.Value : -1)
                .AddInParameter("progressStatus", progressStatus.HasValue ? (int)progressStatus.Value : -1)
                .AddInParameter("betCategory", betCategory.HasValue ? (int)betCategory.Value : -1)
                .AddInParameter("bonusStatus", bonusStatus.HasValue ? (int)bonusStatus.Value : -1)
                .AddInParameter("isVirtual", isVirtual.HasValue ? (isVirtual.Value ? 1 : 0) : -1)
                .AddInParameter("schemeSource", schemeSource.HasValue ? (int)schemeSource.Value : -1)
                .AddInParameter("gameCode", gameCode)
                .AddInParameter("fromDate", startTime.Date.ToString("yyyy-MM-dd HH:mm:ss"))
                .AddInParameter("toDate", endTime.Date.ToString("yyyy-MM-dd HH:mm:ss"))
                .AddInParameter("sortType", sortType)
                .AddInParameter("agentId", agentId)
                .AddInParameter("pageIndex", pageIndex)
                .AddInParameter("pageSize", pageSize)
                .AddInParameter("fieldName", fieldName);
            totalUserCount = 0;
            totalCount = 0;
            totalBuyMoney = 0M;
            totalRealPayRebateMoney = 0M;
            totalPreTaxBonusMoney = 0M;
            totalAfterTaxBonusMoney = 0M;
            totalAddMoney = 0M;
            totalRedbagMoney = 0M;
            totalBonusAwardsMoney = 0M;
            totalRedBagAwardsMoney = 0M;

            //var ds = query.GetDataSet2("P_Order_QueryOrderList");
            var ds = query.GetDataSet();
            if (ds.Tables.Count == 2)
            {
                // 0  汇总
                var summaryTable = ds.Tables[0];
                if (summaryTable.Rows.Count > 0)
                {
                    var row = summaryTable.Rows[0];
                    totalUserCount = UsefullHelper.GetDbValue<int>(row[0]);
                    totalCount = UsefullHelper.GetDbValue<int>(row[1]);
                    totalBuyMoney = UsefullHelper.GetDbValue<decimal>(row[2]);
                    totalRealPayRebateMoney = UsefullHelper.GetDbValue<decimal>(row[3]);
                    totalPreTaxBonusMoney = UsefullHelper.GetDbValue<decimal>(row[4]);
                    totalAfterTaxBonusMoney = UsefullHelper.GetDbValue<decimal>(row[5]);
                    totalAddMoney = UsefullHelper.GetDbValue<decimal>(row[6]);
                    totalRedbagMoney = UsefullHelper.GetDbValue<decimal>(row[7]);
                    totalBonusAwardsMoney = UsefullHelper.GetDbValue<decimal>(row[8]);
                    totalRedBagAwardsMoney = UsefullHelper.GetDbValue<decimal>(row[9]);
                }

                // 1 明细
                var detailTable = ds.Tables[1];
                return ORMHelper.DataTableToList<BettingOrderInfo>(detailTable);
            }
            return new List<BettingOrderInfo>();


            //var dt = query.GetDataTable(out outputs);            

            //totalRealPayRebateMoney = UsefullHelper.GetDbValue<decimal>(outputs["totalRealPayRebateMoney"]);
            //totalRedbagMoney = UsefullHelper.GetDbValue<decimal>(outputs["totalRedbagMoney"]);
            //totalCount = UsefullHelper.GetDbValue<int>(outputs["totalCount"]);
            //totalUserCount = UsefullHelper.GetDbValue<int>(outputs["totalUserCount"]);
            //totalBuyMoney = UsefullHelper.GetDbValue<decimal>(outputs["totalBetMoney"]);
            ////totalOrderMoney = UsefullHelper.GetDbValue<decimal>(outputs["totalOrderMoney"]);
            //totalPreTaxBonusMoney = UsefullHelper.GetDbValue<decimal>(outputs["totalPreTaxBonusMoney"]);
            //totalAfterTaxBonusMoney = UsefullHelper.GetDbValue<decimal>(outputs["totalAfterTaxBonusMoney"]);
            //totalAddMoney = UsefullHelper.GetDbValue<decimal>(outputs["totalAddMoney"]);
            //totalBonusAwardsMoney = UsefullHelper.GetDbValue<decimal>(outputs["totalBonusAwardsMoney"]);
            //totalRedBagAwardsMoney = UsefullHelper.GetDbValue<decimal>(outputs["totalRedBagAwardsMoney"]);

            //return ORMHelper.DataTableToList<BettingOrderInfo>(dt);
        }



        public IList<BettingOrderInfo> QueryBettingOrderListByChaseKeyLine(string keyLine)
        {
            Session.Clear();
            // 通过数据库存储过程进行查询
            Dictionary<string, object> outputs = new Dictionary<string, object>();
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Order_QueryOrderListByChaseKeyLine"))
                .AddInParameter("keyLine", keyLine);

            var dt = query.GetDataTable();
            return ORMHelper.DataTableToList<BettingOrderInfo>(dt);
        }
        public IList<BettingAnteCodeInfo> QueryAnteCodeListBySchemeId(string schemeId)
        {
            Session.Clear();
            // 通过数据库存储过程进行查询
            Dictionary<string, object> outputs = new Dictionary<string, object>();
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Order_QueryAnteCodeListBySchemeId"))
                .AddInParameter("schemeId", schemeId);

            var dt = query.GetDataTable();
            return ORMHelper.DataTableToList<BettingAnteCodeInfo>(dt);
        }
        public IList<TogetherOrderInfo> QueryCreateTogetherOrderListByUserId(string userId, BonusStatus? bonus, string gameCode, DateTime dateFrom, DateTime dateTo, int pageIndex, int pageSize, out int totalCount, out decimal totalBuyMoney, out decimal totalOrderMoney)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            // 通过数据库存储过程进行查询
            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Order_QueryCreateTogetherSchemeListByUserId"))
                .AddInParameter("userId", userId)
                .AddInParameter("bonusStatus", bonus.HasValue ? (int)bonus.Value : -1)
                .AddInParameter("gameCode", gameCode)
                .AddInParameter("dateFrom", dateFrom.ToString("yyyy-MM-dd"))
                .AddInParameter("dateTo", dateTo.AddDays(1).ToString("yyyy-MM-dd"))
                .AddInParameter("pageIndex", pageIndex)
                .AddInParameter("pageSize", pageSize)
                .AddOutParameter("totalCount", "Int32")
                .AddOutParameter("totalBuyMoney", "Decimal")
                .AddOutParameter("totalOrderMoney", "Decimal");
            var dt = query.GetDataTable(out outputs);

            totalCount = UsefullHelper.GetDbValue<int>(outputs["totalCount"]);
            totalBuyMoney = UsefullHelper.GetDbValue<decimal>(outputs["totalBuyMoney"]);
            totalOrderMoney = UsefullHelper.GetDbValue<decimal>(outputs["totalOrderMoney"]);

            return ORMHelper.DataTableToList<TogetherOrderInfo>(dt);
        }
        public IList<TogetherOrderInfo> QueryJoinTogetherOrderListByUserId(string userId, BonusStatus? bonus, string gameCode, DateTime dateFrom, DateTime dateTo, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            // 通过数据库存储过程进行查询
            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Order_QueryJoinTogetherSchemeListByUserId"))
                .AddInParameter("userId", userId)
                .AddInParameter("bonusStatus", bonus.HasValue ? (int)bonus.Value : -1)
                .AddInParameter("gameCode", gameCode)
                .AddInParameter("dateFrom", dateFrom.ToString("yyyy-MM-dd"))
                .AddInParameter("dateTo", dateTo.AddDays(1).ToString("yyyy-MM-dd"))
                .AddInParameter("pageIndex", pageIndex)
                .AddInParameter("pageSize", pageSize)
                .AddOutParameter("totalCount", "Int32");
            var dt = query.GetDataTable(out outputs);

            totalCount = UsefullHelper.GetDbValue<int>(outputs["totalCount"]);

            return ORMHelper.DataTableToList<TogetherOrderInfo>(dt);
        }
        public IList<TogetherReportInfoGroupByUserInfo> QueryCreateTogetherReportGroupByUser(string userIdList, DateTime dateFrom, DateTime dateTo)
        {
            Session.Clear();
            // 通过数据库存储过程进行查询
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Order_QueryCreateTogetherReportGroupByUser"))
                .AddInParameter("userIdList", userIdList)
                .AddInParameter("dateFrom", dateFrom.ToString("yyyy-MM-dd"))
                .AddInParameter("dateTo", dateTo.AddDays(1).ToString("yyyy-MM-dd"));
            var dt = query.GetDataTable();

            return ORMHelper.DataTableToList<TogetherReportInfoGroupByUserInfo>(dt);
        }

        #endregion

        #region 充值提现

        public IList<FillMoneyQueryInfo> QueryFillMoneyList(string userId, string agentTypeList, string statusList, string sourceList, DateTime startTime, DateTime endTime
            , int pageIndex, int pageSize, string orderId, out int totalCount, out decimal totalRequestMoney, out decimal totalResponseMoney)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            if (pageSize == -1)
                pageSize = int.MaxValue;
            else
                pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            // 通过数据库存储过程进行查询
            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Fund_QueryFillMoneyList"))
                .AddInParameter("userId", userId)
                .AddInParameter("agentList", agentTypeList)
                .AddInParameter("statusList", statusList)
                .AddInParameter("sourceList", sourceList)
                .AddInParameter("startTime", startTime.ToString("yyyy-MM-dd"))
                .AddInParameter("endTime", endTime.AddDays(1).ToString("yyyy-MM-dd"))
                .AddInParameter("pageIndex", pageIndex)
                .AddInParameter("pageSize", pageSize)
                .AddInParameter("orderId", orderId)
                .AddOutParameter("totalCount", "Int32")
                .AddOutParameter("totalRequestMoney", "Decimal")//Decimal
                .AddOutParameter("totalResponseMoney", "Decimal");
            var dataset = query.GetDataSet(out outputs);
            totalCount = UsefullHelper.GetDbValue<int>(outputs["totalCount"]);
            totalRequestMoney = UsefullHelper.GetDbValue<decimal>(outputs["totalRequestMoney"]);
            totalResponseMoney = UsefullHelper.GetDbValue<decimal>(outputs["totalResponseMoney"]);
            if (dataset != null && dataset.Tables.Count >= 1)
            {
                //totalRequestMoney = UsefullHelper.GetDbValue<decimal>(dataset.Tables[0].Rows[0][0]);
                //totalResponseMoney = UsefullHelper.GetDbValue<decimal>(dataset.Tables[0].Rows[0][1]);
                return ORMHelper.DataTableToList<FillMoneyQueryInfo>(dataset.Tables[0]);
            }
            else
            {
                totalRequestMoney = 0;
                totalResponseMoney = 0;
                return new List<FillMoneyQueryInfo>();
            }

            //var dt = query.GetDataTable(out outputs);
            //totalCount = UsefullHelper.GetDbValue<int>(outputs["totalCount"]);
            //totalRequestMoney = UsefullHelper.GetDbValue<decimal>(outputs["totalRequestMoney"]);
            //totalResponseMoney = UsefullHelper.GetDbValue<decimal>(outputs["totalResponseMoney"]);
            //return ORMHelper.DataTableToList<FillMoneyQueryInfo>(dt);
        }
        /// </summary>
        /// 查询充值专员给用户的充值记录记录
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="agentTypeList"></param>
        /// <param name="statusList"></param>
        /// <param name="sourceList"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderId"></param>
        /// <param name="totalCount"></param>
        /// <param name="totalRequestMoney"></param>
        /// <param name="totalResponseMoney"></param>
        /// <returns></returns>
        public IList<FillMoneyQueryInfo> QueryMyFillMoneyListByCzzy(string userId, string agentTypeList, string statusList, string sourceList, DateTime startTime, DateTime endTime
           , int pageIndex, int pageSize, string orderId, out int totalCount, out decimal totalRequestMoney, out decimal totalResponseMoney)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            if (pageSize == -1)
                pageSize = int.MaxValue;
            else
                pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            // 通过数据库存储过程进行查询
            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Fund_QueryFillMoneyListByCzzy"))
                .AddInParameter("userId", userId)
                .AddInParameter("agentList", agentTypeList)
                .AddInParameter("statusList", statusList)
                .AddInParameter("sourceList", sourceList)
                .AddInParameter("startTime", startTime.ToString("yyyy-MM-dd"))
                .AddInParameter("endTime", endTime.AddDays(1).ToString("yyyy-MM-dd"))
                .AddInParameter("pageIndex", pageIndex)
                .AddInParameter("pageSize", pageSize)
                .AddInParameter("orderId", orderId)
                .AddOutParameter("totalCount", "Int32")
                .AddOutParameter("totalRequestMoney", "Decimal")//Decimal
                .AddOutParameter("totalResponseMoney", "Decimal");
            var dataset = query.GetDataSet(out outputs);
            totalCount = UsefullHelper.GetDbValue<int>(outputs["totalCount"]);
            totalRequestMoney = UsefullHelper.GetDbValue<decimal>(outputs["totalRequestMoney"]);
            totalResponseMoney = UsefullHelper.GetDbValue<decimal>(outputs["totalResponseMoney"]);
            if (dataset != null && dataset.Tables.Count >= 1)
            {
                return ORMHelper.DataTableToList<FillMoneyQueryInfo>(dataset.Tables[0]);
            }
            else
            {
                totalRequestMoney = 0;
                totalResponseMoney = 0;
                return new List<FillMoneyQueryInfo>();
            }
        }

        public List<Withdraw_QueryInfo> QueryWithdrawList(string userId, WithdrawAgentType? agent, WithdrawStatus? status, decimal minMoney, decimal maxMoney, DateTime startTime, DateTime endTime, int sortType, int pageIndex, int pageSize, string orderId,
            out int winCount, out int refusedCount, out decimal totalWinMoney, out decimal totalRefusedMoney, out decimal totalResponseMoney, out int totalCount, out decimal totalMoney)
        {
            Session.Clear();
            endTime = endTime.AddDays(1).Date;
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;

            var query = from r in this.Session.Query<Withdraw>()
                        join u in this.Session.Query<UserRegister>() on r.UserId equals u.UserId
                        where (userId == string.Empty || r.UserId == userId)
                        && r.RequestTime >= startTime && r.RequestTime < endTime
                        && (status == null || r.Status == status)
                        && (orderId == string.Empty || r.BankCode == orderId)
                        && (agent == null || r.WithdrawAgent == agent)
                        && (minMoney == -1 || r.RequestMoney >= minMoney)
                        && (maxMoney == -1 || r.RequestMoney <= maxMoney)
                        select new Withdraw_QueryInfo
                        {
                            BankCardNumber = r.BankCardNumber,
                            BankCode = r.BankCode,
                            BankName = r.BankName,
                            BankSubName = r.BankSubName,
                            CityName = r.CityName,
                            OrderId = r.OrderId,
                            ProvinceName = r.ProvinceName,
                            RequestMoney = r.RequestMoney,
                            RequestTime = r.RequestTime,
                            ResponseTime = r.ResponseTime,
                            ResponseMoney = r.ResponseMoney,
                            WithdrawAgent = r.WithdrawAgent,
                            Status = r.Status,
                            ResponseMessage = r.ResponseMessage,
                            RequesterDisplayName = u.DisplayName,
                            RequesterUserKey = u.UserId,
                        };
            winCount = query.Where(p => p.Status == WithdrawStatus.Success).Count();
            refusedCount = query.Where(p => p.Status == WithdrawStatus.Refused).Count();

            totalWinMoney = winCount == 0 ? 0M : query.Where(p => p.Status == WithdrawStatus.Success).Sum(p => p.RequestMoney);
            totalRefusedMoney = refusedCount == 0 ? 0M : query.Where(p => p.Status == WithdrawStatus.Refused).Sum(p => p.RequestMoney);
            totalCount = query.Count();
            totalMoney = query.Count() == 0 ? 0M : query.Sum(p => p.RequestMoney);
            totalResponseMoney = winCount == 0 ? 0M : query.Where(p => p.ResponseMoney.HasValue == true).Sum(p => p.ResponseMoney.Value);

            if (sortType == -1)
                query = query.OrderBy(p => p.RequestTime);
            if (sortType == 0)
                query = query.OrderBy(p => p.RequestMoney);
            if (sortType == 1)
                query = query.OrderByDescending(p => p.RequestMoney);

            if (pageSize == -1)
                return query.ToList();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
        public Withdraw_QueryInfo GetWithdrawById(string orderId)
        {
            Session.Clear();
            // 通过数据库存储过程进行查询
            var result = Session.CreateSQLQuery("EXEC [P_Fund_QueryWithdrawById] @OrderId=:OrderId")
                .SetString("OrderId", orderId)
                .UniqueResult() as object[];

            if (result == null) return null;

            var info = new Withdraw_QueryInfo();
            info.LoadArray(result);
            return info;
        }

        #endregion

        #region 查询用户资金明细

        public IList<FundDetailInfo> QueryUserFundDetail(string userId, string keyLine, DateTime fromDate, DateTime toDate, string accountTypeList, string categoryList
             , int pageIndex, int pageSize, out int totalPayinCount, out decimal totalPayinMoney, out int totalPayoutCount, out decimal totalPayoutMoney)
        {
            Session.Clear();
            userId = string.IsNullOrEmpty(userId) ? string.Empty : userId;
            keyLine = string.IsNullOrEmpty(keyLine) ? string.Empty : keyLine;
            accountTypeList = string.IsNullOrEmpty(accountTypeList) ? string.Empty : accountTypeList;
            categoryList = string.IsNullOrEmpty(categoryList) ? string.Empty : categoryList;
            toDate = toDate.AddDays(1).Date;
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            if (pageSize < 10000)
                pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;


            // 通过数据库存储过程进行查询
            Dictionary<string, object> outputs = new Dictionary<string, object>();
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Fund_QueryMyFundDetail"))
               .AddInParameter("UserId", userId)
               .AddInParameter("KeyLine", keyLine)
               .AddInParameter("StartTime", fromDate)
               .AddInParameter("EndTime", toDate)
               .AddInParameter("AccountList", accountTypeList)
               .AddInParameter("CategoryList", categoryList)              
               .AddInParameter("PageIndex", pageIndex)
               .AddInParameter("PageSize", pageSize);
            var ds = query.GetDataSet();
            IList<FundDetailInfo> result = new List<FundDetailInfo>();
            totalPayinCount = 0;
            totalPayinMoney = 0M;
            totalPayoutCount = 0;
            totalPayoutMoney = 0M;
            if (ds.Tables.Count == 3)
            {
                result = Common.Database.ORM.ORMHelper.DataTableToInfoList<FundDetailInfo>(ds.Tables[0]);
                var payInTable = ds.Tables[1];
                if (payInTable.Rows.Count > 0)
                {
                    totalPayinCount = int.Parse(payInTable.Rows[0][0].ToString());
                    totalPayinMoney = decimal.Parse(payInTable.Rows[0][1].ToString());
                }
                var payOutTable = ds.Tables[2];
                if (payOutTable.Rows.Count > 0)
                {
                    totalPayoutCount = int.Parse(payOutTable.Rows[0][0].ToString());
                    totalPayoutMoney = decimal.Parse(payOutTable.Rows[0][1].ToString());
                }
            }
            return result;
            //result.TicketList = Common.Database.ORM.ORMHelper.DataTableToInfoList<Sports_TicketQueryInfo>(ds.Tables[0]);
            //result.MatchList = Common.Database.ORM.ORMHelper.DataTableToInfoList<MatchInfo>(ds.Tables[1]);
            //result.TotalTicketCount = result.TicketList.Count;
            //result.TotalMatchCount = result.MatchList.Count;



            //var query = from r in this.Session.Query<FundDetail>()
            //            where (string.IsNullOrEmpty(userId) || r.UserId == userId)
            //            && (string.IsNullOrEmpty(keyLine) || r.KeyLine == keyLine)
            //            && r.CreateTime >= fromDate && r.CreateTime < toDate
            //            && (accountTypeList.Count == 0 || accountTypeList.Contains(r.AccountType))
            //            && (categoryList.Count == 0 || categoryList.Contains(r.Category))
            //            && r.Category != BusinessHelper.FundCategory_IntegralRequestWithdraw
            //            && r.Category != BusinessHelper.FundCategory_IntegralCompleteWithdraw
            //            select new FundDetailInfo
            //            {
            //                Id = r.Id,
            //                UserId = r.UserId,
            //                AccountType = r.AccountType,
            //                AfterBalance = r.AfterBalance,
            //                BeforeBalance = r.BeforeBalance,
            //                Category = r.Category,
            //                KeyLine = r.KeyLine,
            //                OperatorId = r.OperatorId,
            //                OrderId = r.OrderId,
            //                PayMoney = r.PayMoney,
            //                PayType = r.PayType,
            //                Summary = r.Summary,
            //                CreateTime = r.CreateTime,
            //            };

            //totalPayinCount = query.Where(p => p.PayType == PayType.Payin).Count();
            //totalPayoutCount = query.Where(p => p.PayType == PayType.Payout).Count();

            //totalPayinMoney = totalPayinCount == 0 ? 0M : query.Where(p => p.PayType == PayType.Payin).Sum(p => p.PayMoney);
            //totalPayoutMoney = totalPayoutCount == 0 ? 0M : query.Where(p => p.PayType == PayType.Payout).Sum(p => p.PayMoney);

            //if (pageSize < 10000)
            //    return query.OrderByDescending(p => p.Id).Skip(pageIndex * pageSize).Take(pageSize).ToList();
            //else
            //    return query.OrderByDescending(p => p.Id).ToList();
        }


        /// <summary>
        /// 资金报表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="keyLine"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="accountTypeList"></param>
        /// <param name="categoryList"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalPayinCount"></param>
        /// <param name="totalPayinMoney"></param>
        /// <param name="totalPayoutCount"></param>
        /// <param name="totalPayoutMoney"></param>
        /// <returns></returns>
        public IList<FundDetailInfo> QueryUserFundDetailListReport(string userId, string keyLine, DateTime fromDate, DateTime toDate, string accountTypeList, string categoryList
             , int pageIndex, int pageSize, out int totalPayinCount, out decimal totalPayinMoney, out int totalPayoutCount, out decimal totalPayoutMoney)
        {
            Session.Clear();
            userId = string.IsNullOrEmpty(userId) ? string.Empty : userId;
            keyLine = string.IsNullOrEmpty(keyLine) ? string.Empty : keyLine;
            accountTypeList = string.IsNullOrEmpty(accountTypeList) ? string.Empty : accountTypeList;
            categoryList = string.IsNullOrEmpty(categoryList) ? string.Empty : categoryList;
            toDate = toDate.AddDays(1).Date;
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            if (pageSize < 10000)
                pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;


            // 通过数据库存储过程进行查询
            Dictionary<string, object> outputs = new Dictionary<string, object>();
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Fund_QueryMyFundDetail_Report"))
               .AddInParameter("UserId", userId)
               .AddInParameter("KeyLine", keyLine)
               .AddInParameter("StartTime", fromDate)
               .AddInParameter("EndTime", toDate)
               .AddInParameter("AccountList", accountTypeList)
               .AddInParameter("CategoryList", categoryList)
               .AddInParameter("PageIndex", pageIndex)
               .AddInParameter("PageSize", pageSize);
            var ds = query.GetDataSet();
            IList<FundDetailInfo> result = new List<FundDetailInfo>();
            totalPayinCount = 0;
            totalPayinMoney = 0M;
            totalPayoutCount = 0;
            totalPayoutMoney = 0M;
            if (ds.Tables.Count == 3)
            {
                result = Common.Database.ORM.ORMHelper.DataTableToInfoList<FundDetailInfo>(ds.Tables[0]);
                var payInTable = ds.Tables[1];
                if (payInTable.Rows.Count > 0)
                {
                    totalPayinCount = int.Parse(payInTable.Rows[0][0].ToString());
                    totalPayinMoney = decimal.Parse(payInTable.Rows[0][1].ToString());
                }
                var payOutTable = ds.Tables[2];
                if (payOutTable.Rows.Count > 0)
                {
                    totalPayoutCount = int.Parse(payOutTable.Rows[0][0].ToString());
                    totalPayoutMoney = decimal.Parse(payOutTable.Rows[0][1].ToString());
                }
            }
            return result;
            //result.TicketList = Common.Database.ORM.ORMHelper.DataTableToInfoList<Sports_TicketQueryInfo>(ds.Tables[0]);
            //result.MatchList = Common.Database.ORM.ORMHelper.DataTableToInfoList<MatchInfo>(ds.Tables[1]);
            //result.TotalTicketCount = result.TicketList.Count;
            //result.TotalMatchCount = result.MatchList.Count;



            //var query = from r in this.Session.Query<FundDetail>()
            //            where (string.IsNullOrEmpty(userId) || r.UserId == userId)
            //            && (string.IsNullOrEmpty(keyLine) || r.KeyLine == keyLine)
            //            && r.CreateTime >= fromDate && r.CreateTime < toDate
            //            && (accountTypeList.Count == 0 || accountTypeList.Contains(r.AccountType))
            //            && (categoryList.Count == 0 || categoryList.Contains(r.Category))
            //            && r.Category != BusinessHelper.FundCategory_IntegralRequestWithdraw
            //            && r.Category != BusinessHelper.FundCategory_IntegralCompleteWithdraw
            //            select new FundDetailInfo
            //            {
            //                Id = r.Id,
            //                UserId = r.UserId,
            //                AccountType = r.AccountType,
            //                AfterBalance = r.AfterBalance,
            //                BeforeBalance = r.BeforeBalance,
            //                Category = r.Category,
            //                KeyLine = r.KeyLine,
            //                OperatorId = r.OperatorId,
            //                OrderId = r.OrderId,
            //                PayMoney = r.PayMoney,
            //                PayType = r.PayType,
            //                Summary = r.Summary,
            //                CreateTime = r.CreateTime,
            //            };

            //totalPayinCount = query.Where(p => p.PayType == PayType.Payin).Count();
            //totalPayoutCount = query.Where(p => p.PayType == PayType.Payout).Count();

            //totalPayinMoney = totalPayinCount == 0 ? 0M : query.Where(p => p.PayType == PayType.Payin).Sum(p => p.PayMoney);
            //totalPayoutMoney = totalPayoutCount == 0 ? 0M : query.Where(p => p.PayType == PayType.Payout).Sum(p => p.PayMoney);

            //if (pageSize < 10000)
            //    return query.OrderByDescending(p => p.Id).Skip(pageIndex * pageSize).Take(pageSize).ToList();
            //else
            //    return query.OrderByDescending(p => p.Id).ToList();
        }


        public IList<FundDetailInfo> QueryUserFundDetail_CPS(string userId, string keyLine, DateTime fromDate, DateTime toDate, string accountTypeList
            , int pageIndex, int pageSize, out int totalPayinCount, out decimal totalPayinMoney, out int totalPayoutCount, out decimal totalPayoutMoney)
        {
            Session.Clear();
            userId = string.IsNullOrEmpty(userId) ? string.Empty : userId;
            keyLine = string.IsNullOrEmpty(keyLine) ? string.Empty : keyLine;
            accountTypeList = string.IsNullOrEmpty(accountTypeList) ? string.Empty : accountTypeList;
            toDate = toDate.AddDays(1).Date;
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            if (pageSize < 10000)
                pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;


            // 通过数据库存储过程进行查询
            Dictionary<string, object> outputs = new Dictionary<string, object>();
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Fund_QueryMyFundDetail_CPS"))
               .AddInParameter("UserId", userId)
               .AddInParameter("KeyLine", keyLine)
               .AddInParameter("StartTime", fromDate)
               .AddInParameter("EndTime", toDate)
               .AddInParameter("AccountList", accountTypeList)
               .AddInParameter("PageIndex", pageIndex)
               .AddInParameter("PageSize", pageSize);
            var ds = query.GetDataSet();
            IList<FundDetailInfo> result = new List<FundDetailInfo>();
            totalPayinCount = 0;
            totalPayinMoney = 0M;
            totalPayoutCount = 0;
            totalPayoutMoney = 0M;
            if (ds.Tables.Count == 3)
            {
                result = Common.Database.ORM.ORMHelper.DataTableToInfoList<FundDetailInfo>(ds.Tables[0]);
                var payInTable = ds.Tables[1];
                if (payInTable.Rows.Count > 0)
                {
                    totalPayinCount = int.Parse(payInTable.Rows[0][0].ToString());
                    totalPayinMoney = decimal.Parse(payInTable.Rows[0][1].ToString());
                }
                var payOutTable = ds.Tables[2];
                if (payOutTable.Rows.Count > 0)
                {
                    totalPayoutCount = int.Parse(payOutTable.Rows[0][0].ToString());
                    totalPayoutMoney = decimal.Parse(payOutTable.Rows[0][1].ToString());
                }
            }
            return result;
        }

        public List<FundDetailInfo> QueryUserFundDetail_ContainCommission(string userId, DateTime fromDate, DateTime toDate, List<AccountType> accountTypeList, List<string> categoryList
            , int pageIndex, int pageSize, out int totalPayinCount, out decimal totalPayinMoney, out int totalPayoutCount, out decimal totalPayoutMoney)
        {
            Session.Clear();
            toDate = toDate.AddDays(1);
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            if (pageSize < 10000)
                pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from r in this.Session.Query<FundDetail>()
                        where (string.IsNullOrEmpty(userId) || r.UserId == userId)
                        && r.CreateTime >= fromDate && r.CreateTime < toDate
                        && (accountTypeList.Count == 0 || accountTypeList.Contains(r.AccountType))
                        && (categoryList.Count == 0 || categoryList.Contains(r.Category))
                        && (r.Category != BusinessHelper.FundCategory_IntegralRequestWithdraw && r.Category != BusinessHelper.FundCategory_IntegralSchemeDeduct && r.Category != BusinessHelper.FundCategory_IntegralCompleteWithdraw && r.Category != BusinessHelper.FundCategory_IntegralRefusedWithdraw && r.Category != BusinessHelper.FundCategory_IntegralManualDeductMoney && r.Category != BusinessHelper.FundCategory_IntegralManualRemitMoney)
                        select new FundDetailInfo
                        {
                            Id = r.Id,
                            UserId = r.UserId,
                            AccountType = r.AccountType,
                            AfterBalance = r.AfterBalance,
                            BeforeBalance = r.BeforeBalance,
                            Category = r.Category,
                            KeyLine = r.KeyLine,
                            OperatorId = r.OperatorId,
                            OrderId = r.OrderId,
                            PayMoney = r.PayMoney,
                            PayType = r.PayType,
                            Summary = r.Summary,
                            CreateTime = r.CreateTime,
                        };
            totalPayinCount = query.Where(p => p.PayType == PayType.Payin).Count();
            totalPayoutCount = query.Where(p => p.PayType == PayType.Payout).Count();

            totalPayinMoney = totalPayinCount == 0 ? 0M : query.Where(p => p.PayType == PayType.Payin).Sum(p => p.PayMoney);
            totalPayoutMoney = totalPayoutCount == 0 ? 0M : query.Where(p => p.PayType == PayType.Payout).Sum(p => p.PayMoney);

            //return query.OrderByDescending(p => p.CreateTime).Skip(pageIndex * pageSize).Take(pageSize).ToList();
            if (pageSize < 10000)
                return query.OrderByDescending(p => p.Id).Skip(pageIndex * pageSize).Take(pageSize).ToList();
            else
                return query.OrderByDescending(p => p.Id).ToList();
        }

        public List<FundDetailInfo> QueryUserFundDetail_Commission(string userId, DateTime fromDate, DateTime toDate, int pageIndex, int pageSize
            , out int totalPayinCount, out decimal totalPayinMoney)
        {
            Session.Clear();
            toDate = toDate.AddDays(1);
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            var query = from r in this.Session.Query<FundDetail>()
                        where (string.IsNullOrEmpty(userId) || r.UserId == userId)
                        && r.CreateTime >= fromDate && r.CreateTime < toDate
                        && r.PayType == PayType.Payin
                        && r.AccountType == AccountType.Commission
                        //&& (r.Category != BusinessHelper.FundCategory_IntegralRequestWithdraw && r.Category != BusinessHelper.FundCategory_IntegralSchemeDeduct 
                        //    && r.Category != BusinessHelper.FundCategory_IntegralCompleteWithdraw && r.Category != BusinessHelper.FundCategory_IntegralRefusedWithdraw 
                        //    && r.Category != BusinessHelper.FundCategory_IntegralManualDeductMoney && r.Category != BusinessHelper.FundCategory_IntegralManualRemitMoney)
                        //orderby r.CreateTime descending
                        select new FundDetailInfo
                        {
                            Id = r.Id,
                            UserId = r.UserId,
                            AccountType = r.AccountType,
                            AfterBalance = r.AfterBalance,
                            BeforeBalance = r.BeforeBalance,
                            Category = r.Category,
                            KeyLine = r.KeyLine,
                            OperatorId = r.OperatorId,
                            OrderId = r.OrderId,
                            PayMoney = r.PayMoney,
                            PayType = r.PayType,
                            Summary = r.Summary,
                            CreateTime = r.CreateTime,
                        };
            totalPayinCount = query.Count();
            if (totalPayinCount > 0)
            {
                totalPayinMoney = query.Sum(p => p.PayMoney);
                return query.OrderByDescending(p => p.CreateTime).Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            totalPayinMoney = 0M;
            return new List<FundDetailInfo>();
        }

        public List<FundDetailInfo> QueryFundDetailByDateTime(DateTime date)
        {
            Session.Clear();
            var query = from f in this.Session.Query<FundDetail>()
                        where f.CreateTime >= date && f.CreateTime < date.AddDays(1)
                        select new FundDetailInfo
                        {
                            AccountType = f.AccountType,
                            AfterBalance = f.AfterBalance,
                            BeforeBalance = f.BeforeBalance,
                            Category = f.Category,
                            CreateTime = f.CreateTime,
                            Id = f.Id,
                            KeyLine = f.KeyLine,
                            OperatorId = f.OperatorId,
                            OrderId = f.OrderId,
                            PayMoney = f.PayMoney,
                            PayType = f.PayType,
                            Summary = f.Summary,
                            UserId = f.UserId,
                        };
            return query.ToList();
        }

        public List<FundDetailInfo> QueryFundDetailByDateTime(DateTime date, string userId, int[] accountArray)
        {
            Session.Clear();
            var query = from f in this.Session.Query<FundDetail>()
                        where f.UserId == userId
                        && (f.CreateTime >= date && f.CreateTime < date.AddDays(1))
                        && (accountArray.Length == 0 || accountArray.Contains((int)f.AccountType))
                        select new FundDetailInfo
                        {
                            AccountType = f.AccountType,
                            AfterBalance = f.AfterBalance,
                            BeforeBalance = f.BeforeBalance,
                            Category = f.Category,
                            CreateTime = f.CreateTime,
                            Id = f.Id,
                            KeyLine = f.KeyLine,
                            OperatorId = f.OperatorId,
                            OrderId = f.OrderId,
                            PayMoney = f.PayMoney,
                            PayType = f.PayType,
                            Summary = f.Summary,
                            UserId = f.UserId,
                        };
            return query.ToList();
        }

        public List<OCDouDouDetailInfo> QueryUserOCDouDouDetail(string userId, DateTime fromDate, DateTime toDate, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from d in this.Session.Query<OCDouDouDetail>()
                        where d.UserId == userId
                        && (d.CreateTime >= fromDate && d.CreateTime < toDate)
                        select new OCDouDouDetailInfo
                        {
                            AfterBalance = d.AfterBalance,
                            BeforeBalance = d.BeforeBalance,
                            Category = d.Category,
                            CreateTime = d.CreateTime,
                            OrderId = d.OrderId,
                            PayMoney = d.PayMoney,
                            PayType = d.PayType,
                            Summary = d.Summary,
                            UserId = d.UserId
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<FundDetailInfo> QueryFundDetailInfo(string keyLine)
        {
            Session.Clear();
            var query = from r in this.Session.Query<FundDetail>()
                        where r.KeyLine == keyLine
                        select new FundDetailInfo
                        {
                            Id = r.Id,
                            UserId = r.UserId,
                            AccountType = r.AccountType,
                            AfterBalance = r.AfterBalance,
                            BeforeBalance = r.BeforeBalance,
                            Category = r.Category,
                            KeyLine = r.KeyLine,
                            OperatorId = r.OperatorId,
                            OrderId = r.OrderId,
                            PayMoney = r.PayMoney,
                            PayType = r.PayType,
                            Summary = r.Summary,
                            CreateTime = r.CreateTime,
                        };
            return query.ToList();
        }

        #endregion

        #region 中奖查询

        public IList<BonusOrderInfo> QueryBonusInfoList(string userId, string gameCode, string gameType, string issuseNumber, string completeData, string key, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            // 通过数据库存储过程进行查询
            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Order_QueryBonusOrderList"))
                .AddInParameter("userId", userId)
                .AddInParameter("gameCode", gameCode)
                .AddInParameter("gameType", gameType)
                .AddInParameter("issuseNumber", issuseNumber)
                .AddInParameter("completeData", completeData)
                .AddInParameter("key", key)
                .AddInParameter("pageIndex", pageIndex)
                .AddInParameter("pageSize", pageSize)
                .AddOutParameter("totalCount", "Int32");
            var dt = query.GetDataTable(out outputs);

            totalCount = (int)outputs["totalCount"];

            return ORMHelper.DataTableToList<BonusOrderInfo>(dt);
        }

        #endregion

        #region 排行榜查询

        #region 发单盈利排行榜 - 竞彩类

        public List<RankInfo_BettingProfit_Sport> QueryRankInfoList_BettingProfit_Sport(DateTime fromDate, DateTime toDate, string gameCode, string gameType, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;

            var dict = new Dictionary<string, object>();
            // 通过数据库存储过程进行查询
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Rank_QueryBettingProfitRank_Sport"))
                .AddInParameter("fromDate", fromDate)
                .AddInParameter("toDate", toDate)
                .AddInParameter("gameCode", gameCode)
                .AddInParameter("gameType", gameType)
                .AddInParameter("pageIndex", pageIndex)
                .AddInParameter("pageSize", pageSize)
                .AddOutParameter("totalCount", "Int32");
            var dt = query.GetDataTable(out dict);
            totalCount = UsefullHelper.GetDbValue<int>(dict["totalCount"]);

            var list = new List<RankInfo_BettingProfit_Sport>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new RankInfo_BettingProfit_Sport
                {
                    BonusMoney = row["BonusMoney"] == DBNull.Value ? 0 : decimal.Parse(row["BonusMoney"].ToString()),
                    GameCode = row["GameCode"] == DBNull.Value ? "" : row["GameCode"].ToString(),
                    GameType = row["GameType"] == DBNull.Value ? "" : row["GameType"].ToString(),
                    UserId = row["UserId"] == DBNull.Value ? "" : row["UserId"].ToString(),
                    UserHideDisplayNameCount = row["HideDisplayNameCount"] == DBNull.Value ? 0 : int.Parse(row["HideDisplayNameCount"].ToString()),
                    UserDisplayName = row["DisplayName"] == DBNull.Value ? "" : row["DisplayName"].ToString(),
                    TotalOrderCount = row["TotalOrderCount"] == DBNull.Value ? 0 : int.Parse(row["TotalOrderCount"].ToString()),
                    TotalMoney = row["TotalMoney"] == DBNull.Value ? 0 : decimal.Parse(row["TotalMoney"].ToString()),
                    ProfitMoney = row["ProfitMoney"] == DBNull.Value ? 0 : decimal.Parse(row["ProfitMoney"].ToString()),
                    GoldCrownCount = row["GoldCrownCount"] == DBNull.Value ? 0 : int.Parse(row["GoldCrownCount"].ToString()),
                    GoldCupCount = row["GoldCupCount"] == DBNull.Value ? 0 : int.Parse(row["GoldCupCount"].ToString()),
                    GoldDiamondsCount = row["GoldDiamondsCount"] == DBNull.Value ? 0 : int.Parse(row["GoldDiamondsCount"].ToString()),
                    GoldStarCount = row["GoldStarCount"] == DBNull.Value ? 0 : int.Parse(row["GoldStarCount"].ToString()),
                    SilverCrownCount = row["SilverCrownCount"] == DBNull.Value ? 0 : int.Parse(row["SilverCrownCount"].ToString()),
                    SilverCupCount = row["SilverCupCount"] == DBNull.Value ? 0 : int.Parse(row["SilverCupCount"].ToString()),
                    SilverDiamondsCount = row["SilverDiamondsCount"] == DBNull.Value ? 0 : int.Parse(row["SilverDiamondsCount"].ToString()),
                    SilverStarCount = row["SilverStarCount"] == DBNull.Value ? 0 : int.Parse(row["SilverStarCount"].ToString()),
                });
            }
            return list;
        }

        #endregion

        #region 大奖排行榜 - 竞彩类

        public List<RankInfo_BettingProfit_Sport> QueryRankInfoList_BigBonus_Sport(DateTime fromDate, DateTime toDate, string gameCode, string gameType, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;

            var dict = new Dictionary<string, object>();
            // 通过数据库存储过程进行查询
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Rank_QueryBigBonusRank_Sport"))
                .AddInParameter("fromDate", fromDate)
                .AddInParameter("toDate", toDate)
                .AddInParameter("gameCode", gameCode)
                .AddInParameter("gameType", gameType)
                .AddInParameter("pageIndex", pageIndex)
                .AddInParameter("pageSize", pageSize)
                .AddOutParameter("totalCount", "Int32");
            var dt = query.GetDataTable(out dict);
            totalCount = UsefullHelper.GetDbValue<int>(dict["totalCount"]);

            var list = new List<RankInfo_BettingProfit_Sport>();
            foreach (DataRow row in dt.Rows)
            {
                //SchemeId = row["SchemeId"] == DBNull.Value ? "" : row["SchemeId"].ToString(),
                list.Add(new RankInfo_BettingProfit_Sport
                {
                    BonusMoney = row["BonusMoney"] == DBNull.Value ? 0 : decimal.Parse(row["BonusMoney"].ToString()),
                    GameCode = row["GameCode"] == DBNull.Value ? "" : row["GameCode"].ToString(),
                    GameType = row["GameType"] == DBNull.Value ? "" : row["GameType"].ToString(),
                    UserId = row["UserId"] == DBNull.Value ? "" : row["UserId"].ToString(),
                    UserHideDisplayNameCount = row["HideDisplayNameCount"] == DBNull.Value ? 0 : int.Parse(row["HideDisplayNameCount"].ToString()),
                    UserDisplayName = row["DisplayName"] == DBNull.Value ? "" : row["DisplayName"].ToString(),
                    TotalOrderCount = 0,
                    TotalMoney = row["TotalMoney"] == DBNull.Value ? 0 : decimal.Parse(row["TotalMoney"].ToString()),
                    ProfitMoney = 0M,
                    GoldCrownCount = row["GoldCrownCount"] == DBNull.Value ? 0 : int.Parse(row["GoldCrownCount"].ToString()),
                    GoldCupCount = row["GoldCupCount"] == DBNull.Value ? 0 : int.Parse(row["GoldCupCount"].ToString()),
                    GoldDiamondsCount = row["GoldDiamondsCount"] == DBNull.Value ? 0 : int.Parse(row["GoldDiamondsCount"].ToString()),
                    GoldStarCount = row["GoldStarCount"] == DBNull.Value ? 0 : int.Parse(row["GoldStarCount"].ToString()),
                    SilverCrownCount = row["SilverCrownCount"] == DBNull.Value ? 0 : int.Parse(row["SilverCrownCount"].ToString()),
                    SilverCupCount = row["SilverCupCount"] == DBNull.Value ? 0 : int.Parse(row["SilverCupCount"].ToString()),
                    SilverDiamondsCount = row["SilverDiamondsCount"] == DBNull.Value ? 0 : int.Parse(row["SilverDiamondsCount"].ToString()),
                    SilverStarCount = row["SilverStarCount"] == DBNull.Value ? 0 : int.Parse(row["SilverStarCount"].ToString()),
                });

            }
            return list;
        }

        #endregion

        #region 跟单盈利排行榜 - 竞彩类

        public List<RankInfo_BettingProfit_Sport> QueryRankInfoList_JoinProfit_Sport(DateTime fromDate, DateTime toDate, string gameCode, string gameType, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;

            var dict = new Dictionary<string, object>();
            // 通过数据库存储过程进行查询
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Rank_QueryJoinProfitRank_Sport"))
                .AddInParameter("fromDate", fromDate)
                .AddInParameter("toDate", toDate)
                .AddInParameter("gameCode", gameCode)
                .AddInParameter("gameType", gameType)
                .AddInParameter("pageIndex", pageIndex)
                .AddInParameter("pageSize", pageSize)
                .AddOutParameter("totalCount", "Int32");
            var dt = query.GetDataTable(out dict);
            totalCount = UsefullHelper.GetDbValue<int>(dict["totalCount"]);

            var list = new List<RankInfo_BettingProfit_Sport>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new RankInfo_BettingProfit_Sport
                {
                    BonusMoney = row["BonusMoney"] == DBNull.Value ? 0 : decimal.Parse(row["BonusMoney"].ToString()),
                    GameCode = row["GameCode"] == DBNull.Value ? "" : row["GameCode"].ToString(),
                    GameType = row["GameType"] == DBNull.Value ? "" : row["GameType"].ToString(),
                    UserId = row["JoinUserId"] == DBNull.Value ? "" : row["JoinUserId"].ToString(),
                    UserHideDisplayNameCount = row["HideDisplayNameCount"] == DBNull.Value ? 0 : int.Parse(row["HideDisplayNameCount"].ToString()),
                    UserDisplayName = row["DisplayName"] == DBNull.Value ? "" : row["DisplayName"].ToString(),
                    TotalOrderCount = row["TotalOrderCount"] == DBNull.Value ? 0 : int.Parse(row["TotalOrderCount"].ToString()),
                    TotalMoney = row["TotalMoney"] == DBNull.Value ? 0 : decimal.Parse(row["TotalMoney"].ToString()),
                    ProfitMoney = row["ProfitMoney"] == DBNull.Value ? 0 : decimal.Parse(row["ProfitMoney"].ToString()),
                    GoldCrownCount = row["GoldCrownCount"] == DBNull.Value ? 0 : int.Parse(row["GoldCrownCount"].ToString()),
                    GoldCupCount = row["GoldCupCount"] == DBNull.Value ? 0 : int.Parse(row["GoldCupCount"].ToString()),
                    GoldDiamondsCount = row["GoldDiamondsCount"] == DBNull.Value ? 0 : int.Parse(row["GoldDiamondsCount"].ToString()),
                    GoldStarCount = row["GoldStarCount"] == DBNull.Value ? 0 : int.Parse(row["GoldStarCount"].ToString()),
                    SilverCrownCount = row["SilverCrownCount"] == DBNull.Value ? 0 : int.Parse(row["SilverCrownCount"].ToString()),
                    SilverCupCount = row["SilverCupCount"] == DBNull.Value ? 0 : int.Parse(row["SilverCupCount"].ToString()),
                    SilverDiamondsCount = row["SilverDiamondsCount"] == DBNull.Value ? 0 : int.Parse(row["SilverDiamondsCount"].ToString()),
                    SilverStarCount = row["SilverStarCount"] == DBNull.Value ? 0 : int.Parse(row["SilverStarCount"].ToString()),
                });
            }
            return list;
        }

        #endregion


        #region 成功的战绩排行

        public List<RankInfo_BettingProfit_Sport> QueryRankInfoList_SuccessOrder_Sport(DateTime fromDate, DateTime toDate, string gameCode, string gameType, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            totalCount = 0;
            List<RankInfo_BettingProfit_Sport> bettingList = new List<RankInfo_BettingProfit_Sport>();
            string strSql = "select orderTable.BonusMoney,orderTable.TotalMoney,orderTable.TotalOrderCount,orderTable.UserId,orderTable.GameCode,orderTable.GameType,(orderTable.BonusMoney-orderTable.TotalMoney) ProfitMoney,orderTable.DisplayName,orderTable.HideDisplayNameCount,userBeedTable.GoldCrownCount,userBeedTable.GoldCupCount,userBeedTable.GoldDiamondsCount,userBeedTable.GoldStarCount,userBeedTable.SilverCrownCount,userBeedTable.SilverCupCount,userBeedTable.SilverDiamondsCount,userBeedTable.SilverStarCount from  ( select c.UserId,u.DisplayName,u.HideDisplayNameCount,COUNT(c.SchemeId) TotalOrderCount,c.GameCode,c.GameType,SUM(c.AfterTaxBonusMoney) BonusMoney,SUM(c.TotalMoney) TotalMoney from C_Sports_Order_Complate c inner join C_Sports_Together t on c.SchemeId=t.SchemeId inner join C_User_Register u on c.UserId=u.UserId where ComplateDate>=:StartTime and ComplateDate<:EndTime group by c.GameCode,c.GameType,c.UserId ,u.DisplayName,u.HideDisplayNameCount  ) orderTable left join  ( select UserId,GameCode,GameType,GoldCrownCount,GoldCupCount,GoldDiamondsCount,GoldStarCount,SilverCrownCount,SilverCupCount,SilverDiamondsCount,SilverStarCount from C_User_Beedings where GameCode=:GameCode ) userBeedTable on orderTable.UserId=userBeedTable.UserId and orderTable.GameType=userBeedTable.GameType where orderTable.GameCode=:GameCode order by orderTable.BonusMoney desc";
            var result = Session.CreateSQLQuery(strSql)
                                .SetDateTime("StartTime", fromDate)
                                .SetDateTime("EndTime", toDate)
                                .SetString("GameCode", gameCode)
                                .List();

            if (result != null && result.Count > 0)
            {
                totalCount = result.Count;
                foreach (var item in result)
                {
                    var array = item as object[];
                    RankInfo_BettingProfit_Sport info = new RankInfo_BettingProfit_Sport();
                    info.BonusMoney = array[0] == null ? 0M : Convert.ToDecimal(array[0]);
                    info.TotalMoney = array[1] == null ? 0M : Convert.ToDecimal(array[1]);
                    info.TotalOrderCount = array[2] == null ? 0 : Convert.ToInt32(array[2]);
                    info.UserId = array[3] == null ? string.Empty : Convert.ToString(array[3]);
                    info.GameCode = array[4] == null ? string.Empty : Convert.ToString(array[4]);
                    info.GameType = array[5] == null ? string.Empty : Convert.ToString(array[5]);
                    info.ProfitMoney = array[6] == null ? 0M : Convert.ToDecimal(array[6]);
                    info.UserDisplayName = array[7] == null ? string.Empty : Convert.ToString(array[7]);
                    info.UserHideDisplayNameCount = array[8] == null ? 0 : Convert.ToInt32(array[8]);
                    info.GoldCrownCount = array[9] == null ? 0 : Convert.ToInt32(array[9]);
                    info.GoldCupCount = array[10] == null ? 0 : Convert.ToInt32(array[10]);
                    info.GoldDiamondsCount = array[11] == null ? 0 : Convert.ToInt32(array[11]);
                    info.GoldStarCount = array[12] == null ? 0 : Convert.ToInt32(array[12]);
                    info.SilverCrownCount = array[13] == null ? 0 : Convert.ToInt32(array[13]);
                    info.SilverCupCount = array[14] == null ? 0 : Convert.ToInt32(array[14]);
                    info.SilverDiamondsCount = array[15] == null ? 0 : Convert.ToInt32(array[15]);
                    info.SilverStarCount = array[16] == null ? 0 : Convert.ToInt32(array[16]);
                    bettingList.Add(info);
                }
            }
            if (bettingList != null && bettingList.Count > 0)
                bettingList = bettingList.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            return bettingList;

            #region 20151015Old
            //string[] array = new[] { "100622", "100623", "100624", "100625", "100626", "100629", "100631", "100637", "100639", "100641", "100643", "100645", "100648", "100650", "100653", "100658", "100660", "100663", "100668", "100672", "100674", "100675", "100676", "100677", "100678", "100679", "100680", "100681", "100682", "100683", "100627", "100630", "100628", "100632", "100633", "100634", "100635", "100636", "100638", "100640", "100642", "100644", "100646", "100647", "100649", "100651", "100652", "100654", "100655", "100656", "100657", "100659", "100661", "100662", "100664", "100665", "100667", "100669", "100670", "100671", "100673" };
            //var query = from o in this.Session.Query<Sports_Order_Complate>()
            //            join t in this.Session.Query<Sports_Together>() on o.SchemeId equals t.SchemeId
            //            where o.SchemeType == SchemeType.TogetherBetting
            //            && (o.IsVirtualOrder == false || array.Contains(o.UserId))
            //            && (gameCode == string.Empty || o.GameCode == gameCode)
            //            && (gameType == string.Empty || o.GameType == gameType)
            //            && (o.ComplateDateTime >= fromDate && o.ComplateDateTime < toDate)
            //            select new
            //            {
            //                UserId = o.UserId,
            //                GameCode = o.GameCode,
            //                GameType = o.GameType,
            //                BonusMoney = o.AfterTaxBonusMoney,
            //                TotalMoney = o.TotalMoney,
            //            };
            //var result = query.ToList().GroupBy(x => new { x.UserId, x.GameCode, x.GameType }).Select(s => new
            //{
            //    UserId = s.Key.UserId,
            //    GameCode = s.Key.GameCode,
            //    GameType = s.Key.GameType,
            //    BonusMoney = s.Sum(a => a.BonusMoney),
            //    TotalMoney = s.Sum(a => a.TotalMoney),
            //    TotalOrderCount = s.Count()
            //});
            //var userList = result.ToList().Skip(pageIndex * pageSize).Take(pageSize);
            //var query2 = from r in userList.ToList()
            //             join u in this.Session.Query<UserRegister>() on r.UserId equals u.UserId
            //             join b in this.Session.Query<UserBeedings>() on r.UserId equals b.UserId
            //             where b.GameCode == r.GameCode && b.GameCode == gameCode.ToUpper() && b.GameType == r.GameType && (b.GameType == gameType.ToUpper() || gameType == string.Empty)
            //             orderby r.BonusMoney descending
            //             select new RankInfo_BettingProfit_Sport
            //             {
            //                 BonusMoney = r.BonusMoney,
            //                 TotalMoney = r.TotalMoney,
            //                 TotalOrderCount = r.TotalOrderCount,
            //                 UserId = r.UserId,
            //                 GameCode = gameCode,
            //                 GameType = gameType,
            //                 ProfitMoney = r.BonusMoney - r.TotalMoney,
            //                 UserDisplayName = u.DisplayName,
            //                 UserHideDisplayNameCount = u.HideDisplayNameCount,
            //                 GoldCrownCount = b.GoldCrownCount,
            //                 GoldCupCount = b.GoldCupCount,
            //                 GoldDiamondsCount = b.GoldDiamondsCount,
            //                 GoldStarCount = b.GoldStarCount,
            //                 SilverCrownCount = b.SilverCrownCount,
            //                 SilverCupCount = b.SilverCupCount,
            //                 SilverDiamondsCount = b.SilverDiamondsCount,
            //                 SilverStarCount = b.SilverStarCount,
            //             };
            //totalCount = query2.Count();
            //return query2.ToList(); 
            #endregion




            #region old
            //Session.Clear();
            //pageIndex = pageIndex < 0 ? 0 : pageIndex;
            //pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;

            //var query = from o in this.Session.Query<Sports_Order_Complate>()
            //            join t in this.Session.Query<Sports_Together>() on o.SchemeId equals t.SchemeId
            //            where o.SchemeType == SchemeType.TogetherBetting
            //            && o.IsVirtualOrder == false
            //            && (gameCode == string.Empty || o.GameCode == gameCode)
            //            && (gameType == string.Empty || o.GameType == gameType)
            //            && (o.ComplateDateTime >= fromDate && o.ComplateDateTime < toDate)
            //            group o by new { UserId = o.UserId, TotalMoney = o.TotalMoney, BonusMoney = o.AfterTaxBonusMoney } into _o
            //            select new
            //            {
            //                BonusMoney = _o.Key.BonusMoney,
            //                TotalMoney = _o.Key.TotalMoney,
            //                TotalOrderCount = _o.Count(),
            //                UserId = _o.Key.UserId,
            //            };

            //var userList = query.ToList();
            //var query2 = from r in userList.Skip(pageIndex * pageSize).Take(pageSize).ToArray()
            //             join u in this.Session.Query<UserRegister>() on r.UserId equals u.UserId
            //             join b in this.Session.Query<UserBeedings>() on new { GameCode = gameCode, GameType = gameType, UserId = r.UserId } equals new { GameCode = b.GameCode, GameType = b.GameType, UserId = b.UserId }
            //             select new RankInfo_BettingProfit_Sport
            //             {
            //                 BonusMoney = r.BonusMoney,
            //                 TotalMoney = r.TotalMoney,
            //                 TotalOrderCount = r.TotalOrderCount,
            //                 UserId = r.UserId,
            //                 GameCode = gameCode,
            //                 GameType = gameType,
            //                 ProfitMoney = r.BonusMoney - r.TotalMoney,
            //                 UserDisplayName = u.DisplayName,
            //                 UserHideDisplayNameCount = u.HideDisplayNameCount,
            //                 GoldCrownCount = b.GoldCrownCount,
            //                 GoldCupCount = b.GoldCupCount,
            //                 GoldDiamondsCount = b.GoldDiamondsCount,
            //                 GoldStarCount = b.GoldStarCount,
            //                 SilverCrownCount = b.SilverCrownCount,
            //                 SilverCupCount = b.SilverCupCount,
            //                 SilverDiamondsCount = b.SilverDiamondsCount,
            //                 SilverStarCount = b.SilverStarCount,
            //             };
            //totalCount = query2.Count();
            //return query2.ToList();
            #endregion
        }

        #endregion

        /// <summary>
        /// 跟单排行榜
        /// </summary>
        public List<RankInfo_BeFollower> QueryRankInfoList_BeFollowerCount(DateTime fromDate, DateTime toDate, string gameCode, string gameType, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            totalCount = 0;
            List<RankInfo_BeFollower> beFollowerList = new List<RankInfo_BeFollower>();
            string strSql = "select followTable.f_userCount,followTable.CreaterUserId,u.DisplayName,u.HideDisplayNameCount,b.GoldCrownCount,b.GoldCupCount,b.GoldDiamondsCount,b.GoldStarCount,b.SilverCrownCount,b.SilverCupCount,b.SilverDiamondsCount,b.SilverStarCount from  ( select COUNT(1) as f_userCount,CreaterUserId from C_Together_FollowerRecord where GameCode=:GameCode and CreateTime>=:StartTime and CreateTime<:EndTime group by CreaterUserId ) followTable inner join C_User_Register u on followTable.CreaterUserId=u.UserId  inner join  ( select UserId,sum(b.GoldCrownCount) GoldCrownCount,sum(b.GoldCupCount)GoldCupCount,sum(b.GoldDiamondsCount)GoldDiamondsCount,sum(b.GoldStarCount)GoldStarCount,sum(b.SilverCrownCount)SilverCrownCount,sum(b.SilverCupCount)SilverCupCount,sum(b.SilverDiamondsCount)SilverDiamondsCount,sum(b.SilverStarCount)SilverStarCount  from C_User_Beedings b where b.GameCode=:GameCode group by UserId)b on followTable.CreaterUserId=b.UserId order by followTable.f_userCount desc";
            var result = Session.CreateSQLQuery(strSql)
                              .SetString("GameCode", gameCode)
                              .SetDateTime("StartTime", fromDate)
                              .SetDateTime("EndTime", toDate).List();
            if (result != null && result.Count > 0)
            {
                totalCount = result.Count;
                foreach (var item in result)
                {
                    var array = item as object[];
                    RankInfo_BeFollower info = new RankInfo_BeFollower();
                    info.BeFollowCount = array[0] == null ? 0 : Convert.ToInt32(array[0]);
                    info.UserId = array[1] == null ? string.Empty : array[1].ToString();
                    info.UserDisplayName = array[2] == null ? string.Empty : array[2].ToString();
                    info.UserHideDisplayNameCount = array[3] == null ? 0 : Convert.ToInt32(array[3]);
                    info.GoldCrownCount = array[4] == null ? 0 : Convert.ToInt32(array[4]);
                    info.GoldCupCount = array[5] == null ? 0 : Convert.ToInt32(array[5]);
                    info.GoldDiamondsCount = array[6] == null ? 0 : Convert.ToInt32(array[6]);
                    info.GoldStarCount = array[7] == null ? 0 : Convert.ToInt32(array[7]);
                    info.SilverCrownCount = array[8] == null ? 0 : Convert.ToInt32(array[8]);
                    info.SilverCupCount = array[9] == null ? 0 : Convert.ToInt32(array[9]);
                    info.SilverDiamondsCount = array[10] == null ? 0 : Convert.ToInt32(array[10]);
                    info.SilverStarCount = array[11] == null ? 0 : Convert.ToInt32(array[11]);
                    beFollowerList.Add(info);
                }
            }
            if (beFollowerList != null && beFollowerList.Count > 0)
                beFollowerList = beFollowerList.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            return beFollowerList;

            #region old
            //var query = from r in this.Session.Query<TogetherFollowerRule>()
            //            where (gameCode == string.Empty || r.GameCode == gameCode)
            //            && (gameType == string.Empty || r.GameType == gameType)
            //            && (r.CreateTime >= fromDate && r.CreateTime < toDate)
            //            group r by r.CreaterUserId into _r
            //            select new
            //            {
            //                UserId = _r.Key,
            //                Count = _r.Count(),
            //            };

            //var flowList = query.ToList();

            //var query2 = from f in flowList.Skip(pageIndex * pageSize).Take(pageSize).ToArray()
            //             join u in this.Session.Query<UserRegister>() on f.UserId equals u.UserId
            //             join b in this.Session.Query<UserBeedings>() on new { GameCode = gameCode, GameType = gameType, UserId = f.UserId } equals new { GameCode = b.GameCode, GameType = b.GameType, UserId = b.UserId }
            //             select new RankInfo_BeFollower
            //             {
            //                 UserId = f.UserId,
            //                 BeFollowCount = f.Count,
            //                 UserDisplayName = u.DisplayName,
            //                 UserHideDisplayNameCount = u.HideDisplayNameCount,
            //                 GoldCrownCount = b.GoldCrownCount,
            //                 GoldCupCount = b.GoldCupCount,
            //                 GoldDiamondsCount = b.GoldDiamondsCount,
            //                 GoldStarCount = b.GoldStarCount,
            //                 SilverCrownCount = b.SilverCrownCount,
            //                 SilverCupCount = b.SilverCupCount,
            //                 SilverDiamondsCount = b.SilverDiamondsCount,
            //                 SilverStarCount = b.SilverStarCount,
            //             };

            //totalCount = query2.Count();
            //return query2.ToList(); 
            #endregion

        }

        /// <summary>
        /// 合买人气排行榜
        /// </summary>
        public List<RankInfo_HotTogether> QueryRankInfoList_HotTogether(DateTime fromDate, DateTime toDate, string gameCode, string gameType, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            List<RankInfo_HotTogether> rankList = new List<RankInfo_HotTogether>();
            totalCount = 0;
            string strSql = "select FollowerCount,SucessOrderCount,ctable.UserId,DisplayName,u.HideDisplayNameCount from  ( select c.UserId,c.GameCode,COUNT(SchemeId) SucessOrderCount from C_Sports_Order_Complate c where c.ProgressStatus=90 and c.GameCode=:GameCode and SchemeType=3 and c.ComplateDateTime>=:StartTime and c.ComplateDateTime<:EndTime group by c.UserId,c.GameCode )ctable inner join ( select f.CreaterUserId,f.GameCode,COUNT(1) FollowerCount from C_Together_FollowerRecord f where f.GameCode=:GameCode group by f.CreaterUserId,f.GameCode ) ftable on ctable.UserId=ftable.CreaterUserId inner join C_User_Register u on ctable.UserId=u.UserId";

            var result = Session.CreateSQLQuery(strSql)
                              .SetString("GameCode", gameCode)
                              .SetDateTime("StartTime", fromDate)
                              .SetDateTime("EndTime", toDate).List();
            if (result != null && result.Count > 0)
            {
                totalCount = result.Count;
                foreach (var item in result)
                {
                    RankInfo_HotTogether info = new RankInfo_HotTogether();
                    var array = item as object[];
                    info.FollowUserCount = array[0] == null ? 0 : Convert.ToInt32(array[0]);
                    info.SucessOrderCount = array[1] == null ? 0 : Convert.ToInt32(array[1]);
                    info.UserId = array[2] == null ? string.Empty : array[2].ToString();
                    info.UserDisplayName = array[3] == null ? string.Empty : array[3].ToString();
                    info.UserHideDisplayNameCount = array[4] == null ? 0 : Convert.ToInt32(array[4]);
                    rankList.Add(info);
                }
            }
            return rankList;

            #region 20151015 Old
            //var order = from o in Session.Query<Sports_Order_Complate>()
            //            where o.SchemeType == SchemeType.TogetherBetting && o.GameCode == gameCode && o.ProgressStatus == ProgressStatus.Complate
            //            group o by new { o.UserId, o.GameCode } into _result
            //            select new
            //            {
            //                UserId = _result.Key.UserId,
            //                GameCode = _result.Key.GameCode,
            //                OrderCount = _result.Count(),
            //            };
            //var OrderList = order.ToList();
            //var followerRecord = from f in Session.Query<TogetherFollowerRecord>()
            //                     where f.GameCode == gameCode
            //                     group f by new { f.CreaterUserId, f.GameCode } into _result
            //                     select new
            //                     {
            //                         UserId = _result.Key.CreaterUserId,
            //                         GameCode = _result.Key.GameCode,
            //                         FollowerCount = _result.Count(),
            //                     };
            //var FollowerRecordList = followerRecord.ToList();

            //var result = from o in OrderList
            //             join f in FollowerRecordList
            //             on o.UserId equals f.UserId
            //             join u in Session.Query<UserRegister>()
            //             on o.UserId equals u.UserId
            //             where o.GameCode == f.GameCode
            //             orderby f.FollowerCount, o.OrderCount
            //             select new RankInfo_HotTogether
            //             {
            //                 FollowUserCount = f.FollowerCount,
            //                 SucessOrderCount = o.OrderCount,
            //                 UserId = u.UserId,
            //                 UserDisplayName = u.DisplayName,
            //                 UserHideDisplayNameCount = u.HideDisplayNameCount,

            //             };
            //totalCount = result.Count();
            //var query = result.Skip(pageIndex * pageSize).Take(pageSize);
            //return query.ToList(); 
            #endregion

            #region old

            //Session.Clear();
            //pageIndex = pageIndex < 0 ? 0 : pageIndex;
            //pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;

            //var query = from j in this.Session.Query<UserBeedings>()
            //            //join o in this.Session.Query<Sports_Order_Complate>() on j.UserId equals o.UserId
            //            where (gameCode == string.Empty || j.GameCode == gameCode)
            //            && (gameType == string.Empty || j.GameType == gameType)
            //            orderby j.BeFollowerUserCount descending
            //            select j;


            ////totalCount = query.Count();
            //var beedList = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            //var userBeedList = beedList.Select(p => p.UserId).ToList();

            //var query2 = from o in this.Session.Query<Sports_Order_Complate>()
            //             where (string.Empty == gameCode || o.GameCode == gameCode)
            //             && (string.Empty == gameType || o.GameType == gameType)
            //             && o.IsVirtualOrder == false
            //             && (o.CreateTime >= fromDate && o.CreateTime < toDate)
            //             && (userBeedList.Contains(o.UserId))
            //             group o by o.UserId into _o
            //             select new
            //             {
            //                 UserId = _o.Key,
            //                 OrderCount = _o.Count(),
            //             };

            //var orderList = query2.ToList();

            //var query3 = from b in beedList
            //             join o in orderList on b.UserId equals o.UserId
            //             join u in this.Session.Query<UserRegister>() on o.UserId equals u.UserId
            //             join bb in this.Session.Query<UserBeedings>() on new { GameCode = gameCode, GameType = gameType, UserId = o.UserId }
            //                equals new { GameCode = bb.GameCode, GameType = bb.GameType, UserId = bb.UserId }
            //             select new RankInfo_HotTogether
            //             {
            //                 FollowUserCount = b.BeFollowerUserCount,
            //                 SucessOrderCount = o.OrderCount,
            //                 UserId = u.UserId,
            //                 UserDisplayName = u.DisplayName,
            //                 UserHideDisplayNameCount = u.HideDisplayNameCount,
            //                 GoldCrownCount = b.GoldCrownCount,
            //                 GoldCupCount = b.GoldCupCount,
            //                 GoldDiamondsCount = b.GoldDiamondsCount,
            //                 GoldStarCount = b.GoldStarCount,
            //                 SilverCrownCount = b.SilverCrownCount,
            //                 SilverCupCount = b.SilverCupCount,
            //                 SilverDiamondsCount = b.SilverDiamondsCount,
            //                 SilverStarCount = b.SilverStarCount,
            //             };
            //totalCount = query3.Count();
            //return query3.ToList();

            #endregion
        }


        #region 累积中奖排行榜 - 竞彩类

        public IList<RankInfo_TotalBonus_Sport> QueryRankInfoList_TotalBonus_Sport(DateTime fromDate, DateTime toDate, string gameCode, string gameType, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var dict = new Dictionary<string, object>();
            // 通过数据库存储过程进行查询
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Rank_QueryTotalBonusRank_Sport"))
                .AddInParameter("fromDate", fromDate)
                .AddInParameter("toDate", toDate)
                .AddInParameter("gameCode", gameCode)
                .AddInParameter("gameType", gameType)
                .AddInParameter("pageIndex", pageIndex)
                .AddInParameter("pageSize", pageSize)
                .AddOutParameter("totalCount", "Int32");
            var dt = query.GetDataTable(out dict);
            totalCount = UsefullHelper.GetDbValue<int>(dict["totalCount"]);

            return ORMHelper.DataTableToList<RankInfo_TotalBonus_Sport>(dt);
        }

        #endregion

        #region 中奖排行榜 - 按彩种查

        public IList<RankInfo_TotalBonus_Sport> QueryRankReport_BonusByGameCode_All(DateTime fromDate, DateTime toDate, int topCount, string gameCode)
        {
            Session.Clear();

            #region nhibernate.linq

            //var query = from o in this.Session.Query<Sports_Order_Complate>()
            //            where o.IsVirtualOrder == false
            //            && (string.Empty == gameCode || o.GameCode == gameCode)
            //            && (o.CreateTime >= fromDate && o.CreateTime < toDate)
            //            && o.AfterTaxBonusMoney > 0
            //            group o by o.UserId into _o
            //            select new
            //            {
            //                UserId = _o.Key,
            //                BonusMoney = _o.Sum(p => p.AfterTaxBonusMoney),
            //                OrderCount = _o.Count(),
            //                TotalMoney = _o.Sum(p => p.TotalMoney),
            //            };
            //var userList = query.ToList();//.OrderByDescending(p => p.BonusMoney);

            //var query2 = from l in userList
            //             join u in this.Session.Query<UserRegister>() on l.UserId equals u.UserId
            //             orderby l.BonusMoney descending
            //             select new RankInfo_TotalBonus_Sport
            //             {
            //                 BonusMoney = l.BonusMoney,
            //                 ProfitMoney = l.BonusMoney - l.TotalMoney,
            //                 TotalOrderCount = l.OrderCount,
            //                 TotalOrderMoney = l.TotalMoney,
            //                 UserId = u.UserId,
            //                 UserDisplayName = u.DisplayName,
            //                 UserHideDisplayNameCount = u.HideDisplayNameCount,
            //             };

            //var test = query2.Take(topCount).ToList();



            //var query3 = from l in
            //                 (from o in this.Session.Query<Sports_Order_Complate>()
            //                  where o.IsVirtualOrder == false
            //                  && (string.Empty == gameCode || o.GameCode == gameCode)
            //                  && (o.CreateTime >= fromDate && o.CreateTime < toDate)
            //                  && o.AfterTaxBonusMoney > 0
            //                  group o by o.UserId into _o
            //                  select new
            //                  {
            //                      UserId = _o.Key,
            //                      BonusMoney = _o.Sum(p => p.AfterTaxBonusMoney),
            //                      OrderCount = _o.Count(),
            //                      TotalMoney = _o.Sum(p => p.TotalMoney),
            //                  })
            //             join u in this.Session.Query<UserRegister>() on l.UserId equals u.UserId
            //             orderby l.BonusMoney descending
            //             select new RankInfo_TotalBonus_Sport
            //             {
            //                 BonusMoney = l.BonusMoney,
            //                 ProfitMoney = l.BonusMoney - l.TotalMoney,
            //                 TotalOrderCount = l.OrderCount,
            //                 TotalOrderMoney = l.TotalMoney,
            //                 UserId = u.UserId,
            //                 UserDisplayName = u.DisplayName,
            //                 UserHideDisplayNameCount = u.HideDisplayNameCount,
            //             };

            //var test2 = query2.Take(topCount).ToList();

            #endregion

            // 通过数据库存储过程进行查询
            var query4 = CreateOutputQuery(Session.GetNamedQuery("P_Rank_QueryBonusRankByGameCode_All"))
                .AddInParameter("fromDate", fromDate)
                .AddInParameter("toDate", toDate)
                .AddInParameter("topCount", topCount)
                .AddInParameter("gameCode", gameCode);
            var dt = query4.GetDataTable();

            return ORMHelper.DataTableToList<RankInfo_TotalBonus_Sport>(dt);
        }

        #endregion

        #endregion

        #region 过关统计

        /// <summary>
        /// 查询过关统计
        /// </summary>
        public List<SportsOrder_GuoGuanInfo> QueryReportInfoList_GuoGuan(bool? isVirtualOrder, SchemeBettingCategory? category, string key, string gameCode, string gameType,
            string issuseNumber, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();

            #region linq

            //var query = from c in this.Session.Query<Sports_Order_Complate>()
            //            join u in this.Session.Query<UserRegister>() on c.UserId equals u.UserId
            //            join b in this.Session.Query<UserBeedings>() on new { GameCode = c.GameCode, GameType = c.GameType, UserId = c.UserId } equals new { GameCode = b.GameCode, GameType = b.GameType, UserId = b.UserId }
            //            where c.IsVirtualOrder == isVirtualOrder
            //            && (key == string.Empty || (c.SchemeId == key || u.DisplayName == key))
            //            && (c.GameCode == gameCode)
            //            && (gameType == string.Empty || c.GameType == gameType)
            //            && (issuseNumber == string.Empty || c.IssuseNumber == issuseNumber)
            //            && (complateDate == string.Empty || c.ComplateDate == complateDate)
            //            && (category == null || c.SchemeBettingCategory == category)
            //            orderby c.AfterTaxBonusMoney, c.TotalMoney descending
            //            select new SportsOrder_GuoGuanInfo
            //            {
            //                BetCount = c.BetCount,
            //                BonusMoney = c.AfterTaxBonusMoney,
            //                BonusStatus = c.BonusStatus,
            //                Error1Count = c.Error1Count,
            //                Error2Count = c.Error2Count,
            //                GameCode = c.GameCode,
            //                GameType = c.GameType,
            //                HitMatchCount = c.HitMatchCount,
            //                IssuseNumber = c.IssuseNumber,
            //                SchemeId = c.SchemeId,
            //                RightCount = c.RightCount,
            //                SchemeType = c.SchemeType,
            //                TotalMoney = c.TotalMoney,
            //                UserId = c.UserId,
            //                UserDisplayName = u.DisplayName,
            //                UserHideDisplayNameCount = u.HideDisplayNameCount,
            //                GoldCrownCount = b.GoldCrownCount,
            //                GoldCupCount = b.GoldCupCount,
            //                GoldDiamondsCount = b.GoldDiamondsCount,
            //                GoldStarCount = b.GoldStarCount,
            //                SilverCrownCount = b.SilverCrownCount,
            //                SilverCupCount = b.SilverCupCount,
            //                SilverDiamondsCount = b.SilverDiamondsCount,
            //                SilverStarCount = b.SilverStarCount,
            //            };

            //totalCount = query.Count();
            //return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            #endregion

            startTime = startTime.Date;
            endTime = endTime.AddDays(1).Date;

            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Report_GuoGuanTongJi_Sport"))
                .AddInParameter("isVirtualOrder", !isVirtualOrder.HasValue ? -1 : Convert.ToInt32(isVirtualOrder.Value))
               .AddInParameter("bettingCategory", !category.HasValue ? -1 : (int)category.Value)
               .AddInParameter("key_UID_UName_SchemeId", key)
               .AddInParameter("gameCode", gameCode.ToUpper())
               .AddInParameter("gameType", gameType.ToUpper())
               .AddInParameter("issuseNumber", issuseNumber)
               .AddInParameter("startTime", startTime)
               .AddInParameter("endTime", endTime)
               .AddInParameter("pageIndex", pageIndex)
               .AddInParameter("pageSize", pageSize)
               .AddOutParameter("TotalCount", "Int32");

            var dt = query.GetDataTable(out outputs);
            totalCount = (int)outputs["TotalCount"];
            var listInfo = new List<SportsOrder_GuoGuanInfo>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    listInfo.Add(new SportsOrder_GuoGuanInfo
                    {
                        SchemeId = row["SchemeId"] == DBNull.Value ? "" : row["SchemeId"].ToString(),
                        UserId = row["UserId"] == DBNull.Value ? "" : row["UserId"].ToString(),
                        GameCode = row["GameCode"] == DBNull.Value ? "" : row["GameCode"].ToString(),
                        GameType = row["GameType"] == DBNull.Value ? "" : row["GameType"].ToString(),
                        SchemeType = (SchemeType)Convert.ToInt32(row["SchemeType"]),
                        IssuseNumber = row["IssuseNumber"] == DBNull.Value ? "" : row["IssuseNumber"].ToString(),
                        BetCount = row["BetCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["BetCount"]),
                        TotalMatchCount = row["TotalMatchCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["TotalMatchCount"]),
                        TotalMoney = row["TotalMoney"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TotalMoney"]),
                        BonusStatus = (BonusStatus)Convert.ToInt32(row["BonusStatus"]),
                        HitMatchCount = row["HitMatchCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["HitMatchCount"]),
                        BonusMoney = row["BonusMoney"] == DBNull.Value ? 0 : Convert.ToDecimal(row["BonusMoney"]),
                        UserDisplayName = row["DisplayName"] == DBNull.Value ? "" : row["DisplayName"].ToString(),
                        UserHideDisplayNameCount = row["HideDisplayNameCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["HideDisplayNameCount"]),
                        RightCount = row["RightCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["RightCount"]),
                        Error1Count = row["Error1Count"] == DBNull.Value ? 0 : Convert.ToInt32(row["Error1Count"]),
                        Error2Count = row["Error2Count"] == DBNull.Value ? 0 : Convert.ToInt32(row["Error2Count"]),
                        GoldCrownCount = row["GoldCrownCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["GoldCrownCount"]),
                        GoldCupCount = row["GoldCupCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["GoldCupCount"]),
                        GoldDiamondsCount = row["GoldDiamondsCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["GoldDiamondsCount"]),
                        GoldStarCount = row["GoldStarCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["GoldStarCount"]),
                        SilverCrownCount = row["SilverCrownCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["SilverCrownCount"]),
                        SilverCupCount = row["SilverCupCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["SilverCupCount"]),
                        SilverDiamondsCount = row["SilverDiamondsCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["SilverDiamondsCount"]),
                        SilverStarCount = row["SilverStarCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["SilverStarCount"]),
                        IsVirtualOrder = row["IsVirtualOrder"] == DBNull.Value ? false : Convert.ToBoolean(row["IsVirtualOrder"]),
                        SchemeBettingCategory = (SchemeBettingCategory)Convert.ToInt32(row["SchemeBettingCategory"]),
                        BetTime = Convert.ToDateTime(row["BetTime"]),
                    });
                }
            }
            return listInfo;
        }

        #endregion

        #region 自定义报表

        public DataTable GetDataTable(string sql, Dictionary<string, object> paramList)
        {
            Session.Clear();
            var query = CreateOutputQuery(Session.CreateSQLQuery(sql));
            if (paramList != null)
            {
                foreach (var p in paramList)
                {
                    query.AddInParameter(p.Key, p.Value);
                }
            }
            return query.GetDataTable();
        }
        public DataSet GetDataSet(string sql, Dictionary<string, object> paramList)
        {
            Session.Clear();
            var query = CreateOutputQuery(Session.CreateSQLQuery(sql));
            if (paramList != null)
            {
                foreach (var p in paramList)
                {
                    query.AddInParameter(p.Key, p.Value);
                }
            }
            return query.GetDataSet();
        }

        #endregion

        #region 后台首页统计

        public BackgroundIndexReportInfo_Collection BackgroundIndexReport(DateTime startTime, DateTime endTime)
        {
            Session.Clear();
            string sTime = startTime.ToShortDateString() + " 00:00:00";
            string eTime = endTime.ToShortDateString() + " 23:59:59";

            var dt = CreateOutputQuery(Session.GetNamedQuery("P_Report_BackgroundIndexsStatistics"))
                .AddInParameter("sTime", sTime)
                .AddInParameter("eTime", eTime)
                .GetDataTable();
            BackgroundIndexReportInfo_Collection collection = new BackgroundIndexReportInfo_Collection();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    collection.ReportList.Add(new BackgroundIndexReportInfo
                    {
                        ReportMoney_Day = Convert.ToDecimal(row["ReportMoney"]),
                        ReportMoney_Month = 0,
                        ReportType = row["ReportType"].ToString(),
                        ListUrl_Day = row["ListUrl"].ToString(),
                        ListUrl_Month = ""
                    });
                }
            }
            return collection;
        }

        #endregion

        #region 比赛查询

        public List<CoreJCZQMatchInfo> QueryJCZQCanBetMatch()
        {
            Session.Clear();
            var query = from m in this.Session.Query<JCZQ_Match>()
                        where m.DSStopBettingTime > DateTime.Now
                        select new CoreJCZQMatchInfo
                        {
                            FSStopBettingTime = m.FSStopBettingTime,
                            GuestTeamName = m.GuestTeamName,
                            HomeTeamName = m.HomeTeamName,
                            LeagueColor = m.LeagueColor,
                            LeagueName = m.LeagueName,
                            MatchData = m.MatchData,
                            MatchId = m.MatchId,
                            MatchIdName = m.MatchIdName,
                            MatchNumber = m.MatchNumber,
                            PrivilegesType = m.PrivilegesType,
                            StartDateTime = m.StartDateTime,
                            LeagueId = m.LeagueId,
                        };
            return query.ToList();
        }

        public List<CoreJCLQMatchInfo> QueryJCLQCanBetMatch()
        {
            Session.Clear();
            var query = from m in this.Session.Query<JCLQ_Match>()
                        where m.DSStopBettingTime > DateTime.Now
                        select new CoreJCLQMatchInfo
                        {
                            FSStopBettingTime = m.FSStopBettingTime,
                            GuestTeamName = m.GuestTeamName,
                            HomeTeamName = m.HomeTeamName,
                            LeagueColor = m.LeagueColor,
                            LeagueName = m.LeagueName,
                            MatchData = m.MatchData,
                            MatchId = m.MatchId,
                            MatchIdName = m.MatchIdName,
                            MatchNumber = m.MatchNumber,
                            PrivilegesType = m.PrivilegesType,
                            StartDateTime = m.StartDateTime,
                            LeagueId = m.LeagueId,
                        };
            return query.ToList();
        }

        #endregion

        public BettingOrderInfo QueryOrderDetailBySchemeId(string schemeId)
        {
            Session.Clear();
            var query = from o in Session.Query<OrderDetail>()
                        where (o.SchemeId == schemeId)
                        select new BettingOrderInfo
                        {
                            TicketStatus = o.TicketStatus,
                            IsVirtualOrder = o.IsVirtualOrder,
                            IssuseNumber = o.CurrentIssuseNumber,
                            CurrentBettingMoney = o.CurrentBettingMoney,
                            TotalMoney = o.TotalMoney,
                            ProgressStatus = o.ProgressStatus,
                            SchemeId = o.SchemeId,
                            AfterTaxBonusMoney = o.AfterTaxBonusMoney,
                            PreTaxBonusMoney = o.PreTaxBonusMoney,
                            BonusStatus = o.BonusStatus,
                            SchemeBettingCategory = o.SchemeBettingCategory,
                            SchemeSource = o.SchemeSource,
                            SchemeType = o.SchemeType,
                            AddMoney = o.AddMoney,
                            AgentId = o.AgentId,
                            Amount = o.Amount,
                            BetTime = o.BetTime,
                            BonusAwardsMoney = o.BonusAwardsMoney,
                            CreateTime = o.CreateTime,
                            GameCode = o.GameCode,
                            GameTypeName = o.GameTypeName,
                            PlayType = o.PlayType,
                            RealPayRebateMoney = o.RealPayRebateMoney,
                            RedBagAwardsMoney = o.RedBagAwardsMoney,
                            RedBagMoney = o.RedBagMoney,
                            Security = TogetherSchemeSecurity.Public,
                            StopAfterBonus = o.StopAfterBonus,
                            TotalIssuseCount = o.TotalIssuseCount,
                            UserId = o.UserId,
                            VipLevel = 0,
                            WinNumber = string.Empty,
                        };
            return query.FirstOrDefault();
        }
        public List<Sports_TicketQueryInfo> QueryTicketInfoList(string schemeId, int pageIndex, int pageSize, DateTime startTime, DateTime endTime, string gameCode, out int totalCount)
        {
            Session.Clear();
            startTime = startTime.Date;
            endTime = endTime.AddDays(1).Date;
            var query = from t in this.Session.Query<Sports_Ticket>()
                        where (schemeId == string.Empty || t.SchemeId == schemeId) && (t.CreateTime >= startTime && t.CreateTime < endTime) && (gameCode == string.Empty || t.GameCode == gameCode)
                        orderby t.TicketId ascending
                        select new Sports_TicketQueryInfo
                        {
                            AfterTaxBonusMoney = t.AfterTaxBonusMoney,
                            Amount = t.Amount,
                            BetMoney = t.BetMoney,
                            BetUnits = t.BetUnits,
                            BonusStatus = t.BonusStatus,
                            CreateTime = t.CreateTime,
                            GameCode = t.GameCode,
                            GameType = t.GameType,
                            IssuseNumber = t.IssuseNumber,
                            PlayType = t.PlayType,
                            PreTaxBonusMoney = t.PreTaxBonusMoney,
                            BarCode = t.BarCode,
                            PrintNumber1 = t.PrintNumber1,
                            PrintNumber2 = t.PrintNumber2,
                            PrintNumber3 = t.PrintNumber3,
                            SchemeId = t.SchemeId,
                            TicketId = t.TicketId,
                            TicketStatus = t.TicketStatus,
                            BetContent = t.BetContent,
                            LocOdds = t.LocOdds,
                        };

            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public decimal GetAllUserBalanceMoney(string userId)
        {
            Session.Clear();
            var list = Session.Query<UserBalance>().Where(s => s.UserId == userId || string.IsNullOrEmpty(userId)).ToList();
            var sumMoney = list.Sum(s => s.FillMoneyBalance + s.BonusBalance + s.ExpertsBalance + s.FreezeBalance + s.RedBagBalance);
            return sumMoney;
        }
        public UserBetStatistics_Collection QueryUserBetStatistiscList()
        {
            UserBetStatistics_Collection collection = new UserBetStatistics_Collection();
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Report_QueryUserBetStatistics"));
            var dt = query.GetDataTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    UserBetStatisticsInfo info = new UserBetStatisticsInfo();
                    info.DisplayName = row["DisplayName"] == DBNull.Value ? string.Empty : Convert.ToString(row["DisplayName"]);
                    info.Email = row["Email"] == DBNull.Value ? string.Empty : Convert.ToString(row["Email"]);
                    info.LastBetTime = row["LastBetTime"] == DBNull.Value ? Convert.ToDateTime(null) : Convert.ToDateTime(row["LastBetTime"]);
                    info.LastLoginTime = row["LastLoginTime"] == DBNull.Value ? Convert.ToDateTime(null) : Convert.ToDateTime(row["LastLoginTime"]);
                    info.Mobile = row["Mobile"] == DBNull.Value ? string.Empty : Convert.ToString(row["Mobile"]);
                    info.ParentId = row["ParentId"] == DBNull.Value ? string.Empty : Convert.ToString(row["ParentId"]);
                    info.RealName = row["RealName"] == DBNull.Value ? string.Empty : Convert.ToString(row["RealName"]);
                    info.StrRebate = row["StrRebate"] == DBNull.Value ? string.Empty : Convert.ToString(row["StrRebate"]);
                    info.TotalBalance = row["TotalBalance"] == DBNull.Value ? 0M : Convert.ToDecimal(row["TotalBalance"]);
                    info.TotalBettingMoney = row["TotalBettingMoney"] == DBNull.Value ? 0M : Convert.ToDecimal(row["TotalBettingMoney"]);
                    info.UserId = row["UserId"] == DBNull.Value ? string.Empty : Convert.ToString(row["UserId"]);
                    collection.BetList.Add(info);
                }
            }
            return collection;
        }
        public AgentRebateStatistics_Collection QueryAgentRebateStatisticsList(string viewType, string gs_AgentId, DateTime startTime, DateTime endTime)
        {
            startTime = startTime.Date;
            endTime = endTime.AddDays(1).Date;
            AgentRebateStatistics_Collection collection = new AgentRebateStatistics_Collection();
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Report_QueryAgentRebateStatistics"))
                        .AddInParameter("ViewType", viewType)
                        .AddInParameter("GS_AgentId", gs_AgentId)
                        .AddInParameter("StartTime", startTime)
                        .AddInParameter("EndTime", endTime);
            var dt = query.GetDataTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    AgentRebateStatisticsInfo info = new AgentRebateStatisticsInfo();
                    info.CreateTime = row["CreateTime"] == DBNull.Value ? Convert.ToDateTime(null) : Convert.ToDateTime(row["CreateTime"]);
                    info.DisplayName = row["DisplayName"] == DBNull.Value ? string.Empty : Convert.ToString(row["DisplayName"]);
                    info.GameCode = row["GameCode"] == DBNull.Value ? string.Empty : Convert.ToString(row["GameCode"]);
                    info.GameType = row["GameType"] == DBNull.Value ? string.Empty : Convert.ToString(row["GameType"]);
                    info.OrderTotalMoney = row["OrderTotalMoney"] == DBNull.Value ? 0M : Convert.ToDecimal(row["OrderTotalMoney"]);
                    info.PayMoney = row["PayMoney"] == DBNull.Value ? 0M : Convert.ToDecimal(row["PayMoney"]);
                    info.Rebate = row["Rebate"] == DBNull.Value ? 0M : Convert.ToDecimal(row["Rebate"]);
                    info.SchemeId = row["SchemeId"] == DBNull.Value ? string.Empty : Convert.ToString(row["SchemeId"]);
                    info.UserId = row["UserId"] == DBNull.Value ? string.Empty : Convert.ToString(row["UserId"]);
                    collection.AgentRebateList.Add(info);
                }
            }
            return collection;
        }
        public SendMsgHistoryRecord_Collection QueryFailMsgList()
        {
            Session.Clear();
            SendMsgHistoryRecord_Collection collection = new SendMsgHistoryRecord_Collection();
            collection.TotalCount = 0;
            var query = from o in Session.Query<Sports_Order_Complate>()
                        join m in Session.Query<SendMsgHistoryRecord>() on o.SchemeId equals m.SchemeId
                        where (o.ProgressStatus == ProgressStatus.Aborted || o.ProgressStatus == ProgressStatus.AutoStop) && m.MsgResultStatus != "4072" && m.MsgResultStatus != "2" && m.MsgResultStatus != "406" && m.SendNumber <= 6 && m.SchemeId != string.Empty && o.CreateTime >= DateTime.Now.Date
                        select new SendMsgHistoryRecordInfo
                        {
                            PhoneNumber = m.PhoneNumber,
                            UserId = m.UserId,
                            SchemeId = m.SchemeId,
                            CreateTime = m.CreateTime,
                            MsgId = m.MsgId,
                            MsgContent = m.MsgContent
                        };
            var _query = from o in Session.Query<OrderDetail>()
                         join m in Session.Query<SendMsgHistoryRecord>() on o.SchemeId equals m.SchemeId
                         where (o.TicketStatus == TicketStatus.Ticketed) && o.CurrentBettingMoney != o.TotalMoney && m.MsgResultStatus != "4072" && m.MsgResultStatus != "2" && m.MsgResultStatus != "406" && m.SendNumber <= 6 && m.SchemeId != string.Empty && o.CreateTime >= DateTime.Now.Date
                         select new SendMsgHistoryRecordInfo
                         {
                             PhoneNumber = m.PhoneNumber,
                             UserId = m.UserId,
                             SchemeId = m.SchemeId,
                             CreateTime = m.CreateTime,
                             MsgId = m.MsgId,
                             MsgContent = m.MsgContent
                         };
            var strSql = "select isnull(PhoneNumber,'')PhoneNumber,isnull(UserId,'')UserId,isnull(SchemeId,'')SchemeId,CreateTime,MsgId,isnull(MsgContent,'')MsgContent  from E_SendMsg_HistoryRecord where MsgContent like '%http://www.qcw.com%' and MsgResultStatus!='2' and MsgResultStatus!='406' and MsgResultStatus!='4072' and CreateTime>='" + DateTime.Now.Date + "'";
            var query_sy = Session.CreateSQLQuery(strSql).List();
            if (query_sy != null && query_sy.Count > 0)
            {
                collection.TotalCount += query_sy.Count;
                foreach (var item in query_sy)
                {
                    var array = item as object[];
                    SendMsgHistoryRecordInfo info = new SendMsgHistoryRecordInfo();
                    info.PhoneNumber = Convert.ToString(array[0]);
                    info.UserId = Convert.ToString(array[1]);
                    info.SchemeId = Convert.ToString(array[2]);
                    info.CreateTime = Convert.ToDateTime(array[3]);
                    info.MsgId = Convert.ToInt32(array[4]);
                    info.MsgContent = Convert.ToString(array[5]);
                    collection.ListHistoryRecord.Add(info);
                }
            }
            if (query != null && query.Count() > 0)
            {
                collection.TotalCount += query.Count();
                collection.ListHistoryRecord.AddRange(query.ToList());
            }
            if (_query != null && _query.Count() > 0)
            {
                collection.TotalCount += _query.Count();
                collection.ListHistoryRecord.AddRange(_query.ToList());
            }
            if (collection != null)
                collection.ListHistoryRecord = collection.ListHistoryRecord.OrderBy(s => s.CreateTime).ToList();
            return collection;
        }

        public IList<BettingOrderInfo> QueryVirtualOrderList(string userIdOrName, SchemeType? schemeType, ProgressStatus? progressStatus, BonusStatus? bonusStatus, SchemeBettingCategory? betCategory, string gameCode
             , DateTime startTime, DateTime endTime, int pageIndex, int pageSize
             , out int totalCount, out int totalUserCount, out decimal totalBuyMoney, out decimal totalOrderMoney, out decimal totalPreTaxBonusMoney, out decimal totalAfterTaxBonusMoney, out decimal totalAddMoney, TicketStatus? ticketStatus = null, SchemeSource? schemeSource = null)
        {
            startTime = startTime.Date.AddHours(1);
            endTime = endTime.Date.AddDays(1).AddHours(1);
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            if (pageSize == -1)
                pageSize = int.MaxValue;
            else
                pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from o in this.Session.Query<OrderDetail>()
                        join u in this.Session.Query<UserRegister>() on o.UserId equals u.UserId
                        join g in this.Session.Query<LotteryGame>() on o.GameCode equals g.GameCode
                        where (userIdOrName == string.Empty || o.UserId == userIdOrName)
                        && (schemeType == null || o.SchemeType == schemeType)
                        && (progressStatus == null || o.ProgressStatus == progressStatus)
                        && (bonusStatus == null || o.BonusStatus == bonusStatus)
                        && (betCategory == null || o.SchemeBettingCategory == betCategory)
                        && (gameCode == string.Empty || o.GameCode == gameCode)
                        && (o.BetTime > startTime && o.BetTime <= endTime)
                        && (ticketStatus == null || o.TicketStatus == ticketStatus)
                        && (schemeSource == null || o.SchemeSource == schemeSource)
                        && o.IsVirtualOrder == true
                        select new BettingOrderInfo
                        {
                            AddMoney = o.AddMoney,
                            AfterTaxBonusMoney = o.AfterTaxBonusMoney,
                            AgentId = o.AgentId,
                            Amount = o.Amount,
                            BonusStatus = o.BonusStatus,
                            CreateTime = o.CreateTime,
                            CreatorDisplayName = u.DisplayName,
                            CurrentBettingMoney = o.CurrentBettingMoney,
                            GameCode = g.GameCode,
                            GameName = g.DisplayName,
                            GameTypeName = (o.SchemeBettingCategory == SchemeBettingCategory.ErXuanYi ? "主客二选一" : o.GameTypeName),
                            HideDisplayNameCount = u.HideDisplayNameCount,
                            IssuseNumber = o.CurrentIssuseNumber,
                            PlayType = o.PlayType,
                            PreTaxBonusMoney = o.PreTaxBonusMoney,
                            ProgressStatus = o.ProgressStatus,
                            RowNumber = 0,
                            SchemeBettingCategory = o.SchemeBettingCategory,
                            SchemeId = o.SchemeId,
                            SchemeSource = o.SchemeSource,
                            SchemeType = o.SchemeType,
                            StopAfterBonus = o.StopAfterBonus,
                            TicketStatus = o.TicketStatus,
                            TotalIssuseCount = o.TotalIssuseCount,
                            TotalMoney = o.TotalMoney,
                            UserId = o.UserId,
                            VipLevel = u.VipLevel,
                            Security = TogetherSchemeSecurity.Public,
                            IsVirtualOrder = o.IsVirtualOrder,
                            BetTime = o.BetTime,
                        };
            totalCount = query.Count();
            if (totalCount == 0)
            {
                totalUserCount = 0;
            }
            else
            {
                var t = from o in this.Session.Query<OrderDetail>()
                        where (userIdOrName == string.Empty || o.UserId == userIdOrName)
                        && (schemeType == null || o.SchemeType == schemeType)
                        && (progressStatus == null || o.ProgressStatus == progressStatus)
                        && (bonusStatus == null || o.BonusStatus == bonusStatus)
                        && (betCategory == null || o.SchemeBettingCategory == betCategory)
                        && (gameCode == string.Empty || o.GameCode == gameCode)
                        && (o.BetTime > startTime && o.BetTime <= endTime)
                        && o.IsVirtualOrder == true
                        group o by o.UserId into g
                        select g.Key;

                var array = t.ToArray();
                totalUserCount = array.Count();
            }
            var rightQuery = query.Where(o => o.TicketStatus == TicketStatus.Ticketed);
            totalBuyMoney = totalCount == 0 ? 0M : rightQuery.Count() == 0 ? 0M : rightQuery.Sum(p => p.CurrentBettingMoney);
            totalOrderMoney = totalCount == 0 ? 0M : query.Sum(p => p.TotalMoney);
            totalPreTaxBonusMoney = totalCount == 0 ? 0M : rightQuery.Count() == 0 ? 0M : rightQuery.Sum(p => p.PreTaxBonusMoney);
            totalAfterTaxBonusMoney = totalCount == 0 ? 0M : rightQuery.Count() == 0 ? 0M : rightQuery.Sum(p => p.AfterTaxBonusMoney);
            totalAddMoney = totalCount == 0 ? 0M : query.Where(s => s.IsVirtualOrder == false).Count() == 0 ? 0 : query.Sum(p => p.AddMoney);

            var query2 = from o in this.Session.Query<OrderDetail>()
                         join u in this.Session.Query<UserRegister>() on o.UserId equals u.UserId
                         join g in this.Session.Query<LotteryGame>() on o.GameCode equals g.GameCode
                         where (userIdOrName == string.Empty || o.UserId == userIdOrName)
                         && (schemeType == null || o.SchemeType == schemeType)
                         && (progressStatus == null || o.ProgressStatus == progressStatus)
                         && (bonusStatus == null || o.BonusStatus == bonusStatus)
                         && (betCategory == null || o.SchemeBettingCategory == betCategory)
                         && (gameCode == string.Empty || o.GameCode == gameCode)
                         && (o.BetTime > startTime && o.BetTime <= endTime)
                         && (ticketStatus == null || o.TicketStatus == ticketStatus)
                         && o.IsVirtualOrder == true
                         orderby o.CreateTime descending
                         select new BettingOrderInfo
                         {
                             AddMoney = o.AddMoney,
                             AfterTaxBonusMoney = o.AfterTaxBonusMoney,
                             AgentId = o.AgentId,
                             Amount = o.Amount,
                             BonusStatus = o.BonusStatus,
                             CreateTime = o.CreateTime,
                             CreatorDisplayName = u.DisplayName,
                             CurrentBettingMoney = o.CurrentBettingMoney,
                             GameCode = g.GameCode,
                             GameName = g.DisplayName,
                             GameTypeName = (o.SchemeBettingCategory == SchemeBettingCategory.ErXuanYi ? "主客二选一" : o.GameTypeName),
                             HideDisplayNameCount = u.HideDisplayNameCount,
                             IssuseNumber = o.CurrentIssuseNumber,
                             PlayType = o.PlayType,
                             PreTaxBonusMoney = o.PreTaxBonusMoney,
                             ProgressStatus = o.ProgressStatus,
                             RowNumber = 0,
                             SchemeBettingCategory = o.SchemeBettingCategory,
                             SchemeId = o.SchemeId,
                             SchemeSource = o.SchemeSource,
                             SchemeType = o.SchemeType,
                             StopAfterBonus = o.StopAfterBonus,
                             TicketStatus = o.TicketStatus,
                             TotalIssuseCount = o.TotalIssuseCount,
                             TotalMoney = o.TotalMoney,
                             UserId = o.UserId,
                             VipLevel = u.VipLevel,
                             Security = TogetherSchemeSecurity.Public,
                             BetTime = o.BetTime,
                             IsVirtualOrder = o.IsVirtualOrder,
                         };
            return query2.Skip(pageIndex * pageSize).Take(pageSize).ToList();

        }


        #region 后台延误开奖订单列表

        public string QueryJCDelayPrizeMatchId()
        {
            Session.Clear();
            string strMatchId = string.Empty;
            string strJCZQ = string.Empty;
            string strJCLQ = string.Empty;
            //var query = from o in Session.Query<Sports_Order_Running>()
            //            join a in Session.Query<Sports_AnteCode>() on o.SchemeId equals a.SchemeId
            //            where o.BonusStatus == BonusStatus.Waitting && o.IsVirtualOrder == false && o.CanChase == true && o.StopTime <= DateTime.Now && (o.GameCode == "JCLQ" || o.GameCode == "JCZQ") select a.MatchId;
            //if (query != null && query.Count() > 0)
            //{
            //    return string.Join(",", query.ToList());
            //}
            string strSql = "select a.GameCode,a.MatchId from C_Sports_Order_Running r inner join C_Sports_AnteCode a on r.SchemeId=a.SchemeId where r.BonusStatus=0 and IsVirtualOrder=0 and CanChase=1 and StopTime<GETDATE() and r.GameCode in ('JCZQ','JCLQ') group by a.GameCode,a.MatchId";
            var result = Session.CreateSQLQuery(strSql).List();
            if (result != null && result.Count > 0)
            {
                strJCLQ = "JCLQ|";
                strJCZQ = "JCZQ|";
                foreach (var item in result)
                {
                    var array = item as object[];
                    if (array != null && array.Length == 2)
                    {
                        if (array[0].ToString() == "JCLQ")
                            strJCLQ += array[1] + ",";
                        else if (array[1].ToString() == "JCZQ")
                            strJCZQ += array[1] + ",";
                    }

                }
                if (!string.IsNullOrEmpty(strJCZQ) || !string.IsNullOrEmpty(strJCLQ))
                {
                    strJCLQ = strJCLQ.TrimEnd(',');
                    strJCZQ = strJCZQ.TrimEnd(',');
                    strMatchId = strJCLQ + "^" + strJCZQ;
                }
            }
            return strMatchId;
        }
        public List<DelayPrizeOrderInfo> QuerySZCDelayPrizeOrderList(List<string> gameCodeList)
        {
            Session.Clear();
            var query = from o in Session.Query<Sports_Order_Running>()
                        join u in Session.Query<UserRegister>() on o.UserId equals u.UserId
                        where o.BonusStatus == BonusStatus.Waitting && o.IsVirtualOrder == false && o.StopTime <= DateTime.Now && o.CanChase == true && gameCodeList.Contains(o.GameCode)
                        select new DelayPrizeOrderInfo
                        {
                            UserId = u.UserId,
                            UserName = u.DisplayName,
                            SchemeId = o.SchemeId,
                            GameCode = o.GameCode,
                            GameType = o.GameType,
                            TotalMoney = o.TotalMoney,
                            SuccessMoney = o.SuccessMoney,
                            PlayType = o.PlayType,
                            IssuseNumber = o.IssuseNumber,
                            Amount = o.Amount,
                            BetCount = o.BetCount,
                            CreateTime = o.CreateTime,
                            BetTime = o.BetTime,
                            SchemeType = o.SchemeType,
                            SchemeSource = o.SchemeSource,
                            SchemeBettingCategory = o.SchemeBettingCategory,
                            TicketStatus = o.TicketStatus,
                            ProgressStatus = o.ProgressStatus,
                            BonusStatus = o.BonusStatus,
                        };
            if (query != null && query.Count() > 0)
                return query.ToList();
            return new List<DelayPrizeOrderInfo>();
        }

        public List<DelayPrizeOrderInfo> QueryJCDelayPrizeOrderList(List<string> orderIdList)
        {
            Session.Clear();
            var query = from o in Session.Query<Sports_Order_Running>()
                        join u in Session.Query<UserRegister>() on o.UserId equals u.UserId
                        where o.BonusStatus == BonusStatus.Waitting && o.IsVirtualOrder == false && o.StopTime <= DateTime.Now && orderIdList.Contains(o.SchemeId) && o.CanChase == true
                        select new DelayPrizeOrderInfo
                        {
                            UserId = u.UserId,
                            UserName = u.DisplayName,
                            SchemeId = o.SchemeId,
                            GameCode = o.GameCode,
                            GameType = o.GameType,
                            TotalMoney = o.TotalMoney,
                            SuccessMoney = o.SuccessMoney,
                            PlayType = o.PlayType,
                            IssuseNumber = o.IssuseNumber,
                            Amount = o.Amount,
                            BetCount = o.BetCount,
                            CreateTime = o.CreateTime,
                            BetTime = o.BetTime,
                            SchemeType = o.SchemeType,
                            SchemeSource = o.SchemeSource,
                            SchemeBettingCategory = o.SchemeBettingCategory,
                            TicketStatus = o.TicketStatus,
                            ProgressStatus = o.ProgressStatus,
                            BonusStatus = o.BonusStatus,
                        };
            if (query != null && query.Count() > 0)
                return query.ToList();
            return new List<DelayPrizeOrderInfo>();
        }

        #endregion

        public int QueryTogetherFollowerCount(string createUserId)
        {
            Session.Clear();
            var result = Session.Query<TogetherFollowerRule>().Where(s => s.CreaterUserId == createUserId);
            if (result != null && result.Count() > 0)
                return result.Count();
            return 0;
        }

        public bool IsUserValidateRealName(string userId)
        {
            Session.Clear();
            var sql = string.Format(@" SELECT count(*) c  FROM  [E_Authentication_RealName]  where userid='{0}' and [IsSettedRealName]=1 and [RealName]<>'' and [IdCardNumber]<>''", userId);
            return this.Session.CreateSQLQuery(sql).UniqueResult<int>() > 0;
        }

        /// <summary>
        /// 查询某个yqid下面的 能满足领红包条件的用户个数
        /// </summary>
        /// <param name="AgentId">普通用户代理 邀请注册的会员</param>
        /// <returns></returns>
        public string QueryYqidRegisterByAgentId(string AgentId)
        {
            Session.Clear();
            // 通过数据库存储过程进行查询
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Blog_UserSpreadGiveRedBagCount"))
                .AddInParameter("AgentId", AgentId);
            var ds = query.GetDataSet();
            string str = string.Empty;
            if (ds.Tables.Count == 3)
            {
                var dtCount = ds.Tables[0];
                if (dtCount.Rows.Count > 0)
                {
                    var row = dtCount.Rows[0];
                    str += UsefullHelper.GetDbValue<int>(row[0]) + "|";
                }
                var dtSumCount = ds.Tables[1];
                if (dtSumCount.Rows.Count > 0)
                {
                    var row = dtSumCount.Rows[0];
                    str += UsefullHelper.GetDbValue<int>(row[0]) + "|";
                }
                var dtRedBagMoney = ds.Tables[2];
                if (dtRedBagMoney.Rows.Count > 0)
                {
                    var row = dtRedBagMoney.Rows[0];
                    str += UsefullHelper.GetDbValue<int>(row[0]);
                }
            }
            return str;
        }
    }
}
