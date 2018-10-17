using Autofac;
using Autofac.Integration.Mvc;
using Kason.Net.Common;
using Kason.Sg.Core.Codec.MessagePack;
using Kason.Sg.Core.Consul;
using Kason.Sg.Core.CPlatform;
using Kason.Sg.Core.CPlatform.Utilities;
using Kason.Sg.Core.DotNetty;
using Kason.Sg.Core.Log;
using Kason.Sg.Core.ProxyGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace LotteryPC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Application_Autofac();
        }
        public void Application_Autofac()
        {
            string consul = ConfigHelper.AllConfigInfo["ConsulSettings"]["IpAddrs"].ToString();
            string Token = ConfigHelper.AllConfigInfo["ConsulSettings"]["Token"] != null ? ConfigHelper.AllConfigInfo["ConsulSettings"]["Token"].ToString() : "";
            var config = new ConfigInfo(consul);
            config.Token = Token;
            var builder = new ContainerBuilder();
            // ILog log = LogManager.GetLogger("Logger");

            //注册您的MVC控制器。 （MvcApplication是Global.asax中类的名称。）
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            //可选：注册需要DI的模型绑定器
            builder.RegisterModelBinders(typeof(MvcApplication).Assembly);
            builder.RegisterModelBinderProvider();

            builder.AddMicroService(option =>
            {
                option.AddClient();


                option.UseLog4();

                // option.AddCache();

                //  option.AddClientIntercepted(typeof(CacheProviderInterceptor));
                //option.UseZooKeeperManager(new ConfigInfo("127.0.0.1:2181"));
                option.UseConsulManager(config);

                //else if (registerConfig.Provider == RegisterProvider.Zookeeper)
                //    option.UseZooKeeperManager(new ZookeeperConfigInfo(registerConfig.Address));
                option.UseDotNettyTransport();
                //  option.AddApiGateWay();
                //option.UseProtoBufferCodec();
                option.UseMessagePackCodec();
                builder.Register(m => new CPlatformContainer(ServiceLocator.Current));
            });
            ServiceLocator.Current = builder.Build();

           // IServiceProxyProvider ser = ServiceLocator.Current.Resolve<IServiceProxyProvider>();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(ServiceLocator.Current));

        }
    }
}
