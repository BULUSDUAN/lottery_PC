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

namespace Lottery.Api.Controllers
{
    [Area("api")]
    [ReusltFilter]
    public class DataController : BaseController
    {
        public IActionResult Index()
        {
            return JsonEx(new { name = "12313" });
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
                //var param = System.Web.Helpers.Json.Decode(entity.Param);
                string gameCode = p.GameCode;
                param.Add("gameCode", gameCode);
                var gameIssuseInfo = await _serviceProxyProvider.Invoke<Issuse_QueryInfo>(param, "api/Data/QueryCurrentIssuseInfo");
                //var gameIssuseInfo = WCFClients.GameIssuseClient.QueryCurrentIssuseInfo(param.GameCode);
                //param.Clear();
                //param.Add("key", "Site.GameDelay." + p.GameCode.ToUpper());
                //var config = await _serviceProxyProvider.Invoke<CoreConfigInfo>(param, "api/Data/QueryCoreConfigByKey");
                //var config = WCFClients.GameClient.QueryCoreConfigByKey("Site.GameDelay." + param.GameCode.ToUpper()).ConfigValue;
                //var config = await QueryCurrentIssuseInfo(_serviceProxyProvider, "Site.GameDelay." + p.GameCode.ToUpper());
                //Dictionary<string, object> param = new Dictionary<string, object>();
                param.Clear();
                param.Add("key", "Site.GameDelay." + gameCode.ToUpper());
                var config = await _serviceProxyProvider.Invoke<C_Core_Config>(param, "api/Data/QueryCoreConfigByKey");
                string DelayTime = string.Empty;
                if (config != null)
                {
                    DelayTime = config.ConfigValue;
                }
                if (gameIssuseInfo != null && !string.IsNullOrEmpty(DelayTime))
                {
                    var list = new List<object>();
                    DateTime? OpeningTime = null;
                    if (gameCode.ToUpper() == "FC3D"|| gameCode.ToUpper() == "PL3")
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
                //var param = System.Web.Helpers.Json.Decode(entity.Param);

                foreach (var gameCode in gameCodeArray)
                {
                    param.Clear();
                    param.Add("gameCode", gameCode);
                    var gameInfo = await _serviceProxyProvider.Invoke<LotteryIssuse_QueryInfo>(param, "api/Data/QueryNextIssuseListByLocalStopTime");
                    //var list = WebRedisHelper.QueryNextIssuseListByLocalStopTime(gameCode);
                    //LotteryIssuse_QueryInfo gameInfo;
                    //if (list == null || list.Count <= 0)
                    //    gameInfo=null;
                    //gameInfo= list.Where(p => p.LocalStopTime > DateTime.Now).OrderBy(p => p.OfficialStopTime).FirstOrDefault();
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

                //var list = LoadAllGameIssuse_RefreshByLocalStopTime();
                //foreach (var gameInfo in list)
                //{
                //    result.Add(new
                //    {
                //        GameCode = gameInfo.GameCode,
                //        CurrIssuseNumber = gameInfo.IssuseNumber,
                //        LocalStopTime = ConvertDateTimeInt(gameInfo.LocalStopTime),
                //        OfficialStopTime = ConvertDateTimeInt(gameInfo.OfficialStopTime),
                //        DelayTime = gameInfo.GameDelaySecond,
                //        ServiceTime = ConvertDateTimeInt(DateTime.Now),
                //    });
                //}

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
                //if (currTime > 0)
                //{
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "查询服务器当前时间成功",
                        MsgId = entity.MsgId,
                        Value = currTime,
                    });
                //}
                //return Json(new LotteryServiceResponse
                //{
                //    Code = ResponseCode.失败,
                //    Message = "查询服务器当前时间失败",
                //    MsgId = entity.MsgId,
                //    Value = 0,
                //});
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
                BannerType bannerType = BannerType.APP;
                //if (entity.SourceCode == SchemeSource.Iphone)
                //    bannerType = BannerType.IOS;
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("bannerType", (int)bannerType);
                param.Add("returnRecord", 10);

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
                List<C_Bank_Info> BankList = await _serviceProxyProvider.Invoke<List<C_Bank_Info>>(new Dictionary<string,object>(), "api/Data/GetBankList");
                var returnlist = new List<object>();
                if (BankList != null && BankList.Count > 0)
                {
                    foreach (var item in BankList)
                    {
                        returnlist.Add(new {id=item.Id,value=item.BankCode,name=item.BankName });
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
                Log("116", ex);
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
                Log("116", ex);
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "服务器内部错误，请联系接口提供商",
                    MsgId = entity.MsgId,
                    Value = "",
                });
            }
        }

