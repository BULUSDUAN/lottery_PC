using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class FC3D_KuaDu_ZManager : DBbase
    {
        public void AddFC3D_KuaDu_Z(FC3D_KuaDu_Z entity)
        {
            LottertDataDB.GetDal<FC3D_KuaDu_Z>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public FC3D_KuaDu_Z QueryLastFC3D_KuaDu_Z()
        {
             
            return LottertDataDB.CreateQuery<FC3D_KuaDu_Z>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<FC3D_KuaDu_Z> QueryFC3D_KuaDu_Z(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<FC3D_KuaDu_Z>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询FC3D_KuaDu_Z本期是否生成
        /// </summary>
        public int QueryFC3D_KuaDu_ZIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<FC3D_KuaDu_Z>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
