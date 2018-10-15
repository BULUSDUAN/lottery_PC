using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using app.lottery.site.Controllers;
using Common.Utilities;
using System.Text.RegularExpressions;
using Kason.Sg.Core.ProxyGenerator;
using log4net;
using System.Threading.Tasks;

namespace app.lottery.site.iqucai.Controllers
{
    public class TestController : BaseController
    {
        #region 调用服务使用示例
        private readonly ILog logger = null;
        private readonly IServiceProxyProvider serviceProxyProvider;
        public TestController(IServiceProxyProvider _serviceProxyProvider, ILog log)
        {
            serviceProxyProvider = _serviceProxyProvider;
            logger = log;

        }
        #endregion
        /// <summary>
        /// 神单主题活动
        /// </summary>
        public async Task<ActionResult> Index()
        {
            logger.Info("记录日志！！！");

            Dictionary<string, object> model = new Dictionary<string, object>();
            model["id"] = "123";
            var str = await serviceProxyProvider.Invoke<object>(model, "apiUsers/User/GetUserName");
            return View();
        }
    }
}
