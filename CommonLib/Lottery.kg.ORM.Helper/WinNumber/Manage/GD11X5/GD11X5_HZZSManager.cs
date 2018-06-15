using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class GD11X5_HZZSManager : DBbase
    {
        public void AddGD11X5_HZZS(GD11X5_HZZS entity)
        {
            LottertDataDB.GetDal<GD11X5_HZZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public GD11X5_HZZS QueryLastGD11X5_HZZS()
        {
             
            return LottertDataDB.CreateQuery<GD11X5_HZZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<GD11X5_HZZS> QueryGD11X5_HZZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<GD11X5_HZZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }
    }
}
