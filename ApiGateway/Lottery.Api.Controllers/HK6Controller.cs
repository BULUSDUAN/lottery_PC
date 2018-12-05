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
using Kason.Sg.Core.ApiGateWay.ServiceDiscovery.Implementation;
using Kason.Sg.Core.CPlatform.Address;
using EntityModel.CoreModel;
using EntityModel.Enum;
using KaSon.FrameWork.Common.ExceptionEx;
using KaSon.FrameWork.Common.Sport;
using EntityModel;
using EntityModel.Communication;
using KaSon.FrameWork.Common;
//using Lottery.Service.IModuleServices;

namespace Lottery.Api.Controllers
{
    /// <summary>
    /// 六合彩
    /// </summary>
    [Area("api")]
    public class HK6Controller : BaseController
    {

       

     /// <summary>
     /// 六合彩投注
     /// </summary>
     /// <param name="_serviceProxyProvider"></param>
     /// <param name="address"></param>
     /// <returns></returns>
        public async Task<IActionResult> Betting([FromServices]IServiceProxyProvider _serviceProxyProvider,  LotteryServiceRequest entity)
        {
            HK6Sports_BetingInfo p = JsonHelper.Deserialize<HK6Sports_BetingInfo>(entity.Param);
            string userToken = p.UserToken;
           
         
         
            decimal totalMoney = p.TotalMoney;
            bool stopAfterBonus = p.winStop;
           
            decimal redBagMoney = p.redBagMoney;
            if (redBagMoney <= 0)
                redBagMoney = 0;

          
            if (string.IsNullOrEmpty(totalMoney.ToString()))
                throw new Exception("投注金额不能为空");
            if (string.IsNullOrEmpty(userToken))
                throw new Exception("userToken不能为空");
            if (totalMoney > 200000)
                throw new Exception("您的购买金额不能超过20万");
           // var IsSaveOrder = "0";//是否为保存订单，0：不是保存订单；1：保存订单；
          
            string userid = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
            var param = new Dictionary<string, object>();
            param.Add("info", p);

            var result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "HK6Betting/Betting/Betting");

            return Json(result);
        }
        public async Task<IActionResult> ReCharge([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity) {
            CommonActionResult result = new CommonActionResult();
            var p = JsonHelper.Decode(entity.Param);
            string userToken = p.UserToken;
            string userDisplayName = p.userDisplayName;
            decimal Money = p.Money;
            if (string.IsNullOrEmpty(userToken))
                throw new Exception("userToken不能为空");
            string userid = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
            var param = new Dictionary<string, object>();
            param.Add("userId", userid);//laofan
            param.Add("userDisplayName", userDisplayName);
            param.Add("Money", Money);
            
            result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "hk6dataservice/data/recharge");
            //hk6dataservice/data/recharge


            return Json(result);
        }

        public async Task<IActionResult> GameWithdraw([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            CommonActionResult result = new CommonActionResult();
            var p = JsonHelper.Decode(entity.Param);
            string userToken = p.UserToken;
            string userDisplayName = p.userDisplayName;
            decimal Money = p.Money;
            if (string.IsNullOrEmpty(userToken))
                throw new Exception("userToken不能为空");
            string userid = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
            var param = new Dictionary<string, object>();
            param.Add("userId", userid);//laofan
            param.Add("userDisplayName", userDisplayName);
            param.Add("Money", Money);

            result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "hk6dataservice/data/GameWithdraw");
            //hk6dataservice/data/recharge


            return Json(result);
        }

    }
}
