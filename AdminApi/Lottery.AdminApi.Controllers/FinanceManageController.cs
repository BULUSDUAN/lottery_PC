using EntityModel;
using EntityModel.Enum;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.ORM.Helper;
using Lottery.AdminApi.Controllers.CommonFilterActtribute;
using Lottery.AdminApi.Model.HelpModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using KaSon.FrameWork.Common.ExceptionEx;
using EntityModel.ExceptionExtend;
using KaSon.FrameWork.Common.Utilities;
using System.IO;
using EntityModel.CoreModel;
using System.Linq;
using KaSon.FrameWork.Common.Shunlifu;

namespace Lottery.AdminApi.Controllers
{
    /// <summary>
    /// 财务管理
    /// </summary>
    [Area("api")]
    //[ReusltFilter]
    public class FinanceManageController : BaseController
    {
        private readonly static AdminService _service = new AdminService();
        #region 充值明细
        /// <summary>
        /// 充值明细分页
        /// </summary>
        public IActionResult FillMoneyDetail(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("C102"))
                    throw new LogicException("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);
                string userKey = p.userKey;
                string orderId = p.orderId;
                string status = p.status;
                string schemeSource = p.schemeSource;
                string agentType = p.agentType;
                string startTimeStr = p.startTime;
                string endTimeStr = p.endTime;
                string pageIndexStr = p.pageIndex;
                string pageSizeStr = p.pageSize;
                //bool wccz = false;
                //bool zwsb = false;
                //bool czdcsj = false;
                //bool czmxckyhxq = false;
                //if (CheckRights("WCCZ100"))
                //    wccz = true;
                //if (CheckRights("ZWSB110"))
                //    zwsb = true;
                //if (CheckRights("CZDCSJ120"))
                //    czdcsj = true;
                //if (CheckRights("CZMXCKYHXQ130"))
                //    czmxckyhxq = true;
                //ViewBag.wccz = wccz;
                //ViewBag.zwsb = zwsb;
                //ViewBag.czdcsj = czdcsj;
                //ViewBag.czmxckyhxq = czmxckyhxq;
                //ViewBag.CZMX_CZBS = CheckRights("CZMX_CZBS");
                //ViewBag.CZMX_QQCZJE = CheckRights("CZMX_QQCZJE");
                //ViewBag.CZMX_WCCZJQ = CheckRights("CZMX_WCCZJQ");
                //var service = new AdminService();
                var UserKey = string.IsNullOrEmpty(userKey) ? "" : userKey.Trim();
                var OrderId = string.IsNullOrEmpty(orderId) ? "" : orderId.Trim();
                var UserResult = new UserQueryInfo();
                if (!string.IsNullOrEmpty(UserKey))
                    UserResult = _service.QueryUserByKey(UserKey);
                var StartTime = string.IsNullOrEmpty(startTimeStr) ? DateTime.Now : DateTime.Parse(startTimeStr);
                var EndTime = string.IsNullOrEmpty(endTimeStr) ? DateTime.Now : DateTime.Parse(endTimeStr);
                var PageIndex = string.IsNullOrEmpty(pageIndexStr) ? base.PageIndex : int.Parse(pageIndexStr);
                var PageSize = string.IsNullOrEmpty(pageSizeStr) ? base.PageSize : int.Parse(pageSizeStr);
                var FillDetail = _service.QueryFillMoneyList(UserKey,
                    agentType,
                    status,
                    schemeSource,
                    StartTime, EndTime,
                    PageIndex, PageSize, OrderId);
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = "查询成功",
                    Value = new
                    {
                        UserQueryInfo = UserResult,
                        FillDetail = FillDetail
                    }
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
        /// 手工完成充值
        /// </summary>
        public IActionResult CompleteFillMoney(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("WCCZ100"))
                    throw new LogicException("对不起,您的权限不足!");
                var p = JsonHelper.Decode(entity.Param);
                string orderId = p.orderId;
                orderId = PreconditionAssert.IsNotEmptyString(orderId, "订单ID不能为空");
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
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }
        /// <summary>
        /// 导出充值扣款明细
        /// </summary>
        public FileResult ExportFillDetail(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("CZDCSJ120"))
                    throw new LogicException("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);
                string userKey = p.userKey;
                string endTimeStr = p.endTime;
                string startTimeStr = p.startTime;
                userKey = string.IsNullOrEmpty(userKey) ? "" : userKey;
                var startTime = string.IsNullOrEmpty(startTimeStr) ? DateTime.Now.AddMonths(-1) : DateTime.Parse(startTimeStr);
                var endTime = string.IsNullOrEmpty(endTimeStr) ? DateTime.Now : DateTime.Parse(endTimeStr);
                IList<FillMoneyQueryInfo> FillDetail = _service.QueryFillMoneyList(userKey, "", "", "", startTime, endTime, 0, int.MaxValue, "").FillMoneyList;

                NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
                //添加一个sheet
                NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("充值扣款明细(" + startTime.ToString("yyyy-MM-dd") + "至" + endTime.ToString("yyyy-MM-dd"));
                NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("请求时间");
                row1.CreateCell(1).SetCellValue("响应时间");
                row1.CreateCell(2).SetCellValue("用户编号");
                row1.CreateCell(3).SetCellValue("用户名");
                row1.CreateCell(4).SetCellValue("处理状态");
                row1.CreateCell(5).SetCellValue("交易类型");
                row1.CreateCell(6).SetCellValue("充值来源");
                row1.CreateCell(7).SetCellValue("充值金额");
                row1.CreateCell(8).SetCellValue("交易说明");
                int i = 0;
                foreach (var item in FillDetail)
                {
                    string temp = "";
                    if (item.Status == FillMoneyStatus.Failed)
                    {
                        temp = "失败";
                    }
                    else if (item.Status == FillMoneyStatus.Requesting)
                    {
                        temp = "请求中";
                    }
                    else
                    {
                        temp = "成功";
                    }

                    Func<FillMoneyAgentType, string> format = (FillMoneyAgentType type) =>
                    {
                        switch (type)
                        {
                            case FillMoneyAgentType.Alipay:
                                return "支付宝";
                            case FillMoneyAgentType.AlipayWAP:
                                return "支付宝-WAP支付";
                            case FillMoneyAgentType.CallsPay:
                                return "手机充值卡支付";
                            case FillMoneyAgentType.ChinaPay:
                                return "网银在线";
                            case FillMoneyAgentType.KuanQian:
                                return "快钱";
                            case FillMoneyAgentType.ManualDeduct:
                                return "手工扣款";
                            case FillMoneyAgentType.ManualFill:
                                return "手工充值";
                            case FillMoneyAgentType.Tenpay:
                                return "财付通";
                            case FillMoneyAgentType.Yeepay:
                                return "易宝";
                        }
                        return "未知来源";
                    };
                    i++;
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i);
                    rowtemp.CreateCell(0).SetCellValue(item.RequestTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    rowtemp.CreateCell(1).SetCellValue((item.ResponseTime.HasValue ? item.ResponseTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : ""));
                    rowtemp.CreateCell(2).SetCellValue(item.UserId);
                    rowtemp.CreateCell(3).SetCellValue(item.UserDisplayName);
                    rowtemp.CreateCell(4).SetCellValue(temp);
                    rowtemp.CreateCell(5).SetCellValue(item.GoodsName);
                    rowtemp.CreateCell(6).SetCellValue(format(item.FillMoneyAgent));
                    rowtemp.CreateCell(7).SetCellValue(item.RequestMoney.ToString("N2"));
                    rowtemp.CreateCell(8).SetCellValue(item.GoodsDescription);
                }
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms, "application/vnd.ms-excel", string.Format("{0}.xls", DateTime.Now.ToString("yyyy-MM-dd")));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 提现管理

