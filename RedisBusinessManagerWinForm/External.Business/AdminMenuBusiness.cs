using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using External.Domain.Entities.AdminMenu;
using External.Domain.Managers.AdminMenu;
using GameBiz.Core;
using External.Core;

namespace External.Business
{
    public class AdminMenuBusiness
    {
        public IList<MenuItem> QueryMenuListByUserId(string userId)
        {
            using (var menuManager = new MenuManager())
            {
                return menuManager.QueryMenuListByUserId(userId);
            }
        }
        public IList<MenuItem> QueryAllMenuList()
        {
            using (var menuManager = new MenuManager())
            {
                return menuManager.QueryAllMenuList().Where(s => (s.MenuType == MenuType.Web_Menu || s.MenuType == MenuType.All) && s.IsEnable == true).ToList();
            }
        }
        public IList<MenuItem> QueryAgentMenuList()
        {
            using (var menuManager = new MenuManager())
            {
                return menuManager.QueryAllMenuList().Where(s => (s.MenuType == MenuType.Agent_Menu || s.MenuType == MenuType.All) && s.IsEnable == true).ToList();
            }
        }
        public FunctionCollection QueryLowerLevelFuncitonList()
        {
            using (var manager = new MenuManager())
            {
                return manager.QueryLowerLevelFuncitonList();
            }
        }
        public FunctionCollection QueryLowerLevelFuncitonByParentId(string parentId)
        {
            using (var manager = new MenuManager())
            {
                return manager.QueryLowerLevelFuncitonByParentId(parentId);
            }
        }
        public FunctionInfo QueryCurrentFuncitonById(string Id)
        {

            using (var manager = new MenuManager())
            {
                return manager.QueryCurrentFuncitonById(Id);
            }
        }


        public Withdraw_QueryInfoCollection QueryWithdrawList(string userId, WithdrawAgentType? agent, WithdrawStatus? status, decimal minMoney, decimal maxMoney, DateTime startTime, DateTime endTime, int sortType, string operUserId, int pageIndex, int pageSize,string bankcode)
        {
            var statusList = new List<int>();
            if (status.HasValue) statusList.Add((int)status.Value);

            var result = new Withdraw_QueryInfoCollection();
            var totalCount = 0;
            var totalMoney = 0M;
            var totalResponseMoney = 0M;
            var winCount = 0;
            var refusedCount = 0;
            var totalWinMoney = 0M;
            var totalRefusedMoney = 0M;
            result.WithdrawList = new MenuManager().QueryWithdrawList(userId, agent, status, minMoney, maxMoney, startTime, endTime, sortType, operUserId, pageIndex, pageSize,bankcode,
                out   winCount, out   refusedCount, out   totalWinMoney, out   totalRefusedMoney, out totalResponseMoney, out   totalCount, out totalMoney);
            result.TotalCount = totalCount;
            result.TotalMoney = totalMoney;
            result.WinCount = winCount;
            result.RefusedCount = refusedCount;
            result.TotalWinMoney = totalWinMoney;
            result.TotalRefusedMoney = totalRefusedMoney;
            result.TotalResponseMoney = totalResponseMoney;
            return result;
        }


        public Withdraw_QueryInfoCollection QueryWithdrawList2(decimal minMoney, decimal maxMoney, DateTime startTime, DateTime endTime, WithdrawStatus? status)
        {
            var result = new Withdraw_QueryInfoCollection();
            result.WithdrawList = new MenuManager().QueryWithdrawList2(minMoney, maxMoney, startTime, endTime, status);
            return result;
        }


        public int QueryWithdrawList3(decimal minMoney, decimal maxMoney, DateTime startTime, DateTime endTime)
        {
            var result = new MenuManager().QueryWithdrawList3(minMoney, maxMoney, startTime, endTime);
            return result;
        }

        public Withdraw_QueryInfoCollection QueryWithdrawListR(string userId, WithdrawAgentType? agent, WithdrawStatus? status, decimal minMoney, decimal maxMoney, DateTime startTime, DateTime endTime, int sortType, string operUserId, int pageIndex, int pageSize, string bankcode)
        {
            var statusList = new List<int>();
            if (status.HasValue) statusList.Add((int)status.Value);

            var result = new Withdraw_QueryInfoCollection();
            var totalCount = 0;
            var totalMoney = 0M;
            var totalResponseMoney = 0M;
            var winCount = 0;
            var refusedCount = 0;
            var totalWinMoney = 0M;
            var totalRefusedMoney = 0M;
            result.WithdrawList = new MenuManager().QueryWithdrawListR(userId, agent, status, minMoney, maxMoney, startTime, endTime, sortType, operUserId, pageIndex, pageSize, bankcode,
                out winCount, out refusedCount, out totalWinMoney, out totalRefusedMoney, out totalResponseMoney, out totalCount, out totalMoney);
            result.TotalCount = totalCount;
            result.TotalMoney = totalMoney;
            result.WinCount = winCount;
            result.RefusedCount = refusedCount;
            result.TotalWinMoney = totalWinMoney;
            result.TotalRefusedMoney = totalRefusedMoney;
            result.TotalResponseMoney = totalResponseMoney;
            return result;
        }


        public Withdraw_QueryInfoCollection QueryWithdrawListR2(decimal minMoney, decimal maxMoney, DateTime startTime, DateTime endTime, WithdrawStatus? status)
        {
            var result = new Withdraw_QueryInfoCollection();
            result.WithdrawList = new MenuManager().QueryWithdrawListR2(minMoney, maxMoney, startTime, endTime, status);
            return result;
        }


        public int QueryWithdrawListR3(decimal minMoney, decimal maxMoney, DateTime startTime, DateTime endTime)
        {
            var result = new MenuManager().QueryWithdrawListR3(minMoney, maxMoney, startTime, endTime);
            return result;
        }


    }
}
