using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class LN11X5_DLZSManager : DBbase
    {
        public void AddLN11X5_DLZS(LN11X5_DLZS entity)
        {
            LottertDataDB.GetDal<LN11X5_DLZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public LN11X5_DLZS QueryLastLN11X5_DLZS()
        {
             
            return LottertDataDB.CreateQuery<LN11X5_DLZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<LN11X5_DLZS> QueryLN11X5_DLZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<LN11X5_DLZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询LN11X5_DLZS本期是否生成
        /// </summary>
        public int QueryLN11X5_DLZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<LN11X5_DLZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
