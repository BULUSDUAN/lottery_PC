using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class YDJ11_GHZSManager : DBbase
    {
        public void AddYDJ11_GHZS(YDJ11_GHZS entity)
        {
            LottertDataDB.GetDal<YDJ11_GHZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public YDJ11_GHZS QueryLastYDJ11_GHZS()
        {
             
            return LottertDataDB.CreateQuery<YDJ11_GHZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<YDJ11_GHZS> QueryYDJ11_GHZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<YDJ11_GHZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询YDJ11_GHZS本期是否生成
        /// </summary>
        public int QueryYDJ11_GHZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<YDJ11_GHZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
