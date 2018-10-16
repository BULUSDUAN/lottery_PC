//using KaSon.FrameWork.Common.ValidateCodeHelper;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using KaSon.FrameWork.Common;
//using Microsoft.AspNetCore.Mvc;
//using EntityModel.Enum;
//using Lottery.AdminApi.Model.HelpModel;
//using System.DrawingCore;
//using KaSon.FrameWork.Common.ExceptionEx;
//using KaSon.FrameWork.Common.Utilities;
//using KaSon.FrameWork.ORM.Helper.Admin;
//using KaSon.FrameWork.Common.Net;
//using EntityModel;
//using Lottery.AdminApi.Controllers;

//namespace Lottery.AdminApi.Controllers
//{
//    public class ActivityController : BaseController
//    {
//        public static readonly AdminPartService _service = new AdminPartService();
//        public IActionResult FillMoneyNextMonthGive()
//        {
//            //ViewBag.Month = string.IsNullOrEmpty(Request["month"]) ? DateTime.Today.Month : int.Parse(Request["month"]);
//            //ViewBag.IsComplate = string.IsNullOrEmpty(Request["isComplate"]) ? false : bool.Parse(Request["isComplate"]);
//            //ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? 0 : int.Parse(Request["pageIndex"]);
//            //ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? 50 : int.Parse(Request["pageSize"]);
//            ////ViewBag.RecordList = this.ActivityClient.QueryA20131101InfoCollection(ViewBag.Month, ViewBag.IsComplate, ViewBag.PageIndex, ViewBag.PageSize, this.CurrentUser.UserToken);
//            return View();
//        }

//        public JsonResult DoGiveMoney(LotteryServiceRequest entity)
//        {
//            try
//            {
//                var p = JsonHelper.Decode(entity.Param);
//                string recordIdStr = p.RecordIdStr;
//                //this.ActivityClient.Activity20131101Give(recordIdStr, this.CurrentUser.UserToken);
//                return Json(new { IsSuccess = true, Msg = "赠送成功" });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message });
//            }
//        }

//        public ActionResult QueryMonthReturnPoint(LotteryServiceRequest entity)
//        {
//            //ViewBag.Month = string.IsNullOrEmpty(Request["slt_month"]) ? DateTime.Now.Month : int.Parse(Request["slt_month"]);
//            //ViewBag.StartTime = DateTime.Today.AddMonths(-1);
//            //ViewBag.EndTime = DateTime.Today;
//            //ViewBag.RecordList = this.ActivityClient.QueryMonthReturnPoint(ViewBag.Month, this.CurrentUser.UserToken);

//            return View();
//        }

//        public JsonResult DoExecReturnPoint()
//        {
//            try
//            {
//                //var startTime = DateTime.Parse(Request["startTime"]);
//                //var endTime = DateTime.Parse(Request["endTime"]);
//                ////var msg = this.ActivityClient.InvokeMonthReturnPoint(startTime, endTime, this.CurrentUser.UserToken);
//                return Json(new { IsSuccess = true, Msg = "" });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message });
//            }
//        }

//        public ActionResult CouponsManager(LotteryServiceRequest entity)
//        {
//            var p = JsonHelper.Decode(entity.Param);
//            string Summary = string.IsNullOrEmpty(p.summary) ? string.Empty : p.summary;
//            bool? useable = true;
//            string Useable = useable;
//            if (!string.IsNullOrEmpty(p.useable))
//            {
//                ViewBag.Useable = useable = bool.Parse(p.useable);
//            }
//            string BelongUserId = string.IsNullOrEmpty(p.belongUserId) ? string.Empty : p.belongUserId;
//            string PageIndex = string.IsNullOrEmpty(p.pageIndex) ? 0 : int.Parse(p.pageIndex);
//            string PageSize = string.IsNullOrEmpty(p.pageSize) ? 50 : int.Parse(p.pageSize);

//            ViewBag.List = AdminPartService.QueryCouponsList(ViewBag.Summary, ViewBag.Useable, ViewBag.BelongUserId, ViewBag.PageIndex, ViewBag.PageSize, this.CurrentUser.UserToken);
//            return View();
//        }

