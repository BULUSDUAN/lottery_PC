using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class GD11X5_012DWZSManager : DBbase
    {
        public void AddGD11X5_012DWZS(GD11X5_012DWZS entity)
        {
            LottertDataDB.GetDal<GD11X5_012DWZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public GD11X5_012DWZS QueryLastGD11X5_012DWZS()
        {
             
            return LottertDataDB.CreateQuery<GD11X5_012DWZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<GD11X5_012DWZS> QueryGD11X5_012DWZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<GD11X5_012DWZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询GD11X5_012DWZS本期是否生成
        /// </summary>
        public int QueryGD11X5_012DWZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<GD11X5_012DWZS>().Count(p => p.IssuseNumber == issuseNumber);
        }

    }
}
