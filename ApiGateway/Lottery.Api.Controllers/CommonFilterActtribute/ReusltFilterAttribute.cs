using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.Api.Controllers.CommonFilterActtribute
{
   
    public class ReusltFilterAttribute : Attribute, IActionFilter
    {
        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {


        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {


            var origin = context.HttpContext.Request.Headers["Origin"].ToString();
            //string requestHeaders = context.HttpContext.Request.Headers["Access-Control-Request-Headers"];
            if (String.IsNullOrEmpty(origin)) {
                origin = "*";
            }

            context.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", origin);
           
            context.HttpContext.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            context.HttpContext.Response.Headers.Add("Access-Control-Request-Headers", "Content-Type");
            context.HttpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");

          
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
