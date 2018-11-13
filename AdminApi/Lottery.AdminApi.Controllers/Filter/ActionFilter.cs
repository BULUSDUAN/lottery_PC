using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Microsoft.AspNetCore.Mvc.Filters;
using Kason.Sg.Core.CPlatform.Filters.Implementation;
using KaSon.FrameWork.Common;
using EntityModel.CoreModel;
using Microsoft.AspNetCore.Mvc;
using EntityModel.Enum;
using Lottery.AdminApi.Model.HelpModel;
using KaSon.FrameWork.Common.CheckToken;

namespace Lottery.AdminApi.Controllers
{
    /// <summary>
    /// Action执行Filter
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class ActionFilterAttribute : FilterAttribute, IActionFilter, IResultFilter
    {
        #region IActionFilter 成员

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //step2

            //throw new NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //step1


            //if (filterContext.HttpContext.Session["CurrentUser"] == null)
            //    filterContext.Result = new RedirectResult(string.Format("/Home/Login?backurl={0}", filterContext.HttpContext.Request.Url.ToString()));
            //throw new NotImplementedException();        
        }

        #endregion

        #region IResultFilter 成员

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            //step4

            //throw new NotImplementedException();
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            //step3

            //throw new NotImplementedException();
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class CheckLoginAttribute : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //throw new NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //if (filterContext.HttpContext.Session.GetObj<CurrentUserInfo>("CurrentUser") == null)
            //    filterContext.Result = new JsonResult(new LotteryServiceResponse
            //    {
            //        Code = AdminResponseCode.失败,
            //        Message = "请先登录后再进行操作"
            //    });
            //filterContext.HttpContext.Request.Host +
            //获取到请求的地址
            try
            {
                var url = filterContext.HttpContext.Request.PathBase + filterContext.HttpContext.Request.Path;
                var param = filterContext.HttpContext.Request.Form["param"];
                var p = JsonHelper.Decode(param);
                string userToken = p.userToken;
                if (string.IsNullOrEmpty(userToken))
                {
                    filterContext.Result = new JsonResult(new LotteryServiceResponse
                    {
                        Code = AdminResponseCode.未登录,
                        Message = "请先登录后再进行操作"
                    });
                }
                var userId = UserAuthentication.ValidateAuthentication_Admin(userToken);
                var flag = BaseController.UserIsLogin(userId);
                if (!flag)
                {
                    filterContext.Result = new JsonResult(new LotteryServiceResponse
                    {
                        Code = AdminResponseCode.未登录,
                        Message = "登陆超时，请重新登陆"
                    });
                }
            }
            catch (Exception ex)
            {
                filterContext.Result = new JsonResult(new LotteryServiceResponse
                {
                    Code = AdminResponseCode.未登录,
                    Message = ex.Message
                });
            }
        }
    }
    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    //public class CheckErrorAttribute : FilterAttribute, IExceptionFilter
    //{
    //    public void OnException(ExceptionContext filterContext)
    //    {
    //        //filterContext.Result = new RedirectResult(string.Format("/Home/ErrorEx?error={0}", filterContext.Exception.Message));
    //        filterContext.HttpContext.Response.Write(filterContext.Exception.Message.Replace("\r\n", "<br />"));
    //        filterContext.HttpContext.Response.Write("<br />");
    //        filterContext.HttpContext.Response.Write("<br />");
    //        filterContext.HttpContext.Response.Write(filterContext.Exception.ToString().Replace("\r\n", "<br />"));
    //        filterContext.ExceptionHandled = true;
    //    }
    //}
}