//        public JsonResult BuildCoupons()
//        {
//            try
//            {
//                var summary = Request["summary"];
//                var money = decimal.Parse(Request["money"]);
//                var count = int.Parse(Request["count"]);
//                this.ActivityClient.BuildCoupons(summary, money, count, this.CurrentUser.UserToken);
//                return Json(new { IsSuccess = true, Msg = "生成完成" });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message });
//            }
//        }
//        #region 活动列表
//        /// <summary>
//        /// 查询传统足球_14场_任九_加奖
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult QueryA20121009Detail()
//        {
//            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? base.PageIndex : int.Parse(Request["pageIndex"]);
//            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? base.PageSize : int.Parse(Request["pageSize"]);
//            ViewBag.UserKey = string.IsNullOrWhiteSpace(Request["userKey"]) ? "" : Request["userKey"]
//;
//            ViewBag.StartTime = string.IsNullOrWhiteSpace(Request["startTime"]) ? DateTime.Now : DateTime.Parse(Request["startTime"]);
//            ViewBag.EndTime = string.IsNullOrWhiteSpace(Request["endTime"]) ? DateTime.Now : DateTime.Parse(Request["endTime"]);
//            ViewBag.A20121009Detail = ActivityClient.QueryA20121009Info(ViewBag.UserKey, ViewBag.StartTime, ViewBag.EndTime, ViewBag.PageIndex, ViewBag.PageSize, CurrentUser.UserToken);
//            return View();
//        }
//        /// <summary>
//        /// 二期活动_奖金转入送1个点
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult QueryA20120925Detail()
//        {
//            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? base.PageIndex : int.Parse(Request["pageIndex"]);
//            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? base.PageSize : int.Parse(Request["pageSize"]);
//            ViewBag.UserKey = string.IsNullOrWhiteSpace(Request["userKey"]) ? "" : Request["userKey"]
//;
//            ViewBag.StartTime = string.IsNullOrWhiteSpace(Request["startTime"]) ? DateTime.Now : DateTime.Parse(Request["startTime"]);
//            ViewBag.EndTime = string.IsNullOrWhiteSpace(Request["endTime"]) ? DateTime.Now : DateTime.Parse(Request["endTime"]);
//            ViewBag.A20120925Detail = ActivityClient.QueryA20120925Info(ViewBag.UserKey, ViewBag.StartTime, ViewBag.EndTime, ViewBag.PageIndex, ViewBag.PageSize, CurrentUser.UserToken);
//            return View();
//        }
//        /// <summary>
//        /// 充值送钱_认证后充200送50_充值送百分之10
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult QueryA20130807Detail()
//        {
//            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? base.PageIndex : int.Parse(Request["pageIndex"]);
//            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? base.PageSize : int.Parse(Request["pageSize"]);
//            ViewBag.UserKey = string.IsNullOrWhiteSpace(Request["userKey"]) ? "" : Request["userKey"]
//;
//            ViewBag.StartTime = string.IsNullOrWhiteSpace(Request["startTime"]) ? DateTime.Now : DateTime.Parse(Request["startTime"]);
//            ViewBag.EndTime = string.IsNullOrWhiteSpace(Request["endTime"]) ? DateTime.Now : DateTime.Parse(Request["endTime"]);
//            ViewBag.A20130807Detail = ActivityClient.QueryA20130807Info(ViewBag.UserKey, ViewBag.StartTime, ViewBag.EndTime, ViewBag.PageIndex, ViewBag.PageSize, CurrentUser.UserToken);
//            return View();
//        }
//        /// <summary>
//        /// 实名手机认证送3元
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult QueryA20130808Detail()
//        {
//            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? base.PageIndex : int.Parse(Request["pageIndex"]);
//            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? base.PageSize : int.Parse(Request["pageSize"]);
//            ViewBag.UserKey = string.IsNullOrWhiteSpace(Request["userKey"]) ? "" : Request["userKey"]
//;
//            ViewBag.StartTime = string.IsNullOrWhiteSpace(Request["startTime"]) ? DateTime.Now : DateTime.Parse(Request["startTime"]);
//            ViewBag.EndTime = string.IsNullOrWhiteSpace(Request["endTime"]) ? DateTime.Now : DateTime.Parse(Request["endTime"]);
//            ViewBag.A20130808Detail = ActivityClient.QueryA20130808Info(ViewBag.UserKey, ViewBag.StartTime, ViewBag.EndTime, ViewBag.PageIndex, ViewBag.PageSize, CurrentUser.UserToken);
//            return View();
//        }
//        /// <summary>
//        /// 用户完成手机认证以及实名认证，首次充值大于20赠送10元
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult QueryA20120925CZGiveMoneyDetail()
//        {
//            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? base.PageIndex : int.Parse(Request["pageIndex"]);
//            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? base.PageSize : int.Parse(Request["pageSize"]);
//            ViewBag.UserKey = string.IsNullOrWhiteSpace(Request["userKey"]) ? "" : Request["userKey"]
//;
//            ViewBag.StartTime = string.IsNullOrWhiteSpace(Request["startTime"]) ? DateTime.Now : DateTime.Parse(Request["startTime"]);
//            ViewBag.EndTime = string.IsNullOrWhiteSpace(Request["endTime"]) ? DateTime.Now : DateTime.Parse(Request["endTime"]);
//            ViewBag.A20120925CZGiveMoneyDetail = ActivityClient.QueryA20120925CZGiveMoneyInfo(ViewBag.UserKey, ViewBag.StartTime, ViewBag.EndTime, ViewBag.PageIndex, ViewBag.PageSize, CurrentUser.UserToken);
//            return View();
//        }
//        /// <summary>
//        /// 竞彩足球加奖
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult QueryA20130903Detail()
//        {
//            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? base.PageIndex : int.Parse(Request["pageIndex"]);
//            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? base.PageSize : int.Parse(Request["pageSize"]);
//            ViewBag.UserKey = string.IsNullOrWhiteSpace(Request["userKey"]) ? "" : Request["userKey"]
//;
//            ViewBag.StartTime = string.IsNullOrWhiteSpace(Request["startTime"]) ? DateTime.Now : DateTime.Parse(Request["startTime"]);
//            ViewBag.EndTime = string.IsNullOrWhiteSpace(Request["endTime"]) ? DateTime.Now : DateTime.Parse(Request["endTime"]);
//            ViewBag.QueryA20130903Detail = ActivityClient.QueryA20130903Info(ViewBag.UserKey, ViewBag.StartTime, ViewBag.EndTime, ViewBag.PageIndex, ViewBag.PageSize, CurrentUser.UserToken);
//            return View();
//        }
//        /// <summary>
//        /// 转运红包
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult QueryA20130903ZYHBDetail()
//        {
//            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? base.PageIndex : int.Parse(Request["pageIndex"]);
//            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? base.PageSize : int.Parse(Request["pageSize"]);
//            ViewBag.UserKey = string.IsNullOrWhiteSpace(Request["userKey"]) ? "" : Request["userKey"]
//;
//            ViewBag.StartTime = string.IsNullOrWhiteSpace(Request["startTime"]) ? DateTime.Now : DateTime.Parse(Request["startTime"]);
//            ViewBag.EndTime = string.IsNullOrWhiteSpace(Request["endTime"]) ? DateTime.Now : DateTime.Parse(Request["endTime"]);
//            ViewBag.QueryA20130903ZYHBDetail = ActivityClient.QueryA20130903ZYHBInfo(ViewBag.UserKey, ViewBag.StartTime, ViewBag.EndTime, ViewBag.PageIndex, ViewBag.PageSize, CurrentUser.UserToken);
//            return View();
//        }
//        /// <summary>
//        /// 用户返点
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult QueryA20131101YHFDDetail()
//        {
//            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? base.PageIndex : int.Parse(Request["pageIndex"]);
//            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? base.PageSize : int.Parse(Request["pageSize"]);
//            ViewBag.UserKey = string.IsNullOrWhiteSpace(Request["userKey"]) ? "" : Request["userKey"]
//;
//            ViewBag.StartTime = string.IsNullOrWhiteSpace(Request["startTime"]) ? DateTime.Now : DateTime.Parse(Request["startTime"]);
//            ViewBag.EndTime = string.IsNullOrWhiteSpace(Request["endTime"]) ? DateTime.Now : DateTime.Parse(Request["endTime"]);
//            ViewBag.QueryA20131101YHFDDetail = ActivityClient.QueryA20131101YHFDInfo(ViewBag.UserKey, ViewBag.StartTime, ViewBag.EndTime, ViewBag.PageIndex, ViewBag.PageSize, CurrentUser.UserToken);
//            return View();
//        }
//        /// <summary>
//        /// 新用户首充送钱
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult QueryA20131101NewUserCZDetail()
//        {
//            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? base.PageIndex : int.Parse(Request["pageIndex"]);
//            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? base.PageSize : int.Parse(Request["pageSize"]);
//            ViewBag.UserKey = string.IsNullOrWhiteSpace(Request["userKey"]) ? "" : Request["userKey"]
//;
//            ViewBag.StartTime = string.IsNullOrWhiteSpace(Request["startTime"]) ? DateTime.Now : DateTime.Parse(Request["startTime"]);
//            ViewBag.EndTime = string.IsNullOrWhiteSpace(Request["endTime"]) ? DateTime.Now : DateTime.Parse(Request["endTime"]);
//            //ViewBag.IsGive = string.IsNullOrWhiteSpace(Request["isGive"]) ? -1 : Convert.ToInt32(Request["isGive"]);
//            ViewBag.QueryA20131101NewUserCZDetail = ActivityClient.QueryA20131101NewUserCZInfo(ViewBag.UserKey, 1, ViewBag.StartTime, ViewBag.EndTime, ViewBag.PageIndex, ViewBag.PageSize, CurrentUser.UserToken);
//            return View();
//        }
//        /// <summary>
//        /// 首次中奖超过100送5元
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult QueryA20121128Detail()
//        {
//            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? base.PageIndex : int.Parse(Request["pageIndex"]);
//            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? base.PageSize : int.Parse(Request["pageSize"]);
//            ViewBag.UserKey = string.IsNullOrWhiteSpace(Request["userKey"]) ? "" : Request["userKey"]
//;
//            ViewBag.StartTime = string.IsNullOrWhiteSpace(Request["startTime"]) ? DateTime.Now : DateTime.Parse(Request["startTime"]);
//            ViewBag.EndTime = string.IsNullOrWhiteSpace(Request["endTime"]) ? DateTime.Now : DateTime.Parse(Request["endTime"]);
//            ViewBag.QueryA20121128Detail = ActivityClient.QueryA20121128Info(ViewBag.UserKey, ViewBag.StartTime, ViewBag.EndTime, ViewBag.PageIndex, ViewBag.PageSize, CurrentUser.UserToken);
//            return View();
//        }
//        /// <summary>
//        /// 首次充20送11
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult QueryA20140214Detail()
//        {
//            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? base.PageIndex : int.Parse(Request["pageIndex"]);
//            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? base.PageSize : int.Parse(Request["pageSize"]);
//            ViewBag.UserKey = string.IsNullOrWhiteSpace(Request["userKey"]) ? "" : Request["userKey"]
//;
//            ViewBag.StartTime = string.IsNullOrWhiteSpace(Request["startTime"]) ? DateTime.Now : DateTime.Parse(Request["startTime"]);
//            ViewBag.EndTime = string.IsNullOrWhiteSpace(Request["endTime"]) ? DateTime.Now : DateTime.Parse(Request["endTime"]);
//            ViewBag.QueryA20140214Detail = ActivityClient.QueryA20140214Info(ViewBag.UserKey, ViewBag.StartTime, ViewBag.EndTime, ViewBag.PageIndex, ViewBag.PageSize, CurrentUser.UserToken);
//            return View();
//        }
//        /// <summary>
//        /// 时时彩红包
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult QueryA20140214SSCHBDetail()
//        {
//            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? base.PageIndex : int.Parse(Request["pageIndex"]);
//            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? base.PageSize : int.Parse(Request["pageSize"]);
//            ViewBag.UserKey = string.IsNullOrWhiteSpace(Request["userKey"]) ? "" : Request["userKey"]
//;
//            ViewBag.StartTime = string.IsNullOrWhiteSpace(Request["startTime"]) ? DateTime.Now : DateTime.Parse(Request["startTime"]);
//            ViewBag.EndTime = string.IsNullOrWhiteSpace(Request["endTime"]) ? DateTime.Now : DateTime.Parse(Request["endTime"]);
//            ViewBag.QueryA20140214SSCHBDetail = ActivityClient.QueryA20140214SSCHBInfo(ViewBag.UserKey, ViewBag.StartTime, ViewBag.EndTime, ViewBag.PageIndex, ViewBag.PageSize, CurrentUser.UserToken);
//            return View();
//        }
//        #region 充值最高送1000
//        /// <summary>
//        /// 充值最高送1000
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult QueryA20140318FillReturn1000()
//        {
//            const int pageIndex = 0;
//            const int pageSize = 50;
//            var month = DateTime.Today.Month;
//            ViewBag.Month = month;
//            ViewBag.IsComplete = 0;
//            ViewBag.PageIndex = pageIndex;
//            ViewBag.PageSize = pageSize;
//            ViewBag.RecordList = ActivityClient.QueryA20140318Return1000InfoCollection(month, false, pageIndex, pageSize, CurrentUser.UserToken);
//            return View();
//        }
//        [HttpPost]
//        public ActionResult QueryA20140318FillReturn1000(string month, string isComplete, string pageIndex)
//        {
//            int m = DateTime.Today.Month, c, p;
//            int.TryParse(month, out m);
//            int.TryParse(isComplete, out c);
//            int.TryParse(pageIndex, out p);
//            ViewBag.Month = m;
//            ViewBag.IsComplete = c;
//            ViewBag.PageIndex = p;
//            ViewBag.PageSize = 50;
//            ViewBag.RecordList = ActivityClient.QueryA20140318Return1000InfoCollection(m, c != 0, p, 50, CurrentUser.UserToken);
//            return View();
//        }
//        public JsonResult DoGiveMoney20140318(string recordIdStr)
//        {
//            if (string.IsNullOrEmpty(recordIdStr))
//                return Json(new { IsSuccess = false, Msg = "还未选中任何项" });
//            try
//            {
//                this.ActivityClient.Activity2014ReturnGifts(recordIdStr, this.CurrentUser.UserToken);
//                return Json(new { IsSuccess = true, Msg = "赠送成功" });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message });
//            }
//        }
//        #endregion
//        #region 竞彩、北单奖上奖
//        /// <summary>
//        /// 充值最高送1000
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult QueryA20140318PrizeDouble()
//        {
//            ViewBag.PageIndex = 0;
//            ViewBag.PageSize = 10;
//            ViewBag.RecordList = ActivityClient.QueryA20140318PrizeDouble(string.Empty, 0, string.Empty, string.Empty, string.Empty, string.Empty, 0, 10, CurrentUser.UserToken);
//            return View();
//        }
//        [HttpPost]
//        public ActionResult QueryA20140318PrizeDouble(string userId, string schemeId, string gameCode, string issuseNumber, string pageIndex)
//        {
//            ViewBag.UserId = userId;
//            ViewBag.SchemeId = schemeId;
//            ViewBag.GameCode = gameCode;
//            ViewBag.IssuseNumber = issuseNumber;
//            var m = 0;
//            int.TryParse(pageIndex, out m);
//            ViewBag.PageIndex = m;
//            ViewBag.PageSize = 10;
//            ViewBag.RecordList = ActivityClient.QueryA20140318PrizeDouble(userId, 0, schemeId, gameCode, string.Empty, issuseNumber, m, 10, CurrentUser.UserToken);
//            return View();
//        }
//        #endregion
//        #region 发布会赠送记录
//        public ActionResult QueryA20140318PublishData()
//        {
//            //var list = ActivityClient.QueryA20140318InfoList(0, 50, CurrentUser.UserToken);
//            ViewBag.PageIndex = 0;
//            ViewBag.PageSize = 50;
//            //ViewBag.RecordList = list;
//            return View();
//        }
//        [HttpPost]
//        public ActionResult QueryA20140318PublishData(string pageIndex)
//        {
//            var pindex = 0;
//            int.TryParse(pageIndex, out pindex);
//            const int psize = 50;
//            ViewBag.PageIndex = pindex;
//            ViewBag.PageSize = psize;
//            //var list = ActivityClient.QueryA20140318InfoList(pindex, psize, CurrentUser.UserToken);
//            //ViewBag.RecordList = list;
//            return View();
//        }
//        #endregion

