using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business;
using External.Domain.Entities.Agent;
using Common.Utilities;
using System.Collections;
using NHibernate.Linq;
using External.Core.Agnet;
using Common.Database.ORM;
using GameBiz.Domain.Entities;
using External.Core.Login;

namespace External.Domain.Managers.Agent
{
    public class AgentManager : GameBizEntityManagement
    {
        //public void AddAgentUser(AgentUsers entity)
        //{
        //    this.Add<AgentUsers>(entity);
        //}
        public void AddAgentReturnPoint(params AgentReturnPointReality[] entity)
        {
            this.Add<AgentReturnPointReality>(entity);
        }
        //public void AddAgentReturnPoint(AgentReturnPoint[] collection)
        //{
        //    this.Add<AgentReturnPoint>(collection);
        //}
        public void AddAgentCommissionApply(AgentCommissionApply entity)
        {
            this.Add<AgentCommissionApply>(entity);
        }
        public void AddAgentCommissionDetail(AgentCommissionDetail entity)
        {
            this.Add<AgentCommissionDetail>(entity);
        }
        public void AddRebateDetail(RebateDetail entity)
        {
            this.Add<RebateDetail>(entity);
        }
        public void AddAgentCaculateHistory(AgentCaculateHistory entity)
        {
            this.Add<AgentCaculateHistory>(entity);
        }
        public void AddAgentReturnInitialPoint(params AgentReturnPoint[] entity)
        {
            this.Add<AgentReturnPoint>(entity);
        }

        public void UpdateAgentUser(UserRegister userRegister)
        {
            this.Update<UserRegister>(userRegister);
        }
        public void UpdateAgentReturnPoint(params AgentReturnPointReality[] agentReturnPoint)
        {
            this.Update<AgentReturnPointReality>(agentReturnPoint);
        }
        public void UpdateAgentReturnInitialPoint(params AgentReturnPoint[] agentReturnPoint)
        {
            this.Update<AgentReturnPoint>(agentReturnPoint);
        }
        public void UpdateAgentCommissionApply(AgentCommissionApply entity)
        {
            this.Update<AgentCommissionApply>(entity);
        }
        public void UpdateAgentCommissionDetail(IList<AgentCommissionDetail> agentCommissionApplyList)
        {
            this.Update<AgentCommissionDetail>(agentCommissionApplyList.ToArray<AgentCommissionDetail>());
        }




        public AgentCaculateHistory GetAgentCaculateHistoryByDesc()
        {
            Session.Clear();

            var query = from a in this.Session.Query<AgentCaculateHistory>()
                        orderby a.CreateTime descending
                        select a;
            return query.FirstOrDefault<AgentCaculateHistory>();
        }

        /// <summary>
        /// 查询代理用户
        /// </summary>
        public IList<AgentUserInfo> GetAgentUserByKeyword(string keyword, int isAgent, string pAgentId, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();

            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Agent_QueryUserList"));
            query = query.AddInParameter("keyword", keyword);
            query = query.AddInParameter("pAgentId", pAgentId);
            query = query.AddInParameter("isAgent", isAgent);
            query = query.AddInParameter("pageIndex", pageIndex);
            query = query.AddInParameter("pageSize", pageSize);
            query = query.AddOutParameter("totalCount", "Int32");
            var dt = query.GetDataTable(out outputs);
            totalCount = UsefullHelper.GetDbValue<int>(outputs["totalCount"]);
            return ORMHelper.DataTableToList<AgentUserInfo>(dt);
        }

        public IList<AgentCommissionDetailInfo> GetAgentCommissionDetailList(string pAgentId, string userId, string displayName, DateTime fromDate, DateTime toDate, int pageIndex, int pageSize, int applyState, out int totalCount)
        {
            Session.Clear();

            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Agent_QueryCommissionDetailList"));
            query = query.AddInParameter("fromDate", fromDate);
            query = query.AddInParameter("toDate", toDate);
            query = query.AddInParameter("pAgentId", pAgentId);
            query = query.AddInParameter("userId", userId);
            query = query.AddInParameter("displayName", displayName);
            query = query.AddInParameter("pageIndex", pageIndex);
            query = query.AddInParameter("pageSize", pageSize);
            query = query.AddInParameter("applyState", applyState);
            query = query.AddOutParameter("totalCount", "Int32");
            var dt = query.GetDataTable(out outputs);
            totalCount = UsefullHelper.GetDbValue<int>(outputs["totalCount"]);
            return ORMHelper.DataTableToList<AgentCommissionDetailInfo>(dt);
        }

