using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Business;
using NHibernate.Linq;
using GameBiz.Auth.Domain.Entities;
using NHibernate.Criterion;
using External.Domain.Entities.AdminMenu;
using GameBiz.Business;
using GameBiz.Core;
using GameBiz.Domain.Entities;

namespace External.Domain.Managers.AdminMenu
{
    public class MenuManager : GameBizEntityManagement
    {
        public IList<MenuItem> QueryMenuListByUserId(string userId)
        {
            Session.Clear();
            var sqlSb = new StringBuilder();
            sqlSb.AppendLine("SELECT * FROM [E_Menu_List] WHERE [FunctionId] IS NULL OR [FunctionId] IN (");
            sqlSb.AppendLine("	SELECT [FunctionId] FROM [C_Auth_RoleFunction] WHERE [RoleId] IN (");
            sqlSb.AppendLine("		SELECT [RoleId] FROM [C_Auth_UserRole] WHERE [UserId] = :UserId");
            sqlSb.AppendLine("	) AND [Status] = 0");
            sqlSb.AppendLine(") and MenuType in (10,1) and IsEnable=1 ORDER BY [MenuId]");

            return Session.CreateSQLQuery(sqlSb.ToString())
                .AddEntity(typeof(MenuItem))
                .SetString("UserId", userId)
                .List<MenuItem>();
        }
        public IList<MenuItem> QueryAllMenuList()
        {
            Session.Clear();
            return Session.CreateCriteria<MenuItem>()
                .AddOrder(NHibernate.Criterion.Order.Asc("MenuId"))
                .List<MenuItem>();
        }
        public FunctionCollection QueryLowerLevelFuncitonList()
        {
            Session.Clear();
            FunctionCollection collection = new FunctionCollection();
            string strSql = "select isnull(FunctionId,'') FunctionId,isnull(DisplayName,'') DisplayName,isnull(ParentId,'') ParentId,isnull(ParentPath,'') ParentPath from C_Auth_Function_List";
            var result = Session.CreateSQLQuery(strSql).List();
            if (result != null && result.Count > 0)
            {
                foreach (var item in result)
                {
                    var array = item as object[];
                    FunctionInfo info = new FunctionInfo();
                    info.FunctionId = array[0] == null ? string.Empty : array[0].ToString();
                    info.DisplayName = array[1] == null ? string.Empty : array[1].ToString();
                    info.ParentId = array[2] == null ? string.Empty : array[2].ToString();
                    info.ParentPath = array[3] == null ? string.Empty : array[3].ToString();
                    collection.Add(info);
                }
            }
            return collection;
        }
        public FunctionCollection QueryLowerLevelFuncitonByParentId(string parentId)
        {
            Session.Clear();
            FunctionCollection collection = new FunctionCollection();
            string strSql = "select isnull(FunctionId,'') FunctionId,isnull(DisplayName,'') DisplayName,isnull(ParentId,'') ParentId,isnull(ParentPath,'') ParentPath from C_Auth_Function_List where ParentId like '%" + parentId + "%'";
            var result = Session.CreateSQLQuery(strSql).List();
            if (result != null && result.Count > 0)
            {
                foreach (var item in result)
                {
                    var array = item as object[];
                    FunctionInfo info = new FunctionInfo();
                    info.FunctionId = array[0] == null ? string.Empty : array[0].ToString();
                    info.DisplayName = array[1] == null ? string.Empty : array[1].ToString();
                    info.ParentId = array[2] == null ? string.Empty : array[2].ToString();
                    info.ParentPath = array[3] == null ? string.Empty : array[3].ToString();
                    collection.Add(info);
                }
            }
            return collection;
        }
        public FunctionInfo QueryCurrentFuncitonById(string Id)
        {
            Session.Clear();
            FunctionInfo info = new FunctionInfo();
            string strSql = "select isnull(FunctionId,'') FunctionId,isnull(DisplayName,'') DisplayName,isnull(ParentId,'') ParentId,isnull(ParentPath,'') ParentPath from C_Auth_Function_List where FunctionId =:ID";
            var result = Session.CreateSQLQuery(strSql).SetString("ID", Id).List();
            if (result != null && result.Count > 0)
            {
                foreach (var item in result)
                {
                    var array = item as object[];
                    info.FunctionId = array[0] == null ? string.Empty : array[0].ToString();
                    info.DisplayName = array[1] == null ? string.Empty : array[1].ToString();
                    info.ParentId = array[2] == null ? string.Empty : array[2].ToString();
                    info.ParentPath = array[3] == null ? string.Empty : array[3].ToString();
                }
            }
            return info;
        }
        /// <summary>
        /// 财务明细报表--按到账时间做条件
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="agent"></param>
        /// <param name="status"></param>
        /// <param name="minMoney"></param>
        /// <param name="maxMoney"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="sortType"></param>
        /// <param name="operUserId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="bankcode"></param>
        /// <param name="winCount"></param>
        /// <param name="refusedCount"></param>
        /// <param name="totalWinMoney"></param>
        /// <param name="totalRefusedMoney"></param>
        /// <param name="totalResponseMoney"></param>
        /// <param name="totalCount"></param>
        /// <param name="totalMoney"></param>
        /// <returns></returns>
        /// 
        public List<GameBiz.Core.Withdraw_QueryInfo> QueryWithdrawListR(string userId, GameBiz.Core.WithdrawAgentType? agent, GameBiz.Core.WithdrawStatus? status, decimal minMoney, decimal maxMoney, DateTime startTime, DateTime endTime, int sortType, string operUserId, int pageIndex, int pageSize, string bankcode,
          out int winCount, out int refusedCount, out decimal totalWinMoney, out decimal totalRefusedMoney, out decimal totalResponseMoney, out int totalCount, out decimal totalMoney)
        {
            Session.Clear();
            endTime = endTime.AddDays(1);
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from r in this.Session.Query<Withdraw>()
                        join u in this.Session.Query<UserRegister>() on r.UserId equals u.UserId
                        join real in this.Session.Query<External.Domain.Entities.Authentication.UserRealName>() on r.UserId equals real.UserId
                        join m in this.Session.Query<External.Domain.Entities.Authentication.UserMobile>() on r.UserId equals m.UserId
                        where (userId == string.Empty || r.UserId == userId)
                        && r.ResponseTime >= startTime && r.ResponseTime <= endTime
                        && (status == null || r.Status == status)
                        && (bankcode == string.Empty || bankcode == r.BankCode)
                        && (agent == null || r.WithdrawAgent == agent)
                        && (minMoney == -1 || r.RequestMoney >= minMoney)
                        && (maxMoney == -1 || r.RequestMoney < maxMoney)
                        && (operUserId == "" || r.ResponseUserId == operUserId)
                        select new GameBiz.Core.Withdraw_QueryInfo
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
                            RequesterRealName = real.RealName,
                            RequesterMobile = m.Mobile,
                            ProcessorsUserKey = r.ResponseUserId,
                        };
            winCount = query.Where(p => p.Status == GameBiz.Core.WithdrawStatus.Success).Count();
            refusedCount = query.Where(p => p.Status == GameBiz.Core.WithdrawStatus.Refused).Count();

