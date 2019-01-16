using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class SSQ_C3Manager : DBbase
    {
        public void AddSSQ_C3(SSQ_C3 entity)
        {
            LottertDataDB.GetDal<SSQ_C3>().Add(entity);
        }

        public SSQ_C3 QueryLastSSQ_C3()
        {
             
            return LottertDataDB.CreateQuery<SSQ_C3>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<SSQ_C3> QuerySSQ_C3(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<SSQ_C3>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询SSQ_C3本期是否生成
        /// </summary>
        public int QuerySSQ_C3IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<SSQ_C3>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

    }
}
