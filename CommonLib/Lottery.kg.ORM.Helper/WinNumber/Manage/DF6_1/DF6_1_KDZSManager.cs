﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class DF6_1_KDZSManager : DBbase
    {
        public void AddDF6_1_KDZS(DF6_1_KDZS entity)
        {
            LottertDataDB.GetDal<DF6_1_KDZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public DF6_1_KDZS QueryLastDF6_1_KDZS()
        {
             
            return LottertDataDB.CreateQuery<DF6_1_KDZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<DF6_1_KDZS> QueryDF6_1_KDZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<DF6_1_KDZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询DF6_1_KDZS本期是否生成
        /// </summary>
        public int QueryDF6_1_KDZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<DF6_1_KDZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
