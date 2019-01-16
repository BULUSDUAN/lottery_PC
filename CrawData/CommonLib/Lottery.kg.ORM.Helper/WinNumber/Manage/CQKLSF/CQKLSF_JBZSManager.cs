using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;
namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class CQKLSF_JBZSManager : DBbase
    {
        public void AddCQKLSF_JBZS(CQKLSF_JBZS entity)
        {
            LottertDataDB.GetDal<CQKLSF_JBZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQKLSF_JBZS QueryLastCQKLSF_JBZS()
        {
            return LottertDataDB.CreateQuery<CQKLSF_JBZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQKLSF_JBZS> QueryCQKLSF_JBZS(int index)
        {
            var query = from s in LottertDataDB.CreateQuery<CQKLSF_JBZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQKLSF_JBZS本期是否生成
        /// </summary>
        public int QueryCQKLSF_JBZSIssuseNumber(string issuseNumber)
        {

            return LottertDataDB.CreateQuery<CQKLSF_JBZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
