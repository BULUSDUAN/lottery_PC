//using EntityModel;
//using KaSon.FrameWork.Common;
//using KaSon.FrameWork.ORM.Helper.Admin;
//using Lottery.AdminApi.Controllers.CommonFilterActtribute;
//using Lottery.AdminApi.Model.HelpModel;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Lottery.AdminApi.Controllers
//{
//    /// <summary>
//    /// 权限控制器
//    /// </summary>
//    [Area("api")]
//    [ReusltFilter]
//    public class SiteSettingsController:BaseController
//    {
//        #region 新版权限列表
//        /// <summary>
//        /// 获取权限列表
//        /// </summary>
//        /// <returns></returns>
//        public IActionResult SetRoleFunction(LotteryServiceRequest entity)
//        {
//            try
//            {
//                var p = JsonHelper.Decode(entity.Param);
//                var roleId = p.roleId;
//                var roleList = new RoleInfo_Query();
//                var service = new AdminService();
//                if (!string.IsNullOrEmpty(roleId))
//                {
//                    roleList = roleId.GetSystemRoleById(roleId, CurrentUser.UserToken);
//                    ViewBag.RoleById = roleList;
//                }
//                var functionList = service.QueryLowerLevelFuncitonList();
//                ViewBag.AllFunctionList = functionList;
//                StringBuilder strHtml = new StringBuilder();
//                if (!string.IsNullOrWhiteSpace(Request["roleId"]))
//                {
//                    //<fieldset class=\"fielwd\" currFunctionId=" + functionId + " id=\"qx_" + functionId + "\"><legend id=\"le_gnd_name\">【" + $(this).find("a").text() + "】功能权限点</legend><div class=\"div_gnd_" + functionId + "\"><ul class=\"ul_" + functionId + "\"></ul></div><span funid=\"" + functionId + "\" class=\"close_\">移除</span></fieldset>
//                    var query = from f in functionList
//                                join r in roleList.FunctionList on f.FunctionId equals r.FunctionId
//                                where f.ParentId != "0"
//                                select new RoleFunctionInfo
//                                {
//                                    FunctionId = f.FunctionId == null ? string.Empty : f.FunctionId,
//                                    DisplayName = f.DisplayName == null ? string.Empty : f.DisplayName,
//                                    ParentId = f.ParentId == null ? string.Empty : f.ParentId,
//                                    Mode = r.Mode == null ? string.Empty : r.Mode,
//                                    ParentPath = f.ParentPath == null ? string.Empty : f.ParentPath,
//                                };//查询当前角色下的所有列表权限点
//                    if (query != null && query.Count() > 0)
//                    {
//                        foreach (var rf in query)
//                        {
//                            foreach (var _rf in query)
//                            {
//                                if (rf.FunctionId == _rf.ParentId || (rf.ParentId == rf.ParentPath && rf.ParentId != "0"))
//                                {
//                                    strHtml.Append("<fieldset class=\"fielwd\" currFunctionId=" + rf.FunctionId + " id=\"qx_" + rf.FunctionId + "\"><legend class=\"legend_\" id=\"le_gnd_name\">【" + rf.DisplayName + "】功能权限点</legend><span funid=\"" + rf.FunctionId + "\" class=\"close_\">移除</span><div class=\"div_gnd_" + rf.FunctionId + "\"><ul class=\"ul_" + rf.FunctionId + "\">");
//                                    strHtml.Append("<li><input type=\"checkbox\"  value=\"\"  class=\"chk_cd_" + rf.FunctionId + " cdclick\"  id=\"chk_cd\" parentId=" + rf.ParentPath + " functionId=" + rf.FunctionId + " function=" + rf.FunctionId + " parent=" + rf.ParentId + " checked=checked /><a class=\"a_gnd\" function=" + rf.FunctionId + " parentId=" + rf.ParentPath + ">" + rf.DisplayName + "</a><span class=\"sp_gnd_RW\"><input type=\"checkbox\"  value=\"\"  class=\"chk_gnd_R\" checked=checked />读<input type=\"checkbox\"  value=\"\"  class=\"chk_gnd_W\" checked=checked />写</span></li>");
//                                    var tempFunIdList = functionList.Where(s => s.ParentId == rf.FunctionId).ToList();//筛选当前列表下所有功能权限点
//                                    foreach (var tem in tempFunIdList)
//                                    {
//                                        var _currFun = query.FirstOrDefault(s => s.FunctionId == tem.FunctionId);//筛选当前角色是否包含当前列表下的功能点
//                                        if (_currFun != null)//如果包含则加载相应数据
//                                        {
//                                            var R = _currFun.Mode == "R" ? "checked=checked" : string.Empty;
//                                            var W = _currFun.Mode == "W" ? "checked=checked" : string.Empty;
//                                            var isRW = (_currFun.Mode == "RW" || _currFun.Mode == "WR");
//                                            if (isRW)
//                                            {
//                                                strHtml.Append("<li><input type=\"checkbox\"  value=\"\"  class=\"chk_gnd\" parentId=" + _currFun.ParentPath + " functionId=" + _currFun.FunctionId + " parent=" + _currFun.ParentId + " checked=checked /><a class=\"a_gnd\" function=" + _currFun.FunctionId + " parentId=" + _currFun.ParentPath + ">" + _currFun.DisplayName + "</a><span class=\"sp_gnd_RW\"><input type=\"checkbox\"  value=\"\"  class=\"chk_gnd_R\" checked=checked />读<input type=\"checkbox\"  value=\"\"  class=\"chk_gnd_W\" checked=checked />写</span></li>");
//                                            }
//                                            else
//                                            {
//                                                strHtml.Append("<li><input type=\"checkbox\"  value=\"\"  class=\"chk_gnd\" parentId=" + _currFun.ParentPath + " functionId=" + _currFun.FunctionId + "  parent=" + _currFun.ParentId + " checked=checked /><a class=\"a_gnd\" function=" + _currFun.FunctionId + " parentId=" + _currFun.ParentPath + " >" + _currFun.DisplayName + "</a><span class=\"sp_gnd_RW\"><input type=\"checkbox\"  value=\"\"  class=\"chk_gnd_R\" " + R + " />读<input type=\"checkbox\"  value=\"\"  class=\"chk_gnd_W\" " + W + " />写</span></li>");
//                                            }
//                                        }
//                                        else//显示不包含的权限点
//                                        {
//                                            strHtml.Append("<li><input type=\"checkbox\"  value=\"\"  class=\"chk_gnd\" parentId=" + tem.ParentPath + " functionId=" + tem.FunctionId + "  parent=" + tem.ParentId + " /><a class=\"a_gnd\" function=" + tem.FunctionId + " parentId=" + tem.ParentPath + "  parent=" + tem.ParentId + ">" + tem.DisplayName + "</a><span class=\"sp_gnd_RW\"><input type=\"checkbox\"  value=\"\"  class=\"chk_gnd_R\" />读<input type=\"checkbox\"  value=\"\"  class=\"chk_gnd_W\" />写</span></li>");
//                                        }
//                                    }
//                                    strHtml.Append("</ul></div></fieldset>");
//                                    break;
//                                }
//                            }
//                        }
//                    }
//                }
//                ViewBag.GNDHtml = strHtml.ToString();

