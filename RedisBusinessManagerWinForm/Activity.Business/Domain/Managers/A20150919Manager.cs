using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Business;
using NHibernate.Linq;
using Activity.Core;
using GameBiz.Domain.Entities;

namespace Activity.Domain.Entities
{
    public class A20150919Manager : GameBizEntityManagement
    {
        public void AddActivityConfig(ActivityConfig entity)
        {
            this.Add(entity);
        }

        public void AddA20150919_注册送红包(A20150919_注册送红包 entity)
        {
            this.Add(entity);
        }

        public void AddA20150919_已绑定身份和手机的用户登录送红包(A20150919_已绑定身份和手机的用户登录送红包 entity)
        {
            this.Add(entity);
        }

        public void AddA20150919_注册绑定送红包(A20150919_注册绑定送红包 entity)
        {
            this.Add(entity);
        }

        public void AddA20150919_充值送红包配置(A20150919_充值送红包配置 entity)
        {
            this.Add(entity);
        }

        public void AddA20150919_充值送红包记录(A20150919_充值送红包记录 entity)
        {
            this.Add(entity);
        }

        public void AddA20150919_加奖配置(A20150919_加奖配置 entity)
        {
            this.Add(entity);
        }

        public void AddA20150919_加奖赠送记录(A20150919_加奖赠送记录 entity)
        {
            this.Add(entity);
        }

        public void AddA20150919_红包使用配置(A20150919_红包使用配置 entity)
        {
            this.Add(entity);
        }

        public void AddA20150919_列表用户不加奖(A20150919_列表用户不加奖 entity)
        {
            this.Add(entity);
        }

        public void UpdateActivityConfig(ActivityConfig entity)
        {
            this.Update(entity);
        }

        public void UpdateA20150919_注册绑定送红包(A20150919_注册绑定送红包 entity)
        {
            this.Update(entity);
        }

        public void UpdateA20150919_充值送红包配置(A20150919_充值送红包配置 entity)
        {
            this.Update(entity);
        }

        public void UpdateA20150919_加奖配置(A20150919_加奖配置 entity)
        {
            this.Update(entity);
        }

        public void DeleteA20150919_充值送红包配置(A20150919_充值送红包配置 entity)
        {
            this.Delete(entity);
        }

        public void DeleteA20150919_加奖配置(A20150919_加奖配置 entity)
        {
            this.Delete(entity);
        }

        public void DeleteA20150919_红包使用配置(A20150919_红包使用配置 entity)
        {
            this.Delete(entity);
        }

        public void DeleteA20150919_列表用户不加奖(A20150919_列表用户不加奖 entity)
        {
            this.Delete(entity);
        }

        public A20150919_注册绑定送红包 QueryByUserId(string userId)
        {
            this.Session.Clear();
            return this.Session.Query<A20150919_注册绑定送红包>().FirstOrDefault(p => p.UserId == userId);
        }

        public A20150919_已绑定身份和手机的用户登录送红包 QueryA20150919_已绑定身份和手机的用户登录送红包(string userId, string date)
        {
            this.Session.Clear();
            return this.Session.Query<A20150919_已绑定身份和手机的用户登录送红包>().FirstOrDefault(p => p.UserId == userId && p.LoginDate == date);
        }

        public ActivityConfig QueryActivityConfig(string key)
        {
            this.Session.Clear();
            return this.Session.Query<ActivityConfig>().FirstOrDefault(p => p.ConfigKey == key);
        }

        public List<ActivityConfig> QueryActivityConfig()
        {
            this.Session.Clear();
            return this.Session.Query<ActivityConfig>().ToList();
        }

        public List<A20150919_充值送红包配置> QueryA20150919_充值送红包配置()
        {
            this.Session.Clear();
            return this.Session.Query<A20150919_充值送红包配置>().OrderByDescending(p => p.FillMoney).ToList();
        }
        public A20150919_充值送红包配置 QueryA20150919_充值送红包配置(int id)
        {
            this.Session.Clear();
            return this.Session.Query<A20150919_充值送红包配置>().FirstOrDefault(p => p.Id == id);
        }

        public List<FillMoneyGiveRedBagConfigInfo> QueryFillMoneyGiveRedBagConfigList()
        {
            this.Session.Clear();
            var query = from c in this.Session.Query<A20150919_充值送红包配置>()
                        orderby c.FillMoney ascending
                        select new FillMoneyGiveRedBagConfigInfo
                        {
                            CreateTime = c.CreateTime,
                            FillMoney = c.FillMoney,
                            GiveMoney = c.GiveMoney,
                            Id = c.Id
                        };
            return query.ToList();
        }

