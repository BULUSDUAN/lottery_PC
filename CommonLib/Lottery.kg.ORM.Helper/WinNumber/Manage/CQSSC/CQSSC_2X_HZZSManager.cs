using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;
namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class CQSSC_2X_HZZSManager : DBbase
    {
        public void AddCQSSC_2X_HZZS(CQSSC_2X_HZZS entity)
        {
            LottertDataDB.GetDal<CQSSC_2X_HZZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQSSC_2X_HZZS QueryLastCQSSC_2X_HZZS()
        {
            
            return LottertDataDB.CreateQuery<CQSSC_2X_HZZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQSSC_2X_HZZS> QueryCQSSC_2X_HZZS(int index)
        {
           
            var query = from s in LottertDataDB.CreateQuery<CQSSC_2X_HZZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQSSC_2X_HZZS本期是否生成
        /// </summary>
        public int QueryCQSSC_2X_HZZSIssuseNumber(string issuseNumber)
        {
           
            return LottertDataDB.CreateQuery<CQSSC_2X_HZZS>().Count(p => p.IssuseNumber == issuseNumber);
        }

    }
}
