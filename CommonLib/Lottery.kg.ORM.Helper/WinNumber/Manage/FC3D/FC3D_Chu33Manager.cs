using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class FC3D_Chu33Manager : DBbase
    {
        public void AddFC3D_Chu33(FC3D_Chu33 entity)
        {
            LottertDataDB.GetDal<FC3D_Chu33>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public FC3D_Chu33 QueryLastFC3D_Chu33()
        {
             
            return LottertDataDB.CreateQuery<FC3D_Chu33>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<FC3D_Chu33> QueryFC3D_Chu33(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<FC3D_Chu33>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询FC3D_Chu33本期是否生成
        /// </summary>
        public int QueryFC3D_Chu33IssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<FC3D_Chu33>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
