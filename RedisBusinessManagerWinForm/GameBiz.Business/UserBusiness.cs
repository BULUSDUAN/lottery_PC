using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Auth.Domain.Entities;
using Common.Business;
using GameBiz.Domain.Managers;
using GameBiz.Domain.Entities;
using Common.Utilities;
using GameBiz.Core;
using GameBiz.Auth.Domain.Managers;
using Common;
using System.Data;
using Common.JSON;
using System.IO;

namespace GameBiz.Business
{
    public class UserBusiness
    {
        public void RegisterUser(SystemUser user, UserRegInfo regInfo)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                using (var manager = new UserBalanceManager())
                {
                    var register = new UserRegister
                    {
                        User = user,
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
                            var agentUser = manager.GetUserRegister(regInfo.AgentId);
                            if (agentUser != null)
                            {
                                register.ParentPath = agentUser.ParentPath + "/" + agentUser.UserId;
                            }
                        }
                    }
                    catch { }
                    manager.AddUserRegister(register);

                    var balance = new UserBalance
                    {
                        User = user,
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
                    manager.AddUserBalance(balance);
                }
                biz.CommitTran();
            }
        }
        public void ChangeUserStatus(string userId, EnableStatus status)
        {
            if (status == EnableStatus.Unknown)
            {
                throw new ArgumentException("要修改的用户状态不正确，不能为未知状态");
            }
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();
                using (var userManager = new UserBalanceManager())
                {
                    var user = userManager.GetUserRegister(userId);
                    if (user == null)
                    {
                        throw new ArgumentException("指定的用户不存在。");
                    }
                    user.IsEnable = (status == EnableStatus.Enable);
                    userManager.UpdateUserRegister(user);
                }
                biz.CommitTran();
            }
        }
        public void UpdateDisplayName(string userId, string newDisplayName)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new UserBalanceManager())
                {
                    var user = manager.GetUserRegister(userId);
                    user.DisplayName = newDisplayName;
                    manager.UpdateUserRegister(user);
                }
                biz.CommitTran();
            }
        }
        public void ChangeUserHideDisplayNameCount(string userId, int hideDisplayNameCount)
        {
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();
                using (var manager = new UserBalanceManager())
                {
                    var user = manager.GetUserRegister(userId);
                    user.HideDisplayNameCount = hideDisplayNameCount;
                    manager.UpdateUserRegister(user);
                }
                biz.CommitTran();
            }
        }
        public void UpdateUserVipLevel(string userId, int vipLevel)
        {
            using (var manager = new UserBalanceManager())
            {
                var user = manager.GetUserRegister(userId);
                user.VipLevel = vipLevel;
                manager.UpdateUserRegister(user);
            }
        }
        public UserRegister GetRegisterById(string userId)
        {
            using (var manager = new UserBalanceManager())
            {
                var reg = manager.GetUserRegister(userId);
                return reg;
            }
        }
        public void UpdateUserAgentId(string userId, string agentId)
        {
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var manager = new UserBalanceManager();
                var reg = manager.GetUserRegister(userId);
                if (reg == null)
                    throw new Exception("不存在此用户：" + userId);
                reg.AgentId = agentId;
                manager.UpdateUserRegister(reg);

                biz.CommitTran();
            }
        }
        public void LogOffUserAgent(string userId)
        {
            //开启事务
            using (var biz = new GameBizBusinessManagement())
            {
                biz.BeginTran();

                var manager = new UserBalanceManager();
                var reg = manager.GetUserRegister(userId);
                if (reg == null)
                    throw new Exception("不存在此用户：" + userId);
                reg.IsAgent = false;
                manager.UpdateUserRegister(reg);

                var userManager = new UserManager();
                var user = userManager.LoadUser(userId);
                if (user == null)
                    throw new Exception("不存在此用户：" + userId);
                var role = user.RoleList.FirstOrDefault(p => p.RoleId == "Agent");
                if (role != null)
                    user.RoleList.Remove(role);

                biz.CommitTran();
            }
        }

        #region 财务人员设置

        public void FinanceSetting(string opeType, FinanceSettingsInfo info, string operatorId)
        {
            try
            {
                using (var manage = new UserBalanceManager())
                {
                    if (info != null)
                    {

                        FinanceSettings entity = new FinanceSettings();
                        ObjectConvert.ConverInfoToEntity(info, ref entity);
                        entity.OperatorId = operatorId;
                        entity.CreateTime = DateTime.Now;
                        switch (opeType.ToLower())
                        {
                            case "add":
                                AddFinanceSettings(entity);
                                break;
                            case "update":
                                //UpdateFinanceSettings(entity);
                                manage.UpdateFinanceSettings(entity);
                                break;
                            case "delete":
                                manage.DeleteFinanceSettings(entity.FinanceId.ToString());
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
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void AddFinanceSettings(FinanceSettings entity)
        {
            try
            {
                using (var manage = new UserBalanceManager())
                {
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
            }
            catch (Exception ex)
            {
                throw new Exception("保存数据失败,原因:" + ex.Message);
            }
        }
        //public void UpdateFinanceSettings(FinanceSettings entity)
        //{
        //    try
        //    {
        //        using (var manage = new UserBalanceManager())
        //        {
        //            manage.UpdateFinanceSettings(entity);
        //            //FinanceSettings fEntity = manage.GetFinanceSettingsByFinanceId(entity.FinanceId);
        //            //if (fEntity != null && fEntity.FinanceId > 0)
        //            //{
        //            //    if (fEntity.OperateType != entity.OperateType)
        //            //    {
        //            //        if (manage.IsExistFinance(entity.UserId, entity.OperateType))
        //            //        {
        //            //            throw new Exception("当前用户已存在，不能重复添加！");
        //            //        }
        //            //        else
        //            //        {
        //            //            manage.UpdateFinanceSettings(entity);
        //            //        }
        //            //    }
        //            //}
        //            //else
        //            //{
        //            //    throw new Exception("为查询到当前财务员记录！");
        //            //}
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("保存数据失败,原因:" + ex.Message);
        //    }
        //}

        public FinanceSettingsInfo_Collection GetFinanceSettingsCollection(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            using (var manage = new UserBalanceManager())
            {
                return manage.GetFinanceSettingsCollection(userId, startTime, endTime, pageIndex, pageSize);
            }
        }

        public string GetCaiWuOperator()
        {
            using (var manage = new UserBalanceManager())
            {
                return manage.GetCaiWuOperator();
            }
        }
        public FinanceSettingsInfo GetFinanceSettingsByFinanceId(string FinanceId)
        {
            using (var manage = new UserBalanceManager())
            {
                return manage.GetFinanceSettingsByFinanceId(FinanceId);
            }
        }

        #endregion

        #region 首页发送短信

        public void AddSendMsgHistoryRecord(SendMsgHistoryRecordInfo info)
        {
            using (var manager = new UserBalanceManager())
            {
                if (info == null)
                    throw new Exception("未查询到数据！");
                var ListInfo = manager.GetSendMsgHistoryRecord(info.PhoneNumber, info.IP, info.MsgType);
                if (ListInfo != null && ListInfo.Count > 1)
                    throw new Exception("每天只能发送三次，并且手机号码必须不相同！");
                SendMsgHistoryRecord entity = new SendMsgHistoryRecord();
                ObjectConvert.ConverInfoToEntity(info, ref entity);
                manager.AddSendMsgHistoryRecord(entity);
            }
        }


        public SendMsgHistoryRecord_Collection QueryHistoryRecordCollection(string mobile, string status, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            using (var manager = new UserBalanceManager())
            {
                return manager.QueryHistoryRecordCollection(mobile, status, startTime, endTime, pageIndex, pageSize);
            }
        }

        #endregion

        #region 网站服务项

        public UserSiteServiceInfo QueryUserSiteServiceById(int Id)
        {
            using (var manager = new UserBalanceManager())
            {
                UserSiteServiceInfo info = new UserSiteServiceInfo();
                var entity = manager.QueryUserSiteServiceById(Id);
                if (entity == null)
                    return new UserSiteServiceInfo();
                ObjectConvert.ConverEntityToInfo(entity, ref info);
                return info;
            }
        }
        public void UpdateSiteService(UserSiteServiceInfo info)
        {
            using (var manager = new UserBalanceManager())
            {
                if (info.Id > 0)
                {
                    var entity = manager.QueryUserSiteServiceById(info.Id);
                    if (entity == null)
                        throw new Exception("您还没有当前服务项");
                    entity.IsEnable = info.IsEnable;
                    entity.UpdateTime = DateTime.Now;
                    manager.UpdateUserSiteService(entity);
                }
                else
                {
                    var entity = new UserSiteService();
                    entity.ExtendedOne = info.ExtendedOne;
                    entity.ExtendedTwo = info.ExtendedTwo;
                    entity.IsEnable = info.IsEnable;
                    entity.Remarks = entity.Remarks;
                    entity.UpdateTime = DateTime.Now;
                    entity.UserId = info.UserId;
                    entity.CreateTime = DateTime.Now;
                    entity.ServiceType = info.ServiceType;
                    manager.AddUserSiteService(entity);
                }
            }
        }
        public UserSiteServiceInfo QueryUserSiteServiceByUserId(string userId)
        {
            using (var manager = new UserBalanceManager())
            {
                UserSiteServiceInfo info = new UserSiteServiceInfo();
                var entity = manager.QueryUserSiteServiceByUserId(userId, ServiceType.DrawingNotice);
                if (entity == null) return null;
                ObjectConvert.ConverEntityToInfo(entity, ref info);
                return info;
            }
        }

        #endregion

        #region 网站结余

        private DataTable GetNewUserBalanceHistoryTable()
        {
            var table = new DataTable("C_User_Balance_History");
            //table.Columns.Add("Id", typeof(long));
            table.Columns.Add("SaveDateTime", typeof(string));
            table.Columns.Add("UserId", typeof(string));
            table.Columns.Add("FillMoneyBalance", typeof(decimal));
            table.Columns.Add("BonusBalance", typeof(decimal));
            table.Columns.Add("CommissionBalance", typeof(decimal));
            table.Columns.Add("ExpertsBalance", typeof(decimal));
            table.Columns.Add("FreezeBalance", typeof(decimal));
            table.Columns.Add("RedBagBalance", typeof(decimal));
            table.Columns.Add("UserGrowth", typeof(int));
            table.Columns.Add("CurrentDouDou", typeof(int));
            table.Columns.Add("CreateTime", typeof(DateTime));
            //table.PrimaryKey = new DataColumn[] { table.Columns["Id"] };
            return table;
        }

        /// <summary>
        /// 保存系统用户结余金额
        /// </summary>
        public void SaveUserBalanceLog(string saveDate)
        {
            var manager = new UserBalanceManager();
            var old = manager.QueryUserBalanceReport(saveDate);
            if (old != null)
                throw new Exception(string.Format("日期 {0} 的网站结余数据已生成。", saveDate));

            var userBalanceList = manager.QueryAllUserBalance(saveDate);
            var table = GetNewUserBalanceHistoryTable();
            for (int i = 0; i < userBalanceList.Count; i++)
            {
                var current = userBalanceList[i];
                DataRow r = table.NewRow();
                //r["Id"] = i;
                r["SaveDateTime"] = saveDate;
                r["UserId"] = current.UserId;
                r["FillMoneyBalance"] = current.FillMoneyBalance;
                r["BonusBalance"] = current.BonusBalance;
                r["CommissionBalance"] = current.CommissionBalance;
                r["ExpertsBalance"] = current.ExpertsBalance;
                r["FreezeBalance"] = current.FreezeBalance;
                r["RedBagBalance"] = current.RedBagBalance;
                r["UserGrowth"] = current.UserGrowth;
                r["CurrentDouDou"] = current.CurrentDouDou;
                r["CreateTime"] = DateTime.Now;
                table.Rows.Add(r);
            }
            //批量插入表
            new Sports_Manager().SqlBulkAddTable(table,
                "SaveDateTime", "UserId", "FillMoneyBalance", "BonusBalance", "CommissionBalance", "ExpertsBalance", "FreezeBalance", "RedBagBalance", "UserGrowth", "CurrentDouDou", "CreateTime");

            var report = new GameBiz.Domain.Entities.UserBalanceReport
            {
                CreateTime = DateTime.Now,
                SaveDateTime = saveDate,
                TotalBonusBalance = userBalanceList.Sum(p => p.BonusBalance),
                TotalCommissionBalance = userBalanceList.Sum(p => p.CommissionBalance),
                TotalDouDou = userBalanceList.Sum(p => p.CurrentDouDou),
                TotalExpertsBalance = userBalanceList.Sum(p => p.ExpertsBalance),
                TotalFillMoneyBalance = userBalanceList.Sum(p => p.FillMoneyBalance),
                TotalFreezeBalance = userBalanceList.Sum(p => p.FreezeBalance),
                TotalRedBagBalance = userBalanceList.Sum(p => p.RedBagBalance),
                TotalUserGrowth = userBalanceList.Sum(p => p.UserGrowth),
            };
            manager.AddUserBalanceReport(report);

            //缓存json文件
            var balanceContent = JsonSerializer.Serialize<List<UserBalanceHistoryInfo>>(userBalanceList);
            var reportContent = JsonSerializer.Serialize<UserBalanceReport>(report);
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WebReports", DateTime.Now.ToString("yyyyMM"));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var balanceFilePath = Path.Combine(path, string.Format("balanceList_{0}.json", saveDate));
            var reportFilePath = Path.Combine(path, string.Format("balanceReport_{0}.json", saveDate));
            File.WriteAllText(balanceFilePath, balanceContent, Encoding.UTF8);
            File.WriteAllText(reportFilePath, reportContent, Encoding.UTF8);
        }

        public UserBalanceHistoryInfoCollection QueryUserBalanceHistoryList(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            using (var manager = new UserBalanceManager())
            {
                return manager.QueryUserBalanceHistoryList(userId, startTime, endTime, pageIndex, pageSize);
            }
        }
        public UserBalanceReportInfoCollection QueryUserBalanceReportList(DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            using (var manager = new UserBalanceManager())
            {
                return manager.QueryUserBalanceReportList(startTime, endTime, pageIndex, pageSize);
            }
        }

        public NewKPIDetailInfoCollection NewKPIDetailList(DateTime startTime, DateTime endTime)
        {
            using (var userinfo = new UserBalanceManager())
            {             
               
                return userinfo.NewKPIDetailList(startTime, endTime);
               
            }
        }
        public NewSummaryReportInfoCollection NewSummaryReport(DateTime startTime, DateTime endTime)
        {
            using (var userinfo = new UserBalanceManager())
            {

                return userinfo.NewSummaryReport(startTime, endTime);

            }
        }
        
        #endregion
    }
}
