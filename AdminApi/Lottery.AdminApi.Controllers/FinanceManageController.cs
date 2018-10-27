//using EntityModel;
//using EntityModel.Enum;
//using KaSon.FrameWork.Common;
//using KaSon.FrameWork.ORM.Helper;
//using Lottery.AdminApi.Controllers.CommonFilterActtribute;
//using Lottery.AdminApi.Model.HelpModel;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using KaSon.FrameWork.Common.ExceptionEx;
//using EntityModel.ExceptionExtend;
//using KaSon.FrameWork.Common.Utilities;
//using System.IO;
//using EntityModel.CoreModel;
//using System.Linq;

//namespace Lottery.AdminApi.Controllers
//{
//    /// <summary>
//    /// 财务管理
//    /// </summary>
//    [Area("api")]
//    [ReusltFilter]
//    public class FinanceManageController : BaseController
//    {
//        #region 充值明细
//        /// <summary>
//        /// 充值明细分页
//        /// </summary>
//        public IActionResult FillMoneyDetail(LotteryServiceRequest entity)
//        {
//            try
//            {
//                if (!CheckRights("C102"))
//                    throw new LogicException("对不起，您的权限不足！");
//                var p = JsonHelper.Decode(entity.Param);
//                string userKey = p.userKey;
//                string orderId = p.orderId;
//                string status = p.status;
//                string schemeSource = p.schemeSource;
//                string agentType = p.agentType;
//                string startTimeStr = p.startTime;
//                string endTimeStr = p.endTime;
//                string pageIndexStr = p.pageIndex;
//                string pageSizeStr = p.pageSize;
//                //bool wccz = false;
//                //bool zwsb = false;
//                //bool czdcsj = false;
//                //bool czmxckyhxq = false;
//                //if (CheckRights("WCCZ100"))
//                //    wccz = true;
//                //if (CheckRights("ZWSB110"))
//                //    zwsb = true;
//                //if (CheckRights("CZDCSJ120"))
//                //    czdcsj = true;
//                //if (CheckRights("CZMXCKYHXQ130"))
//                //    czmxckyhxq = true;
//                //ViewBag.wccz = wccz;
//                //ViewBag.zwsb = zwsb;
//                //ViewBag.czdcsj = czdcsj;
//                //ViewBag.czmxckyhxq = czmxckyhxq;
//                //ViewBag.CZMX_CZBS = CheckRights("CZMX_CZBS");
//                //ViewBag.CZMX_QQCZJE = CheckRights("CZMX_QQCZJE");
//                //ViewBag.CZMX_WCCZJQ = CheckRights("CZMX_WCCZJQ");
//                var service = new AdminService();
//                var UserKey = string.IsNullOrEmpty(userKey) ? "" : userKey.Trim();
//                var OrderId = string.IsNullOrEmpty(orderId) ? "" : orderId.Trim();
//                var UserResult = new UserQueryInfo();
//                if (!string.IsNullOrEmpty(UserKey))
//                    UserResult = service.QueryUserByKey(ViewBag.UserKey, CurrentUser.UserToken);
//                var StartTime = string.IsNullOrEmpty(startTimeStr) ? DateTime.Now : DateTime.Parse(startTimeStr);
//                var EndTime = string.IsNullOrEmpty(endTimeStr) ? DateTime.Now : DateTime.Parse(endTimeStr);
//                var PageIndex = string.IsNullOrEmpty(pageIndexStr) ? base.PageIndex : int.Parse(pageIndexStr);
//                var PageSize = string.IsNullOrEmpty(pageSizeStr) ? base.PageSize : int.Parse(pageSizeStr);
//                var FillDetail = service.QueryFillMoneyList(ViewBag.UserKey,
//                    agentType,
//                    status,
//                    schemeSource,
//                    StartTime, EndTime,
//                    PageIndex, PageSize, OrderId);
//                return Json(new LotteryServiceResponse
//                {
//                    Code = AdminResponseCode.成功,
//                    Message = "查询成功",
//                    Value = new
//                    {
//                        UserQueryInfo = UserResult,
//                        FillDetail = FillDetail
//                    }
//                });
//            }
//            catch (Exception ex)
//            {
//                return Json(new LotteryServiceResponse
//                {
//                    Code = AdminResponseCode.失败,
//                    Message = ex.ToGetMessage() + "●" + ex.ToString()
//                });
//            }
//        }

//        /// <summary>
//        /// 手工完成充值
//        /// </summary>
//        public ActionResult CompleteFillMoney(LotteryServiceRequest entity)
//        {
//            try
//            {
//                if (!CheckRights("WCCZ100"))
//                    throw new LogicException("对不起,您的权限不足!");
//                var p = JsonHelper.Decode(entity.Param);
//                string orderId = p.orderId;
//                orderId = PreconditionAssert.IsNotEmptyString(orderId, "订单ID不能为空");
//                var service = new AdminService();
//                if (!string.IsNullOrWhiteSpace((string)p.sta) && (string)p.sta == "Fail")
//                {
//                    var result = service.ManualCompleteFillMoneyOrder(orderId, FillMoneyStatus.Failed, CurrentUser.UserId);
//                    service.AddSysOperationLog("", this.CurrentUser.UserId, "充值手工置为失败", string.Format("操作员【{0}】充值手工置为失败【{1}】，订单号【{2}】", this.CurrentUser.UserId, result.IsSuccess ? "成功" : "失败", orderId));
//                    return Json(new LotteryServiceResponse()
//                    {
//                        Code = result.IsSuccess == true ? AdminResponseCode.成功 : AdminResponseCode.失败,
//                        Message = (result.IsSuccess ? "手工置为失败成功" : "手工置为失败失败")
//                    }
//                    );
//                }
//                else
//                {
//                    var result = service.ManualCompleteFillMoneyOrder(orderId, FillMoneyStatus.Success, CurrentUser.UserId);
//                    service.AddSysOperationLog("", this.CurrentUser.UserId, "充值手工置为成功", string.Format("操作员【{0}】充值手工置为成功【{1}】，订单号【{2}】", CurrentUser.UserId, result.IsSuccess ? "成功" : "失败", orderId));
//                    return Json(new LotteryServiceResponse() { Code = result.IsSuccess == true ? AdminResponseCode.成功 : AdminResponseCode.失败, Message = result.Message });
//                }
//            }
//            catch (Exception ex)
//            {
//                return Json(new LotteryServiceResponse
//                {
//                    Code = AdminResponseCode.失败,
//                    Message = ex.ToGetMessage() + "●" + ex.ToString()
//                });
//            }
//        }
//        /// <summary>
//        /// 导出充值扣款明细
//        /// </summary>
//        public FileResult ExportFillDetail(LotteryServiceRequest entity)
//        {
//            try
//            {
//                if (!CheckRights("CZDCSJ120"))
//                    throw new LogicException("对不起，您的权限不足！");
//                var p = JsonHelper.Decode(entity.Param);
//                string userKey = p.userKey;
//                string endTimeStr = p.endTime;
//                string startTimeStr = p.startTime;
//                userKey = string.IsNullOrEmpty(userKey) ? "" : userKey;
//                var service = new AdminService();
//                var startTime = string.IsNullOrEmpty(startTimeStr) ? DateTime.Now.AddMonths(-1) : DateTime.Parse(startTimeStr);
//                var endTime = string.IsNullOrEmpty(endTimeStr) ? DateTime.Now : DateTime.Parse(endTimeStr);
//                IList<FillMoneyQueryInfo> FillDetail = service.QueryFillMoneyList(userKey, "", "", "", startTime, endTime, 0, int.MaxValue, "").FillMoneyList;

