using EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Lottery.Kg.ORM.Helper.UserHelper
{
     public class UserBalanceManager:DBbase
    {
        /// <summary>
        /// 查询用户余额
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public C_User_Balance QueryUserBalance(string userId)
        {

            return DB.CreateQuery<C_User_Balance>().FirstOrDefault(p => p.UserId == userId);

        }

        public void UpdateUserBalance(C_User_Balance entity)
        {
            var maxTime = 3;
            var currentTime = 0;
            while (currentTime < maxTime)
            {
                try
                {
                    DB.GetDal<C_User_Balance>().Update(entity);
                    break;
                }
               
                catch (Exception ex)
                {
                    //var writer = Common.Log.LogWriterGetter.GetLogWriter();
                    //writer.Write("ERROR_UserBalanceManager", "_UpdateUserBalance", Common.Log.LogType.Error, "更新用户资金问题出错", ex.ToString());
                    //throw new Exception("资金处理错误，请重试", ex);
                }

                currentTime++;
                Thread.Sleep(1000);
            }

        }
    }
}
