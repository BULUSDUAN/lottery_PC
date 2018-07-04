using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class FC3D_HZFBManager : DBbase
    {
        public void AddFC3D_HZFB(FC3D_HZFB entity)
        {
            LottertDataDB.GetDal<FC3D_HZFB>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public FC3D_HZFB QueryLastFC3D_HZFB()
        {
             
            return LottertDataDB.CreateQuery<FC3D_HZFB>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<FC3D_HZFB> QueryFC3D_HZFB(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<FC3D_HZFB>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询FC3D_HZFB本期是否生成
        /// </summary>
        public int QueryFC3D_HZFBIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<FC3D_HZFB>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
