using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class FC3D_ZHHMManager : DBbase
    {
        public void AddFC3D_ZHHM(FC3D_ZHHM entity)
        {
            LottertDataDB.GetDal<FC3D_ZHHM>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public FC3D_ZHHM QueryLastFC3D_ZHHM()
        {
             
            return LottertDataDB.CreateQuery<FC3D_ZHHM>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<FC3D_ZHHM> QueryFC3D_ZHHM(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<FC3D_ZHHM>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询FC3D_ZHHM本期是否生成
        /// </summary>
        public int QueryFC3D_ZHHMIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<FC3D_ZHHM>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
