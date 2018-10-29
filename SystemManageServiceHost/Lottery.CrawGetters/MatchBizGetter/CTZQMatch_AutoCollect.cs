using EntityModel;
using EntityModel.Enum;
using EntityModel.MatchModel;
using KaSon.FrameWork.Common.Net;
using log4net;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using KaSon.FrameWork.Common.Expansion;
using System.IO;
using KaSon.FrameWork.Common.JSON;
using KaSon.FrameWork.Common.Cryptography;
using MongoDB.Bson;
using MongoDB.Driver;
using KaSon.FrameWork.ORM.Helper;
using System.Threading.Tasks;
using EntityModel.ExceptionExtend;
using System.Threading;
using EntityModel.LotteryJsonInfo;
using EntityModel.CoreModel;

namespace Lottery.CrawGetters.MatchBizGetter
{
    /// <summary>
    /// 采集传统足球赛事数据
    /// </summary>
   
    // <summary>
    /// 采集传统足球赛事数据
    /// </summary>
    public class CTZQMatch_AutoCollect : IBallAutoCollect
    {
      //  private ILogWriter _logWriter = null;
        private const string logCategory = "Services.Info";
        private string logInfoSource = "Auto_Collect_CTZQMatch_Info_";
        private const string logErrorCategory = "Services.Error";
        private const string logErrorSource = "Auto_Collect_CTZQMatch_Error";

        private long BeStop = 0;
        private System.Timers.Timer timer = null;
        private int CTZQ_advanceMinutes = 0;
        private string SavePath = string.Empty;
        private ILogger<CTZQMatch_AutoCollect> _logWriter = null;
        //private MatchManager manager = new MatchManager(DbAccess_Match_Helper.DbAccess);
        //  private static readonly ILog logger = LogManager.GetLogger(CTZQMatch);
        //public void Start( string gameCode)
        //{
        //    gameCode = gameCode.ToUpper();
        //    logInfoSource += gameCode;
        //  //  _logWriter = logWriter;

        //    BeStop = false;
        ////    CTZQ_advanceMinutes = ServiceHelper.Get_CTZQ_AdvanceMinutes();
        // //   CollectMatchs(gameCode);
        //}
        public string Category { get; set; }
        public string Key { get; set; }
        private Task thread = null;
        public void Start(string gameCode)
        {
            gameCode = gameCode.ToUpper();
            logInfoSource += gameCode;
            //  _logWriter = logWriter;

            BeStop = 0;
            if (thread != null)
            {
                throw new LogicException("已经运行");
            }
           // gameCode = gameCode.ToUpper();
            BeStop = 0;
            // fn("",null);
            thread = Task.Factory.StartNew(() =>
            {
               // ConcurrentDictionary<string, string> all = new ConcurrentDictionary<string, string>();
                Dictionary<string, string> dic = null;
                while (Interlocked.Read(ref BeStop) == 0)
                {
                    ////TODO：销售期间，暂停采集
                    try
                    {


                        CollectCTZQMatchCore(gameCode);

                    }
                    catch 
                    {
                        Thread.Sleep(10000);
                    }
                    finally
                    {
                        Thread.Sleep(10000);
                    }
                }
            });
            //  thread.Start();


        }

        private IMongoDatabase mDB;
        public CTZQMatch_AutoCollect(IMongoDatabase _mDB)
        {
            mDB = _mDB;
        }
        //private void CollectMatchs(string gameCode)
        //{
        //    try
        //    {
        //        if (BeStop)
        //        {
        //            WriteLog("------------------------BeStop------------------------------");
        //            return;
        //        }

        //        var timeSpan = ServiceHelper.GetCollect_CTZQMatch_Interval();
        //        this.WriteLog(string.Format("开始倒计时 -->{0}毫秒", timeSpan));
        //        timer = ServiceHelper.ExcuteByTimer(timeSpan, () =>
        //        {
        //            try
        //            {
        //                this.WriteLog("=======================倒计时完成*************");
        //                CollectCTZQMatchCore(gameCode);
        //                this.WriteLog("完成一次足球赛事采集");
        //            }
        //            catch (Exception ex)
        //            {
        //                this.WriteLog("------收集传统足球赛事  异常----------" + ex.ToString());
        //            }
        //            finally
        //            {
        //                Thread.Sleep(2000);
        //                //递归
        //                CollectMatchs(gameCode);
        //            }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        this.WriteLog(string.Format("------CollectMatchs异常-----{0}", ex.ToString()));
        //        ServiceHelper.ExcuteByTimer(2000, () =>
        //        {
        //            CollectMatchs(gameCode);
        //        });
        //    }
        //}

