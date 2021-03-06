﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Lottery.Base.Controllers;
using Microsoft.AspNetCore.Mvc;
using Kason.Sg.Core.ApiGateWay;
using Kason.Sg.Core.ApiGateWay.OAuth;
using Kason.Sg.Core.CPlatform;
using Kason.Sg.Core.CPlatform.Filters.Implementation;
using Kason.Sg.Core.CPlatform.Routing;
using Kason.Sg.Core.ProxyGenerator;
using Kason.Sg.Core.ProxyGenerator.Utilitys;
using Kason.Sg.Core.ApiGateWay.ServiceDiscovery;
using Kason.Sg.Core.CPlatform.Utilities;
using Lottery.ApiGateway.Model.HelpModel;
using KaSon.FrameWork.Common.KaSon;
using KaSon.FrameWork.Common;
using EntityModel.Enum;
using Lottery.Api.Controllers.CommonFilterActtribute;
using EntityModel;
using KaSon.FrameWork.Common.ExceptionEx;
using EntityModel.CoreModel;
using System.IO;
using System.Text;
//using Lottery.Service.IModuleServices;

namespace Lottery.Api.Controllers
{
    [Area("api")]
    [ReusltFilter]
    public class CommonController : BaseController
    {
        public async Task<IActionResult> GetAppendBettingDate([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var result = new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "",
                    MsgId = entity.MsgId,
                };
                var p = JsonHelper.Decode(entity.Param);
                string IssuseNumber = p.IssuseNumber;
                string GameCode = p.GameCode;
                int Count = p.Count;

                if (string.IsNullOrEmpty(IssuseNumber))
                    throw new Exception("当前期号不能为空");
                if (string.IsNullOrEmpty(GameCode))
                    throw new Exception("彩票类型不能为空");
                if (Count < 1)
                    throw new Exception("追期数必须1期以上");
                // BettingDateHelper
                IList<string> list = new List<string>();
                if (Count == 1)
                {
                    list.Add(IssuseNumber);
                }
                var MainGameCode = new List<string>() { "SSQ", "DLT", "FC3D", "PL3" };
                if (Count > 1)
                {
                    int currentMaxDate = BettingDateHelper.GetMaxDate(GameCode);
                    if (currentMaxDate == 0)
                    {
                        result.Message = "不支持彩种";
                        result.Value = list;
                        result.Code = ResponseCode.失败;
                        return JsonEx(result);
                    }
                    if (MainGameCode.Contains(GameCode.ToUpper()))
                    {
                        Dictionary<string, object> param = new Dictionary<string, object>();
                        param.Add("gameCode", GameCode.ToUpper());
                        param.Add("currIssueNumber", IssuseNumber);
                        param.Add("issueCount", Count);
                        list = await _serviceProxyProvider.Invoke<List<string>>(param, "api/Data/GetMaxIssueByGameCode");
                    }
                    else
                    {
                        list = BettingDateHelper.GetUpdate(IssuseNumber, currentMaxDate, GameCode, Count);
                    }
                    result.Value = list;
                }

                return JsonEx(result);
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "获取追期期号失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage(),
                });
            }
        }


        #region 按钮上的广告
        /// <summary>
        /// 按钮上的广告
        /// 重庆时时彩     加奖92%  ★★★奖金★★★
        /// 竞猜足球  加奖12%(红包)
        /// 江西11选5，加奖60% ★★奖金★★
        /// 竞猜篮球 加奖15%(红包) 
        /// 胜负彩 加奖15% (红包)
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GameInfoIndex([FromServices]IServiceProxyProvider _serviceProxyProvider)
        {
            //var APP_Advertising_Key = "APP_Advertising_V2";
            //var APP_Advertising_Value = await GetAppConfigByKey(_serviceProxyProvider, APP_Advertising_Key);
            //if (string.IsNullOrEmpty(APP_Advertising_Value))
            //{
            //    var APP_AdList = JsonHelper.Deserialize<APP_Advertising>(APP_Advertising_Value);
            //    var 
            //}
            try
            {
                var GameAdList = await _serviceProxyProvider.Invoke<List<APP_Advertising>>(new Dictionary<string, object>(), "api/Data/GetGameInfoIndex");
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询按钮广告成功",
                    MsgId = "",
                    Value = GameAdList
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询按钮广告失败" + "●" + ex.ToString(),
                    MsgId = "",
                    Value = ex.ToGetMessage()
                });
            }
        }
        #endregion


        public async Task<IActionResult> GetAPP_tuijianyouli([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string userToken = p.UserToken;
                if (string.IsNullOrEmpty(userToken))
                    throw new Exception("参数不能为空");
                string userid = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["userId"] = userid;
                var bindInfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/user/GetLocalLoginByUserId");
                var key = "";
                if (bindInfo != null && bindInfo.IsAgent)
                {
                    key = "APP_tuijianyoulipid";
                }
                else
                {
                    key = "APP_tuijianyoulifxid";
                }
                var value = await GetAppConfigByKey(_serviceProxyProvider, key);
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询配置成功",
                    MsgId = entity.MsgId,
                    Value = JsonHelper.Deserialize<object>(value)
                });

            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询配置失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage()
                });
            }
        }


        public async Task<IActionResult> GetAPP_shareScheme([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string userToken = p.UserToken;
                if (string.IsNullOrEmpty(userToken))
                    throw new Exception("参数不能为空");
                string userid = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["userId"] = userid;
                var bindInfo = await _serviceProxyProvider.Invoke<LoginInfo>(param, "api/user/GetLocalLoginByUserId");
                var key = "";
                if (bindInfo != null && bindInfo.IsAgent)
                {
                    key = "APP_shareScheme_Pid";
                }
                else
                {
                    key = "APP_shareScheme_Fxid";
                }
                var value = await GetAppConfigByKey(_serviceProxyProvider, key);
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询配置成功",
                    MsgId = entity.MsgId,
                    Value = JsonHelper.Deserialize<object>(value)
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询配置失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage()
                });
            }
        }


        public async Task<IActionResult> GetAppConfig([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var APP_Common_Key = "APP_Common";
                var APP_UserCenter_Key = "APP_UserCenter";
                var APP_Index_Key = "APP_Index";
                var APP_ServicePhone_Key = "Site.Service.Phone";
                var APP_ScoreURL_Key = "APP_ScoreURL";
                var APP_ExternalLinks_Key = "APP_ExternalLinks";
                var APP_RechargeActivityUrl_Key = "APP_RechargeActivityUrl_Key";
                var APP_NormalShareable_Key = "APP_NormalShareable";
                var APP_Common_Value = await GetAppConfigByKey(_serviceProxyProvider, APP_Common_Key);
                var APP_UserCenter_Value = await GetAppConfigByKey(_serviceProxyProvider, APP_UserCenter_Key);
                var APP_Index_Value = await GetAppConfigByKey(_serviceProxyProvider, APP_Index_Key);
                var APP_ServicePhone_Value = await GetAppConfigByKey(_serviceProxyProvider, APP_ServicePhone_Key);
                var APP_ScoreURL_Value = await GetAppConfigByKey(_serviceProxyProvider, APP_ScoreURL_Key);
                var APP_ExternalLinks_Value = await GetAppConfigByKey(_serviceProxyProvider, APP_ExternalLinks_Key);
                var APP_RechargeActivityUrl_Value = await GetAppConfigByKey(_serviceProxyProvider, APP_RechargeActivityUrl_Key);
                var APP_NormalShareable_Value = await GetAppConfigByKey(_serviceProxyProvider, APP_NormalShareable_Key);
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询配置成功",
                    MsgId = entity.MsgId,
                    Value = new
                    {
                        APP_Common = JsonHelper.Deserialize<object>(APP_Common_Value),
                        APP_UserCenter = JsonHelper.Deserialize<object>(APP_UserCenter_Value),
                        APP_Index = JsonHelper.Deserialize<object>(APP_Index_Value),
                        APP_ServicePhone = APP_ServicePhone_Value,
                        APP_ScoreURL = APP_ScoreURL_Value,
                        APP_ExternalLinks = JsonHelper.Deserialize<object>(APP_ExternalLinks_Value),
                        APP_RechargeActivityUrl = APP_RechargeActivityUrl_Value,
                        APP_NormalShareable = APP_NormalShareable_Value
                    },
                });
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询配置失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = "",
                });
            }
        }

        public async Task<IActionResult> PhoneCheckversionNew([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                SchemeSource Type = entity.SourceCode;
                if (Type == SchemeSource.NewAndroid)
                {
                    var AKey = "ANDRIOD_PhoneCheckversionNew_V2";
                    var AValue = await GetAppConfigByKey(_serviceProxyProvider, AKey);
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "查询配置成功",
                        MsgId = entity.MsgId,
                        Value = JsonHelper.Deserialize<object>(AValue)
                    });
                }
                else if (Type == SchemeSource.NewIphone)
                {
                    var IKey = "IOS_PhoneCheckversionNew_V2";
                    var IValue = await GetAppConfigByKey(_serviceProxyProvider, IKey);
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "查询配置成功",
                        MsgId = entity.MsgId,
                        Value = JsonHelper.Deserialize<object>(IValue)
                    });
                }
                else
                {
                    return Json(new LotteryServiceResponse
                    {
                        Code = ResponseCode.成功,
                        Message = "查询配置成功",
                        MsgId = entity.MsgId,
                        Value = ""
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "查询配置失败" + "●" + ex.ToString(),
                    MsgId = entity.MsgId,
                    Value = ex.ToGetMessage()
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
                //var flag = KaSon.FrameWork.Common.Redis.RedisHelperEx.KeyExists(key);
                //var v = "";
                //if (flag)
                //{
                //    v = KaSon.FrameWork.Common.Redis.RedisHelperEx.StringGet(key);
                //}
                //if (string.IsNullOrEmpty(v))
                //{
                //    var param = new Dictionary<string, object>();
                //    param.Add("key", key);
                //    var config = await _serviceProxyProvider.Invoke<C_Core_Config>(param, "api/Data/QueryCoreConfigByKey");
                //    if (config != null)
                //    {
                //        v = config.ConfigValue;
                //        KaSon.FrameWork.Common.Redis.RedisHelperEx.StringSet(key, config.ConfigValue, 3 * 60);
                //    }
                //    if (string.IsNullOrEmpty(v))
                //    {
                //        return defalutValue;
                //    }
                //}
                //return v;
                var param = new Dictionary<string, object>();
                param.Add("key", key);
                var config = await _serviceProxyProvider.Invoke<C_Core_Config>(param, "api/Data/QueryCoreConfigByKey");
                if (config != null)
                {
                    return config.ConfigValue;
                }
                return defalutValue;
            }
            catch (Exception)
            {
                return defalutValue;
            }
        }

        /// <summary>
        /// Kson 获取日志
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="SerName"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetTimeLog([FromServices]IServiceProxyProvider _serviceProxyProvider, string SerName = "Order", string FileName = "")
        {
            string config = "";
            if (SerName.ToLower() == "api")
            {

                config = KaSon.FrameWork.Common.Utilities.FileHelper.GetLogInfo("Log_Log\\APITimeInfo", "LogTime_");
            }
            else
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("FileName", FileName);
                config = await _serviceProxyProvider.Invoke<string>(param, "api/" + SerName + "/ReadSqlTimeLog");
            }
            config = string.IsNullOrEmpty(config) ? "没有数据" : config;
            return Content(config);
        }
        /// <summary>
        /// 校验银行卡号
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IActionResult> CheckBlank([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            var result = new LotteryServiceResponse
            {
                Code = ResponseCode.成功,
                Message = "银行卡号格式正确",
                MsgId = entity.MsgId,
                Value =true
            };
            var p = JsonHelper.Decode(entity.Param);
            string CardNum = p.CardNum;
            if (string.IsNullOrEmpty(CardNum))
            {
                result.Message = "银行卡号不能为空";
                result.Code = ResponseCode.失败;
                result.Value = false;
            }
            else {
               var bol= KaSon.FrameWork.Common.Utilities.CheckBlankCard.MatchLuhn(CardNum);
                if (!bol)
                {
                    result.Message = "银行卡号格式不正确";
                    result.Code = ResponseCode.失败;
                    result.Value = false;
                
                }
                else {
                    result.Message = "银行卡号格式正确";
                    result.Code = ResponseCode.成功;
                    result.Value = true;

                }
            }
            
                  return Json(result);
        }

            /// <summary>
            /// PlamtType=1 默认度api 日志,2读服务日志,
            /// </summary>
            /// <param name="_serviceProxyProvider"></param>
            /// <param name="PlamtType"></param>
            /// <param name="SerName"></param>
            /// <param name="FileName"></param>
            /// <returns></returns>
            public async Task<IActionResult> GetLogInfo([FromServices]IServiceProxyProvider _serviceProxyProvider,int PlamtType=1, int DicType = 1, string SerName="Order",string FileName="")
        {
            string config = "";
            string DicTypeName = "APILogError";
            string ApiDicTypeName = "Error";
            switch (DicType)
            {
                case 1:
                 //   config = KaSon.FrameWork.Common.Utilities.FileHelper.GetLogInfo("Log_Log\\APILogError", "");
                    DicTypeName = "APILogError";
                    ApiDicTypeName = "Error";
                    break;
                case 2:
                   // config = KaSon.FrameWork.Common.Utilities.FileHelper.GetLogInfo("Log_Log\\APITimeInfo", "");
                    DicTypeName = "APITimeInfo";
                    ApiDicTypeName = "Info";
                    break;
                case 3:
                    // config = KaSon.FrameWork.Common.Utilities.FileHelper.GetLogInfo("Log_Log\\SevTimeIoginfo", "");
                    DicTypeName = "SevTimeIoginfo";
                    ApiDicTypeName = "Debug";
                    break;
                case 4:
                    DicTypeName = "UserLogError";
                    //  config = KaSon.FrameWork.Common.Utilities.FileHelper.GetLogInfo("Log_Log\\UserLogError", "");
                    ApiDicTypeName = "Warn";
                    break;
                case 5:
                    DicTypeName = "RedisTimeInfoLog";
                   // config = KaSon.FrameWork.Common.Utilities.FileHelper.GetLogInfo("Log_Log\\RedisTimeInfoLog", "");
                    break;
                case 6:
                    DicTypeName = "SQLInfo";
                    ApiDicTypeName = "Fatal";
                    // config = KaSon.FrameWork.Common.Utilities.FileHelper.GetLogInfo("Log_Log\\RedisTimeInfoLog", "");
                    break;
                //SevTimeIoginfoUserLogError
                default:
                    break;
            }
            StringBuilder sb = new StringBuilder();

            if (PlamtType == 1)
            {
                sb.Append( KaSon.FrameWork.Common.Utilities.FileHelper.GetLogInfo("Log_Log\\"+ DicTypeName, ""));
                sb.Append("新的日志******************\r\n");
                sb.Append("新的日志******************\r\n");
                sb.Append("新的日志******************\r\n");
                sb.Append("新的日志******************\r\n");
                if (!string.IsNullOrEmpty(FileName))
                {
                    sb.Append(KaSon.FrameWork.Common.Utilities.FileHelper.GetLogInfoByDate("Log_Log\\" + ApiDicTypeName, FileName));
                }
                else
                {
                    sb.Append(KaSon.FrameWork.Common.Utilities.FileHelper.GetLogInfo("Log_Log\\" + ApiDicTypeName, ""));
                }
                
            }
            else {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["DicName"] = DicTypeName;
                param["ApiDicTypeName"] = ApiDicTypeName;
                sb.Append(await _serviceProxyProvider.Invoke<string>(param, "api/Betting/ReadLog"));
            }
            //if (SerName.ToLower() == "api")
            //{


            //}
            //else
            //{
            //   
            //    param.Add("FileName", FileName);
            //    config = await _serviceProxyProvider.Invoke<string>(param, "api/" + SerName + "/ReadSqlTimeLog");
            //}
            //config = string.IsNullOrEmpty(config) ? "没有数据" : config;
            //  Response.ContentType =;
            // "text/html; charset=UTF-8"
            return Content(sb.ToString());
        }


        public async Task<IActionResult> GetAllConfigValue([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            var p = JsonHelper.Decode(entity.Param);
            var userName = p.userName;
            if (userName != "xgadmin")
            {
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.失败,
                    Message = "不存在此接口",
                    MsgId = entity.MsgId
                });
            }
            var bindInfo = await _serviceProxyProvider.Invoke<LoginInfo>(new Dictionary<string, object>(), "api/betting/GetAllConfigValue");
            return Json(new LotteryServiceResponse
            {
                Code = ResponseCode.成功,
                Message = "查找成功",
                MsgId = entity.MsgId,
                Value = bindInfo
            });
        }
    }
}
