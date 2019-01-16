using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Linq;
//using Kason.Sg.Core.Zookeeper;
//using ZookeeperConfigInfo = Kason.Sg.Core.Zookeeper.Configurations.ConfigInfo;


using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Http;
using KaSon.FrameWork.Common.Net;
using Kason.Sg.Core.Log4net;
using KaSon.FrameWork.Common;
using Craw.Service.IModuleServices;
using Craw.Service.ModuleServices;
using CrawHost;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace CrawHost
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public IContainer ApplicationContainer { get; private set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(env.ContentRootPath)
             // .AddCacheFile("Config/cacheSettings.json", optional: false)
              .AddJsonFile("Config/appsettings.json", optional: true, reloadOnChange: true)
            //  .AddGatewayFile("Config/gatewaySettings.json", optional: false)
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
           // services.AddCors();
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
           // return services;
        }
        private IServiceProvider RegisterAutofac(IServiceCollection services)
        {
            services.AddLogging();

           // var ser = services.BuildServiceProvider();
            services.AddMvc() .AddJsonOptions(options =>
            {
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });
            //  var log = ser.GetRequiredService<ILogger<NumCrawService>>();
            // var log1 = ser.GetRequiredService<ILogger<CrawRepository>>();
            string constr = "";
            string dbname = "";
            try
            {
                JToken MongoSettings = ConfigHelper.AllConfigInfo["MongoSettings"];

                constr = MongoSettings["connectionString"].ToString();
                dbname = MongoSettings["dbName"].ToString();
            }
            catch (Exception)
            {

                Console.WriteLine("Mongo配置文件节点出错");
            }
            //services.AddTransient(b => { },);

            //var ser = services.BuildServiceProvider();
            //return ser;
            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.Register(b =>
            {
               
                return new MongoClient(constr).GetDatabase(dbname);
            }).As<IMongoDatabase >();
            builder.RegisterType<CrawRepository>().As<CrawRepository>();
            builder.RegisterType<NumCrawService>().As<INumCrawService>();
            //builder.Register(b =>
            //{
            //    CrawRepository mb = null;
            //    b.TryResolve<CrawRepository>(out mb);
            //    return new NumCrawService(log,mb);
            //}).As<INumCrawService>();

            //builder.RegisterType(typeof(NumCrawService)).As(typeof(INumCrawService)).SingleInstance();


            ServiceBuilder.Local = builder.Build();
            return new AutofacServiceProvider(ServiceBuilder.Local);

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider svp)
        {
            //loggerFactory.AddConsole();
            //   var log = new Log4NetProvider("Config/log4net.config");
            ServiceBuilder.LoggerFactory = loggerFactory;
          //  Lottery.CrawGetters.InitConfigInfo.logFactory = loggerFactory;
            loggerFactory.AddProvider(new Log4NetProvider("Config/log4net.config"));

            Lottery.CrawGetters.InitConfigInfo.logFactory = loggerFactory;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/mg/Home/Error");
            }
           // app.UseCors(builder => builder.WithOrigins("*"));
            //app.UseCors(builder =>
            //{
            //    var policy = Kason.Sg.Core.ApiGateWay.AppConfig.Policy;
            //    builder.WithOrigins(policy.Origins);
            //    if (policy.AllowAnyHeader)
            //        builder.AllowAnyHeader();
            //    if (policy.AllowAnyMethod)
            //        builder.AllowAnyMethod();
            //    if (policy.AllowAnyOrigin)
            //        builder.AllowAnyOrigin();
            //    if (policy.AllowCredentials)
            //        builder.AllowCredentials();
            //});
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