        public AgentCommissionDetailCollection GetAgentGetCommissionReportList(string gameCode, string gameType, string pAgentId, string userId, string displayName, int category, DateTime fromDate, DateTime toDate, int pageIndex, int pageSize)
        {
            Session.Clear();

            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Agent_GetCommissionReportList"));
            query = query.AddInParameter("GameCode", gameCode);
            query = query.AddInParameter("GameType", gameType);
            query = query.AddInParameter("PAgentId", pAgentId);
            query = query.AddInParameter("UserId", userId);
            query = query.AddInParameter("DisplayName", displayName);
            query = query.AddInParameter("Category", category);
            query = query.AddInParameter("FromDate", fromDate);
            query = query.AddInParameter("ToDate", toDate);
            query = query.AddInParameter("PageIndex", pageIndex);
            query = query.AddInParameter("PageSize", pageSize);
            query = query.AddOutParameter("TotalCount", "Int32");
            var ds = query.GetDataSet(out outputs);
            var totalCount = UsefullHelper.GetDbValue<int>(outputs["TotalCount"]);
            var listReport = ORMHelper.DataTableToList<AgentCommissionDetailInfo>(ds.Tables[0]);
            var list = ORMHelper.DataTableToList<AgentCommissionDetailInfo>(ds.Tables[1]);
            var saleTotal = listReport.Sum(o => o.Sale);
            var actualCommissionTotal = listReport.Sum(o => o.BeforeCommission);
            return new AgentCommissionDetailCollection()
            {
                AgentCommissionDetailList = list,
                AgentCommissionReport = listReport,
                ActualCommissionTotal = actualCommissionTotal,
                SaleTotal = saleTotal,
                TotalCount = totalCount
            };
        }

        public IList<AgentCommissionApplyInfo> GetAgentCommissionApplyList(DateTime fromDate, DateTime toDate, int pageIndex, int pageSize, int applyState, string pAgentId, string userId, out int totalCount)
        {
            Session.Clear();

            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Agent_QueryCommissionApplyList"));
            query = query.AddInParameter("fromDate", fromDate);
            query = query.AddInParameter("toDate", toDate);
            query = query.AddInParameter("pageIndex", pageIndex);
            query = query.AddInParameter("pageSize", pageSize);
            query = query.AddInParameter("applyState", applyState);
            query = query.AddInParameter("pAgentId", pAgentId);
            query = query.AddInParameter("userId", userId);
            query = query.AddOutParameter("totalCount", "Int32");
            var dt = query.GetDataTable(out outputs);
            totalCount = UsefullHelper.GetDbValue<int>(outputs["totalCount"]);
            return ORMHelper.DataTableToList<AgentCommissionApplyInfo>(dt);
        }

        public IList<AgentWaitingCommissionOrderInfo> QueryAgentWaitingCommissionOrderList(DateTime fromDate, DateTime toDate, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();

            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Agent_QueryWaitingCommissionOrderList"));
            query = query.AddInParameter("fromDate", fromDate);
            query = query.AddInParameter("toDate", toDate);
            query = query.AddInParameter("pageIndex", pageIndex);
            query = query.AddInParameter("pageSize", pageSize);
            query = query.AddOutParameter("totalCount", "Int32");
            var dt = query.GetDataTable(out outputs);
            totalCount = UsefullHelper.GetDbValue<int>(outputs["totalCount"]);

            return ORMHelper.DataTableToList<AgentWaitingCommissionOrderInfo>(dt);
        }

        public IList<AgentLottoTopInfo> QueryAgentLottoTopList(string agentId, string userId, string displayName, DateTime fromDate, DateTime toDate, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();

            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Agent_QueryLottoTopList"));
            query = query.AddInParameter("agentId", agentId);
            query = query.AddInParameter("userId", userId);
            query = query.AddInParameter("displayName", displayName);
            query = query.AddInParameter("fromDate", fromDate);
            query = query.AddInParameter("toDate", toDate);
            query = query.AddInParameter("pageIndex", pageIndex);
            query = query.AddInParameter("pageSize", pageSize);
            query = query.AddOutParameter("totalCount", "Int32");
            var dt = query.GetDataTable(out outputs);
            totalCount = UsefullHelper.GetDbValue<int>(outputs["totalCount"]);

            return ORMHelper.DataTableToList<AgentLottoTopInfo>(dt);
        }

