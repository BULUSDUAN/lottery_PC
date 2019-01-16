using EntityModel;
using EntityModel.CoreModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
    public class A20150919Manager:DBbase
    {
        public void AddActivityConfig(E_Activity_Config entity)
        {
            DB.GetDal<E_Activity_Config>().Add(entity);
        }

        public void AddA20150919_注册送红包(E_A20150919_注册送红包 entity)
        {
            DB.GetDal<E_A20150919_注册送红包>().Add(entity);
        }

        public void AddA20150919_已绑定身份和手机的用户登录送红包(E_A20150919_已绑定身份和手机的用户登录送红包 entity)
        {
            DB.GetDal<E_A20150919_已绑定身份和手机的用户登录送红包>().Add(entity);
        }

        public void AddA20150919_注册绑定送红包(E_A20150919_注册绑定送红包 entity)
        {
            DB.GetDal<E_A20150919_注册绑定送红包>().Add(entity);
        }

        public void AddA20150919_充值送红包配置(E_A20150919_充值送红包配置 entity)
        {
            DB.GetDal<E_A20150919_充值送红包配置>().Add(entity);
        }

        public void AddA20150919_充值送红包记录(E_A20150919_充值送红包记录 entity)
        {
            DB.GetDal<E_A20150919_充值送红包记录>().Add(entity);
        }

        public void AddA20150919_加奖配置(E_A20150919_加奖配置 entity)
        {
            DB.GetDal<E_A20150919_加奖配置>().Add(entity);
        }

        public void AddA20150919_加奖赠送记录(E_A20150919_加奖赠送记录 entity)
        {
            DB.GetDal<E_A20150919_加奖赠送记录>().Add(entity);
        }

        public void AddA20150919_红包使用配置(E_A20150919_红包使用配置 entity)
        {
            DB.GetDal<E_A20150919_红包使用配置>().Add(entity);
        }

        public void AddA20150919_列表用户不加奖(E_A20150919_列表用户不加奖 entity)
        {
            DB.GetDal<E_A20150919_列表用户不加奖>().Add(entity);
        }

        public void UpdateActivityConfig(E_Activity_Config entity)
        {
            DB.GetDal<E_Activity_Config>().Update(entity);
        }

        public void UpdateA20150919_注册绑定送红包(E_A20150919_注册绑定送红包 entity)
        {
            var num= DB.GetDal<E_A20150919_注册绑定送红包>().Update(entity);
        }

        public void UpdateA20150919_充值送红包配置(E_A20150919_充值送红包配置 entity)
        {
            DB.GetDal<E_A20150919_充值送红包配置>().Update(entity);
        }

        public void UpdateA20150919_加奖配置(E_A20150919_加奖配置 entity)
        {
            DB.GetDal<E_A20150919_加奖配置>().Update(entity);
        }

        public void DeleteA20150919_充值送红包配置(E_A20150919_充值送红包配置 entity)
        {
            DB.GetDal<E_A20150919_充值送红包配置>().Delete(entity);
        }

        public void DeleteA20150919_加奖配置(E_A20150919_加奖配置 entity)
        {
            DB.GetDal<E_A20150919_加奖配置>().Delete(entity);
        }

        public void DeleteA20150919_红包使用配置(E_A20150919_红包使用配置 entity)
        {
            DB.GetDal<E_A20150919_红包使用配置>().Delete(entity);
        }

        public void DeleteA20150919_列表用户不加奖(E_A20150919_列表用户不加奖 entity)
        {
            DB.GetDal<E_A20150919_列表用户不加奖>().Delete(entity);
        }

        public E_A20150919_注册绑定送红包 QueryByUserId(string userId)
        {
          
            return DB.CreateQuery<E_A20150919_注册绑定送红包>().Where(p => p.UserId == userId).FirstOrDefault();
        }

        public E_A20150919_已绑定身份和手机的用户登录送红包 QueryA20150919_已绑定身份和手机的用户登录送红包(string userId, string date)
        {
           
            return DB.CreateQuery<E_A20150919_已绑定身份和手机的用户登录送红包>().Where(p => p.UserId == userId && p.LoginDate == date).FirstOrDefault();
        }

        public E_Activity_Config QueryActivityConfig(string key)
        {
          
            return DB.CreateQuery<E_Activity_Config>().Where(p => p.ConfigKey == key).FirstOrDefault();
        }

        public List<E_Activity_Config> QueryActivityConfig()
        {
           
            return DB.CreateQuery<E_Activity_Config>().ToList();
        }

        public List<E_A20150919_充值送红包配置> QueryA20150919_充值送红包配置()
        {
           
            return DB.CreateQuery<E_A20150919_充值送红包配置>().OrderByDescending(p => p.FillMoney).ToList();
        }
        public E_A20150919_充值送红包配置 QueryA20150919_充值送红包配置(int id)
        {
          
            return DB.CreateQuery<E_A20150919_充值送红包配置>().Where(p => p.Id == id).FirstOrDefault();
        }

        public List<FillMoneyGiveRedBagConfigInfo> QueryFillMoneyGiveRedBagConfigList()
        {
           
            var query = from c in DB.CreateQuery<E_A20150919_充值送红包配置>()
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
          
            var query = from p in DB.CreateQuery<E_A20150919_充值送红包记录>()
                        where p.UserId == userId && p.GiveMonth == month
                        select p;
            var list = query.ToList();
            return list.Count > 0 ? list.Sum(p => p.GiveMoney) : 0M;
        }

        public decimal QueryTotalFillMoneyGiveRedBag(string userId, string month, int paytype)
        {
         
            var query = from p in DB.CreateQuery<E_A20150919_充值送红包记录>()
                        where p.UserId == userId && p.GiveMonth == month && p.PayType == paytype
                        select p;
            var list = query.ToList();
            return list.Count > 0 ? list.Sum(p => p.GiveMoney) : 0M;
        }

        public List<AddBonusMoneyConfigInfo> QueryAddBonusMoneyConfig()
        {
          
            var query = from c in DB.CreateQuery<E_A20150919_加奖配置>()
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

        public List<E_A20150919_加奖配置> QueryA20150919_加奖配置(string gameCode)
        {
           
            var query = from c in DB.CreateQuery<E_A20150919_加奖配置>()
                        where c.GameCode == gameCode
                        orderby c.OrderIndex ascending
                        select c;
            return query.ToList();
        }

        public E_A20150919_加奖配置 QueryAddBonusMoneyConfig(int id)
        {
          
            return DB.CreateQuery<E_A20150919_加奖配置>().Where(p => p.Id == id).FirstOrDefault();
        }

        public E_A20150919_加奖赠送记录 QueryA20150919_加奖赠送记录(string orderId)
        {
          
            return DB.CreateQuery<E_A20150919_加奖赠送记录>().Where(p => p.OrderId == orderId).FirstOrDefault();
        }

        public List<E_A20150919_红包使用配置> QueryRedBagUseConfig()
        {
           
            return DB.CreateQuery<E_A20150919_红包使用配置>().ToList();
        }

        public E_A20150919_红包使用配置 QueryRedBagUseConfig(int id)
        {
           
            return DB.CreateQuery<E_A20150919_红包使用配置>().Where(p => p.Id == id).FirstOrDefault();
        }

        public E_A20150919_红包使用配置 QueryRedBagUseConfig(string gameCode)
        {
           
            return DB.CreateQuery<E_A20150919_红包使用配置>().Where(p => p.GameCode == gameCode).FirstOrDefault();
        }

        public E_A20150919_列表用户不加奖 QueryUserGameCodeNotAddMoney(int id)
        {
          
            return DB.CreateQuery<E_A20150919_列表用户不加奖>().Where(p => p.Id == id).FirstOrDefault();
        }

        public E_A20150919_列表用户不加奖 QueryA20150919_列表用户不加奖(string userId, string gameCode, string gameType, string playType)
        {
            
            return DB.CreateQuery<E_A20150919_列表用户不加奖>().Where(p => p.GameCode == gameCode && p.UserId == userId && p.GameType == gameType && p.PlayType == playType).FirstOrDefault();
        }

        public int Query不加奖用户列表(List<string> userList, string gameCode, string gameType, string playType)
        {
          
            var query = from p in DB.CreateQuery<E_A20150919_列表用户不加奖>()
                        where p.GameCode == gameCode && p.GameType == gameType && p.PlayType == playType
                        && userList.Contains(p.UserId)
                        select p;
            return query.Count();
        }

        public List<UserGameCodeNotAddMoneyInfo> QueryUserGameCodeNotAddMoneyList(string userId)
        {
           
            var query = (from c in DB.CreateQuery<E_A20150919_列表用户不加奖>()
                        join u in DB.CreateQuery<C_User_Register>() on c.UserId equals u.UserId
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
                        }).ToList();
            return query;
        }

    }
}
