using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business;
using NHibernate.Linq;
using Activity.Domain.Entities;

namespace Activity.Domain.Managers
{
    public class A20130807Manager : GameBizEntityManagement
    {

        public void AddA20130807_充值送钱(A20130807_充值送钱 entity)
        {
            this.Add<A20130807_充值送钱>(entity);
        }

        public void UpdateA20130807_充值送钱(A20130807_充值送钱 entity)
        {
            this.Update<A20130807_充值送钱>(entity);
        }

        public A20130807_充值送钱 QueryA20130807_充值送钱(string userId)
        {
            Session.Clear();
            return this.Session.Query<A20130807_充值送钱>().FirstOrDefault(p => p.UserId == userId);
        }
    }
}