//                NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
//                //添加一个sheet
//                NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("充值扣款明细(" + startTime.ToString("yyyy-MM-dd") + "至" + endTime.ToString("yyyy-MM-dd"));
//                NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
//                row1.CreateCell(0).SetCellValue("请求时间");
//                row1.CreateCell(1).SetCellValue("响应时间");
//                row1.CreateCell(2).SetCellValue("用户编号");
//                row1.CreateCell(3).SetCellValue("用户名");
//                row1.CreateCell(4).SetCellValue("处理状态");
//                row1.CreateCell(5).SetCellValue("交易类型");
//                row1.CreateCell(6).SetCellValue("充值来源");
//                row1.CreateCell(7).SetCellValue("充值金额");
//                row1.CreateCell(8).SetCellValue("交易说明");
//                int i = 0;
//                foreach (var item in FillDetail)
//                {
//                    string temp = "";
//                    if (item.Status == FillMoneyStatus.Failed)
//                    {
//                        temp = "失败";
//                    }
//                    else if (item.Status == FillMoneyStatus.Requesting)
//                    {
//                        temp = "请求中";
//                    }
//                    else
//                    {
//                        temp = "成功";
//                    }

//                    Func<FillMoneyAgentType, string> format = (FillMoneyAgentType type) =>
//                    {
//                        switch (type)
//                        {
//                            case FillMoneyAgentType.Alipay:
//                                return "支付宝";
//                            case FillMoneyAgentType.AlipayWAP:
//                                return "支付宝-WAP支付";
//                            case FillMoneyAgentType.CallsPay:
//                                return "手机充值卡支付";
//                            case FillMoneyAgentType.ChinaPay:
//                                return "网银在线";
//                            case FillMoneyAgentType.KuanQian:
//                                return "快钱";
//                            case FillMoneyAgentType.ManualDeduct:
//                                return "手工扣款";
//                            case FillMoneyAgentType.ManualFill:
//                                return "手工充值";
//                            case FillMoneyAgentType.Tenpay:
//                                return "财付通";
//                            case FillMoneyAgentType.Yeepay:
//                                return "易宝";
//                        }
//                        return "未知来源";
//                    };
//                    i++;
//                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i);
//                    rowtemp.CreateCell(0).SetCellValue(item.RequestTime.ToString("yyyy-MM-dd HH:mm:ss"));
//                    rowtemp.CreateCell(1).SetCellValue((item.ResponseTime.HasValue ? item.ResponseTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : ""));
//                    rowtemp.CreateCell(2).SetCellValue(item.UserId);
//                    rowtemp.CreateCell(3).SetCellValue(item.UserDisplayName);
//                    rowtemp.CreateCell(4).SetCellValue(temp);
//                    rowtemp.CreateCell(5).SetCellValue(item.GoodsName);
//                    rowtemp.CreateCell(6).SetCellValue(format(item.FillMoneyAgent));
//                    rowtemp.CreateCell(7).SetCellValue(item.RequestMoney.ToString("N2"));
//                    rowtemp.CreateCell(8).SetCellValue(item.GoodsDescription);
//                }
//                System.IO.MemoryStream ms = new System.IO.MemoryStream();
//                book.Write(ms);
//                ms.Seek(0, SeekOrigin.Begin);
//                return File(ms, "application/vnd.ms-excel", string.Format("{0}.xls", DateTime.Now.ToString("yyyy-MM-dd")));
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }

//        #endregion

//        #region 提现管理
//        /// <summary>
//        /// 财务新提现管理列表
//        /// </summary>
//        public IActionResult NewWithdrawDetail(LotteryServiceRequest entity)
//        {
//            try
//            {
//                if (!CheckRights("C113"))
//                    throw new Exception("对不起，您的权限不足！");
//                //bool cktxxq = false;
//                //bool txmxdcsj = false;
//                //if (CheckRights("CKTXXQ100"))
//                //    cktxxq = true;
//                //if (CheckRights("TXMXDCSJ110"))
//                //    txmxdcsj = true;
//                //ViewBag.cktxxq = cktxxq;
//                //ViewBag.txmxdcsj = txmxdcsj;
//                //foreach (HttpPostedFileBase item in Request.Files)
//                //{
//                //    var fileName = item.FileName;
//                //    var stream = item.InputStream;
//                //}

//                //ViewBag.WithdrawLine = base.GameClient.QueryConfigByKey("WithdrawQuery_Line").ConfigValue;
//                var p = JsonHelper.Decode(entity.Param);
//                string bankcode = p.bankcode;
//                string userKey = p.userKey;
//                string operUserId = p.operUserId;
//                string startTimeStr = p.startTime;
//                string endTimeStr = p.endTime;
//                string pageIndexStr = p.pageIndex;
//                string pageSizeStr = p.pageSize;

//                bankcode = string.IsNullOrEmpty(bankcode) ? "" : bankcode.Trim();
//                userKey = string.IsNullOrEmpty(userKey) ? "" : userKey.Trim();
//                if (!string.IsNullOrEmpty(ViewBag.UserKey))
//                {
//                    ViewBag.UserResult = base.ExternalClient.QueryUserByKey(ViewBag.UserKey, CurrentUser.UserToken);
//                }
//                ViewBag.OperUserId = string.IsNullOrEmpty(Request["operUserId"]) ? "" : Request["operUserId"].Trim();
//                ViewBag.StartTime = string.IsNullOrEmpty(Request["startTime"]) ? DateTime.Today : Convert.ToDateTime(Request["startTime"]);
//                ViewBag.EndTime = string.IsNullOrEmpty(Request["endTime"]) ? DateTime.Today : Convert.ToDateTime(Request["endTime"]);

//                ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? 0 : Convert.ToInt32(Request["pageIndex"]);
//                ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 30 : Convert.ToInt32(Request["pageSize"]);
//                ViewBag.PageIndexS = string.IsNullOrEmpty(Request["pageIndexS"]) ? 0 : Convert.ToInt32(Request["pageIndexS"]);
//                ViewBag.PageSizeS = string.IsNullOrEmpty(Request["pageSizeS"]) ? 10 : Convert.ToInt32(Request["pageSizeS"]);
//                ViewBag.PageIndexR = string.IsNullOrEmpty(Request["pageIndexR"]) ? 0 : Convert.ToInt32(Request["pageIndexR"]);
//                ViewBag.PageSizeR = string.IsNullOrEmpty(Request["pageSizeR"]) ? 10 : Convert.ToInt32(Request["pageSizeR"]);

//                ViewBag.HasSuccess = string.IsNullOrEmpty(Request["s"]) ? false : Convert.ToBoolean(Request["s"]);
//                ViewBag.HasRefused = string.IsNullOrEmpty(Request["r"]) ? false : Convert.ToBoolean(Request["r"]);
//                WithdrawAgentType? agent = null;
//                if (!string.IsNullOrEmpty(Request["agentWay"]))
//                {
//                    var intAgent = int.Parse(Request["agentWay"]);
//                    if (intAgent != -1)
//                    {
//                        agent = (WithdrawAgentType)intAgent;
//                    }
//                }
//                ViewBag.Agent = agent;
//                var minMaxMoney = string.IsNullOrEmpty(Request["money"]) ? "-1_-1" : Request["money"];
//                ViewBag.MinMaxMoney = minMaxMoney;
//                var minMoney = -1M;
//                var maxMoney = -1M;
//                var minMaxMoneyArray = minMaxMoney.Split('_');
//                if (minMaxMoneyArray.Length == 2)
//                {
//                    minMoney = decimal.Parse(minMaxMoneyArray[0]);
//                    maxMoney = decimal.Parse(minMaxMoneyArray[1]);
//                }
//                var sortType = -1;
//                var sortTypeStr = string.IsNullOrEmpty(Request["slt_sort_type"]) ? "" : Request["slt_sort_type"];
//                if (!string.IsNullOrEmpty(sortTypeStr))
//                    sortType = int.Parse(sortTypeStr);
//                ViewBag.SortType = sortType;



