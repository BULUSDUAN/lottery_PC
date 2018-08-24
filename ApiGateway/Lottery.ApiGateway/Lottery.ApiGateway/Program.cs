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
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Net;
using EntityModel.CoreModel;
using System.Collections.Generic;

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
                    options.Limits.MaxRequestBodySize = 500 * 1024;
                    options.Limits.MaxResponseBufferSize= 500*1024 * 1024;
                    
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

            // ConsoleOut.Install();
            //Console.Clear()；
            //ConsoleHelper.Clear();

            //  Issuse_QueryInfo cur = await _serviceProxyProvider.Invoke<Issuse_QueryInfo>(param, "api/Data/QueryCurretNewIssuseInfo");
            //CTZQ  T14C
           

           
            //{"GameCode":"CTZQ","GameType":"T14C","IssuseNumber":"18099","NewVerType":"1"}
            
            

            if (!bool.Parse(ISConsoleLog)) ConsoleHelper.Clear();

            Task.WhenAll(new Task[] {
                CTZQ_BJDC(),
                JCLQ(),
                JCZQ()
            });


            //定时更新最新期号信息 
            //Task.Factory.StartNew(async delegate {
            //    await Task.Delay(1000 * 5);

            //    var _serviceProxyProvider = ServiceLocator.GetService<IServiceProxyProvider>();
            //    Issuse_QueryInfoEX val= await _serviceProxyProvider.Invoke<Issuse_QueryInfoEX>(new Dictionary<string, object>(), "api/Data/QueryCurretNewIssuseInfoList");
            //    HashTableCache.Set_Issuse_QueryInfo(val);// = val;
            //                                             //Console.WriteLine("");
            //                                             //初始化传统采集信息
            //    HashTableCache.Init_CTZQ_Data(val);
            //    HashTableCache.Init_BJDC_Data(val.BJDC_IssuseNumber.IssuseNumber);
            //   //Task.Factory.StartNew(()=> {
            //   //     ;
            //   // });

            //});
            //Task.Factory.StartNew(() => {

            //    HashTableCache.Init_JCLQ_Data();
            //});
            //Task.Factory.StartNew(() => {

            //    HashTableCache.Init_JCZQ_Data("1");
            //    HashTableCache.Init_JCZQ_Data();
            //});
            Console.WriteLine("API 启动...");
            host.Run();

          

        }


        public static async Task CTZQ_BJDC()
        {
            while (true)
            {
                try
                {
                    var _serviceProxyProvider = ServiceLocator.GetService<IServiceProxyProvider>();
                    Issuse_QueryInfoEX val = await _serviceProxyProvider.Invoke<Issuse_QueryInfoEX>(new Dictionary<string, object>(), "api/Data/QueryCurretNewIssuseInfoList");
                    //HashTableCache.Set_Issuse_QueryInfo(val);
                    //HashTableCache.ClearHashTable();
                    HashTableCache.Init_CTZQ_Issuse_Data();
                    HashTableCache.Init_CTZQ_Data(val);
                    HashTableCache.Init_BJDC_Data(val.BJDC_IssuseNumber.IssuseNumber);
                }
                catch (Exception ex)
                {

                }
                await Task.Delay(5000);
            }
        }

        public static async Task JCLQ()
        {
            while (true)
            {
                try
                {
                    HashTableCache.Init_JCLQ_Data();
                }
                catch (Exception ex)
                {

                }
                await Task.Delay(5000);
            }
        }

        public static async Task JCZQ()
        {
            while (true)
            {
                try
                {
                    HashTableCache.Init_JCZQ_Data("1");
                    HashTableCache.Init_JCZQ_Data();
                }
                catch (Exception ex)
                {

                }
                await Task.Delay(5000);
            }
        }
    }
}
