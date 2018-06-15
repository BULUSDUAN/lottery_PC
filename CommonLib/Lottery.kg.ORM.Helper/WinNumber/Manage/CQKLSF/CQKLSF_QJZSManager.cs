using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;
namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class CQKLSF_QJZSManager : DBbase
    {
        public void AddCQKLSF_QJZS(CQKLSF_QJZS entity)
        {
            LottertDataDB.GetDal<CQKLSF_QJZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQKLSF_QJZS QueryLastCQKLSF_QJZS()
        {
          
            return LottertDataDB.CreateQuery<CQKLSF_QJZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQKLSF_QJZS> QueryCQKLSF_QJZS(int index)
        {

            var query = from s in LottertDataDB.CreateQuery<CQKLSF_QJZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQKLSF_QJZS本期是否生成
        /// </summary>
        public int QueryCQKLSF_QJZSIssuseNumber(string issuseNumber)
        {
            return LottertDataDB.CreateQuery<CQKLSF_QJZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
