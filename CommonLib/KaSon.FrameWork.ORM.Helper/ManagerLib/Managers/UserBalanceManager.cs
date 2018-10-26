using EntityModel;
using EntityModel.Enum;
using EntityModel.PayModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace KaSon.FrameWork.ORM.Helper
{
     public class UserBalanceManager:DBbase
    {
        private string GetOperFun(PayType p)
        {
            switch (p)
            {
                case PayType.Payin:
                    return "+=";
                case PayType.Payout:
                    return "-=";
            }
            throw new Exception("PayType类型不正确");
        }
        /// <summary>
        /// 支付到用户余额
        /// </summary>
        public void PayToUserBalance(string userId, params PayDetail[] array)
        {
            if (array.Length <= 0)
                return;

            var setList = new List<string>();
            foreach (var item in array)
            {
                switch (item.AccountType)
                {
                    case AccountType.Bonus:
                        setList.Add(string.Format(" [BonusBalance]{0}{1}", GetOperFun(item.PayType), item.PayMoney));
                      //  DB.GetDal<>
                        break;
                    case AccountType.Freeze:
                        setList.Add(string.Format(" [FreezeBalance]{0}{1}", GetOperFun(item.PayType), item.PayMoney));
                        break;
                    case AccountType.Commission:
                        setList.Add(string.Format(" [CommissionBalance]{0}{1}", GetOperFun(item.PayType), item.PayMoney));
                        break;
                    case AccountType.FillMoney:
                        setList.Add(string.Format(" [FillMoneyBalance]{0}{1}", GetOperFun(item.PayType), item.PayMoney));
                        break;
                    case AccountType.Experts:
                        setList.Add(string.Format(" [ExpertsBalance]{0}{1}", GetOperFun(item.PayType), item.PayMoney));
                        break;
                    case AccountType.RedBag:
                        setList.Add(string.Format(" [RedBagBalance]{0}{1}", GetOperFun(item.PayType), item.PayMoney));
                        break;
                    case AccountType.UserGrowth:
                        setList.Add(string.Format(" [UserGrowth]{0}{1}", GetOperFun(item.PayType), item.PayMoney));
                        break;
                    case AccountType.DouDou:
                        setList.Add(string.Format(" [CurrentDouDou]{0}{1}", GetOperFun(item.PayType), item.PayMoney));
                        break;
                    default:
                        break;
                }
            }

            var sql = string.Format("update [C_User_Balance] set {0},[Version]+=1 FROM  C_User_Balance where userid='{1}'", string.Join(",", setList), userId);
            DB.CreateSQLQuery(sql).Excute();
        }

        /// <summary>
        /// 查询用户余额
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public C_User_Balance QueryUserBalance(string userId)
        {

            return DB.CreateQuery<C_User_Balance>().Where(p => p.UserId == userId).FirstOrDefault();

        }
        public C_User_Register LoadUserRegister(string userId)
        {
            return DB.CreateQuery<C_User_Register>().Where(p => p.UserId == userId).FirstOrDefault();
            //return this.LoadByKey<C_User_Register>(userId);
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

        public C_User_Register QueryUserRegister(string userId)
        {
            
            return DB.CreateQuery<C_User_Register>().Where(p => p.UserId == userId).FirstOrDefault();
        }

        public void UpdateUserRegister(C_User_Register user)
        {
            DB.GetDal<C_User_Register>().Update(user);
        }

        public C_User_Register GetUserRegister(string userId)
        {
            return DB.CreateQuery<C_User_Register>().Where(u => u.UserId == userId).FirstOrDefault();
        }
    }
}
