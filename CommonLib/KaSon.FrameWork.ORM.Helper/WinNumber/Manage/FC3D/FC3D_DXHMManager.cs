﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class FC3D_DXHMManager : DBbase
    {
        /// <summary>
        /// 添加大乐透基本走势数据
        /// </summary>
        public void AddFC3D_DXHM(FC3D_DXHM entity)
        {
            LottertDataDB.GetDal<FC3D_DXHM>().Add(entity);
        }

        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public FC3D_DXHM QueryLastFC3D_DXHM()
        {
             
            return LottertDataDB.CreateQuery<FC3D_DXHM>().OrderByDescending(p => p.CreateTime).FirstOrDefault();
        }

        public List<FC3D_DXHM> QueryFC3D_DXHM(int index)
        {
             
            var query = from s in LottertDataDB.CreateQuery<FC3D_DXHM>()
                        orderby s.IssuseNumber descending
                        select s;
            return query.Take(index).ToList();
        }

        /// <summary>
        /// 查询FC3D_DXHM本期是否生成
        /// </summary>
        public int QueryFC3D_DXHMIssuseNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<FC3D_DXHM>().Where(p => p.IssuseNumber == issuseNumber).Count();
        }
    }
}
