using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class JXSSC_3X_JOZSManager : DBbase
    {
        public void AddJXSSC_3X_JOZS(JXSSC_3X_JOZS entity)
        {
            LottertDataDB.GetDal<JXSSC_3X_JOZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public JXSSC_3X_JOZS QueryLastJXSSC_3X_JOZS()
        {
             
            return LottertDataDB.CreateQuery<JXSSC_3X_JOZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<JXSSC_3X_JOZS> QueryJXSSC_3X_JOZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<JXSSC_3X_JOZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询JXSSC_3X_JOZS本期是否生成
        /// </summary>
        public int QueryJXSSC_3X_JOZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JXSSC_3X_JOZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
