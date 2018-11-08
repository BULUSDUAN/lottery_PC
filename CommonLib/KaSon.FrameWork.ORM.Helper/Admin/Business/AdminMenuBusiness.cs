using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel.Enum;
using EntityModel.CoreModel;

namespace KaSon.FrameWork.ORM.Helper
{
    public class AdminMenuBusiness
    {
        public List<E_Menu_List> QueryAllMenuList()
        {
            var menuManager = new MenuManager();
            var list= menuManager.QueryAllMenuList().Where(s => (s.MenuType == (int)MenuType.Web_Menu || s.MenuType == (int)MenuType.All) && s.IsEnable == true).ToList();
            return list;
        }
        public List<E_Menu_List> QueryMenuListByUserId(string userId)
        {
            var menuManager = new MenuManager();
            var list= menuManager.QueryMenuListByUserId(userId);
            return list;
        }

        public List<C_Auth_Function_List> QueryLowerLevelFuncitonList()
        {
            var manager = new MenuManager();
            return manager.QueryLowerLevelFuncitonList();   
        }
        public Withdraw_QueryInfoCollection QueryWithdrawList(string userId, WithdrawAgentType? agent, WithdrawStatus? status, decimal minMoney, decimal maxMoney, DateTime startTime, DateTime endTime, int sortType, string operUserId, int pageIndex, int pageSize, string bankcode)
        {
            var statusList = new List<int>();
            if (status.HasValue) statusList.Add((int)status.Value);

            var result = new Withdraw_QueryInfoCollection();
            return new MenuManager().QueryWithdrawList(userId, agent, status, minMoney, maxMoney, startTime, endTime, sortType, operUserId, pageIndex, pageSize, bankcode);
        }
        public Withdraw_QueryInfoCollection QueryWithdrawList2(decimal minMoney, decimal maxMoney, DateTime startTime, DateTime endTime, WithdrawStatus? status)
        {
            var result = new Withdraw_QueryInfoCollection
            {
                WithdrawList = new MenuManager().QueryWithdrawList2(minMoney, maxMoney, startTime, endTime, status)
            };
            return result;
        }

        
    }
}