//                return View();
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        public JsonResult GetLowerLevelFuncitonList()
//        {
//            try
//            {
//                var parentId = PreconditionAssert.IsNotEmptyString(Request["ParentId"], "上级编号不能为空");
//                var result = ExternalClient.QueryLowerLevelFuncitonByParentId(parentId);
//                RoleInfo_Query roleFunctionList = new RoleInfo_Query();
//                if (Request["RoleId"] != null && !string.IsNullOrEmpty(Request["RoleId"]))
//                    roleFunctionList = base.ExternalClient.GetSystemRoleById(Request["RoleId"], CurrentUser.UserToken);
//                StringBuilder strBud = new StringBuilder();
//                var funcInfo = ExternalClient.QueryCurrentFuncitonById(parentId);
//                strBud.Append("<li><input type=\"checkbox\"  value=\"\"    class=\"chk_cd_" + funcInfo.FunctionId + " cdclick\"  id=\"chk_cd\" parentId=" + funcInfo.ParentPath + " functionId=" + funcInfo.FunctionId + " function=" + funcInfo.FunctionId + "  parent=" + funcInfo.ParentId + " /><a class=\"a_gnd\" function=" + funcInfo.FunctionId + " parentId=" + funcInfo.ParentPath + " >" + funcInfo.DisplayName + "</a><span class=\"sp_gnd_RW\"><input type=\"checkbox\"  value=\"\"  class=\"chk_gnd_R\" />读<input type=\"checkbox\"  value=\"\"  class=\"chk_gnd_W\" />写</span></li>");
//                foreach (var item in result)
//                {
//                    if (roleFunctionList != null && roleFunctionList.FunctionList.Count > 0)
//                    {
//                        var currFun = roleFunctionList.FunctionList.FirstOrDefault(s => s.FunctionId == item.FunctionId);
//                        if (currFun != null)
//                        {
//                            var R = currFun.Mode == "R" ? "checked=checked" : string.Empty;
//                            var W = currFun.Mode == "W" ? "checked=checked" : string.Empty;
//                            var isRW = (currFun.Mode == "RW" || currFun.Mode == "WR");
//                            if (isRW)
//                            {
//                                strBud.Append("<li><input type=\"checkbox\"  value=\"\"  class=\"chk_gnd\" parentId=" + item.ParentPath + " functionId=" + item.FunctionId + "  parent=" + item.ParentId + " checked=checked /><a class=\"a_gnd\" function=" + item.FunctionId + " parentId=" + item.ParentPath + " >" + item.DisplayName + "</a><span class=\"sp_gnd_RW\"><input type=\"checkbox\"  value=\"\"  class=\"chk_gnd_R\" checked=checked />读<input type=\"checkbox\"  value=\"\"  class=\"chk_gnd_W\" checked=checked />写</span></li>");
//                            }
//                            else
//                            {
//                                strBud.Append("<li><input type=\"checkbox\"  value=\"\"  class=\"chk_gnd\" parentId=" + item.ParentPath + " functionId=" + item.FunctionId + "    parent=" + currFun.ParentId + " checked=checked /><a class=\"a_gnd\" function=" + item.FunctionId + " parentId=" + item.ParentPath + " >" + item.DisplayName + "</a><span class=\"sp_gnd_RW\"><input type=\"checkbox\"  value=\"\"  class=\"chk_gnd_R\" " + R + " />读<input type=\"checkbox\"  value=\"\"  class=\"chk_gnd_W\" " + W + " />写</span></li>");
//                            }
//                        }
//                        else
//                        {
//                            strBud.Append("<li><input type=\"checkbox\"  value=\"\"  class=\"chk_gnd\" parentId=" + item.ParentPath + " functionId=" + item.FunctionId + "   parent=" + item.ParentId + " /><a class=\"a_gnd\" function=" + item.FunctionId + " parentId=" + item.ParentPath + " >" + item.DisplayName + "</a><span class=\"sp_gnd_RW\"><input type=\"checkbox\"  value=\"\"  class=\"chk_gnd_R\" />读<input type=\"checkbox\"  value=\"\"  class=\"chk_gnd_W\" />写</span></li>");
//                        }
//                    }
//                    else
//                    {
//                        strBud.Append("<li><input type=\"checkbox\"  value=\"\"  class=\"chk_gnd\" parentId=" + item.ParentPath + " functionId=" + item.FunctionId + "   parent=" + item.ParentId + " /><a class=\"a_gnd\" function=" + item.FunctionId + " parentId=" + item.ParentPath + " >" + item.DisplayName + "</a><span class=\"sp_gnd_RW\"><input type=\"checkbox\"  value=\"\"  class=\"chk_gnd_R\" />读<input type=\"checkbox\"  value=\"\"  class=\"chk_gnd_W\" />写</span></li>");
//                    }
//                }
//                return Json(new { IsSuccess = true, Msg = strBud.ToString() });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message });
//            }
//        }

