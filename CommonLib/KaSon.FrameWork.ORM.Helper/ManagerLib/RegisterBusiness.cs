using EntityModel;
using EntityModel.Communication;
using EntityModel.CoreModel;

using EntityModel.Enum;
using EntityModel.ExceptionExtend;
using KaSon.FrameWork.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
   public class RegisterBusiness:DBbase
    {
        public void RegisterUser(SystemUser user, string[] roleIds)
        {
            if (roleIds.Length == 0)
            {
                throw new LogicException("必须指定角色");
            }

            DB.Begin();
            try
            {
                var lastKey = DB.CreateQuery<C_Auth_KeyRule>().Where(p => p.RuleKey == "MAXKEY").FirstOrDefault();
                IList<string> skipList;
                user.UserId = BeautyNumberHelper.GetNextCommonNumber(lastKey.RuleValue, out skipList);

                var roleList = GetRoleListByIds(roleIds);
                if (roleList == null)
                {
                    throw new LogicException("指定的角色可能不存在 - " + string.Join(",", roleIds));
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
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
        

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
            try
            {
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
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }


        }

     

        /// <summary>
        /// 查询某个赠送记录
        /// </summary>
        public E_TaskList QueryTaskListByCategory(string userId, TaskCategory taskCategory)
        {
            
            return DB.CreateQuery<E_TaskList>().Where(p => p.UserId == userId && p.TaskCategory == (int)taskCategory).FirstOrDefault();
        }

        public CommonActionResult UserRegister(RegisterInfo_Local regInfo,string fxid)
        {
            DB.Begin();
            try
            {
                string userId = null;
                var roleIds = ConfigHelper.AllConfigInfo["PageRegisterDefaultRole"].ToString().Split(',');
                #region 注册权限控制帐号

                var authBiz = new GameBizAuthBusiness();
                var regBiz = new RegisterBusiness();
                var userEntity = new SystemUser
                {
                    RegFrom = string.IsNullOrEmpty(regInfo.ComeFrom) ? "LOCAL" : regInfo.ComeFrom,
                    AgentId = regInfo.AgentId,
                };
                regBiz.RegisterUser(userEntity, roleIds);
                userId = userEntity.UserId;

                #endregion

                #region 注册核心系统显示帐号

                var userRegInfo = new UserRegInfo
                {
                    DisplayName = regInfo.LoginName,
                    ComeFrom = string.IsNullOrEmpty(regInfo.ComeFrom) ? "LOCAL" : regInfo.ComeFrom,
                    Referrer = regInfo.Referrer,
                    ReferrerUrl = regInfo.ReferrerUrl,
                    RegisterIp = regInfo.RegisterIp,
                    RegType = regInfo.RegType,
                    AgentId = regInfo.AgentId,
                };

                regBiz.RegisterUser(userEntity, userRegInfo);

                #endregion

                #region 注册本地登录帐号

                var loginBiz = new LocalLoginBusiness();
                var loginEntity = new LoginLocal
                {
                    LoginName = regInfo.LoginName,
                    Password = regInfo.Password,
                    mobile = regInfo.Mobile
                };
                loginBiz.Register(loginEntity, userEntity.UserId);

                #endregion

                #region 如果是通过代理链接注册，则设置用户返点 屏蔽：范  
                if (!string.IsNullOrEmpty(regInfo.AgentId))
                {
                    SetUserRebate(userId, regInfo.AgentId);
                }

                #endregion

                #region 初始化用户战绩数据和中奖概率数据

                InitUserBeedingAndBounsPercent(userId);

                #endregion

                #region 初始化其它数据

                InitBlog_ProfileBonusLevel(userId);
                InitUserAttentionSummary(userId);

                #endregion

                #region fxid分享推广数据
                if (!string.IsNullOrEmpty(fxid))
                {
                    var manager = new BlogManager();
                    manager.AddBlog_UserShareSpread(new E_Blog_UserShareSpread
                    {
                        UserId = userId,
                        AgentId = fxid,
                        CreateTime = DateTime.Now,
                        isGiveLotteryRedBag = false,
                        isGiveRegisterRedBag = false,
                        UpdateTime = DateTime.Now,
                        giveRedBagMoney = 0,
                        isGiveRechargeRedBag = false
                    });
                }
                #endregion

                DB.Commit();

                return new CommonActionResult
                {
                    IsSuccess = true,
                    Message = "注册成功",
                    ReturnValue = userId,
                };
            }
            catch(Exception ex) {

                DB.Rollback();
                throw ex;

            }
        }

        private void SetUserRebate(string userId, string agentId)
        {
            try
            {
                var agentManager = new OCAgentManager();
                var parentRebateList = agentManager.QueryOCAgentRebateList(agentId);
                var rebateList = new List<string>();
                foreach (var item in parentRebateList)
                {
                    rebateList.Add(string.Format("{0}:{1}:{2}:{3}", item.GameCode, item.GameType, item.SubUserRebate, item.RebateType));
                }
                var setString = string.Join("|", rebateList.ToArray());
                //new OCAgentBusiness().UpdateOCAgentRebate(agentId, userId, setString);

                //new OCAgentBusiness().EditOCAgentRebate(agentId, userId, setString);
            }
            catch (Exception ex)
            {
                //var writer = Common.Log.LogWriterGetter.GetLogWriter();
                //writer.Write("SetUserRebate", "SetUserRebate_设置返点", Common.Log.LogType.Error, "设置返点异常", ex.ToString());
            }
        }

        private void InitUserBeedingAndBounsPercent(string userId)
        {
            var sportsManager = new Sports_Manager();

            var allGameCodeArray = new string[] { "CTZQ", "BJDC", "JCZQ", "JCLQ", "SSQ", "DLT", "FC3D", "PL3", "CQSSC", "JX11X5" };
            var lotteryGameCodeArray = new string[] { "SSQ", "DLT", "FC3D", "PL3", "CQSSC", "JX11X5" };
            foreach (var item in allGameCodeArray)
            {
                if (lotteryGameCodeArray.Contains(item))
                {
                    //数字彩
                    AddUserBeedingAndBonusPercent(sportsManager, userId, item, string.Empty);
                }
                else
                {
                    //足彩
                    var gameTypeArray = GetGameTypeArray(item);
                    foreach (var t in gameTypeArray)
                    {
                        AddUserBeedingAndBonusPercent(sportsManager, userId, item, t);
                    }

                }
            }
        }

        private void InitBlog_ProfileBonusLevel(string userId)
        {
            var manager = new BlogManager();
            var BlogProfileBonusLevel = new E_Blog_ProfileBonusLevel
            {
                UserId = userId,
                MaxLevelName = "幸运彩民",
                MaxLevelValue = 0,
                TotalBonusMoney = 0,
                UpdateTime = DateTime.Now,
                WinHundredMillionCount = 0,
                WinOneHundredCount = 0,
                WinOneHundredThousandCount = 0,
                WinOneMillionCount = 0,
                WinOneThousandCount = 0,
                WinTenMillionCount = 0,
                WinTenThousandCount = 0,
            };
            manager.AddBlog_ProfileBonusLevel(BlogProfileBonusLevel);

            var BlogDataReport = new E_Blog_DataReport
            {
                CreateSchemeCount = 0,
                JoinSchemeCount = 0,
                TotalBonusCount = 0,
                TotalBonusMoney = 0,
                UpdateTime = DateTime.Now,
                UserId = userId,
            };
            manager.AddBlog_DataReport(BlogDataReport);

        }

        private void InitUserAttentionSummary(string userId)
        {
            var sportsManager = new Sports_Manager();
            var UserAttentionSummary = new C_User_Attention_Summary
            {
                UserId = userId,
                UpdateTime = DateTime.Now,
                BeAttentionUserCount = 0,
                FollowerUserCount = 0,
            };
            sportsManager.AddUserAttentionSummary(UserAttentionSummary);
        }

        private void AddUserBeedingAndBonusPercent(Sports_Manager sportsManager, string userId, string gameCode, string gameType)
        {
            var beeding = sportsManager.QueryUserBeedings(userId, gameCode, gameType);
            if (beeding == null)
            {
                var UserBeedings = new C_User_Beedings
                {
                    UserId = userId,
                    UpdateTime = DateTime.Now,
                    GameCode = gameCode,
                    GameType = gameType,
                    BeFollowedTotalMoney = 0M,
                    BeFollowerUserCount = 0,
                    GoldCrownCount = 0,
                    GoldCupCount = 0,
                    GoldDiamondsCount = 0,
                    GoldStarCount = 0,
                    SilverCrownCount = 0,
                    SilverCupCount = 0,
                    SilverDiamondsCount = 0,
                    SilverStarCount = 0,
                    TotalBonusMoney = 0M,
                    TotalBonusTimes = 0,
                };
                sportsManager.AddUserBeedings(UserBeedings);

            }
            var bonusPercent = sportsManager.QueryUserBonusPercent(userId, gameCode, gameType);
            if (bonusPercent == null)
            {
                var UserBonusPercent = new C_User_BonusPercent
                {
                    BonusPercent = 0M,
                    CreateTime = DateTime.Now,
                    CurrentDate = DateTime.Now.ToString("yyyyMM"),
                    GameCode = gameCode,
                    GameType = gameType,
                    UserId = userId,
                    BonusOrderCount = 0,
                    TotalOrderCount = 0,
                };
                sportsManager.AddUserBonusPercent(UserBonusPercent);

            }
        }

        private string[] GetGameTypeArray(string gameCode)
        {
            switch (gameCode)
            {
                case "CTZQ":
                    return new string[] { "T14C", "TR9", "T6BQC", "T4CJQ" };
                case "BJDC":
                    return new string[] { "SPF", "ZJQ", "SXDS", "BF", "BQC" };
                case "JCZQ":
                    return new string[] { "SPF", "BRQSPF", "BF", "ZJQ", "BQC", "HH" };
                case "JCLQ":
                    return new string[] { "SF", "RFSF", "SFC", "DXF", "HH" };
            }
            return new string[] { };
        }
    }
}
