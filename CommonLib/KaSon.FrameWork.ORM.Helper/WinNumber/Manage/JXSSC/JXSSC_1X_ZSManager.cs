using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class JXSSC_1X_ZSManager : DBbase
    {
        public void AddJXSSC_1X_ZS(JXSSC_1X_ZS entity)
        {
            LottertDataDB.GetDal<JXSSC_1X_ZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public JXSSC_1X_ZS QueryLastJXSSC_1X_ZS()
        {
             
            return LottertDataDB.CreateQuery<JXSSC_1X_ZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<JXSSC_1X_ZS> QueryJXSSC_1X_ZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<JXSSC_1X_ZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询JXSSC_1X_ZS本期是否生成
        /// </summary>
        public int QueryJXSSC_1X_ZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JXSSC_1X_ZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
