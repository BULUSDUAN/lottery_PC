using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class SSQ_KuaDu_1_6Manager : DBbase
    {
        public void AddSSQ_KuaDu_1_6(SSQ_KuaDu_1_6 entity)
        {
            LottertDataDB.GetDal<SSQ_KuaDu_1_6>().Add(entity);
        }

        public SSQ_KuaDu_1_6 QueryLastSSQ_KuaDu_1_6()
        {
             
            return LottertDataDB.CreateQuery<SSQ_KuaDu_1_6>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }
        public List<SSQ_KuaDu_1_6> QuerySSQ_KuaDu_1_6(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<SSQ_KuaDu_1_6>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
            //return query.Distinct(new ImportComparer()).Take(index).ToList();
        }

        /// <summary>
        /// 查询SSQ_KuaDu_1_6本期是否生成
        /// </summary>
        public int QuerySSQ_KuaDu_1_6IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<SSQ_KuaDu_1_6>().Count(p => p.IssuseNumber == issuseNumber);
        }

    }
}
