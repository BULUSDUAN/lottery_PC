using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using Activity.Business;
using Activity.Domain.Entities;

namespace Activity.Domain.Managers
{
    public class A20120925Manager : GameBiz.Business.GameBizEntityManagement
    {
        public void AddA20120925_加奖(A20120925_加奖 entity)
        {
            this.Add<A20120925_加奖>(entity);
        }
        public void AddA20120925_认证送彩金(A20120925_认证送彩金 entity)
        {
            this.Add<A20120925_认证送彩金>(entity);
        }
        public void AddA20120925_奖金转入送1(A20120925_奖金转入送1 entity)
        {
            this.Add<A20120925_奖金转入送1>(entity);
        }
        public void AddA20120925_Vip充值赠送2(A20120925_Vip充值赠送2 entity)
        {
            this.Add<A20120925_Vip充值赠送2>(entity);
        }
        public void AddA20121009_CTZQ加奖(A20121009_CTZQ加奖 entity)
        {
            this.Add<A20121009_CTZQ加奖>(entity);
        }

        public void UpdateA20120925_认证送彩金(A20120925_认证送彩金 entity)
        {
            this.Update<A20120925_认证送彩金>(entity);
        }

        public A20120925_认证送彩金 QueryA20120925_认证送彩金(string userId)
        {
            Session.Clear();
            return this.Session.Query<A20120925_认证送彩金>().FirstOrDefault(p => p.UserId == userId);
        }

        public A20121009_CTZQ加奖 QueryA20121009_CTZQ加奖(string userId, string gameType, string issuseNumber)
        {
            Session.Clear();
            return this.Session.Query<A20121009_CTZQ加奖>().FirstOrDefault(p => p.UserId == userId && p.GameType == gameType && p.IssuseNumber == issuseNumber);
        }
    }
}
