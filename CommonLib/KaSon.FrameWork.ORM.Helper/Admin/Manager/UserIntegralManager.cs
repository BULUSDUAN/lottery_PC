using System;
using System.Collections.Generic;
using System.Linq;
using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Enum;

namespace KaSon.FrameWork.ORM.Helper
{
    public class UserIntegralManager : DBbase
    {
        public C_User_IntegralBalance GetUserIntegralBalance(string userId)
        {
            return DB.CreateQuery<C_User_IntegralBalance>().Where(x=>x.UserId==userId).FirstOrDefault();
        }
        public void AddUserIntegralBalance(C_User_IntegralBalance entity)
        {
            DB.GetDal<C_User_IntegralBalance>().Add(entity);
        }
        public void UpdateUserIntegralBalance(C_User_IntegralBalance entity)
        {
            DB.GetDal<C_User_IntegralBalance>().Update(entity);
        }
        public void AddUserGetPrize(C_User_GetPrize entity)
        {
            DB.GetDal<C_User_GetPrize>().Add(entity);
        }
        public bool UserIsPrize(string userId, string prizeType)
        {
            var query = from s in DB.CreateQuery<C_User_GetPrize>() where s.UserId == userId && s.PrizeType == prizeType select s;
            if (query != null && query.ToList().Count > 0)
            {
                return true;
            }
            return false;
        }
        public void AddUserIntegralDetail(C_User_IntegralDetail entity)
        {
            DB.GetDal<C_User_IntegralDetail>().Add(entity);
        }
        public C_Activity_PrizeConfig GetActivityPrizeConfig(int activityId)
        {
            return DB.CreateQuery<C_Activity_PrizeConfig>().Where(x=>x.ActivityId==activityId).FirstOrDefault();
        }
        public void AddActivityPrizeConfig(C_Activity_PrizeConfig entity)
        {
            DB.GetDal<C_Activity_PrizeConfig>().Add(entity);
        }
        public void UpdateActivityPrizeConfig(C_Activity_PrizeConfig entity)
        {
            DB.GetDal<C_Activity_PrizeConfig>().Update(entity);
        }
        public void DeleteActivityPrizeConfig(int activityId)
        {
            string strSql = "delete from C_Activity_PrizeConfig where ActivityId=@activityId";
            DB.CreateSQLQuery(strSql).SetInt("@activityId", activityId);
        }
        public void UpdateUserRegister(string userId, int vipLevel)
        {
            string strSql = "update C_User_Register set VipLevel=@VipLevel where UserId=@UserId";
            DB.CreateSQLQuery(strSql) .SetInt("VipLevel", vipLevel).SetString("UserId", userId);
        }
        public List<ActivityPrizeConfigInfo> QueryActivityPrizeConfigCollection(string title, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, out int totalCount)
        {
            DateTime sTime = Convert.ToDateTime(startTime.ToShortDateString());
            DateTime eTime = Convert.ToDateTime(endTime.ToShortDateString());
            var query = from a in DB.CreateQuery<C_Activity_PrizeConfig>()
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

        public void UpdateCoreConfig(C_Core_Config entity)
        {
            DB.GetDal<C_Core_Config>().Update(entity);
        }

        public C_Core_Config QueryCoreConfig(int Id)
        {
            return DB.CreateQuery<C_Core_Config>().Where(p => p.Id == Id).FirstOrDefault();
        }

        public List<CoreConfigInfo> QueryAllCoreConfig()
        {
             
            return (from c in DB.CreateQuery<C_Core_Config>()
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
            return (from a in DB.CreateQuery<C_App_Config>()
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
        public C_App_Config QueryAppConfigByAgentId(string appAgentId)
        {
            return DB.CreateQuery<C_App_Config>().Where(p => p.AppAgentId == appAgentId).FirstOrDefault();
        }
        public void UpdateAppConfig(C_App_Config entity)
        {
            DB.GetDal<C_App_Config>().Update(entity);
        }

        #endregion

        #region APP嵌套地址配置


        public C_APP_NestedUrlConfig QueryNestedUrlByKey(string key)
        {
            return DB.CreateQuery<C_APP_NestedUrlConfig>().Where(s => s.ConfigKey == key && s.IsEnable == true).FirstOrDefault();
        }
        public List<C_APP_NestedUrlConfig> QueryNestedUrlList()
        {
            return DB.CreateQuery<C_APP_NestedUrlConfig>().Where(s => s.IsEnable == true).ToList();
        }
        public List<C_APP_NestedUrlConfig> QueryNestedUrlListBytUrlType(int urlType)
        {
            return DB.CreateQuery<C_APP_NestedUrlConfig>().Where(s => s.IsEnable == true && (s.UrlType == (int)UrlType.All || s.UrlType == urlType)).ToList();
        }

        #endregion
    }
}
