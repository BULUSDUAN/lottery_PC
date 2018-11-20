using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business;
using NHibernate.Linq;
using Activity.Domain.Entities;

namespace Activity.Domain.Managers
{
    public class A20130808Manager : GameBizEntityManagement
    {

        public void AddA20130808_注册送3元(A20130808_注册送3元 entity)
        {
            this.Add<A20130808_注册送3元>(entity);
        }

        public int QueryA20130808_注册送3元(string userId)
        {
            Session.Clear();
            var count = this.Session.Query<A20130808_注册送3元>().Count(p => p.UserId == userId);
            return count;
        }
    }
}
