﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class DF6_1_DXZSManager : DBbase
    {
        public void AddDF6_1_DXZS(DF6_1_DXZS entity)
        {
            LottertDataDB.GetDal<DF6_1_DXZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public DF6_1_DXZS QueryLastDF6_1_DXZS()
        {
             
            return LottertDataDB.CreateQuery<DF6_1_DXZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<DF6_1_DXZS> QueryDF6_1_DXZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<DF6_1_DXZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询DF6_1_DXZS本期是否生成
        /// </summary>
        public int QueryDF6_1_DXZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<DF6_1_DXZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}