//        /// <summary>
//        /// 购彩返利
//        /// </summary>
//        public ActionResult BuyLotteryRebate()
//        {
//            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? base.PageIndex : int.Parse(Request["pageIndex"]);
//            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? base.PageSize : int.Parse(Request["pageSize"]);
//            ViewBag.UserKey = string.IsNullOrWhiteSpace(Request["userKey"]) ? "" : Request["userKey"]
//;
//            ViewBag.StartTime = string.IsNullOrWhiteSpace(Request["startTime"]) ? DateTime.Now : DateTime.Parse(Request["startTime"]);
//            ViewBag.EndTime = string.IsNullOrWhiteSpace(Request["endTime"]) ? DateTime.Now : DateTime.Parse(Request["endTime"]);
//            ViewBag.QueryA20140902Detail = ActivityClient.QueryA20140902_BuyLotteryRebate(ViewBag.UserKey, ViewBag.StartTime, ViewBag.EndTime, ViewBag.PageIndex, ViewBag.PageSize, CurrentUser.UserToken);
//            return View();
//        }

//        /// <summary>
//        /// 加奖百分之一十八
//        /// </summary>
//        public ActionResult AddMoneryEighteenPercent()
//        {
//            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? base.PageIndex : int.Parse(Request["pageIndex"]);
//            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? base.PageSize : int.Parse(Request["pageSize"]);
//            ViewBag.UserKey = string.IsNullOrWhiteSpace(Request["userKey"]) ? "" : Request["userKey"]
//;
//            ViewBag.StartTime = string.IsNullOrWhiteSpace(Request["startTime"]) ? DateTime.Now : DateTime.Parse(Request["startTime"]);
//            ViewBag.EndTime = string.IsNullOrWhiteSpace(Request["endTime"]) ? DateTime.Now : DateTime.Parse(Request["endTime"]);
//            ViewBag.AddMoneryEighteenPercent = ActivityClient.QueryA20140902_AddMoneryEighteenPercent(ViewBag.UserKey, ViewBag.StartTime, ViewBag.EndTime, ViewBag.PageIndex, ViewBag.PageSize, CurrentUser.UserToken);
//            return View();
//        }