        /// <summary>
        /// 传统足球采集核心方法
        /// </summary>
        /// <param name="gameCode"></param>
        public void CollectCTZQMatchCore(string gameCode)
        {
            this.WriteLog("开始采集500wan奖期信息 编码：" + gameCode);

            try
            {
                var issuseNumberList = new List<string>();

                try
                {
                    var indexUrl_9188 = string.Empty;
                    var indexUrl_500 = string.Empty;
                    var issuseXml_9188 = string.Empty;
                    var issuseXml_500 = string.Empty;
                    indexUrl_9188 = GetIssuseIndexUrl_9188(gameCode);
                    indexUrl_500 = GetIssuseIndexUrl(gameCode);
                    this.WriteLog("开始查询奖期列表数据，查询地址：" + indexUrl_9188);
                    issuseXml_9188 = PostManager.Get(indexUrl_9188, Encoding.UTF8, 0, (request) =>
                    {
                        var result =  ServiceHelper.IsUseProxy("CTZQ");
                        if (ServiceHelper.IsUseProxy("CTZQ"))
                        {
                            var proxy = ServiceHelper.GetProxyUrl();
                            if (!string.IsNullOrEmpty(proxy))
                            {
                                request.Proxy = new System.Net.WebProxy(proxy);
                            }
                        }
                    });
                    this.WriteLog("开始查询奖期列表数据，查询地址：" + indexUrl_500);
                    issuseXml_500 = PostManager.Get(indexUrl_500, Encoding.UTF8, 0, (request) =>
                    {
                        var result = ServiceHelper.IsUseProxy("CTZQ");
                        if (ServiceHelper.IsUseProxy("CTZQ"))
                        {
                            var proxy = ServiceHelper.GetProxyUrl();
                            if (!string.IsNullOrEmpty(proxy))
                            {
                                request.Proxy = new System.Net.WebProxy(proxy);
                            }
                        }
                    });
                    issuseNumberList.AddRange(GetCTZQIssuseList_9188(issuseXml_9188));
                    issuseNumberList.AddRange(GetCTZQIssuseList(issuseXml_500));

                    this.WriteLog("查询奖期列表数据完成,获取记录数：" + issuseNumberList.Count);
                }
                catch (Exception ex)
                {
                    throw new Exception("查询奖期列表数据失败 - " + ex.Message, ex);
                }
                var source = ServiceHelper.GetSystemConfig("CTZQ_Match_Source");
                var gameIssuseList = new List<CTZQ_IssuseInfo>();
                foreach (var issuseNumber in issuseNumberList.Distinct())
                {
                    var stopTime = DateTime.Now;
                    var ds_stopTime = DateTime.Now;
                    var officialStopTime = DateTime.Now;
                    var oddList = new List<C_CTZQ_Odds>();
                    var matchCollection = new List<CTZQ_MatchInfo>();
                    if (source == "ok" && (gameCode == "T14C" || gameCode == "TR9"))
                    {
                        var url = GetTeamInfoUrl_Okooo(gameCode, issuseNumber);
                        string content = PostManager.Get(url, Encoding.GetEncoding("gb2312"));
                        GetCTZQ_MatchList_OK(gameCode, issuseNumber, content, out stopTime, out oddList);
                    }
                    var matchInfoUrl = GetTeamInfoUrl(gameCode, issuseNumber);
                    this.WriteLog(string.Format("开始查询第{0}期的赛事信息，查询地址{1}", issuseNumber, matchInfoUrl));
                    string matchXml = PostManager.Get(matchInfoUrl, Encoding.UTF8, 0, (request) =>
                    {
                        if (ServiceHelper.IsUseProxy("CTZQ"))
                        {
                            var proxy = ServiceHelper.GetProxyUrl();
                            if (!string.IsNullOrEmpty(proxy))
                            {
                                request.Proxy = new System.Net.WebProxy(proxy);
                            }
                        }
                    });
                    //比赛数据
                    matchCollection = GetCTZQ_MatchList_500W(gameCode, issuseNumber, matchXml, out stopTime, out oddList);

                    this.WriteLog(string.Format("查询第{0}期的赛事信息 成功！队伍个数：{1}", issuseNumber, matchCollection.Count));
                    try
                    {
                       // this.WriteLog("开始向数据库写入队伍赛事信息");

                        #region 写入队伍信息

                        var matchList = BuildNewCTZQMatchList(matchCollection, gameCode, issuseNumber);
                        var addList = new List<CTZQ_MatchInfo>();
                        var updateList = new List<CTZQ_MatchInfo>();
                        //var p = new ObjectPersistence(DbAccess_Match_Helper.DbAccess);
                        foreach (var item in matchList)
                        {
                            if (item.Key == DBChangeState.Add)
                                addList.Add(item.Value);
                            else
                                updateList.Add(item.Value);
                        }
                        if (addList.Count > 0)
                        {
                            var category = (int)NoticeCategory.CTZQ_Match;
                            var state = (int)DBChangeState.Add;
                            var paramT = gameCode + "^" + issuseNumber + "^" + string.Join("_", (from l in addList select l.Id).ToArray());
                            var param = string.Join("_", (from l in addList select l.Id).ToArray());
                            var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                            var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, paramT, sign);

                            //发送 传统足球队伍 添加 通知
                            var innerKey = string.Format("{0}_{1}", "CTZQ_Match", "Add");
                            new Sports_Business(this.mDB).UpdateLocalData(param, paramT,  NoticeType.CTZQ_Match, innerKey);
                          //  ServiceHelper.AddAndSendNotification(param, paramT, innerKey, NoticeType.CTZQ_Match);
                        }
                        if (updateList.Count > 0)
                        {
                            var category = (int)NoticeCategory.CTZQ_Match;
                            var state = (int)DBChangeState.Update;
                            var paramT = gameCode + "^" + issuseNumber + "^" + string.Join("_", (from l in updateList select l.Id).ToArray());
                            var param = string.Join("_", (from l in updateList select l.Id).ToArray());
                            var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                            var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, paramT, sign);

                            //发送 传统足球队伍 修改 通知
                            var innerKey = string.Format("{0}_{1}", "CTZQ_Match", "Update");
                            new Sports_Business(this.mDB).UpdateLocalData(param, paramT, NoticeType.CTZQ_Match, innerKey);
                        }

                        #endregion

                        this.WriteLog("向数据库写入队伍赛事信息成功");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("向数据库写入队伍赛事信息失败 - " + ex.Message, ex);
                    }

                    try
                    {
                        this.WriteLog("开始向数据库写入队伍赔率信息");

                        #region 写入队伍赔率
                        var matchOddsList = BuildNewCTZQMatchOddsList(oddList, gameCode, issuseNumber);
                        var addList = new List<C_CTZQ_Odds>();
                        var updateList = new List<C_CTZQ_Odds>();
                        foreach (var item in matchOddsList)
                        {
                            if (item.Key == DBChangeState.Add)
                                addList.Add(item.Value);
                            else
                                updateList.Add(item.Value);
                        }
                        if (addList.Count > 0)
                        {
                            var category = (int)NoticeCategory.CTZQ_Odds;
                            var state = (int)DBChangeState.Add;
                            var paramT = gameCode + "^" + issuseNumber + "^" + string.Join("_", (from l in addList select l.Id).ToArray());
                            var param = string.Join("_", (from l in addList select l.Id).ToArray());
                            var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                            var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, paramT, sign);

                            //发送 队伍赔率 添加 通知
                            var innerKey = string.Format("{0}_{1}", "CTZQ_Odds", "Add");
                            if (!string.IsNullOrEmpty(param))
                                new Sports_Business(this.mDB).UpdateLocalData(param, "", NoticeType.CTZQ_Odds, innerKey);
                          //  ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.CTZQ_Odds);

                        }
                        if (updateList.Count > 0)
                        {
                            var category = (int)NoticeCategory.CTZQ_Odds;
                            var state = (int)DBChangeState.Update;
                            var paramT = gameCode + "^" + issuseNumber + "^" + string.Join("_", (from l in updateList select l.Id).ToArray());
                            var param = string.Join("_", (from l in updateList select l.Id).ToArray());
                            var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                            var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, paramT, sign);

                            //发送 队伍赔率 修改 通知
                            var innerKey = string.Format("{0}_{1}", "CTZQ_Odds", "Update");
                            if (!string.IsNullOrEmpty(param))
                                new Sports_Business(this.mDB).UpdateLocalData(param, "", NoticeType.CTZQ_Odds, innerKey);
                           // ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.CTZQ_Odds);
                        }

                        #endregion

                        this.WriteLog("向数据库写入队伍赔率信息成功");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("向数据库写入队伍赔率信息失败 - " + ex.Message, ex);
                    }

                    #region  采集中国竞彩网开始投注时间

                    var IssuseStartTime = string.Empty;
                    try
                    {
                        var matchInfoUrlToZGJC = GetTeamInfoUrlToZGJC(gameCode, issuseNumber);
                        this.WriteLog(string.Format("开始查询中国竞彩第{0}期的赛事信息，查询地址{1}", issuseNumber, matchInfoUrlToZGJC));
                        string matchXmlToZGJC = PostManager.Get(matchInfoUrlToZGJC, Encoding.GetEncoding("gb2312"), 0, (request) =>
                        {
                            if (ServiceHelper.IsUseProxy("CTZQ"))
                            {
                                var proxy = ServiceHelper.GetProxyUrl();
                                if (!string.IsNullOrEmpty(proxy))
                                {
                                    request.Proxy = new System.Net.WebProxy(proxy);
                                }
                            }
                        });
                        matchXmlToZGJC = matchXmlToZGJC.Replace("getNumBack(", "");
                        matchXmlToZGJC = matchXmlToZGJC.Replace(");", "");
                        var param = KaSon.FrameWork.Common.JSON.JsonHelper.Decode(matchXmlToZGJC);
                        IssuseStartTime = param.result.start;
                        string IssuseStopTime = param.result.end;
                        stopTime = DateTime.Parse(IssuseStopTime);
                        #region old
                        //var startTime = matchXmlToZGJC.IndexOf("开售时间:");
                        //var endTime = matchXmlToZGJC.LastIndexOf("停售时间:");
                        //IssuseStartTime = matchXmlToZGJC.Substring(startTime, endTime - startTime).Replace("开售时间:", "").Replace(";", "");
                        //var startTime_stop = matchXmlToZGJC.IndexOf("停售时间:");
                        //var endTime_stop = matchXmlToZGJC.LastIndexOf("计奖时间:");
                        //var IssuseStopTime = matchXmlToZGJC.Substring(startTime_stop, endTime_stop - startTime_stop).Replace("停售时间:", "").Replace(";", "");
                        //stopTime = DateTime.Parse(IssuseStopTime); 
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("计算中国竞彩网数据出错", ex);
                    }
                    finally
                    {
                        gameIssuseList.Add(new CTZQ_IssuseInfo
                        {
                            GameCode = "CTZQ",
                            GameType = gameCode,
                            IssuseNumber = issuseNumber,
                            Id = string.Format("{0}|{1}|{2}", "CTZQ", gameCode, issuseNumber),
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            WinNumber = string.Empty,
                            StopBettingTime = stopTime.AddMinutes(CTZQ_advanceMinutes).AddMinutes(-10).ToString("yyyy-MM-dd HH:mm:ss"),
                            DSStopBettingTime = stopTime.AddMinutes(CTZQ_advanceMinutes).ToString("yyyy-MM-dd HH:mm:ss"),
                            StartTime = string.IsNullOrEmpty(IssuseStartTime) ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss") : DateTime.Parse(IssuseStartTime).ToString("yyyy-MM-dd HH:mm:sss"),
                            OfficialStopTime = stopTime.ToString("yyyy-MM-dd HH:mm:ss")
                        });
                    }

