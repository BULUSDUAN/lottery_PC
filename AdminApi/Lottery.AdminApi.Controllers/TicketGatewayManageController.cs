using EntityModel.CoreModel;
using EntityModel.Enum;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Net;
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
    [Area("api")]
    [ReusltFilter]
    [CheckLogin]
    public class TicketGatewayManageController:BaseController
    {
        private readonly static AdminService _service = new AdminService();
        #region 中民出票查询
    
        /// <summary>
        /// 接口开启奖期
        /// </summary>
        public JsonResult InterOpenPrizePeriod(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var gameCode = PreconditionAssert.IsNotEmptyString((string)p.gameCode, "彩种不能为空！");
                var startTime = PreconditionAssert.IsNotEmptyString((string)p.startTime, "开始时间不能为空！");
                var endTime = PreconditionAssert.IsNotEmptyString((string)p.endTime, "停止时间不能为空！");
                var title = (string)p.title;
                if (title == "高频彩")
                {
                    var openPrizePeriodResult = _service.OpenIssuseBatch_Fast(gameCode, Convert.ToDateTime(startTime), Convert.ToDateTime(endTime));
                    return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = openPrizePeriodResult.Message });
                   
                }
                else if (title == "低频彩")
                {
                    var openPrizePeriodResult = _service.OpenIssuseBatch_Slow(gameCode, Convert.ToInt32(startTime), Convert.ToInt32(endTime));
                    return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = openPrizePeriodResult.Message });
                  
                }
                else
                {
                    var openPrizePeriodResult = _service.OpenIssuseBatch_Daily(gameCode, Convert.ToInt32(startTime), Convert.ToInt32(endTime));
                    return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = openPrizePeriodResult.Message });
                 
                }
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }

        #endregion

        #region 传统足球
        public JsonResult DoPrize_CTZQ(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var gameType = PreconditionAssert.IsNotEmptyString((string)p.gameType, "玩法不能为空");
                var issuseNumber = PreconditionAssert.IsNotEmptyString((string)p.issuseNumber, "期号不能为空");
                var winnumber = PreconditionAssert.IsNotEmptyString((string)p.winnumber, "中奖号码不能为空");
                //var result = _service.QueryUnPrizeTicketAndDoPrizeByGameCode("CTZQ", gameType, issuseNumber, winnumber, -1);
                //return Json(new { IsSuccess = true, Msg = result });
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = "目前暂停业务" });
           
            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }

        public JsonResult GetBonusPoolList_CTZQ(LotteryServiceRequest entity)
        {
            return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = "目前暂停业务" });
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                var collection = new CTZQ_BonusPoolCollection();
                var gameCode = "CTZQ";
                var gameType = PreconditionAssert.IsNotEmptyString((string)p.gameType, "玩法不能为空");
                var issuseNumber = PreconditionAssert.IsNotEmptyString((string)p.issuseNumber, "期号不能为空");
                var result = GetBonusPoolListJson_CTZQ(gameCode, gameType, issuseNumber);
                return Json(new LotteryServiceResponse() { Code = AdminResponseCode.成功, Message = result });

            }
            catch (Exception ex)
            {
                return JsonEx(new LotteryServiceResponse { Code = AdminResponseCode.失败, Message = ex.Message });
            }
        }

        private string GetBonusPoolListJson_CTZQ(string gameCode, string gameType, string issuseNumber)
        {
            var lotteryRes = ConfigHelper.AllConfigInfo["MatchRoot"].ToString();
            var fileName = string.Format(@"{3}/{0}/{2}/{0}_{1}_BonusLevel.json", gameCode, gameType, issuseNumber, lotteryRes);
            var json = ReadFileString(fileName);
            return json;
        }

        private string ReadFileString(string fileName)
        {
            string strResult = PostManager.Get(fileName, Encoding.UTF8);

            if (!string.IsNullOrEmpty(strResult))
            {
                if (strResult.ToLower().StartsWith("var"))
                {
                    string[] strArray = strResult.Split('=');
                    if (strArray != null && strArray.Length == 2)
                    {
                        if (strArray[1].ToString().Trim().EndsWith(";"))
                        {
                            return strArray[1].ToString().Trim().TrimEnd(';');
                        }
                        return strArray[1].ToString().Trim();
                    }
                }
            }
            return strResult;
        }
        #endregion
    }
}