        public decimal QueryTotalFillMoneyGiveRedBag(string userId, string month)
        {
            this.Session.Clear();
            var query = from p in this.Session.Query<A20150919_充值送红包记录>()
                        where p.UserId == userId && p.GiveMonth == month
                        select p;
            var list = query.ToList();
            return list.Count > 0 ? list.Sum(p => p.GiveMoney) : 0M;
        }

        public decimal QueryTotalFillMoneyGiveRedBag(string userId, string month, int paytype)
        {
            this.Session.Clear();
            var query = from p in this.Session.Query<A20150919_充值送红包记录>()
                        where p.UserId == userId && p.GiveMonth == month && p.PayType == paytype
                        select p;
            var list = query.ToList();
            return list.Count > 0 ? list.Sum(p => p.GiveMoney) : 0M;
        }

        public List<AddBonusMoneyConfigInfo> QueryAddBonusMoneyConfig()
        {
            this.Session.Clear();
            var query = from c in this.Session.Query<A20150919_加奖配置>()
                        select new AddBonusMoneyConfigInfo
                        {
                            AddBonusMoneyPercent = c.AddBonusMoneyPercent,
                            CreateTime = c.CreateTime,
                            GameCode = c.GameCode,
                            GameType = c.GameType,
                            Id = c.Id,
                            MaxAddBonusMoney = c.MaxAddBonusMoney,
                            PlayType = c.PlayType,
                            OrderIndex = c.OrderIndex,
                            AddMoneyWay = c.AddMoneyWay
                        };
            return query.ToList();
        }

        public List<A20150919_加奖配置> QueryA20150919_加奖配置(string gameCode)
        {
            this.Session.Clear();
            var query = from c in this.Session.Query<A20150919_加奖配置>()
                        where c.GameCode == gameCode
                        orderby c.OrderIndex ascending
                        select c;
            return query.ToList();
        }

        public A20150919_加奖配置 QueryAddBonusMoneyConfig(int id)
        {
            this.Session.Clear();
            return this.Session.Query<A20150919_加奖配置>().FirstOrDefault(p => p.Id == id);
        }

        public A20150919_加奖赠送记录 QueryA20150919_加奖赠送记录(string orderId)
        {
            this.Session.Clear();
            return this.Session.Query<A20150919_加奖赠送记录>().FirstOrDefault(p => p.OrderId == orderId);
        }

        public List<A20150919_红包使用配置> QueryRedBagUseConfig()
        {
            this.Session.Clear();
            return this.Session.Query<A20150919_红包使用配置>().ToList();
        }

        public A20150919_红包使用配置 QueryRedBagUseConfig(int id)
        {
            this.Session.Clear();
            return this.Session.Query<A20150919_红包使用配置>().FirstOrDefault(p => p.Id == id);
        }

        public A20150919_红包使用配置 QueryRedBagUseConfig(string gameCode)
        {
            this.Session.Clear();
            return this.Session.Query<A20150919_红包使用配置>().FirstOrDefault(p => p.GameCode == gameCode);
        }

        public A20150919_列表用户不加奖 QueryUserGameCodeNotAddMoney(int id)
        {
            this.Session.Clear();
            return this.Session.Query<A20150919_列表用户不加奖>().FirstOrDefault(p => p.Id == id);
        }

        public A20150919_列表用户不加奖 QueryA20150919_列表用户不加奖(string userId, string gameCode, string gameType, string playType)
        {
            this.Session.Clear();
            return this.Session.Query<A20150919_列表用户不加奖>().FirstOrDefault(p => p.GameCode == gameCode && p.UserId == userId && p.GameType == gameType && p.PlayType == playType);
        }

        public int Query不加奖用户列表(List<string> userList, string gameCode, string gameType, string playType)
        {
            this.Session.Clear();
            var query = from p in this.Session.Query<A20150919_列表用户不加奖>()
                        where p.GameCode == gameCode && p.GameType == gameType && p.PlayType == playType
                        && userList.Contains(p.UserId)
                        select p;
            return query.Count();
        }

        public List<UserGameCodeNotAddMoneyInfo> QueryUserGameCodeNotAddMoneyList(string userId)
        {
            this.Session.Clear();
            var query = from c in this.Session.Query<A20150919_列表用户不加奖>()
                        join u in this.Session.Query<UserRegister>() on c.UserId equals u.UserId
                        where (string.IsNullOrEmpty(userId) || c.UserId == userId)
                        select new UserGameCodeNotAddMoneyInfo
                        {
                            CreateTime = c.CreateTime,
                            GameCode = c.GameCode,
                            Id = c.Id,
                            UserId = c.UserId,
                            UserName = u.DisplayName,
                            GameType = c.GameType,
                            PlayType = c.PlayType,
                        };
            return query.ToList();
        }

    }
}