//        /// <summary>
//        /// 足彩不中也有奖（安慰奖）
//        /// </summary>
//        public ActionResult FootballConsolationPrizeList()
//        {
//            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? base.PageIndex : int.Parse(Request["pageIndex"]);
//            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? base.PageSize : int.Parse(Request["pageSize"]);
//            ViewBag.UserKey = string.IsNullOrWhiteSpace(Request["userKey"]) ? "" : Request["userKey"]
//;
//            ViewBag.StartTime = string.IsNullOrWhiteSpace(Request["startTime"]) ? DateTime.Now : DateTime.Parse(Request["startTime"]);
//            ViewBag.EndTime = string.IsNullOrWhiteSpace(Request["endTime"]) ? DateTime.Now : DateTime.Parse(Request["endTime"]);
//            ViewBag.FootballConsolationPrize = ActivityClient.QueryFootballConsolationPrizeList(ViewBag.UserKey, ViewBag.StartTime, ViewBag.EndTime, ViewBag.PageIndex, ViewBag.PageSize, CurrentUser.UserToken);
//            return View();
//        }

//        /// <summary>
//        /// 购彩不花钱
//        /// </summary>
//        public ActionResult BuyLotteryNoMoney()
//        {
//            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? base.PageIndex : int.Parse(Request["pageIndex"]);
//            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? base.PageSize : int.Parse(Request["pageSize"]);
//            ViewBag.UserKey = string.IsNullOrWhiteSpace(Request["userKey"]) ? "" : Request["userKey"]
//;
//            ViewBag.StartTime = string.IsNullOrWhiteSpace(Request["startTime"]) ? DateTime.Now : DateTime.Parse(Request["startTime"]);
//            ViewBag.EndTime = string.IsNullOrWhiteSpace(Request["endTime"]) ? DateTime.Now : DateTime.Parse(Request["endTime"]);
//            ViewBag.BuyLotteryNoMoney = ActivityClient.QueryBuyLotteryNoMoneyList(ViewBag.UserKey, ViewBag.StartTime, ViewBag.EndTime, ViewBag.PageIndex, ViewBag.PageSize, CurrentUser.UserToken);
//            return View();
//        }

