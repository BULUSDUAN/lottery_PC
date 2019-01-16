using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class HNKLSF_JOZSManager : DBbase
    {
        public void AddHNKLSF_JOZS(HNKLSF_JOZS entity)
        {
            LottertDataDB.GetDal<HNKLSF_JOZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public HNKLSF_JOZS QueryLastHNKLSF_JOZS()
        {
             
            return LottertDataDB.CreateQuery<HNKLSF_JOZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<HNKLSF_JOZS> QueryHNKLSF_JOZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<HNKLSF_JOZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询HNKLSF_JOZS本期是否生成
        /// </summary>
        public int QueryHNKLSF_JOZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<HNKLSF_JOZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

    }
}
