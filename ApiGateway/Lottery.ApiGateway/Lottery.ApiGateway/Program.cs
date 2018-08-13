using Microsoft.AspNetCore.Hosting;
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

namespace Lottery.ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string hostUrl = ConfigHelper.AllConfigInfo["Host"].ToString();
            var host = new WebHostBuilder()
                .UseUrls(hostUrl)
                .UseKestrel(options =>
                {
                    options.Limits.MaxConcurrentConnections = 30000;
                    options.Limits.MaxConcurrentUpgradedConnections = 2000;
                    options.Limits.MaxRequestBodySize = 50 * 1024;
                    options.Limits.MinRequestBodyDataRate =
                        new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                    options.Limits.MinResponseDataRate =
                        new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            // ConsoleOut.Install();
            //Console.Clear()；
            //ConsoleHelper.Clear();


            host.Run();

        }
    }
}
