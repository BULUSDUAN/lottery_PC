using EntityModel.Domain.Entities;
using EntityModel.ExceptionExtend;
using KaSon.FrameWork.Common.Expansion;
using KaSon.FrameWork.Common.JSON;
using KaSon.FrameWork.Common.Net;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Lottery.CrawGetters.MatchBizGetter
{
  
    /// <summary>
    /// 采集欧赔
    /// </summary>
    public class JCZQ_OZSP_AutoCollect 
    {
      
        private const string logCategory = "Services.Info";
        private string logInfoSource = "Auto_Collect_JCZQ_OZSP_Info_";
        private const string logErrorCategory = "Services.Error";
        private const string logErrorSource = "Auto_Collect_JCZQ_OZSP_Error";

        private long BeStop = 0;
        private System.Timers.Timer timer = null;
        private string SavePath = string.Empty;



        private IMongoDatabase mDB;
        public JCZQ_OZSP_AutoCollect(IMongoDatabase _mDB)
        {
            mDB = _mDB;
        }

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


                        DoWork(gameCode);

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

        public void DoWork(string gameCode)
        {
            this.WriteLog("进入DoWork  开始采集数据");
            try
            {
                Save_OZ_SPInfo<JCZQ_SPF_WLXE_SPInfo>(Get_OZ_SPF_SPInfo<JCZQ_SPF_WLXE_SPInfo>("WLXE"), "wlxe_SP.json", "WLXE");
                Save_OZ_SPInfo<JCZQ_SPF_AM_SPInfo>(Get_OZ_SPF_SPInfo<JCZQ_SPF_AM_SPInfo>("AM"), "am_SP.json", "AM");
                Save_OZ_SPInfo<JCZQ_SPF_LB_SPInfo>(Get_OZ_SPF_SPInfo<JCZQ_SPF_LB_SPInfo>("LB"), "lb_SP.json", "LB");
                Save_OZ_SPInfo<JCZQ_SPF_Bet365_SPInfo>(Get_OZ_SPF_SPInfo<JCZQ_SPF_Bet365_SPInfo>("Bet365"), "bet365_SP.json", "Bet365");
                Save_OZ_SPInfo<JCZQ_SPF_SNAI_SPInfo>(Get_OZ_SPF_SPInfo<JCZQ_SPF_SNAI_SPInfo>("SNAI"), "snai_SP.json", "SNAI");
                Save_OZ_SPInfo<JCZQ_SPF_YDS_SPInfo>(Get_OZ_SPF_SPInfo<JCZQ_SPF_YDS_SPInfo>("YDS"), "yds_SP.json", "YDS");
                Save_OZ_SPInfo<JCZQ_SPF_WD_SPInfo>(Get_OZ_SPF_SPInfo<JCZQ_SPF_WD_SPInfo>("WD"), "wd_SP.json", "WD");
                Save_OZ_SPInfo<JCZQ_SPF_Bwin_SPInfo>(Get_OZ_SPF_SPInfo<JCZQ_SPF_Bwin_SPInfo>("Bwin"), "bwin_SP.json", "Bwin");
                Save_OZ_SPInfo<JCZQ_SPF_Coral_SPInfo>(Get_OZ_SPF_SPInfo<JCZQ_SPF_Coral_SPInfo>("Coral"), "coral_SP.json", "Coral");
                Save_OZ_SPInfo<JCZQ_SPF_Oddset_SPInfo>(Get_OZ_SPF_SPInfo<JCZQ_SPF_Oddset_SPInfo>("Oddset"), "oddset_SP.json", "Oddset");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            this.WriteLog("DoWork  完成");
        }

        #region 采集欧赔SP

        private List<T> Get_OZ_SPF_SPInfo<T>(string category) where T : JCZQ_SPF_OZ_SPInfo, new()
        {
            var xml = Get_OZ_SPXmlContent(category);
            var list = new List<T>();
            if (string.IsNullOrEmpty(xml))
                return list;
            if (xml == "404") return list;

            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var root = doc.SelectSingleNode("xml");
            if (root == null)
                throw new Exception("从xml中查询节点错误  - " + xml);
            foreach (XmlNode item in root.ChildNodes)
            {
                var matchId = item.Attributes["xid"].Value;
                if (string.IsNullOrEmpty(matchId))
                    continue;

                var matchData = matchId.Substring(0, 6);
                var matchNumber = matchId.Substring(6);

                list.Add(new T
                {
                    MatchId = matchId,
                    MatchData = matchData,
                    MatchNumber = matchNumber,
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),

                    WinOdds = item.Attributes["oh"].Value.GetDecimal(),
                    FlatOdds = item.Attributes["od"].Value.GetDecimal(),
                    LoseOdds = item.Attributes["oa"].Value.GetDecimal(),
                    OddsMid = item.Attributes["oddsmid"].Value,
                    Flag = item.Attributes["cflag"].Value,
                });
            }
            return list;
        }

        /// <summary>
        /// 查询欧赔
        /// </summary>
        private string Get_OZ_SPXmlContent(string category)
        {
            var url = string.Empty;
            switch (category)
            {
                case "WLXE":
                    url = "http://jc.cpdyj.com/api/getzcodds?lotyid=6&qh=&cid=1&_={0}";
                    break;
                case "AM":
                    url = "http://jc.cpdyj.com/api/getzcodds?lotyid=6&qh=&cid=2&_={0}";
                    break;
                case "LB":
                    url = "http://jc.cpdyj.com/api/getzcodds?lotyid=6&qh=&cid=3&_={0}";
                    break;
                case "Bet365":
                    url = "http://jc.cpdyj.com/api/getzcodds?lotyid=6&qh=&cid=4&_={0}";
                    break;
                case "SNAI":
                    url = "http://jc.cpdyj.com/api/getzcodds?lotyid=6&qh=&cid=5&_={0}";
                    break;
                case "YDS":
                    url = "http://jc.cpdyj.com/api/getzcodds?lotyid=6&qh=&cid=6&_={0}";
                    break;
                case "WD":
                    url = "http://jc.cpdyj.com/api/getzcodds?lotyid=6&qh=&cid=7&_={0}";
                    break;
                case "Bwin":
                    url = "http://jc.cpdyj.com/api/getzcodds?lotyid=6&qh=&cid=8&_={0}";
                    break;
                case "Coral":
                    url = "http://jc.cpdyj.com/api/getzcodds?lotyid=6&qh=&cid=9&_={0}";
                    break;
                case "Oddset":
                    url = "http://jc.cpdyj.com/api/getzcodds?lotyid=6&qh=&cid=10&_={0}";
                    break;
            }

            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long tt = (DateTime.Now.Ticks - startTime.Ticks) / 10000;
            url = string.Format(url, tt);
            var encoding = Encoding.GetEncoding("gb2312");
            var content = PostManager.Get(url, encoding, 0, (request) =>
            {
                //request.Host = "intf.cpdyj.com";
                //request.Referer = "http://jc.cpdyj.com/index.html";

                if (ServiceHelper.IsUseProxy("JCZQ"))
                {
                    var proxy = ServiceHelper.GetProxyUrl();
                    if (!string.IsNullOrEmpty(proxy))
                    {
                        request.Proxy = new System.Net.WebProxy(proxy);
                    }
                }
            });
            //content = content.Replace(replaceFlag, "").Replace(");", "");

            return content;
        }

        /// <summary>
        /// 创建文件全路径
        /// </summary>
        //private string BuildFileFullName(string fileName)
        //{
        //    if (string.IsNullOrEmpty(SavePath))
        //        SavePath = ServiceHelper.Get_JCZQ_SavePath();
        //    //var path = Path.Combine(SavePath, issuseNumber);
        //    try
        //    {
        //        if (!Directory.Exists(SavePath))
        //            Directory.CreateDirectory(SavePath);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.WriteLog(string.Format("创建目录{0}失败：{1}。", SavePath, ex.ToString()));
        //    }
        //    return Path.Combine(SavePath, fileName);
        //}
        /// <summary>
        /// 保存OZ SP
        /// </summary>
        private void Save_OZ_SPInfo<T>(List<T> list, string tablename, string category) where T : JCZQ_SPF_OZ_SPInfo
        {
         //   var fileFullName = BuildFileFullName(fileName);
            if (list.Count == 0) return;
            var customerSavePath = new string[] { "JCZQ" };
            //不是平均赔率 采集变化历史
            if (!string.IsNullOrEmpty(category))
            {
                #region 保存SP走势数据

                foreach (var item in list)
                {
                    try
                    {
                        if (item.WinOdds == 0 || item.FlatOdds == 0 || item.LoseOdds == 0) continue;

                        var url = string.Empty;
                        #region 获取URL
                        switch (category)
                        {
                            case "WLXE":
                                url = "http://jc.cpdyj.com/staticdata/oddsinfo/jc/ouzhi01/{1}.xml?_={0}";
                                break;
                            case "AM":
                                url = "http://jc.cpdyj.com/staticdata/oddsinfo/jc/ouzhi02/{1}.xml?_={0}";
                                break;
                            case "LB":
                                url = "http://jc.cpdyj.com/staticdata/oddsinfo/jc/ouzhi03/{1}.xml?_={0}";
                                break;
                            case "Bet365":
                                url = "http://jc.cpdyj.com/staticdata/oddsinfo/jc/ouzhi04/{1}.xml?_={0}";
                                break;
                            case "SNAI":
                                url = "http://jc.cpdyj.com/staticdata/oddsinfo/jc/ouzhi05/{1}.xml?_={0}";
                                break;
                            case "YDS":
                                url = "http://jc.cpdyj.com/staticdata/oddsinfo/jc/ouzhi06/{1}.xml?_={0}";
                                break;
                            case "WD":
                                url = "http://jc.cpdyj.com/staticdata/oddsinfo/jc/ouzhi07/{1}.xml?_={0}";
                                break;
                            case "Bwin":
                                url = "http://jc.cpdyj.com/staticdata/oddsinfo/jc/ouzhi08/{1}.xml?_={0}";
                                break;
                            case "Coral":
                                url = "http://jc.cpdyj.com/staticdata/oddsinfo/jc/ouzhi09/{1}.xml?_={0}";
                                break;
                            case "Oddset":
                                url = "http://jc.cpdyj.com/staticdata/oddsinfo/jc/ouzhi10/{1}.xml?_={0}";
                                break;
                        }
                        #endregion

                        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
                        long tt = (DateTime.Now.Ticks - startTime.Ticks) / 10000;
                        url = string.Format(url, tt, item.OddsMid);
                        var encoding = Encoding.GetEncoding("gb2312");
                        var spTrendXml = PostManager.Get(url, encoding, 0, (request) =>
                        {
                            request.Host = "intf.cpdyj.com";
                            request.Referer = "http://jc.cpdyj.com/index.html";
                            if (ServiceHelper.IsUseProxy("JCZQ"))
                            {
                                var proxy = ServiceHelper.GetProxyUrl();
                                if (!string.IsNullOrEmpty(proxy))
                                {
                                    request.Proxy = new System.Net.WebProxy(proxy);
                                }
                            }
                        });

                        if (string.IsNullOrEmpty(spTrendXml))
                            continue;
                        if (spTrendXml == "404")
                            continue;

                        var doc = new XmlDocument();
                        doc.LoadXml(spTrendXml);
                        var root = doc.SelectSingleNode("xml");
                        if (root == null)
                            throw new Exception("从xml中查询节点错误  - " + spTrendXml);

                        var trendList = new List<JCZQ_SPF_SP_Trend>();
                        foreach (XmlNode t in root.ChildNodes)
                        {
                            //<row oh="2.00" od="3.10" oa="3.50" tp="1" gtime="2013-05-31 13:58"></row>
                            var win = t.Attributes["oh"].Value.GetDecimal();
                            var flat = t.Attributes["od"].Value.GetDecimal();
                            var lose = t.Attributes["oa"].Value.GetDecimal();
                            var tp = t.Attributes["tp"].Value.GetInt32();
                            var gtime = t.Attributes["gtime"].Value.GetDateTime();

                            trendList.Add(new JCZQ_SPF_SP_Trend
                            {
                                CreateTime = gtime.ToString("yyyy-MM-dd HH:mm:ss"),
                                OddsMid = item.OddsMid,
                                TP = tp,
                                WinOdds = win,
                                FlatOdds = flat,
                                LoseOdds = lose,
                            });
                        }
                        if (trendList.Count != 0)
                        {
                            //写入文件
                           // var trendFullName = BuildFileFullName(string.Format("{0}_{1}_SP.json", category.ToLower(), item.OddsMid));
                            try
                            {
                              var coll=  mDB.GetCollection<BsonDocument>("JCZQ_SPF_SP_Trend");

                                BsonDocument bdDoc = new BsonDocument();
                                bdDoc.Add("Category", category.ToLower());
                                bdDoc.Add("OddsMid", item.OddsMid);
                                bdDoc.Add("Content", JsonHelper .Serialize(trendList));
                               var mFilter = MongoDB.Driver.Builders<BsonDocument>.Filter.Eq("Category", category.ToLower()) &
                                    Builders<BsonDocument>.Filter.Eq("OddsMid", item.OddsMid);

                                coll.DeleteMany(mFilter);
                                coll.InsertOne(bdDoc);

                                //ServiceHelper.CreateOrAppend_JSONFile(trendFullName, JsonSerializer.Serialize(trendList), (log) =>
                                //{
                                //    this.WriteLog(log);
                                //});

                                ////上传文件
                                //ServiceHelper.PostFileToServer(trendFullName, customerSavePath, (log) =>
                                //{
                                //    this.WriteLog(log);
                                //});

                            }
                            catch (Exception ex)
                            {
                               this.WriteLog(string.Format("写入 OZ Trend SP 数据文件 {0} 失败：{1}", tablename, ex.ToString()));
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                      this.WriteLog(string.Format("写入 OZ SP 数据文件 {0} 失败：{1}", tablename, ex.ToString()));
                    }
                }

                #endregion
            }
            try
            {
                var coll = mDB.GetCollection<T>(tablename);

                //BsonDocument bdDoc = new BsonDocument();
                //bdDoc.Add("Category", category.ToLower());
                //bdDoc.Add("OddsMid", item.OddsMid);
                //bdDoc.Add("Content", JsonHelper.Serialize(trendList));
                //var mFilter = MongoDB.Driver.Builders<BsonDocument>.Filter.Eq("Category", category.ToLower()) &
                //     Builders<BsonDocument>.Filter.Eq("OddsMid", item.OddsMid);

                coll.DeleteMany(null);
                coll.InsertMany(list);
                //ServiceHelper.CreateOrAppend_JSONFile(fileFullName, JsonSerializer.Serialize(list), (log) =>
                //{
                //    this.WriteLog(log);
                //});

                ////上传文件
                //ServiceHelper.PostFileToServer(fileFullName, customerSavePath, (log) =>
                //{
                //    this.WriteLog(log);
                //});
            }
            catch (Exception ex)
            {
                this.WriteLog(string.Format("写入 OZ SP 数据文件 {0} 失败：{1}", tablename, ex.ToString()));
            }
        }

        #endregion

        public void Stop()
        {
            BeStop = 1;
            if (timer != null)
                timer.Stop();
        }

        public void WriteError(string log)
        {
            //if (_logWriter != null)
            //    _logWriter.Write(logErrorCategory, logErrorSource, LogType.Error, "自动采集竞彩足球队伍数据", log);


        }

        public void WriteLog(string log)
        {
            //if (_logWriter != null)
            //    _logWriter.Write(logCategory, logInfoSource, LogType.Information, "自动采集竞彩足球队伍数据", log);


        }
    }
}
