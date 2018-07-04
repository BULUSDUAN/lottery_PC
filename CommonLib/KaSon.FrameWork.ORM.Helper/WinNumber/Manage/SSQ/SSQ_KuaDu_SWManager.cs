using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class SSQ_KuaDu_SWManager : DBbase
    {
        public void AddSSQ_KuaDu_SW(SSQ_KuaDu_SW entity)
        {
            LottertDataDB.GetDal<SSQ_KuaDu_SW>().Add(entity);
        }

        public SSQ_KuaDu_SW QueryLastSSQ_KuaDu_SW()
        {
             
            return LottertDataDB.CreateQuery<SSQ_KuaDu_SW>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }
        public List<SSQ_KuaDu_SW> QuerySSQ_KuaDu_SW(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<SSQ_KuaDu_SW>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询SSQ_KuaDu_SW本期是否生成
        /// </summary>
        public int QuerySSQ_KuaDu_SWIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<SSQ_KuaDu_SW>().Count(p => p.IssuseNumber == issuseNumber);
        }

    }
}
