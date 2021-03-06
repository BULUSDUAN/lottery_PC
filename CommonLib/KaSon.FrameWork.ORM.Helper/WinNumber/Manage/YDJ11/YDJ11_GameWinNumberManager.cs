﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
    public class YDJ11_GameWinNumberManager : DBbase
    {
        public void AddYDJ11_GameWinNumber(YDJ11_GameWinNumber entity)
        {
            LottertDataDB.GetDal<YDJ11_GameWinNumber>().Add(entity);
        }

        public YDJ11_GameWinNumber QueryWinNumber(string issuseNumber)
        {
             
            return LottertDataDB.CreateQuery<YDJ11_GameWinNumber>().Where(p => p.IssuseNumber == issuseNumber).FirstOrDefault();
        }

        public List<YDJ11_GameWinNumber> QueryYDJ11_GameWinNumber(int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<YDJ11_GameWinNumber>()
                        orderby s.IssuseNumber descending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<YDJ11_GameWinNumber> QueryYDJ11_GameWinNumber(DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<YDJ11_GameWinNumber>()
                        where s.CreateTime >= startTime && s.CreateTime < endTime
                        orderby s.IssuseNumber ascending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
        public List<YDJ11_GameWinNumber> QueryYDJ11_GameWinNumberDesc(DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
             
            var query = from s in LottertDataDB.CreateQuery<YDJ11_GameWinNumber>()
                        where s.CreateTime >= startTime && s.CreateTime < endTime
                        orderby s.IssuseNumber descending
                        select s;
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
