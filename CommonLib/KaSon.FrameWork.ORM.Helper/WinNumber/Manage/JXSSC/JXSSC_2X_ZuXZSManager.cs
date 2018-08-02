using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class JXSSC_2X_ZuXZSManager : DBbase
    {
        public void AddJXSSC_2X_ZuXZS(JXSSC_2X_ZuXZS entity)
        {
            LottertDataDB.GetDal<JXSSC_2X_ZuXZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public JXSSC_2X_ZuXZS QueryLastJXSSC_2X_ZuXZS()
        {
             
            return LottertDataDB.CreateQuery<JXSSC_2X_ZuXZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<JXSSC_2X_ZuXZS> QueryJXSSC_2X_ZuXZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<JXSSC_2X_ZuXZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询JXSSC_2X_ZuXZS本期是否生成
        /// </summary>
        public int QueryJXSSC_2X_ZuXZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JXSSC_2X_ZuXZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
