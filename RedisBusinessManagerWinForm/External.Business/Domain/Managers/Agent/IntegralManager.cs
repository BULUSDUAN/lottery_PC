using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business;
using GameBiz.Core;
using GameBiz.Domain.Entities;
using Common;
using NHibernate.Linq;
using External.Core.Login;

namespace External.Business.Domain.Managers.Agent
{
    public class IntegralManager : GameBizEntityManagement
    {

        public List<FundDetailInfo> QueryUserFundDetail(string userId, DateTime fromDate, DateTime toDate, List<AccountType> accountTypeList, List<string> categoryList
           , List<PayType> payTypeList, int pageIndex, int pageSize, out int totalPayinCount, out decimal totalPayinMoney, out int totalPayoutCount, out decimal totalPayoutMoney)
        {
            Session.Clear();
            toDate = toDate.AddDays(1);
            var query = from r in this.Session.Query<FundDetail>()
                        where (string.IsNullOrEmpty(userId) || r.UserId == userId)
                        && r.CreateTime >= fromDate && r.CreateTime < toDate
                        && (accountTypeList.Count == 0 || accountTypeList.Contains(r.AccountType)) && r.AccountType != AccountType.Bonus
                        && (categoryList.Count == 0 || categoryList.Contains(r.Category))
                        && (payTypeList.Count == 0 || payTypeList.Contains(r.PayType))
                        && r.AccountType != AccountType.FillMoney
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
            totalPayinCount = query.Where(p => p.PayType == PayType.Payin && p.AccountType != AccountType.FillMoney).Count();
            totalPayoutCount = query.Where(p => p.PayType == PayType.Payout && p.AccountType != AccountType.FillMoney).Count();

            totalPayinMoney = totalPayinCount == 0 ? 0M : query.Where(p => p.PayType == PayType.Payin && p.AccountType != AccountType.FillMoney).Sum(p => p.PayMoney);
            totalPayoutMoney = totalPayoutCount == 0 ? 0M : query.Where(p => p.PayType == PayType.Payout && p.AccountType != AccountType.FillMoney).Sum(p => p.PayMoney);

            return query.OrderByDescending(p => p.Id).Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<FundDetailInfo> QueryUserFundDetail_CPS(string userId, DateTime fromDate, DateTime toDate, List<AccountType> accountTypeList
           , List<PayType> payTypeList, int pageIndex, int pageSize, out int totalPayinCount, out decimal totalPayinMoney, out int totalPayoutCount, out decimal totalPayoutMoney)
        {
            Session.Clear();
            toDate = toDate.AddDays(1);
            var query = from r in this.Session.Query<FundDetail>()
                        where (string.IsNullOrEmpty(userId) || r.UserId == userId)
                        && r.CreateTime >= fromDate && r.CreateTime < toDate
                        && (accountTypeList.Count == 0 || accountTypeList.Contains(r.AccountType)) && r.AccountType != AccountType.Bonus
                        && (payTypeList.Count == 0 || payTypeList.Contains(r.PayType))
                        && r.AccountType != AccountType.FillMoney
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
            totalPayinCount = query.Where(p => p.PayType == PayType.Payin && p.AccountType != AccountType.FillMoney).Count();
            totalPayoutCount = query.Where(p => p.PayType == PayType.Payout && p.AccountType != AccountType.FillMoney).Count();

            totalPayinMoney = totalPayinCount == 0 ? 0M : query.Where(p => p.PayType == PayType.Payin && p.AccountType != AccountType.FillMoney).Sum(p => p.PayMoney);
            totalPayoutMoney = totalPayoutCount == 0 ? 0M : query.Where(p => p.PayType == PayType.Payout && p.AccountType != AccountType.FillMoney).Sum(p => p.PayMoney);

            return query.OrderByDescending(p => p.Id).Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
        public UserQueryInfoCollection QueryAgentLowerUserList(string agentId, int pageIndex, int pageSize)
        {
            Session.Clear();
            UserQueryInfoCollection collection = new UserQueryInfoCollection();
            collection.TotalCount = 0;
            //string strSql = "select u.UserId,u.DisplayName,isnull(r.RealName,'')RealName,isnull(m.Mobile,'')Mobile,isnull(u.RegisterIp,'')RegisterIp,u.CreateTime,isnull(u.AgentId,'')AgentId,isnull(b.CommissionBalance,0)CommissionBalance,isnull(b.BonusBalance,0)BonusBalance,isnull(b.FreezeBalance,0)FreezeBalance from C_User_Register u left join E_Authentication_RealName r on u.UserId=r.UserId left join E_Authentication_Mobile m on r.UserId=m.UserId left join C_User_Balance b on u.UserId=b.UserId where u.ParentPath like '%" + agentId + "%'";
            string strSql = "select u.UserId,u.DisplayName,isnull(r.RealName,'')RealName,isnull(m.Mobile,'')Mobile,isnull(u.RegisterIp,'')RegisterIp,u.CreateTime,isnull(u.AgentId,'')AgentId,isnull(b.CommissionBalance,0)CommissionBalance,isnull(b.BonusBalance,0)BonusBalance,isnull(b.FreezeBalance,0)FreezeBalance,isnull(b.FillMoneyBalance,0)FillMoneyBalance from C_User_Register u left join E_Authentication_RealName r on u.UserId=r.UserId left join E_Authentication_Mobile m on r.UserId=m.UserId left join C_User_Balance b on u.UserId=b.UserId where u.AgentId=:agentId";
            var result = Session.CreateSQLQuery(strSql).SetString("agentId", agentId).List();
            if (result != null)
            {
                collection.TotalCount = result.Count;
                foreach (var item in result)
                {
                    var array = item as object[];
                    UserQueryInfo info = new UserQueryInfo();
                    info.UserId = array[0].ToString();
                    info.DisplayName = array[1].ToString();
                    info.RealName = array[2].ToString();
                    info.Mobile = array[3].ToString();
                    info.RegisterIp = array[4].ToString();
                    info.RegTime = Convert.ToDateTime(array[5]);
                    info.AgentId = array[6].ToString();
                    info.CommissionBalance = Convert.ToDecimal(array[7]) + Convert.ToDecimal(array[10]);
                    info.BonusBalance = Convert.ToDecimal(array[8]);
                    info.FreezeBalance = Convert.ToDecimal(array[9]);
                    collection.UserList.Add(info);
                }
            }
            collection.UserList = collection.UserList.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            return collection;

        }

        public List<Withdraw_QueryInfo> QueryWithdrawList(string userId, WithdrawStatus? status, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string orderId,
            out int winCount, out int refusedCount, out decimal totalWinMoney, out decimal totalRefusedMoney, out decimal totalResponseMoney, out int totalCount, out decimal totalMoney)
        {
            Session.Clear();
            endTime = endTime.AddDays(1).Date;
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            if (status == WithdrawStatus.Refused || status == WithdrawStatus.Success)
            {
                var query = from r in this.Session.Query<Withdraw>()
                            join u in this.Session.Query<UserRegister>() on r.UserId equals u.UserId
                            where (userId == string.Empty || r.UserId == userId)
                            && r.ResponseTime >= startTime && r.ResponseTime < endTime
                            && (status == null || r.Status == status) && (orderId == string.Empty || r.OrderId == orderId)
                            && r.WithdrawAgent == WithdrawAgentType.Integral_BankCard
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

                query = query.OrderBy(p => p.RequestTime);
                if (pageSize == -1)
                    return query.ToList();
                return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            else
            {
                var query = from r in this.Session.Query<Withdraw>()
                            join u in this.Session.Query<UserRegister>() on r.UserId equals u.UserId
                            where (userId == string.Empty || r.UserId == userId)
                            && r.RequestTime >= startTime && r.RequestTime < endTime
                            && (status == null || r.Status == status) && (orderId == string.Empty || r.OrderId == orderId)
                            && r.WithdrawAgent == WithdrawAgentType.Integral_BankCard
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

                query = query.OrderBy(p => p.RequestTime);
                if (pageSize == -1)
                    return query.ToList();
                return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
        }

        public List<Withdraw_QueryInfo> QueryCPSWithdrawList(string userId, WithdrawStatus? status, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string orderId,
           out int winCount, out int refusedCount, out decimal totalWinMoney, out decimal totalRefusedMoney, out decimal totalResponseMoney, out int totalCount, out decimal totalMoney)
        {
            Session.Clear();
            endTime = endTime.AddDays(1).Date;
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            if (status == WithdrawStatus.Refused || status == WithdrawStatus.Success)
            {
                var query = from r in this.Session.Query<Withdraw>()
                            join u in this.Session.Query<UserRegister>() on r.UserId equals u.UserId
                            where (userId == string.Empty || r.UserId == userId)
                            && r.ResponseTime >= startTime && r.ResponseTime < endTime
                            && (status == null || r.Status == status) && (orderId == string.Empty || r.OrderId == orderId)
                            && r.WithdrawAgent == WithdrawAgentType.CPS_BankCard
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

                query = query.OrderBy(p => p.RequestTime);
                if (pageSize == -1)
                    return query.ToList();
                return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            else
            {
                var query = from r in this.Session.Query<Withdraw>()
                            join u in this.Session.Query<UserRegister>() on r.UserId equals u.UserId
                            where (userId == string.Empty || r.UserId == userId)
                            && r.RequestTime >= startTime && r.RequestTime < endTime
                            && (status == null || r.Status == status) && (orderId == string.Empty || r.OrderId == orderId)
                            && r.WithdrawAgent == WithdrawAgentType.CPS_BankCard
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

                query = query.OrderBy(p => p.RequestTime);
                if (pageSize == -1)
                    return query.ToList();
                return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
        }

        public List<FundDetailInfo> QueryUserFundDetail_ContainCommission(string userId, DateTime fromDate, DateTime toDate, List<AccountType> accountTypeList, List<string> categoryList
            , int pageIndex, int pageSize, out int totalPayinCount, out decimal totalPayinMoney, out int totalPayoutCount, out decimal totalPayoutMoney)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            if (pageSize < 10000)
                pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            fromDate = fromDate.Date;
            toDate = toDate.AddDays(1).Date;
            var query = from r in this.Session.Query<FundDetail>()
                        where (string.IsNullOrEmpty(userId) || r.UserId == userId)
                        && r.CreateTime >= fromDate && r.CreateTime < toDate
                        && (accountTypeList.Count == 0 || accountTypeList.Contains(r.AccountType))
                        && (categoryList.Count == 0 || categoryList.Contains(r.Category))

                        && (r.Category == BusinessHelper.FundCategory_IntegralRequestWithdraw || r.Category == BusinessHelper.FundCategory_IntegralSchemeDeduct || r.Category == BusinessHelper.FundCategory_IntegralCompleteWithdraw || r.Category == BusinessHelper.FundCategory_IntegralRefusedWithdraw || r.Category == BusinessHelper.FundCategory_IntegralManualDeductMoney || r.Category == BusinessHelper.FundCategory_IntegralManualRemitMoney || r.Category == BusinessHelper.FundCategory_SchemeDeduct || r.Category == BusinessHelper.FundCategory_IntergralPayOut || r.Category == BusinessHelper.FundCategory_Betting || r.Category == BusinessHelper.FundCategory_TicketFailed)
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
            if (pageSize == -1)
                return query.OrderByDescending(p => p.Id).ToList();
            else
                return query.OrderByDescending(p => p.Id).Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        //代理会员管理
        public IList QueryUserList(DateTime regFrom, DateTime regTo, string keyType, string keyValue, bool? isEnable, bool? isFillMoney, bool? isAgent, string commonBlance, string bonusBlance, string freezeBlance, string vipRange, string comeFrom, string agentId, int pageIndex, int pageSize, out int totalCount, out  decimal totalFillMoneyBalance, out  decimal totalBonusBalance, out  decimal totalCommissionBalance, out  decimal totalExpertsBalance, out  decimal totalFreezeBalance, out decimal totalRedBagBalance, out int totalDouDou)
        {
            #region 构造查询语句

            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var sqlCondition = new List<string>();

            #region 条件 - 注册时间
            sqlCondition.Add(string.Format("AND R.CreateTime >= N'{0:yyyy-MM-dd}' AND R.CreateTime <= N'{1:yyyy-MM-dd}'", regFrom, regTo.AddDays(1)));
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

            #region 条件 - 禁用状态、充值状态、经销商状态
            if (isEnable.HasValue)
            {
                sqlCondition.Add(string.Format("AND R.IsEnable = {0} ", isEnable.Value ? 1 : 0));
            }
            if (isFillMoney.HasValue)
            {
                sqlCondition.Add(string.Format("AND R.IsFillMoney = {0} ", isFillMoney.Value ? 1 : 0));
            }
            if (isAgent.HasValue)
            {
                sqlCondition.Add(string.Format("AND R.IsAgent = {0} ", isAgent.Value ? 1 : 0));
            }
            #endregion

            #region 条件 - 账户余额

            var spliter = '-';
            if (!string.IsNullOrEmpty(commonBlance))
            {
                var from = commonBlance.Split(spliter)[0];
                if (!string.IsNullOrEmpty(from)) { sqlCondition.Add(string.Format("AND B.FillMoneyBalance >= {0} ", decimal.Parse(from))); }
                var to = commonBlance.Split(spliter)[1];
                if (!string.IsNullOrEmpty(to)) { sqlCondition.Add(string.Format("AND B.FillMoneyBalance <= {0} ", decimal.Parse(to))); }
            }
            if (!string.IsNullOrEmpty(bonusBlance))
            {
                var from = bonusBlance.Split(spliter)[0];
                if (!string.IsNullOrEmpty(from)) { sqlCondition.Add(string.Format("AND B.BonusBalance >= {0} ", decimal.Parse(from))); }
                var to = bonusBlance.Split(spliter)[1];
                if (!string.IsNullOrEmpty(to)) { sqlCondition.Add(string.Format("AND B.BonusBalance <= {0} ", decimal.Parse(to))); }
            }
            if (!string.IsNullOrEmpty(freezeBlance))
            {
                var from = freezeBlance.Split(spliter)[0];
                if (!string.IsNullOrEmpty(from)) { sqlCondition.Add(string.Format("AND B.FreezeBalance >= {0} ", decimal.Parse(from))); }
                var to = freezeBlance.Split(spliter)[1];
                if (!string.IsNullOrEmpty(to)) { sqlCondition.Add(string.Format("AND B.FreezeBalance <= {0} ", decimal.Parse(to))); }
            }

            #endregion

            #region VIP等级限制

            if (!string.IsNullOrEmpty(vipRange))
            {
                var from = vipRange.Split(spliter)[0];
                if (!string.IsNullOrEmpty(from)) { sqlCondition.Add(string.Format("AND [R].[VipLevel] >= {0} ", decimal.Parse(from))); }
                var to = vipRange.Split(spliter)[1];
                if (!string.IsNullOrEmpty(to)) { sqlCondition.Add(string.Format("AND [R].[VipLevel] <= {0} ", decimal.Parse(to))); }
            }

            #endregion

            #region  条件 - 注册来源、经销商
            if (!string.IsNullOrEmpty(comeFrom))
            {
                sqlCondition.Add(string.Format("AND [R].[ComeFrom] = N'{0}' ", comeFrom));
            }
            if (!string.IsNullOrEmpty(agentId))
            {
                sqlCondition.Add(string.Format("AND R.AgentId = N'{0}' ", agentId));
            }
            #endregion

            var sqlBuilder_count = new StringBuilder();
            sqlBuilder_count.AppendLine("select COUNT(1)  AS totalCount ,SUM(PayMoney) as totalFreezeBalance ,SUM(tab.CommissionBalance) as  totalCommissionBalance from(");
            sqlBuilder_count.AppendLine("SELECT [UserId],[DisplayName],[ComeFrom],[RegisterIp],[CreateTime],IsEnable,IsAgent,IsFillMoney,AgentId");
            sqlBuilder_count.AppendLine("     ,FillMoneyBalance,BonusBalance,FreezeBalance,CommissionBalance,RedBagBalance,ExpertsBalance");
            sqlBuilder_count.AppendLine("     ,IsSettedMobile,Mobile,IsSettedRealName,RealName,CardType,IdCardNumber,IsSettedEmail,Email,VipLevel");
            sqlBuilder_count.AppendLine("FROM (");
            sqlBuilder_count.AppendLine("     SELECT ROW_NUMBER() OVER(ORDER BY [R].[CreateTime] DESC) AS [RowNumber],[R].[UserId],[R].[VipLevel],[R].[DisplayName],[R].[ComeFrom],[R].[RegisterIp],[R].[CreateTime],R.IsEnable,R.IsAgent,R.IsFillMoney,R.AgentId");
            sqlBuilder_count.AppendLine("         ,[B].FillMoneyBalance,B.BonusBalance,B.FreezeBalance,B.CommissionBalance,B.RedBagBalance,B.ExpertsBalance");
            sqlBuilder_count.AppendLine("         ,M.IsSettedMobile,M.Mobile,N.IsSettedRealName,N.RealName,N.CardType,N.IdCardNumber,E.IsSettedEmail,E.Email");
            sqlBuilder_count.AppendLine("     FROM [C_User_Register] AS [R] with(nolock)");
            sqlBuilder_count.AppendLine("     INNER JOIN [C_User_Balance] AS [B] with(nolock) ON [R].[UserId] = [B].[UserId]");
            sqlBuilder_count.AppendLine("     LEFT OUTER JOIN [E_Authentication_Email] AS [E] with (nolock)  ON [E].[UserId] = [R].[UserId]");
            sqlBuilder_count.AppendLine("     LEFT OUTER JOIN [E_Authentication_Mobile] AS [M] with (nolock)  ON [M].[UserId] = [R].[UserId]");
            sqlBuilder_count.AppendLine("     LEFT OUTER JOIN [E_Authentication_RealName] AS [N] with (nolock)  ON [N].[UserId] = [R].[UserId]");
            sqlBuilder_count.AppendLine("     WHERE 1=1 ");
            sqlBuilder_count.AppendLine(string.Join(" ", sqlCondition.ToArray()));
            sqlBuilder_count.AppendLine(string.Format(") AS T"));
            sqlBuilder_count.AppendLine(") tab left join (select sum(f.PayMoney) PayMoney,f.UserId from C_Withdraw w inner join C_Fund_Detail f on w.OrderId=f.OrderId where w.Status=1 and f.AccountType=20 and f.Category='" + BusinessHelper.FundCategory_IntegralRequestWithdraw + "' group by f.UserId) wf on tab.UserId=wf.UserId ");



            var sqlBuilder_query = new StringBuilder();
            sqlBuilder_query.AppendLine("select tab.[UserId],[DisplayName],[ComeFrom],[RegisterIp],[CreateTime],IsEnable,IsAgent,IsFillMoney,AgentId,FillMoneyBalance,BonusBalance,wf.PayMoney FreezeBalance,CommissionBalance,RedBagBalance,ExpertsBalance,IsSettedMobile,Mobile,IsSettedRealName,RealName,CardType,IdCardNumber,IsSettedEmail,Email,VipLevel from(");
            sqlBuilder_query.AppendLine("SELECT [UserId],[DisplayName],[ComeFrom],[RegisterIp],[CreateTime],IsEnable,IsAgent,IsFillMoney,AgentId");
            sqlBuilder_query.AppendLine("     ,FillMoneyBalance,BonusBalance,FreezeBalance,CommissionBalance,RedBagBalance,ExpertsBalance");
            sqlBuilder_query.AppendLine("     ,IsSettedMobile,Mobile,IsSettedRealName,RealName,CardType,IdCardNumber,IsSettedEmail,Email,VipLevel");
            sqlBuilder_query.AppendLine("FROM (");
            sqlBuilder_query.AppendLine("     SELECT ROW_NUMBER() OVER(ORDER BY [R].[CreateTime] DESC) AS [RowNumber],[R].[UserId],[R].[VipLevel],[R].[DisplayName],[R].[ComeFrom],[R].[RegisterIp],[R].[CreateTime],R.IsEnable,R.IsAgent,R.IsFillMoney,R.AgentId");
            sqlBuilder_query.AppendLine("         ,[B].FillMoneyBalance,B.BonusBalance,B.FreezeBalance,B.CommissionBalance,B.RedBagBalance,B.ExpertsBalance");
            sqlBuilder_query.AppendLine("         ,M.IsSettedMobile,M.Mobile,N.IsSettedRealName,N.RealName,N.CardType,N.IdCardNumber,E.IsSettedEmail,E.Email");
            sqlBuilder_query.AppendLine("     FROM [C_User_Register] AS [R] with(nolock)");
            sqlBuilder_query.AppendLine("     INNER JOIN [C_User_Balance] AS [B] with(nolock) ON [R].[UserId] = [B].[UserId]");
            sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [E_Authentication_Email] AS [E] with (nolock)  ON [E].[UserId] = [R].[UserId]");
            sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [E_Authentication_Mobile] AS [M] with (nolock)  ON [M].[UserId] = [R].[UserId]");
            sqlBuilder_query.AppendLine("     LEFT OUTER JOIN [E_Authentication_RealName] AS [N] with (nolock)  ON [N].[UserId] = [R].[UserId]");
            sqlBuilder_query.AppendLine("     WHERE 1=1 ");
            sqlBuilder_query.AppendLine(string.Join(" ", sqlCondition.ToArray()));
            sqlBuilder_query.AppendLine(string.Format(") AS T WHERE [RowNumber] > {0} AND [RowNumber] <= {1}", pageIndex * pageSize, (pageIndex + 1) * pageSize));
            sqlBuilder_query.AppendLine(") tab left join (select sum(f.PayMoney) PayMoney,f.UserId from C_Withdraw w inner join C_Fund_Detail f on w.OrderId=f.OrderId where w.Status=1 and f.AccountType=20 and f.Category='" + BusinessHelper.FundCategory_IntegralRequestWithdraw + "' group by f.UserId) wf on tab.UserId=wf.UserId  order by tab.CreateTime desc");

            #endregion

            var totalList = Session.CreateSQLQuery(sqlBuilder_count.ToString())
                .List();
            totalCount = 0;
            totalFillMoneyBalance = 0M;
            totalBonusBalance = 0M;
            totalCommissionBalance = 0M;
            totalExpertsBalance = 0M;
            totalFreezeBalance = 0M;
            totalRedBagBalance = 0M;
            totalDouDou = 0;

            if (totalList.Count == 1)
            {
                var array = totalList[0] as object[];
                if (array.Length == 3 && Convert.ToInt32(array[0]) > 0)
                {
                    totalCount = int.Parse(array[0].ToString());
                    totalFreezeBalance = array[1] == null ? 0M : decimal.Parse(array[1].ToString());
                    totalCommissionBalance = array[2] == null ? 0M : decimal.Parse(array[2].ToString());
                }
            }
            return Session.CreateSQLQuery(sqlBuilder_query.ToString())
                .List();
        }
        /// <summary>
        /// 获取当前冻结金额
        /// </summary>
        public decimal GetFreeBalanceByUserId_Agent(string userId)
        {
            Session.Clear();
            string strSql = "select sum(f.PayMoney) PayMoney from C_Withdraw w inner join C_Fund_Detail f on w.OrderId=f.OrderId where w.Status=1 and f.AccountType=20 and f.Category!='申请提取积分' and f.UserId=:UserId group by f.UserId";
            var result = Session.CreateSQLQuery(strSql).SetString("UserId", userId).UniqueResult();
            if (result != null)
                return Convert.ToDecimal(result);
            return 0M;
        }
    }
}
