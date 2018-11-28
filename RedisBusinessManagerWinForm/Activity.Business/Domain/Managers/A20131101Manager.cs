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
    public class A20131101Manager : GameBizEntityManagement
    {
        public void AddA20131101_新用户首充送钱(A20131101_新用户首充送钱 entity)
        {
            this.Add<A20131101_新用户首充送钱>(entity);
        }
        public void AddA20131101_用户返点(A20131101_用户返点 entity)
        {
            this.Add<A20131101_用户返点>(entity);
        }
        public void UpdateA20131101_新用户首充送钱(A20131101_新用户首充送钱 entity)
        {
            this.Update<A20131101_新用户首充送钱>(entity);
        }

        public A20131101_新用户首充送钱 QueryA20131101_新用户首充送钱(string userId)
        {
            this.Session.Clear();
            return this.Session.Query<A20131101_新用户首充送钱>().FirstOrDefault(p => p.UserId == userId);
        }

        public List<A20131101_新用户首充送钱> QueryA20131101List(int month)
        {
            this.Session.Clear();
            return this.Session.Query<A20131101_新用户首充送钱>().Where(p => p.NextMonth == month && p.NextMonthGiveMoney > 0 && p.IsGiveComplate == false).ToList();
        }
        public List<A20131101_新用户首充送钱> QueryA20131101List(long[] idArray)
        {
            this.Session.Clear();
            return this.Session.Query<A20131101_新用户首充送钱>().Where(p => idArray.Contains(p.Id)).ToList();
        }

        public List<A20131101Info> QueryA20131101InfoCollection(int month, bool isComplate, int pageIndex, int pageSize, out int totalCount)
        {
            this.Session.Clear();
            var query = from a in this.Session.Query<A20140318_充值最高送1000>()
                        join u in this.Session.Query<UserRegister>() on a.UserId equals u.UserId
                        where a.NextMonth == month && a.IsGiveComplete == isComplate
                        select new A20131101Info
                        {
                            Id = a.Id,
                            FillMoney = a.FillMoney,
                            GiveComplate = a.IsGiveComplete,
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

        public bool IsExecReturnPoint(int month)
        {
            this.Session.Clear();
            return this.Session.Query<A20131101_用户返点>().Where(p => p.Month == month).Count() != 0;
        }

        public List<ActivityMonthReturnPointInfo> QueryMonthReturnPoint(int month)
        {
            this.Session.Clear();
            var query = from a in this.Session.Query<A20131101_用户返点>()
                        join u in this.Session.Query<UserRegister>() on a.UserId equals u.UserId
                        where a.Month == month
                        select new ActivityMonthReturnPointInfo
                        {
                            CreateTime = a.CreateTime,
                            GiveMoney = a.GiveMoney,
                            Id = a.Id,
                            Month = a.Month,
                            TotalBetMoney = a.TotalMoney,
                            UserId = u.UserId,
                            UserDisplayName = u.DisplayName,
                        };
            return query.ToList();
        }

    }
}