//                ViewBag.WithdrawList_Success = base.ExternalClient.QueryWithdrawListR(
//                        ViewBag.UserKey, ViewBag.Agent, WithdrawStatus.Success, minMoney, maxMoney, ViewBag.StartTime, ViewBag.EndTime, sortType, ViewBag.OperUserId, ViewBag.PageIndex, ViewBag.PageSize, ViewBag.bankcode, CurrentUser.UserToken);
//                if (ViewBag.HasSuccess)
//                {
//                    ViewBag.WithdrawList_Success = base.ExternalClient.QueryWithdrawListR(
//                            ViewBag.UserKey, ViewBag.Agent, WithdrawStatus.Success, minMoney, maxMoney, ViewBag.StartTime, ViewBag.EndTime, sortType, ViewBag.OperUserId, ViewBag.PageIndex, ViewBag.PageSize, ViewBag.bankcode, CurrentUser.UserToken);
//                }
//                if (ViewBag.HasRefused)
//                {
//                    ViewBag.WithdrawList_Refused = base.ExternalClient.QueryWithdrawListR(
//                            ViewBag.UserKey, ViewBag.Agent, WithdrawStatus.Refused, minMoney, maxMoney, ViewBag.StartTime, ViewBag.EndTime, sortType, ViewBag.OperUserId, ViewBag.PageIndex, ViewBag.PageSize, ViewBag.bankcode, CurrentUser.UserToken);
//                }
//                return View();
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }

//        public IActionResult WithdrawLineSelect()
//        {
//            var service = new AdminService();
//            var WithdrawLine = service.QueryConfigByKey("WithdrawQuery_LineKey").ConfigValue.Split('|').ToList();
//            return Json(new LotteryServiceResponse() {
//                Code = AdminResponseCode.成功,
//                Message = "查询成功",
//                Value = WithdrawLine.Select(p => new {
//                    showName=p=="-1_-1"?"全部": p.Replace("_-1", "以上"),
//                    value=p
//                }).ToList()
//            });
//        }

//        /// <summary>
//        /// 提现管理列表
//        /// </summary>
//        public IActionResult WithdrawDetail()
//        {
//            try
//            {
//                if (!CheckRights("C103"))
//                    throw new Exception("对不起，您的权限不足！");
//                bool cktxxq = false;
//                bool txmxdcsj = false;
//                if (CheckRights("CKTXXQ100"))
//                    cktxxq = true;
//                if (CheckRights("TXMXDCSJ110"))
//                    txmxdcsj = true;
//                ViewBag.cktxxq = cktxxq;
//                ViewBag.txmxdcsj = txmxdcsj;
//                //foreach (HttpPostedFileBase item in Request.Files)
//                //{
//                //    var fileName = item.FileName;
//                //    var stream = item.InputStream;
//                //}

//                //ViewBag.WithdrawLine = base.GameClient.QueryConfigByKey("WithdrawQuery_Line").ConfigValue;
//                ViewBag.LoginName = CurrentUser.LoginName;

//                ViewBag.WithdrawLine = base.GameClient.QueryConfigByKey("WithdrawQuery_LineKey").ConfigValue.Split('|');

//                ViewBag.bankcode = string.IsNullOrEmpty(Request["bankcode"]) ? "" : Request["bankcode"].Trim();
//                ViewBag.UserKey = string.IsNullOrEmpty(Request["userKey"]) ? "" : Request["userKey"].Trim();
//                if (!string.IsNullOrEmpty(ViewBag.UserKey))
//                {
//                    ViewBag.UserResult = base.ExternalClient.QueryUserByKey(ViewBag.UserKey, CurrentUser.UserToken);
//                }
//                ViewBag.OperUserId = string.IsNullOrEmpty(Request["operUserId"]) ? "" : Request["operUserId"].Trim();
//                ViewBag.StartTime = string.IsNullOrEmpty(Request["startTime"]) ? DateTime.Today : Convert.ToDateTime(Request["startTime"]);
//                ViewBag.EndTime = string.IsNullOrEmpty(Request["endTime"]) ? DateTime.Today : Convert.ToDateTime(Request["endTime"]);

//                ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? 0 : Convert.ToInt32(Request["pageIndex"]);
//                ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 30 : Convert.ToInt32(Request["pageSize"]);
//                ViewBag.PageIndexS = string.IsNullOrEmpty(Request["pageIndexS"]) ? 0 : Convert.ToInt32(Request["pageIndexS"]);
//                ViewBag.PageSizeS = string.IsNullOrEmpty(Request["pageSizeS"]) ? 10 : Convert.ToInt32(Request["pageSizeS"]);
//                ViewBag.PageIndexR = string.IsNullOrEmpty(Request["pageIndexR"]) ? 0 : Convert.ToInt32(Request["pageIndexR"]);
//                ViewBag.PageSizeR = string.IsNullOrEmpty(Request["pageSizeR"]) ? 10 : Convert.ToInt32(Request["pageSizeR"]);

//                ViewBag.HasSuccess = string.IsNullOrEmpty(Request["s"]) ? false : Convert.ToBoolean(Request["s"]);
//                ViewBag.HasRefused = string.IsNullOrEmpty(Request["r"]) ? false : Convert.ToBoolean(Request["r"]);
//                WithdrawAgentType? agent = null;
//                if (!string.IsNullOrEmpty(Request["agentWay"]))
//                {
//                    var intAgent = int.Parse(Request["agentWay"]);
//                    if (intAgent != -1)
//                    {
//                        agent = (WithdrawAgentType)intAgent;
//                    }
//                }
//                ViewBag.Agent = agent;
//                var minMaxMoney = string.IsNullOrEmpty(Request["money"]) ? "-1_-1" : Request["money"];
//                ViewBag.MinMaxMoney = minMaxMoney;
//                var minMoney = -1M;
//                var maxMoney = -1M;
//                var minMaxMoneyArray = minMaxMoney.Split('_');
//                if (minMaxMoneyArray.Length == 2)
//                {
//                    minMoney = decimal.Parse(minMaxMoneyArray[0]);
//                    maxMoney = decimal.Parse(minMaxMoneyArray[1]);
//                }
//                var sortType = -1;
//                var sortTypeStr = string.IsNullOrEmpty(Request["slt_sort_type"]) ? "" : Request["slt_sort_type"];
//                if (!string.IsNullOrEmpty(sortTypeStr))
//                    sortType = int.Parse(sortTypeStr);
//                ViewBag.SortType = sortType;


