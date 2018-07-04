using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class FC3D_ZHXTManager : DBbase
    {
        public void AddFC3D_ZHXT(FC3D_ZHXT entity)
        {
            LottertDataDB.GetDal<FC3D_ZHXT>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public FC3D_ZHXT QueryLastFC3D_ZHXT()
        {
             
            return LottertDataDB.CreateQuery<FC3D_ZHXT>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<FC3D_ZHXT> QueryFC3D_ZHXT(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<FC3D_ZHXT>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询FC3D_ZHXT本期是否生成
        /// </summary>
        public int QueryFC3D_ZHXTIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<FC3D_ZHXT>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
