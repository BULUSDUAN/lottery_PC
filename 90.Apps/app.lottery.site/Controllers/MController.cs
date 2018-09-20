using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using app.lottery.site.Controllers;

namespace app.lottery.site.cbbao.Controllers
{
    public class MController : BaseController
    {
        //
        // GET: /M/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 幸运转盘
        /// </summary>
        /// <returns></returns>
        public ActionResult Lucky()
        {
            try
            {
                ViewBag.CurrentUser = CurrentUser;
                //ViewBag.QueryJoinLuckDraw = WCFClients.ActivityClient.QueryJoinLuckDraw(100);
            }
            catch (Exception)
            {
                ViewBag.CurrentUser = null;
                ViewBag.QueryJoinLuckDraw = null;
            }

            return View();
        }
    }
}
