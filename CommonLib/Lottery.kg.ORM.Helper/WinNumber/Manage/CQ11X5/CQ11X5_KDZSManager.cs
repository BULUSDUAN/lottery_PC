using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;
namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class CQ11X5_KDZSManager : DBbase
    {
        public void AddCQ11X5_KDZS(CQ11X5_KDZS entity)
        {
            LottertDataDB.GetDal<CQ11X5_KDZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQ11X5_KDZS QueryLastCQ11X5_KDZS()
        {
           
            return LottertDataDB.CreateQuery<CQ11X5_KDZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQ11X5_KDZS> QueryCQ11X5_KDZS(int index)
        {
           
            var query = from s in LottertDataDB.CreateQuery<CQ11X5_KDZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQ11X5_KDZS本期是否生成
        /// </summary>
        public int QueryCQ11X5_KDZSIssuseNumber(string issuseNumber)
        {
            return LottertDataDB.CreateQuery<CQ11X5_KDZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
