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
using Lottery.Api.Controllers.CommonFilterActtribute;
//using Lottery.Service.IModuleServices;

namespace Lottery.Api.Controllers
{
    /// <summary>
    /// 六合彩
    /// </summary>
    [Area("api")]
    [ReusltFilter]
    public class BJPKController : BaseController
    {


        
        /// <summary>
        /// 六合彩投注
        /// </summary>
        /// <param name="_serviceProxyProvider"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public async Task<IActionResult> Betting([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            CommonActionResult result = new CommonActionResult();
            HK6Sports_BetingInfo p = JsonHelper.Deserialize<HK6Sports_BetingInfo>(entity.Param);
            p.SchemeSource =(int) entity.SourceCode;
            string userToken = p.UserToken;



            decimal totalMoney = p.TotalMoney;
            bool stopAfterBonus = p.winStop;

            decimal redBagMoney = p.redBagMoney;
            if (redBagMoney <= 0)
                redBagMoney = 0;


            if (string.IsNullOrEmpty(totalMoney.ToString()))
            {
                result.Code = 300;
                result.StatuCode = 300;
                result.Message = "投注金额不能为空";
                result.IsSuccess = false;
                return Json(result);
            }
              //  throw new Exception("投注金额不能为空");
            if (string.IsNullOrEmpty(userToken))
            {
                result.Message = "token 不能为空";
                result.Code = 300;
                result.StatuCode = 300;
                return Json(result);
            }
            // throw new Exception("");
            if (totalMoney > 200000)
            {
                result.Code = 300;
                result.StatuCode = 300;
                result.Message = "您的购买金额不能超过20万";
                result.IsSuccess = false;
                return Json(result);
            }
           // throw new Exception("您的购买金额不能超过20万");
          //  var IsSaveOrder = "0";//是否为保存订单，0：不是保存订单；1：保存订单；

            string userid = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
            var param = new Dictionary<string, object>();
            p.userId = userid;
            param.Add("info", p);

             result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "PKBetting/Betting/Betting");
          //  result.ReturnObj = p;
            return Json(result);
        }
     
        public async Task<IActionResult> OrderInfo([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            CommonActionResult result = new CommonActionResult();

            var p = JsonHelper.Decode(entity.Param);
            string userToken = p.UserToken;
            string PageIndex = p.PageIndex;

            if (string.IsNullOrEmpty(userToken))
            {
                result.Message = "token 不能为空";
                result.Code = 300;
                result.StatuCode = 300;
                return Json(result);
            }
            if (string.IsNullOrEmpty(PageIndex))
            {
                result.Message = "PageIndex 不能为空";
                result.Code = 300;
                result.StatuCode = 300;
                return Json(result);
            }
            if (int.Parse(PageIndex)<=0)
            {
                result.Message = "PageIndex 页数必须大于0";
                result.Code = 300;
                result.StatuCode = 300;
                return Json(result);
            }
            string userid = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
            var param = new Dictionary<string, object>();
            param.Add("userId", userid);//laofan
            param.Add("PageIndex", int.Parse(PageIndex));//laofan
            result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "PKDataService/data/OrderInfo");
            //hk6dataservice/data/recharge


            return Json(result);

        }
        public async Task<IActionResult> PlayInfo([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            CommonActionResult result = new CommonActionResult();
           // Console.WriteLine("PlayInfo");

            try
            {
               
                var p = JsonHelper.Decode(entity.Param);
                string userToken = p.UserToken;
                // Console.WriteLine("PlayInfo1111"+ userToken);
                if (string.IsNullOrEmpty(userToken))
                {
                    result.Message = "token 不能为空";
                    result.Code = 300;
                    result.StatuCode = 300;
                    return Json(result);
                }
                // Console.WriteLine("PlayInfo2222222");
                string userid = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                var param = new Dictionary<string, object>();
                //  param.Add("userId", userid);//laofan
                // Console.WriteLine("param2222222");
                result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "PKDataService/data/PlayInfo");
                //hk6dataservice/data/recharge
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.StatuCode = 500;
                result.Value= entity;
                result.ReturnValue = ex.ToString();
                result.Message = "系统错误";
            }
           


            return JsonEx(result);
        }
        // string userToken = KaSon.FrameWork.Common.CheckToken.UserAuthentication.GetUserToken(loginEntity.UserId, loginEntity.LoginName);
         public async Task<IActionResult> PlayCategory([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            CommonActionResult result = new CommonActionResult();
            //Sum(string tokens, string IssueNo, string winNum)
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string UserToken = p.UserToken;
              
                string userid = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(UserToken);
                var param = new Dictionary<string, object>();
                //param.Add("userId", userid);//laofan
              
                //  Console.WriteLine("param2222222");
                result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "PKDataService/data/PlayCategory");

            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.StatuCode = 500;
                result.Message = "系统错误";
                result.Value = entity;
                result.ReturnValue = ex.ToString();
            }

            return Json(result);
        }

        public async Task<IActionResult> CurrentIssuseNo([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            //GetCurrentIssuseNo
            CommonActionResult result = new CommonActionResult();
            //Console.WriteLine("PlayInfo");

            try
            {

                var p = JsonHelper.Decode(entity.Param);
                string userToken = p.UserToken;
                //   Console.WriteLine("PlayInfo1111" + userToken);
                if (string.IsNullOrEmpty(userToken)) {
                    result.Message = "token 不能为空";
                    result.Code =300;
                    result.StatuCode = 300;
                    return Json(result);
                }
                   
              //  Console.WriteLine("PlayInfo2222222");
                string userid = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
                var param = new Dictionary<string, object>();
                //  param.Add("userId", userid);//laofan
              //  Console.WriteLine("param2222222");
                result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "PKDataService/data/GetCurrentIssuseNo");
                //hk6dataservice/data/recharge
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.StatuCode = 500;
                result.Message = "系统错误";
                result.Value = entity;
                result.ReturnValue = ex.ToString();
            }


            return Json(result);
        }

      
        public async Task<IActionResult> HostoryData([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            CommonActionResult result = new CommonActionResult();

            var p = JsonHelper.Decode(entity.Param);
            string userToken = p.UserToken;
            string PageIndex = p.PageIndex;

            if (string.IsNullOrEmpty(userToken))
            {
                result.Message = "token 不能为空";
                result.Code = 300;
                result.StatuCode = 300;
                return Json(result);
            }
            if (string.IsNullOrEmpty(PageIndex))
            {
                result.Message = "PageIndex 不能为空";
                result.Code = 300;
                result.StatuCode = 300;
                return Json(result);
            }
            if (int.Parse(PageIndex) <= 0)
            {
                result.Message = "PageIndex 页数必须大于0";
                result.Code = 300;
                result.StatuCode = 300;
                return Json(result);
            }
            string userid = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(userToken);
            var param = new Dictionary<string, object>();
            param.Add("userId", userid);//laofan
            param.Add("PageIndex", int.Parse(PageIndex));//laofan
            result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "PKDataService/data/HostoryData");
            //hk6dataservice/data/recharge


            return Json(result);
        }
    }
}
