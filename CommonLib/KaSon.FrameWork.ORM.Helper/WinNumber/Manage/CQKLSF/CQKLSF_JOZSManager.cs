using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;
namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class CQKLSF_JOZSManager : DBbase
    {
        public void AddCQKLSF_JOZS(CQKLSF_JOZS entity)
        {
            LottertDataDB.GetDal<CQKLSF_JOZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQKLSF_JOZS QueryLastCQKLSF_JOZS()
        {
            return LottertDataDB.CreateQuery<CQKLSF_JOZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQKLSF_JOZS> QueryCQKLSF_JOZS(int index)
        {
            var query = from s in LottertDataDB.CreateQuery<CQKLSF_JOZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQKLSF_JOZS本期是否生成
        /// </summary>
        public int QueryCQKLSF_JOZSIssuseNumber(string issuseNumber)
        {
            return LottertDataDB.CreateQuery<CQKLSF_JOZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
