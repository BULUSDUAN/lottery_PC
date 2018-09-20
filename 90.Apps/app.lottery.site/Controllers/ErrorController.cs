using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace app.lottery.site.cbbao.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult Def()
        {
            var httpException = RouteData.Values["httpException"] as Exception;
            ViewBag.ErrorMessage = httpException == null ? "发生了未知异常！" : httpException.ToString();
            return View();
        }
        public ActionResult Err_403()
        {
            return View();
        }
        public ActionResult Err_404()
        {
            return View();
        }
        public ActionResult Err_500()
        {
            return View();
        }

    }
}
