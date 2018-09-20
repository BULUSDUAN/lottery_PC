using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using app.lottery.site.Controllers;

namespace app.lottery.site.iqucai.Controllers
{
    public class PhoneBuyController : BaseController
    {
        //
        // GET: /PhoneBuy/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 手机购彩   触屏版
        /// </summary>
        /// <returns></returns>
        public ActionResult PhoneBuy()
        {
            return View();
        }

    }
}
