using Lottery.Kg.ORM.Helper.WinNumber.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class CQ11X5_012LZZSManager : DBbase
    {
        public void AddCQ11X5_012LZZS(CQ11X5_012LZZS entity)
        {
            LottertDataDB.GetDal<CQ11X5_012LZZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQ11X5_012LZZS QueryLastCQ11X5_012LZZS()
        {
            
            return LottertDataDB.CreateQuery<CQ11X5_012LZZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQ11X5_012LZZS> QueryCQ11X5_012LZZS(int index)
        {            
            var query = from s in LottertDataDB.CreateQuery<CQ11X5_012LZZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQ11X5_012LZZS本期是否生成
        /// </summary>
        public int QueryCQ11X5_012LZZSIssuseNumber(string issuseNumber)
        {           
            return LottertDataDB.CreateQuery<CQ11X5_012LZZS>().Count(p => p.IssuseNumber == issuseNumber);
        }

    }
}