        public IList<AgentFillMoneyTopInfo> QueryAgentFillMoneyTopList(string agentId, string userId, string displayName, DateTime fromDate, DateTime toDate, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();

            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Agent_QueryFillMoneyTopList"));
            query = query.AddInParameter("agentId", agentId);
            query = query.AddInParameter("userId", userId);
            query = query.AddInParameter("displayName", displayName);
            query = query.AddInParameter("fromDate", fromDate);
            query = query.AddInParameter("toDate", toDate);
            query = query.AddInParameter("pageIndex", pageIndex);
            query = query.AddInParameter("pageSize", pageSize);
            query = query.AddOutParameter("totalCount", "Int32");
            var dt = query.GetDataTable(out outputs);
            totalCount = UsefullHelper.GetDbValue<int>(outputs["totalCount"]);

            return ORMHelper.DataTableToList<AgentFillMoneyTopInfo>(dt);
        }

        public IList<AgentSchemeInfo> GetAgentScheme(string agentId, string userId, string displayName, int progressStatus, int ticketStatus
            , DateTime fromDate, DateTime toDate, int pageIndex, int pageSize, out int totalCount, out int totalUser, out int totalScheme, out decimal totalMoney1)
        {
            Session.Clear();

            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;

            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Agent_GetScheme"));
            query = query.AddInParameter("agentId", agentId);
            query = query.AddInParameter("userId", userId);
            query = query.AddInParameter("displayName", displayName);
            query = query.AddInParameter("progressStatus", progressStatus);
            query = query.AddInParameter("ticketStatus", ticketStatus);
            query = query.AddInParameter("fromDate", fromDate);
            query = query.AddInParameter("toDate", toDate);
            query = query.AddInParameter("pageIndex", pageIndex);
            query = query.AddInParameter("pageSize", pageSize);
            query = query.AddOutParameter("totalCount", "Int32");
            var ds = query.GetDataSet(out outputs);
            totalCount = UsefullHelper.GetDbValue<int>(outputs["totalCount"]);
            totalUser = UsefullHelper.GetDbValue<int>(ds.Tables[1].Rows[0]["TotalUser"]);
            totalScheme = UsefullHelper.GetDbValue<int>(ds.Tables[1].Rows[0]["TotalScheme"]);
            totalMoney1 = UsefullHelper.GetDbValue<decimal>(ds.Tables[1].Rows[0]["TotalMoney1"]);
            return ORMHelper.DataTableToList<AgentSchemeInfo>(ds.Tables[0]);
        }

        public AgentWithdrawRecordCollection GetAgentWithdrawRecord(string userId, DateTime fromDate, DateTime toDate, int pageIndex, int pageSize)
        {
            Session.Clear();
            int totalCount = 0;

            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;

            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Agent_GetWithdrawRecord"));
            query = query.AddInParameter("userId", userId);
            query = query.AddInParameter("fromDate", fromDate);
            query = query.AddInParameter("toDate", toDate);
            query = query.AddInParameter("pageIndex", pageIndex);
            query = query.AddInParameter("pageSize", pageSize);
            query = query.AddOutParameter("totalCount", "Int32");
            var ds = query.GetDataSet(out outputs);
            totalCount = UsefullHelper.GetDbValue<int>(outputs["totalCount"]);
            var list = ORMHelper.DataTableToList<AgentWithdrawRecordInfo>(ds.Tables[0]);

            var requestingMoney = UsefullHelper.GetDbValue<decimal>(ds.Tables[1].Rows[0]["RequestingMoney"]);
            var successMoney = UsefullHelper.GetDbValue<decimal>(ds.Tables[1].Rows[0]["SuccessMoney"]);
            var refusedMoney = UsefullHelper.GetDbValue<decimal>(ds.Tables[1].Rows[0]["RefusedMoney"]);

            return new AgentWithdrawRecordCollection()
            {
                AgentWithdrawRecordList = list,
                TotalCount = totalCount,
                RequestingMoney = requestingMoney,
                SuccessMoney = successMoney,
                RefusedMoney = refusedMoney
            };
        }

        //public IList<AgentReturnPoint> GetAgentReturnPointByAgentIdFromAndAgentIdTo(string agentIdFrom, string agentIdTo)
        //{
        //    Session.Clear();

