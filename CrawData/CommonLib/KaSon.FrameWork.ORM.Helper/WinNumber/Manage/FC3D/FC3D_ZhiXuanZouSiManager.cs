using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class FC3D_ZhiXuanZouSiManager : DBbase
    {
        /// <summary>
        /// 添加大乐透基本走势数据
        /// </summary>
        public void AddZhiXuanZouSi(FC3D_ZhiXuanZouSi entity)
        {
            LottertDataDB.GetDal<FC3D_ZhiXuanZouSi>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public FC3D_ZhiXuanZouSi QueryLastZhiXuanZouSi()
        {
             
            return LottertDataDB.CreateQuery<FC3D_ZhiXuanZouSi>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<FC3D_ZhiXuanZouSi> QueryFC3D_ZhiXuanZouSi(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<FC3D_ZhiXuanZouSi>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询FC3D_ZhiXuanZouSi本期是否生成
        /// </summary>
        public int QueryFC3D_ZhiXuanZouSiIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<FC3D_ZhiXuanZouSi>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
