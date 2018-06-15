using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class CQSSC_DXDSManager : DBbase
    {
        public void AddCQSSC_DXDS(CQSSC_DXDS entity)
        {
            LottertDataDB.GetDal<CQSSC_DXDS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQSSC_DXDS QueryLastCQSSC_DXDS()
        {
             
            return LottertDataDB.CreateQuery<CQSSC_DXDS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQSSC_DXDS> QueryCQSSC_DXDS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<CQSSC_DXDS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQSSC_DXDS本期是否生成
        /// </summary>
        public int QueryCQSSC_DXDSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<CQSSC_DXDS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
