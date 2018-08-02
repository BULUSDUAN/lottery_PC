using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;
namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class CQKLSF_TWZSManager : DBbase
    {
        public void AddCQKLSF_TWZS(CQKLSF_TWZS entity)
        {
            LottertDataDB.GetDal<CQKLSF_TWZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQKLSF_TWZS QueryLastCQKLSF_TWZS()
        {
            return LottertDataDB.CreateQuery<CQKLSF_TWZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQKLSF_TWZS> QueryCQKLSF_TWZS(int index)
        {
            var query = from s in LottertDataDB.CreateQuery<CQKLSF_TWZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQ11X5_JBZS本期是否生成
        /// </summary>
        public int QueryCQKLSF_TWZSIssuseNumber(string issuseNumber)
        {
            return LottertDataDB.CreateQuery<CQKLSF_TWZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
