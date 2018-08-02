using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EntityModel;
namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class CQKLSF_GHZSManager : DBbase
    {
        public void AddCQKLSF_GHZS(CQKLSF_GHZS entity)
        {
            LottertDataDB.GetDal<CQKLSF_GHZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQKLSF_GHZS QueryLastCQKLSF_GHZS()
        {
             
            return LottertDataDB.CreateQuery<CQKLSF_GHZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQKLSF_GHZS> QueryCQKLSF_GHZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<CQKLSF_GHZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQKLSF_GHZS本期是否生成
        /// </summary>
        public int QueryCQKLSF_GHZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<CQKLSF_GHZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

    }
}
