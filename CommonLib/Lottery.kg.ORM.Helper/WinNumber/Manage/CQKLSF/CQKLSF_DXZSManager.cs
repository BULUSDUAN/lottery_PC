using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;
namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class CQKLSF_DXZSManager : DBbase
    {
        public void AddCQKLSF_DXZS(CQKLSF_DXZS entity)
        {
           LottertDataDB.GetDal<CQKLSF_DXZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQKLSF_DXZS QueryLastCQKLSF_DXZS()
        {            
            return LottertDataDB.CreateQuery<CQKLSF_DXZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQKLSF_DXZS> QueryCQKLSF_DXZS(int index)
        {
            var query = from s in LottertDataDB.CreateQuery<CQKLSF_DXZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQKLSF_DXZS本期是否生成
        /// </summary>
        public int QueryCQKLSF_DXZSIssuseNumber(string issuseNumber)
        {
            return LottertDataDB.CreateQuery<CQKLSF_DXZS>().Count(p => p.IssuseNumber == issuseNumber);
        }

    }
}
