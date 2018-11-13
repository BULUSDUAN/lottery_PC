using EntityModel.CoreModel;
using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel;
using KaSon.FrameWork.Common.Utilities;
using EntityModel.Communication;
using EntityModel.ExceptionExtend;

namespace KaSon.FrameWork.ORM.Helper
{
    public partial class AdminService
    {
        public LoginInfo LoginAdmin(string loginName, string password, string loginIp)
        {
            var loginBiz = new LocalLoginBusiness();
            var loginEntity = loginBiz.AdminLogin(loginName, password);
            if (loginEntity == null)
            {
                return new LoginInfo { IsSuccess = false, Message = "登录名或密码错误", LoginFrom = "ADMIN", };
            }

            var register = new UserBalanceManager().LoadUserRegister(loginEntity.UserId);
            if (register == null)
            {
                return new LoginInfo { IsSuccess = false, Message = "找不到用户注册信息", LoginFrom = "ADMIN", };
            }
            if (!register.IsEnable)
            {
                return new LoginInfo { IsSuccess = false, Message = "用户已禁用", LoginFrom = "ADMIN", };
            }
            var authBiz = new GameBizAuthBusiness();
            if (!IsRoleType(loginEntity.User, RoleType.BackgroundRole))
            {
                return new LoginInfo { IsSuccess = false, Message = "此帐号角色不允许在此登录", LoginFrom = "ADMIN", };
            }
            var userToken = authBiz.GetUserToken(loginEntity.User.UserId);

            bool isAdmin = false;
            if (loginEntity.User != null)
            {
                var query = loginEntity.User.RoleList.FirstOrDefault(s => s.IsAdmin == true);
                if (query != null && query.IsAdmin)
                    isAdmin = true;
            }

            //获取权限点
            List<string> _FunctionList = new List<string>();
            try
            {
                if (loginEntity.User != null && loginEntity.User.RoleList != null)
                {
                    List<string> _role = new List<string>();
                    foreach (var item in loginEntity.User.RoleList)
                    {
                        _role.Add(item.RoleId);
                    }
                    _FunctionList = loginBiz.QueryFunctionByRole(_role.ToArray());
                }
            }
            catch
            {
            }

            //! 执行扩展功能代码 - 提交事务前
            BusinessHelper.ExecPlugin<IUser_AfterLogin>(new object[] { loginEntity.UserId, "ADMIN", loginIp, DateTime.Now });

            return new LoginInfo { IsSuccess = true, Message = "登录成功", CreateTime = loginEntity.CreateTime, LoginFrom = "ADMIN", RegType = loginEntity.Register.RegType, Referrer = loginEntity.Register.Referrer, UserId = loginEntity.User.UserId, VipLevel = loginEntity.Register.VipLevel, LoginName = loginEntity.LoginName, DisplayName = loginEntity.LoginName, UserToken = userToken, FunctionList = _FunctionList, IsAdmin = isAdmin };
        }

        private bool IsRoleType(SystemUser user, RoleType roleType)
        {
            foreach (var role in user.RoleList)
            {
                if (role.RoleType == roleType)
                {
                    return true;
                }
            }
            return false;
        }

        #region 权限的增删改查
        /// <summary>
        /// 查询后台菜单
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public List<MenuInfo> QueryMyMenuCollection(string userId,bool adminFlag)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication_Admin(userToken);

            var biz = new AdminMenuBusiness();
            List<E_Menu_List> list;
            //GameBizAuthBusiness.CheckIsAdmin(userToken)
            if (adminFlag)
            {
                list = biz.QueryAllMenuList();
            }
            else
            {
                list = biz.QueryMenuListByUserId(userId);
            }
            var result = list.Select(p => new MenuInfo()
            {
                Description = p.Description,
                DisplayName = p.DisplayName,
                IsEnable = p.IsEnable,
                MenuId = p.MenuId,
                MenuType = (MenuType)p.MenuType,
                ParentId = p.ParentMenuId,
                Url = p.Url
            }).ToList();
            return result;
        }

