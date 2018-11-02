using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Enum;
using Kason.Sg.Core.ProxyGenerator;
using Lottery.ApiGateway.Model.HelpModel;
using Lottery.Base.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using EntityModel.Communication;
using System.IO;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Utilities;
using EntityModel.LotteryJsonInfo;
using Lottery.Api.Controllers.CommonFilterActtribute;
using KaSon.FrameWork.Common.ExceptionEx;
using EntityModel.ExceptionExtend;
using KaSon.FrameWork.Common.Redis;
using EntityModel.Redis;
using Microsoft.Extensions.Caching.Distributed;
using CSRedis;
using KaSon.FrameWork.Common.Net;
using System.Globalization;
using KaSon.FrameWork.Common.Sport;
using KaSon.FrameWork.Common.Expansion;

namespace Lottery.Api.Controllers
{
    [Area("api")]
    [ReusltFilter]
    public class DataController : BaseController
    {
        private IDistributedCache _Cache;
        public IActionResult Index()
        {
            return JsonEx(new { name = "12313" });


            //  _Cache
        }
        #region 查询彩种奖期信息(101)
        /// <summary>
        /// 查询彩种奖期信息_101
        /// </summary>
        public async Task<IActionResult> QueryGameIssuseInfo([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                Dictionary<string, object> param = new Dictionary<string, object>();
                string gameCode = p.GameCode;

                param.Add("key", "Site.GameDelay." + gameCode.ToUpper());
                var configtask = _serviceProxyProvider.Invoke<C_Core_Config>(param, "api/Data/QueryCoreConfigByKey");
                param.Clear();

                param.Add("gameCode", gameCode);
                var gameIssuseInfo = await _serviceProxyProvider.Invoke<Issuse_QueryInfo>(param, "api/Data/QueryCurrentIssuseInfo");

                string DelayTime = string.Empty;
                var config = await configtask;
                if (config != null)
                {
                    DelayTime = config.ConfigValue;
                }
                if (gameIssuseInfo != null && !string.IsNullOrEmpty(DelayTime))
                {
                    var list = new List<object>();
                    DateTime? OpeningTime = null;
                    if (gameCode.ToUpper() == "FC3D" || gameCode.ToUpper() == "PL3")
                    {
                        OpeningTime = gameIssuseInfo.LocalStopTime.Date.AddHours(21).AddMinutes(30);
                    }
                    list.Add(new
                    {
                        CurrIssuseNumber = gameIssuseInfo.IssuseNumber,
                        LocalStopTime = gameIssuseInfo.LocalStopTime,
                        OfficialStopTime = gameIssuseInfo.OfficialStopTime,
                        DelayTime = DelayTime,
                        ServiceTime = DateTime.Now,
                        OpeningTime = OpeningTime
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
                //Log4Log.LogEX(KLogLevel.APIError, "API或服务错误***", ex);
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = "",
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "获取失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = "获取失败",
                });
            }
        }
        #endregion

        #region 查询所有彩种的当前奖期信息(1011)
        /// <summary>
        /// 查询所有彩种的当前奖期信息_1011
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryAllGameCurrentIssuseInfo([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var gameCodeArray = new string[] { "SSQ", "DLT", "FC3D", "PL3", "CQSSC", "JX11X5" };
                var result = new List<object>();
                Dictionary<string, object> param = new Dictionary<string, object>();
                foreach (var gameCode in gameCodeArray)
                {
                    param.Clear();
                    param.Add("gameCode", gameCode);
                    var gameInfo = await _serviceProxyProvider.Invoke<LotteryIssuse_QueryInfo>(param, "api/Data/QueryNextIssuseListByLocalStopTime");
                    if (gameInfo == null) continue;
                    result.Add(new
                    {
                        GameCode = gameInfo.GameCode,
                        CurrIssuseNumber = gameInfo.IssuseNumber,
                        LocalStopTime = gameInfo.LocalStopTime,
                        OfficialStopTime = gameInfo.OfficialStopTime,
                        DelayTime = gameInfo.GameDelaySecond,
                        ServiceTime = DateTime.Now,
                    });
                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询彩种奖期信息成功",
                    MsgId = entity.MsgId,
                    Value = "",
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = "",
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        #endregion

        #region 获取服务器当前时间(102)
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
                var currTime = DateTime.Now;
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询服务器当前时间成功",
                    MsgId = entity.MsgId,
                    Value = currTime,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = "",
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        #endregion

        #region 查询广告信息(105)
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
                SchemeSource schemeSource = entity.SourceCode;
                BannerType bannerType = new BannerType();
                switch (schemeSource)
                {
                    case SchemeSource.NewAndroid:
                        bannerType = BannerType.APP;
                        break;
                    case SchemeSource.NewIphone:
                        bannerType = BannerType.APP;
                        break;
                    case SchemeSource.Wap:
                        bannerType = BannerType.Touch_Index;
                        break;
                }
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("bannerType", (int)bannerType);
                param.Add("returnRecord", 10);

                var bannerList = await _serviceProxyProvider.Invoke<SiteMessageBannerInfo_Collection>(param, "api/Data/QuerySitemessageBanngerList_Web");
                if (bannerList.ListInfo == null) bannerList.ListInfo = new List<SiteMessageBannerInfo>();
                var list = new List<object>();
                foreach (var item in bannerList.ListInfo)
                {
                    list.Add(new
                    {
                        BannerId = item.BannerId,
                        BannerTitle = item.BannerTitle,
                        BannerType = item.BannerType,
                        CreateTime = item.CreateTime,
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
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = "",
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "获取失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = "获取失败",
                });
            }
        }
        #endregion

        #region 银行信息(116)
        /// <summary>
        /// 查询银行列表_116
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryBankList([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                List<C_Bank_Info> BankList = await _serviceProxyProvider.Invoke<List<C_Bank_Info>>(new Dictionary<string, object>(), "api/Data/GetBankList");
                var returnlist = new List<object>();
                if (BankList != null && BankList.Count > 0)
                {
                    foreach (var item in BankList)
                    {
                        returnlist.Add(new { id = item.Id, value = item.BankCode, name = item.BankName });
                    }
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "查询成功",
                        MsgId = entity.MsgId,
                        Value = returnlist,
                    });
                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询成功",
                    MsgId = entity.MsgId,
                    Value = returnlist,
                });

            }
            catch (ArgumentException ex)
            {
                //Log("116", ex);
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = "",
                });
            }
            catch (Exception ex)
            {
                //Log("116", ex);
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        #endregion

        #region 查询当前遗漏(119)
        /// <summary>
        /// 查看重庆时时彩当前是否遗漏_119
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryCQSSCCurrNumberOmission([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                if (entity.Param == null) throw new LogicException("请求参数错误！");
                var p = JsonHelper.Decode(entity.Param);
                string gameCode = p.GameCode;
                string gameType = p.GameType;
                string issuseNumber = p.IssuseNumber;
                if (string.IsNullOrEmpty(gameCode) || string.IsNullOrEmpty(gameType) || string.IsNullOrEmpty(issuseNumber))
                    throw new LogicException("请求参数错误！");
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("key", gameCode + "_" + gameType + "_" + issuseNumber);
                var result = new LotteryServiceResponse() { Code = ResponseCode.失败, Message = "请求参数错误！", Value = "" };
                switch (gameType.ToUpper())
                {
                    case "1XDX":
                        result = await QueryCQSSCCurrNumberOmission_1XDX(_serviceProxyProvider, param);
                        break;
                    case "2XDX":
                        result = await QueryCQSSCCurrNumberOmission_2XZX(_serviceProxyProvider, param);
                        break;
                    case "3XDX":
                        result = await QueryCQSSCCurrNumberOmission_3XZX(_serviceProxyProvider, param);
                        break;
                    case "2XZXFS":
                        result = await QueryCQSSCCurrNumberOmission_2XZuX(_serviceProxyProvider, param);
                        break;
                    case "ZX6":
                    case "ZX3DS":
                    case "ZX3FS":
                        result = await QueryCQSSCCurrNumberOmission_ZX3_ZX6(_serviceProxyProvider, param);
                        break;
                    case "DXDS":
                        result = await QueryCQSSCCurrNumberOmission_DXDS(_serviceProxyProvider, param);
                        break;
                    case "5XDX":
                    case "5XTX":
                        result = await QueryCQSSCCurrNumberOmission_5XJBZS(_serviceProxyProvider, param);
                        break;
                }
                result.MsgId = p.MsgId;
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }


        #region 一星单选

        public async Task<LotteryServiceResponse> QueryCQSSCCurrNumberOmission_1XDX([FromServices]IServiceProxyProvider _serviceProxyProvider, IDictionary<string, object> param)
        {

            param.Add("index", 1);
            var result = await _serviceProxyProvider.Invoke<CQSSC_1X_ZS>(param, "api/Data/QueryCQSSCCurrNumberOmission_1XDX");
            if (result == null)
                throw new LogicException("未查询到遗漏数据！");
            var list = new List<object>();
            list.Add(new
            {
                IssuseNumber = result.IssuseNumber,
                WinNumber = result.WinNumber,
                Red_G0 = result.Red_G0,
                Red_G1 = result.Red_G1,
                Red_G2 = result.Red_G2,
                Red_G3 = result.Red_G3,
                Red_G4 = result.Red_G4,
                Red_G5 = result.Red_G5,
                Red_G6 = result.Red_G6,
                Red_G7 = result.Red_G7,
                Red_G8 = result.Red_G8,
                Red_G9 = result.Red_G9,
                D_Red1 = result.D_Red1,
                X_Red1 = result.X_Red1,
                J_Red1 = result.J_Red1,
                O_Red1 = result.O_Red1,
                Z_Red1 = result.Z_Red1,
                H_Red1 = result.H_Red1,

                O_Red1_0 = result.O_Red1_0,
                O_Red1_1 = result.O_Red1_1,
                O_Red1_2 = result.O_Red1_2,
            });
            return new LotteryServiceResponse
            {
                Code = ResponseCode.成功,
                Message = "查询当前遗漏成功！",
                Value = list,
            };
        }

        #endregion

        #region 2星直选

        public async Task<LotteryServiceResponse> QueryCQSSCCurrNumberOmission_2XZX([FromServices]IServiceProxyProvider _serviceProxyProvider, IDictionary<string, object> param)
        {
            param.Add("index", 1);
            var result = await _serviceProxyProvider.Invoke<CQSSC_2X_ZXZS>(param, "api/Data/QueryCQSSCCurrNumberOmission_2XZX");
            if (result == null)
                throw new LogicException("未查询到遗漏数据！");
            var list = new List<object>();
            list.Add(new
            {
                IssuseNumber = result.IssuseNumber,
                WinNumber = result.WinNumber,
                Red_S0 = result.Red_S0,
                Red_S1 = result.Red_S1,
                Red_S2 = result.Red_S2,
                Red_S3 = result.Red_S3,
                Red_S4 = result.Red_S4,
                Red_S5 = result.Red_S5,
                Red_S6 = result.Red_S6,
                Red_S7 = result.Red_S7,
                Red_S8 = result.Red_S8,
                Red_S9 = result.Red_S9,
                Red_G0 = result.Red_G0,
                Red_G1 = result.Red_G1,
                Red_G2 = result.Red_G2,
                Red_G3 = result.Red_G3,
                Red_G4 = result.Red_G4,
                Red_G5 = result.Red_G5,
                Red_G6 = result.Red_G6,
                Red_G7 = result.Red_G7,
                Red_G8 = result.Red_G8,
                Red_G9 = result.Red_G9,
            });
            return new LotteryServiceResponse
            {
                Code = ResponseCode.成功,
                Message = "查询当前遗漏成功！",
                Value = list,
            };
        }

        #endregion

        #region 3星直选

        public async Task<LotteryServiceResponse> QueryCQSSCCurrNumberOmission_3XZX([FromServices]IServiceProxyProvider _serviceProxyProvider, IDictionary<string, object> param)
        {
            param.Add("index", 1);
            var result = await _serviceProxyProvider.Invoke<CQSSC_3X_ZXZS>(param, "api/Data/QueryCQSSCCurrNumberOmission_3XZX");
            if (result == null)
                throw new LogicException("未查询到遗漏数据！");
            var list = new List<object>();
            list.Add(new
            {
                IssuseNumber = result.IssuseNumber,
                WinNumber = result.WinNumber,
                Red_B0 = result.Red_G0,
                Red_B1 = result.Red_G1,
                Red_B2 = result.Red_G2,
                Red_B3 = result.Red_G3,
                Red_B4 = result.Red_G4,
                Red_B5 = result.Red_G5,
                Red_B6 = result.Red_G6,
                Red_B7 = result.Red_G7,
                Red_B8 = result.Red_G8,
                Red_B9 = result.Red_G9,
                Red_S0 = result.Red_S0,
                Red_S1 = result.Red_S1,
                Red_S2 = result.Red_S2,
                Red_S3 = result.Red_S3,
                Red_S4 = result.Red_S4,
                Red_S5 = result.Red_S5,
                Red_S6 = result.Red_S6,
                Red_S7 = result.Red_S7,
                Red_S8 = result.Red_S8,
                Red_S9 = result.Red_S9,
                Red_G0 = result.Red_G0,
                Red_G1 = result.Red_G1,
                Red_G2 = result.Red_G2,
                Red_G3 = result.Red_G3,
                Red_G4 = result.Red_G4,
                Red_G5 = result.Red_G5,
                Red_G6 = result.Red_G6,
                Red_G7 = result.Red_G7,
                Red_G8 = result.Red_G8,
                Red_G9 = result.Red_G9,
            });
            return new LotteryServiceResponse
            {
                Code = ResponseCode.成功,
                Message = "查询当前遗漏成功！",
                Value = list,
            };
        }

        #endregion

        #region 二星组选

        public async Task<LotteryServiceResponse> QueryCQSSCCurrNumberOmission_2XZuX([FromServices]IServiceProxyProvider _serviceProxyProvider, IDictionary<string, object> param)
        {
            param.Add("index", 1);
            var result = await _serviceProxyProvider.Invoke<CQSSC_2X_ZuXZS>(param, "api/Data/QueryCQSSCCurrNumberOmission_2XZuX");
            //var result = WCFClients.ChartClient.QueryCQSSCCurrNumberOmission_2XZuX(gameCode + "_" + gameType + "_" + issuseNumber, 1);
            if (result == null)
                throw new LogicException("未查询到遗漏数据！");
            var list = new List<object>();
            list.Add(new
            {
                IssuseNumber = result.IssuseNumber,
                WinNumber = result.WinNumber,
                Red_0 = result.Red_0,
                Red_1 = result.Red_1,
                Red_2 = result.Red_2,
                Red_3 = result.Red_3,
                Red_4 = result.Red_4,
                Red_5 = result.Red_5,
                Red_6 = result.Red_6,
                Red_7 = result.Red_7,
                Red_8 = result.Red_8,
                Red_9 = result.Red_9,
            });
            return new LotteryServiceResponse
            {
                Code = ResponseCode.成功,
                Message = "查询当前遗漏成功！",
                Value = list,
            };
        }

        #endregion

        #region 组三 组六

        public async Task<LotteryServiceResponse> QueryCQSSCCurrNumberOmission_ZX3_ZX6([FromServices]IServiceProxyProvider _serviceProxyProvider, IDictionary<string, object> param)
        {
            param.Add("index", 1);
            var result = await _serviceProxyProvider.Invoke<CQSSC_3X_ZuXZS>(param, "api/Data/QueryCQSSCCurrNumberOmission_ZX3_ZX6");
            //var result = WCFClients.ChartClient.QueryCQSSCCurrNumberOmission_ZX3_ZX6(gameCode + "_" + gameType + "_" + issuseNumber, 1);
            if (result == null)
                throw new LogicException("未查询到遗漏数据！");
            var list = new List<object>();
            list.Add(new
            {
                IssuseNumber = result.IssuseNumber,
                WinNumber = result.WinNumber,
                Red_0 = result.Red_0,
                Red_1 = result.Red_1,
                Red_2 = result.Red_2,
                Red_3 = result.Red_3,
                Red_4 = result.Red_4,
                Red_5 = result.Red_5,
                Red_6 = result.Red_6,
                Red_7 = result.Red_7,
                Red_8 = result.Red_8,
                Red_9 = result.Red_9,
            });
            return new LotteryServiceResponse
            {
                Code = ResponseCode.成功,
                Message = "查询当前遗漏成功！",
                Value = list,
            };
        }

        #endregion

        #region 大小单双

        public async Task<LotteryServiceResponse> QueryCQSSCCurrNumberOmission_DXDS([FromServices]IServiceProxyProvider _serviceProxyProvider, IDictionary<string, object> param)
        {
            param.Add("index", 1);
            var result = await _serviceProxyProvider.Invoke<CQSSC_DXDS>(param, "api/Data/QueryCQSSCCurrNumberOmission_DXDS");
            //var result = WCFClients.ChartClient.QueryCQSSCCurrNumberOmission_DXDS(gameCode + "_" + gameType + "_" + issuseNumber, 1);
            if (result == null)
                throw new LogicException("未查询到遗漏数据！");
            var list = new List<object>();
            list.Add(new
            {
                IssuseNumber = result.IssuseNumber,
                WinNumber = result.WinNumber,
                D_Red_S = result.D_Red_S,
                Dan_Red_S = result.Dan_Red_S,
                X_Red_S = result.X_Red_S,
                S_Red_S = result.S_Red_S,
                D_Red_G = result.D_Red_G,
                X_Red_G = result.X_Red_G,
                Dan_Red_G = result.Dan_Red_G,
                S_Red_G = result.S_Red_G,

                DD = result.DD,
                DX = result.DX,
                DDan = result.DDan,
                DS = result.DS,
                XD = result.XD,
                XX = result.XX,
                XDan = result.XDan,
                XS = result.XS,
                DanD = result.DanD,
                DanX = result.DanX,
                DanDan = result.DanDan,
                DanS = result.DanS,
                SD = result.SD,
                SX = result.SX,
                SDan = result.SDan,
                SS = result.SS,
            });
            return new LotteryServiceResponse
            {
                Code = ResponseCode.成功,
                Message = "查询当前遗漏成功！",
                Value = list,
            };
        }

        #endregion

        #region 五星基本走势

        public async Task<LotteryServiceResponse> QueryCQSSCCurrNumberOmission_5XJBZS([FromServices]IServiceProxyProvider _serviceProxyProvider, IDictionary<string, object> param)
        {
            param.Add("index", 1);
            var result = await _serviceProxyProvider.Invoke<CQSSC_5X_JBZS>(param, "api/Data/QueryCQSSCCurrNumberOmission_5XJBZS");
            //var result = WCFClients.ChartClient.QueryCQSSCCurrNumberOmission_5XJBZS(gameCode + "_" + gameType + "_" + issuseNumber, 1);
            if (result == null)
                throw new LogicException("未查询到遗漏数据！");
            var list = new List<object>();
            list.Add(new
            {
                IssuseNumber = result.IssuseNumber,
                WinNumber = result.WinNumber,
                Red_W0 = result.Red_W0,
                Red_W1 = result.Red_W1,
                Red_W2 = result.Red_W2,
                Red_W3 = result.Red_W3,
                Red_W4 = result.Red_W4,
                Red_W5 = result.Red_W5,
                Red_W6 = result.Red_W6,
                Red_W7 = result.Red_W7,
                Red_W8 = result.Red_W8,
                Red_W9 = result.Red_W9,
                Red_Q0 = result.Red_Q0,
                Red_Q1 = result.Red_Q1,
                Red_Q2 = result.Red_Q2,
                Red_Q3 = result.Red_Q3,
                Red_Q4 = result.Red_Q4,
                Red_Q5 = result.Red_Q5,
                Red_Q6 = result.Red_Q6,
                Red_Q7 = result.Red_Q7,
                Red_Q8 = result.Red_Q8,
                Red_Q9 = result.Red_Q9,
                Red_B0 = result.Red_B0,
                Red_B1 = result.Red_B1,
                Red_B2 = result.Red_B2,
                Red_B3 = result.Red_B3,
                Red_B4 = result.Red_B4,
                Red_B5 = result.Red_B5,
                Red_B6 = result.Red_B6,
                Red_B7 = result.Red_B7,
                Red_B8 = result.Red_B8,
                Red_B9 = result.Red_B9,
                Red_S0 = result.Red_S0,
                Red_S1 = result.Red_S1,
                Red_S2 = result.Red_S2,
                Red_S3 = result.Red_S3,
                Red_S4 = result.Red_S4,
                Red_S5 = result.Red_S5,
                Red_S6 = result.Red_S6,
                Red_S7 = result.Red_S7,
                Red_S8 = result.Red_S8,
                Red_S9 = result.Red_S9,
                Red_G0 = result.Red_G0,
                Red_G1 = result.Red_G1,
                Red_G2 = result.Red_G2,
                Red_G3 = result.Red_G3,
                Red_G4 = result.Red_G4,
                Red_G5 = result.Red_G5,
                Red_G6 = result.Red_G6,
                Red_G7 = result.Red_G7,
                Red_G8 = result.Red_G8,
                Red_G9 = result.Red_G9,
            });
            return new LotteryServiceResponse
            {
                Code = ResponseCode.成功,
                Message = "查询当前遗漏成功！",
                Value = list,
            };
        }

        #endregion
        #endregion

        #region 查询文章列表(新闻详情_123、新闻列表_208)
        /// <summary>
        /// 新闻详情_123
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryArticleDetail([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                //var param = System.Web.Helpers.Json.Decode(entity.Param);
                string id = p.ArticleId;
                if (string.IsNullOrEmpty(id))
                    throw new LogicException("未找到对应文章");
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("articleId", id);
                var resultInfo = await _serviceProxyProvider.Invoke<ArticleInfo_Query>(param, "api/Data/QueryArticleById_Web");
                //var resultInfo = WCFClients.ExternalClient.QueryArticleById_Web(id);
                if (resultInfo != null && !string.IsNullOrEmpty(resultInfo.Id))
                {
                    var list = new List<object>();
                    var content = resultInfo.Description;
                    if (!string.IsNullOrEmpty(content))
                    {
                        var index = content.IndexOf("<img");
                        if (index >= 0)
                            content = content.Replace("<img", "<img  width=\"100%\" ");
                    }
                    list.Add(new
                    {
                        Id = resultInfo.Id,
                        PreId = resultInfo.PreId,
                        NextId = resultInfo.NextId,
                        Title = resultInfo.Title,
                        PreTitle = resultInfo.PreTitle,
                        NextTitle = resultInfo.NextTitle,
                        Content = content,
                        DescContent = resultInfo.DescContent,
                        CreateTime = resultInfo.CreateTime,
                        author = "玩彩编辑",
                        readcount = resultInfo.ReadCount
                    });
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "查询文章内容",
                        MsgId = entity.MsgId,
                        Value = list,
                    });
                }
                throw new LogicException("未查询到文章内容");
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = "",
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = string.Empty,
                });
            }
        }

        /// <summary>
        /// 新闻列表_208
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> New_QueryArticleList([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                //热点彩讯  FocusCMS
                //赛事点评 Match_Comment
                //彩票资讯 Lottery_GameCode
                string category = p.Category;
                int pageIndex = p.PageIndex;
                int pageSize = p.PageSize;
                var list = new List<object>();
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("pageIndex", pageIndex);
                param.Add("pageSize", pageSize);
                var aList = new ArticleInfo_QueryCollection();
                switch (category)
                {
                    case "hot"://今日热点
                        param.Add("category", "Lottery_Hot");
                        param.Add("gameCode", "");
                        //aList = await _serviceProxyProvider.Invoke<ArticleInfo_QueryCollection>(param, "api/Data/QueryArticleList_YouHua");
                        break;
                    case "gpc"://高频彩
                        param.Add("category", "Lottery_GameCode");
                        param.Add("gameCode", "JX11X5|CQSSC|SD11X5|GD11X5|GDKLSF|JSKS|SDKLPK3");
                        //aList = await _serviceProxyProvider.Invoke<ArticleInfo_QueryCollection>(param, "api/Data/QueryArticleList_YouHua");
                        break;
                    case "szc"://数字彩
                        param.Add("category", "Lottery_GameCode");
                        param.Add("gameCode", "SSQ|DLT|PL3|FC3D");
                        //aList = await _serviceProxyProvider.Invoke<ArticleInfo_QueryCollection>(param, "api/Data/QueryArticleList_YouHua");
                        break;
                    case "jjc"://竞技彩
                        param.Add("category", "Lottery_GameCode");
                        param.Add("gameCode", "JCZQ|JCLQ|BJDC");
                        //aList = await _serviceProxyProvider.Invoke<ArticleInfo_QueryCollection>(param, "api/Data/QueryArticleList_YouHua");
                        break;
                    case "FocusCMS"://焦点新闻
                        param.Add("category", "FocusCMS");
                        param.Add("gameCode", "");
                        //aList = await _serviceProxyProvider.Invoke<ArticleInfo_QueryCollection>(param, "api/Data/QueryArticleList_YouHua");
                        break;
                    case "BonusCMS"://焦点新闻
                        param.Add("category", "BonusCMS");
                        param.Add("gameCode", "");
                        //aList = await _serviceProxyProvider.Invoke<ArticleInfo_QueryCollection>(param, "api/Data/QueryArticleList_YouHua");
                        break;
                }


                if (param.Count > 0)
                {
                    //var Key = RedisKeys.ArticleListKey;
                    //string cacheKey = string.Format("{0}_{1}_{2}_{3}_{4}", Key, param["category"].ToString(), param["gameCode"].ToString(), pageIndex, pageSize);
                    //var obj = RedisHelperEx.DB_CoreCacheData.GetObj<ArticleInfo_QueryCollection>(cacheKey);
                    //if (obj == null)
                    //{
                    aList = await _serviceProxyProvider.Invoke<ArticleInfo_QueryCollection>(param, "api/Data/QueryArticleList_YouHua");
                    //if (aList != null)
                    //{
                    //    RedisHelperEx.DB_CoreCacheData.SetObj(cacheKey, aList, TimeSpan.FromMinutes(10));
                    //}
                    //}
                    //else
                    //{
                    //    aList = obj;
                    //}
                }
                if (aList != null && aList.ArticleList.Count > 0)
                {
                    foreach (var item in aList.ArticleList)
                    {
                        list.Add(new
                        {
                            Id = item.Id,
                            Title = item.Title,
                            GameCode = item.GameCode,
                            Category = item.Category.Trim(),
                            CreateTime = item.CreateTime,
                            ReadCount = item.ReadCount,
                            IsRedTitle = item.IsRedTitle
                        });
                    }
                }
                if (list != null && list.Count > 0)
                {
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "查询文章成功",
                        MsgId = entity.MsgId,
                        Value = list,
                    });
                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询文章成功",
                    MsgId = entity.MsgId,
                    Value = string.Empty,
                });
            }
            catch (ArgumentNullException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "获取失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = "获取失败",
                });
            }
        }
        #endregion

        #region 查询网站公告(124,140)
        /// <summary>
        /// 查询网站公告列表_124
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryNoticeList([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                int pageIndex = p.PageIndex;
                int pageSize = p.PageSize;
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("pageIndex", pageIndex);
                param.Add("pageSize", pageSize);
                param.Add("agent", (int)BulletinAgent.Local);
                var noticeList = await _serviceProxyProvider.Invoke<BulletinInfo_Collection>(param, "api/Data/QueryDisplayBulletinCollection");
                //var noticeList = WCFClients.ExternalClient.QueryDisplayBulletinCollection(BulletinAgent.Local, pageIndex, pageSize, userToken);
                noticeList.BulletinList.OrderByDescending(a => a.IsPutTop).OrderByDescending(a => a.CreateTime).ToList();
                var list = new List<object>();
                if (noticeList != null && noticeList.BulletinList.Count > 0)
                {

                    foreach (var item in noticeList.BulletinList)
                    {
                        list.Add(new
                        {
                            Id = item.Id,
                            Title = item.Title,
                            GameCode = string.Empty,
                            Category = "GG",
                            CreateTime = item.CreateTime,
                        });
                    }
                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询公告列表成功",
                    MsgId = entity.MsgId,
                    Value = list,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = string.Empty,
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = string.Empty,
                });
            }
        }
        /// <summary>
        /// 查询公告详情_140
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryDisplayBulletinDetailById([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string Id = p.BulletinId;
                if (string.IsNullOrEmpty(Id))
                    throw new LogicException("公告编号不能为空");
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("bulletinId", Convert.ToInt64(Id));
                var result = await _serviceProxyProvider.Invoke<BulletinInfo_Query>(param, "api/Data/QueryDisplayBulletinDetailById");
                //var result = WCFClients.ExternalClient.QueryDisplayBulletinDetailById(Convert.ToInt64(Id));
                var list = new List<object>();
                if (result != null)
                {
                    list.Add(new
                    {
                        Id = result.Id,
                        Title = result.Title,
                        Content = result.Content,
                        CreateTime = result.CreateTime == null ? "" : result.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        CreatorDisplayName = result.CreatorDisplayName,
                    });

                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询公告详情成功",
                    MsgId = entity.MsgId,
                    Value = list,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "获取失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = "获取失败",
                });
            }
        }
        #endregion

        #region 提交建议(126)
        /// <summary>
        /// 提交建议_126
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> SubmitSuggestions([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string suggestion = p.Content;
                string UserId = p.UserId;
                string UserName = p.UserName;
                if (string.IsNullOrEmpty(suggestion))
                    throw new LogicException("意见内容不能为空");
                else if (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(UserName))
                    throw new LogicException("请登录后再提交意见");
                string createUserName = p.UserName;
                string mobile = p.Mobile;
                string PageOpenSpeed = p.PageOpenSpeed;
                string InterfaceBeautiful = p.InterfaceBeautiful;
                string ComposingReasonable = p.ComposingReasonable;
                string OperationReasonable = p.OperationReasonable;
                string ContentConveyDistinct = p.ContentConveyDistinct;

                UserIdeaInfo_Add ideaInfo = new UserIdeaInfo_Add()
                {
                    Description = suggestion,
                    Category = "APP建议",
                    IsAnonymous = false,
                    CreateUserId = UserId,
                    CreateUserDisplayName = createUserName,
                    CreateUserMoibile = mobile,
                    PageOpenSpeed = decimal.Parse(PageOpenSpeed),
                    InterfaceBeautiful = decimal.Parse(InterfaceBeautiful),
                    ComposingReasonable = decimal.Parse(ComposingReasonable),
                    OperationReasonable = decimal.Parse(OperationReasonable),
                    ContentConveyDistinct = decimal.Parse(ContentConveyDistinct),
                };
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("userIdea", ideaInfo);
                var result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/Data/SubmitUserIdea");
                //var result = WCFClients.ExternalClient.SubmitUserIdea(ideaInfo);
                return Json(new LotteryServiceResponse
                {
                    Code = result.IsSuccess ? ResponseCode.成功 : ResponseCode.失败,
                    Message = result.IsSuccess ? "提交意见成功" : "提交意见失败",
                    MsgId = entity.MsgId,
                    Value = result.Message,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        #endregion

        #region 匿名用户Token值
        /// <summary>
        /// 匿名用户Token值
        /// </summary>
        private async Task<string> GuestUserToken([FromServices]IServiceProxyProvider _serviceProxyProvider)
        {
            var result = await _serviceProxyProvider.Invoke<CommonActionResult>(new Dictionary<string, object>(), "api/Data/GetGuestToken");
            return result.ReturnValue;
            //return WCFClients.ExternalClient.GetGuestToken().ReturnValue;
        }
        #endregion

        #region 查询网站配置(141)

        /// <summary>
        /// 根据Key值，查询网站相关配置_141
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryCoreConfigByKey([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string key = p.ConfigKey;
                if (string.IsNullOrEmpty(key))
                    throw new LogicException("传入参数错误");
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("key", key);
                var value = string.Empty;
                switch (key)
                {
                    case "IsShowData":
                        //value = WCFClients.GameClient.QueryCoreConfigByKey("IsShowData").ConfigValue;
                        value = "true";
                        break;
                    default:
                        var config = await _serviceProxyProvider.Invoke<C_Core_Config>(param, "api/Data/QueryCoreConfigByKey");
                        value = config.ConfigValue;
                        //value = WCFClients.GameClient.QueryCoreConfigByKey(key).ConfigValue;
                        break;
                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询配置信息成功",
                    MsgId = entity.MsgId,
                    Value = value,
                });
            }

            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }

        #endregion

        #region 查询版本信息(143)
        /// <summary>
        /// 查询APP升级配置_143
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryAPPConfig([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string appAgentId = p.AppAgentId;
                if (string.IsNullOrEmpty(appAgentId))
                    appAgentId = "100000";//公司APP特定编号
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("appAgentId", appAgentId);
                var configInfo = await _serviceProxyProvider.Invoke<APPConfigInfo>(param, "api/Data/QueryAppConfigByAgentId");
                //var configInfo = WCFClients.GameClient.QueryAppConfigByAgentId(appAgentId);
                if (configInfo == null)
                    throw new LogicException("未查询到当前代理升级信息");
                List<string> contentList = new List<string>();
                var array = configInfo.ConfigUpdateContent.Split('*');
                foreach (var item in array)
                {
                    contentList.Add("* " + item);
                }
                string updateUrl = string.Format("{0}/" + configInfo.AgentName + "_{1}.apk", configInfo.ConfigDownloadUrl, configInfo.ConfigVersion);
                if (entity.SourceCode == EntityModel.Enum.SchemeSource.NewIphone)
                    updateUrl = configInfo.ConfigDownloadUrl;
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询版本信息成功",
                    MsgId = entity.MsgId,
                    Value = new
                    {
                        Version = configInfo.ConfigVersion,
                        Content = contentList,
                        UpdateUrl = updateUrl,
                        Code = configInfo.ConfigCode,
                        IsUpgrade = Convert.ToBoolean(configInfo.IsForcedUpgrade),
                    },
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        #endregion

        #region 查询APP嵌套地址(145)
        /// <summary>
        /// 根据key值查询APP嵌套地址_145
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryNestedUrlConfigListByUrlType([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                //var p = System.Web.Helpers.Json.Decode(entity.Param);
                //int urlTyp = p.UrlType;
                //if (urlTyp <= 0)
                //    throw new Exception("传入参数错误");
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("urlType", 10);
                var result = await _serviceProxyProvider.Invoke<NestedUrlConfig_Collection>(param, "api/Data/QueryNestedUrlConfigListByUrlType");
                //var result = WCFClients.GameClient.QueryNestedUrlConfigListByUrlType(10);
                var list = new List<object>();
                if (result != null)
                {
                    foreach (var item in result.NestedUrlList)
                    {
                        list.Add(new
                        {
                            ConfigKey = item.ConfigKey,
                            CreateTime = item.CreateTime,
                            Id = item.Id,
                            IsEnable = item.IsEnable,
                            Remarks = item.Remarks,
                            Url = item.Url,
                            UrlType = item.UrlType,
                        });
                    }
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "查询成功",
                        MsgId = entity.MsgId,
                        Value = list,
                    });
                }
                throw new LogicException("查询嵌套地址失败");
            }
            //catch (ArgumentException ex)
            //{
            //    return new LotteryServiceResponse
            //    {
            //        Code = ResponseCode.失败,
            //        Message = "业务参数错误",
            //        MsgId = entity.MsgId,
            //        Value = ex.ToGetMessage(),
            //    };
            //}
            catch (Exception ex)
            {
                var list = new List<object>();
                list.Add(new
                {
                    ConfigKey = "all_sjzy",
                    CreateTime = DateTime.Now,
                    Id = 1,
                    IsEnable = true,
                    Remarks = "手机主页",
                    Url = "http://m1.qcwapps.com/",
                    UrlType = "10",
                });
                list.Add(new
                {
                    ConfigKey = "all_zcdz",
                    CreateTime = DateTime.Now,
                    Id = 3,
                    IsEnable = true,
                    Remarks = "注册地址",
                    Url = "http://m1.qcwapps.com/home/appRegistNew",
                    UrlType = "10",
                });
                list.Add(new
                {
                    ConfigKey = "all_czdz",
                    CreateTime = DateTime.Now,
                    Id = 3,
                    IsEnable = true,
                    Remarks = "充值地址",
                    Url = "http://m1.qcwapps.com/weixin/LoginForToken_APP_recharge",
                    UrlType = "10",
                });
                list.Add(new
                {
                    ConfigKey = "all_tkdz",
                    CreateTime = DateTime.Now,
                    Id = 3,
                    IsEnable = true,
                    Remarks = "提款地址",
                    Url = "http://m1.qcwapps.com/weixin/LoginForToken_APP_drawings",
                    UrlType = "10",
                });
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询成功",
                    MsgId = entity.MsgId,
                    Value = list,
                });
                //return new LotteryServiceResponse
                //{
                //    Code = ResponseCode.失败,
                //    Message = "查询APP嵌套地址失败",
                //    MsgId = entity.MsgId,
                //    Value = ex.ToGetMessage(),
                //};
            }
        }

        #endregion

        #region 站内信(146,147)

        /// <summary>
        /// 查询站内信_146
        /// </summary>
        public async Task<IActionResult> QueryInnermailList([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                int innerstatus = p.InnerStatus;
                string userToken = p.UserToken;
                string userId = p.UserId;
                int pageIndex = p.PageIndex;
                int pageSize = p.PageSize;
                if (string.IsNullOrEmpty(userToken) || string.IsNullOrEmpty(userId))
                    throw new LogicException("未获取到有效用户信息");
                string tokenuserId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                if (tokenuserId != userId)
                    throw new LogicException("token验证失败");
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("pageIndex", pageIndex);
                param.Add("pageSize", pageSize);
                #region 20151226
                //InnerMailInfo_QueryCollection collection = new InnerMailInfo_QueryCollection();
                //if (innerstatus == 3)
                //{
                //    collection = WCFClients.GameQueryClient.QueryMyInnerMailList(pageIndex, pageSize, UserToken);
                //}
                //else
                //{
                //    var type = (InnerMailHandleType)(innerstatus);
                //    collection = WCFClients.GameQueryClient.QueryUnReadInnerMailListByReceiver(userId, pageIndex, pageSize, type);
                //} 
                SiteMessageInnerMailListNew_Collection collection = new SiteMessageInnerMailListNew_Collection();
                if (innerstatus == 3)
                {
                    param.Add("userId", userId);
                    collection = await _serviceProxyProvider.Invoke<SiteMessageInnerMailListNew_Collection>(param, "api/Data/QueryMyInnerMailList");
                    //collection = WCFClients.GameQueryClient.QueryMyInnerMailList(pageIndex, pageSize, UserToken);
                }
                else
                {
                    var type = innerstatus;
                    param.Add("handleType", type);
                    param.Add("userId", userId);
                    collection = await _serviceProxyProvider.Invoke<SiteMessageInnerMailListNew_Collection>(param, "api/Data/QueryUnReadInnerMailListByReceiver");
                    //collection = WCFClients.GameQueryClient.QueryUnReadInnerMailListByReceiver(userId, pageIndex, pageSize, type);
                }
                #endregion
                var list = new List<object>();
                if (collection != null && collection.TotalCount > 0)
                {
                    #region 20151226
                    //foreach (var item in collection.InnerMailList)
                    //{
                    //    list.Add(new
                    //    {
                    //        ActionTime = ConvertDateTimeInt(item.ActionTime),
                    //        Content = string.IsNullOrEmpty(item.Content) ? string.Empty : item.Content,
                    //        HandleType = item.HandleType,
                    //        MailId = item.MailId,
                    //        SenderId = item.SenderId,
                    //        SendTime = ConvertDateTimeInt(item.SendTime),
                    //        Title = item.Title,
                    //        UpdateTime = ConvertDateTimeInt(item.UpdateTime)
                    //    });
                    //} 
                    foreach (var item in collection.MailList)
                    {
                        list.Add(new
                        {
                            ActionTime = item.SendTime,
                            Content = string.IsNullOrEmpty(item.MsgContent) ? string.Empty : item.MsgContent,
                            HandleType = item.HandleType,
                            MailId = item.MailId,
                            SenderId = item.SenderId,
                            SendTime = item.SendTime,
                            Title = item.Title,
                            UpdateTime = item.SendTime
                        });
                    }
                    #endregion
                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询站内信成功",
                    MsgId = entity.MsgId,
                    Value = list,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = "",
                });
            }
        }
        /// <summary>
        /// 查询站内信内容_147
        /// </summary>
        public async Task<IActionResult> QueryInnermailContentByMailId([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string mailId = p.MailId;
                string userToken = p.UserToken;
                if (string.IsNullOrEmpty(userToken))
                    throw new LogicException("您还未登录，请先登录！");
                if (string.IsNullOrEmpty(mailId))
                    throw new LogicException("站信编号不能为空！");
                string userId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("innerMailId", mailId);
                param.Add("userId", userId);
                var mailContent = await _serviceProxyProvider.Invoke<InnerMailInfo_Query>(param, "api/Data/ReadInnerMail");
                //var mailContent = WCFClients.GameQueryClient.ReadInnerMail(mailId, userToken);
                if (mailContent != null)
                {
                    var list = new List<object>();
                    var content = mailContent.Content;
                    if (!string.IsNullOrEmpty(content))
                    {
                        var index = content.IndexOf("<img");
                        if (index >= 0)
                            content = content.Replace("<img", "<img  width=\"100%\" ");
                    }
                    list.Add(new
                    {
                        ActionTime = mailContent.ActionTime,
                        Content = string.IsNullOrEmpty(content) ? string.Empty : content,
                        HandleType = mailContent.HandleType,
                        MailId = mailContent.MailId,
                        SenderId = mailContent.SenderId,
                        AuthorName = "爱玩彩",
                        SendTime = mailContent.SendTime,
                        Title = mailContent.Title,
                        UpdateTime = mailContent.UpdateTime
                    });
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "查询站内信内容成功",
                        MsgId = entity.MsgId,
                        Value = list,
                    });
                }
                throw new Exception("查询站内信内容失败");
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = "",
                });
            }
        }

        #endregion

        #region 查询红包使用规则(163)

        public async Task<IActionResult> QueryRedBagUseConfig([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {

                //var result = WCFClients.ActivityClient.QueryRedBagUseConfig();
                var result = await _serviceProxyProvider.Invoke<string>(new Dictionary<string, object>(), "api/Data/QueryRedBagUseConfig");
                var list = new List<object>();
                if (!string.IsNullOrEmpty(result))
                {
                    var arrConfig = result.Split('|');
                    if (arrConfig.Length > 0)
                    {
                        foreach (var item in arrConfig)
                        {
                            var arrTemp = item.Split('_');
                            if (arrTemp.Length >= 2)
                            {
                                list.Add(new
                                {
                                    GameCode = arrTemp[1],
                                    UsePercent = arrTemp[2]
                                });
                            }
                        }
                    }
                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询红包使用规则成功",
                    MsgId = entity.MsgId,
                    Value = list,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询红包使用规则出错" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }

        #endregion

        #region 资讯新闻(208)
        /// <summary>
        /// 资讯新闻
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryArticleList([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                //热点彩讯  FocusCMS
                //赛事点评 Match_Comment
                //彩票资讯 Lottery_GameCode
                string category = p.Category;
                int pageIndex = p.PageIndex;
                int pageSize = p.PageSize;
                var list = new List<object>();
                Dictionary<string, object> param = new Dictionary<string, object>();
                //param.Add("pageIndex", pageIndex);
                //param.Add("pageSize", pageSize);
                switch (category)
                {
                    case "hot"://今日热点
                        param.Add("category", "Lottery_Hot");
                        param.Add("gameCode", "");
                        param.Add("pageIndex", pageIndex);
                        param.Add("pageSize", pageSize);
                        var hot = await _serviceProxyProvider.Invoke<ArticleInfo_QueryCollection>(param, "api/Data/QueryArticleList_YouHua");
                        //var hot = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_Hot", "", pageIndex, pageSize);
                        foreach (var item in hot.ArticleList)
                        {
                            list.Add(new
                            {
                                Id = item.Id,
                                Title = item.Title,
                                GameCode = item.GameCode,
                                Category = item.Category.Trim(),
                                CreateTime = item.CreateTime,
                            });
                        }
                        break;
                    case "gpc"://高频彩
                        param.Add("category", "Lottery_GameCode");
                        param.Add("gameCode", "JX11X5|CQSSC|SD11X5|GD11X5|GDKLSF|JSKS|SDKLPK3");
                        param.Add("pageIndex", pageIndex);
                        param.Add("pageSize", pageSize);
                        var gpc = await _serviceProxyProvider.Invoke<ArticleInfo_QueryCollection>(param, "api/Data/QueryArticleList_YouHua");
                        //var gpc = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "JX11X5|CQSSC|SD11X5|GD11X5|GDKLSF|JSKS|SDKLPK3", pageIndex, pageSize);
                        foreach (var item in gpc.ArticleList)
                        {
                            list.Add(new
                            {
                                Id = item.Id,
                                Title = item.Title,
                                GameCode = item.GameCode,
                                Category = item.Category.Trim(),
                                CreateTime = item.CreateTime,
                            });
                        }
                        break;
                    case "szc"://数字彩
                        param.Add("category", "Lottery_GameCode");
                        param.Add("gameCode", "SSQ|DLT|PL3|FC3D");
                        param.Add("pageIndex", pageIndex);
                        param.Add("pageSize", pageSize);
                        var scz = await _serviceProxyProvider.Invoke<ArticleInfo_QueryCollection>(param, "api/Data/QueryArticleList_YouHua");
                        //var scz = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "SSQ|DLT|PL3|FC3D", pageIndex, pageSize);
                        foreach (var item in scz.ArticleList)
                        {
                            list.Add(new
                            {
                                Id = item.Id,
                                Title = item.Title,
                                GameCode = item.GameCode,
                                Category = item.Category.Trim(),
                                CreateTime = item.CreateTime,
                            });
                        }
                        break;
                    case "jjc"://竞技彩
                        param.Add("category", "Lottery_GameCode");
                        param.Add("gameCode", "JCZQ|JCLQ|BJDC");
                        param.Add("pageIndex", pageIndex);
                        param.Add("pageSize", pageSize);
                        var jjc = await _serviceProxyProvider.Invoke<ArticleInfo_QueryCollection>(param, "api/Data/QueryArticleList_YouHua");
                        //var jjc = WCFClients.ExternalClient.QueryArticleList_YouHua("Lottery_GameCode", "JCZQ|JCLQ|BJDC", pageIndex, pageSize);
                        foreach (var item in jjc.ArticleList)
                        {
                            list.Add(new
                            {
                                Id = item.Id,
                                Title = item.Title,
                                GameCode = item.GameCode,
                                Category = item.Category.Trim(),
                                CreateTime = item.CreateTime,
                            });
                        }
                        break;
                    case "FocusCMS"://焦点新闻
                        param.Add("category", "FocusCMS");
                        param.Add("gameCode", "");
                        param.Add("pageIndex", pageIndex);
                        param.Add("pageSize", pageSize);
                        var FocusCMS = await _serviceProxyProvider.Invoke<ArticleInfo_QueryCollection>(param, "api/Data/QueryArticleList_YouHua");
                        //var FocusCMS = WCFClients.ExternalClient.QueryArticleList_YouHua("FocusCMS", "", pageIndex, pageSize);
                        foreach (var item in FocusCMS.ArticleList)
                        {
                            list.Add(new
                            {
                                Id = item.Id,
                                Title = item.Title,
                                GameCode = item.GameCode,
                                Category = item.Category.Trim(),
                                CreateTime = item.CreateTime,
                            });
                        }
                        break;
                    case "BonusCMS"://焦点新闻
                        param.Add("category", "BonusCMS");
                        param.Add("gameCode", "");
                        param.Add("pageIndex", pageIndex);
                        param.Add("pageSize", pageSize);
                        var BonusCMS = await _serviceProxyProvider.Invoke<ArticleInfo_QueryCollection>(param, "api/Data/QueryArticleList_YouHua");
                        //var BonusCMS = WCFClients.ExternalClient.QueryArticleList_YouHua("BonusCMS", "", pageIndex, pageSize);
                        foreach (var item in BonusCMS.ArticleList)
                        {
                            list.Add(new
                            {
                                Id = item.Id,
                                Title = item.Title,
                                GameCode = item.GameCode,
                                Category = item.Category.Trim(),
                                CreateTime = item.CreateTime,
                            });
                        }
                        break;
                }
                if (list != null && list.Count > 0)
                {
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "查询文章成功",
                        MsgId = entity.MsgId,
                        Value = list,
                    });
                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询文章成功",
                    MsgId = entity.MsgId,
                    Value = string.Empty,
                });
            }
            catch (ArgumentNullException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = string.Empty,
                });
            }
        }
        #endregion

        #region 用户协议配置/首页配置(213,216)
        /// <summary>
        /// 用户协议配置_213
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> UserAgreementConfig([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("key", "Agreement_Config");
                var config = await _serviceProxyProvider.Invoke<C_Core_Config>(param, "api/Data/QueryCoreConfigByKey");
                var Agreement = config.ConfigValue;
                //await _serviceProxyProvider.Invoke<string>(null, "api/Data/Agreement_Config");
                //var Agreement = WebRedisHelperEx.Agreement_Config;
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "获取成功",
                    MsgId = entity.MsgId,
                    Value = Agreement
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }

        /// <summary>
        /// 首页配置_216
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> IndexConfig([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("key", "Index_Config");
                var config = await _serviceProxyProvider.Invoke<C_Core_Config>(param, "api/Data/QueryCoreConfigByKey");
                //var Index = await _serviceProxyProvider.Invoke<string>(null, "api/Data/Agreement_Config");
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "获取成功",
                    MsgId = entity.MsgId,
                    Value = config.ConfigValue
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        #endregion

        #region 世界杯数据(217)
        /// <summary>
        /// 世界杯数据_217
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LotteryServiceResponse SJBMatchJson(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string GameType = p.GameType;
                if (string.IsNullOrEmpty(GameType))
                    throw new LogicException("传入游戏类型不能为空");
                //string photourl = ConfigurationManager.AppSettings["ResourceSiteUrl_res"].ToString();
                var photourl = ConfigHelper.AllConfigInfo["ResourceSiteUrl_res"].ToString();
                string data = string.Empty;
                if (GameType.ToUpper() == "GJ")
                {
                    List<JCZQ_SJBMatchInfo> list = Json_JCZQ.SJBMatchList(GameType);
                    foreach (var item in list)
                    {
                        string image = string.Format("{0}/images/football/{1}.png", photourl, item.Team);
                        data += string.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}|", item.MatchId, item.Team, item.BetState, item.BonusMoney, item.SupportRate,
                            item.Probadbility, image);
                    }
                    var str = "{\"data\":[{\"data\":\"" + data.TrimEnd('|') + "\",\"id\":\"104895\",\"p_id\":\"467130\",\"name\":\"2018\u4e16\u754c\u676f\",\"odds_type\":\"CHP\"}]}";
                    return new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "获取成功",
                        MsgId = entity.MsgId,
                        Value = str,
                    };
                    //return "{\"data\":[{\"data\":\"" + data.TrimEnd('|') + "\",\"id\":\"104895\",\"p_id\":\"467130\",\"name\":\"2018\u4e16\u754c\u676f\",\"odds_type\":\"CHP\"}]}";
                }
                else if (GameType.ToUpper() == "GYJ")
                {
                    List<JCZQ_SJBMatchInfo> list = Json_JCZQ.SJBMatchList(GameType);
                    foreach (var item in list)
                    {
                        if (item.Team != "其它")
                        {
                            var arr = item.Team.Split('—');
                            string image1 = string.Format("{0}/images/football/{1}.png", photourl, arr[0]);
                            string image2 = string.Format("{0}/images/football/{1}.png", photourl, arr[1]);
                            data += string.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}—{7}|", item.MatchId, item.Team, item.BetState, item.BonusMoney, item.SupportRate,
                                item.Probadbility, image1, image2);
                        }
                        else
                        {
                            data += string.Format("{0}-{1}-{2}-{3}-{4}-{5}-nopic|", item.MatchId, item.Team, item.BetState, item.BonusMoney, item.SupportRate,
                                item.Probadbility);

                        }

                    }
                    var str = "{\"data\":[{\"data\":\"" + data.TrimEnd('|') + "\",\"id\":\"104895\",\"p_id\":\"467131\",\"name\":\"2018\u4e16\u754c\u676f\",\"odds_type\":\"FNL\"}]}";
                    return new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "获取成功",
                        MsgId = entity.MsgId,
                        Value = str,
                    };
                    //return "{\"data\":[{\"data\":\"" + data.TrimEnd('|') + "\",\"id\":\"104895\",\"p_id\":\"467131\",\"name\":\"2018\u4e16\u754c\u676f\",\"odds_type\":\"FNL\"}]}";
                }
                else if (GameType == "GYJB")
                {
                    List<JCZQ_SJBMatchInfo> list = Json_JCZQ.SJBMatchList("GYJ");
                    foreach (var item in list)
                    {
                        if (item.Team != "其它")
                        {
                            var arr = item.Team.Split('—');
                            string image1 = string.Format("{0}/images/football/{1}.png", photourl, arr[0]);
                            string image2 = string.Format("{0}/images/football/{1}.png", photourl, arr[1]);
                            data += string.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}—{7}|", item.MatchId, item.Team, item.BetState, item.BonusMoney, item.SupportRate,
                                item.Probadbility, image1, image2);
                        }
                        else
                        {
                            data += string.Format("{0}-{1}-{2}-{3}-{4}-{5}-nopic|", item.MatchId, item.Team, item.BetState, item.BonusMoney, item.SupportRate,
                                item.Probadbility);

                        }

                    }
                    var str = "{\"data\":[{\"data\":\"" + data.TrimEnd('|') + "\",\"id\":\"104895\",\"p_id\":\"467131\",\"name\":\"2018\u4e16\u754c\u676f\",\"odds_type\":\"FNL\"}]}";
                    return new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "获取成功",
                        MsgId = entity.MsgId,
                        Value = str,
                    };
                }
                else
                {
                    return new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "暂无数据",
                        MsgId = entity.MsgId,
                        Value = "",
                    };
                }
            }
            catch (Exception ex)
            {

                return new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                };
            }
        }

        #endregion

        #region 获取用户分享信息(225)
        /// <summary>
        /// 获取用户分享信息_225
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryShareSpreadUsers([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string UserId = p.UserId;
                if (string.IsNullOrEmpty(UserId))
                    throw new LogicException("UserId不能为空");

                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("agentId", UserId);
                param.Add("startTime", DateTime.Now);
                param.Add("endTime", DateTime.Now);
                param.Add("pageIndex", 0);
                param.Add("pageSize", 15);
                var resultmodel = await _serviceProxyProvider.Invoke<ShareSpreadCollection>(param, "api/Data/QueryShareSpreadUsers");
                //var resultmodel = WCFClients.ExternalClient.QueryShareSpreadUsers(UserId, DateTime.Now, DateTime.Now, 0, 15);
                var result = new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "获取用户分享数据成功",
                    MsgId = entity.MsgId,
                    Value = resultmodel,
                };
                return Json(result);
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = "",
                });
            }
        }
        #endregion

        #region 查询活动列表(127)
        /// <summary>
        /// 查询活动列表_127
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryActivInfoList([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                int pageIndex = p.PageIndex;
                int pageSize = p.PageSize;
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("pageIndex", pageIndex);
                param.Add("pageSize", pageSize);
                var resultList = await _serviceProxyProvider.Invoke<ActivityListInfoCollection>(param, "api/Data/QueryActivInfoList");
                //var resultList = WCFClients.ExternalClient.QueryActivInfoList(pageIndex, pageSize);
                if (resultList != null && resultList.List.Count > 0)
                {
                    var list = new List<object>();
                    foreach (var item in resultList.List)
                    {
                        list.Add(new
                        {
                            ActivityIndex = item.ActivityIndex,
                            ImageUrl = item.ImageUrl,
                            IsShow = item.IsShow,
                            ActiveName = item.ActiveName,
                            LinkUrl = ConvertHelper.GetDomain() + item.LinkUrl,
                            Title = item.Title,
                            Summary = item.Summary,
                            BeginTime = item.BeginTime,
                            EndTime = item.EndTime,
                            CreateTime = item.CreateTime
                        });
                    }
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "查询活动列表成功",
                        MsgId = entity.MsgId,
                        Value = list,
                    });
                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询活动列表成功",
                    MsgId = entity.MsgId,
                    Value = string.Empty,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }

        #endregion

        #region 竞彩最新期号(130)
        /// <summary>
        /// 查询最新期号
        /// </summary>
        public async Task<IActionResult> QueryJingCaiIssuse([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                int count = p.ReturnCount;
                if (count <= 0)
                    count = 10;
                string gameCode = p.GameCode;
                string gameType = p.GameType;
                var list = new List<object>();
                if (gameCode.ToUpper() == "BJDC")
                {
                    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    var jsonData = string.Empty;
                    var bjdcIssuseFileName = Path.Combine(path, string.Format("lottery_lastIssuse_{0}.json", gameCode));
                    if (System.IO.File.Exists(bjdcIssuseFileName))
                    {
                        jsonData = System.IO.File.ReadAllText(bjdcIssuseFileName, Encoding.UTF8);
                    }
                    foreach (var item in jsonData.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        list.Add(new
                        {
                            IssuseNumber = item,
                        });
                    }
                }
                else if (gameCode.ToUpper() == "CTZQ")
                {
                    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsonData");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    var jsonData = string.Empty;
                    var ctzqIssuseFileName = Path.Combine(path, string.Format("lottery_lastIssuse_{0}.json", gameType));
                    if (System.IO.File.Exists(ctzqIssuseFileName))
                    {
                        jsonData = System.IO.File.ReadAllText(ctzqIssuseFileName, Encoding.UTF8);
                    }
                    foreach (var item in jsonData.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        list.Add(new
                        {
                            IssuseNumber = item,
                        });
                    }
                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询最新期号成功",
                    MsgId = entity.MsgId,
                    Value = list,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }
        #endregion

        #region 获取投注内容中文名称(131)

        public async Task<IActionResult> GetAnteCodeDisplayName(LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string gameType = p.GameType;
                string gameCode = p.GameCode;
                string anteCode = p.AnteCode;
                string letBall = p.LetBall;
                string strName = string.Empty;
                switch (gameCode.ToUpper())
                {
                    case "JCLQ":
                        strName = ANTECODES_JCLQ(gameType, anteCode);
                        break;
                    case "JCZQ":
                        strName = ANTECODES_JCZQ(gameType, anteCode, Convert.ToInt32(letBall));
                        break;
                    case "BJDC":
                        strName = ANTECODES_BJDC(gameType, anteCode);
                        break;
                    case "CTZQ":
                        strName = ANTECODES_CTZQ(gameType, anteCode);
                        break;
                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "获取投注内容显示名成功",
                    MsgId = entity.MsgId,
                    Value = strName,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }

        #endregion

        #region 查询比赛数据列表(135)

        public async Task<IActionResult> QueryMatchDataList([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                //string userToken = p.UserToken;
                string gameCode = p.GameCode == null ? null : ((string)p.GameCode).ToUpper();
                string gameType = p.GameType == null ? null : ((string)p.GameType).ToUpper();
                string issuseNumber = p.IssuseNumber == null ? null : ((string)p.IssuseNumber).ToUpper();
                string newVerType = p.NewVerType;

                if (string.IsNullOrEmpty(gameCode))
                    throw new LogicException("彩种不能为空");
                if (string.IsNullOrEmpty(gameType))
                    throw new LogicException("玩法不能为空");
                if ((gameCode == "CTZQ" || gameCode == "BJDC") && string.IsNullOrEmpty(issuseNumber))
                    throw new LogicException("期号不能为空");

                var matchDataList = new List<object>();
                var param = new Dictionary<string, object>();
                //object mlist = null;
                string key = "";
                switch (gameCode)
                {
                    case "CTZQ":
                        //var cur = await _serviceProxyProvider.Invoke<ActivityListInfoCollection>(param, "api/Data/QueryActivInfoList");
                        //var cur = WCFClients.GameIssuseClient.QueryCurretNewIssuseInfo(gameCode, gameType);
                        //cache 获取
                        //var _issuse = HashTableCache._IssuseCTZQHt[gameType] ?? Json_CTZQ.IssuseList(gameType);
                        key = $"{EntityModel.Redis.RedisKeys.Key_CTZQ_Issuse_List}_{gameType}";
                        //var issuse = RedisHelperEx.DB_Match.GetObjs<CtzqIssuesWeb>(key);
                        param.Add("Key", key);
                        var issuse = await _serviceProxyProvider.Invoke<List<CtzqIssuesWeb>>(param, "api/Data/GetCTZQIssuseList_ByRedis");
                        if (issuse == null)
                        {
                            issuse = Json_CTZQ.IssuseList(gameType);
                        }
                        var theissuse = issuse.FirstOrDefault(c => c.IssuseNumber == issuseNumber);
                        if (theissuse != null)
                        {
                            var now = DateTime.Now;
                            if (Convert.ToDateTime(theissuse.StartTime) > now) break;
                            key = $"{EntityModel.Redis.RedisKeys.Key_CTZQ_Match_Odds_List}_{gameType}_{theissuse.IssuseNumber}";
                            param.Clear();
                            param.Add("Key", key);
                            //var oddlist_ctzq = RedisHelperEx.DB_Match.GetObjs<CTZQ_MatchInfo_WEB>(key);
                            var oddlist_ctzq = await _serviceProxyProvider.Invoke<List<CTZQ_MatchInfo_WEB>>(param, "api/Data/GetCTZQMatchOddsList_ByRedis");
                            if (oddlist_ctzq == null)
                            {
                                oddlist_ctzq = Json_CTZQ.MatchList_WEB(issuseNumber, gameType); ;
                            }
                            matchDataList.AddRange(oddlist_ctzq);
                        }
                        break;
                    case "BJDC":
                        key = $"{EntityModel.Redis.RedisKeys.Key_BJDC_Match_Odds_List}_{gameType}_{issuseNumber}";
                        param.Add("Key", key);
                        //var oddlist_bjdc = RedisHelperEx.DB_Match.GetObjs<BJDC_MatchInfo_WEB>(key);
                        var oddlist_bjdc = await _serviceProxyProvider.Invoke<List<BJDC_MatchInfo_WEB>>(param, "api/Data/GetBJDCMatchOddsLis_ByRedis");
                        if (oddlist_bjdc == null)
                        {
                            oddlist_bjdc = Json_BJDC.MatchList_WEB(issuseNumber, gameType);
                        }

                        matchDataList.AddRange(oddlist_bjdc);
                        break;
                    case "JCZQ":
                        key = EntityModel.Redis.RedisKeys.Key_JCZQ_Match_Odds_List;
                        string reidskey = key + "_" + gameType + (newVerType == null ? "" : newVerType);
                        param.Add("Key", reidskey);
                        var oddlist_jczq = await _serviceProxyProvider.Invoke<List<JCZQ_MatchInfo_WEB>>(param, "api/Data/GetJCZQMatchOddsList_ByRedis");
                        //var oddlist_jczq = RedisHelperEx.DB_Match.GetObjs<JCZQ_MatchInfo_WEB>(reidskey);
                        if (oddlist_jczq == null)
                        {
                            if (gameType.ToLower() == "hhdg")
                                oddlist_jczq = Json_JCZQ.GetJCZQHHDGList();
                            else
                            {
                                oddlist_jczq = Json_JCZQ.MatchList_WEB(gameType, newVerType);
                                #region 新逻辑20181022
                                //如果gametype为让分胜负与大小分，则需要拼装他们的state_hhdg
                                if (gameType.ToLower() == "brqspf")
                                {
                                    var oddlist_jczq_hhdg = Json_JCZQ.GetJCZQHHDGList();
                                    if (oddlist_jczq_hhdg != null && oddlist_jczq != null)
                                    {
                                        foreach (var item in oddlist_jczq)
                                        {
                                            var hhdgitem = oddlist_jczq_hhdg.FirstOrDefault(c => c.MatchId == item.MatchId);
                                            if (hhdgitem != null) item.State_HHDG = hhdgitem.State_HHDG;
                                        }
                                    }
                                }

                                #endregion
                            }
                        }
                        matchDataList.AddRange(oddlist_jczq);
                        break;
                    case "JCLQ":
                        key = $"{EntityModel.Redis.RedisKeys.Key_JCLQ_Match_Odds_List}_{gameType}";
                        param.Add("Key", key);
                        var oddlist_jclq = await _serviceProxyProvider.Invoke<List<JCLQ_MatchInfo_WEB>>(param, "api/Data/GetJCLQMatchOddsList_ByRedis");
                        //var oddlist_jclq = RedisHelperEx.DB_Match.GetObjs<JCLQ_MatchInfo_WEB>(key);
                        if (oddlist_jclq == null)
                        {
                            if (gameType.ToLower() == "hhdg")
                                oddlist_jclq = Json_JCLQ.GetJCLQHHDGList();

                            else
                            {
                                oddlist_jclq = Json_JCLQ.MatchList_WEB(gameType);

                                #region 新逻辑20181022
                                //新逻辑20181022
                                //如果gametype为让分胜负与大小分，则需要拼装他们的state_hhdg
                                if (gameType.ToLower() == "rfsf" || gameType.ToLower() == "dxf")
                                {
                                    var oddlist_jclq_hhdg = Json_JCLQ.GetJCLQHHDGList();
                                    if (oddlist_jclq_hhdg != null && oddlist_jclq != null)
                                    {
                                        foreach (var item in oddlist_jclq)
                                        {
                                            var hhdgitem = oddlist_jclq_hhdg.FirstOrDefault(c => c.MatchId == item.MatchId);
                                            if (hhdgitem != null) item.State_HHDG = hhdgitem.State_HHDG;
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                        matchDataList.AddRange(oddlist_jclq);
                        break;
                    default:
                        throw new ArgumentException(string.Format("传入彩种{0}没有队伍信息", gameCode));
                }

                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询队伍信息成功",
                    MsgId = entity.MsgId,
                    Value = matchDataList,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "获取失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = "获取失败",
                });
            }
        }


        #endregion

        #region 查询最新期号(136)
        /// <summary>
        /// 查询最新期号_136
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> QueryCurrenIssuse([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string id = p.GameCode == null ? null : ((string)p.GameCode).ToUpper();
                string type = p.GameType == null ? null : ((string)p.GameType).ToUpper();
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("gameCode", id);
                param.Add("gameType", type);
                Issuse_QueryInfo cur = await _serviceProxyProvider.Invoke<Issuse_QueryInfo>(param, "api/Data/QueryCurretNewIssuseInfo");
                //Issuse_QueryInfo cur = WCFClients.GameIssuseClient.QueryCurretNewIssuseInfo(id, type);
                var list = new List<object>();
                List<CtzqIssuesWeb> issuse = Json_CTZQ.IssuseList(type);
                var theissuse = issuse.FirstOrDefault(c => c.IssuseNumber == cur.IssuseNumber);
                DateTime? startDate = null;
                if (theissuse != null)
                {
                    startDate = Convert.ToDateTime(theissuse.StartTime);
                }
                if (cur != null)
                {
                    list.Add(new { issuse = cur.IssuseNumber, stoptime = cur.LocalStopTime, servertime = DateTime.Now, starttime = startDate != null ? startDate.Value : cur.StartTime });
                }

                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询最新期号成功",
                    MsgId = entity.MsgId,
                    Value = list,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "获取失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = "获取失败",
                });
            }
        }
        #endregion

        #region 竞彩编码解析

        private static Dictionary<string, string> GetAnteCodeDic(string[] array1, string[] array2)
        {

            Dictionary<string, string> dicAnteCode = new Dictionary<string, string>();
            for (int i = 0; i < array1.Length; i++)
            {
                for (int j = i; j < array2.Length; j++)
                {
                    dicAnteCode.Add(array1[i], array2[j]);
                    break;
                }
            }
            return dicAnteCode;
        }
        /// <summary>
        /// 北单投注编码及显示码
        /// </summary>
        /// <param name="type">玩法类型</param>
        /// <param name="isCode">是否投注编码</param>
        /// <returns>返回投注编码或显示码</returns>
        public static string ANTECODES_BJDC(string type, string code, bool isCode = false)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(type))
                return string.Empty;
            try
            {
                switch (type)
                {
                    case "spf":
                        return GetAnteCodeDic("3,1,0".Split(','), "胜,平,负".Split(','))[code];
                    //return isCode ? "3,1,0" : "胜,平,负";
                    case "zjq":
                        return GetAnteCodeDic("0,1,2,3,4,5,6,7".Split(','), "0,1,2,3,4,5,6,7+".Split(','))[code];
                    //return isCode ? "0,1,2,3,4,5,6,7" : "0,1,2,3,4,5,6,7+";
                    case "sxds":
                        return GetAnteCodeDic("SD,SS,XD,XS".Split(','), "上单,上双,下单,下双".Split(','))[code];
                    //return isCode ? "SD,SS,XD,XS" : "上单,上双,下单,下双";
                    case "bf":
                        return GetAnteCodeDic("00,01,02,03,10,11,12,13,20,21,22,23,30,31,32,33,40,41,42,04,14,24,X0,XX,0X".Split(','), "0:0,0:1,0:2,0:3,1:0,1:1,1:2,1:3,2:0,2:1,2:2,2:3,3:0,3:1,3:2,3:3,4:0,4:1,4:2,0:4,1:4,2:4,胜其他,平其他,负其他".Split(','))[code];

                    //return isCode ? "00,01,02,03,10,11,12,13,20,21,22,23,30,31,32,33,40,41,42,04,14,24,X0,XX,0X" : "0:0,0:1,0:2,0:3,1:0,1:1,1:2,1:3,2:0,2:1,2:2,2:3,3:0,3:1,3:2,3:3,4:0,4:1,4:2,0:4,1:4,2:4,胜其他,平其他,负其他";
                    case "bqc":
                        return GetAnteCodeDic("33,31,30,13,11,10,03,01,00".Split(','), "胜胜,胜平,胜负,平胜,平平,平负,负胜,负平,负负".Split(','))[code];
                    //return isCode ? "33,31,30,13,11,10,03,01,00" : "胜胜,胜平,胜负,平胜,平平,平负,负胜,负平,负负";
                    default:
                        return GetAnteCodeDic("3,1,0".Split(','), "胜,平,负".Split(','))[code];
                        //return isCode ? "3,1,0" : "胜,平,负";
                }
            }
            catch
            { return string.Empty; }
        }

        /// <summary>
        /// 竞彩足球投注编码及显示码
        /// </summary>
        /// <param name="type">玩法类型</param>
        /// <param name="isCode">是否投注编码</param>
        /// <returns>返回投注编码或显示码</returns>
        public static string ANTECODES_JCZQ(string type, string code, int letball = 0, bool isCode = false)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(type))
                return string.Empty;
            try
            {
                switch (type)
                {
                    case "spf":
                        //return isCode ? "3,1,0" : "胜,平,负";
                        if (letball == 0)
                            return GetAnteCodeDic("3,1,0".Split(','), "胜,平,负".Split(','))[code];
                        else
                            return GetAnteCodeDic("3,1,0".Split(','), "让球胜,让球平,让球负".Split(','))[code];
                    //return isCode ? "3,1,0" : "胜,平,负,让球胜,让球平,让球负";
                    case "bqcspf":
                        return GetAnteCodeDic("3,1,0".Split(','), "胜,平,负".Split(','))[code];
                    //return isCode ? "3,1,0" : "胜,平,负";
                    case "zjq":
                        return GetAnteCodeDic("0,1,2,3,4,5,6,7".Split(','), "0,1,2,3,4,5,6,7+".Split(','))[code];
                    //return isCode ? "0,1,2,3,4,5,6,7" : "0,1,2,3,4,5,6,7+";
                    case "bf":
                        return GetAnteCodeDic("00,01,02,03,10,11,12,13,20,21,22,23,30,31,32,33,40,41,42,04,14,24,50,51,52,05,15,25,X0,XX,0X".Split(','), "0:0,0:1,0:2,0:3,1:0,1:1,1:2,1:3,2:0,2:1,2:2,2:3,3:0,3:1,3:2,3:3,4:0,4:1,4:2,0:4,1:4,2:4,5:0,5:1,5:2,0:5,1:5,2:5,胜其他,平其他,负其他".Split(','))[code];
                    //return isCode ? "00,01,02,03,10,11,12,13,20,21,22,23,30,31,32,33,40,41,42,04,14,24,50,51,52,05,15,25,X0,XX,0X" : "0:0,0:1,0:2,0:3,1:0,1:1,1:2,1:3,2:0,2:1,2:2,2:3,3:0,3:1,3:2,3:3,4:0,4:1,4:2,0:4,1:4,2:4,5:0,5:1,5:2,0:5,1:5,2:5,胜其他,平其他,负其他";
                    case "bqc":
                        return GetAnteCodeDic("33,31,30,13,11,10,03,01,00".Split(','), "胜胜,胜平,胜负,平胜,平平,平负,负胜,负平,负负".Split(','))[code];
                    //return isCode ? "33,31,30,13,11,10,03,01,00" : "胜胜,胜平,胜负,平胜,平平,平负,负胜,负平,负负";
                    case "hh":
                        return GetAnteCodeDic("3,1,0,00,01,02,03,10,11,12,13,20,21,22,23,30,31,32,33,40,41,42,04,14,24,50,51,52,05,15,25,X0,XX,0X,0,1,2,3,4,5,6,7,33,31,30,13,11,10,03,01,00,3,1,0".Split(','), "让球胜,让球平,让球负,0:0,0:1,0:2,0:3,1:0,1:1,1:2,1:3,2:0,2:1,2:2,2:3,3:0,3:1,3:2,3:3,4:0,4:1,4:2,0:4,1:4,2:4,5:0,5:1,5:2,0:5,1:5,2:5,胜其他,平其他,负其他,0,1,2,3,4,5,6,7+,胜胜,胜平,胜负,平胜,平平,平负,负胜,负平,负负,胜,平,负".Split(','))[code];
                    //return isCode ? "3,1,0,00,01,02,03,10,11,12,13,20,21,22,23,30,31,32,33,40,41,42,04,14,24,50,51,52,05,15,25,X0,XX,0X,0,1,2,3,4,5,6,7,33,31,30,13,11,10,03,01,00,3,1,0" : "让球胜,让球平,让球负,0:0,0:1,0:2,0:3,1:0,1:1,1:2,1:3,2:0,2:1,2:2,2:3,3:0,3:1,3:2,3:3,4:0,4:1,4:2,0:4,1:4,2:4,5:0,5:1,5:2,0:5,1:5,2:5,胜其他,平其他,负其他,0,1,2,3,4,5,6,7+,胜胜,胜平,胜负,平胜,平平,平负,负胜,负平,负负,胜,平,负";
                    default:
                        return GetAnteCodeDic("3,1,0".Split(','), "胜,平,负".Split(','))[code];
                        //return isCode ? "3,1,0" : "胜,平,负";
                }
            }
            catch { return string.Empty; }
        }

        /// <summary>
        /// 竞彩篮球投注编码及显示码
        /// </summary>
        /// <param name="type">玩法类型</param>
        /// <param name="isCode">是否投注编码</param>
        /// <returns>返回投注编码或显示码</returns>
        public static string ANTECODES_JCLQ(string type, string code, bool isCode = false)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(type))
                return string.Empty;
            try
            {
                switch (type)
                {
                    case "sf":
                    case "rfsf":
                        return GetAnteCodeDic("3,0".Split(','), "主胜,客胜".Split(','))[code];
                    //return isCode ? "3,0" : "主胜,客胜";
                    case "sfc":
                        return GetAnteCodeDic("01,02,03,04,05,06,11,12,13,14,15,16".Split(','), "胜1-5分,胜6-10分,胜11-15分,胜16-20分,胜21-25分,胜26分以上,负1-5分,负6-10分,负11-15分,负16-20分,负21-25分,负26分以上".Split(','))[code];
                    //return isCode ? "01,02,03,04,05,06,11,12,13,14,15,16" : "胜1-5分,胜6-10分,胜11-15分,胜16-20分,胜21-25分,胜26分以上,负1-5分,负6-10分,负11-15分,负16-20分,负21-25分,负26分以上";
                    case "dxf":
                        return GetAnteCodeDic("3,0".Split(','), "大,小".Split(','))[code];
                    //return isCode ? "3,0" : "大,小";
                    case "hh":
                        return GetAnteCodeDic("3,0,3,0,01,02,03,04,05,06,11,12,13,14,15,16,3,0".Split(','), "主胜,客胜,主胜,客胜,胜1-5分,胜6-10分,胜11-15分,胜16-20分,胜21-25分,胜26分以上,负1-5分,负6-10分,负11-15分,负16-20分,负21-25分,负26分以上,大,小".Split(','))[code];
                    //return isCode ? "3,0,3,0,01,02,03,04,05,06,11,12,13,14,15,16,3,0" : "主胜,客胜,主胜,客胜,胜1-5分,胜6-10分,胜11-15分,胜16-20分,胜21-25分,胜26分以上,负1-5分,负6-10分,负11-15分,负16-20分,负21-25分,负26分以上,大,小";
                    default:
                        return GetAnteCodeDic("3,0".Split(','), "主胜,客胜".Split(','))[code];
                        //return isCode ? "3,0" : "主胜,客胜";
                }
            }
            catch { return string.Empty; }
        }

        /// <summary>
        /// 传统足球投注编码及显示码
        /// </summary>
        /// <param name="type">玩法类型</param>
        /// <param name="isCode">是否投注编码</param>
        /// <returns>返回投注编码或显示码</returns>
        public static string ANTECODES_CTZQ(string type, string code, bool isCode = false)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(type))
                return string.Empty;
            try
            {
                switch (type)
                {
                    case "t6bqc":
                    case "tr9":
                    case "t14c":
                        return GetAnteCodeDic("3,1,0".Split(','), "胜,平,负".Split(','))[code];
                    //return isCode ? "3,1,0" : "胜,平,负";
                    case "t4cjq":
                        return GetAnteCodeDic("0,1,2,3".Split(','), "0,1,2,3+".Split(','))[code];
                    //return isCode ? "0,1,2,3" : "0,1,2,3+";
                    default:
                        return GetAnteCodeDic("3,1,0".Split(','), "胜,平,负".Split(','))[code];
                        //return isCode ? "3,1,0" : "胜,平,负";
                }
            }
            catch { return string.Empty; }
        }

        #endregion

        #region 查询北京单场最新期号(139)
        public async Task<IActionResult> QueryBJDCIssuse([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var list = new List<object>();
                var result = await _serviceProxyProvider.Invoke<BJDCIssuseInfo>(new Dictionary<string, object>(), "api/Data/QueryBJDCCurrentIssuseInfo");
                //var result = WCFClients.GameIssuseClient.QueryBJDCCurrentIssuseInfo();
                if (result != null)
                {
                    list.Add(
                        new
                        {
                            issuse = result.IssuseNumber,
                            stoptime = DateTime.Parse(result.MinLocalStopTime),
                            servertime = DateTime.Now
                        });
                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询北京单场最新期号成功",
                    MsgId = entity.MsgId,
                    Value = list,
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "获取失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = "获取失败",
                });
            }
        }
        #endregion

        #region 查询首页的焦点新闻(包括最多三条公告)
        public async Task<IActionResult> GetIndexNewsFocus([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                int PageSize = p.PageSize;
                //var RedisKey = RedisKeys.IndexNewsFocus + PageSize;
                //var IndexNewsFocusValue = RedisHelperEx.DB_Other.Get(RedisKey);
                //if (string.IsNullOrEmpty(IndexNewsFocusValue))
                //{
                //1.去获取公告
                var GGCount = 3;
                int Surplus = PageSize - GGCount >= 0 ? PageSize - GGCount : 0;
                GGCount = Surplus == 0 ? PageSize : GGCount;
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("pageIndex", 0);
                param.Add("pageSize", GGCount);
                param.Add("agent", (int)BulletinAgent.Local);
                var noticeList = await _serviceProxyProvider.Invoke<BulletinInfo_Collection>(param, "api/Data/QueryDisplayBulletinCollection");
                var ReturnList = new List<IndexNewsFocusModel>();
                if (noticeList != null && noticeList.BulletinList != null)
                {
                    foreach (var item in noticeList.BulletinList)
                    {
                        var additem = new IndexNewsFocusModel()
                        {
                            Category = "GG",
                            CreateTime = item.CreateTime,
                            Id = item.Id.ToString(),
                            IsRedTitle = true,
                            Title = item.Title
                        };
                        ReturnList.Add(additem);
                    }
                    GGCount = noticeList.BulletinList.Count;
                    Surplus = PageSize - GGCount >= 0 ? PageSize - GGCount : 0;
                }
                else
                {
                    Surplus = PageSize;
                }
                //2.获取焦点新闻
                if (Surplus > 0)
                {
                    Dictionary<string, object> focusParam = new Dictionary<string, object>();
                    focusParam.Add("pageIndex", 0);
                    focusParam.Add("pageSize", Surplus);
                    focusParam.Add("category", "FocusCMS");
                    focusParam.Add("gameCode", "");
                    ArticleInfo_QueryCollection aList = await _serviceProxyProvider.Invoke<ArticleInfo_QueryCollection>(focusParam, "api/Data/QueryArticleList_YouHua");
                    if (aList != null && aList.ArticleList != null)
                    {
                        foreach (var item in aList.ArticleList)
                        {
                            var additem = new IndexNewsFocusModel()
                            {
                                Category = "FocusCMS",
                                CreateTime = item.CreateTime,
                                Id = item.Id.ToString(),
                                IsRedTitle = item.IsRedTitle,
                                Title = item.Title
                            };
                            ReturnList.Add(additem);
                        }
                    }
                }
                //RedisHelperEx.DB_Other.Set(RedisKey, JsonHelper.Serialize(ReturnList), 3 * 60);
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询成功",
                    MsgId = entity.MsgId,
                    Value = ReturnList,
                });
                //}
                //else
                //{
                //    var list = JsonHelper.Deserialize<List<IndexNewsFocusModel>>(IndexNewsFocusValue);
                //    return Json(new LotteryServiceResponse
                //    {
                //        Code = ResponseCode.成功,
                //        Message = "查询成功",
                //        MsgId = entity.MsgId,
                //        Value = list,
                //    });
                //}
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "获取失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = "获取失败",
                });
            }

        }
        #endregion

        //public async Task<IActionResult> TestOne()
        //{
        //    LotteryServiceResponse s = null;
        //    MemoryStream ms = new MemoryStream();
        //    var ss = JsonHelper.Serialize(s);
        //    var a = JsonHelper.Deserialize<LotteryServiceResponse>(ss);
        //    return Json("");
        //}

        public async Task<IActionResult> GetBankValueByNum([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var aliurl = "https://ccdcapi.alipay.com/validateAndCacheCardInfo.json";
                var p = JsonHelper.Decode(entity.Param);
                string cardNo = p.cardNo;
                var R_Url = aliurl + "?_input_charset=utf-8&cardNo=" + cardNo + "&cardBinCheck=true";
                var result = PostManager.Get(R_Url, Encoding.UTF8);
                var obj = JsonHelper.Decode(result);

                object value = new
                {
                    validatedNum = CheckBlankCard.MatchLuhn(cardNo),
                    bank = obj.bank,
                    validated = obj.validated,
                    cardType = obj.cardType,
                    key = obj.key,
                    messages = obj.messages,
                    stat = obj.stat
                };
                //if ((bool)obj.validated == true)
                //{

                //}
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询成功",
                    MsgId = entity.MsgId,
                    Value = value,
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = null,
                });
            }
        }


        public async Task<IActionResult> GetMinWithdraw([FromServices]IServiceProxyProvider _serviceProxyProvider)
        {
            try
            {
                //Dictionary<string, object> param = new Dictionary<string, object>();
                //param.Add("key", "Site.Financial.MinWithDrwaMoney");
                //var config = await _serviceProxyProvider.Invoke<C_Core_Config>(param, "api/Data/QueryCoreConfigByKey");
                decimal RequestMoney = 100;
                var key = "Site.Financial.MinWithDrwaMoney";
                var configvalue = await GetAppConfigByKey(_serviceProxyProvider, key);
                if (!string.IsNullOrEmpty(configvalue))
                {
                    //var minmoney = config.ConfigValue;
                    decimal.TryParse(configvalue, out RequestMoney);
                }
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询成功",
                    MsgId = "",
                    Value = RequestMoney,
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = "",
                    Value = ex.ToGetMessage(),
                });
            }
        }

        /// <summary>
        /// 获取app相关配置
        /// </summary>
        private async Task<string> GetAppConfigByKey([FromServices]IServiceProxyProvider _serviceProxyProvider, string key, string defalutValue = "")
        {
            try
            {
                //1.从redis中取
                //2.取不到则在sql中取
                //3.不为空则存入redis中，3分钟缓存
                //var v = RedisHelperEx.DB_Other.Get(key);
                //if (string.IsNullOrEmpty(v))
                //{
                //var v = string.Empty;
                var param = new Dictionary<string, object>();
                param.Add("key", key);
                var config = await _serviceProxyProvider.Invoke<C_Core_Config>(param, "api/Data/QueryCoreConfigByKey");
                if (config != null)
                {
                    return config.ConfigValue;
                    //RedisHelperEx.DB_Other.Set(key, config.ConfigValue, 3 * 60);
                }
                return defalutValue;
                //if (string.IsNullOrEmpty(v))
                //{
                //    return defalutValue;
                //}
                //}
                //return v;
            }
            catch (Exception)
            {
                return defalutValue;
            }
        }


        #region 查找首页可单关的竞彩足球比赛（最多20场）
        public async Task<IActionResult> QueryQuickBuyJCZQ([FromServices]IServiceProxyProvider _serviceProxyProvider)
        {
            try
            {
                var key = EntityModel.Redis.RedisKeys.Key_JCZQ_Match_Odds_List;
                string reidskey = $"{key}_HHDG1";
                var param = new Dictionary<string, object>();
                param.Add("Key", reidskey);
                var oddlist_jczq = await _serviceProxyProvider.Invoke<List<JCZQ_MatchInfo_WEB>>(param, "api/Data/GetJCZQMatchOddsList_ByRedis");
                //var oddlist_jczq = RedisHelperEx.DB_Match.GetObjs<JCZQ_MatchInfo_WEB>(reidskey);
                if (oddlist_jczq == null)
                {
                    oddlist_jczq = Json_JCZQ.GetJCZQHHDGList();
                }
                var result_jczq = new List<JCZQ_MatchInfo_WEB>();
                if (oddlist_jczq != null && oddlist_jczq.Count > 0)
                {
                    var now = DateTime.Now;
                    foreach (var item in oddlist_jczq)
                    {
                        if (item.State_HHDG.Contains("2") && Convert.ToDateTime(item.FSStopBettingTime) > now && item.NoSaleState_BRQSPF == "0")
                        {
                            result_jczq.Add(item);
                        }
                    }
                }
                if (result_jczq.Count > 20) result_jczq = result_jczq.Take(20).ToList();
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询成功",
                    MsgId = "",
                    Value = result_jczq.Select(p => new
                    {
                        MatcheDateTime = p.MatcheDateTime,
                        MatchData = p.MatchData,
                        MatchId = p.MatchId,
                        MatchNumber = p.MatchNumber,
                        FSStopBettingTime = p.FSStopBettingTime,
                        GuestTeamName = p.GuestTeamName,
                        HomeTeamName = p.HomeTeamName,
                        LeagueColor = p.LeagueColor,
                        LeagueName = p.LeagueName,
                        //LetBall = p.LetBall,
                        MatchIdName = p.MatchIdName,
                        StartDateTime = p.StartDateTime,
                        //FXId = p.FXId,
                        //MatchStopDesc = p.MatchStopDesc,
                        PrivilegesType = p.PrivilegesType,
                        SP_Flat_Odds_BRQ = p.SP_Flat_Odds_BRQ,
                        SP_Lose_Odds_BRQ = p.SP_Lose_Odds_BRQ,
                        SP_Win_Odds_BRQ = p.SP_Win_Odds_BRQ,
                        //State_HHDG = p.State_HHDG
                    }).ToList(),
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "获取失败" + "●" + ex.ToString(),
                    MsgId = "",
                    Value = ex.ToGetMessage(),
                });
            }

        }

        #endregion

        public async Task<IActionResult> QueryGameIssuseInfo_App([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                Dictionary<string, object> param = new Dictionary<string, object>();
                string gameCode = p.GameCode;

                param.Add("key", "Site.GameDelay." + gameCode.ToUpper());
                var configtask = _serviceProxyProvider.Invoke<C_Core_Config>(param, "api/Data/QueryCoreConfigByKey");
                param.Clear();

                param.Add("gameCode", gameCode);
                var gameIssuseInfo = await _serviceProxyProvider.Invoke<Issuse_QueryInfo>(param, "api/Data/QueryCurrentIssuseInfo");

                string DelayTime = string.Empty;
                var config = await configtask;
                if (config != null)
                {
                    DelayTime = config.ConfigValue;
                }
                if (gameIssuseInfo != null && !string.IsNullOrEmpty(DelayTime))
                {
                    var list = new List<object>();
                    DateTime? OpeningTime = null;
                    if (gameCode.ToUpper() == "FC3D" || gameCode.ToUpper() == "PL3")
                    {
                        OpeningTime = gameIssuseInfo.LocalStopTime.Date.AddHours(21).AddMinutes(30);
                    }
                    var LastIssuse = BettingHelper.BuildLastIssuseNumber(gameCode, gameIssuseInfo.IssuseNumber);
                    param.Clear();
                    param.Add("gameCode", gameCode);
                    param.Add("gameType", "");
                    param.Add("issuseNumber", LastIssuse);
                    var theLastIssuseWinNumber = await _serviceProxyProvider.Invoke<WinNumber_QueryInfo>(param, "api/Data/GetWinNumber");
                    list.Add(new
                    {
                        CurrIssuseNumber = gameIssuseInfo.IssuseNumber,
                        LocalStopTime = gameIssuseInfo.LocalStopTime,
                        OfficialStopTime = gameIssuseInfo.OfficialStopTime,
                        DelayTime = DelayTime,
                        ServiceTime = DateTime.Now,
                        OpeningTime = OpeningTime,
                        LastIssuseNumber = LastIssuse,
                        LastIssuseWinNumber = string.IsNullOrEmpty(theLastIssuseWinNumber.WinNumber) ? "---" : theLastIssuseWinNumber.WinNumber
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
                //Log4Log.LogEX(KLogLevel.APIError, "API或服务错误***", ex);
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "业务参数错误" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = "",
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "获取失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = "获取失败",
                });
            }

        }

        public async Task<IActionResult> GetWinNumber([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string gameCode = p.GameCode;
                string gameType = p.GameType;
                string issuseNumber = p.IssuseNumber;
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("gameCode", gameCode);
                param.Add("gameType", string.IsNullOrEmpty(gameType) ? "" : gameType);
                param.Add("issuseNumber", issuseNumber);
                var IssuseWinNumber = await _serviceProxyProvider.Invoke<WinNumber_QueryInfo>(param, "api/Data/GetWinNumber");
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询彩种奖期信息成功",
                    MsgId = entity.MsgId,
                    Value = IssuseWinNumber,
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "获取失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = "获取失败",
                });
            }
        }


        #region 游戏相关接口
        public static string OperatorCode = string.Empty;
        public static string SecretKey = string.Empty;
        public static string PreName = string.Empty;
        public static string GamePassWord = "DJW7389a9";
        public static string GameUrl = string.Empty;
        public static void InitGameParam()
        {
            if (string.IsNullOrEmpty(OperatorCode))
            {
                if (ConfigHelper.AllConfigInfo["GameApi"] != null)
                {
                    OperatorCode = ConfigHelper.AllConfigInfo["GameApi"]["OperatorCode"].ToString();
                    SecretKey = ConfigHelper.AllConfigInfo["GameApi"]["SecretKey"].ToString();
                    PreName = ConfigHelper.AllConfigInfo["GameApi"]["PreName"].ToString();
                    GameUrl = ConfigHelper.AllConfigInfo["GameApi"]["URL"].ToString();
                    //GamePassWord = ConfigHelper.AllConfigInfo["GameApi"]["GamePassWord"].ToString();
                }
                else
                {
                    throw new Exception("配置文件出错");
                }
            }
        }
        /// <summary>
        /// 登录游戏（先注册后登录）
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> LoginGame([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            //var testparam = "";
            try
            {
                InitGameParam();
                var p = JsonHelper.Decode(entity.Param);
                string userToken = p.UserToken;
                string gameCode = p.GameCode;
                int? mggameType = p.mggameType;
                var GameType = "SMG";
                if (mggameType == 1)
                {
                    GameType = "SMF";
                }
                if (string.IsNullOrEmpty(gameCode) || string.IsNullOrEmpty(userToken)) throw new Exception("参数出错");
                string userId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                //var param = new Dictionary<string, object>();
                //param["userId"] = userId;
                //var loginInfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/user/GetLocalLoginByUserId");
                //if (loginInfo == null || string.IsNullOrEmpty(loginInfo.DisplayName)) throw new Exception("获取到用户信息有误");
                //var gameLoginName = PreName + loginInfo.DisplayName;
                var gameLoginName = PreName + userId;
                var pwd = GamePassWord;
                var sign = MD5Helper.UpperMD5($"{OperatorCode}&{pwd}&{gameLoginName}&{SecretKey}");
                var loginParam = new
                {
                    command = "LOGIN",
                    gameprovider = "2",
                    sign = sign,
                    @params = new
                    {
                        username = gameLoginName,
                        operatorcode = OperatorCode,
                        password = pwd,
                        gamecode = gameCode,
                        isfreegame = "true",
                        ismobile = "true",
                        language = "CN",
                        extraparameter = new
                        {
                            type = GameType
                        }
                    }
                }.ToJson();
                //testparam = loginParam;
                var loginResult = PostManager.Post(GameUrl, loginParam, Encoding.UTF8, 30, null, "application/json");
                //var loginResult = PostManager.HttpPost(GameUrl, loginParam, "utf-8");
                if (loginResult.Contains("Bad Request"))
                {
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.失败,
                        Message = loginResult,
                        MsgId = "",
                        Value = "传入参数" + loginParam + "",
                    });
                }
                var jsonLoginResult = JsonHelper.Decode(loginResult);
                if (jsonLoginResult.ErrorCode == 0)
                {
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "查询成功",
                        MsgId = "",
                        Value = new
                        {
                            url = jsonLoginResult.Params.url
                        },
                    });
                }
                else
                {
                    //★
                    throw new Exception("登录失败★" + jsonLoginResult);
                }

            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = "",
                    Value = ex.ToGetMessage(),
                });
            }
        }

        /// <summary>
        /// 获取余额
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetGameBalance([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            var gameresult = "";
            try
            {
                InitGameParam();
                var p = JsonHelper.Decode(entity.Param);
                string userToken = p.UserToken;
                int? mggameType = p.mggameType;
                var GameType = "SMG";
                if (mggameType == 1)
                {
                    GameType = "SMF";
                }
                string userId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                //var param = new Dictionary<string, object>();
                //param["userId"] = userId;
                //var loginInfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/user/GetLocalLoginByUserId");
                //if (loginInfo == null || string.IsNullOrEmpty(loginInfo.DisplayName)) throw new Exception("获取到用户信息有误");
                //var gameLoginName = PreName + loginInfo.DisplayName;
                var gameLoginName = PreName + userId;
                var pwd = GamePassWord;
                var sign = MD5Helper.UpperMD5($"{OperatorCode}&{pwd}&{gameLoginName}&{SecretKey}");
                var strParam = new
                {
                    command = "GET_BALANCE",
                    gameprovider = "2",
                    sign = sign,
                    @params = new
                    {
                        username = gameLoginName,
                        operatorcode = OperatorCode,
                        password = pwd,
                        extraparameter = new
                        {
                            type = GameType
                        }
                    }
                }.ToJson();
                //testparam = strParam;
                var result = PostManager.Post(GameUrl, strParam, Encoding.UTF8, 45, null, "application/json");
                gameresult = result;
                //var result = PostManager.HttpPost(GameUrl, strParam, "utf-8");
                if (result.Contains("Bad Request"))
                {
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.失败,
                        Message = result,
                        MsgId = "",
                        Value = "传入参数" + strParam + "",
                    });
                }
                var jsonResult = JsonHelper.Decode(result);
                if (jsonResult.ErrorCode == 0)
                {
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "查询成功",
                        MsgId = "",
                        Value = new
                        {
                            balance = jsonResult.Params.Balance
                        }
                    });
                }
                else if (jsonResult.ErrorCode == 13)
                {
                    var strCreateParam = new
                    {
                        command = "CREATE_ACCOUNT",
                        gameprovider = "2",
                        sign = sign,
                        @params = new
                        {
                            username = gameLoginName,
                            operatorcode = OperatorCode,
                            password = pwd,
                            extraparameter = new
                            {
                                type = GameType
                            }
                        }
                    }.ToJson();
                    //var createResult = PostManager.HttpPost(GameUrl, strCreateParam, "utf-8");
                    var createResult = PostManager.Post(GameUrl, strCreateParam, Encoding.UTF8, 30, null, "application/json");
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "查询成功",
                        MsgId = "",
                        Value = new
                        {
                            balance = 0
                        }
                    });
                }
                else
                {
                    throw new Exception("查询失败★" + result + ";传入参数" + strParam);
                }
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString() + "|" + gameresult,
                    MsgId = "",
                    Value = ex.ToGetMessage(),
                });
            }
        }

        /// <summary>
        /// 游戏充值(根据mggameType判断，如果是0则是)
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GameRecharge([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            //逻辑
            //1.判断余额是否足够充值
            //2.冻结需要充值的金额，并生成一条游戏充值数据存入游戏交易表中，返回订单号
            //3.充值到游戏平台，提交订单号
            //4.判断返回的数据，如果充值成功则则继续请求转账确认接口（先修改交易表数据）
            //5.返回成功则扣钱，如果充值失败，则返还冻结金额给用户（修改交易表数据）
            var gameresult = "";
            var gamerechargeParam = "";
            try
            {
                InitGameParam();
                var p = JsonHelper.Decode(entity.Param);
                string userToken = p.UserToken;
                decimal money = p.Money;
                int? mggameType = p.mggameType;
                var GameType = "SMG";
                if (mggameType == 1)
                {
                    GameType = "SMF";
                }
                if (money <= 0) throw new Exception("参数错误");
                string userId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                var param = new Dictionary<string, object>();
                param["userId"] = userId;
                var loginInfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/user/GetLocalLoginByUserId");
                if (loginInfo == null || string.IsNullOrEmpty(loginInfo.DisplayName)) throw new Exception("获取到用户信息有误");
                param.Clear();
                param.Add("userId", userId);
                param.Add("money", money);
                param.Add("userDisplayName", loginInfo.DisplayName);
                param.Add("gameType", mggameType == null ? 0 : mggameType.Value);
                var freezeResult = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/data/FreezeGameRecharge");
                if (freezeResult.IsSuccess)
                {
                    var orderId = freezeResult.ReturnValue;
                    var gameLoginName = PreName + userId;
                    var pwd = GamePassWord;
                    var providerSerialNo = "";
                    try //调用第三方接口第一步（过程中有失败则直接返回失败）
                    {
                        var sign = MD5Helper.UpperMD5($"{money.ToString()}&{OperatorCode}&{pwd}&{orderId}&{gameLoginName}&{SecretKey}");
                        var rechargeParam = new
                        {
                            command = "DEPOSIT",
                            gameprovider = "2",
                            sign = sign,
                            @params = new
                            {
                                username = gameLoginName,
                                operatorcode = OperatorCode,
                                password = pwd,
                                serialNo = freezeResult.ReturnValue,
                                amount = money.ToString(),
                                extraparameter = new
                                {
                                    type = GameType
                                }
                            }
                        }.ToJson();
                        gamerechargeParam = "step1.param:" + rechargeParam;
                        var result = PostManager.Post(GameUrl, rechargeParam, Encoding.UTF8, 45, null, "application/json");
                        gameresult = "step1.result:" + result;
                        var jsonResult = JsonHelper.Decode(result);
                        if (jsonResult.ErrorCode == 0)
                        {
                            providerSerialNo = jsonResult.Params.providerSerialNo;
                            param.Clear();
                            param.Add("orderId", orderId);
                            param.Add("isSuccess", true);
                            param.Add("providerSerialNo", providerSerialNo);
                            var pResult = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/data/EndFreezeGameRecharge");
                            return Json(new LotteryServiceResponse
                            {
                                Code = ResponseCode.成功,
                                Message = "充值成功",
                                MsgId = "",
                                Value = "",
                            });
                        }
                        else //失败，返还冻结金额并返回
                        {
                            param.Clear();
                            param.Add("orderId", orderId);
                            param.Add("isSuccess", false);
                            param.Add("providerSerialNo", providerSerialNo);
                            var endResult = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/data/EndFreezeGameRecharge");
                            return Json(new LotteryServiceResponse
                            {
                                Code = ResponseCode.失败,
                                Message = "充值失败" + "●" + "游戏充值第一步参数：" + gamerechargeParam + ";游戏充值第一步返回：" + gameresult,
                                MsgId = "",
                                Value = "",
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        //param.Clear();
                        //param.Add("orderId", orderId);
                        //param.Add("isSuccess", false);
                        //var endResult = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/data/EndFreezeGameRecharge");
                        return Json(new LotteryServiceResponse
                        {
                            Code = ResponseCode.失败,
                            Message = ex.ToGetMessage() + "●" + "游戏充值第一步参数：" + gamerechargeParam + ";游戏充值第一步返回：" + gameresult + ";" + ex.ToString(),
                            MsgId = "",
                            Value = "",
                        });
                    }
                    //try //充值第二步，确认转账，成功则扣钱
                    //{
                    //    //确认转账
                    //    var confirmSign = MD5Helper.UpperMD5($"{OperatorCode}&{pwd}&{providerSerialNo}&{gameLoginName}&{SecretKey}");
                    //    var confirmParam = new
                    //    {
                    //        command = "CHECK_TRANSFER_STATUS",
                    //        gameprovider = "2",
                    //        sign = confirmSign,
                    //        @params = new
                    //        {
                    //            username = gameLoginName,
                    //            operatorcode = OperatorCode,
                    //            password = pwd,
                    //            serialNo = providerSerialNo,
                    //        }
                    //    }.ToJson();
                    //    gamerechargeParam = "step2.param:" + confirmParam;
                    //    var confirmResult = PostManager.Post(GameUrl, confirmParam, Encoding.UTF8, 45, null, "application/json");
                    //    gameresult = "step2.result:" + confirmResult;
                    //    var jsonConfirmResult = JsonHelper.Decode(confirmResult);
                    //    if (jsonConfirmResult.ErrorCode == 0) //确认成功
                    //    {
                    //        param.Clear();
                    //        param.Add("orderId", freezeResult.ReturnValue);
                    //        param.Add("isSuccess", true);
                    //        var endResult = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/data/EndFreezeGameRecharge");
                    //        return Json(new LotteryServiceResponse
                    //        {
                    //            Code = ResponseCode.成功,
                    //            Message = "充值成功",
                    //            MsgId = "",
                    //            Value = ""
                    //        });
                    //    }
                    //    else
                    //    {
                    //        param.Clear();
                    //        param.Add("orderId", freezeResult.ReturnValue);
                    //        param.Add("isSuccess", false);
                    //        var endResult = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/data/EndFreezeGameRecharge");
                    //        return Json(new LotteryServiceResponse
                    //        {
                    //            Code = ResponseCode.成功,
                    //            Message = "充值失败",
                    //            MsgId = "",
                    //            Value = ""
                    //        });
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    return Json(new LotteryServiceResponse
                    //    {
                    //        Code = ResponseCode.失败,
                    //        Message = ex.ToGetMessage() + "●" + "游戏充值第二步参数：" + gamerechargeParam + ";游戏充值第二步返回：" + gameresult + ";" + ex.ToString(),
                    //        MsgId = "",
                    //        Value = "",
                    //    });
                    //}
                }
                else
                {
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.失败,
                        Message = freezeResult.Message,
                        MsgId = "",
                        Value = ""
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + "游戏充值参数：" + gamerechargeParam + ";游戏充值返回：" + gameresult + ";" + ex.ToString(),
                    MsgId = "",
                    Value = ex.ToGetMessage(),
                });
            }
        }

        /// <summary>
        /// 获取游戏大厅列表
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetGameLobby([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            //1.请求注册接口
            try
            {
                //2.获取游戏列表
                var AKey = "AppGameList";
                var AValue = await GetAppConfigByKey(_serviceProxyProvider, AKey);
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询配置成功",
                    MsgId = entity.MsgId,
                    Value = JsonHelper.Deserialize<object>(AValue)
                });
                //}
                //throw new Exception("获取失败★" + result);
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + ex.ToString(),
                    MsgId = "",
                    Value = ex.ToGetMessage(),
                });
            }
        }

        /// <summary>
        /// 游戏提现到余额
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> GameWithdraw([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            ////1.存入游戏交易表中
            ////2.执行游戏提现
            ////3.判断返回数据，如果成功则修改交易表数据，再次请求确认转账接口，并记录，修改交易表数据
            ////4.如果返回失败，则直接修改为失败
            //执行后才插入数据库
            var gameResult = "";
            var gameParam = "";
            try
            {
                InitGameParam();
                var p = JsonHelper.Decode(entity.Param);
                string userToken = p.UserToken;
                decimal money = p.Money;
                int? mggameType = p.mggameType;
                var GameType = "SMG";
                if (mggameType == 1)
                {
                    GameType = "SMF";
                }
                if (money <= 0) throw new Exception("参数错误");
                string userId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                var param = new Dictionary<string, object>();
                param["userId"] = userId;
                var loginInfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/user/GetLocalLoginByUserId");
                if (loginInfo == null || string.IsNullOrEmpty(loginInfo.DisplayName)) throw new Exception("获取到用户信息有误");
                param.Clear();
                param.Add("userId", userId);
                param.Add("money", money);
                param.Add("userDisplayName", loginInfo.DisplayName);
                param.Add("gameType", mggameType == null ? 0 : mggameType.Value);
                var withdrawInfo = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/data/AddGameWithdraw_Step1");
                if (withdrawInfo.IsSuccess)
                {
                    var orderId = withdrawInfo.ReturnValue;
                    var providerSerialNo = "";//第三方订单号
                    var gameLoginName = PreName + userId;//用户名
                    var pwd = GamePassWord;//密码
                    try //调用第三方接口第一步（过程中有失败则直接当做失败）
                    {
                        var sign = MD5Helper.UpperMD5($"{money.ToString()}&{OperatorCode}&{pwd}&{orderId}&{gameLoginName}&{SecretKey}");
                        var withdrawParam = new
                        {
                            command = "WITHDRAW",
                            gameprovider = "2",
                            sign = sign,
                            @params = new
                            {
                                username = gameLoginName,
                                operatorcode = OperatorCode,
                                password = pwd,
                                serialNo = orderId,
                                amount = money.ToString(),
                                extraparameter = new
                                {
                                    type = GameType
                                }
                            }
                        }.ToJson();
                        gameParam = "step1.param:" + withdrawParam;
                        var result = PostManager.Post(GameUrl, withdrawParam, Encoding.UTF8, 45, null, "application/json");
                        gameResult = "step1.result:" + result;
                        var jsonResult = JsonHelper.Decode(result);
                        if (jsonResult.ErrorCode == 0)
                        {
                            providerSerialNo = jsonResult.Params.providerSerialNo;
                            //把订单号update到数据库
                            param.Clear();
                            param.Add("OrderId", orderId);
                            param.Add("IsSuccess", true);
                            param.Add("providerSerialNo", providerSerialNo);
                            var com = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/data/EndAddGameWithdraw");
                            return Json(new LotteryServiceResponse
                            {
                                Code = ResponseCode.成功,
                                Message = "提款成功",
                                MsgId = "",
                                Value = "",
                            });
                        }
                        else
                        {
                            param.Clear();
                            param.Add("OrderId", orderId);
                            param.Add("IsSuccess", false);
                            param.Add("providerSerialNo", providerSerialNo);
                            var com = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/data/EndAddGameWithdraw");
                            return Json(new LotteryServiceResponse
                            {
                                Code = ResponseCode.失败,
                                Message = "提款失败" + "●" + "游戏提款第一步参数：" + gameParam + ";游戏提款第一步返回：" + gameResult,
                                MsgId = "",
                                Value = "",
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(new LotteryServiceResponse
                        {
                            Code = ResponseCode.失败,
                            Message = "提款失败" + "●" + ex.ToString() + "游戏提款第一步参数：" + gameParam + ";游戏提款第一步返回：" + gameResult,
                            MsgId = "",
                            Value = ex.ToGetMessage(),
                        });
                    }
                    //try //调用第三方接口第二步（过程中有失败则返回失败，在任务中继续请求）
                    //{
                    //    //确认转账
                    //    var confirmSign = MD5Helper.UpperMD5($"{OperatorCode}&{pwd}&{providerSerialNo}&{gameLoginName}&{SecretKey}");
                    //    var confirmParam = new
                    //    {
                    //        command = "CHECK_TRANSFER_STATUS",
                    //        gameprovider = "2",
                    //        sign = confirmSign,
                    //        @params = new
                    //        {
                    //            username = gameLoginName,
                    //            operatorcode = OperatorCode,
                    //            password = pwd,
                    //            serialNo = providerSerialNo,
                    //        }
                    //    }.ToJson();
                    //    gameParam += "|step2.param:" + confirmParam;
                    //    var confirmResult = PostManager.Post(GameUrl, confirmParam, Encoding.UTF8, 45, null, "application/json");
                    //    gameResult += "|step2.result:" + confirmResult;
                    //    var jsonConfirmResult = JsonHelper.Decode(confirmResult);
                    //    if (jsonConfirmResult.ErrorCode == 0) //确认
                    //    {
                    //        //确认后发钱
                    //        param.Clear();
                    //        param.Add("OrderId", orderId);
                    //        param.Add("IsSuccess", true);
                    //        var result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/data/EndAddGameWithdraw");
                    //        return Json(new LotteryServiceResponse
                    //        {
                    //            Code = ResponseCode.失败,
                    //            Message = "提款成功",
                    //            MsgId = "",
                    //            Value = "",
                    //        });
                    //    }
                    //    else
                    //    {
                    //        //返回失败
                    //        param.Clear();
                    //        param.Add("OrderId", orderId);
                    //        param.Add("IsSuccess", false);
                    //        var result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "api/data/EndAddGameWithdraw");
                    //        return Json(new LotteryServiceResponse
                    //        {
                    //            Code = ResponseCode.失败,
                    //            Message = "提款失败" + "●" + "游戏提款第二步参数：" + gameParam + ";游戏提款第二步返回：" + gameResult,
                    //            MsgId = "",
                    //            Value = "",
                    //        });
                    //    }
                    //}
                    //catch (Exception)
                    //{
                    //    return Json(new LotteryServiceResponse
                    //    {
                    //        Code = ResponseCode.失败,
                    //        Message = "提款失败" + "●" +  "游戏提款第二步参数：" + gameParam + ";游戏提款第二步返回：" + gameResult,
                    //        MsgId = "",
                    //        Value = "",
                    //    });
                    //}
                }
                else
                {
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.失败,
                        Message = withdrawInfo.Message,
                        MsgId = "",
                        Value = ""
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = ex.ToGetMessage() + "●" + "游戏提款参数：" + gameParam + ";游戏提款返回：" + gameResult + ";" + ex.ToString(),
                    MsgId = "",
                    Value = ex.ToGetMessage(),
                });
            }
        }

        //public async Task<IActionResult> TestCreateAccount()
        //{
        //    InitGameParam();
        //    string gameprovider = "2";
        //    string userName = $"DJW18588515737";
        //    string password = "1";
        //    var oldsign = MD5Helper.UpperMD5($"{OperatorCode}&{password}&{userName}&{SecretKey}");
        //    var obj = new
        //    {
        //        gameprovider = gameprovider,
        //        command = "GET_BALANCE",
        //        sign = oldsign,
        //        @params = new
        //        {
        //            username = userName,
        //            operatorcode = OperatorCode,
        //            password = password,
        //        },
        //    };
        //    string json = obj.ToJson();
        //    string msg = $"{GameUrl}--{json}--{OperatorCode}&{password}&{userName}&{SecretKey}";
        //    try
        //    {
        //        var result = PostManager.Post(GameUrl, json, Encoding.UTF8, 30, null, "application/json");
        //        //var result = PostManager.HttpPost(GameUrl, json, "utf-8");
        //        return Json(new LotteryServiceResponse
        //        {
        //            Code = ResponseCode.成功,
        //            Message = msg + result,
        //            MsgId = "",
        //            Value = msg + result,
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new LotteryServiceResponse
        //        {
        //            Code = ResponseCode.失败,
        //            Message = ex.Message + msg,
        //            MsgId = "",
        //            Value = ex.Message + msg,
        //        });
        //    }
        //}        
        #endregion
    }
}
