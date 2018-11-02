using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel.Enum;

namespace KaSon.FrameWork.ORM.Helper
{
   public class FundManager:DBbase
    {
        /// <summary>
        /// 红包
        /// </summary>
        /// <param name="RedBagDetail"></param>
        public void AddRedBagDetail(C_Fund_RedBagDetail RedBagDetail)
        {

            DB.GetDal<C_Fund_RedBagDetail>().Add(RedBagDetail);
        }
        public void AddOCDouDouDetail(C_Fund_OCDouDouDetail entity)
        {
            DB.GetDal<C_Fund_OCDouDouDetail>().Add(entity);
         
        }

        /// <summary>
        /// 资金明细
        /// </summary>
        /// <param name="RedBagDetail"></param>
        public void AddFundDetail(C_Fund_Detail FundDetail)
        {
            DB.GetDal<C_Fund_Detail>().Add(FundDetail);
        }
        public C_Fund_Detail QueryUserClearChaseRecord(string orderId, string userId)
        {
            // Session.Clear();
            int payType = (int)PayType.Payout;
            int Freeze =(int)AccountType.Freeze;
            return this.DB.CreateQuery<C_Fund_Detail>().Where(p => p.UserId == userId && p.OrderId == orderId && p.PayType == payType && p.AccountType == Freeze).FirstOrDefault();
        }

        /// <summary>
        /// 红包使用配置 k_todo, make:是否可以放到内存，定时获取红包配置
        /// </summary>
        /// <param name="gameCode"></param>
        /// <returns></returns>
        public decimal QueryRedBagUseConfig(string gameCode)
        {
          //  this.Session.Clear();
            return this.DB.CreateSQLQuery(string.Format("select DISTINCT TOP 1 [UsePercent] from [E_A20150919_红包使用配置] where gamecode='{0}'", gameCode)).First<decimal>();
        }

        public void AddWithdraw(C_Withdraw entity)
        {
            DB.GetDal<C_Withdraw>().Add(entity);
        }

        public int QueryTodayWithdrawTimes(string userId)
        {

            var query = from d in DB.CreateQuery<C_Withdraw>()
                        where d.UserId == userId && d.RequestTime >= DateTime.Now.Date && d.RequestTime < DateTime.Now.AddDays(1).Date
                        select d;
            return query.Count();
        }

        public List<C_Fund_Detail> QueryFundDetailList(string orderId, string userId)
        {
            return DB.CreateQuery<C_Fund_Detail>().Where(f => f.OrderId == orderId && f.UserId == userId).ToList();
        }

        public int QueryFillMoneyCount(string userId)
        {
         
            return DB.CreateQuery<C_FillMoney>().Where(p => p.UserId == userId && p.Status == (int)FillMoneyStatus.Success).Count();
        }

        /// <summary>
        /// 查询用户投注金额
        /// </summary>
        public decimal QueryBetMoney(string userId)
        {
          
           
            var query = DB.CreateQuery<C_OrderDetail>().Where(s => s.UserId == userId).ToList();
            if (query != null && query.Count() > 0)
                return query.Sum(s => s.CurrentBettingMoney);
            return 0M;
        }
        public decimal QueryAgentFreezeBalanceByUserId(string userId)
        {
            var query = from w in DB.CreateQuery<C_Withdraw>()
                        join f in DB.CreateQuery<C_Fund_Detail>() 
                        on w.OrderId equals f.OrderId
                        where w.UserId == userId && w.Status == (int)WithdrawStatus.Requesting 
                        && f.AccountType == (int)AccountType.Freeze 
                        && f.Category == BusinessHelper.FundCategory_IntegralRequestWithdraw
                        select f.PayMoney;
            if (query != null && query.Count() > 0)
                return query.Sum(s => s);
            return 0M;
        }
        public C_FinanceSettings GetFinanceSettingsInfo(string userId, string operateType)
        {
            return DB.CreateQuery<C_FinanceSettings>().Where(x => x.UserId == userId && x.OperateType == operateType).FirstOrDefault();
        }
        public void AddFillMoney(C_FillMoney entity)
        {
            DB.GetDal<C_FillMoney>().Add(entity);
        }
        public bool UpdateUserCreditType(string userId, int updateUserCreditType)
        {
            string strSql = "Update C_User_Register set UserCreditType=@userCreditType Where userId=@userId";
            var flag = DB.CreateSQLQuery(strSql)
                             .SetString("@userId", userId)
                             .SetInt("@userCreditType", updateUserCreditType);
            return true;
        }
        public void UpdateFillMoney(C_FillMoney entity)
        {
            DB.GetDal<C_FillMoney>().Update(entity);
        }
        public C_FillMoney QueryFillMoney(string orderId)
        {
            return DB.CreateQuery<C_FillMoney>().Where(f => f.OrderId == orderId).FirstOrDefault();
        }
        public C_Withdraw QueryWithdraw(string orderId)
        {
            return DB.CreateQuery<C_Withdraw>().Where(d => d.OrderId == orderId).FirstOrDefault();
        }
        public void UpdateWithdraw(C_Withdraw entity)
        {
            DB.GetDal<C_Withdraw>().Update(entity);
        }

        /// <summary>
        /// 查询用户总充值金额
        /// </summary>
        public decimal QueryUserTotalFillMoney(string userId)
        {
            return DB.CreateQuery<C_FillMoney>().Where(p => p.UserId == userId && p.Status == (int)FillMoneyStatus.Success).Sum(p => p.RequestMoney);
        }

        /// <summary>
        /// 查询用户总提现金额
        /// </summary>
        public decimal QueryUserTotalWithdrawMoney(string userId)
        {
            return DB.CreateQuery<C_Withdraw>().Where(p => p.UserId == userId && p.Status == (int)WithdrawStatus.Success).Sum(p => p.RequestMoney);
        }
    }
}