//                ViewBag.WithdrawList_Request = base.ExternalClient.QueryWithdrawList(
//                        ViewBag.UserKey, ViewBag.Agent, WithdrawStatus.Request, minMoney, maxMoney, ViewBag.StartTime, ViewBag.EndTime, sortType, ViewBag.OperUserId, ViewBag.PageIndex, ViewBag.PageSize, ViewBag.bankcode, CurrentUser.UserToken);
//                ViewBag.WithdrawList_Requesting = base.ExternalClient.QueryWithdrawList(
//                        ViewBag.UserKey, ViewBag.Agent, WithdrawStatus.Requesting, minMoney, maxMoney, ViewBag.StartTime, ViewBag.EndTime, sortType, ViewBag.OperUserId, ViewBag.PageIndex, ViewBag.PageSize, ViewBag.bankcode, CurrentUser.UserToken);
//                if (ViewBag.HasSuccess)
//                {
//                    ViewBag.WithdrawList_Success = base.ExternalClient.QueryWithdrawList(
//                            ViewBag.UserKey, ViewBag.Agent, WithdrawStatus.Success, minMoney, maxMoney, ViewBag.StartTime, ViewBag.EndTime, sortType, ViewBag.OperUserId, ViewBag.PageIndex, ViewBag.PageSize, ViewBag.bankcode, CurrentUser.UserToken);
//                }
//                if (ViewBag.HasRefused)
//                {
//                    ViewBag.WithdrawList_Refused = base.ExternalClient.QueryWithdrawList(
//                            ViewBag.UserKey, ViewBag.Agent, WithdrawStatus.Refused, minMoney, maxMoney, ViewBag.StartTime, ViewBag.EndTime, sortType, ViewBag.OperUserId, ViewBag.PageIndex, ViewBag.PageSize, ViewBag.bankcode, CurrentUser.UserToken);
//                }
//                return View();
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        /// <summary>
//        /// 提现订单详情
//        /// </summary>
//        public IActionResult WithdrawItem()
//        {
//            try
//            {
//                if (!CheckRights("C104"))
//                    throw new Exception("对不起，您的权限不足！");
//                bool txqxdcsj = false;
//                bool txckyhxq = false;
//                bool txckddxq = false;
//                bool txcl = false;
//                if (CheckRights("TXQXDCSJ100"))
//                    txqxdcsj = true;
//                if (CheckRights("TXCKYHXQ110"))
//                    txckyhxq = true;
//                if (CheckRights("TXCKDDXQ120"))
//                    txckddxq = true;
//                if (CheckRights("TXCL130"))
//                    txcl = true;
//                ViewBag.txqxdcsj = txqxdcsj;
//                ViewBag.txckyhxq = txckyhxq;
//                ViewBag.txckddxq = txckddxq;
//                ViewBag.txcl = txcl;
//                ViewBag.OrderId = string.IsNullOrEmpty(Request["orderId"]) ? "" : Request["orderId"];
//                string userId = string.IsNullOrEmpty(Request["UserId"]) ? "" : Request["UserId"];
//                if (!string.IsNullOrEmpty(ViewBag.OrderId))
//                {
//                    ViewBag.WithdrawInfo = base.FundClient.GetWithdrawById(ViewBag.OrderId, CurrentUser.UserToken);
//                    if (ViewBag.WithdrawInfo != null && (ViewBag.WithdrawInfo.Status == WithdrawStatus.Requesting || ViewBag.WithdrawInfo.Status == WithdrawStatus.Request))
//                    {
//                        ViewBag.RemarkList = GetRemarkInfoList();
//                    }
//                }
//                return View();
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }

//        public ContentResult QueryUserTotalFillMoney()
//        {
//            try
//            {
//                var userId = Request["userId"];
//                var money = FundClient.QueryUserTotalFillMoney(userId);
//                return Content(money.ToString("f"));
//            }
//            catch (Exception)
//            {
//                return Content("0");
//            }
//        }

//        public ContentResult QueryUserTotalDrawMoney()
//        {
//            try
//            {
//                var userId = Request["userId"];
//                var money = FundClient.QueryUserTotalWithdrawMoney(userId);
//                return Content(money.ToString("f"));
//            }
//            catch (Exception)
//            {
//                return Content("0");
//            }
//        }

//        private List<RemarkInfo> GetRemarkInfoList()
//        {
//            var result = new List<RemarkInfo>();
//            var errorList = new List<string>();
//            errorList.Add("接受提现，已打款");
//            errorList.Add("银行卡信息有误，请及时联系在线客服处理！");
//            errorList.Add("银行卡账户与户名不符,请及时联系在线客服处理！");
//            errorList.Add("您本次提款含充值账户资金，需扣除手续费，如确认提款请重新申请！");
//            errorList.Add("您所使用的银行卡为二类三类账户，无法接收打款，请联系客户更换！");
//            errorList.Add("您所使用的银行卡状态异常，或已经超出限额，请联系客户更换处理！");
//            errorList.Add("活动赠送现金仅供购彩使用，不予直接提现！");
//            errorList.Add("同一身份注册多个账号！");
//            errorList.Add("您的账户存在异常行为，为了保障您的资金安全，请联系在线客服并提供手持身份证正面和反面照片！");
//            for (int i = 0; i < errorList.Count; i++)
//            {
//                var item = errorList[i];
//                result.Add(new RemarkInfo
//                {
//                    Index = i,
//                    Message = item,
//                    Value = i == 0,
//                });
//            }
//            return result;
//        }
//        #region old
//        //private List<RemarkInfo> GetRemarkInfoList()
//        //{
//        //    var result = new List<RemarkInfo>();
//        //    var errorList = new List<string>();
//        //    errorList.Add("接受提现，已打款");
//        //    errorList.Add("银行卡信息有误，请及时联系在线客服处理！");
//        //    errorList.Add("银行卡账户与户名不符,请及时联系在线客服处理！");
//        //    errorList.Add("请绑定与真实身份信息一致的实名支付宝账户!");
//        //    errorList.Add("支付宝账户不存在！请及时联系在线客服处理！");
//        //    errorList.Add("支付宝未实名认证！");
//        //    errorList.Add("活动赠送现金仅供购彩使用，不予直接提现！");
//        //    errorList.Add("同一身份注册多个账号！");
//        //    errorList.Add("您的账户存在异常行为，为了保障您的资金安全，请联系在线客服并提供手持身份证正面和反面照片！");
//        //    for (int i = 0; i < errorList.Count; i++)
//        //    {
//        //        var item = errorList[i];
//        //        result.Add(new RemarkInfo
//        //        {
//        //            Index = i,
//        //            Message = item,
//        //            Value = i == 0,
//        //        });
//        //    }
//        //    return result;
//        //} 
//        #endregion
//        private string GetDisposeMsg(string msg)
//        {
//            string tempMsg = string.Empty;
//            switch (msg)
//            {
//                case "1":
//                    tempMsg = "初审：资金可提现";
//                    break;
//                case "2":
//                    tempMsg = "初审：消费未满单笔充值金额的30%不可提现";
//                    break;
//                case "3":
//                    tempMsg = "初审：网站赠送部分只能购彩不可提现";
//                    break;
//            }
//            return tempMsg;
//        }
//        /// <summary>
//        /// 拒绝提现
//        /// </summary>
//        public IActionResult RefusedWithdraw()
//        {
//            try
//            {
//                string orderId = PreconditionAssert.IsNotEmptyString(Request["orderId"], "提现订单编号为空");
//                string message = PreconditionAssert.IsNotEmptyString(Request["message"], "备注不能为空");
//                var result = base.FundClient.RefusedWithdraw(orderId, message, CurrentUser.UserToken);
//                return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message });
//            }
//        }

