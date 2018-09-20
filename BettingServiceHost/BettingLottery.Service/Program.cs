using Autofac;
using Microsoft.Extensions.Logging;
using Kason.Sg.Core.Caching;
using Kason.Sg.Core.Caching.Configurations;
using Kason.Sg.Core.Codec.MessagePack;
using Kason.Sg.Core.Consul;
using Kason.Sg.Core.Consul.Configurations;
using Kason.Sg.Core.CPlatform;
using Kason.Sg.Core.CPlatform.Configurations;
using Kason.Sg.Core.CPlatform.Utilities;
using Kason.Sg.Core.DotNetty;

//using Kason.Sg.Core.EventBusKafka;
using Kason.Sg.Core.EventBusRabbitMQ;
using Kason.Sg.Core.Log4net;
using Kason.Sg.Core.Nlog;
using Kason.Sg.Core.ProxyGenerator;
using Kason.Sg.Core.ServiceHosting;
using Kason.Sg.Core.ServiceHosting.Internal.Implementation;
using System;
using System.Text;
using Kason.Sg.Core.EventBusRabbitMQ.Configurations;
using KaSon.FrameWork.Common;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using KaSon.FrameWork.ORM.Provider;
using KaSon.FrameWork.ORM.Helper;
using KaSon.FrameWork.ORM.Helper.AutoTask;
using KaSon.FrameWork.Common.KaSon;

namespace BettingLottery.Service.Host
{

    public class Program
    {
        static void Main(string[] args)
        {

            string consul = ConfigHelper.AllConfigInfo["ConsulSettings"]["IpAddrs"].ToString();
            string Token = ConfigHelper.AllConfigInfo["ConsulSettings"]["Token"].ToString();

            JToken RebbitMqSettings = ConfigHelper.AllConfigInfo["RebbitMqSettings"];
            JToken HostSettings = ConfigHelper.AllConfigInfo["HostSettings"];
            string Sports_SchemeJobSeconds = ConfigHelper.AllConfigInfo["Sports_SchemeJobSeconds"].ToString();
            var config = new ConfigInfo(consul, reloadOnChange: true);
            config.Token = Token;
            //JToken ORMSettings = ConfigHelper.AllConfigInfo["ORMSettings"];
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var host = new ServiceHostBuilder()
               // .CaptureStartupErrors(true),
                .RegisterServices(builder =>
                {
                    builder.AddMicroService(option =>
                    {
                        option.AddServiceRuntime()
                        .AddRelateService()
                        .AddConfigurationWatch()
                        //option.UseZooKeeperManager(new ConfigInfo("127.0.0.1:2181"));
                        .UseConsulManager(config)
                        .UseDotNettyTransport()
                        //.UseRabbitMQTransport()
                        //.AddRabbitMQAdapt()

                       // .AddCache()
                        //.UseKafkaMQTransport(kafkaOption =>
                        //{
                        //    kafkaOption.Servers = "127.0.0.1";
                        //    kafkaOption.LogConnectionClose = false;
                        //    kafkaOption.MaxQueueBuffering = 10;
                        //    kafkaOption.MaxSocketBlocking = 10;
                        //    kafkaOption.EnableAutoCommit = false;
                        //})
                        //.AddKafkaMQAdapt()
                        //.UseProtoBufferCodec()
                        .UseMessagePackCodec();
                        builder.Register(p => new CPlatformContainer(ServiceLocator.Current));
                    });
                })
                //.SubscribeAt()
               // .UseLog4net(LogLevel.Error, "Config/log4net.config")
               // .UseNLog(LogLevel.Error, "Config/NLog.config")
               .UseLog4net("Config/log4net.config")
          
                .UseServer(options =>
                {
                  
                    options.Token = "True";
                    options.ExecutionTimeoutInMilliseconds = 30000;

                    options.MaxConcurrentRequests = 2000;
                })
                // .UseServiceCache()
                //.Configure(build =>
                //build.AddEventBusJson(RebbitMqSettings))
                .Configure(build =>
                //build.AddCacheFile("cacheSettings.json", optional: false, reloadOnChange: true))
                //  .Configure(build =>
                build.AddCPlatformJSON(HostSettings))
                .UseProxy()
                .UseStartup<Startup>()
                .Build();



            //var list = JsonHelper.Deserialize<List<KaSon.FrameWork.ORM.OrmConfigInfo>>(ORMSettings.ToString());
            //DbProvider.InitConfigJson(list);
          
            // InitConfigInfo.logFactory = ServiceLocator.GetService<ILoggerProvider>();
          


            using (host.Run())
            {
                #region 初始化配置
                InitConfigInfo.logFactory  = ServiceLocator.GetService<ILoggerFactory>();
               
                #endregion

                Console.WriteLine($"服务端启动成功，{DateTime.Now}。");
                AutoTaskServices.AutoCaheData(int.Parse(Sports_SchemeJobSeconds));
            }
            //初始化内存期号 k_todo，可用彩种类型,执行一次
            LotteryGameManager lotGm = new LotteryGameManager();
            lotGm.StartInitData();

            //清空打印

            ConsoleHelper.Clear();

            //这个要保留，认主线程一直运行
            Console.ReadLine();


        }
    }

}

