using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Enum;
using EntityModel.ExceptionExtend;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.NPOI;
using KaSon.FrameWork.Common.Utilities;
using KaSon.FrameWork.Common.Xml;
using KaSon.FrameWork.ORM.Helper;
using Lottery.AdminApi.Controllers.CommonFilterActtribute;
using Lottery.AdminApi.Model.HelpModel;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lottery.AdminApi.Controllers
{
    /// <summary>
    /// 会员管理
    /// </summary>
    [Area("api")]
    public class UserManageController :BaseController
    {
        private readonly static AdminService _service = new AdminService();
        private readonly static string GiveMoneyManager = "ADMIN|LAOGAN";
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IActionResult MemberManage(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("U101"))
                {
                    throw new LogicException("对不起，您的权限不足！");
                }
                var p = JsonHelper.Decode(entity.Param);
                var KeyType = (string)p.keyType ;
                var KeyValue = (string)p.keyValue ;

                DateTime StartTime = string.IsNullOrWhiteSpace((string)p.startTime) ? DateTime.Now.AddYears(-1) : Convert.ToDateTime((string)p.startTime);
                DateTime EndTime = string.IsNullOrWhiteSpace((string)p.endTime) ? DateTime.Now : Convert.ToDateTime((string)p.endTime);

                bool? isEnable = null;
                if (!string.IsNullOrWhiteSpace((string)p.isEnable ))
                { isEnable = bool.Parse((string)p.isEnable ); }
                var IsEnable = isEnable;

                bool? isFillMoney = null;
                if (!string.IsNullOrWhiteSpace((string)p.isFillMoney ))
                { isFillMoney = bool.Parse((string)p.isFillMoney ); }
                var IsFillMoney = isFillMoney;

                #region 2018-1-23
                bool? IsUserType = null;//是否内部员工
                if (!string.IsNullOrWhiteSpace((string)p.userType ))
                { IsUserType = bool.Parse((string)p.userType ); }
                #endregion

                bool? isAgent = null;
                if (!string.IsNullOrWhiteSpace((string)p.isAgent ))
                { isAgent = bool.Parse((string)p.isAgent ); }

                var AgentId = (string)p.agentId ;
                var PageIndex = string.IsNullOrWhiteSpace((string)p.pageIndex ) ? base.PageIndex : int.Parse((string)p.pageIndex );
                var PageSize = string.IsNullOrWhiteSpace((string)p.pageSize ) ? base.PageSize : int.Parse((string)p.pageSize );

                var OrderBy = string.IsNullOrWhiteSpace((string)p.hid_OrderBy ) ? string.Empty : p.hid_OrderBy .ToString();
                var StrDesc = string.IsNullOrWhiteSpace((string)p.hid_desc ) ? string.Empty : p.hid_desc .ToString();
                var UserCreditType = string.IsNullOrWhiteSpace((string)p.UserCreditType ) ? -1 : int.Parse(p.UserCreditType .ToString());
                var UsersSummary = _service.QueryUserList(StartTime, EndTime, KeyType, KeyValue, isEnable, isFillMoney, IsUserType, isAgent,
                    "", "", "", "", "", AgentId, PageIndex, PageSize, CurrentUser.UserToken, OrderBy, UserCreditType);
                return Json(new LotteryServiceResponse() {Code=AdminResponseCode.成功,Value=UsersSummary });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }

        /// <summary>
        /// 会员概况
        /// </summary>
        public ActionResult UserSummary(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string UserId = p.id;
                //if (!string.IsNullOrEmpty(ViewBag.UserId))
                //{
                   var UserResult = _service.QueryUserByKey(UserId);

                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = UserResult });
                //}
             
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        /// <summary>
        /// 批量禁用用户
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IActionResult BatchDisableUser(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("U102"))
                {
                    throw new LogicException("对不起，您的权限不足！");
                }
                var p = JsonHelper.Decode(entity.Param);
                var userIds = PreconditionAssert.IsNotEmptyString((string)p.UserIds, "用户编号不能为空");
                var arrUserId = userIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                int successCount = 0;
                foreach (var item in arrUserId)
                {
                    var result = _service.DisableUser(item, CurrentUser.UserToken);
                    if (result.IsSuccess)
                        successCount++;
                }
                string strMsg = string.Format("总共{0}条，成功{0}条", arrUserId.Length, successCount);
                return Json(new LotteryServiceResponse{ Code = AdminResponseCode.成功, Message = strMsg });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse{ Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        /// <summary>
        /// 批量启用用户
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IActionResult BatchEnableUser(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("U102"))
                {
                    throw new LogicException("对不起，您的权限不足！");
                }
                var p = JsonHelper.Decode(entity.Param);
                var userIds = PreconditionAssert.IsNotEmptyString((string)p.UserIds, "用户编号不能为空");
                var arrUserId = userIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                int successCount = 0;
                foreach (var item in arrUserId)
                {
                    var result = _service.EnableUser(item, CurrentUser.UserToken);
                    if (result.IsSuccess)
                        successCount++;
                }
                string strMsg = string.Format("总共{0}条，成功{0}条", arrUserId.Length, successCount);
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = strMsg });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        /// <summary>
        /// 批量设置内部人员
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IActionResult BatchSetInnerUser(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("U102"))
                {
                    throw new LogicException("对不起，您的权限不足！");
                }
                var p = JsonHelper.Decode(entity.Param);
                var userIds = PreconditionAssert.IsNotEmptyString((string)p.UserIds, "用户编号不能为空");
                var result = _service.BatchSetInnerUser(userIds);
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        /// <summary>
        /// 获取指定时长未登陆过的用户
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IActionResult GetNotOnlineRecently(LotteryServiceRequest entity)
        {
            try
            {
                if (!GiveMoneyManager.Split('|',StringSplitOptions.RemoveEmptyEntries).Contains(CurrentUser.DisplayName.ToUpper()))
                {
                    throw new LogicException("对不起，您的权限不足！");
                }
                var p = JsonHelper.Decode(entity.Param);
                int Days = string.IsNullOrWhiteSpace((string)p.days) ? 14 : int.Parse((string)p.days);
                int PageIndex = string.IsNullOrWhiteSpace((string)p.pageIndex) ? base.PageIndex : int.Parse((string)p.pageIndex);
                int PageSize = string.IsNullOrWhiteSpace((string)p.pageSize) ? base.PageSize : int.Parse((string)p.pageSize);
                var NotOnlineRecentlyList =_service.QueryNotOnlineRecentlyList(Days,PageIndex,PageSize);
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Value = NotOnlineRecentlyList });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        /// <summary>
        /// 赠送红包给指定保留用户
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IActionResult GiveMoneyToStayUser(LotteryServiceRequest entity)
        {
            try
            {
                if (!GiveMoneyManager.Split('|', StringSplitOptions.RemoveEmptyEntries).Contains(CurrentUser.DisplayName.ToUpper()))
                {
                    return Json(new LotteryServiceResponse()
                    {
                        Code = AdminResponseCode.失败,
                        Message = "当前用户不具有相关权限"
                    });
                }
                var p = JsonHelper.Decode(entity.Param);
                var UserIds = (string)p.UserIds;
                if (string.IsNullOrEmpty(UserIds))
                {
                    return Json(new LotteryServiceResponse()
                    {
                        Code = AdminResponseCode.失败,
                        Message = "用户编号不能为空"
                    });
                }
                var uList = UserIds.Split(',');
                if (uList.Count() == 1)
                {
                    var result = _service.GiveMoneyToStayUser(uList[0], this.CurrentUser.UserId);
                    return Json(result);
                }
                else
                {
                    var str = new StringBuilder();
                    foreach (var item in uList)
                    {
                        var result = _service.GiveMoneyToStayUser(item, this.CurrentUser.UserId);
                        if (!string.IsNullOrWhiteSpace(result))
                        {
                            str.AppendLine("用户UserId:" + item + "赠送成功;" + result);
                        }
                        else str.AppendLine("用户UserId:" + item + "赠送失败");
                    }
                    return Json(new LotteryServiceResponse()
                    {
                        Code = AdminResponseCode.成功,
                        Message = str.ToString()
                    });
                }
            }
            catch (LogicException ex)
            {
                return JsonEx(new LotteryServiceResponse()
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToString()
                });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse()
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToString()
                });
            }
        }
        /// <summary>
        /// 内部私有方法
        /// </summary>
        private Dictionary<int, string> EarningsList
        {
            get
            {
                return new Dictionary<int, string>()
                {
                    {0,"0" },
                    {1,"0|100" },
                    {2,"100|1000" },
                    {3,"1000|5000" },
                    {4,"5000|10000" },
                    {5,"10000|20000" },
                    {6,"20000" },
                    {7,"-0|-100" },
                    {8,"-100|-1000" },
                    {9,"-1000|-5000" },
                    {10,"-5000|-10000" },
                    {11,"-10000|-20000" },
                    {12,"-20000" },
                };
            }
        }
        /// <summary>
        /// 批量赠送红包给所有指定时间的保留用户
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IActionResult GiveMoneyToTotalStayUser(LotteryServiceRequest entity)
        {
            //1.每次查询1000条数据，查询完后循环发钱，发完后继续查询，直到没有为止
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var Flag = GiveMoneyManager.Split('|').Contains(CurrentUser.DisplayName.ToUpper());
                if (Flag == false)
                {
                    return JsonEx(new LotteryServiceResponse(){ Code = AdminResponseCode.失败, Message = "您没有权限操作" });
                }
                var Days = string.IsNullOrWhiteSpace((string)p.days) ? 14 : int.Parse((string)p.days);
                var Earnings = string.IsNullOrWhiteSpace((string)p.earnings) ? 0 : int.Parse((string)p.earnings);
                string theEarnings = EarningsList[Earnings];
                var GiveSuccessCount = 0;
                var GiveFalseCount = 0;
                var Index = 0;
                var PageSize = 1000;
                while (true)
                {
                    var list = _service.QueryNotOnlineRecentlyList(Days, Index, PageSize, theEarnings);
                    if (list == null || list.UserList == null || list.UserList.Count == 0)
                    {
                        break;
                    }
                    foreach (var item in list.UserList)
                    {
                        var result = _service.GiveMoneyToStayUser(item.UserId, this.CurrentUser.UserId);
                        if (!string.IsNullOrWhiteSpace(result)){
                            GiveSuccessCount++;
                        }
                        else
                        { GiveFalseCount++; }
                    }
                    Index++;
                }
                return Json(new LotteryServiceResponse()
                {
                    Code = AdminResponseCode.成功,
                    Message = string.Format("一共派发给{0}位用户，成功{1}位，失败{2}位", GiveSuccessCount + GiveFalseCount, GiveSuccessCount, GiveFalseCount)
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse()
                {
                    Code = AdminResponseCode.失败,
                    Message = string.Format("派发中途出错:" + ex.Message)
                });
            }
        }
        /// <summary>
        /// 到处指定时长的保留用户
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public FileResult ExportStayUserExcel(LotteryServiceRequest entity)
        {
            var p = JsonHelper.Decode(entity.Param);
            var Flag = GiveMoneyManager.Split('|').Contains(CurrentUser.DisplayName.ToUpper());
            if (Flag == false)
            {
                return null;
            }
            var Days = string.IsNullOrWhiteSpace((string)p.days) ? 14 : int.Parse((string)p.days);
            var Earnings = string.IsNullOrWhiteSpace((string)p.earnings) ? 0 : int.Parse((string)p.earnings);
            string theEarnings = EarningsList[Earnings];
            var list =_service.QueryExcelNotOnlineRecentlyList(Days, theEarnings);
            //创建Excel文件的对象
            NPOI.XSSF.UserModel.XSSFWorkbook book = new NPOI.XSSF.UserModel.XSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("用户编号");
            row1.CreateCell(1).SetCellValue("手机");
            row1.CreateCell(2).SetCellValue("登录名");
            row1.CreateCell(3).SetCellValue("真实姓名");
            row1.CreateCell(4).SetCellValue("总充值金额");
            row1.CreateCell(5).SetCellValue("总中奖金额	");
            row1.CreateCell(6).SetCellValue("总提现金额");
            row1.CreateCell(7).SetCellValue("用户充值余额");
            row1.CreateCell(8).SetCellValue("用户奖金余额");
            row1.CreateCell(9).SetCellValue("盈亏");
            //将数据逐步写入sheet1各个行
            int i = 0;
            foreach (var item in list.UserList)
            {
                i++;
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i);
                rowtemp.CreateCell(0).SetCellValue(item.UserId);
                rowtemp.CreateCell(1).SetCellValue(item.Mobile);
                rowtemp.CreateCell(2).SetCellValue(item.DisplayName);
                rowtemp.CreateCell(3).SetCellValue(item.RealName);
                rowtemp.CreateCell(4).SetCellValue(item.TotalFillMoney.ToString("N2"));
                rowtemp.CreateCell(5).SetCellValue(item.TotalBonusMoney.ToString("N2"));
                rowtemp.CreateCell(6).SetCellValue(item.TotalWithdraw.ToString("N2"));
                rowtemp.CreateCell(7).SetCellValue(item.FillMoneyBalance.ToString("N2"));
                rowtemp.CreateCell(8).SetCellValue(item.BonusBalance.ToString("N2"));
                rowtemp.CreateCell(9).SetCellValue(item.Earnings.ToString("N2"));
            }

            // 写入到客户端 
            OverWriteNPOI ms = new OverWriteNPOI
            {
                AllowClose = false
            };
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            ms.AllowClose = true;
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("{0}.xlsx", DateTime.Now.ToString("yyyy-MM-dd")));
            
        }
        /// <summary>
        /// 用户详情
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IActionResult MemberDetail(LotteryServiceRequest entity)
        {
            try
            {
                //if (!CheckRights("U102"))
                if (CheckRights("HYGLCKYHXQ110"))//查看会员详情
                    throw new Exception("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);
                UserViewEntity ViewModel = new UserViewEntity();
                string userId = string.Empty;
                if ((string)p.SearchType != null && (string)p.SearchType == "uName")
                {
                    if ((string)p.u_UserName != null)
                    {
                        userId = _service.GetUserIdByLoginName((string)p.u_UserName);
                    }
                }
                else
                {
                    userId = (string)p.userKey;
                }

                if (!string.IsNullOrWhiteSpace(userId))
                {
                    ViewModel.UserKey = userId;
                    ViewModel.UserName = (string)p.u_UserName;
                    UserQueryInfo userResult = _service.QueryUserByKey(userId);
                    ViewModel.UserResult = userResult;
                    ViewModel.HistoryLogin = _service.QueryCache_UserLoginHistoryCollectionByUserId(
                        userId, CurrentUser.UserToken);

                    try
                    {
                        ViewModel.Bank = _service.QueryBankCardByUserId(userId, CurrentUser.UserToken);
                    }
                    catch (Exception)
                    {
                        ViewModel.Bank = null;
                    }


                    var result = _service.GetUserTokenByKey(userId, CurrentUser.UserToken);
                    if (result.IsSuccess)
                    {
                        ViewModel.UserToken = result.ReturnValue;
                    }
                    else
                    {
                        ViewModel.UserToken = "";
                    }

                    if (userResult != null)
                    {
                        if (userResult.FreezeBalance > 0M)
                        {
                            ViewModel.FreezeList = _service.QueryUserBalanceFreezeListByUser(userResult.UserId, 0, 10, CurrentUser.UserToken);
                        }
                        if (_service.CheckIsAuthenticatedUserRealName(userResult.UserId, CurrentUser.UserToken))//等于true，已实名认证
                        {
                            ViewModel.RealNameInfo = _service.GetUserRealNameInfo(userResult.UserId, CurrentUser.UserToken);
                        }
                        if (_service.CheckIsAuthenticatedUserMobile(userResult.UserId, CurrentUser.UserToken))//等于true，已手机认证
                        {
                            ViewModel.MobileInfo = _service.GetUserMobileInfo(userResult.UserId, CurrentUser.UserToken);
                        }
                        if (_service.CheckIsAuthenticatedUserEmail(userResult.UserId, CurrentUser.UserToken))//等于true，邮箱认证
                        {
                            ViewModel.EmailInfo = _service.GetUserEmailInfo(userResult.UserId, CurrentUser.UserToken);
                        }
                    }
                    ViewModel.UserCreditType = userResult.UserCreditType;
                    ViewModel.ApliyCount = userResult.ApliyCount;
                    ViewModel.QQNumber = userResult.QQNumber;
                }
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = ViewModel });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <returns></returns>
        public IActionResult ResetUserPassword(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("CZDLMM100"))
                {
                    throw new LogicException("对不起，您的权限不足!");
                }
                var p = JsonHelper.Decode(entity.Param);
                var userId = PreconditionAssert.IsNotEmptyString((string)p.userId, "操作ID不正确");
                var result = _service.ResetUserPassword(userId, CurrentUser.UserToken);
                _service.AddSysOperationLog(userId, CurrentUser.UserId, "重置密码", string.Format("操作员【{0}】重置用户【{1}】的登录密码", CurrentUser.UserId, userId));

                return Json(new LotteryServiceResponse(){ Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse (){ Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        /// <summary>
        /// 重置资金密码
        /// </summary>
        /// <returns></returns>
        public IActionResult ResetBalancePassword(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("CZZJMM110"))
                {
                    throw new LogicException("对不起，您的权限不足!");
                }
                var p = JsonHelper.Decode(entity.Param);
                var userId = PreconditionAssert.IsNotEmptyString((string)p.userId, "操作ID不正确");
                var result = _service.ResetUserBalancePwd(userId, CurrentUser.UserToken);
                _service.AddSysOperationLog(userId, CurrentUser.UserId, "重置资金密码", string.Format("操作员【{0}】重置用户【{1}】的资金密码", CurrentUser.UserId, userId));
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        /// <summary>
        /// 禁用用户
        /// </summary>
        public IActionResult DisableUser(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("jyqyzt"))
                {
                    throw new LogicException("对不起，您的权限不足!");
                }
                var p = JsonHelper.Decode(entity.Param);
                var userId = PreconditionAssert.IsNotEmptyString((string)p.userId, "操作ID不正确");
                var result = _service.DisableUser(userId, CurrentUser.UserToken);
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        /// <summary>
        /// 修改VIP级别
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IActionResult UpdateUserVipLevel(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("XGVIPJB130"))
                {
                    throw new LogicException("对不起,您的权限不足!");
                }
                var p = JsonHelper.Decode(entity.Param);
                var userId = PreconditionAssert.IsNotEmptyString((string)p.userId, "用户编号不能为空");
                var vipLevel = PreconditionAssert.IsInt32((string)p.vipLevel, "输入VIP级别不能为空");
                var result = _service.UpdateUserVipLevel(userId, vipLevel, CurrentUser.UserToken);
                _service.AddSysOperationLog(userId, CurrentUser.UserId, "调整用户VIP级别", string.Format("操作员【{0}】修改了 用户【{1}】 的vip等级", CurrentUser.UserId, userId));
                return Json(new LotteryServiceResponse{ Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        /// <summary>
        /// 手工充值
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IActionResult ManualFillMoney(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("SGCZ140"))
                {
                    throw new LogicException("对不起,您的权限不足");
                }
                var p = JsonHelper.Decode(entity.Param);
                string userKey = PreconditionAssert.IsNotEmptyString((string)p.userKey, "未指定用户");
                UserFillMoneyAddInfo info = new UserFillMoneyAddInfo();
                info.GoodsName = "手工充值";
                info.FillMoneyAgent = FillMoneyAgentType.ManualFill;
                info.RequestMoney = Convert.ToDecimal(PreconditionAssert.IsNotEmptyString((string)p.requestMoney, "充值金额不能为空"));
                info.GoodsDescription = PreconditionAssert.IsNotEmptyString((string)p.remark, "描述不能为空");

                var fillResult = _service.ManualFillMoney(info, userKey, CurrentUser.UserToken);

                _service.AddSysOperationLog(userKey, CurrentUser.UserId, "手工充值", string.Format("操作员【{0}】给用户【{1}】手工充值【{2}】元", this.CurrentUser.UserId, userKey, (string)p.requestMoney));

                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = fillResult.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        /// <summary>
        /// 手工打款
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IActionResult ManualAdd(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("SGDK150"))
                {
                    throw new LogicException("对不起,您的权限不足!");
                }
                var p = JsonHelper.Decode(entity.Param);
                string userKey = PreconditionAssert.IsNotEmptyString((string)p.userKey, "未指定用户");
                AccountType accountType = (AccountType)Enum.Parse(typeof(AccountType), PreconditionAssert.IsNotEmptyString((string)p.accountType, "账户不能为空"));
                decimal money = Convert.ToDecimal(PreconditionAssert.IsNotEmptyString((string)p.requestMoney, "打款金额不能为空"));
                string remark = PreconditionAssert.IsNotEmptyString((string)p.remark, "描述不能为空");
                var result = _service.ManualAddMoney("", money, accountType, userKey, remark, CurrentUser.UserToken);
                _service.AddSysOperationLog(userKey, CurrentUser.UserId, "手工打款", string.Format("操作员【{0}】对用户【{1}】打款【{2}】元", CurrentUser.UserId, userKey, money));
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        /// <summary>
        /// 手工扣款
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IActionResult ManualDeduct(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("SGKK160"))
                {
                    throw new LogicException("对不起,您的权限不足!");
                }
                var p = JsonHelper.Decode(entity.Param);
                string userKey = PreconditionAssert.IsNotEmptyString((string)p.userKey, "未指定用户");
                AccountType accountType = (AccountType)Enum.Parse(typeof(AccountType), PreconditionAssert.IsNotEmptyString((string)p.accountType, "账户不能为空"));
                decimal money = Convert.ToDecimal(PreconditionAssert.IsNotEmptyString((string)p.requestMoney, "扣款金额不能为空"));
                string remark = PreconditionAssert.IsNotEmptyString((string)p.remark, "描述不能为空");
                var result = _service.ManualDeductMoney("", money, accountType, userKey, remark, CurrentUser.UserToken);
                _service.AddSysOperationLog(userKey, CurrentUser.UserId, "手工扣款", string.Format("操作员【{0}】对用户【{1}】扣款【{2}】元", CurrentUser.UserId, userKey, money));
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        /// <summary>
        /// 实名认证
        /// </summary>
        /// <returns></returns>
        public IActionResult UpdateUserRealName(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("SGKK160"))
                {
                    throw new LogicException("对不起,您的权限不足!");
                }
                var p = JsonHelper.Decode(entity.Param);
                var userId = PreconditionAssert.IsNotEmptyString((string)p.userId, "用户编号不能为空");
                var userName = PreconditionAssert.IsNotEmptyString((string)p.realName, "真实姓名不能为空");
                var realID = PreconditionAssert.IsNotEmptyString((string)p.idCardNumber, "身份证不能为空");
                PreconditionAssert.IsTrue(ValidateHelper.CheckIdCard(realID), "请输入正确的身份证号码。");
                PreconditionAssert.IsFalse(SensitiveAnalyzer.IsHaveSensitive(userName), "真实姓名不允许包含敏感词，如有疑问请联系客服。");
                var result = _service.UpdateRealNameAuthentication(userId, userName, realID, CurrentUser.UserToken);
                _service.AddSysOperationLog(userId, CurrentUser.UserId, "更新实名认证", string.Format("操作员【{0}】更新用户【{1}】实名认证，真实姓名【{2}】，身份证号【{3}】", CurrentUser.UserId, userId, userName, realID));
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public IActionResult CancelUserRealName(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("SGKK160"))
                {
                    throw new LogicException("对不起,您的权限不足!");
                }
                var p = JsonHelper.Decode(entity.Param);
                var userId = PreconditionAssert.IsNotEmptyString((string)p.userId, "用户编号不能为空");
                var result = _service.LogOffRealNameAuthen(userId, CurrentUser.UserToken);
                _service.AddSysOperationLog(userId, CurrentUser.UserId, "解除实名认证", string.Format("操作员【{0}】解除用户【{1}】实名认证", CurrentUser.UserId, userId));
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public IActionResult AuthenticateUserRealName(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var userId = PreconditionAssert.IsNotEmptyString((string)p.userId, "操作ID不正确");
                var userRealNameInfo = new UserRealNameInfo
                {
                    AuthFrom = "LOCAL",
                    CardType = "0",
                    RealName = PreconditionAssert.IsNotEmptyString((string)p.realName, "真实姓名不能为空"),
                    IdCardNumber = PreconditionAssert.IsNotEmptyString((string)p.idCardNumber, "身份证不能为空")
                };
                PreconditionAssert.IsTrue(ValidateHelper.CheckIdCard(userRealNameInfo.IdCardNumber), "请输入正确的身份证号码。");
                var result = _service.AuthenticateUserRealName_BackSite(userId, userRealNameInfo.RealName, userRealNameInfo.IdCardNumber, CurrentUser.UserToken);
                _service.AddSysOperationLog(userId, CurrentUser.UserId, "实名认证", string.Format("操作员【{0}】实名认证用户【{1}】，真实姓名【{2}】，身份证号【{3}】", CurrentUser.UserId, userId, (string)p.realName, (string)p.idCardNumber));

                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = "操作成功" });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = "操作失败-" + ex.Message });
            }
        }
        /// <summary>
        /// 手机认证
        /// </summary>
        /// <returns></returns>
        public IActionResult UpdateMobile(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("GXSJRZ210"))
                {
                    throw new LogicException("对不起,您的权限不足");
                }
                var p = JsonHelper.Decode(entity.Param);
                var userId = PreconditionAssert.IsNotEmptyString((string)p.userId, "用户编号不能为空");
                var mobile = PreconditionAssert.IsNotEmptyString((string)p.mobile, "手机号码不能为空");
                PreconditionAssert.IsTrue(ValidateHelper.IsMobile(mobile), "手机号码格式错误。");
                var result = _service.UpdateMobileAuthen(userId, mobile, CurrentUser.UserToken);
                _service.AddSysOperationLog(userId, CurrentUser.UserId, "手机认证", string.Format("操作员【{0}】对用户【{1}】手机认证，手机号【{2}】", CurrentUser.UserId, userId, mobile));
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public IActionResult CancelMobile(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("GXSJRZ210"))
                {
                    throw new LogicException("对不起,您的权限不足");
                }
                var p = JsonHelper.Decode(entity.Param);
                var userId = PreconditionAssert.IsNotEmptyString((string)p.userId, "用户编号不能为空");
                var result = _service.LogOffMobileAuthen(userId, CurrentUser.UserToken);
                _service.AddSysOperationLog(userId, CurrentUser.UserId, "取消手机认证", string.Format("操作员【{0}】对用户【{1}】取消手机认证", CurrentUser.UserId, userId));
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public IActionResult AuthenticationUserMobile(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var userId = PreconditionAssert.IsNotEmptyString((string)p.userId, "操作ID不正确");
                var mobile = PreconditionAssert.IsNotEmptyString((string)p.mobile, "手机号不能为空");
                PreconditionAssert.IsTrue(ValidateHelper.IsMobile(mobile), "手机号码格式错误。");
                var result = _service.AuthenticationUserMobile(userId, mobile, SchemeSource.Web, CurrentUser.UserToken);
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = "操作成功" });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = "操作失败-" + ex.Message });
            }
        }
        /// <summary>
        /// 银行卡绑定
        /// </summary>
        /// <returns></returns>
        public IActionResult UpdateBank(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("GXYHK230"))
                {
                    throw new LogicException("对不起,您的权限不足");
                }
                var p = JsonHelper.Decode(entity.Param);
                var userId = PreconditionAssert.IsNotEmptyString((string)p.userId, "用户编号不能为空");
                var userBankCardNumber = PreconditionAssert.IsNotEmptyString((string)p.userBankCardNumber, "银行卡号不能为空");
                var userBankCode = PreconditionAssert.IsNotEmptyString((string)p.userBankCode, "银行编码不能为空");
                var userBankName = PreconditionAssert.IsNotEmptyString((string)p.userBankName, "银行名称不能为空");
                var userProvinceName = PreconditionAssert.IsNotEmptyString((string)p.userProvinceName, "省不能为空");
                var userCityName = PreconditionAssert.IsNotEmptyString((string)p.userCityName, "城市不能为空");
                var userTrueName = PreconditionAssert.IsNotEmptyString((string)p.userTrueName, "真实姓名不能为空");
                var userBankSubName = userBankName;
                if (!new string[] { "CMB", "ICBC", "CCB", "BOC" }.Contains(userBankCode))
                {
                    userBankSubName = PreconditionAssert.IsNotEmptyString((string)p.userBankSubName, "请输入开户银行支行");
                }
                PreconditionAssert.IsTrue(ValidateHelper.IsBankCardNumber(userBankCardNumber), "请输入正确的银行卡号码");

                C_BankCard b = new C_BankCard
                {
                    UserId = userId,
                    BankCardNumber = userBankCardNumber,
                    BankCode = userBankCode,
                    BankName = userBankName,
                    BankSubName = userBankSubName,
                    ProvinceName = userProvinceName,
                    CityName = userCityName,
                    RealName = userTrueName
                };
                var result = _service.UpdateBankCard(b, CurrentUser.UserToken);
                _service.AddSysOperationLog(userId, CurrentUser.UserId, "更新银行卡", string.Format("操作员【{0}】给用户【{1}】更新银行卡，卡号【{2}】 ", CurrentUser.UserId, userId, userBankCardNumber));
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public IActionResult CancelBank(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("GXYHK230"))
                {
                    throw new LogicException("对不起,您的权限不足");
                }
                var p = JsonHelper.Decode(entity.Param);
                var userId = PreconditionAssert.IsNotEmptyString((string)p.userId, "用户编号不能为空");
                var result = _service.CancelBankCard(userId);
                _service.AddSysOperationLog(userId, CurrentUser.UserId, "取消绑定银行卡", string.Format("操作员【{0}】给用户【{1}】手工取消绑定银行卡 ", CurrentUser.UserId, userId));
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public IActionResult AddBank(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var userId = PreconditionAssert.IsNotEmptyString((string)p.userId, "用户编号不能为空");
                var userBankCardNumber = PreconditionAssert.IsNotEmptyString((string)p.userBankCardNumber, "银行卡号不能为空");
                var userBankCode = PreconditionAssert.IsNotEmptyString((string)p.userBankCode, "银行编码不能为空");
                var userBankName = PreconditionAssert.IsNotEmptyString((string)p.userBankName, "银行名称不能为空");
                var userProvinceName = PreconditionAssert.IsNotEmptyString((string)p.userProvinceName, "省不能为空");
                var userCityName = PreconditionAssert.IsNotEmptyString((string)p.userCityName, "城市不能为空");
                var userTrueName = PreconditionAssert.IsNotEmptyString((string)p.userTrueName, "真实姓名不能为空");
                var userBankSubName = PreconditionAssert.IsNotEmptyString((string)p.userBankSubName, "请输入开户银行支行");
                PreconditionAssert.IsTrue(ValidateHelper.IsBankCardNumber(userBankCardNumber), "请输入正确的银行卡号码");
                C_BankCard b = new C_BankCard
                {
                    UserId = userId,
                    BankCardNumber = userBankCardNumber,
                    BankCode = userBankCode,
                    BankName = userBankName,
                    BankSubName = userBankSubName,
                    ProvinceName = userProvinceName,
                    CityName = userCityName,
                    RealName = userTrueName
                };
                var result =_service.AddBankCard(b, CurrentUser.UserToken);
                _service.AddSysOperationLog(userId, CurrentUser.UserId, "绑定银行卡", string.Format("操作员【{0}】给用户【{1}】手工绑定银行卡,卡号【{2}】元", CurrentUser.UserId, userId, userBankCardNumber));
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        #region 李狄你的
        
        /// <summary>
        /// 订单查询
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IActionResult OrderQuery(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("CKDDMX250"))
                    throw new Exception("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);
                int PageIndex = string.IsNullOrWhiteSpace((string)p.pageIndex) ? base.PageIndex : Convert.ToInt32((string)p.pageIndex);
                int PageSize = string.IsNullOrWhiteSpace((string)p.pageSize) ? base.PageSize : Convert.ToInt32((string)p.pageSize);
                string UserKey = string.IsNullOrWhiteSpace((string)p.userKey) ? "" : (string)p.userKey;
                DateTime StartTime = string.IsNullOrWhiteSpace((string)p.startTime) ? DateTime.Today : Convert.ToDateTime((string)p.startTime);
                DateTime EndTime = string.IsNullOrWhiteSpace((string)p.endTime) ? DateTime.Today : Convert.ToDateTime((string)p.endTime);
                string GameCode = string.IsNullOrWhiteSpace((string)p.gameCode) ? "" : (string)p.gameCode;
                string bonusStatus_str = string.IsNullOrWhiteSpace((string)p.bonusStatus) ? "" : (string)p.bonusStatus;
                string betCategory_str = string.IsNullOrWhiteSpace((string)p.betCategory) ? "": (string)p.betCategory;
                string FieldName = string.IsNullOrWhiteSpace((string)p.fieldName) ? "" : (string)p.fieldName;
                int sortType = -1;
                var sortTypeStr = string.IsNullOrEmpty((string)p.sortType) ? "" : (string)p.sortType;
                if (!string.IsNullOrEmpty(sortTypeStr))
                {
                    sortType = sortTypeStr == "asc" ? 0 : 1;
                }
                //方案类型
                SchemeType? schemeType = null;
                if (!string.IsNullOrWhiteSpace((string)p.schemeType))
                {
                    schemeType = (SchemeType)Convert.ToInt32((string)p.schemeType);
                }

                //进度
                ProgressStatus? progressStatus = null;
                if (!string.IsNullOrWhiteSpace((string)p.progressStatus))
                {
                    progressStatus = (ProgressStatus)Convert.ToInt32((string)p.progressStatus);
                }

                //中奖状态
                BonusStatus? bonusStatus = null;
                if (bonusStatus_str == "1")
                {
                    bonusStatus = BonusStatus.Win;
                }
                if (bonusStatus_str == "2")
                {
                    bonusStatus = BonusStatus.Lose;
                }

                //出票状态
                TicketStatus? ticketStatus = null;
                if (!string.IsNullOrWhiteSpace((string)p.ticketStatus))
                {
                    ticketStatus = (TicketStatus)Convert.ToInt32((string)p.ticketStatus);
                }
                ///投注类别
                SchemeBettingCategory? betCategory = null;
                if (betCategory_str == "0")
                {
                    betCategory = SchemeBettingCategory.GeneralBetting;
                }
                if (betCategory_str == "1")
                {
                    betCategory = SchemeBettingCategory.SingleBetting;
                }
                if (betCategory_str == "2")
                {
                    betCategory = SchemeBettingCategory.FilterBetting;
                }
                if (betCategory_str == "3")
                    betCategory = SchemeBettingCategory.YouHua;
                if (betCategory_str == "4")
                    betCategory = SchemeBettingCategory.XianFaQiHSC;
                if (betCategory_str == "5")
                    betCategory = SchemeBettingCategory.ErXuanYi;
                if (betCategory_str == "8")
                    betCategory = SchemeBettingCategory.HunHeDG;

                //投注来源
                SchemeSource? schemeSource = null;
                if ((string)p.SchemeSource != null && !string.IsNullOrEmpty((string)p.SchemeSource))
                {
                    schemeSource = (SchemeSource)int.Parse((string)p.SchemeSource);
                }
                BettingOrderInfoCollection orderList = _service.QueryBettingOrderList(UserKey, schemeType, progressStatus,
    bonusStatus, betCategory, null,GameCode,StartTime,EndTime.AddDays(1), sortType,PageIndex,PageSize, CurrentUser.UserToken,FieldName, ticketStatus, schemeSource);
                var OrdersSearchResult = orderList;
                var GameList = _service.QueryGameList(CurrentUser.UserToken);
                IEnumerable<IGrouping<DateTime, BettingOrderInfo>> groupList =null;
                if (orderList != null)
                {
                    groupList = orderList.OrderList.GroupBy(o => o.BetTime.Date);
                }
                return Json(new LotteryServiceResponse() {Code=AdminResponseCode.成功,Value=new { OrdersSearchResult, GameList, groupList } });
            }
            catch (Exception ex)
            {
               return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        
        
        /// <summary>
        /// 手工完成充值
        /// </summary>
        public IActionResult CompleteFillMoney(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("WCCZ100"))
                {
                    throw new LogicException("对不起,您的权限不足!");
                }
                var p = JsonHelper.Decode(entity.Param);
                string orderId = PreconditionAssert.IsNotEmptyString((string)p.orderId, "订单ID不能为空");
                if (!string.IsNullOrWhiteSpace((string)p.sta) && (string)p.sta == "Fail")
                {
                    var result = _service.ManualCompleteFillMoneyOrder(orderId, FillMoneyStatus.Failed, CurrentUser.UserId);
                    _service.AddSysOperationLog("", this.CurrentUser.UserId, "充值手工置为失败", string.Format("操作员【{0}】充值手工置为失败【{1}】，订单号【{2}】", this.CurrentUser.UserId, result.IsSuccess ? "成功" : "失败", orderId));
                    return Json(new LotteryServiceResponse()
                    {
                        Code = result.IsSuccess == true ? AdminResponseCode.成功 : AdminResponseCode.失败,
                        Message = (result.IsSuccess ? "手工置为失败成功" : "手工置为失败失败")
                    }
                    );
                }
                else
                {
                    var result = _service.ManualCompleteFillMoneyOrder(orderId, FillMoneyStatus.Success, CurrentUser.UserId);
                    _service.AddSysOperationLog("", this.CurrentUser.UserId, "充值手工置为成功", string.Format("操作员【{0}】充值手工置为成功【{1}】，订单号【{2}】", CurrentUser.UserId, result.IsSuccess ? "成功" : "失败", orderId));
                    return Json(new LotteryServiceResponse() { Code = result.IsSuccess == true ? AdminResponseCode.成功 : AdminResponseCode.失败, Message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }
        /// <summary>
        /// 批量导出Excel
        /// </summary>
        /// <returns></returns>
        public FileResult ExportExcel(LotteryServiceRequest entity)
        {
            var p = JsonHelper.Decode(entity.Param);
            var bankcode = string.IsNullOrWhiteSpace((string)p.bankcode) ? "" : (string)p.bankcode.Trim();
            string MinMaxMoney = string.IsNullOrEmpty((string)p.MinMaxMoney) ? "" : (string)p.MinMaxMoney.ToString();
            var PageIndex = string.IsNullOrWhiteSpace((string)p.pageIndex) ? 0 : Convert.ToInt32((string)p.pageIndex);
            var PageSize = string.IsNullOrWhiteSpace((string)p.pageSize) ? 30 : Convert.ToInt32((string)p.pageSize);
            var StartTime = string.IsNullOrWhiteSpace((string)p.StartTime) ? DateTime.Today : Convert.ToDateTime((string)p.StartTime);
            var EndTime = string.IsNullOrWhiteSpace((string)p.EndTime) ? DateTime.Today : Convert.ToDateTime((string)p.EndTime);
            var HasSuccess = string.IsNullOrWhiteSpace((string)p.HasSuccess) ? false : Convert.ToBoolean((string)p.HasSuccess);
            var HasRefused = string.IsNullOrWhiteSpace((string)p.HasRefused) ? false : Convert.ToBoolean((string)p.HasRefused);
            var minMaxMoney = string.IsNullOrEmpty((string)p.money) ? "-1_-1" : (string)p.money;
            var minMoney = -1M;
            var maxMoney = -1M;
            var minMaxMoneyArray = minMaxMoney.Split('_');
            if (minMaxMoneyArray.Length == 2)
            {
                minMoney = decimal.Parse(minMaxMoneyArray[0]);
                maxMoney = decimal.Parse(minMaxMoneyArray[1]);
            }
            var sortType = -1;
            WithdrawAgentType? agent = WithdrawAgentType.BankCard;
            Withdraw_QueryInfoCollection withdraw_requesting = null;
            if (HasSuccess)
            {
                withdraw_requesting = _service.QueryWithdrawList2(minMoney, maxMoney, StartTime, EndTime, WithdrawStatus.Success, CurrentUser.UserToken);
            }
            else if (HasRefused)
            {
                withdraw_requesting = _service.QueryWithdrawList2(minMoney, maxMoney, StartTime, EndTime, WithdrawStatus.Refused, CurrentUser.UserToken);
            }
            else
            {
                withdraw_requesting = _service.QueryWithdrawList(
                                    "", agent, WithdrawStatus.Requesting, minMoney, maxMoney, StartTime, EndTime, sortType, "", PageIndex, PageSize, bankcode);
            }

            //创建Excel文件的对象
            NPOI.XSSF.UserModel.XSSFWorkbook book = new NPOI.XSSF.UserModel.XSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("商户订单号");
            row1.CreateCell(1).SetCellValue("银行卡号");
            row1.CreateCell(2).SetCellValue("真实姓名");
            row1.CreateCell(3).SetCellValue("到账金额");
            row1.CreateCell(4).SetCellValue("转账说明");
            //将数据逐步写入sheet1各个行
            int i = 0;
            foreach (var item in withdraw_requesting.WithdrawList)
            {
                i++;
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i);
                rowtemp.CreateCell(0).SetCellValue(item.OrderId);
                rowtemp.CreateCell(1).SetCellValue(item.BankCardNumber);
                rowtemp.CreateCell(2).SetCellValue(item.RequesterRealName);
                rowtemp.CreateCell(3).SetCellValue(item.ResponseMoney.HasValue ? item.ResponseMoney.Value.ToString("N2") : "");
                rowtemp.CreateCell(4).SetCellValue("转账");
            }

            // 写入到客户端 
            OverWriteNPOI ms = new OverWriteNPOI
            {
                AllowClose = false
            };
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            ms.AllowClose = true;
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("{0}.xlsx", DateTime.Now.ToString("yyyy-MM-dd")));
        }
        public IActionResult CompleteWithdrawALL(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string orderIds = PreconditionAssert.IsNotEmptyString((string)p.orderIds, "提现订单编号为空");
                string[] arrs = orderIds.Split(',');
                if (arrs.Length == 0)
                {
                    return Json(new { IsSuccess = false, Msg = "请勾选订单编号" });
                }
                string msg = string.Empty;
                foreach (var item in arrs)
                {
                    var result = _service.CompleteWithdraw(item, "接受提现，已打款", CurrentUser.UserToken);
                    //提款成功发送短信给用户
                    if (result.IsSuccess)
                    {
                        try
                        {
                            var with = _service.GetWithdrawById(item);
                            var mobile = with.RequesterMobile;
                            var content = "尊敬的 " + with.RequesterRealName + " 您好，您申请【" + with.ResponseMoney.Value.ToString("f") + "元】已处理，请注意查收。";
                            var returned = SendMessage(mobile, content);
                        }
                        catch
                        {

                        }
                    }
                }
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功 });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = ex.Message });
            }
        }
        public IActionResult RefusedWithdrawALL(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string orderIds = PreconditionAssert.IsNotEmptyString((string)p.orderIds, "提现订单编号为空");
                string[] arrs = orderIds.Split(',');
                if (arrs.Length == 0)
                {
                    return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = "请勾选订单编号" });
                }
                foreach (var item in arrs)
                {
                    var result = _service.RefusedWithdraw(item, "第三方打款失败，请稍候再重新申请", CurrentUser.UserToken);
                }
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = "" });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        #endregion
        public IActionResult ClearUserBindCache(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var userId = PreconditionAssert.IsNotEmptyString((string)p.userId, "用户编号不能为空");
                var result = _service.ManualClearUserBindCache(userId);
                _service.AddSysOperationLog(userId, CurrentUser.UserId, "清理用户缓存数据", string.Format("操作员【{0}】清理用户【{1}】缓存数据", CurrentUser.UserId, userId));
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public IActionResult BuildUserBindCache(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var userId = PreconditionAssert.IsNotEmptyString((string)p.userId, "用户编号不能为空");
                var result = _service.ManualBuildUserBindCache(userId);
                _service.AddSysOperationLog(userId, CurrentUser.UserId, "生成用户缓存数据", string.Format("操作员【{0}】生成用户【{1}】缓存数据", CurrentUser.UserId, userId));
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        /// <summary>
        /// 设为可疑用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="updateUserCreditType"></param>
        /// <returns></returns>
        public IActionResult UpdateUserCreditType(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string userId = (string)p.userId;
                int? updateUserCreditType = Convert.ToInt32((string)p.updateUserCreditType);
                if (string.IsNullOrEmpty(userId) || !updateUserCreditType.HasValue)
                {
                    return Json(new LotteryServiceResponse(){ Code = AdminResponseCode.失败, Message = "参数有误" });
                }
                var flag = _service.UpdateUserCreditType(userId, updateUserCreditType.Value);
                return Json(new LotteryServiceResponse() { Code = flag ? AdminResponseCode.成功 : AdminResponseCode.失败, Message = flag ? "操作成功" : "操作失败" });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = "操作失败-" + ex.Message });
            }
        }
        /// <summary>
        /// QQ的认证
        /// </summary>
        /// <returns></returns>
        public IActionResult AddQQNumber(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var userId = PreconditionAssert.IsNotEmptyString((string)p.userId, "用户编号不能为空");
                var qqNumber = PreconditionAssert.IsNotEmptyString((string)p.qqNumber, "QQ号码不能为空");
                var result =_service.AddUserQQ(userId, qqNumber);
                _service.AddSysOperationLog(userId, CurrentUser.UserId, "绑定QQ", string.Format("操作员【{0}】给用户【{1}】绑定QQ 【{2}】 ", CurrentUser.UserId, userId, qqNumber));
                if (result.IsSuccess)
                {
                    return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = "操作成功" });
                }
                else
                {
                    return Json(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = "操作失败" });
                }
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = "操作失败-" + ex.Message });
            }
        }
        public IActionResult CancelQQNumber(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var userId = PreconditionAssert.IsNotEmptyString((string)p.userId, "用户编号不能为空");
                var result = _service.CancelUserQQ(userId);
                _service.AddSysOperationLog(userId, this.CurrentUser.UserId, "取消绑定QQ", string.Format("操作员【{0}】给用户【{1}】取消绑定QQ ", CurrentUser.UserId, userId));
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = "操作成功" });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = "操作失败-" + ex.Message });
            }
        }
        /// <summary>
        /// 经销商
        /// </summary>
        /// <returns></returns>
        public IActionResult UpdateAgentId(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("HFDL280"))
                {
                    throw new LogicException("对不起,您的权限不足!");
                }
                var p = JsonHelper.Decode(entity.Param);
                var userId = PreconditionAssert.IsNotEmptyString((string)p.userId, "用户编号不能为空");
                var agentId = PreconditionAssert.IsNotEmptyString((string)p.agentId, "经销商编号不能为空");
                var result = _service.UpdateUserAgentId(userId, agentId, CurrentUser.UserToken);
                _service.AddSysOperationLog(userId, CurrentUser.UserId, "更新经销商", string.Format("操作员【{0}】更新用户【{1}】经销商为【{2}】", CurrentUser.UserId, userId, agentId));
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }

        }
        public IActionResult ResAgent(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var userId = PreconditionAssert.IsNotEmptyString((string)p.userId, "用户编号不能为空");
                var result = _service.LogOffUserAgent(userId, CurrentUser.UserToken);
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        /// <summary>
        /// 支付宝绑定
        /// </summary>
        /// <returns></returns>
        public IActionResult UpdateApliyNumber(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var userId = PreconditionAssert.IsNotEmptyString((string)p.userId, "用户编号不能为空");
                var apliyNumber = PreconditionAssert.IsNotEmptyString((string)p.apliyNumber, "支付宝不能为空");
                if (!ValidateHelper.IsEmail(apliyNumber) && !ValidateHelper.IsMobile(apliyNumber))
                {
                    return Json(new { Succuss = false, Msg = "支付宝账号格式错误，请输入正确的支付宝账号（邮箱或手机号)。" });
                }
                else
                {
                    var result = _service.AddUserAlipay(userId, apliyNumber);
                    _service.AddSysOperationLog(userId, CurrentUser.UserId, "支付宝绑定", string.Format("操作员【{0}】给用户【{1}】支付宝绑定【{2}】 ", CurrentUser.UserId, userId, apliyNumber));
                    if (result.IsSuccess)
                    {
                        return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = result.Message });
                    }
                    else
                    {
                        return Json(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = result.Message });
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public IActionResult CancelApliyNumber(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var userId = PreconditionAssert.IsNotEmptyString((string)p.userId, "用户编号不能为空");
                var result = _service.CancelUserAlipay(userId);
                _service.AddSysOperationLog(userId, CurrentUser.UserId, "取消支付宝绑定", string.Format("操作员【{0}】给用户【{1}】取消支付宝绑定", CurrentUser.UserId, userId));
                if (result.IsSuccess)
                {
                    return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = result.Message });
                }
                else
                {
                    return Json(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        /// <summary>
        /// 发送站内信
        /// </summary>
        public IActionResult SiteMessage()
        {
            try
            {
                if (!CheckRights("U103"))
                    throw new Exception("对不起，您的权限不足！");
                var Role = _service.GetSystemRoleCollection(CurrentUser.UserToken);

                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = Role });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public IActionResult SendLetter(LotteryServiceRequest entity)
        {
            if (!CheckRights("FSXX100"))
            {
                throw new LogicException("对不起,您的权限不足!");
            }
            var p = JsonHelper.Decode(entity.Param);
            InnerMailInfo_Send sendMail = new InnerMailInfo_Send();
            try
            {
                sendMail.ActionTime = null;
                sendMail.Content = PreconditionAssert.IsNotEmptyString((string)p.content, "内容不能为空！");
                sendMail.Title = PreconditionAssert.IsNotEmptyString((string)p.title, "标题不能为空！");
                string userid = (string)p.userid;
                if (!string.IsNullOrEmpty(userid))
                {
                    #region 
                    string userId = "";
                    if (userid.Split(',').Length > 0)
                    {
                        for (int i = 0; i < userid.Split(',').Length; i++)
                        {
                            userId += userid.Split(',')[i] + "|";
                        }
                        sendMail.Receivers = userId.TrimEnd('|');
                    }
                    else
                    {
                        sendMail.Receivers = (string)p.userid;
                    }
                    var sendResult = _service.SendInnerMail(sendMail, CurrentUser.UserToken);
                    return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = sendResult.Message });
                    #endregion
                }
                else
                {
                    sendMail.Receivers = PreconditionAssert.IsNotEmptyString((string)p.receivers, "发信对象不能为空！");
                    var userIds = _service.QueryUserIdByRoleId(sendMail.Receivers);
                    if (!string.IsNullOrEmpty(userIds))
                    {
                        var arrUserIds = userIds.Split('|');
                        int count = arrUserIds.Length;
                        if (arrUserIds.Length % 500 > 0) count++;
                        for (int i = 0; i < count; i++)
                        {
                            var temp = arrUserIds.Skip(i * 500).Take(500);
                            sendMail.Receivers = string.Join("|", temp);
                            var sendResult = _service.SendInnerMail(sendMail, CurrentUser.UserToken);
                        }
                    }
                    return Json(new LotteryServiceResponse(){ Code = AdminResponseCode.成功, Message = "发送站内信成功" });
                }
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse{ Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public IActionResult SiteNoticeManager()
        {
            if (!CheckRights("WZTZGL"))
                throw new Exception("对不起，您的权限不足！");
            var SiteMessageTags = _service.QuerySiteMessageTags();
            var ConfigList = _service.QuerySiteNoticeConfig();
            return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = new { ConfigList, SiteMessageTags } });
        }
        public IActionResult UpdateSiteNotice(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("WZTZGL_XG"))
                {
                    throw new LogicException("对不起,您的权限不足!");
                }
                var p = JsonHelper.Decode(entity.Param);
                var key = (string)p.key.ToString();
                var category = (SiteMessageCategory)int.Parse((string)p.category);
                var tempTitle = (string)p.tempTitle;
                var tempContent = (string)p.tempContent;

                var result = _service.UpdateSiteNotice(key, category, tempTitle, tempContent);

                return Json(new LotteryServiceResponse(){ Code = result.IsSuccess?AdminResponseCode.成功:AdminResponseCode.失败, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse(){ Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public IActionResult SMSSendRecordLog(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var UserId = (string)p.userId;
                var MobileNumber = (string)p.mobileNumber;
                var Status = (string)p.status;
                var StartTime = string.IsNullOrEmpty((string)p.startTime) ? DateTime.Today : Convert.ToDateTime((string)p.startTime);
                var EndTime = string.IsNullOrEmpty((string)p.endTime) ? DateTime.Today.AddDays(1) : Convert.ToDateTime((string)p.endTime);
                var PageIndex = string.IsNullOrEmpty((string)p.pageIndex) ? base.PageIndex : int.Parse((string)p.pageIndex);
                var PageSize = string.IsNullOrEmpty((string)p.pageSize) ? base.PageSize : int.Parse((string)p.pageSize);
                var List = _service.QuerySMSSendRecordList(UserId, MobileNumber, StartTime, EndTime, Status, PageIndex, PageSize);
                _service.AddSysOperationLog(UserId, CurrentUser.UserId, "查询短信发送记录", string.Format("操作员【{0}】查询手机号【{1}】的查询短信发送记录", CurrentUser.UserId, MobileNumber));
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = List });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public IActionResult RepeatSMS(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var userId = (string)p.userId;
                var mobileNumber = (string)p.mobileNumber;
                var content = (string)p.content;
                var result = _service.SendSMS(mobileNumber, content, userId);
                return Json(new LotteryServiceResponse(){ Code = result.IsSuccess?AdminResponseCode.成功:AdminResponseCode.失败, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        /// <summary>
        /// 用户手机校验码管理
        /// </summary>
        public IActionResult ValidateCode(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                if (!string.IsNullOrWhiteSpace((string)p.mobile))
                {
                    var Mobile = (string)p.mobile;
                    var ValidationMobile = _service.QueryValidationMobileByMobile(Mobile, CurrentUser.UserToken);
                    _service.AddSysOperationLog((string)p.mobile, CurrentUser.UserId, "查询手机验证码", string.Format("操作员【{0}】查询手机号【{1}】的验证码", CurrentUser.UserId, (string)p.mobile));
                    return Json(new LotteryServiceResponse() {Code=AdminResponseCode.成功,Value=ValidationMobile });
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace((string)p.userKey))
                    {
                        var UserKey = string.IsNullOrWhiteSpace((string)p.userKey) ? "" : (string)p.userKey;
                        var Mobile = (string)p.mobile;
                        var ValidationMobile = _service.QueryValidationMobileByMobile(Mobile, CurrentUser.UserToken);
                        return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = ValidationMobile });
                    }
                    else
                    {
                        return Json(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = "userKey为空" });
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public IActionResult DoSendSMS(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("PLFSDX100"))
                {
                    throw new LogicException("对不起,您的权限不足!");
                }
                var p = JsonHelper.Decode(entity.Param);
                var content = (string)p.Content;
                var mobileString = (string)p.Mobile;
                var mobileArray = mobileString.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                var errorList = new List<string>();
                foreach (var item in mobileArray)
                {
                    try
                    {
                        var result = _service.SendSMS(item, content, "");
                    }
                    catch (Exception ex)
                    {
                        errorList.Add(item);
                    }
                }
                return Json(new LotteryServiceResponse(){ Code=AdminResponseCode.成功, Message = string.Format("发送成功{0}条，失败{1}条，失败手机号为：{2}", mobileArray.Length - errorList.Count, errorList.Count, string.Join(",", errorList.ToArray())) });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse(){ Code = AdminResponseCode.成功, Message = ex.Message });
            }
        }
        public IActionResult DoAgent(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var txt_userId = (string)p.txt_userId;
                var txt_oCAgentCategory = (OCAgentCategory)Convert.ToInt32((string)p.txt_oCAgentCategory);
                var txt_superiorUserId = string.Empty;
                if (txt_oCAgentCategory != OCAgentCategory.Company)
                {
                    txt_superiorUserId = (string)p.txt_superiorUserId;
                }
                var result = _service.AddOCAgent(txt_oCAgentCategory, txt_superiorUserId, txt_userId, CPSMode.PayRebate, "");
                return Json(new LotteryServiceResponse(){ Code = result.IsSuccess?AdminResponseCode.成功:AdminResponseCode.失败, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse(){ Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public IActionResult DoTogetherHotUser(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var txt_userId = (string)p.txt_userId;
                var result = _service.AddTogetherHotUser(txt_userId);
                _service.AddSysOperationLog(txt_userId, CurrentUser.UserId, "添加红人", "操作员【" + CurrentUser.LoginName + "】添加红人,用户" + txt_userId + "被添加为红人,执行结果:" + result.Message);
                return Json(new LotteryServiceResponse(){ Code = result.IsSuccess?AdminResponseCode.成功:AdminResponseCode.失败, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public IActionResult DeleteTogetherHotUser(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string txt_userId = Convert.ToString(PreconditionAssert.IsNotEmptyString((string)p.userId, "处理用户ID异常"));
                var result = _service.DeleteTogetherHotUser(txt_userId);
                return Json(new LotteryServiceResponse() { Code = result.IsSuccess ? AdminResponseCode.成功 : AdminResponseCode.失败, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }

        }
        public IActionResult AddUserSchemeShareExpert(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("U110"))
                    throw new Exception("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);
                var userId = PreconditionAssert.IsNotEmptyString((string)p.UserId, "用户编号不能为空");
                var shortIndex = (string)p.ShortIndex == null ? 0 : Convert.ToInt32((string)p.ShortIndex);
                var source = Convert.ToInt32((string)p.Source);
                if (source <= 0)
                    throw new Exception("请选择类型");
                var result = _service.AddUserSchemeShareExpert(userId, shortIndex, (CopyOrderSource)source);
                return Json(new LotteryServiceResponse(){ Code = result.IsSuccess?AdminResponseCode.成功:AdminResponseCode.失败, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse(){ Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public IActionResult DeleteUserSchemeShareExpert(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("U110"))
                {
                    throw new LogicException("对不起,您的权限不足!");
                }
                var p = JsonHelper.Decode(entity.Param);
                var userId = PreconditionAssert.IsNotEmptyString((string)p.Id, "主键编号不能为空");
                var result = _service.DeleteUserSchemeShareExpert(userId);
                return Json(new LotteryServiceResponse() { Code = result.IsSuccess ? AdminResponseCode.成功 : AdminResponseCode.失败, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public IActionResult QueryUserSchemeShareExpertList(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("U110"))
                    throw new Exception("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);
                var UserKey = (string)p.UserKey == null ? string.Empty : (string)p.UserKey.ToString();
                var Source = string.IsNullOrEmpty((string)p.Source) ? -1 : Convert.ToInt32((string)p.Source);
                var PageIndex = (string)p.PageIndex == null ? base.PageIndex : Convert.ToInt32((string)p.PageIndex);
                var PageSize = (string)p.PageSize == null ? base.PageSize : Convert.ToInt32((string)p.PageSize);
                var SchemeShareList = _service.QueryUserSchemeShareExpertList(UserKey,Source,PageIndex,PageSize);
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = SchemeShareList });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        /// <summary>
        /// 代理统计数据
        /// </summary>
        public IActionResult AgentDetail(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var AgentId = string.IsNullOrWhiteSpace((string)p.agentId) ? string.Empty : (string)p.agentId;
                var GameCode = string.IsNullOrWhiteSpace((string)p.gameCode) ? string.Empty : (string)p.gameCode;
                var StartTime = string.IsNullOrWhiteSpace((string)p.startTime) ? DateTime.Now : Convert.ToDateTime((string)p.startTime);
                var EndTime = string.IsNullOrWhiteSpace((string)p.endTime) ? DateTime.Now : Convert.ToDateTime((string)p.endTime);
                var PageIndex = string.IsNullOrWhiteSpace((string)p.pageIndex) ? base.PageIndex : int.Parse((string)p.pageIndex);
                var PageSize = string.IsNullOrWhiteSpace((string)p.pageSize) ? base.PageSize : int.Parse((string)p.pageSize);
                bool? isFillMoney = null;
                if (!string.IsNullOrWhiteSpace((string)p.isFillMoney)) { isFillMoney = bool.Parse((string)p.isFillMoney); }
                var IsFillMoney = isFillMoney;
                var AgentDetailList = _service.QueryAgentDetail(AgentId, GameCode, StartTime, EndTime, PageIndex, PageSize, (isFillMoney!=null&&isFillMoney.HasValue ? isFillMoney.Value : false));
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = AgentDetailList });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
    }
}
