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
    public class UserMobileManager : GameBiz.Business.GameBizEntityManagement
    {
        public UserMobile GetUserMobile(string userId)
        {
            return GetByKey<UserMobile>(userId);
        }
        public UserMobile GetOtherUserMobile(string mobile, string userId)
        {
            Session.Clear();
            var list = Session.CreateCriteria<UserMobile>()
                .Add(Restrictions.Eq("Mobile", mobile))
                .Add(Restrictions.Not(Restrictions.Eq("UserId", userId)))
                .List<UserMobile>();
            if (list.Count == 0)
            {
                return null;
            }
            return list[0];

        }
        //首页活动注册认证手机
        public UserMobile GetOtherUserMobileIndex(string mobile)
        {
            Session.Clear();
            var list = Session.CreateCriteria<UserMobile>()
                .Add(Restrictions.Eq("Mobile", mobile))
                .List<UserMobile>();
            if (list.Count == 0)
            {
                return null;
            }
            return list[0];
        }
        public void AddUserMobile(UserMobile mobileEntity)
        {
            mobileEntity.CreateTime = DateTime.Now;
            mobileEntity.UpdateTime = DateTime.Now;
            Add<UserMobile>(mobileEntity);
        }
        public void DeleteUserMobile(UserMobile entity)
        {
            this.Delete<UserMobile>(entity);
        }
        public void UpdateUserMobile(UserMobile mobileEntity)
        {
            mobileEntity.UpdateTime = DateTime.Now;
            Update<UserMobile>(mobileEntity);
        }
        public SystemUser LoadUser(string userId)
        {
            return LoadByKey<SystemUser>(userId);
        }
        public bool ExistMobile(string mobile)
        {
            return Session.CreateCriteria<UserMobile>()
                .Add(Restrictions.Eq("Mobile", mobile))
                .List().Count.Equals(0);
        }
        public UserMobile GetMobileInfoByMobile(string mobile)
        {
            var query = Session.Query<UserMobile>().Where(s=>s.Mobile == mobile);
            if (query != null && query.Count() > 0)
            {
                var resutl = query.FirstOrDefault(s => s.IsSettedMobile == true);
                if (resutl != null)
                    return resutl;
                else
                {
                    resutl = query.FirstOrDefault(s => s.IsSettedMobile == false);
                    if (resutl != null)
                        return resutl;
                }
            }
            return null;
        }
    }
}