            totalWinMoney = winCount == 0 ? 0M : query.Where(p => p.Status == GameBiz.Core.WithdrawStatus.Success).Sum(p => p.RequestMoney);
            totalRefusedMoney = refusedCount == 0 ? 0M : query.Where(p => p.Status == GameBiz.Core.WithdrawStatus.Refused).Sum(p => p.RequestMoney);
            totalCount = query.Count();
            totalMoney = query.Count() == 0 ? 0M : query.Sum(p => p.RequestMoney);
            totalResponseMoney = winCount == 0 ? 0M : query.Where(p => p.ResponseMoney.HasValue == true).Sum(p => p.ResponseMoney.Value);

            if (sortType == -1)
                query = query.OrderByDescending(p => p.ResponseTime);
            if (sortType == 0)
                query = query.OrderBy(p => p.RequestMoney);
            if (sortType == 1)
                query = query.OrderByDescending(p => p.RequestMoney);

            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }


        /// <summary>
        /// 报表下载--按到账时间做条件
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="agent"></param>
        /// <param name="status"></param>
        /// <param name="minMoney"></param>
        /// <param name="maxMoney"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="sortType"></param>
        /// <param name="operUserId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="bankcode"></param>
        /// <param name="winCount"></param>
        /// <param name="refusedCount"></param>
        /// <param name="totalWinMoney"></param>
        /// <param name="totalRefusedMoney"></param>
        /// <param name="totalResponseMoney"></param>
        /// <param name="totalCount"></param>
        /// <param name="totalMoney"></param>
        /// <returns></returns>
        /// 
        public List<GameBiz.Core.Withdraw_QueryInfo> QueryWithdrawListR2(decimal minMoney, decimal maxMoney, DateTime startTime, DateTime endTime, WithdrawStatus? status)
        {
            Session.Clear();
            endTime = endTime.AddDays(1);
            var query = from r in this.Session.Query<Withdraw>()
                        join u in this.Session.Query<UserRegister>() on r.UserId equals u.UserId
                        join real in this.Session.Query<External.Domain.Entities.Authentication.UserRealName>() on r.UserId equals real.UserId
                        join m in this.Session.Query<External.Domain.Entities.Authentication.UserMobile>() on r.UserId equals m.UserId
                        where
                         r.ResponseTime >= startTime && r.ResponseTime <= endTime
                        //&& r.Status == GameBiz.Core.WithdrawStatus.Requesting
                        && (status == null || r.Status == status)
                        && r.WithdrawAgent == GameBiz.Core.WithdrawAgentType.BankCard
                        && (minMoney == -1 || r.RequestMoney >= minMoney)
                        && (maxMoney == -1 || r.RequestMoney < maxMoney)
                        select new GameBiz.Core.Withdraw_QueryInfo
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
                            RequesterRealName = real.RealName,
                            RequesterMobile = m.Mobile,
                            ProcessorsUserKey = r.ResponseUserId,
                        };

