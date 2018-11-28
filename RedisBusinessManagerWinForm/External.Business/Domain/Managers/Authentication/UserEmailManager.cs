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
    public class BettingPointManager : GameBiz.Business.GameBizEntityManagement
    {
        public UserEmail GetUserEmail(string userId)
        {
            return GetByKey<UserEmail>(userId);
        }
        public UserEmail GetOtherUserEmail(string email, string userId)
        {
            Session.Clear();
            var list = Session.CreateCriteria<UserEmail>()
                .Add(Restrictions.Eq("Email", email))
                .Add(Restrictions.Not(Restrictions.Eq("UserId", userId)))
                .List<UserEmail>();
            if (list.Count == 0)
            {
                return null;
            }
            return list[0];
        }
        public void AddUserEmail(UserEmail realName)
        {
            realName.CreateTime = DateTime.Now;
            realName.UpdateTime = DateTime.Now;
            Add<UserEmail>(realName);
        }
        public void UpdateUserEmail(UserEmail realName)
        {
            realName.UpdateTime = DateTime.Now;
            Update<UserEmail>(realName);
        }
        public SystemUser LoadUser(string userId)
        {
            return LoadByKey<SystemUser>(userId);
        }
        public UserEmail GetEmailInfoByEmail(string email)
        {
            var query = Session.Query<UserEmail>().Where(s => s.Email == email);
            if (query != null && query.Count() > 0)
            {
                var resutl = query.FirstOrDefault(s => s.IsSettedEmail == true);
                if (resutl != null)
                    return resutl;
                else
                {
                    resutl = query.FirstOrDefault(s => s.IsSettedEmail == false);
                    if (resutl != null)
                        return resutl;
                }
            }
            return null;
        }
    }
}
