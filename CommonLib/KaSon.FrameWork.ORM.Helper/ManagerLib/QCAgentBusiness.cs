using EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
   public class QCAgentBusiness:DBbase
    {
        public void EditOCAgentRebate(string parentUserId, string userId, string setString)
        {



            var agentManager = new OCAgentManager();
            var parentRebateList = agentManager.QueryOCAgentRebateList(parentUserId);
            var userRebateList = agentManager.QueryOCAgentRebateList(userId);

            //var userAgent = agentManager.QueryOCAgent(userId);
            var array = setString.Split('|');
            foreach (var item in array)
            {
                if (string.IsNullOrEmpty(item))
                    continue;
                var configArray = item.Split(':');
                if (configArray.Length != 4)
                    continue;

                var gameCode = configArray[0];
                var gameType = configArray[1];
                var rebateValue = decimal.Parse(configArray[2]);
                var rebateType = int.Parse(configArray[3]);
                var IsEnableSetMaxPayRabate = Convert.ToBoolean(new CacheDataBusiness().QueryCoreConfigByKey("IsEnableSetMaxPayRabate").ConfigValue);
                var AllowSetMaxPayRabate = Convert.ToDecimal(new CacheDataBusiness().QueryCoreConfigByKey("AllowSetMaxPayRabate").ConfigValue);
                if (IsEnableSetMaxPayRabate && !string.IsNullOrEmpty(parentUserId))
                {
                    if (rebateValue > AllowSetMaxPayRabate)
                        throw new Exception("设置返点不能高于" + AllowSetMaxPayRabate + "%");
                }
                //if (rebateValue <= 0M)
                if (rebateValue < 0M || rebateValue > 100M)
                    throw new Exception("返点必须大于0小于100");

                var parent = parentRebateList.OrderByDescending(p => p.CreateTime).Where(p => p.GameCode == gameCode && p.GameType == gameType && p.RebateType == rebateType).FirstOrDefault();
                if (parent == null)
                    continue;

                if (rebateValue > parent.Rebate && !string.IsNullOrEmpty(parentUserId))
                    throw new Exception("设置的返点不能大于上级返点");
                var keepRebate = decimal.Parse(new CacheDataBusiness().QueryCoreConfigByKey("ReservReturnPoint").ConfigValue);
                if ((parent.Rebate - rebateValue) < keepRebate && !string.IsNullOrEmpty(parentUserId))
                    throw new Exception(string.Format("用户自身保留的返点不能低于{0}%", keepRebate));

                //查询用户返点数据
                var rebate = userRebateList.OrderByDescending(p => p.CreateTime).Where(p => p.GameCode == gameCode && p.GameType == gameType && p.RebateType == rebateType).FirstOrDefault();
                if (rebate == null)
                {
                    //user 为普通用户
                    //添加返点数据
                    var OCAgentRebate=(new P_OCAgent_Rebate
                    {
                        CreateTime = DateTime.Now,
                        GameCode = gameCode,
                        GameType = gameType,
                        UserId = userId,
                        Rebate = rebateValue,
                        SubUserRebate = 0,
                        RebateType = rebateType, 
                    });
                    agentManager.AddOCAgentRebate(OCAgentRebate);

                }
                else
                {
                    //user 为代理用户
                    //修改返点数据
                    //if (rebateValue < rebate.Rebate)
                    //    throw new Exception("不能降低用户的返点");
                    rebate.Rebate = rebateValue;
                    rebate.CreateTime = DateTime.Now;
                    agentManager.UpdateOCAgentRebate(rebate);
                }
                //更新代理返点的同时更新他的下级返点
                UpdateLowerAgentRebate(userId, gameCode, gameType, rebateValue, rebateType);
            }
        }

        /// <summary>
        /// 更新代理返点的同时更新他的下级返点
        /// </summary>
        public void UpdateLowerAgentRebate(string agentId, string gameCode, string gameType, decimal rebate, int rebateType)
        {
            var ocAgentMannger = new OCAgentManager();
            var localAgent = new LocalLoginBusiness();
            //查询全部下级代理
            var agentList = ocAgentMannger.QueryAgentSubUser(agentId);
            if (agentList == null || agentList.Count == 0)
                return;
            var keepRebate = decimal.Parse(new CacheDataBusiness().QueryCoreConfigByKey("ReservReturnPoint").ConfigValue);
            foreach (var agent in agentList)
            {
                var agenrRebate = ocAgentMannger.QueryOCAgentDefaultRebateByRebateType(agent.UserId, gameCode, gameType, rebateType);
                if (agenrRebate == null)
                    continue;
                if (rebate > agenrRebate.Rebate || (rebate == agenrRebate.Rebate && keepRebate == 0M))
                    continue;
                agenrRebate.Rebate = rebate - keepRebate;
                agenrRebate.CreateTime = DateTime.Now;
                ocAgentMannger.UpdateOCAgentRebate(agenrRebate);

                UpdateLowerAgentRebate(agent.UserId, gameCode, gameType, agenrRebate.Rebate, rebateType);
            }
        }
    }
}
