using EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.UserHelper
{
   public class OCAgentManager:DBbase
    {
        /// <summary>
        /// 查询用户所有的返点配置
        /// </summary>
        public List<P_OCAgent_Rebate> QueryOCAgentRebateList(string userId)
        {
          
            return DB.CreateQuery<P_OCAgent_Rebate>().Where(p => p.UserId == userId).OrderByDescending(p => p.CreateTime).ToList();
        }

        /// <summary>
        /// 新增用户返点配置
        /// </summary>
        public void AddOCAgentRebate(P_OCAgent_Rebate OCAgentRebate) {

             DB.GetDal<P_OCAgent_Rebate>().Add(OCAgentRebate);

        }

        /// <summary>
        /// 修改用户返点
        /// </summary>
        /// <param name="rebate"></param>
        public void UpdateOCAgentRebate(P_OCAgent_Rebate rebate) {

            DB.GetDal<P_OCAgent_Rebate>().Update(rebate);
        }

        /// <summary>
        /// 根据代理ID查询注册用户列表
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public List<C_User_Register> QueryAgentSubUser(string agentId)
        {
           
            return DB.CreateQuery<C_User_Register>().Where(p => p.AgentId == agentId).ToList();
        }

        /// <summary>
        /// 查询代理对应彩种的返点
        /// </summary>
        public P_OCAgent_Rebate QueryOCAgentDefaultRebateByRebateType(string userId, string gameCode, string gameType, int rebateType)
        {
           
            return DB.CreateQuery<P_OCAgent_Rebate>().OrderByDescending(p => p.CreateTime).Where(p => p.UserId == userId && p.GameCode == gameCode && p.GameType == gameType && p.RebateType == rebateType).FirstOrDefault();
        }

    }
}
