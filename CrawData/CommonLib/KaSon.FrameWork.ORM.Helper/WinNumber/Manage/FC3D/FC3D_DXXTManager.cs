using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class FC3D_DXXTManager : DBbase
    {
        /// <summary>
        /// 添加大乐透基本走势数据
        /// </summary>
        public void AddFC3D_DXXT(FC3D_DXXT entity)
        {
            LottertDataDB.GetDal<FC3D_DXXT>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public FC3D_DXXT QueryLastFC3D_DXXT()
        {
             
            return LottertDataDB.CreateQuery<FC3D_DXXT>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<FC3D_DXXT> QueryFC3D_DXXT(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<FC3D_DXXT>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询FC3D_DXXT本期是否生成
        /// </summary>
        public int QueryFC3D_DXXTIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<FC3D_DXXT>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
