using Kason.Sg.Core.ProxyGenerator;
using KaSon.FrameWork.Helper;
using Lottery.ApiGateway.Model.Enum;
using Lottery.ApiGateway.Model.HelpModel;
using Lottery.Base.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static KaSon.FrameWork.Helper.JsonHelper;

namespace Lottery.Api.Controllers
{
    [Area("Data")]
    public class DataController: BaseController
    {
        /// <summary>
        /// 查询中奖列表
        /// </summary>
        public async Task<IActionResult> QueryGameIssuseInfo([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                
                var p = WebHelper.Decode(entity.Param);
                Dictionary<string, object> param = new Dictionary<string, object>();
                //var param = System.Web.Helpers.Json.Decode(entity.Param);
                param.Add("GameCode", p.GameCode);
                var gameIssuseInfo = await _serviceProxyProvider.Invoke<Issuse_QueryInfo>(param, "api/Data/QueryCurrentIssuseInfo");
                //var gameIssuseInfo = WCFClients.GameIssuseClient.QueryCurrentIssuseInfo(param.GameCode);
                param.Clear();
                param.Add("key", "Site.GameDelay." + p.GameCode.ToUpper());
                var config= await _serviceProxyProvider.Invoke<CoreConfigInfo>(param, "api/Data/QueryCoreConfigByKey").ConfigValue;
                //var config = WCFClients.GameClient.QueryCoreConfigByKey("Site.GameDelay." + param.GameCode.ToUpper()).ConfigValue;
                var DelayTime = config;
                if (gameIssuseInfo != null && DelayTime != null)
                {
                    var list = new List<object>();
                    list.Add(new
                    {
                        CurrIssuseNumber = gameIssuseInfo.IssuseNumber,
                        LocalStopTime = ConvertHelper.ConvertDateTimeInt(gameIssuseInfo.LocalStopTime),
                        OfficialStopTime = ConvertHelper.ConvertDateTimeInt(gameIssuseInfo.OfficialStopTime),
                        DelayTime = DelayTime,
                        ServiceTime = ConvertHelper.ConvertDateTimeInt(DateTime.Now),
                    });
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "查询彩种奖期信息成功",
                        MsgId = entity.MsgId,
                        Value = list,
                    });
                }
                else
                    throw new ArgumentException("查询彩种奖期信息失败");
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.Message,
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.Message,
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                });
            }
        }
    }
}
