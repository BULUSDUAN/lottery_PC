using EntityModel.CoreModel;
using EntityModel.ExceptionExtend;
using KaSon.FrameWork.Common.Expansion;
using KaSon.FrameWork.Common.Net;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Lottery.CrawGetters.MatchBizGetter
{
 
    public class BJDC_OZSP_AutoCollect : IBallAutoCollect
    {
       // private ILogWriter _logWriter = null;
        private const string logCategory = "Services.Info";
        private string logInfoSource = "Auto_Collect_BJDC_OZSP_Info_";
        private const string logErrorCategory = "Services.Error";
        private const string logErrorSource = "Auto_Collect_BJDC_OZSP_Error";
        private long BeStop = 0;
        private System.Timers.Timer timer = null;
        private string Sp_SavePath = string.Empty;
        private IMongoDatabase mDB;
        public BJDC_OZSP_AutoCollect(IMongoDatabase _mDB)
        {
            mDB = _mDB;
        }
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


                        DoWork(gameCode);

                    }
                    catch (Exception ex)
                    {
                        WriteLog(ex.Message);
                        Thread.Sleep(2000);
                    }
                    finally
                    {
                        Thread.Sleep(2000);
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
                //var maxIssuseCount = 1;
                ////取期号http://jc.cpdyj.com/index.html";
                //System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
                //long tt = (DateTime.Now.Ticks - startTime.Ticks) / 10000;
                //var url = string.Format("http://intf.cpdyj.com/data/dc/issue.js?callback=expe&_={0}", tt);
                //var encoding = Encoding.GetEncoding("gb2312");
                //var content = PostManager.Get(url, encoding, 0, (request) =>
                //{
                //    request.Host = "intf.cpdyj.com";
                //    request.Referer = "http://bd.cpdyj.com/";
                //    if (ServiceHelper.IsUseProxy("BJDC"))
                //    {
                //        var proxy = ServiceHelper.GetProxyUrl();
                //        if (!string.IsNullOrEmpty(proxy))
                //        {
                //            request.Proxy = new System.Net.WebProxy(proxy);
                //        }
                //    }
                //});
                //content = content.Replace("expe(", "").Replace(");", "").Trim();
                //var issuseStrList = new List<string>();
                //var array = System.Web.Helpers.Json.Decode(content);
                //foreach (var item in array)
                //{
                //    if (issuseStrList.Count >= maxIssuseCount)
                //        break;
                //    issuseStrList.Add(item[0]);
                //}
                var issuseStrList = new List<string>();
                var getIssuseUrl = "http://www.9188.com/data/phot/85/c.xml";
                var xml = PostManager.Get(getIssuseUrl, Encoding.UTF8, 0, (request) =>
                {
                    if (ServiceHelper.IsUseProxy("BJDC"))
                    {
                        var proxy = ServiceHelper.GetProxyUrl();
                        if (!string.IsNullOrEmpty(proxy))
                        {
                            request.Proxy = new System.Net.WebProxy(proxy);
                        }
                    }
                });
                if (string.IsNullOrEmpty(xml))
                    throw new Exception(string.Format("请求地址：{0}返回数据为空", getIssuseUrl));
                var doc = new XmlDocument();
                doc.LoadXml(xml);
                var root = doc.SelectSingleNode("Resp");
                if (root == null)
                    throw new Exception("从xml中查询节点错误  - " + xml);


                foreach (XmlNode item in root.ChildNodes)
                {
                    var matchId = item.Attributes["pid"].Value;
                    if (string.IsNullOrEmpty(matchId))
                        continue;

                    issuseStrList.Add(matchId);
                }



                foreach (var currentIssuseNumber in issuseStrList)
                {

                    #region 保存OZSP

                    Save_OZ_SPInfo<BJDC_SPF_WLXE_SPInfo>(Get_OZ_SPF_SPInfo<BJDC_SPF_WLXE_SPInfo>("WLXE"), "wlxe_SP.json", "WLXE", currentIssuseNumber);
                    Save_OZ_SPInfo<BJDC_SPF_AM_SPInfo>(Get_OZ_SPF_SPInfo<BJDC_SPF_AM_SPInfo>("AM"), "am_SP.json", "AM", currentIssuseNumber);
                    Save_OZ_SPInfo<BJDC_SPF_LB_SPInfo>(Get_OZ_SPF_SPInfo<BJDC_SPF_LB_SPInfo>("LB"), "lb_SP.json", "LB", currentIssuseNumber);
                    Save_OZ_SPInfo<BJDC_SPF_Bet365_SPInfo>(Get_OZ_SPF_SPInfo<BJDC_SPF_Bet365_SPInfo>("Bet365"), "bet365_SP.json", "Bet365", currentIssuseNumber);
                    Save_OZ_SPInfo<BJDC_SPF_SNAI_SPInfo>(Get_OZ_SPF_SPInfo<BJDC_SPF_SNAI_SPInfo>("SNAI"), "snai_SP.json", "SNAI", currentIssuseNumber);
                    Save_OZ_SPInfo<BJDC_SPF_YDS_SPInfo>(Get_OZ_SPF_SPInfo<BJDC_SPF_YDS_SPInfo>("YDS"), "yds_SP.json", "YDS", currentIssuseNumber);
                    Save_OZ_SPInfo<BJDC_SPF_WD_SPInfo>(Get_OZ_SPF_SPInfo<BJDC_SPF_WD_SPInfo>("WD"), "wd_SP.json", "WD", currentIssuseNumber);
                    Save_OZ_SPInfo<BJDC_SPF_Bwin_SPInfo>(Get_OZ_SPF_SPInfo<BJDC_SPF_Bwin_SPInfo>("Bwin"), "bwin_SP.json", "Bwin", currentIssuseNumber);
                    Save_OZ_SPInfo<BJDC_SPF_Coral_SPInfo>(Get_OZ_SPF_SPInfo<BJDC_SPF_Coral_SPInfo>("Coral"), "coral_SP.json", "Coral", currentIssuseNumber);
                    Save_OZ_SPInfo<BJDC_SPF_Oddset_SPInfo>(Get_OZ_SPF_SPInfo<BJDC_SPF_Oddset_SPInfo>("Oddset"), "oddset_SP.json", "Oddset", currentIssuseNumber);
                    //todo 格式不一样  要重新解析
                    //Save_OZ_SPInfo<BJDC_SPF_TZBL_SPInfo>(Get_OZ_SPF_SPInfo<BJDC_SPF_TZBL_SPInfo>("TZBL"), "tzbl_SP.json", "Oddset", currentIssuseNumber); 


                    #endregion
                }

            }
            catch (Exception ex)
            {
                this.WriteLog(ex.ToString());
                throw ex;
            }
            this.WriteLog("DoWork  完成");
        }
        /// <summary>
        /// 字符处理
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="s3"></param>
        /// <returns></returns>
        private string substringH(string s1, string s2, string s3)
        {
            int p1 = 0;
            int p2 = 0;
            try
            {
                p1 = s1.IndexOf(s2);
                p2 = s1.IndexOf(s3, p1 + s2.Length);
                if (p1 < 0)
                    return null;
                if (p2 < 0)
                    return null;
                if (p2 < p1)
                    return null;
                return s1.Substring(p1 + s2.Length, p2 - p1 - s2.Length);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 创建文件全路径
        /// </summary>
        //private string BuildFileFullName(string fileName, string issuseNumber)
        //{
        //    if (string.IsNullOrEmpty(Sp_SavePath))
        //        Sp_SavePath = ServiceHelper.Get_BJDC_SPSavePath();
        //    var path = Path.Combine(Sp_SavePath, issuseNumber);
        //    try
        //    {
        //        if (!Directory.Exists(path))
        //            Directory.CreateDirectory(path);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.WriteLog(string.Format("创建目录{0}失败：{1}。", path, ex.ToString()));
        //    }
        //    return Path.Combine(path, fileName);
        //}

        /// <summary>
        /// 保存OZ SP
        /// </summary>
        private void Save_OZ_SPInfo<T>(List<T> list, string fileName, string category, string issuseNumber) where T : BJDC_SPF_OZ_SPInfo
        {
            var baseTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));//.AddSeconds(1366558200)

          //  var fileFullName = BuildFileFullName(fileName, issuseNumber);
            if (list.Count == 0) return;
            //不是平均赔率 采集变化历史
            if (!string.IsNullOrEmpty(category))
            {
                #region 保存SP走势数据

                foreach (var item in list)
                {
                    try
                    {
                        if (item.Win_Odds == 0 || item.Flat_Odds == 0 || item.Lose_Odds == 0) continue;

                        var url = string.Empty;
                        #region 获取URL
                        switch (category)
                        {
                            case "WLXE":
                                url = "http://jc.cpdyj.com/staticdata/oddsinfo/newbd/ouzhi01/{1}.xml?_={0}";
                                break;
                            case "AM":
                                url = "http://jc.cpdyj.com/staticdata/oddsinfo/newbd/ouzhi02/{1}.xml?_={0}";
                                break;
                            case "LB":
                                url = "http://jc.cpdyj.com/staticdata/oddsinfo/newbd/ouzhi03/{1}.xml?_={0}";
                                break;
                            case "Bet365":
                                url = "http://jc.cpdyj.com/staticdata/oddsinfo/newbd/ouzhi04/{1}.xml?_={0}";
                                break;
                            case "SNAI":
                                url = "http://jc.cpdyj.com/staticdata/oddsinfo/newbd/ouzhi05/{1}.xml?_={0}";
                                break;
                            case "YDS":
                                url = "http://jc.cpdyj.com/staticdata/oddsinfo/newbd/ouzhi06/{1}.xml?_={0}";
                                break;
                            case "WD":
                                url = "http://jc.cpdyj.com/staticdata/oddsinfo/newbd/ouzhi07/{1}.xml?_={0}";
                                break;
                            case "Bwin":
                                url = "http://jc.cpdyj.com/staticdata/oddsinfo/newbd/ouzhi08/{1}.xml?_={0}";
                                break;
                            case "Coral":
                                url = "http://jc.cpdyj.com/staticdata/oddsinfo/newbd/ouzhi09/{1}.xml?_={0}";
                                break;
                            case "Oddset":
                                url = "http://jc.cpdyj.com/staticdata/oddsinfo/newbd/ouzhi10/{1}.xml?_={0}";
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
                            if (ServiceHelper.IsUseProxy("BJDC"))
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

                        var trendList = new List<BJDC_SPF_SP_Trend>();
                        foreach (XmlNode t in root.ChildNodes)
                        {
                            //<row oh="3.20" od="3.00" oa="2.25" tp="1" gtime="1370047821"></row>
                            var win = t.Attributes["oh"].Value.GetDecimal();
                            var flat = t.Attributes["od"].Value.GetDecimal();
                            var lose = t.Attributes["oa"].Value.GetDecimal();
                            var tp = t.Attributes["tp"].Value.GetInt32();
                            var gtime = baseTime.AddSeconds(t.Attributes["gtime"].Value.GetInt32());

                            trendList.Add(new BJDC_SPF_SP_Trend
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
                         var coll=   mDB.GetCollection<BJDC_SPF_SP_Trend>("BJDC_SPF_SP_"+ category);
                        var mFilter =Builders<BJDC_SPF_SP_Trend>.Filter.Eq(b => b.OddsMid, item.OddsMid) 
                                & Builders<BJDC_SPF_SP_Trend>.Filter.Eq(b => b.issuseNumber, issuseNumber);
                            foreach (var item1 in trendList)
                            {
                                item1.issuseNumber = issuseNumber;
                            }

                            coll.DeleteMany(mFilter);
                            coll.InsertMany(trendList);

                            //var trendFullName = BuildFileFullName(string.Format("{0}_{1}_SP.json", category.ToLower(), item.OddsMid), issuseNumber);
                            //try
                            //{
                            //    ServiceHelper.CreateOrAppend_JSONFile(trendFullName, JsonSerializer.Serialize(trendList), (log) =>
                            //    {
                            //        this.WriteLog(log);
                            //    });
                            //}
                            //catch (Exception ex)
                            //{
                            //    this.WriteLog(string.Format("写入 OZ Trend SP 数据文件 {0} 失败：{1}", fileFullName, ex.ToString()));
                            //}

                            var customerSavePath = new string[] { "BJDC", issuseNumber };
                            //上传文件
                            //ServiceHelper.PostFileToServer(trendFullName, customerSavePath, (log) =>
                            //{
                            //    this.WriteLog(log);
                            //});
                        }

                    }
                    catch (Exception ex)
                    {
                     //   this.WriteLog(string.Format("写入 OZ SP 数据文件 {0} 失败：{1}", fileFullName, ex.ToString()));
                    }
                }

                #endregion
            }
            try
            {

                var coll = mDB.GetCollection<BJDC_SPF_OZ_SPInfo>("BJDC_SPF_OZ_SPInfo");
                var mFilter = Builders<BJDC_SPF_OZ_SPInfo>.Filter.Eq(b => b.IssuseNumber, issuseNumber);
                foreach (var item1 in list)
                {
                    item1.IssuseNumber = issuseNumber;
                }

                coll.DeleteMany(mFilter);
                coll.InsertMany(list);

                //ServiceHelper.CreateOrAppend_JSONFile(fileFullName, JsonSerializer.Serialize(list), (log) =>
                //{
                //    this.WriteLog(log);
                //});

                var customerSavePath = new string[] { "BJDC", issuseNumber };
                //上传文件
                //ServiceHelper.PostFileToServer(fileFullName, customerSavePath, (log) =>
                //{
                //    this.WriteLog(log);
                //});
            }
            catch (Exception ex)
            {
               // this.WriteLog(string.Format("写入 OZ SP 数据文件 {0} 失败：{1}", fileFullName, ex.ToString()));
            }
        }
        private List<T> Get_OZ_SPF_SPInfo<T>(string category) where T : BJDC_SPF_OZ_SPInfo, new()
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
            var issuseNumber = root.Attributes["expect"].Value;
            foreach (XmlNode item in root.ChildNodes)
            {
                var matchId = item.Attributes["xid"].Value;
                if (string.IsNullOrEmpty(matchId))
                    continue;

                list.Add(new T
                {
                    IssuseNumber = issuseNumber,
                    MatchOrderId = matchId.GetInt32(),
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),

                    Win_Odds = item.Attributes["oh"].Value.GetDecimal(),
                    Flat_Odds = item.Attributes["od"].Value.GetDecimal(),
                    Lose_Odds = item.Attributes["oa"].Value.GetDecimal(),
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
                    url = "http://bd.cpdyj.com/staticdata/oddsinfo/newbd/ouzhi01/bd.xml?_={0}";
                    break;
                case "AM":
                    url = "http://jc.cpdyj.com/staticdata/oddsinfo/newbd/ouzhi02/bd.xml?_={0}";
                    break;
                case "LB":
                    url = "http://jc.cpdyj.com/staticdata/oddsinfo/newbd/ouzhi03/bd.xml?_={0}";
                    break;
                case "Bet365":
                    url = "http://jc.cpdyj.com/staticdata/oddsinfo/newbd/ouzhi04/bd.xml?_={0}";
                    break;
                case "SNAI":
                    url = "http://jc.cpdyj.com/staticdata/oddsinfo/newbd/ouzhi05/bd.xml?_={0}";
                    break;
                case "YDS":
                    url = "http://jc.cpdyj.com/staticdata/oddsinfo/newbd/ouzhi06/bd.xml?_={0}";
                    break;
                case "WD":
                    url = "http://jc.cpdyj.com/staticdata/oddsinfo/newbd/ouzhi07/bd.xml?_={0}";
                    break;
                case "Bwin":
                    url = "http://jc.cpdyj.com/staticdata/oddsinfo/newbd/ouzhi08/bd.xml?_={0}";
                    break;
                case "Coral":
                    url = "http://jc.cpdyj.com/staticdata/oddsinfo/newbd/ouzhi09/bd.xml?_={0}";
                    break;
                case "Oddset":
                    url = "http://jc.cpdyj.com/staticdata/oddsinfo/newbd/ouzhi10/bd.xml?_={0}";
                    break;
                case "TZBL":
                    url = "http://bd.cpdyj.com/staticdata/oddsinfo/newbd/bdstat.xml?_=1370078649221";
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
                if (ServiceHelper.IsUseProxy("BJDC"))
                {
                    var proxy = ServiceHelper.GetProxyUrl();
                    if (!string.IsNullOrEmpty(proxy))
                    {
                        request.Proxy = new System.Net.WebProxy(proxy);
                    }
                }
            });
            return content;
        }

        public void Stop()
        {
            BeStop = 1;

            if (timer != null)
                timer.Stop();
        }

        public void WriteError(string log)
        {
            Console.WriteLine(log);
            //if (_logWriter != null)
            //    _logWriter.Write(logErrorCategory, logErrorSource, LogType.Error, "自动采集北京单场数据异常", log);
        }

        public void WriteLog(string log)
        {
            Console.WriteLine(log);
            //if (_logWriter != null)
            //    _logWriter.Write(logCategory, logInfoSource, LogType.Information, "自动采集北京单场数据", log);
        }
    }
}
