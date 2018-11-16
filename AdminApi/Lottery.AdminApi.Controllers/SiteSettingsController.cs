using EntityModel;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.ORM.Helper;
using Lottery.AdminApi.Controllers.CommonFilterActtribute;
using Lottery.AdminApi.Model.HelpModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel.Enum;
using KaSon.FrameWork.Common.ExceptionEx;
using KaSon.FrameWork.Common.Utilities;
using EntityModel.ExceptionExtend;
using EntityModel.Communication;
using KaSon.FrameWork.Common.Net;
using EntityModel.CoreModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static KaSon.FrameWork.Common.Utilities.ConvertHelper;

namespace Lottery.AdminApi.Controllers
{
    /// <summary>
    /// 权限控制器
    /// </summary>
    [Area("api")]
    //[ReusltFilter]
    public class SiteSettingsController : BaseController
    {
        private readonly static AdminService _service = new AdminService();
        #region 新版权限列表
        /// <summary>
        /// 角色管理
        /// </summary>
        public  IActionResult RoleManage()
        {
            try
            {
                if (!CheckRights("X101"))
                    throw new Exception("对不起，您的权限不足！");
                var service = new AdminService();
                var list = service.GetSystemRoleCollection();
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = "查询成功",
                    Value = list
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage(),
                });
            }
        }

        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <returns></returns>
        public  IActionResult SetRoleFunction(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("X101"))
                {
                    throw new LogicException("对不起，您的权限不足！");
                }
                var p = JsonHelper.Decode(entity.Param);
                string roleId = p.roleId;
                var roleList = new RoleInfo_Query();
                var service = new AdminService();
                if (!string.IsNullOrEmpty(roleId))
                {
                    roleList = service.GetSystemRoleById(roleId);
                    ViewBag.RoleById = roleList;
                }
                var functionList = service.QueryLowerLevelFuncitonList();
                var result = new List<ShowRoleFunctionInfo>();
                foreach (var item in functionList)
                {
                    var model = roleList.FunctionList.FirstOrDefault(t => t.FunctionId == item.FunctionId);
                    var addmodel = new ShowRoleFunctionInfo()
                    {
                        DisplayName = item.DisplayName,
                        FunctionId = item.FunctionId,
                        ParentId = item.ParentId,
                        ParentPath = item.ParentPath,
                        SelectType = (int)AdminSelectType.NotSelect
                    };
                    if (model != null)
                    {
                        addmodel.SelectType = (int)AdminSelectType.Selecct;
                    }
                    if (item.FunctionId == "Q101")
                    {
                        addmodel.SelectType = (int)AdminSelectType.MustSelect;
                    }
                    result.Add(addmodel);
                }
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = "查询成功",
                    Value = result
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                });
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        public IActionResult AddRoleFunction(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("TJJS100"))
                {
                    throw new LogicException("对不起，您的权限不足！");
                }
                var p = JsonHelper.Decode(entity.Param);
                string RoleName = p.roleName;
                string RoleId = p.roleId;
                int? RoleType = p.roleType;
                bool? IsAdmin = p.isAdmin;
                string FunctionIds = p.functionIds;
                RoleInfo_Add roleInfo = new RoleInfo_Add();
                roleInfo.RoleName = PreconditionAssert.IsNotEmptyString(RoleName, "角色名不能为空");
                roleInfo.RoleId = PreconditionAssert.IsNotEmptyString(RoleId, "角色编号不能为空");
                roleInfo.RoleType = (RoleType)(PreconditionAssert.IsTrue(RoleType.HasValue, "请选择角色类型") ? RoleType.Value : 2);
                roleInfo.IsAdmin = IsAdmin.HasValue ? IsAdmin.Value : false;
                var service = new AdminService();
                var result = new CommonActionResult();
                if (roleInfo.IsAdmin)
                {
                    var funIds = service.QueryLowerLevelFuncitonList();
                    foreach (var item in funIds)
                    {
                        roleInfo.FunctionList.Add(new RoleFunctionInfo() { FunctionId = item.FunctionId, Mode = "RW" });
                    }
                    result = service.AddSystemRole(roleInfo, CurrentUser.UserToken);
                }
                else
                {
                    var funIds = PreconditionAssert.IsNotEmptyString(FunctionIds, "功能编号集异常").Split(',');
                    foreach (var item in funIds)
                    {
                        roleInfo.FunctionList.Add(new RoleFunctionInfo() { FunctionId = item, Mode = "RW" });
                    }
                    result = service.AddSystemRole(roleInfo, CurrentUser.UserToken);
                }
                service.AddSysOperationLog("", CurrentUser.UserId, "角色管理", "操作员【" + CurrentUser.LoginName + "】新增角色，角色名称:" + roleInfo.RoleName + "，角色编号:" + roleInfo.RoleId+";结果:"+(result.IsSuccess?"成功":"失败"));
                return Json(new LotteryServiceResponse
                {
                    Code = result.IsSuccess? AdminResponseCode.成功 : AdminResponseCode.失败,
                    Message = "新增成功"
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = "新增失败"
                });
            }
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public IActionResult UpdateRoleFunciton(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("TJJS100"))
                {
                    throw new LogicException("对不起，您的权限不足！");
                }
                var service = new AdminService();
                var p = JsonHelper.Decode(entity.Param);
                string RoleName = p.roleName;
                string RoleId = p.roleId;
                string FunctionIds = p.functionIds;
                var roleInfo = new RoleInfo_Update();
                roleInfo.RoleName = PreconditionAssert.IsNotEmptyString(RoleName, "角色名不能为空");
                roleInfo.RoleId = PreconditionAssert.IsNotEmptyString(RoleId, "角色编号不能为空");
                var roleById = service.GetSystemRoleById(RoleId);
                if (roleById.IsAdmin)
                {
                    var _result = service.UpdateSystemRole(roleInfo, CurrentUser.UserToken);
                    return Json(new LotteryServiceResponse
                    {
                        Code = _result.IsSuccess ? AdminResponseCode.成功 : AdminResponseCode.失败,
                        Message = "更新成功"
                    });
                }
                var funIds = PreconditionAssert.IsNotEmptyString(FunctionIds, "功能编号集异常").Split(',');
                foreach (var item in roleById.FunctionList)
                {
                    var currFun = funIds.FirstOrDefault(c => c == item.FunctionId);
                    if (currFun == null)
                        roleInfo.RemoveFunctionList.Add(new RoleFunctionInfo() { FunctionId = item.FunctionId });
                }
                foreach (var item in funIds)
                {

                    var fun = roleById.FunctionList.FirstOrDefault(c => c.FunctionId == item);
                    //if (fun != null)
                    //{
                    //    if (temp[1] != fun.Mode)
                    //    {
                    //        roleInfo.ModifyFunctionList.Add(new RoleFunctionInfo() { FunctionId = temp[0], Mode = temp[1] });
                    //    }
                    //}
                    if(fun==null)
                    {
                        roleInfo.AddFunctionList.Add(new RoleFunctionInfo() { FunctionId = item, Mode = "RW" });
                    }
                }
                var result = service.UpdateSystemRole(roleInfo, CurrentUser.UserToken);
                service.AddSysOperationLog("", CurrentUser.UserId, "角色管理", "操作员【" + CurrentUser.LoginName + "】修改角色，角色名称" + roleInfo.RoleName + "，角色编号" + roleById + ";结果:" + (result.IsSuccess ? "成功" : "失败"));
                return Json(new LotteryServiceResponse
                {
                    Code = result.IsSuccess ? AdminResponseCode.成功 : AdminResponseCode.失败,
                    Message = "更新成功"
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }

        #endregion

        #region 人员管理
        /// <summary>
        /// 人员列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IActionResult SysOperatorManage(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("X102"))
                    throw new Exception("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);
                string pageIndexStr = p.pageIndex;
                string pageSizeStr = p.pageSize;
                var service = new AdminService();
                //if (CheckRights("TJRY100"))
                //    tjry = true;
                //if (CheckRights("XGRY110"))
                //    xgry = true;
                int PageIndex = string.IsNullOrEmpty(pageIndexStr) ? base.PageIndex : Convert.ToInt32(pageIndexStr);
                int PageSize = string.IsNullOrEmpty(pageSizeStr) ? base.PageSize : Convert.ToInt32(pageSizeStr);
                var result = service.GetOpratorCollection(PageIndex, PageSize);
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = "查询成功",
                    Value = result
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage()
                });
            }
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <returns></returns>
        public  IActionResult SysOperatorInfo(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("X102"))
                    throw new Exception("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);
                string UserId = p.UserId;
                var service = new AdminService();
                var result = new List<ShowUserRole>();
                if (string.IsNullOrEmpty(UserId))
                {
                    var roleList = service.QueryRoleCollection();
                    var RoleIds = service.QueryUserRoleIdsByUserId(UserId);
                    var array = RoleIds.Split(new string[] { "%item%" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in roleList)
                    {
                        var newItem = new ShowUserRole()
                        {
                            IsAdmin = item.IsAdmin,
                            IsInner = item.IsInner,
                            ParentRoleId = item.ParentRoleId,
                            RoleName = item.RoleName,
                            RoleId = item.RoleId,
                            RoleType = item.RoleType,
                            SelectType = (int)AdminSelectType.NotSelect
                        };
                        var temp = array.FirstOrDefault(c => c == item.RoleId);
                        if (temp != null)
                        {
                            newItem.SelectType = (int)AdminSelectType.Selecct;
                        }
                        result.Add(newItem);
                    }
                }
                else
                {
                    var roleList = service.QueryRoleCollection();
                    if (roleList != null)
                    {
                        result = roleList.Select(c => new ShowUserRole()
                        {
                            IsAdmin = c.IsAdmin,
                            IsInner = c.IsInner,
                            ParentRoleId = c.ParentRoleId,
                            RoleId = c.RoleId,
                            RoleName = c.RoleName,
                            RoleType = c.RoleType,
                            SelectType = (int)AdminSelectType.NotSelect
                        }).ToList();
                    }
                }
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = "查询成功",
                    Value = result
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IActionResult AddOprator(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("TJRY100"))
                    throw new Exception("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);
                string roles = p.roles;
                string loginName = p.loginName;
                var roleResult = PreconditionAssert.IsNotEmptyString(roles, "用户角色不能为空");
                var array = roleResult.Split(new char[] { '|' }, StringSplitOptions.None).ToList();
                RegisterInfo_Admin regInfo = new RegisterInfo_Admin();
                regInfo.DisplayName = PreconditionAssert.IsNotEmptyString(loginName, "登录名不能为空");
                regInfo.LoginName = regInfo.DisplayName;
                regInfo.CreateTime = DateTime.Now;
                regInfo.RegisterIp = IpManager.GetClientUserIp(HttpContext);
                regInfo.RoleIdList = array;
                regInfo.RegType = "LOCAL";
                var service = new AdminService();
                var result = service.AddBackgroundAdminUser(regInfo);
                return Json(new LotteryServiceResponse {  Code =  AdminResponseCode.成功,  Message = result.Message + ",默认密码为123456" });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }

        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <returns></returns>
        public  IActionResult UpdateOprator(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("XGRY110"))
                    throw new Exception("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);
                string roles = p.roles;
                //string loginNameStr = p.loginName;
                string userId = p.userId;
                var roleResult = PreconditionAssert.IsNotEmptyString(roles, "用户角色不能为空");
                var array = roleResult.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                StringBuilder addRoleIdList = new StringBuilder();
                StringBuilder removeRoleIdList = new StringBuilder();
                //var loginName = PreconditionAssert.IsNotEmptyString(loginNameStr, "登录名不能为空");
                var service = new AdminService();
                var RoleIds = service.QueryUserRoleIdsByUserId(userId);
                if (!string.IsNullOrEmpty(RoleIds))
                {
                    var arrayRoleIds = RoleIds.Split(new string[] { "%item%" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in arrayRoleIds)
                    {
                        if (!array.Contains(item))
                        {
                            removeRoleIdList.Append(item);
                            removeRoleIdList.Append(",");
                        }
                    }
                }
                foreach (var item in array)
                {
                    var arrayRoleIds = RoleIds.Split(new string[] { "%item%" }, StringSplitOptions.RemoveEmptyEntries);
                    if (!arrayRoleIds.Contains(item))
                    {
                        addRoleIdList.Append(item);
                        addRoleIdList.Append(",");
                    }
                }
                var result = service.UpdateBackgroundUserInfo(userId, addRoleIdList.ToString(), removeRoleIdList.ToString());
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = "修改完成" });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }
        #endregion

        #region 插件相关

        /// <summary>
        /// 插件管理
        /// </summary>
        public IActionResult PluginClass(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("P102"))
                    throw new Exception("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);
                //bool sccj = false;
                //bool tjcj = false;
                //bool xgcj = false;
                //if (CheckRights("SCCJ110"))
                //    sccj = true;
                //if (CheckRights("TJCJ100"))
                //    tjcj = true;
                //if (CheckRights("XGCJ110"))
                //    xgcj = true;
                //ViewBag.sccj = sccj;
                //ViewBag.tjcj = tjcj;
                //ViewBag.xgcj = xgcj;
                string pageIndexStr = p.pageIndex;
                string pageSizeStr = p.pageSize;
                int PageIndex = string.IsNullOrEmpty(pageIndexStr) ? base.PageIndex : Convert.ToInt32(pageIndexStr);
                int PageSize = string.IsNullOrEmpty(pageSizeStr) ? base.PageSize : Convert.ToInt32(pageSizeStr);
                var service = new AdminService();
                var QueryPluginClassList = service.QueryPluginClassList(PageIndex, PageSize);
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = "查询成功", Value = QueryPluginClassList });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }

        /// <summary>
        /// 添加/修改插件(获取要修改的插件数据)
        /// </summary>
        public IActionResult AddPluginClass(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string idstr = p.id;
                if (string.IsNullOrEmpty(idstr))
                {
                    return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = "查询失败，请先传入需要查询的id" });
                }
                int id = int.Parse(idstr);
                var service = new AdminService();
                var model = service.PluginClassInfoById(id);
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = "查询成功", Value = model });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
            
        }

        /// <summary>
        /// 添加，修改插件
        /// </summary>
        public IActionResult PublishPluginInfo(LotteryServiceRequest entity)
        {
            try
            {

                C_Activity_PluginClass plugin = new C_Activity_PluginClass();
                var p = JsonHelper.Decode(entity.Param);
                string className = p.className;
                string interfaceName = p.interfaceName;
                string assemblyFileName = p.assemblyFileName;
                string isEnable = p.isEnable;
                string orderIndexstr = p.orderIndex;
                string startTime = p.startTime;
                string endTime = p.endTime;
                string idstr = p.id;
                plugin.ClassName = PreconditionAssert.IsNotEmptyString(className, "类名不能为空");
                plugin.InterfaceName = PreconditionAssert.IsNotEmptyString(interfaceName, "接口名不能为空");
                plugin.AssemblyFileName = PreconditionAssert.IsNotEmptyString(assemblyFileName, "组件文件名不能为空");
                plugin.IsEnable = bool.Parse(isEnable);
                var orderIndex = PreconditionAssert.IsNotEmptyString(orderIndexstr, "排序索引不能为空");
                plugin.OrderIndex = int.Parse(orderIndex);
                var service = new AdminService();
                if (!string.IsNullOrEmpty(startTime))
                {
                    plugin.StartTime = Convert.ToDateTime(startTime);
                }
                if (!string.IsNullOrEmpty(endTime))
                {
                    plugin.EndTime = Convert.ToDateTime(endTime);
                }

                if (string.IsNullOrEmpty(idstr))
                {
                    if (!CheckRights("TJCJ100"))
                        throw new Exception("对不起，您的权限不足！");
                    var noticeResult = service.AddPluginClass(plugin);
                    return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = "添加成功" });
                }
                else
                {
                    if (!CheckRights("XGCJ110"))
                        throw new Exception("对不起，您的权限不足！");
                    int id = int.Parse(idstr);
                    plugin.Id = id;
                    service.UpdatePluginClass(plugin);
                    return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = "修改成功"});
                }
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }


        /// <summary>
        /// 删除插件
        /// </summary>
        public IActionResult DeletePlugin(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("SCCJ110"))
                    throw new Exception("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);
                string id = p.id;
                if (string.IsNullOrEmpty(id))
                {
                    throw new Exception("删除对象为空");
                }
                var service = new AdminService();
                var noticeResult = service.DeletePluginClass(int.Parse(id));
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = "删除成功" });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }

        #endregion

        #region 网站配置

        public IActionResult WebSiteConfig(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string key = p.configKey;
                //bool gxpz = false;
                //if (CheckRights("GXPZ100"))
                //    gxpz = true;
                ////ViewBag.gxpz = gxpz;
                var service = new AdminService();
                if (string.IsNullOrEmpty(key))
                {
                    throw new Exception("传入的参数为空");
                }
                var WebSiteInfo = service.QueryCoreConfigByKey(key);
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = "查询成功",
                    Value = WebSiteInfo
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }
        /// <summary>
        /// 网站配置列表
        /// </summary>
        /// <returns></returns>
        public IActionResult WebSiteConfigList()
        {
            try
            {
                if (!CheckRights("P101"))
                    throw new Exception("对不起，您的权限不足！");
                //bool pzckxq = false;
                //if (CheckRights("PZCKXQ110"))
                //    pzckxq = true;
                //ViewBag.pzckxq = pzckxq;
                var service = new AdminService();
                var WebSiteInfoList = service.QueryAllCoreConfig();
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = "查询成功",
                    Value = WebSiteInfoList
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }
        public IActionResult UpdateWebSite(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("GXPZ100"))
                    throw new Exception("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);
                string id = p.id;
                string configName = p.configName;
                string configValue = p.configValue;
                id = PreconditionAssert.IsNotEmptyString(id, "配置编号不能为空！");
                configName = PreconditionAssert.IsNotEmptyString(configName, "名称不能为空！");
                configValue = PreconditionAssert.IsNotEmptyString(configValue, "值不能为空！");
                C_Core_Config info = new C_Core_Config
                {
                    Id = Convert.ToInt32(id),
                    ConfigName = configName,
                    ConfigValue = configValue
                };
                var service = new AdminService();
                var result = service.UpdateCoreConfigInfo(info);
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = "更新成功"
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }

        #endregion

        #region 修改充值接口

        public IActionResult QueryFillMoneyAPI()
        {
            try
            {
                if (!CheckRights("XGCZPZ"))
                    throw new Exception("对不起，您的权限不足！");
                //ViewBag.XGCZPZ_GX = CheckRights("XGCZPZ_GX");
                var service = new AdminService();
                var configList = service.QueryAllCoreConfig();
                var list = new List<C_Core_Config>();
                CoreConfigInfoCollection collection = new CoreConfigInfoCollection();
                if (configList != null && configList.Count > 0)
                {
                    list = configList.Where(s => s.ConfigKey == "FillMoney_Enable_GateWay"
                        || s.ConfigKey == "FillMoney.CallBackDomain"
                        || s.ConfigKey == "FillMoneyAgent.IPS_Url"
                        || s.ConfigKey == "FillMoneyAgent.AgentId"
                        || s.ConfigKey == "FillMoneyAgent.AgentKey"

                        || s.ConfigKey == "FillMoneyAgent.ZF_Url"
                        || s.ConfigKey == "FillMoneyAgent.ZF_Key"
                        || s.ConfigKey == "FillMoneyAgent.ZF_AgentId"

                        || s.ConfigKey == "IPS_FillMoney.JumpUrl"
                        || s.ConfigKey == "IPS_FillMoney.JumpUrl_WAP"
                        || s.ConfigKey == "ZF_FillMoney.JumpUrl"
                        || s.ConfigKey == "ZF_FillMoney.JumpUrl_WAP"

                        || s.ConfigKey == "FillMoneyAgent.HC_Url"
                        || s.ConfigKey == "FillMoneyAgent.HC_Key"
                        || s.ConfigKey == "FillMoneyAgent.HC_AgentId"
                        || s.ConfigKey == "HC_FillMoney.JumpUrl"
                        || s.ConfigKey == "HC_FillMoney.JumpUrl_WAP"


                        || s.ConfigKey == "HC_FillMoney.NoCard_Jump_Url"
                        || s.ConfigKey == "HC_FillMoney.NoCard_Jump_Url_WAP"
                        || s.ConfigKey == "FillMoneyAgent.HC_NoCard_Key"
                        || s.ConfigKey == "FillMoneyAgent.HC_NoCard_AgentId"

                        || s.ConfigKey == "FillMoneyAgent.WXPay_AppId"
                        || s.ConfigKey == "FillMoneyAgent.WXPay_Mchid"
                        || s.ConfigKey == "FillMoneyAgent.WXPay_Key"

                        || s.ConfigKey == "YS_FillMoney.JumpUrl"
                        || s.ConfigKey == "FillMoneyAgent.YSPay_Url"
                        || s.ConfigKey == "FillMoneyAgent.YSPay_Src"
                        || s.ConfigKey == "FillMoneyAgent.YSBusiCode"
                        || s.ConfigKey == "FillMoneyAgent.YSPay_PayeeName"
                        || s.ConfigKey == "FillMoneyAgent.YSPay_Pfxpassword"
                        || s.ConfigKey == "YS_FillMoney.JumpUrl_WAP"

                        || s.ConfigKey == "QCW_Alipay"
                        || s.ConfigKey == "QCW_Mobile_Alipay"

                         || s.ConfigKey == "WXYF_PAY_GATEWAYURL"
                        || s.ConfigKey == "WXYF_PAY_MERCHANTCODE"
                        || s.ConfigKey == "WXYF_PAY_MERCHANTKEY"
                        || s.ConfigKey == "WXYF_PAY_REQREFERER"
                         || s.ConfigKey == "WXYF_PAY_REQREFERER2"

                        || s.ConfigKey == "ZT_PAY_HashKey"
                        || s.ConfigKey == "ZT_PAY_REQREFERER"
                         || s.ConfigKey == "ZT_PAY_REQREFERER2"
                         || s.ConfigKey == "ZT_PAY_vendorid"
                         || s.ConfigKey == "ZT_PAY_GATEWAYURL"

                         || s.ConfigKey == "HW_PAY_Url"
                         || s.ConfigKey == "HW_PAY_merID"
                         || s.ConfigKey == "HW_PAY_terID"
                         || s.ConfigKey == "HW_PAY_serverPublicKey"
                         || s.ConfigKey == "HW_PAY_publicKey"
                         || s.ConfigKey == "HW_PAY_privateKey"
                         || s.ConfigKey == "HW_PAY_version"

                         || s.ConfigKey == "101ka_PAY_PostURL"
                         || s.ConfigKey == "101ka_PAY_URL"
                         || s.ConfigKey == "101ka_WAP_PAY_URL"
                         || s.ConfigKey == "101ka_PAY_MerId"
                         || s.ConfigKey == "101ka_PAY_Key"

                        ).ToList();
                    //collection.AddRange(list);
                    //ViewBag.ConfigList = collection;
                }
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = "查询成功",
                    Value= list
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }
        public IActionResult UpdateFillMoneyConfig(LotteryServiceRequest entity)
        {
            try
            {
                if(!CheckRights("XGCZPZ_GX"))
                    throw new Exception("对不起，您的权限不足！");
                var scuccessCount = 0;
                var p = JsonHelper.Decode(entity.Param);
                var s = new List<trytemp>();
                string updateListStr = p.updateList;
                var updateList = JsonHelper.Deserialize<List<trytemp>>(updateListStr);
                var service = new AdminService();
                foreach (dynamic item in updateList)
                {
                    var result = service.UpdateCoreConfigInfoByKey(item.key, item.value);
                    if (result.IsSuccess)
                        scuccessCount++;
                }
                var str = "成功更新数据:" + scuccessCount + "条";
                return Json(new LotteryServiceResponse
                {
                    Code = scuccessCount>0?AdminResponseCode.成功: AdminResponseCode.失败,
                    Message = str,
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }

        //public IActionResult UpdatePayRedBagConfig(LotteryServiceRequest entity)
        //{
        //    try
        //    {
        //        string value = Request["postDic"].ToString().TrimEnd(',');
        //        var result = GameClient.UpdateCoreConfigInfoByKey("PayRedBagConfig", value);
        //        if (result.IsSuccess)
        //            return Json(new { IsSuccess = true, Msg = "更新数据成功" });
        //        else
        //            return Json(new { IsSuccess = false, Msg = "更新数据失败" + result.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { IsSuccess = false, Msg = ex.Message });
        //    }
        //}
        #endregion

        #region 系统操作日志

        ///// <summary>
        ///// 添加后台操作日志
        ///// </summary>
        //public CommonActionResult AddSysOperationLog(string userId, string operUserId, string menuName, string desc)
        //{
        //    try
        //    {
        //        new SiteMessageBusiness().AddSysOperationLog(userId, operUserId, menuName, desc);
        //        return new CommonActionResult(true, "新增日志成功");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message, ex);
        //    }
        //}
        /// <summary>
        /// 查询后台操作日志
        /// </summary>
        public IActionResult QuerySysOperationList(LotteryServiceRequest entity)
        {
            try
            {
                //string menuName, string userId, string operUserId, DateTime startTime, DateTime endTimen, int pageIndex, int pageSize,
                var p = JsonHelper.Decode(entity.Param);
                string menuName = p.menuName;
                string userId = p.userId;
                string operUserId = p.operUserId;
                string startTimeStr = p.startTime;
                string endTimeStr = p.endTimen;
                string pageIndexStr = p.pageIndex;
                string pageSizeStr = p.pageSize;
                DateTime startTime = string.IsNullOrEmpty(startTimeStr) ? DateTime.Now.Date.AddDays(-7): Convert.ToDateTime(startTimeStr).Date.AddDays(1);
                DateTime endTime = string.IsNullOrEmpty(endTimeStr) ? DateTime.Now.Date.AddDays(1):Convert.ToDateTime(endTimeStr).Date.AddDays(1);
                int pageIndex = string.IsNullOrEmpty(pageIndexStr) ? base.PageIndex : Convert.ToInt32(pageIndexStr);
                int pageSize = string.IsNullOrEmpty(pageSizeStr) ? base.PageSize : Convert.ToInt32(pageSizeStr);
                var service = new AdminService();
                var result= service.QuerySysOperationList(menuName, userId, operUserId, startTime, endTime, pageIndex, pageSize);
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功 ,
                    Message = "查询成功",
                    Value= result
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }

        #endregion

        #region 公告更新
        /// <summary>
        /// 公告管理缓存
        /// </summary>
        public void BulletinInner()
        {
            var argList = new string[] { string.Format("arg={0}|{1}", "bulletin_home", 4), string.Format("arg={0}|{1}", "bulletin_list", 7) };
            var url = string.Format("{0}/{1}", ConfigHelper.AllConfigInfo["WebSiteUrl"], "Cache/Write");
            foreach (var arg in argList)
            {
                var result = PostManager.Post(url, arg, Encoding.Default);
                if (result != "1")
                {
                    throw new Exception("更新\"" + arg + "\"失败 -" + result);
                }
            }
        }
        #endregion

        /// <summary>
        /// 文章管理缓存
        /// </summary>
        /// <param name="gameCodes"></param>
        public void ArticleInner(params string[] gameCodes)
        {
            var gameCodeList = new List<string>(new string[] { "" });
            gameCodeList.AddRange(gameCodes);

            var argList = new List<string>(new string[] { string.Format("arg={0}|{1}", "hot_article_home", 10) });
            foreach (var gameCode in gameCodeList)
            {
                argList.Add(string.Format("arg={0}|{1}|{2}", "article_info", gameCode, 7));
                argList.Add(string.Format("arg={0}|{1}|{2}", "article_expert", gameCode, 4));
            }
            if (gameCodeList.Contains("SD11X5") || gameCodeList.Contains("GD11X5") || gameCodeList.Contains("JX11X5"))
            {
                argList.Add(string.Format("arg={0}|{1}|{2}", "article_info", "11X5", 10));
                argList.Add(string.Format("arg={0}|{1}|{2}", "article_expert", "11X5", 10));
            }
            if (gameCodeList.Contains("CQSSC") || gameCodeList.Contains("JXSSC"))
            {
                argList.Add(string.Format("arg={0}|{1}|{2}", "article_info", "SSC", 10));
                argList.Add(string.Format("arg={0}|{1}|{2}", "article_expert", "SSC", 10));
            }
            var url = string.Format("{0}/{1}", ConfigHelper.AllConfigInfo["WebSiteUrl"].ToString(), "Cache/Write");
            foreach (var arg in argList)
            {
                var result = PostManager.Post(url, arg, Encoding.UTF8);
                if (result != "1")
                {
                    throw new Exception("更新\"" + arg + "\"失败 -" + result);
                }
            }
        }

        #region 合买数据管理
       
        /// <summary>
        /// 更新用户战绩
        /// </summary>
        public JsonResult TogetherComputeUserBeedings(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var date = DateTime.Parse(PreconditionAssert.IsNotEmptyString((string)p.currentDate, "日期不能为空"));
                var result = _service.ComputeUserBeedings(date.ToString("yyyyMMdd"));
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }

        /// <summary>
        /// 更新用户幸运指数
        /// </summary>
        public JsonResult TogetherComputeLuckyUser(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var datefrom = DateTime.Parse(PreconditionAssert.IsNotEmptyString((string)p.startTime, "日期不能为空"));
                var dateto = DateTime.Parse(PreconditionAssert.IsNotEmptyString((string)p.endTime, "日期不能为空"));
                var result = _service.ComputeLucyUser();
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }
        /// <summary>
        /// 更新时间段中奖概率
        /// </summary>
        public JsonResult UpdateUserBonusRatio(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var datefrom = DateTime.Parse(PreconditionAssert.IsNotEmptyString((string)p.startTime, "日期不能为空"));
                var dateto = DateTime.Parse(PreconditionAssert.IsNotEmptyString((string)p.endTime, "日期不能为空"));
                var result = _service.ComputeBonusPercent();
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }
        #endregion
    }
}
