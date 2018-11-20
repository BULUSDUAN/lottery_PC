using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business;
using Activity.Domain.Entities;
using NHibernate.Linq;
using Activity.Core;

namespace Activity.Business.Domain.Managers
{
    public class A20140318Manager : GameBizEntityManagement
    {
        public void AddA20140318_现场送彩票(A20140318_现场送彩票 entity)
        {
            this.Add(entity);
        }

        public void UpdateA20140318_现场送彩票(A20140318_现场送彩票 entity)
        {
            this.Update(entity);
        }

        public A20140318_现场送彩票 QueryA20140318_现场送彩票(string schemeId)
        {
            this.Session.Clear();
            return this.Session.Query<A20140318_现场送彩票>().FirstOrDefault(p => p.SchemeId == schemeId);
        }

        public List<A20140318Info> QueryA20140318InfoList(int pageIndex, int pageSize, out int totalCount)
        {
            this.Session.Clear();
            var query = from a in this.Session.Query<A20140318_现场送彩票>()
                        orderby a.CreateTime ascending
                        select new A20140318Info
                        {
                            BetCount = a.BetCount,
                            BetMoney = a.BetMoney,
                            BonusMoney = a.BonusMoney,
                            CreateTime = a.CreateTime,
                            SchemeId = a.SchemeId,
                            UserId = a.UserId,
                            MobileNumber = a.MobileNumber,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
