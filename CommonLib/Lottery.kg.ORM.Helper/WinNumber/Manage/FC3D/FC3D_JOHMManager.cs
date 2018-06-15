using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class FC3D_JOHMManager : DBbase
    {
        public void AddFC3D_JOHM(FC3D_JOHM entity)
        {
            LottertDataDB.GetDal<FC3D_JOHM>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public FC3D_JOHM QueryLastFC3D_JOHM()
        {
             
            return LottertDataDB.CreateQuery<FC3D_JOHM>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<FC3D_JOHM> QueryFC3D_JOHM(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<FC3D_JOHM>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询FC3D_JOHM本期是否生成
        /// </summary>
        public int QueryFC3D_JOHMIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<FC3D_JOHM>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
