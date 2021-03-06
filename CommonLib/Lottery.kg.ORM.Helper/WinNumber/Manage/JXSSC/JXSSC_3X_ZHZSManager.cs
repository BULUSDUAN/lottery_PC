﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class JXSSC_3X_ZHZSManager : DBbase
    {
        public void AddJXSSC_3X_ZHZS(JXSSC_3X_ZHZS entity)
        {
            LottertDataDB.GetDal<JXSSC_3X_ZHZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public JXSSC_3X_ZHZS QueryLastJXSSC_3X_ZHZS()
        {
             
            return LottertDataDB.CreateQuery<JXSSC_3X_ZHZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<JXSSC_3X_ZHZS> QueryJXSSC_3X_ZHZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<JXSSC_3X_ZHZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询JXSSC_3X_ZHZS本期是否生成
        /// </summary>
        public int QueryJXSSC_3X_ZHZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JXSSC_3X_ZHZS>().Count(p => p.IssuseNumber == issuseNumber);
        }
    }
}
