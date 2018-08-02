using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class SSQ_JiOuManager : DBbase
    {
        public void AddSSQ_JiOu(SSQ_JiOu entity)
        {
            LottertDataDB.GetDal<SSQ_JiOu>().Add(entity);
        }

        public SSQ_JiOu QueryLastSSQ_JiOu()
        {
             
            return LottertDataDB.CreateQuery<SSQ_JiOu>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }
        public List<SSQ_JiOu> QuerySSQ_JiOu(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<SSQ_JiOu>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询SSQ_JiOu本期是否生成
        /// </summary>
        public int QuerySSQ_JiOuIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<SSQ_JiOu>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
