using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Cryptography;
using EntityModel.Enum;
using KaSon.FrameWork.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Lottery.Kg.ORM.Helper.UserHelper
{
   public class RegisterBusiness:DBbase
    {
        public void RegisterUser(SystemUser user, string[] roleIds)
        {
            if (roleIds.Length == 0)
            {
                throw new AuthException("必须指定角色");
            }

            DB.Begin();

            var lastKey = DB.CreateQuery<C_Auth_KeyRule>().Where(p=>p.RuleKey== "MAXKEY").FirstOrDefault();
            IList<string> skipList;
            user.UserId = BeautyNumberHelper.GetNextCommonNumber(lastKey.RuleValue, out skipList);

            var roleList = GetRoleListByIds(roleIds);
            if (roleList.Count != roleIds.Length)
            {
                throw new AuthException("指定的角色可能不存在 - " + string.Join(",", roleIds));
            }
            user.RoleList = roleList;
            //userManager.AddSystemUser(user);
            UpdateLastUserKey(user.UserId, skipList.Count);
            if (skipList.Count > 0)
            {
                AddSkipBeautyKey(skipList.ToArray(), lastKey.RuleValue, user.UserId);
            }

            DB.Commit();

        }

        public IList<SystemRole> GetRoleListByIds(string[] roleIds)
        {
            var list = new List<SystemRole>();
            foreach (var roleId in roleIds)
            {
                var AR = DB.CreateQuery<C_Auth_Roles>().Where(p=>p.RoleId==roleId).ToList().Select(p=>new SystemRole {
                     RoleId=p.RoleId,
                }) .FirstOrDefault();
                list.Add(AR);
            }
            return list;
        }


        public void UpdateLastUserKey(string userKey, int skipCount)
        {
            var maxRule = DB.CreateQuery<C_Auth_KeyRule>().Where(p => p.RuleKey == "MAXKEY").FirstOrDefault();
            maxRule.RuleValue = userKey;
            DB.GetDal<C_Auth_KeyRule>().Update(maxRule);
            
            var regCountRule = DB.CreateQuery<C_Auth_KeyRule>().Where(p => p.RuleKey == "REGCOUNT").FirstOrDefault();
            if (regCountRule == null)
            {
                regCountRule = new C_Auth_KeyRule
                {
                    RuleKey = "REGCOUNT",
                    RuleValue = "1",
                };
                DB.GetDal<C_Auth_KeyRule>().Add(regCountRule);
            }
            else
            {
                regCountRule.RuleValue = (int.Parse(regCountRule.RuleValue) + 1).ToString();
                DB.GetDal<C_Auth_KeyRule>().Update(regCountRule);
            }
           
            var skipCountRule = DB.CreateQuery<C_Auth_KeyRule>().Where(p => p.RuleKey == "SKIPCOUNT").FirstOrDefault();
            if (skipCountRule == null)
            {
                skipCountRule = new C_Auth_KeyRule
                {
                    RuleKey = "SKIPCOUNT",
                    RuleValue = skipCount.ToString(),
                };
                DB.GetDal<C_Auth_KeyRule>().Add(skipCountRule);
            }
            else
            {
                skipCountRule.RuleValue = (int.Parse(skipCountRule.RuleValue) + skipCount).ToString();
                DB.GetDal<C_Auth_KeyRule>().Update(skipCountRule);
            }
        }


        public void AddSkipBeautyKey(string[] userKeys, string prevKey, string nextKey)
        {
            var list = new List<C_Auth_BeautyKey>();
            foreach (var key in userKeys)
            {
                var item = new C_Auth_BeautyKey
                {
                    BeautyKey = key,
                    PrevUserKey = prevKey,
                    NextUserKey = nextKey,
                    Status = "SKIPED",
                };
                list.Add(item);
            }
            DB.GetDal<C_Auth_BeautyKey>().Add(list.ToArray());
           
        }

        public void RegisterUser(SystemUser user, UserRegInfo regInfo)
        {         
                    DB.Begin();
                  var loginBiz = new LocalLoginBusiness();
                  
                    var register = new C_User_Register
                    {
                        
                        DisplayName = regInfo.DisplayName,
                        ComeFrom = regInfo.ComeFrom,
                        RegType = regInfo.RegType,
                        RegisterIp = regInfo.RegisterIp,
                        Referrer = regInfo.Referrer,
                        ReferrerUrl = regInfo.ReferrerUrl,
                        IsEnable = true,
                        IsAgent = false,
                        IsFillMoney = false,
                        AgentId = regInfo.AgentId,
                        CreateTime = DateTime.Now,
                        VipLevel = 0,
                        UserId = user.UserId,
                    };
                    try
                    {
                        if (!string.IsNullOrEmpty(regInfo.AgentId))
                        {
                            var agentUser = loginBiz.GetRegisterById(regInfo.AgentId);
                            if (agentUser != null)
                            {
                                register.ParentPath = agentUser.ParentPath + "/" + agentUser.UserId;
                            }
                        }
                    }
                    catch { }

            var AuthUser = new C_Auth_Users
            {
                UserId = user.UserId,
                RegFrom = user.RegFrom,
                AgentId = regInfo.AgentId,
                CreateTime = DateTime.Now,
            };
                     
               DB.GetDal<C_User_Register>().Add(register);
               DB.GetDal<C_Auth_Users>().Add(AuthUser);

            var balance = new C_User_Balance
            {
                        
                        BonusBalance = 0M,
                        FreezeBalance = 0M,
                        CommissionBalance = 0M,
                        ExpertsBalance = 0M,
                        FillMoneyBalance = 0M,
                        RedBagBalance = 0M,
                        CurrentDouDou = 0,
                        UserGrowth = 0,
                        IsSetPwd = false,
                        NeedPwdPlace = string.Empty,
                        Password = string.Empty,
                        UserId = user.UserId,
                        Version = 0,
                        AgentId = regInfo.AgentId,
                    };
            DB.GetDal<C_User_Balance>().Add(balance);

            DB.Commit();
            
        }

        /// <summary>
        /// 资金收入/支出明细
        /// </summary>
        public class PayDetail
        {
            public AccountType AccountType { get; set; }
            public PayType PayType { get; set; }
            public decimal PayMoney { get; set; }
        }

        /// <summary>
        /// 查询某个赠送记录
        /// </summary>
        public E_TaskList QueryTaskListByCategory(string userId, TaskCategory taskCategory)
        {
            
            return DB.CreateQuery<E_TaskList>().FirstOrDefault(p => p.UserId == userId && p.TaskCategory == (int)taskCategory);
        }

        #region 成长值 和 澳彩豆豆

        /// <summary>
        /// 收入 --添加用户成长值
        /// 返回用户vip等级
        /// </summary>
        public int Payin_UserGrowth(string category, string orderId, string userId, int userGrowth, string summary)
        {
            if (userGrowth <= 0) return 0;

            var balanceManager = new LocalLoginBusiness();
            //var fundManager = new FundManager();
            var user = balanceManager.GetRegisterById(userId);
            var userBalance = balanceManager.QueryUserBalanceInfo(userId);
            var Fund_UserGrowthDetail = new C_Fund_UserGrowthDetail() {
                OrderId = orderId,
                UserId = userId,
                Category = category,
                CreateTime = DateTime.Now,
                BeforeBalance = userBalance.UserGrowth,
                PayMoney = userGrowth,
                PayType = (int)PayType.Payin,
                Summary = summary,
                AfterBalance = userBalance.UserGrowth + userGrowth,
           };
            DB.GetDal<C_Fund_UserGrowthDetail>().Add(Fund_UserGrowthDetail);
          
            var payDetailList = new List<PayDetail>();
            payDetailList.Add(new PayDetail
            {
                AccountType = AccountType.UserGrowth,
                PayMoney = userGrowth,
                PayType = PayType.Payin,
            });

            var vipLevel = GetUserVipLevel(userBalance.UserGrowth + userGrowth);
            //更新成长值
            //userBalance.UserGrowth += userGrowth;
            //balanceManager.UpdateUserBalance(userBalance);
            PayToUserBalance(userId, payDetailList.ToArray());
            if (user.VipLevel < vipLevel)
            {
                for (int i = user.VipLevel + 1; i <= vipLevel; i++)
                {
                    //达到相应等级赠送红包
                    if (new int[] { 3, 4, 5, 6 }.Contains(i))
                    {
                        var redBag = 0M;
                        switch (i)
                        {
                            case 3:
                                redBag = 2M;
                                break;
                            case 4:
                                redBag = 10M;
                                break;
                            case 5:
                                redBag = 20M;
                                break;
                            case 6:
                                redBag = 88M;
                                break;
                            default:
                                break;
                        }
                        //if (redBag > 0M)
                            //Payin_To_Balance(AccountType.RedBag, FundCategory_UserLevelUp, userId, orderId, redBag, string.Format("用户等级提升到{0}级", i), RedBagCategory.UserUpLevel);
                    }
                }
                //修改vip等级
                user.VipLevel = vipLevel;
                //balanceManager.UpdateUserRegister(user);
            }

            return user.VipLevel;
        }

        /// <summary>
        /// 计算用户等级
        /// </summary>
        private static int GetUserVipLevel(int userGrowth)
        {
            if (userGrowth >= 20000)
                return 9;
            if (userGrowth >= 16000)
                return 8;
            if (userGrowth >= 12000)
                return 7;
            if (userGrowth >= 8000)
                return 6;
            if (userGrowth >= 4000)
                return 5;
            if (userGrowth >= 2000)
                return 4;
            if (userGrowth >= 1000)
                return 3;
            if (userGrowth >= 500)
                return 2;
            return 0;
        }

        #endregion


        /// <summary>
        /// 支付到用户余额
        /// </summary>
        public void PayToUserBalance(string userId, params PayDetail[] array)
        {
            if (array.Length <= 0)
                return;

            var setList = new List<string>();
            Expression<Func<C_User_Balance, bool>> where;
            Expression<Func<C_User_Balance, C_User_Balance>> update;
            where = b => b.UserId == userId;
            foreach (var item in array)
            {
                switch (item.AccountType)
                {
                    case AccountType.Bonus:
                        setList.Add(string.Format(" [BonusBalance]{0}{1}", GetOperFun(item.PayType), item.PayMoney));
                        //update = b => new C_User_Balance {
                        //    BonusBalance= GetOperFun(item.PayType),                            item.PayMoney)
                        //};

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

            var sql = string.Format("update [C_User_Balance] set {0},[Version]+=1 where userid='{1};select 1'", string.Join(",", setList), userId);
            // DB.CreateSQLExc();
          var result=  DB.CreateSQLQuery(sql).First<int>();
            Console.WriteLine("result:"+ result);
        }

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

    }
}