//        public IActionResult RefusedWithdrawALL()
//        {
//            try
//            {
//                string orderIds = PreconditionAssert.IsNotEmptyString(Request["orderIds"], "提现订单编号为空");
//                string[] arrs = orderIds.Split(',');
//                if (arrs.Length == 0)
//                {
//                    return Json(new { IsSuccess = false, Msg = "请勾选订单编号" });
//                }
//                foreach (var item in arrs)
//                {
//                    var result = base.FundClient.RefusedWithdraw(item, "第三方打款失败，请稍候再重新申请", CurrentUser.UserToken);

//                }
//                return Json(new { IsSuccess = true, Msg = "" });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message });
//            }
//        }
//        #region old
//        //public IActionResult RefusedWithdrawALL()
//        //{
//        //    try
//        //    {
//        //        string orderIds = PreconditionAssert.IsNotEmptyString(Request["orderIds"], "提现订单编号为空");
//        //        string[] arrs = orderIds.Split(',');
//        //        if (arrs.Length == 0)
//        //        {
//        //            return Json(new { IsSuccess = false, Msg = "请勾选订单编号" });
//        //        }
//        //        foreach (var item in arrs)
//        //        {
//        //            var result = base.FundClient.RefusedWithdraw(item, "提现失败，请联系客服处理", CurrentUser.UserToken);

//        //        }
//        //        return Json(new { IsSuccess = true, Msg = "" });
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        return Json(new { IsSuccess = false, Msg = ex.Message });
//        //    }
//        //} 
//        #endregion


//        /// <summary>
//        /// 完成提现
//        /// </summary>
//        public IActionResult CompleteWithdraw()
//        {
//            try
//            {
//                string orderId = PreconditionAssert.IsNotEmptyString(Request["orderId"], "提现订单编号为空");
//                string message = PreconditionAssert.IsNotEmptyString(Request["message"], "备注不能为空");
//                var result = base.FundClient.CompleteWithdraw(orderId, message, CurrentUser.UserToken);

//                //提款成功发送短信给用户
//                if (result.IsSuccess)
//                {
//                    try
//                    {
//                        var with = base.FundClient.GetWithdrawById(orderId, CurrentUser.UserToken);
//                        var mobile = with.RequesterMobile;
//                        //var content = "尊敬的 " + with.RequesterRealName + " 您好，您提现【" + with.ResponseMoney.Value.ToString("f") + "元】已到账，请注意查收。";
//                        var content = "尊敬的 " + with.RequesterRealName + " 您好，您申请【" + with.ResponseMoney.Value.ToString("f") + "元】已处理，请注意查收。";
//                        //var smsmres = SMSSenderFactory.SendSMS(mobile, content);
//                        var returned = SendMessage(mobile, content);
//                    }
//                    catch
//                    {

//                    }
//                }
//                return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message });
//            }
//        }


//        public IActionResult CompleteWithdrawALL()
//        {
//            try
//            {
//                string orderIds = PreconditionAssert.IsNotEmptyString(Request["orderIds"], "提现订单编号为空");
//                string[] arrs = orderIds.Split(',');
//                if (arrs.Length == 0)
//                {
//                    return Json(new { IsSuccess = false, Msg = "请勾选订单编号" });
//                }
//                string msg = string.Empty;
//                foreach (var item in arrs)
//                {
//                    var result = base.FundClient.CompleteWithdraw(item, "接受提现，已打款", CurrentUser.UserToken);
//                    //提款成功发送短信给用户
//                    if (result.IsSuccess)
//                    {
//                        try
//                        {
//                            var with = base.FundClient.GetWithdrawById(item, CurrentUser.UserToken);
//                            var mobile = with.RequesterMobile;
//                            //var content = "尊敬的 " + with.RequesterRealName + " 您好，您提现【" + with.ResponseMoney.Value.ToString("f") + "元】已到账，请注意查收。";
//                            var content = "尊敬的 " + with.RequesterRealName + " 您好，您申请【" + with.ResponseMoney.Value.ToString("f") + "元】已处理，请注意查收。";
//                            //var smsmres = SMSSenderFactory.SendSMS(mobile, content);
//                            var returned = SendMessage(mobile, content);
//                        }
//                        catch
//                        {

//                        }
//                    }
//                }
//                return Json(new { IsSuccess = true, Msg = "" });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message });
//            }
//        }



//        public IActionResult CompleteWithdrawALL_Pay()
//        {

//            try
//            {
//                if (CurrentUser.LoginName != "caiwu01")
//                {
//                    return Json(new { IsSuccess = false, Msg = "你没有该权限" });
//                }

//                string orderIds = PreconditionAssert.IsNotEmptyString(Request["orderIds"], "提现订单编号为空");
//                string[] arrs = orderIds.Split(',');
//                if (arrs.Length == 0)
//                {
//                    return Json(new { IsSuccess = false, Msg = "请勾选订单编号" });
//                }
//                string msg = "提交成功，如果有错误会提示";
//                foreach (var item in arrs)
//                {
//                    var with = base.FundClient.GetWithdrawById(item, CurrentUser.UserToken);
//                    string channelid = Common.Pay.shunlifu.SLF_PAY.GetInstance().GetBankNo(with.BankName);
//                    ushort channel = 0;
//                    if (channelid == "0")
//                    {
//                        msg += string.Format("该订单提交失败,原因是未找到银行对应编号,订单号:{0},银行名字：{1}\r\n", item, with.BankName);
//                        continue;
//                    }
//                    channel = ushort.Parse(channelid);
//                    var slf_pay = Common.Pay.shunlifu.SLF_PAY.GetInstance();
//                    slf_pay.AppId = this.SLF_PAY_APP_ID;
//                    slf_pay.PrivateKey = this.SLF_PAY_privateKey;
//                    slf_pay.gateway_url = ConfigurationManager.AppSettings["gateway_url"].ToString();
//                    var pay_msg = slf_pay.ChargePay(1, channel, item, with.ResponseMoney.Value, "127.0.0.1", "", "代付", "代付", CurrentUser.UserId, with.RequesterRealName, with.BankCardNumber);
//                    string[] arr = pay_msg.Split('|');
//                    if (arr[0] == "true")
//                    {
//                        if (arr[1] == "0")
//                        {
//                            var result = base.FundClient.ReuqestWithdraw(item, CurrentUser.UserToken, this.SLF_PAY_APP_ID.ToString());
//                        }
//                        else
//                        {
//                            msg += string.Format("该订单提交失败,原因是银行返回失败,请手动处理这一笔,订单号:{0},银行名字：{1}\r\n", item, with.BankName);
//                            continue;
//                        }

//                        //var result = base.FundClient.CompleteWithdraw(item, "接受提现，已打款", CurrentUser.UserToken);
//                        //提款成功发送短信给用户
//                        //if (result.IsSuccess)
//                        //{
//                        //    try
//                        //    {
//                        //        var mobile = with.RequesterMobile;
//                        //        var content = "尊敬的 " + with.RequesterRealName + " 您好，您申请【" + with.ResponseMoney.Value.ToString("f") + "元】已处理，请注意查收。";
//                        //        var returned = SendMessage(mobile, content);
//                        //    }
//                        //    catch { }
//                        //}
//                    }
//                    else
//                    {
//                        msg += string.Format("该订单提交失败,原因是代付平台返回错误信息,订单号:{0},错误信息：{1}\r\n", item, arr[1]);
//                    }
//                }
//                return Json(new { IsSuccess = true, Msg = msg });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message });
//            }
//        }


