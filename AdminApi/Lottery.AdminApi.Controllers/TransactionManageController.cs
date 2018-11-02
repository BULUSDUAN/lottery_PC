using EntityModel;
using EntityModel.Communication;
using EntityModel.CoreModel;
using EntityModel.Enum;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Utilities;
using KaSon.FrameWork.ORM.Helper;
using Lottery.AdminApi.Controllers.CommonFilterActtribute;
using Lottery.AdminApi.Model.HelpModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.AdminApi.Controllers
{
    [Area("api")]
    [ReusltFilter]
    public class TransactionManageController:BaseController
    {
        private readonly static AdminService _service = new AdminService();

        #region 订单查询
        /// <summary>
        /// 订单查询
        /// </summary>
        public ActionResult OrderQuery(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("J101"))
                    throw new Exception("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);

                //bool ckddxq = false;
                //bool ckhyxq = false;
                //bool ckqtdd = false;
                //bool dcsj = false;
                //if (CheckRights("CKDDXQ100"))
                //    ckddxq = true;
                //if (CheckRights("CKHYXQ110"))
                //    ckhyxq = true;
                //if (CheckRights("CKQTDD120"))
                //    ckqtdd = true;
                //if (CheckRights("DCSJ130"))
                //    dcsj = true;
                //ViewBag.ckddxq = ckddxq;
                //ViewBag.ckhyxq = ckhyxq;
                //ViewBag.ckqtdd = ckqtdd;
                //ViewBag.dcsj = dcsj;
                //ViewBag.CKDDRS = CheckRights("CKDDRS");
                //ViewBag.CKDDTS = CheckRights("CKDDTS");
                //ViewBag.CKSJTZ = CheckRights("CKSJTZ");
                //ViewBag.CKSQJE = CheckRights("CKSQJE");
                //ViewBag.CKSHJE = CheckRights("CKSHJE");
                //ViewBag.CKJJE = CheckRights("CKJJE");
                //ViewBag.CKDDTZJE = CheckRights("CKDDTZJE");
                //ViewBag.CKDDRBJE = CheckRights("CKDDRBJE");
                var GameList =_service.QueryGameList(CurrentUser.UserToken);
                var PageIndex = string.IsNullOrWhiteSpace((string)p.pageIndex) ? base.PageIndex : Convert.ToInt32((string)p.pageIndex);
                var PageSize = string.IsNullOrWhiteSpace((string)p.pageSize) ? base.PageSize : Convert.ToInt32((string)p.pageSize);
                var UserKey = string.IsNullOrWhiteSpace((string)p.userKey) ? "" : p.userKey;
                var StartTime = string.IsNullOrWhiteSpace((string)p.startTime) ? DateTime.Today : Convert.ToDateTime((string)p.startTime);
                var EndTime = string.IsNullOrWhiteSpace((string)p.endTime) ? DateTime.Today : Convert.ToDateTime((string)p.endTime);
                var GameCode = string.IsNullOrWhiteSpace((string)p.gameCode) ? "" : (string)p.gameCode;
                var bonusStatus = string.IsNullOrWhiteSpace((string)p.bonusStatus) ? "" : (string)p.bonusStatus;
                var betCategory = string.IsNullOrWhiteSpace((string)p.betCategory) ? "" : (string)p.betCategory;
                var FieldName = string.IsNullOrWhiteSpace((string)p.fieldName) ? "" : (string)p.fieldName;
                var sortType = -1;
                var sortTypeStr = string.IsNullOrEmpty((string)p.sortType) ? "" : (string)p.sortType;
                if (!string.IsNullOrEmpty(sortTypeStr))
                {
                    sortType = sortTypeStr == "asc" ? 0 : 1;
                }
               var SortType = sortTypeStr;
                //方案类型
                SchemeType? schemeType = null;
                if (!string.IsNullOrWhiteSpace((string)p.schemeType))
                {
                    schemeType = (SchemeType)Convert.ToInt32((string)p.schemeType);
                }
                var SchemeType = schemeType;

                //进度
                ProgressStatus? progressStatus = null;
                if (!string.IsNullOrWhiteSpace((string)p.progressStatus))
                {
                    progressStatus = (ProgressStatus)Convert.ToInt32((string)p.progressStatus);
                }
                var ProgressStatus = progressStatus;

                //中奖状态
                BonusStatus? BonusStatu = null;
                if (bonusStatus == "1")
                {
                    BonusStatu = BonusStatus.Win;
                }
                if (bonusStatus == "2")
                {
                    BonusStatu = BonusStatus.Lose;
                }

                //出票状态
                TicketStatus? ticketStatus = null;
                if (!string.IsNullOrWhiteSpace((string)p.ticketStatus))
                {
                    ticketStatus = (TicketStatus)Convert.ToInt32((string)p.ticketStatus);
                    // progressStatus = ProgressStatus.Running;
                }
                var TicketStatus = ticketStatus;
                ///投注类别
                SchemeBettingCategory? betCategorys = null;
                if (betCategory == "0")
                {
                    betCategorys = SchemeBettingCategory.GeneralBetting;
                }
                if (betCategory == "1")
                {
                    betCategorys = SchemeBettingCategory.SingleBetting;
                }
                if (betCategory == "2")
                {
                    betCategorys = SchemeBettingCategory.FilterBetting;
                }
                if (betCategory == "3")
                    betCategorys = SchemeBettingCategory.YouHua;
                if (betCategory == "4")
                    betCategorys = SchemeBettingCategory.XianFaQiHSC;
                if (betCategory == "5")
                    betCategorys = SchemeBettingCategory.ErXuanYi;
                if (betCategory == "8")
                    betCategorys = SchemeBettingCategory.HunHeDG;

                //投注来源
                SchemeSource? schemeSource = null;
                if ((string)p.SchemeSource != null && !string.IsNullOrEmpty((string)p.SchemeSource))
                {
                    schemeSource = (SchemeSource)int.Parse((string)p.SchemeSource);
                    var SchemeSource = (string)p.SchemeSource.ToString();
                    var tou = ViewBag.SchemeSource;
                }
                else
                    ViewBag.SchemeSource = "";
                //BettingOrderInfoCollection orderList = base.QueryClient.QueryBettingOrderList(ViewBag.UserKey, schemeType, progressStatus,
                //    bonusStatus, betCategory, null, ViewBag.GameCode, ViewBag.StartTime, ViewBag.EndTime.AddDays(1), sortType, ViewBag.PageIndex, ViewBag.PageSize, CurrentUser.UserToken, ticketStatus, schemeSource);
                var orderInfo = new OrderInfo();
                BettingOrderInfoCollection orderList = _service.QueryBettingOrderList(UserKey, schemeType, progressStatus,
                BonusStatu, betCategorys, null, GameCode, StartTime, EndTime.AddDays(1), sortType, PageIndex, PageSize, CurrentUser.UserToken, FieldName, ticketStatus, schemeSource);
                orderInfo.BettingOrderInfo = orderList;
                if (orderList != null)
                {
                    IEnumerable<IGrouping<DateTime, BettingOrderInfo>> groupList = orderList.OrderList.GroupBy(o => o.BetTime.Date);
                    orderInfo.BettingOrder = groupList;
                    return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = orderInfo });
                }
               
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = "订单信息有错" });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }

        public ActionResult OrderDetail(LotteryServiceRequest entity)
        {
            try
            {

                if (!CheckRights("J102"))
                    throw new Exception("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);
                var OrderID = PreconditionAssert.IsNotEmptyString((string)p.orderID, "订单编号不能为空");

                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = OrderID });
               
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }

        #endregion

        #region 订单明细

        //更新订单缓存
        public JsonResult UpdateOrderCache(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var schemeId = PreconditionAssert.IsNotEmptyString((string)p.schemeId, "订单号不能为空！");
                var result = _service.ManualDeleteOrderCache(schemeId);
                return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }


        }

        public ActionResult OrderDetailInfo_Detail(LotteryServiceRequest entity)
        {
            try
            {
                //bool gxddhc = false;//GXDDHC120 更新订单缓存
                //bool sggxggtj = false;//SGGXGGTJ100  手工更新过关统计
                //bool sgpcx = false;// SGPCX110 手工票查询
                //bool sgtz = false;// SGTZ130  手工投注
                //bool tzsb = false;// SGTZSB140 投注失败
                //bool sgyddd = false;//SGYDDD160 手工移动订单
                //bool xgddwypj = false;// XGDDWYPJ150 修改订单为已派奖
                //bool xgddzj = false;// XGDDZJ170 修改订单中奖
                //if (CheckRights("GXDDHC120"))
                //    gxddhc = true;
                //if (CheckRights("SGGXGGTJ100"))
                //    sggxggtj = true;
                //if (CheckRights("SGPCX110"))
                //    sgpcx = true;
                //if (CheckRights("SGTZ130"))
                //    sgtz = true;
                //if (CheckRights("SGTZSB140"))
                //    tzsb = true;
                //if (CheckRights("SGYDDD160"))
                //    sgyddd = true;
                //if (CheckRights("XGDDWYPJ150"))
                //    xgddwypj = true;
                //if (CheckRights("XGDDZJ170"))
                //    xgddzj = true;
                //ViewBag.gxddhc = gxddhc;
                //ViewBag.sggxggtj = sggxggtj;
                //ViewBag.sgpcx = sgpcx;
                //ViewBag.sgtz = sgtz;
                //ViewBag.tzsb = tzsb;
                //ViewBag.sgyddd = sgyddd;
                //ViewBag.xgddwypj = xgddwypj;
                //ViewBag.xgddzj = xgddzj;
                var p = JsonHelper.Decode(entity.Param);
                var OrderID = PreconditionAssert.IsNotEmptyString((string)p.orderID, "订单编号不能为空");
                var SportsOrder = new OrderDetailInfo();
                SportsOrder.OrderDetail =_service.QuerySportsSchemeInfo(OrderID);
                SportsOrder.CodeList = _service.QuerySportsOrderAnteCodeList(OrderID);
                if (SportsOrder.OrderDetail != null)
                {
                    SportsOrder.UserKey = SportsOrder.OrderDetail.UserId;
                }
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = SportsOrder });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }
        public ActionResult OrderDetailInfo_Chase(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var OrderID = PreconditionAssert.IsNotEmptyString((string)p.orderID, "订单编号不能为空");
                var detailResult = _service.QueryBettingOrderListByChaseKeyLine(OrderID);
                var orderDetailInfo = new OrderDetailInfo();
                orderDetailInfo.DetailResult = detailResult;
                if (detailResult.OrderList.Count > 0)
                {
                    orderDetailInfo.FirstItem = detailResult.OrderList[0];
                    orderDetailInfo.FirstAnteCode = _service.QueryAnteCodeListBySchemeId(detailResult.OrderList[0].SchemeId);
                    orderDetailInfo.UserKey = detailResult.OrderList[0].UserId;
                }
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = orderDetailInfo });

            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }

        #endregion

        #region 手工处理订单
        //public JsonResult ManualUpdateHitCount(LotteryServiceRequest entity)
        //{
        //    try
        //    {
        //        var p = JsonHelper.Decode(entity.Param);
        //        var schemeId = PreconditionAssert.IsNotEmptyString((string)p.schemeId, "订单号不能为空！");
        //        var gameCode = PreconditionAssert.IsNotEmptyString((string)p.gameCode, "编号不能为空！");


        //        var result = new CommonActionResult(false, "更新出错");
        //        switch (gameCode.ToUpper())
        //        {
        //            case "CTZQ":
        //                result = _service.Update_CTZQ_HitCountBySchemeId(schemeId);
        //                break;
        //            case "BJDC":
        //                result = _service.Update_BJDC_HitCountBySchemeId(schemeId, CurrentUser.UserToken);
        //                break;
        //            case "JCZQ":
        //                result = _service.Update_JCZQ_HitCountBySchemeId(schemeId, CurrentUser.UserToken);
        //                break;
        //            case "JCLQ":
        //                result = _service.Update_JCLQ_HitCountBySchemeId(schemeId, CurrentUser.UserToken);
        //                break;
        //        }
        //        return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { IsSuccess = false, Msg = ex.Message });
        //    }
        //}

        //投注失败
        public JsonResult ManualUpdateBetFail(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var schemeId = PreconditionAssert.IsNotEmptyString((string)p.schemeId, "订单号不能为空！");
                //var userId = PreconditionAssert.IsNotEmptyString(Request["userId"], "用户编号不能为空！");
                _service.AddSysOperationLog(schemeId, this.CurrentUser.UserId, "撤销订单", string.Format("操作员【{0}】撤销订单编号【{1}】", this.CurrentUser.UserId, schemeId));
                var result = _service.BetFail(schemeId);

                return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }

        #endregion

        /// <summary>
        /// 手工返点
        /// </summary>
        /// <returns></returns>
        public JsonResult ManualAgentPayIn(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string schemeId = (string)p.SchemeId;
                if (string.IsNullOrEmpty(schemeId))
                    throw new Exception("订单号不能为空！");
                var result = _service.ManualAgentPayIn(schemeId);
                return Json(new { result.IsSuccess, Msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }

        /// <summary>
        /// 重新派奖
        /// </summary>
        /// <returns></returns>
        public JsonResult RePrizeOrder(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                if ((string)p.SchemeId == null)
                    throw new Exception("订单号不能为空！");
                var result = _service.ManualPrizeOrder((string)p.SchemeId);
                return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }
        /// <summary>
        /// 手工出票
        /// </summary>
        /// <returns></returns>
        public JsonResult ManualBet(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                if ((string)p.SchemeId == null)
                    throw new Exception("订单号不能为空！");
                var result = _service.ManualBet((string)p.SchemeId);
                return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }


    }
}
