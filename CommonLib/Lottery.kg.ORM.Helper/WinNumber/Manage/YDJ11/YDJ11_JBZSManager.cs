using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class YDJ11_JBZSManager : DBbase
    {
        public void AddYDJ11_JBZS(YDJ11_JBZS entity)
        {
            LottertDataDB.GetDal<YDJ11_JBZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public YDJ11_JBZS QueryLastYDJ11_JBZS()
        {
             
            return LottertDataDB.CreateQuery<YDJ11_JBZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<YDJ11_JBZS> QueryYDJ11_JBZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<YDJ11_JBZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询YDJ11_JBZS本期是否生成
        /// </summary>
        public int QueryYDJ11_JBZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<YDJ11_JBZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
