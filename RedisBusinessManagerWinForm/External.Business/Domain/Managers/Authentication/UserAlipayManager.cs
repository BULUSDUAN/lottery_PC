using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Auth.Domain.Entities;
using Common.Business;
using NHibernate.Criterion;
using External.Domain.Entities.Authentication;
using NHibernate.Linq;
using NHibernate.Linq;

namespace External.Domain.Managers.Authentication
{
    public class UserAlipayManager : GameBiz.Business.GameBizEntityManagement
    {
        public UserAlipay GetUserAlipay(string userId)
        {
            return GetByKey<UserAlipay>(userId);
        }
        public UserAlipay GetUserAlipayByAccount(string alipay)
        {
            Session.Clear();
            return Session.Query<UserAlipay>().FirstOrDefault(p => p.AlipayAccount == alipay);
        }


        public UserAlipay GetOtherUserAlipay(string alipay, string userId)
        {
            Session.Clear();
            var list = Session.CreateCriteria<UserAlipay>()
                .Add(Restrictions.Eq("AlipayAccount", alipay))
                .Add(Restrictions.Not(Restrictions.Eq("UserId", userId)))
                .List<UserAlipay>();
            if (list.Count == 0)
            {
                return null;
            }
            return list[0];
        }
        public void AddUserAlipay(UserAlipay entity)
        {
            entity.CreateTime = DateTime.Now;
            entity.UpdateTime = DateTime.Now;
            Add<UserAlipay>(entity);
        }
        public void UpdateUserAlipay(UserAlipay entity)
        {
            entity.UpdateTime = DateTime.Now;
            Update<UserAlipay>(entity);
        }
        public void DeleteUserAlipay(UserAlipay entity)
        {
            entity.UpdateTime = DateTime.Now;
            Delete<UserAlipay>(entity);
        }
        public SystemUser LoadUser(string userId)
        {
            return LoadByKey<SystemUser>(userId);
        }
    }
}
