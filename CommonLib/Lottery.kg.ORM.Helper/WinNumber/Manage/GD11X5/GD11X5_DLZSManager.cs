using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class GD11X5_DLZSManager : DBbase
    {
        public void AddGD11X5_DLZS(GD11X5_DLZS entity)
        {
            LottertDataDB.GetDal<GD11X5_DLZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public GD11X5_DLZS QueryLastGD11X5_DLZS()
        {
             
            return LottertDataDB.CreateQuery<GD11X5_DLZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<GD11X5_DLZS> QueryGD11X5_DLZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<GD11X5_DLZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询GD11X5_DLZS本期是否生成
        /// </summary>
        public int QueryGD11X5_DLZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<GD11X5_DLZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
