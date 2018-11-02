using EntityModel;
using EntityModel.Enum;
using EntityModel.ExceptionExtend;
using EntityModel.Interface;
using EntityModel.LotteryJsonInfo;
using KaSon.FrameWork.Common.Expansion;
using KaSon.FrameWork.Common.Net;
using KaSon.FrameWork.ORM.Helper;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Lottery.CrawGetters.MatchBizGetter
{
    internal class JCZQMatchResultCache
    {
        public string Id { get; set; }
        public int CacheTimes { get; set; }
        public C_JCZQ_MatchResult MatchResult { get; set; }
    }
    public class JCZQMatchResult_AutoCollect : BaseAutoCollect, IAutoCollect
    {
     //   private ILogWriter _logWriter = null;
        private const string logCategory = "Services.Info";
        private string logInfoSource = "Auto_Collect_JCZQMatchResult_Info_";
        private const string logErrorCategory = "Services.Error";
        private const string logErrorSource = "Auto_Collect_JCZQMatchResult_Error";

       
        private System.Timers.Timer timer = null;
        private int JCZQ_advanceMinutes = 0;
        private int JCZQ_Result_Day = 0;
        private int currentGetResultTimes = 0;
        private string SavePath = string.Empty;
        private List<JCZQMatchResultCache> cacheMatchResult = new List<JCZQMatchResultCache>();

        //public void Start(Common.Log.ILogWriter logWriter, string gameCode)
        //{
        //    gameCode = gameCode.ToUpper();
        //    logInfoSource += gameCode;
        //    _logWriter = logWriter;

        //    BeStop = false;
        //    JCZQ_advanceMinutes = ServiceHelper.Get_JCZQ_AdvanceMinutes();
        //    JCZQ_Result_Day = ServiceHelper.Get_JCZQ_Result_Day();
        //    cacheMatchResult = new List<JCZQMatchResultCache>();
        //    currentGetResultTimes = 0;
        //    CollectMatchs(gameCode);
        //}
        public string Key { get; set; }
        public string Category { get; set; }
        private IMongoDatabase mDB;
        
       
        private long BeStop = 0;
        private Task thread = null;
        private string gameCode { get; set; }
        private int sleepSecond = 5;
        public JCZQMatchResult_AutoCollect(IMongoDatabase _mDB, string _gameName, int _sleepSecond = 5) : base(_gameName + "MatchResult", _mDB)
        {
            this.sleepSecond = _sleepSecond;
            this.gameCode = _gameName;
            mDB = _mDB;
        }
        public void Start()
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
                    catch(Exception exa)
                    {
                        Thread.Sleep(2000);
                    }
                    finally
                    {
                        if (isError)
                        {
                            Thread.Sleep(2000);
                        }
                        else
                        {
                            Thread.Sleep(this.sleepSecond * 1000);
                        }
                    }
                }
            });
            //  thread.Start();


        }

        public void DoWork(string gameCode)
        {
            this.WriteLog("进入DoWork  开始采集数据");
            currentGetResultTimes++;
            //以310为基础作对比
            var matchResult_310 = new List<C_JCZQ_MatchResult>();
            var matchResult_ZhiDing = new List<C_JCZQ_MatchResult>();
            var matchResult = new List<C_JCZQ_MatchResult>();
            var resultSource = ServiceHelper.GetSystemConfig("JCZQ_Result_Source");
            this.WriteLog("开始从" + resultSource + "采集竞彩足球比赛结果数据");

            //首先采集310结果
            matchResult_310 = Get_Match_Result_From310Win();
            if (resultSource == "cpdyj")
            {
                matchResult_ZhiDing = Get_Match_Result();
            }
            if (resultSource == "aicai")
            {
                matchResult_ZhiDing = Get_Match_Result_FromAiCai();
            }
            if (resultSource == "zzjcw")
            {
                matchResult_ZhiDing = Get_Match_Result_FromZZJCW();
            }
            if (resultSource == "500wan")
            {
                matchResult_ZhiDing = Get_Match_Result_From500wan();
            }
            if (resultSource == "ok")
            {
                matchResult_ZhiDing = Get_Match_Result_FromOk();
            }
            if (resultSource == "wangyi")
            {
                matchResult_ZhiDing = Get_Match_Result_FromWy();
            }

            foreach (var item in matchResult_ZhiDing)
            {
                var result_310 = matchResult_310.Where(p => p.MatchId == item.MatchId).FirstOrDefault();
                if (result_310 == null) continue;
                if (item.SPF_Result != result_310.SPF_Result
                    || item.BRQSPF_Result != result_310.BRQSPF_Result
                    || item.BF_Result != result_310.BF_Result
                    || item.ZJQ_Result != result_310.ZJQ_Result
                    || item.BQC_Result != result_310.BQC_Result)
                {
                    var str = string.Format("采集比赛结果中{0}比赛对比出错。", item.MatchId);
                    this.WriteLog(string.Format("{0}比赛对比错误：1、SPF_Result-{1},BRQSPF_Result-{2},BF_Result-{3},ZJQ_Result-{4},BQC_Result-{5} 2、SPF_Result-{6},BRQSPF_Result-{7},BF_Result-{8},ZJQ_Result-{9},BQC_Result-{10}。 发送短信内容：{11}",
                       item.MatchId, result_310.SPF_Result, result_310.BRQSPF_Result, result_310.BF_Result, result_310.ZJQ_Result, result_310.BQC_Result, item.SPF_Result, item.BRQSPF_Result, item.BF_Result, item.ZJQ_Result, item.BQC_Result, str));
                    continue;
                }
                matchResult.Add(result_310);
            }
            this.WriteLog(string.Format("采集竞彩足球比赛结果数据完成,记录{0}条", matchResult.Count));
           // GetNewJCZQList<C_JCZQ_MatchResult>(matchResult, "Match_Result_List.json");
            ServiceHelper.BuildNewMatchList<C_JCZQ_MatchResult>(mDB, "JCZQ_Match_Result_List", matchResult, null, CompareNewJCZQList);
            #region 发送 比赛结果通知

            this.WriteLog("开始=>发送队伍比赛结果通知");
            //期号|比赛编号_结果&比赛编号_结果&比赛编号_结果
            //其中结果应表示为   DXF:3;RFSF:5;SF:46;
            var temp = new List<string>();
            foreach (var item in matchResult)
            {
                temp.Add(string.Format("{0}_BF:{1};BQC:{2};BRQSPF:{3};SPF:{4};ZJQ:{5};", item.MatchId, item.BF_Result, item.BQC_Result, item.BRQSPF_Result, item.SPF_Result, item.ZJQ_Result));
            }
            var param = string.Join("#", temp.ToArray());
            var paramT = string.Join("_", (from l in matchResult select l.MatchId).ToArray());
            //发送 竞彩足球比赛结果 添加 通知
            var innerKey = string.Format("{0}_{1}", "C_JCZQ_MatchResult", "Add");
            //ServiceHelper.AddAndSendNotification(param, paramT, innerKey, NoticeType.JCZQ_MatchResult);
            new Sports_Business(this.mDB).UpdateLocalData(param, paramT, NoticeType.JCZQ_MatchResult, innerKey);
            //采集多少次后生成html结果
            //var buildHtmlTimes = int.Parse(ServiceHelper.GetSystemConfig("JCZQ_Result_BuildStaticHtml_Times"));
            //if (currentGetResultTimes > buildHtmlTimes)
            //{
            //    try
            //    {
            //        //currentGetResultTimes = 0;
            //        //this.WriteLog("开始生成静态相关数据.");

            //        //this.WriteLog("1.生成开奖结果首页");
            //        //var log = ServiceHelper.SendBuildStaticFileNotice("301");
            //        //this.WriteLog("1.生成开奖结果首页结果：" + log);

            //        ////this.WriteLog("2.生成彩种开奖历史");
            //        ////log = ServiceHelper.SendBuildStaticFileNotice("302", "JCZQ");
            //        ////this.WriteLog("2.生成彩种开奖历史结果：" + log);

            //        //this.WriteLog("2.生成彩种开奖详细");
            //        //log = ServiceHelper.SendBuildStaticFileNotice("303", "JCZQ");
            //        //this.WriteLog("2.生成彩种开奖详细结果：" + log);
            //    }
            //    catch (Exception ex)
            //    {
            //        this.WriteLog("生成静态数据异常：" + ex.Message);
            //    }
            //}

            this.WriteLog("发送队伍比赛结果通知 完成");

            #endregion


            this.WriteLog("DoWork  完成");
        }


   

     


 
        private List<T> CompareNewJCZQList<T>(List<T> newList, List<T> oldList)
            //  where T : C_JCZQ_Match_SPF + C_JCZQ_SPF_SP
            where T : IBallBaseInfo// EntityModel.LotteryJsonInfo.JCZQBase
        {
            if (newList == null)
                return new List<T>();
            if (oldList == null)
                return newList;

            var list = new List<T>();
            foreach (var item in newList)
            {
                var old = oldList.FirstOrDefault(p => p.MatchId == item.MatchId);
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
        //结果采集地址 post 方式
        //        endDate	2012-08-13
        //page	1
        //pagesize	50
        //startDate	2012-08-12
        //http://trade.cpdyj.com/jc/getopeninfo.go
        /// <summary>
        /// cpdyj采集结果
        /// </summary>
        private List<C_JCZQ_MatchResult> Get_Match_Result()
        {
            var list = new List<C_JCZQ_MatchResult>();
            var url = "http://trade.cpdyj.com/jc/getopeninfo.go";
            var page = 1;
            var totalPage = 1;
            var pagesize = 50;
            var totalCount = 50;
            var startDate = DateTime.Now.AddDays(-JCZQ_Result_Day).ToString("yyyy-MM-dd");
            var endDate = DateTime.Now.ToString("yyyy-MM-dd");

            while (totalPage >= page)
            {
                var request = string.Format("startDate={0}&endDate={1}&page={2}&pagesize={3}", startDate, endDate, page, pagesize);
                var xmlContent = PostManager.PostCustomer(url, request, Encoding.GetEncoding("gb2312"), (r) =>
                {
                    if (ServiceHelper.IsUseProxy("JCZQ"))
                    {
                        var proxy = ServiceHelper.GetProxyUrl();
                        if (!string.IsNullOrEmpty(proxy))
                        {
                            r.Proxy = new System.Net.WebProxy(proxy);
                        }
                    }
                });
                var doc = new XmlDocument();
                doc.LoadXml(xmlContent);
                var root = doc.SelectSingleNode("Resp/matchs");
                totalCount = root.Attributes["total"].Value.GetInt32();
                totalPage = root.Attributes["totalpage"].Value.GetInt32();

                foreach (XmlNode item in root.ChildNodes)
                {
                    //todo:不让球胜平负
                    var matchId = item.Attributes["expectItemID"].Value;
                    var matchData = matchId.Substring(0, 6);
                    var matchNumber = matchId.Replace(matchData, "");
                    var state = item.Attributes["state"].Value;
                    if (state != "2")
                    {
                        //异常的比赛
                        //3 是取消的比赛  4 是延期  todo 其它
                        list.Add(new C_JCZQ_MatchResult
                        {
                            CreateTime = DateTime.Now,//.ToString("yyyy-MM-dd HH:mm:ss"),
                            MatchId = matchId,
                            MatchData = matchData,
                            MatchNumber = matchNumber,
                            MatchState = state,
                            SPF_Result = string.Empty,
                            SPF_SP = 1.0M,
                            BRQSPF_Result = string.Empty,
                            BRQSPF_SP = 1.0M,
                            ZJQ_Result = string.Empty,
                            ZJQ_SP = 1.0M,
                            BQC_Result = string.Empty,
                            BQC_SP = 1.0M,
                            BF_Result = string.Empty,
                            BF_SP = 1.0M,
                            HalfHomeTeamScore = 0,
                            HalfGuestTeamScore = 0,
                            FullHomeTeamScore = 0,
                            FullGuestTeamScore = 0,
                        });
                        continue;
                    }
                    var halfScore = item.Attributes["halfScore"].Value.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    var fullScore = item.Attributes["endScore"].Value.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    var brqspfResult = string.Empty;
                    if (fullScore.Length == 2)
                    {
                        var h = int.Parse(fullScore[0]);
                        var g = int.Parse(fullScore[1]);
                        if (h > g)
                            brqspfResult = "3";
                        if (h == g)
                            brqspfResult = "1";
                        if (h < g)
                            brqspfResult = "0";
                    }

                    list.Add(new C_JCZQ_MatchResult
                    {
                        CreateTime = DateTime.Now,//.ToString("yyyy-MM-dd HH:mm:ss"),
                        MatchId = matchId,
                        MatchData = matchData,
                        MatchNumber = matchNumber,
                        MatchState = state,
                        SPF_Result = item.Attributes["spfReslut"].Value,
                        SPF_SP = item.Attributes["spfMoney"].Value.GetDecimal(),
                        BRQSPF_Result = brqspfResult,
                        BRQSPF_SP = item.Attributes["spfMoney"].Value.GetDecimal(),
                        ZJQ_Result = item.Attributes["jqsReslut"].Value,
                        ZJQ_SP = item.Attributes["jqsMoney"].Value.GetDecimal(),
                        BQC_Result = item.Attributes["bqcReslut"].Value.Replace("-", ""),
                        BQC_SP = item.Attributes["bqcMoney"].Value.GetDecimal(),
                        BF_Result = item.Attributes["bfReslut"].Value.Replace("9", "X").Replace(":", ""),
                        BF_SP = item.Attributes["cbfsp"].Value.GetDecimal(),
                        HalfHomeTeamScore = halfScore.Length == 2 ? halfScore[0].GetInt32() : 0,
                        HalfGuestTeamScore = halfScore.Length == 2 ? halfScore[1].GetInt32() : 0,
                        FullHomeTeamScore = fullScore.Length == 2 ? fullScore[0].GetInt32() : 0,
                        FullGuestTeamScore = fullScore.Length == 2 ? fullScore[1].GetInt32() : 0,
                    });
                }

                page++;
            }
            return list;
        }

        //当改为9188接口时，注意把 JCZQ_Result_Day 改为比之前的设置 +1
        /// <summary>
        /// 9188采集结果
        /// </summary>
        private List<C_JCZQ_MatchResult> Get_Match_Result_From9188()
        {
            var list = new List<C_JCZQ_MatchResult>();
            for (int i = 0; i < JCZQ_Result_Day; i++)
            {
                var date = DateTime.Now.AddDays(-i).ToString("yyyyMMdd").Substring(2);
                var url = string.Format("http://www.9188.com/data/jincai/award/{0}/{0}.xml", date);
                var xmlContent = PostManager.Get(url, Encoding.UTF8, 0, (request) =>
                {
                    if (ServiceHelper.IsUseProxy("JCZQ"))
                    {
                        var proxy = ServiceHelper.GetProxyUrl();
                        if (!string.IsNullOrEmpty(proxy))
                        {
                            request.Proxy = new System.Net.WebProxy(proxy);
                        }
                    }
                });
                if (xmlContent == "404")
                    continue;

                Func<int, int, int> ComputeResult = (int h, int g) =>
                {
                    if (h > g) return 3;
                    if (h == g) return 1;
                    return 0;
                };
                Func<int, int, string> ComputeBF = (int h, int g) =>
                {
                    var key = string.Format("{0}{1}", h, g);
                    if (h > g)
                    {
                        var sArray = new string[] { "10", "20", "21", "30", "31", "32", "40", "41", "42", "50", "51", "52" };
                        if (sArray.Contains(key)) return key;
                        return "X0";
                    }
                    if (h == g)
                    {
                        var pArray = new string[] { "00", "11", "22", "33" };
                        if (pArray.Contains(key)) return key;
                        return "XX";
                    }
                    if (h < g)
                    {
                        var fArray = new string[] { "01", "02", "12", "03", "13", "23", "04", "14", "24", "05", "15", "25" };
                        if (fArray.Contains(key)) return key;
                        return "0X";
                    }
                    return string.Empty;
                };

                var doc = new XmlDocument();
                doc.LoadXml(xmlContent);
                var root = doc.SelectSingleNode("rows");
                foreach (XmlNode item in root.ChildNodes)
                {
                    var matchId = item.Attributes["tid"].Value;
                    var matchData = matchId.Substring(0, 6);
                    var matchNumber = matchId.Replace(matchData, "");
                    var halfHome = item.Attributes["hms"].Value.GetInt32();
                    var halfGuest = item.Attributes["hss"].Value.GetInt32();
                    var fullHome = item.Attributes["ms"].Value.GetInt32();
                    var fullGuest = item.Attributes["ss"].Value.GetInt32();

                    //todo:不让球胜平负
                    list.Add(new C_JCZQ_MatchResult
                    {
                        CreateTime = DateTime.Now,//.ToString("yyyy-MM-dd HH:mm:ss"),
                        MatchId = matchId,
                        MatchData = matchData,
                        MatchNumber = matchNumber,
                        MatchState = "2",
                        SPF_Result = ComputeResult(fullHome, fullGuest).ToString(),
                        SPF_SP = 0M,
                        ZJQ_Result = (fullHome + fullGuest) >= 7 ? "7" : (fullHome + fullGuest).ToString(),
                        ZJQ_SP = 0M,
                        BQC_Result = string.Format("{0}{1}", ComputeResult(halfHome, halfGuest), ComputeResult(fullHome, fullGuest)),
                        BQC_SP = 0M,
                        BF_Result = ComputeBF(fullHome, fullGuest),
                        BF_SP = 0M,
                        HalfHomeTeamScore = halfHome,
                        HalfGuestTeamScore = halfGuest,
                        FullHomeTeamScore = fullHome,
                        FullGuestTeamScore = fullGuest,
                    });
                }
            }

            return list;
        }

        /// <summary>
        /// 310win采集结果
        /// </summary>
        public List<C_JCZQ_MatchResult> Get_Match_Result_From310Win()
        {
            var list = new List<C_JCZQ_MatchResult>();

            //过关数据
            //var url = "http://www.310win.com/jingcaizuqiu/banquanchang/kaijiang_jc_all_2.html";
            //单关数据
            var url = "http://www.310win.com/jingcaizuqiu/kaijiang_jc_all.html";
            //var postData = string.Format("__VIEWSTATE=%2FwEPDwUJMzYzMzQyODQ2ZGQTU54JfdsFlPL78umJ2GdLvnQLNg%3D%3D&__EVENTVALIDATION=%2FwEWBAKz%2FMymDQLg2ZN%2BAsKGtEYCjOeKxgZTuQ3CfTwdLjkkRCt%2BjN6hlGXAzA%3D%3D&txtStartDate={0}&txtEndDate={1}&Button1="
            //    , DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd"), DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
            //var content = PostManager.Post(url, postData, Encoding.UTF8);
            var content = PostManager.Get(url, Encoding.UTF8, 0, (request) =>
            {
                if (ServiceHelper.IsUseProxy("JCZQ"))
                {
                    var proxy = ServiceHelper.GetProxyUrl();
                    if (!string.IsNullOrEmpty(proxy))
                    {
                        request.Proxy = new System.Net.WebProxy(proxy);
                    }
                }
            });
            var tempArray = new string[] { };
            //step 1 得到div内容
            var index = content.IndexOf("<div id=\"lottery_container\">");
            content = content.Substring(index);
            index = content.IndexOf("</div>");
            content = content.Substring(0, index);

            //step 2 得到table内容
            index = content.IndexOf("<table");
            content = content.Substring(index, content.Length - index);

            //step 3 得到所有的行
            index = content.IndexOf("<tr");
            content = content.Substring(index, content.Length - index);
            index = content.LastIndexOf("</tr>");
            content = content.Substring(0, index);

            var rows = content.Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in rows)
            {
                try
                {
                    var row = item.Trim();
                    if (!row.Contains("<tr id=")) continue;

                    index = row.IndexOf(">");
                    row = row.Substring(index + 1);
                    //解析一场比赛的结果
                    var matchIdName = string.Empty;
                    var matchDate = string.Empty;
                    var matchNumber = string.Empty;
                    var matchId = string.Empty;
                    var leagueName = string.Empty;
                    var homeTeamName = string.Empty;
                    var guestTeamName = string.Empty;
                    //2 是正常；3 是取消的比赛；4 是延期
                    var matchState = string.Empty;
                    var halfHomeTeamScore = 0;
                    var halfGuestTeamScore = 0;
                    var fullHomeTeamScore = 0;
                    var fullGuestTeamScore = 0;

                    var spf_Result = string.Empty;
                    var spf_SP = 1.0M;
                    var brqspf_Result = string.Empty;
                    var brqspf_SP = 1.0M;
                    var zjq_Result = string.Empty;
                    var zjq_SP = 1.0M;
                    var bqc_Result = string.Empty;
                    var bqc_SP = 1.0M;
                    var bf_Result = string.Empty;
                    var bf_SP = 1.0M;
                    var letBall = 0;

                    var tds = row.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                    #region 解析每行数据
                    for (int i = 0; i < tds.Length; i++)
                    {
                        var td = tds[i].Trim();
                        if (!td.Contains("<td")) continue;

                        index = td.IndexOf(">");
                        var tdContent = td.Substring(index + 1).Replace("\r\n", string.Empty);

                        switch (i)
                        {
                            case 0:
                                //比赛名称
                                //周二02910-29 04:00
                                matchIdName = tdContent.Substring(0, 5);
                                matchDate = GetMatchDate(tdContent, false);
                                matchNumber = tdContent.Substring(2, 3);
                                matchId = matchDate + matchNumber;
                                break;
                            case 1:
                                //联赛名称
                                leagueName = tdContent;
                                break;
                            case 2:
                                //主队名称 和 让球数
                                tempArray = tdContent.Split(new string[] { "<b>" }, StringSplitOptions.RemoveEmptyEntries);
                                if (tempArray.Length == 2)
                                {
                                    homeTeamName = tempArray[0].Trim();
                                    letBall = Format310WinLetBall(tempArray[1]);
                                }
                                break;
                            case 3:
                                //比分
                                switch (tdContent)
                                {
                                    case "推迟":
                                    case "腰斩":
                                        matchState = "4";
                                        break;
                                    case "取消":
                                        matchState = "3";
                                        break;
                                    default:
                                        matchState = "2";
                                        break;
                                }
                                tempArray = tdContent.Split('-');
                                if (matchState == "2" && tempArray.Length == 2)
                                {
                                    fullHomeTeamScore = int.Parse(tempArray[0].Replace("<b>", "").Trim());
                                    fullGuestTeamScore = int.Parse(tempArray[1].Replace("</b>", "").Trim());
                                }
                                if (matchState == "3")
                                {
                                    fullHomeTeamScore = -1;
                                    fullGuestTeamScore = -1;
                                }
                                break;
                            case 4:
                                //客队名称
                                guestTeamName = tdContent;
                                break;
                            case 5:
                                //半场比分
                                tempArray = tdContent.Split('-');
                                if (matchState == "2" && tempArray.Length == 2)
                                {
                                    halfHomeTeamScore = int.Parse(tempArray[0].Trim());
                                    halfGuestTeamScore = int.Parse(tempArray[1].Trim());
                                }
                                if (matchState == "3")
                                {
                                    halfHomeTeamScore = -1;
                                    halfGuestTeamScore = -1;
                                }
                                break;
                            case 6:
                                //让球胜平负 结果
                                tempArray = tdContent.Split(new string[] { "<br />" }, StringSplitOptions.RemoveEmptyEntries);
                                if (matchState == "2" && tempArray.Length == 2)
                                {
                                    index = tempArray[0].IndexOf(">");
                                    tempArray[0] = tempArray[0].Substring(index + 1);
                                    spf_Result = Format310WinSPFResult(tempArray[0].Replace("</span>", ""));
                                    if (string.IsNullOrEmpty(spf_Result))
                                    {
                                        //if (fullHomeTeamScore + letBall > fullGuestTeamScore)
                                        //    spf_Result = "3";
                                        //if (fullHomeTeamScore + letBall == fullGuestTeamScore)
                                        //    spf_Result = "1";
                                        //if (fullHomeTeamScore + letBall < fullGuestTeamScore)
                                        //    spf_Result = "0";
                                        spf_Result = "-1";
                                    }


                                    index = tempArray[1].IndexOf("<u>");
                                    tempArray[1] = tempArray[1].Substring(index + 3);
                                    tempArray[1] = tempArray[1].Replace("</u></a>", "");
                                    if (string.IsNullOrEmpty(tempArray[1]))
                                        spf_SP = 0M;
                                    else
                                        decimal.TryParse(tempArray[1], out spf_SP);
                                }
                                if (matchState == "3")
                                {
                                    spf_Result = "-1";
                                    spf_SP = 1M;
                                }
                                break;
                            case 7:
                                //胜平负 结果
                                tempArray = tdContent.Split(new string[] { "<br />" }, StringSplitOptions.RemoveEmptyEntries);
                                if (matchState == "2" && tempArray.Length == 2)
                                {
                                    index = tempArray[0].IndexOf(">");
                                    tempArray[0] = tempArray[0].Substring(index + 1);
                                    brqspf_Result = Format310WinSPFResult(tempArray[0].Replace("</span>", ""));
                                    if (string.IsNullOrEmpty(brqspf_Result))
                                    {
                                        //if (fullHomeTeamScore > fullGuestTeamScore)
                                        //    brqspf_Result = "3";
                                        //if (fullHomeTeamScore == fullGuestTeamScore)
                                        //    brqspf_Result = "1";
                                        //if (fullHomeTeamScore < fullGuestTeamScore)
                                        //    brqspf_Result = "0";
                                        brqspf_Result = "-1";
                                    }

                                    index = tempArray[1].IndexOf("<u>");
                                    tempArray[1] = tempArray[1].Substring(index + 3);
                                    tempArray[1] = tempArray[1].Replace("</u></a>", "");
                                    if (string.IsNullOrEmpty(tempArray[1]))
                                        brqspf_SP = 0M;
                                    else
                                        decimal.TryParse(tempArray[1], out brqspf_SP);
                                }
                                if (matchState == "3")
                                {
                                    brqspf_Result = "-1";
                                    brqspf_SP = 1M;
                                }
                                break;

                            case 8:
                                //比分 结果
                                tempArray = tdContent.Split(new string[] { "<br />" }, StringSplitOptions.RemoveEmptyEntries);
                                if (matchState == "2" && tempArray.Length == 2)
                                {
                                    index = tempArray[0].IndexOf(">");
                                    tempArray[0] = tempArray[0].Substring(index + 1);
                                    bf_Result = Format310WinBFResult(tempArray[0].Replace("</span>", ""));
                                    if (string.IsNullOrEmpty(bf_Result))
                                    {
                                        bf_Result = "-1";
                                    }
                                    index = tempArray[1].IndexOf("<u>");
                                    tempArray[1] = tempArray[1].Substring(index + 3);
                                    tempArray[1] = tempArray[1].Replace("</u></a>", "");
                                    if (string.IsNullOrEmpty(tempArray[1]))
                                        bf_SP = 0M;
                                    else
                                        decimal.TryParse(tempArray[1], out bf_SP);
                                }
                                if (matchState == "3")
                                {
                                    bf_Result = "-1";
                                    bf_SP = 1M;
                                }
                                break;
                            case 9:
                                //总进球数 结果
                                tempArray = tdContent.Split(new string[] { "<br />" }, StringSplitOptions.RemoveEmptyEntries);
                                if (matchState == "2" && tempArray.Length == 2)
                                {
                                    index = tempArray[0].IndexOf(">");
                                    tempArray[0] = tempArray[0].Substring(index + 1);
                                    zjq_Result = Format310WinZJQResult(tempArray[0].Replace("</span>", ""));
                                    if (string.IsNullOrEmpty(zjq_Result))
                                    {
                                        zjq_Result = "-1";
                                    }

                                    index = tempArray[1].IndexOf("<u>");
                                    tempArray[1] = tempArray[1].Substring(index + 3);
                                    tempArray[1] = tempArray[1].Replace("</u></a>", "");
                                    if (string.IsNullOrEmpty(tempArray[1]))
                                        zjq_SP = 0M;
                                    else
                                        decimal.TryParse(tempArray[1], out zjq_SP);
                                }
                                if (matchState == "3")
                                {
                                    zjq_Result = "-1";
                                    zjq_SP = 1M;
                                }
                                break;
                            case 10:
                                //半全场 结果
                                //<span style='color:#f00;'>2</span><br /><a href="/sp/zuqiu/48654_103.html" target="_blank"><u>8.04</u></a>
                                tempArray = tdContent.Split(new string[] { "<br />" }, StringSplitOptions.RemoveEmptyEntries);
                                if (matchState == "2" && tempArray.Length == 2)
                                {
                                    index = tempArray[0].IndexOf(">");
                                    tempArray[0] = tempArray[0].Substring(index + 1);
                                    bqc_Result = Format310WinBQCResult(tempArray[0].Replace("</span>", ""));
                                    if (string.IsNullOrEmpty(bqc_Result))
                                    {
                                        bqc_Result = "-1";
                                    }

                                    index = tempArray[1].IndexOf("<u>");
                                    tempArray[1] = tempArray[1].Substring(index + 3);
                                    tempArray[1] = tempArray[1].Replace("</u></a>", "");
                                    if (string.IsNullOrEmpty(tempArray[1]))
                                    {
                                        bqc_Result = "-1";
                                        bqc_SP = 0M;
                                    }
                                    else
                                    {
                                        decimal.TryParse(tempArray[1], out bqc_SP);
                                    }
                                }
                                if (matchState == "3")
                                {
                                    bqc_Result = "-1";
                                    bqc_SP = 1M;
                                }
                                break;


                            case 11:
                                //BQC
                                //<span style='color:#f00;'>胜胜</span><br /><a href="/sp/zuqiu/48654_104.html" target="_blank"><u>3.94</u></a>

                                break;
                            case 12:
                                break;
                            default:
                                break;
                        }
                    }
                    #endregion

                    if (string.IsNullOrEmpty(matchState)) continue;
                    if (matchState == "2" && (bf_SP == 1M && bqc_SP == 1M && spf_SP == 1M && zjq_SP == 1M && brqspf_SP == 1M)) continue;
                    if (string.IsNullOrEmpty(matchState) || (bf_Result == "" && bqc_Result == "" && spf_Result == "" && zjq_Result == "" && brqspf_Result == "") || (bf_Result == "-1" && bqc_Result == "-1" && spf_Result == "-1" && zjq_Result == "-1" && brqspf_Result == "-1")) continue;
                    if (string.IsNullOrEmpty(bf_Result) || string.IsNullOrEmpty(spf_Result) || string.IsNullOrEmpty(brqspf_Result) || string.IsNullOrEmpty(zjq_Result) || string.IsNullOrEmpty(bqc_Result)) continue;

                    list.Add(new C_JCZQ_MatchResult
                    {
                        MatchId = matchId,
                        MatchData = matchDate,
                        MatchNumber = matchNumber,
                        CreateTime = DateTime.Now,
                        MatchState = matchState,
                        FullGuestTeamScore = fullGuestTeamScore,
                        FullHomeTeamScore = fullHomeTeamScore,
                        HalfGuestTeamScore = halfGuestTeamScore,
                        HalfHomeTeamScore = halfHomeTeamScore,
                        BF_Result = bf_Result,
                        BF_SP = bf_SP,
                        BQC_Result = bqc_Result,
                        BQC_SP = bqc_SP,
                        SPF_Result = spf_Result,
                        SPF_SP = spf_SP,
                        ZJQ_Result = zjq_Result,
                        ZJQ_SP = zjq_SP,
                        BRQSPF_Result = brqspf_Result,
                        BRQSPF_SP = brqspf_SP,
                    });
                }
                catch (Exception ex)
                {
                    this.WriteLog("解析表格数据失败：" + ex.ToString());
                }
            }
            return list;
        }

        #region 格式化310Win的彩果数据

        private int Format310WinLetBall(string content)
        {
            var l = 0;
            var lb = content.Trim();
            var lbIndex = lb.IndexOf(">");
            if (lbIndex < 0) return l;
            lb = lb.Substring(lbIndex + 1);
            lbIndex = lb.IndexOf("<");
            if (lbIndex < 0) return l;
            int.TryParse(lb.Substring(0, lbIndex), out l);
            return l;
        }
        private string Format310WinSPFResult(string content)
        {
            switch (content)
            {
                case "胜":
                    return "3";
                case "平":
                    return "1";
                case "负":
                    return "0";
                default:
                    break;
            }
            return string.Empty;
        }
        private string Format310WinBFResult(string content)
        {
            var allowCode = new string[] { "1:0", "2:0", "2:1", "3:0", "3:1", "3:2", "4:0", "4:1", "4:2", "5:0", "5:1", "5:2",
                                           "0:0", "1:1", "2:2", "3:3",
                                           "0:1", "0:2","1:2","0:3","1:3","2:3","0:4","1:4","2:4","0:5","1:5","2:5"};
            if (allowCode.Contains(content))
                return content.Replace(":", "");
            if (content == "胜其他")
                return "X0";
            if (content == "平其他")
                return "XX";
            if (content == "负其他")
                return "0X";
            var tempArray = content.Split(':');
            if (tempArray.Length != 2)
                return content;
            var h = int.Parse(tempArray[0]);
            var g = int.Parse(tempArray[1]);
            if (h == g)
                return "XX";
            if (h > g)
                return "X0";
            if (h < g)
                return "0X";
            return content;
        }
        private string Format310WinZJQResult(string content)
        {
            var allowCode = new string[] { "0", "1", "2", "3", "4", "5", "6" };
            if (allowCode.Contains(content))
                return content;
            return "7";
        }
        private string Format310WinBQCResult(string content)
        {
            var r = string.Empty;
            foreach (var item in content)
            {
                r += Format310WinSPFResult(item.ToString());
            }
            return r;
        }
        private string GetMatchDate(string matchIdName, bool isFuture)
        {
            var matchWeek = matchIdName.Length == 2 ? matchIdName : matchIdName.Substring(0, 2);
            for (int i = 0; i <= 7; i++)
            {
                var currentDate = DateTime.Now.AddDays(isFuture ? i : -i);
                var chinaName = string.Empty;
                switch (currentDate.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        chinaName = "周一";
                        break;
                    case DayOfWeek.Tuesday:
                        chinaName = "周二";
                        break;
                    case DayOfWeek.Wednesday:
                        chinaName = "周三";
                        break;
                    case DayOfWeek.Thursday:
                        chinaName = "周四";
                        break;
                    case DayOfWeek.Friday:
                        chinaName = "周五";
                        break;
                    case DayOfWeek.Saturday:
                        chinaName = "周六";
                        break;
                    case DayOfWeek.Sunday:
                        chinaName = "周日";
                        break;
                }
                if (matchWeek == chinaName)
                    return currentDate.ToString("yyyyMMdd").Substring(2);
            }
            return string.Empty;
        }

        #endregion

        /// <summary>
        /// aicai采集结果
        /// </summary>
        public List<C_JCZQ_MatchResult> Get_Match_Result_FromAiCai()
        {
            #region 解析结果

            Func<int, int, int, string> getSPF_Result = (home, lb, guest) =>
            {
                var h = home + lb;
                if (h > guest)
                    return "3";
                if (h < guest)
                    return "0";
                return "1";
            };
            Func<int, int, string> getBF_Result = (home, guest) =>
            {
                var sArray = new string[] { "10", "20", "21", "30", "31", "32", "40", "41", "42", "50", "51", "52" };
                var pArray = new string[] { "00", "11", "22", "33", };
                var fArray = new string[] { "01", "02", "12", "03", "13", "23", "04", "14", "24", "05", "15", "25" };
                var t = string.Format("{0}{1}", home, guest);
                if (home > guest)
                {
                    //胜结果
                    if (sArray.Contains(t))
                        return t;
                    return "X0";
                }
                if (home < guest)
                {
                    //负结果
                    if (fArray.Contains(t))
                        return t;
                    return "0X";
                }
                //平结果  
                if (pArray.Contains(t))
                    return t;
                return "XX";
            };
            Func<int, int, string> getZJQ_Result = (home, guest) =>
            {
                var total = home + guest;
                if (total >= 7)
                    return "7";
                return total.ToString();
            };
            Func<int, int, int, int, string> getBQC_Result = (homeHalf, guestHalf, homeFull, guestFull) =>
            {
                var half = getSPF_Result(homeHalf, 0, guestHalf);
                var full = getSPF_Result(homeFull, 0, guestFull);
                return string.Format("{0}{1}", half, full);
            };

            #endregion

            var list = new List<C_JCZQ_MatchResult>();
            //HttpUtility
            var url = "http://www.aicai.com/lottery/zcReport!zcMatchResult.jhtml";
            var content = PostManager.Get(url, Encoding.UTF8, 0, (request) =>
            {
                if (ServiceHelper.IsUseProxy("JCZQ"))
                {
                    var proxy = ServiceHelper.GetProxyUrl();
                    if (!string.IsNullOrEmpty(proxy))
                    {
                        request.Proxy = new System.Net.WebProxy(proxy);
                    }
                }
            });
            content = System.Web.HttpUtility.HtmlDecode(content);

            var flag = "<div class=\"kjjc_tablebox\">";
            var flagIndex = content.IndexOf(flag);
            content = content.Substring(flagIndex);
            content = content.Replace(flag, "");

            flag = "<form id=\"jq_form\"";
            flagIndex = content.IndexOf(flag);
            content = content.Substring(0, flagIndex);
            flag = "</thead>";
            flagIndex = content.IndexOf(flag);
            content = content.Substring(flagIndex);
            content = content.Replace(flag, "");
            var rows = content.Split(new string[] { "<tr class=" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var row in rows)
            {
                try
                {
                    #region 解析一行数据

                    #region 定义变量

                    // 2 表示完场的比赛
                    var matchIdName = string.Empty;
                    var matchDate = string.Empty;
                    var matchNumber = string.Empty;
                    var matchId = string.Empty;
                    var matchState = "2";
                    var halfHomeTeamScore = -1;
                    var halfGuestTeamScore = -1;
                    var fullHomeTeamScore = -1;
                    var fullGuestTeamScore = -1;

                    var spf_Result = "-1";
                    var spf_SP = 1.0M;
                    var zjq_Result = "-1";
                    var zjq_SP = 1.0M;
                    var bqc_Result = "-1";
                    var bqc_SP = 1.0M;
                    var bf_Result = "-1";
                    var bf_SP = 1.0M;

                    #endregion

                    #region 解析比分和结果状态

                    flag = "<td class=\"xx_bg_1\">";
                    flagIndex = row.IndexOf(flag);
                    if (flagIndex < 0) continue;

                    var rowContent = row.Substring(flagIndex);
                    rowContent = rowContent.Replace(flag, "");
                    flag = "</td>";
                    flagIndex = rowContent.IndexOf(flag);

                    matchIdName = rowContent.Substring(0, flagIndex).Replace("\r\n\t", "").Replace(" ", "");
                    matchDate = GetMatchDate(matchIdName, false);
                    matchNumber = matchIdName.Substring(2);
                    matchId = matchDate + matchNumber;

                    flag = "<strong class=\"green\">";
                    flagIndex = rowContent.IndexOf(flag);
                    rowContent = rowContent.Substring(flagIndex);
                    rowContent = rowContent.Replace(flag, "");

                    flag = "</strong>";
                    flagIndex = rowContent.IndexOf(flag);
                    //让球
                    var lb = int.Parse(rowContent.Substring(0, flagIndex));

                    flag = "<td><a href=";
                    flagIndex = rowContent.IndexOf(flag);
                    rowContent = rowContent.Substring(flagIndex);
                    rowContent = rowContent.Replace(flag, "");

                    flag = "<td>";
                    flagIndex = rowContent.IndexOf(flag);
                    rowContent = rowContent.Substring(flagIndex);
                    rowContent = rowContent.Replace(flag, "");

                    flag = "</td>";
                    flagIndex = rowContent.IndexOf(flag);
                    var half_BF = rowContent.Substring(0, flagIndex);
                    var half_BF_Array = half_BF.Split(':');
                    if (half_BF_Array.Length == 2)
                    {
                        //半场比分
                        halfHomeTeamScore = int.Parse(half_BF_Array[0].Trim());
                        halfGuestTeamScore = int.Parse(half_BF_Array[1].Trim());
                    }

                    flag = "<strong class=\"red\">";
                    flagIndex = rowContent.IndexOf(flag);
                    rowContent = rowContent.Substring(flagIndex);
                    rowContent = rowContent.Replace(flag, "");

                    flag = "</strong></td>";
                    flagIndex = rowContent.IndexOf(flag);
                    var full_BF = rowContent.Substring(0, flagIndex);
                    var full_BF_Array = full_BF.Split(':');
                    if (full_BF_Array.Length == 2)
                    {
                        //全场比分
                        fullHomeTeamScore = int.Parse(full_BF_Array[0].Trim());
                        fullGuestTeamScore = int.Parse(full_BF_Array[1].Trim());
                        spf_Result = getSPF_Result(fullHomeTeamScore, lb, fullGuestTeamScore);
                        bf_Result = getBF_Result(fullHomeTeamScore, fullGuestTeamScore);
                        zjq_Result = getZJQ_Result(fullHomeTeamScore, fullGuestTeamScore);
                        bqc_Result = getBQC_Result(halfHomeTeamScore, halfGuestTeamScore, fullHomeTeamScore, fullGuestTeamScore);
                    }
                    rowContent = rowContent.Replace(flag, "");

                    //为空 表示  延迟
                    if (string.IsNullOrWhiteSpace(half_BF))
                    {
                        matchState = "4";
                    }
                    if (half_BF.Trim() == "*")
                    {
                        matchState = "3";
                    }

                    #endregion

                    if (matchState == "2")
                    {
                        #region 解析SP

                        var spArray = rowContent.Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);
                        if (spArray.Length != 7) continue;
                        var resultRow = string.Empty;
                        var spRow = string.Empty;
                        foreach (var item in spArray)
                        {
                            if (!string.IsNullOrEmpty(resultRow) && !string.IsNullOrEmpty(spRow))
                                break;
                            if (item.IndexOf(">彩果<") > 0)
                                resultRow = item;
                            if (item.IndexOf(">奖金<") > 0)
                                spRow = item;
                        }

                        flag = "</td>";
                        flagIndex = spRow.IndexOf(flag);
                        spRow = spRow.Substring(flagIndex);
                        spArray = spRow.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < spArray.Length; i++)
                        {
                            var sp = spArray[i].Replace("\r\n\t", "").Replace(" ", "");
                            if (string.IsNullOrWhiteSpace(sp) || string.IsNullOrEmpty(sp)) continue;
                            switch (i)
                            {
                                case 0:
                                    spf_SP = decimal.Parse(sp);
                                    break;
                                case 1:
                                    bf_SP = decimal.Parse(sp);
                                    break;
                                case 2:
                                    zjq_SP = decimal.Parse(sp);
                                    break;
                                case 3:
                                    bqc_SP = decimal.Parse(sp);
                                    break;
                                default:
                                    break;
                            }
                        }

                        #endregion
                    }
                    list.Add(new C_JCZQ_MatchResult
                    {
                        MatchId = matchId,
                        MatchData = matchDate,
                        MatchNumber = matchNumber,
                        CreateTime = DateTime.Now,
                        MatchState = matchState,
                        FullGuestTeamScore = fullGuestTeamScore,
                        FullHomeTeamScore = fullHomeTeamScore,
                        HalfGuestTeamScore = halfGuestTeamScore,
                        HalfHomeTeamScore = halfHomeTeamScore,
                        BF_Result = bf_Result,
                        BF_SP = bf_SP,
                        BQC_Result = bqc_Result,
                        BQC_SP = bqc_SP,
                        SPF_Result = spf_Result,
                        SPF_SP = spf_SP,
                        ZJQ_Result = zjq_Result,
                        ZJQ_SP = zjq_SP,
                    });

                    #endregion
                }
                catch (Exception ex)
                {
                    this.WriteLog("解析表格数据失败：" + ex.ToString());
                }
            }

            return list;
        }

        public List<string> JCId = new List<string>();

        #region 接受成功票号保存
        private void GetrderInfoList(string content)
        {
            //var fileFullName = BuildFileFullNameId("MatchIdList.txt");
            //File.WriteAllText(fileFullName, content, Encoding.UTF8);
            var coll = mDB.GetCollection<BsonDocument>("MatchIdList");
            BsonDocument BDdoc = new BsonDocument();
            BDdoc.Add("Content",content);
            coll.DeleteMany(null);
            coll.InsertOne(BDdoc);
        }
        private List<string> ResponseList()
        {
            var coll = mDB.GetCollection<BsonDocument>("MatchIdList");

           var bddoc= coll.Find(null).FirstOrDefault<BsonDocument>();

          
            if (bddoc !=null)
                return bddoc["Content"].ToString().Split(',').ToList();
            else
                return new List<string>();
        }
        /// <summary>
        /// 创建文件全路径
        /// </summary>
        //private string BuildFileFullNameId(string fileName)
        //{
        //    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
        //    if (!Directory.Exists(path))
        //        Directory.CreateDirectory(path);
        //    return Path.Combine(path, fileName);
        //}
        #endregion
        /// <summary>
        /// 中国竞彩网采集结果
        /// </summary>
        public List<C_JCZQ_MatchResult> Get_Match_Result_FromZZJCW()
        {
            JCId = ResponseList();
            var list = new List<C_JCZQ_MatchResult>();
            var url = "http://info.sporttery.cn/football/match_result.php";
            var encoding = Encoding.GetEncoding("gb2312");
            var content = PostManager.Get(url, encoding, 0, null);
            var tempArray = new string[] { };
            //step 1 得到div内容
            var index = content.IndexOf("<div class=\"pagebox\"");
            content = content.Substring(index);
            index = content.IndexOf("</div>");
            content = content.Substring(0, index);
            var aas = content.Split(new string[] { "</a>" }, StringSplitOptions.RemoveEmptyEntries);
            var aList = new List<string>();
            foreach (var a in aas)
            {
                if (aas.Count() > 1)
                {
                    if (!a.Contains("<a")) continue;
                    var s = a.IndexOf("<a href=\"");
                    var e = a.IndexOf("\" target='_self'");
                    url = a.Substring(s, e - s).Replace("<a href=\"", "http://info.sporttery.cn/football/");
                    if (aList.Contains(url)) continue;
                    aList.Add(url);
                }
                else
                {
                    url = "http://info.sporttery.cn/football/match_result.php";
                }
                content = PostManager.Get(url, encoding, 0, null);
                //step 1 得到div内容
                index = content.IndexOf("--><tr class='tr1'>");
                content = content.Substring(index);
                index = content.IndexOf("<table");
                content = content.Substring(0, index);

                //step 3 得到所有的行
                index = content.IndexOf("<tr");
                content = content.Substring(index, content.Length - index);
                index = content.LastIndexOf("</tr>");
                content = content.Substring(0, index);

                var rows = content.Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);
                this.WriteLog(string.Format("已获得数据{0}条", rows.Count()));
                foreach (var item in rows)
                {
                    try
                    {
                        var row = item.Trim();
                        //if (!row.Contains("<tr id=")) continue;

                        index = row.IndexOf(">");
                        row = row.Substring(index + 1);
                        //解析一场比赛的结果
                        var matchIdName = string.Empty;
                        var matchDate = string.Empty;
                        var matchNumber = string.Empty;
                        var matchId = string.Empty;
                        var leagueName = string.Empty;
                        var halfScore = string.Empty;
                        var Score = string.Empty;
                        //2 是正常；3 是取消的比赛；4 是延期
                        var matchState = string.Empty;
                        var halfHomeTeamScore = 0;
                        var halfGuestTeamScore = 0;
                        var fullHomeTeamScore = 0;
                        var fullGuestTeamScore = 0;

                        var spf_Result = string.Empty;
                        var spf_SP = 1.0M;
                        var brqspf_Result = string.Empty;
                        var brqspf_SP = 1.0M;
                        var zjq_Result = string.Empty;
                        var zjq_SP = 1.0M;
                        var bqc_Result = string.Empty;
                        var bqc_SP = 1.0M;
                        var bf_Result = string.Empty;
                        var bf_SP = 1.0M;
                        var letBall = 0;
                        var jcid = string.Empty;

                        var tds = row.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);

                        #region 解析每行数据
                        for (int i = 0; i < tds.Length; i++)
                        {
                            var td = tds[i].Trim();
                            if (!td.Contains("<td")) continue;

                            index = td.IndexOf(">");
                            var tdContent = td.Substring(index + 1).Replace("\r\n", string.Empty);

                            switch (i)
                            {
                                case 0:
                                    break;
                                case 1:
                                    //比赛名称
                                    //周二02910-29 04:00
                                    matchIdName = tdContent.Substring(0, 5);
                                    matchDate = GetMatchDate(tdContent, false);
                                    matchNumber = tdContent.Substring(2, 3);
                                    matchId = matchDate + matchNumber;
                                    break;
                                case 4:
                                    //半场比分
                                    halfScore = tdContent;
                                    break;
                                case 5:
                                    //全场比分
                                    Score = tdContent;
                                    break;
                                case 6:
                                    //比分
                                    switch (tdContent)
                                    {
                                        case "推迟":
                                        case "腰斩":
                                            matchState = "4";
                                            break;
                                        case "取消":
                                            matchState = "3";
                                            break;
                                        //case "进行中":
                                        //    matchState = "0";
                                        //break;
                                        case "已完成":
                                            matchState = "2";
                                            break;
                                    }
                                    if (matchState == "2")
                                    {
                                        fullHomeTeamScore = int.Parse(Score.Split(':')[0]);
                                        fullGuestTeamScore = int.Parse(Score.Split(':')[1]);
                                        halfHomeTeamScore = int.Parse(halfScore.Split(':')[0]);
                                        halfGuestTeamScore = int.Parse(halfScore.Split(':')[1]);
                                    }
                                    if (matchState == "3")
                                    {
                                        fullHomeTeamScore = -1;
                                        fullGuestTeamScore = -1;
                                        halfGuestTeamScore = -1;
                                        halfHomeTeamScore = -1;
                                        spf_Result = "-1";
                                        brqspf_Result = "-1";
                                        zjq_Result = "-1";
                                        bqc_Result = "-1";
                                        bf_Result = "-1";
                                    }
                                    break;
                                case 7:
                                    if (string.IsNullOrEmpty(tdContent) || tdContent.Contains("取消"))
                                    {
                                        var s = tds[3].IndexOf("php?m=");
                                        var e = tds[3].IndexOf("\" style=\"");
                                        var str = tds[3].Substring(s, e - s).Replace("php?m=", "").Trim();
                                        jcid = str;
                                        continue;
                                    }
                                    var start = tdContent.IndexOf("id=");
                                    var end = tdContent.IndexOf("target=");
                                    tdContent = tdContent.Substring(start, end - start).Replace("id=", "").Replace("\"", "").Trim();
                                    jcid = tdContent;
                                    if (JCId.Contains(jcid)) continue;
                                    GetResultAndSP_ZZJCW(tdContent, out spf_Result, out brqspf_Result, out zjq_Result, out bqc_Result, out bf_Result, out spf_SP, out brqspf_SP, out zjq_SP, out bqc_SP, out bf_SP);
                                    break;
                            }
                        }
                        #endregion
                        if (string.IsNullOrEmpty(matchState) || (bf_Result == "" && bqc_Result == "" && spf_Result == "" && zjq_Result == "" && brqspf_Result == "") || (bf_Result == "-1" && bqc_Result == "-1" && spf_Result == "-1" && zjq_Result == "-1" && brqspf_Result == "-1" && matchState != "3")) continue;
                        if (string.IsNullOrEmpty(bf_Result) || string.IsNullOrEmpty(spf_Result) || string.IsNullOrEmpty(brqspf_Result) || string.IsNullOrEmpty(zjq_Result) || string.IsNullOrEmpty(bqc_Result)) continue;
                        if (JCId.Contains(jcid)) continue;
                        if (!(bf_Result == "-1" || bqc_Result == "-1" || spf_Result == "-1" || zjq_Result == "-1" || brqspf_Result == "-1") || matchState == "3")
                            JCId.Add(jcid);
                        list.Add(new C_JCZQ_MatchResult
                        {
                            MatchId = matchId,
                            MatchData = matchDate,
                            MatchNumber = matchNumber,
                            CreateTime = DateTime.Now,
                            MatchState = matchState,
                            FullGuestTeamScore = fullGuestTeamScore,
                            FullHomeTeamScore = fullHomeTeamScore,
                            HalfGuestTeamScore = halfGuestTeamScore,
                            HalfHomeTeamScore = halfHomeTeamScore,
                            BF_Result = bf_Result,
                            BF_SP = bf_SP,
                            BQC_Result = bqc_Result,
                            BQC_SP = bqc_SP,
                            SPF_Result = spf_Result,
                            SPF_SP = spf_SP,
                            ZJQ_Result = zjq_Result,
                            ZJQ_SP = zjq_SP,
                            BRQSPF_Result = brqspf_Result,
                            BRQSPF_SP = brqspf_SP,
                        });
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog("解析表格数据失败：" + ex.ToString());
                    }
                }
            }
            GetrderInfoList(string.Join(",", JCId));
            return list;
        }

        public void GetResultAndSP_ZZJCW(string id, out string spf_Result, out string brqspf_Result, out string zjq_Result, out string bqc_Result, out string bf_Result
            , out decimal spf_SP, out decimal brqspf_SP, out decimal zjq_SP, out decimal bqc_SP, out decimal bf_SP)
        {
            spf_Result = string.Empty;
            spf_SP = 1.0M;
            brqspf_Result = string.Empty;
            brqspf_SP = 1.0M;
            zjq_Result = string.Empty;
            zjq_SP = 1.0M;
            bqc_Result = string.Empty;
            bqc_SP = 1.0M;
            bf_Result = string.Empty;
            bf_SP = 1.0M;
            var url = string.Format("http://info.sporttery.cn/football/pool_result.php?id={0}", id);
            var encoding = Encoding.GetEncoding("gb2312");
            var content = PostManager.Get(url, encoding, 0, null);
            var tempArray = new string[] { };
            //step 1 得到div内容
            var index = content.IndexOf("<form action=\"pool_result0528.php\"");
            content = content.Substring(index);
            index = content.LastIndexOf("</table>");
            content = content.Substring(0, index + "</table>".Length);

            //step 3 得到所有的行
            index = content.IndexOf("<table");
            content = content.Substring(index, content.Length - index);
            var tables = content.Split(new string[] { "<table class=\"date6 jf\"" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < tables.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        #region 竞猜足球对应结果
                        var table = tables[i];
                        var trs = table.Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries)[2];
                        index = trs.IndexOf(">");
                        trs = trs.Substring(index + 1);
                        var tds = trs.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                        for (int j = 0; j < tds.Length; j++)
                        {
                            var td = tds[j].Trim();
                            if (!td.Contains("<td")) continue;
                            index = td.IndexOf(">");
                            var tdContent = td.Substring(index + 1).Replace("\r\n", string.Empty);
                            switch (j)
                            {
                                case 1:
                                    if (string.IsNullOrEmpty(tdContent))
                                    {
                                        brqspf_Result = "-1";
                                        break;
                                    }
                                    brqspf_Result = Format310WinSPFResult(tdContent);
                                    break;
                                case 3:
                                    if (string.IsNullOrEmpty(tdContent))
                                    {
                                        spf_Result = "-1";
                                        break;
                                    }
                                    spf_Result = Format310WinSPFResult(tdContent);
                                    break;
                                case 4:
                                    if (string.IsNullOrEmpty(tdContent))
                                    {
                                        bf_Result = "-1";
                                        break;
                                    }
                                    bf_Result = Format310WinBFResult(tdContent);
                                    break;
                                case 5:
                                    if (string.IsNullOrEmpty(tdContent))
                                    {
                                        zjq_Result = "-1";
                                        break;
                                    }
                                    zjq_Result = Format310WinZJQResult(tdContent);
                                    break;
                                case 6:
                                    if (string.IsNullOrEmpty(tdContent))
                                    {
                                        bqc_Result = "-1";
                                        break;
                                    }
                                    bqc_Result = Format310WinBQCResult(tdContent);
                                    break;
                                default:
                                    break;
                            }
                        }
                        #endregion
                        break;
                    case 1:
                        #region 胜平负SP
                        if (spf_Result == "-1") continue;
                        table = tables[i].Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowUp.gif\" />", "").Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowDown.gif\" />", "");
                        var lr = table.Replace("\r\n", "").Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);
                        trs = lr[lr.Length - 2];
                        index = trs.IndexOf(">");
                        trs = trs.Substring(index + 1);
                        tds = trs.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                        if (spf_Result == "3")
                            decimal.TryParse(tds[2].Substring(tds[2].IndexOf(">") + 1).Replace("\r\n", string.Empty), out spf_SP);
                        if (spf_Result == "1")
                            decimal.TryParse(tds[3].Substring(tds[3].IndexOf(">") + 1).Replace("\r\n", string.Empty), out spf_SP);
                        if (spf_Result == "0")
                            decimal.TryParse(tds[4].Substring(tds[4].IndexOf(">") + 1).Replace("\r\n", string.Empty), out spf_SP);
                        #endregion
                        break;
                    case 2:
                        #region 不让球胜平负SP
                        if (brqspf_Result == "-1") continue;
                        table = tables[i].Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowUp.gif\" />", "").Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowDown.gif\" />", "");
                        var lnr = table.Replace("\r\n", "").Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);
                        trs = lnr[lnr.Length - 2];
                        index = trs.IndexOf(">");
                        trs = trs.Substring(index + 1);
                        tds = trs.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                        if (brqspf_Result == "3")
                            decimal.TryParse(tds[1].Substring(tds[1].IndexOf(">") + 1).Replace("\r\n", string.Empty), out brqspf_SP);
                        if (brqspf_Result == "1")
                            decimal.TryParse(tds[2].Substring(tds[2].IndexOf(">") + 1).Replace("\r\n", string.Empty), out brqspf_SP);
                        if (brqspf_Result == "0")
                            decimal.TryParse(tds[3].Substring(tds[3].IndexOf(">") + 1).Replace("\r\n", string.Empty), out brqspf_SP);
                        #endregion
                        break;
                    case 3:
                        #region 总进球SP
                        if (zjq_Result == "-1") continue;
                        table = tables[i].Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowUp.gif\" />", "").Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowDown.gif\" />", "");
                        var lzjq = table.Replace("\r\n", "").Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);
                        trs = lzjq[lzjq.Length - 2];
                        index = trs.IndexOf(">");
                        trs = trs.Substring(index + 1);
                        tds = trs.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                        if (zjq_Result == "0")
                            decimal.TryParse(tds[1].Substring(tds[1].IndexOf(">") + 1).Replace("\r\n", string.Empty), out zjq_SP);
                        if (zjq_Result == "1")
                            decimal.TryParse(tds[2].Substring(tds[2].IndexOf(">") + 1).Replace("\r\n", string.Empty), out zjq_SP);
                        if (zjq_Result == "2")
                            decimal.TryParse(tds[3].Substring(tds[3].IndexOf(">") + 1).Replace("\r\n", string.Empty), out zjq_SP);
                        if (zjq_Result == "3")
                            decimal.TryParse(tds[4].Substring(tds[4].IndexOf(">") + 1).Replace("\r\n", string.Empty), out zjq_SP);
                        if (zjq_Result == "4")
                            decimal.TryParse(tds[5].Substring(tds[5].IndexOf(">") + 1).Replace("\r\n", string.Empty), out zjq_SP);
                        if (zjq_Result == "5")
                            decimal.TryParse(tds[6].Substring(tds[6].IndexOf(">") + 1).Replace("\r\n", string.Empty), out zjq_SP);
                        if (zjq_Result == "6")
                            decimal.TryParse(tds[7].Substring(tds[7].IndexOf(">") + 1).Replace("\r\n", string.Empty), out zjq_SP);
                        if (zjq_Result == "7")
                            decimal.TryParse(tds[8].Substring(tds[8].IndexOf(">") + 1).Replace("\r\n", string.Empty), out zjq_SP);
                        #endregion
                        break;
                    case 4:
                        #region 半全场SP
                        if (bqc_Result == "-1") continue;
                        table = tables[i].Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowUp.gif\" />", "").Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowDown.gif\" />", "");
                        var lbqc = table.Replace("\r\n", "").Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);
                        trs = lbqc[lbqc.Length - 2];
                        index = trs.IndexOf(">");
                        trs = trs.Substring(index + 1);
                        tds = trs.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                        if (bqc_Result == "33")
                            decimal.TryParse(tds[1].Substring(tds[1].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bqc_SP);
                        if (bqc_Result == "31")
                            decimal.TryParse(tds[2].Substring(tds[2].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bqc_SP);
                        if (bqc_Result == "30")
                            decimal.TryParse(tds[3].Substring(tds[3].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bqc_SP);
                        if (bqc_Result == "13")
                            decimal.TryParse(tds[4].Substring(tds[4].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bqc_SP);
                        if (bqc_Result == "11")
                            decimal.TryParse(tds[5].Substring(tds[5].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bqc_SP);
                        if (bqc_Result == "10")
                            decimal.TryParse(tds[6].Substring(tds[6].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bqc_SP);
                        if (bqc_Result == "03")
                            decimal.TryParse(tds[7].Substring(tds[7].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bqc_SP);
                        if (bqc_Result == "01")
                            decimal.TryParse(tds[8].Substring(tds[8].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bqc_SP);
                        if (bqc_Result == "00")
                            decimal.TryParse(tds[9].Substring(tds[9].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bqc_SP);
                        #endregion
                        break;
                    case 5:
                        #region 比分SP
                        if (bf_Result == "-1") continue;
                        table = tables[i].Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowUp.gif\" />", "").Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowDown.gif\" />", "");
                        var trsL = table.Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);
                        var num1 = new string[] { "10", "20", "21", "30", "31", "32", "40", "41", "42", "50", "51", "52", "X0" };
                        var num2 = new string[] { "00", "11", "22", "33", "XX" };
                        var num3 = new string[] { "01", "02", "12", "03", "13", "23", "04", "14", "24", "05", "15", "25", "0X" };
                        if (num1.Contains(bf_Result))
                        {
                            trs = trsL[3];
                            index = trs.IndexOf(">");
                            trs = trs.Substring(index + 1);
                            tds = trs.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                            if (bf_Result == "10")
                                decimal.TryParse(tds[0].Substring(tds[0].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                            if (bf_Result == "20")
                                decimal.TryParse(tds[1].Substring(tds[1].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                            if (bf_Result == "21")
                                decimal.TryParse(tds[2].Substring(tds[2].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                            if (bf_Result == "30")
                                decimal.TryParse(tds[3].Substring(tds[3].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                            if (bf_Result == "31")
                                decimal.TryParse(tds[4].Substring(tds[4].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                            if (bf_Result == "32")
                                decimal.TryParse(tds[5].Substring(tds[5].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                            if (bf_Result == "40")
                                decimal.TryParse(tds[6].Substring(tds[6].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                            if (bf_Result == "41")
                                decimal.TryParse(tds[7].Substring(tds[7].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                            if (bf_Result == "42")
                                decimal.TryParse(tds[8].Substring(tds[8].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                            if (bf_Result == "50")
                                decimal.TryParse(tds[9].Substring(tds[9].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                            if (bf_Result == "51")
                                decimal.TryParse(tds[10].Substring(tds[10].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                            if (bf_Result == "52")
                                decimal.TryParse(tds[11].Substring(tds[11].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                            if (bf_Result == "X0")
                                decimal.TryParse(tds[12].Substring(tds[12].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                        }
                        if (num2.Contains(bf_Result))
                        {
                            trs = trsL[5];
                            index = trs.IndexOf(">");
                            trs = trs.Substring(index + 1);
                            tds = trs.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                            if (bf_Result == "00")
                                decimal.TryParse(tds[0].Substring(tds[0].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                            if (bf_Result == "11")
                                decimal.TryParse(tds[1].Substring(tds[1].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                            if (bf_Result == "22")
                                decimal.TryParse(tds[2].Substring(tds[2].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                            if (bf_Result == "33")
                                decimal.TryParse(tds[3].Substring(tds[3].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                            if (bf_Result == "XX")
                                decimal.TryParse(tds[4].Substring(tds[4].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                        }
                        if (num3.Contains(bf_Result))
                        {
                            trs = trsL[7];
                            index = trs.IndexOf(">");
                            trs = trs.Substring(index + 1);
                            tds = trs.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                            if (bf_Result == "01")
                                decimal.TryParse(tds[0].Substring(tds[0].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                            if (bf_Result == "02")
                                decimal.TryParse(tds[1].Substring(tds[1].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                            if (bf_Result == "12")
                                decimal.TryParse(tds[2].Substring(tds[2].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                            if (bf_Result == "03")
                                decimal.TryParse(tds[3].Substring(tds[3].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                            if (bf_Result == "13")
                                decimal.TryParse(tds[4].Substring(tds[4].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                            if (bf_Result == "23")
                                decimal.TryParse(tds[5].Substring(tds[5].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                            if (bf_Result == "04")
                                decimal.TryParse(tds[6].Substring(tds[6].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                            if (bf_Result == "14")
                                decimal.TryParse(tds[7].Substring(tds[7].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                            if (bf_Result == "24")
                                decimal.TryParse(tds[8].Substring(tds[8].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                            if (bf_Result == "05")
                                decimal.TryParse(tds[9].Substring(tds[9].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                            if (bf_Result == "15")
                                decimal.TryParse(tds[10].Substring(tds[10].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                            if (bf_Result == "25")
                                decimal.TryParse(tds[11].Substring(tds[11].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                            if (bf_Result == "0X")
                                decimal.TryParse(tds[12].Substring(tds[12].IndexOf(">") + 1).Replace("\r\n", string.Empty), out bf_SP);
                        }
                        #endregion
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 500wan采集结果
        /// </summary>
        public List<C_JCZQ_MatchResult> Get_Match_Result_From500wan()
        {
            var list = new List<C_JCZQ_MatchResult>();

            for (int i = 0; i < 3; i++)
            {
                var date = DateTime.Now.AddDays(-i).ToString("yyyy-MM-dd");
                var url = string.Format("http://zx.500.com/jczq/kaijiang.php?playid=0&d={0}", date);

                var endf = Encoding.Default;
                try
                {
                    endf = Encoding.GetEncoding("GB2312");
                }
                catch 
                {
                    endf = Encoding.Default;

                }
                var content = PostManager.Get(url, endf, 0, (request) =>
                {
                    if (ServiceHelper.IsUseProxy("JCZQ"))
                    {
                        var proxy = ServiceHelper.GetProxyUrl();
                        if (!string.IsNullOrEmpty(proxy))
                        {
                            request.Proxy = new System.Net.WebProxy(proxy);
                        }
                    }
                });
                var tempArray = new string[] { };
                //step 1 得到div内容
                var index = content.IndexOf("<div class=\"lea_list\">");
                content = content.Substring(index);
                index = content.IndexOf("<div class=\"ld_bottom have_b\">");
                content = content.Substring(0, index);

                //step 2 得到table内容
                index = content.IndexOf("<table");
                content = content.Substring(index, content.Length - index);

                //step 3 去掉多余内容
                index = content.LastIndexOf("</th>");
                content = content.Substring(index, content.Length - index);

                //step 4 得到所有的行
                index = content.IndexOf("<tr");
                if (index == -1) continue;
                content = content.Substring(index, content.Length - index);
                index = content.LastIndexOf("</tr>");
                content = content.Substring(0, index);

                var rows = content.Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var item in rows)
                {
                    try
                    {
                        var row = item.Trim();
                        //if (!row.Contains("<tr id=")) continue;

                        index = row.IndexOf(">");
                        row = row.Substring(index + 1);
                        //解析一场比赛的结果
                        var matchIdName = string.Empty;
                        var matchDate = string.Empty;
                        var matchNumber = string.Empty;
                        var matchId = string.Empty;
                        var leagueName = string.Empty;
                        var homeTeamName = string.Empty;
                        var guestTeamName = string.Empty;
                        //2 是正常；3 是取消的比赛；4 是延期
                        var matchState = "2";
                        var halfHomeTeamScore = 0;
                        var halfGuestTeamScore = 0;
                        var fullHomeTeamScore = 0;
                        var fullGuestTeamScore = 0;

                        var spf_Result = string.Empty;
                        var spf_SP = 1.0M;
                        var brqspf_Result = string.Empty;
                        var brqspf_SP = 1.0M;
                        var zjq_Result = string.Empty;
                        var zjq_SP = 1.0M;
                        var bqc_Result = string.Empty;
                        var bqc_SP = 1.0M;
                        var bf_Result = string.Empty;
                        var bf_SP = 1.0M;
                        var letBall = 0;

                        var tds = row.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                        #region 解析每行数据
                        for (int j = 0; j < tds.Length; j++)
                        {
                            var td = tds[j].Trim();
                            if (!td.Contains("<td")) continue;

                            index = td.IndexOf(">");
                            var tdContent = td.Substring(index + 1).Replace("\r\n", string.Empty);

                            switch (j)
                            {
                                case 0:
                                    //比赛名称
                                    //周二02910-29 04:00
                                    matchIdName = tdContent.Substring(0, 5);
                                    matchDate = GetMatchDate(tdContent, false);
                                    matchNumber = tdContent.Substring(2, 3);
                                    matchId = matchDate + matchNumber;
                                    break;
                                case 1:
                                    //联赛名称
                                    index = tdContent.IndexOf(">");
                                    tdContent = tdContent.Substring(index + 1).Replace("\r\n", string.Empty).Replace("</a>", "");
                                    leagueName = tdContent;
                                    break;
                                case 3:
                                    //主队名称
                                    index = tdContent.IndexOf(">");
                                    tdContent = tdContent.Substring(index + 1).Replace("\r\n", string.Empty).Replace("</a>", "");
                                    homeTeamName = tdContent;
                                    break;
                                case 4:
                                    //让球数
                                    index = tdContent.IndexOf(">");
                                    tdContent = tdContent.Substring(index + 1).Replace("\r\n", string.Empty).Replace("</span>", "");
                                    letBall = int.Parse(tdContent);
                                    break;
                                case 5:
                                    //主队名称
                                    index = tdContent.IndexOf(">");
                                    tdContent = tdContent.Substring(index + 1).Replace("\r\n", string.Empty).Replace("</a>", "");
                                    guestTeamName = tdContent;
                                    break;
                                case 6:
                                    //半场 全场比分
                                    tempArray = tdContent.Split(')');
                                    if (tempArray.Length != 2) continue;
                                    var half = tempArray[0].Replace("(", "").Split(':');
                                    if (half.Length != 2) continue;
                                    var full = tempArray[1].Split(':');
                                    if (full.Length != 2) continue;

                                    //半场比分
                                    halfHomeTeamScore = int.Parse(half[0].Trim());
                                    halfGuestTeamScore = int.Parse(half[1].Trim());
                                    //全场比分
                                    fullHomeTeamScore = int.Parse(full[0].Trim());
                                    fullGuestTeamScore = int.Parse(full[1].Trim());
                                    bf_Result = Format310WinBFResult(string.Format("{0}:{1}", fullHomeTeamScore, fullGuestTeamScore));
                                    //bf_Result = string.Format("{0}{1}", fullHomeTeamScore, fullGuestTeamScore);
                                    bf_SP = 2M;

                                    break;
                                case 8:
                                    //让球胜平负 结果
                                    spf_Result = Format310WinSPFResult(tdContent);

                                    td = tds[9].Replace("</span>", "").Trim();
                                    if (!td.Contains("<td")) continue;

                                    index = td.LastIndexOf(">");
                                    tdContent = td.Substring(index + 1).Replace("\r\n", string.Empty);
                                    if (tdContent == "--")
                                    {
                                        spf_Result = "-1";
                                        spf_SP = 0M;
                                    }
                                    else
                                    {
                                        spf_SP = decimal.Parse(tdContent);
                                    }
                                    break;
                                case 11:
                                    //胜平负 结果
                                    brqspf_Result = Format310WinSPFResult(tdContent);

                                    td = tds[12].Replace("</span>", "").Trim();
                                    if (!td.Contains("<td")) continue;

                                    index = td.LastIndexOf(">");
                                    tdContent = td.Substring(index + 1).Replace("\r\n", string.Empty);
                                    if (tdContent == "--")
                                    {
                                        brqspf_Result = "-1";
                                        brqspf_SP = 0M;
                                    }
                                    else
                                    {
                                        brqspf_SP = decimal.Parse(tdContent);
                                    }
                                    break;
                                case 14:
                                    //总进球数 结果
                                    zjq_Result = Format310WinZJQResult(tdContent);

                                    td = tds[15].Replace("</span>", "").Trim();
                                    if (!td.Contains("<td")) continue;

                                    index = td.LastIndexOf(">");
                                    tdContent = td.Substring(index + 1).Replace("\r\n", string.Empty);
                                    if (tdContent == "--")
                                    {
                                        zjq_Result = "-1";
                                        zjq_SP = 0M;
                                    }
                                    else
                                    {
                                        zjq_SP = decimal.Parse(tdContent);
                                    }
                                    break;
                                case 17:
                                    //半全场 结果
                                    bqc_Result = Format310WinBQCResult(tdContent);

                                    td = tds[18].Replace("</span>", "").Trim();
                                    if (!td.Contains("<td")) continue;

                                    index = td.LastIndexOf(">");
                                    tdContent = td.Substring(index + 1).Replace("\r\n", string.Empty);
                                    if (tdContent == "--")
                                    {
                                        bqc_Result = "-1";
                                        bqc_SP = 0M;
                                    }
                                    else
                                    {
                                        bqc_SP = decimal.Parse(tdContent);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        #endregion

                        if (string.IsNullOrEmpty(matchState)) continue;
                        if (matchState == "2" && (bf_SP == 1M && bqc_SP == 1M && spf_SP == 1M && zjq_SP == 1M && brqspf_SP == 1M)) continue;
                        if (string.IsNullOrEmpty(bf_Result) || string.IsNullOrEmpty(spf_Result) || string.IsNullOrEmpty(brqspf_Result) || string.IsNullOrEmpty(zjq_Result) || string.IsNullOrEmpty(bqc_Result)) continue;
                        list.Add(new C_JCZQ_MatchResult
                        {
                            MatchId = matchId,
                            MatchData = matchDate,
                            MatchNumber = matchNumber,
                            CreateTime = DateTime.Now,
                            MatchState = matchState,
                            FullGuestTeamScore = fullGuestTeamScore,
                            FullHomeTeamScore = fullHomeTeamScore,
                            HalfGuestTeamScore = halfGuestTeamScore,
                            HalfHomeTeamScore = halfHomeTeamScore,
                            BF_Result = bf_Result,
                            BF_SP = bf_SP,
                            BQC_Result = bqc_Result,
                            BQC_SP = bqc_SP,
                            SPF_Result = spf_Result,
                            SPF_SP = spf_SP,
                            ZJQ_Result = zjq_Result,
                            ZJQ_SP = zjq_SP,
                            BRQSPF_Result = brqspf_Result,
                            BRQSPF_SP = brqspf_SP,
                        });
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog("解析表格数据失败：" + ex.ToString());
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 澳客采集结果
        /// </summary>
        /// <returns></returns>
        public List<C_JCZQ_MatchResult> Get_Match_Result_FromOk()
        {
            var list = new List<C_JCZQ_MatchResult>();
            var url = string.Format("http://www.okooo.com/jingcai/kaijiang/?LotteryType=SportteryWDL&StartDate={0}&EndDate={1}", DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd"));
            var encoding = Encoding.GetEncoding("gb2312");
            var content = PostManager.Get(url, encoding, 0, null);
            if (string.IsNullOrEmpty(content) || content.Trim() == "404")
                return list;
            var tempArray = new string[] { };
            //step 1 得到div内容
            var index = content.IndexOf("<table");
            content = content.Substring(index);
            index = content.IndexOf("</table>");
            content = content.Substring(0, index);
            var rows = content.Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);
            var aList = new List<string>();
            foreach (var item in rows)
            {
                if (!item.Contains("<td class=\"noborder\">")) continue;

                try
                {
                    var row = item.Trim();

                    index = row.IndexOf(">");
                    row = row.Substring(index + 1);
                    //解析一场比赛的结果
                    var matchIdName = string.Empty;
                    var matchDate = string.Empty;
                    var matchNumber = string.Empty;
                    var matchId = string.Empty;
                    var leagueName = string.Empty;
                    var halfScore = string.Empty;
                    var Score = string.Empty;
                    //2 是正常；3 是取消的比赛；4 是延期
                    var matchState = string.Empty;
                    var halfHomeTeamScore = 0;
                    var halfGuestTeamScore = 0;
                    var fullHomeTeamScore = 0;
                    var fullGuestTeamScore = 0;

                    var spf_Result = string.Empty;
                    var spf_SP = 1.0M;
                    var brqspf_Result = string.Empty;
                    var brqspf_SP = 1.0M;
                    var zjq_Result = string.Empty;
                    var zjq_SP = 1.0M;
                    var bqc_Result = string.Empty;
                    var bqc_SP = 1.0M;
                    var bf_Result = string.Empty;
                    var bf_SP = 1.0M;

                    var tds = row.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);

                    #region 解析每行数据
                    for (int i = 0; i < tds.Length; i++)
                    {
                        var td = tds[i].Replace("-->", "").Trim();
                        if (!td.Contains("<td")) continue;

                        //index = td.IndexOf(">");
                        //var tdContent = td.Substring(index + 1).Replace("\r\n", string.Empty);

                        var tdContent = CutHtml(td);
                        switch (i)
                        {
                            case 0:
                                //比赛名称
                                //周二02910-29 04:00
                                matchIdName = tdContent.Substring(0, 5);
                                matchDate = GetMatchDate(tdContent, false);
                                matchNumber = tdContent.Substring(2, 3);
                                matchId = matchDate + matchNumber;
                                break;
                            case 6:
                                if (tdContent == "-") continue;
                                //半场比分
                                halfScore = tdContent.Replace("-", ":");
                                break;
                            case 7:
                                if (string.IsNullOrEmpty(halfScore)) continue;
                                //全场比分
                                Score = tdContent.Replace("-", ":");
                                matchState = "2";
                                fullHomeTeamScore = int.Parse(Score.Split(':')[0]);
                                fullGuestTeamScore = int.Parse(Score.Split(':')[1]);
                                halfHomeTeamScore = int.Parse(halfScore.Split(':')[0]);
                                halfGuestTeamScore = int.Parse(halfScore.Split(':')[1]);
                                break;
                            case 8:
                                if (string.IsNullOrEmpty(halfScore)) continue;
                                //index = tdContent.IndexOf(">");
                                //tdContent = tdContent.Substring(index + 1).Replace("</b>", "");
                                if (string.IsNullOrEmpty(tdContent))
                                {
                                    brqspf_Result = "-1";
                                    brqspf_SP = 1M;
                                }
                                else
                                {
                                    brqspf_Result = tdContent;
                                }
                                break;
                            case 9:
                                if (string.IsNullOrEmpty(halfScore)) continue;
                                if (brqspf_Result == "-1")
                                {
                                    if (tdContent == "-")
                                        brqspf_SP = 0M;
                                    continue;
                                }
                                brqspf_SP = decimal.Parse(tdContent);
                                break;
                            case 11:
                                if (string.IsNullOrEmpty(halfScore)) continue;
                                //index = tdContent.IndexOf(">");
                                //tdContent = tdContent.Substring(index + 1).Replace("</b>", "");
                                if (string.IsNullOrEmpty(tdContent))
                                {
                                    spf_Result = "-1";
                                    spf_SP = 1M;
                                }
                                else
                                {
                                    spf_Result = tdContent;
                                }
                                break;
                            case 12:
                                if (string.IsNullOrEmpty(halfScore)) continue;
                                if (spf_Result == "-1")
                                {
                                    if (tdContent == "-")
                                        spf_SP = 0M;
                                    continue;
                                }
                                spf_SP = decimal.Parse(tdContent);
                                break;
                            case 13:
                                if (string.IsNullOrEmpty(halfScore)) continue;
                                //index = tdContent.IndexOf(">");
                                //tdContent = tdContent.Substring(index + 1).Replace("</b>", "");
                                if (string.IsNullOrEmpty(tdContent))
                                {
                                    bf_Result = "-1";
                                    bf_SP = 1M;
                                }
                                else
                                {
                                    bf_Result = Format310WinBFResult(tdContent);
                                }
                                break;
                            case 14:
                                if (string.IsNullOrEmpty(halfScore)) continue;
                                if (bf_Result == "-1")
                                {
                                    if (tdContent == "-")
                                        bf_SP = 0M;
                                    continue;
                                }
                                bf_SP = decimal.Parse(tdContent);
                                break;
                            case 15:
                                if (string.IsNullOrEmpty(halfScore)) continue;
                                //index = tdContent.IndexOf(">");
                                //tdContent = tdContent.Substring(index + 1).Replace("</b>", "");
                                if (string.IsNullOrEmpty(tdContent))
                                {
                                    zjq_Result = "-1";
                                    zjq_SP = 1M;
                                }
                                else
                                {
                                    zjq_Result = Format310WinZJQResult(tdContent);
                                }
                                break;
                            case 16:
                                if (string.IsNullOrEmpty(halfScore)) continue;
                                if (zjq_Result == "-1")
                                {
                                    if (tdContent == "-")
                                        zjq_SP = 0M;
                                    continue;
                                }
                                zjq_SP = decimal.Parse(tdContent);
                                break;
                            case 17:
                                if (string.IsNullOrEmpty(halfScore)) continue;
                                //index = tdContent.IndexOf(">");
                                //tdContent = tdContent.Substring(index + 1).Replace("</b>", "");
                                if (string.IsNullOrEmpty(tdContent))
                                {
                                    bqc_Result = "-1";
                                    bqc_SP = 1M;
                                }
                                else
                                {
                                    bqc_Result = tdContent.Replace("-", "");
                                }
                                break;
                            case 18:
                                if (string.IsNullOrEmpty(halfScore)) continue;
                                if (bqc_Result == "-1")
                                {
                                    if (tdContent == "-")
                                        bqc_SP = 0M;
                                    continue;
                                }
                                bqc_SP = decimal.Parse(tdContent);
                                break;
                            default:
                                break;
                        }
                    }
                    #endregion
                    if (string.IsNullOrEmpty(matchState) || (bf_Result == "" && bqc_Result == "" && spf_Result == "" && zjq_Result == "" && brqspf_Result == "") || (bf_Result == "-1" && bqc_Result == "-1" && spf_Result == "-1" && zjq_Result == "-1" && brqspf_Result == "-1")) continue;
                    if (string.IsNullOrEmpty(bf_Result) || string.IsNullOrEmpty(spf_Result) || string.IsNullOrEmpty(brqspf_Result) || string.IsNullOrEmpty(zjq_Result) || string.IsNullOrEmpty(bqc_Result)) continue;
                    list.Add(new C_JCZQ_MatchResult
                    {
                        MatchId = matchId,
                        MatchData = matchDate,
                        MatchNumber = matchNumber,
                        CreateTime = DateTime.Now,
                        MatchState = matchState,
                        FullGuestTeamScore = fullGuestTeamScore,
                        FullHomeTeamScore = fullHomeTeamScore,
                        HalfGuestTeamScore = halfGuestTeamScore,
                        HalfHomeTeamScore = halfHomeTeamScore,
                        BF_Result = bf_Result,
                        BF_SP = bf_SP,
                        BQC_Result = bqc_Result,
                        BQC_SP = bqc_SP,
                        SPF_Result = spf_Result,
                        SPF_SP = spf_SP,
                        ZJQ_Result = zjq_Result,
                        ZJQ_SP = zjq_SP,
                        BRQSPF_Result = brqspf_Result,
                        BRQSPF_SP = brqspf_SP,
                    });
                }
                catch (Exception ex)
                {
                    this.WriteLog("解析表格数据失败：" + ex.ToString());
                }
            }
            return list;
        }

        /// <summary>
        /// 网易采集结果
        /// </summary>
        /// <returns></returns>
        public List<C_JCZQ_MatchResult> Get_Match_Result_FromWy()
        {
            var list = new List<C_JCZQ_MatchResult>();
            for (int d = 0; d < 3; d++)
            {
                var url = string.Format("http://caipiao.163.com/award/jczqspfp.html?category=all&selectedDate={0}", DateTime.Now.AddDays(-d).ToString("yyyy-MM-dd"));
                var encoding = Encoding.UTF8;
                var content = PostManager.Get(url, encoding, 0, null);
                if (string.IsNullOrEmpty(content) || content.Trim() == "404")
                    return list;
                var tempArray = new string[] { };
                //step 1 得到div内容
                var index = content.IndexOf("<div class=\"mt_10 all_play clearfix\">");
                content = content.Substring(index);
                index = content.IndexOf("<div class=\"ball_right\">");
                content = content.Substring(0, index);

                index = content.IndexOf("<table class=\"zqdc_table ss_list\">");
                var table = content.Substring(index);
                index = table.IndexOf("</table>");
                table = table.Substring(0, index);

                index = table.IndexOf("<tbody>");
                table = table.Substring(index);
                //index = table.IndexOf(" </table>");
                //table = table.Substring(0, index);

                index = content.IndexOf("<dl class=\"bf_detail\">");
                var table2 = content.Substring(index);
                index = table2.IndexOf("</dl>");
                table2 = table2.Substring(0, index);

                //index = table.IndexOf("<tbody>");
                //table = table.Substring(index);

                var rows = table.Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);
                var rows2 = table2.Split(new string[] { "</dd>" }, StringSplitOptions.RemoveEmptyEntries);


                var matchidList = new List<string>();
                foreach (var item in rows)
                {
                    if (!item.Contains("<td>")) continue;

                    var row = item.Trim();
                    index = row.IndexOf(">");
                    row = row.Substring(index + 1);

                    var tds = row.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                    var matchid = CutHtml(tds[0]);
                    var matchIdName = matchid.Substring(0, 5);
                    var matchDate = GetMatchDate(matchid, false);
                    var matchNumber = matchid.Substring(2, 3);
                    var matchId = matchDate + matchNumber;
                    matchidList.Add(string.Format("{0}^{1}^{2}^{3}", matchId, matchIdName, matchDate, matchNumber));
                }
                var brqspfList = new List<string>();
                var spfList = new List<string>();
                var zjqList = new List<string>();
                var bqcList = new List<string>();
                var bfList = new List<string>();

                #region 处理结果
                for (int i = 0; i < rows2.Length - 1; i++)
                {
                    var dd = rows2[i].Trim();
                    index = dd.IndexOf("<tbody>");
                    var str = dd.Substring(index);
                    var trl = str.Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);
                    switch (i)
                    {
                        case 0:
                            //不让求胜平负
                            foreach (var item in trl)
                            {
                                if (!item.Contains("<td")) continue;
                                var tds = item.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                                var c = CutHtml(tds[0]);
                                if (c.Contains("-") || c.Contains("取消"))
                                {
                                    brqspfList.Add("-1");
                                    continue;
                                }
                                var brqspf = Format310WinSPFResult(c);
                                brqspfList.Add(brqspf);
                            }
                            break;
                        case 1:
                            //胜平负
                            foreach (var item in trl)
                            {
                                if (!item.Contains("<td")) continue;
                                var tds = item.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                                var c = CutHtml(tds[0]);
                                if (c.Contains("-") || c.Contains("取消"))
                                {
                                    spfList.Add("-1");
                                    continue;
                                }
                                var spf = Format310WinSPFResult(c.Replace("让球", ""));
                                spfList.Add(spf);
                            }
                            break;
                        case 2:
                            //总进球
                            foreach (var item in trl)
                            {
                                if (!item.Contains("<td")) continue;
                                var tds = item.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                                var c = CutHtml(tds[0]);
                                if (c.Contains("-") || c.Contains("取消"))
                                {
                                    zjqList.Add("-1");
                                    continue;
                                }
                                var zjq = Format310WinZJQResult(c);
                                zjqList.Add(zjq);
                            }
                            break;
                        case 3:
                            //半全场
                            foreach (var item in trl)
                            {
                                if (!item.Contains("<td")) continue;
                                var tds = item.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                                var c = CutHtml(tds[0]);
                                if (c.Contains("-") || c.Contains("取消"))
                                {
                                    bqcList.Add("-1");
                                    continue;
                                }
                                var bqc = Format310WinBQCResult(c);
                                bqcList.Add(bqc);
                            }
                            break;
                        case 4:
                            //比分
                            foreach (var item in trl)
                            {
                                if (!item.Contains("<td")) continue;
                                var tds = item.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                                var c = CutHtml(tds[0]);
                                if (c.Contains("-") || c.Contains("取消"))
                                {
                                    bfList.Add("-1");
                                    continue;
                                }
                                var bf = Format310WinBFResult(c);
                                bfList.Add(bf);
                            }
                            break;
                        default:
                            break;
                    }
                }
                #endregion

                if (matchidList.Count == brqspfList.Count && matchidList.Count == spfList.Count && matchidList.Count == zjqList.Count && matchidList.Count == bqcList.Count && matchidList.Count == bfList.Count)
                {
                    for (int i = 0; i < matchidList.Count; i++)
                    {
                        var strList = matchidList[i].Split('^');
                        if (strList.Length != 4)
                        {
                            list.Add(new C_JCZQ_MatchResult
                            {
                                MatchId = strList[0],
                                MatchData = strList[2],
                                MatchNumber = strList[3],
                                CreateTime = DateTime.Now,
                                MatchState = "2",
                                FullGuestTeamScore = 0,
                                FullHomeTeamScore = 0,
                                HalfGuestTeamScore = 0,
                                HalfHomeTeamScore = 0,
                                BF_Result = bfList[i],
                                BF_SP = 1,
                                BQC_Result = bqcList[i],
                                BQC_SP = 1,
                                SPF_Result = spfList[i],
                                SPF_SP = 1,
                                ZJQ_Result = zjqList[i],
                                ZJQ_SP = 1,
                                BRQSPF_Result = brqspfList[i],
                                BRQSPF_SP = 1,
                            });
                        }
                    }
                }

            }
            return list;
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

        public void Stop()
        {
            BeStop = 1;
          
        }

       
    }
}
