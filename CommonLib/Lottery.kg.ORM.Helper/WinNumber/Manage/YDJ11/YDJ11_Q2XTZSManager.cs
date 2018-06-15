using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class YDJ11_Q2XTZSManager : DBbase
    {
        public void AddYDJ11_Q2XTZS(YDJ11_Q2XTZS entity)
        {
            LottertDataDB.GetDal<YDJ11_Q2XTZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public YDJ11_Q2XTZS QueryLastYDJ11_Q2XTZS()
        {
             
            return LottertDataDB.CreateQuery<YDJ11_Q2XTZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<YDJ11_Q2XTZS> QueryYDJ11_Q2XTZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<YDJ11_Q2XTZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询YDJ11_Q2XTZS本期是否生成
        /// </summary>
        public int QueryYDJ11_Q2XTZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<YDJ11_Q2XTZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