//        public IActionResult WitheraeDetail2()
//        {
//            try
//            {
//                if (CurrentUser.LoginName != "caiwu01")
//                {
//                    return Json(new { IsSuccess = false, Msg = "你没有该权限" });
//                }

//                string orderIds = PreconditionAssert.IsNotEmptyString(Request["orderIds"], "提现订单编号为空");
//                string[] arrs = orderIds.Split(',');
//                if (arrs.Length == 0)
//                {
//                    return Json(new { IsSuccess = false, Msg = "请勾选订单编号" });
//                }
//                string msg = "提交顺利付打款不成功的订单能会自动还回来申请中";
//                var slf_pay = Common.Pay.shunlifu.SLF_PAY.GetInstance();
//                slf_pay.AppId = this.SLF_PAY_APP_ID;
//                slf_pay.PrivateKey = this.SLF_PAY_privateKey;
//                slf_pay.gateway_url = ConfigurationManager.AppSettings["gateway_url"].ToString();
//                foreach (var item in arrs)
//                {
//                    var result = slf_pay.QueryPay(item);
//                    if (result == "true|2")
//                    {
//                        base.FundClient.ReuqestWithdraw2(item, CurrentUser.UserToken, this.SLF_PAY_APP_ID.ToString());
//                    }

//                }
//                return Json(new { IsSuccess = true, Msg = msg });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message });
//            }
//        }

//        #region "华势"
//        /// <summary>
//        /// 接口请求地址
//        /// </summary>
//        public string payworth_agentPayurl
//        {
//            get
//            {
//                string defalutValue = "https://client.payworth.net/agentpay/pay";
//                try
//                {
//                    var v = ConfigurationManager.AppSettings["payworth_agentPayurl"].ToString();
//                    if (string.IsNullOrEmpty(v))
//                    {
//                        return defalutValue;
//                    }
//                    else
//                    {
//                        return v;
//                    }
//                }
//                catch (Exception)
//                {
//                    return defalutValue;
//                }
//            }
//        }
//        /// <summary>
//        /// 商户密钥
//        /// </summary>
//        public string payworth_Md5key
//        {
//            get
//            {
//                string defalutValue = "95ff8e3b2ff06eb4f894e46fb028ccedc8d2294e068632e810c10bg6adgegg05";
//                try
//                {
//                    var v = ConfigurationManager.AppSettings["payworth_Md5key"].ToString();
//                    if (string.IsNullOrEmpty(v))
//                    {
//                        return defalutValue;
//                    }
//                    else
//                    {
//                        return v;
//                    }
//                }
//                catch (Exception)
//                {
//                    return defalutValue;
//                }
//            }
//        }
//        /// <summary>
//        /// 合作伙伴在华势的用户ID
//        /// </summary>
//        public string payworth_parterId
//        {
//            get
//            {
//                string defalutValue = "100000000001986";
//                try
//                {
//                    var v = ConfigurationManager.AppSettings["payworth_parterId"].ToString();
//                    if (string.IsNullOrEmpty(v))
//                    {
//                        return defalutValue;
//                    }
//                    else
//                    {
//                        return v;
//                    }
//                }
//                catch (Exception)
//                {
//                    return defalutValue;
//                }
//            }
//        }
//        /// <summary>
//        /// 提交华势代付打款
//        /// </summary>
//        /// <returns></returns>
//        public IActionResult CompleteWithdrawALL_PayWorth()
//        {
//            try
//            {
//                string orderIds = PreconditionAssert.IsNotEmptyString(Request["orderIds"], "提现订单编号为空");
//                string CerPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificate", "tomcat.cer");
//                string[] arrs = orderIds.Split(',');
//                if (arrs.Length == 0)
//                {
//                    return Json(new { IsSuccess = false, Msg = "请勾选订单编号" });
//                }
//                string msg = "提交成功，如果有错误会提示";
//                int count = 0;//序号
//                //同卡在同批次中只能提交一次
//                //string sbBankCard = string.Empty;
//                String TempRSAStr = string.Empty;//明细串
//                //decimal batchAmount=0;//明细金额总和
//                foreach (var item in arrs)
//                {
//                    var with = base.FundClient.GetWithdrawById(item, CurrentUser.UserToken);
//                    //检查是否支持该银行
//                    string bankid = Common.Pay.payworth.HttpsRequest.checkBankName(with.BankName);
//                    if (bankid == "0")
//                    {
//                        msg += string.Format("该订单提交失败,原因是华势暂不支持该银行打款,订单号:{0},银行名字：{1}\r\n", item, with.BankName);
//                        continue;
//                    }
//                    //if (sbBankCard.Contains(with.BankCardNumber))
//                    //{
//                    //    msg += string.Format("该订单提交失败,原因是同卡在同批次中只能提交一次,订单号:{0},银行卡号：{1}\r\n", item, with.BankCardNumber);
//                    //    continue;
//                    //}
//                    //sbBankCard=string.Format(",{0}",with.BankCardNumber);
//                    count++;//序号
//                    //batchAmount = batchAmount + with.RequestMoney;
//                    //序号，银行账户，开户名，开户行，分行，支行，公/私，金额，币种，省，市，手机号，证件类型，证件号，用户协议号，商户订单号，备注 
//                    TempRSAStr = string.Format("" + count + "," + with.BankCardNumber + "," + with.RequesterRealName + "," + with.BankName + "," + with.CityName + "," + with.BankSubName + ",私," + with.RequestMoney.ToString("0.00") + ",CNY," + with.ProvinceName + "," + with.CityName + "," + with.RequesterMobile + ",,,," + with.OrderId + ",");

//                    Dictionary<String, String> DateContent = new Dictionary<String, String>();
//                    DateContent.Add("batchBizid", payworth_parterId);//商户号
//                    DateContent.Add("_input_charset", "gbk");
//                    DateContent.Add("batchBiztype", "00000");
//                    DateContent.Add("batchDate", DateTime.Now.ToString("yyyyMMdd"));//提交日期
//                    DateContent.Add("batchVersion", "00");
//                    DateContent.Add("batchCurrnum", "BC" + Common.Pay.payworth.Assist.GetSerialNumber());//商户批次号
//                    DateContent.Add("batchCount", "1");//明细笔数总和(根据实际定义)
//                    DateContent.Add("batchAmount", with.RequestMoney.ToString("0.00"));//明细金额总和(根据实际定义)

//                    TempRSAStr = Common.Pay.payworth.RSAUtil.EncryptRSAByCer(TempRSAStr, CerPath);
//                    DateContent.Add("batchContent", Common.Pay.payworth.HttpsRequest.URLencode(TempRSAStr));//必须使用这个URLencode
//                    String Md5str = Common.Pay.payworth.UtilSign.GetMd5str(DateContent, payworth_Md5key);
//                    DateContent.Add("sign_type", "MD5");
//                    DateContent.Add("sign", Md5str);//
//                    String HtmlStr = Common.Pay.payworth.HttpsRequest.OpenReadWithHttps(payworth_agentPayurl, Common.Pay.payworth.HttpsRequest.GetParam(DateContent));
//                    if (HtmlStr.Contains("succ"))
//                    {
//                        var result = base.FundClient.ReuqestWithdraw(item, CurrentUser.UserToken, this.payworth_parterId.ToString());
//                    }
//                    else
//                    {
//                        msg += string.Format("该订单提交失败,原因是代付平台返回错误信息,订单号:{0},错误信息：{1}\r\n", item, HtmlStr);
//                    }
//                }
//                return Json(new { IsSuccess = true, Msg = msg });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message });
//            }
//        }
//        #endregion
//        #endregion

