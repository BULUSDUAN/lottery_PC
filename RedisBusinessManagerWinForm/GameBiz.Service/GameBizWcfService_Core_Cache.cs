using System;
using System.Linq;
using System.Transactions;
using Common.Communication;
using GameBiz.Business;
using GameBiz.Core;
using Common.Business;
using System.Configuration;
using GameBiz.Auth.Business;
using System.Collections.Generic;
using Common.Utilities;
using Common.Net;
using System.Threading;

namespace GameBiz.Service
{
    public partial class GameBizWcfService_Core
    {
        #region 超级发起人

        /// <summary>
        /// 查询超级发起人
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public SupperCreatorCollection QueryCache_SupperCreatorCollection()
        {
            try
            {
                return new CacheDataBusiness().QuerySupperCreatorCollection();
            }
            catch (Exception ex)
            {
                throw new Exception("查询超级发起人 - " + ex.Message, ex);
            }
        }

        #endregion

        #region 用户登录日志

        /// <summary>
        /// 查询用户最后一次登录信息
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public UserLoginHistoryInfo QueryCache_UserLastLoginInfo(string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                return new CacheDataBusiness().QueryUserLastLoginInfo(userId);
            }
            catch (Exception ex)
            {
                throw new Exception("查询用户登录日志 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查询用户历史登录
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public UserLoginHistoryCollection QueryCache_UserLoginHistoryCollection(string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                return new CacheDataBusiness().QueryUserLoginHistoryCollection(userId);
            }
            catch (Exception ex)
            {
                throw new Exception("获取最近登录 - " + ex.Message, ex);
            }
        }
        /// <summary>
        /// 根据用户编号查询用户历史登录
        /// </summary>
        public UserLoginHistoryCollection QueryCache_UserLoginHistoryCollectionByUserId(string userId, string userToken)
        {
            // 验证用户身份及权限
            var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            try
            {
                return new CacheDataBusiness().QueryUserLoginHistoryCollection(userId);
            }
            catch (Exception ex)
            {
                throw new Exception("获取最近登录 - " + ex.Message, ex);
            }
        }

        #endregion

        #region 个人主页

        #region 基础信息

        /// <summary>
        /// 修改用户基础信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="displayName"></param>
        /// <param name="hideNameCode"></param>
        /// <param name="headImage"></param>
        /// <param name="regTime"></param>
        /// <param name="userToken"></param>
        public void UpdateProfileUserInfo(string userId, string displayName, int? hideNameCode, string headImage, DateTime? regTime)
        {
            try
            {
                var biz = new CacheDataBusiness();
                biz.UpdateProfileUserInfo(userId, displayName, hideNameCode, headImage, null, null, regTime);
            }
            catch (Exception ex)
            {
                Common.Log.LogWriterGetter.GetLogWriter().Write("Error", "UpdateProfileVisitHistory", ex);
            }
        }
        /// <summary>
        /// 查询用户基础信息
        /// </summary>
        public ProfileUserInfo QueryProfileUserInfo(string userId)
        {
            var biz = new CacheDataBusiness();
            return biz.QueryProfileUserInfo(userId);
        }

        #endregion

        #region 数据统计

        /// <summary>
        /// 查询统计数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public ProfileDataReport QueryProfileDataReport(string userId)
        {
            var biz = new CacheDataBusiness();
            return biz.QueryProfileDataReport(userId);
        }
        #endregion

        #region 访客历史记录

        /// <summary>
        /// 修改历史访客记录
        /// </summary>
        public void UpdateProfileVisitHistory(string userId, string visitorIp, string userToken)
        {
            try
            {
                // 验证用户身份及权限
                var visitorUserId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

                if (!visitorUserId.Equals("Guest", StringComparison.OrdinalIgnoreCase))
                {
                    new CacheDataBusiness().UpdateProfileVisitHistory(userId, visitorUserId, visitorIp, DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                Common.Log.LogWriterGetter.GetLogWriter().Write("Error", "UpdateProfileVisitHistory", ex);
            }
        }
        /// <summary>
        /// 查询历史访客
        /// </summary>
        public ProfileVisitHistoryCollection QueryProfileVisitHistoryCollection(string userId, int count)
        {
            var biz = new CacheDataBusiness();
            return biz.QueryProfileVisitHistoryCollection(userId);
        }
        #endregion

        #region 获奖级别统计

        /// <summary>
        /// 查询获奖级别标题
        /// </summary>
        public string QueryProfileBonusLevelTitle(string userId)
        {
            var biz = new CacheDataBusiness();
            return biz.QueryProfileBonusLevelTitle(userId);
        }

        /// <summary>
        /// 查询获奖级别
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ProfileBonusLevelCollection QueryProfileBonusLevelCollection(string userId)
        {
            var biz = new CacheDataBusiness();
            return biz.QueryProfileBonusLevelCollection(userId);
        }

        /// <summary>
        /// 查询获奖级别
        /// </summary>
        public ProfileBonusLevelInfo QueryProfileBonusLevelInfo(string userId)
        {
            var biz = new CacheDataBusiness();
            return biz.QueryProfileBonusLevelInfo(userId);
        }


        #endregion

        #region 最新动态

        /// <summary>
        /// 查询最新动态
        /// </summary>
        /// <param name="userId"></param>
        public ProfileDynamicCollection QueryProfileDynamicCollection(string userId, int pageIndex, int pageSize)
        {
            var biz = new CacheDataBusiness();
            return biz.QueryProfileDynamicCollection(userId, pageIndex, pageSize);
        }
        #endregion

        #region 最新中奖

        /// <summary>
        /// 查询最新中奖
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public ProfileLastBonusCollection QueryProfileLastBonusCollection(string userId)
        {
            var biz = new CacheDataBusiness();
            return biz.QueryProfileLastBonusCollection(userId);
        }
        #endregion

        #region 定制跟单

        /// <summary>
        /// 查询已跟单数
        /// </summary>
        public int QueryProfileFollowedCount(string userId, string gameCode, string gameType)
        {
            var biz = new CacheDataBusiness();
            return biz.QueryProfileFollowedCount(userId, gameCode, gameType);
        }

        #endregion

        #region 用户关注

        /// <summary>
        /// 查询关注用户数量
        /// </summary>
        public int QueryProfileAttentionCount(string userId)
        {
            var biz = new CacheDataBusiness();
            return biz.QueryProfileAttentionCount(userId);
        }
        /// <summary>
        /// 查询已关注用户数量
        /// </summary>
        public int QueryProfileAttentionedCount(string userId)
        {
            var biz = new CacheDataBusiness();
            return biz.QueryProfileAttentionedCount(userId);
        }
        /// <summary>
        /// 查询已关注用户
        /// </summary>
        public ProfileAttentionCollection QueryProfileAttentionCollection(string userId, int pageIndex, int pageSize)
        {
            var biz = new CacheDataBusiness();
            return biz.QueryProfileAttentionCollection(userId, pageIndex, pageSize);
        }

        #endregion

        #endregion

        #region 系统配置相关

        public CoreConfigInfoCollection QueryAllCoreConfig(string userToken)
        {
            // 验证用户身份及权限
            var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new CacheDataBusiness().QueryAllCoreConfig();
            }
            catch (Exception ex)
            {
                throw new Exception("查询系统配置出错", ex);
            }
        }

        public CoreConfigInfo QueryCoreConfigByKey(string key)
        {
            try
            {
                return new CacheDataBusiness().QueryCoreConfigByKey(key);
            }
            catch (Exception ex)
            {
                throw new Exception("查询系统配置出错", ex);
            }
        }

        public CommonActionResult UpdateCoreConfigInfo(CoreConfigInfo info)
        {
            try
            {
                new CacheDataBusiness().UpdateCoreConfigInfo(info);
                return new CommonActionResult(true, "更新成功");
            }
            catch (Exception ex)
            {
                throw new Exception("更新系统配置出错", ex);
            }
        }
        /// <summary>
        /// 清理系统配置缓存
        /// </summary>
        /// <returns></returns>
        public CommonActionResult ClearCoreConfig()
        {
            try
            {
                //new CacheDataBusiness().ClearCoreConfig();
                return new CommonActionResult(true, "清理缓存成功");
            }
            catch (Exception ex)
            {
                throw new Exception("清理缓存失败", ex);
            }
        }

        public CommonActionResult LoadCoreConfigToRedis()
        {
            try
            {
                new CacheDataBusiness().LoadCoreConfigToRedis();
                return new CommonActionResult(true, "加载成功");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(false, ex.ToString());
            }
        }

        #region APP相关配置

        /// <summary>
        /// 查询APP所有配置
        /// </summary>
        /// <returns></returns>
        public APPConfig_Collection QueryAppConfigList()
        {
            try
            {
                return new CacheDataBusiness().QueryAppConfigList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 根据代理商编码查询APP配置
        /// </summary>
        /// <returns></returns>
        public APPConfigInfo QueryAppConfigByAgentId(string appAgentId)
        {
            try
            {
                return new CacheDataBusiness().QueryAppConfigByAgentId(appAgentId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 更新App配置
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public CommonActionResult UpdateAppConfig(APPConfigInfo info)
        {
            try
            {
                new CacheDataBusiness().UpdateAppConfig(info);
                return new CommonActionResult(true, "更新成功");
            }
            catch (Exception ex)
            {
                throw new Exception("更新APP配置出错", ex);
            }
        }
        /// <summary>
        /// 清空APP升级配置
        /// </summary>
        public CommonActionResult ClearAPPConfig()
        {
            try
            {
                new CacheDataBusiness().ClearAPPConfig();
                return new CommonActionResult(true, "清理缓存成功");
            }
            catch (Exception ex)
            {
                throw new Exception("清理缓存失败", ex);
            }
        }

        #endregion

        #region APP嵌套地址配置

        public NestedUrlConfigInfo QueryNestedUrlConfigByKey(string configKey)
        {
            try
            {
                return new CacheDataBusiness().QueryNestedUrlConfigByKey(configKey);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 根据UrlType查询所有APP嵌套配置
        /// </summary>
        /// <returns></returns>
        public NestedUrlConfig_Collection QueryNestedUrlConfigListByUrlType(int urlType)
        {
            try
            {
                return new CacheDataBusiness().QueryNestedUrlConfigListByUrlType(urlType);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 清空APP嵌套地址配置
        /// </summary>
        public CommonActionResult ClearNestedUrlConfig()
        {
            try
            {
                new CacheDataBusiness().ClearNestedUrlConfig();
                return new CommonActionResult(true, "清理缓存成功");
            }
            catch (Exception ex)
            {
                throw new Exception("清理缓存失败", ex);
            }
        }

        #endregion


        #endregion


        #region  插件相关

        /// <summary>
        /// 添加插件相关信息
        /// </summary>
        public CommonActionResult AddPluginClass(PluginClassInfo pluginClass)
        {
            try
            {
                new PluginClassBusiness().AddPluginClass(pluginClass);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("添加插件信息出错 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 更新插件相关信息
        /// </summary>
        public CommonActionResult UpdatePluginClass(PluginClassInfo pluginClass, int id)
        {
            try
            {
                new PluginClassBusiness().UpdatePluginClass(pluginClass, id);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("更新插件信息出错 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 删除插件相关信息
        /// </summary>
        public CommonActionResult DeletePluginClass(int id)
        {
            try
            {
                new PluginClassBusiness().DeletePluginClass(id);
                return new CommonActionResult(true, "操作成功");
            }
            catch (Exception ex)
            {
                throw new Exception("删除插件信息出错 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询某个插件信息
        /// </summary>
        public PluginClassInfo PluginClassInfoById(int id)
        {
            try
            {
                return new PluginClassBusiness().PluginClassInfoById(id);
            }
            catch (Exception ex)
            {
                throw new Exception("查询:" + id + "插件信息出错 - " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询插件列表
        /// </summary>
        public PluginClassInfo_Collection QueryPluginClassCollection(string interfaceName, int pageIndex, int pageSize)
        {
            var biz = new PluginClassBusiness();
            return biz.QueryPluginClassCollection(interfaceName, pageIndex, pageSize);
        }

        /// <summary>
        /// 查询插件列表
        /// </summary>
        public PluginClassInfo_Collection QueryPluginClassList(int pageIndex, int pageSize)
        {
            var biz = new PluginClassBusiness();
            return biz.QueryPluginClassList(pageIndex, pageSize);
        }

        #endregion
    }
}
