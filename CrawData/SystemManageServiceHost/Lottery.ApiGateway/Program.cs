using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;

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
using Newtonsoft.Json.Linq;
using Lottery.CrawGetters;
using CrawHost;
using Autofac;
using Craw.Service.IModuleServices;

namespace CrawHost
{
    internal class StartGameTypeModel
    {
        /// <summary>
        /// 玩法
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 是否启动
        /// </summary>
        public string IsStart { get; set; }
        public string Desc { get; set; }
        public string Param { get; set; }
        public string Path { get; set; }
    }
    public class Program
    {
        public static void Main(string[] args)
        {
           // string consul = ConfigHelper.AllConfigInfo["ConsulSettings"]["IpAddrs"].ToString();

            JToken CrawSettings = ConfigHelper.AllConfigInfo["CrawSettings"];

            JToken RebbitMqSettings = ConfigHelper.AllConfigInfo["RebbitMqSettings"];
            JToken HostSettings = ConfigHelper.AllConfigInfo["HostSettings"];
            JToken MongoSettings = ConfigHelper.AllConfigInfo["MongoSettings"];
            JToken BonusPoolSetting = ConfigHelper.AllConfigInfo["BonusPoolSetting"];
            JToken Auto_CollectSettings = ConfigHelper.AllConfigInfo["Auto_CollectSettings"];

            string Sports_SchemeJobSeconds = ConfigHelper.AllConfigInfo["Sports_SchemeJobSeconds"].ToString();
            var mongoConfig = new kason.Sg.Core.Mongo.ConfigInfo();
            mongoConfig.connectionString = MongoSettings["connectionString"].ToString();
            mongoConfig.SingleInstance = bool.Parse(MongoSettings["SingleInstance"].ToString());
            mongoConfig.dbName = MongoSettings["dbName"].ToString();

            //初始化数据
            Lottery.CrawGetters.InitConfigInfo.MongoSettings = MongoSettings;

            Lottery.CrawGetters.InitConfigInfo.MongoTableSettings = MongoSettings["TableNamesSettings"];
            Lottery.CrawGetters.InitConfigInfo.BonusPoolSetting = CrawSettings["BonusPoolSettings"];
            Lottery.CrawGetters.InitConfigInfo.MatchSettings = CrawSettings["MatchSettings"];
            ServiceHelper.MatchSettings = CrawSettings["MatchSettings"];
            Lottery.CrawGetters.InitConfigInfo.NumLettory_SleepTimeSpanSettings = CrawSettings["NumLettory_SleepTimeSpanSettings"];

            int tt = 2;
            tt &= ~1;
            //var sslCert = new X509Certificate2(Path.Combine("", "Certs/localhost.pfx"), "password");
            string hostUrl = ConfigHelper.AllConfigInfo["Host"].ToString();
            //string ISConsoleLog = ConfigHelper.AllConfigInfo["ISConsoleLog"].ToString();
            Lottery.CrawGetters.InitConfigInfo.Init(CrawSettings);
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

            //  ConsoleHelper.Clear();

            Task.Factory.StartNew(async delegate
            {
                var obj = await Start_ConfigGameType(Auto_CollectSettings);
            });
            host.Run();


        }

        static void Clear()
        {

            Console.ReadKey(true);
            Console.Clear();
            Clear();
        }
        static async Task<object> Start_ConfigGameType(JToken Auto_CollectSettings)
        {
            string strList = Auto_CollectSettings.ToString();
            var list = new List<StartGameTypeModel>();
            try
            {
                list = JsonHelper.Deserialize<List<StartGameTypeModel>>(strList);
            }
            catch (Exception ex)
            {

                throw;
            }
            // Thread.Sleep(5000);
            try
            {
                try
                {
                   // throw new Exception("");
                    var inumcaw1 = ServiceBuilder.Local.Resolve<INumCrawService>();

                    inumcaw1.UpdateErrorIssue();
                    Console.WriteLine("成功更新错误期号");
                }
                catch (Exception e)
                {
                    Console.WriteLine("更新错误期号,出错" + e.ToString());
                    Environment.Exit(0);

                }
              
              
                //IServiceProxyProvider _serviceProxyProvider = ServiceLocator.Current.Resolve<IServiceProxyProvider>();
                foreach (var item in list)
                {
                    Dictionary<string, object> model = new Dictionary<string, object>();
                    string path = item.Path;
                    // Console.WriteLine(path);
                    if (bool.Parse(item.IsStart.ToString()))//是否启动
                    {
                        if (!string.IsNullOrEmpty(item.Param.ToString()))
                        {
                            var parr = item.Param.ToString().Split(',');
                            foreach (var item1 in parr)
                            {
                                var parr1 = item1.Split('|');
                                model[parr1[0].Trim()] = parr1[1].Trim();
                            }
                        }
                        string temp = "";
                        var inumcaw = ServiceBuilder.Local.Resolve<INumCrawService>();
                        switch (item.Key.ToString())
                        {
                            case "HK6Issuse":
                               temp= await inumcaw.NumLettory_HK6Issuse("HK6");
                                break;
                            case "OpenWinHK6":
                                 temp = await inumcaw.NumLettory_HK6Issuse("OpenWinHK6");
                                break;
                            case "HostoryHK6":
                                 temp = await inumcaw.NumLettory_HK6Issuse("HostoryHK6");
                                break;
                            case "OpenWinPK10":
                                temp = await inumcaw.NumLettory_BJPK("OpenWinPK10");
                                break;
                            default:
                                break;
                        }
                        Console.WriteLine(temp);
                        //try
                        //{


                        //    // Console.WriteLine(path);

                        //    //model["gameName"] = "SSQ";

                        //    await _serviceProxyProvider.Invoke<object>(model, path);
                        //    //byte[] byteArray = System.Text.Encoding.GetEncoding("gb2312").GetBytes(item["Desc"].ToString());

                        //    ///  string aaa2 = System.Text.Encoding.GetEncoding("gb2312").GetString(byteArray);
                        //   // Console.WriteLine(string.Format("{0}{1}", item.Desc.ToString(), " 启动了"));
                        //}
                        //catch (Exception ex)
                        //{
                        //    Console.WriteLine(string.Format("{0}{1}{2}", item.Desc.ToString(), " 启动失败", ex.Message));

                        //}


                        //   ServiceLocator.Current.s<IServiceProxyProvider>()


                    }

                }



            }
            catch (Exception ex)
            {

                throw;
            }

            return Task.FromResult("");

        }


    }
}
