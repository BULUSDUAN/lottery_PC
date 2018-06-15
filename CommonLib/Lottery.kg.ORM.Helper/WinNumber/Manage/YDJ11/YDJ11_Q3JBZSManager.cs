using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class YDJ11_Q3JBZSManager : DBbase
    {
        public void AddYDJ11_Q3JBZS(YDJ11_Q3JBZS entity)
        {
            LottertDataDB.GetDal<YDJ11_Q3JBZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public YDJ11_Q3JBZS QueryLastYDJ11_Q3JBZS()
        {
             
            return LottertDataDB.CreateQuery<YDJ11_Q3JBZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<YDJ11_Q3JBZS> QueryYDJ11_Q3JBZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<YDJ11_Q3JBZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询YDJ11_Q3JBZS本期是否生成
        /// </summary>
        public int QueryYDJ11_Q3JBZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<YDJ11_Q3JBZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
