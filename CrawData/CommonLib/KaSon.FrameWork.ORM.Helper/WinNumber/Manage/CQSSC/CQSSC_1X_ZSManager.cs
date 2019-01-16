using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;
namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class CQSSC_1X_ZSManager : DBbase
    {
        public void AddCQSSC_1X_ZS(CQSSC_1X_ZS entity)
        {
            LottertDataDB.GetDal<CQSSC_1X_ZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQSSC_1X_ZS QueryLastCQSSC_1X_ZS()
        {

            return LottertDataDB.CreateQuery<CQSSC_1X_ZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQSSC_1X_ZS> QueryCQSSC_1X_ZS(int index)
        {
           
            var query = from s in LottertDataDB.CreateQuery<CQSSC_1X_ZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQSSC_1X_ZS本期是否生成
        /// </summary>
        public int QueryCQSSC_1X_ZSIssuseNumber(string issuseNumber)
        {
           
            return LottertDataDB.CreateQuery<CQSSC_1X_ZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
