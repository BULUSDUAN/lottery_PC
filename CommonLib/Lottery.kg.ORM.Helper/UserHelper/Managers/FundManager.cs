using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Lottery.Kg.ORM.Helper.UserHelper
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

        /// <summary>
        /// 资金明细
        /// </summary>
        /// <param name="RedBagDetail"></param>
        public void AddFundDetail(C_Fund_Detail FundDetail)
        {

            DB.GetDal<C_Fund_Detail>().Add(FundDetail);
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
    }
}
