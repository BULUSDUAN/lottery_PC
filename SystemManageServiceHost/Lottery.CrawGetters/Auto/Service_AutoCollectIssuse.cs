using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Linq;
using EntityModel.ExceptionExtend;
using System.Threading.Tasks;
using System.Threading;
using MongoDB.Driver;

namespace Lottery.CrawGetters.Auto
{

    /// <summary>
    /// 自动采集中奖号码服务
    /// </summary>
    public class Service_AutoCollectIssuse : BaseAutoCollect
    {
        private long BeStop = 0;
        ILogger<Service_AutoCollectWinNumber> _log = null;
        private TimeSpan sleep;
      
        private IMongoDatabase mDB;
        private string gameName = "";
        public Service_AutoCollectIssuse(IMongoDatabase _mDB,string _gameName):base(_gameName,_mDB)
        {
            _log = InitConfigInfo.logFactory.CreateLogger<Service_AutoCollectWinNumber>();
            mDB = _mDB;
          
            this.gameName = _gameName;
        }
     
        //if (fatellogger == null) {
        //       fatellogger = ;//.Fatal(message, exception);
        //   }
        //private List<IWinNumberPlugin> plugins
        //{
        //    get
        //    {

        //        //支持热更新，避免重启程序
        //        return HttpWcfClient.DefaultCache.GetCache<List<IWinNumberPlugin>>(MethodBase.GetCurrentMethod(),
        //              TimeSpan.FromDays(365),
        //              (id) =>
        //              {
        //                  var list = new List<IWinNumberPlugin>();
        //                  foreach (var file in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory,
        //                                   "WinNumberPlugin*.dll", SearchOption.TopDirectoryOnly))
        //                  {
        //                      Assembly assembly = Assembly.Load(File.ReadAllBytes(file));
        //                      var q = from type in assembly.GetTypes()
        //                              where typeof(IWinNumberPlugin).IsAssignableFrom(type)
        //                              select type;
        //                      foreach (var t in q)
        //                      {
        //                          list.Add((IWinNumberPlugin)Activator.CreateInstance(t));
        //                      }
        //                  }
        //                  list.Sort((x, y) => x.Rank() - y.Rank());
        //                  return list;
        //              },
        //              (t) => t.Count > 0);
        //    }
        //}



        //private void Publish(string gameCode, string issuseNumber, Dictionary<string, string> winNumber)
        //{
        //    //TODO:实现统一的集中数据中心
        //    var json = JsonHelper.Serialize(new { issuse = issuseNumber, number = winNumber });
        //    RedisValue msg = Common.Compression.ByteCompresser.Compress(Encoding.UTF8.GetBytes(json));
        //    RedisHelper.MasterInstance.GetSubscriber().Publish(gameCode.ToUpper(), msg);
        //    foreach (var item in plugins)
        //    {
        //        //string key = winNumber[winNumber.Max((p) => p.Key)];
        //        pool.Post((t) =>
        //        {
        //            try
        //            {
        //                var p = t[0] as IWinNumberPlugin;
        //                p.Publish(t[1] as string, t[2] as string, t[3] as Dictionary<string, string>);
        //            }
        //            catch (Exception e)
        //            {
        //                logger.Error("", e);
        //            }
        //        }, new object[] { item, gameCode, issuseNumber, winNumber }, null);

        //    }
        //}

        //public static Common.Utilities.ThreadPool processPool = HttpWcfClient.CreateThreadPool(16);

        //public static Common.Utilities.ThreadPool pool = HttpWcfClient.CreateThreadPool(16);

        //private TimeSpan sleep;
        //private Service_AutoCollectWinNumber()
        //{

        //}
        //public Service_AutoCollectWinNumber(TimeSpan sleep)
        //{
        //    this.sleep = sleep;
        //}

        //
        //Amib.Threading.STPStartInfo si = new Amib.Threading.STPStartInfo();
        //  si.MaxWorkerThreads = AppSettingsHelper.GetInt32("CacheThread", 256);
        //    return new Common.Utilities.ThreadPool(si);


        //private ConcurrentDictionary<string, string> all = new ConcurrentDictionary<string, string>();//<string, Dictionary<string, string>>();
        private List<string> Process()
        {
         return   WinNumberGetter_1680660.GetIssuseNum();

        }
        //private static Dictionary<string, string> CheckIssuseNumber(string gameCode, Dictionary<string, string> dict)
        //{
        //    if (dict.Count == 0) return null;
        //    //发生过数据源错误，多出几期的情况
        //    Dictionary<string, string> ret = new Dictionary<string, string>();
        //    var keys = dict.Keys.ToArray();
        //    bool[] status = ServiceHelper.GameBizClient.QueryIssuse(gameCode, keys);

        //    for (int idx = 0; idx < status.Length; idx++)
        //    {
        //        if (status[idx]) ret[keys[idx]] = dict[keys[idx]];
        //    }
        //    return ret.Count > 0 ? ret : null;
        //}


        //// private readonly ConcurrentDictionary<string, string> succeedWinNumber = new ConcurrentDictionary<string, string>();
        //private Thread thread = null;

        public string Key="";

        private Task thread=null;
        
        public void Start( Func<List<string>,  bool> fn)
        {
            if (thread != null)
            {
                throw new LogicException("已经运行");
            }
            gameName = gameName.ToUpper();
            BeStop = 0;
           // fn("",null);
            thread =Task.Factory.StartNew ((Fn) =>
            {
                List <string> all = new List<string>();
                List<string> dic = null;
                while (Interlocked.Read(ref BeStop) == 0)
                {
                    ////TODO：销售期间，暂停采集
                    WriteLogAll();
                    try
                    {
                        //每周天执行一次
                        System.DayOfWeek w = DateTime.Now.DayOfWeek;
                        if (System.DayOfWeek.Sunday==w && DateTime.Now.Hour==10 && DateTime.Now.Minute==30)
                        {
                            dic = Process();
                            var Nfn = Fn as Func<List<string>, bool>;

                            var bol = Nfn(dic);
                            if (bol)
                            {
                                foreach (var item in dic)
                                {
                                    var one = all.Where(b => b == item).FirstOrDefault();
                                    if (one == null)
                                    {
                                        all.Add(item);
                                    }
                                }

                                WriteLog("成功同步到数据库");
                            }
                            Thread.Sleep(1000 * 61);

                        }
                       
                        
                       // return false;
                      // WriteLog(gameName,)
                    }
                    catch (Exception ex)
                    {
                        WriteError("处理:" + ex.Message);
                    }
                    finally
                    {
                        Thread.Sleep(sleep);
                    }
                }
            },fn);
          //  thread.Start();


        }

        public void Stop()
        {
            Interlocked.Exchange(ref BeStop, 1);

            if (thread != null)
            {
               
                thread = null;
            }


        }
        //private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //public void WriteLog(string log)
        //{
        //    logger.Info(log);
        //}

        //public void WriteError(string log)
        //{
        //    logger.Error(log);
        //}


    }
}
