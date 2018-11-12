using EntityModel.Enum;
using EntityModel.ExceptionExtend;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Utilities;
using KaSon.FrameWork.ORM.Helper;
using Lottery.AdminApi.Controllers.CommonFilterActtribute;
using Lottery.AdminApi.Model.HelpModel;
using Microsoft.AspNetCore.Cors;
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
    //[ReusltFilter]
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
                if (!CheckRights("B101"))
                {
                    throw new LogicException("对不起，您的权限不足！");
                }
                var p = JsonHelper.Decode(entity.Param);
                var IssueNum = PreconditionAssert.IsNotEmptyString((string)p.IssueNum, "北单期号不能为空");
                var result = _service.ManualUpdate_BJDC_MatchList(IssueNum, CurrentUser.UserToken);
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
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
                if (!CheckRights("B101"))
                {
                    throw new LogicException("对不起，您的权限不足！");
                }
                var p = JsonHelper.Decode(entity.Param);
                var IssueNum = PreconditionAssert.IsNotEmptyString((string)p.IssueNum, "北单期号不能为空");
                var result = _service.ManualUpdate_BJDC_MatchResultList(IssueNum, CurrentUser.UserToken);
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
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
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
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
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
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
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
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
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public IActionResult UpdateCtzq(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("B101"))
                {
                    throw new LogicException("对不起,您的权限不足!");
                }
                var p = JsonHelper.Decode(entity.Param);
                var gameType = PreconditionAssert.IsNotEmptyString((string)p.GameType, "玩法不能为空");
                var IssueNum = PreconditionAssert.IsNotEmptyString((string)p.IssueNum, "期号不能为空");

                var result = _service.ManualUpdate_CTZQ_MatchList(gameType, IssueNum, CurrentUser.UserToken);
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public IActionResult UpdateMatchId(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var gameCode = PreconditionAssert.IsNotEmptyString((string)p.GameCode, "彩种不能为空");
                var matchId = PreconditionAssert.IsNotEmptyString((string)p.MatchId, "比赛编号不能为空");
                var issuse = p.Issuse.ToString();

                var result = _service.DoMatchCancel(gameCode, matchId, issuse);
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = result.Message });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public IActionResult EnableMatchBet(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                if (!CheckRights("B102"))
                    throw new LogicException("对不起，您的权限不足！");
                var gameCode = string.IsNullOrEmpty((string)p.GameCode) ? "JCZQ" : (string)p.GameCode;
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
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }

        }
        public IActionResult UpdateMatchPrivile(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("JYBS100"))
                    throw new LogicException("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);
                var gameCode = PreconditionAssert.IsNotEmptyString((string)p.GameCode, "请选择彩种");
                var matchId = PreconditionAssert.IsNotEmptyString((string)p.matchId, "比赛编号不能为空");
                var privilegesType = (string)p.privilegesType;
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
        public IActionResult UpdateLotteryGame(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var gameCode = (string)p.gameCode;
                var status = int.Parse((string)p.status);

                _service.UpdateLotteryGame(CurrentUser.UserToken, gameCode, status);
                return Json(new LotteryServiceResponse { Code = AdminResponseCode.成功, Message = "修改成功！" });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }

        }
        public IActionResult QueryIndexMatchList(LotteryServiceRequest entity)
        {
            try
            {
                if (!CheckRights("BS200"))
                    throw new Exception("对不起，您的权限不足！");
                var p = JsonHelper.Decode(entity.Param);
                string MatchId = (string)p.MatchId ?? string.Empty;
                int PageIndex = (string)p.PageIndex == null ? base.PageIndex : Convert.ToInt32((string)p.PageIndex);
                int PageSize = (string)p.PageSize == null ? base.PageSize : Convert.ToInt32((string)p.PageSize);
                var chk = Convert.ToBoolean((string)p.HasImg);
                var hasImg = "-1";
                if (chk)
                    hasImg = string.Empty;
                var IndexMatchList = _service.QueryIndexMatchCollection(MatchId, hasImg, PageIndex, PageSize);
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Value = IndexMatchList });
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse() { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }
        public IActionResult IndexMatchInfo(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string Id = (string)p.Id;
                if (Id == null)
                    return null;
                var IndexMatchInfo = _service.QueryIndexMatchInfo(Convert.ToInt32(Id));
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
                var id = PreconditionAssert.IsNotEmptyString((string)p.Id, "编号不能为空");
                var loadFile = loadfile;
                var matchId = (string)p.MatchId;
                var imgPath = string.Empty;
                if (loadFile.Length <= 0)
                    imgPath = (string)p.ImgPath;
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
