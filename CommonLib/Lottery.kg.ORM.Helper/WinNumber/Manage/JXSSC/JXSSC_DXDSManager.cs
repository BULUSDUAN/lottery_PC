using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class JXSSC_DXDSManager : DBbase
    {
        public void AddJXSSC_DXDS(JXSSC_DXDS entity)
        {
            LottertDataDB.GetDal<JXSSC_DXDS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public JXSSC_DXDS QueryLastJXSSC_DXDS()
        {
             
            return LottertDataDB.CreateQuery<JXSSC_DXDS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<JXSSC_DXDS> QueryJXSSC_DXDS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<JXSSC_DXDS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询JXSSC_DXDS本期是否生成
        /// </summary>
        public int QueryJXSSC_DXDSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JXSSC_DXDS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