//        /// <summary>
//        /// 添加
//        /// </summary>
//        public JsonResult AddRoleFunction()
//        {
//            try
//            {
//                RoleInfo_Add roleInfo = new RoleInfo_Add();
//                roleInfo.RoleName = PreconditionAssert.IsNotEmptyString(Request["RoleName"], "角色名不能为空");
//                roleInfo.RoleId = PreconditionAssert.IsNotEmptyString(Request["RoleId"], "角色编号不能为空");
//                roleInfo.RoleType = (RoleType)Convert.ToInt32(PreconditionAssert.IsNotEmptyString(Request["RoleType"], "角色类型不能为空"));
//                roleInfo.IsAdmin = string.IsNullOrWhiteSpace(Request["IsAdmin"]) ? false : Convert.ToBoolean(Request["IsAdmin"]);
//                if (roleInfo.IsAdmin)
//                {
//                    var funIds = ExternalClient.QueryLowerLevelFuncitonList();
//                    foreach (var item in funIds)
//                    {
//                        roleInfo.FunctionList.Add(new RoleFunctionInfo() { FunctionId = item.FunctionId, Mode = "RW" });
//                    }
//                    var result = base.ExternalClient.AddSystemRole(roleInfo, CurrentUser.UserToken);
//                    return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
//                }
//                else
//                {
//                    var funIds = PreconditionAssert.IsNotEmptyString(Request["FunctionIds"], "功能编号集异常").Split(',');
//                    foreach (var item in funIds)
//                    {
//                        string[] temp = item.Split('|');
//                        if (temp.Length >= 3 && Convert.ToBoolean(temp[2]))
//                            roleInfo.FunctionList.Add(new RoleFunctionInfo() { FunctionId = temp[0], Mode = temp[1] });
//                    }
//                    var result = base.ExternalClient.AddSystemRole(roleInfo, CurrentUser.UserToken);
//                    return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
//                }
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message });
//            }
//        }
//        public JsonResult UpdateRoleFunciton()
//        {
//            try
//            {
//                var roleInfo = new RoleInfo_Update();
//                roleInfo.RoleName = PreconditionAssert.IsNotEmptyString(Request["RoleName"], "角色名不能为空");
//                roleInfo.RoleId = PreconditionAssert.IsNotEmptyString(Request["RoleId"], "角色编号不能为空");
//                var roleById = base.ExternalClient.GetSystemRoleById(Request["RoleId"], CurrentUser.UserToken);
//                var IsAdmin = string.IsNullOrWhiteSpace(Request["IsAdmin"]) ? false : Convert.ToBoolean(Request["IsAdmin"]);
//                if (IsAdmin)
//                {
//                    var _result = base.ExternalClient.UpdateSystemRole(roleInfo, CurrentUser.UserToken);
//                    return Json(new { IsSuccess = _result.IsSuccess, Msg = _result.Message });
//                }
//                var funIds = PreconditionAssert.IsNotEmptyString(Request["FunctionIds"], "功能编号集异常").Split(',');

