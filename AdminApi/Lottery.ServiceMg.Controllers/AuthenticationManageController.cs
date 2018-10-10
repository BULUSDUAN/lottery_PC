﻿using Microsoft.AspNetCore.Mvc;
using Kason.Sg.Core.ApiGateWay.ServiceDiscovery;
using Kason.Sg.Core.ApiGateWay.Utilities;
using Kason.Sg.Core.CPlatform.Address;
using Kason.Sg.Core.CPlatform.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace Lottery.ServiceMg.Controllers
{
    [Area("mg")]
    public class AuthenticationManageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> EditServiceToken([FromServices]IServiceDiscoveryProvider serviceDiscoveryProvider, string address)
        {
            var list = await serviceDiscoveryProvider.GetAddressAsync(address); ;
            return View(list.FirstOrDefault());
        }

        [HttpPost]
        public async Task<IActionResult> EditServiceToken([FromServices]IServiceDiscoveryProvider serviceDiscoveryProvider, IpAddressModel model)
        {
           await serviceDiscoveryProvider.EditServiceToken(model);
            return Json(ServiceResult.Create(true));
        }
    }
}