            query = query.OrderByDescending(p => p.ResponseTime);

            return query.ToList();
        }

        public int QueryWithdrawListR3(decimal minMoney, decimal maxMoney, DateTime startTime, DateTime endTime)
        {
            Session.Clear();
            //endTime = endTime.AddDays(1);
            var query = from r in this.Session.Query<Withdraw>()
                        join u in this.Session.Query<UserRegister>() on r.UserId equals u.UserId
                        join real in this.Session.Query<External.Domain.Entities.Authentication.UserRealName>() on r.UserId equals real.UserId
                        join m in this.Session.Query<External.Domain.Entities.Authentication.UserMobile>() on r.UserId equals m.UserId
                        where
                         r.ResponseTime >= startTime && r.ResponseTime <= endTime
                        && (minMoney == -1 || r.RequestMoney >= minMoney)
                        && (maxMoney == -1 || r.RequestMoney < maxMoney)
                        select new GameBiz.Core.Withdraw_QueryInfo
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
                            RequesterRealName = real.RealName,
                            RequesterMobile = m.Mobile,
                            ProcessorsUserKey = r.ResponseUserId,
                        };

            query = query.OrderByDescending(p => p.ResponseTime);

            return query.ToList().Count();
        }



        public List<GameBiz.Core.Withdraw_QueryInfo> QueryWithdrawList(string userId, GameBiz.Core.WithdrawAgentType? agent, GameBiz.Core.WithdrawStatus? status, decimal minMoney, decimal maxMoney, DateTime startTime, DateTime endTime, int sortType, string operUserId, int pageIndex, int pageSize, string bankcode,
            out int winCount, out int refusedCount, out decimal totalWinMoney, out decimal totalRefusedMoney, out decimal totalResponseMoney, out int totalCount, out decimal totalMoney)
        {
            Session.Clear();
            endTime = endTime.AddDays(1);
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from r in this.Session.Query<Withdraw>()
                        join u in this.Session.Query<UserRegister>() on r.UserId equals u.UserId
                        join real in this.Session.Query<External.Domain.Entities.Authentication.UserRealName>() on r.UserId equals real.UserId
                        join m in this.Session.Query<External.Domain.Entities.Authentication.UserMobile>() on r.UserId equals m.UserId
                        where (userId == string.Empty || r.UserId == userId)
                        && r.RequestTime >= startTime && r.RequestTime < endTime
                        && (status == null || r.Status == status)
                        && (bankcode == string.Empty || bankcode == r.BankCode)
                        && (agent == null || r.WithdrawAgent == agent)
                        && (minMoney == -1 || r.RequestMoney >= minMoney)
                        && (maxMoney == -1 || r.RequestMoney < maxMoney)
                        && (operUserId == "" || r.ResponseUserId == operUserId)
                        select new GameBiz.Core.Withdraw_QueryInfo
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
                            RequesterRealName = real.RealName,
                            RequesterMobile = m.Mobile,
                            ProcessorsUserKey = r.ResponseUserId,
                        };
            winCount = query.Where(p => p.Status == GameBiz.Core.WithdrawStatus.Success).Count();
            refusedCount = query.Where(p => p.Status == GameBiz.Core.WithdrawStatus.Refused).Count();

            totalWinMoney = winCount == 0 ? 0M : query.Where(p => p.Status == GameBiz.Core.WithdrawStatus.Success).Sum(p => p.RequestMoney);
            totalRefusedMoney = refusedCount == 0 ? 0M : query.Where(p => p.Status == GameBiz.Core.WithdrawStatus.Refused).Sum(p => p.RequestMoney);
            totalCount = query.Count();
            totalMoney = query.Count() == 0 ? 0M : query.Sum(p => p.RequestMoney);
            totalResponseMoney = winCount == 0 ? 0M : query.Where(p => p.ResponseMoney.HasValue == true).Sum(p => p.ResponseMoney.Value);

            if (sortType == -1)
                query = query.OrderBy(p => p.RequestTime);
            if (sortType == 0)
                query = query.OrderBy(p => p.RequestMoney);
            if (sortType == 1)
                query = query.OrderByDescending(p => p.RequestMoney);

            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }



        public List<GameBiz.Core.Withdraw_QueryInfo> QueryWithdrawList2(decimal minMoney, decimal maxMoney, DateTime startTime, DateTime endTime, WithdrawStatus? status)
        {
            Session.Clear();
            endTime = endTime.AddDays(1);
            var query = from r in this.Session.Query<Withdraw>()
                        join u in this.Session.Query<UserRegister>() on r.UserId equals u.UserId
                        join real in this.Session.Query<External.Domain.Entities.Authentication.UserRealName>() on r.UserId equals real.UserId
                        join m in this.Session.Query<External.Domain.Entities.Authentication.UserMobile>() on r.UserId equals m.UserId
                        where
                         r.RequestTime >= startTime && r.RequestTime < endTime
                            //&& r.Status == GameBiz.Core.WithdrawStatus.Requesting
                        && (status == null || r.Status == status)
                        && r.WithdrawAgent == GameBiz.Core.WithdrawAgentType.BankCard
                        && (minMoney == -1 || r.RequestMoney >= minMoney)
                        && (maxMoney == -1 || r.RequestMoney < maxMoney)
                        select new GameBiz.Core.Withdraw_QueryInfo
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
                            RequesterRealName = real.RealName,
                            RequesterMobile = m.Mobile,
                            ProcessorsUserKey = r.ResponseUserId,
                        };

            query = query.OrderBy(p => p.RequestTime);

            return query.ToList();
        }


        public int QueryWithdrawList3(decimal minMoney, decimal maxMoney, DateTime startTime, DateTime endTime)
        {
            Session.Clear();
            //endTime = endTime.AddDays(1);
            var query = from r in this.Session.Query<Withdraw>()
                        join u in this.Session.Query<UserRegister>() on r.UserId equals u.UserId
                        join real in this.Session.Query<External.Domain.Entities.Authentication.UserRealName>() on r.UserId equals real.UserId
                        join m in this.Session.Query<External.Domain.Entities.Authentication.UserMobile>() on r.UserId equals m.UserId
                        where
                         r.RequestTime >= startTime && r.RequestTime < endTime
                        && (minMoney == -1 || r.RequestMoney >= minMoney)
                        && (maxMoney == -1 || r.RequestMoney < maxMoney)
                        select new GameBiz.Core.Withdraw_QueryInfo
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
                            RequesterRealName = real.RealName,
                            RequesterMobile = m.Mobile,
                            ProcessorsUserKey = r.ResponseUserId,
                        };

            query = query.OrderBy(p => p.RequestTime);

            return query.ToList().Count();
        }

    }
}
