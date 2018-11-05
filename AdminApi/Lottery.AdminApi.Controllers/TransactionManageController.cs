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
                   
                }

                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = orderInfo });
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

        public JsonResult MoveRunningOrderToComplateOrder(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var schemeId = PreconditionAssert.IsNotEmptyString((string)p.schemeId, "订单号不能为空！");
                //移动订单数据
                var result = _service.MoveRunningOrderToComplateOrder(schemeId);
                return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }
        /// <summary>
        /// 修改订单中奖数据
        /// </summary>
        /// <returns></returns>
        public JsonResult ManualSetOrderBonusMoney(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var schemeId = PreconditionAssert.IsNotEmptyString((string)p.schemeId, "订单号不能为空！");
                var money = PreconditionAssert.IsNotEmptyString((string)p.bonusMoney, "中奖金额不能为空！");
                var bonusMoney = decimal.Parse(money);
                var bonusCount = PreconditionAssert.IsInt32((string)p.bonusCount, "中奖注数不能为空！");
                var hitMatchCount = PreconditionAssert.IsInt32((string)p.hitMatchCount, "命中场数不能为空！");
                var bonusCountDescription = PreconditionAssert.IsNotEmptyString((string)p.bonusCountDescription, "中奖注数描述不能为空！");
                var bonusCountDisplayName = PreconditionAssert.IsNotEmptyString((string)p.bonusCountDisplayName, "中奖注数显示名称不能为空！");
                //修改订单中奖金额
                var result = _service.ManualSetOrderBonusMoney(schemeId, bonusMoney, bonusCount, hitMatchCount, bonusCountDescription, bonusCountDisplayName);
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }

        public JsonResult ManualSetOrderNotBonus(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var schemeId = PreconditionAssert.IsNotEmptyString((string)p.schemeId, "订单号不能为空！");
                //var hitMatchCount = PreconditionAssert.IsInt32(Request["hitMatchCount"], "命中场数不能为空！");
                //设为不中奖
                var result = _service.ManualSetOrderNotBonus(schemeId, 0);
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public JsonResult ManualAnalyzeTogetherScheme(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var schemeId = PreconditionAssert.IsNotEmptyString((string)p.schemeId, "订单号不能为空！");
                var result = _service.AnalysisSchemeTogether(schemeId);
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
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

                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
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
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
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
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
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
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }

        #region 修改票状态

        public ActionResult UpdateSchemeTicketId(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var orderId = (string)p.orderId;

                var PageIndex = string.IsNullOrEmpty((string)p.pageIndex) ? 0 : int.Parse((string)p.pageIndex);
                var PageSize = string.IsNullOrEmpty((string)p.pageSize) ? 10 : int.Parse((string)p.pageSize);
                var list = _service.QuerySportsTicketList(orderId, ViewBag.PageIndex, ViewBag.PageSize);
                var result = new Sports_TicketQueryInfoCollection();
                //var List = list.TicketList;
                //var TotalCount = list.TotalCount;
                result.TicketList = list.TicketList;
                result.TotalCount= list.TotalCount;
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = result });

            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }

        public JsonResult DoUpdateSchemeTicket(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var ticketId = (string)p.ticketId;
                var bonusStatus = (BonusStatus)int.Parse((string)p.bonusStatus);
                var preMoney = decimal.Parse((string)p.preMoney);
                var aftMoney = decimal.Parse((string)p.aftMoney);

                var result = _service.UpdateSchemeTicket(ticketId, bonusStatus, preMoney, aftMoney);

                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = result.Message});
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }

        #endregion

        #region 订单奖金派奖
        public ActionResult OrderPrizeMoney(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                //if (!CheckRights("J105"))
                //    throw new Exception("对不起，您的权限不足！");
                //bool pj = false;
                //bool plpj = false;
                //bool pjckddxq = false;
                //bool pjckhyxq = false;
                //if (CheckRights("PJ100"))
                //    pj = true;
                //if (CheckRights("PLPJ110"))
                //    plpj = true;
                //if (CheckRights("PJCKDDXQ120"))
                //    pjckddxq = true;
                //if (CheckRights("PJCKHYXQ130"))
                //    pjckhyxq = true;
                //ViewBag.pj = pj;
                //ViewBag.plpj = plpj;
                //ViewBag.pjckddxq = pjckddxq;
                //ViewBag.pjckhyxq = pjckhyxq;
                var GameList = _service.QueryGameList(CurrentUser.UserToken);
                var PageIndex = string.IsNullOrWhiteSpace((string)p.pageIndex) ? base.PageIndex : Convert.ToInt32((string)p.pageIndex);
                var PageSize = string.IsNullOrWhiteSpace((string)p.pageSize) ? base.PageSize : Convert.ToInt32((string)p.pageSize);
                var UserKey = string.IsNullOrWhiteSpace((string)p.userKey) ? "" : (string)p.userKey;
                var StartTime = string.IsNullOrWhiteSpace((string)p.startTime) ? DateTime.Today.AddDays(-7) : Convert.ToDateTime((string)p.startTime);
                var EndTime = string.IsNullOrWhiteSpace((string)p.endTime) ? DateTime.Today : Convert.ToDateTime((string)p.endTime);
                var GameCode = string.IsNullOrWhiteSpace((string)p.gameCode) ? "" : (string)p.gameCode;
                var orderInfo = new OrderInfo();
                Sports_SchemeQueryInfoCollection orderList = _service.QueryWaitForPrizeMoneyOrderList(ViewBag.StartTime, ViewBag.EndTime.AddDays(+1), ViewBag.GameCode, ViewBag.PageIndex, ViewBag.PageSize);
                orderInfo.OrdersSearchResult = orderList;
                if (orderList != null)
                {
                    IEnumerable<IGrouping<DateTime, Sports_SchemeQueryInfo>> groupList = orderList.List.GroupBy(o => o.CreateTime.Date);
                    orderInfo.GroupOrderList = groupList;
                }

                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = orderInfo });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }

        public JsonResult DoPrizeMoney(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var schemeIdArr = PreconditionAssert.IsNotEmptyString((string)p.schemeIdArr, "传入派奖订单号不能为空！");
                var result = _service.SportsPrizeMoney(schemeIdArr);

                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        #endregion

        #region 奖期手工派奖
        public JsonResult ManualInterfacePrize(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string gameCodeStr = PreconditionAssert.IsNotEmptyString((string)p.gameCode, "彩种不能为空！");
                string issuseNumber = PreconditionAssert.IsNotEmptyString((string)p.issuseNumber, "期号不能为空！");
                string winNumber = PreconditionAssert.IsNotEmptyString((string)p.winNumber, "中奖号不能为空！");

                var gameCode = string.Empty;
                var gameType = string.Empty;
                var ozbArray = new string[] { "OZB_GJ", "OZB_GYJ", "SJB_GJ", "SJB_GYJ" };
                if (ozbArray.Contains(gameCodeStr))
                {
                    var array = gameCodeStr.Split('_');
                    if (array.Length == 2)
                    {
                        gameCode = array[0];
                        gameType = array[1];
                    }
                }
                else
                {
                    gameCode = gameCodeStr;
                }

                try
                {
                    //step 1 导入开奖号
                    var r = _service.ImportWinNumber(gameCode, issuseNumber, winNumber);
                }
                catch (Exception ex)
                {
                    return Json(new { IsSuccess = false, Msg = string.Format("{0}期导入开奖号码异常：{1}", issuseNumber, ex.Message) });
                }

                try
                {
                    //step 2 奖期派奖
                    var r = _service.IssusePrize(gameCode, gameType, issuseNumber, winNumber);
                }
                catch (Exception ex)
                {
                    return Json(new { IsSuccess = false, Msg = string.Format("{0}期【奖期】派奖异常：{1}", issuseNumber, ex.Message) });
                }

                try
                {
                    //step 3 生成相关静态数据
                    var dpc = new string[] { "FC3D", "PL3", "SSQ", "DLT" };

                    base.SendBuildStaticDataNotice("401", gameCode);

                    if (dpc.Contains(gameCode))
                    {
                        base.SendBuildStaticDataNotice("301", "");
                    }

                    base.SendBuildStaticDataNotice("302", gameCode);

                    base.SendBuildStaticDataNotice("303", gameCode);

                    if (dpc.Contains(gameCode))
                    {
                        base.SendBuildStaticDataNotice("10", "");
                    }

                    base.SendBuildStaticDataNotice("900", gameCode);
                }
                catch (Exception ex)
                {
                    return Json(new { IsSuccess = false, Msg = string.Format("生成静态数据异常：", ex.Message) });
                }
                return Json(new { IsSuccess = true, Msg = "执行成功" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }
        #endregion

    }
}
