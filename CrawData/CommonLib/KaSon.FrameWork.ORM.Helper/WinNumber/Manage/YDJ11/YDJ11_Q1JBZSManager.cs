using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class YDJ11_Q1JBZSManager : DBbase
    {
        public void AddYDJ11_Q1JBZS(YDJ11_Q1JBZS entity)
        {
            LottertDataDB.GetDal<YDJ11_Q1JBZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public YDJ11_Q1JBZS QueryLastYDJ11_Q1JBZS()
        {
             
            return LottertDataDB.CreateQuery<YDJ11_Q1JBZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<YDJ11_Q1JBZS> QueryYDJ11_Q1JBZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<YDJ11_Q1JBZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询YDJ11_Q1JBZS本期是否生成
        /// </summary>
        public int QueryYDJ11_Q1JBZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<YDJ11_Q1JBZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