                    #endregion

                }

                try
                {
                    this.WriteLog("开始向数据库写入奖期数据。");

                    #region 写入奖期信息
                    // 要测试，如果是追加的情况
                    var issuseNewList = BuildNewGameIssuse(gameIssuseList, gameCode);

                    var addList = new List<CTZQ_IssuseInfo>();
                    var updateList = new List<CTZQ_IssuseInfo>();
                    foreach (var item in issuseNewList)
                    {
                        if (item.Key == DBChangeState.Add)
                            addList.Add(item.Value);
                        else
                            updateList.Add(item.Value);
                    }
                    if (addList.Count > 0)
                    {
                        var category = (int)NoticeCategory.CTZQ_Issuse;
                        var state = (int)DBChangeState.Add;
                        var paramT = gameCode + "^" + string.Join("_", (from l in addList select l.Id).ToArray());
                        var param = string.Join("_", (from l in addList select l.Id).ToArray());
                        var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                        var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, paramT, sign);

                        //发送 奖期数据 添加 通知
                        var innerKey = string.Format("{0}_{1}", "CTZQ_Issuse", "Add");
                        if (!string.IsNullOrEmpty(param))
                            new Sports_Business(this.mDB).UpdateLocalData(param, paramT, NoticeType.CTZQ_Issuse, innerKey);
                        //ServiceHelper.AddAndSendNotification(param, paramT, innerKey, NoticeType.CTZQ_Issuse);
                    }
                    if (updateList.Count > 0)
                    {
                        var category = (int)NoticeCategory.CTZQ_Issuse;
                        var state = (int)DBChangeState.Update;
                        var paramT = gameCode + "^" + string.Join("_", (from l in updateList select l.Id).ToArray());
                        var param = string.Join("_", (from l in updateList select l.Id).ToArray());
                        var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                        var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, paramT, sign);

                        //发送 奖期数据 修改 通知
                        var innerKey = string.Format("{0}_{1}", "CTZQ_Issuse", "Update");
                        if (!string.IsNullOrEmpty(param))
                            new Sports_Business(this.mDB).UpdateLocalData(param, paramT, NoticeType.CTZQ_Issuse, innerKey);
                        //ServiceHelper.AddAndSendNotification(param, paramT, innerKey, NoticeType.CTZQ_Issuse);
                    }

                    #endregion

                    this.WriteLog("向数据库写入奖期数据完成。");
                }
                catch (Exception ex)
                {
                    throw new Exception("向数据库写入奖期数据失败 - " + ex.Message, ex);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Stop()
        {
            BeStop = 1;
            if (timer != null)
                timer.Stop();
        }

        public void WriteLog(string log)
        {
            //if (_logWriter != null)
            //    _logWriter.Write(logCategory, logInfoSource, LogType.Information, "自动采集传统足球赛事信息", log);
        }

        public void WriteError(string log)
        {
            //if (_logWriter != null)
            //    _logWriter.Write(logErrorCategory, logErrorSource, LogType.Error, "自动采集传统足球赛事信息", log);
        }

        #region 处理奖期数据

        private List<KeyValuePair<DBChangeState, CTZQ_IssuseInfo>> BuildNewGameIssuse(List<CTZQ_IssuseInfo> currentList, string GameType)
        {
            var result = new List<KeyValuePair<DBChangeState, CTZQ_IssuseInfo>>();
            if (currentList.Count == 0) return result;
            //var issuseFileFullName = BuildFileFullName(string.Format("Match_{0}_List.json", gameCode), issuseNumber);
            //var customerSavePath = new string[] { "CTZQ", issuseNumber };

            string currentListStr =KaSon.FrameWork.Common.JSON. JsonHelper.Serialize(currentList);

        //    string tablename = Lottery.CrawGetters.InitConfigInfo.MongoTableSettings["CTZQMatch"].ToString();
            var mFilter = MongoDB.Driver.Builders<CTZQ_IssuseInfo>.Filter.Eq(b=>b.GameType, GameType) & Builders<CTZQ_IssuseInfo>.Filter.In(b=>b.IssuseNumber, currentList.Select(b=>b.IssuseNumber));
            try
            {
                var coll = mDB.GetCollection<CTZQ_IssuseInfo>("CTZQ_IssuseInfo");
                var GameTypekeys = Builders<CTZQ_IssuseInfo>.IndexKeys.Ascending(b=>b.GameType);
                var IssuseNumberkeys = Builders<CTZQ_IssuseInfo>.IndexKeys.Ascending(b => b.IssuseNumber);

               // var keys= Builders<C_CTZQ_GameIssuse>.IndexKeys.Combine(GameTypekeys, IssuseNumberkeys);
              //  IList<IndexKeysDefinition<C_CTZQ_GameIssuse>> LIST = new List<IndexKeysDefinition<C_CTZQ_GameIssuse>>();
                //  var options = new CreateOneIndexOptions() { Unique = true };
                //  Builders<C_CTZQ_GameIssuse> .IndexOptions.SetUnique(true);
                // var indexModel = new CreateIndexModel<C_CTZQ_GameIssuse>(nameof(C_CTZQ_GameIssuse go));
                //coll.Indexes.CreateOne(keys, options);
                //coll.i
 //coll.Indexes.CreateOne(Builders<C_CTZQ_GameIssuse>.IndexKeys.Combine(
 //      Builders<C_CTZQ_GameIssuse>.IndexKeys.Ascending(b=>b.GameType),
 //      Builders<C_CTZQ_GameIssuse>.IndexKeys.Ascending(b => b.IssuseNumber)), new CreateIndexOptions()
 //      {     Name= "GameIssuse",
 //          Unique = true,
 //          Sparse = true
 //      });
                //coll.c
                // var count = coll.Find(mFilter).CountDocuments();
                var document = coll.Find(mFilter).ToList();
                // BsonDocument one = coll.fincou;
               // var updated = Builders<BsonDocument>.Update.Set("Content", currentListStr);
                if (document.Count()>0)
                {//更新
                 //Thread.Sleep(2000);
                 // var text = document["Content"].ToString().Trim();//.Replace("var data=", "").Replace("];", "]");
                    var oldList = document;// document ==null ? new List<C_CTZQ_GameIssuse>() : JSONHelper.Deserialize<List<C_CTZQ_GameIssuse>>(text);
                    var newList = GetNewGameIssuse(oldList, currentList);
                    // UpdateResult uresult = coll.UpdateOne(mFilter, updated);
                    //if (uresult.ModifiedCount > 0)
                    //{
                    //    //成功修改一行以上
                    //}
                    coll.DeleteMany(mFilter);
                   // coll.InsertMany(currentList);

                    foreach (var item in newList)
                    {
                        var old = oldList.FirstOrDefault(l => l.Id == item.Id);
                        if (old != null)
                        {
                            oldList.Remove(old);
                            result.Add(new KeyValuePair<DBChangeState, CTZQ_IssuseInfo>(DBChangeState.Update, item));
                        }
                        else
                        {
                            oldList.RemoveAt(0);
                            result.Add(new KeyValuePair<DBChangeState, CTZQ_IssuseInfo>(DBChangeState.Add, item));
                        }
                        oldList.Add(item);
                    }

                }
                else
                {
                    foreach (var item in currentList)
                    {
                        result.Add(new KeyValuePair<DBChangeState, CTZQ_IssuseInfo>(DBChangeState.Add, item));
                    }
                  
                }
                coll.InsertMany(currentList);
            }
            catch (Exception)
            {

                throw;
            }

            
            return result;
        }

        private List<CTZQ_IssuseInfo> GetNewGameIssuse(List<CTZQ_IssuseInfo> oldList, List<CTZQ_IssuseInfo> newList)
        {
            var list = new List<CTZQ_IssuseInfo>();
            foreach (var item in newList)
            {
                var old = oldList.FirstOrDefault(p => p.Id == item.Id && p.CreateTime == item.CreateTime);
                if (old == null)
                {
                    list.Add(item);
                    continue;
                }
                if (item.Equals(old))
                    continue;

                list.Add(item);
            }
            return list;
        }

        private List<string> GetCTZQIssuseList(string xmlContent)
        {
            var list = new List<string>();
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xmlContent);
                var root = doc.SelectSingleNode("xml");
                if (root == null)
                    throw new Exception("根节点为空");
                foreach (XmlNode node in root.ChildNodes)
                {
                    //if (node.Attributes["iscurrent"].Value.GetNullString() == "1")
                    list.Add(node.Attributes["expect"].Value.GetNullString());
                    //list.Add(new Issuse_CTZQ
                    //{
                    //    CreateTime = DateTime.Now,
                    //    IssuseNumber = node.Attributes["expect"].Value.GetNullString(),
                    //    IsCurrentIssuse = node.Attributes["iscurrent"].Value.GetBoolen(),
                    //});
                }
                return list;
            }
            catch (Exception ex)
            {
                this.WriteLog("解析CTZQ期号数据出错 " + ex.ToString());
                return list;
            }
        }

        private List<string> GetCTZQIssuseList_9188(string xmlContent)
        {
            var list = new List<string>();
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xmlContent);
                var root = doc.SelectSingleNode("Resp");
                if (root == null)
                    throw new Exception("根节点为空");
                foreach (XmlNode node in root.ChildNodes)
                {
                    //if (node.Attributes["sale"].Value.GetNullString().Substring(2))
                    list.Add(node.Attributes["pid"].Value.GetNullString().Substring(2));
                    //list.Add(new Issuse_CTZQ
                    //{
                    //    CreateTime = DateTime.Now,
                    //    IssuseNumber = node.Attributes["expect"].Value.GetNullString(),
                    //    IsCurrentIssuse = node.Attributes["iscurrent"].Value.GetBoolen(),
                    //});
                }
                return list;
            }
            catch (Exception ex)
            {
                this.WriteLog("解析CTZQ期号数据出错 " + ex.ToString());
                return list;
            }
        }

        #endregion

        #region 处理队伍信息

        public List<C_CTZQ_Match> GetZS_MatchList(string gameCode, string issuseNumber)
        {
            var list = new List<C_CTZQ_Match>();
            var clUrl = "";
            switch (gameCode)
            {
                case "T14C":
                case "TR9":
                    clUrl = string.Format("http://live.caibb.com/apps?lotyid=1&expect=20{0}", issuseNumber);
                    break;
                case "T6BQC":
                    clUrl = string.Format("http://live.caibb.com/apps?lotyid=4&expect=20{0}", issuseNumber);
                    break;
                case "T4CJQ":
                    clUrl = string.Format("http://live.caibb.com/apps?lotyid=3&expect=20{0}", issuseNumber);
                    break;
                default:
                    break;
            }
            var clDoc = new XmlDocument();
            var clContent = PostManager.Get(clUrl, Encoding.GetEncoding("gb2312"), 0, (request) =>
            {
                if (ServiceHelper.IsUseProxy("CTZQ"))
                {
                    var proxy = ServiceHelper.GetProxyUrl();
                    if (!string.IsNullOrEmpty(proxy))
                    {
                        request.Proxy = new System.Net.WebProxy(proxy);
                    }
                }
            });

            if (clContent == "404" || clContent == "[]" || string.IsNullOrEmpty(clContent))
                return list;

            var dic = JsonHelper.Decode(clContent) as Dictionary<string, object>;
            foreach (var item in dic)
            {
                var v = item.Value as Dictionary<string, object>;
                var xid = v.ContainsKey("xid") ? v["xid"].ToString() : "0";
                var htid = v.ContainsKey("htid") ? v["htid"].ToString() : "0";
                var atid = v.ContainsKey("atid") ? v["atid"].ToString() : "0";
                var lid = v.ContainsKey("lid") ? v["lid"].ToString() : "0";
                var oddsmid = v.ContainsKey("mid") ? v["mid"].ToString() : "0";
                var cl = v.ContainsKey("cl") ? v["cl"].ToString() : "0";
                var sid = v.ContainsKey("sid") ? v["sid"].ToString() : "0";


                list.Add(new C_CTZQ_Match
                {
                    OrderNumber = int.Parse(xid),
                    HomeTeamId = htid,
                    GuestTeamId = atid,
                    MatchId = int.Parse(lid),
                    Mid = int.Parse(oddsmid),
                    Color = cl,
                    //赛季 id
                    FXId = int.Parse(sid),
                });
            }

            //clDoc.LoadXml(clContent);

            //var root = clDoc.SelectSingleNode("xml");
            //foreach (XmlNode item in root.ChildNodes)
            //{
            //    var xid = item.Attributes["xid"].Value;
            //    var htid = item.Attributes["htid"].Value;
            //    var gtid = item.Attributes["atid"].Value;
            //    var lid = item.Attributes["lid"].Value;
            //    var oddsmid = item.Attributes["mid"].Value;
            //    var cl = item.Attributes["cl"].Value;
            //    var sid = item.Attributes["sid"].Value;

            //    list.Add(new CTZQ_Match
            //    {
            //        OrderNumber = int.Parse(xid),
            //        HomeTeamId = htid,
            //        GuestTeamId = gtid,
            //        MatchId = int.Parse(lid),
            //        Mid = int.Parse(oddsmid),
            //        Color = cl,
            //        //赛季 id
            //        FXId = int.Parse(sid),
            //    });
            //}

            return list;
        }

        public List<CTZQ_MatchInfo> GetCTZQ_MatchList_500W(string gameCode, string issuseNumber, string xmlContent, out DateTime stopTime, out List<C_CTZQ_Odds> oddList)
        {
            stopTime = DateTime.Now;
            oddList = new List<C_CTZQ_Odds>();
            var list = new List<CTZQ_MatchInfo>();
            var t14cFXDic = new Dictionary<string, string>();
            var t6bqcFXDic = new Dictionary<string, string>();
            var t4cjqFXDic = new Dictionary<string, string>();
            try
            {
                var zsMathList = GetZS_MatchList(gameCode, issuseNumber);

                var doc = new XmlDocument();
                doc.LoadXml(xmlContent);
                var root = doc.SelectSingleNode("xml");
                foreach (XmlNode item in root.ChildNodes)
                {
                    if (item.Name == "head")
                    {
                        stopTime = item.Attributes["fsendtime"].Value.GetDateTime();
                        continue;
                    }
                    if (item.Name == "row")
                    {
                        var orderNumber = item.Attributes["ordernum"].Value.GetInt32();
                        var zsMatch = zsMathList.FirstOrDefault(p => p.OrderNumber == orderNumber);
                        if (gameCode == "T6BQC")
                            orderNumber *= 2;
                        var backColor = item.Attributes["backcolor"].Value;

                        var homeTeamName = item.Attributes["hometeam"].Value;
                        var guestTeamName = item.Attributes["guestteam"].Value;
                        var matchName = item.Attributes["simplegbname"].Value;
                        //var homeTeam = manager.QueryTeamEntity(homeTeamName);
                        //var guestTeam = manager.QueryTeamEntity(guestTeamName);
                        //var match = manager.QueryLeagueEntity(matchName);
                        var orderNum = item.Attributes["ordernum"].Value;
                        var fxKey = string.Format("{0}_{1}", issuseNumber, orderNum);
                        var matchId = 0;
                        var fxId = 0;
                        var id = string.Format("{0}|{1}|{2}|{3}", "CTZQ", gameCode, issuseNumber, orderNum);
                        var okMatchid = _okMatchId.FirstOrDefault(p => p.Key == id);
                        if (!string.IsNullOrEmpty(okMatchid.Value))
                        {
                            var txt = okMatchid.Value;
                            var strL = txt.Split('^');
                            if (strL.Length == 2)
                            {
                                matchId = int.Parse(strL[0]);
                                fxId = int.Parse(strL[1]);
                            }
                        }
                       
                        list.Add(new CTZQ_MatchInfo
                        {
                            GameCode = "CTZQ",
                            IssuseNumber = issuseNumber,
                            OrderNumber = orderNumber,
                            Id = id,
                            MatchStartTime = GetMatchStartTime(item.Attributes["resultscore"].Value).ToString("yyyy-MM-dd HH:mm:ss"),
                            Color = zsMatch != null ? zsMatch.Color : backColor,
                            MatchId = zsMatch != null ? zsMatch.MatchId : matchId,
                            MatchName = matchName,
                            HomeTeamId = zsMatch != null ? zsMatch.HomeTeamId : string.Empty,
                            HomeTeamName = homeTeamName,
                            GuestTeamId = zsMatch != null ? zsMatch.GuestTeamId : string.Empty,
                            GuestTeamName = guestTeamName,
                            MatchState = (int)CTZQMatchState.Waiting,
                            HomeTeamStanding = item.Attributes["homestanding"].Value,
                            GuestTeamStanding =item.Attributes["gueststanding"].Value,
                            HomeTeamHalfScore = item.Attributes["hhomescore"] == null ? -1 : item.Attributes["hhomescore"].Value.GetInt32(),
                            HomeTeamScore = item.Attributes["homescore"].Value.GetInt32(),
                            GuestTeamHalfScore = item.Attributes["hguestscore"] == null ? -1 : item.Attributes["hguestscore"].Value.GetInt32(),
                            GuestTeamScore = item.Attributes["guestscore"].Value.GetInt32(),
                            MatchResult = (item.Attributes["result"] == null) ? string.Empty : item.Attributes["result"].Value,
                            UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            GameType = gameCode,
                            Mid = zsMatch != null ? zsMatch.Mid : 0,
                            FXId = zsMatch != null ? zsMatch.FXId : fxId,
                        });

                        var averageOdds = string.Empty;
                        var halfAverageOdds = string.Empty;
                        var fullAverageOdds = string.Empty;

                        if (gameCode == "T14C" || gameCode == "TR9")
                            averageOdds = item.Attributes["plurl"].Value.GetNullString().Replace("&nbsp;", "|");
                        if (gameCode == "T6BQC")
                        {
                            halfAverageOdds = item.Attributes["hpl"].Value.GetNullString().Replace("&nbsp;", "|");
                            fullAverageOdds = item.Attributes["pl"].Value.GetNullString().Replace("&nbsp;", "|");
                        }
                        if (gameCode == "T4CJQ")
                            averageOdds = item.Attributes["pl"].Value.GetNullString().Replace("&nbsp;", "|");
                        oddList.Add(new C_CTZQ_Odds
                        {
                            Id = string.Format("{0}|{1}|{2}|{3}", "CTZQ", gameCode, issuseNumber, item.Attributes["ordernum"].Value),
                            LSWin = item.Attributes["winls"].Value.GetDecimal(),
                            LSFlat = item.Attributes["drawls"].Value.GetDecimal(),
                            LSLose = item.Attributes["lostls"].Value.GetDecimal(),
                            KLWin = item.Attributes["winkl"].Value.GetDecimal(),
                            KLFlat = item.Attributes["drawkl"].Value.GetDecimal(),
                            KLLose = item.Attributes["lostkl"].Value.GetDecimal(),
                            YPSW = item.Attributes["asian"].Value.Replace("&nbsp;", "|"),
                            AverageOdds = averageOdds,
                            HalfAverageOdds = halfAverageOdds,
                            FullAverageOdds = fullAverageOdds,
                            UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        });
                    }
                }
                return list;

            }
            catch (Exception ex)
            {
                this.WriteLog("解析队伍信息出错 " + ex.ToString());
                return list;
            }
        }

        private Dictionary<string, string> _okMatchId = new Dictionary<string, string>();

        public List<C_CTZQ_Match> GetCTZQ_MatchList_OK(string gameCode, string issuseNumber, string content, out DateTime stopTime, out List<C_CTZQ_Odds> oddList)
        {
            stopTime = DateTime.Now;
            _okMatchId = new Dictionary<string, string>();
            oddList = new List<C_CTZQ_Odds>();
            var list = new List<C_CTZQ_Match>();
            var t14cFXDic = new Dictionary<string, string>();
            var t6bqcFXDic = new Dictionary<string, string>();
            var t4cjqFXDic = new Dictionary<string, string>();
            var result = string.Empty;
            try
            {
                var zsMathList = GetZS_MatchList(gameCode, issuseNumber);
                var index = content.IndexOf("<div class=\"jcmian zucaidiv\"");
                content = content.Substring(index);
                index = content.IndexOf("<table");
                content = content.Substring(index);
                index = content.IndexOf("</table>");
                content = content.Substring(0, index);
                var rows = content.Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var item in rows)
                {
                    //if (item.Contains("<table")) continue;
                    if (item.Contains("</tbody>")) continue;
                    var row = item.Trim();

                    index = row.IndexOf(">");
                    row = row.Substring(index + 1);
                    var orderNumber = string.Empty;
                    var backColor = string.Empty;
                    var homeTeamName = string.Empty;
                    var guestTeamName = string.Empty;
                    var matchName = string.Empty;
                    var orderNum = string.Empty;
                    var fxKey = string.Empty;
                    var matchStartTime = DateTime.Now;
                    var hOdd = 0M;
                    var aOdd = 0M;
                    var gOdd = 0M;
                    var matchId = 0;
                    var fxId = 0;
                    var homeTeamStanding = string.Empty;
                    var guestTeamStanding = string.Empty;


                    var tds = row.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < tds.Length; i++)
                    {
                        var td = tds[i].Trim();
                        if (!td.Contains("<td")) continue;

                        var tdContent = CutHtml(td);
                        switch (i)
                        {
                            case 0:
                                orderNumber = tdContent;
                                break;
                            case 1:
                                index = td.IndexOf("soccer/league/") + "soccer/league/".Length;
                                var mid = td.Substring(index);
                                index = mid.IndexOf("/\"");
                                matchId = int.Parse(mid.Substring(0, index));
                                index = td.IndexOf("style=\"background-color:") + "style=\"background-color:".Length;
                                td = td.Substring(index);
                                backColor = td.Substring(0, 7);
                                matchName = tdContent;

                                break;
                            case 2:
                                matchStartTime = DateTime.Parse(string.Format("2015-{0}", tdContent));
                                break;
                            case 3:
                                var abq = td.Split(new string[] { "</a>" }, StringSplitOptions.RemoveEmptyEntries);
                                for (int j = 0; j < abq.Length; j++)
                                {
                                    var str = abq[j];
                                    switch (j)
                                    {
                                        case 0:
                                            index = str.IndexOf("<i>[") + "<i>[".Length;
                                            var hts = str.Substring(index);
                                            index = hts.IndexOf("]</i>");
                                            if (index != -1)
                                                homeTeamStanding = hts.Substring(0, index);
                                            index = str.IndexOf("<em class=\"pltxt\"") + "<em class=\"pltxt\"".Length;
                                            str = str.Substring(index);
                                            index = str.LastIndexOf("</em>");
                                            var hh = str.Substring(0, index).Replace(">", "").Trim(); result = hh.ToString();
                                            hOdd = string.IsNullOrEmpty(hh) ? 0M : decimal.Parse(hh.Replace("style=\"color:red\"", ""));
                                            index = str.IndexOf("<span");
                                            str = str.Substring(index);
                                            homeTeamName = CutHtml(str);
                                            break;
                                        case 1:
                                            var aa = CutHtml(str); result = str.ToString();
                                            aOdd = string.IsNullOrEmpty(CutHtml(str)) ? 0M : decimal.Parse(CutHtml(str.Replace("style=\"color:red\"", "")));
                                            break;
                                        case 2:
                                            index = str.IndexOf("<i>[") + "<i>[".Length;
                                            var gts = str.Substring(index);
                                            index = gts.IndexOf("]</i>");
                                            if (index != -1)
                                                guestTeamStanding = gts.Substring(0, index);
                                            index = str.IndexOf("<em class=\"pltxt\"") + "<em class=\"pltxt\"".Length;
                                            str = str.Substring(index);
                                            index = str.IndexOf("</em>");
                                            var gg = str.Substring(0, index).Replace(">", "").Trim(); result = gg.ToString();
                                            gOdd = string.IsNullOrEmpty(gg) ? 0M : decimal.Parse(gg.Replace("style=\"color:red\"", ""));
                                            index = str.IndexOf("<span");
                                            str = str.Substring(index);
                                            guestTeamName = CutHtml(str);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            case 5:
                                index = td.IndexOf("/soccer/match/") + "/soccer/match/".Length;
                                var yox = td.Substring(index);
                                index = yox.IndexOf("/odds/");
                                fxId = int.Parse(yox.Substring(0, index));
                                break;
                            default:
                                break;
                        }
                    }

                    var zsMatch = zsMathList.FirstOrDefault(p => p.OrderNumber == int.Parse(orderNumber));
                    list.Add(new C_CTZQ_Match
                    {
                        GameCode = "CTZQ",
                        IssuseNumber = issuseNumber,
                        OrderNumber = int.Parse(orderNumber),
                        Id = string.Format("{0}|{1}|{2}|{3}", "CTZQ", gameCode, issuseNumber, orderNumber),
                        MatchStartTime = matchStartTime,//.ToString("yyyy-MM-dd HH:mm:ss"),
                        Color = zsMatch != null ? zsMatch.Color : backColor,
                        MatchId = zsMatch != null ? zsMatch.MatchId : matchId,
                        MatchName = matchName,
                        HomeTeamId = zsMatch != null ? zsMatch.HomeTeamId : string.Empty,
                        HomeTeamName = homeTeamName,
                        GuestTeamId = zsMatch != null ? zsMatch.GuestTeamId : string.Empty,
                        GuestTeamName = guestTeamName,
                        MatchState = (int)CTZQMatchState.Waiting,
                        HomeTeamStanding =int.Parse( homeTeamStanding),
                        GuestTeamStanding = int.Parse(guestTeamStanding),
                        HomeTeamHalfScore = -1,
                        HomeTeamScore = -1,
                        GuestTeamHalfScore = -1,
                        GuestTeamScore = -1,
                        MatchResult = string.Empty,
                        UpdateTime = DateTime.Now,//.ToString("yyyy-MM-dd HH:mm:ss"),
                        GameType = gameCode,
                        Mid = zsMatch != null ? zsMatch.Mid : 0,
                        FXId = zsMatch != null ? zsMatch.FXId : fxId,
                    });

                    _okMatchId.Add(string.Format("{0}|{1}|{2}|{3}", "CTZQ", gameCode, issuseNumber, orderNumber), string.Format("{0}^{1}", matchId, fxId));

                    var averageOdds = string.Empty;
                    var halfAverageOdds = string.Empty;
                    var fullAverageOdds = string.Empty;

                    if (gameCode == "T14C" || gameCode == "TR9")
                        averageOdds = string.Format("{0}|{1}|{2}", hOdd, aOdd, gOdd);
                    //if (gameCode == "T6BQC")
                    //{
                    //    halfAverageOdds = string.Format("{0}|{1}|{2}", hOdd, aOdd, gOdd);
                    //    fullAverageOdds = string.Format("{0}|{1}|{2}", hOdd, aOdd, gOdd);
                    //}
                    //if (gameCode == "T4CJQ")
                    //    averageOdds = string.Format("{0}|{1}|{2}", hOdd, aOdd, gOdd);
                    oddList.Add(new C_CTZQ_Odds
                    {
                        Id = string.Format("{0}|{1}|{2}|{3}", "CTZQ", gameCode, issuseNumber, orderNumber),
                        LSWin = 0M,
                        LSFlat = 0M,
                        LSLose = 0M,
                        KLWin = 0M,
                        KLFlat = 0M,
                        KLLose = 0M,
                        YPSW = string.Empty,
                        AverageOdds = averageOdds,
                        HalfAverageOdds = halfAverageOdds,
                        FullAverageOdds = fullAverageOdds,
                        UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    });
                }
                return list;

            }
            catch (Exception ex)
            {
                this.WriteLog("解析队伍信息出错 " + ex.ToString() + result.ToString());
                return list;
            }
        }

        private DateTime GetMatchStartTime(string xmlValue)
        {
            if (string.IsNullOrEmpty(xmlValue.GetNullString()))
            {
                return DateTime.Now;
            }
            else
            {
                var dt = DateTime.Parse(DateTime.Now.Year + "-" + xmlValue.GetNullString());
                //if (dt < DateTime.Now)
                //{
                //    dt = dt.AddYears(1);
                //}
                return dt;
            }
        }
        private List<KeyValuePair<DBChangeState, CTZQ_MatchInfo>> BuildNewCTZQMatchList(List<CTZQ_MatchInfo> currentList, string GameType, string issuseNumber)
        {
            var result = new List<KeyValuePair<DBChangeState, CTZQ_MatchInfo>>();
            if (currentList.Count == 0) return result;
          //  var issuseFileFullName = BuildFileFullName(string.Format("Match_{0}_List.json", gameCode), issuseNumber);
            var customerSavePath = new string[] { "CTZQ", issuseNumber };

                  try
            {
                string currentListStr = KaSon.FrameWork.Common.JSON.JsonHelper.Serialize(currentList);

                // string tablename = Lottery.CrawGetters.InitConfigInfo.MongoTableSettings["CTZQMatch"].ToString();IssuseNumber  IssuseNumber
                var mFilter = MongoDB.Driver.Builders<CTZQ_MatchInfo>.Filter.Eq(b=>b.GameType, GameType) & Builders<CTZQ_MatchInfo>.Filter.Eq(b => b.IssuseNumber, issuseNumber);

                var coll = mDB.GetCollection<CTZQ_MatchInfo>("CTZQ_MatchInfo");
               // var count = coll.Find(mFilter).CountDocuments();
                var document = coll.Find(mFilter).ToList<CTZQ_MatchInfo>();
                // BsonDocument one = coll.fincou;
               // var updated = Builders<BsonDocument>.Update.Set("Content", currentListStr);
                if (document.Count>0)
                {//更新
                 //Thread.Sleep(2000);
                  //  var text = document["Content"].ToString().Trim();//.Replace("var data=", "").Replace("];", "]");
                 //  var oldList = string.IsNullOrEmpty(text) ? new List<CTZQ_MatchInfo>() : KaSon.FrameWork.Common.JSON.JsonHelper.Deserialize<List<CTZQ_MatchInfo>>(text);
                    var newList = GetNewMatch(document, currentList);

                    coll.DeleteMany(mFilter);

                    //UpdateResult uresult = coll.UpdateOne(mFilter, updated);
                    //if (uresult.ModifiedCount > 0)
                    //{
                    //    //成功修改一行以上
                    //}
                    foreach (var item in newList)
                    {
                        result.Add(new KeyValuePair<DBChangeState, CTZQ_MatchInfo>(DBChangeState.Update, item));
                    }
                   
                }
                else {
                    foreach (var item in currentList)
                    {
                        result.Add(new KeyValuePair<DBChangeState, CTZQ_MatchInfo>(DBChangeState.Add, item));
                    }
                    //BsonDocument bson = new BsonDocument();
                    //bson.Add("GameCode", gameCode);
                    //bson.Add("IssuseNumber", issuseNumber);
                    //bson.Add("Content", currentListStr);
                   
                }
                coll.InsertMany(currentList);
            }
            catch (Exception)
            {

                throw;
            }
           
            //if (File.Exists(issuseFileFullName))
            //{
            //    var text = File.ReadAllText(issuseFileFullName).Trim().Replace("var data=", "").Replace("];", "]");
            //    var oldList = string.IsNullOrEmpty(text) ? new List<C_CTZQ_Match>() :JSONHelper.Deserialize<List<C_CTZQ_Match>>(text);
            //    var newList = GetNewMatch(oldList, currentList);
            //    //ServiceHelper.CreateOrAppend_JSONFile(issuseFileFullName, JSONHelper.Serialize(currentList), (log) =>
            //    //{
            //    //    this.WriteLog(log);
            //    //});
            //    foreach (var item in newList)
            //    {
            //        result.Add(new KeyValuePair<DBChangeState, C_CTZQ_Match>(DBChangeState.Update, item));
            //    }
              
               
            //    //上传文件
            //    //ServiceHelper.PostFileToServer(issuseFileFullName, customerSavePath, (log) =>
            //    //{
            //    //    this.WriteLog(log);
            //    //});
            //    return result;
            //}

            ////录入到monodb
           

            //currentListStr = JSONHelper.Serialize(currentList);
            //BsonDocument bson = new BsonDocument();
            //bson.Add("GameCode", gameCode);
            //bson.Add("IssuseNumber", issuseNumber);
            //bson.Add("Content", currentListStr);
            //var mFilter = MongoDB.Driver.Builders<MongoDB.Bson.BsonDocument>.Filter.Eq("GameCode", gameCode) & Builders<BsonDocument>.Filter.Eq("IssuseNumber", issuseNumber);
            //// var mUpdateDocument =Builders<MongoDB.Bson.BsonDocument>.Update.Set("Content", content);
            //try
            //{
            //    var coll = mDB.GetCollection<BsonDocument>(tablename);
            //    coll.DeleteMany(mFilter);
            //    coll.InsertOne(bson);
            //}
            //catch (Exception)
            //{

            //    throw;
            //}
            ////try
            ////{
            ////    ServiceHelper.CreateOrAppend_JSONFile(issuseFileFullName, JSONHelper.Serialize(currentList), (log) =>
            ////    {
            ////        this.WriteLog(log);
            ////    });
            ////}
            ////catch (Exception ex)
            ////{
            ////    this.WriteLog(string.Format("第一次写入 {0}  文件失败：{1}", issuseFileFullName, ex.ToString()));
            ////}
            ////上传文件
            ////ServiceHelper.PostFileToServer(issuseFileFullName, customerSavePath, (log) =>
            ////{
            ////    this.WriteLog(log);
            ////});
            //foreach (var item in currentList)
            //{
            //    result.Add(new KeyValuePair<DBChangeState, C_CTZQ_Match>(DBChangeState.Add, item));
            //}
            return result;
        }

        private List<CTZQ_MatchInfo> GetNewMatch(List<CTZQ_MatchInfo> oldList, List<CTZQ_MatchInfo> newList)
        {
            var list = new List<CTZQ_MatchInfo>();
            foreach (var item in newList)
            {
                var old = oldList.FirstOrDefault(p => p.Id == item.Id);
                if (old == null)
                {
                    list.Add(item);
                    continue;
                }
                if (item.Equals(old))
                    continue;

                list.Add(item);
            }
            return list;
        }

        #endregion

        #region 处理赔率数据

        private List<KeyValuePair<DBChangeState, C_CTZQ_Odds>> BuildNewCTZQMatchOddsList(List<C_CTZQ_Odds> currentList, string gameType, string issuseNumber)
        {
            var result = new List<KeyValuePair<DBChangeState, C_CTZQ_Odds>>();
            if (currentList.Count == 0) return result;
         //   var issuseFileFullName = BuildFileFullName(string.Format("Match_{0}_Odds_List.json", gameCode), issuseNumber);


            var customerSavePath = new string[] { "CTZQ", issuseNumber };

            string currentListStr = KaSon.FrameWork.Common.JSON.JsonHelper.Serialize(currentList);

          //  string tablename = Lottery.CrawGetters.InitConfigInfo.MongoTableSettings["CTZQMatch"].ToString();
            var mFilter = MongoDB.Driver.Builders<MongoDB.Bson.BsonDocument>.Filter.Eq("GameType", gameType) & Builders<BsonDocument>.Filter.Eq("IssuseNumber", issuseNumber);

            try
            {
                var coll = mDB.GetCollection<BsonDocument>("C_CTZQ_Odds");
                // var count = coll.Find(mFilter).CountDocuments();
                var document = coll.Find(mFilter).FirstOrDefault();
                // BsonDocument one = coll.fincou;
                var updated = Builders<BsonDocument>.Update.Set("Content", currentListStr);
                if (document != null)
                {//更新
                 //Thread.Sleep(2000);
                    var text = document["Content"].ToString().Trim();//.Replace("var data=", "").Replace("];", "]");
                    var oldList = string.IsNullOrEmpty(text) ? new List<C_CTZQ_Odds>() : KaSon.FrameWork.Common.JSON.JsonHelper.Deserialize<List<C_CTZQ_Odds>>(text);
                    var newList = GetNewMatchOdds(oldList, currentList);
                    DeleteResult uresult = coll.DeleteMany(mFilter);
                    if (uresult.DeletedCount > 0)
                    {
                        //成功修改一行以上
                    }
                    foreach (var item in newList)
                    {
                        result.Add(new KeyValuePair<DBChangeState, C_CTZQ_Odds>(DBChangeState.Update, item));
                    }

                }
                else
                {
                    foreach (var item in currentList)
                    {
                        result.Add(new KeyValuePair<DBChangeState, C_CTZQ_Odds>(DBChangeState.Add, item));
                    }
                   
                }
                BsonDocument bson = new BsonDocument();
                bson.Add("GameType", gameType);
                bson.Add("IssuseNumber", issuseNumber);
                bson.Add("Content", currentListStr);
                coll.InsertOne(bson);
            }
            catch (Exception)
            {

                throw;
            }

            return result;


        }

        private List<C_CTZQ_Odds> GetNewMatchOdds(List<C_CTZQ_Odds> oldList, List<C_CTZQ_Odds> newList)
        {
            var list = new List<C_CTZQ_Odds>();
            foreach (var item in newList)
            {
                var old = oldList.FirstOrDefault(p => p.Id == item.Id);
                if (old == null)
                {
                    list.Add(item);
                    continue;
                }
                if (item.Equals(old))
                    continue;

                list.Add(item);
            }
            return list;
        }

        #endregion

        /// <summary>
        /// 创建文件全路径
        /// </summary>
        private string BuildFileFullName(string fileName, string issuseNumber)
        {
           // if (string.IsNullOrEmpty(SavePath)) 
               // SavePath = ServiceHelper.Get_CTZQ_SavePath();
            var path = Path.Combine(SavePath, issuseNumber);
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                this.WriteLog(string.Format("创建目录{0}失败：{1}。", path, ex.ToString()));
            }
            return Path.Combine(path, fileName);
        }

        /// <summary>
        /// 查奖期地址
        /// </summary>
        private string GetIssuseIndexUrl(string gameCode)
        {
            switch (gameCode)
            {
                case "T14C":
                case "TR9":
                    return "http://www.500.com/static/info/sfc/daigou/xml/index.xml";
                case "T6BQC":
                    return "http://www.500.com/static/info/zc6/daigou/xml/index.xml";
                case "T4CJQ":
                    return "http://www.500.com/static/info/jq4/daigou/xml/index.xml";
            }
            return string.Empty;
        }

        /// <summary>
        /// 查奖期地址
        /// </summary>
        private string GetIssuseIndexUrl_9188(string gameCode)
        {
            switch (gameCode)
            {
                case "T14C":
                case "TR9":
                    return "http://www.9188.com/data/phot/80/s.xml";
                case "T6BQC":
                    return "http://www.9188.com/data/phot/83/s.xml";
                case "T4CJQ":
                    return "http://www.9188.com/data/phot/82/s.xml";
            }
            return string.Empty;
        }

        /// <summary>
        /// 查队伍地址
        /// </summary>
        private string GetTeamInfoUrl(string gameCode, string issuseNumber)
        {
            switch (gameCode)
            {
                case "T14C":
                case "TR9":
                    return string.Format("http://www.500.com/static/public/sfc/daigou/xml/{0}.xml", issuseNumber);
                case "T6BQC":
                    return string.Format("http://www.500.com/static/public/zc6/daigou/xml/{0}.xml", issuseNumber);
                case "T4CJQ":
                    return string.Format("http://www.500.com/static/public/jq4/daigou/xml/{0}.xml", issuseNumber);
            }
            return string.Empty;
        }
        /// <summary>
        /// 查队伍地址
        /// </summary>
        private string GetTeamInfoUrl_Okooo(string gameCode, string issuseNumber)
        {
            switch (gameCode)
            {
                case "T14C":
                case "TR9":
                    return string.Format("http://www.okooo.com/zucai/{0}/", issuseNumber);
            }
            return string.Empty;
        }
        /// <summary>
        /// 查队伍地址:中国竞彩
        /// </summary>
        private string GetTeamInfoUrlToZGJC(string gameCode, string issuseNumber)
        {
            //switch (gameCode)
            //{
            //    case "T14C":
            //    case "TR9":
            //        return string.Format("http://info.sporttery.cn/iframe/lottery_iframe_2017.php?key=wilo&num={0}", issuseNumber);
            //    case "T6BQC":
            //        return string.Format("http://info.sporttery.cn/iframe/lottery_iframe_2017.php?key=hafu&num={0}", issuseNumber);
            //    case "T4CJQ":
            //        return string.Format("http://info.sporttery.cn/iframe/lottery_iframe_2017.php?key=goal&num={0}", issuseNumber);
            //}
            //return string.Empty;
            switch (gameCode)
            {
                case "T14C":
                case "TR9"://
                    return string.Format("http://i.sporttery.cn/wap/fb_lottery/fb_lottery_nums?key=wilo&num={0}&f_callback=getNumBack&_=1491028685199", issuseNumber);
                case "T6BQC":
                    return string.Format("http://i.sporttery.cn/wap/fb_lottery/fb_lottery_nums?key=hafu&num={0}&f_callback=getNumBack&_=1491028685199", issuseNumber);
                case "T4CJQ":
                    return string.Format("http://i.sporttery.cn/wap/fb_lottery/fb_lottery_nums?key=goal&num={0}&f_callback=getNumBack&_=1491028685199", issuseNumber);
            }
            return string.Empty;
        }

        /// <summary>
        /// 查队伍地址:澳客
        /// </summary>
        private string GetTeamInfoUrlToOK(string gameCode, string issuseNumber)
        {
            switch (gameCode)
            {
                case "T14C":
                    return string.Format("http://www.okooo.com/zucai/{0}/", issuseNumber);
                case "TR9":
                    return string.Format("http://www.okooo.com/zucai/ren9/{0}/", issuseNumber);
                    //case "T6BQC":
                    //    return string.Format("http://info.sporttery.cn/iframe/lottery_iframe_2013.php?key=hafu&num={0}", issuseNumber);
                    //case "T4CJQ":
                    //    return string.Format("http://info.sporttery.cn/iframe/lottery_iframe_2013.php?key=goal&num={0}", issuseNumber);
            }
            return string.Empty;
        }

        /// <summary>
        /// 切掉html文本
        /// </summary>
        private string CutHtml(string html)
        {
            var index_1 = html.IndexOf("<");
            var index_2 = html.IndexOf(">");
            if (index_1 > index_2)
                html = html.Substring(index_2 + 1);

            var b = new StringBuilder();
            var append = true;
            foreach (var item in html)
            {
                if (item == '\t' || item == '\n') continue;
                if (item == '<')
                {
                    //找到前置符
                    append = false;
                }
                if (item == '>')
                {
                    //找到结束符
                    append = true;
                    continue;
                }
                if (!append) continue;

                b.Append(item);
            }
            return b.ToString().Trim();
        }

        #region 分析相关

        private string GetFXNumberUrl(string gameType, string issuseNumber)
        {
            switch (gameType.ToUpper())
            {
                case "T14C":
                case "TR9":
                    return string.Format("http://www.310win.com/buy/toto14.aspx?issueNum={0}", issuseNumber);
                case "T4CJQ":
                    return string.Format("http://www.310win.com/buy/toto4.aspx?issueNum={0}", issuseNumber);
                case "T6BQC":
                    return string.Format("http://www.310win.com/buy/toto6.aspx?issueNum={0}", issuseNumber);
            }
            return string.Empty;
        }
        /// <summary>
        /// 采集310win的FXId
        /// </summary>
        private Dictionary<string, string> GetCTZQ_FX(string gameType, string issuseNumber)
        {
            string url = GetFXNumberUrl(gameType, "20" + issuseNumber);
            string html = PostManager.PostCustomer(url, string.Empty, Encoding.UTF8, (request) =>
            {
                if (ServiceHelper.IsUseProxy("CTZQ"))
                {
                    var proxy = ServiceHelper.GetProxyUrl();
                    if (!string.IsNullOrEmpty(proxy))
                    {
                        request.Proxy = new System.Net.WebProxy(proxy);
                    }
                }
            });
            if (html == "404")
                return new Dictionary<string, string>();
            switch (gameType)
            {
                case "T14C":
                case "TR9":
                    return GetFXT14CAndTR9(issuseNumber, html);
                case "T4CJQ":
                    return GetFXT4CJQ(issuseNumber, html);
                case "T6BQC":
                    return GetFXT6BQC(issuseNumber, html);
            }
            return new Dictionary<string, string>();
        }

        private Dictionary<string, string> GetFXT14CAndTR9(string issuseNumber, string html)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            for (int Id = 1; Id <= 14; Id++)
            {
                int tableIndex = html.IndexOf("<td>" + Id.ToString() + "</td>");
                html = html.Substring(tableIndex, html.Length - tableIndex);

                var winNumber = string.Empty;
                int winNumberIndex = html.IndexOf("span id=\"HomeOrder_") + "span id=\"HomeOrder_".Length;
                winNumber = html.Substring(winNumberIndex, 6);

                var key = string.Format("{0}_{1}", issuseNumber, Id);
                dic.Add(key, winNumber);

            }
            return dic;
        }

        private Dictionary<string, string> GetFXT4CJQ(string issuseNumber, string html)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            for (int Id = 1; Id <= 4; Id++)
            {
                int tableIndex = html.IndexOf("<td rowspan=\"2\">" + Id.ToString() + "</td>");
                html = html.Substring(tableIndex, html.Length - tableIndex);

                var winNumber = string.Empty;
                int winNumberIndex = html.IndexOf("span id=\"HomeOrder_") + "span id=\"HomeOrder_".Length;
                winNumber = html.Substring(winNumberIndex, 6);

                var key = string.Format("{0}_{1}", issuseNumber, Id);
                dic.Add(key, winNumber);

            }
            return dic;
        }

        private Dictionary<string, string> GetFXT6BQC(string issuseNumber, string html)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            for (int Id = 1; Id <= 6; Id++)
            {
                int tableIndex = html.IndexOf("<td rowspan=\"2\">" + Id.ToString() + "</td>");
                html = html.Substring(tableIndex, html.Length - tableIndex);

                var winNumber = string.Empty;
                int winNumberIndex = html.IndexOf("span id=\"HomeOrder_") + "span id=\"HomeOrder_".Length;
                winNumber = html.Substring(winNumberIndex, 6);

                var key = string.Format("{0}_{1}", issuseNumber, Id);
                dic.Add(key, winNumber);

            }
            return dic;
        }

        /// <summary>
        /// 采集OKOOO的FXId
        /// </summary>
        private Dictionary<string, string> GetCTZQ_FX_OKOOO(string gameType, string issuseNumber)
        {
            var url = string.Empty;
            var html = string.Empty;
            switch (gameType.ToUpper())
            {
                case "T14C":
                case "TR9":
                    url = string.Format("http://www.okooo.com/zucai/{0}/", issuseNumber);
                    html = PostManager.Get(url, Encoding.GetEncoding("GBK"));
                    return GetFXT14AndTr9(html, issuseNumber);
                case "T4CJQ":
                    url = string.Format("http://www.okooo.com/zucai/jinqiu/{0}/", issuseNumber);
                    html = PostManager.Get(url, Encoding.GetEncoding("GBK"));
                    return GetT4CJQ(html, issuseNumber);
                case "T6BQC":
                    url = string.Format("http://www.okooo.com/zucai/liuban/{0}/", issuseNumber);
                    html = PostManager.Get(url, Encoding.GetEncoding("GBK"));
                    return GetT6BQC(html, issuseNumber);
            }
            return new Dictionary<string, string>();
        }

        private Dictionary<string, string> GetFXT14AndTr9(string html, string issuseNumber)
        {
            var dic = new Dictionary<string, string>();
            var target = "<div class=\"jcmian zucaidiv\" id=\"gametablesend\"";
            var index = html.IndexOf(target);
            html = html.Substring(index + target.Length);

            target = "<div id=\"betbottom\"";
            index = html.IndexOf(target);
            html = html.Substring(0, index);
            var tableArray = html.Split(new string[] { "<table " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in tableArray)
            {
                if (string.IsNullOrEmpty(item)) continue;
                var trArray = item.Split(new string[] { "<tr id=" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var tr in trArray)
                {
                    if (string.IsNullOrEmpty(tr)) continue;
                    //if (tr.IndexOf("id=\"tr") < 0) continue;

                    var matchId = string.Empty;
                    var fxId = string.Empty;
                    var tdArray = tr.Split(new string[] { "<td" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var td in tdArray)
                    {
                        var temp = string.Empty;
                        target = "class=\"xh td1\">";
                        index = td.IndexOf(target);
                        if (index >= 0)
                        {
                            //取编号
                            temp = td.Substring(index + target.Length);
                            target = "</td>";
                            index = temp.IndexOf(target);
                            temp = temp.Substring(0, index);

                            matchId = string.Format("{0}_{1}", issuseNumber, temp);
                        }
                        target = "href=\"/soccer/match/";
                        index = td.IndexOf(target);
                        if (index >= 0)
                        {
                            //取FXId
                            temp = td.Substring(index + target.Length);
                            target = "/odds/";
                            index = temp.IndexOf(target);
                            temp = temp.Substring(0, index);
                            fxId = temp;
                            break;
                        }
                    }
                    if (!string.IsNullOrEmpty(matchId) && !string.IsNullOrEmpty(fxId))
                        dic.Add(matchId, fxId);
                }
            }
            return dic;
        }

        private Dictionary<string, string> GetT4CJQ(string html, string issuseNumber)
        {
            var dic = new Dictionary<string, string>();
            var target = "<div class=\"jcmian zucaidiv\" id=\"gametablesend\"";
            var index = html.IndexOf(target);
            html = html.Substring(index + target.Length);

            target = "<div id=\"betbottom\"";
            index = html.IndexOf(target);
            html = html.Substring(0, index);
            var tableArray = html.Split(new string[] { "<table " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in tableArray)
            {
                if (string.IsNullOrEmpty(item)) continue;
                var trArray = item.Split(new string[] { "<tr class=\"alltrObj" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 1; i < trArray.Length; i++)
                {
                    var tr = trArray[i];
                    if (string.IsNullOrEmpty(tr)) continue;
                    if (i % 2 != 0) continue;
                    var matchId = string.Empty;
                    var fxId = string.Empty;

                    matchId = string.Format("{0}_{1}", issuseNumber, (i / 2).ToString());
                    var temp = string.Empty;
                    target = "matchId=\"";
                    index = tr.IndexOf(target);
                    temp = tr.Substring(index + target.Length);
                    target = "\" id=";
                    index = temp.IndexOf(target);
                    fxId = temp.Substring(0, index);
                    dic.Add(matchId, fxId);
                }
            }
            return dic;
        }

        private Dictionary<string, string> GetT6BQC(string html, string issuseNumber)
        {
            var dic = new Dictionary<string, string>();
            var target = "<div class=\"jcmian zucaidiv\" id=\"gametablesend\"";
            var index = html.IndexOf(target);
            html = html.Substring(index + target.Length);

            target = "<div id=\"betbottom\"";
            index = html.IndexOf(target);
            html = html.Substring(0, index);
            var tableArray = html.Split(new string[] { "<table " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in tableArray)
            {
                if (string.IsNullOrEmpty(item)) continue;
                var trArray = item.Split(new string[] { "<tr class=\"alltrObj" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 1; i < trArray.Length; i++)
                {
                    var tr = trArray[i];
                    if (string.IsNullOrEmpty(tr)) continue;
                    if (i % 2 != 0) continue;
                    var matchId = string.Empty;
                    var fxId = string.Empty;

                    matchId = string.Format("{0}_{1}", issuseNumber, (i / 2).ToString());
                    var temp = string.Empty;
                    target = "matchId=\"";
                    index = tr.IndexOf(target);
                    temp = tr.Substring(index + target.Length);
                    target = "\" id=";
                    index = temp.IndexOf(target);
                    fxId = temp.Substring(0, index);
                    dic.Add(matchId, fxId);
                }
            }
            return dic;
        }
        #endregion
    }
}