//        #region 中奖明细
//        /// <summary>
//        /// 中奖明细
//        /// </summary>
//        /// <returns></returns>
//        public IActionResult WinDetail()
//        {
//            try
//            {
//                ViewData["images"] = Request["strImage"] == null ? ConfigurationManager.AppSettings["ShareRes"].ToString() + "/images/desc.png" : Request["strImage"].ToString();
//                ViewData["Amount_img"] = Request["strAmountImage"] == null ? ConfigurationManager.AppSettings["ShareRes"].ToString() + "/images/desc.png" : Request["strAmountImage"].ToString();
//                ViewBag.GameList = base.GameIssuseClient.QueryGameList(CurrentUser.UserToken);
//                ViewBag.UserKey = string.IsNullOrEmpty(Request["userKey"]) ? null : Request["userKey"];
//                ViewBag.GameCode = string.IsNullOrEmpty(Request["gameCode"]) ? null : Request["gameCode"];
//                ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? base.PageIndex : Convert.ToInt32(Request["pageIndex"]);
//                ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? base.PageSize : Convert.ToInt32(Request["pageSize"]);
//                ViewBag.StartTime = string.IsNullOrEmpty(Request["startTime"]) ? DateTime.Now.AddMonths(-1) : Convert.ToDateTime(Request["startTime"]);
//                ViewBag.EndTime = string.IsNullOrEmpty(Request["endTime"]) ? DateTime.Now : Convert.ToDateTime(Request["endTime"]);
//                ViewBag.SortAmount = string.IsNullOrEmpty(Request["strSortAmount"]) ? "desc:1" : Request["strSortAmount"];
//                ViewBag.Sortdat = string.IsNullOrEmpty(Request["strSortdat"]) ? "desc:1" : Request["strSortdat"];
//                ViewBag.flag = string.IsNullOrEmpty(Request["flag"]) ? "" : Request["flag"];
//                //if (ViewBag.flag == "1")
//                //{
//                //    ViewBag.BonusDetail = base.FundClient.QueryBonusDetail(ViewBag.UserKey, ViewBag.GameCode, ViewBag.StartTime, ViewBag.EndTime.AddDays(+1), ViewBag.PageIndex, ViewBag.PageSize, ViewBag.Sortdat, CurrentUser.UserToken);
//                //}
//                //else if (ViewBag.flag == "2")
//                //{
//                //    ViewBag.BonusDetail = base.FundClient.QueryBonusDetail(ViewBag.UserKey, ViewBag.GameCode, ViewBag.StartTime, ViewBag.EndTime.AddDays(+1), ViewBag.PageIndex, ViewBag.PageSize, ViewBag.SortAmount, CurrentUser.UserToken);
//                //}
//                //else
//                //{
//                //    ViewBag.BonusDetail = base.FundClient.QueryBonusDetail(ViewBag.UserKey, ViewBag.GameCode, ViewBag.StartTime, ViewBag.EndTime.AddDays(+1), ViewBag.PageIndex, ViewBag.PageSize, "desc:1", CurrentUser.UserToken);
//                //}
//                return View();
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        #endregion



//        #region 资金明细查询
//        ///// <summary>
//        ///// 财务资金明细报表
//        ///// </summary>
//        public IActionResult NewMoneyDetail(string id)
//        {
//            try
//            {
//                if (!CheckRights("C114"))
//                    throw new Exception("对不起，您的权限不足！");
//                return View();
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }

//        /// <summary>
//        /// 资金明细报表查询
//        /// </summary>
//        public IActionResult NewMoneyDetail_Inner(string id)
//        {
//            try
//            {
//                if (!string.IsNullOrEmpty(id))
//                {
//                    ViewBag.UserKey = id.Split('|')[0];
//                    ViewBag.ShowUser = bool.Parse(id.Split('|')[1]);
//                    ViewBag.PageSize = int.Parse(id.Split('|')[2]);
//                }
//                else
//                {
//                    ViewBag.UserKey = Request["userKey"];
//                    ViewBag.ShowUser = string.IsNullOrEmpty(Request["showUser"]) ? true : Convert.ToBoolean(Request["showUser"]);
//                    ViewBag.PageSize = string.IsNullOrEmpty(Request["pagesize_f"]) ? base.PageSize : Convert.ToInt32(Request["pagesize_f"]);
//                }
//                ViewBag.TypeList = string.IsNullOrEmpty(Request["typeList"]) ? "10|20|30|50|70" : Request["typeList"].TrimStart('|');
//                ViewBag.StartTime = string.IsNullOrEmpty(Request["startTime"]) ? DateTime.Today : Convert.ToDateTime(Request["startTime"]);
//                ViewBag.EndTime = string.IsNullOrEmpty(Request["endTime"]) ? DateTime.Now : Convert.ToDateTime(Request["endTime"]);
//                ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageindex_f"]) ? base.PageIndex : Convert.ToInt32(Request["pageindex_f"]);
//                ViewBag.sel_OperateType = string.IsNullOrEmpty(Request["sel_OperateType"]) ? "" : Request["sel_OperateType"].ToString();

//                ViewBag.KeyLine = Request["keyLine"];

//                ViewBag.FundDetails = this.QueryClient.QueryUserFundDetailListReport(ViewBag.UserKey, ViewBag.KeyLine, ViewBag.StartTime, ViewBag.EndTime, ViewBag.TypeList, ViewBag.sel_OperateType, ViewBag.PageIndex, ViewBag.PageSize, CurrentUser.UserToken);
//                return View();
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }

//        /// <summary>
//        /// 资金明细查询
//        /// </summary>
//        public IActionResult MoneyDetail(string id)
//        {
//            try
//            {
//                if (!CheckRights("C101"))
//                    throw new Exception("对不起，您的权限不足！");
//                return View();
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }


//        /// <summary>
//        /// 资金明细查询
//        /// </summary>
//        public IActionResult MoneyDetail_Inner(string id)
//        {
//            try
//            {
//                if (!string.IsNullOrEmpty(id))
//                {
//                    ViewBag.UserKey = id.Split('|')[0];
//                    ViewBag.ShowUser = bool.Parse(id.Split('|')[1]);
//                    ViewBag.PageSize = int.Parse(id.Split('|')[2]);
//                }
//                else
//                {
//                    ViewBag.UserKey = Request["userKey"];
//                    ViewBag.ShowUser = string.IsNullOrEmpty(Request["showUser"]) ? true : Convert.ToBoolean(Request["showUser"]);
//                    ViewBag.PageSize = string.IsNullOrEmpty(Request["pagesize_f"]) ? base.PageSize : Convert.ToInt32(Request["pagesize_f"]);
//                }
//                ViewBag.TypeList = string.IsNullOrEmpty(Request["typeList"]) ? "10|20|30|50|70" : Request["typeList"].TrimStart('|');
//                ViewBag.StartTime = string.IsNullOrEmpty(Request["startTime"]) ? DateTime.Today : Convert.ToDateTime(Request["startTime"]);
//                ViewBag.EndTime = string.IsNullOrEmpty(Request["endTime"]) ? DateTime.Now : Convert.ToDateTime(Request["endTime"]);
//                ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageindex_f"]) ? base.PageIndex : Convert.ToInt32(Request["pageindex_f"]);
//                ViewBag.sel_OperateType = string.IsNullOrEmpty(Request["sel_OperateType"]) ? "" : Request["sel_OperateType"].ToString();
//                ViewBag.KeyLine = Request["keyLine"];

