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

namespace Craw.Service.ModuleServices
{
    /// <summary>
    /// 足球 篮球采集
    /// </summary>
    [ModuleName("ballcreaw")]
    public class BallCrawService : KgBaseService, IBallCrawService
    {

        //IKgLog log = null;
        //public BettingService()
        //{
        //    log = new Log4Log();
        //}
        ILogger<NumCrawService> _Log;
        private static IList<CTZQMatch_AutoCollect> aotoCollectList = new List<CTZQMatch_AutoCollect>();

        private static IList<IBallAutoCollect> BallAutoCollectList = new List<IBallAutoCollect>();

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
                    IBallAutoCollect p = BallAutoCollectList.Where(b => b.Key == gameName && b.Category== Type).FirstOrDefault();
                    if (p == null)
                    {

                        if (Type == "Match")//赛事
                        {
                            //执行任务
                            p  = new CTZQMatch_AutoCollect(rep.MDB);
                            p.Start(gameName);
                            p.Key = gameName;
                            BallAutoCollectList.Add(p);
                        }
                        else if (Type == "Pool")
                        {
                            p = new CTZQPool_AutoCollect(rep.MDB);
                            p.Start(gameName);
                            p.Key = gameName;
                            BallAutoCollectList.Add(p);
                        }

                       
                    }
                }
            }
        


            return Task.FromResult( "数字彩采集开奖号-开始服务");
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
                        IBallAutoCollect p = BallAutoCollectList.Where(b => b.Key == name && b.Category==Type).FirstOrDefault();
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


            return Task.FromResult("数字彩采集开奖号-开始服务");
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

               
                    IBallAutoCollect p = BallAutoCollectList.Where(b => b.Key == Type).FirstOrDefault();
                    if (p == null)
                    {

                        if (Type == "Match")//赛事
                        {
                            //执行任务
                            p = new JCZQMatch_AutoCollect(rep.MDB);
                            p.Start("All");
                            p.Key = Type;
                            BallAutoCollectList.Add(p);
                        }
                    else if (Type == "Result")
                    {
                        p = new JCZQMatchResult_AutoCollect(rep.MDB);
                        p.Start("All");
                        p.Key = Type;
                        BallAutoCollectList.Add(p);
                    }
                    else if (Type == "OZSP")
                    {
                        p = new JCZQ_OZSP_AutoCollect(rep.MDB);
                        p.Start("All");
                        p.Key = Type;
                        BallAutoCollectList.Add(p);
                    }


                }
                
            }



            return Task.FromResult("数字彩采集开奖号-开始服务");
        }
        public Task<string> JCLQMatch_Start() {
            lock (aotoCollectList)
            {

                string Type = "LQ";
                IBallAutoCollect p = BallAutoCollectList.Where(b => b.Key == Type).FirstOrDefault();
                if (p == null)
                {
                    p = new JCLQMatch_AutoCollect(rep.MDB);
                    p.Start("All");
                    p.Key = "LQ";
                    BallAutoCollectList.Add(p);


                }

            }
            return Task.FromResult("数字彩采集开奖号-开始服务");
        }

        public Task<string> BJDCMatch_OZSP_Start(string Type)
        {
            lock (aotoCollectList)
            {


                IBallAutoCollect p = BallAutoCollectList.Where(b => b.Key == Type).FirstOrDefault();
                if (p == null)
                {

                    if (Type == "BJDCMatch")//赛事
                    {
                        //执行任务
                        p = new BJDCMatch_AutoCollect(rep.MDB);
                        p.Start("All");
                        p.Key = Type;
                        BallAutoCollectList.Add(p);
                    }
                    else if (Type == "BJDCOZSP")
                    {
                        p = new BJDC_OZSP_AutoCollect(rep.MDB);
                        p.Start("All");
                        p.Key = Type;
                        BallAutoCollectList.Add(p);
                    }
                    


                }

            }
            return Task.FromResult("数字彩采集开奖号-开始服务");
        }
        /// <summary>
        /// 数字彩采集开奖号-开始服务
        /// </summary>
        /// <param name="gameName"></param>
        /// <returns></returns>
        public Task<string> NumLettory_BonusPool_Start(string gameName)
        {

            lock (aotoCollectList)
            {
                //执行任务
                Service_AutoCollectBonusPool auto = new Service_AutoCollectBonusPool();
                        auto.Start(gameName, new CrawORMService(rep.MDB).BonusPoolStart);
                aotoPoolCollectList.Add(auto);
                
            }


            return Task.FromResult("数字彩采集开奖号-开始服务");
        }

        public Task<string> Login(string name)
        {
            throw new NotImplementedException();
        }

        //WinNumber
    }
}
