﻿using Autofac;
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

namespace Lottery.Service.Host
{
    public class Program
    {
        static void Main(string[] args)
        {

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var host = new ServiceHostBuilder()
                .RegisterServices(builder =>
                {
                    builder.AddMicroService(option =>
                    {
                        option.AddServiceRuntime()
                        .AddRelateService()
                        .AddConfigurationWatch()
                        //option.UseZooKeeperManager(new ConfigInfo("127.0.0.1:2181"));
                        .UseConsulManager(new ConfigInfo("10.0.3.27:8500",reloadOnChange:true))
                        .UseDotNettyTransport()
                        .UseRabbitMQTransport()
                        .AddRabbitMQAdapt()
                       .AddCache()
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
                .SubscribeAt() 
               // .UseLog4net(LogLevel.Error, "Configs/log4net.config")
                .UseNLog(LogLevel.Error, "Config/NLog.config")
                //.UseServer("127.0.0.1", 98)
                //.UseServer("127.0.0.1", 98，“true”) //自动生成Token
                //.UseServer("127.0.0.1", 98，“123456789”) //固定密码Token
                .UseServer(options =>
                {
                  //  options.IpEndpoint = new IPEndPoint(IPAddress.Any, 98);  
                    //options.Port = 98;
                   // options.Ip = "127.0.0.1";
                    options.Token = "True";
                    options.ExecutionTimeoutInMilliseconds = 30000;
                    options.MaxConcurrentRequests = 200;
                })
                .UseServiceCache()
                .Configure(build =>
                build.AddEventBusFile("eventBusSettings.json", optional: false))
                .Configure(build =>
                build.AddCacheFile("cacheSettings.json", optional: false,reloadOnChange:true))
                  .Configure(build =>
                build.AddCPlatformFile("surgingSettings.json", optional: false, reloadOnChange: true))
                .UseProxy()
                .UseStartup<Startup>()
                .Build();

            using (host.Run())
            {
                Console.WriteLine($"服务端启动成功，{DateTime.Now}。");
            }
        }
    }
}