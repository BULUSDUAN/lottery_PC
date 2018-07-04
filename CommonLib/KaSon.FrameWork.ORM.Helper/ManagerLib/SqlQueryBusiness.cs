using KaSon.FrameWork.ORM.Helper.UserHelper.Managers;
using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.UserHelper
{
   public class SqlQueryBusiness:DBbase
    {
        /// <summary>
        /// 查询某个yqid下面的 能满足领红包条件的用户个数
        /// </summary>
        /// <param name="AgentId">普通用户代理 邀请注册的会员</param>
        /// <returns></returns>
        public string QueryYqidRegisterByAgentId(string AgentId)
        {
            var manager = new SqlQueryManager();
            
             return manager.QueryYqidRegisterByAgentId(AgentId);
            
        }
    }
}
