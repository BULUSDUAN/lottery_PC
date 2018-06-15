using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class YDJ11_HZZSManager : DBbase
    {
        public void AddYDJ11_HZZS(YDJ11_HZZS entity)
        {
            LottertDataDB.GetDal<YDJ11_HZZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public YDJ11_HZZS QueryLastYDJ11_HZZS()
        {
             
            return LottertDataDB.CreateQuery<YDJ11_HZZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<YDJ11_HZZS> QueryYDJ11_HZZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<YDJ11_HZZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询YDJ11_HZZS本期是否生成
        /// </summary>
        public int QueryYDJ11_HZZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<YDJ11_HZZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
