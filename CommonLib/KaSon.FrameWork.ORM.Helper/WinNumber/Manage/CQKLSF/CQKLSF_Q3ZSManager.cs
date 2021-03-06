﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;
namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class CQKLSF_Q3ZSManager : DBbase
    {
        public void AddCQKLSF_Q3ZS(CQKLSF_Q3ZS entity)
        {
            LottertDataDB.GetDal<CQKLSF_Q3ZS>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public CQKLSF_Q3ZS QueryLastCQKLSF_Q3ZS()
        {
            return LottertDataDB.CreateQuery<CQKLSF_Q3ZS>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<CQKLSF_Q3ZS> QueryCQKLSF_Q3ZS(int index)
        {
            var query = from s in LottertDataDB.CreateQuery<CQKLSF_Q3ZS>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询CQKLSF_Q3ZS本期是否生成
        /// </summary>
        public int QueryCQKLSF_Q3ZSIssuseNumber(string issuseNumber)
        {
            return LottertDataDB.CreateQuery<CQKLSF_Q3ZS>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
