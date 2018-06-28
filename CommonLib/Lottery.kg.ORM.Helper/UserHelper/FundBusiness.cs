using KaSon.FrameWork.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.Kg.ORM.Helper.UserHelper
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
