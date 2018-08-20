using System;
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
//using Lottery.Service.IModuleServices;

namespace Lottery.Api.Controllers
{
    [Area("api")]
    [ReusltFilter]
    // [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class CommonController : BaseController
    {

        // private readonly IServiceProxyProvider _serviceProxyProvider;
        // private readonly IServiceRouteProvider _serviceRouteProvider;
        //// private readonly IAuthorizationServerProvider _authorizationServerProvider;


        // public HomeController(IServiceProxyProvider serviceProxyProvider,
        //     IServiceRouteProvider serviceRouteProvider
        //     )
        // {
        //     _serviceProxyProvider = serviceProxyProvider;
        //     _serviceRouteProvider = serviceRouteProvider;
        //    // _authorizationServerProvider = authorizationServerProvider;
        // }
        public async Task<IActionResult> GetAppendBettingDate([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var result = new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "",
                    MsgId = entity.MsgId,
                    // Value = returnValue.TrimEnd('~'),
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
                        // throw new Exception("不支持彩种");
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
            //            string json = @"{""records"":[
            //{""desc"":""2元赢千万"",""flag"":""1"",""name"":""ssq""},{""desc"":""天天开奖"",""flag"":""1"",""name"":""3d""},{""desc"":""加奖92%(奖金)"",""flag"":""0"",""name"":""cqssc""},{""desc"":""30选7"",""flag"":""0"",""name"":""qlc""},{""desc"":""暂未开售"",""flag"":""0"",""name"":""jxssc""},{""desc"":""3元赢千万"",""flag"":""1"",""name"":""dlt""},{""desc"":""2元赢500万"",""flag"":""1"",""name"":""qxc""},{""desc"":""2元赢10万"",""flag"":""1"",""name"":""plw""},{""desc"":""天天开奖"",""flag"":""1"",""name"":""pls""},{""desc"":""加奖60%(奖金)"",""flag"":""0"",""name"":""jx11x5""},{""desc"":""每日84期"",""flag"":""1"",""name"":""gd11x5""},{""desc"":""10分钟1期"",""flag"":""1"",""name"":""sd115""},{""desc"":""暂未开售"",""flag"":""0"",""name"":""klpk""},{""desc"":""加奖15% (红包)"",""flag"":""1"",""name"":""14sfc""},{""desc"":""加奖15% (红包)"",""flag"":""1"",""name"":""r9""},{""desc"":""加奖15% (红包)"",""flag"":""1"",""name"":""4cjq""},{""desc"":""加奖15% (红包)"",""flag"":""1"",""name"":""6cb""},{""desc"":""挑战高赔率"",""flag"":""1"",""name"":""bd""},{""desc"":""加奖12%(红包)"",""flag"":""1"",""name"":""jczq""},{""desc"":""加奖15%(红包)"",""flag"":""1"",""name"":""jclq""},{""desc"":""返奖率87%"",""flag"":""1"",""name"":""jczqdg""},{""desc"":""返奖率87%"",""flag"":""1"",""name"":""jclqdg""},{""desc"":""赛事任你挑"",""flag"":""1"",""name"":""bdsf""},{""desc"":""竞猜冠亚军"",""flag"":""1"",""name"":""gyj""}]}";
            //return WebRedisHelper.APP_Advertising;

            var APP_Advertising_Key = "APP_Advertising";
            var APP_Advertising_Value = await GetAppConfigByKey(_serviceProxyProvider, APP_Advertising_Key);
            return Json(new LotteryServiceResponse
            {
                Code = ResponseCode.成功,
                Message = "查询按钮广告成功",
                MsgId = "",
                Value = JsonHelper.Deserialize<object>(APP_Advertising_Value)
            });

        }
        #endregion
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
        public async Task<IActionResult> GameInfoIndex_V2([FromServices]IServiceProxyProvider _serviceProxyProvider)
        {
            //            string json = @"{""records"":[
            //{""desc"":""2元赢千万"",""flag"":""1"",""name"":""ssq""},{""desc"":""天天开奖"",""flag"":""1"",""name"":""3d""},{""desc"":""加奖92%(奖金)"",""flag"":""0"",""name"":""cqssc""},{""desc"":""30选7"",""flag"":""0"",""name"":""qlc""},{""desc"":""暂未开售"",""flag"":""0"",""name"":""jxssc""},{""desc"":""3元赢千万"",""flag"":""1"",""name"":""dlt""},{""desc"":""2元赢500万"",""flag"":""1"",""name"":""qxc""},{""desc"":""2元赢10万"",""flag"":""1"",""name"":""plw""},{""desc"":""天天开奖"",""flag"":""1"",""name"":""pls""},{""desc"":""加奖60%(奖金)"",""flag"":""0"",""name"":""jx11x5""},{""desc"":""每日84期"",""flag"":""1"",""name"":""gd11x5""},{""desc"":""10分钟1期"",""flag"":""1"",""name"":""sd115""},{""desc"":""暂未开售"",""flag"":""0"",""name"":""klpk""},{""desc"":""加奖15% (红包)"",""flag"":""1"",""name"":""14sfc""},{""desc"":""加奖15% (红包)"",""flag"":""1"",""name"":""r9""},{""desc"":""加奖15% (红包)"",""flag"":""1"",""name"":""4cjq""},{""desc"":""加奖15% (红包)"",""flag"":""1"",""name"":""6cb""},{""desc"":""挑战高赔率"",""flag"":""1"",""name"":""bd""},{""desc"":""加奖12%(红包)"",""flag"":""1"",""name"":""jczq""},{""desc"":""加奖15%(红包)"",""flag"":""1"",""name"":""jclq""},{""desc"":""返奖率87%"",""flag"":""1"",""name"":""jczqdg""},{""desc"":""返奖率87%"",""flag"":""1"",""name"":""jclqdg""},{""desc"":""赛事任你挑"",""flag"":""1"",""name"":""bdsf""},{""desc"":""竞猜冠亚军"",""flag"":""1"",""name"":""gyj""}]}";
            //return WebRedisHelper.APP_Advertising;

            var APP_Advertising_Key = "APP_Advertising_V2";
            var APP_Advertising_Value = await GetAppConfigByKey(_serviceProxyProvider, APP_Advertising_Key);
            return Json(new LotteryServiceResponse
            {
                Code = ResponseCode.成功,
                Message = "查询按钮广告成功",
                MsgId = "",
                Value = JsonHelper.Deserialize<object>(APP_Advertising_Value)
            });

        }
        #endregion



        public async Task<IActionResult> GetAppConfig([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                var APP_Common_Key = "APP_Common";
                var APP_UserCenter_Key = "APP_UserCenter";
                var APP_Index_Key = "APP_Index";
                var APP_tuijianyouli_Key = "APP_tuijianyouli";
                var APP_tuijianyoulipid_Key = "APP_tuijianyoulipid";
                var APP_tuijianyoulifxid_Key = "APP_tuijianyoulifxid";
                var APP_shareScheme_Key = "APP_shareScheme";
                var APP_ServicePhone_Key = "Site.Service.Phone";
                var APP_ScoreURL_Key = "APP_ScoreURL";

                var APP_Common_Value = await GetAppConfigByKey(_serviceProxyProvider, APP_Common_Key);
                var APP_UserCenter_Value = await GetAppConfigByKey(_serviceProxyProvider, APP_UserCenter_Key);
                var APP_Index_Value = await GetAppConfigByKey(_serviceProxyProvider, APP_Index_Key);
                var APP_tuijianyouli_Value = await GetAppConfigByKey(_serviceProxyProvider, APP_tuijianyouli_Key);
                var APP_tuijianyoulipid_Value = await GetAppConfigByKey(_serviceProxyProvider, APP_tuijianyoulipid_Key);
                var APP_tuijianyoulifxid_Value = await GetAppConfigByKey(_serviceProxyProvider, APP_tuijianyoulifxid_Key);
                var APP_shareScheme_Value = await GetAppConfigByKey(_serviceProxyProvider, APP_shareScheme_Key);
                var APP_ServicePhone_Value = await GetAppConfigByKey(_serviceProxyProvider, APP_ServicePhone_Key);
                var APP_ScoreURL_Value = await GetAppConfigByKey(_serviceProxyProvider, APP_ScoreURL_Key);
                return Json(new LotteryServiceResponse
                {
                    Code = ResponseCode.成功,
                    Message = "查询配置成功",
                    MsgId = entity.MsgId,
                    Value = new
                    {
                        APP_Common =JsonHelper.Deserialize<object>(APP_Common_Value),
                        APP_UserCenter = JsonHelper.Deserialize<object>(APP_UserCenter_Value),
                        APP_Index = JsonHelper.Deserialize<object>(APP_Index_Value),
                        APP_tuijianyouli = JsonHelper.Deserialize<object>(APP_tuijianyouli_Value),
                        APP_tuijianyoulipid = JsonHelper.Deserialize<object>(APP_tuijianyoulipid_Value),
                        APP_tuijianyoulifxid = JsonHelper.Deserialize<object>(APP_tuijianyoulifxid_Value),
                        APP_shareScheme = JsonHelper.Deserialize<object>(APP_shareScheme_Value),
                        APP_ServicePhone= APP_ServicePhone_Value,
                        APP_ScoreURL = APP_ScoreURL_Value,
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
                    Value="",
                });
            }
        }

        public async Task<IActionResult> PhoneCheckversionNew([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            try
            {
                SchemeSource Type = entity.SourceCode;
                if (Type == SchemeSource.Android)
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
                else if (Type == SchemeSource.Iphone)
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
                    Message = "查询配置失败",
                    MsgId = entity.MsgId,
                    Value = "",
                });
            }
        }
        /// <summary>
        /// 获取app相关配置
        /// </summary>
        private async Task<string> GetAppConfigByKey([FromServices]IServiceProxyProvider _serviceProxyProvider,string key,string defalutValue = "")
        {
            try
            {
                //1.从redis中取
                //2.取不到则在sql中取
                //3.不为空则存入redis中，3分钟缓存
                var flag = KaSon.FrameWork.Common.Redis.RedisHelper.KeyExists(key);
                var v = "";
                if (flag)
                {
                    v = KaSon.FrameWork.Common.Redis.RedisHelper.StringGet(key);
                }
                else
                {
                    var param = new Dictionary<string, object>();
                    param.Add("key", key);
                    var config = await _serviceProxyProvider.Invoke<C_Core_Config>(param, "api/Data/QueryCoreConfigByKey");
                    if (config != null)
                    {
                        v = config.ConfigValue;
                        KaSon.FrameWork.Common.Redis.RedisHelper.StringSet(key, config.ConfigValue, 3 * 60);
                    }
                }
                if (string.IsNullOrEmpty(v))
                {
                    return defalutValue;
                }
                else
                {
                    return v;
                }
            }
            catch (Exception)
            {
                return defalutValue;
            }
        }
    }
}
