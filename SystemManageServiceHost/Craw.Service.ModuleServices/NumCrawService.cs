﻿using Kason.Sg.Core.CPlatform.Ioc;
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
using Newtonsoft.Json.Linq;
using Lottery.CrawGetters;

namespace Craw.Service.ModuleServices
{
    [ModuleName("NumCraw")]
    public class NumCrawService : KgBaseService, INumCrawService
    {

        //IKgLog log = null;
        //public BettingService()
        //{
        //    log = new Log4Log();
        //}
        ILogger<NumCrawService> _Log;
        private static IList<Service_AutoCollectWinNumber> aotoCollectList = new List<Service_AutoCollectWinNumber>();
        private static IList<Service_AutoCollectIssuse> aotoIssuseCollectList = new List<Service_AutoCollectIssuse>();
        private static IList<Service_AutoCollectBonusPool> aotoPoolCollectList = new List<Service_AutoCollectBonusPool>();

        private static Dictionary<string, System.Timers.Timer> timerList = new Dictionary<string, System.Timers.Timer>();
        private readonly CrawRepository rep;
        public NumCrawService(ILogger<NumCrawService> log, CrawRepository _rep)
        {
            _Log = log;
            this.rep = _rep;
        }

        /// <summary>
        /// 数字彩采集开奖号-开始服务
        /// </summary>
        /// <param name="gameName"></param>
        /// <returns></returns>
        public Task<string> NumLettory_WinNumber_Start(string gameName)
        {
            JToken sleeptimes = Lottery.CrawGetters.InitConfigInfo.NumLettory_SleepTimeSpanSettings;
            lock (aotoCollectList)
            {
                bool bol = false;
                //停留时间
                TimeSpan stopTime = TimeSpan.FromSeconds(20);
                switch (gameName)
                {
                    //重庆时时彩
                    case "CQSSC": //重庆时时彩
                    case "JX11X5"://江西11选5
                    case "SD11X5"://11选5
                    case "GD11X5":
                    case "GDKLSF":
                    case "JSKS":
                    case "SDKLPK3":
                    case "FC3D":
                    case "PL3":
                    case "SSQ":
                    case "DLT":
                    case "HK6":
                        bol = true;
                        stopTime = TimeSpan.FromSeconds(int.Parse(sleeptimes[gameName].ToString()));
                        break;
                    default:
                        break;
                }
                if (bol)
                {
                    var p = aotoCollectList.Where(b => b.Key == gameName).FirstOrDefault();
                    if (p == null)
                    {
                        //执行任务
                        Service_AutoCollectWinNumber auto = new Service_AutoCollectWinNumber(rep.MDB, gameName, stopTime);
                        auto.Key = gameName;
                        auto.Start(new CrawORMService(rep.MDB).Start);
                        aotoCollectList.Add(auto);
                    }
                }
            }


            return Task.FromResult("数字彩采集开奖号-开始服务");
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<string> NumLettory_WinNumber_Stop(string gameName)
        {
            lock (aotoCollectList)
            {
                switch (gameName)
                {
                    //重庆时时彩
                    case "CQSSC": //重庆时时彩
                    case "JX11X5"://江西11选5
                    case "SD11X5"://11选5
                    case "GD11X5":
                    case "GDKLSF":
                    case "JSKS":
                    case "SDKLPK3":
                    case "FC3D":
                    case "PL3":
                    case "SSQ":
                    case "DLT":
                    case "HK6":
                        var p = aotoCollectList.Where(b => b.Key == gameName).FirstOrDefault();
                        if (p != null)
                        {
                            p.Stop();
                            ////执行任务
                            //Service_AutoCollectWinNumber auto = new Service_AutoCollectWinNumber(TimeSpan.FromSeconds(20));
                            //auto.Start(name);
                            //aotoCollectList.Add(auto);
                            aotoCollectList.Remove(p);
                        }
                        break;
                    default:
                        break;
                }
            }

            return Task.FromResult("成功停止服务" + gameName);
        }

        /// <summary>
        /// 数字彩采集开奖号-开始服务
        /// </summary>
        /// <param name="gameName"></param>
        /// <returns></returns>
        public Task<string> NumLettory_BonusPool_Start(string gameName)
        {

            lock (aotoPoolCollectList)
            {
                bool bol = false;
                switch (gameName)
                {
                    case "SSQ":
                    case "DLT":
                        bol = true;

                        break;
                    default:
                        break;
                }

                if (bol)
                {
                    var p = aotoPoolCollectList.Where(b => b.Key == gameName).FirstOrDefault();
                    if (p == null)
                    {
                        TimeSpan stopTime = TimeSpan.FromSeconds(20);
                        JToken sleeptimes = Lottery.CrawGetters.InitConfigInfo.NumLettory_SleepTimeSpanSettings;
                        stopTime = TimeSpan.FromSeconds(int.Parse(sleeptimes[gameName].ToString()));
                        Service_AutoCollectBonusPool auto = new Service_AutoCollectBonusPool(rep.MDB, gameName, stopTime);
                        //执行任务
                        //    Service_AutoCollectBonusPool auto = new Service_AutoCollectBonusPool();
                        auto.Key = gameName;
                        auto.Start(gameName, new CrawORMService(rep.MDB).BonusPoolStart);
                        aotoPoolCollectList.Add(auto);
                    }
                }
                //执行任务


            }


            return Task.FromResult("数字彩采集奖金池-开始服务");
        }
        public Task<string> NumLettory_BonusPool_Stop(string gameName)
        {
            lock (aotoPoolCollectList)
            {
                switch (gameName)
                {
                    case "SSQ":
                    case "DLT":
                        var p = aotoPoolCollectList.Where(b => b.Key == gameName).FirstOrDefault();
                        if (p != null)
                        {
                            p.Stop();
                            ////执行任务
                            //Service_AutoCollectWinNumber auto = new Service_AutoCollectWinNumber(TimeSpan.FromSeconds(20));
                            //auto.Start(name);
                            //aotoCollectList.Add(auto);
                            aotoPoolCollectList.Remove(p);
                        }
                        break;
                    default:
                        break;
                }
            }


            return Task.FromResult("成功停止服务" + gameName);
        }

        public Task<string> NumLettory_HK6Issuse(string gameName = "HK6")
        {
            // string gameName = "HK6";
            lock (aotoPoolCollectList)
            {
                switch (gameName)
                {
                    case "HK6":

                        var p = timerList.GetValueOrDefault(gameName); ;
                        if (p == null)
                        {
                            string tempStr = Lottery.CrawGetters.InitConfigInfo.NumLettory_SleepTimeSpanSettings["HK6"].ToString();
                            int initTimeData = int.Parse(tempStr);
                            System.Timers.Timer timer = ServiceHelper.ExcuteByTimer(initTimeData, () =>
                            {
                                try
                                {


                                    Service_AutoCollectIssuse auto = new Service_AutoCollectIssuse(rep.MDB, gameName);
                                    //执行任务
                                    //    Service_AutoCollectBonusPool auto = new Service_AutoCollectBonusPool();
                                    auto.Key = gameName;
                                    auto.Start(new CrawORMService(rep.MDB).HK6IssuseStart);
                                   
                                }
                                catch (Exception)
                                {


                                }
                            });
                            timerList.Add(gameName, timer);
                        }
                        break;
                    case "HostoryHK6":

                        var p1 = timerList.GetValueOrDefault(gameName);
                        if (p1 == null)
                        {
                            string tempStr = Lottery.CrawGetters.InitConfigInfo.NumLettory_SleepTimeSpanSettings["HK6"].ToString();
                            int initTimeData = int.Parse(tempStr);
                            System.Timers.Timer timer = ServiceHelper.ExcuteByTimer(initTimeData, () =>
                            {
                                try
                                {


                                    Service_AutoCollectIssuse auto = new Service_AutoCollectIssuse(rep.MDB, gameName);

                                    auto.Key = gameName;
                                    auto.StartHostory(new CrawORMService(rep.MDB).HK6HostoryNum);
                                }
                                catch (Exception)
                                {


                                }
                            });


                            timerList.Add(gameName, timer);
                        }
                        break;
                    case "OpenWinHK6":

                        //var p2 = aotoIssuseCollectList.Where(b => b.Key == gameName).FirstOrDefault();
                        //if (p2 == null)
                        //{
                        //    Service_AutoCollectIssuse auto = new Service_AutoCollectIssuse(rep.MDB, gameName);
                        //    //执行任务 StartOpenWinNum
                        //    //    Service_AutoCollectBonusPool auto = new Service_AutoCollectBonusPool();
                        //    auto.Key = gameName;
                        //    auto.StartOpenWinNum(new CrawORMService(rep.MDB).HK6OpenwinNum);
                        //    aotoIssuseCollectList.Add(auto);
                        //}
                        var p2 = timerList.GetValueOrDefault(gameName);
                        if (p2 == null)
                        {
                            string tempStr = Lottery.CrawGetters.InitConfigInfo.NumLettory_SleepTimeSpanSettings["HK6"].ToString();
                            int initTimeData = int.Parse(tempStr);
                            System.Timers.Timer timer = ServiceHelper.ExcuteByTimer(initTimeData, () =>
                            {
                                var _log = InitConfigInfo.logFactory.CreateLogger<Service_AutoCollectIssuse>();
                                try
                                {
                                   
                                    Console.WriteLine("1结算timer 启动");
                                    _log.LogTrace("1结算timer 启动");
                                    Service_AutoCollectIssuse auto = new Service_AutoCollectIssuse(rep.MDB, gameName);

                                    auto.Key = gameName;
                                    auto.StartOpenWinNum(new CrawORMService(rep.MDB).HK6OpenwinNum);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("结算出错："+ex.ToString());
                                    _log.LogError("结算出错：" + ex.ToString());
                                }
                            });


                            timerList.Add(gameName, timer);
                        }
                        break;
                    default:
                        break;
                }
            }


            return Task.FromResult("成功停止服务" + gameName);
        }


        public Task<string> NumLettory_BJPK(string gameName = "BJPK") {

            lock (aotoPoolCollectList)
            {
                switch (gameName)
                {

                    case "OpenWinPK10":

                        //var p2 = aotoIssuseCollectList.Where(b => b.Key == gameName).FirstOrDefault();
                        //if (p2 == null)
                        //{
                        //    Service_AutoCollectIssuse auto = new Service_AutoCollectIssuse(rep.MDB, gameName);
                        //    //执行任务 StartOpenWinNum
                        //    //    Service_AutoCollectBonusPool auto = new Service_AutoCollectBonusPool();
                        //    auto.Key = gameName;
                        //    auto.StartOpenWinNum(new CrawORMService(rep.MDB).HK6OpenwinNum);
                        //    aotoIssuseCollectList.Add(auto);
                        //}
                        var p2 = timerList.GetValueOrDefault(gameName);
                        if (p2 == null)
                        {
                            string tempStr = Lottery.CrawGetters.InitConfigInfo.NumLettory_SleepTimeSpanSettings["HK6"].ToString();
                            int initTimeData = int.Parse(tempStr);
                            System.Timers.Timer timer = ServiceHelper.ExcuteByTimer(initTimeData, () =>
                            {
                                var _log = InitConfigInfo.logFactory.CreateLogger<Service_AutoCollectIssuse>();
                                try
                                {

                                    Console.WriteLine("PK10结算timer 启动");
                                    _log.LogTrace("PK10结算timer 启动");
                                    Service_AutoCollectIssuse auto = new Service_AutoCollectIssuse(rep.MDB, gameName);

                                    auto.Key = gameName;
                                    auto.StartOpenWinNumBJPK(new CrawORMService(rep.MDB).BJPKOpenwinNum);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("结算出错：" + ex.ToString());
                                    _log.LogError("结算出错：" + ex.ToString());
                                }
                            });


                            timerList.Add(gameName, timer);
                        }
                        break;
                    default:
                        break;
                }
            }


            return Task.FromResult("成功停止服务" + gameName);
        }
        //WinNumber
    }
}