        /// <summary>
        /// 查询用户数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IActionResult QueryUserByKey(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string userKey = p.userKey;
                userKey = string.IsNullOrEmpty(userKey) ? "" : userKey.Trim();
                if (string.IsNullOrEmpty(userKey))
                {
                    throw new LogicException("参数有误");
                }
                var UserResult = _service.QueryUserByKey(userKey);
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = "查找成功", Value = UserResult });

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

        ///// <summary>
        ///// 财务新提现管理列表
        ///// </summary>
        //public IActionResult NewWithdrawDetail(LotteryServiceRequest entity)
        //{
        //    try
        //    {
        //        if (!CheckRights("C113"))
        //            throw new Exception("对不起，您的权限不足！");
        //        //bool cktxxq = false;
        //        //bool txmxdcsj = false;
        //        //if (CheckRights("CKTXXQ100"))
        //        //    cktxxq = true;
        //        //if (CheckRights("TXMXDCSJ110"))
        //        //    txmxdcsj = true;
        //        //ViewBag.cktxxq = cktxxq;
        //        //ViewBag.txmxdcsj = txmxdcsj;
        //        //foreach (HttpPostedFileBase item in Request.Files)
        //        //{
        //        //    var fileName = item.FileName;
        //        //    var stream = item.InputStream;
        //        //}

        //        //ViewBag.WithdrawLine = base.GameClient.QueryConfigByKey("WithdrawQuery_Line").ConfigValue;
        //        var p = JsonHelper.Decode(entity.Param);
        //        string bankcode = p.bankcode;
        //        string userKey = p.userKey;
        //        string operUserId = p.operUserId;
        //        string startTimeStr = p.startTime;
        //        string endTimeStr = p.endTime;
        //        string pageIndexStr = p.pageIndex;
        //        string pageSizeStr = p.pageSize;
        //        string withdrawStatus = p.WithdrawStatus;
        //        bankcode = string.IsNullOrEmpty(bankcode) ? "" : bankcode.Trim();
        //        userKey = string.IsNullOrEmpty(userKey) ? "" : userKey.Trim();
        //        ViewBag.OperUserId = string.IsNullOrEmpty(operUserId) ? "" : operUserId.Trim();
        //        ViewBag.StartTime = string.IsNullOrEmpty(startTimeStr) ? DateTime.Today : Convert.ToDateTime(startTimeStr);
        //        ViewBag.EndTime = string.IsNullOrEmpty(endTimeStr) ? DateTime.Today : Convert.ToDateTime(endTimeStr);

        //        ViewBag.PageIndex = string.IsNullOrEmpty(pageIndexStr) ? 0 : Convert.ToInt32(pageIndexStr);
        //        ViewBag.PageSize = string.IsNullOrEmpty(pageSizeStr) ? 30 : Convert.ToInt32(pageSizeStr);
        //        //ViewBag.PageIndexS = string.IsNullOrEmpty(Request["pageIndexS"]) ? 0 : Convert.ToInt32(Request["pageIndexS"]);
        //        //ViewBag.PageSizeS = string.IsNullOrEmpty(Request["pageSizeS"]) ? 10 : Convert.ToInt32(Request["pageSizeS"]);
        //        //ViewBag.PageIndexR = string.IsNullOrEmpty(Request["pageIndexR"]) ? 0 : Convert.ToInt32(Request["pageIndexR"]);
        //        //ViewBag.PageSizeR = string.IsNullOrEmpty(Request["pageSizeR"]) ? 10 : Convert.ToInt32(Request["pageSizeR"]);

        //        //ViewBag.HasSuccess = string.IsNullOrEmpty(Request["s"]) ? false : Convert.ToBoolean(Request["s"]);
        //        //ViewBag.HasRefused = string.IsNullOrEmpty(Request["r"]) ? false : Convert.ToBoolean(Request["r"]);
        //        var service = new AdminService();
        //        WithdrawAgentType? agent = null;
        //        if (!string.IsNullOrEmpty(Request["agentWay"]))
        //        {
        //            var intAgent = int.Parse(Request["agentWay"]);
        //            if (intAgent != -1)
        //            {
        //                agent = (WithdrawAgentType)intAgent;
        //            }
        //        }
        //        ViewBag.Agent = agent;
        //        var minMaxMoney = string.IsNullOrEmpty(Request["money"]) ? "-1_-1" : Request["money"];
        //        ViewBag.MinMaxMoney = minMaxMoney;
        //        var minMoney = -1M;
        //        var maxMoney = -1M;
        //        var minMaxMoneyArray = minMaxMoney.Split('_');
        //        if (minMaxMoneyArray.Length == 2)
        //        {
        //            minMoney = decimal.Parse(minMaxMoneyArray[0]);
        //            maxMoney = decimal.Parse(minMaxMoneyArray[1]);
        //        }
        //        var sortType = -1;
        //        var sortTypeStr = string.IsNullOrEmpty(Request["slt_sort_type"]) ? "" : Request["slt_sort_type"];
        //        if (!string.IsNullOrEmpty(sortTypeStr))
        //            sortType = int.Parse(sortTypeStr);
        //        ViewBag.SortType = sortType;



        //        ViewBag.WithdrawList_Success = service.QueryWithdrawListR(
        //                ViewBag.UserKey, ViewBag.Agent, withdrawStatus, minMoney, maxMoney, ViewBag.StartTime, ViewBag.EndTime, sortType, ViewBag.OperUserId, ViewBag.PageIndex, ViewBag.PageSize, ViewBag.bankcode, CurrentUser.UserToken);
        //        if (ViewBag.HasSuccess)
        //        {
        //            ViewBag.WithdrawList_Success = base.ExternalClient.QueryWithdrawListR(
        //                    ViewBag.UserKey, ViewBag.Agent, WithdrawStatus.Success, minMoney, maxMoney, ViewBag.StartTime, ViewBag.EndTime, sortType, ViewBag.OperUserId, ViewBag.PageIndex, ViewBag.PageSize, ViewBag.bankcode, CurrentUser.UserToken);
        //        }
        //        if (ViewBag.HasRefused)
        //        {
        //            ViewBag.WithdrawList_Refused = base.ExternalClient.QueryWithdrawListR(
        //                    ViewBag.UserKey, ViewBag.Agent, WithdrawStatus.Refused, minMoney, maxMoney, ViewBag.StartTime, ViewBag.EndTime, sortType, ViewBag.OperUserId, ViewBag.PageIndex, ViewBag.PageSize, ViewBag.bankcode, CurrentUser.UserToken);
        //        }
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public IActionResult WithdrawLineSelect()
        {
            try
            {
                var WithdrawLine = _service.QueryConfigByKey("WithdrawQuery_LineKey").ConfigValue.Split('|').ToList();
                return Json(new LotteryServiceResponse()
                {
                    Code = AdminResponseCode.成功,
                    Message = "查询成功",
                    Value = WithdrawLine.Select(p => new
                    {
                        showName = p == "-1_-1" ? "全部" : p.Replace("_-1", "以上"),
                        value = p
                    }).ToList()
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
        /// 提现管理列表(根据传进来的withdrawStatus去搜索列表)
        /// </summary>
        public IActionResult WithdrawDetail(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("C103"))
                    throw new Exception("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);
                string bankcode = p.bankcode;
                string userKey = p.userKey;
                string operUserId = p.operUserId;
                string startTimeStr = p.startTime;
                string endTimeStr = p.endTime;
                string pageIndexStr = p.pageIndex;
                string pageSizeStr = p.pageSize;
                string withdrawStatus = p.WithdrawStatus;
                string money = p.money;
                string slt_sort_type = p.slt_sort_type;
                string agentWay = p.agentWay;
                bankcode = string.IsNullOrEmpty(bankcode) ? "" : bankcode.Trim();
                userKey = string.IsNullOrEmpty(userKey) ? "" : userKey.Trim();
                //bool cktxxq = false;
                //bool txmxdcsj = false;
                //if (CheckRights("CKTXXQ100"))
                //    cktxxq = true;
                //if (CheckRights("TXMXDCSJ110"))
                //    txmxdcsj = true;
                //ViewBag.cktxxq = cktxxq;
                //ViewBag.txmxdcsj = txmxdcsj;
                //foreach (HttpPostedFileBase item in Request.Files)
                //{
                //    var fileName = item.FileName;
                //    var stream = item.InputStream;
                //}

                //ViewBag.WithdrawLine = base.GameClient.QueryConfigByKey("WithdrawQuery_Line").ConfigValue;

                bankcode = string.IsNullOrEmpty(bankcode) ? "" : bankcode.Trim();
                userKey = string.IsNullOrEmpty(userKey) ? "" : userKey.Trim();
                operUserId = string.IsNullOrEmpty(operUserId) ? "" : operUserId.Trim();
                var StartTime = string.IsNullOrEmpty(startTimeStr) ? DateTime.Today : Convert.ToDateTime(startTimeStr);
                var EndTime = string.IsNullOrEmpty(endTimeStr) ? DateTime.Today : Convert.ToDateTime(endTimeStr);
                var PageIndex = string.IsNullOrEmpty(pageIndexStr) ? base.PageIndex : Convert.ToInt32(pageIndexStr);
                var PageSize = string.IsNullOrEmpty(pageSizeStr) ? base.PageSize : Convert.ToInt32(pageSizeStr);
                WithdrawAgentType? agent = null;
                if (!string.IsNullOrEmpty(agentWay))
                {
                    var intAgent = int.Parse(agentWay);
                    if (intAgent != -1)
                    {
                        agent = (WithdrawAgentType)intAgent;
                    }
                }
                var minMaxMoney = string.IsNullOrEmpty(money) ? "-1_-1" : money;
                ViewBag.MinMaxMoney = minMaxMoney;
                var minMoney = -1M;
                var maxMoney = -1M;
                var minMaxMoneyArray = minMaxMoney.Split('_');
                if (minMaxMoneyArray.Length == 2)
                {
                    minMoney = decimal.Parse(minMaxMoneyArray[0]);
                    maxMoney = decimal.Parse(minMaxMoneyArray[1]);
                }
                var sortType = -1;
                var sortTypeStr = string.IsNullOrEmpty(slt_sort_type) ? "" : slt_sort_type;
                if (!string.IsNullOrEmpty(sortTypeStr))
                    sortType = int.Parse(sortTypeStr);
                var list = _service.QueryWithdrawList(
                       userKey, agent, (WithdrawStatus)(Convert.ToInt32(withdrawStatus)), minMoney, maxMoney, StartTime, EndTime, sortType, operUserId, PageIndex, PageSize, bankcode);

                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = "查找成功", Value = list });
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
        /// 提现订单详情
        /// </summary>
        public IActionResult WithdrawItem(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("C104"))
                    throw new Exception("对不起，您的权限不足！");
                //bool txqxdcsj = false;
                //bool txckyhxq = false;
                //bool txckddxq = false;
                //bool txcl = false;
                //if (CheckRights("TXQXDCSJ100"))
                //    txqxdcsj = true;
                //if (CheckRights("TXCKYHXQ110"))
                //    txckyhxq = true;
                //if (CheckRights("TXCKDDXQ120"))
                //    txckddxq = true;
                //if (CheckRights("TXCL130"))
                //    txcl = true;
                //ViewBag.txqxdcsj = txqxdcsj;
                //ViewBag.txckyhxq = txckyhxq;
                //ViewBag.txckddxq = txckddxq;
                //ViewBag.txcl = txcl;
                //ViewBag.OrderId = string.IsNullOrEmpty(Request["orderId"]) ? "" : Request["orderId"];
                //string userId = string.IsNullOrEmpty(Request["UserId"]) ? "" : Request["UserId"];
                var p = JsonHelper.Decode(entity.Param);
                string orderId = p.orderId;
                if (string.IsNullOrEmpty(orderId))
                {
                    throw new LogicException("参数有误");
                }
                var WithdrawInfo = _service.GetWithdrawById(orderId);
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = "查询成功",
                    Value = WithdrawInfo
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
        /// 查询用户总充值与总体现金额
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IActionResult QueryUserTotalFillDrawMoney(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string userId = p.userId;
                var fillMoney = _service.QueryUserTotalFillMoney(userId);
                var withdrawMoney = _service.QueryUserTotalWithdrawMoney(userId);
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = "查询成功",
                    Value = new
                    {
                        withdrawMoney = withdrawMoney.ToString("f2"),
                        fillMoney = fillMoney.ToString("f2")
                    }
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

        //public ContentResult QueryUserTotalDrawMoney()
        //{
        //    try
        //    {
        //        var userId = Request["userId"];
        //        var money = FundClient.QueryUserTotalWithdrawMoney(userId);
        //        return Content(money.ToString("f"));
        //    }
        //    catch (Exception)
        //    {
        //        return Content("0");
        //    }
        //}

        //public IActionResult GetRemarkInfoList()
        //{
        //    var errorList = new List<string>();
        //    errorList.Add("接受提现，已打款");
        //    errorList.Add("银行卡信息有误，请及时联系在线客服处理！");
        //    errorList.Add("银行卡账户与户名不符,请及时联系在线客服处理！");
        //    errorList.Add("您本次提款含充值账户资金，需扣除手续费，如确认提款请重新申请！");
        //    errorList.Add("您所使用的银行卡为二类三类账户，无法接收打款，请联系客户更换！");
        //    errorList.Add("您所使用的银行卡状态异常，或已经超出限额，请联系客户更换处理！");
        //    errorList.Add("活动赠送现金仅供购彩使用，不予直接提现！");
        //    errorList.Add("同一身份注册多个账号！");
        //    errorList.Add("您的账户存在异常行为，为了保障您的资金安全，请联系在线客服并提供手持身份证正面和反面照片！");
        //    //for (int i = 0; i < errorList.Count; i++)
        //    //{
        //    //    var item = errorList[i];
        //    //    result.Add(new RemarkInfo
        //    //    {
        //    //        Index = i,
        //    //        Message = item,
        //    //        Value = i == 0,
        //    //    });
        //    //}
        //    return Json;
        //}
        #region old
        //private List<RemarkInfo> GetRemarkInfoList()
        //{
        //    var result = new List<RemarkInfo>();
        //    var errorList = new List<string>();
        //    errorList.Add("接受提现，已打款");
        //    errorList.Add("银行卡信息有误，请及时联系在线客服处理！");
        //    errorList.Add("银行卡账户与户名不符,请及时联系在线客服处理！");
        //    errorList.Add("请绑定与真实身份信息一致的实名支付宝账户!");
        //    errorList.Add("支付宝账户不存在！请及时联系在线客服处理！");
        //    errorList.Add("支付宝未实名认证！");
        //    errorList.Add("活动赠送现金仅供购彩使用，不予直接提现！");
        //    errorList.Add("同一身份注册多个账号！");
        //    errorList.Add("您的账户存在异常行为，为了保障您的资金安全，请联系在线客服并提供手持身份证正面和反面照片！");
        //    for (int i = 0; i < errorList.Count; i++)
        //    {
        //        var item = errorList[i];
        //        result.Add(new RemarkInfo
        //        {
        //            Index = i,
        //            Message = item,
        //            Value = i == 0,
        //        });
        //    }
        //    return result;
        //} 
        #endregion
        private string GetDisposeMsg(string msg)
        {
            string tempMsg = string.Empty;
            switch (msg)
            {
                case "1":
                    tempMsg = "初审：资金可提现";
                    break;
                case "2":
                    tempMsg = "初审：消费未满单笔充值金额的30%不可提现";
                    break;
                case "3":
                    tempMsg = "初审：网站赠送部分只能购彩不可提现";
                    break;
            }
            return tempMsg;
        }
        /// <summary>
        /// 拒绝提现
        /// </summary>
        public IActionResult RefusedWithdraw(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string orderId = p.orderId;
                string message = p.message;
                orderId = PreconditionAssert.IsNotEmptyString(orderId, "提现订单编号为空");
                message = PreconditionAssert.IsNotEmptyString(message, "备注不能为空");
                var result = _service.RefusedWithdraw(orderId, message, CurrentUser.UserId);
                return Json(new LotteryServiceResponse
                {
                    Code = result.IsSuccess ? AdminResponseCode.成功 : AdminResponseCode.失败,
                    Message = result.Message,
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
        /// 批量驳回
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
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
                    var result = _service.RefusedWithdraw(item, "第三方打款失败，请稍候再重新申请", CurrentUser.UserId);
                }
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = "" });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        #region old
        //public IActionResult RefusedWithdrawALL()
        //{
        //    try
        //    {
        //        string orderIds = PreconditionAssert.IsNotEmptyString(Request["orderIds"], "提现订单编号为空");
        //        string[] arrs = orderIds.Split(',');
        //        if (arrs.Length == 0)
        //        {
        //            return Json(new { IsSuccess = false, Msg = "请勾选订单编号" });
        //        }
        //        foreach (var item in arrs)
        //        {
        //            var result = base.FundClient.RefusedWithdraw(item, "提现失败，请联系客服处理", CurrentUser.UserToken);

        //        }
        //        return Json(new { IsSuccess = true, Msg = "" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { IsSuccess = false, Msg = ex.Message });
        //    }
        //} 
        #endregion


        /// <summary>
        /// 完成提现
        /// </summary>
        public IActionResult CompleteWithdraw(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string orderId = p.orderId;
                string message = p.message;
                orderId = PreconditionAssert.IsNotEmptyString(orderId, "提现订单编号为空");
                message = PreconditionAssert.IsNotEmptyString(message, "备注不能为空");
                var result = _service.CompleteWithdraw(orderId, message, CurrentUser.UserId);
                //提款成功发送短信给用户
                if (result.IsSuccess)
                {
                    try
                    {
                        var with = _service.GetWithdrawById(orderId);
                        var mobile = with.RequesterMobile;
                        //var content = "尊敬的 " + with.RequesterRealName + " 您好，您提现【" + with.ResponseMoney.Value.ToString("f") + "元】已到账，请注意查收。";
                        var content = "尊敬的 " + with.RequesterRealName + " 您好，您申请【" + with.ResponseMoney.Value.ToString("f2") + "元】已处理，请注意查收。";
                        //var smsmres = SMSSenderFactory.SendSMS(mobile, content);
                        var returned = SendMessage(mobile, content);
                    }
                    catch
                    {

                    }
                }
                return Json(new LotteryServiceResponse() { Code = result.IsSuccess ? AdminResponseCode.成功 : AdminResponseCode.失败, Message = result.Message });
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


        public IActionResult CompleteWithdrawALL(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string orderIds = p.orderIds;
                orderIds = PreconditionAssert.IsNotEmptyString(orderIds, "提现订单编号为空");
                string[] arrs = orderIds.Split(',');
                string msg = string.Empty;
                var successCount = 0;
                var falseCount = 0;
                foreach (var item in arrs)
                {
                    var result = _service.CompleteWithdraw(item, "接受提现，已打款", CurrentUser.UserToken);
                    //提款成功发送短信给用户
                    if (result.IsSuccess)
                    {
                        successCount++;
                        try
                        {
                            var with = _service.GetWithdrawById(item);
                            var mobile = with.RequesterMobile;
                            //var content = "尊敬的 " + with.RequesterRealName + " 您好，您提现【" + with.ResponseMoney.Value.ToString("f") + "元】已到账，请注意查收。";
                            var content = "尊敬的 " + with.RequesterRealName + " 您好，您申请【" + with.ResponseMoney.Value.ToString("f") + "元】已处理，请注意查收。";
                            //var smsmres = SMSSenderFactory.SendSMS(mobile, content);
                            var returned = SendMessage(mobile, content);
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        falseCount++;
                    }
                }
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = "成功：" + successCount + "条;失败：" + falseCount + "条", });
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



        //public IActionResult CompleteWithdrawALL_Pay(LotteryServiceRequest entity)
        //{

        //    try
        //    {
        //        if (CurrentUser.LoginName != "caiwu01")
        //        {
        //            return Json(new { IsSuccess = false, Msg = "你没有该权限" });
        //        }
        //        var p = JsonHelper.Decode(entity.Param);
        //        string orderIds = p.orderIds;
        //        orderIds = PreconditionAssert.IsNotEmptyString(orderIds, "提现订单编号为空");
        //        string[] arrs = orderIds.Split(',');
        //        if (arrs.Length == 0)
        //        {
        //            return Json(new { IsSuccess = false, Msg = "请勾选订单编号" });
        //        }
        //        string msg = "提交成功，如果有错误会提示";
        //        foreach (var item in arrs)
        //        {
        //            var with = _service.GetWithdrawById(item);
        //            string channelid = SLF_PAY.GetInstance().GetBankNo(with.BankName);
        //            ushort channel = 0;
        //            if (channelid == "0")
        //            {
        //                msg += string.Format("该订单提交失败,原因是未找到银行对应编号,订单号:{0},银行名字：{1}\r\n", item, with.BankName);
        //                continue;
        //            }
        //            channel = ushort.Parse(channelid);
        //            var slf_pay = SLF_PAY.GetInstance();
        //            slf_pay.AppId = this.SLF_PAY_APP_ID;
        //            slf_pay.PrivateKey = this.SLF_PAY_privateKey;
        //            slf_pay.gateway_url = ConfigHelper.AllConfigInfo["Pay"]["gateway_url"]==null?"": ConfigHelper.AllConfigInfo["Pay"]["gateway_url"].ToString();
        //            var pay_msg = slf_pay.ChargePay(1, channel, item, with.ResponseMoney.Value, "127.0.0.1", "", "代付", "代付", CurrentUser.UserId, with.RequesterRealName, with.BankCardNumber);
        //            string[] arr = pay_msg.Split('|');
        //            if (arr[0] == "true")
        //            {
        //                if (arr[1] == "0")
        //                {
        //                    var result = _service.ReuqestWithdraw(item, CurrentUser.UserToken, this.SLF_PAY_APP_ID.ToString());
        //                }
        //                else
        //                {
        //                    msg += string.Format("该订单提交失败,原因是银行返回失败,请手动处理这一笔,订单号:{0},银行名字：{1}\r\n", item, with.BankName);
        //                    continue;
        //                }

        //                //var result = base.FundClient.CompleteWithdraw(item, "接受提现，已打款", CurrentUser.UserToken);
        //                //提款成功发送短信给用户
        //                //if (result.IsSuccess)
        //                //{
        //                //    try
        //                //    {
        //                //        var mobile = with.RequesterMobile;
        //                //        var content = "尊敬的 " + with.RequesterRealName + " 您好，您申请【" + with.ResponseMoney.Value.ToString("f") + "元】已处理，请注意查收。";
        //                //        var returned = SendMessage(mobile, content);
        //                //    }
        //                //    catch { }
        //                //}
        //            }
        //            else
        //            {
        //                msg += string.Format("该订单提交失败,原因是代付平台返回错误信息,订单号:{0},错误信息：{1}\r\n", item, arr[1]);
        //            }
        //        }
        //        return Json(new { IsSuccess = true, Msg = msg });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { IsSuccess = false, Msg = ex.Message });
        //    }
        //}
        //#region 顺利付
        //public ulong SLF_PAY_APP_ID
        //{
        //    get
        //    {
        //        ulong defalutValue = 11006;
        //        //return defalutValue;
        //        try
        //        {
        //            var v = ConfigHelper.AllConfigInfo["Pay"]["slf_appid"].ToString();
        //            if (string.IsNullOrEmpty(v))
        //            {
        //                return defalutValue;
        //            }
        //            else
        //            {
        //                return ulong.Parse(v);
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            return defalutValue;
        //        }
        //    }
        //}

        //public string SLF_PAY_privateKey
        //{
        //    get
        //    {
        //        string defalutValue = "";
        //        //return defalutValue;
        //        try
        //        {
        //            var v = ConfigHelper.AllConfigInfo["Pay"]["slf_privateKey"].ToString();
        //            if (string.IsNullOrEmpty(v))
        //            {
        //                return defalutValue;
        //            }
        //            else
        //            {
        //                return v;
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            return defalutValue;
        //        }
        //    }
        //}
        //#endregion

        //public IActionResult WitheraeDetail2(LotteryServiceRequest entity)
        //{
        //    try
        //    {
        //        if (CurrentUser.LoginName != "caiwu01")
        //        {
        //            return Json(new { IsSuccess = false, Msg = "你没有该权限" });
        //        }
        //        var p = JsonHelper.Decode(entity.Param);
        //        string orderIds = p.orderIds;
        //        orderIds = PreconditionAssert.IsNotEmptyString(orderIds, "提现订单编号为空");
        //        string[] arrs = orderIds.Split(',');
        //        if (arrs.Length == 0)
        //        {
        //            return Json(new { IsSuccess = false, Msg = "请勾选订单编号" });
        //        }
        //        string msg = "提交顺利付打款不成功的订单能会自动还回来申请中";
        //        var slf_pay = SLF_PAY.GetInstance();
        //        slf_pay.AppId = this.SLF_PAY_APP_ID;
        //        slf_pay.PrivateKey = this.SLF_PAY_privateKey;
        //        slf_pay.gateway_url = ConfigHelper.AllConfigInfo["Pay"]["gateway_url"].ToString();
        //        foreach (var item in arrs)
        //        {
        //            var result = slf_pay.QueryPay(item);
        //            if (result == "true|2")
        //            {
        //                _service.ReuqestWithdraw2(item, CurrentUser.UserToken, this.SLF_PAY_APP_ID.ToString());
        //            }

        //        }
        //        return Json(new { IsSuccess = true, Msg = msg });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { IsSuccess = false, Msg = ex.Message });
        //    }
        //}

        //#region "华势"
        ///// <summary>
        ///// 接口请求地址
        ///// </summary>
        //public string payworth_agentPayurl
        //{
        //    get
        //    {
        //        string defalutValue = "https://client.payworth.net/agentpay/pay";
        //        try
        //        {
        //            var v = ConfigHelper.AllConfigInfo["Pay"]["payworth_agentPayurl"].ToString();
        //            if (string.IsNullOrEmpty(v))
        //            {
        //                return defalutValue;
        //            }
        //            else
        //            {
        //                return v;
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            return defalutValue;
        //        }
        //    }
        //}
        ///// <summary>
        ///// 商户密钥
        ///// </summary>
        //public string payworth_Md5key
        //{
        //    get
        //    {
        //        string defalutValue = "95ff8e3b2ff06eb4f894e46fb028ccedc8d2294e068632e810c10bg6adgegg05";
        //        try
        //        {
        //            var v = ConfigHelper.AllConfigInfo["Pay"]["payworth_Md5key"].ToString();
        //            if (string.IsNullOrEmpty(v))
        //            {
        //                return defalutValue;
        //            }
        //            else
        //            {
        //                return v;
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            return defalutValue;
        //        }
        //    }
        //}
        ///// <summary>
        ///// 合作伙伴在华势的用户ID
        ///// </summary>
        //public string payworth_parterId
        //{
        //    get
        //    {
        //        string defalutValue = "100000000001986";
        //        try
        //        {
        //            var v = ConfigHelper.AllConfigInfo["Pay"]["payworth_parterId"].ToString();
        //            if (string.IsNullOrEmpty(v))
        //            {
        //                return defalutValue;
        //            }
        //            else
        //            {
        //                return v;
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            return defalutValue;
        //        }
        //    }
        //}
        ///// <summary>
        ///// 提交华势代付打款
        ///// </summary>
        ///// <returns></returns>
        //public IActionResult CompleteWithdrawALL_PayWorth(LotteryServiceRequest entity)
        //{
        //    try
        //    {
        //        var p = JsonHelper.Decode(entity.Param);
        //        string orderIds = p.orderIds;
        //        orderIds = PreconditionAssert.IsNotEmptyString(orderIds, "提现订单编号为空");
        //        string CerPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificate", "tomcat.cer");
        //        string[] arrs = orderIds.Split(',');
        //        if (arrs.Length == 0)
        //        {
        //            return Json(new { IsSuccess = false, Msg = "请勾选订单编号" });
        //        }
        //        string msg = "提交成功，如果有错误会提示";
        //        int count = 0;//序号
        //        //同卡在同批次中只能提交一次
        //        //string sbBankCard = string.Empty;
        //        String TempRSAStr = string.Empty;//明细串
        //        //decimal batchAmount=0;//明细金额总和
        //        foreach (var item in arrs)
        //        {
        //            var with = _service.GetWithdrawById(item);
        //            //检查是否支持该银行
        //            string bankid = Common.Pay.payworth.HttpsRequest.checkBankName(with.BankName);
        //            if (bankid == "0")
        //            {
        //                msg += string.Format("该订单提交失败,原因是华势暂不支持该银行打款,订单号:{0},银行名字：{1}\r\n", item, with.BankName);
        //                continue;
        //            }
        //            //if (sbBankCard.Contains(with.BankCardNumber))
        //            //{
        //            //    msg += string.Format("该订单提交失败,原因是同卡在同批次中只能提交一次,订单号:{0},银行卡号：{1}\r\n", item, with.BankCardNumber);
        //            //    continue;
        //            //}
        //            //sbBankCard=string.Format(",{0}",with.BankCardNumber);
        //            count++;//序号
        //            //batchAmount = batchAmount + with.RequestMoney;
        //            //序号，银行账户，开户名，开户行，分行，支行，公/私，金额，币种，省，市，手机号，证件类型，证件号，用户协议号，商户订单号，备注 
        //            TempRSAStr = string.Format("" + count + "," + with.BankCardNumber + "," + with.RequesterRealName + "," + with.BankName + "," + with.CityName + "," + with.BankSubName + ",私," + with.RequestMoney.ToString("0.00") + ",CNY," + with.ProvinceName + "," + with.CityName + "," + with.RequesterMobile + ",,,," + with.OrderId + ",");

        //            Dictionary<String, String> DateContent = new Dictionary<String, String>();
        //            DateContent.Add("batchBizid", payworth_parterId);//商户号
        //            DateContent.Add("_input_charset", "gbk");
        //            DateContent.Add("batchBiztype", "00000");
        //            DateContent.Add("batchDate", DateTime.Now.ToString("yyyyMMdd"));//提交日期
        //            DateContent.Add("batchVersion", "00");
        //            DateContent.Add("batchCurrnum", "BC" + Common.Pay.payworth.Assist.GetSerialNumber());//商户批次号
        //            DateContent.Add("batchCount", "1");//明细笔数总和(根据实际定义)
        //            DateContent.Add("batchAmount", with.RequestMoney.ToString("0.00"));//明细金额总和(根据实际定义)

        //            TempRSAStr = Common.Pay.payworth.RSAUtil.EncryptRSAByCer(TempRSAStr, CerPath);
        //            DateContent.Add("batchContent", Common.Pay.payworth.HttpsRequest.URLencode(TempRSAStr));//必须使用这个URLencode
        //            String Md5str = Common.Pay.payworth.UtilSign.GetMd5str(DateContent, payworth_Md5key);
        //            DateContent.Add("sign_type", "MD5");
        //            DateContent.Add("sign", Md5str);//
        //            String HtmlStr = Common.Pay.payworth.HttpsRequest.OpenReadWithHttps(payworth_agentPayurl, Common.Pay.payworth.HttpsRequest.GetParam(DateContent));
        //            if (HtmlStr.Contains("succ"))
        //            {
        //                var result = service.ReuqestWithdraw(item, CurrentUser.UserToken, this.payworth_parterId.ToString());
        //            }
        //            else
        //            {
        //                msg += string.Format("该订单提交失败,原因是代付平台返回错误信息,订单号:{0},错误信息：{1}\r\n", item, HtmlStr);
        //            }
        //        }
        //        return Json(new { IsSuccess = true, Msg = msg });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { IsSuccess = false, Msg = ex.Message });
        //    }
        //}
        //#endregion
        #endregion

        #region 中奖明细
        ///// <summary>
        ///// 中奖明细
        ///// </summary>
        ///// <returns></returns>
        //public IActionResult WinDetail(LotteryServiceRequest entity)
        //{
        //    try
        //    {
        //        var p = JsonHelper.Decode(entity.Param);
        //        string userKey = p.userKey;
        //        string gameCode = p.gameCode;
        //        string pageIndexStr = p.pageIndex;
        //        string pageSizeStr = p.pageSize;
        //        string startTimeStr = p.startTime;
        //        string endTimeStr = p.endTime;
        //        string strSortAmount = p.strSortAmount;
        //        string strSortdat = p.strSortdat;
        //        string flag = p.flag;
        //        //ViewData["images"] = Request["strImage"] == null ? ConfigHelper.AllConfigInfo["ShareRes"].ToString() + "/images/desc.png" : Request["strImage"].ToString();
        //        //ViewData["Amount_img"] = Request["strAmountImage"] == null ? ConfigHelper.AllConfigInfo["ShareRes"].ToString() + "/images/desc.png" : Request["strAmountImage"].ToString();
        //        //ViewBag.GameList = base.GameIssuseClient.QueryGameList(CurrentUser.UserToken);
        //        var UserKey = string.IsNullOrEmpty(userKey) ? null : userKey;
        //        var GameCode = string.IsNullOrEmpty(gameCode) ? null : gameCode;
        //        var PageIndex = string.IsNullOrEmpty(pageIndexStr) ? base.PageIndex : Convert.ToInt32(pageIndexStr);
        //        var PageSize = string.IsNullOrEmpty(pageSizeStr) ? base.PageSize : Convert.ToInt32(pageSizeStr);
        //        var StartTime = string.IsNullOrEmpty(startTimeStr) ? DateTime.Now.AddMonths(-1) : Convert.ToDateTime(startTimeStr);
        //        var EndTime = string.IsNullOrEmpty(endTimeStr) ? DateTime.Now : Convert.ToDateTime(endTimeStr);
        //        var SortAmount = string.IsNullOrEmpty(strSortAmount) ? "desc:1" : strSortAmount;
        //        var Sortdat = string.IsNullOrEmpty(strSortdat) ? "desc:1" : strSortdat;
        //        flag = string.IsNullOrEmpty(flag) ? "" : flag;
        //        //if (ViewBag.flag == "1")
        //        //{
        //        //    ViewBag.BonusDetail = base.FundClient.QueryBonusDetail(ViewBag.UserKey, ViewBag.GameCode, ViewBag.StartTime, ViewBag.EndTime.AddDays(+1), ViewBag.PageIndex, ViewBag.PageSize, ViewBag.Sortdat, CurrentUser.UserToken);
        //        //}
        //        //else if (ViewBag.flag == "2")
        //        //{
        //        //    ViewBag.BonusDetail = base.FundClient.QueryBonusDetail(ViewBag.UserKey, ViewBag.GameCode, ViewBag.StartTime, ViewBag.EndTime.AddDays(+1), ViewBag.PageIndex, ViewBag.PageSize, ViewBag.SortAmount, CurrentUser.UserToken);
        //        //}
        //        //else
        //        //{
        //        //    ViewBag.BonusDetail = base.FundClient.QueryBonusDetail(ViewBag.UserKey, ViewBag.GameCode, ViewBag.StartTime, ViewBag.EndTime.AddDays(+1), ViewBag.PageIndex, ViewBag.PageSize, "desc:1", CurrentUser.UserToken);
        //        //}
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        #endregion



        #region 资金明细查询
        /////// <summary>
        /////// 财务资金明细报表
        /////// </summary>
        //public IActionResult NewMoneyDetail(string id)
        //{
        //    try
        //    {
        //        if (!CheckRights("C114"))
        //            throw new Exception("对不起，您的权限不足！");
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        /// <summary>
        /// 资金明细报表查询
        /// </summary>
        public IActionResult NewMoneyDetail_Inner(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string userKey = p.userKey;
                string showUser = p.showUser;
                string pageSizeStr = p.pageSize;
                string typeList = p.typeList;
                string startTimeStr = p.startTime;
                string endTimeStr = p.endTime;
                string pageIndexStr = p.pageIndex;
                string keyLine = p.keyLine;
                string operateType = p.operateType;
                //if (!string.IsNullOrEmpty(id))
                //{
                //    ViewBag.UserKey = id.Split('|')[0];
                //    ViewBag.ShowUser = bool.Parse(id.Split('|')[1]);
                //    ViewBag.PageSize = int.Parse(id.Split('|')[2]);
                //}
                //else
                //{
                //    ViewBag.UserKey = Request["userKey"];
                //    ViewBag.ShowUser = string.IsNullOrEmpty(Request["showUser"]) ? true : Convert.ToBoolean(Request["showUser"]);
                //    ViewBag.PageSize = string.IsNullOrEmpty(Request["pagesize_f"]) ? base.PageSize : Convert.ToInt32(Request["pagesize_f"]);
                //}
                typeList = string.IsNullOrEmpty(typeList) ? "10|20|30|50|70" : typeList.Trim('|');
                var StartTime = string.IsNullOrEmpty(startTimeStr) ? DateTime.Today : Convert.ToDateTime(startTimeStr);
                var EndTime = string.IsNullOrEmpty(endTimeStr) ? DateTime.Now : Convert.ToDateTime(endTimeStr);
                var PageIndex = string.IsNullOrEmpty(pageIndexStr) ? base.PageIndex : Convert.ToInt32(pageIndexStr);
                var PageSize = string.IsNullOrEmpty(pageSizeStr) ? base.PageIndex : Convert.ToInt32(pageSizeStr);
                operateType = string.IsNullOrEmpty(operateType) ? "" : operateType.ToString();
                var FundDetails = _service.QueryUserFundDetailListReport(userKey, keyLine, StartTime, EndTime, typeList, operateType, PageIndex, PageSize);
                return Json(new LotteryServiceResponse()
                {
                    Code = AdminResponseCode.成功,
                    Message = "",
                    Value = FundDetails
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

        ///// <summary>
        ///// 资金明细查询
        ///// </summary>
        //public IActionResult MoneyDetail(string id)
        //{
        //    try
        //    {
        //        if (!CheckRights("C101"))
        //            throw new Exception("对不起，您的权限不足！");
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        /// <summary>
        /// 资金明细查询
        /// </summary>
        public IActionResult MoneyDetail_Inner(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("CKZJMX240"))
                {
                    throw new LogicException("对不起,您的权限不足");
                }
                var p = JsonHelper.Decode(entity.Param);
                string UserKey = string.Empty;
                if (!string.IsNullOrEmpty((string)p.id))
                {
                    UserKey = (string)p.id.Split('|')[0];
                    bool ShowUser = bool.Parse((string)p.id.Split('|')[1]);
                    int PageSize = int.Parse((string)p.id.Split('|')[2]);
                }
                else
                {
                    UserKey = (string)p.userKey;
                    bool ShowUser = string.IsNullOrWhiteSpace((string)p.showUser) ? true : Convert.ToBoolean((string)p.showUser);
                    int PageSize = string.IsNullOrWhiteSpace((string)p.pagesize_f) ? base.PageSize : Convert.ToInt32((string)p.pagesize_f);
                }
                string TypeList = string.IsNullOrWhiteSpace((string)p.typeList) ? "10|20|30|50|70" : (string)p.typeList.TrimStart('|');
                DateTime StartTime = string.IsNullOrWhiteSpace((string)p.startTime) ? DateTime.Today : Convert.ToDateTime((string)p.startTime);
                DateTime EndTime = string.IsNullOrWhiteSpace((string)p.endTime) ? DateTime.Now : Convert.ToDateTime((string)p.endTime);
                int PageIndex = string.IsNullOrWhiteSpace((string)p.pageindex_f) ? base.PageIndex : Convert.ToInt32((string)p.pageindex_f);
                string sel_OperateType = string.IsNullOrWhiteSpace((string)p.sel_OperateType) ? "" : (string)p.sel_OperateType.ToString();
                string KeyLine = (string)p.keyLine;

                var FundDetails = _service.QueryUserFundDetailList(UserKey, KeyLine, StartTime, EndTime, TypeList, sel_OperateType, PageIndex, PageSize);
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Value = FundDetails });
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
        /// 查询返点明细
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IActionResult QueryCommissionDetail(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("C112"))
                {
                    throw new LogicException("对不起,您的权限不足");
                }
                var p = JsonHelper.Decode(entity.Param);
                string userKey = p.userKey;
                string startTimeStr = p.startTime;
                string endTimeStr = p.endTime;
                string pageIndexStr = p.pageIndex;
                string pageSizeStr = p.pageSize;
                var StartTime = string.IsNullOrEmpty(startTimeStr) ? DateTime.Today : Convert.ToDateTime(startTimeStr);
                var EndTime = string.IsNullOrEmpty(endTimeStr) ? DateTime.Now : Convert.ToDateTime(endTimeStr);
                var PageIndex = string.IsNullOrEmpty(pageIndexStr) ? base.PageIndex : Convert.ToInt32(pageIndexStr);
                var PageSize = string.IsNullOrEmpty(pageSizeStr) ? base.PageSize : Convert.ToInt32(pageSizeStr);
                var FundDetails = _service.QueryUserFundDetail_Commission(userKey, StartTime, EndTime, PageIndex, PageSize);
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = "查询成功",
                    Value = FundDetails
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
        /// 查询资金明细
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IActionResult MoneyDetail_Inner_Withdraw(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("C101"))
                {
                    throw new LogicException("对不起,您的权限不足");
                }
                var p = JsonHelper.Decode(entity.Param);
                string userKey = p.userKey;
                string pageSizeStr = p.pageSize;
                string typeList = p.typeList;
                string startTimeStr = p.startTime;
                string endTimeStr = p.endTime;
                string pageIndexStr = p.pageIndex;
                string operateType = p.operateType;
                string keyLine = p.keyLine;
                var PageSize = string.IsNullOrEmpty(pageSizeStr) ? base.PageSize : Convert.ToInt32(pageSizeStr);
                var TypeList = string.IsNullOrEmpty(typeList) ? "10|20|30|50|60|70" : typeList.Trim('|');
                var StartTime = string.IsNullOrEmpty(startTimeStr) ? DateTime.Today : Convert.ToDateTime(startTimeStr);
                var EndTime = string.IsNullOrEmpty(endTimeStr) ? DateTime.Now : Convert.ToDateTime(endTimeStr);
                var PageIndex = string.IsNullOrEmpty(pageIndexStr) ? base.PageIndex : Convert.ToInt32(pageIndexStr);
                operateType = string.IsNullOrEmpty(operateType) ? "" : operateType.ToString();
                var FundDetails = _service.QueryUserFundDetailList(userKey, keyLine, StartTime, EndTime, TypeList, operateType, PageIndex, PageSize);
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = "查询成功",
                    Value = FundDetails
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

        #region 财务人员设置

        public IActionResult FinanceSetting(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("C105"))
                    throw new Exception("对不起，您的权限不足！");
                //bool sccw = false;
                //bool xgcwry = false;
                //if (CheckRights("SCCW100"))
                //    sccw = true;
                //if (CheckRights("XGCWRY120"))
                //    xgcwry = true;
                //ViewBag.sccw = sccw;
                //ViewBag.xgcwry = xgcwry;
                var p = JsonHelper.Decode(entity.Param);
                string userId = p.userId;
                //string startTimeStr = p.startTime;
                //string endTimeStr = p.endTime;
                string pageIndexStr = p.pageIndex;
                string pageSizeStr = p.pageSize;
                var UserId = string.IsNullOrEmpty(userId) ? "" : userId.ToString();
                //var StartTime = string.IsNullOrEmpty(startTimeStr) ? DateTime.Now : Convert.ToDateTime(startTimeStr);
                //var EndTime = string.IsNullOrEmpty(endTimeStr) ? DateTime.Now : Convert.ToDateTime(endTimeStr);
                var PageIndex = string.IsNullOrEmpty(pageIndexStr) ? base.PageIndex : int.Parse(pageIndexStr);
                var PageSize = string.IsNullOrEmpty(pageSizeStr) ? base.PageSize : int.Parse(pageSizeStr);
                var FinanceSettingList = _service.GetFinanceSettingsCollection(UserId, PageIndex, PageSize);
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = "查询成功",
                    Value = FinanceSettingList
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

        public IActionResult FinanceSettingList(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("C106"))
                    throw new Exception("对不起，您的权限不足！");
                //bool xgcw = false;
                //if (CheckRights("XGCW100"))
                //    xgcw = true;
                //ViewBag.xgcw = xgcw;
                var p = JsonHelper.Decode(entity.Param);
                string financeId = p.financeId;
                var FinanceUser = _service.GetCaiWuOperator();
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = "查询成功",
                    Value = FinanceUser
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
        /// 查询财务人员列表及某个财务人员明细
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IActionResult FinanceSettingInfo(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("C106"))
                    throw new Exception("对不起，您的权限不足！");
                //bool xgcw = false;
                //if (CheckRights("XGCW100"))
                //    xgcw = true;
                //ViewBag.xgcw = xgcw;
                var p = JsonHelper.Decode(entity.Param);
                string financeId = p.financeId;
                if (string.IsNullOrEmpty(financeId))
                {
                    throw new LogicException("参数有误");
                }
                var FinanceInfo = _service.GetFinanceSettingsByFinanceId(financeId);
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = "查询成功",
                    Value = FinanceInfo
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



        public IActionResult OperateFinanceSetting(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string type = p.type;
                string userId = p.userId;
                string operateType = p.operateType;
                string maxMoney = p.maxMoney;
                string minMoney = p.minMoney;
                string financeId = p.financeId;
                if (string.IsNullOrEmpty(type))
                {
                    throw new LogicException("参数有误");
                }
                C_FinanceSettings info = new C_FinanceSettings();
                if (type != "add")
                {
                    info.FinanceId = Convert.ToInt32(financeId);
                }
                if (type != "delete")
                {
                    info.UserId = PreconditionAssert.IsNotEmptyString(userId, "财务人员不能为空！");
                    info.OperateType = PreconditionAssert.IsNotEmptyString(operateType, "操作类型不能为空！");
                    //info.OperateRank = PreconditionAssert.IsNotEmptyString(Request["OperateRank"], "操作级别不能为空!");
                    if (minMoney == null || maxMoney == null)
                    {
                        throw new LogicException("金额不能为空");
                    }
                    else if (Convert.ToDecimal(minMoney) > Convert.ToDecimal(maxMoney))
                    {
                        PreconditionAssert.IsNotEmptyString("", "最小金额不能大于最大金额!");
                    }
                    info.MinMoney = Convert.ToDecimal(minMoney);
                    info.MaxMoney = Convert.ToDecimal(maxMoney);
                }
                switch (type.ToString().ToLower())
                {
                    case "add":
                        var flag = _service.FinanceSetting("add", info, CurrentUser.UserId);
                        return Json(new LotteryServiceResponse
                        {
                            Code = flag.IsSuccess?AdminResponseCode.成功: AdminResponseCode.失败,
                            Message = flag.Message
                        });

                    case "update":
                        flag = _service.FinanceSetting("update", info, CurrentUser.UserId);
                        return Json(new LotteryServiceResponse
                        {
                            Code = flag.IsSuccess ? AdminResponseCode.成功 : AdminResponseCode.失败,
                            Message = flag.Message
                        });

                    case "delete":
                        flag = _service.FinanceSetting("delete", info, CurrentUser.UserId);
                        return Json(new LotteryServiceResponse
                        {
                            Code = flag.IsSuccess ? AdminResponseCode.成功 : AdminResponseCode.失败,
                            Message = flag.Message
                        });
                    default:
                        return Json(new { IsSuccess = false, Msg = "操作失败！" });
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

        #endregion

        //#region "艾付代付"
        //string af_merchant_no = ConfigHelper.AllConfigInfo["Pay"]["af_merchant_no"] == null ? "155805008498" : ConfigHelper.AllConfigInfo["Pay"]["af_merchant_no"].ToString();
        //string af_key = ConfigHelper.AllConfigInfo["Pay"]["af_key"] == null ? "58c1ecf1-5bf1-11e8-b565-b51b2f239b86" : ConfigHelper.AllConfigInfo["Pay"]["af_key"].ToString();
        //string af_url = ConfigHelper.AllConfigInfo["Pay"]["af_url"] == null ? "http://pay.ifeepay.com/withdraw/singleWithdraw" : ConfigHelper.AllConfigInfo["Pay"]["af_url"].ToString();
        ///// <summary>
        ///// 代付支付密码998909
        ///// </summary>
        //string af_pay_pwd = ConfigHelper.AllConfigInfo["Pay"]["af_pay_pwd"] == null ? "111234" : ConfigHelper.AllConfigInfo["Pay"]["af_pay_pwd"].ToString();
        ///// <summary>
        ///// 提交艾付代付打款
        ///// </summary>
        ///// <returns></returns>
        //public JsonResult CompleteWithdrawALL_AF(LotteryServiceRequest entity)
        //{
        //    try
        //    {
        //        var p = JsonHelper.Decode(entity.Param);
        //        string orderIds = p.orderIds;
        //        orderIds = PreconditionAssert.IsNotEmptyString(orderIds, "提现订单编号为空");
        //        string[] arrs = orderIds.Split(',');
        //        string msg = "提交成功，如果有错误会提示";
        //        String TempRSAStr = string.Empty;//明细串
        //        foreach (var item in arrs)
        //        {
        //            var with = _service.GetWithdrawById(item);
        //            //检查是否支持该银行
        //            string bankCode = Common.Pay.af.afPay.checkBankName(with.BankName);
        //            if (bankCode == "0")
        //            {
        //                msg += string.Format("该订单提交失败,原因是艾付暂不支持该银行打款,订单号:{0},银行名字：{1}\r\n", item, with.BankName);
        //                continue;
        //            }
        //            //MD5签名串
        //            string src = string.Format(@"merchant_no={0}&order_no={1}&card_no={2}&account_name={3}&bank_branch={4}&cnaps_no={5}&bank_code={6}&bank_name={7}&amount={8}&pay_pwd={9}",
        //                this.af_merchant_no, with.OrderId, with.BankCardNumber, Common.Pay.af.afUtil.EncodeBase64(Encoding.UTF8, with.RequesterRealName), "", "", bankCode, Common.Pay.af.afUtil.EncodeBase64(Encoding.UTF8, with.BankName), with.ResponseMoney.Value.ToString("f2"), this.af_pay_pwd);
        //            src += "&key=" + af_key;
        //            string sign = Common.Pay.af.afUtil.MD5(src, Encoding.UTF8);
        //            src += "&sign=" + sign;

        //            string responseStr = Common.Pay.af.afUtil.HttpPost(af_url + "?" + src.ToString(), "", Encoding.UTF8);
        //            var json = Newtonsoft.Json.Linq.JObject.Parse(responseStr);
        //            //代付申请成功
        //            if (json["result_code"].ToString() == "000000")
        //            {
        //                var result = base.FundClient.ReuqestWithdraw(item, CurrentUser.UserToken, this.af_merchant_no.ToString());
        //            }
        //            else
        //            {
        //                msg += string.Format("该订单提交失败,原因是代付平台返回错误信息,订单号:{0},错误信息：{1}\r\n", item, json["result_msg"].ToString());
        //            }
        //        }
        //        return Json(new { IsSuccess = true, Msg = msg });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { IsSuccess = false, Msg = ex.Message });
        //    }
        //}

        //#endregion

        //#region "神付代付"
        //string shenfu_userid = ConfigHelper.AllConfigInfo["Pay"]["shenfu_userid"] == null ? "030adaa43bc846e7ad52f9bd86d8fc6b" : ConfigHelper.AllConfigInfo["Pay"]["shenfu_userid"].ToString();
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public JsonResult CompleteWithdrawALL_SF(LotteryServiceRequest entity)
        //{
        //    // string shenfu_userid = ConfigHelper.AllConfigInfo["Pay"]["shenfu_userid"].ToString() == "" ? "030adaa43bc846e7ad52f9bd86d8fc6b" : ConfigHelper.AllConfigInfo["Pay"]["shenfu_userid"].ToString();
        //    string shenfu_userName = ConfigHelper.AllConfigInfo["Pay"]["shenfu_userName"] == null ? "wc" : ConfigHelper.AllConfigInfo["Pay"]["shenfu_userName"].ToString();
        //    string shenfu_key = ConfigHelper.AllConfigInfo["Pay"]["shenfu_key"] == null ? "e10a9875e95b4645828dab8b3f6d62e7" : ConfigHelper.AllConfigInfo["Pay"]["shenfu_key"].ToString();
        //    try
        //    {
        //        var p = JsonHelper.Decode(entity.Param);
        //        string orderIds = p.orderIds;
        //        orderIds = PreconditionAssert.IsNotEmptyString(orderIds, "提现订单编号为空");
        //        string[] arrs = orderIds.Split(',');
        //        string msg = "提交成功，如果有错误会提示";
        //        String TempRSAStr = string.Empty;//明细串
        //        foreach (var item in arrs)
        //        {
        //            var with = _service.GetWithdrawById(item, CurrentUser.UserToken);
        //            //检查是否支持该银行
        //            string bankCode = Common.Pay.af.afPay.checkBankName(with.BankName);
        //            if (bankCode == "0")
        //            {
        //                msg += string.Format("该订单提交失败,原因是神付暂不支持该银行打款,订单号:{0},银行名字：{1}\r\n", item, with.BankName);
        //                continue;
        //            }

        //            Common.Pay.shenfu.WithdrawRequest request = new Common.Pay.shenfu.WithdrawRequest()
        //            {
        //                orderId = with.OrderId,
        //                amount = Convert.ToInt32(with.ResponseMoney.Value * 100),
        //                customerUserId = with.OrderId.Replace("MWD", ""),
        //                comment = "代付打款",
        //                notifyUrl = this.FillMoneyCallBackDomain + "/user/ShenFuDFNotifyUrl",//"http://paytz.wc878.com/ShenFuDFNotifyUrl",
        //                account = with.BankCardNumber,
        //                bank = with.BankName,
        //                realName = with.RequesterRealName,
        //                //bank = Common.Pay.af.afUtil.EncodeBase64(Encoding.UTF8, with.BankName),
        //                //realName = Common.Pay.af.afUtil.EncodeBase64(Encoding.UTF8, with.RequesterRealName),
        //                bankProvince = with.ProvinceName,
        //                bankCity = with.CityName,
        //                bankBranchName = with.BankSubName,
        //                payMethod = 2,
        //                payChannel = 100

        //            };

        //            string json = Common.Pay.shenfu.sfpay.Serialize(request);
        //            string resultString = Common.Pay.shenfu.sfpay.PostJson(string.Format("{0}?customer={1}&sign={2}", "https://s.wcoes.com/paygate/api/withdraw", shenfu_userid, Common.Pay.shenfu.sfpay.MD5(json, shenfu_key)), json);

        //            Common.Pay.shenfu.Result<string> result = new Common.Pay.shenfu.sfpay().Deserialize<Common.Pay.shenfu.Result<string>>(resultString);
        //            //代付申请成功
        //            if (result.succeed)
        //            {
        //                var results = base.FundClient.ReuqestWithdraw(item, CurrentUser.UserToken, shenfu_userid.ToString());
        //            }
        //            else
        //            {
        //                msg += string.Format("该订单提交失败,原因是代付平台返回错误信息,订单号:{0},错误信息：{1}\r\n", item, result.message.ToString());
        //            }
        //        }
        //        return Json(new { IsSuccess = true, Msg = msg });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { IsSuccess = false, Msg = ex.Message });
        //    }
        //}

        ///// <summary>
        ///// 天下付代付
        ///// </summary>
        ///// <returns></returns>
        //public JsonResult CompleteWithdrawALL_TXF(LotteryServiceRequest entity)
        //{
        //    string tfb_Key = "s,Kw70ESV7";
        //    string tfb_spid = "1800113378";
        //    try
        //    {
        //        var p = JsonHelper.Decode(entity.Param);
        //        string orderIds = p.orderIds;
        //        orderIds = PreconditionAssert.IsNotEmptyString(orderIds, "提现订单编号为空");
        //        string[] arrs = orderIds.Split(',');
        //        string msg = "提交成功，如果有错误会提示";
        //        String TempRSAStr = string.Empty;//明细串
        //        foreach (var item in arrs)
        //        {
        //            var with = _service.GetWithdrawById(item, CurrentUser.UserToken);
        //            //检查是否支持该银行
        //            //string bankCode = Common.Pay.af.afPay.checkBankName(with.BankName);
        //            //if (bankCode == "0")
        //            //{
        //            //    msg += string.Format("该订单提交失败,原因是神付暂不支持该银行打款,订单号:{0},银行名字：{1}\r\n", item, with.BankName);
        //            //    continue;
        //            //}
        //            var code = Common.Pay.tianfubao.Pay.txfdfResult(tfb_spid, item, Convert.ToInt32(with.ResponseMoney.Value * 100).ToString(), with.RequesterRealName, with.BankCardNumber, tfb_Key, "http://api.tfb8.com/cgi-bin/v2.0/api_pay_single.cgi");

        //            //代付申请成功
        //            if (code.Split('|')[0] == "0")
        //            {
        //                //var results = base.FundClient.ReuqestWithdraw(item, CurrentUser.UserToken, shenfu_userid.ToString());
        //                var result = _service.CompleteWithdraw(item, "接受提现,完成打款", CurrentUser.UserToken);

        //                //提款成功发送短信给用户
        //                if (result.IsSuccess)
        //                {
        //                    try
        //                    {
        //                        var mobile = with.RequesterMobile;
        //                        var content = "尊敬的 " + with.RequesterRealName + " 您好，您申请【" + with.ResponseMoney.Value.ToString("f") + "元】已处理，请注意查收。";
        //                        var returned = SendMessage(mobile, content);
        //                    }
        //                    catch
        //                    {

        //                    }
        //                }
        //            }
        //            else if (code.Split('|')[0] == "1")
        //            {
        //                var results = _service.ReuqestWithdraw(item, CurrentUser.UserToken, shenfu_userid.ToString());
        //            }
        //            else
        //            {
        //                msg += string.Format("该订单提交失败,原因是代付平台返回错误信息,订单号:{0},错误信息：{1}\r\n", item, code.Split('|')[0] + "_" + code.Split('|')[1]);
        //            }
        //        }
        //        return Json(new { IsSuccess = true, Msg = msg });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { IsSuccess = false, Msg = ex.Message });
        //    }
        //}


        //public JsonResult CompleteWithdrawALL_TXF2(LotteryServiceRequest entity)
        //{
        //    string tfb_Key = "s,Kw70ESV7";
        //    string tfb_spid = "1800113378";
        //    try
        //    {
        //        var p = JsonHelper.Decode(entity.Param);
        //        string orderIds = p.orderIds;
        //        orderIds = PreconditionAssert.IsNotEmptyString(orderIds, "提现订单编号为空");
        //        string[] arrs = orderIds.Split(',');     
        //        string msg = "提交查询成功，刷新该页面";
        //        String TempRSAStr = string.Empty;//明细串
        //        foreach (var item in arrs)
        //        {
        //            var with = _service.GetWithdrawById(item, CurrentUser.UserToken);
        //            if (with.AgentId != shenfu_userid)
        //                continue;
        //            //检查是否支持该银行
        //            //string bankCode = Common.Pay.af.afPay.checkBankName(with.BankName);
        //            //if (bankCode == "0")
        //            //{
        //            //    msg += string.Format("该订单提交失败,原因是神付暂不支持该银行打款,订单号:{0},银行名字：{1}\r\n", item, with.BankName);
        //            //    continue;
        //            //}
        //            var code = Common.Pay.tianfubao.Pay.txfdfquery(tfb_spid, item, tfb_Key, "http://api.tfb8.com/cgi-bin/v2.0/api_pay_single_query.cgi");

        //            //代付申请成功
        //            if (code.Split('|')[0] == "0")
        //            {
        //                //var results = base.FundClient.ReuqestWithdraw2(item, CurrentUser.UserToken, shenfu_userid.ToString());
        //                var result = _service.CompleteWithdraw(item, "接受提现，已打款", CurrentUser.UserToken);
        //            }
        //            else
        //            {
        //                _service.ReuqestWithdraw2(item, CurrentUser.UserToken, shenfu_userid.ToString());
        //            }
        //        }
        //        return Json(new { IsSuccess = true, Msg = msg });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { IsSuccess = false, Msg = ex.Message });
        //    }
        //}
        //#endregion

        //#region 合付宝代付
        //public JsonResult CompleteWithdrawALL_HFB(LotteryServiceRequest entity)
        //{
        //    string hfb_merchantNo = ConfigHelper.AllConfigInfo["Pay"]["hfb.merchantNo"].ToString();
        //    Common.Pay.hfb.hfbpay.GetInstance().SignCertPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificate", ConfigHelper.AllConfigInfo["Pay"]["hfb.signCert.path"].ToString());
        //    Common.Pay.hfb.hfbpay.GetInstance().PublicCertPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificate", ConfigHelper.AllConfigInfo["Pay"]["hfb.publicCert.path"].ToString());
        //    Common.Pay.hfb.hfbpay.GetInstance().SignCertPwd = ConfigHelper.AllConfigInfo["Pay"]["hfb.signCert.pwd"].ToString();
        //    try
        //    {
        //        var p = JsonHelper.Decode(entity.Param);
        //        string orderIds = p.orderIds;
        //        orderIds = PreconditionAssert.IsNotEmptyString(orderIds, "提现订单编号为空");
        //        string[] arrs = orderIds.Split(',');
        //        string msg = "提交成功，如果有错误会提示";
        //        String TempRSAStr = string.Empty;//明细串
        //        foreach (var item in arrs)
        //        {
        //            var with = _service.GetWithdrawById(item, CurrentUser.UserToken);
        //            //检查是否支持该银行
        //            string bankCode = Common.Pay.af.afPay.checkBankName(with.BankName);
        //            if (bankCode == "0")
        //            {
        //                msg += string.Format("该订单提交失败,原因是神付暂不支持该银行打款,订单号:{0},银行名字：{1}\r\n", item, with.BankName);
        //                continue;
        //            }
        //            string money = Convert.ToInt32(with.ResponseMoney.Value * 100).ToString();
        //            var result = Common.Pay.hfb.hfbpay.daifu("https://cashier.hefupal.com/paygate/v1/dfpay", with.BankCardNumber, with.RequesterRealName, with.BankName,
        //                money, CurrentUser.UserId, hfb_merchantNo, with.OrderId, this.FillMoneyCallBackDomain + "/user/HFBDFNotifyUrl");
        //            //代付申请成功
        //            if (result.Split('|')[0] == "0")
        //            {
        //                var results = _service.ReuqestWithdraw(item, CurrentUser.UserToken, shenfu_userid.ToString());
        //            }
        //            else
        //            {
        //                msg += string.Format("该订单提交失败,原因是代付平台返回错误信息,订单号:{0},错误信息：{1}\r\n", item, result.Split('|')[1]);
        //            }
        //        }
        //        return Json(new { IsSuccess = true, Msg = msg });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { IsSuccess = false, Msg = ex.Message });
        //    }
        //}
        //#endregion

        //#region 提交鲨鱼代付
        //public JsonResult CompleteWithdrawALL_SY(LotteryServiceRequest entity)
        //{
        //    string url = "http://106.14.211.216:51243/payment/WithdrawApply.do";
        //    string custid = "00000000519066";
        //    string key = "mWJooFJv1knf";
        //    try
        //    {
        //        var p = JsonHelper.Decode(entity.Param);
        //        string orderIds = p.orderIds;
        //        orderIds = PreconditionAssert.IsNotEmptyString(orderIds, "提现订单编号为空");
        //        string[] arrs = orderIds.Split(',');
        //        string msg = "提交成功，如果有错误会提示";
        //        String TempRSAStr = string.Empty;//明细串
        //        foreach (var item in arrs)
        //        {
        //            var with = _service.GetWithdrawById(item);
        //            //检查是否支持该银行
        //            string bankCode = Common.Pay.af.afPay.checkBankName(with.BankName);
        //            if (bankCode == "0")
        //            {
        //                msg += string.Format("该订单提交失败,原因是神付暂不支持该银行打款,订单号:{0},银行名字：{1}\r\n", item, with.BankName);
        //                continue;
        //            }

        //            string money = Convert.ToInt32((with.ResponseMoney.Value + 5) * 100).ToString();
        //            var result = Common.Pay.shayupay.shayupaying.daifu(url, custid, with.OrderId, money, this.FillMoneyCallBackDomain + "/user/SYDFNotifyUrl",
        //               with.RequesterMobile, with.RequesterRealName, with.RequesterRealName, with.BankName,
        //                with.BankCardNumber, key);
        //            //代付申请成功
        //            if (result.Split('|')[0] == "0")
        //            {
        //                var results = _service.ReuqestWithdraw(item, CurrentUser.UserToken, custid);
        //            }
        //            else
        //            {
        //                msg += string.Format("该订单提交失败,原因是代付平台返回错误信息,订单号:{0},错误信息：{1}\r\n", item, result.Split('|')[1]);
        //            }
        //        }
        //        return Json(new { IsSuccess = true, Msg = msg });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { IsSuccess = false, Msg = ex.Message });
        //    }
        //}
        //#endregion

        //#region 金海哲
        //public JsonResult CompleteWithdrawALL_JHZ()
        //{
        //    try
        //    {
        //        var p = JsonHelper.Decode(entity.Param);
        //        string orderIds = p.orderIds;
        //        orderIds = PreconditionAssert.IsNotEmptyString(orderIds, "提现订单编号为空");
        //        string[] arrs = orderIds.Split(',');
        //        string msg = "提交成功，如果有错误会提示";
        //        String TempRSAStr = string.Empty;//明细串
        //        foreach (var item in arrs)
        //        {
        //            var with = _service.GetWithdrawById(item, CurrentUser.UserToken);
        //            string money = Convert.ToInt32((with.ResponseMoney.Value) * 100).ToString();
        //            var result = Common.Pay.jhz.JHZPayAPI.daifu(with.OrderId, money, with.RequesterRealName, with.RequesterMobile, with.BankCardNumber);
        //            //代付申请成功
        //            if (result.Split('|')[0] == "0")
        //            {
        //                var results = _service.ReuqestWithdraw(item, CurrentUser.UserToken, "500000871696");
        //            }
        //            else
        //            {
        //                msg += string.Format("该订单提交失败,原因是代付平台返回错误信息,订单号:{0},错误信息：{1}\r\n", item, result.Split('|')[1]);
        //            }
        //        }
        //        return Json(new { IsSuccess = true, Msg = msg });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { IsSuccess = false, Msg = ex.Message });
        //    }
        //}

        //public JsonResult CompleteWithdrawALL_JHZ_SX()
        //{
        //    try
        //    {
        //        string orderIds = PreconditionAssert.IsNotEmptyString(Request["orderIds"], "提现订单编号为空");
        //        string[] arrs = orderIds.Split(',');
        //        if (arrs.Length == 0)
        //        {
        //            return Json(new { IsSuccess = false, Msg = "请勾选订单编号" });
        //        }
        //        string msg = "提交查询成功，刷新该页面";
        //        String TempRSAStr = string.Empty;//明细串
        //        foreach (var item in arrs)
        //        {
        //            var with = base.FundClient.GetWithdrawById(item, CurrentUser.UserToken);
        //            if (with.AgentId != "500000871696")
        //                continue;
        //            //检查是否支持该银行
        //            //string bankCode = Common.Pay.af.afPay.checkBankName(with.BankName);
        //            //if (bankCode == "0")
        //            //{
        //            //    msg += string.Format("该订单提交失败,原因是神付暂不支持该银行打款,订单号:{0},银行名字：{1}\r\n", item, with.BankName);
        //            //    continue;
        //            //}
        //            var code = Common.Pay.jhz.JHZPayAPI.querydaifu(item);

        //            //代付申请成功
        //            if (code.Split('|')[0] == "0")
        //            {
        //                var result = base.FundClient.CompleteWithdraw(item, "接受提现，已打款", CurrentUser.UserToken);
        //            }
        //            else
        //            {
        //                base.FundClient.ReuqestWithdraw2(item, CurrentUser.UserToken, "500000871696");
        //            }
        //        }
        //        return Json(new { IsSuccess = true, Msg = msg });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { IsSuccess = false, Msg = ex.Message });
        //    }
        //}
        //#endregion

        #region 网站结余统计
        //public ActionResult QueryUserBalanceReportList(LotteryServiceRequest entity)
        //{
        //    if (!CheckRights("C110"))
        //        throw new Exception("对不起，您的权限不足！");
        //    ViewBag.StartTime = Request["StartTime"] == null ? DateTime.Now : Convert.ToDateTime(Request["StartTime"]);
        //    ViewBag.EndTime = Request["EndTime"] == null ? DateTime.Now : Convert.ToDateTime(Request["EndTime"]);
        //    ViewBag.PageIndex = Request["PageIndex"] == null ? this.PageIndex : Convert.ToInt32(Request["PageIndex"]);
        //    ViewBag.PageSize = Request["PageSize"] == null ? this.PageSize : Convert.ToInt32(Request["PageSize"]);
        //    ViewBag.BalanaceHistoryList = QueryClient.QueryUserBalanceReportList(ViewBag.StartTime, ViewBag.EndTime, ViewBag.PageIndex, ViewBag.PageSize);
        //    return View();
        //} 
        #endregion

        #region 网站结余明细
        public ActionResult QueryUserBalanceHistoryList(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("C109"))
                    throw new Exception("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);
                string UserId = p.userId;
                string StartTimeStr = p.startTime;
                string EndTimeStr = p.endTime;
                string pageIndexStr = p.pageIndex;
                string pageSizeStr = p.PageSize;
                //UserId = string.IsNullOrEmpty() ? string.Empty : UserId.ToString();
                var StartTime = string.IsNullOrEmpty(StartTimeStr) ? DateTime.Now : Convert.ToDateTime(StartTimeStr);
                var EndTime = string.IsNullOrEmpty(EndTimeStr) ? DateTime.Now : Convert.ToDateTime(EndTimeStr);
                var PageIndex = string.IsNullOrEmpty(pageIndexStr) ? this.PageIndex : Convert.ToInt32(pageIndexStr);
                var PageSize = string.IsNullOrEmpty(pageSizeStr) ? this.PageSize : Convert.ToInt32(pageSizeStr);
                var BalanaceHistoryList = _service.QueryUserBalanceHistoryList(UserId, StartTime, EndTime, PageIndex, PageSize);
                return Json(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.成功,
                    Message = "查询成功",
                    Value = BalanaceHistoryList
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

        #region 第三方游戏提款存款列表
        /// <summary>
        /// 提款存款列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IActionResult ThirdPartyGameList(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string gameTypeStr = p.gameType;
                string userId = p.userId;
                string orderId = p.orderId;
                string transferTypeStr = p.transferType;
                string statusStr = p.status;
                string endTimeStr = p.endTime;
                string startTimeStr = p.startTime;
                string pageIndexStr = p.pageIndex;
                string pageSizeStr = p.pageSize;
                var param = new ThirdPartyGameListParam();
                param.GameType = string.IsNullOrEmpty(gameTypeStr) ? -1 : Convert.ToInt32(gameTypeStr);
                param.TransferType = string.IsNullOrEmpty(transferTypeStr) ? -1 : Convert.ToInt32(transferTypeStr);
                param.Status = string.IsNullOrEmpty(statusStr) ? -1 : Convert.ToInt32(statusStr);
                param.StartTime = string.IsNullOrEmpty(startTimeStr) ? DateTime.Now.AddMonths(-1).Date : Convert.ToDateTime(startTimeStr).Date;
                param.EndTime = string.IsNullOrEmpty(endTimeStr) ? DateTime.Now.AddDays(1).Date : Convert.ToDateTime(endTimeStr).Date;
                param.OrderId = string.IsNullOrEmpty(orderId)?"": orderId;
                param.UserId = string.IsNullOrEmpty(userId) ? "" : userId;
                param.PageIndex = string.IsNullOrEmpty(pageIndexStr)?base.PageIndex:Convert.ToInt32(pageIndexStr);
                param.PageSize = string.IsNullOrEmpty(pageSizeStr) ? base.PageIndex : Convert.ToInt32(pageSizeStr);
                var list = _service.ThirdPartyGameDetail(param);
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
                    Message = ex.ToGetMessage() + "●" + ex.ToString()
                });
            }
        }

        #endregion

        #region 首页报表
        public IActionResult GetIndexReportForms()
        {
            try
            {
                var report = _service.GetIndexReportForms();
                return Json(new LotteryServiceResponse() {
                    Code = AdminResponseCode.成功 ,
                    Message = "查询成功",
                    Value=report
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
    }
}
