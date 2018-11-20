using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Activity.Domain.Entities;
using NHibernate.Linq;
using GameBiz.Business;
using GameBiz.Core;

namespace Activity.Business.Domain.Managers
{
    public class A20141203Manager_GiveLotty : GameBizEntityManagement
    {
        public void AddGiveLotty(A20141203GiveLottery entity)
        {
            this.Add<A20141203GiveLottery>(entity);
        }
        public A20141203GiveLottery GetA20141203GiveLottery(string userId,SchemeSource schemeSource)
        {
            return Session.Query<A20141203GiveLottery>().FirstOrDefault(s => s.UserId == userId && s.ActivityType == schemeSource);
        }
        public bool CheckIsActivityByCardNumber(string idCardNumber)
        {
            return Session.Query<A20141203GiveLottery>().Where(s => s.IdCardNumber == idCardNumber).Count() > 0 ? true : false;
        }
    }
}
