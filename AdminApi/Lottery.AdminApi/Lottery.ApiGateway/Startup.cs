using System;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Kason.Sg.Core.ApiGateWay;
using Kason.Sg.Core.ApiGateWay.OAuth.Implementation.Configurations;
using Kason.Sg.Core.Caching.Configurations;
using Kason.Sg.Core.Codec.MessagePack;
//using Kason.Sg.Core.Consul;
//using Kason.Sg.Core.Consul.Configurations;
using Kason.Sg.Core.CPlatform;
using Kason.Sg.Core.CPlatform.Utilities;
using Kason.Sg.Core.DotNetty;
using Kason.Sg.Core.ProxyGenerator;
using Kason.Sg.Core.System.Intercept;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
//using Kason.Sg.Core.Zookeeper;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
//using Kason.Sg.Core.Zookeeper;
//using ZookeeperConfigInfo = Kason.Sg.Core.Zookeeper.Configurations.ConfigInfo;

using ApiGateWayConfig = Kason.Sg.Core.ApiGateWay.AppConfig;

namespace Lottery.ApiGateway
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public IContainer ApplicationContainer { get; private set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(env.ContentRootPath)
              .AddCacheFile("Config/cacheSettings.json", optional: false)
              .AddJsonFile("Config/appsettings.json", optional: true, reloadOnChange: true)
              .AddGatewayFile("Config/gatewaySettings.json", optional: false)
              .AddJsonFile($"Config/appsettings.{env.EnvironmentName}.json", optional: true);
            Configuration = builder.Build();
        }
        /// <summary>
        /// 注册 controller
        /// </summary>
        /// <param name="services"></param>
        private void RegisterController(IServiceCollection services)
        {
            var list = AssemblyHelper.LoadAssembly();
            var feature = new ControllerFeature();
            //services.AddCors();
            services.AddMvc().ConfigureApplicationPartManager(m =>
            {
                //   var feature = new ControllerFeature();
                feature = new ControllerFeature();
                foreach (var item in list)
                {
                    m.ApplicationParts.Add(new AssemblyPart(item));
                }


                // m.ApplicationParts.Add(new AssemblyPart(apicontrollerAssembly));
                m.FeatureProviders.Add(new ControllerFeatureProvider());
                m.PopulateFeature(feature);
                services.AddSingleton(feature.Controllers.Select(t => t.AsType()).ToArray());
            });

        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddCors();//注册跨域
            //services.Configure<CookiePolicyOptions>(option => {
            //    //option.CheckConsentNeeded = context => false;
            //    option.MinimumSameSitePolicy = SameSiteMode.None;
            //});
            RegisterController(services);
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSession();
            return RegisterAutofac(services);
        }
        private IServiceProvider RegisterAutofac(IServiceCollection services)
        {
            var registerConfig = ApiGateWayConfig.Register;
            //string consul = ConfigHelper.AllConfigInfo["ConsulSettings"]["IpAddrs"].ToString();
            //string Token = ConfigHelper.AllConfigInfo["ConsulSettings"]["Token"] != null ? ConfigHelper.AllConfigInfo["ConsulSettings"]["Token"].ToString() : "";


            //var config = new ConfigInfo(consul, reloadOnChange: true);
            //config.Token = Token;
            
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(CustomExceptionFilterAttribute));
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });
            services.AddLogging();

            var builder = new ContainerBuilder();
            builder.Populate(services);

            builder.AddMicroService(option =>
            {
                option.AddClient();
                //  option.AddCache();

                option.AddClientIntercepted(typeof(CacheProviderInterceptor));
                //option.UseConsulManager(config);
                //option.UseZooKeeperManager(new ConfigInfo("127.0.0.1:2181"));
                // if (registerConfig.Provider == RegisterProvider.Consul)
                //else if (registerConfig.Provider == RegisterProvider.Zookeeper)
                //    option.UseZooKeeperManager(new ZookeeperConfigInfo(registerConfig.Address));
                option.UseDotNettyTransport();
                option.AddApiGateWay();
                //option.UseProtoBufferCodec();
                option.UseMessagePackCodec();
                builder.Register(m => new CPlatformContainer(ServiceLocator.Current));
            });
            ServiceLocator.Current = builder.Build();
            return new AutofacServiceProvider(ServiceLocator.Current);

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider svp)
        {
            //loggerFactory.AddConsole();
            //   var log = new Log4NetProvider("Config/log4net.config");

            //loggerFactory.AddProvider(new Log4NetProvider("Config/log4net.config"));

            InitConfigInfo.logFactory = loggerFactory;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/mg/Home/Error");
            }

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials()
                  .AllowAnyOrigin();
            });
            //不使用静态文件
            //var myProvider = new FileExtensionContentTypeProvider();
            ////myProvider.Mappings.Add(".html", "text/plain");
            //app.UseStaticFiles(new StaticFileOptions() { ContentTypeProvider = myProvider });
            //app.UseStaticFiles();

            MyHttpContext.ServiceProvider = svp;
            app.UseSession();
            app.UseMvc(routes =>
            {

                routes.MapRoute(
                 name: "areaRoute",
               template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"

               );
                //routes.MapRoute(
                //    name: "default",
                //    template: "{controller=Home}/{action=Index}/{id?}");



                //routes.MapRoute(
                //"Path",
                //"{*path}",
                //new { controller = "Services", action = "Path" });
            });
        }
    }
}