        //    var query = from a in this.Session.Query<AgentReturnPoint>()
        //                where a.AgentIdFrom == agentIdFrom && (a.AgentIdTo == agentIdTo || a.AgentIdTo == "-")
        //                orderby a.SetLevel descending
        //                select a;
        //    return query.ToList<AgentReturnPoint>();
        //}

        public AgentCommissionApply GetAgentCommissionApply(string applyOrderId)
        {
            Session.Clear();

            var query = from a in this.Session.Query<AgentCommissionApply>()
                        where a.ID == applyOrderId
                        select a;
            return query.FirstOrDefault<AgentCommissionApply>();
        }

        public AgentCloseReturnPointInfo GetAgentReturnPointByUserId(string userId, string gameCode, string gameType)
        {
            try
            {
                Session.Clear();

                var query = from a in this.Session.Query<AgentReturnPointReality>()
                            join o in this.Session.Query<UserRegister>() on a.UserId equals o.UserId
                            join o1 in this.Session.Query<AgentReturnPointReality>() on new { UserId = o.AgentId, a.GameCode, a.GameType } equals new { o1.UserId, o1.GameCode, o1.GameType }
                            where a.UserId == userId && a.GameCode == gameCode && a.GameType == gameType
                            select new AgentCloseReturnPointInfo()
                            {
                                AgentId = a.UserId,
                                ReturnPoint = a.MyPoint.HasValue ? a.MyPoint.Value : 0,
                                GameCode = a.GameCode,
                                GameType = a.GameType,
                                PAgentId = o.AgentId,
                                PReturnPoint = o1.MyPoint.HasValue ? o1.MyPoint.Value : 0,
                            };
                return query.FirstOrDefault();

            }
            catch (Exception ex)
            {
                var writer = Common.Log.LogWriterGetter.GetLogWriter();
                writer.Write("EXEC_Plugin_GetAgentReturnPointByUserId_Error_", userId, ex);
                return null;
            }
            //var query = CreateOutputQuery(Session.GetNamedQuery("P_Agent_GetReturnPointByUserId"));
            //query = query.AddInParameter("userId", userId);
            //query = query.AddInParameter("gameCode", gameCode);
            //query = query.AddInParameter("gameType", gameType);
            //var dt = query.GetDataTable();
            //if (dt.Rows.Count == 0)
            //{
            //    return null;
            //}
            //return ORMHelper.ConvertDataRowToEntity<AgentCloseReturnPointInfo>(dt.Rows[0]);
        }

        //public AgentReturnPointCollection GetAgentReturnPointList(string agentIdFrom, string agentIdTo, bool isUpAndLower)
        //{
        //    Session.Clear();

        //    var query = CreateOutputQuery(Session.GetNamedQuery("P_Agent_GetReturnPointList"));
        //    query = query.AddInParameter("agentIdFrom", agentIdFrom);
        //    query = query.AddInParameter("agentIdTo", agentIdTo);
        //    query = query.AddInParameter("isUpAndLower", isUpAndLower ? 0 : 1);
        //    var ds = query.GetDataSet();

        //    var collection = new AgentReturnPointCollection();
        //    collection.AgentReturnPointList = ORMHelper.DataTableToList<AgentReturnPointInfo>(ds.Tables[0]);
        //    if (isUpAndLower)
        //    {
        //        collection.AgentReturnPointListByUp = ORMHelper.DataTableToList<AgentReturnPointInfo>(ds.Tables[1]);
        //        collection.AgentReturnPointListByLower = ORMHelper.DataTableToList<AgentReturnPointInfo>(ds.Tables[2]);
        //    }

        //    return collection;
        //}


