using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Domain.Entities;
using GameBiz.Core;
using NHibernate.Linq;

namespace GameBiz.Business.Domain.Managers
{
    public class UserIntegralManager : GameBizEntityManagement
    {
        public UserIntegralBalance GetUserIntegralBalance(string userId)
        {
            return this.GetByKey<UserIntegralBalance>(userId);
        }
        public void AddUserIntegralBalance(UserIntegralBalance entity)
        {
            this.Add<UserIntegralBalance>(entity);
        }
        public void UpdateUserIntegralBalance(UserIntegralBalance entity)
        {
            this.Update<UserIntegralBalance>(entity);
        }
        public void AddUserGetPrize(UserGetPrize entity)
        {
            this.Add<UserGetPrize>(entity);
        }
        public bool UserIsPrize(string userId, string prizeType)
        {
            Session.Clear();
            var query = from s in Session.QueryOver<UserGetPrize>().List() where s.UserId == userId && s.PrizeType == prizeType select s;
            if (query != null && query.ToList().Count > 0)
            {
                return true;
            }
            return false;
        }
        public void AddUserIntegralDetail(UserIntegralDetail entity)
        {
            this.Add<UserIntegralDetail>(entity);
        }
        public ActivityPrizeConfig GetActivityPrizeConfig(int activityId)
        {
            return this.LoadByKey<ActivityPrizeConfig>(activityId);
        }
        public void AddActivityPrizeConfig(ActivityPrizeConfig entity)
        {
            this.Add<ActivityPrizeConfig>(entity);
        }
        public void UpdateActivityPrizeConfig(ActivityPrizeConfig entity)
        {
            this.Update<ActivityPrizeConfig>(entity);
        }
        public void DeleteActivityPrizeConfig(int activityId)
        {
            Session.Clear();
            string strSql = "delete from C_Activity_PrizeConfig where ActivityId=:ActivityId";
            Session.CreateSQLQuery(strSql)
                   .SetInt32("ActivityId", activityId).UniqueResult();
        }
        public void UpdateUserRegister(string userId, int vipLevel)
        {
            Session.Clear();
            string strSql = "update C_User_Register set VipLevel=:VipLevel where UserId=:UserId";
            Session.CreateSQLQuery(strSql)
                .SetInt32("VipLevel", vipLevel)
                .SetString("UserId", userId)
                .UniqueResult();
        }
        public List<ActivityPrizeConfigInfo> QueryActivityPrizeConfigCollection(string title, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
            Session.Clear();
            DateTime sTime = Convert.ToDateTime(startTime.ToShortDateString());
            DateTime eTime = Convert.ToDateTime(endTime.ToShortDateString());
            var query = from a in Session.QueryOver<ActivityPrizeConfig>().List()
                        where (title == string.Empty || a.ActivityTitle.Contains(title)) && (a.CreateTime >= sTime && a.CreateTime < eTime.AddDays(1))
                        select new ActivityPrizeConfigInfo
                            {
                                ActivityId = a.ActivityId,
                                ActivityTitle = a.ActivityTitle,
                                ActivityContent = a.ActivityContent,
                                IsEnabled = a.IsEnabled,
                                CreateTime = a.CreateTime
                            };
            totalCount = query.ToList().Count;
            return query.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public void UpdateCoreConfig(CoreConfig entity)
        {
            this.Update(entity);
        }

        public CoreConfig QueryCoreConfig(int id)
        {
            Session.Clear();
            return this.Session.Query<CoreConfig>().FirstOrDefault(p => p.Id == id);
        }

        public List<CoreConfigInfo> QueryAllCoreConfig()
        {
            Session.Clear();
            return (from c in this.Session.Query<CoreConfig>()
                    select new CoreConfigInfo
                    {
                        ConfigKey = c.ConfigKey,
                        Id = c.Id,
                        ConfigName = c.ConfigName,
                        ConfigValue = c.ConfigValue,
                        CreateTime = c.CreateTime
                    }).ToList();
        }


        #region APP升级配置相关操作

        public List<APPConfigInfo> QueryAppConfigList()
        {
            Session.Clear();
            return (from a in Session.Query<APPConfig>()
                    select new APPConfigInfo
                    {
                        AppAgentId = string.IsNullOrEmpty(a.AppAgentId)?string.Empty:a.AppAgentId,
                        AgentName = string.IsNullOrEmpty(a.AgentName) ? string.Empty : a.AgentName,
                        ConfigCode = string.IsNullOrEmpty(a.ConfigCode) ? string.Empty : a.ConfigCode,
                        ConfigDownloadUrl = string.IsNullOrEmpty(a.ConfigDownloadUrl) ? string.Empty : a.ConfigDownloadUrl,
                        ConfigExtended = string.IsNullOrEmpty(a.ConfigExtended) ? string.Empty : a.ConfigExtended,
                        ConfigName = string.IsNullOrEmpty(a.ConfigName) ? string.Empty : a.ConfigName,
                        ConfigUpdateContent = string.IsNullOrEmpty(a.ConfigUpdateContent) ? string.Empty : a.ConfigUpdateContent,
                        IsForcedUpgrade = a.IsForcedUpgrade==null?false:a.IsForcedUpgrade,
                        ConfigVersion = string.IsNullOrEmpty(a.ConfigVersion) ? string.Empty : a.ConfigVersion
                    }).ToList();
        }
        public APPConfig QueryAppConfigByAgentId(string appAgentId)
        {
            Session.Clear();
            return this.Session.Query<APPConfig>().FirstOrDefault(p => p.AppAgentId == appAgentId);
        }
        public void UpdateAppConfig(APPConfig entity)
        {
            this.Update<APPConfig>(entity);
        }

        #endregion

        #region APP嵌套地址配置


        public NestedUrlConfig QueryNestedUrlByKey(string key)
        {
            Session.Clear();
            return Session.Query<NestedUrlConfig>().FirstOrDefault(s => s.ConfigKey == key&&s.IsEnable==true);
        }
        public List<NestedUrlConfig> QueryNestedUrlList()
        {
            Session.Clear();
            return Session.Query<NestedUrlConfig>().Where(s=>s.IsEnable==true).ToList();
        }
        public List<NestedUrlConfig> QueryNestedUrlListBytUrlType(int urlType)
        {
            Session.Clear();
            return Session.Query<NestedUrlConfig>().Where(s => s.IsEnable == true&&(s.UrlType==UrlType.All||s.UrlType==(UrlType)urlType)).ToList();
        }

        #endregion
    }
}
