using System;
using System.Collections.Generic;
using System.Linq;
using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Enum;
using EntityModel.ExceptionExtend;

namespace KaSon.FrameWork.ORM.Helper
{
    /// <summary>
    /// 代理相关业务
    /// </summary>
    public class OCAgentBusiness : DBbase
    {
        /// <summary>
        /// 后台会员详情，更新经销商
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="agentId"></param>
        public void UpdateUserAgentId(string userId, string agentId)
        {
            //开启事务
            DB.Begin();
            var agentManager = new OCAgentManager();
            var manager = new UserBalanceManager();
            string oldAgentId = string.Empty;
            var reg = manager.GetUserRegister(userId);
            var ocAgent = agentManager.QueryOCAgent(agentId);
            if (reg == null)
                throw new Exception("不存在此用户：" + userId);
            else if (ocAgent == null)
                throw new Exception("上级用户不是代理商");
            var parentAgent = manager.GetUserRegister(agentId);
            oldAgentId = reg.AgentId;
            reg.AgentId = agentId;
            reg.ParentPath = parentAgent.ParentPath + "/" + agentId;
            manager.UpdateUserRegister(reg);
            if (ocAgent != null)
            {
                var currAgent = agentManager.QueryOCAgent(userId);
                if (currAgent != null)
                {
                    currAgent.ParentUserId = agentId;
                    currAgent.ParentPath = ocAgent.ParentPath + "/" + agentId;
                    agentManager.UpdateOCAgent(currAgent);
                }
            }
            var currUserRebateList = agentManager.QueryOCAgentRebateList(userId);
            if (currUserRebateList != null && currUserRebateList.Count > 0)
            {
                var parentRebateList = agentManager.QueryOCAgentRebateList(agentId);
                if (parentRebateList != null && parentRebateList.Count > 0)
                {
                    var keepRebate = decimal.Parse(new CacheDataBusiness().QueryCoreConfigByKey("ReservReturnPoint").ConfigValue);
                    foreach (var item in currUserRebateList)
                    {
                        var rebate = parentRebateList.FirstOrDefault(s => s.UserId == agentId && s.GameCode == item.GameCode && s.GameType == item.GameType);
                        if (rebate != null)
                        {
                            var result = item.Rebate - (rebate.Rebate - keepRebate);//判断：当自身返点高于上级减去系统保留返点时，则用上级返点
                            if (result >= 0)
                            {
                                item.Rebate = (rebate.Rebate >= 0 ? rebate.Rebate : 0);
                                if (item.SubUserRebate > rebate.Rebate)
                                    item.SubUserRebate = rebate.Rebate;
                                else
                                    item.SubUserRebate = rebate.SubUserRebate >= 0 ? rebate.SubUserRebate : 0;
                                agentManager.UpdateOCAgentRebate(item);
                            }
                        }
                    }
                    //修改当前用户下级相关联用户的返点
                    UpdateRelationChildRebate(userId, oldAgentId, agentId, reg.ParentPath);
                }
            }
            DB.Commit();
        }
        public void UpdateRelationChildRebate(string currUserId, string oldAgentId, string newAgentId, string path)
        {
            var manager = new OCAgentManager();
            var agentManager = new OCAgentManager();
            var parentUser = agentManager.QueryOCAgentRebateList(currUserId);
            var xjUser = agentManager.QueryLowerAgentListByParentId(currUserId);
            List<string> strUserIds = new List<string>();
            foreach (var item in xjUser)
            {
                var currRebate = parentUser.FirstOrDefault(s => s.GameCode == item.GameCode && (item.GameType == string.Empty || s.GameType == item.GameType));
                if (currRebate != null)
                {
                    if (item.Rebate > currRebate.Rebate)
                    {
                        item.Rebate = currRebate.Rebate;
                        if (item.SubUserRebate > currRebate.Rebate)
                            item.SubUserRebate = currRebate.Rebate;
                        else
                            item.SubUserRebate = item.SubUserRebate >= 0 ? item.SubUserRebate : 0;
                        agentManager.UpdateOCAgentRebate(item);
                    }
                }
                if (!strUserIds.Contains(item.UserId))
                {
                    strUserIds.Add(item.UserId);
                    UserBalanceManager userManager = new UserBalanceManager();
                    var userRegist = userManager.GetUserRegister(item.UserId);
                    if (userRegist != null)
                    {
                        if (!string.IsNullOrEmpty(userRegist.ParentPath))
                        {
                            var parentPath = path + "/";
                            parentPath = parentPath.Replace(currUserId, "");
                            parentPath = parentPath.Replace("//", "/");
                            int index = userRegist.ParentPath.IndexOf(currUserId);
                            var newPath = parentPath + userRegist.ParentPath.Substring(index, userRegist.ParentPath.Length - (index));
                            userRegist.ParentPath = newPath;
                            userManager.UpdateUserRegister(userRegist);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 添加代理
        /// </summary>
        public void AddOCAgent(OCAgentCategory category, string parentUserId, string userId, CPSMode cpsmode)
        {
                //DB.Begin();
                if (parentUserId == userId)
                    throw new Exception("不能将自己添加为自己的上级代理");
                var balanceManager = new UserBalanceManager();
                var parent = balanceManager.QueryUserRegister(parentUserId);
                if (category != OCAgentCategory.Company && parent == null && category != OCAgentCategory.Company && !string.IsNullOrEmpty(parentUserId))
                    throw new LogicException(string.Format("上级用户{0}不存在", parentUserId));
                var user = balanceManager.QueryUserRegister(userId);
                if (user == null)
                    throw new LogicException(string.Format("用户{0}不存在", userId));
                if (category != OCAgentCategory.Company)
                {
                    if (string.IsNullOrEmpty(user.AgentId))
                        throw new Exception("用户不属于您的下级用户");
                    if (user.AgentId != parentUserId)
                        throw new Exception("该用户非您发展的用户");
                }
                var agentManager = new OCAgentManager();
                var userAgent = agentManager.QueryOCAgent(userId);
                if (userAgent != null)
                    throw new Exception(string.Format("用户{0}已经是代理", userId));
                var parentAgent = agentManager.QueryOCAgent(parentUserId);
                if (category != OCAgentCategory.Company)
                {
                    if (parentAgent == null)
                        throw new Exception("上级用户不是代理");
                    if (category != OCAgentCategory.SportLotteryAgent)
                    {
                        switch ((OCAgentCategory)parentAgent.OCAgentCategory)
                        {
                            case OCAgentCategory.Company:
                                if (category == OCAgentCategory.Company)
                                    throw new LogicException("添加代理类型不正确，不能为公司类型");
                                break;
                            case OCAgentCategory.Market:
                                if (category == OCAgentCategory.Company)
                                    throw new LogicException("添加代理类型不正确，不能为公司类型");
                                break;
                            case OCAgentCategory.GeneralAgent:
                                if (category != OCAgentCategory.GeneralAgent)
                                    throw new LogicException("添加代理类型不正确，只能添加普通代理");
                                break;
                            default:
                                break;
                        }
                    }
                }
                user.AgentId = parentUserId;
                user.IsAgent = true;
                user.ParentPath = parentAgent != null ? parentAgent.ParentPath + "/" + parentUserId : user.ParentPath;
                balanceManager.UpdateUserRegister(user);
                var authBiz = new GameBizAuthBusiness();
                var userManager = new UserManager();
                var strRole = userManager.QueryUserRoleIdsByUserId(userId);
                if (!string.IsNullOrEmpty(strRole))
                {
                    var array_Role = strRole.Split(new string[] { "%item%" }, StringSplitOptions.RemoveEmptyEntries);
                    if (array_Role != null && array_Role.Length > 0)
                    {
                        if (!array_Role.Contains("Agent"))
                            authBiz.AddUserRoles(userId, new string[] { "Agent" });
                    }
                    else
                        authBiz.AddUserRoles(userId, new string[] { "Agent" });
                }
                else
                    authBiz.AddUserRoles(userId, new string[] { "Agent" });
                //保存代理上下级关系
                agentManager.AddOCAgent(new P_OCAgent()
                {
                    CreateTime = DateTime.Now,
                    OCAgentCategory = (int)category,
                    ParentUserId = parentUserId,
                    UserId = userId,
                    CustomerDomain = string.Empty,
                    ParentPath = parentAgent != null ? parentAgent.ParentPath + "/" + parentUserId : user.ParentPath,
                    CPSMode = category == OCAgentCategory.Company ? (int)cpsmode :parentAgent.CPSMode,
                });
                //返点配置
                var rebateList = agentManager.QueryOCAgentRebateList(parentUserId);
                foreach (var item in rebateList)
                {
                    if (item.SubUserRebate < 0M)
                        throw new Exception("添加下级代理前，请先设置上级代理的下级默认返点");
                    var currAgent = agentManager.QueryOCAgentDefaultRebateByRebateType(userId, item.GameCode, item.GameType, item.RebateType);
                    if (currAgent == null)
                    {
                        agentManager.AddOCAgentRebate(new P_OCAgent_Rebate()
                        {
                            CreateTime = DateTime.Now,
                            GameCode = item.GameCode,
                            GameType = item.GameType,
                            UserId = userId,
                            Rebate = item.SubUserRebate,
                            SubUserRebate = 0M,
                            RebateType = item.RebateType,
                            CPSMode = category == OCAgentCategory.Company ? (int)cpsmode : parentAgent.CPSMode,
                        });
                    }
                }
               // DB.Commit();
        }
        /// <summary>
        /// 查询代理销售充值注册量明细汇总
        /// </summary>
        public OCAagentDetailInfoCollection QueryAgentDetail(string agentId, string gameCode, DateTime starTime, DateTime endTime, int pageIndex, int pageSize, bool isRecharge)
        {
            var collection = new OCAagentDetailInfoCollection();
            collection = new OCAgentManager().QueryAgentDetail(agentId, gameCode, starTime, endTime, pageIndex, pageSize, isRecharge);
            return collection;
        }
    }
}
