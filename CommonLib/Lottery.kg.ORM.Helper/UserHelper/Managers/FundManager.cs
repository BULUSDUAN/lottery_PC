using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
