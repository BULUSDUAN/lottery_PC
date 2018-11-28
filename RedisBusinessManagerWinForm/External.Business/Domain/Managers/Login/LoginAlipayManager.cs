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
    public class LoginAlipayManager : GameBiz.Business.GameBizEntityManagement
    {
        public LoginAlipay QueryByOpenId(string openId)
        {
            this.Session.Clear();
            return this.Session.Query<LoginAlipay>().FirstOrDefault(p => p.OpenId == openId);
        }
        public LoginAlipay QueryByUserId(string userId)
        {
            this.Session.Clear();
            return this.Session.Query<LoginAlipay>().FirstOrDefault(p => p.UserId == userId);
        }
        public LoginAlipay QueryAlipayByUserIdOpenId(string userId, string openId)
        {
            this.Session.Clear();
            return this.Session.Query<LoginAlipay>().FirstOrDefault(p => p.OpenId == openId && p.UserId == userId);
        }
        public void Register(LoginAlipay login)
        {
            Add<LoginAlipay>(login);
        }

    }
}
