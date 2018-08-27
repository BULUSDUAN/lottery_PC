using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper

{
   public class SqlQueryManager:DBbase
    {
        /// <summary>
        /// 查询某个yqid下面的 能满足领红包条件的用户个数
        /// </summary>
        /// <param name="AgentId">普通用户代理 邀请注册的会员</param>
        /// <returns></returns>
        public string QueryYqidRegisterByAgentId(string AgentId)
        {

            // 通过数据库存储过程进行查询
            var query = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "P_Blog_TotalUserCount").SQL;
            var TotalUserCount_query = DB.CreateSQLQuery(query)
                .SetString("@AgentId", AgentId)
                .SetString("@UserId", "0")
                .SetInt("@TotalUserCount", 0)
                .SetInt("@UserCount", 0).First<int>();
                //List<E_Blog_UserSpread>();
            var UserSpread= SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "P_Blog_UserSpread").SQL;
            var UserSpread_query = DB.CreateSQLQuery(UserSpread)
               .SetString("@AgentId", AgentId).First<int>();

           var RedBagMoney= SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "P_Blog_RedBagMoney").SQL;
            var RedBagMoney_query = DB.CreateSQLQuery(RedBagMoney)
              .SetString("@AgentId", AgentId).First<decimal>();

            string str = string.Empty;
           
                
                if (TotalUserCount_query > 0)
                {
                  
                    str += TotalUserCount_query + "|";
                }
              
                if (UserSpread_query > 0)
                {
                   
                    str += UserSpread_query + "|";
                }
              
                if (RedBagMoney_query > 0)
                {
                  
                    str += RedBagMoney_query;
                }
            
            return str;
        }

        public Withdraw_QueryInfo GetWithdrawById(string orderId)
        {
            try
            {
                // 通过数据库存储过程进行查询
                var result = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "rj_Fund_QueryWithdrawById").SQL;
                var withdraw_Info = DB.CreateSQLQuery(result).SetString("@OrderId", orderId)
                    .First<Withdraw_QueryInfo>();

                return withdraw_Info;
            }
             catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<Withdraw_QueryInfo> QueryWithdrawList(string userId, WithdrawAgentType? agent, WithdrawStatus? status, decimal minMoney, decimal maxMoney, int sortType, int pageIndex, int pageSize, string orderId,
       out int winCount, out int refusedCount, out decimal totalWinMoney, out decimal totalRefusedMoney, out decimal totalResponseMoney, out int totalCount, out decimal totalMoney)
        {
         
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;

            int? Status = (int?)status;
            int? Agent = (int?)agent;
            var query = (from r in DB.CreateQuery<C_Withdraw>()
                         join u in DB.CreateQuery<C_User_Register>() on r.UserId equals u.UserId
                         where (userId == string.Empty || r.UserId == userId)                  
                         && (status == null || r.Status == Status)
                         && (orderId == string.Empty || r.OrderId == orderId)
                         && (agent == null || r.WithdrawAgent == Agent)
                         && (minMoney == -1 || r.RequestMoney >= minMoney)
                         && (maxMoney == -1 || r.RequestMoney <= maxMoney)select new {r,u })
                         .ToList().Select(b => new Withdraw_QueryInfo {
                             BankCardNumber = b.r.BankCardNumber,
                             BankCode = b.r.BankCode,
                             BankName = b.r.BankName,
                             BankSubName = b.r.BankSubName,
                             CityName = b.r.CityName,
                             OrderId = b.r.OrderId,
                             ProvinceName = b.r.ProvinceName,
                             RequestMoney = b.r.RequestMoney,
                             RequestTime = b.r.RequestTime,
                             ResponseTime = b.r.ResponseTime,
                             ResponseMoney = b.r.ResponseMoney,
                             WithdrawAgent = b.r.WithdrawAgent,
                             Status = b.r.Status,
                             ResponseMessage = b.r.ResponseMessage,
                             RequesterDisplayName = b.u.DisplayName,
                             RequesterUserKey = b.u.UserId,
                         });
                     
            int Success = (int)WithdrawStatus.Success;
            winCount = query.Where(p => p.Status == Success).Count();
            refusedCount = query.Where(p => p.Status == (int)WithdrawStatus.Refused).Count();

            totalWinMoney = winCount == 0 ? 0M : query.Where(p => p.Status == (int)WithdrawStatus.Success).Sum(p => p.RequestMoney);
            totalRefusedMoney = refusedCount == 0 ? 0M : query.Where(p => p.Status == (int)WithdrawStatus.Refused).Sum(p => p.RequestMoney);
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
            return query.Skip(pageIndex * pageSize).Take(pageSize).OrderByDescending(P=>P.RequestTime).ToList();
        }

    }
}