        public IList<AgentReturnPointRealityInfo> GetAgentReturnPointByHighAndLow(string userId)
        {
            Session.Clear();

            //var query = CreateOutputQuery(Session.GetNamedQuery("P_Agent_GetReturnPointList"));
            //query = query.AddInParameter("userId", userId);
            //var dt = query.GetDataTable();

            //return ORMHelper.DataTableToList<AgentReturnPointRealityInfo>(dt);

            var query = from s in Session.Query<AgentReturnPointReality>()
                        where s.UserId == userId
                        select new AgentReturnPointRealityInfo
                        {
                            UserId = s.UserId,
                            GameCode = s.GameCode,
                            GameType = s.GameType,
                            LowerPoint = s.LowerPoint.HasValue ? s.LowerPoint.Value : 0,
                            MyPoint = s.MyPoint.HasValue ? s.MyPoint.Value : 0,
                        };
            if (query != null)
                return query.ToList();
            return new List<AgentReturnPointRealityInfo>();

        }
        public IList<AgentReturnPointReality> GetAgentReturnPointList(string userId)
        {
            Session.Clear();

            var query = from a in this.Session.Query<AgentReturnPointReality>()
                        where a.UserId == userId
                        select a;
            return query.ToList();
        }
        public IList<AgentReturnPoint> GetAgentReturnPointListByUserId(string userid)
        {
            Session.Clear();
            var query = from n in this.Session.Query<AgentReturnPoint>()
                        where (userid == string.Empty || n.AgentIdFrom == userid)
                        select n;
            if (query != null)
            {
                if (string.IsNullOrEmpty(userid))
                    return query.Where(p => p.AgentIdFrom == string.Empty).ToList();
                else
                    return query.ToList();
            }
            return new List<AgentReturnPoint>();

        }

        public IList<AgentCommissionDetailInfo> GetCommissionReport(string userId, string displayName, DateTime fromDate, DateTime toDate, string pAgentId, int applyState, out decimal saleTotal, out decimal actualCommissionTotal)
        {
            Session.Clear();

            Dictionary<string, object> outputs;
            var query = CreateOutputQuery(Session.GetNamedQuery("P_Agent_GetCommissionReport"));
            query = query.AddInParameter("pAgentId", pAgentId);
            query = query.AddInParameter("userId", userId);
            query = query.AddInParameter("displayName", displayName);
            query = query.AddInParameter("fromDate", fromDate);
            query = query.AddInParameter("toDate", toDate);
            query = query.AddInParameter("applyState", applyState);
            query = query.AddOutParameter("saleTotal", "Decimal");
            query = query.AddOutParameter("actualCommissionTotal", "Decimal");
            var dt = query.GetDataTable(out outputs);
            var list = ORMHelper.DataTableToList<AgentCommissionDetailInfo>(dt);
            saleTotal = list.Sum(o => o.Sale);
            actualCommissionTotal = list.Sum(o => o.BeforeCommission);
            return list;
        }

        public IList<AgentCommissionDetail> GetCommissionReportByFromTimeAndToDateAndPAgentId(DateTime fromDate, DateTime toDate, string pAgentId, int applyState)
        {
            Session.Clear();

            var query = from a in this.Session.Query<AgentCommissionDetail>()
                        where a.ComplateDateTime >= fromDate && a.ComplateDateTime < toDate && a.PAgentId == pAgentId && a.ApplyState == applyState
                        select a;
            return query.ToList<AgentCommissionDetail>();
        }

        public AgentCommissionApply AgentCommissionApplyByUserId(string userId, int applyState = 1)
        {
            Session.Clear();

            var query = from a in this.Session.Query<AgentCommissionApply>()
                        where a.RequestUserId == userId && a.ApplyState == applyState
                        orderby a.RequestTime descending
                        select a;
            return query.FirstOrDefault<AgentCommissionApply>();
        }

        public int GetUserCount(string userId, bool isAgent)
        {
            Session.Clear();
            return (from a in this.Session.Query<UserRegister>()
                    where a.AgentId == userId && a.IsAgent == isAgent
                    select a).Count();
        }

        public UserRegister GetUserByAgentIdAndUserId(string agentId, string userId)
        {
            Session.Clear();

            var query = from a in this.Session.Query<UserRegister>()
                        where a.UserId == userId && a.AgentId == agentId
                        select a;

            return query.FirstOrDefault();
        }

        public string CheckIsUserConcern(string userId, string agentId)
        {
            Session.Clear();

            var query = CreateOutputQuery(Session.GetNamedQuery("P_Agent_CheckIsUserConcern"));
            query = query.AddInParameter("AgentId", agentId);
            query = query.AddInParameter("UserId", userId);
            var dt = query.GetDataTable();
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            return agentId;
        }


        public bool GetRebateDetail(string schemeId)
        {
            Session.Clear();

            var sqlStr = "SELECT COUNT(1) FROM P_RebateDetail WHERE SchemeId='" + schemeId + "'";

            var result = Session.CreateSQLQuery(sqlStr).UniqueResult();
            return result.ToString() == "0";
        }

