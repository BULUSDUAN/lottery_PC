using Kason.Sg.Core.ProxyGenerator;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LotteryPC.Controllers
{
    public class HomeController : Controller
    {
        #region 调用服务使用示例
        private readonly ILog logger = null;
        private readonly IServiceProxyProvider serviceProxyProvider;
        public HomeController(ILog log, IServiceProxyProvider _serviceProxyProvider)
        {
            serviceProxyProvider = _serviceProxyProvider;
            logger = log;

        }
        #endregion
        public async Task<ActionResult> Index()
        {
            logger.Info("记录日志！！！");

            Dictionary<string, object> model = new Dictionary<string, object>();
            model["DicName"] = "123";
            model["ApiDicTypeName"] = "123";
            var str = await serviceProxyProvider.Invoke<object>(model, "api/Betting/ReadLog");
            return Content("testtesttesttesttesttest");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}