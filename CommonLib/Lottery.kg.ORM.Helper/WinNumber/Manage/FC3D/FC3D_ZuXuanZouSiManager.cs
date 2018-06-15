using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class FC3D_ZuXuanZouSiManager : DBbase
    {
        /// <summary>
        /// 添加大乐透基本走势数据
        /// </summary>
        public void AddFC3D_ZuXuanZouSi(FC3D_ZuXuanZouSi entity)
        {
            LottertDataDB.GetDal<FC3D_ZuXuanZouSi>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public FC3D_ZuXuanZouSi QueryLastFC3D_ZuXuanZouSi()
        {
             
            return LottertDataDB.CreateQuery<FC3D_ZuXuanZouSi>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<FC3D_ZuXuanZouSi> QueryFC3D_ZuXuanZouSi(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<FC3D_ZuXuanZouSi>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询FC3D_ZuXuanZouSi本期是否生成
        /// </summary>
        public int QueryFC3D_ZuXuanZouSiIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<FC3D_ZuXuanZouSi>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
