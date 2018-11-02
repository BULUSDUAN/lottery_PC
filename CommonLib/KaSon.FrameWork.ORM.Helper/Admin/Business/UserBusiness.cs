using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using EntityModel;
using EntityModel.CoreModel;
using EntityModel.ExceptionExtend;

namespace KaSon.FrameWork.ORM.Helper
{
    public class UserBusiness : DBbase
    {
        public void UpdateUserVipLevel(string userId, int vipLevel)
        {
            var manager = new UserBalanceManager();
            var user = manager.GetUserRegister(userId);
            user.VipLevel = vipLevel;
            manager.UpdateUserRegister(user);
        }
        public void LogOffUserAgent(string userId)
        {
            //开启事务
           // DB.Begin();
            var manager = new UserBalanceManager();
            var reg = manager.GetUserRegister(userId);
            if (reg == null)
                throw new Exception("不存在此用户：" + userId);
            reg.IsAgent = false;
            manager.UpdateUserRegister(reg);

            var userManager = new UserManager();
            var user = userManager.LoadSystemUser(userId);
            if (user == null)
                throw new Exception("不存在此用户：" + userId);
            if (user.RoleList == null || !user.RoleList.Any())
                throw new LogicException("查询用户角色时出错,UserID:" + userId);
            var role = user.RoleList.FirstOrDefault(p => p.RoleId == "Agent");
            if (role != null)
                user.RoleList.Remove(role);
           // DB.Commit();

        }

        public FinanceSettingsInfo_Collection GetFinanceSettingsCollection(string userId, int pageIndex, int pageSize)
        {
            var manage = new UserBalanceManager();
            return manage.GetFinanceSettingsCollection(userId, pageIndex, pageSize);
        }

        public string GetCaiWuOperator()
        {
            var manage = new UserBalanceManager();
            return manage.GetCaiWuOperator();
        }

        public FinanceSettingsInfo GetFinanceSettingsByFinanceId(string FinanceId)
        {
            var manage = new UserBalanceManager();
            return manage.GetFinanceSettingsByFinanceId(FinanceId);
        }

        public void FinanceSetting(string opeType, C_FinanceSettings info, string operatorId)
        {
            try
            {
                var manage = new UserBalanceManager();
                if (info != null)
                {
                    info.OperatorId = operatorId;
                    info.CreateTime = DateTime.Now;
                    switch (opeType.ToLower())
                    {
                        case "add":
                            AddFinanceSettings(info);
                            break;
                        case "update":
                            //UpdateFinanceSettings(entity);
                            manage.UpdateFinanceSettings(info);
                            break;
                        case "delete":
                            manage.DeleteFinanceSettings(info.FinanceId.ToString());
                            break;
                    }
                }
                else
                {
                    if (opeType.ToLower() != "delete")
                    {
                        throw new Exception("保存数据失败！");
                    }
                    else
                    {
                        throw new Exception("删除数据失败！");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void AddFinanceSettings(C_FinanceSettings entity)
        {
            try
            {
                var manage = new UserBalanceManager();
                var FinanceInfo = manage.GetFinanceUserByUserId(entity.UserId);
                if (FinanceInfo != null && FinanceInfo.Count > 0)
                {
                    var query = from f in FinanceInfo where f.OperateType == entity.OperateType select f;
                    if (query != null && query.ToList().Count > 0)
                    {
                        throw new Exception("当前用户已存在，不能重复添加！");
                    }
                }
                manage.AddFinanceSettings(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("保存数据失败,原因:" + ex.Message);
            }
        }

        public UserBalanceHistoryInfoCollection QueryUserBalanceHistoryList(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            var manager = new UserBalanceManager();
            return manager.QueryUserBalanceHistoryList(userId, startTime, endTime, pageIndex, pageSize);
        }
    }
}
