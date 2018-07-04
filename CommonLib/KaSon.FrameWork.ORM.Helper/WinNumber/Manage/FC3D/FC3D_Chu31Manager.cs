using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class FC3D_Chu31Manager : DBbase
    {
        public void AddFC3D_Chu31(FC3D_Chu31 entity)
        {
            LottertDataDB.GetDal<FC3D_Chu31>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public FC3D_Chu31 QueryLastFC3D_Chu31()
        {
             
            return LottertDataDB.CreateQuery<FC3D_Chu31>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<FC3D_Chu31> QueryFC3D_Chu31(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<FC3D_Chu31>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询FC3D_Chu31本期是否生成
        /// </summary>
        public int QueryFC3D_Chu31IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<FC3D_Chu31>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
