using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Enum;
using KaSon.FrameWork.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static EntityModel.CoreModel.ReportInfo;

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

        public List<Withdraw_QueryInfo> QueryWithdrawList(string userId, WithdrawAgentType? agent, int status, decimal minMoney, decimal maxMoney, int sortType, int pageIndex, int pageSize, string orderId,
       out int winCount, out int refusedCount, out decimal totalWinMoney, out decimal totalRefusedMoney, out decimal totalResponseMoney, out int totalCount, out decimal totalMoney)
        {
         
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;

          
            int? Agent = (int?)agent;
            var query = (from r in DB.CreateQuery<C_Withdraw>()
                         join u in DB.CreateQuery<C_User_Register>() on r.UserId equals u.UserId
                         where (userId == string.Empty || r.UserId == userId)                  
                         && (status == -1 || r.Status == status)
                         && (orderId == string.Empty || r.OrderId == orderId)
                         && (agent == null || r.WithdrawAgent == Agent)
                         && (minMoney == -1 || r.RequestMoney >= minMoney)
                         && (maxMoney == -1 || r.RequestMoney <= maxMoney)select new {r,u })
                         .ToList().OrderByDescending(p => p.r.RequestTime).Select(b => new Withdraw_QueryInfo {
                             BankCardNumber = ConvertHelper.GetBankCardNumberxxxString(b.r.BankCardNumber),
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

            //if (sortType == -1)
            //    query = query.OrderBy(p => p.RequestTime);
            //if (sortType == 0)
            //    query = query.OrderBy(p => p.RequestMoney);
            //if (sortType == 1)
            //    query = query.OrderByDescending(p => p.RequestMoney);
             
            //if (pageSize == -1)
            //    return query.ToList();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<Withdraw_QueryInfo> QueryWithdrawList(string userId, WithdrawAgentType? agent, int status, decimal minMoney, decimal maxMoney, DateTime startTime, DateTime endTime, int sortType, int pageIndex, int pageSize, string orderId,
        out int winCount, out int refusedCount, out decimal totalWinMoney, out decimal totalRefusedMoney, out decimal totalResponseMoney, out int totalCount, out decimal totalMoney)
        {
         
            endTime = endTime.AddDays(1).Date;
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            int? Agent = (int?)agent;
            var query = (from r in DB.CreateQuery<C_Withdraw>()
                         join u in DB.CreateQuery<C_User_Register>() on r.UserId equals u.UserId
                         where (userId == string.Empty || r.UserId == userId)
                        && r.RequestTime >= startTime && r.RequestTime < endTime
                        && (status == -1 || r.Status == status)
                        && (orderId == string.Empty || r.BankCode == orderId)
                        && (agent == null || r.WithdrawAgent == Agent)
                        && (minMoney == -1 || r.RequestMoney >= minMoney)
                        && (maxMoney == -1 || r.RequestMoney <= maxMoney)
                         select new { r, u })
                         .ToList().OrderByDescending(p => p.r.RequestTime).Select(b => new Withdraw_QueryInfo
                         {
                             BankCardNumber = ConvertHelper.GetBankCardNumberxxxString(b.r.BankCardNumber),
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
            winCount = query.Where(p => p.Status == (int)WithdrawStatus.Success).Count();
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
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }


        public BettingOrderInfo QueryOrderDetailBySchemeId(string schemeId)
        {
          
            var query = (from P in DB.CreateQuery<C_OrderDetail>()
                        where (P.SchemeId == schemeId)
                        select P).ToList().Select(o=>new BettingOrderInfo
                        {
                            TicketStatus = (TicketStatus)o.TicketStatus,
                            IsVirtualOrder = o.IsVirtualOrder,
                            IssuseNumber = o.CurrentIssuseNumber,
                            CurrentBettingMoney = o.CurrentBettingMoney,
                            TotalMoney = o.TotalMoney,
                            ProgressStatus = (ProgressStatus)o.ProgressStatus,
                            SchemeId = o.SchemeId,
                            AfterTaxBonusMoney = o.AfterTaxBonusMoney,
                            PreTaxBonusMoney = o.PreTaxBonusMoney,
                            BonusStatus = (BonusStatus)o.BonusStatus,
                            SchemeBettingCategory = (SchemeBettingCategory)o.SchemeBettingCategory,
                            SchemeSource = (SchemeSource)o.SchemeSource,
                            SchemeType = (SchemeType)o.SchemeType,
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
                        });
            return query.FirstOrDefault();
        }

        public int QueryTogetherFollowerCount(string createUserId)
        {
          
            var result = DB.CreateQuery<C_Together_FollowerRule>().Where(s => s.CreaterUserId == createUserId);
            if (result != null && result.Count() > 0)
                return result.Count();
            return 0;
        }

        #region 过关统计

        /// <summary>
        /// 查询过关统计
        /// </summary>
        public List<SportsOrder_GuoGuanInfo> QueryReportInfoList_GuoGuan(bool? isVirtualOrder, SchemeBettingCategory? category, string key, string gameCode, string gameType,
            string issuseNumber, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
           

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
            var query = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "P_Report_GuoGuanTongJi_Sport").SQL;
            var listInfo =DB.CreateSQLQuery(query).
                SetInt("isVirtualOrder", !isVirtualOrder.HasValue ? -1 : Convert.ToInt32(isVirtualOrder.Value))
               .SetInt("bettingCategory", !category.HasValue ? -1 : (int)category.Value)
               .SetString("key_UID_UName_SchemeId", key)
               .SetString("gameCode", gameCode.ToUpper())
               .SetString("gameType", gameType.ToUpper())
               .SetString("issuseNumber", issuseNumber)
               .SetString("startTime", startTime.ToString("yyyy-MM-dd HH:mm"))
               .SetString("endTime", endTime.ToString("yyyy-MM-dd HH:mm"))
               .SetInt("pageIndex", pageIndex)
               .SetInt("pageSize", pageSize)
               .SetString("TotalCount", "Int32").List<SportsOrder_GuoGuanInfo>().ToList();
            return listInfo;
        }

        #endregion
    }
}