        //private List<object> GetBankDic()
        //{
        //    //var dic = new Dictionary<string, string>();
        //    //dic.Add("CMB", "招商银行");
        //    //dic.Add("ICBC", "中国工商银行");
        //    //dic.Add("CCB", "中国建设银行");
        //    //dic.Add("BOC", "中国银行");
        //    //dic.Add("COMM", "中国交通银行");
        //    //dic.Add("CITIC", "中信银行");
        //    //dic.Add("CIB", "兴业银行");
        //    //dic.Add("CEBBANK", "中国光大银行");
        //    //dic.Add("CMBC", "中国民生银行");
        //    //dic.Add("ABC", "中国农业银行");
        //    ////dic.Add("SPAB", "平安银行");
        //    //dic.Add("GDB", "广东发展银行");
        //    //dic.Add("SDB", "深圳发展银行");
        //    //dic.Add("BJB", "北京银行");
        //    //dic.Add("SPDB", "上海浦东发展银行");
        //    //dic.Add("SHB", "上海银行");
        //    ////dic.Add("NBBANK", "宁波银行");
        //    ////dic.Add("WZCB", "温州银行");
        //    //dic.Add("CQCB", "重庆银行");

        //    return dic;
        //}
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
                if (entity.Param == null) throw new ArgumentException("请求参数错误！");
                var p = JsonHelper.Decode(entity.Param);
                string gameCode = p.GameCode;
                string gameType = p.GameType;
                string issuseNumber = p.IssuseNumber;
                if (string.IsNullOrEmpty(gameCode) || string.IsNullOrEmpty(gameType) || string.IsNullOrEmpty(issuseNumber))
                    throw new ArgumentException("请求参数错误！");
                Dictionary<string, object> param = new Dictionary<string, object>();
                //param.Add("gameCode", gameCode);
                //param.Add("gameType", gameType);
                //param.Add("issuseNumber", issuseNumber);
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
                    Message = "服务器内部错误，请联系接口提供商",
                    MsgId = entity.MsgId,
                    Value = "",
                });
            }
            //throw new ArgumentException("请求参数错误！");
        }


        #region 一星单选

        public async Task<LotteryServiceResponse> QueryCQSSCCurrNumberOmission_1XDX([FromServices]IServiceProxyProvider _serviceProxyProvider, IDictionary<string, object> param)
        {
            param.Add("index", 1);
            var result = await _serviceProxyProvider.Invoke<CQSSC_1X_ZS>(param, "api/Data/QueryCQSSCCurrNumberOmission_1XDX");
            //var result = WCFClients.ChartClient.QueryCQSSCCurrNumberOmission_1XDX(gameCode + "_" + gameType + "_" + issuseNumber, 1);
            if (result == null)
                throw new Exception("未查询到遗漏数据！");
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
            //var result = WCFClients.ChartClient.QueryCQSSCCurrNumberOmission_2XZX(gameCode + "_" + gameType + "_" + issuseNumber, 1);
            if (result == null)
                throw new Exception("未查询到遗漏数据！");
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
                throw new Exception("未查询到遗漏数据！");
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
                throw new Exception("未查询到遗漏数据！");
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
                throw new Exception("未查询到遗漏数据！");
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
                throw new Exception("未查询到遗漏数据！");
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
                throw new Exception("未查询到遗漏数据！");
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
        ///// <summary>
        ///// 旧接口_不实现
        ///// </summary>
        ///// <param name="entity"></param>
        ///// <returns></returns>
        //public LotteryServiceResponse QueryArticleList(LotteryServiceRequest entity)
        //{
        //    try
        //    {
        //        var p = System.Web.Helpers.Json.Decode(entity.Param);
        //        //热点彩讯  FocusCMS
        //        //赛事点评 Match_Comment
        //        //彩票资讯 Lottery_GameCode
        //        string category = p.Category;
        //        int pageIndex = p.PageIndex;
        //        int pageSize = p.PageSize;
        //        string userToken = this.GuestUserToken;
        //        var resultList = WCFClients.ExternalClient.QueryArticleList("", "", category, pageIndex, pageSize, userToken);
        //        if (resultList != null && resultList.ArticleList.Count > 0)
        //        {
        //            var list = new List<object>();
        //            foreach (var item in resultList.ArticleList)
        //            {
        //                list.Add(new
        //                {
        //                    Id = item.Id,
        //                    Title = item.Title,
        //                    GameCode = item.GameCode,
        //                    Category = item.Category.Trim(),
        //                    //Description = item.Description,
        //                    //Content = item.DescContent,
        //                    CreateTime = ConvertDateTimeInt(item.CreateTime),
        //                });
        //            }
        //            return new LotteryServiceResponse
        //            {
        //                Code = ResponseCode.成功,
        //                Message = "查询文章成功",
        //                MsgId = entity.MsgId,
        //                Value = list,
        //            };
        //        }
        //        return new LotteryServiceResponse
        //        {
        //            Code = ResponseCode.成功,
        //            Message = "查询文章成功",
        //            MsgId = entity.MsgId,
        //            Value = string.Empty,
        //        };
        //    }
        //    catch (ArgumentNullException ex)
        //    {
        //        return new LotteryServiceResponse
        //        {
        //            Code = ResponseCode.失败,
        //            Message = "业务参数错误",
        //            MsgId = entity.MsgId,
        //            Value = ex.Message,
        //        };
        //    }
        //    catch (Exception)
        //    {
        //        return new LotteryServiceResponse
        //        {
        //            Code = ResponseCode.失败,
        //            Message = "查询文章列表失败",
        //            MsgId = entity.MsgId,
        //            Value = string.Empty,
        //        };
        //    }
        //}

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
                    throw new AggregateException("未找到对应文章");
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
                throw new Exception("未查询到文章内容");
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
                    Message = "查询文章明细失败",
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
                switch (category)
                {
                    case "hot"://今日热点
                        param.Add("category", "Lottery_Hot");
                        param.Add("gameCode", "");
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
                                ReadCount = item.ReadCount,
                            });
                        }
                        break;
                    case "gpc"://高频彩
                        param.Add("category", "Lottery_GameCode");
                        param.Add("gameCode", "JX11X5|CQSSC|SD11X5|GD11X5|GDKLSF|JSKS|SDKLPK3");
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
                                ReadCount = item.ReadCount,
                            });
                        }
                        break;
                    case "szc"://数字彩
                        param.Add("category", "Lottery_GameCode");
                        param.Add("gameCode", "SSQ|DLT|PL3|FC3D");
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
                                ReadCount = item.ReadCount,
                            });
                        }
                        break;
                    case "jjc"://竞技彩
                        param.Add("category", "Lottery_GameCode");
                        param.Add("gameCode", "JCZQ|JCLQ|BJDC");
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
                                ReadCount = item.ReadCount,
                            });
                        }
                        break;
                    case "FocusCMS"://焦点新闻
                        param.Add("category", "FocusCMS");
                        param.Add("gameCode", "");
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
                                ReadCount = item.ReadCount,
                            });
                        }
                        break;
                    case "BonusCMS"://焦点新闻
                        param.Add("category", "BonusCMS");
                        param.Add("gameCode", "");
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
                                ReadCount = item.ReadCount,
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
                    Message = "业务参数错误",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                });
            }
            catch (Exception)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询文章列表失败",
                    MsgId = entity.MsgId,
                    Value = string.Empty,
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
                string userToken = await GuestUserToken(_serviceProxyProvider);
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("pageIndex", pageIndex);
                param.Add("pageSize", pageSize);
                param.Add("userToken", userToken);
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
                            //Description = DeleteHtml(item.Content, 25),
                            //Content = item.Content,
                            CreateTime = item.CreateTime,
                        });
                    }
                    //return Json(new LotteryServiceResponse
                    //{
                    //    Code = ResponseCode.成功,
                    //    Message = "查询公告列表成功",
                    //    MsgId = entity.MsgId,
                    //    Value = list,
                    //});
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
                    Message = "业务参数错误",
                    MsgId = entity.MsgId,
                    Value = string.Empty,
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询公告失败",
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
                    throw new ArgumentException("公告编号不能为空");
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
                    Message = "业务参数错误",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询公告详情失败",
                    MsgId = entity.MsgId,
                    Value = string.Empty,
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
                    throw new Exception("意见内容不能为空");
                else if (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(UserName))
                    throw new Exception("请登录后再提交意见");
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
                    Message = "业务参数错误",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "提交意见失败",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
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
                    throw new AggregateException("传入参数错误");
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
                    Message = "业务参数错误",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询配置信息出错",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
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
                    throw new Exception("未查询到当前代理升级信息");
                List<string> contentList = new List<string>();
                var array = configInfo.ConfigUpdateContent.Split('*');
                foreach (var item in array)
                {
                    contentList.Add("* " + item);
                }
                string updateUrl = string.Format("{0}/" + configInfo.AgentName + "_{1}.apk", configInfo.ConfigDownloadUrl, configInfo.ConfigVersion);
                if (entity.SourceCode == EntityModel.Enum.SchemeSource.Iphone)
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
                    Message = "业务参数错误",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询最新的版本号失败",
                    MsgId = entity.MsgId,
                    Value = "无法匹配设备",
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
                throw new Exception("查询嵌套地址失败");
            }
            //catch (ArgumentException ex)
            //{
            //    return new LotteryServiceResponse
            //    {
            //        Code = ResponseCode.失败,
            //        Message = "业务参数错误",
            //        MsgId = entity.MsgId,
            //        Value = ex.Message,
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
                //    Value = ex.Message,
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
                string UserToken = p.UserToken;
                string userId = p.UserId;
                int pageIndex = p.PageIndex;
                int pageSize = p.PageSize;
                if (string.IsNullOrEmpty(UserToken) || string.IsNullOrEmpty(userId))
                    throw new ArgumentException("未获取到有效用户信息");
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
                    param.Add("userToken", UserToken);
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
                    Message = "业务参数错误",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询站内信失败",
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
                    throw new ArgumentException("您还未登录，请先登录！");
                if (string.IsNullOrEmpty(mailId))
                    throw new ArgumentException("站信编号不能为空！");
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("innerMailId", mailId);
                param.Add("userToken", userToken);
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
                    Message = "业务参数错误",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询站内信失败",
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
                var result = await _serviceProxyProvider.Invoke<string>(new Dictionary<string,object>(), "api/Data/QueryRedBagUseConfig");
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
                    Value = "",
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
                    Message = "业务参数错误",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询文章列表失败",
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
                //var Agreement = WebRedisHelper.Agreement_Config;
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
                    Message = ex.Message,
                    MsgId = entity.MsgId,
                    Value = ex.Message,
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
                    Message = ex.Message,
                    MsgId = entity.MsgId,
                    Value = ex.Message,
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
                    throw new Exception("传入游戏类型不能为空");
                //string photourl = ConfigurationManager.AppSettings["ResourceSiteUrl_res"].ToString();
                var photourl = ConfigHelper.ConfigInfo["ResourceSiteUrl_res"].ToString();
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
                    Message = ex.Message,
                    MsgId = entity.MsgId,
                    Value = ex.Message,
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
                    throw new ArgumentException("UserId不能为空");

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
                    Message = "业务参数错误",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
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
                    Message = "业务参数错误",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询活动列表失败",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
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
                    Message = "业务参数错误",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询最新奖期失败",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
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
                    Message = "业务参数错误",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "获取投注内容显示名成功",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
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
                    throw new Exception("彩种不能为空");
                if (string.IsNullOrEmpty(gameType))
                    throw new Exception("玩法不能为空");
                if ((gameCode == "CTZQ" || gameCode == "BJDC") && string.IsNullOrEmpty(issuseNumber))
                    throw new Exception("期号不能为空");

                var matchDataList = new List<object>();
                switch (gameCode)
                {
                    case "CTZQ":
                        //var cur = await _serviceProxyProvider.Invoke<ActivityListInfoCollection>(param, "api/Data/QueryActivInfoList");
                        //var cur = WCFClients.GameIssuseClient.QueryCurretNewIssuseInfo(gameCode, gameType);
                        var issuse = Json_CTZQ.IssuseList(gameType);
                        var theissuse = issuse.FirstOrDefault(c => c.IssuseNumber == issuseNumber);
                        if (theissuse != null)
                        {
                            var now = DateTime.Now;
                            if (Convert.ToDateTime(theissuse.StartTime) > now) break;
                            matchDataList.AddRange(Json_CTZQ.MatchList_WEB(issuseNumber, gameType));
                        }
                        break;
                    case "BJDC":
                        matchDataList.AddRange(Json_BJDC.MatchList_WEB(issuseNumber, gameType));
                        break;
                    case "JCZQ":
                        if (gameType.ToLower() == "hhdg")
                            matchDataList.AddRange(Json_JCZQ.GetJCZQHHDGList());
                        else
                            matchDataList.AddRange(Json_JCZQ.MatchList_WEB(gameType, newVerType));
                        break;
                    case "JCLQ":
                        if (gameType.ToLower() == "hhdg")
                            matchDataList.AddRange(Json_JCLQ.GetJCLQHHDGList());
                        else
                            matchDataList.AddRange(Json_JCLQ.MatchList_WEB(gameType));
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
                    Message = "业务参数错误",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询比赛数据失败",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
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

                list.Add(new { issuse = cur.IssuseNumber, stoptime = startDate != null ? startDate.Value : cur.StartTime });

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
                    Message = "业务参数错误",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询最新期号失败",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
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
                var result = await _serviceProxyProvider.Invoke<BJDCIssuseInfo>(new Dictionary<string,object>(), "api/Data/QueryBJDCCurrentIssuseInfo");
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
                    Message = "业务参数错误",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询北京单场最新奖期失败",
                    MsgId = entity.MsgId,
                    Value = ex.Message,
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
                var obj= JsonHelper.Decode(result);
                object value = new
                {
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
                    Message = "查询失败",
                    MsgId = entity.MsgId,
                    Value = null,
                });
            }
        }
    }
}
