using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class SSQ_DXManager : DBbase
    {
        public void AddSSQ_DX(SSQ_DX entity)
        {
            LottertDataDB.GetDal<SSQ_DX>().Add(entity);
        }

        public SSQ_DX QueryLastSSQ_DX()
        {
             
            return LottertDataDB.CreateQuery<SSQ_DX>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }
        public List<SSQ_DX> QuerySSQ_DX(int index) 
        {
             
            var query = from s in LottertDataDB.CreateQuery<SSQ_DX>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询SSQ_DX本期是否生成
        /// </summary>
        public int QuerySSQ_DXIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<SSQ_DX>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
