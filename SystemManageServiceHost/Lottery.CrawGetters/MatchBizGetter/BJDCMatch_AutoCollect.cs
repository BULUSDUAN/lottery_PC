using EntityModel;
using EntityModel.Domain.Entities;
using EntityModel.Enum;
using EntityModel.LotteryJsonInfo;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;
using KaSon.FrameWork.Common.Expansion;
using System.Text.RegularExpressions;
using KaSon.FrameWork.ORM.Helper;
using MongoDB.Driver;
using EntityModel.CoreModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EntityModel.ExceptionExtend;
using System.Threading;

namespace Lottery.CrawGetters.MatchBizGetter
{
  

    public class BJDCMatch_AutoCollect : IBallAutoCollect
    {
       
        private const string logCategory = "Services.Info";
        private string logInfoSource = "Auto_Collect_BJDC_Info_";
        private const string logErrorCategory = "Services.Error";
        private const string logErrorSource = "Auto_Collect_BJDC_Error";
        private long BeStop = 0;
        private System.Timers.Timer timer = null;
        private string Sp_SavePath = string.Empty;
        private int LeagueAdvanceMinutes = 15;
        private List<C_BJDC_Match> CurrentMatchList = new List<C_BJDC_Match>();
        private int collectResultTotalTime = 1000 * 60 * 10;
        private int currentTime = 0;
        private List<BJDCMatchResultCache> cacheMatchResult = new List<BJDCMatchResultCache>();
        //private MatchManager manager = new MatchManager(DbAccess_Match_Helper.DbAccess);
        private IMongoDatabase mDB;
        public BJDCMatch_AutoCollect(IMongoDatabase _mDB)
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


