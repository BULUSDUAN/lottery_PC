﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class YDJ11_Q2JBZSManager : DBbase
    {
        public void AddYDJ11_Q2JBZS(YDJ11_Q2JBZS entity)
        {
            LottertDataDB.GetDal<YDJ11_Q2JBZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public YDJ11_Q2JBZS QueryLastYDJ11_Q2JBZS()
        {
             
            return LottertDataDB.CreateQuery<YDJ11_Q2JBZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<YDJ11_Q2JBZS> QueryYDJ11_Q2JBZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<YDJ11_Q2JBZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询YDJ11_Q2JBZS本期是否生成
        /// </summary>
        public int QueryYDJ11_Q2JBZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<YDJ11_Q2JBZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