        /// <summary>
        /// 查询销量排行
        /// </summary>
        public AgentCommDetailByTotalSaleCollection GettCommDetailByTotalSale(DateTime fromDate, DateTime toDate)
        {
            Session.Clear();
            var collection = new AgentCommDetailByTotalSaleCollection();
            var sqlStr = @"SELECT C.PAgentId AS UserId,R.DisplayName,C.Sale AS TotalSale FROM (
                        SELECT PAgentId,SUM(Sale) AS Sale FROM P_Agent_CommissionDetail
                        WHERE ComplateDateTime>=:FromDate AND ComplateDateTime<:ToDate
                        GROUP BY PAgentId ) AS C
                        LEFT JOIN C_User_Register AS R ON R.UserId=C.PAgentId
                        ORDER BY C.Sale DESC";
            var list = Session.CreateSQLQuery(sqlStr).SetDateTime("FromDate", fromDate).SetDateTime("ToDate", toDate).List();
            foreach (var item in list)
            {
                var array = item as object[];
                collection.AgentCommDetailByTotalSaleList.Add(new AgentCommDetailByTotalSaleInfo() { UserId = (string)array[0], DisplayName = (string)array[1], TotalSale = (decimal)array[2] });
            }
            return collection;
        }
        /// <summary>
        /// 代理用户列表
        /// </summary>
        /// <returns></returns>
        public IList QueryAgentUserManagerList(DateTime createFrom, DateTime createTo, string keyType, string keyValue, int pageIndex, int pageSize, out int totalCount)
        {
            #region 构造查询语句

            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var sqlCondition = new List<string>();
            #region 条件 - 开通时间
            sqlCondition.Add(string.Format("AND O.CreateTime >=N'{0:yyyy-MM-dd}' AND O.CreateTime <=N'{1:yyyy-MM-dd}'", createFrom, createTo.AddDays(1)));
            #endregion

            #region 条件 - 关键字
            if (!string.IsNullOrEmpty(keyValue))
            {
                if (string.IsNullOrEmpty(keyType) || keyType.Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    sqlCondition.Add(string.Format("AND (R.UserId = N'{0}' OR [R].[DisplayName] LIKE N'{0}%' OR M.Mobile LIKE N'{0}%' OR N.RealName LIKE N'{0}%' OR N.IdCardNumber LIKE N'{0}%' OR E.Email LIKE N'{0}%')", keyValue));
                }
                else if (keyType.Equals("UserId", StringComparison.OrdinalIgnoreCase))
                {
                    sqlCondition.Add(string.Format("AND R.UserId = N'{0}'", keyValue));
                }
                else if (keyType.Equals("DisplayName", StringComparison.OrdinalIgnoreCase))
                {
                    sqlCondition.Add(string.Format("AND R.DisplayName LIKE N'{0}%'", keyValue));
                }
                else if (keyType.Equals("Mobile", StringComparison.OrdinalIgnoreCase))
                {
                    sqlCondition.Add(string.Format("AND M.Mobile LIKE N'{0}%'", keyValue));
                }
                else if (keyType.Equals("RealName", StringComparison.OrdinalIgnoreCase))
                {
                    sqlCondition.Add(string.Format("AND N.RealName LIKE N'{0}%'", keyValue));
                }
                else if (keyType.Equals("IdCard", StringComparison.OrdinalIgnoreCase))
                {
                    sqlCondition.Add(string.Format("AND N.IdCardNumber LIKE N'{0}%'", keyValue));
                }
                else if (keyType.Equals("Email", StringComparison.OrdinalIgnoreCase))
                {
                    sqlCondition.Add(string.Format("AND E.Email LIKE N'{0}%'", keyValue));
                }
            }
            #endregion

            var sqlBuilder_count = new StringBuilder();
            sqlBuilder_count.AppendLine("SELECT  COUNT(1) AS TotalCount,SUM(b.FillMoneyBalance) as FillMoneyBalance,SUM(b.BonusBalance) as BonusBalance,Sum(b.CommissionBalance) as CommissionBalance,SUM(ExpertsBalance)as ExpertsBalance,SUM(FreezeBalance)as FreezeBalance,SUM(RedBagBalance)as RedBagBalance FROM [C_User_Register] AS [R] with (nolock)");
            sqlBuilder_count.AppendLine("INNER JOIN [C_User_Balance] AS [B] with(nolock) ON [R].[UserId] = [B].[UserId]");
            sqlBuilder_count.AppendLine("LEFT OUTER JOIN [E_Authentication_Email] AS [E] with (nolock) ON [E].[UserId] = [R].[UserId] ");
            sqlBuilder_count.AppendLine("LEFT OUTER JOIN [E_Authentication_Mobile] AS [M] with (nolock) ON [M].[UserId] = [R].[UserId] ");
            sqlBuilder_count.AppendLine("LEFT OUTER JOIN [E_Authentication_RealName] AS [N] with (nolock) ON [N].[UserId] = [R].[UserId] ");
            sqlBuilder_count.AppendLine("LEFT OUTER JOIN [P_OCAgent] as [O] ON [O].[UserId] = [R].[UserId]");
            sqlBuilder_count.AppendLine("WHERE 1 = 1");
            sqlBuilder_count.AppendLine(string.Join(" ", sqlCondition.ToArray()));


            var sqlBuilder_query = new StringBuilder();
            sqlBuilder_query.AppendLine("SELECT t.[UserId],[DisplayName],[ComeFrom],[RegisterIp],[CreateTime],[VipLevel],IsSettedMobile,Mobile,");
            sqlBuilder_query.AppendLine("IsSettedRealName,RealName,CardType,IdCardNumber,T.AgentId,AgentRegTime,xj.xjyh,xjdl.xjdl,IsAgent,IsEnable");
            sqlBuilder_query.AppendLine("FROM (");
            sqlBuilder_query.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY [R].[CreateTime] DESC) AS [RowNumber],[R].[UserId],[R].[DisplayName],[R].[ComeFrom],[R].[RegisterIp],[R].[CreateTime],[R].[VipLevel],[M].IsSettedMobile,[M].Mobile,N.IsSettedRealName,N.RealName,N.CardType,N.IdCardNumber,R.AgentId,o.CreateTime as'AgentRegTime',R.IsAgent,R.IsEnable");
            sqlBuilder_query.AppendLine("FROM [P_OCAgent]  AS [O] with (nolock)");
            sqlBuilder_query.AppendLine("INNER JOIN[C_User_Register]as [R] ON [R].[UserId] = [O].[UserId]");
            sqlBuilder_query.AppendLine("LEFT OUTER JOIN [E_Authentication_Mobile] AS [M] with (nolock)  ON [M].[UserId] = [O].[UserId]");
            sqlBuilder_query.AppendLine("LEFT OUTER JOIN [E_Authentication_RealName] AS [N] with (nolock)  ON [N].[UserId] = [O].[UserId]");
            sqlBuilder_query.AppendLine("WHERE 1 = 1");
            sqlBuilder_query.AppendLine(string.Join(" ", sqlCondition.ToArray()));
            sqlBuilder_query.AppendLine(") AS T ");

