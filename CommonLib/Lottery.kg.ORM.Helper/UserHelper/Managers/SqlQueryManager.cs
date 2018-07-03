using EntityModel;
using EntityModel.CoreModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.UserHelper
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
            var query = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "P_Blog_UserSpreadGiveRedBagCount").SQL;
            var UserSpreadGiveRedBagCount_query = DB.CreateSQLQuery(query).SetString("AgentId", AgentId).List<E_Blog_UserSpread>();
               
           
            string str = string.Empty;
            //if (UserSpreadGiveRedBagCount_query.Count == 3)
            //{
            //    var dtCount = UserSpreadGiveRedBagCount_query.;
            //    if (dtCount.Rows.Count > 0)
            //    {
            //        var row = dtCount.Rows[0];
            //        str += UsefullHelper.GetDbValue<int>(row[0]) + "|";
            //    }
            //    var dtSumCount = ds.Tables[1];
            //    if (dtSumCount.Rows.Count > 0)
            //    {
            //        var row = dtSumCount.Rows[0];
            //        str += UsefullHelper.GetDbValue<int>(row[0]) + "|";
            //    }
            //    var dtRedBagMoney = ds.Tables[2];
            //    if (dtRedBagMoney.Rows.Count > 0)
            //    {
            //        var row = dtRedBagMoney.Rows[0];
            //        str += UsefullHelper.GetDbValue<int>(row[0]);
            //    }
            //}
            return str;
        }

        public Withdraw_QueryInfo GetWithdrawById(string orderId)
        {
         
            // 通过数据库存储过程进行查询
            var result = SqlModule.UserSystemModule.FirstOrDefault(x => x.Key == "rj_Fund_QueryWithdrawById").SQL;
           var withdraw_Info = DB.CreateSQLQuery(result).SetString("OrderId", orderId)
                .First<Withdraw_QueryInfo>();

            return withdraw_Info;
        }
    }
}
