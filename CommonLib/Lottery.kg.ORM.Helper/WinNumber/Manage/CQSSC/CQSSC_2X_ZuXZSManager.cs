using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;
namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class CQSSC_2X_ZuXZSManager : DBbase
    {
        public void AddCQSSC_2X_ZuXZS(CQSSC_2X_ZuXZS entity)
        {
            LottertDataDB.GetDal<CQSSC_2X_ZuXZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQSSC_2X_ZuXZS QueryLastCQSSC_2X_ZuXZS()
        {        
            return LottertDataDB.CreateQuery<CQSSC_2X_ZuXZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQSSC_2X_ZuXZS> QueryCQSSC_2X_ZuXZS(int index)
        {
            var query = from s in LottertDataDB.CreateQuery<CQSSC_2X_ZuXZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQSSC_2X_ZuXZS本期是否生成
        /// </summary>
        public int QueryCQSSC_2X_ZuXZSIssuseNumber(string issuseNumber)
        {
            
            return LottertDataDB.CreateQuery<CQSSC_2X_ZuXZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
