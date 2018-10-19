using EntityModel.Enum;
using EntityModel.ExceptionExtend;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Utilities;
using KaSon.FrameWork.ORM.Helper;
using Lottery.AdminApi.Controllers.CommonFilterActtribute;
using Lottery.AdminApi.Model.HelpModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Lottery.AdminApi.Controllers
{
    /// <summary>
    /// 比赛管理
    /// </summary>
    [Area("api")]
    [ReusltFilter]
    public class MacthManagerController : BaseController
    {
        private static readonly AdminService _service = new AdminService();
        /// <summary>
        /// 北单数据更新
        /// </summary>
        /// <returns></returns>
        public IActionResult UpdateBjdc(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var IssueNum = PreconditionAssert.IsNotEmptyString(p.IssueNum, "北单期号不能为空");
                var result = _service.ManualUpdate_BJDC_MatchList(IssueNum, CurrentUser.UserToken);
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        /// <summary>
        /// 北单结果更新
        /// </summary>
        /// <returns></returns>
        public IActionResult UpdateBJDCResult(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var IssueNum = PreconditionAssert.IsNotEmptyString(p.IssueNum, "北单期号不能为空");
                var result = _service.ManualUpdate_BJDC_MatchResultList(IssueNum, CurrentUser.UserToken);
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        /// <summary>
        /// 竞彩足球数据更新
        /// </summary>
        /// <returns></returns>
        public IActionResult UpdateJczq()
        {
            try
            {
                var result = _service.ManualUpdate_JCZQ_MatchList(CurrentUser.UserToken);
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        /// <summary>
        /// 竞彩足球SP更新
        /// </summary>
        /// <returns></returns>
        public IActionResult UpdateJczq_SP()
        {
            try
            {
                var result = _service.UpdateOddsList_JCZQ_Manual();
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public IActionResult UpdateJclq()
        {
            try
            {
                var result = _service.ManualUpdate_JCLQ_MatchList(CurrentUser.UserToken);
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public IActionResult UpdateJclq_SP()
        {
            try
            {
                var result = _service.UpdateOddsList_JCLQ_Manual();
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public IActionResult UpdateCtzq(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var gameType = PreconditionAssert.IsNotEmptyString(p.GameType, "玩法不能为空");
                var IssueNum = PreconditionAssert.IsNotEmptyString(p.IssueNum, "期号不能为空");

                var result = _service.ManualUpdate_CTZQ_MatchList(gameType, IssueNum, CurrentUser.UserToken);
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public IActionResult UpdateMatchId(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var gameCode = PreconditionAssert.IsNotEmptyString(p.GameCode, "彩种不能为空");
                var matchId = PreconditionAssert.IsNotEmptyString(p.MatchId, "比赛编号不能为空");
                var issuse = p.Issuse.ToString();

                var result = _service.DoMatchCancel(gameCode, matchId, issuse);
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public IActionResult EnableMatchBet(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                if (!CheckRights("B102"))
                    throw new LogicException("对不起，您的权限不足！");
                var gameCode = string.IsNullOrEmpty(p.GameCode) ? "JCZQ" : p.GameCode;
                var result = new LotteryServiceResponse() { Code = AdminResponseCode.成功 };
                switch (gameCode)
                {
                    case "JCZQ":
                        result.Value = _service.QueryCurrentJCZQMatchInfo(CurrentUser.UserToken);
                        break;
                    case "JCLQ":
                        result.Value = _service.QueryCurrentJCLQMatchInfo(CurrentUser.UserToken);
                        break;
                    case "BJDC":
                        result.Value = _service.QueryCurrentBJDCMatchInfo(CurrentUser.UserToken);
                        break;
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }

        }
        public IActionResult UpdateMatchPrivile(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("JYBS100"))
                    throw new LogicException("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);
                var gameCode = PreconditionAssert.IsNotEmptyString(p.GameCode, "请选择彩种");
                var matchId = PreconditionAssert.IsNotEmptyString(p.matchId, "比赛编号不能为空");
                var privilegesType = p.privilegesType;
                switch (gameCode)
                {
                    case "JCZQ":
                        _service.UpdateJCZQMatchInfo(matchId, privilegesType, CurrentUser.UserToken);
                        break;
                    case "JCLQ":
                        _service.UpdateJCLQMatchInfo(matchId, privilegesType, CurrentUser.UserToken);
                        break;
                    case "BJDC":
                        _service.UpdateBJDCMatchInfo(matchId, privilegesType);
                        break;
                    default:
                        break;
                }

                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = "操作成功" });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public JsonResult UpdateLotteryGame(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var gameCode = p.gameCode;
                var status = int.Parse(p.status);

                _service.UpdateLotteryGame(CurrentUser.UserToken, gameCode, status);
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = "修改成功！" });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }

        }
        public ActionResult QueryIndexMatchList(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("BS200"))
                    throw new Exception("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);
                string MatchId = p.MatchId ?? string.Empty;
                int PageIndex = p.PageIndex == null ? base.PageIndex : Convert.ToInt32(p.PageIndex);
                int PageSize = p.PageSize == null ? base.PageSize : Convert.ToInt32(p.PageSize);
                var chk = Convert.ToBoolean(p.HasImg);
                var hasImg = "-1";
                if (chk)
                    hasImg = string.Empty;
                //ViewBag.HasImg = chk;
                var IndexMatchList = _service.QueryIndexMatchCollection(MatchId, hasImg, PageIndex, PageSize);
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = IndexMatchList });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public ActionResult IndexMatchInfo(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                if (p.Id == null)
                    return null;
                var id = p.Id;
                var IndexMatchInfo = _service.QueryIndexMatchInfo(Convert.ToInt32(id));
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = IndexMatchInfo });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public IActionResult UpdateIndexMatch(LotteryServiceRequest entity, IFormFile loadfile)
        {
            try
            {
                if (!CheckRights("BS200_XG"))
                {
                    throw new Exception("对不起，您的权限不足！");
                }
                var p = JsonHelper.Decode(entity.Param);
                var id = PreconditionAssert.IsNotEmptyString(p.Id, "编号不能为空");
                var loadFile = loadfile;
                var matchId = p.MatchId;
                var imgPath = string.Empty;
                if (loadFile.Length <= 0)
                    imgPath = p.ImgPath;
                else
                    imgPath = LoadImageFile(loadFile, "/images/add/", matchId);
                var result = _service.UpdateIndexMatch(Convert.ToInt32(id), imgPath);
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
    }
}
