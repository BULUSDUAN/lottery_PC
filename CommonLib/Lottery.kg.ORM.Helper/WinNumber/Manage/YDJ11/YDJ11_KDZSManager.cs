using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class YDJ11_KDZSManager : DBbase
    {
        public void AddYDJ11_KDZS(YDJ11_KDZS entity)
        {
            LottertDataDB.GetDal<YDJ11_KDZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public YDJ11_KDZS QueryLastYDJ11_KDZS()
        {
             
            return LottertDataDB.CreateQuery<YDJ11_KDZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<YDJ11_KDZS> QueryYDJ11_KDZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<YDJ11_KDZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询YDJ11_KDZS本期是否生成
        /// </summary>
        public int QueryYDJ11_KDZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<YDJ11_KDZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
