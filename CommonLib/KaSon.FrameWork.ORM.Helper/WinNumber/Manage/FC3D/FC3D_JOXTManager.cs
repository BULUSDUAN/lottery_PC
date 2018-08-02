using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class FC3D_JOXTManager : DBbase
    {
        public void AddFC3D_JOXT(FC3D_JOXT entity)
        {
            LottertDataDB.GetDal<FC3D_JOXT>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public FC3D_JOXT QueryLastFC3D_JOXT()
        {
             
            return LottertDataDB.CreateQuery<FC3D_JOXT>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<FC3D_JOXT> QueryFC3D_JOXT(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<FC3D_JOXT>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询FC3D_JOXT本期是否生成
        /// </summary>
        public int QueryFC3D_JOXTIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<FC3D_JOXT>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

    }
}
