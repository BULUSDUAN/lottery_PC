﻿
using EntityModel;
using Kason.Sg.Core.Caching;
using Kason.Sg.Core.CPlatform;
using Kason.Sg.Core.CPlatform.EventBus.Events;
using Kason.Sg.Core.CPlatform.Filters.Implementation;
using Kason.Sg.Core.CPlatform.Ioc;
using Kason.Sg.Core.CPlatform.Routing.Implementation;
using Kason.Sg.Core.CPlatform.Runtime.Client.Address.Resolvers.Implementation.Selectors.Implementation;
using Kason.Sg.Core.CPlatform.Runtime.Server.Implementation.ServiceDiscovery.Attributes;
using Kason.Sg.Core.CPlatform.Support;
using Kason.Sg.Core.CPlatform.Support.Attributes;
using Kason.Sg.Core.ProxyGenerator.Implementation;
using Kason.Sg.Core.System.Intercept;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace CrawMatch.Service.IModuleServices
{
    /// <summary>
    /// 采集服务
    /// </summary>
    [ServiceBundle("creaw/{Service}")]
    public interface ICrawService: IServiceKey
    {
      

        [Service(Date = "2018-9-3", Director = "kason", Name = "采集，开奖等登陆服务")]
        Task<string> Login(string name);

    }
}