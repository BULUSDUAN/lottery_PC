using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;
using Common.Business;
using NHibernate.Linq;
using GameBiz.Domain.Entities;
using Common;

namespace GameBiz.Business
{
    public class FundManager : GameBizEntityManagement
    {
        public void AddFundDetail(FundDetail entity)
        {
            this.Add<FundDetail>(entity);
        }
        public void AddFundDetailArray(FundDetail[] array)
        {
            this.Add<FundDetail>(array);
        }
        public void AddFillMoney(FillMoney entity)
        {
            this.Add<FillMoney>(entity);
        }
        public void AddWithdraw(Withdraw entity)
        {
            this.Add<Withdraw>(entity);
        }
        public void AddRedBagDetail(RedBagDetail entity)
        {
            this.Add<RedBagDetail>(entity);
        }
        public void AddOCDouDouDetail(OCDouDouDetail entity)
        {
            this.Add<OCDouDouDetail>(entity);
        }
        public void AddUserGrowthDetail(UserGrowthDetail entity)
        {
            this.Add<UserGrowthDetail>(entity);
        }

        public void UpdateFundDetail(params FundDetail[] entity)
        {
            this.Update<FundDetail>(entity);
        }
        public void UpdateFillMoney(FillMoney entity)
        {
            this.Update<FillMoney>(entity);
        }
        public void UpdateWithdraw(Withdraw entity)
        {
            this.Update<Withdraw>(entity);
        }
        public FundDetail QueryFundDetailByOrderId(string orderId)
        {
            this.Session.Clear();
            return this.Session.Query<FundDetail>().FirstOrDefault(p => p.OrderId == orderId && p.CreateTime > DateTime.Now.AddDays(-1) && p.CreateTime < DateTime.Now.AddDays(1));
        }

        public decimal GetUserSumFillMoney(string userId)
        {
            Session.Clear();
            var query = from f in this.Session.Query<FillMoney>()
                        where f.UserId == userId && f.Status == FillMoneyStatus.Success && f.ResponseMoney.HasValue
                        select f;
            if (query.Count() == 0) return 0M;
            return query.Sum(p => p.ResponseMoney.Value);
        }
        public FundDetail[] QueryFundDetail(string keyLine)
        {
            Session.Clear();
            return (from d in this.Session.Query<FundDetail>() where d.KeyLine == keyLine select d).ToArray();
        }
        public FundDetail[] QueryFundDetailByKeyLineUserId(string keyLine, string userId)
        {
            Session.Clear();
            return (from f in this.Session.Query<FundDetail>() where f.KeyLine == keyLine && f.UserId == userId select f).ToArray();
        }
        public List<FundDetail> QueryFundDetailList(string orderId, string userId)
        {
            Session.Clear();
            return (from f in this.Session.Query<FundDetail>() where f.OrderId == orderId && f.UserId == userId select f).ToList();
        }

        /// <summary>
        /// 查询用户投注金额
        /// </summary>
        public decimal QueryBetMoney(string userId)
        {
            //Session.Clear();
            //var query = from f in this.Session.Query<FundDetail>()
            //            where f.UserId == userId
            //            && f.Category == BusinessHelper.FundCategory_Betting
            //            && f.PayType == PayType.Payout
            //            select f;
            //if (query.Count() <= 0)
            //    return 0M;
            //return query.Sum(p => p.PayMoney);
            Session.Clear();
            var query = Session.Query<OrderDetail>().Where(s => s.UserId == userId).ToList();
            if (query != null && query.Count() > 0)
                return query.Sum(s => s.CurrentBettingMoney);
            return 0M;
        }

