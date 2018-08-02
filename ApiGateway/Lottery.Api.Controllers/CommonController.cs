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
//using Lottery.Service.IModuleServices;

namespace Lottery.Api.Controllers
{
    [Area("api")]
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
        public IActionResult GetAppendBettingDate(LotteryServiceRequest entity)
        {
            var result = new LotteryServiceResponse
            {
                Code = ResponseCode.成功,
                Message = "投注成功",
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
            if (Count<1)
                throw new Exception("追期数必须1期以上");
            // BettingDateHelper
            IList<string> list = new List<string>();
            if (Count == 1) {
                list.Add(IssuseNumber);
            }

            if (Count > 1) {
                int currentMaxDate = BettingDateHelper.GetMaxDate(GameCode);
                if (currentMaxDate == 0) {
                    // throw new Exception("不支持彩种");
                    result.Message = "不支持彩种";
                    result.Value = list;
                    result.Code = ResponseCode.失败;
                    return JsonEx(result);
                   
                }
                list = BettingDateHelper.GetUpdate(IssuseNumber, currentMaxDate, GameCode, Count);
                result.Value = list;
            }

            return JsonEx(result);
        }

    }
}
