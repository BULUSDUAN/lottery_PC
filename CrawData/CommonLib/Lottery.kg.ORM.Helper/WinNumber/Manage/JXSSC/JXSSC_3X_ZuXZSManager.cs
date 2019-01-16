using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class JXSSC_3X_ZuXZSManager : DBbase
    {
        public void AddJXSSC_3X_ZuXZS(JXSSC_3X_ZuXZS entity)
        {
            LottertDataDB.GetDal<JXSSC_3X_ZuXZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public JXSSC_3X_ZuXZS QueryLastJXSSC_3X_ZuXZS()
        {
             
            return LottertDataDB.CreateQuery<JXSSC_3X_ZuXZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<JXSSC_3X_ZuXZS> QueryJXSSC_3X_ZuXZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<JXSSC_3X_ZuXZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询JXSSC_3X_ZuXZS本期是否生成
        /// </summary>
        public int QueryJXSSC_3X_ZuXZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JXSSC_3X_ZuXZS>().Count(p => p.IssuseNumber == issuseNumber);
        }

    }
}
