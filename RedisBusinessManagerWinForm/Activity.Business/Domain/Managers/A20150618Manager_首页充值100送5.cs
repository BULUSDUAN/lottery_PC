using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business;
using Activity.Domain.Entities;
using NHibernate.Linq;
using Activity.Core;
using GameBiz.Domain.Entities;

namespace Activity.Business.Domain.Managers
{
    public class A20150618Manager_首页充值100送5 : GameBizEntityManagement
    {
        public void AddA20150618_首页充值100送5(A20150618_首页充值100送5 entity)
        {
            this.Add<A20150618_首页充值100送5>(entity);
        }

        public void UpdateA20150618_首页充值100送5(A20150618_首页充值100送5 entity)
        {
            this.Update<A20150618_首页充值100送5>(entity);
        }

        public A20150618_首页充值100送5 QueryA20150618_首页充值100送5(string userId)
        {
            Session.Clear();
            return this.Session.Query<A20150618_首页充值100送5>().FirstOrDefault(p => p.UserId == userId);
        }
        /// <summary>
        /// 根据ID查询充值记录
        /// </summary>
        /// <returns></returns>
        public List<A2015618Info> ListQueryA20150618_首页充值100送5(string userId)
        {
            Session.Clear();
            var query = from t in this.Session.Query<A20150618_首页充值100送5>()
                        where (userId == string.Empty || t.UserId == userId)
                        select new A2015618Info
                        {
                            UserId = t.UserId,
                            FillMoney = t.FillMoney,
                            OrderId = t.OrderId,
                        };
            return query.ToList();
        }
        public int QueryActiviyCount(string idCarNumber)
        {
            Session.Clear();
            return Session.Query<A20150618_首页充值100送5>().Where(s => s.IdCardNumber == idCarNumber).Count();
        }
    }
}
