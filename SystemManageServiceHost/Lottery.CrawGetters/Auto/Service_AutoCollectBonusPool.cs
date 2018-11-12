using EntityModel.BonusPool;
using EntityModel.Enum;
using Lottery.CrawGetters.BonusPoolGetter;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lottery.CrawGetters.Auto
{
 
    public class Service_AutoCollectBonusPool : BaseAutoCollect
    {
        ILogger<Service_AutoCollectBonusPool> _log = null;

        public string Key { get; set; }
        private long BeStop = 0;
        private TimeSpan sleep;
        //public void Start(string gameName)
        //{
        //    gameName = gameName.ToUpper();
        //    logInfoSource = "AutoCollectBonusPool_Info_" + gameName;
        //    _logWriter = logWriter;


        //    BeStop = false;
        //    CollectBonusPool(gameName);
        //}
        //  private IMongoDatabase mDB;
        //ILogger<Service_AutoCollectWinNumber> _log = null;
        private IMongoDatabase mDB;
        private string gameName = "";
      
        public Service_AutoCollectBonusPool(IMongoDatabase _mDB, string _gameName, TimeSpan sleep) : base(_gameName+"Pool", _mDB)
        {
            _log = InitConfigInfo.logFactory.CreateLogger<Service_AutoCollectBonusPool>();
            mDB = _mDB;
          this.sleep = sleep;
            this.gameName = _gameName;
        }
        private Task thread = null;
        public void Start(string gameCode, Func<string, OpenDataInfo, bool> fn)
        {
            thread = Task.Factory.StartNew((Fn) =>
            {
                var Nfn = Fn as Func<string, OpenDataInfo, bool>;

                while (Interlocked.Read(ref BeStop) == 0)
                {
                    this.WriteLogAll();
                    BonusPoolResult bp= CrawStart(gameCode, Nfn);
                    switch (bp)
                    {
                        case BonusPoolResult.Success:
                            // _log.LogInformation("采集成功");
                            this.WriteLog("采集成功 ");
                            break;
                        case BonusPoolResult.Fail:
                            break;
                        case BonusPoolResult.NoGameCode:
                            break;
                        case BonusPoolResult.Error:
                            break;
                        case BonusPoolResult.CompleteNoData:
                            break;
                        default:
                            break;
                    }
                    Thread.Sleep(sleep);
                }

            }, fn);

        }
        public void Stop()
        {
            Interlocked.Exchange(ref BeStop, 1);

            if (thread != null)
            {

                thread = null;
            }


        }
        private EntityModel.Enum.BonusPoolResult CrawStart(string gameCode, Func<string, OpenDataInfo, bool> fn)
        {
          //  bool bol = true;
            try
            {
                //    if (BeStop)
                //    {
                //        WriteLog("------------------------BeStop------------------------------" + gameCode);
                //        return;
                //    }

                //    ////
                //    //var info11 = OpenDataGetter.CreateInstance("aicaipiao").GetOpenData(gameCode, "");
                //    ////
                //    this.WriteLog(string.Format("CollectBonusPool 开始--> 查询{0}奖期奖池 ", gameCode));
                //    int timeSpan = ServiceHelper.GetAutoDoCollectBonusPoolInterval();
                //    this.WriteLog(string.Format("{0}毫秒后开始自动执行奖池数据采集", timeSpan));
                //timer = ServiceHelper.ExcuteByTimer(timeSpan, () =>
                //{
                //    this.WriteLog("倒计时完成。自动执行奖池数据采集");
                string tablename = InitConfigInfo.MongoSettings["BonusPoolTableName"].ToString();
                try
                {
                    if (gameCode != "SSQ" && gameCode != "DLT") return BonusPoolResult.NoGameCode;

                    //开始取奖池
                    int maxRepeatTimes = 5;
                    string value = InitConfigInfo.BonusPoolSetting["CollectBonusPool_InterfaceType_" + gameCode].ToString();

                    string[] interfaceTypeArray = value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);// ServiceHelper.GetCollectBonusPoolInterfaceTypeArray(gameCode);
                    var info = new OpenDataInfo();
                    var i = 0;
                    var currentTimes = 0;
                    while (true)
                    {
                        try
                        {
                            if (Interlocked.Read(ref BeStop) != 0)
                                break;
                            if (currentTimes >= maxRepeatTimes)
                                break;
                            var t = interfaceTypeArray[i];
                            //this.WriteLog("使用采集接口 " + t + " 第 " + currentTimes + " 次");
                            info = OpenDataGetter.CreateInstance(t).GetOpenData(gameCode, "");
                            //this.WriteLog(string.Format("已采集到奖池信息，{0}第{1}期", info.GameCode, info.IssuseNumber));
                            if (info != null && !string.IsNullOrEmpty(info.IssuseNumber) && info.GameCode == gameCode)
                                break;

                            if (i >= interfaceTypeArray.Length - 1)
                                i = 0;
                            else
                                i++;

                            currentTimes++;
                        }
                        catch (Exception ex)
                        {
                            currentTimes++;
                            this.WriteError("采集奖池数据异常 " + ex.ToString());
                        }
                    }
                    if (info.GradeList.Count == 0)
                    {
                        this.WriteLog("奖池采集完成，但未获取到奖池信息，请手工处理");
                        return BonusPoolResult.CompleteNoData;
                    }
                    var bol =  fn(gameCode, info);

                    if (!bol) {
                        this.WriteError("数据库同步异常 ");
                        return BonusPoolResult.Fail;
                    };



                }
                catch (Exception ex)
                {
                    this.WriteError("异常 " + ex.ToString());
                }
               

                // });
            }
            catch (Exception ex)
            {
                this.WriteError("异常 " + ex.ToString());
                return BonusPoolResult.Error;
            }
            return BonusPoolResult.Success;
        }
    }
}
