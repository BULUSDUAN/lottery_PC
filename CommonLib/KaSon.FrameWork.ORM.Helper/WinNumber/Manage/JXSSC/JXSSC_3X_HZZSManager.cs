﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class JXSSC_3X_HZZSManager : DBbase
    {
        public void AddJXSSC_3X_HZZS(JXSSC_3X_HZZS entity)
        {
            LottertDataDB.GetDal<JXSSC_3X_HZZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public JXSSC_3X_HZZS QueryLastJXSSC_3X_HZZS()
        {
             
            return LottertDataDB.CreateQuery<JXSSC_3X_HZZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<JXSSC_3X_HZZS> QueryJXSSC_3X_HZZS(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<JXSSC_3X_HZZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询JXSSC_3X_HZZS本期是否生成
        /// </summary>
        public int QueryJXSSC_3X_HZZSIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<JXSSC_3X_HZZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
