using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class YDJ11_012LZZSManager : DBbase
    {
        public void AddYDJ11_012LZZS(YDJ11_012LZZS entity)
        {
            LottertDataDB.GetDal<YDJ11_012LZZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public YDJ11_012LZZS QueryLastYDJ11_012LZZS()
        {
             
            return LottertDataDB.CreateQuery<YDJ11_012LZZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<YDJ11_012LZZS> QueryYDJ11_012LZZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<YDJ11_012LZZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询YDJ11_012LZZS本期是否生成
        /// </summary>
        public int QueryYDJ11_012LZZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<YDJ11_012LZZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
