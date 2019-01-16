using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class JXSSC_3X_C3YSManager : DBbase
    {
        public void AddJXSSC_3X_C3YS(JXSSC_3X_C3YS entity)
        {
            LottertDataDB.GetDal<JXSSC_3X_C3YS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public JXSSC_3X_C3YS QueryLastJXSSC_3X_C3YS()
        {
             
            return LottertDataDB.CreateQuery<JXSSC_3X_C3YS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<JXSSC_3X_C3YS> QueryJXSSC_3X_C3YS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<JXSSC_3X_C3YS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询JXSSC_3X_C3YS本期是否生成
        /// </summary>
        public int QueryJXSSC_3X_C3YSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JXSSC_3X_C3YS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