                        DoWork(gameCode, true);

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
        public void DoWork(string gameCode, bool getResult)
        {
            this.WriteLog("进入DoWork  开始采集数据");

            //采集奖期
            //采集比赛信息
            var issuseNumber = string.Empty;
            try
            {
                this.WriteLog("开始采集赛事信息...");
                var maxCacheTimes = int.Parse(ServiceHelper.GetSystemConfig("BJDC_Result_CacheTimes"));
                if (maxCacheTimes <= 0)
                    throw new Exception("BJDC_Result_CacheTimes 必须大于0 ");

                //var p = new ObjectPersistence(DbAccess_Match_Helper.DbAccess);
                var issuseList = new List<KeyValuePair<DBChangeState, C_BJDC_Issuse>>();
                var leagueList = new List<KeyValuePair<DBChangeState, C_BJDC_Match>>();
                var matchResultList = new List<KeyValuePair<DBChangeState, C_BJDC_MatchResult>>();
                var matchResult_sfggList = new List<KeyValuePair<DBChangeState, BJDC_Match_SFGGResult>>();
                var matchsfggList = new List<KeyValuePair<DBChangeState, BJDC_Match_SFGG>>();

                #region 采集期号，比赛，结果

                var maxIssuseCount = 2;

                //取期号
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
                long tt = (DateTime.Now.Ticks - startTime.Ticks) / 10000;
                var issuseStrList = new List<string>();
                #region 500wan
                var url = "http://trade.500.com/bjdc/index.php";
                var encoding = Encoding.GetEncoding("gb2312");
                var content = PostManager.Get(url, encoding);
                //step 1 得到div内容
                var index = content.IndexOf("<option");
                content = content.Substring(index);
                index = content.IndexOf("</select>");
                content = content.Substring(0, index);
                var rows = content.Split(new string[] { "</option>" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in rows)
                {
                    var issuse = CutHtml(item).Replace("当前期", "").Replace(" ", "").Replace("投注比例", "").Replace("平均赔率", "").Replace("亚　　盘", "");
                    if (string.IsNullOrEmpty(issuse) || issuse.Contains("亚"))
                        continue;
                    if (issuseStrList.Count >= maxIssuseCount)
                        break;
                    issuseStrList.Add(issuse);
                }
                #endregion

                #region okooo
                //var url_ok = "http://www.okooo.com/danchang/";
                //var content_ok = PostManager.Get(url_ok, Encoding.GetEncoding("gb2312"));
                ////step 1 得到div内容
                //var index_ok = content_ok.IndexOf("<option");
                //content_ok = content_ok.Substring(index_ok);
                //index_ok = content_ok.IndexOf("</select>");
                //content_ok = content_ok.Substring(0, index_ok);
                //var rows_ok = content_ok.Split(new string[] { "</option>" }, StringSplitOptions.RemoveEmptyEntries);
                //foreach (var item in rows_ok)
                //{
                //    var issuse = CutHtml(item).Replace("当前期", "").Replace(" ", "").Replace("投注比例", "").Replace("平均赔率", "").Replace("亚　　盘", "").Replace("期", "");
                //    if (string.IsNullOrEmpty(issuse) || issuse.Contains("亚"))
                //        continue;
                //    if (issuseStrList.Count >= maxIssuseCount)
                //        break;
                //    issuseStrList.Add(issuse);
                //}
                #endregion

                //#region 澳客

                //var url_wy = "http://www.woying.com/danchang/shengpingfu/";
                //var content_wy = PostManager.Get(url_wy, Encoding.GetEncoding("utf-8"));

                ////step 1 得到div内容
                //index = content_wy.IndexOf("当前期数为:<b>") + "当前期数为:<b>".Length;
                //content_wy = content_wy.Substring(index);
                //index = content_wy.IndexOf("</b>期");
                //content_wy = "1" + content_wy.Substring(0, index);
                //if (!issuseStrList.Contains(content_wy))
                //    issuseStrList.Add(content_wy);

                //#endregion

                foreach (var issuseStr in issuseStrList)
                {
                    if (string.IsNullOrEmpty(issuseNumber))
                        issuseNumber = issuseStr;

                    var currentIssuseList = new List<KeyValuePair<DBChangeState, C_BJDC_Issuse>>();
                    var currentMatchResultList = new List<KeyValuePair<DBChangeState, C_BJDC_MatchResult>>();
                    var currentSFGGList = new List<KeyValuePair<DBChangeState, BJDC_Match_SFGG>>();
                    var currentMatchResult_sfggList = new List<KeyValuePair<DBChangeState, BJDC_Match_SFGGResult>>();
                    this.WriteLog(string.Format("开始采集期：{0}", issuseStr));

                    var currentMatchList = BuildLeagueInfoCollectionFrom9188(issuseStr, getResult, out currentIssuseList, out currentMatchResultList, out currentSFGGList, out currentMatchResult_sfggList);
                    issuseList.AddRange(currentIssuseList);//奖期
                    leagueList.AddRange(currentMatchList);//比赛
                    matchResultList.AddRange(currentMatchResultList);//结果
                    matchsfggList.AddRange(currentSFGGList);
                    matchResult_sfggList.AddRange(currentMatchResult_sfggList);

                    this.WriteLog(string.Format("采集到期号{0}，赛事队伍条数{1}条", issuseStr, leagueList.Count));
                    this.WriteLog(string.Format("采集到期号{0}，比赛结果{1}条", issuseStr, matchResultList.Count));
                    this.WriteLog(string.Format("采集到期号{0}，胜负过关赛事队伍条数{1}条", issuseStr, matchsfggList.Count));

                    //var l = GetBJDCMatchList_CPDJY(issuseStr);
                }


                #endregion

                //this.WriteLog(string.Format("采集到期号{0}，赛事队伍条数{1}条", issuseNumber, leagueList.Count));
                //this.WriteLog(string.Format("采集到期号{0}，比赛结果{1}条", issuseNumber, matchResultList.Count));

                #region 通知

                if (issuseList.Count > 0)
                {
                    //foreach (var item in issuseList)
                    //{
                    //    try
                    //    {
                    //        p.Add(item.Value);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        this.WriteLog(string.Format("向数据库写入 北单奖期 数据异常 编号：{0}，异常：{1}", item.Value.IssuseNumber, ex.ToString()));
                    //    }
                    //}
                    var category = (int)NoticeCategory.BJDC_Issuse;
                    var state = (int)DBChangeState.Add;
                    var param = string.Join("_", (from l in issuseList select l.Value.IssuseNumber).ToArray());
                    var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                    var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                    //发送 北单期号 添加 通知
                    var innerKey = string.Format("{0}_{1}", "C_BJDC_Issuse", "Add");
                 //   ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.BJDC_Issuse);
                    new Sports_Business(this.mDB).UpdateLocalData(param, "", NoticeType.BJDC_Issuse, innerKey);
                    //ServiceHelper.SendNotice(issuse_Request, (log) =>
                    //{
                    //    this.WriteLog(log);
                    //});
                }

                #region 比赛队伍通知
                var addList = new List<C_BJDC_Match>();
                var updateList = new List<C_BJDC_Match>();
                foreach (var league in leagueList)
                {
                    try
                    {
                        if (league.Key == DBChangeState.Add)
                        {
                            addList.Add(league.Value);
                            //p.Add(league.Value);
                        }
                        else
                        {
                            updateList.Add(league.Value);
                            //p.Modify(league.Value);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(string.Format("向数据库写入 北单队伍 数据异常 编号：{0}，异常：{1}", league.Value.Id, ex.ToString()));
                    }
                }
                if (addList.Count > 0)
                {
                    var g = addList.GroupBy(t => t.IssuseNumber);
                    foreach (var i in g)
                    {
                        var category = (int)NoticeCategory.BJDC_Match;
                        var state = (int)DBChangeState.Add;
                        var param = string.Format("{0}|{1}", i.Key, string.Join("_", (from l in addList where l.IssuseNumber == i.Key select l.MatchOrderId).ToArray()));
                        //var param = string.Join("_", (from l in addList select l.Id).ToArray());
                        var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                        var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                        //发送 北单队伍 添加 通知
                        var innerKey = string.Format("{0}_{1}", "C_BJDC_Match", "Add");
                     //   ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.BJDC_Match);
                        new Sports_Business(this.mDB).UpdateLocalData(param, "", NoticeType.BJDC_Match, innerKey);
                        //ServiceHelper.SendNotice(issuse_Request, (log) =>
                        //{
                        //    this.WriteLog(log);
                        //});
                    }
                }
                if (updateList.Count > 0)
                {
                    var g = updateList.GroupBy(t => t.IssuseNumber);
                    foreach (var i in g)
                    {
                        var category = (int)NoticeCategory.BJDC_Match;
                        var state = (int)DBChangeState.Update;
                        var param = string.Format("{0}|{1}", i.Key, string.Join("_", (from l in updateList where l.IssuseNumber == i.Key select l.MatchOrderId).ToArray()));
                        //var param = string.Join("_", (from l in updateList select l.Id).ToArray());
                        var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                        var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                        //发送 北单队伍 修改 通知
                        var innerKey = string.Format("{0}_{1}", "C_BJDC_Match", "Update");
                       // ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.BJDC_Match);
                        new Sports_Business(this.mDB).UpdateLocalData(param, "", NoticeType.BJDC_Match, innerKey);
                        //ServiceHelper.SendNotice(issuse_Request, (log) =>
                        //{
                        //    this.WriteLog(log);
                        //});
                    }
                }
                #endregion

                #region 比赛胜负过关队伍通知

                var add_sfggList = new List<BJDC_Match_SFGG>();
                var update_sfggList = new List<BJDC_Match_SFGG>();
                foreach (var league in matchsfggList)
                {
                    try
                    {
                        if (league.Key == DBChangeState.Add)
                        {
                            add_sfggList.Add(league.Value);
                            //p.Add(league.Value);
                        }
                        else
                        {
                            update_sfggList.Add(league.Value);
                            //p.Modify(league.Value);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(string.Format("向数据库写入 北单队伍 数据异常 编号：{0}，异常：{1}", league.Value.Id, ex.ToString()));
                    }
                }
                if (add_sfggList.Count > 0)
                {
                    var g = add_sfggList.GroupBy(t => t.IssuseNumber);
                    foreach (var i in g)
                    {
                        var category = (int)NoticeCategory.BJDC_Match_SFGG;
                        var state = (int)DBChangeState.Add;
                        var param = string.Format("{0}|{1}", i.Key, string.Join("_", (from l in add_sfggList where l.IssuseNumber == i.Key select l.MatchOrderId).ToArray()));
                        //var param = string.Join("_", (from l in addList select l.Id).ToArray());
                        var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                        var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                        //发送 北单队伍 添加 通知
                        var innerKey = string.Format("{0}_{1}", "BJDC_Match_SFGG", "Add");
                      //  ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.BJDC_Match_SFGG);
                        new Sports_Business(this.mDB).UpdateLocalData(param, "", NoticeType.BJDC_Match_SFGG, innerKey);
                        //ServiceHelper.SendNotice(issuse_Request, (log) =>
                        //{
                        //    this.WriteLog(log);
                        //});
                    }
                }
                if (update_sfggList.Count > 0)
                {
                    var g = update_sfggList.GroupBy(t => t.IssuseNumber);
                    foreach (var i in g)
                    {
                        var category = (int)NoticeCategory.BJDC_Match_SFGG;
                        var state = (int)DBChangeState.Update;
                        var param = string.Format("{0}|{1}", i.Key, string.Join("_", (from l in update_sfggList where l.IssuseNumber == i.Key select l.MatchOrderId).ToArray()));
                        //var param = string.Join("_", (from l in updateList select l.Id).ToArray());
                        var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                        var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                        //发送 北单队伍 修改 通知
                        var innerKey = string.Format("{0}_{1}", "BJDC_Match_SFGG", "Update");
                      //  ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.BJDC_Match_SFGG);
                        new Sports_Business(this.mDB).UpdateLocalData(param, "", NoticeType.BJDC_Match_SFGG, innerKey);
                        //ServiceHelper.SendNotice(issuse_Request, (log) =>
                        //{
                        //    this.WriteLog(log);
                        //});
                    }
                }
                #endregion

                #region 比赛结果通知
                var addResultList = new List<C_BJDC_MatchResult>();
                var updateResultList = new List<C_BJDC_MatchResult>();
                foreach (var r in matchResultList)
                {
                    try
                    {
                        if (r.Key == DBChangeState.Add)
                        {
                            addResultList.Add(r.Value);
                            //p.Add(r.Value);
                        }
                        else
                        {
                            updateResultList.Add(r.Value);
                            //p.Modify(r.Value);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(string.Format("向数据库写入 北单比赛结果 数据异常 编号：{0}，异常：{1}", r.Value.Id, ex.ToString()));
                    }
                }
                if (addResultList.Count > 0)
                {
                    var g = addResultList.GroupBy(t => t.IssuseNumber);
                    foreach (var i in g)
                    {
                        var category = (int)NoticeCategory.BJDC_MatchResult;
                        var state = (int)DBChangeState.Add;
                        //期号|比赛编号_结果&比赛编号_结果&比赛编号_结果
                        //其中结果应表示为   SPF:3;ZJQ:5;BF:46;
                        var temp = new List<string>();
                        foreach (var item in addResultList.Where(t => t.IssuseNumber == i.Key))
                        {
                            //todo:注意看 这4个结果中有没有多余的字符,以下为正确的格式
                            //SPF_Result	SXDS_Result	ZJQ_Result	BF_Result	BQC_Result
                            //    3	          XS	         2	       20	      33
                            temp.Add(string.Format("{0}_SPF:{1};ZJQ:{2};SXDS:{3};BF:{4};BQC:{5};", item.MatchOrderId, item.SPF_Result, item.ZJQ_Result, item.SXDS_Result, item.BF_Result, item.BQC_Result));
                        }
                        var param = string.Format("{0}|{1}", i.Key, string.Join("#", temp.ToArray()));

                        var paramT = string.Format("{0}|{1}", i.Key, string.Join("_", (from l in addResultList where l.IssuseNumber == i.Key select l.MatchOrderId).ToArray()));
                        //var param = string.Join("_", (from l in addResultList select l.Id).ToArray());
                        var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                        var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, paramT, sign);

                        //发送 北单比赛结果 添加 通知
                        var innerKey = string.Format("{0}_{1}", "C_BJDC_MatchResult", "Add");
                       // ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.BJDC_MatchResult);
                        new Sports_Business(this.mDB).UpdateLocalData(param, "", NoticeType.BJDC_MatchResult, innerKey);
                        try
                        {
                            //this.WriteLog("开始生成静态相关数据.");

                            //this.WriteLog("1.生成开奖结果首页");
                            //var log = ServiceHelper.SendBuildStaticFileNotice("301");
                            //this.WriteLog("1.生成开奖结果首页结果：" + log);

                            ////this.WriteLog("2.生成彩种开奖历史");
                            ////log = ServiceHelper.SendBuildStaticFileNotice("302", "BJDC");
                            ////this.WriteLog("2.生成彩种开奖历史结果：" + log);

                            //this.WriteLog("2.生成彩种开奖详细");
                            //log = ServiceHelper.SendBuildStaticFileNotice("303", "BJDC");
                            //this.WriteLog("2.生成彩种开奖详细结果：" + log);
                        }
                        catch (Exception ex)
                        {
                            this.WriteLog("生成静态数据异常：" + ex.Message);
                        }

                        //ServiceHelper.SendNotice(issuse_Request, (log) =>
                        //{
                        //    this.WriteLog(log);
                        //});
                    }
                }
                if (updateResultList.Count > 0)
                {
                    var g = updateResultList.GroupBy(t => t.IssuseNumber);
                    foreach (var i in g)
                    {
                        var category = (int)NoticeCategory.BJDC_MatchResult;
                        var state = (int)DBChangeState.Update;

                        //期号|比赛编号_结果&比赛编号_结果&比赛编号_结果
                        //其中结果应表示为   SPF:3;ZJQ:5;BF:46;
                        var temp = new List<string>();
                        foreach (var item in updateResultList.Where(t => t.IssuseNumber == i.Key))
                        {
                            //todo:注意看 这4个结果中有没有多余的字符,以下为正确的格式
                            //SPF_Result	SXDS_Result	ZJQ_Result	BF_Result	BQC_Result
                            //    3	          XS	         2	       20	      33
                            temp.Add(string.Format("{0}_SPF:{1};ZJQ:{2};SXDS:{3};BF:{4};BQC:{5};", item.MatchOrderId, item.SPF_Result, item.ZJQ_Result, item.SXDS_Result, item.BF_Result, item.BQC_Result));
                        }
                        var param = string.Format("{0}|{1}", i.Key, string.Join("#", temp.ToArray()));


                        var paramT = string.Format("{0}|{1}", i.Key, string.Join("_", (from l in updateResultList where l.IssuseNumber == i.Key select l.MatchOrderId).ToArray()));
                        //var param = string.Join("_", (from l in updateResultList select l.Id).ToArray());
                        var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                        var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, paramT, sign);

                        //发送 北单比赛结果 修改 通知
                        var innerKey = string.Format("{0}_{1}", "C_BJDC_MatchResult", "Update");
                     //   ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.BJDC_MatchResult);
                        new Sports_Business(this.mDB).UpdateLocalData(param, "", NoticeType.BJDC_MatchResult, innerKey);
                        try
                        {
                            //this.WriteLog("开始生成静态相关数据.");

                            //this.WriteLog("1.生成开奖结果首页");
                            //var log = ServiceHelper.SendBuildStaticFileNotice("301");
                            //this.WriteLog("1.生成开奖结果首页结果：" + log);

                            //this.WriteLog("2.生成彩种开奖历史");
                            //log = ServiceHelper.SendBuildStaticFileNotice("302", "BJDC");
                            //this.WriteLog("2.生成彩种开奖历史结果：" + log);

                            //this.WriteLog("3.生成彩种开奖详细");
                            //log = ServiceHelper.SendBuildStaticFileNotice("303", "BJDC");
                            //this.WriteLog("3.生成彩种开奖详细结果：" + log);
                        }
                        catch (Exception ex)
                        {
                            this.WriteLog("生成静态数据异常：" + ex.Message);
                        }

                        //ServiceHelper.SendNotice(issuse_Request, (log) =>
                        //{
                        //    this.WriteLog(log);
                        //});
                    }
                }
                #endregion

                #region 胜负过关比赛结果通知
                var addResult_sfggList = new List<BJDC_Match_SFGGResult>();
                var updateResult_sfggList = new List<BJDC_Match_SFGGResult>();
                foreach (var r in matchResult_sfggList)
                {
                    try
                    {
                        if (r.Key == DBChangeState.Add)
                        {
                            addResult_sfggList.Add(r.Value);
                            //p.Add(r.Value);
                        }
                        else
                        {
                            updateResult_sfggList.Add(r.Value);
                            //p.Modify(r.Value);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(string.Format("向数据库写入 北单比赛结果 数据异常 编号：{0}，异常：{1}", r.Value.Id, ex.ToString()));
                    }
                }
                if (addResult_sfggList.Count > 0)
                {
                    var g = addResult_sfggList.GroupBy(t => t.IssuseNumber);
                    foreach (var i in g)
                    {
                        var category = (int)NoticeCategory.BJDC_MatchResult_SFGG;
                        var state = (int)DBChangeState.Add;
                        //期号|比赛编号_结果&比赛编号_结果&比赛编号_结果
                        //其中结果应表示为   SPF:3;ZJQ:5;BF:46;
                        var temp = new List<string>();
                        foreach (var item in addResult_sfggList.Where(t => t.IssuseNumber == i.Key))
                        {
                            //todo:注意看 这4个结果中有没有多余的字符,以下为正确的格式
                            //SPF_Result	SXDS_Result	ZJQ_Result	BF_Result	BQC_Result
                            //    3	          XS	         2	       20	      33
                            temp.Add(string.Format("{0}_SF:{1};", item.MatchOrderId, item.SF_Result));
                        }
                        var param = string.Format("{0}|{1}", i.Key, string.Join("#", temp.ToArray()));

                        var paramT = string.Format("{0}|{1}", i.Key, string.Join("_", (from l in addResult_sfggList where l.IssuseNumber == i.Key select l.MatchOrderId).ToArray()));
                        //var param = string.Join("_", (from l in addResultList select l.Id).ToArray());
                        var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                        var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, paramT, sign);

                        //发送 北单比赛结果 添加 通知
                        var innerKey = string.Format("{0}_{1}", "C_BJDC_MatchResult", "Add");
                       // ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.BJDC_MatchResult_SFGG);
                        new Sports_Business(this.mDB).UpdateLocalData(param, "", NoticeType.BJDC_MatchResult_SFGG, innerKey);
                        try
                        {
                            //this.WriteLog("开始生成静态相关数据.");

                            //this.WriteLog("1.生成开奖结果首页");
                            //var log = ServiceHelper.SendBuildStaticFileNotice("301");
                            //this.WriteLog("1.生成开奖结果首页结果：" + log);

                            //this.WriteLog("2.生成彩种开奖历史");
                            //log = ServiceHelper.SendBuildStaticFileNotice("302", "BJDC");
                            //this.WriteLog("2.生成彩种开奖历史结果：" + log);

                            //this.WriteLog("3.生成彩种开奖详细");
                            //log = ServiceHelper.SendBuildStaticFileNotice("303", "BJDC");
                            //this.WriteLog("3.生成彩种开奖详细结果：" + log);
                        }
                        catch (Exception ex)
                        {
                            this.WriteLog("生成静态数据异常：" + ex.Message);
                        }

                        //ServiceHelper.SendNotice(issuse_Request, (log) =>
                        //{
                        //    this.WriteLog(log);
                        //});
                    }
                }
                if (updateResult_sfggList.Count > 0)
                {
                    var g = updateResult_sfggList.GroupBy(t => t.IssuseNumber);
                    foreach (var i in g)
                    {
                        var category = (int)NoticeCategory.BJDC_MatchResult_SFGG;
                        var state = (int)DBChangeState.Update;

                        //期号|比赛编号_结果&比赛编号_结果&比赛编号_结果
                        //其中结果应表示为   SPF:3;ZJQ:5;BF:46;
                        var temp = new List<string>();
                        foreach (var item in updateResult_sfggList.Where(t => t.IssuseNumber == i.Key))
                        {
                            //todo:注意看 这4个结果中有没有多余的字符,以下为正确的格式
                            //SPF_Result	SXDS_Result	ZJQ_Result	BF_Result	BQC_Result
                            //    3	          XS	         2	       20	      33
                            temp.Add(string.Format("{0}_SF:{1};", item.MatchOrderId, item.SF_Result));
                        }
                        var param = string.Format("{0}|{1}", i.Key, string.Join("#", temp.ToArray()));


                        var paramT = string.Format("{0}|{1}", i.Key, string.Join("_", (from l in updateResult_sfggList where l.IssuseNumber == i.Key select l.MatchOrderId).ToArray()));
                        //var param = string.Join("_", (from l in updateResultList select l.Id).ToArray());
                        var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                        var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, paramT, sign);

                        //发送 北单比赛结果 修改 通知
                        var innerKey = string.Format("{0}_{1}", "C_BJDC_MatchResult", "Update");
                        //ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.BJDC_MatchResult_SFGG);
                        new Sports_Business(this.mDB).UpdateLocalData(param, "", NoticeType.BJDC_MatchResult_SFGG, innerKey);
                        try
                        {
                            //this.WriteLog("开始生成静态相关数据.");

                            //this.WriteLog("1.生成开奖结果首页");
                            //var log = ServiceHelper.SendBuildStaticFileNotice("301");
                            //this.WriteLog("1.生成开奖结果首页结果：" + log);

                            //this.WriteLog("2.生成彩种开奖历史");
                            //log = ServiceHelper.SendBuildStaticFileNotice("302", "BJDC");
                            //this.WriteLog("2.生成彩种开奖历史结果：" + log);

                            //this.WriteLog("3.生成彩种开奖详细");
                            //log = ServiceHelper.SendBuildStaticFileNotice("303", "BJDC");
                            //this.WriteLog("3.生成彩种开奖详细结果：" + log);
                        }
                        catch (Exception ex)
                        {
                            this.WriteLog("生成静态数据异常：" + ex.Message);
                        }

                        //ServiceHelper.SendNotice(issuse_Request, (log) =>
                        //{
                        //    this.WriteLog(log);
                        //});
                    }
                }
                #endregion

                #endregion

                this.WriteLog("采集赛事信息完成");
            }
            catch (Exception ex)
            {
                this.WriteLog(ex.ToString());
            }

            #region 采集SP值

            try
            {
                //采集Sp值
                this.WriteLog("开始采集SP数据...");
                if (string.IsNullOrEmpty(issuseNumber))
                    throw new Exception("期号为空");

                var spf_xml = GetBJSingleSpXml("SPF", issuseNumber);
                WriteBJSingle_SP_Json("SPF", issuseNumber, spf_xml);

                var zjq_xml = GetBJSingleSpXml("ZJQ", issuseNumber);
                WriteBJSingle_SP_Json("ZJQ", issuseNumber, zjq_xml);

                var sxds_xml = GetBJSingleSpXml("SXDS", issuseNumber);
                WriteBJSingle_SP_Json("SXDS", issuseNumber, sxds_xml);

                var bf_xml = GetBJSingleSpXml("BF", issuseNumber);
                WriteBJSingle_SP_Json("BF", issuseNumber, bf_xml);

                var bqc_xml = GetBJSingleSpXml("BQC", issuseNumber);
                WriteBJSingle_SP_Json("BQC", issuseNumber, bqc_xml);

                this.WriteLog("采集SP数据完成。");
            }
            catch (Exception ex)
            {
                this.WriteLog(ex.ToString());
            }

            #endregion

            this.WriteLog("采集 期号、比赛信息、SP值、比赛结果 数据完成");
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

        /// <summary>
        /// 采集SP值的xml
        /// </summary>
        private string GetBJSingleSpXml(string gameType, string issuseNumber)
        {
            string url = string.Empty;
            switch (gameType)
            {
                ////胜平负BJSPF
                //case "SPF":
                //    url = string.Format("http://www.500cache.com/static/public/bjdc/xml/sp/just_{0}_34.xml", issuseNumber);
                //    break;
                ////总进球BJZJQ
                //case "ZJQ":
                //    url = string.Format("http://www.500cache.com/static/public/bjdc/xml/sp/just_{0}_40.xml", issuseNumber);
                //    break;
                ////上下单双BJSXDS
                //case "SXDS":
                //    url = string.Format("http://www.500cache.com/static/public/bjdc/xml/sp/just_{0}_41.xml", issuseNumber);
                //    break;
                ////比分BJBF
                //case "BF":
                //    url = string.Format("http://www.500cache.com/static/public/bjdc/xml/sp/just_{0}_42.xml", issuseNumber);
                //    break;
                ////半全场BJBQChttp://zc.trade.500wan.com/static/public/bjdc/xml/sp/just_{0}_51.xml
                //case "BQC":
                //    url = string.Format("http://www.500cache.com/static/public/bjdc/xml/sp/just_{0}_51.xml", issuseNumber);
                //    break;
                //胜平负BJSPF
                case "SPF":
                    url = string.Format("http://trade.500.com/static/public/bjdc/xml/sp/just_{0}_34.xml", issuseNumber);
                    break;
                //总进球BJZJQ
                case "ZJQ":
                    url = string.Format("http://trade.500.com/static/public/bjdc/xml/sp/just_{0}_40.xml", issuseNumber);
                    break;
                //上下单双BJSXDS
                case "SXDS":
                    url = string.Format("http://trade.500.com/static/public/bjdc/xml/sp/just_{0}_41.xml", issuseNumber);
                    break;
                //比分BJBF
                case "BF":
                    url = string.Format("http://trade.500.com/static/public/bjdc/xml/sp/just_{0}_42.xml", issuseNumber);
                    break;
                //半全场BJBQC
                case "BQC":
                    url = string.Format("http://trade.500.com/static/public/bjdc/xml/sp/just_{0}_51.xml", issuseNumber);
                    break;

            }
            if (string.IsNullOrEmpty(url))
                throw new Exception(string.Format("传入gameType:{0}不正确", gameType));
            var xml = PostManager.Get(url, Encoding.GetEncoding("GB2312"), 0, (request) =>
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
                throw new Exception(string.Format("请求地址：{0}返回数据为空", url));
            return xml;
        }

        ///// <summary>
        ///// 创建文件全路径
        ///// </summary>
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
        /// 写入SP值
        /// </summary>
        private void WriteBJSingle_SP_Json(string gameType, string issuseNumber, string xmlContent)
        {
            var tableName = string.Format("SP_{0}", gameType);
          //  var fileFullName = BuildFileFullName(fileName, issuseNumber);

            var doc = new XmlDocument();
            doc.LoadXml(xmlContent);
            var root = doc.SelectSingleNode("w");
            if (root == null)
                throw new Exception(string.Format("{0}第{1}期,从xml中查询节点错误  - {2}", gameType, issuseNumber, xmlContent));


           
            //   ServiceHelper.BuildList_GN(mDB, "BJDC_ZJQ_SpInfo", collection, null, mFilter);


            var json = string.Empty;
          //FilterDefinition<T> mFilter = null;
            switch (gameType)
            {
                //胜平负SPF
                case "SPF":
                    var spf_sp = LoadSPF_SPList(root, issuseNumber);
                    //GetNew_ZJQ_SPList
                   //var spf_DifferSp = Save_SPF_SP_And_GetDiffer(fileFullName, spf_sp);
                    var mFilter = MongoDB.Driver.Builders<BJDC_SPF_SpInfo>.Filter.Eq(b => b.IssuseNumber, issuseNumber)
               & Builders<BJDC_SPF_SpInfo>.Filter.Eq(b => b.GameType, gameType);
                    var spf_DifferSp = ServiceHelper.Save_And_GetDiffer<BJDC_SPF_SpInfo>(mDB, tableName, spf_sp, mFilter, GetNew_SPF_SPList);
                 //   Write_SPF_SP_Trend_JSON(spf_DifferSp);
                    ServiceHelper.Write_Trend_JSON<BJDC_SPF_SpInfo>(mDB, tableName, spf_DifferSp, CurrentMatchList);

                    //  ServiceHelper.BuildList_BJDC(mDB, "BJDC_ZJQ_SpInfo", collection, null, mFilter);
                    //  Write_SPF_SP_Trend_JSON(spf_DifferSp);
                    break;
                //总进球ZJQ
                case "ZJQ":
                    var zjq_sp = LoadZJQ_SPList(root, issuseNumber);
               //var zjq_DifferSp = Save_ZJQ_SP_And_GetDiffer(fileFullName, zjq_sp);
                    //Write_ZJQ_SP_Trend_JSON(zjq_DifferSp);
                    var mFilter_ZJQ = MongoDB.Driver.Builders<BJDC_ZJQ_SpInfo>.Filter.Eq(b => b.IssuseNumber, issuseNumber)
           & Builders<BJDC_ZJQ_SpInfo>.Filter.Eq(b => b.GameType, gameType);
                    var spf_DifferSp_ZJQ = ServiceHelper.Save_And_GetDiffer<BJDC_ZJQ_SpInfo>(mDB, tableName, zjq_sp, mFilter_ZJQ, GetNew_ZJQ_SPList);
                    //   Write_SPF_SP_Trend_JSON(spf_DifferSp);
                    ServiceHelper.Write_Trend_JSON<BJDC_ZJQ_SpInfo>(mDB, tableName, spf_DifferSp_ZJQ, CurrentMatchList);

                    break;
                //上下单双SXDS
                case "SXDS":
                    var sxds_sp = LoadSXDS_SPList(root, issuseNumber);
                    //var sxds_DifferSp = Save_SXDS_SP_And_GetDiffer(fileFullName, sxds_sp);
                    //Write_SXDS_SP_Trend_JSON(sxds_DifferSp);

                    var mFilter_SXDS = MongoDB.Driver.Builders<BJDC_SXDS_SpInfo>.Filter.Eq(b => b.IssuseNumber, issuseNumber)
    & Builders<BJDC_SXDS_SpInfo>.Filter.Eq(b => b.GameType, gameType);
                    var spf_DifferSp_SXDS = ServiceHelper.Save_And_GetDiffer<BJDC_SXDS_SpInfo>(mDB, tableName, sxds_sp, mFilter_SXDS, GetNew_SXDS_SPList);
                    //   Write_SPF_SP_Trend_JSON(spf_DifferSp);
                    ServiceHelper.Write_Trend_JSON<BJDC_SXDS_SpInfo>(mDB, tableName, spf_DifferSp_SXDS, CurrentMatchList);

                    break;
                //比分BF
                case "BF":
                    var bf_sp = LoadBF_SPList(root, issuseNumber);
                    //var bf_DifferSp = Save_BF_SP_And_GetDiffer(fileFullName, bf_sp);
                    //Write_BF_SP_Trend_JSON(bf_DifferSp);


                    var mFilter_BF = MongoDB.Driver.Builders<BJDC_BF_SpInfo>.Filter.Eq(b => b.IssuseNumber, issuseNumber)
    & Builders<BJDC_BF_SpInfo>.Filter.Eq(b => b.GameType, gameType);
                    var spf_DifferSp_BF = ServiceHelper.Save_And_GetDiffer<BJDC_BF_SpInfo>(mDB, tableName, bf_sp, mFilter_BF, GetNew_BF_SPList);
                    //   Write_SPF_SP_Trend_JSON(spf_DifferSp);
                    ServiceHelper.Write_Trend_JSON<BJDC_BF_SpInfo>(mDB, tableName, spf_DifferSp_BF, CurrentMatchList);

                    break;
                //半全场BJBQC
                case "BQC":
                    var bqc_sp = LoadBQC_SPList(root, issuseNumber);
                   // var bqc_DifferSp = Save_BQC_SP_And_GetDiffer(fileFullName, bqc_sp);
                    //Write_BQC_SP_Trend_JSON(bqc_DifferSp);




                    var mFilter_BQC = MongoDB.Driver.Builders<BJDC_BQC_SpInfo>.Filter.Eq(b => b.IssuseNumber, issuseNumber)
    & Builders<BJDC_BQC_SpInfo>.Filter.Eq(b => b.GameType, gameType);
                    var spf_DifferSp_BQC = ServiceHelper.Save_And_GetDiffer<BJDC_BQC_SpInfo>(mDB, tableName, bqc_sp, mFilter_BQC, GetNew_BQC_SPList);
                    //   Write_SPF_SP_Trend_JSON(spf_DifferSp);
                    ServiceHelper.Write_Trend_JSON<BJDC_BQC_SpInfo>(mDB, tableName, spf_DifferSp_BQC, CurrentMatchList);

                    break;
            }

            var customerSavePath = new string[] { "BJDC", issuseNumber };
            //上传文件
            //ServiceHelper.PostFileToServer(fileFullName, customerSavePath, (log) =>
            //{
            //    this.WriteLog(log);
            //});
        }

        #region 解析联赛队伍信息
        //private List<BJDC_Team_History> GetHistory(string id)
        //{
        //    if (string.IsNullOrEmpty(id)) return new List<BJDC_Team_History>();
        //    var baseTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));//.AddSeconds(1366558200)
        //    var encoding = Encoding.UTF8;
        //    var url = string.Format("http://www.9188.com/data/static/odds/historymatch/{0}.xml", id);
        //    var hxml = PostManager.Get(url, encoding, 0, (request) =>
        //    {
        //        if (ServiceHelper.IsUseProxy("BJDC"))
        //        {
        //            var proxy = ServiceHelper.GetProxyUrl();
        //            if (!string.IsNullOrEmpty(proxy))
        //            {
        //                request.Proxy = new System.Net.WebProxy(proxy);
        //            }
        //        }
        //    });
        //    var historyList = new List<BJDC_Team_History>();
        //    if (!string.IsNullOrEmpty(hxml) && hxml != "404")
        //    {
        //        var doc = new XmlDocument();
        //        doc.LoadXml(hxml);
        //        var root = doc.SelectSingleNode("xml");
        //        if (root != null)
        //        {
        //            foreach (XmlNode t in root.ChildNodes)
        //            {
        //                //<r ln="亚洲预" hteam="澳大利亚" ateam="阿曼" mtime="1364286600" hscore="2" ascore="2" bc="0:1" bet="1.75" binfo="输" htid="632" atid="933" cl="393465"></r>
        //                historyList.Add(new BJDC_Team_History
        //                {
        //                    Ln = t.Attributes["ln"].Value,
        //                    HTeam = t.Attributes["hteam"].Value,
        //                    ATeam = t.Attributes["ateam"].Value,
        //                    MTime = baseTime.AddSeconds(double.Parse(t.Attributes["mtime"].Value)).ToString("yyyy-MM-dd HH:mm:ss"),
        //                    HScore = t.Attributes["hscore"].Value.GetInt32(),
        //                    AScore = t.Attributes["ascore"].Value.GetInt32(),
        //                    Bc = t.Attributes["bc"].Value,
        //                    Bet = t.Attributes["bet"].Value,
        //                    BInfo = t.Attributes["binfo"].Value,
        //                    HTId = t.Attributes["htid"].Value,
        //                    ATId = t.Attributes["atid"].Value,
        //                    Cl = t.Attributes["cl"].Value,
        //                });
        //            }
        //        }
        //    }
        //    return historyList;
        //}

        private List<KeyValuePair<DBChangeState, C_BJDC_Match>> BuildLeagueInfoCollectionFrom9188(string currentIssuseNumber, bool getResult,
            out List<KeyValuePair<DBChangeState, C_BJDC_Issuse>> issuseList,
            out List<KeyValuePair<DBChangeState, C_BJDC_MatchResult>> leagueResultList,
            out List<KeyValuePair<DBChangeState, BJDC_Match_SFGG>> result_SFGG,
            out List<KeyValuePair<DBChangeState, BJDC_Match_SFGGResult>> leagueResult_sfggList)
        {
            var result = new List<KeyValuePair<DBChangeState, C_BJDC_Match>>();
            result_SFGG = new List<KeyValuePair<DBChangeState, BJDC_Match_SFGG>>();
            leagueResultList = new List<KeyValuePair<DBChangeState, C_BJDC_MatchResult>>();
            issuseList = new List<KeyValuePair<DBChangeState, C_BJDC_Issuse>>();
            leagueResult_sfggList = new List<KeyValuePair<DBChangeState, BJDC_Match_SFGGResult>>();

            //解析当前赛事信息
            var currentLeagueInfoList = new List<C_BJDC_Match>();
            var matchSource = ServiceHelper.GetSystemConfig("BJDC_Match_Source");
            this.WriteLog(string.Format("采集比赛源：{0}", matchSource));
            if (matchSource == "9188")
            {
                currentLeagueInfoList = GetBJDCMatchList(currentIssuseNumber);
            }
            if (matchSource == "500wan")
            {
                currentLeagueInfoList = GetBJDCMatchList_500wan(currentIssuseNumber);
            }
            if (matchSource == "GuanWang")
            {
                currentLeagueInfoList = GetBJDCMatchList_GuanWang(currentIssuseNumber);
            }

            //解析当前赛事信息
            //var currentLeagueInfoList = GetBJDCMatchList(currentIssuseNumber);
            //var currentLeagueInfoList = GetBJDCMatchList_CPDJY(currentIssuseNumber);
            //var match_SFGGList = GetBJDC_SFGGMatchList_CPDJY(currentIssuseNumber);
            var match_SFGGList = new List<BJDC_Match_SFGG>();
            //var match_SFGGList = GetBJDC_SFGGMatchList_9188(currentIssuseNumber);
            //if (currentLeagueInfoList.Count == 0)
            //    return result;
            //全局赛事信息
            CurrentMatchList = currentLeagueInfoList;
            var customerSavePath = new string[] { "BJDC" };


            #region 保存历史对阵数据

            //foreach (var item in currentLeagueInfoList)
            //{
            //    try
            //    {
            //        if (string.IsNullOrEmpty(Sp_SavePath))
            //            Sp_SavePath = ServiceHelper.Get_BJDC_SPSavePath();
            //        var path = Path.Combine(Sp_SavePath, item.IssuseNumber);
            //        if (!Directory.Exists(path))
            //            Directory.CreateDirectory(path);
            //        customerSavePath = new string[] { "BJDC", item.IssuseNumber };
            //        var hFullFileName = Path.Combine(path, string.Format("TeamHistory_{0}.json", item.Hi));
            //        this.WriteLog(string.Format("准备采集对阵历史{0}", item.Hi));
            //        if (!File.Exists(hFullFileName))
            //        {
            //            var hList = GetHistory(item.Hi);
            //            ServiceHelper.CreateOrAppend_JSONFile(hFullFileName, JsonSerializer.Serialize(hList), (log) =>
            //            {
            //                this.WriteLog(log);
            //            });
            //            //上传文件
            //            ServiceHelper.PostFileToServer(hFullFileName, customerSavePath, (log) =>
            //            {
            //                this.WriteLog(log);
            //            });
            //        }
            //        var gFullFileName = Path.Combine(path, string.Format("TeamHistory_{0}.json", item.Gi));
            //        this.WriteLog(string.Format("准备采集对阵历史{0}", item.Gi));
            //        if (!File.Exists(gFullFileName))
            //        {
            //            var gList = GetHistory(item.Gi);
            //            ServiceHelper.CreateOrAppend_JSONFile(gFullFileName, JsonSerializer.Serialize(gList), (log) =>
            //            {
            //                this.WriteLog(log);
            //            });
            //            //上传文件
            //            ServiceHelper.PostFileToServer(gFullFileName, customerSavePath, (log) =>
            //            {
            //                this.WriteLog(log);
            //            });
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        this.WriteLog(string.Format("写入 队伍历史 数据文件   失败：{0}", ex.ToString()));
            //    }
            //}

            #endregion

            var currentLeagueResult_sfggList = new List<BJDC_Match_SFGGResult>();
            var currentLeagueResultList = new List<C_BJDC_MatchResult>();
            if (getResult)
            {
                var currentLeagueResultList_500wan = new List<C_BJDC_MatchResult>();
                var currentLeagueResultList_aicai = new List<C_BJDC_MatchResult>();
                var currentLeagueResultList_9188 = new List<C_BJDC_MatchResult>();
                var currentLeagueResultList_okooo = new List<C_BJDC_MatchResult>();
                var result310win = GetBJDC_MatchResultFrom310Win(currentIssuseNumber);
                var resultSource = ServiceHelper.GetSystemConfig("BJDC_Result_Source");
                this.WriteLog(string.Format("采集结果源：{0}", resultSource));
                if (string.IsNullOrEmpty(resultSource))
                    currentLeagueResultList = result310win;
                //if (resultSource == "9188")
                //{
                //    currentLeagueResultList_9188 = GetBJDCMatchResultList(currentIssuseNumber);
                //}
                //if (resultSource == "310win")
                //{
                //    currentLeagueResultList = GetBJDC_MatchResultFrom310Win(currentIssuseNumber);
                //}
                //if (resultSource == "aicai")
                //{
                //    currentLeagueResultList_aicai = GetBJDC_MatchResultFromAiCai(currentIssuseNumber);
                //}
                if (resultSource == "500wan")
                {
                    currentLeagueResultList_500wan = GetBJDC_MatchResultFrom500wan(currentIssuseNumber);
                    foreach (var item in currentLeagueResultList_500wan)
                    {
                        var result_310 = result310win.Where(p => p.MatchOrderId == item.MatchOrderId).FirstOrDefault();
                        if (result_310 == null) continue;
                        if (item.SPF_Result != result_310.SPF_Result || item.SXDS_Result != result_310.SXDS_Result || item.BF_Result != result_310.BF_Result || item.ZJQ_Result != result_310.ZJQ_Result || item.BQC_Result != result_310.BQC_Result)
                            continue;
                        currentLeagueResultList.Add(result_310);
                    }
                }
                if (resultSource == "okooo")
                {
                    currentLeagueResultList_okooo = GetBJDC_MatchResultFromokooo(currentIssuseNumber);
                    foreach (var item in currentLeagueResultList_okooo)
                    {
                        var result_310 = result310win.Where(p => p.MatchOrderId == item.MatchOrderId).FirstOrDefault();
                        if (result_310 == null) continue;
                        if (item.SPF_Result != result_310.SPF_Result || item.SXDS_Result != result_310.SXDS_Result || item.BF_Result != result_310.BF_Result || item.ZJQ_Result != result_310.ZJQ_Result || item.BQC_Result != result_310.BQC_Result)
                            continue;
                        currentLeagueResultList.Add(result_310);
                    }
                }

                this.WriteLog(string.Format("采集期号 {0}  来源  {1}  结果记录{2}", currentIssuseNumber, resultSource, currentLeagueResultList.Count));

                //待定
                currentLeagueResult_sfggList = GetBJDC_Match_SFGGResultFrom310Win(currentIssuseNumber);
                this.WriteLog(string.Format("采集胜负过关期号 {0}  来源  {1}  结果记录{2}", currentIssuseNumber, "310win", currentLeagueResultList.Count));
            }

            var leagueListFileFullName = "BJDC_Match_List";// BuildFileFullName("BJDC_Match_List.json", currentIssuseNumber);
            var sfgg_ListFileFullName = "BJDC_Match_SFGG_List";// BuildFileFullName("BJDC_Match_SFGG_List.json", currentIssuseNumber);
            var leagueResultListFileFullName = "BJDC_MatchResult_List";// BuildFileFullName("BJDC_MatchResult_List.json", currentIssuseNumber);
            var issuseFileFullName = "BJDC_Match_IssuseNumber_List";//  BuildFileFullName("BJDC_Match_IssuseNumber_List.json", string.Empty);
            var first = currentLeagueInfoList.OrderByDescending(l => l.MatchStartTime).FirstOrDefault();
            var leagueResultListFileFullName_sfgg = "BJDC_MatchResult_SFGG_List";//  BuildFileFullName("BJDC_MatchResult_SFGG_List.json", currentIssuseNumber);

            #region 保存奖期数据

            customerSavePath = new string[] { "BJDC" };
          var coll=  mDB.GetCollection<C_BJDC_Issuse>("Match_IssuseNumber_List");
            var mFilter = MongoDB.Driver.Builders<C_BJDC_Issuse>.Filter.Eq(b => b.IssuseNumber, currentIssuseNumber);//& Builders<C_BJDC_Issuse>.Filter.Eq(b => b.IssuseNumber, issuseNumber);
            var old1= coll.Find<C_BJDC_Issuse>(mFilter).FirstOrDefault();
            var nIssuse = new C_BJDC_Issuse
            {
                IssuseNumber = currentIssuseNumber,
                MinMatchStartTime = first == null ? DateTime.Now : first.MatchStartTime,
                MinLocalStopTime = first == null ? DateTime.Now : first.LocalStopTime,
            };
            if (old1 == null)
            {
                issuseList.Add(new KeyValuePair<DBChangeState, C_BJDC_Issuse>(DBChangeState.Add, nIssuse));
            }
            else {
                coll.DeleteMany(mFilter);
                coll.InsertOne(nIssuse);
                if (first != null)
                {
                    if (first.IssuseNumber == old1.IssuseNumber && first.LocalStopTime != old1.MinLocalStopTime)
                    {
                        issuseList.Add(new KeyValuePair<DBChangeState, C_BJDC_Issuse>(DBChangeState.Update, new C_BJDC_Issuse
                        {
                            IssuseNumber = currentIssuseNumber,
                            MinMatchStartTime = first == null ? DateTime.Now : first.MatchStartTime,
                            MinLocalStopTime = first == null ? DateTime.Now : first.LocalStopTime,
                        }));
                    }
                }
            }

            //if (File.Exists(issuseFileFullName))
            //{
            //    var text = File.ReadAllText(issuseFileFullName).Trim().Replace("var data=", "").Replace("];", "]");
            //    var oldList = string.IsNullOrEmpty(text) ? new List<C_BJDC_Issuse>() : JsonHelper.Deserialize<List<C_BJDC_Issuse>>(text);
            //    var old = oldList.FirstOrDefault(f => f.IssuseNumber == currentIssuseNumber);
            //    if (old == null)
            //    {
            //        issuseList.Add(new KeyValuePair<DBChangeState, C_BJDC_Issuse>(DBChangeState.Add, new C_BJDC_Issuse
            //        {
            //            IssuseNumber = currentIssuseNumber,
            //            MinMatchStartTime = first == null ? DateTime.Now : first.MatchStartTime,
            //            MinLocalStopTime = first == null ? DateTime.Now: first.LocalStopTime,
            //        }));
            //        oldList.Add(new C_BJDC_Issuse
            //        {
            //            IssuseNumber = currentIssuseNumber,
            //            MinMatchStartTime = first == null ? DateTime.Now: first.MatchStartTime,
            //            MinLocalStopTime = first == null ? DateTime.Now : first.LocalStopTime,
            //        });
            //        if (oldList.Count > 5)
            //        {
            //            oldList.Remove(oldList.OrderBy(i => i.IssuseNumber).First());
            //        }
            //    }
            //    else
            //    {
            //        oldList.Remove(old);
            //        oldList.Add(new C_BJDC_Issuse
            //        {
            //            IssuseNumber = currentIssuseNumber,
            //            MinMatchStartTime = first == null ? DateTime.Now : first.MatchStartTime,
            //            MinLocalStopTime = first == null ? DateTime.Now: first.LocalStopTime,
            //        });
            //        if (first != null)
            //        {
            //            if (first.IssuseNumber == old.IssuseNumber && first.LocalStopTime != old.MinLocalStopTime)
            //            {
            //                issuseList.Add(new KeyValuePair<DBChangeState, C_BJDC_Issuse>(DBChangeState.Update, new C_BJDC_Issuse
            //                {
            //                    IssuseNumber = currentIssuseNumber,
            //                    MinMatchStartTime = first == null ? DateTime.Now: first.MatchStartTime,
            //                    MinLocalStopTime = first == null ? DateTime.Now : first.LocalStopTime,
            //                }));
            //            }
            //        }
            //    }

            //    ServiceHelper.CreateOrAppend_JSONFile(issuseFileFullName, JsonHelper.Serialize(oldList), (log) =>
            //    {
            //        this.WriteLog(log);
            //    });

            //    //上传文件
            //    ServiceHelper.PostFileToServer(issuseFileFullName, customerSavePath, (log) =>
            //    {
            //        this.WriteLog(log);
            //    });
            //}
            //else
            //{
            //    try
            //    {
            //        issuseList.Add(new KeyValuePair<DBChangeState, C_BJDC_Issuse>(DBChangeState.Add, new C_BJDC_Issuse
            //        {
            //            IssuseNumber = currentIssuseNumber,
            //            MinMatchStartTime = first == null ? DateTime.Now : first.MatchStartTime,
            //            MinLocalStopTime = first == null ? DateTime.Now: first.LocalStopTime,
            //        }));
            //        var t = new List<C_BJDC_Issuse>();
            //        t.Add(new C_BJDC_Issuse
            //        {
            //            IssuseNumber = currentIssuseNumber,
            //            MinMatchStartTime = first == null ? DateTime.Now : first.MatchStartTime,
            //            MinLocalStopTime = first == null ? DateTime.Now : first.LocalStopTime,
            //        });

            //        ServiceHelper.CreateOrAppend_JSONFile(issuseFileFullName, JsonHelper.Serialize(t), (log) =>
            //        {
            //            this.WriteLog(log);
            //        });

            //        //上传文件
            //        ServiceHelper.PostFileToServer(issuseFileFullName, customerSavePath, (log) =>
            //        {
            //            this.WriteLog(log);
            //        });
            //    }
            //    catch (Exception ex)
            //    {
            //        this.WriteLog(string.Format("第一次写入 {0}  文件失败：{1}", issuseFileFullName, ex.ToString()));
            //    }
            //}

            #endregion

            #region 保存比赛信息

            customerSavePath = new string[] { "BJDC", currentIssuseNumber };
            if (currentLeagueInfoList.Count != 0)
            {
                var coll_BJDC_Match = mDB.GetCollection<C_BJDC_Match>(leagueListFileFullName);
               var mlist= coll_BJDC_Match.Find<C_BJDC_Match>(null).ToList();
                if (mlist.Count > 0)
                {

                    foreach (var item in GetNewLeagueInfoList(mlist, currentLeagueInfoList))
                    {
                        result.Add(new KeyValuePair<DBChangeState, C_BJDC_Match>(DBChangeState.Update, item));
                    }
                    coll_BJDC_Match.DeleteMany(null);
                }
                else {
                    foreach (var item in GetNewLeagueInfoList(new List<C_BJDC_Match>(), currentLeagueInfoList))
                    {
                        result.Add(new KeyValuePair<DBChangeState, C_BJDC_Match>(DBChangeState.Add, item));
                    }
                }
                coll_BJDC_Match.InsertMany(currentLeagueInfoList);

                //if (F1))
                //{
                //    var textM = File.ReadAllText(leagueListFileFullName).Trim().Replace("var data=", "").Replace("];", "]");
                //    var oldList = string.IsNullOrEmpty(textM) ? new List<C_BJDC_Match>() : JsonHelper.Deserialize<List<C_BJDC_Match>>(textM);
                //    ServiceHelper.CreateOrAppend_JSONFile(leagueListFileFullName, JsonHelper.Serialize(currentLeagueInfoList), (log) =>
                //    {
                //        this.WriteLog(log);
                //    });

                //    foreach (var item in GetNewLeagueInfoList(oldList, currentLeagueInfoList))
                //    {
                //        result.Add(new KeyValuePair<DBChangeState, C_BJDC_Match>(DBChangeState.Update, item));
                //    }

                //    //上传文件
                //    ServiceHelper.PostFileToServer(leagueListFileFullName, customerSavePath, (log) =>
                //    {
                //        this.WriteLog(log);
                //    });
                //}
                //else
                //{
                //    ServiceHelper.CreateOrAppend_JSONFile(leagueListFileFullName, JsonHelper.Serialize(currentLeagueInfoList), (log) =>
                //    {
                //        this.WriteLog(log);
                //    });
                //    foreach (var item in GetNewLeagueInfoList(new List<C_BJDC_Match>(), currentLeagueInfoList))
                //    {
                //        result.Add(new KeyValuePair<DBChangeState, C_BJDC_Match>(DBChangeState.Add, item));
                //    }

                //    //上传文件
                //    ServiceHelper.PostFileToServer(leagueListFileFullName, customerSavePath, (log) =>
                //    {
                //        this.WriteLog(log);
                //    });
                //}
            }

            #endregion

            #region 保存胜负过关比赛信息

            customerSavePath = new string[] { "BJDC", currentIssuseNumber };
            if (match_SFGGList.Count != 0)
            {
                var coll_BJDC_Match = mDB.GetCollection<BJDC_Match_SFGG>(sfgg_ListFileFullName);
                var mlist = coll_BJDC_Match.Find<BJDC_Match_SFGG>(null).ToList();
                if (mlist.Count > 0)
                {

                    foreach (var item in GetNewLeague_SFGGInfoList(mlist, match_SFGGList))
                    {
                        result_SFGG.Add(new KeyValuePair<DBChangeState, BJDC_Match_SFGG>(DBChangeState.Update, item));
                    }
                    coll_BJDC_Match.DeleteMany(null);
                }
                else
                {
                    foreach (var item in GetNewLeague_SFGGInfoList(new List<BJDC_Match_SFGG>(), match_SFGGList))
                    {
                        result_SFGG.Add(new KeyValuePair<DBChangeState, BJDC_Match_SFGG>(DBChangeState.Add, item));
                    }
                }
                coll_BJDC_Match.InsertMany(match_SFGGList);

                //if (File.Exists(sfgg_ListFileFullName))
                //{
                //    var textM = File.ReadAllText(sfgg_ListFileFullName).Trim().Replace("var data=", "").Replace("];", "]");
                //    var oldList = string.IsNullOrEmpty(textM) ? new List<BJDC_Match_SFGG>() : JsonHelper.Deserialize<List<BJDC_Match_SFGG>>(textM);
                //    ServiceHelper.CreateOrAppend_JSONFile(sfgg_ListFileFullName, JsonHelper.Serialize(match_SFGGList), (log) =>
                //    {
                //        this.WriteLog(log);
                //    });

                //    foreach (var item in GetNewLeague_SFGGInfoList(oldList, match_SFGGList))
                //    {
                //        result_SFGG.Add(new KeyValuePair<DBChangeState, BJDC_Match_SFGG>(DBChangeState.Update, item));
                //    }

                //    //上传文件
                //    ServiceHelper.PostFileToServer(sfgg_ListFileFullName, customerSavePath, (log) =>
                //    {
                //        this.WriteLog(log);
                //    });
                //}
                //else
                //{
                //    ServiceHelper.CreateOrAppend_JSONFile(sfgg_ListFileFullName, JsonHelper.Serialize(match_SFGGList), (log) =>
                //    {
                //        this.WriteLog(log);
                //    });
                //    foreach (var item in GetNewLeague_SFGGInfoList(new List<BJDC_Match_SFGG>(), match_SFGGList))
                //    {
                //        result_SFGG.Add(new KeyValuePair<DBChangeState, BJDC_Match_SFGG>(DBChangeState.Add, item));
                //    }

                //    //上传文件
                //    ServiceHelper.JsonHelper(sfgg_ListFileFullName, customerSavePath, (log) =>
                //    {
                //        this.WriteLog(log);
                //    });
                //}
            }

            #endregion

            #region 保存比赛结果

            customerSavePath = new string[] { "BJDC", currentIssuseNumber };
            if (currentLeagueResultList.Count != 0)
            {
                var coll_BJDC_Match = mDB.GetCollection<C_BJDC_MatchResult>(leagueResultListFileFullName);
                var mlist = coll_BJDC_Match.Find<C_BJDC_MatchResult>(null).ToList();
                if (mlist.Count > 0)
                {

                    foreach (var item in GetNewLeagueResultList(mlist, currentLeagueResultList))
                    {
                        leagueResultList.Add(new KeyValuePair<DBChangeState, C_BJDC_MatchResult>(DBChangeState.Update, item));
                    }
                    coll_BJDC_Match.DeleteMany(null);
                }
                else
                {
                    foreach (var item in GetNewLeagueResultList(new List<C_BJDC_MatchResult>(), currentLeagueResultList))
                    {
                        leagueResultList.Add(new KeyValuePair<DBChangeState, C_BJDC_MatchResult>(DBChangeState.Add, item));
                    }
                }

                coll_BJDC_Match.InsertMany(currentLeagueResultList);
                //this.WriteLog(leagueResultListFileFullName);
                ////处理比赛结果
                //if (File.Exists(leagueResultListFileFullName))
                //{
                //    this.WriteLog("文件已存在");
                //    //var text = File.ReadAllText(leagueResultListFileFullName).Trim();
                //    //var oldResultList = string.IsNullOrEmpty(text) ? new List<C_BJDC_MatchResult>() : JsonSerializer.Deserialize<List<C_BJDC_MatchResult>>(text);
                //    ServiceHelper.CreateOrAppend_JSONFile(leagueResultListFileFullName, JsonHelper.Serialize(currentLeagueResultList), (log) =>
                //    {
                //        this.WriteLog(log);
                //    });

                //    //缓存暂不在此处理
                //    //foreach (var item in GetNewLeagueResultList(oldResultList, currentLeagueResultList))
                //    foreach (var item in GetNewLeagueResultList(new List<C_BJDC_MatchResult>(), currentLeagueResultList))
                //    {
                //        leagueResultList.Add(new KeyValuePair<DBChangeState, C_BJDC_MatchResult>(DBChangeState.Update, item));
                //    }

                //    //上传文件
                //    ServiceHelper.PostFileToServer(leagueResultListFileFullName, customerSavePath, (log) =>
                //    {
                //        this.WriteLog(log);
                //    });
                //}
                //else
                //{
                //    this.WriteLog("文件不存在，添加新文件");
                //    ServiceHelper.CreateOrAppend_JSONFile(leagueResultListFileFullName, JsonHelper.Serialize(currentLeagueResultList), (log) =>
                //    {
                //        this.WriteLog(log);
                //    });

                //    foreach (var item in GetNewLeagueResultList(new List<C_BJDC_MatchResult>(), currentLeagueResultList))
                //    {
                //        leagueResultList.Add(new KeyValuePair<DBChangeState, C_BJDC_MatchResult>(DBChangeState.Add, item));
                //    }

                //    //上传文件
                //    ServiceHelper.PostFileToServer(leagueResultListFileFullName, customerSavePath, (log) =>
                //    {
                //        this.WriteLog(log);
                //    });
                //}
            }

            #endregion

            #region 保存胜负过关比赛结果

            customerSavePath = new string[] { "BJDC", currentIssuseNumber };
            if (currentLeagueResult_sfggList.Count != 0)
            {

                var coll_BJDC_Match = mDB.GetCollection<BJDC_Match_SFGGResult>(leagueResultListFileFullName);
                var mlist = coll_BJDC_Match.Find<BJDC_Match_SFGGResult>(null).ToList();
                if (mlist.Count > 0)
                {

                    foreach (var item in GetNewLeagueResult_sfggList(mlist, currentLeagueResult_sfggList))
                    {
                        leagueResult_sfggList.Add(new KeyValuePair<DBChangeState, BJDC_Match_SFGGResult>(DBChangeState.Update, item));
                    }
                    coll_BJDC_Match.DeleteMany(null);
                }
                else
                {
                    foreach (var item in GetNewLeagueResult_sfggList(new List<BJDC_Match_SFGGResult>(), currentLeagueResult_sfggList))
                    {
                        leagueResult_sfggList.Add(new KeyValuePair<DBChangeState, BJDC_Match_SFGGResult>(DBChangeState.Add, item));
                    }
                }

                coll_BJDC_Match.InsertMany(currentLeagueResult_sfggList);
                //this.WriteLog(leagueResultListFileFullName_sfgg);
                ////处理比赛结果
                //if (File.Exists(leagueResultListFileFullName_sfgg))
                //{
                //    this.WriteLog("文件已存在");
                //    //var text = File.ReadAllText(leagueResultListFileFullName).Trim();
                //    //var oldResultList = string.IsNullOrEmpty(text) ? new List<C_BJDC_MatchResult>() : JsonSerializer.Deserialize<List<C_BJDC_MatchResult>>(text);
                //    ServiceHelper.CreateOrAppend_JSONFile(leagueResultListFileFullName_sfgg, JsonHelper.Serialize(currentLeagueResult_sfggList), (log) =>
                //    {
                //        this.WriteLog(log);
                //    });

                //    //缓存暂不在此处理
                //    //foreach (var item in GetNewLeagueResultList(oldResultList, currentLeagueResultList))
                //    foreach (var item in GetNewLeagueResult_sfggList(new List<BJDC_Match_SFGGResult>(), currentLeagueResult_sfggList))
                //    {
                //        leagueResult_sfggList.Add(new KeyValuePair<DBChangeState, BJDC_Match_SFGGResult>(DBChangeState.Update, item));
                //    }

                //    //上传文件
                //    ServiceHelper.PostFileToServer(leagueResultListFileFullName_sfgg, customerSavePath, (log) =>
                //    {
                //        this.WriteLog(log);
                //    });
                //}
                //else
                //{
                //    this.WriteLog("文件不存在，添加新文件");
                //    ServiceHelper.CreateOrAppend_JSONFile(leagueResultListFileFullName_sfgg, JsonHelper.Serialize(currentLeagueResult_sfggList), (log) =>
                //    {
                //        this.WriteLog(log);
                //    });

                //    foreach (var item in GetNewLeagueResult_sfggList(new List<BJDC_Match_SFGGResult>(), currentLeagueResult_sfggList))
                //    {
                //        leagueResult_sfggList.Add(new KeyValuePair<DBChangeState, BJDC_Match_SFGGResult>(DBChangeState.Add, item));
                //    }

                //    //上传文件
                //    ServiceHelper.PostFileToServer(leagueResultListFileFullName_sfgg, customerSavePath, (log) =>
                //    {
                //        this.WriteLog(log);
                //    });
                //}
            }

            #endregion

            return result;
        }

        public List<C_BJDC_MatchResult> GetBJDC_MatchResultFromokooo(string issuseNumber)
        {
            //http://www.okooo.com/danchang/kaijiang/?LotteryNo=160502
            var list = new List<C_BJDC_MatchResult>();
            if (string.IsNullOrEmpty(issuseNumber))
                return list;
            try
            {
                var url = string.Format("http://www.okooo.com/danchang/kaijiang/?LotteryNo={0}", issuseNumber);
                this.WriteLog(string.Format("开始从地址：{0}采集数据", url));
                var html = PostManager.Get(url, Encoding.Default, 0, (request) =>
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
                if (html == "404" || string.IsNullOrEmpty(html))
                    return list;
                var tempArray = new string[] { };
                var indexStart = html.IndexOf("<tr align=\"center\" class=\"WhiteBg BlackWords trClass\">");
                html = html.Substring(indexStart);
                var indexEnd = html.IndexOf("<td colspan=\"18\" align=\"left\" class=\"noborder redtxt\" style=\"border-top:1px solid #8BC1E7;\">");
                html = html.Substring(0, indexEnd);
                string pattern = "<td class=\"noborder\">(?<content>.*?)</tr>.*?<tr.*?>";
                System.Text.RegularExpressions.MatchCollection matchs = Regex.Matches(html, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                foreach (System.Text.RegularExpressions.Match item in matchs)
                {
                    var content = item.Groups["content"].Value.Replace("\r\n", "");
                    var rows = content.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                    int matchOrderId = 0;
                    int.TryParse(rows[0], out matchOrderId);
                    string matchState = "";////"Finish" 完场，"Late" 延迟，"Cancel" 取消
                    var halfResult = rows[6].Trim().Replace(" ", "").Replace("-->", "").Replace("<td>", "").Split('-');
                    string homeHalf_Result = halfResult.Length == 2 ? halfResult[0] : "";
                    string guestHalf_Result = halfResult.Length == 2 ? halfResult[1] : "";
                    var fullResult = rows[7].Trim().Replace("<td class=\"border2\">", "").Split('-');
                    string homeFull_Result = fullResult.Length == 2 ? fullResult[0] : "";
                    string guestFull_Result = fullResult.Length == 2 ? fullResult[1] : "";
                    string spf_result = rows[9].Trim().Replace("<td><b class=\"font_red\">", "").Replace("</b>", "");
                    decimal spf_sp = 1M;
                    string spf_sp1 = rows[10].Trim().Replace("<td class=\"border2\">", "").Replace("</b>", "");
                    decimal.TryParse(spf_sp1, out spf_sp);
                    string bf_result = rows[11].Trim().Replace("<td><b class=\"font_red\">", "").Replace("</b>", "").Replace(":", "");
                    bf_result = FormatBJDC_BFResult(bf_result);
                    decimal bf_sp = 1M;
                    string bf_sp1 = rows[12].Trim().Replace("<td class=\"border2\">", "").Replace("</b>", "");
                    decimal.TryParse(bf_sp1, out bf_sp);
                    string zjq_result = rows[13].Trim().Replace("<td><b class=\"font_red\">", "").Replace("</b>", "").Replace("+", "");
                    decimal zjq_sp = 1M;
                    string zjq_sp1 = rows[14].Trim().Replace("<td class=\"border2\">", "").Replace("</b>", "");
                    decimal.TryParse(zjq_sp1, out zjq_sp);
                    string bqc_result = rows[15].Trim().Replace("<td><b class=\"font_red\">", "").Replace("</b>", "").Replace("-", "");
                    decimal bqc_sp = 1M;
                    string bqc_sp1 = rows[16].Trim().Replace("<td class=\"border2\">", "").Replace("</b>", "");
                    decimal.TryParse(bqc_sp1, out bqc_sp);
                    string sxds_result = FormatGJDC_SXDSResult(rows[17].Trim().Replace("<td><b class=\"font_red\">", "").Replace("</b>", ""));
                    decimal sxds_sp = 1M;
                    string sxds_sp1 = rows[18].Trim().Replace("<td>", "").Replace("</b>", "");
                    decimal.TryParse(sxds_sp1, out sxds_sp);
                    list.Add(new C_BJDC_MatchResult
                    {
                        CreateTime = DateTime.Now,
                        IssuseNumber = issuseNumber,
                        MatchOrderId = matchOrderId,
                        MatchState = matchState,
                        Id = string.Format("{0}|{1}", issuseNumber, matchOrderId),
                        HomeFull_Result = homeFull_Result,
                        HomeHalf_Result = homeHalf_Result,
                        GuestFull_Result = guestFull_Result,
                        GuestHalf_Result = guestHalf_Result,
                        SPF_Result = spf_result,
                        SPF_SP = spf_sp,
                        BF_Result = bf_result,
                        BF_SP = bf_sp,
                        BQC_Result = bqc_result,
                        BQC_SP = bqc_sp,
                        SXDS_Result = sxds_result,
                        SXDS_SP = sxds_sp,
                        ZJQ_Result = zjq_result,
                        ZJQ_SP = zjq_sp,
                    });
                }
            }
            catch (Exception ex)
            {
                this.WriteLog(string.Format("解析第{0}期比赛结果异常：{1}", issuseNumber, ex.ToString()));
            }
            return list;
        }

        public List<C_BJDC_MatchResult> GetBJDC_MatchResultFrom500wan(string issuseNumber)
        {
            var list = new List<C_BJDC_MatchResult>();
            if (string.IsNullOrEmpty(issuseNumber))
                return list;

            try
            {
                var url = string.Format("http://zx.500.com/zqdc/kaijiang.php?playid=0&expect={0}", issuseNumber);
                this.WriteLog(string.Format("开始从地址：{0}采集数据", url));
                var html = PostManager.Get(url, Encoding.Default, 0, (request) =>
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
                if (html == "404" || string.IsNullOrEmpty(html))
                    return list;
                var tempArray = new string[] { };
                //step 1 得到div内容
                var index = html.IndexOf("<div class=\"lea_list\">");
                html = html.Substring(index);
                index = html.IndexOf("<div class=\"ld_bottom have_b\">");
                html = html.Substring(0, index);

                //step 2 得到table内容
                index = html.IndexOf("<table");
                html = html.Substring(index, html.Length - index);

                //step 4 去掉多余内容
                index = html.LastIndexOf("</th>");
                html = html.Substring(index, html.Length - index);

                //step 5 得到所有的行
                index = html.IndexOf("<tr");
                html = html.Substring(index, html.Length - index);
                index = html.LastIndexOf("</tr>");
                //content = content.Substring(0, index + 5);
                html = html.Substring(0, index);
                //Console.WriteLine(content);

                var rows = html.Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);
                this.WriteLog(string.Format("比赛结果行数：{0}行", rows.Length));
                foreach (var item in rows)
                {
                    try
                    {
                        var row = item.Trim();
                        //if (!row.Contains("<tr id=")) continue;

                        index = row.IndexOf(">");
                        row = row.Substring(index + 1);
                        //解析一场比赛的结果
                        //issuseNumber|matchOrderId
                        var id = string.Empty;
                        var matchOrderId = 0;
                        //"Finish" 完场，"Late" 延迟，"Cancel" 取消
                        var matchState = string.Empty;

                        var homeHalf_Result = string.Empty;
                        var homeFull_Result = string.Empty;
                        var guestHalf_Result = string.Empty;
                        var guestFull_Result = string.Empty;

                        var spf_result = string.Empty;
                        var spf_sp = 1M;
                        var bf_result = string.Empty;
                        var bf_sp = 1M;
                        var bqc_result = string.Empty;
                        var bqc_sp = 1M;
                        var zjq_result = string.Empty;
                        var zjq_sp = 1M;
                        var sxds_result = string.Empty;
                        var sxds_sp = 1M;

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
                                    //比赛编号
                                    //1
                                    matchOrderId = int.Parse(tdContent);
                                    break;
                                case 1:
                                    //联赛名称
                                    //NBA
                                    //leagueName = tdContent;
                                    break;
                                case 2:
                                    //比赛时间
                                    //10月31日10:30 
                                    break;
                                case 3:
                                    //主队名称
                                    //埃尔夫斯堡
                                    //homeTeamName = tdContent;
                                    break;
                                case 6:
                                    //全半场比分
                                    if (string.IsNullOrEmpty(tdContent) || tdContent == "-") break;
                                    var bf_Full = tdContent.Split(' ')[1];
                                    var bfL = bf_Full.Split(':');
                                    if (bfL.Count() == 2)
                                    {
                                        homeFull_Result = bfL[0];
                                        guestFull_Result = bfL[1];
                                    }
                                    break;
                                case 8:
                                    //SPF
                                    if (string.IsNullOrEmpty(tdContent) || tdContent == "-") break;
                                    spf_result = FormatBJDC_SPFResult(tdContent);
                                    break;
                                case 11:
                                    //ZJQ
                                    if (string.IsNullOrEmpty(tdContent) || tdContent == "-") break;
                                    zjq_result = tdContent.Replace("+", "");
                                    break;
                                case 14:
                                    //BF
                                    if (string.IsNullOrEmpty(tdContent) || tdContent == "-") break;
                                    bf_result = FormatBJDC_BFResult(tdContent);

                                    break;
                                case 17:
                                    //SXDS
                                    if (string.IsNullOrEmpty(tdContent) || tdContent == "-") break;
                                    var sum = int.Parse(homeFull_Result) + int.Parse(guestFull_Result);//zjq_result.GetInt32();
                                    sxds_result = sum >= 3 ? "S" : "X";
                                    sxds_result += sum % 2 == 0 ? "S" : "D";
                                    break;
                                case 20:
                                    //BQC
                                    if (string.IsNullOrEmpty(tdContent) || tdContent == "-") break;
                                    bqc_result = FormatBJDC_BQCResult(tdContent);
                                    break;
                                default:
                                    break;
                            }
                        }
                        #endregion

                        list.Add(new C_BJDC_MatchResult
                        {
                            CreateTime = DateTime.Now,//.ToString("yyyy-MM-dd HH:mm:ss"),
                            IssuseNumber = issuseNumber,
                            MatchOrderId = matchOrderId,
                            MatchState = matchState,
                            Id = string.Format("{0}|{1}", issuseNumber, matchOrderId),
                            HomeFull_Result = homeFull_Result,
                            HomeHalf_Result = homeHalf_Result,
                            GuestFull_Result = guestFull_Result,
                            GuestHalf_Result = guestHalf_Result,
                            SPF_Result = spf_result,
                            SPF_SP = spf_sp,
                            BF_Result = bf_result,
                            BF_SP = bf_sp,
                            BQC_Result = bqc_result,
                            BQC_SP = bqc_sp,
                            SXDS_Result = sxds_result,
                            SXDS_SP = sxds_sp,
                            ZJQ_Result = zjq_result,
                            ZJQ_SP = zjq_sp,
                        });
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(string.Format("解析第{0}期比赛结果 单条数据 异常：{1}", issuseNumber, ex.ToString()));
                    }
                }
                this.WriteLog(string.Format("从地址：{0}采集数据 {1} 条", url, list.Count));
            }
            catch (Exception ex)
            {
                this.WriteLog(string.Format("解析第{0}期比赛结果异常：{1}", issuseNumber, ex.ToString()));
            }

            return list;
        }

        public List<C_BJDC_Match> GetNewLeagueInfoList(List<C_BJDC_Match> oldList, List<C_BJDC_Match> newList)
        {
            var list = new List<C_BJDC_Match>();
            foreach (var item in newList)
            {
                var old = oldList.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
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
        public List<BJDC_Match_SFGG> GetNewLeague_SFGGInfoList(List<BJDC_Match_SFGG> oldList, List<BJDC_Match_SFGG> newList)
        {
            var list = new List<BJDC_Match_SFGG>();
            foreach (var item in newList)
            {
                var old = oldList.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
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

        private List<C_BJDC_MatchResult> GetNewLeagueResultList(List<C_BJDC_MatchResult> oldList, List<C_BJDC_MatchResult> newList)
        {
            var list = new List<C_BJDC_MatchResult>();
            foreach (var item in newList)
            {
                var old = oldList.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
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

        public List<C_BJDC_MatchResult> GetBJDC_MatchResultFromAiCai(string issuseNumber)
        {
            var list = new List<C_BJDC_MatchResult>();
            if (string.IsNullOrEmpty(issuseNumber))
                return list;

            var url = string.Format("http://www.aicai.com/lottery/dcSp.jhtml?issueNo={0}&gameId=405", issuseNumber);

            #region 计算结果

            Func<string, string[]> getBf = (bf) =>
            {
                return bf.Replace("\r\n", "").Replace("\t", "").Replace(" ", "").Split(':');
            };
            Func<string, decimal> getSp = (sp) =>
            {
                return decimal.Parse(sp.Replace("<span class=\"red\"><script>document.write(getbisai(\"", "").Replace("\"));</script></span>", ""));
            };
            Func<int, int, int, string> getSPF_Result = (home, lb, guest) =>
            {
                var h = home + lb;
                if (h > guest)
                    return "3";
                if (h < guest)
                    return "0";
                return "1";
            };
            Func<int, int, string> getZJQ_Result = (home, guest) =>
            {
                var total = home + guest;
                if (total >= 7)
                    return "7";
                return total.ToString();
            };
            Func<int, int, string> getSXDS_Result = (home, guest) =>
            {
                var sum = home + guest;
                var sxds_result = sum >= 3 ? "S" : "X";
                sxds_result += sum % 2 == 0 ? "S" : "D";
                return sxds_result;
            };
            Func<int, int, string> getBF_Result = (home, guest) =>
            {
                var sArray = new string[] { "10", "20", "21", "30", "31", "32", "40", "41", "42" };
                var pArray = new string[] { "00", "11", "22", "33", };
                var fArray = new string[] { "01", "02", "12", "03", "13", "23", "04", "14", "24" };
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
            Func<int, int, int, int, string> getBQC_Result = (homeHalf, guestHalf, homeFull, guestFull) =>
            {
                var half = getSPF_Result(homeHalf, 0, guestHalf);
                var full = getSPF_Result(homeFull, 0, guestFull);
                return string.Format("{0}{1}", half, full);
            };

            #endregion

            var content = PostManager.Get(url, Encoding.UTF8, 0, (request) =>
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
            content = content.Substring(content.IndexOf("<tbody>"));
            var index = content.IndexOf("</tbody>");
            content = content.Substring(0, index).Replace("<tbody>", "");
            var trs = content.Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var tr in trs)
            {
                var matchOrderId = 0;
                var homeHalf_Result = "-1";
                var homeFull_Result = "-1";
                var guestHalf_Result = "-1";
                var guestFull_Result = "-1";
                var spf_result = "-1";
                var spf_sp = 1M;
                var zjq_result = "-1";
                var zjq_sp = 1M;
                var sxds_result = "-1";
                var sxds_sp = 1M;
                var bf_result = "-1";
                var bf_sp = 1M;
                var bqc_result = "-1";
                var bqc_sp = 1M;
                var lb = 0;
                var matchState = "Finish";

                var tds = tr.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                var noResult = false;

                #region 解析每行数据

                for (int i = 0; i < tds.Length; i++)
                {
                    var td = tds[i];
                    var tdIndex = td.IndexOf("<td>");
                    if (tdIndex < 0) continue;

                    var tdContent = td.Substring(tdIndex + 4);
                    //Console.WriteLine(i + "---" + tdContent);
                    switch (i)
                    {
                        case 0:
                            matchOrderId = tdContent.GetInt32();
                            break;
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            //让球
                            lb = int.Parse(tdContent.Replace("<span>", "").Replace("</span>", ""));
                            break;
                        case 4:
                            break;
                        case 5:
                            //半场比分
                            var halfBfArray = getBf(tdContent);
                            //此行暂无结果
                            noResult = halfBfArray.Length == 1 && halfBfArray[0] == "--";
                            if (halfBfArray.Length != 2)
                            {
                                matchState = "Cancel";
                                break;
                            }
                            homeHalf_Result = halfBfArray[0];
                            guestHalf_Result = halfBfArray[1];
                            break;
                        case 6:
                            //全场比分
                            var fullBfArray = getBf(tdContent);
                            //此行暂无结果
                            noResult = fullBfArray.Length == 1 && fullBfArray[0] == "--";
                            if (fullBfArray.Length != 2)
                            {
                                matchState = "Cancel";
                                break;
                            }
                            homeFull_Result = fullBfArray[0];
                            guestFull_Result = fullBfArray[1];
                            break;
                        case 7:
                            //让球胜平负
                            if (matchState == "Cancel")
                                break;
                            spf_result = getSPF_Result(int.Parse(homeFull_Result), lb, int.Parse(guestFull_Result));
                            spf_sp = getSp(tdContent);
                            break;
                        case 8:
                            //上下单双
                            if (matchState == "Cancel")
                                break;
                            sxds_result = getSXDS_Result(int.Parse(homeFull_Result), int.Parse(guestFull_Result));
                            sxds_sp = getSp(tdContent);
                            break;
                        case 9:
                            //总进球数
                            if (matchState == "Cancel")
                                break;
                            zjq_result = getZJQ_Result(int.Parse(homeFull_Result), int.Parse(guestFull_Result));
                            zjq_sp = getSp(tdContent);
                            break;
                        case 10:
                            //全场比分
                            if (matchState == "Cancel")
                                break;
                            bf_result = getBF_Result(int.Parse(homeFull_Result), int.Parse(guestFull_Result));
                            bf_sp = getSp(tdContent);
                            break;
                        case 11:
                            //半全场
                            if (matchState == "Cancel")
                                break;
                            bqc_result = getBQC_Result(int.Parse(homeHalf_Result), int.Parse(guestHalf_Result), int.Parse(homeFull_Result), int.Parse(guestFull_Result));
                            bqc_sp = getSp(tdContent);
                            break;
                        default:
                            break;
                    }
                }

                #endregion

                if (matchOrderId == 0) continue;
                if (noResult) continue;
                list.Add(new C_BJDC_MatchResult
                {
                    Id = string.Format("{0}|{1}", issuseNumber, matchOrderId),
                    CreateTime = DateTime.Now,
                    IssuseNumber = issuseNumber,
                    MatchOrderId = matchOrderId,
                    HomeHalf_Result = homeHalf_Result,
                    HomeFull_Result = homeFull_Result,
                    GuestHalf_Result = guestHalf_Result,
                    GuestFull_Result = guestFull_Result,
                    SPF_Result = spf_result,
                    SPF_SP = spf_sp,
                    ZJQ_Result = zjq_result,
                    ZJQ_SP = zjq_sp,
                    SXDS_Result = sxds_result,
                    SXDS_SP = sxds_sp,
                    BF_Result = bf_result,
                    BF_SP = bf_sp,
                    BQC_Result = bqc_result,
                    BQC_SP = bqc_sp,
                    MatchState = matchState,
                });
            }

            return list;
        }

        public List<C_BJDC_MatchResult> GetBJDC_MatchResultFrom310Win(string issuseNumber)
        {
            var list = new List<C_BJDC_MatchResult>();
            if (string.IsNullOrEmpty(issuseNumber))
                return list;

            try
            {
                var url = string.Format("http://www.310win.com/beijingdanchang/kaijiang_dc_{0}_all.html", issuseNumber);
                this.WriteLog(string.Format("开始从地址：{0}采集数据", url));
                var html = PostManager.Get(url, Encoding.UTF8, 0, (request) =>
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
                if (html == "404")
                    return list;
                var tempArray = new string[] { };
                //step 1 得到div内容
                var index = html.IndexOf("<div id=\"lottery_container\">");
                html = html.Substring(index);
                index = html.IndexOf("</div>");
                html = html.Substring(0, index);

                //step 2 得到table内容
                index = html.IndexOf("<table");
                html = html.Substring(index, html.Length - index);

                //step 3 得到所有的行
                index = html.IndexOf("<tr");
                html = html.Substring(index, html.Length - index);
                index = html.LastIndexOf("</tr>");
                //content = content.Substring(0, index + 5);
                html = html.Substring(0, index);
                //Console.WriteLine(content);

                var rows = html.Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);
                this.WriteLog(string.Format("比赛结果行数：{0}行", rows.Length));
                foreach (var item in rows)
                {
                    try
                    {
                        var row = item.Trim();
                        if (!row.Contains("<tr id=")) continue;

                        index = row.IndexOf(">");
                        row = row.Substring(index + 1);
                        //解析一场比赛的结果
                        //issuseNumber|matchOrderId
                        var id = string.Empty;
                        var matchOrderId = 0;
                        //"Finish" 完场，"Late" 延迟，"Cancel" 取消
                        var matchState = string.Empty;

                        var homeHalf_Result = string.Empty;
                        var homeFull_Result = string.Empty;
                        var guestHalf_Result = string.Empty;
                        var guestFull_Result = string.Empty;

                        var spf_result = string.Empty;
                        var spf_sp = 1M;
                        var bf_result = string.Empty;
                        var bf_sp = 1M;
                        var bqc_result = string.Empty;
                        var bqc_sp = 1M;
                        var zjq_result = string.Empty;
                        var zjq_sp = 1M;
                        var sxds_result = string.Empty;
                        var sxds_sp = 1M;

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
                                    //比赛编号
                                    //1
                                    matchOrderId = int.Parse(tdContent);
                                    break;
                                case 1:
                                    //联赛名称
                                    //NBA
                                    //leagueName = tdContent;
                                    break;
                                case 2:
                                    //比赛时间
                                    //10月31日10:30 
                                    break;
                                case 3:
                                    //主队名称
                                    //埃尔夫斯堡
                                    //homeTeamName = tdContent;
                                    break;
                                case 4:
                                    //全半场比分
                                    //<b>2-0</b> 或 中文
                                    //<span style=\"color: Red;font-weight:bold;\"> 6-0 </span><br />3-0
                                    tempArray = tdContent.Split(new string[] { "<br />", "<br/>" }, StringSplitOptions.None);
                                    if (tempArray.Length == 2)
                                    {
                                        index = tempArray[0].IndexOf(">");
                                        tempArray[0] = tempArray[0].Substring(index + 1).Replace("</span>", "").Trim();
                                        switch (tempArray[0])
                                        {
                                            case "推迟":
                                                matchState = "Late";
                                                break;
                                            case "腰斩":
                                                matchState = "Cancel";
                                                break;
                                            default:
                                                matchState = "Finish";
                                                break;
                                        }
                                    }

                                    if (matchState == "Finish")
                                    {
                                        var fullArray = tempArray[0].Split('-');
                                        var halfArray = tempArray[1].Split('-');
                                        if (fullArray.Length == 2 && halfArray.Length == 2)
                                        {
                                            homeFull_Result = fullArray[0];
                                            guestFull_Result = fullArray[1];
                                            homeHalf_Result = halfArray[0];
                                            guestHalf_Result = halfArray[1];
                                        }
                                    }
                                    if (matchState == "Cancel")
                                    {
                                        homeHalf_Result = "-1";
                                        homeFull_Result = "-1";
                                        guestHalf_Result = "-1";
                                        guestFull_Result = "-1";
                                    }
                                    break;
                                case 5:
                                    //客队名称
                                    //耶夫勒
                                    //guestTeamName = tdContent;
                                    var t = tdContent;
                                    break;
                                case 6:
                                    //SPF
                                    tempArray = tdContent.Split(new string[] { "<br />", "<br>" }, StringSplitOptions.RemoveEmptyEntries);
                                    if (matchState == "Finish" && tempArray.Length == 2)
                                    {
                                        index = tempArray[0].IndexOf(">");
                                        tempArray[0] = tempArray[0].Substring(index + 1).Replace("</span>", "");
                                        spf_result = FormatBJDC_SPFResult(tempArray[0]);

                                        index = tempArray[1].IndexOf(">");
                                        tempArray[1] = tempArray[1].Substring(index + 1);
                                        tempArray[1] = tempArray[1].Replace("</a>", "");
                                        if (string.IsNullOrEmpty(tempArray[1]))
                                            spf_sp = 1M;
                                        else
                                            decimal.TryParse(tempArray[1], out spf_sp);
                                    }
                                    if (matchState == "Cancel")
                                    {
                                        spf_result = "-1";
                                        spf_sp = 1M;
                                    }
                                    break;
                                case 7:
                                    //ZJQ
                                    //<span style='color:#f00;'>6球</span><br><a href="/sp/danchang/121101_1_6.html" target="_blank" style="color:#666">33.069284</a>
                                    tempArray = tdContent.Split(new string[] { "<br />", "<br>" }, StringSplitOptions.RemoveEmptyEntries);
                                    if (matchState == "Finish" && tempArray.Length == 2)
                                    {
                                        index = tempArray[0].IndexOf(">");
                                        tempArray[0] = tempArray[0].Substring(index + 1).Replace("</span>", "").Replace("球", "").Replace("+", "");
                                        zjq_result = tempArray[0];

                                        index = tempArray[1].IndexOf(">");
                                        tempArray[1] = tempArray[1].Substring(index + 1);
                                        tempArray[1] = tempArray[1].Replace("</a>", "");
                                        if (string.IsNullOrEmpty(tempArray[1]))
                                            zjq_sp = 1M;
                                        else
                                            decimal.TryParse(tempArray[1], out zjq_sp);
                                    }
                                    if (matchState == "Cancel")
                                    {
                                        zjq_result = "-1";
                                        zjq_sp = 1M;
                                    }
                                    break;
                                case 8:
                                    //SXDS
                                    //<span style='color:#f00;'>上双</span><br><a href="/sp/danchang/121101_1_7.html" target="_blank" style="color:#666">4.942275</a>
                                    tempArray = tdContent.Split(new string[] { "<br />", "<br>" }, StringSplitOptions.RemoveEmptyEntries);
                                    if (matchState == "Finish" && tempArray.Length == 2)
                                    {
                                        index = tempArray[0].IndexOf(">");
                                        tempArray[0] = tempArray[0].Substring(index + 1).Replace("</span>", "");
                                        //rfsf_Result = FormatRFSFResult(tempArray[0].Replace("</span>", ""));
                                        var sum = int.Parse(homeFull_Result) + int.Parse(guestFull_Result);//zjq_result.GetInt32();
                                        sxds_result = sum >= 3 ? "S" : "X";
                                        sxds_result += sum % 2 == 0 ? "S" : "D";

                                        index = tempArray[1].IndexOf(">");
                                        tempArray[1] = tempArray[1].Substring(index + 1);
                                        tempArray[1] = tempArray[1].Replace("</a>", "");
                                        if (string.IsNullOrEmpty(tempArray[1]))
                                            sxds_sp = 1M;
                                        else
                                            decimal.TryParse(tempArray[1], out sxds_sp);
                                    }
                                    if (matchState == "Cancel")
                                    {
                                        sxds_result = "-1";
                                        sxds_sp = 1M;
                                    }
                                    break;
                                case 9:
                                    //BF
                                    //<span style='color:#f00;'>胜其他</span><br><a href="/sp/danchang/121101_1_8.html" target="_blank" style="color:#666">16.394782</a>
                                    tempArray = tdContent.Split(new string[] { "<br />", "<br>" }, StringSplitOptions.RemoveEmptyEntries);
                                    if (matchState == "Finish" && tempArray.Length == 2)
                                    {
                                        index = tempArray[0].IndexOf(">");
                                        tempArray[0] = tempArray[0].Substring(index + 1).Replace("</span>", "");
                                        bf_result = FormatBJDC_BFResult(tempArray[0]);

                                        index = tempArray[1].IndexOf(">");
                                        tempArray[1] = tempArray[1].Substring(index + 1);
                                        tempArray[1] = tempArray[1].Replace("</a>", "");
                                        if (string.IsNullOrEmpty(tempArray[1]))
                                            bf_sp = 1M;
                                        else
                                            decimal.TryParse(tempArray[1], out bf_sp);
                                    }
                                    if (matchState == "Cancel")
                                    {
                                        bf_result = "-1";
                                        bf_sp = 1M;
                                    }
                                    break;
                                case 10:
                                    //BQC
                                    //<span style='color:#f00;'>胜-胜</span><br><a href="/sp/danchang/121101_1_9.html" target="_blank" style="color:#666">1.907655</a>
                                    tempArray = tdContent.Split(new string[] { "<br />", "<br>" }, StringSplitOptions.RemoveEmptyEntries);
                                    if (matchState == "Finish" && tempArray.Length == 2)
                                    {
                                        index = tempArray[0].IndexOf(">");
                                        tempArray[0] = tempArray[0].Substring(index + 1).Replace("</span>", "");
                                        bqc_result = FormatBJDC_BQCResult(tempArray[0]);

                                        index = tempArray[1].IndexOf(">");
                                        tempArray[1] = tempArray[1].Substring(index + 1);
                                        tempArray[1] = tempArray[1].Replace("</a>", "");
                                        if (string.IsNullOrEmpty(tempArray[1]))
                                            bqc_sp = 1M;
                                        else
                                            decimal.TryParse(tempArray[1], out bqc_sp);
                                    }
                                    if (matchState == "Cancel")
                                    {
                                        bqc_result = "-1";
                                        bqc_sp = 1M;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        #endregion

                        list.Add(new C_BJDC_MatchResult
                        {
                            CreateTime = DateTime.Now,
                            IssuseNumber = issuseNumber,
                            MatchOrderId = matchOrderId,
                            MatchState = matchState,
                            Id = string.Format("{0}|{1}", issuseNumber, matchOrderId),
                            HomeFull_Result = homeFull_Result,
                            HomeHalf_Result = homeHalf_Result,
                            GuestFull_Result = guestFull_Result,
                            GuestHalf_Result = guestHalf_Result,
                            SPF_Result = spf_result,
                            SPF_SP = spf_sp,
                            BF_Result = bf_result,
                            BF_SP = bf_sp,
                            BQC_Result = bqc_result,
                            BQC_SP = bqc_sp,
                            SXDS_Result = sxds_result,
                            SXDS_SP = sxds_sp,
                            ZJQ_Result = zjq_result,
                            ZJQ_SP = zjq_sp,
                        });
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(string.Format("解析第{0}期比赛结果 单条数据 异常：{1}", issuseNumber, ex.ToString()));
                    }
                }
                this.WriteLog(string.Format("从地址：{0}采集数据 {1} 条", url, list.Count));
            }
            catch (Exception ex)
            {
                this.WriteLog(string.Format("解析第{0}期比赛结果异常：{1}", issuseNumber, ex.ToString()));
            }

            return list;
        }

        private List<BJDC_Match_SFGGResult> GetNewLeagueResult_sfggList(List<BJDC_Match_SFGGResult> oldList, List<BJDC_Match_SFGGResult> newList)
        {
            var list = new List<BJDC_Match_SFGGResult>();
            foreach (var item in newList)
            {
                var old = oldList.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
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

        public List<BJDC_Match_SFGGResult> GetBJDC_Match_SFGGResultFrom310Win(string issuseNumber)
        {
            var list = new List<BJDC_Match_SFGGResult>();
            if (string.IsNullOrEmpty(issuseNumber))
                return list;

            try
            {
                var url = string.Format("http://www.310win.com/others/kaijiang_dc_11_{0}.html", issuseNumber.Substring(1));
                this.WriteLog(string.Format("开始从地址：{0}采集数据", url));
                var html = PostManager.Get(url, Encoding.UTF8, 0, (request) =>
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
                if (html == "404")
                    return list;
                var tempArray = new string[] { };
                //step 1 得到div内容
                var index = html.IndexOf("<div id=\"lottery_container\">");
                html = html.Substring(index);
                index = html.IndexOf("</div>");
                html = html.Substring(0, index);

                //step 2 得到table内容
                index = html.IndexOf("<table");
                html = html.Substring(index, html.Length - index);

                //step 3 得到所有的行
                index = html.IndexOf("<tr");
                html = html.Substring(index, html.Length - index);
                index = html.LastIndexOf("</tr>");
                //content = content.Substring(0, index + 5);
                html = html.Substring(0, index);
                //Console.WriteLine(content);

                var rows = html.Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);
                this.WriteLog(string.Format("比赛结果行数：{0}行", rows.Length));
                foreach (var item in rows)
                {
                    try
                    {
                        var row = item.Trim();
                        if (!row.Contains("<tr id=")) continue;

                        index = row.IndexOf(">");
                        row = row.Substring(index + 1);
                        //解析一场比赛的结果
                        //issuseNumber|matchOrderId
                        var id = string.Empty;
                        var matchOrderId = 0;
                        //"Finish" 完场，"Late" 延迟，"Cancel" 取消
                        var matchState = string.Empty;

                        var homeFull_Result = string.Empty;
                        var guestFull_Result = string.Empty;

                        var sf_result = string.Empty;
                        var sf_sp = 1M;
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
                                    //比赛编号
                                    //1
                                    matchOrderId = int.Parse(tdContent);
                                    break;
                                case 1:
                                    //类别 足球、篮球
                                    break;
                                case 2:
                                    //联赛名称
                                    break;
                                case 3:
                                    //开赛时间
                                    break;
                                case 4:
                                    //主队名
                                    break;
                                case 5:
                                    //比分
                                    //<span style="color: Red;font-weight:bold;"> 2-5 </span>
                                    //tempArray = tdContent.Split(new string[] { "<br />", "<br/>" }, StringSplitOptions.None);

                                    index = tdContent.IndexOf(">");
                                    tdContent = tdContent.Substring(index + 1).Replace("</span>", "").Trim();
                                    switch (tdContent)
                                    {
                                        case "推迟":
                                            matchState = "Late";
                                            break;
                                        case "腰斩":
                                        case "取消":
                                            matchState = "Cancel";
                                            break;
                                        default:
                                            matchState = "Finish";
                                            break;
                                    }

                                    if (matchState == "Finish")
                                    {
                                        var fullArray = tdContent.Split('-');
                                        if (fullArray.Length == 2)
                                        {
                                            homeFull_Result = fullArray[0];
                                            guestFull_Result = fullArray[1];
                                        }
                                    }
                                    if (matchState == "Cancel")
                                    {
                                        homeFull_Result = "-1";
                                        guestFull_Result = "-1";
                                    }
                                    break;
                                case 6:
                                    //客队名称
                                    break;
                                case 7:
                                    //让分(局)
                                    break;
                                case 8:
                                    //结果
                                    index = tdContent.IndexOf(">");
                                    if (index != -1)
                                        sf_result = tdContent.Substring(index + 1).Replace("</span>", "").Trim().Replace("胜", "3").Replace("负", "0");
                                    if (matchState == "Cancel")
                                        sf_result = "-1";
                                    break;
                                case 9:
                                    //开奖SP
                                    tempArray = tdContent.Split(new string[] { "<u>", "</u>" }, StringSplitOptions.None);
                                    if (!string.IsNullOrEmpty(tempArray[1]))
                                        sf_sp = decimal.Parse(tempArray[1]);
                                    break;
                                default:
                                    break;
                            }
                        }
                        #endregion

                        list.Add(new BJDC_Match_SFGGResult
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            IssuseNumber = issuseNumber,
                            MatchOrderId = matchOrderId,
                            MatchState = matchState,
                            Id = string.Format("{0}|{1}", issuseNumber, matchOrderId),
                            HomeFull_Result = homeFull_Result,
                            GuestFull_Result = guestFull_Result,
                            SF_Result = sf_result,
                            SF_SP = sf_sp,
                        });
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(string.Format("解析第{0}期胜负过关比赛结果 单条数据 异常：{1}", issuseNumber, ex.ToString()));
                    }
                }
                this.WriteLog(string.Format("从地址：{0}采集胜负过关数据 {1} 条", url, list.Count));
            }
            catch (Exception ex)
            {
                this.WriteLog(string.Format("解析第{0}期胜负过关比赛结果异常：{1}", issuseNumber, ex.ToString()));
            }

            return list;
        }


        private string FormatBJDC_SPFResult(string content)
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
            return content;
        }
        private string FormatBJDC_BQCResult(string content)
        {
            var r = string.Empty;
            foreach (var item in content.Split('-'))
            {
                r += FormatBJDC_SPFResult(item.ToString());
            }
            return r;
        }
        private string FormatGJDC_SXDSResult(string content)
        {
            switch (content)
            {
                case "上单":
                    return "SD";
                case "上双":
                    return "SS";
                case "下单":
                    return "XD";
                case "下双":
                    return "XS";
                default:
                    return "";
            }
        }
        private string FormatBJDC_BFResult(string content)
        {
            var allowCode = new string[] { "1:0", "2:0", "2:1", "3:0", "3:1", "3:2", "4:0", "4:1", "4:2",
                                           "0:0", "1:1", "2:2", "3:3",
                                           "0:1", "0:2","1:2","0:3","1:3","2:3","0:4","1:4","2:4" };
            if (allowCode.Contains(content))
                return content.Replace(":", "");
            switch (content)
            {
                case "胜其他":
                    return "X0";
                case "平其他":
                    return "XX";
                case "负其他":
                    return "0X";
                default:
                    break;
            }
            return content;
        }

        public List<C_BJDC_Match> GetZS_MatchList(string issuseNumber)
        {
            var list = new List<C_BJDC_Match>();
            var clUrl = "http://live.qcw.com/apps?lotyid=5&expect=" + issuseNumber;
            var clDoc = new XmlDocument();
            var clContent = PostManager.Get(clUrl, Encoding.UTF8, 0, (request) =>
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

            if (string.IsNullOrEmpty(clContent) || clContent == "404" || clContent == "[]")
                return list;

            var dic = JsonHelper.Decode(clContent) as Dictionary<string, object>;
            foreach (var item in dic)
            {
                //htid atid lid mid
                var v = item.Value as Dictionary<string, object>;
                var orderId = item.Key;
                var htid = v.ContainsKey("htid") ? v["htid"].ToString() : "0";
                var atid = v.ContainsKey("atid") ? v["atid"].ToString() : "0";
                var lid = v.ContainsKey("lid") ? v["lid"].ToString() : "0";
                var mid = v.ContainsKey("mid") ? v["mid"].ToString() : "0";
                var cl = v.ContainsKey("cl") ? v["cl"].ToString() : "";
                var ln = v.ContainsKey("ln") ? v["ln"].ToString() : "";

                list.Add(new C_BJDC_Match
                {
                    MatchOrderId = int.Parse(orderId),
                    HomeTeamId = int.Parse(htid),
                    GuestTeamId = int.Parse(atid),
                    MatchId = int.Parse(lid),
                    Mid = int.Parse(mid),
                    MatchColor = cl,
                    IssuseNumber = issuseNumber,
                    //FXId = int.Parse(sid),
                    MatchName = ln
                });
            }

            return list;
        }

        private List<C_BJDC_Match> GetBJDCMatchList_CPDJY(string issuseNumber)
        {
            var matchList = new List<C_BJDC_Match>();
            var zsMathList = GetZS_MatchList(issuseNumber);

            //var fxDic = ServiceHelper.GetFXIdSource() == "okooo" ? GetBJDC_FX_OKOOO(issuseNumber) : GetBJDC_FX(issuseNumber);

            System.DateTime t = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long tt = (DateTime.Now.Ticks - t.Ticks) / 10000;
            var url = string.Format("http://intf.cpdyj.com/data/dc/match/{1}.js?callback=game&_={0}", tt, issuseNumber);
            var encoding = Encoding.GetEncoding("gb2312");
            var content = PostManager.Get(url, encoding, 0, (request) =>
            {
                request.Host = "intf.cpdyj.com";
                request.Referer = "http://bd.cpdyj.com/";
                if (ServiceHelper.IsUseProxy("BJDC"))
                {
                    var proxy = ServiceHelper.GetProxyUrl();
                    if (!string.IsNullOrEmpty(proxy))
                    {
                        request.Proxy = new System.Net.WebProxy(proxy);
                    }
                }
            });
            content = content.Replace("game(", "").Replace(");", "").Trim();

            var array = JsonHelper.Decode(content);
            var ceshi = string.Empty;
            foreach (var matchArray in array)
            {
                foreach (var matchItem in matchArray)
                {
                    //["1","#4397F6","1906","1907","565","韩足总,全北现代,浦项制铁","2.36,3.18,2.74","2013-10-19 12:30:00.0","705787","0","250","2013-10-19 12:15:00.0","0","-1:-1,-1:-1","-1,-1,-1,-1,-1","0,0,0,0,0",0,0]
                    try
                    {
                        var orderNum = int.Parse(matchItem[0]);
                        string[] nameArray = matchItem[5].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        string[] oddArray = matchItem[6].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        if (nameArray.Length != 3 || oddArray.Length != 3)
                            continue;

                        ceshi = nameArray[0];
                        var homeTeamName = nameArray[1];
                        var guestTeamName = nameArray[2];
                        var matchName = nameArray[0].IndexOf("-") == -1 ? nameArray[0] : nameArray[0].Substring(5);
                        //var homeTeam = manager.QueryTeamEntity(homeTeamName);
                        //var guestTeam = manager.QueryTeamEntity(guestTeamName);
                        //var match = manager.QueryLeagueEntity(matchName);

                        var state = BJDCMatchState.Sales;// (BJDCMatchState)int.Parse(item.Attributes["iaudit"].Value);
                        var startTime = DateTime.Parse(matchItem[7]);
                        var endTime = DateTime.Parse(matchItem[11]);

                        var fxKey = string.Format("{0}_{1}", issuseNumber, orderNum);
                        //int fxId = fxDic.ContainsKey(fxKey) ? int.Parse(string.IsNullOrEmpty(fxDic[fxKey]) ? "0" : fxDic[fxKey]) : 0;

                        var zsMatch = zsMathList.FirstOrDefault(p => p.MatchOrderId == orderNum);

                        matchList.Add(new C_BJDC_Match
                        {
                            MatchOrderId = orderNum,
                            Id = string.Format("{0}|{1}", issuseNumber, orderNum),
                            MatchName = matchName,
                            HomeTeamName = homeTeamName,
                            GuestTeamName = guestTeamName,
                            IssuseNumber = issuseNumber,
                            CreateTime = DateTime.Now,
                            LetBall = int.Parse(matchItem[12]),
                            MatchColor = zsMatch != null ? zsMatch.MatchColor : string.Empty,
                            MatchStartTime = startTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            LocalStopTime = endTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            WinOdds = decimal.Parse(oddArray[0]),
                            FlatOdds = decimal.Parse(oddArray[1]),
                            LoseOdds = decimal.Parse(oddArray[2]),
                            HomeTeamSort = "",
                            GuestTeamSort = "",
                            MatchState = (int)state,
                            MatchId = zsMatch != null ? zsMatch.MatchId : 0,
                            HomeTeamId = zsMatch != null ? zsMatch.HomeTeamId : 0,
                            GuestTeamId = zsMatch != null ? zsMatch.GuestTeamId : 0,
                            Mid = zsMatch != null ? zsMatch.Mid : 0,//ServiceHelper.GetLeagueCategory(match),//match == null ? 0 : match.team == null ? 0 : team.Attributes["oddsmid"].Value.GetInt32(),
                            FXId = zsMatch != null ? zsMatch.FXId : 0,
                            Hi = matchItem[2],
                            Gi = matchItem[3],
                        });
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(string.Format("解析单条队伍信息异常：{0}_{1}", ex.ToString(), ceshi));
                    }
                }
            }

            return matchList;
        }
        private List<C_BJDC_Match> GetBJDCMatchList(string issuseNumber)
        {
            var leagueList = new List<C_BJDC_Match>();

            try
            {
                //http://trade.cpdyj.com/staticdata/lotteryinfo/odds/bd/121104.xml?rnd=0.509534405391288
                var matchUrl = string.Format("http://www.9188.com/data/bd/{0}/spf.xml", issuseNumber);
                var xmlContent = PostManager.Get(matchUrl, Encoding.UTF8, 0, (request) =>
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
                if (string.IsNullOrEmpty(xmlContent))
                    return leagueList;
                var docBD = new XmlDocument();
                docBD.LoadXml(xmlContent);
                var matchRoot = docBD.SelectSingleNode("Resp");

                var fxDic = ServiceHelper.GetFXIdSource() == "okooo" ? GetBJDC_FX_OKOOO(issuseNumber) : GetBJDC_FX(issuseNumber);

                //XmlNode lcMatchRoot = null;
                //if (ServiceHelper.Get_BJDC_TeamInfo())
                //{
                //    var url = string.Format("http://odds.iiioo.com/sdata/getzcxml.php?lotyid=5&qh={0}", issuseNumber);
                //    var content = PostManager.Get(url, Encoding.GetEncoding("gb2312"));
                //    var lcDocBD = new XmlDocument();
                //    lcDocBD.LoadXml(content);
                //    lcMatchRoot = lcDocBD.SelectSingleNode("xml");
                //}

                var spUrl = string.Format("http://www.9188.com/data/static/info/bjdc/sp/{0}_odds.xml", issuseNumber);
                var spXml = PostManager.Get(spUrl, Encoding.UTF8, 0, (request) =>
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
                var spDoc = new XmlDocument();
                spDoc.LoadXml(spXml);
                var spRoot = spDoc.SelectSingleNode("xml");

                var zsMathList = GetZS_MatchList(issuseNumber);
                #region 比赛队伍

                foreach (XmlNode item in matchRoot.ChildNodes)
                {
                    try
                    {
                        var state = (BJDCMatchState)int.Parse(item.Attributes["iaudit"].Value);
                        var startTime = item.Attributes["bt"].Value.GetDateTime();
                        var endTime = item.Attributes["et"].Value.GetDateTime().AddMinutes(LeagueAdvanceMinutes);

                        //tod 北单时间计算
                        //var minTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 4, 0, 0);
                        //var maxTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 10, 0, 0);
                        //endTime = startTime > minTime && startTime < maxTime ? minTime : startTime.AddMinutes(-15).AddMinutes(LeagueAdvanceMinutes);

                        var orderId = item.Attributes["mid"].Value.GetInt32();

                        var homeTeamName = item.Attributes["hn"].Value;
                        var guestTeamName = item.Attributes["gn"].Value;
                        var matchName = item.Attributes["mname"].Value;
                        //var homeTeam = manager.QueryTeamEntity(homeTeamName);
                        //var guestTeam = manager.QueryTeamEntity(guestTeamName);
                        //var match = manager.QueryLeagueEntity(matchName);
                        var orderNum = item.Attributes["mid"].Value;

                        var fxKey = string.Format("{0}_{1}", issuseNumber, orderNum);
                        var fxId = fxDic.ContainsKey(fxKey) ? int.Parse(string.IsNullOrEmpty(fxDic[fxKey]) ? "0" : fxDic[fxKey]) : 0;

                        var zsMatch = zsMathList.FirstOrDefault(p => p.IssuseNumber == issuseNumber && p.MatchOrderId == int.Parse(orderNum));
                        //var team = lcMatchRoot == null ? null : lcMatchRoot.SelectSingleNode(string.Format("//row[@xid='{0}']", orderId));
                        var team = spRoot.SelectSingleNode(string.Format("//row[@xid='{0}']", orderNum));

                        leagueList.Add(new C_BJDC_Match
                        {
                            Id = string.Format("{0}|{1}", issuseNumber, orderNum),
                            MatchName = matchName,
                            HomeTeamName = homeTeamName,
                            GuestTeamName = guestTeamName,
                            IssuseNumber = issuseNumber,
                            MatchOrderId = orderId,
                            CreateTime = DateTime.Now,
                            LetBall = item.Attributes["close"].Value.GetInt32(),
                            MatchColor = item.Attributes["cl"].Value,
                            MatchStartTime = startTime,
                            LocalStopTime = endTime,
                            WinOdds = item.Attributes["b3"].Value.GetDecimal(),
                            FlatOdds = item.Attributes["b1"].Value.GetDecimal(),
                            LoseOdds = item.Attributes["b0"].Value.GetDecimal(),
                            HomeTeamSort = "",
                            GuestTeamSort = "",
                            MatchState = (int)state,
                            //MatchId = match == null ? 0 : match.sclassID, //team == null ? 0 : int.Parse(team.Attributes["sid"].Value),
                            //HomeTeamId = homeTeam == null ? 0 : int.Parse(homeTeam.TeamID),  //team == null ? 0 : int.Parse(team.Attributes["htid"].Value),
                            //GuestTeamId = guestTeam == null ? 0 : int.Parse(guestTeam.TeamID),// team == null ? 0 : int.Parse(team.Attributes["gtid"].Value),
                            //Mid = ServiceHelper.GetLeagueCategory(match),//match == null ? 0 : match.team == null ? 0 : team.Attributes["oddsmid"].Value.GetInt32(),
                            FXId = fxId,
                            Hi = team == null ? string.Empty : team.Attributes["htid"].Value,
                            Gi = team == null ? string.Empty : team.Attributes["gtid"].Value,
                            HomeTeamId = zsMatch != null ? zsMatch.HomeTeamId : 0,
                            GuestTeamId = zsMatch != null ? zsMatch.GuestTeamId : 0,
                            Mid = zsMatch != null ? zsMatch.Mid : 0,
                            MatchId = zsMatch != null ? zsMatch.MatchId : 0,
                        });
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(string.Format("解析第{0}期队伍 单条数据 出错{1}", issuseNumber, ex.ToString()));
                    }
                }

                #endregion

            }
            catch (Exception ex)
            {
                this.WriteLog(string.Format("采集第{0}期队伍数据异常：{1}", issuseNumber, ex.ToString()));
            }
            return leagueList;
        }

        public List<C_BJDC_Match> GetBJDCMatchList_GuanWang(string issuseNumber)
        {
            var leagueList = new List<C_BJDC_Match>();

            try
            {
                var zsMathList = GetZS_MatchList(issuseNumber);

                var num = -2;
                while (true)
                {
                    if (num >= 10)
                        break;
                    var date = DateTime.Now.AddDays(num);
                    num++;
                    var url = string.Format("http://www.bjlot.com/ssm/200/html/{0}_{1}_{2}.html", issuseNumber.Substring(1), date.Month.ToString().PadLeft(2, '0'), date.Day.ToString().PadLeft(2, '0'));
                    var encoding = Encoding.GetEncoding("utf-8");
                    var content = PostManager.Get(url, encoding, 0, null);
                    //content = content.Replace("getData(", "").Replace(");", "").Trim();
                    if (string.IsNullOrEmpty(content) || content == "404")
                        continue;
                    var rows = content.Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in rows)
                    {
                        try
                        {
                            var row = item.Trim();
                            if (!row.Contains("id=\"tr_")) continue;
                            var MatchName = string.Empty;
                            var HomeTeamName = string.Empty;
                            var GuestTeamName = string.Empty;
                            var IssuseNumber = string.Empty;
                            var MatchOrderId = 0;
                            var LetBall = 0;
                            var MatchColor = string.Empty;
                            var MatchStartTime = DateTime.Now;
                            var LocalStopTime = DateTime.Now;
                            var WinOdds = 0M;
                            var FlatOdds = 0M;
                            var LoseOdds = 0M;
                            var MatchState = 0;

                            var tds = row.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                            #region 解析每行数据
                            for (int j = 0; j < tds.Length; j++)
                            {
                                var td = tds[j].Trim();
                                if (j == 0)
                                {
                                    //var timeStar = td.IndexOf("title=\"") + "title=\"".Length;
                                    //var timeEnd = td.IndexOf("\" name1=");
                                    //var starDate = DateTime.Parse(td.Substring(timeStar, timeEnd - timeStar));
                                    DateTime starDate = DateTime.Now;
                                    System.Text.RegularExpressions.Match match = Regex.Match(td, "title=\"(?<title>.*?)\" name1=|title=\"(?<title>.*?)\"><td", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                                    if (match.Success)
                                    {
                                        starDate = Convert.ToDateTime(match.Groups["title"].Value);
                                    }
                                    MatchStartTime = starDate.AddMinutes(5);
                                    LocalStopTime = starDate.AddMinutes(LeagueAdvanceMinutes);
                                }
                                var index = td.IndexOf(">");
                                var tdContent = td.Substring(index + 1).Replace("\r\n", string.Empty);
                                var value = CutHtml(tdContent);
                                switch (j)
                                {
                                    case 0:
                                        MatchOrderId = int.Parse(value);
                                        break;
                                    case 1:
                                        if (value == "销售中")
                                            MatchState = 0;
                                        else
                                            MatchState = 1;
                                        break;
                                    case 2:
                                        MatchName = value;
                                        break;
                                    case 4:
                                        HomeTeamName = value;
                                        break;
                                    case 5:
                                        LetBall = int.Parse(value);
                                        break;
                                    case 6:
                                        GuestTeamName = value;
                                        break;
                                    case 7:
                                        var wsp = value;
                                        WinOdds = decimal.Parse(wsp);
                                        break;
                                    case 8:
                                        var fsp = value;
                                        FlatOdds = decimal.Parse(fsp);
                                        break;
                                    case 9:
                                        var lsp = value;
                                        LoseOdds = decimal.Parse(lsp);
                                        break;
                                    default:
                                        break;
                                }
                            }

                            var zsMatch = zsMathList.FirstOrDefault(p => p.IssuseNumber == issuseNumber && p.MatchOrderId == MatchOrderId);

                            leagueList.Add(new C_BJDC_Match
                            {
                                Id = string.Format("{0}|{1}", issuseNumber, MatchOrderId),
                                MatchName = zsMatch != null ? zsMatch.MatchName : MatchName,
                                HomeTeamName = HomeTeamName,
                                GuestTeamName = GuestTeamName,
                                IssuseNumber = issuseNumber,
                                MatchOrderId = MatchOrderId,
                                CreateTime = DateTime.Now,
                                LetBall = LetBall,
                                MatchColor = zsMatch != null ? zsMatch.MatchColor : "",
                                MatchStartTime = MatchStartTime,
                                LocalStopTime = LocalStopTime,
                                WinOdds = WinOdds,
                                FlatOdds = FlatOdds,
                                LoseOdds = LoseOdds,
                                HomeTeamSort = "",
                                GuestTeamSort = "",
                                MatchState = MatchState,
                                HomeTeamId = zsMatch != null ? zsMatch.HomeTeamId : 0,
                                GuestTeamId = zsMatch != null ? zsMatch.GuestTeamId : 0,
                                Mid = zsMatch != null ? zsMatch.Mid : 0,
                                MatchId = zsMatch != null ? zsMatch.MatchId : 0,
                            });


                            #endregion
                        }
                        catch (Exception ex)
                        {
                            this.WriteLog("解析表格数据失败：" + ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.WriteLog(string.Format("采集第{0}期队伍数据异常：{1}", issuseNumber, ex.ToString()));
            }
            return leagueList;
        }


        public List<C_BJDC_Match> GetBJDCMatchList_500wan(string issuseNumber)
        {
            var leagueList = new List<C_BJDC_Match>();

            try
            {
                var url = "http://trade.500.com/bjdc/?expect=" + issuseNumber;
                var content = PostManager.Get(url, Encoding.Default, 0, (request) =>
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
                if (string.IsNullOrEmpty(content) || content == "404")
                    return leagueList;
                var index = content.IndexOf("id=\"vs_table\"");
                content = content.Substring(index);
                index = content.IndexOf("</table>");
                content = content.Substring(0, index);

                var rows = content.Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var item in rows)
                {
                    try
                    {
                        var row = item.Trim();
                        if (!row.Contains("class=\"vs_lines")) continue;

                        index = row.IndexOf("value=\"");
                        row = row.Substring(index + "value=\"".Length);
                        index = row.IndexOf("\" fid=\"");
                        row = row.Substring(0, index).Replace("'", "\"");
                        row = string.Format("[{0}]", row).Replace("homeTeamRank", "&&&").Replace("guestTeamRank", "^^^").Replace("index", "\"index\"").Replace("leagueName", "\"leagueName\"").Replace("homeTeam", "\"homeTeam\"").Replace("guestTeam", "\"guestTeam\"").Replace("endTime", "\"endTime\"")
                            .Replace("rangqiuNum", "\"rangqiuNum\"").Replace("scheduleDate", "\"scheduleDate\"").Replace("disabled", "\"disabled\"").Replace("&&&", "\"homeTeamRank\"").Replace("^^^", "\"guestTeamRank\"").Replace("bgColor", "\"bgColor\"");

                        var entity_500Wan = string.IsNullOrEmpty(row) ? new List<BJDC_500Wan>() : JsonHelper.Deserialize<List<BJDC_500Wan>>(row);
                        if (entity_500Wan.Count > 0)
                        {
                            index = item.IndexOf("<span class=\"sp_w35 eng pjoz\">");
                            var oddsList = item.Substring(index);
                            index = oddsList.IndexOf("<span class=\"sp_w35 eng tzbl\">");
                            oddsList = oddsList.Substring(0, index);

                            var value = CutHtml(oddsList);

                            var date = DateTime.Parse(entity_500Wan[0].endTime);
                            leagueList.Add(new C_BJDC_Match
                            {
                                Id = string.Format("{0}|{1}", issuseNumber, entity_500Wan[0].index),
                                MatchName = entity_500Wan[0].leagueName,
                                HomeTeamName = entity_500Wan[0].homeTeam,
                                GuestTeamName = entity_500Wan[0].guestTeam,
                                IssuseNumber = issuseNumber,
                                MatchOrderId = int.Parse(entity_500Wan[0].index),
                                CreateTime = DateTime.Now,
                                LetBall = int.Parse(entity_500Wan[0].rangqiuNum),
                                MatchColor = entity_500Wan[0].bgColor,
                                MatchStartTime = date.AddMinutes(10),
                                LocalStopTime = date.AddMinutes(LeagueAdvanceMinutes),
                                WinOdds = decimal.Parse(value.Substring(0, 4)),
                                FlatOdds = decimal.Parse(value.Substring(4, 4)),
                                LoseOdds = decimal.Parse(value.Substring(8)),
                                HomeTeamSort = "",
                                GuestTeamSort = "",
                                MatchState = 0,
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog("解析表格数据失败：" + ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                this.WriteLog(string.Format("采集第{0}期队伍数据异常：{1}", issuseNumber, ex.ToString()));
            }
            return leagueList;
        }

        public List<C_BJDC_MatchResult> GetBJDCMatchResultList(string issuseNumber)
        {
            var leagueResultList = new List<C_BJDC_MatchResult>();
            //比赛结果 ,从9188采集
            var resultUrl = string.Format("http://www.9188.com/data/bd/{0}/{0}.xml", issuseNumber);
            var resultXmlContent = PostManager.Get(resultUrl, Encoding.UTF8, 0, (request) =>
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
            if (string.IsNullOrEmpty(resultXmlContent))
                return leagueResultList;
            var resultDoc = new XmlDocument();
            resultDoc.LoadXml(resultXmlContent);
            var resultResp = resultDoc.SelectSingleNode("Resp");
            #region 比赛结果

            foreach (XmlNode item in resultResp)
            {
                var homeHalf_Result = item.Attributes["hms"].Value;
                var homeFull_Result = item.Attributes["ms"].Value;
                var guestHalf_Result = item.Attributes["hss"].Value;
                var guestFull_Result = item.Attributes["ss"].Value;

                if (string.IsNullOrEmpty(homeHalf_Result) && string.IsNullOrEmpty(homeFull_Result) && string.IsNullOrEmpty(guestHalf_Result) && string.IsNullOrEmpty(guestFull_Result))
                    continue;

                //"Finish" 完场，"Late" 延迟，"Cancel" 取消
                //todo  取消状态
                if (homeHalf_Result == "-1" && homeFull_Result == "-1" && guestHalf_Result == "-1" && guestFull_Result == "-1")
                {
                    leagueResultList.Add(new C_BJDC_MatchResult
                    {
                        Id = string.Format("{0}|{1}", issuseNumber, item.Attributes["mid"].Value),
                        CreateTime = DateTime.Now,
                        IssuseNumber = issuseNumber,
                        MatchOrderId = item.Attributes["mid"].Value.GetInt32(),
                        HomeHalf_Result = homeHalf_Result,
                        HomeFull_Result = homeFull_Result,
                        GuestHalf_Result = guestHalf_Result,
                        GuestFull_Result = guestFull_Result,
                        SPF_Result = "-1",
                        SPF_SP = 1.00M,
                        ZJQ_Result = "-1",
                        ZJQ_SP = 1.00M,
                        SXDS_Result = "-1",
                        SXDS_SP = 1.00M,
                        BF_Result = "-1",
                        BF_SP = 1.00M,
                        BQC_Result = "-1",
                        BQC_SP = 1.00M,
                        MatchState = "Cancel",
                    });
                    continue;
                }

                var spf_result = string.Empty;
                var spf_sp = 1M;
                var bf_result = string.Empty;
                var bf_sp = 1M;
                var bqc_result = string.Empty;
                var bqc_sp = 1M;
                var zjq_result = string.Empty;
                var zjq_sp = 1M;
                var sxds_result = string.Empty;
                var sxds_sp = 1M;
                //rs="3:3.866892;90:16.394782;33:1.907655;2:4.942275;6:33.069284"
                var rs = item.Attributes["rs"].Value;
                var rsArray = rs.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                if (rsArray.Length == 5)
                {
                    var spfArray = rsArray[0].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    spf_result = spfArray[0];
                    spf_sp = spfArray[1].GetDecimal();

                    var bfArray = rsArray[1].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    bf_sp = bfArray[1].GetDecimal();
                    switch (bfArray[0])
                    {
                        case "90":
                            bf_result = "X0";
                            break;
                        case "99":
                            bf_result = "XX";
                            break;
                        case "09":
                            bf_result = "0X";
                            break;
                        default:
                            bf_result = bfArray[0];
                            break;
                    }

                    var bqcArray = rsArray[2].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    bqc_result = bqcArray[0];
                    bqc_sp = bqcArray[1].GetDecimal();

                    var zjqArray = rsArray[4].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    zjq_sp = zjqArray[1].GetDecimal();
                    zjq_result = zjqArray[0];

                    var sum = int.Parse(homeFull_Result) + int.Parse(guestFull_Result);//zjqArray[0].GetInt32();
                    var sxdsArray = rsArray[3].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    sxds_sp = sxdsArray[1].GetDecimal();
                    sxds_result = sum >= 3 ? "S" : "X";
                    sxds_result += sum % 2 == 0 ? "S" : "D";
                }

                leagueResultList.Add(new C_BJDC_MatchResult
                {
                    Id = string.Format("{0}|{1}", issuseNumber, item.Attributes["mid"].Value.GetInt32()),
                    CreateTime = DateTime.Now,
                    IssuseNumber = issuseNumber,
                    MatchOrderId = item.Attributes["mid"].Value.GetInt32(),
                    HomeHalf_Result = homeHalf_Result,
                    HomeFull_Result = homeFull_Result,
                    GuestHalf_Result = guestHalf_Result,
                    GuestFull_Result = guestFull_Result,
                    SPF_Result = spf_result,
                    SPF_SP = spf_sp,
                    ZJQ_Result = zjq_result,
                    ZJQ_SP = zjq_sp,
                    SXDS_Result = sxds_result,
                    SXDS_SP = sxds_sp,
                    BF_Result = bf_result,
                    BF_SP = bf_sp,
                    BQC_Result = bqc_result,
                    BQC_SP = bqc_sp,
                    MatchState = "Finish",
                });
            }

            #endregion

            return leagueResultList;
        }

        /// <summary>
        /// 北京单场胜负过关比赛信息
        /// </summary>
        public List<BJDC_Match_SFGG> GetBJDC_SFGGMatchList_CPDJY(string issuseNumber)
        {
            var matchList = new List<BJDC_Match_SFGG>();

            //var fxDic = ServiceHelper.GetFXIdSource() == "okooo" ? GetBJDC_FX_OKOOO(issuseNumber) : GetBJDC_FX(issuseNumber);

            System.DateTime t = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long tt = (DateTime.Now.Ticks - t.Ticks) / 10000;
            var url = string.Format("http://intf2.cpdyj.com/data/dcsf/match/{1}.js?callback=game&_={0}", tt, issuseNumber);
            var encoding = Encoding.GetEncoding("gb2312");
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
            var content = PostManager.Get(url, encoding, 0, null);
            content = content.Replace("game(", "").Replace(");", "").Trim();
            if (string.IsNullOrEmpty(content) || content == "404")
                return matchList;
            var array = JsonHelper.Decode(content);
            var ceshi = string.Empty;
            foreach (var matchArray in array)
            {
                foreach (var matchItem in matchArray)
                {
                    //["141204","200","14-15美职3","篮球","俄克拉荷马城雷霆","夏洛特黄蜂","-8.5","-1","-1","1.70","2.42","-1.000000","2014-12-27 06:05:00","2014-12-27 05:00:00","0","2014-12-26"]
                    try
                    {
                        //var orderNum = matchItem[0];
                        var matchOrderId = int.Parse(matchItem[1]);
                        var matchName = matchItem[2].IndexOf("-") == -1 ? matchItem[2] : matchItem[2].Substring(5);
                        var category = matchItem[3];
                        var homeTeamName = matchItem[4];
                        var guestTeamName = matchItem[5];
                        var letBall = decimal.Parse(matchItem[6]);
                        var winOdds = decimal.Parse(matchItem[9]);
                        var loseOdds = decimal.Parse(matchItem[10]);
                        var matchState = int.Parse(matchItem[14]);
                        var betStopTime = DateTime.Parse(matchItem[13]);

                        matchList.Add(new BJDC_Match_SFGG
                        {
                            MatchOrderId = matchOrderId,
                            Id = string.Format("{0}|{1}", issuseNumber, matchOrderId),
                            MatchName = matchName.Replace("15", ""),
                            HomeTeamName = homeTeamName,
                            GuestTeamName = guestTeamName,
                            IssuseNumber = issuseNumber,
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            LetBall = letBall,
                            WinOdds = winOdds,
                            LoseOdds = loseOdds,
                            MatchState = matchState,
                            BetStopTime = betStopTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            Category = category
                        });
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(string.Format("解析胜负过关单条队伍信息异常：{0}_{1}", ex.ToString(), ceshi));
                    }
                }
            }

            return matchList;
        }

        /// <summary>
        /// 北京单场胜负过关比赛信息
        /// </summary>
        public List<BJDC_Match_SFGG> GetBJDC_SFGGMatchList_9188(string issuseNumber)
        {
            var matchList = new List<BJDC_Match_SFGG>();

            //var fxDic = ServiceHelper.GetFXIdSource() == "okooo" ? GetBJDC_FX_OKOOO(issuseNumber) : GetBJDC_FX(issuseNumber);

            System.DateTime t = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long tt = (DateTime.Now.Ticks - t.Ticks) / 10000;
            var url = string.Format("http://www.9188.com/data/bd/sfgg/{1}/sfgg.xml?rnd={0}", tt, issuseNumber.Substring(1));
            var xmlContent = PostManager.Get(url, Encoding.UTF8, 0, (request) =>
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
            if (string.IsNullOrEmpty(xmlContent))
                return matchList;

            var docBD = new XmlDocument();
            docBD.LoadXml(xmlContent);
            var matchRoot = docBD.SelectSingleNode("Resp");

            foreach (XmlNode item in matchRoot.ChildNodes)
            {
                //<row expect="50903" mid="1" hn="关岛" gn="阿曼" bt="2015-09-08 14:00:00" et="2015-09-08 13:50:00" b3="" b1="" b0="" ms="" ss="" rs="" close="2.5" mname="世亚预" cl="#993333" iaudit="0" icancel="0" isale="0" csf="rz:;rs:;sp3:1.56;sp0:2.76;" istatus="1" ccup="足球"/>
                //["141204","200","14-15美职3","篮球","俄克拉荷马城雷霆","夏洛特黄蜂","-8.5","-1","-1","1.70","2.42","-1.000000","2014-12-27 06:05:00","2014-12-27 05:00:00","0","2014-12-26"]
                try
                {

                    var matchOrderId = item.Attributes["mid"].Value.GetInt32();
                    var matchName = item.Attributes["mname"].Value;
                    var category = item.Attributes["ccup"].Value;
                    var homeTeamName = item.Attributes["hn"].Value;
                    var guestTeamName = item.Attributes["gn"].Value;
                    var letBall = decimal.Parse(item.Attributes["close"].Value);
                    //csf="rz:;rs:;sp3:1.56;sp0:2.76;"
                    var sp = item.Attributes["csf"].Value;
                    var s = sp.IndexOf(";s");
                    sp = sp.Substring(s);
                    var strL = sp.Replace(";sp3:", "").Replace("sp0:", "").Replace(";sf3:", "").Replace("sf0:", "").Split(';');
                    //var sp = item.Attributes["csf"].Value.Replace("rz:;rs:;sp3:", "").Replace("sp0:", "").Split(';');
                    var winOdds = decimal.Parse(strL[0]);
                    var loseOdds = decimal.Parse(strL[1]);
                    var matchState = int.Parse(item.Attributes["istatus"].Value);
                    var betStopTime = DateTime.Parse(item.Attributes["et"].Value);

                    matchList.Add(new BJDC_Match_SFGG
                    {
                        MatchOrderId = matchOrderId,
                        Id = string.Format("{0}|{1}", issuseNumber, matchOrderId),
                        MatchName = matchName.Replace("15", ""),
                        HomeTeamName = homeTeamName,
                        GuestTeamName = guestTeamName,
                        IssuseNumber = issuseNumber,
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        LetBall = letBall,
                        WinOdds = winOdds,
                        LoseOdds = loseOdds,
                        MatchState = matchState,
                        BetStopTime = betStopTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        Category = category
                    });
                }
                catch (Exception ex)
                {
                    this.WriteLog(string.Format("解析胜负过关单条队伍信息异常：{0}_{1}", ex.ToString()));
                }
            }

            return matchList;
        }

        #endregion

        #region 解析胜平负 SP

        //private void Write_SPF_SP_Trend_JSON(BJDC_SPF_SpInfoCollection collection)
        //{
        //    foreach (var item in collection)
        //    {
        //        try
        //        {
        //            var fileName = string.Format("SPF_SP_Trend_{0}.json", item.MatchOrderId);
        //            var fileFullName = BuildFileFullName(fileName, item.IssuseNumber);
        //            if (File.Exists(fileFullName))
        //            {
        //                //停止的比赛，不更新sp走势
        //                var league = CurrentMatchList.FirstOrDefault(p => p.IssuseNumber == item.IssuseNumber && p.MatchOrderId == item.MatchOrderId);
        //                if (league == null || league.MatchState == (int)BJDCMatchState.Stop)
        //                    continue;

        //                var oldJson = File.ReadAllText(fileFullName).Trim().Replace("var data=", "").Replace("];", "]");
        //                var oldList = string.IsNullOrEmpty(oldJson) ? new List<BJDC_SPF_SpInfo>() : JsonHelper.Deserialize<List<BJDC_SPF_SpInfo>>(oldJson);
        //                oldList.Add(item);
        //                ServiceHelper.CreateOrAppend_JSONFile(fileFullName, JsonHelper.Serialize(oldList), (log) =>
        //                {
        //                    this.WriteLog(log);
        //                });

        //            }
        //            else
        //            {
        //                var list = new List<BJDC_SPF_SpInfo>();
        //                list.Add(item);
        //                ServiceHelper.CreateOrAppend_JSONFile(fileFullName, JsonHelper.Serialize(list), (log) =>
        //                {
        //                    this.WriteLog(log);
        //                });

        //            }


        //            var customerSavePath = new string[] { "BJDC", item.IssuseNumber };
        //            //上传文件
        //            ServiceHelper.PostFileToServer(fileFullName, customerSavePath, (log) =>
        //            {
        //                this.WriteLog(log);
        //            });
        //        }
        //        catch (Exception ex)
        //        {
        //            this.WriteLog(string.Format("写入第{0}期，赛事{1} SPF SP值走势失败:{2}", item.IssuseNumber, item.MatchOrderId, ex.ToString()));
        //            continue;
        //        }
        //    }
        //}

        //private BJDC_SPF_SpInfoCollection Save_SPF_SP_And_GetDiffer(string fileFullName, BJDC_SPF_SpInfoCollection currentSp)
        //{
        //    if (currentSp.Count == 0)
        //        return currentSp;

        //    if (File.Exists(fileFullName))
        //    {
        //        try
        //        {
        //            var oldJson = File.ReadAllText(fileFullName).Trim().Replace("var data=", "").Replace("];", "]");
        //            var oldList = string.IsNullOrEmpty(oldJson) ? new List<BJDC_SPF_SpInfo>() : JsonHelper.Deserialize<List<BJDC_SPF_SpInfo>>(oldJson);
        //            ServiceHelper.CreateOrAppend_JSONFile(fileFullName, JsonHelper.Serialize(currentSp), (log) =>
        //            {
        //                this.WriteLog(log);
        //            });


        //            return GetNew_SPF_SPList(oldList, currentSp);
        //        }
        //        catch (Exception ex)
        //        {
        //            this.WriteLog("写入SP值JSON文件失败：" + ex.ToString());
        //            return currentSp;
        //        }
        //    }

        //    try
        //    {
        //        ServiceHelper.CreateOrAppend_JSONFile(fileFullName, JsonSerializer.Serialize(currentSp), (log) =>
        //        {
        //            this.WriteLog(log);
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        this.WriteLog("第一次写入SP值JSON文件失败：" + ex.ToString());
        //    }
        //    return currentSp;
        //}

        private List<BJDC_SPF_SpInfo> GetNew_SPF_SPList(List<BJDC_SPF_SpInfo> oldSP, List<BJDC_SPF_SpInfo> newSp)
        {
            var list = new List<BJDC_SPF_SpInfo>();
            foreach (var item in newSp)
            {
                var old = oldSP.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
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

        private List<BJDC_SPF_SpInfo> LoadSPF_SPList(XmlNode root, string issuseNumber)
        {
            var collection = new List<BJDC_SPF_SpInfo>();
            if (root.ChildNodes.Count == 0) return collection;
            foreach (XmlNode item in root.ChildNodes)
            {
                collection.Add(new BJDC_SPF_SpInfo
                {
                    IssuseNumber = issuseNumber,
                    MatchOrderId = item.Name.Replace("w", "").GetInt32(),
                    //LetBall = item.Attributes["r"].Value.GetInt32(),
                    Win_Odds = item.Attributes["c1"].Value.GetDecimal(),
                    Flat_Odds = item.Attributes["c3"].Value.GetDecimal(),
                    Lose_Odds = item.Attributes["c5"].Value.GetDecimal(),
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                });
            }
            return collection;
        }

        #endregion

        #region 解析总进球
      
        private void Write_ZJQ_SP_Trend_JSON(List<BJDC_ZJQ_SpInfo> collection)
        {
            var coll = mDB.GetCollection<BJDC_ZJQ_SpInfo>("BJDC_ZJQ_SpInfo");
            foreach (var item in collection)
            {
                try
                {
                    var mFilter = MongoDB.Driver.Builders<BJDC_ZJQ_SpInfo>.Filter.Eq(b => b.IssuseNumber, item.IssuseNumber) 
                        & Builders<BJDC_ZJQ_SpInfo>.Filter.Eq(b => b.MatchOrderId, item.MatchOrderId);
                    //   ServiceHelper.BuildList_GN(mDB, "BJDC_ZJQ_SpInfo", collection, null, mFilter);

                    //var fileName = string.Format("ZJQ_SP_Trend_{0}.json", item.MatchOrderId);
                    //var fileFullName = BuildFileFullName(fileName, item.IssuseNumber);
                    var league = CurrentMatchList.FirstOrDefault(p => p.IssuseNumber == item.IssuseNumber && p.MatchOrderId == item.MatchOrderId);
                    if (league == null || league.MatchState == (int)BJDCMatchState.Stop) {

                    }
                    else
                    {
                       var dlist= coll.Find<BJDC_ZJQ_SpInfo>(mFilter).ToList();
                        coll.InsertOne(item);
                    }
                       

                   
                    //if (File.Exists(fileFullName))
                    //{
                    //    //停止的比赛，不更新sp走势
                    //    var league = CurrentMatchList.FirstOrDefault(p => p.IssuseNumber == item.IssuseNumber && p.MatchOrderId == item.MatchOrderId);
                    //    if (league == null || league.MatchState == (int)BJDCMatchState.Stop)
                    //        continue;
                    //    var oldJson = File.ReadAllText(fileFullName).Trim().Replace("var data=", "").Replace("];", "]");
                    //    var oldList = string.IsNullOrEmpty(oldJson) ? new List<BJDC_ZJQ_SpInfo>() : JsonSerializer.Deserialize<List<BJDC_ZJQ_SpInfo>>(oldJson);
                    //    oldList.Add(item);
                    //    ServiceHelper.CreateOrAppend_JSONFile(fileFullName, JsonSerializer.Serialize(oldList), (log) =>
                    //    {
                    //        this.WriteLog(log);
                    //    });
                    //}
                    //else
                    //{
                    //    var list = new List<BJDC_ZJQ_SpInfo>();
                    //    list.Add(item);
                    //    ServiceHelper.CreateOrAppend_JSONFile(fileFullName, JsonSerializer.Serialize(list), (log) =>
                    //    {
                    //        this.WriteLog(log);
                    //    });
                    //}

                    //var customerSavePath = new string[] { "BJDC", item.IssuseNumber };
                    ////上传文件
                    //ServiceHelper.PostFileToServer(fileFullName, customerSavePath, (log) =>
                    //{
                    //    this.WriteLog(log);
                    //});

                }
                catch (Exception ex)
                {
                    this.WriteLog(string.Format("写入第{0}期，赛事{1} SP值走势失败:{2}", item.IssuseNumber, item.MatchOrderId, ex.ToString()));
                    continue;
                }
            }
        }

        //private List<BJDC_ZJQ_SpInfo> Save_ZJQ_SP_And_GetDiffer(string fileFullName, List<BJDC_ZJQ_SpInfo> currentSp)
        //{
        //    if (currentSp.Count == 0)
        //        return currentSp;

        //    if (File.Exists(fileFullName))
        //    {
        //        try
        //        {
        //            var oldJson = File.ReadAllText(fileFullName).Trim().Replace("var data=", "").Replace("];", "]");
        //            var oldList = string.IsNullOrEmpty(oldJson) ? new List<BJDC_ZJQ_SpInfo>() : JsonSerializer.Deserialize<List<BJDC_ZJQ_SpInfo>>(oldJson);
        //            ServiceHelper.CreateOrAppend_JSONFile(fileFullName, JsonSerializer.Serialize(currentSp), (log) =>
        //            {
        //                this.WriteLog(log);
        //            });
        //            return GetNew_ZJQ_SPList(oldList, currentSp);
        //        }
        //        catch (Exception ex)
        //        {
        //            this.WriteLog("写入SP值JSON文件失败：" + ex.ToString());
        //            return currentSp;
        //        }
        //    }

        //    try
        //    {
        //        ServiceHelper.CreateOrAppend_JSONFile(fileFullName, JsonSerializer.Serialize(currentSp), (log) =>
        //        {
        //            this.WriteLog(log);
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        this.WriteLog("第一次写入SP值JSON文件失败：" + ex.ToString());
        //    }
        //    return currentSp;
        //}

        private List<BJDC_ZJQ_SpInfo> GetNew_ZJQ_SPList(List<BJDC_ZJQ_SpInfo> oldSP, List<BJDC_ZJQ_SpInfo> newSp)
        {
            var list = new List<BJDC_ZJQ_SpInfo>();
            foreach (var item in newSp)
            {
                var old = oldSP.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
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

        private List<BJDC_ZJQ_SpInfo> LoadZJQ_SPList(XmlNode root, string issuseNumber)
        {
            var collection = new List<BJDC_ZJQ_SpInfo>();
            if (root.ChildNodes.Count == 0) return collection;
            foreach (XmlNode item in root.ChildNodes)
            {
                collection.Add(new BJDC_ZJQ_SpInfo
                {
                    IssuseNumber = issuseNumber,
                    MatchOrderId = item.Name.Replace("w", "").GetInt32(),
                    //LetBall = item.Attributes["r"].Value.GetInt32(),
                    JinQiu_0_Odds = item.Attributes["c1"].Value.GetDecimal(),
                    JinQiu_1_Odds = item.Attributes["c3"].Value.GetDecimal(),
                    JinQiu_2_Odds = item.Attributes["c5"].Value.GetDecimal(),
                    JinQiu_3_Odds = item.Attributes["c7"].Value.GetDecimal(),
                    JinQiu_4_Odds = item.Attributes["c9"].Value.GetDecimal(),
                    JinQiu_5_Odds = item.Attributes["c11"].Value.GetDecimal(),
                    JinQiu_6_Odds = item.Attributes["c13"].Value.GetDecimal(),
                    JinQiu_7_Odds = item.Attributes["c15"].Value.GetDecimal(),
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                });
            }
            return collection;
        }

        #endregion

        #region 解析上下单双

        //private void Write_SXDS_SP_Trend_JSON(BJDC_SXDS_SpInfoCollection collection)
        //{
        //    foreach (var item in collection)
        //    {
        //        try
        //        {
        //            var fileName = string.Format("SXDS_SP_Trend_{0}.json", item.MatchOrderId);
        //            var fileFullName = BuildFileFullName(fileName, item.IssuseNumber);
        //            if (File.Exists(fileFullName))
        //            {
        //                //停止的比赛，不更新sp走势
        //                var league = CurrentMatchList.FirstOrDefault(p => p.IssuseNumber == item.IssuseNumber && p.MatchOrderId == item.MatchOrderId);
        //                if (league == null || league.MatchState == (int)BJDCMatchState.Stop)
        //                    continue;
        //                var oldJson = File.ReadAllText(fileFullName).Trim().Replace("var data=", "").Replace("];", "]");
        //                var oldList = string.IsNullOrEmpty(oldJson) ? new List<BJDC_SXDS_SpInfo>() : JsonSerializer.Deserialize<List<BJDC_SXDS_SpInfo>>(oldJson);
        //                oldList.Add(item);
        //                ServiceHelper.CreateOrAppend_JSONFile(fileFullName, JsonSerializer.Serialize(oldList), (log) =>
        //                {
        //                    this.WriteLog(log);
        //                });
        //            }
        //            else
        //            {
        //                var list = new List<BJDC_SXDS_SpInfo>();
        //                list.Add(item);
        //                ServiceHelper.CreateOrAppend_JSONFile(fileFullName, JsonSerializer.Serialize(list), (log) =>
        //                {
        //                    this.WriteLog(log);
        //                });
        //            }

        //            var customerSavePath = new string[] { "BJDC", item.IssuseNumber };
        //            //上传文件
        //            ServiceHelper.PostFileToServer(fileFullName, customerSavePath, (log) =>
        //            {
        //                this.WriteLog(log);
        //            });
        //        }
        //        catch (Exception ex)
        //        {
        //            this.WriteLog(string.Format("写入第{0}期，赛事{1} SP值走势失败:{2}", item.IssuseNumber, item.MatchOrderId, ex.ToString()));
        //            continue;
        //        }
        //    }
        //}

        //private BJDC_SXDS_SpInfoCollection Save_SXDS_SP_And_GetDiffer(string fileFullName, BJDC_SXDS_SpInfoCollection currentSp)
        //{
        //    if (currentSp.Count == 0)
        //        return currentSp;

        //    if (File.Exists(fileFullName))
        //    {
        //        try
        //        {
        //            var oldJson = File.ReadAllText(fileFullName).Trim().Replace("var data=", "").Replace("];", "]");
        //            var oldList = string.IsNullOrEmpty(oldJson) ? new List<BJDC_SXDS_SpInfo>() : JsonSerializer.Deserialize<List<BJDC_SXDS_SpInfo>>(oldJson);
        //            ServiceHelper.CreateOrAppend_JSONFile(fileFullName, JsonSerializer.Serialize(currentSp), (log) =>
        //            {
        //                this.WriteLog(log);
        //            });
        //            return GetNew_SXDS_SPList(oldList, currentSp);
        //        }
        //        catch (Exception ex)
        //        {
        //            this.WriteLog("写入SP值JSON文件失败：" + ex.ToString());
        //            return currentSp;
        //        }
        //    }

        //    try
        //    {
        //        ServiceHelper.CreateOrAppend_JSONFile(fileFullName, JsonSerializer.Serialize(currentSp), (log) =>
        //        {
        //            this.WriteLog(log);
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        this.WriteLog("第一次写入SP值JSON文件失败：" + ex.ToString());
        //    }
        //    return currentSp;
        //}

        private List<BJDC_SXDS_SpInfo> GetNew_SXDS_SPList(List<BJDC_SXDS_SpInfo> oldSP, List<BJDC_SXDS_SpInfo> newSp)
        {
            var list = new List<BJDC_SXDS_SpInfo>();
            foreach (var item in newSp)
            {
                var old = oldSP.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
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

        private List<BJDC_SXDS_SpInfo> LoadSXDS_SPList(XmlNode root, string issuseNumber)
        {
            var collection = new List<BJDC_SXDS_SpInfo>();
            if (root.ChildNodes.Count == 0) return collection;
            foreach (XmlNode item in root.ChildNodes)
            {
                collection.Add(new BJDC_SXDS_SpInfo
                {
                    IssuseNumber = issuseNumber,
                    MatchOrderId = item.Name.Replace("w", "").GetInt32(),
                    //LetBall = item.Attributes["r"].Value.GetInt32(),
                    SH_D_Odds = item.Attributes["c1"].Value.GetDecimal(),
                    SH_S_Odds = item.Attributes["c3"].Value.GetDecimal(),
                    X_D_Odds = item.Attributes["c5"].Value.GetDecimal(),
                    X_S_Odds = item.Attributes["c7"].Value.GetDecimal(),
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                });
            }
            return collection;
        }

        #endregion

        #region 解析比分

        //private void Write_BF_SP_Trend_JSON(BJDC_BF_SpInfoCollection collection)
        //{
        //    foreach (var item in collection)
        //    {
        //        try
        //        {
        //            var fileName = string.Format("BF_SP_Trend_{0}.json", item.MatchOrderId);
        //            var fileFullName = BuildFileFullName(fileName, item.IssuseNumber);
        //            if (File.Exists(fileFullName))
        //            {
        //                //停止的比赛，不更新sp走势
        //                var league = CurrentMatchList.FirstOrDefault(p => p.IssuseNumber == item.IssuseNumber && p.MatchOrderId == item.MatchOrderId);
        //                if (league == null || league.MatchState == (int)BJDCMatchState.Stop)
        //                    continue;
        //                var oldJson = File.ReadAllText(fileFullName).Trim().Replace("var data=", "").Replace("];", "]");
        //                var oldList = string.IsNullOrEmpty(oldJson) ? new List<BJDC_BF_SpInfo>() : JsonSerializer.Deserialize<List<BJDC_BF_SpInfo>>(oldJson);
        //                oldList.Add(item);
        //                ServiceHelper.CreateOrAppend_JSONFile(fileFullName, JsonSerializer.Serialize(oldList), (log) =>
        //                {
        //                    this.WriteLog(log);
        //                });
        //            }
        //            else
        //            {
        //                var list = new List<BJDC_BF_SpInfo>();
        //                list.Add(item);
        //                ServiceHelper.CreateOrAppend_JSONFile(fileFullName, JsonSerializer.Serialize(list), (log) =>
        //                {
        //                    this.WriteLog(log);
        //                });
        //            }

        //            var customerSavePath = new string[] { "BJDC", item.IssuseNumber };
        //            //上传文件
        //            ServiceHelper.PostFileToServer(fileFullName, customerSavePath, (log) =>
        //            {
        //                this.WriteLog(log);
        //            });
        //        }
        //        catch (Exception ex)
        //        {
        //            this.WriteLog(string.Format("写入第{0}期，赛事{1} SP值走势失败:{2}", item.IssuseNumber, item.MatchOrderId, ex.ToString()));
        //            continue;
        //        }
        //    }
        //}

        //private BJDC_BF_SpInfoCollection Save_BF_SP_And_GetDiffer(string fileFullName, BJDC_BF_SpInfoCollection currentSp)
        //{
        //    if (currentSp.Count == 0)
        //        return currentSp;

        //    if (File.Exists(fileFullName))
        //    {
        //        try
        //        {
        //            var oldJson = File.ReadAllText(fileFullName).Trim().Replace("var data=", "").Replace("];", "]");
        //            var oldList = string.IsNullOrEmpty(oldJson) ? new List<BJDC_BF_SpInfo>() : JsonSerializer.Deserialize<List<BJDC_BF_SpInfo>>(oldJson);
        //            ServiceHelper.CreateOrAppend_JSONFile(fileFullName, JsonSerializer.Serialize(currentSp), (log) =>
        //            {
        //                this.WriteLog(log);
        //            });
        //            return GetNew_BF_SPList(oldList, currentSp);
        //        }
        //        catch (Exception ex)
        //        {
        //            this.WriteLog("写入SP值JSON文件失败：" + ex.ToString());
        //            return currentSp;
        //        }
        //    }

        //    try
        //    {
        //        ServiceHelper.CreateOrAppend_JSONFile(fileFullName, JsonSerializer.Serialize(currentSp), (log) =>
        //        {
        //            this.WriteLog(log);
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        this.WriteLog("第一次写入SP值JSON文件失败：" + ex.ToString());
        //    }
        //    return currentSp;
        //}

        private List<BJDC_BF_SpInfo> GetNew_BF_SPList(List<BJDC_BF_SpInfo> oldSP, List<BJDC_BF_SpInfo> newSp)
        {
            var list = new List<BJDC_BF_SpInfo>();
            foreach (var item in newSp)
            {
                var old = oldSP.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
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

        private List<BJDC_BF_SpInfo> LoadBF_SPList(XmlNode root, string issuseNumber)
        {
            var collection = new List<BJDC_BF_SpInfo>();
            if (root.ChildNodes.Count == 0) return collection;
            foreach (XmlNode item in root.ChildNodes)
            {
                collection.Add(new BJDC_BF_SpInfo
                {
                    IssuseNumber = issuseNumber,
                    MatchOrderId = item.Name.Replace("w", "").GetInt32(),
                    //LetBall = item.Attributes["r"].Value.GetInt32(),
                    S_QT = item.Attributes["c1"].Value.GetDecimal(),
                    S_10 = item.Attributes["c2"].Value.GetDecimal(),
                    S_20 = item.Attributes["c3"].Value.GetDecimal(),
                    S_21 = item.Attributes["c4"].Value.GetDecimal(),
                    S_30 = item.Attributes["c5"].Value.GetDecimal(),
                    S_31 = item.Attributes["c6"].Value.GetDecimal(),
                    S_32 = item.Attributes["c7"].Value.GetDecimal(),
                    S_40 = item.Attributes["c8"].Value.GetDecimal(),
                    S_41 = item.Attributes["c9"].Value.GetDecimal(),
                    S_42 = item.Attributes["c10"].Value.GetDecimal(),
                    P_QT = item.Attributes["c11"].Value.GetDecimal(),
                    P_00 = item.Attributes["c12"].Value.GetDecimal(),
                    P_11 = item.Attributes["c13"].Value.GetDecimal(),
                    P_22 = item.Attributes["c14"].Value.GetDecimal(),
                    P_33 = item.Attributes["c15"].Value.GetDecimal(),
                    F_QT = item.Attributes["c16"].Value.GetDecimal(),
                    F_01 = item.Attributes["c17"].Value.GetDecimal(),
                    F_02 = item.Attributes["c18"].Value.GetDecimal(),
                    F_12 = item.Attributes["c19"].Value.GetDecimal(),
                    F_03 = item.Attributes["c20"].Value.GetDecimal(),
                    F_13 = item.Attributes["c21"].Value.GetDecimal(),
                    F_23 = item.Attributes["c22"].Value.GetDecimal(),
                    F_04 = item.Attributes["c23"].Value.GetDecimal(),
                    F_14 = item.Attributes["c24"].Value.GetDecimal(),
                    F_24 = item.Attributes["c25"].Value.GetDecimal(),
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                });
            }
            return collection;
        }

        #endregion

        #region 解析半全场

        //private void Write_BQC_SP_Trend_JSON(BJDC_BQC_SpInfoCollection collection)
        //{
        //    foreach (var item in collection)
        //    {
        //        try
        //        {
        //            var fileName = string.Format("BQC_SP_Trend_{0}.json", item.MatchOrderId);
        //            var fileFullName = BuildFileFullName(fileName, item.IssuseNumber);
        //            if (File.Exists(fileFullName))
        //            {
        //                //停止的比赛，不更新sp走势
        //                var league = CurrentMatchList.FirstOrDefault(p => p.IssuseNumber == item.IssuseNumber && p.MatchOrderId == item.MatchOrderId);
        //                if (league == null || league.MatchState == (int)BJDCMatchState.Stop)
        //                    continue;
        //                var oldJson = File.ReadAllText(fileFullName).Trim().Replace("var data=", "").Replace("];", "]");
        //                var oldList = string.IsNullOrEmpty(oldJson) ? new List<BJDC_BQC_SpInfo>() : JsonSerializer.Deserialize<List<BJDC_BQC_SpInfo>>(oldJson);
        //                oldList.Add(item);
        //                ServiceHelper.CreateOrAppend_JSONFile(fileFullName, JsonSerializer.Serialize(oldList), (log) =>
        //                {
        //                    this.WriteLog(log);
        //                });
        //            }
        //            else
        //            {
        //                var list = new List<BJDC_BQC_SpInfo>();
        //                list.Add(item);
        //                ServiceHelper.CreateOrAppend_JSONFile(fileFullName, JsonSerializer.Serialize(list), (log) =>
        //                {
        //                    this.WriteLog(log);
        //                });
        //            }

        //            var customerSavePath = new string[] { "BJDC", item.IssuseNumber };
        //            //上传文件
        //            ServiceHelper.PostFileToServer(fileFullName, customerSavePath, (log) =>
        //            {
        //                this.WriteLog(log);
        //            });
        //        }
        //        catch (Exception ex)
        //        {
        //            this.WriteLog(string.Format("写入第{0}期，赛事{1} SP值走势失败:{2}", item.IssuseNumber, item.MatchOrderId, ex.ToString()));
        //            continue;
        //        }
        //    }
        //}

        //private BJDC_BQC_SpInfoCollection Save_BQC_SP_And_GetDiffer(string fileFullName, BJDC_BQC_SpInfoCollection currentSp)
        //{
        //    if (currentSp.Count == 0)
        //        return currentSp;

        //    if (File.Exists(fileFullName))
        //    {
        //        try
        //        {
        //            var oldJson = File.ReadAllText(fileFullName).Trim().Replace("var data=", "").Replace("];", "]");
        //            var oldList = string.IsNullOrEmpty(oldJson) ? new List<BJDC_BQC_SpInfo>() : JsonSerializer.Deserialize<List<BJDC_BQC_SpInfo>>(oldJson);
        //            ServiceHelper.CreateOrAppend_JSONFile(fileFullName, JsonSerializer.Serialize(currentSp), (log) =>
        //            {
        //                this.WriteLog(log);
        //            });
        //            return GetNew_BQC_SPList(oldList, currentSp);
        //        }
        //        catch (Exception ex)
        //        {
        //            this.WriteLog("写入SP值JSON文件失败：" + ex.ToString());
        //            return currentSp;
        //        }
        //    }

        //    try
        //    {
        //        ServiceHelper.CreateOrAppend_JSONFile(fileFullName, JsonSerializer.Serialize(currentSp), (log) =>
        //        {
        //            this.WriteLog(log);
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        this.WriteLog("第一次写入SP值JSON文件失败：" + ex.ToString());
        //    }
        //    return currentSp;
        //}

        private List<BJDC_BQC_SpInfo> GetNew_BQC_SPList(List<BJDC_BQC_SpInfo> oldSP, List<BJDC_BQC_SpInfo> newSp)
        {
            var list = new List<BJDC_BQC_SpInfo>();
            foreach (var item in newSp)
            {
                var old = oldSP.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
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

        private List<BJDC_BQC_SpInfo> LoadBQC_SPList(XmlNode root, string issuseNumber)
        {
            var collection = new List<BJDC_BQC_SpInfo>();
            if (root.ChildNodes.Count == 0) return collection;
            foreach (XmlNode item in root.ChildNodes)
            {
                collection.Add(new BJDC_BQC_SpInfo
                {
                    IssuseNumber = issuseNumber,
                    MatchOrderId = item.Name.Replace("w", "").GetInt32(),
                    //LetBall = item.Attributes["r"].Value.GetInt32(),
                    SH_SH_Odds = item.Attributes["c1"].Value.GetDecimal(),
                    SH_P_Odds = item.Attributes["c3"].Value.GetDecimal(),
                    SH_F_Odds = item.Attributes["c5"].Value.GetDecimal(),
                    P_SH_Odds = item.Attributes["c7"].Value.GetDecimal(),
                    P_P_Odds = item.Attributes["c9"].Value.GetDecimal(),
                    P_F_Odds = item.Attributes["c11"].Value.GetDecimal(),
                    F_SH_Odds = item.Attributes["c13"].Value.GetDecimal(),
                    F_P_Odds = item.Attributes["c15"].Value.GetDecimal(),
                    F_F_Odds = item.Attributes["c17"].Value.GetDecimal(),
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                });
            }
            return collection;
        }

        #endregion

        #region 解析队伍数据，来源：okoo.com
        /// <summary>
        /// 从okoo.com获取队伍数据
        /// </summary>
        private List<C_BJDC_Match> BuildLeagueInfoCollectionFromOK(out string issuseNumber)
        {
            var matchUrl = "http://www.okooo.com/danchang/";
            var html = PostManager.Get(matchUrl, Encoding.GetEncoding("GB2312"), 0, (request) =>
            {
                if (ServiceHelper.IsUseProxy("BJDC"))
                {
                    var proxy = ServiceHelper.GetProxyUrl();
                    if (!string.IsNullOrEmpty(proxy))
                    {
                        request.Proxy = new System.Net.WebProxy(proxy);
                    }
                }
            }).Replace("\t", null).Replace("\n", null).Replace("\r", null).Replace("                        ", null).Replace("    ", null);
            var issuseNumberRegex = new Regex("<input type=\"hidden\" name=\"LotteryNo\" id=\"LotteryNo\" value=\"(?<value0>.*?)\" />", RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Match mach = issuseNumberRegex.Match(html);
            if (mach.Groups.Count != 2)
                throw new Exception("正则表达式解析 当前奖期出错 - " + html);
            //期号
            issuseNumber = mach.Groups[1].Value;
            this.WriteLog(string.Format("已成功从{0}上解析当前奖期为：{1}", matchUrl, issuseNumber));

            var leagueList = new List<C_BJDC_Match>();
            if (string.IsNullOrEmpty(html))
                return leagueList;
            //解析比赛信息
            var trRegex = new Regex("<tr .*? id=\"tr.*?\">(?<value1>.*?)</tr>", RegexOptions.IgnoreCase);
            var matchsRows = trRegex.Matches(html);//获取赛事信息
            foreach (System.Text.RegularExpressions.Match item in matchsRows)
            {
                if (item.Groups.Count != 2)
                    throw new Exception("解析比赛信息错误 - " + item.Value);
                var leagueAndColor = GetLeagueNameAndColor(item.Groups[1].Value);

                leagueList.Add(new C_BJDC_Match
                {
                    MatchName = leagueAndColor.Key,
                    MatchColor = leagueAndColor.Value,
                    MatchOrderId = GetLeagueID(item.Groups[1].Value).GetInt32(),
                    MatchStartTime = DateTime.Parse(GetGameStartDateTime(item.Groups[1].Value)),//.ToString("yyyy-MM-dd HH:mm:ss"),
                    HomeTeamName = GetHomeTeamName(item.Groups[1].Value),
                    GuestTeamName = GetGuestTeamName(item.Groups[1].Value),
                    LetBall = GetLetBall(item.Groups[1].Value).GetInt32(),
                    WinOdds = GetWinOdds(item.Groups[1].Value).GetDecimal(),
                    FlatOdds = GetFlatOdds(item.Groups[1].Value).GetDecimal(),
                    LoseOdds = GetLoseOdds(item.Groups[1].Value).GetDecimal(),
                });
            }
            return leagueList;
        }
        /// <summary>
        /// 获取比赛开始时间
        /// </summary>
        private string GetGameStartDateTime(string html)
        {
            string DateTimeStr = string.Empty;
            Regex regex = new Regex("title=\"比赛时间：(?<value1>.*?)\"", RegexOptions.IgnoreCase);
            var macht = regex.Match(html);
            DateTimeStr = macht.Groups[1].Value.Trim(); //开赛时间
            return DateTimeStr;
        }
        /// <summary>
        /// 获取联赛名称和颜色
        /// </summary>
        private KeyValuePair<string, string> GetLeagueNameAndColor(string html)
        {
            //联赛名字和颜色
            //格式： 俄甲$#D24444
            string MoC = string.Empty;
            Regex regex = new Regex("<a .*? style=\"background-color:(?<value0>.*?);\">(?<value1>.*?)</a>", RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Match macht = regex.Match(html);
            return new KeyValuePair<string, string>(macht.Groups[2].Value.Trim(), macht.Groups[1].Value.Trim());
        }
        /// <summary>
        /// 获取当期赛事编号
        /// </summary>
        private string GetLeagueID(string html)
        {
            string MatchID = string.Empty;
            Regex regex = new Regex("<i>(?<value0>.*?)</i>", RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Match macht = regex.Match(html);
            MatchID = macht.Groups[1].Value.Trim(); //官方赛事id
            return MatchID;
        }
        /// <summary>
        /// 获取主队名字
        /// </summary>
        private string GetHomeTeamName(string InputHtml)
        {
            string Home = string.Empty;
            Regex regex = new Regex("<span class=\"homenameobj homename\" title=\"\" attr=\".*?\">(?<value0>.*?)</span>", RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Match macht = regex.Match(InputHtml);
            Home = macht.Groups[1].Value.Trim(); //主队名字
            return Home;
        }
        /// <summary>
        /// 获取客队名字
        /// </summary>
        private string GetGuestTeamName(string html)
        {
            string Guest = string.Empty;
            Regex regex = new Regex("<span class=\"awaynameobj awayname\" title=\"\" attr=\".*?\">(?<value0>.*?)</span>", RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Match macht = regex.Match(html);
            Guest = macht.Groups[1].Value.Trim(); //客队名字
            return Guest;
        }
        /// <summary>
        /// 获取让球数
        /// </summary>
        private string GetLetBall(string InputHtml)
        {
            string Handicap = string.Empty;
            Regex regex = new Regex("<span class=\"handicapobj\">(?<value0>.*?)</span>", RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Match macht = regex.Match(InputHtml);
            Handicap = macht.Groups[1].Value.Trim().Replace("(", null).Replace(")", null); //获取让球数
            return Handicap;
        }
        /// <summary>
        /// 获取主胜平均欧赔
        /// </summary>
        private string GetWinOdds(string html)
        {
            string homeWin = string.Empty;
            Regex regex = new Regex("<span class=\"noborder0\">(?<value0>.*?)</span>", RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Match macht = regex.Match(html);
            homeWin = macht.Groups[1].Value.Trim(); //主胜平均欧赔
            return homeWin;
        }

        /// <summary>
        /// 获取和局平均欧赔
        /// </summary>
        private string GetFlatOdds(string html)
        {
            string standoff = string.Empty;
            Regex regex = new Regex("<span class=\"noborder1\">(?<value0>.*?)</span>", RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Match macht = regex.Match(html);
            standoff = macht.Groups[1].Value.Trim(); //和平均欧赔
            return standoff;
        }
        /// <summary>
        /// 获取客胜平均欧赔
        /// </summary>
        private string GetLoseOdds(string html)
        {
            string guestWin = string.Empty;
            Regex regex = new Regex("<span class=\"noborder1\">(?<value0>.*?)</span>", RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Match macht = regex.Match(html);
            guestWin = macht.Groups[1].Value.Trim(); //客胜平均欧赔
            return guestWin;
        }

        #endregion

        public void Stop()
        {
            BeStop = 1;

            if (timer != null)
                timer.Stop();
        }

        public void WriteLog(string log)
        {
            //if (_logWriter != null)
            //    _logWriter.Write(logCategory, logInfoSource, LogType.Information, "自动采集北京单场数据", log);
        }

        public void WriteError(string log)
        {
            //if (_logWriter != null)
            //    _logWriter.Write(logErrorCategory, logErrorSource, LogType.Error, "自动采集北京单场数据异常", log);
        }

        /// <summary>
        /// 采集310win的FXId
        /// </summary>
        private Dictionary<string, string> GetBJDC_FX(string issuseNumber)
        {
            string url = string.Format("http://www.310win.com/buy/DanChang.aspx?TypeID=5&issueNum={0}", issuseNumber);
            string html = PostManager.PostCustomer(url, string.Empty, Encoding.UTF8, (request) =>
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
            if (html == "404")
                return new Dictionary<string, string>();
            return GetBJDC_SPF_FX(issuseNumber, html);
        }
        private Dictionary<string, string> GetBJDC_SPF_FX(string issuseNumber, string html)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            int tableIndex = html.IndexOf("<tr class='niDate'>");
            html = html.Substring(tableIndex, html.Length - tableIndex);

            int endTableIndex = html.IndexOf("<div id=\"divDaohang\"");
            html = html.Substring(0, endTableIndex);

            var rows = html.Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < rows.Length; i++)
            {
                //个别彩种rows是否循环
                if (rows[i].IndexOf("matchid=\"") == -1)
                {
                    continue;
                }
                else if (rows[i].IndexOf("matchid=\"") != -1)
                {
                    var subfrist = rows[i].IndexOf("matchid=\"") + "matchid=\"".Length;
                    var sublast = rows[i].IndexOf("\">");
                    var Id = rows[i].Substring(subfrist, sublast - subfrist);

                    var winNumber = rows[i].Substring(rows[i].IndexOf("<span id=\"HomeOrder_") + "<span id=\"HomeOrder_".Length, 6);
                    var key = string.Format("{0}_{1}", issuseNumber, Id);
                    dic.Add(key, winNumber);
                }
            }
            return dic;
        }

        /// <summary>
        /// 采集OKOOO的FXId
        /// </summary>
        private Dictionary<string, string> GetBJDC_FX_OKOOO(string issuseNumber)
        {
            var dic = new Dictionary<string, string>();
            var bjdcUrl = string.Format("http://www.okooo.com/danchang/{0}/", issuseNumber);
            var html = PostManager.Get(bjdcUrl, Encoding.GetEncoding("GBK"));

            var target = "<div class=\"endmacth\">";
            var index = html.IndexOf(target);
            html = html.Substring(index + target.Length);
            target = "</div>";
            index = html.IndexOf(target);
            html = html.Substring(index + target.Length);

            target = "<div id=\"betbottom\"";
            index = html.IndexOf(target);
            html = html.Substring(0, index);
            var tableArray = html.Split(new string[] { "<table " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in tableArray)
            {
                if (string.IsNullOrEmpty(item)) continue;
                var trArray = item.Split(new string[] { "<tr class=\"alltrObj" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var tr in trArray)
                {
                    if (string.IsNullOrEmpty(tr)) continue;
                    if (tr.IndexOf("id=\"tr") < 0) continue;

                    var matchId = string.Empty;
                    var fxId = string.Empty;
                    var tdArray = tr.Split(new string[] { "<td" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var td in tdArray)
                    {
                        var temp = string.Empty;
                        target = "<i>";
                        index = td.IndexOf(target);
                        if (index >= 0)
                        {
                            //取编号
                            temp = td.Substring(index + target.Length);
                            target = "</i>";
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
                        }
                    }
                    if (!string.IsNullOrEmpty(matchId) && !string.IsNullOrEmpty(fxId) && !dic.ContainsKey(matchId))
                        dic.Add(matchId, fxId);
                }
            }

            return dic;
        }

    }

    public class BJDCMatchResultCache
    {
        public string Id { get; set; }
        public int CacheTimes { get; set; }
        public C_BJDC_MatchResult MatchResult { get; set; }
    }
}
