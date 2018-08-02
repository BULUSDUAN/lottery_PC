using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;


namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class JXSSC_5X_JBZSManager : DBbase
    {
        public void AddJXSSC_5X_JBZS(JXSSC_5X_JBZS entity)
        {
            LottertDataDB.GetDal<JXSSC_5X_JBZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public JXSSC_5X_JBZS QueryLastJXSSC_5X_JBZS()
        {
             
            return LottertDataDB.CreateQuery<JXSSC_5X_JBZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<JXSSC_5X_JBZS> QueryJXSSC_5X_JBZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<JXSSC_5X_JBZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询JXSSC_5X_JBZS本期是否生成
        /// </summary>
        public int QueryJXSSC_5X_JBZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JXSSC_5X_JBZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
