﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class GD11X5_XTZSManager : DBbase
    {
        public void AddGD11X5_XTZS(GD11X5_XTZS entity)
        {
            LottertDataDB.GetDal<GD11X5_XTZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public GD11X5_XTZS QueryLastGD11X5_XTZS()
        {
             
            return LottertDataDB.CreateQuery<GD11X5_XTZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<GD11X5_XTZS> QueryGD11X5_XTZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<GD11X5_XTZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询GD11X5_XTZS本期是否生成
        /// </summary>
        public int QueryGD11X5_XTZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<GD11X5_XTZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
