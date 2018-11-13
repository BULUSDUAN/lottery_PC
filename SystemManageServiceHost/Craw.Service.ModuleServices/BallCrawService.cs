using Kason.Sg.Core.CPlatform.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;



using System.Threading.Tasks;

using System.IO;
using System.Text;

using Craw.Service.IModuleServices;
using Kason.Sg.Core.ProxyGenerator;
using Microsoft.Extensions.Logging;
using SystemManage.ModuleBaseServices;
using EntityModel.ExceptionExtend;
using System.Threading;
using System.Collections.Concurrent;
using Lottery.CrawGetters.Auto;
using Lottery.CrawGetters.MatchBizGetter;
using Lottery.CrawGetters;
using Newtonsoft.Json.Linq;

namespace Craw.Service.ModuleServices
{
    /// <summary>
    /// 足球 篮球采集
    /// </summary>
    [ModuleName("BallCraw")]
    public class BallCrawService : KgBaseService, IBallCrawService
    {

        //IKgLog log = null;
        //public BettingService()
        //{
        //    log = new Log4Log();
        //}
        ILogger<NumCrawService> _Log;
        private static IList<CTZQMatch_AutoCollect> aotoCollectList = new List<CTZQMatch_AutoCollect>();

        private static IList<IAutoCollect> BallAutoCollectList = new List<IAutoCollect>();

