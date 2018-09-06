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
using SystemManage.Api.Filter;
//using Lottery.Service.IModuleServices;

namespace SystemManage.Craw.Controllers
{
    [Area("Manage")]
    [ReusltFilter]
    public class UserController : BaseController
    {

        public async Task<IActionResult> GetAddress([FromServices]IServiceDiscoveryProvider serviceDiscoveryProvider, string address, string queryParam)
        {
         
         
            return Json(new {  });
        }

        
    }
}
