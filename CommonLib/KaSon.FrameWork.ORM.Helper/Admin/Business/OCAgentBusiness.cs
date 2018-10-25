using System;
using System.Collections.Generic;
using System.Linq;
using EntityModel.CoreModel;

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
    }
}
