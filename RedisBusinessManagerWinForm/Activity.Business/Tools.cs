using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Domain.Managers;
using System.Configuration;

namespace Activity.Business
{
    public class Tools
    {
        /// <summary>
        /// 用户是否能参与活动,true：能参与，反之不能
        /// </summary>
        public bool IsUserJoinPrizeActivity(string userId, string schemeId)
        {
            if (string.IsNullOrEmpty(userId))
                return false;
            //特殊用户
            var specialList = ConfigurationManager.AppSettings["SpecialUser"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (specialList.Contains(userId))
                return true;

            var sportsManager = new Sports_Manager();
            var order = sportsManager.QuerySports_Order_Complate(schemeId);
            if (order == null)
                return false;
            var together=sportsManager.QuerySports_Together(order.SchemeId);
            if (together == null)
                return false;
            if (together.SystemGuarantees > 0)
                return false;
            var agentList = ConfigurationManager.AppSettings["JoinActivityAgent"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (agentList.Contains(userId))
                return false;
            var manager = new UserBalanceManager();
            var user = manager.GetUserRegister(userId);
            if (user == null)
                return false;
            if (user.IsAgent==true)
                return false;
            if (string.IsNullOrEmpty(user.AgentId))
                return true;
            return !agentList.Contains(user.AgentId);
        }
    }
}
