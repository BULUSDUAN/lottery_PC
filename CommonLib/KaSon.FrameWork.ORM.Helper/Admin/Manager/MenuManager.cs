using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel.CoreModel;
using EntityModel.Enum;

namespace KaSon.FrameWork.ORM.Helper
{
    public class MenuManager:DBbase
    {
        public List<E_Menu_List> QueryMenuListByUserId(string userId)
        {
            string sql = SqlModule.AdminModule.FirstOrDefault(x => x.Key == "Admin_QueryMenuListByUserId").SQL;
            return DB.CreateSQLQuery(sql)
                .SetString("UserId", userId)
                .List<E_Menu_List>().ToList();
        }
        public List<E_Menu_List> QueryAllMenuList()
        {
            return DB.CreateQuery<E_Menu_List>().OrderBy(p=>p.MenuId).ToList();
        }

        public List<C_Auth_Function_List> QueryLowerLevelFuncitonList()
        {
            var result = DB.CreateQuery<C_Auth_Function_List>().ToList();
            return result;
        }
        public Withdraw_QueryInfoCollection QueryWithdrawList(string userId, WithdrawAgentType? agent,WithdrawStatus? status, decimal minMoney, decimal maxMoney, DateTime startTime, DateTime endTime, int sortType, string operUserId, int pageIndex, int pageSize, string bankcode)
        {
            var result = new Withdraw_QueryInfoCollection();
            endTime = endTime.AddDays(1);
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from r in DB.CreateQuery<C_Withdraw>()
                        join u in DB.CreateQuery<C_User_Register>() on r.UserId equals u.UserId
                        join real in DB.CreateQuery<E_Authentication_RealName>() on r.UserId equals real.UserId
                        join m in DB.CreateQuery<E_Authentication_Mobile>() on r.UserId equals m.UserId
                        where (userId == string.Empty || r.UserId == userId)
                        && r.RequestTime >= startTime && r.RequestTime < endTime
                        && (status == null || r.Status == (int)status)
                        && (bankcode == string.Empty || bankcode == r.BankCode)
                        && (agent == null || r.WithdrawAgent == (int)agent)
                        && (minMoney == -1 || r.RequestMoney >= minMoney)
                        && (maxMoney == -1 || r.RequestMoney < maxMoney)
                        && (operUserId == "" || r.ResponseUserId == operUserId)
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
                            RequesterRealName = real.RealName,
                            RequesterMobile = m.Mobile,
                            ProcessorsUserKey = r.ResponseUserId,
                            UserCreditType = u.UserCreditType
                        };
            result.WinCount = query.Where(p => p.Status == (int)WithdrawStatus.Success).Count();
            result.RefusedCount = query.Where(p => p.Status == (int)WithdrawStatus.Refused).Count();

            result.TotalWinMoney = result.WinCount == 0 ? 0M : query.Where(p => p.Status == (int)WithdrawStatus.Success).Sum(p => p.RequestMoney);
            result.TotalRefusedMoney = result.RefusedCount == 0 ? 0M : query.Where(p => p.Status == (int)WithdrawStatus.Refused).Sum(p => p.RequestMoney);
            result.TotalCount = query.Count();
            result.TotalMoney = query.Count() == 0 ? 0M : query.Sum(p => p.RequestMoney);
            result.TotalResponseMoney = result.WinCount == 0 ? 0M : query.Where(p => p.ResponseMoney.HasValue == true).Sum(p => p.ResponseMoney.Value);
            
            if (sortType == -1)
                query = query.OrderBy(p => p.RequestTime);
            if (sortType == 0)
                query = query.OrderBy(p => p.RequestMoney);
            if (sortType == 1)
                query = query.OrderByDescending(p => p.RequestMoney);
            result.WithdrawList = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            return result;
        }
        public List<Withdraw_QueryInfo> QueryWithdrawList2(decimal minMoney, decimal maxMoney, DateTime startTime, DateTime endTime, WithdrawStatus? status)
        {
            endTime = endTime.AddDays(1);
            var query = from r in DB.CreateQuery<C_Withdraw>()
                        join u in DB.CreateQuery<C_User_Register>() on r.UserId equals u.UserId
                        join real in DB.CreateQuery<E_Authentication_RealName>() on r.UserId equals real.UserId
                        join m in DB.CreateQuery<E_Authentication_Mobile>() on r.UserId equals m.UserId
                        where r.RequestTime >= startTime && r.RequestTime < endTime
                        && (status == null || r.Status == (int)status)
                        && r.WithdrawAgent == (int)WithdrawAgentType.BankCard
                        && (minMoney == -1 || r.RequestMoney >= minMoney)
                        && (maxMoney == -1 || r.RequestMoney < maxMoney)
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
                            RequesterRealName = real.RealName,
                            RequesterMobile = m.Mobile,
                            ProcessorsUserKey = r.ResponseUserId,
                        };
            query = query.OrderBy(p => p.RequestTime);
            return query.ToList();
        }
    }
}
