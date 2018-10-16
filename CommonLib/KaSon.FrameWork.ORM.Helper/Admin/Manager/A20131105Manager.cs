using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business;
using NHibernate.Linq;
using Activity.Domain.Entities;
using Activity.Core;
using GameBiz.Domain.Entities;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.Admin
{
    public class A20131105Manager : DBbase
    {
        public void AddA20131105_优惠券(A20131105_优惠券 entity)
        {
            this.Add<A20131105_优惠券>(entity);
        }

        public void UpdateA20131105_优惠券(A20131105_优惠券 entity)
        {
            this.Update<A20131105_优惠券>(entity);
        }

        public A20131105_优惠券 QueryA20131105_优惠券(string number)
        {
            return DB.CreateQuery<A20131105_优惠券>().FirstOrDefault(p => p.Number == number);
        }

        public A20131105_优惠券 QueryA20131105_优惠券(string summary, string userId)
        {
            this.Session.Clear();
            return this.Session.Query<A20131105_优惠券>().FirstOrDefault(p => p.Summary == summary && p.BelongUserId == userId);
        }

        public List<A20131105CouponsInfo> QueryCouponsList(string summary, bool? canUsable, string belongUserId, int pageIndex, int pageSize, out int totalCount)
        {
            this.Session.Clear();
            var query = from c in this.Session.Query<A20131105_优惠券>()
                        where (string.IsNullOrEmpty(summary) || c.Summary.Contains(summary))
                        && (!canUsable.HasValue || c.CanUsable == canUsable.Value)
                        && (string.IsNullOrEmpty(belongUserId) || c.BelongUserId == belongUserId)
                        select new A20131105CouponsInfo
                        {
                            BelongUserId = c.BelongUserId,
                            CanUsable = c.CanUsable,
                            CreateTime = c.CreateTime,
                            Id = c.Id,
                            Money = c.Money,
                            Number = c.Number,
                            Summary = c.Summary,
                        };

            totalCount = query.Count();
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
