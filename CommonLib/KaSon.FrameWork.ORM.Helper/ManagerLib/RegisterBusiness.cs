using EntityModel;
using EntityModel.CoreModel;

using EntityModel.Enum;
using EntityModel.ExceptionExtend;
using KaSon.FrameWork.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.UserHelper
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
            if (roleList==null)
            {
                throw new AuthException("指定的角色可能不存在 - " + string.Join(",", roleIds));
            }
            //user.RoleList = roleList;
            var Auth_UserRole = new C_Auth_UserRole
            {
                UserId = user.UserId,
                RoleId = roleList
            };

            DB.GetDal<C_Auth_UserRole>().Add(Auth_UserRole);
            //userManager.AddSystemUser(user);
            UpdateLastUserKey(user.UserId, skipList.Count);
            if (skipList.Count > 0)
            {
                AddSkipBeautyKey(skipList.ToArray(), lastKey.RuleValue, user.UserId);
            }

            DB.Commit();

        }

        public string GetRoleListByIds(string[] roleIds)
        {
            var list = "";
            foreach (var roleId in roleIds)
            {
                var AR = DB.CreateQuery<C_Auth_Roles>().Where(p=>p.RoleId==roleId).ToList().Select(p=>new SystemRole {
                     RoleId=p.RoleId,
                }) .FirstOrDefault();
                list=AR.RoleId;
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
        /// 查询某个赠送记录
        /// </summary>
        public E_TaskList QueryTaskListByCategory(string userId, TaskCategory taskCategory)
        {
            
            return DB.CreateQuery<E_TaskList>().Where(p => p.UserId == userId && p.TaskCategory == (int)taskCategory).FirstOrDefault();
        }

      

    }
}
