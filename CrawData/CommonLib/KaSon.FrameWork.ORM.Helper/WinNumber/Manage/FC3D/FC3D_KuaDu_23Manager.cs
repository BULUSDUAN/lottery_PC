using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class FC3D_KuaDu_23Manager : DBbase
    {
        public void AddFC3D_KuaDu_23(FC3D_KuaDu_23 entity)
        {
            LottertDataDB.GetDal<FC3D_KuaDu_23>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public FC3D_KuaDu_23 QueryLastFC3D_KuaDu_23()
        {
             
            return LottertDataDB.CreateQuery<FC3D_KuaDu_23>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<FC3D_KuaDu_23> QueryFC3D_KuaDu_23(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<FC3D_KuaDu_23>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询FC3D_KuaDu_23本期是否生成
        /// </summary>
        public int QueryFC3D_KuaDu_23IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<FC3D_KuaDu_23>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

    }
}
