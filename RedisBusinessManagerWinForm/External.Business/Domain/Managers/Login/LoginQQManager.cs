using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Cryptography;
using NHibernate.Criterion;
using GameBiz.Auth.Domain.Entities;
using Common.Business;
using GameBiz.Domain.Entities;
using System.Collections;
using External.Domain.Entities.Login;
using NHibernate.Linq;

namespace External.Domain.Managers.Login
{
    public class LoginQQManager : GameBiz.Business.GameBizEntityManagement
    {
        public LoginQQ GetQQ(string loginName)
        {
            Session.Clear();
            return Session.CreateCriteria<LoginQQ>()
                .Add(Restrictions.Eq("LoginName", loginName))
                .UniqueResult<LoginQQ>();
        }
        public LoginQQ GetQQByOpenId(string openId)
        {
            Session.Clear();
            return Session.CreateCriteria<LoginQQ>()
                .Add(Restrictions.Eq("OpenId", openId))
                .UniqueResult<LoginQQ>();
        }
        public LoginQQ GetLoginByName(string loginName)
        {
            Session.Clear();
            return Session.CreateCriteria<LoginQQ>()
                .Add(Restrictions.Eq("LoginName", loginName))
                .UniqueResult<LoginQQ>();
        }
        public LoginQQ GetLoginByUserId(string userId)
        {
            return GetByKey<LoginQQ>(userId);
        }
        public LoginQQ QueryLoginByUserIdOpenId(string userId, string openId)
        {
            Session.Clear();
            return this.Session.Query<LoginQQ>().FirstOrDefault(p => p.UserId == userId && p.OpenId == openId);
        }

        public void UpdateLogin(LoginQQ login)
        {
            Update<LoginQQ>(login);
        }
        public void Register(LoginQQ login)
        {
            login.CreateTime = DateTime.Now;
            Add<LoginQQ>(login);
        }
        public SystemUser LoadUser(string userId)
        {
            return LoadByKey<SystemUser>(userId);
        }
        public UserRegister LoadRegister(string userId)
        {
            return LoadByKey<UserRegister>(userId);
        }
        public int GetTodayRegisterCount(DateTime date, string ip)
        {
            Session.Clear();
            var hql = "SELECT COUNT(*) FROM LoginQQ WHERE CreateTime >= :StartDate AND CreateTime < :EndTime AND Register.RegisterIp = :Ip";
            var result = Session.CreateQuery(hql)
                .SetDateTime("StartDate", date.Date)
                .SetDateTime("EndTime", date.Date.AddDays(1))
                .SetString("Ip", ip)
                .UniqueResult<long>();
            return (int)result;
        }

        /// <summary>
        /// 查询用户总数
        /// </summary>
        public int QueryUserRegisterCount()
        {
            this.Session.Clear();
            return this.Session.Query<UserRegister>().Count();
        }


    }
}
