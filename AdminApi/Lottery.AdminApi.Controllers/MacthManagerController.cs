using EntityModel.Enum;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Utilities;
using KaSon.FrameWork.ORM.Helper;
using Lottery.AdminApi.Controllers.CommonFilterActtribute;
using Lottery.AdminApi.Model.HelpModel;
using Microsoft.AspNetCore.Mvc;
using System;

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
        public JsonResult UpdateBjdc(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var IssueNum = PreconditionAssert.IsNotEmptyString(p.IssueNum, "北单期号不能为空");
                var result = _service.ManualUpdate_BJDC_MatchList(IssueNum, CurrentUser.UserToken);
                return Json(new LotteryServiceResponse(){ Code = AdminResponseCode.成功, Message = result.Message });
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
        public JsonResult UpdateBJDCResult(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var IssueNum = PreconditionAssert.IsNotEmptyString(p.IssueNum, "北单期号不能为空");
                var result = _service.ManualUpdate_BJDC_MatchResultList(IssueNum, CurrentUser.UserToken);
                return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }
        /// <summary>
        /// 竞彩足球数据更新
        /// </summary>
        /// <returns></returns>
        public JsonResult UpdateJczq()
        {
            try
            {
                var result = _service.ManualUpdate_JCZQ_MatchList(CurrentUser.UserToken);
                return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }
        /// <summary>
        /// 竞彩足球SP更新
        /// </summary>
        /// <returns></returns>
        public JsonResult UpdateJczq_SP()
        {
            try
            {
                var result = _service.UpdateOddsList_JCZQ_Manual();
                return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }
    }
}