//        /// <summary>
//        /// 首冲送钱
//        /// </summary>
//        public ActionResult FistFillMoneyList()
//        {
//            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? base.PageIndex : int.Parse(Request["pageIndex"]);
//            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? base.PageSize : int.Parse(Request["pageSize"]);
//            ViewBag.UserKey = string.IsNullOrWhiteSpace(Request["userKey"]) ? "" : Request["userKey"]
//;
//            ViewBag.StartTime = string.IsNullOrWhiteSpace(Request["startTime"]) ? DateTime.Now : DateTime.Parse(Request["startTime"]);
//            ViewBag.EndTime = string.IsNullOrWhiteSpace(Request["endTime"]) ? DateTime.Now : DateTime.Parse(Request["endTime"]);
//            ViewBag.FistFillMoneyList = ActivityClient.QueryFistFillMoneyList(ViewBag.UserKey, ViewBag.StartTime, ViewBag.EndTime, ViewBag.PageIndex, ViewBag.PageSize, CurrentUser.UserToken);
//            return View();
//        }

//        #endregion

//        /// <summary>
//        /// 产品发布会现场活动
//        /// </summary>
//        public ActionResult A20140321()
//        {
//            ViewBag.CurrentIssuse = this.GameIssuseClient.QueryCurrentIssuseInfoWithOffical("CQSSC");
//            return View();
//        }

//        public ActionResult A20140525()
//        {
//            ViewBag.CQSSC_CurrentIssuse = this.GameIssuseClient.QueryCurrentIssuseInfoWithOffical("CQSSC");
//            ViewBag.SSQ_CurrentIssuse = this.GameIssuseClient.QueryCurrentIssuseInfoWithOffical("SSQ");

//            return View();
//        }
//        public ActionResult A20140531()
//        {
//            ViewBag.CQSSC_CurrentIssuse = this.GameIssuseClient.QueryCurrentIssuseInfoWithOffical("CQSSC");

//            return View();
//        }

//        public JsonResult DoGive0531()
//        {
//            try
//            {
//                var txt = Request.Form["mobile"];
//                var mobileArray = txt.Split(new string[] { "\r\n", "\r", "\n", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
//                foreach (var item in mobileArray)
//                {
//                    if (string.IsNullOrEmpty(item))
//                        continue;

//                    var mobile = item;
//                    if (mobile.Length != 11)
//                        throw new Exception(string.Format("手机号 {0} 格式 不正确", mobile));

//                    var userId = string.Empty;
//                    var pwd = mobile.Substring(5);
//                    try
//                    {
//                        //尝试获取用户
//                        userId = this.ExternalClient.GetUserIdByLoginName(mobile);
//                    }
//                    catch (Exception)
//                    {
//                        //注册用户
//                        userId = this.ExternalClient.RegisterLoacal(new External.Core.Login.RegisterInfo_Local
//                        {
//                            RegType = "LOCAL",
//                            LoginName = mobile,
//                            Password = pwd,
//                        }).ReturnValue;
//                    }
//                    if (string.IsNullOrEmpty(userId))
//                        throw new Exception(string.Format("输入项 {0} 注册用户失败", item));
//                    try
//                    {
//                        //认证手机
//                        this.ExternalClient.AuthenticationUserMobile(userId, mobile, GameBiz.Core.SchemeSource.Web, this.CurrentUser.UserToken);
//                    }
//                    catch (Exception)
//                    {
//                    }

//                    var cqsscNumber = GetCQSSC_3StarNumber();
//                    try
//                    {

//                        //cqssc投注
//                        var cqssc_CurrentIssuse = this.GameIssuseClient.QueryCurrentIssuseInfoWithOffical("CQSSC");
//                        var cqssc_issuseList = new LotteryBettingIssuseInfoCollection();
//                        cqssc_issuseList.Add(new LotteryBettingIssuseInfo
//                        {
//                            Amount = 1,
//                            IssuseNumber = cqssc_CurrentIssuse.IssuseNumber,
//                            IssuseTotalMoney = 1 * 2M,
//                        });

//                        var cqssc_codeList = new LotteryAnteCodeInfoCollection();
//                        cqssc_codeList.Add(new LotteryAnteCodeInfo
//                        {
//                            GameType = "3XDX",
//                            IsDan = false,
//                            AnteCode = "-,-," + cqsscNumber,
//                        });

//                        var cqssc_betResult = this.GameClient.LotteryBettingWithUserId(new GameBiz.Core.LotteryBettingInfo
//                        {
//                            AnteCodeList = cqssc_codeList,
//                            BettingCategory = SchemeBettingCategory.GeneralBetting,
//                            GameCode = "CQSSC",
//                            IssuseNumberList = cqssc_issuseList,
//                            SchemeSource = SchemeSource.Publisher_0321,
//                            Security = TogetherSchemeSecurity.Public,
//                            StopAfterBonus = true,
//                            TotalMoney = 1 * 2M,
//                        }, userId, 0M, this.CurrentUser.UserToken);
//                        //this.ActivityClient.GiveLottery20140318(cqssc_betResult.ReturnValue.Split('|')[0], userId, mobile, 1, this.CurrentUser.UserToken);

//                    }
//                    catch (Exception)
//                    {

//                    }
//                    //发短信
//                    //                    
//                    var content = string.Format("您在【玩彩网】的登录账号为手机号码，密码为手机号后6位，本次送一注：{0}，请登录玩彩网修改密码。"
//                        , cqsscNumber);
//                    var returned = SendMessage(mobile, content);
//                    //SMSSenderFactory.SendSMS(mobile, content);

//                }

//                return Json(new { IsSuccess = true, Msg = "赠送成功" }, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
//            }
//        }

//        /// <summary>
//        /// 重庆时时彩 三星随机号码
//        /// </summary>
//        private string GetCQSSC_3StarNumber()
//        {
//            var random = new Random();
//            var temp = new List<string>();
//            for (int j = 0; j < 3; j++)
//            {
//                temp.Add(random.Next(0, 10).ToString());
//            }
//            return string.Join(",", temp.ToArray());
//        }

//        /// <summary>
//        /// 双色球单式号
//        /// </summary>
//        private string GetSSQ_DS()
//        {
//            var redList = new List<string>();
//            for (int i = 1; i <= 33; i++)
//            {
//                redList.Add(i.ToString().PadLeft(2, '0'));
//            }

//            var blueList = new List<string>();
//            for (int i = 1; i <= 16; i++)
//            {
//                blueList.Add(i.ToString().PadLeft(2, '0'));
//            }

//            var red_result_list = new List<string>();
//            var random = new Random();
//            while (true)
//            {
//                var index = random.Next(0, redList.Count);
//                var redNumber = redList[index];
//                if (red_result_list.Contains(redNumber)) continue;
//                red_result_list.Add(redNumber);
//                if (red_result_list.Count == 6)
//                    break;
//            }
//            var redNumberArray = (from r in red_result_list orderby r select r).ToArray();
//            var blue_index = random.Next(0, blueList.Count);
//            var blue_number = blueList[blue_index];

