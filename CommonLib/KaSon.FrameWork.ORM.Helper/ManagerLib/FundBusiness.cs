using EntityModel.CoreModel;
using KaSon.FrameWork.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel;

namespace KaSon.FrameWork.ORM.Helper.UserHelper
{
   public class FundBusiness:DBbase
    {
        private string _gbKey = "Q56GtyNkop97H334TtyturfgErvvv98a";
        public void SetBalancePassword(string userId, string oldPassword, bool isSetPwd, string newPassword)
        {
            var balanceManager = new UserBalanceManager();
            var entity = balanceManager.QueryUserBalance(userId);
            if (entity.IsSetPwd)
            {
                oldPassword = Encipherment.MD5(string.Format("{0}{1}", oldPassword, _gbKey)).ToUpper();
                if (string.IsNullOrEmpty(oldPassword) || !oldPassword.Equals(entity.Password))
                {
                    throw new Exception("输入资金密码错误");
                }
            }
            entity.IsSetPwd = isSetPwd;
            entity.Password = Encipherment.MD5(string.Format("{0}{1}", newPassword, _gbKey)).ToUpper();
            balanceManager.UpdateUserBalance(entity);
        }
        //public C_User_Balance QueryUserBalance(string userId)
        //{
        //  //  Session.Clear();
        //    return this.DB.CreateQuery<C_User_Balance>().Where(p => p.UserId == userId).FirstOrDefault();
        //    //var hql = "FROM UserBalance WHERE UserId = ?";
        //    //var balance = Session.CreateQuery(hql)
        //    //    .SetString(0, userId)
        //    //    .SetCacheable(false)
        //    //    .SetCacheMode(NHibernate.CacheMode.Get)
        //    //    .UniqueResult<UserBalance>();

        //    //return balance;
        //}
        /// <summary>
        /// 查询用户账户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserBalanceInfo QueryUserBalance(string userId)
        {
            using (DB)
            {
                // var balance = this.QueryUserBalance(userId);

                var balance = this.DB.CreateQuery<C_User_Balance>().Where(p => p.UserId == userId).FirstOrDefault();
                if (balance == null)
                {
                    throw new ArgumentException("用户账户不存在");
                }
                return new UserBalanceInfo
                {
                    UserId = balance.UserId,
                    FillMoneyBalance = balance.FillMoneyBalance,
                    BonusBalance = balance.BonusBalance,
                    CommissionBalance = balance.CommissionBalance,
                    FreezeBalance = balance.FreezeBalance,
                    ExpertsBalance = balance.ExpertsBalance,
                    RedBagBalance = balance.RedBagBalance,
                    IsSetPwd = balance.IsSetPwd,
                    NeedPwdPlace = balance.NeedPwdPlace,
                    CurrentDouDou = balance.CurrentDouDou,
                    UserGrowth = balance.UserGrowth,
                    CPSBalance = balance.CPSBalance,
                    BalancePwd = balance.Password,
                };
            }
        }
        public void SetBalancePasswordNeedPlace(string userId, string password, string placeList)
        {
            var balanceManager = new UserBalanceManager();
            var entity = balanceManager.QueryUserBalance(userId);
            if (entity.IsSetPwd)
            {
                password = Encipherment.MD5(string.Format("{0}{1}", password, _gbKey)).ToUpper();
                if (string.IsNullOrEmpty(password) || !password.Equals(entity.Password))
                {
                    throw new Exception("输入资金密码错误");
                }
            }
            else
            {
                throw new Exception("必须先设置资金密码");
            }
            entity.NeedPwdPlace = placeList;
            balanceManager.UpdateUserBalance(entity);
        }
    }
}
