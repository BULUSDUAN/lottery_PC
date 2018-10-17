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
using Kason.Sg.Core.CPlatform.Runtime.Client.Address.Resolvers;

namespace app.lottery.site.iqucai.Controllers
{
    public class TestController : Controller
    {
        #region 调用服务使用示例
        public ILog logger = null;
        public IServiceProxyProvider serviceProxyProvider;
        public IAddressResolver addrre;
        public TestController(ILog log, IServiceProxyProvider _serviceProxyProvider, IAddressResolver _addrre)
        {
            serviceProxyProvider = _serviceProxyProvider;
            logger = log;
            addrre = _addrre;

        }
        #endregion
        public ActionResult Index1()
        {
            return Content("Index1Index1Index1Index1");
        }
            /// <summary>
            /// 神单主题活动
            /// </summary>
            public async Task<ActionResult> Index()
        {
            Dictionary<string, object> model = new Dictionary<string, object>();
            model["DicName"] = "123";
            model["ApiDicTypeName"] = "123";
            string serviceId = "BettingLottery.Service.IModuleServices.IBettingService.ReadLog_DicName_ApiDicTypeName";
            var descriptors = await addrre.ResolverEx();
            var descriptor = descriptors.FirstOrDefault(i => i.ServiceDescriptor.Id == serviceId);
            // c.Where(b=>b.Address)
            var str = await serviceProxyProvider.Invoke<object>(model, "api/Betting/ReadLog");
            return Content(str.ToString() + "testtesttesttesttesttest");
        }
    }
}
