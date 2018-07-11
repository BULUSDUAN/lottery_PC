using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Kason.Sg.Core.ApiGateWay;
using Kason.Sg.Core.ApiGateWay.Configurations;
using Kason.Sg.Core.ApiGateWay.OAuth.Implementation.Configurations;
using Kason.Sg.Core.Caching.Configurations;
using Kason.Sg.Core.Codec.MessagePack;
using Kason.Sg.Core.Consul;
using Kason.Sg.Core.Consul.Configurations;
using Kason.Sg.Core.CPlatform;
using Kason.Sg.Core.CPlatform.Utilities;
using Kason.Sg.Core.DotNetty;
using Kason.Sg.Core.ProxyGenerator;
using Kason.Sg.Core.System.Intercept;
//using Kason.Sg.Core.Zookeeper;
using Kason.Sg.Core.Caching;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Linq;
//using Kason.Sg.Core.Zookeeper;
//using ZookeeperConfigInfo = Kason.Sg.Core.Zookeeper.Configurations.ConfigInfo;

using ApiGateWayConfig = Kason.Sg.Core.ApiGateWay.AppConfig;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Http;
using KaSon.FrameWork.Common.Net;

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
              .AddCacheFile("Configs/cacheSettings.json", optional: false)
              .AddJsonFile("Configs/appsettings.json", optional: true, reloadOnChange: true)
              .AddGatewayFile("Configs/gatewaySettings.json", optional: false)
              .AddJsonFile($"Configs/appsettings.{env.EnvironmentName}.json", optional: true);
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
            RegisterController(services);
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSession();
            return RegisterAutofac(services);
        }
        private IServiceProvider RegisterAutofac(IServiceCollection services)
        {
            var registerConfig = ApiGateWayConfig.Register;
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
                //option.UseZooKeeperManager(new ConfigInfo("127.0.0.1:2181"));
               // if (registerConfig.Provider == RegisterProvider.Consul)
                option.UseConsulManager(new ConfigInfo(registerConfig.Address));
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
            loggerFactory.AddConsole();

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
                var policy = Kason.Sg.Core.ApiGateWay.AppConfig.Policy;
                builder.WithOrigins(policy.Origins);
                if (policy.AllowAnyHeader)
                    builder.AllowAnyHeader();
                if (policy.AllowAnyMethod)
                    builder.AllowAnyMethod();
                if (policy.AllowAnyOrigin)
                    builder.AllowAnyOrigin();
                if (policy.AllowCredentials)
                    builder.AllowCredentials();
            });
            //不使用静态文件
            //var myProvider = new FileExtensionContentTypeProvider();
            //myProvider.Mappings.Add(".tpl", "text/plain");
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
