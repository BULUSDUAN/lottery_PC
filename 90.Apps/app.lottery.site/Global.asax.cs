using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Common.Log;
using Common.Net;
using Kason.Net.Common;
using Kason.Sg.Core.Codec.MessagePack;
using Kason.Sg.Core.Consul;
using Kason.Sg.Core.CPlatform;
using Kason.Sg.Core.CPlatform.Utilities;
using Kason.Sg.Core.DotNetty;
using Kason.Sg.Core.Log;
using Kason.Sg.Core.ProxyGenerator;

namespace app.lottery.site.iqucai
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {         
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            routes.MapRoute(
             "ZiXun",//资讯
             "ZiXun/{action}",
             new { controller = "ZiXun" });
            routes.MapRoute(
               "onefix",//单关竞彩
               "onefix/{action}",
               new { controller = "onefix" });
            routes.MapRoute(
                "help",//帮助中心
                "help/{action}",
                new { controller = "help" });
            routes.MapRoute(
                "lotteryTrend2", //走势图
                "lotteryTrend/{action}",
                new { controller = "lotteryTrend" }
                );

            routes.MapRoute(
              "lotteryTrend",//走势图
              "lotteryTrend/{action}/{id}",
              new { controller = "lotteryTrend" }
              );

            routes.MapRoute(
              "Demo",//帮助模板
              "Demo/{action}",
              new { controller = "Demo" }
              );
            routes.MapRoute(
              "member",//会员中心
              "member/{action}",
              new { controller = "member" }
              );

            routes.MapRoute(
              "commissio",//推广
              "commissio/{action}",
              new { controller = "commissio" }
              );
            routes.MapRoute(
             "m",//幸运转盘
             "m/{action}",
             new { controller = "m" }
             );
            routes.MapRoute(
                "vip",//vip
                "vip/{action}",
                new { controller = "vip" }
                );
            routes.MapRoute(
                "user",//注册登录
                "user/{action}",
                new { controller = "user" }
                );
            routes.MapRoute(
                "HuoDong",//活动
                "HuoDong/{action}",
                new { controller = "HuoDong" }
                );
            routes.MapRoute(
                "Activity",//活动
                "Activity/{action}",
                new { controller = "Activity" }
                );
            routes.MapRoute(
               "home",//主页
               "home/{action}",
               new { controller = "home" }
               );
            routes.MapRoute(
               "artists",//赢家平台
               "artists/{action}",
               new { controller = "artists" }
               );
            routes.MapRoute(
             "baodan",//神单分享
             "baodan/{action}",
             new { controller = "baodan" }
             );
            routes.MapRoute("lottery",//开奖历史
                "lottery/{action}",
                new { controller = "lottery" });

            routes.MapRoute(
              "StaticHtml",//生成静态页
              "StaticHtml/{action}",
              new { controller = "StaticHtml" }
              );

            routes.MapRoute("Error",
            "Error/{action}",
            new { controller = "Error" });

            routes.MapRoute("Test",
        "Test/{action}",
        new { controller = "Test" });

            routes.MapRoute(
             "common",
             "{action}",
             new { controller = "common" });

            routes.MapRoute(
               "common2",
               "{action}/{id}",
               new { controller = "common" }
               );
            //routes.MapRoute(
            // "Test",
            // "Test/{action}",
            // new { controller = "Test" }
            // );

            routes.MapRoute(
                "Default", // 默认路由名称
                "{controller}/{action}/{id}", // 带有参数的 URL
                new { controller = "home", action = "default", id = UrlParameter.Optional } // 参数默认值
                );
        }

        #region Applications

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            Application_Autofac();
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            RouteData routeData = new RouteData();
            var exception = Server.GetLastError();
            var httpStatusCode = (exception is HttpException) ? (exception as HttpException).GetHttpCode() : 500; //这里仅仅区分两种错误  
            try
            {
                if (exception != null)
                {
                    if (!(exception is HttpException))
                    {
                        var source = Request.Url.AbsolutePath + "WEB";
                        if (!Request.IsLocal)
                        {
                            source += "-" + IpManager.IPAddress;
                        }
                        WriteException(source, exception);
                    }
                }

                switch (httpStatusCode)
                {
                    case 404:
                        routeData.Values.Add("controller", "Error");
                        routeData.Values.Add("action", "Err_404");
                        break;
                    case 500:
                        routeData.Values.Add("controller", "Error");
                        routeData.Values.Add("action", "Err_500");
                        break;
                    default:
                        routeData.Values.Add("controller", "Error");
                        routeData.Values.Add("action", "Err_404");
                        break;
                }
            }
            catch
            {
            }
            finally
            {
                Server.ClearError();

                // Avoid IIS7 getting in the middle
                Response.TrySkipIisCustomErrors = true;

                // Call target Controller and pass the routeData.
                IController errorController = new app.lottery.site.cbbao.Controllers.ErrorController();
                errorController.Execute(new RequestContext(
                     new HttpContextWrapper(Context), routeData));
            }

        }

        #endregion

        protected void Session_Start(object sender, EventArgs e)
        {
            //在用户第一次访问站点时
            Session["RefferUrl"] = Request.UrlReferrer == null ? "" : Request.UrlReferrer.ToString();
        }

        protected void Session_End(object sender, EventArgs e)
        {
            //WriteException("Session_End", new Exception("测试Session过期"));
        }

        private static void WriteException(string source, Exception ex)
        {
            var logWriter = LogWriterGetter.GetLogWriter();
            if (logWriter != null)
            {
                logWriter.Write("Error", source, ex);
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (Context.Request.FilePath == "/")
            {
                Context.RewritePath("index.html");
            }
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
            DependencyResolver.SetResolver(new AutofacDependencyResolver(ServiceLocator.Current));

        }
    }
}