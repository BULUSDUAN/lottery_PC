using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class FC3D_HZZSManager : DBbase
    {
        public void AddFC3D_HZZS(FC3D_HZZS entity)
        {
            LottertDataDB.GetDal<FC3D_HZZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public FC3D_HZZS QueryLastFC3D_HZZS()
        {
             
            return LottertDataDB.CreateQuery<FC3D_HZZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<FC3D_HZZS> QueryFC3D_HZZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<FC3D_HZZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询FC3D_HZZS本期是否生成
        /// </summary>
        public int QueryFC3D_HZZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<FC3D_HZZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
