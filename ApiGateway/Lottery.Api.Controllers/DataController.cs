using EntityModel.CoreModel;
using EntityModel.Enum;
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
        /// 查询彩种奖期信息_101
        /// </summary>
        public async Task<IActionResult> QueryGameIssuseInfo([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                
                var p = WebHelper.Decode(entity.Param);
                Dictionary<string, object> param = new Dictionary<string, object>();
                //var param = System.Web.Helpers.Json.Decode(entity.Param);
                param.Add("gameCode", p.GameCode);
                var gameIssuseInfo = await _serviceProxyProvider.Invoke<Issuse_QueryInfo>(param, "api/Data/QueryCurrentIssuseInfo");
                //var gameIssuseInfo = WCFClients.GameIssuseClient.QueryCurrentIssuseInfo(param.GameCode);
                param.Clear();
                param.Add("key", "Site.GameDelay." + p.GameCode.ToUpper());
                var config= await _serviceProxyProvider.Invoke<CoreConfigInfo>(param, "api/Data/QueryCoreConfigByKey");
                //var config = WCFClients.GameClient.QueryCoreConfigByKey("Site.GameDelay." + param.GameCode.ToUpper()).ConfigValue;
                var DelayTime = config.ConfigValue;
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
                    Message = "业务参数错误",
                    MsgId = entity.MsgId,
                    Value = "",
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "服务器内部错误，请联系接口提供商",
                    MsgId = entity.MsgId,
                    Value = "",
                });
            }
        }

        /// <summary>
        /// 查询所有彩种的当前奖期信息_1011
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryAllGameCurrentIssuseInfo([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            return null;
        }

        /// <summary>
        /// 获取服务器当前时间_102
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetServerCurrTime([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var currTime = ConvertHelper.ConvertDateTimeInt(DateTime.Now);
                if (currTime > 0)
                {
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "查询服务器当前时间成功",
                        MsgId = entity.MsgId,
                        Value = currTime,
                    });
                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询服务器当前时间失败",
                    MsgId = entity.MsgId,
                    Value = 0,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误",
                    MsgId = entity.MsgId,
                    Value = "",
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "服务器内部错误，请联系接口提供商",
                    MsgId = entity.MsgId,
                    Value = "",
                });
            }
        }

        /// <summary>
        /// 查询广告信息_105
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryMsgBannerList([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                BannerType bannerType = BannerType.APP;
                //if (entity.SourceCode == SchemeSource.Iphone)
                //    bannerType = BannerType.IOS;
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("bannerType", bannerType);
                var bannerList = await _serviceProxyProvider.Invoke<SiteMessageBannerInfo_Collection>(param, "api/Data/QuerySitemessageBanngerList_Web"); 
                if (bannerList != null && bannerList.ListInfo.Count > 0)
                {
                    var list = new List<object>();
                    foreach (var item in bannerList.ListInfo)
                    {
                        list.Add(new
                        {
                            BannerId = item.BannerId,
                            BannerTitle = item.BannerTitle,
                            BannerType = item.BannerType,
                            CreateTime = ConvertHelper.ConvertDateTimeInt(item.CreateTime),
                            ImageUrl = item.ImageUrl,
                            IsEnable = item.IsEnable,
                            JumpUrl = item.JumpUrl,
                        });
                    }
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "查询广告列表成功",
                        MsgId = entity.MsgId,
                        Value = list,
                    });
                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询广告列表失败",
                    MsgId = entity.MsgId,
                    Value = "",
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误",
                    MsgId = entity.MsgId,
                    Value = "",
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "服务器内部错误，请联系接口提供商",
                    MsgId = entity.MsgId,
                    Value = "",
                });
            }
        }

        /// <summary>
        /// 查询银行列表_116
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryBankList([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            return null;
        }
    }
}
