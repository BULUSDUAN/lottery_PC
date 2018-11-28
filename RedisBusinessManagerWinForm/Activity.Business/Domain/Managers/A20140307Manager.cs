using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using GameBiz.Business;
using Activity.Domain.Entities;

namespace Activity.Business.Domain.Managers
{
    public class A20140307Manager : GameBizEntityManagement
    {
        public void Update(A20140307_美家颂 entity)
        {
            this.Update<A20140307_美家颂>(entity);
        }

        public A20140307_美家颂 GetByUserId(string userId)
        {
            this.Session.Clear();
            return this.Session.Query<A20140307_美家颂>().FirstOrDefault(p => p.BelongUserId == userId);
        }

        public A20140307_美家颂 LoadOneNumber()
        {
            this.Session.Clear();
            var query = from n in this.Session.Query<A20140307_美家颂>()
                        where n.BelongUserId == ""
                        orderby n.CreateTime ascending
                        select n;
            return query.FirstOrDefault();
        }

    }
}