//            return string.Format("{0}|{1}", string.Join(",", redNumberArray), blue_number);
//        }

//        public JsonResult DoGive0525()
//        {
//            try
//            {
//                var txt = Request.Form["mobile"];
//                var mobileArray = txt.Split(new string[] { "\r\n", "\r", "\n", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
//                foreach (var item in mobileArray)
//                {
//                    if (string.IsNullOrEmpty(item))
//                        continue;

//                    var mobile = item;
//                    if (mobile.Length != 11)
//                        throw new Exception(string.Format("手机号 {0} 格式 不正确", mobile));

//                    var userId = string.Empty;
//                    var pwd = mobile.Substring(5);
//                    try
//                    {
//                        //尝试获取用户
//                        userId = this.ExternalClient.GetUserIdByLoginName(mobile);
//                    }
//                    catch (Exception)
//                    {
//                        //注册用户
//                        userId = this.ExternalClient.RegisterLoacal(new External.Core.Login.RegisterInfo_Local
//                        {
//                            RegType = "LOCAL",
//                            LoginName = mobile,
//                            Password = pwd,
//                        }).ReturnValue;
//                    }
//                    if (string.IsNullOrEmpty(userId))
//                        throw new Exception(string.Format("输入项 {0} 注册用户失败", item));
//                    try
//                    {
//                        //认证手机
//                        this.ExternalClient.AuthenticationUserMobile(userId, mobile, GameBiz.Core.SchemeSource.Web, this.CurrentUser.UserToken);
//                    }
//                    catch (Exception)
//                    {
//                    }

//                    var cqsscNumber = GetCQSSC_3StarNumber();
//                    try
//                    {

//                        //cqssc投注
//                        var cqssc_CurrentIssuse = this.GameIssuseClient.QueryCurrentIssuseInfoWithOffical("CQSSC");
//                        var cqssc_issuseList = new LotteryBettingIssuseInfoCollection();
//                        cqssc_issuseList.Add(new LotteryBettingIssuseInfo
//                        {
//                            Amount = 1,
//                            IssuseNumber = cqssc_CurrentIssuse.IssuseNumber,
//                            IssuseTotalMoney = 1 * 2M,
//                        });

//                        var cqssc_codeList = new LotteryAnteCodeInfoCollection();
//                        cqssc_codeList.Add(new LotteryAnteCodeInfo
//                        {
//                            GameType = "3XDX",
//                            IsDan = false,
//                            AnteCode = "-,-," + cqsscNumber,
//                        });

//                        var cqssc_betResult = this.GameClient.LotteryBettingWithUserId(new GameBiz.Core.LotteryBettingInfo
//                        {
//                            AnteCodeList = cqssc_codeList,
//                            BettingCategory = SchemeBettingCategory.GeneralBetting,
//                            GameCode = "CQSSC",
//                            IssuseNumberList = cqssc_issuseList,
//                            SchemeSource = SchemeSource.Publisher_0321,
//                            Security = TogetherSchemeSecurity.Public,
//                            StopAfterBonus = true,
//                            TotalMoney = 1 * 2M,
//                        }, userId, 0M, this.CurrentUser.UserToken);
//                        //this.ActivityClient.GiveLottery20140318(cqssc_betResult.ReturnValue.Split('|')[0], userId, mobile, 1, this.CurrentUser.UserToken);

//                    }
//                    catch (Exception)
//                    {

//                    }
//                    var ssqNumber = GetSSQ_DS();
//                    try
//                    {
//                        //ssq投注
//                        var ssq_CurrentIssuse = this.GameIssuseClient.QueryCurrentIssuseInfoWithOffical("SSQ");
//                        var ssq_issuseList = new LotteryBettingIssuseInfoCollection();
//                        ssq_issuseList.Add(new LotteryBettingIssuseInfo
//                        {
//                            Amount = 1,
//                            IssuseNumber = ssq_CurrentIssuse.IssuseNumber,
//                            IssuseTotalMoney = 1 * 2M,
//                        });

//                        var ssq_codeList = new LotteryAnteCodeInfoCollection();
//                        ssq_codeList.Add(new LotteryAnteCodeInfo
//                        {
//                            GameType = "DS",
//                            IsDan = false,
//                            AnteCode = ssqNumber,
//                        });

//                        var ssq_betResult = this.GameClient.LotteryBettingWithUserId(new GameBiz.Core.LotteryBettingInfo
//                        {
//                            AnteCodeList = ssq_codeList,
//                            BettingCategory = SchemeBettingCategory.GeneralBetting,
//                            GameCode = "SSQ",
//                            IssuseNumberList = ssq_issuseList,
//                            SchemeSource = SchemeSource.Publisher_0321,
//                            Security = TogetherSchemeSecurity.Public,
//                            StopAfterBonus = true,
//                            TotalMoney = 1 * 2M,
//                        }, userId, 0M, this.CurrentUser.UserToken);
//                        //this.ActivityClient.GiveLottery20140318(ssq_betResult.ReturnValue.Split('|')[0], userId, mobile, 1, this.CurrentUser.UserToken);

//                    }
//                    catch (Exception)
//                    {

//                    }
//                    //发短信
//                    //                    
//                    var content = string.Format("您在【玩彩网】的登录账号为手机号码，密码为手机号后6位，本次送一注：{0}，一注：{1}，请登录玩彩网修改密码。"
//                        , cqsscNumber, ssqNumber);
//                    //SMSSenderFactory.SendSMS(mobile, content);
//                    var returned = SendMessage(mobile, content);
//                    //添加活动记录
//                }

//                return Json(new { IsSuccess = true, Msg = "赠送成功" }, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
//            }
//        }

//        public JsonResult LoadCurrentIssuse()
//        {
//            try
//            {
//                var currentIssuse = this.GameIssuseClient.QueryCurrentIssuseInfoWithOffical("CQSSC");
//                return Json(new { IsSuccess = true, Msg = "获取成功", IssuseNumber = currentIssuse.IssuseNumber, StopTime = currentIssuse.LocalStopTime.ToString("yyyy-MM-dd HH:mm:ss") }, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
//            }
//        }

//        /// <summary>
//        /// 重庆时时彩 二星全部号码
//        /// </summary>
//        private List<string> GetCQSSC_2StarNumber(int count)
//        {
//            var list = new List<string>();
//            var ac = new ArrayCombination();
//            var s1 = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
//            var s2 = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
//            var s = new int[][] { s1, s2 };