        /// <summary>
        /// 查询角色的信息及所有权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public RoleInfo_Query GetSystemRoleById(string roleId)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            var authBiz = new GameBizAuthBusiness();
            return authBiz.GetSystemRoleById(roleId);
        }

        /// <summary>
        /// 查询菜单下级功能权限点
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public List<C_Auth_Function_List> QueryLowerLevelFuncitonList()
        {
            try
            {
                return new AdminMenuBusiness().QueryLowerLevelFuncitonList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 新增系统角色
        /// todo:后台权限
        /// </summary>
        public CommonActionResult AddSystemRole(RoleInfo_Add roleInfo, string userToken)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            //PreconditionAssert.IsNotEmptyString(roleInfo.RoleId, "添加的角色编号不能为空");
            //PreconditionAssert.IsNotEmptyString(roleInfo.RoleName, "添加的角色名称不能为空");
            //PreconditionAssert.IsNotNull(roleInfo.FunctionList, "传入角色的权限列表不能为null");

            var authBiz = new GameBizAuthBusiness();
            var result = authBiz.AddSystemRole(roleInfo);
            return new CommonActionResult(true, "添加角色成功");
        }


        /// <summary>
        /// 修改系统角色
        /// todo:后台权限
        /// </summary>
        public CommonActionResult UpdateSystemRole(RoleInfo_Update roleInfo, string userToken)
        {
            // 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            PreconditionAssert.IsNotEmptyString(roleInfo.RoleId, "修改的角色编号不能为空");
            PreconditionAssert.IsNotEmptyString(roleInfo.RoleName, "修改的角色名称不能为空");
            //PreconditionAssert.IsNotNull(roleInfo.AddFunctionList, "传入角色的新增权限列表不能为null");
            //PreconditionAssert.IsNotNull(roleInfo.ModifyFunctionList, "传入角色的修改权限列表不能为null");
            //PreconditionAssert.IsNotNull(roleInfo.RemoveFunctionList, "传入角色的移除权限列表不能为null");

            var authBiz = new GameBizAuthBusiness();
            authBiz.UpdateSystemRole(roleInfo);
            return new CommonActionResult(true, "修改角色成功");
        }
        #endregion

        #region 管理端操作日志
        public CommonActionResult AddSysOperationLog(string userId, string operUserId, string menuName, string desc)
        {
            try
            {
                new SiteMessageBusiness().AddSysOperationLog(userId, operUserId, menuName, desc);
                return new CommonActionResult(true, "新增日志成功");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询后台操作日志
        /// </summary>
        public SysOperationLog_Collection QuerySysOperationList(string menuName, string userId, string operUserId, DateTime startTime, DateTime endTimen, int pageIndex, int pageSize)
        {
            try
            {
                return new SiteMessageBusiness().QuerySysOperationList(menuName, userId, operUserId, startTime, endTimen, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 获取角色列表
        /// <summary>
        /// 获取系统角色
        /// todo:后台权限
        /// </summary>
        public List<C_Auth_Roles> GetSystemRoleCollection()
        {
            var authBiz = new GameBizAuthBusiness();
            return authBiz.GetSystemRole();
        }
        #endregion

        #region 人员管理
        /// <summary>
        /// 查询人员列表
        /// </summary>
        public SysOpratorInfo_Collection GetOpratorCollection(int pageIndex, int pageSize)
        {

            //// 验证用户身份及权限
            //var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new GameBizAuthBusiness().GetOpratorCollection(pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询角色列表
        /// </summary>
        public List<C_Auth_Roles> QueryRoleCollection()
        {
            try
            {
                return new GameBizAuthBusiness().QueryRoleCollection();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 根据用户编号，查询角色
        /// </summary>
        public string QueryUserRoleIdsByUserId(string userId)
        {
            // 验证用户身份及权限
            //var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new GameBizAuthBusiness().QueryUserRoleIdsByUserId(userId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private const string C_DefaultPassword = "123456";
        /// <summary>
        /// 增加后台管理人员.
        /// todo:后台权限
        /// </summary>
        public CommonActionResult AddBackgroundAdminUser(RegisterInfo_Admin regInfo)
        {
            //// 验证用户身份及权限
            //var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            if (!Common.Utilities.UsefullHelper.IsInTest)
            {
                if (string.IsNullOrEmpty(regInfo.LoginName) || regInfo.LoginName.Length < 4 || regInfo.LoginName.Length > 20)
                {
                    throw new LogicException("后台人员登录名长度必须在4位到20位之间");
                }
            }
            if (regInfo.RoleIdList.Count == 0)
            {
                throw new LogicException("必须为管理人员指定角色");
            }
            var business = new GameBusinessManagement();
            business.AddBackgroundAdminUser(regInfo);
            return new CommonActionResult(true, "添加后台管理人员成功");
        }

        /// <summary>
        /// 修改后台用户信息
        /// todo:后台权限
        /// </summary>
        public CommonActionResult UpdateBackgroundUserInfo(string userId, string displayName, string addRoleIdList, string removeRoleIdList)
        {
            //// 验证用户身份及权限
            //var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            var business = new GameBusinessManagement();
            business.UpdateBackgroundUserInfo(userId, displayName, addRoleIdList, removeRoleIdList);
            return new CommonActionResult(true, "修改后台管理人员信息成功");
        }
        #endregion

        #region 插件管理
        public PluginClassInfo_Collection QueryPluginClassList(int pageIndex, int pageSize)
        {
            var biz = new PluginClassBusiness();
            return biz.QueryPluginClassList(pageIndex, pageSize);
        }
        /// <summary>
        /// 根据Id查询插件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public C_Activity_PluginClass PluginClassInfoById(int id)
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
        /// 添加插件相关信息
        /// </summary>
        public CommonActionResult AddPluginClass(C_Activity_PluginClass pluginClass)
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
        public CommonActionResult UpdatePluginClass(C_Activity_PluginClass pluginClass)
        {
            try
            {
                new PluginClassBusiness().UpdatePluginClass(pluginClass);
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


        #endregion

        #region 配置相关
        public List<C_Core_Config> QueryAllCoreConfig()
        {
            try
            {
                return new CacheDataBusiness().QueryAllCoreConfig();
            }
            catch (Exception ex)
            {
                throw new Exception("查询系统配置出错", ex);
            }
        }

        public CommonActionResult UpdateCoreConfigInfo(C_Core_Config info)
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
        #endregion

        #region 公告管理
        /// <summary>
        /// 查询后台管理公告列表
        /// todo:后台权限
        /// </summary>
        public BulletinInfo_Collection QueryManagementBulletinCollection(string key, EnableStatus status, int priority, int isPutTop, int pageIndex, int pageSize)
        {
            try
            {
                var siteBiz = new SiteMessageBusiness();
                return siteBiz.QueryManagementBulletins(key, status, priority, isPutTop, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        /// <summary>
        /// 修改公告状态
        /// todo:后台权限
        /// </summary>
        public CommonActionResult ChangeBulleinStatus(long bulletinId, EnableStatus status, string userId)
        {
            //// 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                string desc = "";
                switch (status)
                {
                    case EnableStatus.Enable:
                        desc = "启用";
                        break;
                    case EnableStatus.Disable:
                        desc = "禁用";
                        break;
                    default:
                        throw new LogicException("不允许的修改公告的状态");
                }
                var siteBiz = new SiteMessageBusiness();
                siteBiz.UpdateBulletinStatus(bulletinId, status, userId);

                return new CommonActionResult { IsSuccess = true, Message = desc + "公告成功", };
            }
            catch (LogicException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("执行出错", ex);
            }
        }

        /// <summary>
        /// 根据公告编号，查询公告明细
        /// todo:后台权限
        /// </summary>
        public BulletinInfo_Query QueryManagementBulletinDetailById(long bulletinId)
        {
            //// 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            var dataQuery = new DataQuery();
            return dataQuery.QueryBulletinDetailById(bulletinId);
        }

        /// <summary>
        /// 发布公告
        /// todo:后台权限
        /// </summary>
        public CommonActionResult PublishBulletin(E_SiteMessage_Bulletin_List bulletin, string userId)
        {
            //// 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                string errMsg;
                //! 验证订单正确性
                if (!ValidateBulletin(bulletin, out errMsg))
                {
                    return new CommonActionResult(false, errMsg);
                }

                var siteBiz = new SiteMessageBusiness();
                siteBiz.PublishBulletin(bulletin, userId);

                return new CommonActionResult { IsSuccess = true, Message = "发布公告成功", };
            }
            catch (Exception ex)
            {
                throw new Exception("发布出错",ex);
            }
        }

        /// <summary>
        /// 发布公告
        /// todo:后台权限
        /// </summary>
        public CommonActionResult UpdateBulletin(E_SiteMessage_Bulletin_List bulletin, string userId)
        {
            try
            {
                string errMsg;
                //! 验证订单正确性
                if (!ValidateBulletin(bulletin, out errMsg))
                {
                    return new CommonActionResult(false, errMsg);
                }
                var siteBiz = new SiteMessageBusiness();
                siteBiz.UpdateBulletin(bulletin, userId);
                return new CommonActionResult { IsSuccess = true, Message = "修改公告成功", };
            }
            catch (Exception ex)
            {
                throw new Exception("更新出错",ex);
            }
        }

        private bool ValidateBulletin(E_SiteMessage_Bulletin_List bulletin, out string errMsg)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(bulletin.Title)) throw new Exception("公告标题不能为空");
                if (string.IsNullOrWhiteSpace(bulletin.Content)) throw new Exception("公告内容不能为空");
                //if (bulletin.EffectiveTo.HasValue && bulletin.EffectiveTo.Value < DateTime.Now) throw new Exception("此公告已过期");
                //if (bulletin.EffectiveTo.HasValue && bulletin.EffectiveFrom.HasValue && bulletin.EffectiveTo.Value < bulletin.EffectiveFrom.Value) throw new Exception("此公告有效时间周期不正确");

                errMsg = "";
                return true;
            }
            catch (Exception ex)
            {
                errMsg = ex.ToString();
                return false;
            }
        }
        #endregion

        #region 文章管理
        /// <summary>
        /// 查询文章列表
        /// todo:后台权限
        /// </summary>
        public ArticleInfo_QueryCollection QueryArticleList(string key, string gameCode, string category, int pageIndex, int pageSize)
        {
            //// 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            //var siteBiz = new SiteMessageBusiness();
            //int totalCount;
            //var list = siteBiz.QueryArticleList(key, gameCode, category, pageIndex, pageSize, out totalCount);

            //var result = new ArticleInfo_QueryCollection();
            //result.TotalCount = totalCount;
            //result.LoadList(list);
            //return result;
            var query = new DataQuery();
            return query.QueryArticleList(key, gameCode, category, pageIndex, pageSize);
        }

        /// <summary>
        /// 查询彩种
        /// </summary>
        public GameInfoCollection QueryGameList()
        {
            //// 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return GameBusiness.GameList();
            }
            catch (Exception ex)
            {
                throw new Exception("查询彩种出错", ex);
            }
        }

        /// <summary>
        /// 根据编号查询文章信息_后台
        /// todo:后台权限
        /// </summary>
        public E_SiteMessage_Article_List QueryArticleById_Admin(string articleId)
        {
            //// 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            var siteBiz = new SiteMessageBusiness();
            var info = siteBiz.QueryArticleInfoById(articleId, false);
            return info;
        }
        /// <summary>
        /// 删除文章
        /// todo:后台权限
        /// </summary>
        public CommonActionResult DeleteArticle(string articleId)
        {

            var siteBiz = new SiteMessageBusiness();
            siteBiz.DeleteArticle(articleId);
            return new CommonActionResult(true, "删除文章成功");
        }

        /// <summary>
        /// 修改文章序号
        /// todo:后台权限
        /// </summary>
        public CommonActionResult UpdateArticleIndex(string indexDescription)
        {
            //// 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            var indexCollection = new Dictionary<string, int>();
            var items = indexDescription.Split('|');
            foreach (var item in items)
            {
                var tmp = item.Split(',');
                indexCollection.Add(tmp[0], int.Parse(tmp[1]));
            }
            var siteBiz = new SiteMessageBusiness();
            siteBiz.UpdateArticleIndex(indexCollection);
            return new CommonActionResult(true, "修改文章序号成功");
        }

        /// <summary>
        /// 提交文章
        /// todo:后台权限
        /// </summary>
        public CommonActionResult SubmitArticle(E_SiteMessage_Article_List article)
        {
            //// 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            var siteBiz = new SiteMessageBusiness();
            var articleId = siteBiz.SubmitArticle(article);
            //ClearArticleCollection();
            return new CommonActionResult(true, "提交文章成功") { ReturnValue = articleId };
        }
        /// <summary>
        /// 修改文章
        /// todo:后台权限
        /// </summary>
        public CommonActionResult UpdateArticle(E_SiteMessage_Article_List article)
        {
            //// 验证用户身份及权限
            //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);

            var siteBiz = new SiteMessageBusiness();
            siteBiz.UpdateArticle(article);
            //ClearArticleCollection();
            return new CommonActionResult(true, "修改文章成功");
        }
        #endregion

        #region 广告管理
        /// <summary>
        /// 删除广告
        /// </summary>
        public CommonActionResult DeleteBanner(int bannerId)
        {
            //// 验证用户身份及权限
            //var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new SiteMessageBusiness().DeleteBanner(bannerId);
                return new CommonActionResult(true, "删除广告信息成功！");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 前台查询广告信息
        /// </summary>
        /// <param name="bannerType"></param>
        /// <param name="returnRecord"></param>
        /// <returns></returns>
        public SiteMessageBannerInfo_Collection QuerySitemessageBanngerList_Web(int bannerType, int returnRecord = 10)
        {
            try
            {
                return new DataQuery().QuerySitemessageBanngerList_Web(bannerType, returnRecord);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// 后台查询广告
        /// </summary>
        public SiteMessageBannerInfo_Collection QuerySiteMessageBannerCollection(string title, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            try
            {
                return new SiteMessageBusiness().QuerySiteMessageBannerCollection(title, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 修改广告
        /// </summary>
        /// <param name="info"></param>
        /// <param name="userToken"></param>
        public CommonActionResult UpdateBannerInfo(E_Sitemessage_Banner info)
        {
            //// 验证用户身份及权限
            //var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new SiteMessageBusiness().UpdateBannerInfo(info);
                return new CommonActionResult(true, "修改广告信息成功！");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 新增广告
        /// </summary>
        public CommonActionResult AddBannerInfo(E_Sitemessage_Banner info)
        {
            //// 验证用户身份及权限
            //var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                new SiteMessageBusiness().AddBannerInfo(info);
                return new CommonActionResult(true, "新增广告信息成功！");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 财务管理
        public decimal QueryUserTotalFillMoney(string userId)
        {
            return new FundBusiness().QueryUserTotalFillMoney(userId);
        }

        public decimal QueryUserTotalWithdrawMoney(string userId)
        {
            return new FundBusiness().QueryUserTotalWithdrawMoney(userId);
        }

        /// <summary>
        /// 查询用户资金明细报表定制
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="keyLine"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="accountTypeList"></param>
        /// <param name="categoryList"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public UserFundDetailCollection QueryUserFundDetailListReport(string userId, string keyLine, DateTime fromDate, DateTime toDate, string accountTypeList, string categoryList, int pageIndex, int pageSize)
        {
            //// 验证用户身份及权限
            //var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
            try
            {
                return new SqlQueryBusiness().QueryUserFundDetailListReport(userId, keyLine, fromDate, toDate, accountTypeList, categoryList, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询用户资金明细报表出错 - " + ex.Message, ex);
            }
        }


        public UserFundDetailCollection QueryUserFundDetail_Commission(string userId, DateTime fromDate, DateTime toDate, int pageIndex, int pageSize)
        {
            try
            {
                return new SqlQueryBusiness().QueryUserFundDetail_Commission(userId, fromDate, toDate, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("查询用户资金明细出错 - " + ex.Message, ex);
            }
        }
        #endregion


        public FinanceSettingsInfo_Collection GetFinanceSettingsCollection(string userId, int pageIndex, int pageSize)
        {
            try
            {
                // 验证用户身份及权限
                //var myId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
                return new UserBusiness().GetFinanceSettingsCollection(userId, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string GetCaiWuOperator()
        {
            try
            {
                //// 验证用户身份及权限
                //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
                return new UserBusiness().GetCaiWuOperator();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public FinanceSettingsInfo GetFinanceSettingsByFinanceId(string FinanceId)
        {
            try
            {
                //// 验证用户身份及权限
                //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
                return new UserBusiness().GetFinanceSettingsByFinanceId(FinanceId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CommonActionResult FinanceSetting(string opeType, C_FinanceSettings info, string userId)
        {
            try
            {
                //// 验证用户身份及权限
                //var userId = GameBizAuthBusiness.ValidateUserAuthentication(userToken);
                new UserBusiness().FinanceSetting(opeType, info, userId);
                if (opeType.ToLower() != "delete")
                {
                    return new CommonActionResult(true, "保存数据成功！");
                }
                else
                {
                    return new CommonActionResult(true, "删除数据成功！");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 查询网站结余明细
        /// </summary>
        public UserBalanceHistoryInfoCollection QueryUserBalanceHistoryList(string userId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            try
            {
                return new UserBusiness().QueryUserBalanceHistoryList(userId, startTime, endTime, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        
        /// <summary>
        /// 获取第三方游戏的充值与提款列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ThirdPartyGameCollection ThirdPartyGameDetail(ThirdPartyGameListParam param)
        {
            try
            {
                return new DataQuery().ThirdPartyGameDetail(param);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public List<IndexReportForms> GetIndexReportForms()
        {
            try
            {
                var date = DateTime.Now.Date;
                //当天注册的人
                //var todayRegisterCount = new UserBalanceManager().QueryRegisterUserCount();
                var query = new DataQuery();
                return query.GetIndexReportForms();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 根据UserId获得他当前角色所有的权限
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<C_Auth_Function_List> GetMyAllFunciton(string userId)
        {
            try
            {
                var biz = new GameBizAuthBusiness();
                return biz.GetMyAllFunction(userId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