            #region 20150331 dj 修改

            //sqlBuilder_query.AppendLine("left join(select u.UserId,COUNT(1) xjyh from C_User_Register u inner join C_User_Register r on u.UserId=r.AgentId group by u.UserId) xj on xj.UserId=t.UserId");
            //sqlBuilder_query.AppendLine("left join (select g.UserId,COUNT(1) xjdl from P_OCAgent as g inner join P_OCAgent gg on g.UserId = gg.ParentUserId group by g.UserId) xjdl on xjdl.UserId = t.UserId");

            sqlBuilder_query.AppendLine("left join(select AgentId,COUNT(1) xjyh from C_User_Register where IsAgent=0 group by AgentId) xj on xj.AgentId=t.UserId");
            sqlBuilder_query.AppendLine("left join (select AgentId,COUNT(1)xjdl from C_User_Register where IsAgent=1 group by AgentId) xjdl on xjdl.AgentId = t.userid");

            #endregion

            sqlBuilder_query.AppendLine(string.Format(" WHERE [RowNumber] > {0} AND [RowNumber] <= {1}", pageIndex * pageSize, (pageIndex + 1) * pageSize));
            #endregion

            var totalList = Session.CreateSQLQuery(sqlBuilder_count.ToString())
               .List();
            totalCount = 0;
           
            if (totalList.Count == 1)
            {
                var array = totalList[0] as object[];
                if (array.Length == 7 && Convert.ToInt32(array[0]) > 0)
                {
                    totalCount = int.Parse(array[0].ToString());
                }
            }
            return Session.CreateSQLQuery(sqlBuilder_query.ToString()).List();
        }

    }
}
