﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Kason.Sg.Core.ApiGateWay;
using Kason.Sg.Core.Codec.MessagePack;
using Kason.Sg.Core.Consul;
using Kason.Sg.Core.Consul.Configurations;
using Kason.Sg.Core.CPlatform;
using Kason.Sg.Core.CPlatform.Utilities;
using Kason.Sg.Core.DotNetty;
using Kason.Sg.Core.ProxyGenerator;
using Kason.Sg.Core.ServiceHosting;
using Kason.Sg.Core.System.Intercept;
using System;
using System.IO;
using KaSon.FrameWork.Common;
using System.Threading.Tasks;
using KaSon.FrameWork.Common.KaSon;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Net;
using EntityModel.CoreModel;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Lottery.ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var sslCert = new X509Certificate2(Path.Combine("", "Certs/localhost.pfx"), "password");
            string hostUrl = ConfigHelper.AllConfigInfo["Host"].ToString();
            string ISConsoleLog = ConfigHelper.AllConfigInfo["ISConsoleLog"].ToString();

            var HttpsConfig = ConfigHelper.AllConfigInfo["HttpsConfig"];
            var host = new WebHostBuilder()
                .UseUrls(hostUrl)
                .UseKestrel(options =>
                {
                    options.Limits.MaxConcurrentConnections = 30000;
                    options.Limits.MaxConcurrentUpgradedConnections = 5000;
                    options.Limits.MaxRequestBodySize = 800 * 1024;
                    options.Limits.MaxResponseBufferSize = 800 * 1024 * 1024;

                    options.Limits.MinRequestBodyDataRate =
                        new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                    options.Limits.MinResponseDataRate =
                        new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                    //options.Listen(IPAddress.Any,int.Parse(HttpsConfig["httpsPost"].ToString()), listenOptions =>
                    //{
                    //    listenOptions.UseHttps(HttpsConfig["pfxFilePath"].ToString(), HttpsConfig["pfxPassword"].ToString());
                    //});
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            ConsoleHelper.Clear();

            host.Run();
            
            
        }
    }
}