//                ViewBag.FundDetails = this.QueryClient.QueryUserFundDetailList(ViewBag.UserKey, ViewBag.KeyLine, ViewBag.StartTime, ViewBag.EndTime, ViewBag.TypeList, ViewBag.sel_OperateType, ViewBag.PageIndex, ViewBag.PageSize, CurrentUser.UserToken);
//                return View();
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }

//        public IActionResult QueryCommissionDetail()
//        {
//            ViewBag.UserKey = Request["userKey"];
//            ViewBag.StartTime = string.IsNullOrEmpty(Request["startTime"]) ? DateTime.Today : Convert.ToDateTime(Request["startTime"]);
//            ViewBag.EndTime = string.IsNullOrEmpty(Request["endTime"]) ? DateTime.Now : Convert.ToDateTime(Request["endTime"]);
//            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? base.PageIndex : Convert.ToInt32(Request["pageIndex"]);
//            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? base.PageSize : Convert.ToInt32(Request["pageSize"]);

//            ViewBag.FundDetails = this.QueryClient.QueryUserFundDetail_Commission(ViewBag.UserKey, ViewBag.StartTime, ViewBag.EndTime, ViewBag.PageIndex, ViewBag.PageSize);

//            return View();
//        }

//        public IActionResult MoneyDetail_Inner_Withdraw(string id)
//        {
//            try
//            {
//                ViewBag.UserKey = id.Split('|')[0];
//                ViewBag.PageSize = int.Parse(id.Split('|')[1]);

//                ViewBag.TypeList = string.IsNullOrEmpty(Request["typeList"]) ? "10|20|30|50|60|70" : Request["typeList"].TrimStart('|');
//                ViewBag.StartTime = string.IsNullOrEmpty(Request["startTime"]) ? DateTime.Today : Convert.ToDateTime(Request["startTime"]);
//                ViewBag.EndTime = string.IsNullOrEmpty(Request["endTime"]) ? DateTime.Now : Convert.ToDateTime(Request["endTime"]);
//                ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageindex_f"]) ? base.PageIndex : Convert.ToInt32(Request["pageindex_f"]);
//                ViewBag.sel_OperateType = string.IsNullOrEmpty(Request["sel_OperateType"]) ? "" : Request["sel_OperateType"].ToString();

//                ViewBag.KeyLine = Request["keyLine"];

//                ViewBag.FundDetails = this.QueryClient.QueryUserFundDetailList(ViewBag.UserKey, ViewBag.KeyLine, ViewBag.StartTime, ViewBag.EndTime, ViewBag.TypeList, ViewBag.sel_OperateType, ViewBag.PageIndex, ViewBag.PageSize, CurrentUser.UserToken);
//                return View();
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }

//        #endregion

//        #region 财务人员设置

//        public IActionResult FinanceSetting()
//        {
//            if (!CheckRights("C105"))
//                throw new Exception("对不起，您的权限不足！");
//            bool sccw = false;
//            bool xgcwry = false;
//            if (CheckRights("SCCW100"))
//                sccw = true;
//            if (CheckRights("XGCWRY120"))
//                xgcwry = true;
//            ViewBag.sccw = sccw;
//            ViewBag.xgcwry = xgcwry;
//            ViewBag.UserId = string.IsNullOrEmpty(Request["UserId"]) ? "" : Request["UserId"].ToString();
//            ViewBag.StartTime = string.IsNullOrEmpty(Request["StartTime"]) ? DateTime.Now : Convert.ToDateTime(Request["StartTime"]);
//            ViewBag.EndTime = string.IsNullOrEmpty(Request["EndTime"]) ? DateTime.Now : Convert.ToDateTime(Request["EndTime"]);
//            ViewBag.PageIndex = string.IsNullOrEmpty(Request.QueryString["pageIndex"]) ? base.PageIndex : int.Parse(Request.QueryString["pageIndex"]);
//            ViewBag.PageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? base.PageSize : int.Parse(Request.QueryString["pageSize"]);
//            ViewBag.FinanceSettingList = this.FundClient.GetFinanceSettingsCollection(ViewBag.UserId, ViewBag.StartTime, ViewBag.EndTime, ViewBag.PageIndex, ViewBag.PageSize, CurrentUser.UserToken);
//            return View();
//        }
//        public IActionResult FinanceSettingInfo()
//        {
//            if (!CheckRights("C106"))
//                throw new Exception("对不起，您的权限不足！");
//            bool xgcw = false;
//            if (CheckRights("XGCW100"))
//                xgcw = true;
//            ViewBag.xgcw = xgcw;
//            ViewBag.FinanceUser = this.FundClient.GetCaiWuOperator(CurrentUser.UserToken);
//            if (Request["FinanceId"] != null)
//            {
//                ViewBag.FinanceInfo = this.FundClient.GetFinanceSettingsByFinanceId(Request["FinanceId"], CurrentUser.UserToken);
//            }
//            return View();
//        }
//        public IActionResult OperateFinanceSetting()
//        {
//            try
//            {
//                if (Request["Type"] != null)
//                {

//                    FinanceSettingsInfo info = new FinanceSettingsInfo();
//                    if (Request["Type"] != "add")
//                    {
//                        info.FinanceId = Convert.ToInt32(Request["FinanceId"]);
//                    }
//                    if (Request["Type"] != "delete")
//                    {
//                        info.UserId = PreconditionAssert.IsNotEmptyString(Request["UserId"], "财务人员不能为空！");
//                        info.OperateType = PreconditionAssert.IsNotEmptyString(Request["OperateType"], "操作类型不能为空！");
//                        //info.OperateRank = PreconditionAssert.IsNotEmptyString(Request["OperateRank"], "操作级别不能为空!");
//                        if (Request["MinMoney"] == null || Request["MaxMoney"] == null)
//                        {
//                            PreconditionAssert.IsNotEmptyString("", "金额不能为空!");
//                        }
//                        else if (Convert.ToDecimal(Request["MinMoney"]) > Convert.ToDecimal(Request["MaxMoney"]))
//                        {
//                            PreconditionAssert.IsNotEmptyString("", "最小金额不能大于最大金额!");
//                        }
//                        info.MinMoney = Convert.ToDecimal(Request["MinMoney"]);
//                        info.MaxMoney = Convert.ToDecimal(Request["MaxMoney"]);
//                    }
//                    switch (Request["Type"].ToString().ToLower())
//                    {
//                        case "add":
//                            var flag = this.FundClient.FinanceSetting("add", info, CurrentUser.UserToken);
//                            return Json(new { IsSuccess = flag.IsSuccess, Msg = flag.Message });

//                        case "update":
//                            flag = this.FundClient.FinanceSetting("update", info, CurrentUser.UserToken);
//                            return Json(new { IsSuccess = flag.IsSuccess, Msg = flag.Message });

//                        case "delete":
//                            flag = this.FundClient.FinanceSetting("delete", info, CurrentUser.UserToken);
//                            return Json(new { IsSuccess = flag.IsSuccess, Msg = flag.Message });

//                    }
//                }
//                return Json(new { IsSuccess = false, Msg = "保存数据失败！" });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message });
//            }
//        }

//        #endregion
//    }
//}