//            for (int i = 0; i < count; i++)
//            {
//                ac.Calculate(s, (a) =>
//                {
//                    list.Add(string.Join(",", a));
//                });
//            }

//            return list;
//        }

//        public JsonResult DoGive()
//        {
//            try
//            {
//                var allCode = GetCQSSC_2StarNumber(5);

//                var issuseNumber = Request.Form["issuseNumber"];
//                var txt = Request.Form["mobile"];
//                var mobileArray = txt.Split(new string[] { "\r\n", "\r", "\n", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
//                foreach (var item in mobileArray)
//                {
//                    if (string.IsNullOrEmpty(item))
//                        throw new Exception("输入数据中有空行");
//                    var oneArray = item.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
//                    if (oneArray.Length != 2)
//                        throw new Exception(string.Format("输入项 {0} 不正确", item));

//                    var mobile = oneArray[0];
//                    var count = int.Parse(oneArray[1]);
//                    if (mobile.Length != 11)
//                        throw new Exception(string.Format("手机号 {0} 格式 不正确", mobile));
//                    if (count <= 0)
//                        throw new Exception(string.Format("输入项 {0} 包括的注数不正确", item));

//                    var userId = string.Empty;
//                    var pwd = mobile.Substring(5);
//                    try
//                    {
//                        //尝试获取用户
//                        userId = this.ExternalClient.GetUserIdByLoginName(mobile);
//                    }
//                    catch (Exception)
//                    {
//                        //注册用户
//                        userId = this.ExternalClient.RegisterLoacal(new External.Core.Login.RegisterInfo_Local
//                        {
//                            RegType = "LOCAL",
//                            LoginName = mobile,
//                            Password = pwd,
//                        }).ReturnValue;
//                    }
//                    if (string.IsNullOrEmpty(userId))
//                        throw new Exception(string.Format("输入项 {0} 注册用户失败", item));
//                    try
//                    {
//                        //认证手机
//                        this.ExternalClient.AuthenticationUserMobile(userId, mobile, GameBiz.Core.SchemeSource.Web, this.CurrentUser.UserToken);
//                    }
//                    catch (Exception)
//                    {
//                    }

//                    //投注
//                    var issuseList = new LotteryBettingIssuseInfoCollection();
//                    issuseList.Add(new LotteryBettingIssuseInfo
//                    {
//                        Amount = 1,
//                        IssuseNumber = issuseNumber,
//                        IssuseTotalMoney = count * 2M,
//                    });

//                    var codeList = new LotteryAnteCodeInfoCollection();
//                    var getList = allCode.Take(count).ToList();
//                    foreach (var code in getList)
//                    {
//                        codeList.Add(new LotteryAnteCodeInfo
//                        {
//                            GameType = "2XDX",
//                            IsDan = false,
//                            AnteCode = "-,-,-," + code,
//                        });

//                        allCode.RemoveAt(0);
//                    }


//                    var betResult = this.GameClient.LotteryBettingWithUserId(new GameBiz.Core.LotteryBettingInfo
//                    {
//                        AnteCodeList = codeList,
//                        BettingCategory = SchemeBettingCategory.GeneralBetting,
//                        GameCode = "CQSSC",
//                        IssuseNumberList = issuseList,
//                        SchemeSource = SchemeSource.Publisher_0321,
//                        Security = TogetherSchemeSecurity.Public,
//                        StopAfterBonus = true,
//                        TotalMoney = count * 2M,
//                    }, userId, 0M, this.CurrentUser.UserToken);


//                    //发短信
//                    var content = string.Format("您在【玩彩网】的登录名为：{0}，密码为手机号后6位，即：{1}。 重庆时时彩 二星单选 {2}，祝您好运！"
//                        , mobile, pwd, string.Join(" ", getList));
//                    //SMSSenderFactory.SendSMS(mobile, content);
//                    var returned = SendMessage(mobile, content);
//                    //添加活动记录
//                    //this.ActivityClient.GiveLottery20140318(betResult.ReturnValue.Split('|')[0], userId, mobile, count, this.CurrentUser.UserToken);
//                }

//                return Json(new { IsSuccess = true, Msg = "赠送成功" }, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
//            }
//        }


//        #region 改版活动菜单

//        /// <summary>
//        /// 网站活动配置
//        /// </summary>
//        public ActionResult ActivityConfig()
//        {
//            if (!CheckRights("HDPZ"))
//                throw new Exception("对不起，您的权限不足！");
//            ViewBag.HDPZ_XG = CheckRights("HDPZ_XG");
//            ViewBag.ConfigArray = this.ActivityClient.GetActityConfig().Split('|');
//            return View();
//        }

//        public JsonResult ModifyActivityConfig()
//        {
//            try
//            {
//                var key = Request["config_key"];
//                var value = Request["config_value"];
//                var r = this.ActivityClient.UpdateActivityConfig(key, value);
//                return Json(new { IsSuccess = true, Msg = r.Message }, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
//            }
//        }

//        /// <summary>
//        /// 充值赠送红包配置
//        /// </summary>
//        public ActionResult FillGiveRedBagConfig()
//        {
//            if (!CheckRights("CZZSPZ"))
//                throw new Exception("对不起，您的权限不足！");
//            ViewBag.CZZSPZ_GXGZ = CheckRights("CZZSPZ_GXGZ");
//            ViewBag.CZZSPZ_SCGZ = CheckRights("CZZSPZ_SCGZ");
//            ViewBag.ConfigList = this.ActivityClient.QueryFillMoneyGiveRedBagConfigList();

//            return View();
//        }

//        public JsonResult AddFillGiveRedBagConfig()
//        {
//            try
//            {
//                var fillMoney = decimal.Parse(Request["fillMoney"]);
//                var giveMoney = decimal.Parse(Request["giveMoney"]);
//                var r = this.ActivityClient.AddFillMoneyGiveRedBagConfig(fillMoney, giveMoney);
//                return Json(new { IsSuccess = true, Msg = r.Message }, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
//            }
//        }

//        public JsonResult UpdateFillGiveRedBagConfig()
//        {
//            try
//            {
//                var id = int.Parse(Request["id"]);
//                var fillMoney = decimal.Parse(Request["fillMoney"]);
//                var giveMoney = decimal.Parse(Request["giveMoney"]);
//                var r = this.ActivityClient.UpdateFillMoneyGiveRedBagConfig(id, fillMoney, giveMoney);
//                return Json(new { IsSuccess = true, Msg = r.Message }, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
//            }
//        }

