using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

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

using Kason.Sg.Core.ApiGateWay.ServiceDiscovery.Implementation;
using Kason.Sg.Core.CPlatform.Address;
using SystemManage.Base.Controllers;
//using Lottery.Service.IModuleServices;

namespace SystemManage.Manage.Controllers
{
    [Area("Manage")]
    public class HomeController : BaseController
    {

        /// <summary>
        /// 获取所用服务的描述 
        /// 调试 http://127.0.0.1:729/api/Home/GetServiceDescriptor?address=127.0.0.1:98
        /// </summary>
        /// <param name="serviceDiscoveryProvider"></param>
        /// <param name="address"></param>
        /// <param name="queryParam"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetAddress([FromServices]IServiceDiscoveryProvider serviceDiscoveryProvider, string address, string queryParam)
        {
            var adlist = await serviceDiscoveryProvider.GetAddressAsync();
            IList<object> list = new List<object>();
            foreach (ServiceAddressModel item in adlist)
            {
                var ip = item.Address as IpAddressModel;
                string str = ip.Ip + ":" + ip.Port;
                var lista = await serviceDiscoveryProvider.GetServiceDescriptorAsync(str);
                list.Add(lista);
            }
            return Json(new { A= adlist,B= list });
        }

        /// <summary>
        /// 通过路由调用服务
        /// </summary>
        /// 调试 http://127.0.0.1:729/api/Home/GetServiceByRouter  api/User/GetUser
        /// <param name="serviceDiscoveryProvider"></param>
        /// <param name="address"></param>
        /// <param name="queryParam"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetServiceByRouter([FromServices]IServiceProxyProvider _serviceProxyProvider, string address= "api/user/GetUserList")
        {
            Dictionary<string, object> model = new Dictionary<string, object>();
            model["userName"] = "userName";
            var sdt = DateTime.Now;
            var result = await _serviceProxyProvider.Invoke<object>(model, address);
            var edt = DateTime.Now;
            return Json(new { result= result, s =sdt.ToString("HH:mm:ss ffff"),e=edt.ToString("HH:mm:ss ffff") });
        }
    }
}
