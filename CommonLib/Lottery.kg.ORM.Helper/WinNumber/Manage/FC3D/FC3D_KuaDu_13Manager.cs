using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class FC3D_KuaDu_13Manager : DBbase
    {
        public void AddFC3D_KuaDu_13(FC3D_KuaDu_13 entity)
        {
            LottertDataDB.GetDal<FC3D_KuaDu_13>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public FC3D_KuaDu_13 QueryLastFC3D_KuaDu_13()
        {
             
            return LottertDataDB.CreateQuery<FC3D_KuaDu_13>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<FC3D_KuaDu_13> QueryFC3D_KuaDu_13(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<FC3D_KuaDu_13>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询FC3D_KuaDu_13本期是否生成
        /// </summary>
        public int QueryFC3D_KuaDu_13IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<FC3D_KuaDu_13>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
