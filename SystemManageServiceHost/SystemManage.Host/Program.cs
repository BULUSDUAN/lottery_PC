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
using kason.Sg.Core.Mongo;
using MongoDB.Driver;
using Lottery.CrawGetters;
using System.Threading.Tasks;
using System.Threading;

namespace SystemManage.Host
{

    public class Program
    {
        public IMongoDatabase MDB { get; set; }
        static void Main(string[] args)
        {

            string consul = ConfigHelper.AllConfigInfo["ConsulSettings"]["IpAddrs"].ToString();

            JToken CrawSettings = ConfigHelper.AllConfigInfo["CrawSettings"];

            JToken RebbitMqSettings = ConfigHelper.AllConfigInfo["RebbitMqSettings"];
            JToken HostSettings = ConfigHelper.AllConfigInfo["HostSettings"];
            JToken MongoSettings = ConfigHelper.AllConfigInfo["MongoSettings"];
            JToken BonusPoolSetting = ConfigHelper.AllConfigInfo["BonusPoolSetting"];
            JToken Auto_CollectSettings = ConfigHelper.AllConfigInfo["Auto_CollectSettings"];

            string Sports_SchemeJobSeconds = ConfigHelper.AllConfigInfo["Sports_SchemeJobSeconds"].ToString();
            var mongoConfig = new kason.Sg.Core.Mongo.ConfigInfo();
            mongoConfig.connectionString = MongoSettings["connectionString"].ToString();
            mongoConfig.SingleInstance =bool.Parse( MongoSettings["SingleInstance"].ToString());
            mongoConfig.dbName = MongoSettings["dbName"].ToString();

            //初始化数据
            Lottery.CrawGetters.InitConfigInfo.MongoSettings = MongoSettings;

            Lottery.CrawGetters.InitConfigInfo.MongoTableSettings = MongoSettings["TableNamesSettings"];
            Lottery.CrawGetters.InitConfigInfo.BonusPoolSetting = CrawSettings["BonusPoolSettings"];
            Lottery.CrawGetters.InitConfigInfo.MatchSettings = CrawSettings["MatchSettings"];
            ServiceHelper.MatchSettings = CrawSettings["MatchSettings"];
            Lottery.CrawGetters.InitConfigInfo.NumLettory_SleepTimeSpanSettings = CrawSettings["NumLettory_SleepTimeSpanSettings"];
            
            //JToken ORMSettings = ConfigHelper.AllConfigInfo["ORMSettings"];
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var host = new ServiceHostBuilder()
                .RegisterServices(builder =>
                {
                    builder.AddMicroService(option =>
                    {
                        option.AddServiceRuntime()
                       .UseMongo(mongoConfig)
                        .AddRelateService()
                        .AddConfigurationWatch()
                        //option.UseZooKeeperManager(new ConfigInfo("127.0.0.1:2181"));
                        .UseConsulManager(new Kason.Sg.Core.Consul.Configurations.ConfigInfo(consul,
                    "MagAndCraw/serviceRoutes/",
                    "MagAndCraw/serviceSubscribers/",
                    "MagAndCraw/serviceCommands/",
                    "MagAndCraw/serviceCaches/")
                        { ReloadOnChange = true })
                        .UseDotNettyTransport()
                        //.UseRabbitMQTransport()
                       // .AddRabbitMQAdapt()

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
               // .SubscribeAt()
               // .UseLog4net(LogLevel.Error, "Config/log4net.config")
               // .UseNLog(LogLevel.Error, "Config/NLog.config")
               .UseLog4net("Config/log4net.config")
                //.UseServer("127.0.0.1", 98)
                //.UseServer("127.0.0.1", 98，“true”) //自动生成Token
                //.UseServer("127.0.0.1", 98，“123456789”) //固定密码Token
                .UseServer(options =>
                {
                    //  options.IpEndpoint = new IPEndPoint(IPAddress.Any, 98);  
                    // options.Port = 10098;
                    // options.Ip = "127.0.0.1";
                    options.Token = "True";
                    options.ExecutionTimeoutInMilliseconds = 30000;

                    options.MaxConcurrentRequests = 2000;
                })
                // .UseServiceCache()
                //.Configure(build =>
                //build.AddEventBusJson(RebbitMqSettings))
                //.Configure(build =>
                //build.AddCacheFile("cacheSettings.json", optional: false, reloadOnChange: true))
                  .Configure(build =>
                build.AddCPlatformJSON(HostSettings))
               // .UseProxy()
                .UseStartup<Startup>()
                .Build();

            //string url = "https://1680660.com/smallSix/queryLotteryDate.do?ym=2019-04";
            //var list= Lottery.CrawGetters.WinNumberGetter_1680660.GetIssuseNum();

            //Console.WriteLine(string.Join(Environment.NewLine, list));
            //var list = JsonHelper.Deserialize<List<KaSon.FrameWork.ORM.OrmConfigInfo>>(ORMSettings.ToString());
            //DbProvider.InitConfigJson(list);

            //WinNumberGetter_1680660.winNum();

            using (host.Run())
            {
                Console.WriteLine($"管理、采集，开奖服务端启动成功，{DateTime.Now}。");
                Lottery.CrawGetters.InitConfigInfo.Init(CrawSettings);
                Lottery.CrawGetters.InitConfigInfo.logFactory = ServiceLocator.GetService<ILoggerFactory>();
                KaSon.FrameWork.Common.InitConfigInfo.logFactory = ServiceLocator.GetService<ILoggerFactory>();
                //AutoTaskServices.AutoCaheData(int.Parse(Sports_SchemeJobSeconds));
            }
            // 初始化内存期号 k_todo，可用彩种类型,执行一次
            // LotteryGameManager lotGm = new LotteryGameManager();
            // lotGm.StartInitData();
            //new Sports_Business().Test();


        

            Task.Factory.StartNew(async delegate
            {
                var obj = await Start_ConfigGameType(Auto_CollectSettings);
            });
            //RedisMatchBusiness.ReloadCurrentBJDCMatch();
            Clear();



        }

        static void Clear() {

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
                IServiceProxyProvider _serviceProxyProvider = ServiceLocator.Current.Resolve<IServiceProxyProvider>();
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
                        try
                        {


                           // Console.WriteLine(path);

                            //model["gameName"] = "SSQ";

                            await _serviceProxyProvider.Invoke<object>(model, path);
                            //byte[] byteArray = System.Text.Encoding.GetEncoding("gb2312").GetBytes(item["Desc"].ToString());

                            ///  string aaa2 = System.Text.Encoding.GetEncoding("gb2312").GetString(byteArray);
                           // Console.WriteLine(string.Format("{0}{1}", item.Desc.ToString(), " 启动了"));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(string.Format("{0}{1}{2}", item.Desc.ToString(), " 启动失败", ex.Message));

                        }


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

    internal class StartGameTypeModel {
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
}

