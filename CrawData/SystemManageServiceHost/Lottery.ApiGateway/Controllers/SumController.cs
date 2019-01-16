using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Kason.Sg.Core.CPlatform;
using Kason.Sg.Core.CPlatform.Filters.Implementation;
using Kason.Sg.Core.CPlatform.Routing;

using Kason.Sg.Core.CPlatform.Utilities;
using Lottery.ApiGateway.Model.HelpModel;

using Kason.Sg.Core.CPlatform.Address;
using EntityModel.CoreModel;
using EntityModel.Enum;
using KaSon.FrameWork.Common.ExceptionEx;
using KaSon.FrameWork.Common.Sport;
using EntityModel;
using EntityModel.Communication;
using KaSon.FrameWork.Common;
using Craw.Service.IModuleServices;

//using Lottery.Service.IModuleServices;

namespace CrawHost.Controllers
{
    /// <summary>
    /// 六合彩
    /// </summary>
    [Area("manage")]

    public class SumController
    {



        public async Task<IActionResult> HKSum([FromServices]INumCrawService NumService, LotteryServiceRequest entity)
        {
            CommonActionResult result = new CommonActionResult();
            //Sum(string tokens, string IssueNo, string winNum)
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string UserToken = p.UserToken;
                string IssueNo = p.winIssueNo;
                string winNum = p.winNum;
                string userid = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication_Admin(UserToken);
                var param = new Dictionary<string, object>();
                param.Add("userId", userid);//laofan
                param.Add("IssueNo", IssueNo);//laofan
                param.Add("winNum", winNum);//laofan

                bool bol = true;
                if (!winNum.Contains("|"))
                {
                    bol = false;
                }
                else {
                    var arr = winNum.Split('|');
                    string one = arr[0];
                    string two = arr[1];
                    if (arr.Length != 2)
                    {
                        bol = false;
                    }
                   
                }

                if (!bol)
                {
                    result.IsSuccess = false;
                    result.Code = 300;
                    result.StatuCode = 300;
                    result.Message = "号码格式错误";
                    result.Value = entity;
                    return new JsonResult(result);
                }
               
                // result.ReturnValue = ex.ToString();
               
                // NumService.NumLettory_HK6Issuse
                //  Console.WriteLine("param2222222");
                //   result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "HK6WinSum/WinSum/Sum");
                result = NumService.NumLettory_Sum(userid, IssueNo, winNum);
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.StatuCode = 500;
                result.Message = "系统错误";
                result.Value = entity;
                result.ReturnValue = ex.ToString();
            }

            return new JsonResult (result);
        }

        public async Task<IActionResult> BJPKSum([FromServices]INumCrawService NumService, LotteryServiceRequest entity)
        {
            CommonActionResult result = new CommonActionResult();
            //Sum(string tokens, string IssueNo, string winNum)
            try
            {
                var p = JsonHelper.Decode(entity.Param);
                string UserToken = p.UserToken;
                string IssueNo = p.winIssueNo;
                string winNum = p.winNum;
                string userid = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication_Admin(UserToken);
                var param = new Dictionary<string, object>();
                param.Add("userId", userid);//laofan
                param.Add("IssueNo", IssueNo);//laofan
                param.Add("winNum", winNum);//laofan

                // NumService.NumLettory_HK6Issuse
                //  Console.WriteLine("param2222222");
                //   result = await _serviceProxyProvider.Invoke<CommonActionResult>(param, "HK6WinSum/WinSum/Sum");
                result = NumService.NumLettory_BJPKSum(userid, IssueNo, winNum);
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.StatuCode = 500;
                result.Message = "系统错误";
                result.Value = entity;
                result.ReturnValue = ex.ToString();
            }

            return new JsonResult(result);
        }
    }
}
