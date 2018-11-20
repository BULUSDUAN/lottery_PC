using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business;
using Activity.Domain.Entities;
using NHibernate.Linq;
using Activity.Core;
using GameBiz.Domain.Entities;

namespace Activity.Domain.Managers
{
    public class A20140318Manager_充值最高送1000 : GameBizEntityManagement
    {
        public void Fill(A20140318_充值最高送1000 entity)
        {
            Add(entity);
        }

        public A20140318_充值最高送1000 CheckFillOrNot(string userId)
        {
            this.Session.Clear();
            return this.Session.Query<A20140318_充值最高送1000>().FirstOrDefault(p => p.UserId == userId);
        }

        public List<A20140318FillInfo.A20140318Return1000Info> QueryA20140318Return1000InfoCollection(int month, bool isComplete, int pageIndex, int pageSize, out int totalCount)
        {
            this.Session.Clear();
            var query = from a in this.Session.Query<A20140318_充值最高送1000>()
                        join u in this.Session.Query<UserRegister>() on a.UserId equals u.UserId
                        where a.NextMonth == month + 1 && a.IsGiveComplete == isComplete
                        select new A20140318FillInfo.A20140318Return1000Info
                        {
                            Id = a.Id,
                            FillMoney = a.FillMoney,
                            GiveComplete = a.IsGiveComplete,
                            GivedMoney = a.CurrentGiveMoney,
                            OrderId = a.OrderId,
                            NextMonth = a.NextMonth,
                            NextMonthGiveMoney = a.NextMonthGiveMoney,
                            UserId = u.UserId,
                            UserDisplayName = u.DisplayName,
                        };
            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public void UpdateA20140318_充值最高送1000(A20140318_充值最高送1000 entity)
        {
            Update(entity);
        }

        public List<A20140318_充值最高送1000> QueryA20140318_充值最高送1000List(long[] idArray)
        {
            this.Session.Clear();
            return this.Session.Query<A20140318_充值最高送1000>().Where(p => idArray.Contains(p.Id)).ToList();
        }
    }
}
