using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class SSQ_ZhiHeManager : DBbase
    {
        public void AddSSQ_ZhiHe(SSQ_ZhiHe entity)
        {
            LottertDataDB.GetDal<SSQ_ZhiHe>().Add(entity);
        }

        public SSQ_ZhiHe QueryLastSSQ_ZhiHe()
        {
             
            return LottertDataDB.CreateQuery<SSQ_ZhiHe>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }
        public List<SSQ_ZhiHe> QuerySSQ_ZhiHe(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<SSQ_ZhiHe>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询SSQ_ZhiHe本期是否生成
        /// </summary>
        public int QuerySSQ_ZhiHeIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<SSQ_ZhiHe>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
