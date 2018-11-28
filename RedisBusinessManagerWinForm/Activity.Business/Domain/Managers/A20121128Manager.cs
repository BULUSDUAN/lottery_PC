using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business;
using NHibernate.Linq;
using Activity.Domain.Entities;

namespace Activity.Managers
{
    public class A20121128Manager : GameBizEntityManagement
    {
        public void AddA20121128_首次中奖超过100送5元(A20121128_首次中奖超过100送5元 entity)
        {
            this.Add<A20121128_首次中奖超过100送5元>(entity);
        }

        public A20121128_首次中奖超过100送5元 QueryA20121128_首次中奖超过100送5元(string userId)
        {
            Session.Clear();
            return this.Session.Query<A20121128_首次中奖超过100送5元>().FirstOrDefault(p => p.UserId == userId);
        }
    }
}
