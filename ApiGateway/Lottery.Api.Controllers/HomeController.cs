using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Lottery.Base.Controllers;
using Microsoft.AspNetCore.Mvc;
using Kason.Sg.Core.ApiGateWay;
using Kason.Sg.Core.ApiGateWay.OAuth;
using Kason.Sg.Core.CPlatform;
using Kason.Sg.Core.CPlatform.Filters.Implementation;
using Kason.Sg.Core.CPlatform.Routing;
using Kason.Sg.Core.ProxyGenerator;
using Kason.Sg.Core.ProxyGenerator.Utilitys;
using Kason.Sg.Core.ApiGateWay.ServiceDiscovery;
using Kason.Sg.Core.CPlatform.Utilities;
//using Lottery.Service.IModuleServices;

namespace Lottery.Api.Controllers
{
    [Area("api")]
   // [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class HomeController : BaseController
    {

       // private readonly IServiceProxyProvider _serviceProxyProvider;
       // private readonly IServiceRouteProvider _serviceRouteProvider;
       //// private readonly IAuthorizationServerProvider _authorizationServerProvider;


       // public HomeController(IServiceProxyProvider serviceProxyProvider,
       //     IServiceRouteProvider serviceRouteProvider
       //     )
       // {
       //     _serviceProxyProvider = serviceProxyProvider;
       //     _serviceRouteProvider = serviceRouteProvider;
       //    // _authorizationServerProvider = authorizationServerProvider;
       // }
        public IActionResult Index()
        {
            return JsonEx(new { name="12313" });
        }

        // [HttpPost]

        /// <summary>
        /// 获取所用服务的描述 
        /// 调试 http://127.0.0.1:729/api/Home/GetServiceDescriptor?address=127.0.0.1:98
        /// </summary>
        /// <param name="serviceDiscoveryProvider"></param>
        /// <param name="address"></param>
        /// <param name="queryParam"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetServiceDescriptor([FromServices]IServiceDiscoveryProvider serviceDiscoveryProvider, string address, string queryParam)
        {
           
            var list = await serviceDiscoveryProvider.GetServiceDescriptorAsync(address, queryParam);
            var result = ServiceResult<IEnumerable<ServiceDescriptor>>.Create(true, list);
            return Json(result);
        }

        /// <summary>
        /// 通过路由调用服务
        /// </summary>
        /// 调试 http://127.0.0.1:729/api/Home/GetServiceByRouter  api/User/GetUser
        /// <param name="serviceDiscoveryProvider"></param>
        /// <param name="address"></param>
        /// <param name="queryParam"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetServiceByRouter([FromServices]IServiceProxyProvider _serviceProxyProvider, string address= "api/User/GetLoginUserList")
        {
            Dictionary<string, object> model = new Dictionary<string, object>();
            model["userName"] = "userName";
            var sdt = DateTime.Now;
           // for (int i = 0; i < 10000; i++)
           // {
                var result = await _serviceProxyProvider.Invoke<object>(model, address);
           // }
           var edt = DateTime.Now;
            //var list = await serviceDiscoveryProvider.GetServiceDescriptorAsync(address, queryParam);
            //var result = ServiceResult<IEnumerable<ServiceDescriptor>>.Create(true, list);
            return Json(new { result= result, s =sdt.ToString("HH:mm:ss ffff"),e=edt.ToString("HH:mm:ss ffff") });
        }
        //通过接口协议调用
        /// <summary>
        /// /// 调试 http://127.0.0.1:729/api/Home/GetServiceByFactory
        /// </summary>
        /// <returns></returns>
        //public async Task<IActionResult> GetServiceByFactory()
        //{
        //    Dictionary<string, object> dic = new Dictionary<string, object>();
        //    dic["id"] = "123";
        //    var sdt = DateTime.Now;
        //    var factory = ServiceLocator.GetService<IServiceProxyFactory>();
        //    var userProxy = factory.CreateProxy<IUserService>("User");
        //    var result = await userProxy.GetUserId("1231");
        //    //for (int i = 0; i < 10000; i++)
        //    //{
        //    //   
        //    //}
        //    var edt = DateTime.Now;
        //    //var list = await serviceDiscoveryProvider.GetServiceDescriptorAsync(address, queryParam);
        //    //var result = ServiceResult<IEnumerable<ServiceDescriptor>>.Create(true, list);
        //    return Json(new { s = sdt.ToString("HH:mm:ss ffff"), e = edt.ToString("HH:mm:ss ffff") });
          


        //  //  return Json(result);

        //    //return View();
        //}

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            // return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            return null;
        }
    }
}
