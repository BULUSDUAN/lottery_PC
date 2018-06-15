﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lottery.Kg.ORM.Helper.WinNumber.Model;

namespace Lottery.Kg.ORM.Helper.WinNumber.Manage
{
    public class LN11X5_JBZSManager : DBbase
    {
        public void AddLN11X5_JBZS(LN11X5_JBZS entity)
        {
            LottertDataDB.GetDal<LN11X5_JBZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public LN11X5_JBZS QueryLastLN11X5_JBZS()
        {
             
            return LottertDataDB.CreateQuery<LN11X5_JBZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<LN11X5_JBZS> QueryLN11X5_JBZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<LN11X5_JBZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询LN11X5_JBZS本期是否生成
        /// </summary>
        public int QueryLN11X5_JBZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<LN11X5_JBZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
