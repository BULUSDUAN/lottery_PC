using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Auth.Domain.Entities;
using Common.Business;
using NHibernate.Criterion;
using External.Domain.Entities.Authentication;
using NHibernate.Linq;

namespace External.Domain.Managers.Authentication
{
    public class UserQQManager : GameBiz.Business.GameBizEntityManagement
    {
        public UserQQ GetUserQQ(string userId)
        {
            return GetByKey<UserQQ>(userId);
        }
        public UserQQ GetOtherUserQQ(string qq, string userId)
        {
            Session.Clear();
            var list = Session.CreateCriteria<UserQQ>()
                .Add(Restrictions.Eq("QQ", qq))
                .Add(Restrictions.Not(Restrictions.Eq("UserId", userId)))
                .List<UserQQ>();
            if (list.Count == 0)
            {
                return null;
            }
            return list[0];
        }
        public void AddUserQQ(UserQQ userQQ)
        {
            userQQ.CreateTime = DateTime.Now;
            userQQ.UpdateTime = DateTime.Now;
            Add<UserQQ>(userQQ);
        }
        public void UpdateUserQQ(UserQQ userQQ)
        {
            userQQ.UpdateTime = DateTime.Now;
            Update<UserQQ>(userQQ);
        }
        public void DeleteUserQQ(UserQQ userQQ)
        {
            userQQ.UpdateTime = DateTime.Now;
            Delete<UserQQ>(userQQ);
        }
        public SystemUser LoadUser(string userId)
        {
            return LoadByKey<SystemUser>(userId);
        }
    }
}
