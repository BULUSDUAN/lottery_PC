using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel.Enum;

namespace KaSon.FrameWork.ORM.Helper.UserHelper
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
        public C_Fund_Detail QueryUserClearChaseRecord(string orderId, string userId)
        {
            // Session.Clear();
            int payType = (int)PayType.Payout;
            int Freeze =(int)AccountType.Freeze;
            return this.DB.CreateQuery<C_Fund_Detail>().Where(p => p.UserId == userId && p.OrderId == orderId && p.PayType == payType && p.AccountType == Freeze).FirstOrDefault();
        }

        /// <summary>
        /// KASON
        /// </summary>
        /// <param name="gameCode"></param>
        /// <returns></returns>
        public decimal QueryRedBagUseConfig(string gameCode)
        {
          //  this.Session.Clear();
            return this.DB.CreateSQLQuery(string.Format("select DISTINCT [UsePercent] from [E_A20150919_红包使用配置] where gamecode='{0}'", gameCode)).First<decimal>();
        }
    }
}