        private static IList<Service_AutoCollectBonusPool> aotoPoolCollectList = new List<Service_AutoCollectBonusPool>();
        private readonly CrawRepository rep;
        public BallCrawService( ILogger<NumCrawService> log, CrawRepository _rep)
        {
            _Log = log;
           this.rep = _rep;
        }
        /// <summary>
        /// 数字彩采集开奖号-开始服务
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="gameName"></param>
        /// <returns></returns>
        public Task<string> CTZQMatchAndPool_Start(string Type,string gameName)
        {
            lock (aotoCollectList)
            {

                bool bol = false;
                switch (gameName)
                {
                    //重庆时时彩
                    case "T14C": //14场胜负
                    case "TR9"://胜负任9
                    case "T6BQC"://6场半全
                    case "T4CJQ": //4场进球

                         bol = true;
                        break;
                    default:
                        break;
                }

                if (bol)
                {
                    IAutoCollect p = BallAutoCollectList.Where(b => b.Key == gameName && b.Category== Type).FirstOrDefault();
                    if (p == null)
                    {
                        JToken sleeptimes = Lottery.CrawGetters.InitConfigInfo.NumLettory_SleepTimeSpanSettings;


                        if (Type == "Match")//赛事
                        {
                             //执行任务
                            int sptime =int.Parse( sleeptimes[gameName].ToString());
                            p  = new CTZQMatch_AutoCollect(rep.MDB, gameName, sptime);
                            p.Start();
                            p.Key = gameName;
                            p.Category = Type;
                            BallAutoCollectList.Add(p);
                        }
                        else if (Type == "Pool")
                        {
                            int sptime = int.Parse(sleeptimes[gameName].ToString());
                            p = new CTZQPool_AutoCollect(rep.MDB, gameName, sptime);
                            p.Start();
                            p.Key = gameName;
                            p.Category = Type;
                            BallAutoCollectList.Add(p);
                        }

                       
                    }
                }
            }
        


            return Task.FromResult( "CTZQ-开始服务");
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<string> CTZQMatchAndPool_Stop(string Type, string name)
        {
            lock (aotoCollectList)
            {
                switch (name)
                {
                    case "T14C": //14场胜负
                    case "TR9"://胜负任9
                    case "T6BQC"://6场半全
                    case "T4CJQ": //4场进球
                        IAutoCollect p = BallAutoCollectList.Where(b => b.Key == name && b.Category==Type).FirstOrDefault();
                        if (p != null)
                        {
                            p.Stop();
                            ////执行任务
                            //Service_AutoCollectWinNumber auto = new Service_AutoCollectWinNumber(TimeSpan.FromSeconds(20));
                            //auto.Start(name);
                            //aotoCollectList.Add(auto);
                            BallAutoCollectList.Remove(p);
                        }
                        break;
                    default:
                        break;
                }
            }


            return Task.FromResult(string.Format("{0}-开启服务",Type));
        }

        /// <summary>
        /// 数字彩采集开奖号-开始服务
        /// </summary>
        /// <param name="Type">Match_Result_OZSP</param>
        /// <param name="gameName"></param>
        /// <returns></returns>
        public Task<string> JCZQMatch_Result_OZSP_Start(string Type)
        {
            lock (aotoCollectList)
            {
                JToken sleeptimes = Lottery.CrawGetters.InitConfigInfo.NumLettory_SleepTimeSpanSettings;



                IAutoCollect p = BallAutoCollectList.Where(b => b.Key == Type).FirstOrDefault();
                    if (p == null)
                    {

                        if (Type == "JCZQMatch")//赛事
                    {
                        int sptime = int.Parse(sleeptimes[Type].ToString());
                        //执行任务
                        p = new JCZQMatch_AutoCollect(rep.MDB, "JCZQMatch", sptime);
                            p.Start();
                            p.Key = Type;
                            BallAutoCollectList.Add(p);
                        }
                    else if (Type == "JCZQResult")
                    {
                        int sptime = int.Parse(sleeptimes[Type].ToString());
                        p = new JCZQMatchResult_AutoCollect(rep.MDB, "JCZQResult", sptime);
                        p.Start();
                        p.Key = Type;
                        BallAutoCollectList.Add(p);
                    }
                    else if (Type == "JCZQOZSP")
                    {
                        int sptime = int.Parse(sleeptimes[Type].ToString());
                        p = new JCZQ_OZSP_AutoCollect(rep.MDB, "JCZQOZSP", sptime);
                        p.Start();
                        p.Key = Type;
                        BallAutoCollectList.Add(p);
                    }


                }
                
            }



            return Task.FromResult(string.Format("{0}-开启服务", Type));
        }

        public Task<string> JCZQMatch_Result_OZSP_Stop(string Type)
        {
            lock (BallAutoCollectList)
            {


                IAutoCollect p = BallAutoCollectList.Where(b => b.Key == Type).FirstOrDefault();
                if (p != null)
                {

                    p.Stop();
                    BallAutoCollectList.Remove(p);

                }

            }



            return Task.FromResult(string.Format("{0}-停止服务", Type));
        }
        public Task<string> JCLQMatch_Start() {
            lock (aotoCollectList)
            {
                JToken sleeptimes = Lottery.CrawGetters.InitConfigInfo.NumLettory_SleepTimeSpanSettings;


                string Type = "JCLQ";
                IAutoCollect p = BallAutoCollectList.Where(b => b.Key == Type).FirstOrDefault();
                if (p == null)
                {
                    int sptime = int.Parse(sleeptimes[Type].ToString());
                    p = new JCLQMatch_AutoCollect(rep.MDB, "JCLQ", sptime);
                    p.Start();
                    p.Key = "JCLQ";
                    BallAutoCollectList.Add(p);


                }

            }
            return Task.FromResult(string.Format("{0}-开启服务", "JCLQ"));
        }
        public Task<string> JCLQMatch_Stop()
        {
            lock (BallAutoCollectList)
            {

                string Type = "JCLQ";
                IAutoCollect p = BallAutoCollectList.Where(b => b.Key == Type).FirstOrDefault();
                if (p != null)
                {
                    p.Stop();
                    BallAutoCollectList.Remove(p);


                }

            }
            return Task.FromResult(string.Format("{0}-停止服务", "JCLQ"));
        }
        public Task<string> BJDCMatch_OZSP_Start(string Type)
        {
            lock (aotoCollectList)
            {
                JToken sleeptimes = Lottery.CrawGetters.InitConfigInfo.NumLettory_SleepTimeSpanSettings;


                IAutoCollect p = BallAutoCollectList.Where(b => b.Key == Type).FirstOrDefault();
                if (p == null)
                {

                    if (Type == "BJDCMatchResult")//赛事
                    {
                        int sptime = int.Parse(sleeptimes[Type].ToString());
                        //执行任务
                        p = new BJDCMatch_AutoCollect(rep.MDB, "BJDCMatchResult", sptime);
                        p.Start();
                        p.Key = Type;
                        BallAutoCollectList.Add(p);
                    }
                    else if (Type == "BJDCOZSP")
                    {
                        int sptime = int.Parse(sleeptimes[Type].ToString());
                        p = new BJDC_OZSP_AutoCollect(rep.MDB, "BJDCOZSP", sptime);
                        p.Start();
                        p.Key = Type;
                        BallAutoCollectList.Add(p);
                    }
                    


                }

            }
           // return Task.FromResult("数字彩采集开奖号-开始服务");
            return Task.FromResult(string.Format("{0}-开启服务", Type));
        }

        public Task<string> BJDCMatch_OZSP_Stop(string Type)
        {
            lock (BallAutoCollectList)
            {


                IAutoCollect p = BallAutoCollectList.Where(b => b.Key == Type).FirstOrDefault();
                if (p != null)
                {
                    p.Stop();

                    BallAutoCollectList.Remove(p);
                    //if (Type == "BJDCMatchResult")//赛事
                    //{
                    //    //执行任务
                    //    p = new BJDCMatch_AutoCollect(rep.MDB);
                    //    p.Start("All");
                    //    p.Key = Type;
                    //    BallAutoCollectList.Add(p);
                    //}
                    //else if (Type == "BJDCOZSP")
                    //{
                    //    p = new BJDC_OZSP_AutoCollect(rep.MDB);
                    //    p.Start("All");
                    //    p.Key = Type;
                    //    BallAutoCollectList.Add(p);
                    //}



                }

            }
            return Task.FromResult(string.Format("{0}-停止服务服务", Type));
        }
        

       
        //WinNumber
    }
}
