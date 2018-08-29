using EntityModel.Enum;
using Kason.Sg.Core.CPlatform.Utilities;
using KaSon.FrameWork.Common;
using Lottery.ApiGateway.Model.HelpModel;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Api.Controllers.CommonFilterActtribute
{

    public class ReusltFilterAttribute : Attribute, IActionFilter
    {
        public string cxin = "★";
        public char cyuan = '●';
        //public ReusltFilterAttribute(ILogger<ReusltFilterAttribute> logger){
        //    Console.Write("");

        //}
        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {

        
            var result = context.Result as Microsoft.AspNetCore.Mvc.JsonResult;
            if (result != null)
            {
                string url = context.HttpContext.Request.Path;
                var resp = result.Value as LotteryServiceResponse;
                if (resp != null && resp.Code == ResponseCode.失败 && resp.Message.Contains(cxin))
                {
                    //日志记录
                    //  using ()【】
                    //   Log4Log log4 = new Log4Log();

                    string msg = string.Format("API:{0} \r\n {1}", url, resp.Message);
                    var log = ServiceLocator.GetService<ILogger<ReusltFilterAttribute>>();

                    log.LogError(new Exception(msg), "API或服务错误***");
                    //  Log4Log.LogEX(KLogLevel.APIError, "API或服务错误***", new Exception(msg));

                    var temp = resp.Message.Split(cyuan);
                  //  string st = "系统错误，请重试";
                    string st = temp[0];

                    resp.Message = st;
                 //   resp.Value = "系统错误，请重试";
                    //   Microsoft.AspNetCore.Mvc.JsonResult 
                    // context.Result
                }
                else if (resp.Code == ResponseCode.失败 && !resp.Message.Contains(cxin))
                {
                    var temp = resp.Message.Split(cyuan);
                    //  string url = context.HttpContext.Request.Path;
                    string msg = string.Format("API:{0} \r\n {1}", url, resp.Message);
                    string st = temp[0];
                    resp.Message = st;
                 //   Log4Log.LogEX(KLogLevel.GenError, "用户级别错误***", new Exception(msg));

                    var log = ServiceLocator.GetService<ILogger<ReusltFilterAttribute>>();

                    log.LogWarning(new Exception(msg), "API或服务错误***");
                }
            }

        }

        public string[] AllowSites { get; set; }
        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
            var origin = context.HttpContext.Request.Headers["Origin"].ToString();
            string requestHeaders = context.HttpContext.Request.Headers["Access-Control-Request-Headers"];
            Action action = () =>
            {
                context.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", origin);
                context.HttpContext.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                context.HttpContext.Response.Headers.Add("Access-Control-Request-Headers", "Content-Type");
                // context.HttpContext.Response.Headers.Add("Access-Control-Max-Age", "86400");
                //  context.HttpContext.Response.Headers.Add("Transfer-Encoding", "chunked");
                context.HttpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");

            };
            //Action action = () =>
            //{
            //    context.HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", origin);

            //};
            if (AllowSites != null && AllowSites.Any())
            {
                if (AllowSites[0] == "*")
                {
                    action();
                }
                else if (AllowSites.Contains(origin))
                {
                    action();
                }
            }
            else
            {
                action();
            }



            //var origin = context.HttpContext.Request.Headers["Origin"].ToString();
            ////string requestHeaders = context.HttpContext.Request.Headers["Access-Control-Request-Headers"];
            //if (String.IsNullOrEmpty(origin))
            //{
            //    origin = "*";
            //}

            //context.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", origin);

            //context.HttpContext.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            //context.HttpContext.Response.Headers.Add("Access-Control-Request-Headers", "Content-Type");
            //context.HttpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");


            //Action action = () =>
            //{
            //    context.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", origin);
            //    context.HttpContext.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            //    context.HttpContext.Response.AppendHeader("Access-Control-Request-Headers", "Content-Type");
            //    context.HttpContext.Response.AppendHeader("Access-Control-Allow-Credentials", "true");

            //};
            //Action action = () =>
            //{
            //    context.HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", origin);

            //};
            // action();
            //if (AllowSites != null && AllowSites.Any())
            //{
            //    if (AllowSites[0] == "*")
            //    {
            //        action();
            //    }
            //    else if (AllowSites.Contains(origin))
            //    {
            //        action();
            //    }
            //}


        }
    }
}