//                //foreach (var item in roleById.FunctionList)
//                //{
//                //    if (funIds.FirstOrDefault(p => p.Split('|')[0] == item.FunctionId) == null)
//                //    {
//                //        roleInfo.RemoveFunctionList.Add(new RoleFunctionInfo() { FunctionId = item.FunctionId });
//                //    }
//                //}

//                //foreach (var item in funIds)
//                //{
//                //    var array = item.Split('|');
//                //    if (array != null && array.Length >= 3)
//                //    {
//                //        var currRole = roleById.FunctionList.FirstOrDefault(s => s.FunctionId == array[0]);
//                //        if (currRole != null && !Convert.ToBoolean(array[2]))
//                //            roleInfo.RemoveFunctionList.Add(new RoleFunctionInfo() { FunctionId = array[0] });
//                //    }
//                //}

//                foreach (var item in roleById.FunctionList)
//                {
//                    var currFun = funIds.FirstOrDefault(p => p.Split('|')[0] == item.FunctionId);
//                    if (currFun == null)
//                        roleInfo.RemoveFunctionList.Add(new RoleFunctionInfo() { FunctionId = item.FunctionId });
//                }
//                foreach (var item in funIds)
//                {
//                    string[] temp = item.Split('|');
//                    if (temp != null && Convert.ToBoolean(temp[2]))
//                    {
//                        var fun = roleById.FunctionList.FirstOrDefault(p => p.FunctionId == temp[0]);
//                        if (fun != null)
//                        {
//                            if (temp[1] != fun.Mode)
//                            {
//                                roleInfo.ModifyFunctionList.Add(new RoleFunctionInfo() { FunctionId = temp[0], Mode = temp[1] });
//                            }
//                        }
//                        else
//                        {
//                            roleInfo.AddFunctionList.Add(new RoleFunctionInfo() { FunctionId = temp[0], Mode = temp[1] });
//                        }
//                    }
//                }
//                var result = base.ExternalClient.UpdateSystemRole(roleInfo, CurrentUser.UserToken);

//                ExternalClient.AddSysOperationLog("", CurrentUser.UserId, "角色管理", "操作员【" + CurrentUser.LoginName + "】修改角色，角色名称" + roleInfo.RoleName + "，角色编号" + roleById);

//                return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message });
//            }
//        }

//        #endregion
//    }
//}
