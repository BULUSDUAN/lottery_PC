using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class FC3D_HZTZManager : DBbase
    {
        public void AddFC3D_HZTZ(FC3D_HZTZ entity)
        {
            LottertDataDB.GetDal<FC3D_HZTZ>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public FC3D_HZTZ QueryLastFC3D_HZTZ()
        {
             
            return LottertDataDB.CreateQuery<FC3D_HZTZ>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<FC3D_HZTZ> QueryFC3D_HZTZ(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<FC3D_HZTZ>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询FC3D_HZTZ本期是否生成
        /// </summary>
        public int QueryFC3D_HZTZIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<FC3D_HZTZ>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }

    }
}
