using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class SSQ_HeZhiManager : DBbase
    {
        public void AddSSQ_HeZhi(SSQ_HeZhi entity)
        {
            LottertDataDB.GetDal<SSQ_HeZhi>().Add(entity);
        }

        public SSQ_HeZhi QueryLastSSQ_HeZhi()
        {
             
            return LottertDataDB.CreateQuery<SSQ_HeZhi>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }
        public List<SSQ_HeZhi> QuerySSQ_HeZhi(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<SSQ_HeZhi>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询SSQ_HeZhi本期是否生成
        /// </summary>
        public int QuerySSQ_HeZhiIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<SSQ_HeZhi>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
