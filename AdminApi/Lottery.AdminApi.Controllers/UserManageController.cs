using EntityModel;
using EntityModel.Enum;
using EntityModel.ExceptionExtend;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Utilities;
using KaSon.FrameWork.ORM.Helper;
using Lottery.AdminApi.Controllers.CommonFilterActtribute;
using Lottery.AdminApi.Model.HelpModel;
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
    [ReusltFilter]
    public class UserManageController :BaseController
    {
        private readonly static AdminService _service = new AdminService();
        private readonly static string GiveMoneyManager = "ADMIN|LAOGAN";
        public IActionResult MemberManage(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("U101"))
                {
                    throw new LogicException("对不起，您的权限不足！");
                }
                   
                //if (CheckRights("HYGLCKYHXQ110"))//查看会员详情
                
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

                var AgentId = p.agentId ;
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
        public IActionResult BatchSetInnerUser(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("U102"))
                {
                    throw new LogicException("对不起，您的权限不足！");
                }
                var p = JsonHelper.Decode(entity.Param);
                var userIds = PreconditionAssert.IsNotEmptyString(p.UserIds, "用户编号不能为空");
                var result = _service.BatchSetInnerUser(userIds);
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
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
        public IActionResult GiveMoneyToTotalStayUser(LotteryServiceRequest entity)
        {
            //1.每次查询1000条数据，查询完后循环发钱，发完后继续查询，直到没有为止
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var Flag = GiveMoneyManager.Split('|').Any(x => x == CurrentUser.DisplayName.ToLower());
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
        public FileResult ExportStayUserExcel(LotteryServiceRequest entity)
        {
            var p = JsonHelper.Decode(entity.Param);
            var Flag = GiveMoneyManager.Split('|').Any(x => x ==CurrentUser.DisplayName.ToLower());
            if (Flag == false)
            {
                return null;
            }
            var Days = string.IsNullOrWhiteSpace((string)p.days) ? 14 : int.Parse((string)p.days);
            var Earnings = string.IsNullOrWhiteSpace((string)p.earnings) ? 0 : int.Parse((string)p.earnings);
            string theEarnings = EarningsList[Earnings];
            var list =_service.QueryExcelNotOnlineRecentlyList(Days, theEarnings);
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
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
            MemoryStream ms = new MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", string.Format("{0}.xls", DateTime.Now.ToString("yyyy-MM-dd")));
        }
        public IActionResult MemberDetail(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("U102"))
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
                    UserQueryInfo userResult = _service.QueryUserByKey(userId, CurrentUser.UserToken);
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
                _service.AddSysOperationLog(userId, CurrentUser.UserId, "重置密码", string.Format("操作员【{0}】重置用户【{1}】的登录密码", this.CurrentUser.UserId, userId));

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
                var userId = PreconditionAssert.IsNotEmptyString(p.userId, "用户编号不能为空");
                var vipLevel = PreconditionAssert.IsInt32(p.vipLevel, "输入VIP级别不能为空");
                var result = _service.UpdateUserVipLevel(userId, vipLevel, CurrentUser.UserToken);
                _service.AddSysOperationLog(userId, CurrentUser.UserId, "调整用户VIP级别", string.Format("操作员【{0}】修改了 用户【{1}】 的vip等级", CurrentUser.UserId, userId));
                return Json(new LotteryServiceResponse{ Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        // 手工充值
        public JsonResult ManualFillMoney(LotteryServiceRequest entity)
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

                _service.AddSysOperationLog(userKey, this.CurrentUser.UserId, "手工充值", string.Format("操作员【{0}】给用户【{1}】手工充值【{2}】元", this.CurrentUser.UserId, userKey, (string)p.requestMoney));

                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = fillResult.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        // 手工打款
        public ActionResult ManualAdd(LotteryServiceRequest entity)
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
        // 手工扣款
        public ActionResult ManualDeduct(LotteryServiceRequest entity)
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
                _service.AddSysOperationLog(userKey, this.CurrentUser.UserId, "手工扣款", string.Format("操作员【{0}】对用户【{1}】扣款【{2}】元", this.CurrentUser.UserId, userKey, money));
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
    }
}
