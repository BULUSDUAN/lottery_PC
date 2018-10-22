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

namespace Lottery.AdminApi.Controllers
{
    /// <summary>
    /// 权限控制器
    /// </summary>
    [Area("api")]
    [ReusltFilter]
    public class SiteSettingsController : BaseController
    {
        #region 新版权限列表
        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <returns></returns>
        public IActionResult SetRoleFunction(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("X101"))
                {
                    throw new LogicException("对不起，您的权限不足！");
                }
                var p = JsonHelper.Decode(entity.Param);
                var roleId = p.roleId;
                var roleList = new RoleInfo_Query();
                var service = new AdminService();
                if (!string.IsNullOrEmpty(roleId))
                {
                    roleList = service.GetSystemRoleById(roleId, CurrentUser.UserToken);
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
                        IsSelect = false
                    };
                    if (model != null)
                    {
                        addmodel.IsSelect = true;
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
                    Code = AdminResponseCode.成功,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                });
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        public JsonResult AddRoleFunction(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("TJJS100"))
                {
                    throw new LogicException("对不起，您的权限不足！");
                }
                var p = JsonHelper.Decode(entity.Param);
                string RoleName = p.RoleName;
                string RoleId = p.RoleId;
                int? RoleType = p.RoleType;
                bool? IsAdmin = p.IsAdmin;
                string FunctionIds = p.FunctionIds;
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
                return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public JsonResult UpdateRoleFunciton(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("TJJS100"))
                {
                    throw new LogicException("对不起，您的权限不足！");
                }
                var service = new AdminService();
                var p = JsonHelper.Decode(entity.Param);
                string RoleName = p.RoleName;
                string RoleId = p.RoleId;
                string FunctionIds = p.FunctionIds;
                var roleInfo = new RoleInfo_Update();
                roleInfo.RoleName = PreconditionAssert.IsNotEmptyString(RoleName, "角色名不能为空");
                roleInfo.RoleId = PreconditionAssert.IsNotEmptyString(RoleId, "角色编号不能为空");
                var roleById = service.GetSystemRoleById(RoleId, CurrentUser.UserToken);
                if (roleById.IsAdmin)
                {
                    var _result = service.UpdateSystemRole(roleInfo, CurrentUser.UserToken);
                    return Json(new { IsSuccess = _result.IsSuccess, Msg = _result.Message });
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
                return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }

        #endregion
    }
}