        /// <summary>
        /// 查询用户每天投注金额
        /// </summary>
        public decimal QueryEveryDayBuyLottery(string userId)
        {
            Session.Clear();
            var everDayList = this.Session.Query<FundDetail>().Where(f => f.UserId == userId && f.Category == BusinessHelper.FundCategory_Betting && f.PayType == PayType.Payout && (f.CreateTime > DateTime.Parse(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 23:59:59")) && f.CreateTime <= DateTime.Now)).ToList();
            if (everDayList.Count > 0)
                return everDayList.Sum(f => f.PayMoney);
            return 0M;
        }

        public FundDetail QueryFundDetail(string orderId, string userId)
        {
            Session.Clear();
            return (from f in this.Session.Query<FundDetail>() where f.OrderId == orderId && f.UserId == userId select f).FirstOrDefault();
        }
        public FundDetail QueryUserClearChaseRecord(string orderId, string userId)
        {
            Session.Clear();
            return this.Session.Query<FundDetail>().FirstOrDefault(p => p.UserId == userId && p.OrderId == orderId && p.PayType == PayType.Payout && p.AccountType == AccountType.Freeze);
        }

        public FillMoney QueryFillMoney(string orderId)
        {
            Session.Clear();
            return this.Session.Query<FillMoney>().FirstOrDefault(f => f.OrderId == orderId);
            //return this.SingleByKey<FillMoney>(orderId);
        }

        public FillMoney QueryFillMoneyByOrderId(string orderId)
        {
            Session.Clear();
            return this.Session.Query<FillMoney>().FirstOrDefault(f => f.OrderId == orderId && (f.FillMoneyAgent == FillMoneyAgentType.Alipay || f.FillMoneyAgent == FillMoneyAgentType.AlipayWAP));
            //return this.SingleByKey<FillMoney>(orderId);
        }
        public RedBagDetail QueryRedBagDetail(string orderId)
        {
            Session.Clear();
            return this.Session.Query<RedBagDetail>().FirstOrDefault(f => f.OrderId == orderId);
        }

        public List<FillMoney> QueryLastFillMoney(string userId, int pageIndex, int pageSize)
        {
            Session.Clear();
            var query = from f in this.Session.Query<FillMoney>()
                        where f.UserId == userId && f.Status == FillMoneyStatus.Success && f.FillMoneyAgent != FillMoneyAgentType.QuDao
                        orderby f.RequestTime descending
                        select f;
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public int QueryFillMoneyCount(string userId)
        {
            Session.Clear();
            return this.Session.Query<FillMoney>().Count(p => p.UserId == userId && p.Status == FillMoneyStatus.Success);
        }

        public decimal QueryFillMoneyAccountPayMoney(DateTime startTime, string userId)
        {
            Session.Clear();
            var query = from d in this.Session.Query<FundDetail>()
                        where d.UserId == userId && d.AccountType == AccountType.FillMoney && d.PayType == PayType.Payout && d.Category == BusinessHelper.FundCategory_Betting && d.CreateTime >= startTime
                        select d;
            if (query.Count() <= 0)
                return 0;
            return query.Sum(p => p.PayMoney);
        }

        public int QueryTodayWithdrawTimes(string userId)
        {
            Session.Clear();
            var query = from d in this.Session.Query<Withdraw>()
                        where d.UserId == userId && d.RequestTime >= DateTime.Now.Date && d.RequestTime < DateTime.Now.AddDays(1).Date
                        select d;
            return query.Count();
        }

        public FillMoneyQueryInfo QueryFillMoneyInfo(string orderId)
        {
            Session.Clear();
            var query = from f in this.Session.Query<FillMoney>()
                        join u in this.Session.Query<UserRegister>() on f.UserId equals u.UserId
                        where f.OrderId == orderId
                        select new FillMoneyQueryInfo
                        {
                            DeliveryAddress = f.DeliveryAddress,
                            FillMoneyAgent = f.FillMoneyAgent,
                            GoodsDescription = f.GoodsDescription,
                            GoodsName = f.GoodsName,
                            GoodsType = f.GoodsType,
                            IsNeedDelivery = f.IsNeedDelivery,
                            NotifyUrl = f.NotifyUrl,
                            OrderId = f.OrderId,
                            OuterFlowId = f.OuterFlowId,
                            PayMoney = f.PayMoney,
                            RequestBy = f.RequestBy,
                            RequestExtensionInfo = f.RequestExtensionInfo,
                            RequestMoney = f.RequestMoney,
                            RequestTime = f.RequestTime,
                            ResponseBy = f.ResponseBy,
                            ResponseCode = f.ResponseCode,
                            ResponseMessage = f.ResponseMessage,
                            ResponseMoney = f.ResponseMoney,
                            ResponseTime = f.ResponseTime,
                            ReturnUrl = f.ReturnUrl,
                            ShowUrl = f.ShowUrl,
                            Status = f.Status,
                            UserComeFrom = u.ComeFrom,
                            UserDisplayName = u.DisplayName,
                            UserId = u.UserId,
                            SchemeSource = f.SchemeSource
                        };
            return query.FirstOrDefault();
        }

        public Withdraw QueryWithdraw(string orderId)
        {
            Session.Clear();
            return this.Session.Query<Withdraw>().FirstOrDefault(d => d.OrderId == orderId);
            //return this.SingleByKey<Withdraw>(orderId);
        }

        /// <summary>
        /// 快捷充值 同卡进出 返回快捷充值的总金额 
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="expressAgentId">快捷agentid</param>
        /// <param name="bankCardNumber">快捷卡号</param>
        /// <returns></returns>
        public decimal QueryExpressTotalMoney(string userId, string expressAgentId, string bankCardNumber)
        {
            Session.Clear();
            var query = this.Session.Query<FillMoney>().Where(p => p.UserId == userId && p.Status == FillMoneyStatus.Success && expressAgentId.Contains(p.FillMoneyAgent.ToString()) && p.RequestExtensionInfo == bankCardNumber);
            if (query.Count() == 0) return 0M;
            return query.Sum(p => p.ResponseMoney.Value);
        }
        /// <summary>
        /// 判断该用户是否有快捷充值记录 返回快捷充值记录次数
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="expressAgentId"></param>
        /// <returns></returns>
        public decimal QueryExpressCount(string userId, string expressAgentId)
        {
            Session.Clear();
            var query = this.Session.Query<FillMoney>().Where(p => p.UserId == userId && p.Status == FillMoneyStatus.Success && expressAgentId.Contains(p.FillMoneyAgent.ToString()));
            return query.Count();
        }

        public decimal QueryMonthFillMoney()
        {
            var startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
            var endTime = DateTime.Today.AddDays(1);

            Session.Clear();
            var query = this.Session.Query<FillMoney>().Where(p => p.Status == FillMoneyStatus.Success
                && p.ResponseMoney.HasValue
                && p.ResponseTime.HasValue
                && (p.FillMoneyAgent != FillMoneyAgentType.ManualAdd && p.FillMoneyAgent != FillMoneyAgentType.ManualDeduct)
                && p.RequestTime >= startTime
                && p.RequestTime < endTime);
            if (query.Count() == 0) return 0M;
            return query.Sum(p => p.ResponseMoney.Value);
        }
        public decimal QueryMonthWithdraw()
        {
            var startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
            var endTime = DateTime.Today.AddDays(1);

            Session.Clear();
            var query = this.Session.Query<Withdraw>().Where(p => p.Status == WithdrawStatus.Success
                && p.ResponseMoney.HasValue
                && p.ResponseTime.HasValue
                && p.RequestTime >= startTime
                && p.RequestTime < endTime);
            if (query.Count() == 0) return 0M;
            return query.Sum(p => p.ResponseMoney.Value);
        }

        public FinanceSettingsInfo GetFinanceSettingsInfo(string userId, string operateType)
        {
            Session.Clear();
            string strSql = "select FinanceId,isnull(UserId,'')as UserId,isnull(OperateRank,'')as OperateRank,isnull(OperateType,'')as OperateType,isnull(MinMoney,0)as MinMoney,isnull(MaxMoney,0)as MaxMoney,isnull(OperatorId,'')as OperatorId,isnull(CreateTime,'')as CreateTime from C_FinanceSettings where UserId=:userId and OperateType=:operateType";
            var query = Session.CreateSQLQuery(strSql)
                             .SetString("userId", userId)
                             .SetString("operateType", operateType)
                             .List();
            FinanceSettingsInfo info = new FinanceSettingsInfo();
            if (query != null && query.Count > 0)
            {
                foreach (var item in query)
                {
                    var array = item as object[];
                    info.FinanceId = Convert.ToInt32(array[0]);
                    info.UserId = array[1].ToString();
                    info.OperateRank = array[2].ToString();
                    info.OperateType = array[3].ToString();
                    info.MinMoney = Convert.ToDecimal(array[4]);
                    info.MaxMoney = Convert.ToDecimal(array[5]);
                    info.OperatorId = array[6].ToString();
                    info.CreateTime = Convert.ToDateTime(array[7]);
                }
            }
            return info;
        }

        /// <summary>
        /// 查询某人成长值的赠送记录
        /// </summary>
        public List<UserGrowthDetailInfo> QueryUserGrowthDetailList(string userId, DateTime starTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageSize = pageSize > BusinessHelper.MaxPageSize ? BusinessHelper.MaxPageSize : pageSize;
            var query = from r in this.Session.Query<UserGrowthDetail>()
                        where r.UserId == userId
                        && (r.CreateTime >= starTime && r.CreateTime < endTime)
                        && r.PayMoney > 0
                        orderby r.CreateTime descending
                        select new UserGrowthDetailInfo
                        {
                            Category = r.Category,
                            PayMoney = r.PayMoney,
                            CreateTime = r.CreateTime,
                            PayType = r.PayType,
                            AfterBalance = r.AfterBalance,
                            OrderId = r.OrderId,
                            Summary = r.Summary,
                            UserId = r.UserId
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
        /// <summary>
        /// 查询用户成长值列表
        /// </summary>
        public UserGrowthDetailInfoCollection QueryUserGrowList(string userId, DateTime starTime, DateTime endTime, int pageIndex, int pageSize)
        {
            Session.Clear();
            starTime = starTime.Date;
            endTime = endTime.AddDays(1).Date;
            UserGrowthDetailInfoCollection collection = new UserGrowthDetailInfoCollection();
            collection.TotalCount = 0;
            var query = from u in Session.Query<UserGrowthDetail>()
                        where (userId == string.Empty || u.UserId == userId) && (u.CreateTime >= starTime && u.CreateTime < endTime)
                        select new UserGrowthDetailInfo
                        {
                            AfterBalance = u.AfterBalance == null ? 0 : Convert.ToInt32(u.AfterBalance),
                            Category = u.Category == null ? string.Empty : u.Category.ToString(),
                            CreateTime = Convert.ToDateTime(u.CreateTime),
                            OrderId = u.OrderId == null ? string.Empty : u.OrderId.ToString(),
                            PayMoney = u.PayMoney == null ? 0 : Convert.ToInt32(u.PayType),
                            PayType = u.PayType,
                            Summary = u.Summary == null ? string.Empty : u.Summary.ToString(),
                            UserId = u.UserId == null ? string.Empty : u.UserId.ToString(),
                        };
            if (query != null)
            {
                collection.TotalCount = query.Count();
                collection.TotalPayInMoney = query.ToList().Where(s => s.PayType == PayType.Payin).Sum(s => s.PayMoney);
                collection.TotalPayOutMoney = query.ToList().Where(s => s.PayType == PayType.Payout).Sum(s => s.PayMoney);
                collection.List = query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            return collection;
        }

        /// <summary>
        /// 查询订单的 投注金额中 红包参与投注金额
        /// </summary>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public decimal QuerySchemeRedBagTotalJoinMoney(string schemeId)
        {
            Session.Clear();
            var query = from f in this.Session.Query<FundDetail>()
                        where f.KeyLine.Contains(schemeId)
                        && f.PayType == PayType.Payout
                        && f.AccountType == AccountType.RedBag
                        select f;
            if (query.Count() <= 0)
                return 0M;
            return query.Sum(p => p.PayMoney);
        }
        public decimal QueryUserFillMoneyByUserId(string userId)
        {
            Session.Clear();
            var query = from fd in Session.Query<FundDetail>() join f in Session.Query<FillMoney>() on fd.OrderId equals f.OrderId where fd.AccountType == AccountType.FillMoney && f.UserId == userId && f.FillMoneyAgent != FillMoneyAgentType.QuDao select f.ResponseMoney;
            if (query != null)
                return query.Sum(s => s.Value);
            return 0M;
        }
        public decimal QueryAgentFreezeBalanceByUserId(string userId)
        {
            Session.Clear();
            var query = from w in Session.Query<Withdraw>() join f in Session.Query<FundDetail>() on w.OrderId equals f.OrderId where w.UserId == userId && w.Status == WithdrawStatus.Requesting && f.AccountType == AccountType.Freeze && f.Category == BusinessHelper.FundCategory_IntegralRequestWithdraw select f.PayMoney;
            if (query != null && query.Count() > 0)
                return query.Sum(s => s);
            return 0M;
        }

        /// <summary>
        /// 查询红包使用规则
        /// </summary>
        public decimal QueryRedBagUseConfig(string gameCode)
        {
            this.Session.Clear();
            return this.Session.CreateSQLQuery(string.Format("select [UsePercent] from [E_A20150919_红包使用配置] where gamecode='{0}'", gameCode)).UniqueResult<decimal>();
        }

        /// <summary>
        /// 查询用户总充值金额
        /// </summary>
        public decimal QueryUserTotalFillMoney(string userId)
        {
            Session.Clear();
            return this.Session.Query<FillMoney>().Where(p=>p.UserId==userId && p.Status== FillMoneyStatus.Success).Sum(p=>p.RequestMoney);
        }

        /// <summary>
        /// 查询用户总提现金额
        /// </summary>
        public decimal QueryUserTotalWithdrawMoney(string userId)
        {
            Session.Clear();
            return this.Session.Query<Withdraw>().Where(p=>p.UserId==userId && p.Status== WithdrawStatus.Success).Sum(p=>p.RequestMoney);
        }

    }
}