//        public JsonResult DeleteFillGiveRedBagConfig()
//        {
//            try
//            {
//                var id = int.Parse(Request["id"]);
//                var r = this.ActivityClient.DeleteFillMoneyGiveRedBagConfig(id);
//                return Json(new { IsSuccess = true, Msg = r.Message }, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
//            }
//        }

//        /// <summary>
//        /// 红包使用规则
//        /// </summary>
//        public ActionResult RedBagUseConfig()
//        {
//            if (!CheckRights("HBSYGZ"))
//                throw new Exception("对不起，您的权限不足！");
//            ViewBag.HBSYGZ_TJGZ = CheckRights("HBSYGZ_TJGZ");
//            ViewBag.HBSYGZ_SCGZ = CheckRights("HBSYGZ_SCGZ");
//            ViewBag.List = this.ActivityClient.QueryRedBagUseConfig();
//            return View();
//        }

//        public JsonResult AddRedBagConfig()
//        {
//            try
//            {
//                var gameCode = Request["gameCode"];
//                var percent = decimal.Parse(Request["percent"]);
//                var r = this.ActivityClient.AddRedBagUseConfig(gameCode, percent);
//                return Json(new { IsSuccess = true, Msg = r.Message }, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
//            }
//        }

//        public JsonResult DeleteRedBagUseConfig()
//        {
//            try
//            {
//                var id = int.Parse(Request["id"]);
//                var r = this.ActivityClient.DeleteRedBagUseConfig(id);
//                return Json(new { IsSuccess = true, Msg = r.Message }, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
//            }
//        }

//        /// <summary>
//        /// 彩种加奖配置
//        /// </summary>
//        public ActionResult AddBonusMoneyConfig()
//        {
//            if (!CheckRights("CZJJPZ"))
//                throw new Exception("对不起，您的权限不足！");
//            ViewBag.CZJJPZ_TJGZ = CheckRights("CZJJPZ_TJGZ");
//            ViewBag.CZJJPZ_SCGZ = CheckRights("CZJJPZ_SCGZ");
//            ViewBag.ConfigList = this.ActivityClient.QueryAddBonusMoneyConfig();
//            return View();
//        }

//        public JsonResult AddAddBonusMoneyConfig()
//        {
//            try
//            {
//                var orderIndex = int.Parse(Request["orderIndex"]);
//                var gameCode = Request["gameCode"];
//                var gameType = Request["gameType"];
//                var playType = Request["playType"];
//                var percent = decimal.Parse(Request["percent"]);
//                var maxMoney = decimal.Parse(Request["maxMoney"]);
//                var addMoneyWay = Request["addMoneyWay"];
//                var r = this.ActivityClient.AddAddBonusMoneyConfig(gameCode, gameType, playType, percent, maxMoney, orderIndex, addMoneyWay);
//                return Json(new { IsSuccess = true, Msg = r.Message }, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
//            }
//        }

//        public JsonResult DeleteAddBonusMoneyConfig()
//        {
//            try
//            {
//                var id = int.Parse(Request["id"]);
//                var r = this.ActivityClient.DeleteAddBonusMoneyConfig(id);
//                return Json(new { IsSuccess = true, Msg = r.Message }, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
//            }
//        }

//        /// <summary>
//        /// 彩种取消加奖用户列表
//        /// </summary>
//        public ActionResult UserGameCodeNotAddMoney()
//        {
//            //    if (!CheckRights("CZJJPZ"))
//            //        throw new Exception("对不起，您的权限不足！");
//            //    ViewBag.CZJJPZ_TJGZ = CheckRights("CZJJPZ_TJGZ");
//            //    ViewBag.CZJJPZ_SCGZ = CheckRights("CZJJPZ_SCGZ");
//            ViewBag.UserId = string.IsNullOrWhiteSpace(Request["userId"]) ? string.Empty : Request["userId"].ToString();
//            ViewBag.UserGameCodeNotAddMoneyList = this.ActivityClient.QueryUserGameCodeNotAddMoneyList(ViewBag.UserId);
//            return View();
//        }

//        public JsonResult AddUserGameCodeNotAddMoney()
//        {
//            try
//            {
//                var userIdList = Request["userId"];
//                var gameCode = Request["gameCode"];
//                var gameType = Request["gameType"];
//                var playType = Request["playType"];
//                if (string.IsNullOrEmpty(gameCode))
//                    throw new Exception("彩种传入不能为空。");

//                var msgList = new List<string>();
//                foreach (var userId in userIdList.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries))
//                {
//                    if (string.IsNullOrEmpty(userId))
//                        continue;
//                    try
//                    {
//                        var result = this.ActivityClient.AddUserGameCodeNotAddMoney(userId, gameCode, gameType, playType);
//                        msgList.Add(string.Format("用户{0}添加结果：{1}", userId, result.Message));
//                    }
//                    catch (Exception ex)
//                    {
//                        msgList.Add(string.Format("用户{0}添加结果：{1}", userId, ex.Message));
//                    }
//                }
//                return Json(new { IsSuccess = true, Msg = string.Join("\r\n", msgList.ToArray()) }, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
//            }
//        }

//        public JsonResult DeleteUserGameCodeNotAddMoney()
//        {
//            try
//            {
//                var id = int.Parse(Request["id"]);
//                var r = this.ActivityClient.DeleteUserGameCodeNotAddMoney(id);
//                return Json(new { IsSuccess = true, Msg = r.Message }, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception ex)
//            {
//                return Json(new { IsSuccess = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
//            }
//        }

//        #endregion


//        public ActionResult PayRedBagConfig()
//        {
//            Dictionary<int, decimal> dict = new Dictionary<int, decimal>();
//            var payRedBagConfig = GameClient.QueryCoreConfigByKey("PayRedBagConfig").ConfigValue;
//            if (!string.IsNullOrEmpty(payRedBagConfig))
//            {
//                var configs = payRedBagConfig.Split(',');
//                int i = 0;
//                decimal f = 0;
//                foreach (var item in configs)
//                {
//                    var arr = item.Split('|');
//                    if (arr.Length == 2)
//                    {
//                        i = 0;
//                        f = 0;
//                        int.TryParse(arr[0], out i);
//                        decimal.TryParse(arr[1], out f);
//                        dict.Add(i, f);
//                    }
//                }
//            }
//            ViewBag.PayRedBagConfig = dict;
//            return View();
//        }
//    }
//}
