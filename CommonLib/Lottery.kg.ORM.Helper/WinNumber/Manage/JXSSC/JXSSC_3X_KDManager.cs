using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class JXSSC_3X_KDManager : DBbase
    {
        public void AddJXSSC_3X_KD(JXSSC_3X_KD entity)
        {
            LottertDataDB.GetDal<JXSSC_3X_KD>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public JXSSC_3X_KD QueryLastJXSSC_3X_KD()
        {
             
            return LottertDataDB.CreateQuery<JXSSC_3X_KD>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<JXSSC_3X_KD> QueryJXSSC_3X_KD(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<JXSSC_3X_KD>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询JXSSC_3X_KD本期是否生成
        /// </summary>
        public int QueryJXSSC_3X_KDIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JXSSC_3X_KD>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
