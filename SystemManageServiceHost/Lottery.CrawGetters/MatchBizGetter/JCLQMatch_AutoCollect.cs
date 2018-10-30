using EntityModel.CoreModel;
using EntityModel.Domain.Entities;
using EntityModel.Enum;
using EntityModel.ExceptionExtend;
using EntityModel.Interface;
using EntityModel.LotteryJsonInfo;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Expansion;
using KaSon.FrameWork.Common.Gateway;
using KaSon.FrameWork.Common.Net;
using KaSon.FrameWork.ORM.Helper;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Lottery.CrawGetters.MatchBizGetter
{
    
    /// <summary>
    /// 竞彩篮球
    /// </summary>
    public class JCLQMatch_AutoCollect : IBallAutoCollect
    {
    
        private const string logCategory = "Services.Info";
        private string logInfoSource = "Auto_Collect_JCLQMatch_Info_";
        private const string logErrorCategory = "Services.Error";
        private const string logErrorSource = "Auto_Collect_JCLQMatch_Error";

        private long BeStop = 0;
        private System.Timers.Timer timer = null;
        private int JCLQ_advanceMinutes = 0;
        private int JCLQ_Result_Day = 0;
        private string SavePath = string.Empty;
        private string SavePath_New = string.Empty;
        private string zhm_ServerUrl = "";
        private string zhm_Key = "";
        private string zhm_PartnerId = "";
        private int collectResultTotalTime = 1000 * 60 * 10;
        private int currentTime = 0;
        private List<JCLQMatchResultCache> cacheMatchResult = new List<JCLQMatchResultCache>();
        //private MatchManager manager = new MatchManager(DbAccess_Match_Helper.DbAccess);
        private Dictionary<string, string> _MatchStatus = new Dictionary<string, string>();
        private int OkNumber = -1;
        private Dictionary<string, string> _Match_Ok_HG = new Dictionary<string, string>();
        private DisableMatchConfigInfoCollection DisableMatchConfigList = new DisableMatchConfigInfoCollection();

        private List<string> _sendMessage = new List<string>();
        private string[] ozbTime = new string[] { "160610", "160611", "160612", "160613"
            , "160614", "160615", "160616", "160617", "160618", "160619" , "160620", "160621"
            , "160622", "160624", "160625", "160626", "160627", "160630", "160701", "160702",
            "160703", "160706", "160707", "160710"};

        public string Key { get; set; }
        public string Category { get; set; }
        private IMongoDatabase mDB;
        public JCLQMatch_AutoCollect(IMongoDatabase _mDB)
        {
            mDB = _mDB;
        }
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


                        DoWork(gameCode,true);

                    }
                    catch(Exception ex)
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

            try
            {
                JCLQ_advanceMinutes = int.Parse(ServiceHelper.GetSystemConfig("JCLQ_AdvanceMinutes"));
             //   DisableMatchConfigList = ServiceHelper.QueryDisableMatchConfigList("JCLQ");
                DisableMatchConfigList = new TicketGatewayAdmin().QueryDisableMatchConfigList("JCLQ");
                //zhm_ServerUrl = ServiceHelper.GetZHM_ServerUrl();
                //zhm_Key = ServiceHelper.GetZHM_Key();
                //zhm_PartnerId = ServiceHelper.GetZHM_PartnerId();

                this.WriteLog("开始采集竞彩篮球队伍数据");
                var matchFun = ServiceHelper.GetSystemConfig("JCLQ_Match_Fun");
                var matchList = new List<JCLQ_MatchInfo>();
                var matchList_RFSF = new List<JCLQ_MatchInfo>();
                var matchList_SFC = new List<JCLQ_MatchInfo>();
                var matchList_DXF = new List<JCLQ_MatchInfo>();
                var match_SF = new List<JCLQ_Match_SF>();
                var match_RFSF = new List<JCLQ_Match_RFSF>();
                var match_SFC = new List<JCLQ_Match_SFC>();
                var match_DXF = new List<JCLQ_Match_DXF>();
                var match_HH = new List<JCLQ_Match_HH>();
                var jclq_SP_SFC = new List<JCLQ_SP_SFC>();
                var match_HHDG = new List<JCLQ_Match_HHDG>();
                if (OkNumber >= 10 || OkNumber == -1)
                {
                    OkNumber = 0;
                    GetMatchList_okooo_match();
                }
                OkNumber++;
                try
                {
                    if (matchFun == "old")
                        matchList = GetMatchList();
                    if (matchFun == "new")
                        matchList = GetMatchList_New();
                    if (matchFun == "zzjcw")
                    {
                        matchList = GetMatchList_ZZJCW_SF();
                        this.WriteLog(string.Format("采集竞彩篮球胜负队伍数据完成，数据{0}条", matchList.Count));
                        matchList_RFSF = GetMatchList_ZZJCW_RFSF();
                        this.WriteLog(string.Format("采集竞彩篮球让分胜负队伍数据完成，数据{0}条", matchList_RFSF.Count));
                        matchList_SFC = GetMatchList_ZZJCW_SFC();
                        this.WriteLog(string.Format("采集竞彩篮球胜分差队伍数据完成，数据{0}条", matchList_SFC.Count));
                        matchList_DXF = GetMatchList_ZZJCW_DXF();
                        this.WriteLog(string.Format("采集竞彩篮球大小分队伍数据完成，数据{0}条", matchList_DXF.Count));
                    }
                    //if (matchFun == "ok")
                    //{
                    //    matchList_SFC = GetMatchList_ZZJCW_SFC();
                    //    //matchList = GetMatchList_ZZJCW_SF();
                    //    //matchList_RFSF = GetMatchList_ZZJCW_RFSF();
                    //    //matchList_SFC = GetMatchList_ZZJCW_SFC();
                    //    //matchList_DXF = GetMatchList_ZZJCW_DXF();
                    //}
                    //中民比赛数据对比
                    //matchList = AddZHMPrivilegesType(matchList);
                }
                catch (Exception ex)
                {
                    this.WriteLog(ex.Message);
                }
                this.WriteLog(string.Format("采集竞彩篮球队伍数据完成，数据{0}条", matchList.Count));



                foreach (var item in matchList_DXF)
                {
                    //var leagueMatchId = 0;
                    //var mid = 0;
                    //var hg = _Match_Ok_HG.Where(p => p.Key == item.MatchId).FirstOrDefault();
                    //if (!string.IsNullOrEmpty(hg.Key))
                    //{
                    //    var strL = hg.Value.Split('^');
                    //    if (strL.Length == 2)
                    //    {
                    //        mid = int.Parse(strL[0]);
                    //        leagueMatchId = int.Parse(strL[1]);
                    //    }
                    //}

                    #region HH
                    match_HH.Add(new JCLQ_Match_HH
                    {
                        MatchId = item.MatchId,
                        MatchIdName = item.MatchIdName,
                        MatchData = item.MatchData,
                        MatchNumber = item.MatchNumber,
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        LeagueId = item.LeagueId,
                        LeagueName = item.LeagueName,
                        LeagueColor = item.LeagueColor,
                        HomeTeamName = item.HomeTeamName,
                        GuestTeamName = item.GuestTeamName,
                        MatchState = 0,
                        StartDateTime = item.StartDateTime,
                        DSStopBettingTime = item.DSStopBettingTime,
                        FSStopBettingTime = item.FSStopBettingTime,
                        AverageWin = item.AverageWin,
                        AverageLose = item.AverageLose,
                        HomeTeamId = item.HomeTeamId,
                        GuestTeamId = item.GuestTeamId,
                        Mid = item.Mid,
                        FXId = item.FXId,
                        PrivilegesType = item.PrivilegesType,
                        State = _MatchStatus.Where(p => p.Key == item.MatchId).FirstOrDefault().Key != null ? _MatchStatus.Where(p => p.Key == item.MatchId).FirstOrDefault().Value : "0",
                    });
                    #endregion

                    #region HHDG
                    match_HHDG.Add(new JCLQ_Match_HHDG
                    {
                        MatchId = item.MatchId,
                        MatchIdName = item.MatchIdName,
                        MatchData = item.MatchData,
                        MatchNumber = item.MatchNumber,
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        LeagueId = item.LeagueId,
                        LeagueName = item.LeagueName,
                        LeagueColor = item.LeagueColor,
                        HomeTeamName = item.HomeTeamName,
                        GuestTeamName = item.GuestTeamName,
                        MatchState = 0,
                        StartDateTime = item.StartDateTime,
                        DSStopBettingTime = item.DSStopBettingTime,
                        FSStopBettingTime = item.FSStopBettingTime,
                        AverageWin = item.AverageWin,
                        AverageLose = item.AverageLose,
                        HomeTeamId = item.HomeTeamId,
                        GuestTeamId = item.GuestTeamId,
                        Mid = item.Mid,
                        FXId = item.FXId,
                        PrivilegesType = item.PrivilegesType,
                        State = _MatchStatus.Where(p => p.Key == item.MatchId).FirstOrDefault().Key != null ? _MatchStatus.Where(p => p.Key == item.MatchId).FirstOrDefault().Value : "0",
                    });
                    #endregion

                    if (matchList.Count > 0)
                    {
                        #region SF
                        match_SF.Add(new JCLQ_Match_SF
                        {
                            MatchId = item.MatchId,
                            MatchIdName = item.MatchIdName,
                            MatchData = item.MatchData,
                            MatchNumber = item.MatchNumber,
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            LeagueId = item.LeagueId,
                            LeagueName = item.LeagueName,
                            LeagueColor = item.LeagueColor,
                            HomeTeamName = item.HomeTeamName,
                            GuestTeamName = item.GuestTeamName,
                            MatchState = 0,
                            StartDateTime = item.StartDateTime,
                            DSStopBettingTime = item.DSStopBettingTime,
                            FSStopBettingTime = item.FSStopBettingTime,
                            AverageWin = item.AverageWin,
                            AverageLose = item.AverageLose,
                            HomeTeamId = item.HomeTeamId,
                            GuestTeamId = item.GuestTeamId,
                            Mid = item.Mid,
                            FXId = item.FXId,
                            PrivilegesType = item.PrivilegesType,
                            State = _MatchStatus.Where(p => p.Key == item.MatchId).FirstOrDefault().Key != null ? _MatchStatus.Where(p => p.Key == item.MatchId).FirstOrDefault().Value : "0",
                        });
                        #endregion
                    }

                    #region RFSF
                    match_RFSF.Add(new JCLQ_Match_RFSF
                    {
                        MatchId = item.MatchId,
                        MatchIdName = item.MatchIdName,
                        MatchData = item.MatchData,
                        MatchNumber = item.MatchNumber,
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        LeagueId = item.LeagueId,
                        LeagueName = item.LeagueName,
                        LeagueColor = item.LeagueColor,
                        HomeTeamName = item.HomeTeamName,
                        GuestTeamName = item.GuestTeamName,
                        MatchState = 0,
                        StartDateTime = item.StartDateTime,
                        DSStopBettingTime = item.DSStopBettingTime,
                        FSStopBettingTime = item.FSStopBettingTime,
                        AverageWin = item.AverageWin,
                        AverageLose = item.AverageLose,
                        HomeTeamId = item.HomeTeamId,
                        GuestTeamId = item.GuestTeamId,
                        Mid = item.Mid,
                        FXId = item.FXId,
                        PrivilegesType = item.PrivilegesType,
                        State = _MatchStatus.Where(p => p.Key == item.MatchId).FirstOrDefault().Key != null ? _MatchStatus.Where(p => p.Key == item.MatchId).FirstOrDefault().Value : "0",
                    });
                    #endregion

                    #region SFC
                    match_SFC.Add(new JCLQ_Match_SFC
                    {
                        MatchId = item.MatchId,
                        MatchIdName = item.MatchIdName,
                        MatchData = item.MatchData,
                        MatchNumber = item.MatchNumber,
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        LeagueId = item.LeagueId,
                        LeagueName = item.LeagueName,
                        LeagueColor = item.LeagueColor,
                        HomeTeamName = item.HomeTeamName,
                        GuestTeamName = item.GuestTeamName,
                        MatchState = 0,
                        StartDateTime = item.StartDateTime,
                        DSStopBettingTime = item.DSStopBettingTime,
                        FSStopBettingTime = item.FSStopBettingTime,
                        AverageWin = item.AverageWin,
                        AverageLose = item.AverageLose,
                        HomeTeamId = item.HomeTeamId,
                        GuestTeamId = item.GuestTeamId,
                        Mid = item.Mid,
                        FXId = item.FXId,
                        PrivilegesType = item.PrivilegesType,
                        State = _MatchStatus.Where(p => p.Key == item.MatchId).FirstOrDefault().Key != null ? _MatchStatus.Where(p => p.Key == item.MatchId).FirstOrDefault().Value : "0",
                    });
                    #endregion

                    #region DXF
                    match_DXF.Add(new JCLQ_Match_DXF
                    {
                        MatchId = item.MatchId,
                        MatchIdName = item.MatchIdName,
                        MatchData = item.MatchData,
                        MatchNumber = item.MatchNumber,
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        LeagueId = item.LeagueId,
                        LeagueName = item.LeagueName,
                        LeagueColor = item.LeagueColor,
                        HomeTeamName = item.HomeTeamName,
                        GuestTeamName = item.GuestTeamName,
                        MatchState = 0,
                        StartDateTime = item.StartDateTime,
                        DSStopBettingTime = item.DSStopBettingTime,
                        FSStopBettingTime = item.FSStopBettingTime,
                        AverageWin = item.AverageWin,
                        AverageLose = item.AverageLose,
                        HomeTeamId = item.HomeTeamId,
                        GuestTeamId = item.GuestTeamId,
                        Mid = item.Mid,
                        FXId = item.FXId,
                        PrivilegesType = item.PrivilegesType,
                        State = _MatchStatus.Where(p => p.Key == item.MatchId).FirstOrDefault().Key != null ? _MatchStatus.Where(p => p.Key == item.MatchId).FirstOrDefault().Value : "0",
                    });
                    #endregion
                }


                #region 竞彩篮球采集结果
                var matchResult = new List<JCLQ_MatchResult>();
                if (getResult)
                {
                    this.WriteLog("开始采集竞彩篮球比赛结果");
                    var source = ServiceHelper.GetSystemConfig("JCLQ_Result_Source");
                    var phone = ServiceHelper.GetSystemConfig("Result_Phone");
                    //首先采集310结果
                    var matchResult_310 = GetMatchResultListFrom310Win();
                    var matchResult_ZhiDing = new List<JCLQ_MatchResult>();
                    var matchResult_ZhiDing_F = new List<JCLQ_MatchResult>();
                    if (_sendMessage.Count > 10)
                        _sendMessage = new List<string>();
                    if (source == "cpdyj")
                    {
                        matchResult_ZhiDing = GetMatchResultList();
                    }
                    //if (source == "310win")
                    //{
                    //    matchResult = GetMatchResultListFrom310Win();
                    //}
                    if (source == "aicai")
                    {
                        matchResult_ZhiDing = GetMatchResultListFromAiCai();
                    }
                    if (source == "zzjcw")
                    {
                        matchResult_ZhiDing = GetMatchResultListFromZZJCW();
                    }
                    if (source == "ok")
                    {
                        matchResult_ZhiDing = GetMatchResultListFromOk();
                    }
                    //if (source == "500wan")
                    //{
                    //matchResult_ZhiDing_F = GetMatchResultListFrom500wan();
                    //}

                    foreach (var item in matchResult_310)
                    {
                        var result_Ok = matchResult_ZhiDing.Where(p => p.MatchId == item.MatchId).FirstOrDefault();
                        //var result_500 = matchResult_ZhiDing_F.Where(p => p.MatchId == item.MatchId).FirstOrDefault();
                        //if (result_Ok == null && result_500 == null) continue;

                        var isResultEques = false;
                        if (result_Ok != null)
                        {
                            if ((item.DXF_Result == result_Ok.DXF_Result) && (item.SF_Result == result_Ok.SF_Result)
                              && (item.RFSF_Result == result_Ok.RFSF_Result) && (item.SFC_Result == result_Ok.SFC_Result)
                              && (item.HomeScore == result_Ok.HomeScore) && (item.GuestScore == result_Ok.GuestScore))
                                isResultEques = true;
                        }
                        //if (result_500 != null && !isResultEques)
                        //{
                        //    if ((item.DXF_Result == result_500.DXF_Result) && (item.SF_Result == result_500.SF_Result)
                        //     && (item.RFSF_Result == result_500.RFSF_Result) && (item.SFC_Result == result_500.SFC_Result)
                        //     && (item.HomeScore == result_500.HomeScore) && (item.GuestScore == result_500.GuestScore))
                        //        isResultEques = true;
                        //}
                        if (!isResultEques)
                            continue;

                        matchResult.Add(item);

                        //if ((item.DXF_Result == result_500.DXF_Result || item.DXF_Result == result_Ok.DXF_Result) && (item.SF_Result == result_500.SF_Result || item.SF_Result == result_Ok.SF_Result)
                        //     && (item.RFSF_Result == result_500.RFSF_Result || item.RFSF_Result == result_Ok.RFSF_Result) && (item.SFC_Result == result_500.SFC_Result || item.SFC_Result == result_Ok.SFC_Result))
                        //{
                        //    matchResult.Add(item);
                        //}
                        //else
                        //{
                        //    var pl = phone.Split('|');
                        //    var str_Error = string.Format("采集比赛结果中{0}比赛对比出错。", item.MatchId);
                        //    this.WriteLog(string.Format("{0}比赛对比错误：1、SF_Result-{1},RFSF_Result-{2},SFC_Result-{3},DXF_Result-{4}  2、SF_Result-{5},RFSF_Result-{6},SFC_Result-{7},DXF_Result-{8}。 发送短信内容：{9}",
                        //       item.MatchId, result_500.SF_Result, result_500.RFSF_Result, result_500.SFC_Result, result_500.DXF_Result, item.SF_Result, item.RFSF_Result, item.SFC_Result, item.DXF_Result, str_Error));
                        //    if (_sendMessage.Contains(str_Error)) continue;
                        //    foreach (var p in pl)
                        //    {
                        //        try
                        //        {
                        //            //ServiceHelper.SendSMSMessage(str_Error, p);
                        //        }
                        //        catch
                        //        {
                        //        }
                        //    }

                        //    _sendMessage.Add(str_Error);
                        //    continue;
                        //}

                        //if (item.DXF_Result != result_310.DXF_Result || item.SF_Result != result_310.SF_Result || item.RFSF_Result != result_310.RFSF_Result || item.SFC_Result != result_310.SFC_Result)
                        //{
                        //    var pl = phone.Split('|');
                        //    var str_Error = string.Format("采集比赛结果中{0}比赛对比出错。", item.MatchId);
                        //    this.WriteLog(string.Format("{0}比赛对比错误：1、SF_Result-{1},RFSF_Result-{2},SFC_Result-{3},DXF_Result-{4}  2、SF_Result-{5},RFSF_Result-{6},SFC_Result-{7},DXF_Result-{8}。 发送短信内容：{9}",
                        //       item.MatchId, result_310.SF_Result, result_310.RFSF_Result, result_310.SFC_Result, result_310.DXF_Result, item.SF_Result, item.RFSF_Result, item.SFC_Result, item.DXF_Result, str_Error));
                        //    if (_sendMessage.Contains(str_Error)) continue;
                        //    foreach (var p in pl)
                        //    {
                        //        try
                        //        {
                        //            ServiceHelper.SendSMSMessage(str_Error, p);
                        //        }
                        //        catch
                        //        {
                        //        }
                        //    }

                        //    _sendMessage.Add(str_Error);
                        //    continue;
                        //}
                        //matchResult.Add(result_310);
                    }

                    this.WriteLog(string.Format("采集竞彩篮球比赛结果完成，数据{0}条", matchResult.Count));
                }
                #endregion

                this.WriteLog("开始采集竞彩篮球SP数据");

                var sfList = Get_SF_SP_ZZJCW(); //Get_SF_SP();
                var rfsfList = Get_RFSF_SP_ZZJCW();// Get_RFSF_SP();
                var sfcList = Get_SFC_SP_ZZJCW();// Get_SFC_SP();
                var dxfList = Get_DXF_SP_ZZJCW(); //Get_DXF_SP();

                //var sfList = Get_SF_SP_New(false); //Get_SF_SP();
                //var rfsfList = Get_RFSF_SP_New(false);// Get_RFSF_SP();
                //var sfcList = Get_SFC_SP_New(false);// Get_SFC_SP();
                //var dxfList = Get_DXF_SP_New(false); //Get_DXF_SP();

                ////单式
                //var sfList_DS = Get_SF_SP_New(true); //Get_SF_SP();
                //var rfsfList_DS = Get_RFSF_SP_New(true);// Get_RFSF_SP();
                //var sfcList_DS = Get_SFC_SP_New(true);// Get_SFC_SP();
                //var dxfList_DS = Get_DXF_SP_New(true); //Get_DXF_SP();

                this.WriteLog(string.Format("采集竞彩篮球SP数据完成,SF:{0}条；RFSF:{1}条；SFC:{2}条；DXF:{3}条", sfList.Count, rfsfList.Count, sfcList.Count, dxfList.Count));
                var jclqSP = new List<JCLQ_SP>();
                var jclqSP_DS = new List<JCLQ_SP>();

                #region 循环写入SP
                var il = matchList_DXF.Count;
                var str = string.Empty;
                string[] propertyNames = new[] { "Id", "MatchData", "MatchId", "MatchNumber", "CreateTime" };
                for (int i = 0; i < il; i++)
                {
                    var matchid = matchList_DXF[i].MatchId;
                    var sps = new JCLQ_SP();
                    var sfc_sp = new JCLQ_SP_SFC();
                    //var sps_DS = new JCLQ_SP();
                    sps.MatchId = matchList_DXF[i].MatchId;
                    sps.MatchData = matchList_DXF[i].MatchData;
                    sps.MatchNumber = matchList_DXF[i].MatchNumber;
                    sfc_sp.MatchId = matchList_DXF[i].MatchId;
                    sfc_sp.MatchData = matchList_DXF[i].MatchData;
                    sfc_sp.MatchNumber = matchList_DXF[i].MatchNumber;
                    //sps_DS.MatchId = matchList[i].MatchId;
                    //sps_DS.MatchData = matchList[i].MatchData;
                    //sps_DS.MatchNumber = matchList[i].MatchNumber;
                    var hh = match_HH.Where(p => p.MatchId == matchList_DXF[i].MatchId).FirstOrDefault();
                    var hhdg = match_HHDG.Where(p => p.MatchId == matchList_DXF[i].MatchId).FirstOrDefault();
                    var m_sf = match_SF.Where(p => p.MatchId == matchList_DXF[i].MatchId).FirstOrDefault();
                    var m_rfsf = match_RFSF.Where(p => p.MatchId == matchList_DXF[i].MatchId).FirstOrDefault();
                    var m_sfc = match_SFC.Where(p => p.MatchId == matchList_DXF[i].MatchId).FirstOrDefault();
                    var m_dxf = match_DXF.Where(p => p.MatchId == matchList_DXF[i].MatchId).FirstOrDefault();
                    JCLQ_SF_SP sf = sfList.FirstOrDefault(o => o.MatchId == matchid);
                    if (sf != null)
                    {
                        //if (sf.WinSP == 0M && sf.LoseSP == 0M)
                        //    sf.NoSaleState = "1";
                        //else
                        //    sf.NoSaleState = "0";
                        str = ServiceHelper.getProperties(sf, propertyNames);
                        sps.SF = str;
                        if (hh != null)
                            hh.SF = str;
                        if (hhdg != null)
                            hhdg.SF = str;
                        if (m_sf != null)
                            m_sf.SF = str;
                    }
                    //JCLQ_SF_SP sf_DS = sfList_DS.FirstOrDefault(o => o.MatchId == matchid);
                    //if (sf_DS != null)
                    //{
                    //    str = ServiceHelper.getProperties(sf_DS, propertyNames);
                    //    sps_DS.SF = str;
                    //}
                    JCLQ_RFSF_SP rfsf = rfsfList.FirstOrDefault(o => o.MatchId == matchid);
                    if (rfsf != null)
                    {
                        //if (rfsf.WinSP == 0M && rfsf.LoseSP == 0M)
                        //    rfsf.NoSaleState = "1";
                        //else
                        //    rfsf.NoSaleState = "0";
                        str = ServiceHelper.getProperties(rfsf, propertyNames);
                        sps.RFSF = str;
                        if (hh != null)
                            hh.RFSF = str;
                        if (hhdg != null)
                            hhdg.RFSF = str;
                        if (m_rfsf != null)
                            m_rfsf.RFSF = str;
                    }
                    //JCLQ_RFSF_SP rfsf_DS = rfsfList_DS.FirstOrDefault(o => o.MatchId == matchid);
                    //if (rfsf_DS != null)
                    //{
                    //    str = ServiceHelper.getProperties(rfsf_DS, propertyNames);
                    //    sps_DS.RFSF = str;
                    //}
                    JCLQ_SFC_SP sfc = sfcList.FirstOrDefault(o => o.MatchId == matchid);
                    if (sfc != null)
                    {
                        //if (sfc.GuestWin1_5 == 0M && sfc.GuestWin11_15 == 0M && sfc.GuestWin16_20 == 0M && sfc.GuestWin21_25 == 0M && sfc.GuestWin26 == 0M && sfc.GuestWin6_10 == 0M && sfc.HomeWin1_5 == 0M && sfc.HomeWin11_15 == 0M && sfc.HomeWin16_20 == 0M && sfc.HomeWin21_25 == 0M && sfc.HomeWin26 == 0M && sfc.HomeWin6_10 == 0M)
                        //    sfc.NoSaleState = "1";
                        //else
                        //    sfc.NoSaleState = "0";
                        str = ServiceHelper.getProperties(sfc, propertyNames);
                        sps.SFC = str;
                        if (hhdg != null)
                            hhdg.SFC = str;
                        sfc_sp.SFC = str;
                        if (m_sfc != null)
                            m_sfc.SFC = str;
                    }
                    //JCLQ_SFC_SP sfc_DS = sfcList_DS.FirstOrDefault(o => o.MatchId == matchid);
                    //if (sfc_DS != null)
                    //{
                    //    str = ServiceHelper.getProperties(sfc_DS, propertyNames);
                    //    sps_DS.SFC = str;
                    //}
                    JCLQ_DXF_SP dxf = dxfList.FirstOrDefault(o => o.MatchId == matchid);
                    if (dxf != null)
                    {
                        //if (dxf.DF == 0M && dxf.XF == 0M)
                        //    dxf.NoSaleState = "1";
                        //else
                        //    dxf.NoSaleState = "0";
                        str = ServiceHelper.getProperties(dxf, propertyNames);
                        sps.DXF = str;
                        if (hh != null)
                            hh.DXF = str;
                        if (hhdg != null)
                            hhdg.DXF = str;
                        if (m_dxf != null)
                            m_dxf.DXF = str;
                    }
                    //JCLQ_DXF_SP dxf_DS = dxfList_DS.FirstOrDefault(o => o.MatchId == matchid);
                    //if (dxf_DS != null)
                    //{
                    //    str = ServiceHelper.getProperties(dxf_DS, propertyNames);
                    //    sps_DS.DXF = str;
                    //}
                    jclqSP.Add(sps);
                    jclq_SP_SFC.Add(sfc_sp);
                    //jclqSP_DS.Add(sps_DS);
                }
                #endregion

                this.WriteLog("开始对比竞彩篮球数据");

                //if (matchList.Count() > 0)
                //    GetNewJCLQList<JCLQ_MatchInfo>(matchList, "Match_List.json");
                //else
                //    GetNewJCLQList<JCLQ_MatchInfo>(matchList_RFSF, "Match_List.json");
                ServiceHelper.BuildNewMatchList<JCLQ_MatchInfo>(mDB, "JCLQ_Match_List", matchList_DXF, null, CompareNewJCLQList, SaveHistory_New);
                // GetNewJCLQList<JCLQ_MatchInfo>(matchList_DXF, "Match_List.json");



                //GetNewJCLQList<JCLQ_MatchInfo>(matchList.Count > matchList_RFSF.Count ? matchList : (matchList_RFSF.Count > matchList_SFC.Count ?
                //matchList_RFSF : (matchList_SFC.Count > matchList_DXF.Count ? matchList_SFC : matchList_DXF)), "Match_List.json");


                //GetNewJCLQList<JCLQ_MatchInfo>(matchList, "Match_List.json");
                ServiceHelper.BuildNewMatchList<JCLQ_MatchInfo>(mDB, "JCLQ_Match_RFSF_List", matchList_RFSF, null, CompareNewJCLQList, SaveHistory_New);
                ServiceHelper.BuildNewMatchList<JCLQ_MatchInfo>(mDB, "JCLQ_Match_SFC_List", matchList_SFC, null, CompareNewJCLQList, SaveHistory_New);
                var newMatchList = ServiceHelper.BuildNewMatchList<JCLQ_MatchInfo>(mDB, "JCLQ_Match_DXF_List", matchList_DXF, null, CompareNewJCLQList, SaveHistory_New);

                //GetNewJCLQList<JCLQ_MatchInfo>(matchList_RFSF, "Match_RFSF_List.json");
                //GetNewJCLQList<JCLQ_MatchInfo>(matchList_SFC, "Match_SFC_List.json");
                //var newMatchList = GetNewJCLQList<JCLQ_MatchInfo>(matchList_DXF, "Match_DXF_List.json");
                //过关sp
                var newSF_SP_List = ServiceHelper.BuildNewMatchList<JCLQ_SF_SP>(mDB, "JCLQ_SF_SP", sfList, null, CompareNewJCLQList, SaveHistory_New);
                var newRFSF_SP_List = ServiceHelper.BuildNewMatchList<JCLQ_RFSF_SP>(mDB, "JCLQ_RFSF_SP", rfsfList, null, CompareNewJCLQList, SaveHistory_New);
                var newSFC_SP_List = ServiceHelper.BuildNewMatchList<JCLQ_SFC_SP>(mDB, "JCLQ_SFC_SP", sfcList, null, CompareNewJCLQList, SaveHistory_New);
                var newDXF_SP_List = ServiceHelper.BuildNewMatchList<JCLQ_DXF_SP>(mDB, "JCLQ_DXF_SP", dxfList, null, CompareNewJCLQList, SaveHistory_New);

                //var newSF_SP_List = GetNewJCLQList<JCLQ_SF_SP>(sfList, "SF_SP.json");
                //var newRFSF_SP_List = GetNewJCLQList<JCLQ_RFSF_SP>(rfsfList, "RFSF_SP.json");
                //var newSFC_SP_List = GetNewJCLQList<JCLQ_SFC_SP>(sfcList, "SFC_SP.json");
                //var newDXF_SP_List = GetNewJCLQList<JCLQ_DXF_SP>(dxfList, "DXF_SP.json");
                //把所以的SP集中z
                //GetNewJCLQList<JCLQ_SP>(jclqSP, "SP.json");
                 ServiceHelper.BuildNewMatchList<JCLQ_SP>(mDB, "JCLQ_SP", jclqSP, null, CompareNewJCLQList, SaveHistory_New);
                //New
                ServiceHelper.BuildNewMatchList<JCLQ_Match_HH>(mDB, "JCLQ_Match_HH_List", match_HH, null, CompareNewJCLQList);
                ServiceHelper.BuildNewMatchList<JCLQ_Match_SF>(mDB, "JCLQ_Match_SF_List", match_SF, null, CompareNewJCLQList);
                ServiceHelper.BuildNewMatchList<JCLQ_Match_RFSF>(mDB, "JCLQ_Match_RFSF_List", match_RFSF, null, CompareNewJCLQList);
                ServiceHelper.BuildNewMatchList<JCLQ_Match_SFC>(mDB, "JCLQ_Match_SFC_List", match_SFC, null, CompareNewJCLQList);
                ServiceHelper.BuildNewMatchList<JCLQ_Match_DXF>(mDB, "JCLQ_Match_DXF_List", match_DXF, null, CompareNewJCLQList);
                ServiceHelper.BuildNewMatchList<JCLQ_SP_SFC>(mDB, "JCLQ_SP", jclq_SP_SFC, null, CompareNewJCLQList);
                ServiceHelper.BuildNewMatchList<JCLQ_Match_HHDG>(mDB, "JCLQ_Match_HHDG_List", match_HHDG, null, CompareNewJCLQList);

             

               // GetNewJCLQList_New<JCLQ_Match_HH>(match_HH, "Match_HH_List.json");
                //GetNewJCLQList_New<JCLQ_Match_SF>(match_SF, "Match_SF_List.json");
                //GetNewJCLQList_New<JCLQ_Match_RFSF>(match_RFSF, "Match_RFSF_List.json");
                //GetNewJCLQList_New<JCLQ_Match_SFC>(match_SFC, "Match_SFC_List.json");
                //GetNewJCLQList_New<JCLQ_Match_DXF>(match_DXF, "Match_DXF_List.json");
                //GetNewJCLQList_New<JCLQ_SP_SFC>(jclq_SP_SFC, "SP.json");
                //GetNewJCLQList_New<JCLQ_Match_HHDG>(match_HHDG, "Match_HHDG_List.json");

                ////单关sp
                //var newSF_SP_List_DS = GetNewJCLQList<JCLQ_SF_SP>(sfList_DS, "SF_SP_DS.json");
                //var newRFSF_SP_List_DS = GetNewJCLQList<JCLQ_RFSF_SP>(rfsfList_DS, "RFSF_SP_DS.json");
                //var newSFC_SP_List_DS = GetNewJCLQList<JCLQ_SFC_SP>(sfcList_DS, "SFC_SP_DS.json");
                //var newDXF_SP_List_DS = GetNewJCLQList<JCLQ_DXF_SP>(dxfList_DS, "DXF_SP_DS.json");

                //把所以的SP集中
                //GetNewJCLQList<JCLQ_SP>(jclqSP_DS, "SP_DS.json");

                //var newMatchResultList = GetNewJCLQList<JCLQ_MatchResult>(matchResult, "Match_Result_List.json");
               // GetNewJCLQList<JCLQ_MatchResult>(matchResult, "Match_Result_List.json");

                ServiceHelper.BuildNewMatchList<JCLQ_MatchResult>(mDB, "JCLQ_Match_Result_List", matchResult, null, CompareNewJCLQList, SaveHistory_New);

                var maxCacheTimes = int.Parse(ServiceHelper.GetSystemConfig("JCLQ_Result_CacheTimes"));
                if (maxCacheTimes <= 0)
                    throw new Exception("JCZQ_Result_CacheTimes 必须大于0 ");

                //采集到结果后，缓存一次，到下一次再采集到结果后  再发送通知
                var newMatchResultList = new List<KeyValuePair<DBChangeState, JCLQ_MatchResult>>();
                foreach (var item in matchResult)
                {
                    newMatchResultList.Add(new KeyValuePair<DBChangeState, JCLQ_MatchResult>(DBChangeState.Update, item));

                    #region 去掉缓存

                    //var t = cacheMatchResult.FirstOrDefault(r => r.Id == item.MatchId);
                    //if (t == null)
                    //{
                    //    //缓存集里面没有当前数据
                    //    this.WriteLog(string.Format("添加进缓存 {0}", item.MatchId));
                    //    cacheMatchResult.Add(new JCLQMatchResultCache
                    //    {
                    //        Id = item.MatchId,
                    //        MatchResult = item,
                    //        CacheTimes = 1,
                    //    });
                    //    continue;
                    //}
                    ////缓存集里面有当前数据,把缓存数 +1
                    //var tIndex = cacheMatchResult.IndexOf(t);
                    //if (tIndex < 0) continue;
                    //var cacheTimes = cacheMatchResult[tIndex].CacheTimes;
                    //if (cacheTimes >= maxCacheTimes)
                    //{
                    //    //添加进发送通知列表
                    //    this.WriteLog(string.Format("添加进通知列表 {0}  并移除缓存", item.MatchId));
                    //    newMatchResultList.Add(new KeyValuePair<DBChangeState, JCLQ_MatchResult>(DBChangeState.Update, item));
                    //    cacheMatchResult.RemoveAt(tIndex);
                    //    continue;
                    //}

                    //this.WriteLog(string.Format("更新缓存 {0}", item.MatchId));
                    //cacheMatchResult[tIndex].CacheTimes++;
                    //cacheMatchResult[tIndex].MatchResult = item; 

                    #endregion
                }

                this.WriteLog("对比竞彩篮球数据完成。");

                this.WriteLog("开始对变化的数据发送通知");

                //var p = new ObjectPersistence(DbAccess_Match_Helper.DbAccess);

                #region 发送队伍通知

                this.WriteLog("1、开始=>发送队伍数据通知");
                var addMatchList = new List<JCLQ_MatchInfo>();
                var updateMatchList = new List<JCLQ_MatchInfo>();
                foreach (var r in newMatchList)
                {
                    try
                    {
                        if (r.Key == DBChangeState.Add)
                        {
                            addMatchList.Add(r.Value);
                            //p.Add(r.Value);
                        }
                        else
                        {
                            updateMatchList.Add(r.Value);
                            //p.Modify(r.Value);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(string.Format("向数据库写入 竞彩篮球 数据异常 编号：{0}，异常：{1}", r.Value.MatchId, ex.ToString()));
                    }
                }
                if (addMatchList.Count > 0)
                {
                    var category = (int)NoticeCategory.JCLQ_Match;
                    var state = (int)DBChangeState.Add;
                    var param = string.Join("_", (from l in addMatchList select l.MatchId).ToArray());
                    var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                    var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                    //发送 竞彩足球队伍 添加 通知
                    var innerKey = string.Format("{0}_{1}", "JCLQ_MatchInfo", "Add");
                  //  ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.JCLQ_Match);
                    new Sports_Business(this.mDB).UpdateLocalData(param, "", NoticeType.JCLQ_Match, innerKey);
                    //ServiceHelper.SendNotice(issuse_Request, (log) =>
                    //{
                    //    this.WriteLog(log);
                    //});
                }
                if (updateMatchList.Count > 0)
                {
                    var category = (int)NoticeCategory.JCLQ_Match;
                    var state = (int)DBChangeState.Update;
                    var param = string.Join("_", (from l in updateMatchList select l.MatchId).ToArray());
                    var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                    var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                    //发送 竞彩足球队伍 修改 通知
                    var innerKey = string.Format("{0}_{1}", "JCLQ_MatchInfo", "Update");
                   // ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.JCLQ_Match);
                    new Sports_Business(this.mDB).UpdateLocalData(param, "", NoticeType.JCLQ_Match, innerKey);
                    //ServiceHelper.SendNotice(issuse_Request, (log) =>
                    //{
                    //    this.WriteLog(log);
                    //});
                }

                this.WriteLog("1、发送队伍数据通知 完成");

                #endregion

                #region 发送SF SP数据通知

                this.WriteLog("2、开始=>发送SF SP数据通知");
                var addSF_SP_List = new List<JCLQ_SF_SP>();
                foreach (var r in newSF_SP_List)
                {
                    try
                    {
                        addSF_SP_List.Add(r.Value);
                        //p.Add(r.Value);
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(string.Format("向数据库写入 竞彩篮球 数据异常 编号：{0}，异常：{1}", r.Value.MatchId, ex.ToString()));
                    }
                }
                if (addSF_SP_List.Count > 0)
                {
                    var category = (int)NoticeCategory.JCLQ_SF_SP;
                    var state = (int)DBChangeState.Add;
                    var param = string.Join("_", (from l in addSF_SP_List select l.MatchId).ToArray());
                    var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                    var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                    //发送 竞彩篮球队伍SF SP 添加 通知
                    var innerKey = string.Format("{0}_{1}", "JCLQ_SF_SP", "Add");
                  // ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.JCLQ_SF_SP);
                    new Sports_Business(this.mDB).UpdateLocalData(param, "", NoticeType.JCLQ_SF_SP, innerKey);
                    //ServiceHelper.SendNotice(issuse_Request, (log) =>
                    //{
                    //    this.WriteLog(log);
                    //});
                }
                ////单关
                //var addSF_SP_List_DS = new List<JCLQ_SF_SP>();
                //foreach (var r in newSF_SP_List_DS)
                //{
                //    try
                //    {
                //        addSF_SP_List_DS.Add(r.Value);
                //        //p.Add(r.Value);
                //    }
                //    catch (Exception ex)
                //    {
                //        this.WriteLog(string.Format("向数据库写入 竞彩篮球 数据异常 编号：{0}，异常：{1}", r.Value.MatchId, ex.ToString()));
                //    }
                //}
                //if (addSF_SP_List_DS.Count > 0)
                //{
                //    var category = (int)NoticeCategory.JCLQ_SF_SP_DS;
                //    var state = (int)DBChangeState.Add;
                //    var param = string.Join("_", (from l in addSF_SP_List_DS select l.MatchId).ToArray());
                //    var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                //    var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                //    //发送 竞彩篮球队伍SF SP 添加 通知
                //    var innerKey = string.Format("{0}_{1}", "JCLQ_SF_SP_DS", "Add");
                //    ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.JCLQ_SF_SP_DS);

                //    //ServiceHelper.SendNotice(issuse_Request, (log) =>
                //    //{
                //    //    this.WriteLog(log);
                //    //});
                //}
                this.WriteLog("2、发送SF SP数据通知 完成");

                #endregion

                #region 发送RFSF SP数据通知

                this.WriteLog("3、开始=>发送RFSF SP数据通知");
                var addRFSF_SP_List = new List<JCLQ_RFSF_SP>();
                foreach (var r in newRFSF_SP_List)
                {
                    try
                    {
                        addRFSF_SP_List.Add(r.Value);
                        //p.Add(r.Value);
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(string.Format("向数据库写入 竞彩篮球 数据异常 编号：{0}，异常：{1}", r.Value.MatchId, ex.ToString()));
                    }
                }
                if (addRFSF_SP_List.Count > 0)
                {
                    var category = (int)NoticeCategory.JCLQ_RFSF_SP;
                    var state = (int)DBChangeState.Add;
                    var param = string.Join("_", (from l in addRFSF_SP_List select l.MatchId).ToArray());
                    var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                    var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                    //发送 竞彩篮球队伍RFSF SP 添加 通知
                    var innerKey = string.Format("{0}_{1}", "JCLQ_RFSF_SP", "Add");
                   // ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.JCLQ_RFSF_SP);
                    new Sports_Business(this.mDB).UpdateLocalData(param, "", NoticeType.JCLQ_RFSF_SP, innerKey);
                    //ServiceHelper.SendNotice(issuse_Request, (log) =>
                    //{
                    //    this.WriteLog(log);
                    //});
                }
                ////单关sp
                //var addRFSF_SP_List_DS = new List<JCLQ_RFSF_SP>();
                //foreach (var r in newRFSF_SP_List_DS)
                //{
                //    try
                //    {
                //        addRFSF_SP_List_DS.Add(r.Value);
                //        //p.Add(r.Value);
                //    }
                //    catch (Exception ex)
                //    {
                //        this.WriteLog(string.Format("向数据库写入 竞彩篮球 数据异常 编号：{0}，异常：{1}", r.Value.MatchId, ex.ToString()));
                //    }
                //}
                //if (addRFSF_SP_List_DS.Count > 0)
                //{
                //    var category = (int)NoticeCategory.JCLQ_RFSF_SP_DS;
                //    var state = (int)DBChangeState.Add;
                //    var param = string.Join("_", (from l in addRFSF_SP_List_DS select l.MatchId).ToArray());
                //    var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                //    var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                //    //发送 竞彩篮球队伍RFSF SP 添加 通知
                //    var innerKey = string.Format("{0}_{1}", "JCLQ_RFSF_SP_DS", "Add");
                //    ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.JCLQ_RFSF_SP_DS);

                //    //ServiceHelper.SendNotice(issuse_Request, (log) =>
                //    //{
                //    //    this.WriteLog(log);
                //    //});
                //}
                this.WriteLog("3、发送RFSF SP数据通知 完成");

                #endregion

                #region 发送SFC SP数据通知

                this.WriteLog("4、开始=>发送SFC SP数据通知");
                var addSFC_SP_List = new List<JCLQ_SFC_SP>();
                foreach (var r in newSFC_SP_List)
                {
                    try
                    {
                        addSFC_SP_List.Add(r.Value);
                        //p.Add(r.Value);
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(string.Format("向数据库写入 竞彩篮球 数据异常 编号：{0}，异常：{1}", r.Value.MatchId, ex.ToString()));
                    }
                }
                if (addSFC_SP_List.Count > 0)
                {
                    var category = (int)NoticeCategory.JCLQ_SFC_SP;
                    var state = (int)DBChangeState.Add;
                    var param = string.Join("_", (from l in addSFC_SP_List select l.MatchId).ToArray());
                    var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                    var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                    //发送 竞彩篮球队伍SFC SP 添加 通知
                    var innerKey = string.Format("{0}_{1}", "JCLQ_SFC_SP", "Add");
                   // ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.JCLQ_SFC_SP);
                    new Sports_Business(this.mDB).UpdateLocalData(param, "", NoticeType.JCLQ_SFC_SP, innerKey);
                    //ServiceHelper.SendNotice(issuse_Request, (log) =>
                    //{
                    //    this.WriteLog(log);
                    //});
                }
                ////单关
                //var addSFC_SP_List_DS = new List<JCLQ_SFC_SP>();
                //foreach (var r in newSFC_SP_List_DS)
                //{
                //    try
                //    {
                //        addSFC_SP_List_DS.Add(r.Value);
                //        //p.Add(r.Value);
                //    }
                //    catch (Exception ex)
                //    {
                //        this.WriteLog(string.Format("向数据库写入 竞彩篮球 数据异常 编号：{0}，异常：{1}", r.Value.MatchId, ex.ToString()));
                //    }
                //}
                //if (addSFC_SP_List_DS.Count > 0)
                //{
                //    var category = (int)NoticeCategory.JCLQ_SFC_SP_DS;
                //    var state = (int)DBChangeState.Add;
                //    var param = string.Join("_", (from l in addSFC_SP_List_DS select l.MatchId).ToArray());
                //    var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                //    var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                //    //发送 竞彩篮球队伍SFC SP 添加 通知
                //    var innerKey = string.Format("{0}_{1}", "JCLQ_SFC_SP_DS", "Add");
                //    ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.JCLQ_SFC_SP_DS);

                //    //ServiceHelper.SendNotice(issuse_Request, (log) =>
                //    //{
                //    //    this.WriteLog(log);
                //    //});
                //}
                this.WriteLog("4、发送SFC SP数据通知 完成");

                #endregion

                #region 发送DXF SP数据通知

                this.WriteLog("5、开始=>发送DXF SP数据通知");
                var addDXF_SP_List = new List<JCLQ_DXF_SP>();
                foreach (var r in newDXF_SP_List)
                {
                    try
                    {
                        addDXF_SP_List.Add(r.Value);
                        //p.Add(r.Value);
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(string.Format("向数据库写入 竞彩篮球 数据异常 编号：{0}，异常：{1}", r.Value.MatchId, ex.ToString()));
                    }
                }
                if (addDXF_SP_List.Count > 0)
                {
                    var category = (int)NoticeCategory.JCLQ_DXF_SP;
                    var state = (int)DBChangeState.Add;
                    var param = string.Join("_", (from l in addDXF_SP_List select l.MatchId).ToArray());
                    var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                    var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                    //发送 竞彩篮球队伍DXF SP 添加 通知
                    var innerKey = string.Format("{0}_{1}", "JCLQ_DXF_SP", "Add");
                    //ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.JCLQ_DXF_SP);
                    new Sports_Business(this.mDB).UpdateLocalData(param, "", NoticeType.JCLQ_DXF_SP, innerKey);
                    //ServiceHelper.SendNotice(issuse_Request, (log) =>
                    //{
                    //    this.WriteLog(log);
                    //});
                }
                ////单关
                //var addDXF_SP_List_DS = new List<JCLQ_DXF_SP>();
                //foreach (var r in newDXF_SP_List_DS)
                //{
                //    try
                //    {
                //        addDXF_SP_List_DS.Add(r.Value);
                //        //p.Add(r.Value);
                //    }
                //    catch (Exception ex)
                //    {
                //        this.WriteLog(string.Format("向数据库写入 竞彩篮球 数据异常 编号：{0}，异常：{1}", r.Value.MatchId, ex.ToString()));
                //    }
                //}
                //if (addDXF_SP_List_DS.Count > 0)
                //{
                //    var category = (int)NoticeCategory.JCLQ_DXF_SP_DS;
                //    var state = (int)DBChangeState.Add;
                //    var param = string.Join("_", (from l in addDXF_SP_List_DS select l.MatchId).ToArray());
                //    var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                //    var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                //    //发送 竞彩篮球队伍DXF SP 添加 通知
                //    var innerKey = string.Format("{0}_{1}", "JCLQ_DXF_SP_DS", "Add");
                //    ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.JCLQ_DXF_SP_DS);

                //    //ServiceHelper.SendNotice(issuse_Request, (log) =>
                //    //{
                //    //    this.WriteLog(log);
                //    //});
                //}
                this.WriteLog("5、发送DXF SP数据通知 完成");

                #endregion

                #region 发送比赛结果通知

                this.WriteLog("6、开始=>发送比赛结果通知");
                var addMatchResultList = new List<JCLQ_MatchResult>();
                var updateMatchResultList = new List<JCLQ_MatchResult>();
                foreach (var r in newMatchResultList)
                {
                    try
                    {
                        if (r.Key == DBChangeState.Add)
                        {
                            addMatchResultList.Add(r.Value);
                            //p.Add(r.Value);
                        }
                        else
                        {
                            updateMatchResultList.Add(r.Value);
                            //p.Modify(r.Value);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(string.Format("向数据库写入 竞彩篮球 数据异常 编号：{0}，异常：{1}", r.Value.MatchId, ex.ToString()));
                    }
                }
                if (addMatchResultList.Count > 0)
                {
                    var category = (int)NoticeCategory.JCLQ_MatchResult;
                    var state = (int)DBChangeState.Add;

                    //期号|比赛编号_结果&比赛编号_结果&比赛编号_结果
                    //其中结果应表示为   DXF:3;RFSF:5;SF:46;
                    var temp = new List<string>();
                    foreach (var item in addMatchResultList)
                    {
                        temp.Add(string.Format("{0}_DXF:{1};RFSF:{2};SF:{3};SFC:{4};", item.MatchId, item.DXF_Result, item.RFSF_Result, item.SF_Result, item.SFC_Result));
                    }
                    var param = string.Join("#", temp.ToArray());

                    var paramT = string.Join("_", (from l in addMatchResultList select l.MatchId).ToArray());
                    var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                    var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, paramT, sign);

                    //发送 竞彩篮球比赛结果 添加 通知
                    var innerKey = string.Format("{0}_{1}", "JCLQ_MatchResult", "Add");
                   // ServiceHelper.AddAndSendNotification(param, paramT, innerKey, NoticeType.JCLQ_MatchResult);
                    new Sports_Business(this.mDB).UpdateLocalData(param, "", NoticeType.JCLQ_MatchResult, innerKey);
                    try
                    {
                        //this.WriteLog("开始生成静态相关数据.");

                        //this.WriteLog("1.生成开奖结果首页");
                        //var log = ServiceHelper.SendBuildStaticFileNotice("301");
                        //this.WriteLog("1.生成开奖结果首页结果：" + log);

                        ////this.WriteLog("2.生成彩种开奖历史");
                        ////log = ServiceHelper.SendBuildStaticFileNotice("302", "JCLQ");
                        ////this.WriteLog("2.生成彩种开奖历史结果：" + log);

                        //this.WriteLog("2.生成彩种开奖详细");
                        //log = ServiceHelper.SendBuildStaticFileNotice("303", "JCLQ");
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
                if (updateMatchResultList.Count > 0)
                {
                    var category = (int)NoticeCategory.JCLQ_MatchResult;
                    var state = (int)DBChangeState.Update;

                    //期号|比赛编号_结果&比赛编号_结果&比赛编号_结果
                    //其中结果应表示为   DXF:3;RFSF:5;SF:46;
                    var temp = new List<string>();
                    foreach (var item in updateMatchResultList)
                    {
                        temp.Add(string.Format("{0}_DXF:{1};RFSF:{2};SF:{3};SFC:{4};", item.MatchId, item.DXF_Result, item.RFSF_Result, item.SF_Result, item.SFC_Result));
                    }
                    var param = string.Join("#", temp.ToArray());

                    var paramT = string.Join("_", (from l in updateMatchResultList select l.MatchId).ToArray());
                    var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                    var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, paramT, sign);

                    //发送 竞彩篮球比赛结果 修改 通知
                    var innerKey = string.Format("{0}_{1}", "JCLQ_MatchResult", "Update");
                    //ServiceHelper.AddAndSendNotification(param, paramT, innerKey, NoticeType.JCLQ_MatchResult);
                    new Sports_Business(this.mDB).UpdateLocalData(param, paramT, NoticeType.JCLQ_MatchResult, innerKey);
                    try
                    {
                        //this.WriteLog("开始生成静态相关数据.");

                        //this.WriteLog("1.生成开奖结果首页");
                        //var log = ServiceHelper.SendBuildStaticFileNotice("301");
                        //this.WriteLog("1.生成开奖结果首页结果：" + log);

                        ////this.WriteLog("2.生成彩种开奖历史");
                        ////log = ServiceHelper.SendBuildStaticFileNotice("302", "JCLQ");
                        ////this.WriteLog("2.生成彩种开奖历史结果：" + log);

                        //this.WriteLog("2.生成彩种开奖详细");
                        //log = ServiceHelper.SendBuildStaticFileNotice("303", "JCLQ");
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
                this.WriteLog("6、发送比赛结果通知 完成");

                #endregion

                this.WriteLog("本次采集所有通知发送完成");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            this.WriteLog("DoWork  完成");
        }

        private void SaveHistory_New<T>(List<T> currentList, string tablename)
         where T : IBallBaseInfo
        {
            tablename = "History_" + tablename;
            //if (string.IsNullOrEmpty(SavePath_New))
            //    SavePath_New = ServiceHelper.Get_JCLQ_SavePath_New();
            foreach (var item in currentList.GroupBy(p => p.MatchData))
            {
                var issuseNumber = item.Key;
                //var path = Path.Combine(SavePath_New, issuseNumber);
                //var customerSavePath = new string[] { "JCLQ", "New", issuseNumber };
                try
                {
                    //    if (!Directory.Exists(path))
                    //        Directory.CreateDirectory(path);
                    //    var fullFileName = Path.Combine(path, fileName);
                    //    ServiceHelper.CreateOrAppend_JSONFile(fullFileName, JsonHelper.Serialize(currentList.Where(p => p.MatchData == issuseNumber).ToList()), (log) =>
                    //    {
                    //        this.WriteLog(log);
                    //    });

                    var coll = mDB.GetCollection<BsonDocument>(tablename);
                    BsonDocument bdDoc = new BsonDocument();
                    bdDoc.Add("CameCode", "JCLQ");
                    bdDoc.Add("IssuseNumber", issuseNumber);
                    bdDoc.Add("Content", JsonHelper.Serialize(currentList.Where(p => p.MatchData == issuseNumber).ToList()));

                    var mFilter = MongoDB.Driver.Builders<BsonDocument>.Filter.Eq("IssuseNumber", issuseNumber);// & Builders<CTZQ_MatchInfo>.Filter.Eq(b => b.IssuseNumber, issuseNumber);

                    coll.DeleteMany(mFilter);
                    coll.InsertOne(bdDoc);
                    //上传文件
                    //ServiceHelper.PostFileToServer(fullFileName, customerSavePath, (log) =>
                    //{
                    //    this.WriteLog(log);
                    //});
                }
                catch (Exception ex)
                {
                    this.WriteLog("保存历史数据失败：" + ex.ToString());
                }
            }
        }

        private List<T> CompareNewJCLQList<T>(List<T> newList, List<T> oldList)
            where T : IBallBaseInfo
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

        public List<JCLQ_MatchInfo> GetMatchList()
        {
            var averageUrl = "http://trade.cpdyj.com/staticdata/lotteryinfo/odds/jclq/jclq.xml";
            var averageContent = PostManager.Get(averageUrl, Encoding.GetEncoding("gb2312"), 0, (request) =>
            {
                if (ServiceHelper.IsUseProxy("JCLQ"))
                {
                    var proxy = ServiceHelper.GetProxyUrl();
                    if (!string.IsNullOrEmpty(proxy))
                    {
                        request.Proxy = new System.Net.WebProxy(proxy);
                    }
                }
            });
            //var averageContent = PostManager.Get(averageUrl, Encoding.Default);

            var averageDoc = new XmlDocument();
            averageDoc.LoadXml(averageContent);
            var averageRoot = averageDoc.SelectSingleNode("xml");

            var sfUrl = "http://trade.cpdyj.com/jclq/getmatch.go?playId=SF&ptype=1&wtype=0";
            var sfXmlContent = PostManager.Get(sfUrl, Encoding.GetEncoding("gb2312"), 0, (request) =>
            {
                if (ServiceHelper.IsUseProxy("JCLQ"))
                {
                    var proxy = ServiceHelper.GetProxyUrl();
                    if (!string.IsNullOrEmpty(proxy))
                    {
                        request.Proxy = new System.Net.WebProxy(proxy);
                    }
                }
            });
            var sfList = FomartMatch(averageRoot, sfXmlContent);
            ServiceHelper.BuildNewMatchList<JCLQ_MatchInfo>(mDB, "JCLQ_Match_SF_List", sfList, null, CompareNewJCLQList, SaveHistory_New);

           // GetNewJCLQList<JCLQ_MatchInfo>(sfList, "Match_SF_List.json");

            var url = "http://trade.cpdyj.com/jclq/getmatch.go?playId=RFSF&ptype=1";
            var xmlContent = PostManager.Get(url, Encoding.GetEncoding("gb2312"), 0, (request) =>
            {
                if (ServiceHelper.IsUseProxy("JCLQ"))
                {
                    var proxy = ServiceHelper.GetProxyUrl();
                    if (!string.IsNullOrEmpty(proxy))
                    {
                        request.Proxy = new System.Net.WebProxy(proxy);
                    }
                }
            });
            var rfsfList = FomartMatch(averageRoot, xmlContent);
         //   GetNewJCLQList<JCLQ_MatchInfo>(rfsfList, "Match_RFSF_List.json");
             ServiceHelper.BuildNewMatchList<JCLQ_MatchInfo>(mDB, "JCLQ_Match_SF_List", rfsfList, null, CompareNewJCLQList, SaveHistory_New);

            return MergerMatch(sfList, rfsfList);
        }

        public List<JCLQ_MatchInfo> GetZS_MatchList()
        {
            var list = new List<JCLQ_MatchInfo>();

            var clUrl = "http://live.qcw.com/apps/jclq";

            var clDoc = new XmlDocument();
            var clContent = PostManager.Get(clUrl, Encoding.UTF8, 0, (request) =>
            {
                if (ServiceHelper.IsUseProxy("JCLQ"))
                {
                    var proxy = ServiceHelper.GetProxyUrl();
                    if (!string.IsNullOrEmpty(proxy))
                    {
                        request.Proxy = new System.Net.WebProxy(proxy);
                    }
                }
            });
            if (clContent == "404")
                return list;
            var dic = JsonHelper.Decode(clContent) as Dictionary<string, object>;
            foreach (var item in dic)
            {
                //lid mid
                var matchId = item.Key;
                var v = item.Value as Dictionary<string, object>;
                var htid = v.ContainsKey("htid") ? v["htid"].ToString() : "0";
                var atid = v.ContainsKey("atid") ? v["atid"].ToString() : "0";
                var sid = v.ContainsKey("sid") ? v["sid"].ToString() : "0";
                var mid = v.ContainsKey("mid") ? v["mid"].ToString() : "0";
                var cl = v.ContainsKey("cl") ? v["cl"].ToString() : "";

                list.Add(new JCLQ_MatchInfo
                {
                    MatchId = matchId,
                    HomeTeamId = int.Parse(htid),
                    GuestTeamId = int.Parse(atid),
                    LeagueId = int.Parse(sid),
                    Mid = int.Parse(mid),
                    LeagueColor = cl,
                    //FXId = int.Parse(sid),
                });
            }
            return list;
        
        }

        public List<JCLQ_MatchInfo> GetMatchList_New()
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (DateTime.Now.Ticks - startTime.Ticks) / 10000;
            var url = string.Format("http://intf.cpdyj.com/data/jclq/match.js?callback=dyj_jclq_match&_={0}", t);
            var content = PostManager.Get(url, Encoding.GetEncoding("gb2312"), 0, (request) =>
            {
                request.Host = "intf.cpdyj.com";
                request.Referer = "	http://jclq.cpdyj.com/";
                if (ServiceHelper.IsUseProxy("JCLQ"))
                {
                    var proxy = ServiceHelper.GetProxyUrl();
                    if (!string.IsNullOrEmpty(proxy))
                    {
                        request.Proxy = new System.Net.WebProxy(proxy);
                    }
                }
            });
            content = content.Replace("dyj_jclq_match(", "").Replace(");", "").Trim();
            if (string.IsNullOrEmpty(content) || content == "404")
                return new List<JCLQ_MatchInfo>();

            //XmlNode lcRoot = null;
            //if (ServiceHelper.Get_JCLQ_TeamInfo())
            //{
            //    var lcUrl = "http://odds.iiioo.com/sdata/jclqxml.php";
            //    var lcDoc = new XmlDocument();
            //    lcDoc.LoadXml(PostManager.Get(lcUrl, Encoding.GetEncoding("gb2312")));
            //    lcRoot = lcDoc.SelectSingleNode("xml");
            //}
            var sfList = new List<JCLQ_MatchInfo>();
            var rfsfList = new List<JCLQ_MatchInfo>();
            var array = JsonHelper.Decode(content);
            //var sfFXDic = ServiceHelper.GetFXIdSource() == "okooo" ? GetJCLQ_FX_OKOOO() : GetJCLQ_FX("SF");
            //var rfsfFXDic = ServiceHelper.GetFXIdSource() == "okooo" ? GetJCLQ_FX_OKOOO() : GetJCLQ_FX("RFSF");

            var zsMathList = GetZS_MatchList();

            foreach (var current in array)
            {
                foreach (var item in current)
                {
                    string state = item[6];
                    if (state == "22222222" || state == "11111111") continue;

                    string matchId = item[0];
                    var zsMatch = zsMathList.FirstOrDefault(p => p.MatchId == matchId);
                    if (matchId.Length < 6) continue;

                    var matchData = matchId.Substring(0, 6);
                    var matchNumber = matchId.Substring(6);
                    string league = item[2];
                    var leagueArray = league.Split(',');
                    string average = item[14];
                    var averageArray = average.Split(',');

                    var homeTeamName = leagueArray[2];
                    var guestTeamName = leagueArray[1];
                    var leagueName = leagueArray[0];
                    //var homeTeam = manager.QueryTeamBasketballEntity(homeTeamName);
                    //var guestTeam = manager.QueryTeamBasketballEntity(guestTeamName);
                    //var leagueEntity = manager.QueryLeagueBasketballEntity(leagueName);

                    var leagueId = zsMatch != null ? zsMatch.LeagueId : 0;
                    var homeTeamId = zsMatch != null ? zsMatch.HomeTeamId : 0;
                    var guestTeamId = zsMatch != null ? zsMatch.GuestTeamId : 0;
                    var fsStopBettingTime = DateTime.Now;
                    fsStopBettingTime = DateTime.Parse(item[4]);
                    var hh = fsStopBettingTime.Hour;
                    var hhm = string.Format("{0}.{1}", fsStopBettingTime.Hour, fsStopBettingTime.Minute);
                    var dateWeek = Convert.ToDateTime(fsStopBettingTime);
                    var week = CaculateWeekDay(dateWeek.Year, dateWeek.Month, dateWeek.Day);

                    while (true)
                    {
                        if (hh >= 0 && decimal.Parse(hhm) <= 9.00M)
                            fsStopBettingTime = (week == "6" || week == "7") ?
                                ((hh >= 1 && decimal.Parse(hhm) <= 9.00M) ? Convert.ToDateTime(string.Format("{0} {1}", fsStopBettingTime.ToString("yyyy-MM-dd"), "01:00:00")).AddMinutes(JCLQ_advanceMinutes).AddDays(-1) : fsStopBettingTime.AddMinutes(JCLQ_advanceMinutes))
                                : Convert.ToDateTime(string.Format("{0} {1}", fsStopBettingTime.ToString("yyyy-MM-dd"), "23:59:00")).AddMinutes(JCLQ_advanceMinutes).AddDays(-1);
                        else
                            fsStopBettingTime = fsStopBettingTime.AddMinutes(JCLQ_advanceMinutes);

                        if ((fsStopBettingTime.Hour >= ((week == "6" || week == "7") ? 1 : 0) && fsStopBettingTime.Minute >= 0)
                            && (fsStopBettingTime.Hour <= 9))
                        {
                            hh = fsStopBettingTime.Hour;
                            hhm = string.Format("{0}.{1}", fsStopBettingTime.Hour, fsStopBettingTime.Minute);
                        }
                        else
                        {
                            break;
                        }
                    }


                    var match = new JCLQ_MatchInfo
                    {
                        MatchId = item[0],
                        MatchIdName = item[1],
                        MatchData = matchData,
                        MatchNumber = matchNumber,
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        LeagueId = leagueId,
                        LeagueName = leagueName,
                        LeagueColor = zsMatch != null ? zsMatch.LeagueColor : string.Empty,
                        HomeTeamName = leagueArray[2],
                        GuestTeamName = leagueArray[1],
                        MatchState = 0,//t1odo 
                        StartDateTime = DateTime.Parse(item[3]).ToString("yyyy-MM-dd HH:mm:ss"),
                        //DSStopBettingTime = DateTime.Parse(item[4]).AddMinutes(JCLQ_advanceMinutes).ToString("yyyy-MM-dd HH:mm:ss"),
                        //FSStopBettingTime = DateTime.Parse(item[5]).AddMinutes(JCLQ_advanceMinutes).ToString("yyyy-MM-dd HH:mm:ss"),
                        DSStopBettingTime = fsStopBettingTime.AddMinutes(-15).ToString("yyyy-MM-dd HH:mm:ss"),
                        FSStopBettingTime = fsStopBettingTime.AddMinutes(-15).ToString("yyyy-MM-dd HH:mm:ss"),
                        AverageWin = string.IsNullOrEmpty(averageArray[0]) ? 0M : decimal.Parse(averageArray[0]),
                        AverageLose = string.IsNullOrEmpty(averageArray[1]) ? 0M : decimal.Parse(averageArray[1]),
                        HomeTeamId = homeTeamId,
                        GuestTeamId = guestTeamId,
                        Mid = zsMatch != null ? zsMatch.Mid : 0,
                        FXId = zsMatch != null ? zsMatch.FXId : 0, //分析Id放的季赛Id
                        PrivilegesType = string.Empty,
                        State = state,
                    };

                    //sfList 只包括 00002000
                    //rfsfList 还包括 20002000
                    if (state != "20002000")
                        sfList.Add(match);
                    rfsfList.Add(match);
                }
            }
            ServiceHelper.BuildNewMatchList<JCLQ_MatchInfo>(mDB, "JCLQ_Match_SF_List", sfList, null, CompareNewJCLQList, SaveHistory_New);
            ServiceHelper.BuildNewMatchList<JCLQ_MatchInfo>(mDB, "JCLQ_Match_SF_List", rfsfList, null, CompareNewJCLQList, SaveHistory_New);

            //GetNewJCLQList<JCLQ_MatchInfo>(sfList, "Match_SF_List.json");

            //GetNewJCLQList<JCLQ_MatchInfo>(rfsfList, "Match_RFSF_List.json");

            return MergerMatch(sfList, rfsfList);
        }
        public string CaculateWeekDay(int y, int m, int d)
        {
            if (m == 1 || m == 2)
            {
                m += 12;
                y--;         //把一月和二月看成是上一年的十三月和十四月，例：如果是2004-1-10则换算成：2003-13-10来代入公式计算。
            }
            int week = (d + 2 * m + 3 * (m + 1) / 5 + y + y / 4 - y / 100 + y / 400) % 7;
            string weekstr = "";
            switch (week)
            {
                case 0: weekstr = "1"; break;
                case 1: weekstr = "2"; break;
                case 2: weekstr = "3"; break;
                case 3: weekstr = "4"; break;
                case 4: weekstr = "5"; break;
                case 5: weekstr = "6"; break;
                case 6: weekstr = "7"; break;
            }
            return weekstr;
        }



        public List<JCLQ_MatchInfo> GetMatchList_ZZJCW_SF()
        {
            _MatchStatus = new Dictionary<string, string>();
            var matchList = new List<JCLQ_MatchInfo>();
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (DateTime.Now.Ticks - startTime.Ticks) / 10000;
            var url = "http://i.sporttery.cn/odds_calculator/get_odds?i_format=json&i_callback=getData&poolcode[]=mnl&_=" + t;
            var content = PostManager.Get(url, Encoding.GetEncoding("gb2312"), 0, null);
            if (string.IsNullOrEmpty(content) || content == "404")
                return matchList;
            content = content.Replace("getData(", "").Replace(");", "").Trim();
            var array = JsonHelper.Decode(content);

            var zsMathList = GetZS_MatchList();
            try
            {
                foreach (var item in array)
                {
                    //if (item.Key != "data")
                    //    continue;
                    var matchlist = item.Value;
                    foreach (var match in matchlist)
                    {
                        var match_D = match.Value;
                        string b_date = "";
                        try
                        {
                            b_date = match_D.b_date;
                            if (string.IsNullOrEmpty(b_date))
                            {
                                continue;
                            }
                        }
                        catch (Exception)
                        {

                            continue;
                        }
                        string num = match_D.num;
                        string matchId = b_date.Substring(2).Replace("-", "") + num.Substring(2);
                        var matchData = matchId.Substring(0, 6);
                        var zsMatch = zsMathList.FirstOrDefault(p => p.MatchId == matchId);
                        var matchNumber = matchId.Substring(6);
                        var matchConfig = DisableMatchConfigList.FirstOrDefault(p => p.MatchId.Trim() == matchId);
                        var state = "0";
                        if (!(match_D.mnl.Value == null))
                        {
                            if (int.Parse(match_D.mnl.single.Value) == 1)
                            {
                                state = "1";
                                var ms = _MatchStatus.Where(p => p.Key == matchId).FirstOrDefault();
                                if (ms.Key != null)
                                {
                                    _MatchStatus.Remove(matchId);
                                    _MatchStatus.Add(matchId, ms.Value + "1");
                                }
                                else
                                {
                                    _MatchStatus.Add(matchId, "1");
                                }
                            }
                        }
                        var homeTeamName = match_D.h_cn;
                        var guestTeamName = match_D.a_cn;
                        var leagueName = match_D.l_cn;

                        var leagueId = zsMatch != null ? zsMatch.LeagueId : 0;
                        var homeTeamId = zsMatch != null ? zsMatch.HomeTeamId : 0;
                        var guestTeamId = zsMatch != null ? zsMatch.GuestTeamId : 0;

                        var fsStopBettingTime = DateTime.Now;
                        var dateTime = string.Format("{0}|{1} {2}", match_D.b_date, match_D.date, match_D.time).Split('|');
                        var hh = Convert.ToDateTime(dateTime[1]).Hour;
                        var hhm = string.Format("{0}.{1}", Convert.ToDateTime(dateTime[1]).Hour, Convert.ToDateTime(dateTime[1]).Minute);
                        var dateWeek = Convert.ToDateTime(dateTime[0]);
                        var week = CaculateWeekDay(dateWeek.Year, dateWeek.Month, dateWeek.Day);
                        var dateWeek2 = Convert.ToDateTime(dateTime[1]);
                        var week2 = CaculateWeekDay(dateWeek2.Year, dateWeek2.Month, dateWeek2.Day);

                        var isTrue = false;

                        var isOzb = false;
                        string date =b_date.Substring(2).Replace("-", "");
                        if (ozbTime.Contains(date))
                            isOzb = true;
                        while (true)
                        {
                            if (isTrue)
                            {
                                fsStopBettingTime = fsStopBettingTime.AddMinutes(JCLQ_advanceMinutes);
                            }
                            else
                            {
                                if (hh >= 0 && decimal.Parse(hhm) <= ((week2 == "6" || week2 == "7" || week2 == "1" || week2 == "2" || week2 == "5") ? 9M : isOzb ? 9M : 7.35M))
                                    fsStopBettingTime = isOzb ?
                                        ((hh >= 3 && decimal.Parse(hhm) < 9.30M) ? Convert.ToDateTime(string.Format("{0} {1}", Convert.ToDateTime(dateTime[1]).ToString("yyyy-MM-dd"), "03:00:00")).AddMinutes(JCLQ_advanceMinutes) : Convert.ToDateTime(dateTime[1]).AddMinutes(JCLQ_advanceMinutes))
                                        : (week == "6" || week == "7") ?
                                        ((hh >= 1 && decimal.Parse(hhm) <= 9.30M) ? Convert.ToDateTime(string.Format("{0} {1}", Convert.ToDateTime(dateTime[1]).ToString("yyyy-MM-dd"), "01:00:00")).AddMinutes(JCLQ_advanceMinutes) : Convert.ToDateTime(dateTime[1]).AddMinutes(JCLQ_advanceMinutes))
                                        : Convert.ToDateTime(string.Format("{0} {1}", dateTime[0], "23:59:00")).AddMinutes(JCLQ_advanceMinutes);
                                else
                                    fsStopBettingTime = Convert.ToDateTime(dateTime[1]).AddMinutes(JCLQ_advanceMinutes);
                                isTrue = true;
                            }

                            var hhm_s = string.Format("{0}.{1}", fsStopBettingTime.Hour, fsStopBettingTime.Minute);
                            if ((fsStopBettingTime.Hour >= (isOzb ? 3 : (week == "6" || week == "7") ? 1 : 0) && fsStopBettingTime.Minute >= 0)
                                && (decimal.Parse(hhm_s) <= ((week2 == "6" || week2 == "7" || week2 == "1" || week2 == "2" || week2 == "5") ? 9M : isOzb ? 9M : 7.35M)))
                            {
                                hh = fsStopBettingTime.Hour;
                                hhm = string.Format("{0}.{1}", fsStopBettingTime.Hour, fsStopBettingTime.Minute);
                            }
                            else
                            {
                                break;
                            }
                        }


                        var match_sf = new JCLQ_MatchInfo
                        {
                            MatchId = matchId,
                            MatchIdName = match_D.num,
                            MatchData = matchData,
                            MatchNumber = matchNumber,
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            LeagueId = leagueId,
                            LeagueName = leagueName,
                            LeagueColor = zsMatch != null ? zsMatch.LeagueColor : string.Empty,
                            HomeTeamName = homeTeamName,
                            GuestTeamName = guestTeamName,
                            MatchState = 0,
                            StartDateTime = DateTime.Parse(string.Format("{0} {1}", match_D.date, match_D.time)).ToString("yyyy-MM-dd HH:mm:ss"),
                            DSStopBettingTime = fsStopBettingTime.AddMinutes(-5).ToString("yyyy-MM-dd HH:mm:ss"),
                            FSStopBettingTime = fsStopBettingTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            AverageWin = zsMatch != null ? zsMatch.AverageWin : 0M,
                            AverageLose = zsMatch != null ? zsMatch.AverageLose : 0M,
                            HomeTeamId = homeTeamId,
                            GuestTeamId = guestTeamId,
                            Mid = zsMatch != null ? zsMatch.Mid : 0,
                            FXId = zsMatch != null ? zsMatch.FXId : 0,
                            PrivilegesType = matchConfig == null ? string.Empty : matchConfig.PrivilegesType,
                            State = state,
                        };
                        matchList.Add(match_sf);
                    }
                }
            }
            catch (Exception ex)
            {
                this.WriteLog("解析array错误 " + ex.ToString());
            }
            return matchList;
        }

        public List<JCLQ_MatchInfo> GetMatchList_ZZJCW_RFSF()
        {
            var matchList = new List<JCLQ_MatchInfo>();
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (DateTime.Now.Ticks - startTime.Ticks) / 10000;
            var url = "http://i.sporttery.cn/odds_calculator/get_odds?i_format=json&i_callback=getData&poolcode[]=hdc&_=" + t;
            var content = PostManager.Get(url, Encoding.GetEncoding("gb2312"), 0, null);
            if (string.IsNullOrEmpty(content) || content == "404")
                return matchList;
            content = content.Replace("getData(", "").Replace(");", "").Trim();
            var array = JsonHelper.Decode(content);

            var zsMathList = GetZS_MatchList();
            try
            {
                foreach (var item in array)
                {
                  
                    var matchlist = item.Value;
                    foreach (var match in matchlist)
                    {
                        var match_D = match.Value;
                        string b_date = "";
                        try
                        {
                            b_date = match_D.b_date;
                            if (string.IsNullOrEmpty(b_date))
                            {
                                continue;
                            }
                        }
                        catch (Exception)
                        {

                            continue;
                        }
                        string num = match_D.num;
                        string matchId = b_date.Substring(2).Replace("-", "") + num.Substring(2);
                        var matchData = matchId.Substring(0, 6);
                        var zsMatch = zsMathList.FirstOrDefault(p => p.MatchId == matchId);
                        var matchNumber = matchId.Substring(6);
                        var matchConfig = DisableMatchConfigList.FirstOrDefault(p => p.MatchId.Trim() == matchId);
                        var state = "0";
                        if (!(match_D.hdc.Value == null))
                        {
                            if (int.Parse(match_D.hdc.single.Value) == 1)
                            {
                                state = "1";
                                var ms = _MatchStatus.Where(p => p.Key == matchId).FirstOrDefault();
                                if (ms.Key != null)
                                {
                                    _MatchStatus.Remove(matchId);
                                    _MatchStatus.Add(matchId, ms.Value + "2");
                                }
                                else
                                {
                                    _MatchStatus.Add(matchId, "2");
                                }
                            }
                        }
                        var homeTeamName = match_D.h_cn;
                        var guestTeamName = match_D.a_cn;
                        var leagueName = match_D.l_cn;

                        var leagueId = zsMatch != null ? zsMatch.LeagueId : 0;
                        var homeTeamId = zsMatch != null ? zsMatch.HomeTeamId : 0;
                        var guestTeamId = zsMatch != null ? zsMatch.GuestTeamId : 0;

                        var fsStopBettingTime = DateTime.Now;
                        var dateTime = string.Format("{0}|{1} {2}", match_D.b_date, match_D.date, match_D.time).Split('|');
                        var hh = Convert.ToDateTime(dateTime[1]).Hour;
                        var hhm = string.Format("{0}.{1}", Convert.ToDateTime(dateTime[1]).Hour, Convert.ToDateTime(dateTime[1]).Minute);
                        var dateWeek = Convert.ToDateTime(dateTime[0]);
                        var week = CaculateWeekDay(dateWeek.Year, dateWeek.Month, dateWeek.Day);

                        var dateWeek2 = Convert.ToDateTime(dateTime[1]);
                        var week2 = CaculateWeekDay(dateWeek2.Year, dateWeek2.Month, dateWeek2.Day);

                        var isTrue = false;
                        var isOzb = false;
                        string date =b_date.Substring(2).Replace("-", "");
                        if (ozbTime.Contains(date))
                            isOzb = true;
                        while (true)
                        {
                            if (isTrue)
                            {
                                fsStopBettingTime = fsStopBettingTime.AddMinutes(JCLQ_advanceMinutes);
                            }
                            else
                            {
                                if (hh >= 0 && decimal.Parse(hhm) <= ((week2 == "6" || week2 == "7" || week2 == "1" || week2 == "2" || week2 == "5") ? 9M : isOzb ? 9M : 7.35M))
                                    fsStopBettingTime = isOzb ?
                                        ((hh >= 3 && decimal.Parse(hhm) < 9.30M) ? Convert.ToDateTime(string.Format("{0} {1}", Convert.ToDateTime(dateTime[1]).ToString("yyyy-MM-dd"), "03:00:00")).AddMinutes(JCLQ_advanceMinutes) : Convert.ToDateTime(dateTime[1]).AddMinutes(JCLQ_advanceMinutes))
                                        : (week == "6" || week == "7") ?
                                        ((hh >= 1 && decimal.Parse(hhm) <= 9.30M) ? Convert.ToDateTime(string.Format("{0} {1}", Convert.ToDateTime(dateTime[1]).ToString("yyyy-MM-dd"), "01:00:00")).AddMinutes(JCLQ_advanceMinutes) : Convert.ToDateTime(dateTime[1]).AddMinutes(JCLQ_advanceMinutes))
                                        : Convert.ToDateTime(string.Format("{0} {1}", dateTime[0], "23:59:00")).AddMinutes(JCLQ_advanceMinutes);
                                else
                                    fsStopBettingTime = Convert.ToDateTime(dateTime[1]).AddMinutes(JCLQ_advanceMinutes);
                                isTrue = true;
                            }

                            var hhm_s = string.Format("{0}.{1}", fsStopBettingTime.Hour, fsStopBettingTime.Minute);
                            if ((fsStopBettingTime.Hour >= (isOzb ? 3 : (week == "6" || week == "7") ? 1 : 0) && fsStopBettingTime.Minute >= 0)
                                && (decimal.Parse(hhm_s) <= ((week2 == "6" || week2 == "7" || week2 == "1" || week2 == "2" || week2 == "5") ? 9M : isOzb ? 9M : 7.35M)))
                            {
                                hh = fsStopBettingTime.Hour;
                                hhm = string.Format("{0}.{1}", fsStopBettingTime.Hour, fsStopBettingTime.Minute);
                            }
                            else
                            {
                                break;
                            }
                        }

                        var match_rfsf = new JCLQ_MatchInfo
                        {
                            MatchId = matchId,
                            MatchIdName = match_D.num,
                            MatchData = matchData,
                            MatchNumber = matchNumber,
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            LeagueId = leagueId,
                            LeagueName = leagueName,
                            LeagueColor = zsMatch != null ? zsMatch.LeagueColor : string.Empty,
                            HomeTeamName = homeTeamName,
                            GuestTeamName = guestTeamName,
                            MatchState = 0,
                            StartDateTime = DateTime.Parse(string.Format("{0} {1}", match_D.date, match_D.time)).ToString("yyyy-MM-dd HH:mm:ss"),
                            DSStopBettingTime = fsStopBettingTime.AddMinutes(-5).ToString("yyyy-MM-dd HH:mm:ss"),
                            FSStopBettingTime = fsStopBettingTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            AverageWin = zsMatch != null ? zsMatch.AverageWin : 0M,
                            AverageLose = zsMatch != null ? zsMatch.AverageLose : 0M,
                            HomeTeamId = homeTeamId,
                            GuestTeamId = guestTeamId,
                            Mid = zsMatch != null ? zsMatch.Mid : 0,
                            FXId = zsMatch != null ? zsMatch.FXId : 0,
                            PrivilegesType = matchConfig == null ? string.Empty : matchConfig.PrivilegesType,
                            State = state,
                        };
                        matchList.Add(match_rfsf);
                    }
                }
            }
            catch (Exception ex)
            {
                this.WriteLog("解析array错误 " + ex.ToString());
            }
            return matchList;
        }

        public List<JCLQ_MatchInfo> GetMatchList_ZZJCW_SFC()
        {
            var matchList = new List<JCLQ_MatchInfo>();
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (DateTime.Now.Ticks - startTime.Ticks) / 10000;
            var url = "http://i.sporttery.cn/odds_calculator/get_odds?i_format=json&i_callback=getData&poolcode[]=wnm&_=" + t;
            var content = PostManager.Get(url, Encoding.GetEncoding("gb2312"), 0, null);
            if (string.IsNullOrEmpty(content) || content == "404")
                return matchList;
            content = content.Replace("getData(", "").Replace(");", "").Trim();
            var array = JsonHelper.Decode(content);

            var zsMathList = GetZS_MatchList();
            try
            {
                foreach (var item in array)
                {
                    
                    var matchlist = item.Value;
                    foreach (var match in matchlist)
                    {
                        var match_D = match.Value;
                        string b_date = "";
                        try
                        {
                            b_date = match_D.b_date;
                            if (string.IsNullOrEmpty(b_date))
                            {
                                continue;
                            }
                        }
                        catch (Exception)
                        {

                            continue;
                        }
                        string num = match_D.num;

                        string matchId = b_date.Substring(2).Replace("-", "") + num.Substring(2);
                        var matchData = matchId.Substring(0, 6);
                        var zsMatch = zsMathList.FirstOrDefault(p => p.MatchId == matchId);
                        var matchNumber = matchId.Substring(6);
                        var matchConfig = DisableMatchConfigList.FirstOrDefault(p => p.MatchId.Trim() == matchId);
                        var state = "0";
                        if (!(match_D.wnm.Value == null))
                        {
                            if (int.Parse(match_D.wnm.single.Value) == 1)
                            {
                                state = "1";
                                var ms = _MatchStatus.Where(p => p.Key == matchId).FirstOrDefault();
                                if (ms.Key != null)
                                {
                                    _MatchStatus.Remove(matchId);
                                    _MatchStatus.Add(matchId, ms.Value + "3");
                                }
                                else
                                {
                                    _MatchStatus.Add(matchId, "3");
                                }
                            }
                        }
                        var homeTeamName = match_D.h_cn;
                        var guestTeamName = match_D.a_cn;
                        var leagueName = match_D.l_cn;

                        var leagueId = zsMatch != null ? zsMatch.LeagueId : 0;
                        var homeTeamId = zsMatch != null ? zsMatch.HomeTeamId : 0;
                        var guestTeamId = zsMatch != null ? zsMatch.GuestTeamId : 0;

                        var fsStopBettingTime = DateTime.Now;
                        var dateTime = string.Format("{0}|{1} {2}", match_D.b_date, match_D.date, match_D.time).Split('|');
                        var hh = Convert.ToDateTime(dateTime[1]).Hour;
                        var hhm = string.Format("{0}.{1}", Convert.ToDateTime(dateTime[1]).Hour, Convert.ToDateTime(dateTime[1]).Minute);
                        var dateWeek = Convert.ToDateTime(dateTime[0]);
                        var week = CaculateWeekDay(dateWeek.Year, dateWeek.Month, dateWeek.Day);
                        var dateWeek2 = Convert.ToDateTime(dateTime[1]);
                        var week2 = CaculateWeekDay(dateWeek2.Year, dateWeek2.Month, dateWeek2.Day);

                        var isTrue = false;
                        var isOzb = false;
                        string date =b_date.Substring(2).Replace("-", "");
                        if (ozbTime.Contains(date))
                            isOzb = true;
                        while (true)
                        {
                            if (isTrue)
                            {
                                fsStopBettingTime = fsStopBettingTime.AddMinutes(JCLQ_advanceMinutes);
                            }
                            else
                            {
                                if (hh >= 0 && decimal.Parse(hhm) <= ((week2 == "6" || week2 == "7" || week2 == "1" || week2 == "2" || week2 == "5") ? 9M : isOzb ? 9M : 7.35M))
                                    fsStopBettingTime = isOzb ?
                                        ((hh >= 3 && decimal.Parse(hhm) < 9.30M) ? Convert.ToDateTime(string.Format("{0} {1}", Convert.ToDateTime(dateTime[1]).ToString("yyyy-MM-dd"), "03:00:00")).AddMinutes(JCLQ_advanceMinutes) : Convert.ToDateTime(dateTime[1]).AddMinutes(JCLQ_advanceMinutes))
                                        : (week == "6" || week == "7") ?
                                        ((hh >= 1 && decimal.Parse(hhm) <= 9.30M) ? Convert.ToDateTime(string.Format("{0} {1}", Convert.ToDateTime(dateTime[1]).ToString("yyyy-MM-dd"), "01:00:00")).AddMinutes(JCLQ_advanceMinutes) : Convert.ToDateTime(dateTime[1]).AddMinutes(JCLQ_advanceMinutes))
                                        : Convert.ToDateTime(string.Format("{0} {1}", dateTime[0], "23:59:00")).AddMinutes(JCLQ_advanceMinutes);
                                else
                                    fsStopBettingTime = Convert.ToDateTime(dateTime[1]).AddMinutes(JCLQ_advanceMinutes);
                                isTrue = true;
                            }

                            var hhm_s = string.Format("{0}.{1}", fsStopBettingTime.Hour, fsStopBettingTime.Minute);
                            if ((fsStopBettingTime.Hour >= (isOzb ? 3 : (week == "6" || week == "7") ? 1 : 0) && fsStopBettingTime.Minute >= 0)
                                && (decimal.Parse(hhm_s) <= ((week2 == "6" || week2 == "7" || week2 == "1" || week2 == "2" || week2 == "5") ? 9M : isOzb ? 9M : 7.35M)))
                            {
                                hh = fsStopBettingTime.Hour;
                                hhm = string.Format("{0}.{1}", fsStopBettingTime.Hour, fsStopBettingTime.Minute);
                            }
                            else
                            {
                                break;
                            }
                        }

                        var match_sfc = new JCLQ_MatchInfo
                        {
                            MatchId = matchId,
                            MatchIdName = match_D.num,
                            MatchData = matchData,
                            MatchNumber = matchNumber,
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            LeagueId = leagueId,
                            LeagueName = leagueName,
                            LeagueColor = zsMatch != null ? zsMatch.LeagueColor : string.Empty,
                            HomeTeamName = homeTeamName,
                            GuestTeamName = guestTeamName,
                            MatchState = 0,
                            StartDateTime = DateTime.Parse(string.Format("{0} {1}", match_D.date, match_D.time)).ToString("yyyy-MM-dd HH:mm:ss"),
                            DSStopBettingTime = fsStopBettingTime.AddMinutes(-5).ToString("yyyy-MM-dd HH:mm:ss"),
                            FSStopBettingTime = fsStopBettingTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            AverageWin = zsMatch != null ? zsMatch.AverageWin : 0M,
                            AverageLose = zsMatch != null ? zsMatch.AverageLose : 0M,
                            HomeTeamId = homeTeamId,
                            GuestTeamId = guestTeamId,
                            Mid = zsMatch != null ? zsMatch.Mid : 0,
                            FXId = zsMatch != null ? zsMatch.FXId : 0,
                            PrivilegesType = matchConfig == null ? string.Empty : matchConfig.PrivilegesType,
                            State = state,
                        };
                        matchList.Add(match_sfc);
                    }
                }
            }
            catch (Exception ex)
            {
                this.WriteLog("解析array错误 " + ex.ToString());
            }
            return matchList;
        }

        public List<JCLQ_MatchInfo> GetMatchList_ZZJCW_DXF()
        {
            var matchList = new List<JCLQ_MatchInfo>();
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (DateTime.Now.Ticks - startTime.Ticks) / 10000;
            var url = "http://i.sporttery.cn/odds_calculator/get_odds?i_format=json&i_callback=getData&poolcode[]=hilo&_=" + t;
            var content = PostManager.Get(url, Encoding.GetEncoding("gb2312"), 0, null);
            if (string.IsNullOrEmpty(content) || content == "404")
                return matchList;
            content = content.Replace("getData(", "").Replace(");", "").Trim();
            var array = JsonHelper.Decode(content);

            var zsMathList = GetZS_MatchList();
            try
            {
                foreach (var item in array)
                {
                    //if (item.Key != "data")
                    //    continue;
                    var matchlist = item.Value;
                    foreach (var match in matchlist)
                    {
                        var match_D = match.Value;
                        string b_date = "";
                        try
                        {
                            b_date = match_D.b_date;
                            if (string.IsNullOrEmpty(b_date))
                            {
                                continue;
                            }
                        }
                        catch (Exception)
                        {

                            continue;
                        }
                        //  string matchId1 = match_D.b_date.Substring(2).Replace("-", "") + match_D.num.Substring(2);
                        //  string b_date = match_D.b_date;
                        string num = match_D.num;
                        string matchId = b_date.Substring(2).Replace("-", "") + num.Substring(2);
                        var matchData = matchId.Substring(0, 6);
                        var zsMatch = zsMathList.FirstOrDefault(p => p.MatchId == matchId);
                        var matchNumber = matchId.Substring(6);
                        var matchConfig = DisableMatchConfigList.FirstOrDefault(p => p.MatchId.Trim() == matchId);
                        var state = "0";
                        if (!(match_D.hilo.Value == null))
                        {
                            if (int.Parse(match_D.hilo.single.Value) == 1)
                            {
                                state = "1";
                                var ms = _MatchStatus.Where(p => p.Key == matchId).FirstOrDefault();
                                if (ms.Key != null)
                                {
                                    _MatchStatus.Remove(matchId);
                                    _MatchStatus.Add(matchId, ms.Value + "4");
                                }
                                else
                                {
                                    _MatchStatus.Add(matchId, "4");
                                }
                            }
                        }
                        var homeTeamName = match_D.h_cn.Value;
                        var guestTeamName = match_D.a_cn.Value;
                        var leagueName = match_D.l_cn.Value;

                        var leagueId = zsMatch != null ? zsMatch.LeagueId : 0;
                        var homeTeamId = zsMatch != null ? zsMatch.HomeTeamId : 0;
                        var guestTeamId = zsMatch != null ? zsMatch.GuestTeamId : 0;

                        var fsStopBettingTime = DateTime.Now;
                        var dateTime = string.Format("{0}|{1} {2}", match_D.b_date, match_D.date, match_D.time).Split('|');
                        var hh = Convert.ToDateTime(dateTime[1]).Hour;
                        var hhm = string.Format("{0}.{1}", Convert.ToDateTime(dateTime[1]).Hour, Convert.ToDateTime(dateTime[1]).Minute);
                        var dateWeek = Convert.ToDateTime(dateTime[0]);
                        var week = CaculateWeekDay(dateWeek.Year, dateWeek.Month, dateWeek.Day);
                        var dateWeek2 = Convert.ToDateTime(dateTime[1]);
                        var week2 = CaculateWeekDay(dateWeek2.Year, dateWeek2.Month, dateWeek2.Day);

                        var isTrue = false;
                        var isOzb = false;
                        string date =b_date.Substring(2).Replace("-", "");
                        if (ozbTime.Contains(date))
                            isOzb = true;
                        while (true)
                        {
                            if (isTrue)
                            {
                                fsStopBettingTime = fsStopBettingTime.AddMinutes(JCLQ_advanceMinutes);
                            }
                            else
                            {
                                if (hh >= 0 && decimal.Parse(hhm) <= ((week2 == "6" || week2 == "7" || week2 == "1" || week2 == "2" || week2 == "5") ? 9M : isOzb ? 9M : 7.35M))
                                    fsStopBettingTime = isOzb ?
                                        ((hh >= 3 && decimal.Parse(hhm) < 9.30M) ? Convert.ToDateTime(string.Format("{0} {1}", Convert.ToDateTime(dateTime[1]).ToString("yyyy-MM-dd"), "03:00:00")).AddMinutes(JCLQ_advanceMinutes) : Convert.ToDateTime(dateTime[1]).AddMinutes(JCLQ_advanceMinutes))
                                        : (week == "6" || week == "7") ?
                                        ((hh >= 1 && decimal.Parse(hhm) <= 9.30M) ? Convert.ToDateTime(string.Format("{0} {1}", Convert.ToDateTime(dateTime[1]).ToString("yyyy-MM-dd"), "01:00:00")).AddMinutes(JCLQ_advanceMinutes) : Convert.ToDateTime(dateTime[1]).AddMinutes(JCLQ_advanceMinutes))
                                        : Convert.ToDateTime(string.Format("{0} {1}", dateTime[0], "23:59:00")).AddMinutes(JCLQ_advanceMinutes);
                                else
                                    fsStopBettingTime = Convert.ToDateTime(dateTime[1]).AddMinutes(JCLQ_advanceMinutes);
                                isTrue = true;
                            }

                            var hhm_s = string.Format("{0}.{1}", fsStopBettingTime.Hour, fsStopBettingTime.Minute);
                            if ((fsStopBettingTime.Hour >= (isOzb ? 3 : (week == "6" || week == "7") ? 1 : 0) && fsStopBettingTime.Minute >= 0)
                                && (decimal.Parse(hhm_s) <= ((week2 == "6" || week2 == "7" || week2 == "1" || week2 == "2" || week2 == "5") ? 9M : isOzb ? 9M : 7.35M)))
                            {
                                hh = fsStopBettingTime.Hour;
                                hhm = string.Format("{0}.{1}", fsStopBettingTime.Hour, fsStopBettingTime.Minute);
                            }
                            else
                            {
                                break;
                            }
                        }
                        var match_dxf = new JCLQ_MatchInfo
                        {
                            MatchId = matchId,
                            MatchIdName = match_D.num,
                            MatchData = matchData,
                            MatchNumber = matchNumber,
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            LeagueId = leagueId,
                            LeagueName = leagueName,
                            LeagueColor = zsMatch != null ? zsMatch.LeagueColor : string.Empty,
                            HomeTeamName = homeTeamName,
                            GuestTeamName = guestTeamName,
                            MatchState = 0,
                            StartDateTime = DateTime.Parse(string.Format("{0} {1}", match_D.date, match_D.time)).ToString("yyyy-MM-dd HH:mm:ss"),
                            DSStopBettingTime = fsStopBettingTime.AddMinutes(-5).ToString("yyyy-MM-dd HH:mm:ss"),
                            FSStopBettingTime = fsStopBettingTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            AverageWin = zsMatch != null ? zsMatch.AverageWin : 0M,
                            AverageLose = zsMatch != null ? zsMatch.AverageLose : 0M,
                            HomeTeamId = homeTeamId,
                            GuestTeamId = guestTeamId,
                            Mid = zsMatch != null ? zsMatch.Mid : 0,
                            FXId = zsMatch != null ? zsMatch.FXId : 0,
                            PrivilegesType = matchConfig == null ? string.Empty : matchConfig.PrivilegesType,
                            State = state,
                        };
                        matchList.Add(match_dxf);
                    }
                }
            }
            catch (Exception ex)
            {
                this.WriteLog("解析array错误 " + ex.ToString());
            }
            return matchList;
        }


        public void GetMatchList_okooo_match()
        {
            if (_Match_Ok_HG.Count > 200)
                _Match_Ok_HG = new Dictionary<string, string>();
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            var url = "http://www.okooo.com/jingcailanqiu/shengfencha/";
            var content = PostManager.Get(url, Encoding.GetEncoding("gb2312"), 0, null);
            if (string.IsNullOrEmpty(content) || content == "404")
                return;

            //step 1 得到div内容
            var index = content.IndexOf("<div class=\"jcmian\" id=\"gametablesend\"");
            content = content.Substring(index);
            index = content.IndexOf("<div id=\"betbottom\" class=\"betstyle\"");
            content = content.Substring(0, index);

            var rows = content.Split(new string[] { "<tr class=\"alltrObj" }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                foreach (var item in rows)
                {
                    if (!item.Contains("/basketball/match/")) continue;

                    var star = item.IndexOf("<span class=\"xh\" title=\"") + "<span class=\"xh\" title=\"".Length;
                    var match_D = item.Substring(star, 5);
                    star = item.IndexOf("title=\"比赛时间：") + "title=\"比赛时间：".Length;
                    var date = DateTime.Parse(item.Substring(star, 10));
                    var T_date = DateTime.Parse(CaculateWeekDay_C(date.Year, date.Month, date.Day, match_D.Substring(0, 2))).ToString("yyyy-MM-dd");
                    string matchId = T_date.Substring(2).Replace("-", "") + match_D.Substring(2);

                    star = item.IndexOf("href=\"/basketball/league/") + "href=\"/basketball/league/".Length;
                    var leagueId = item.Substring(star);
                    star = leagueId.IndexOf("/");
                    leagueId = leagueId.Substring(0, star);

                    star = item.IndexOf("/basketball/match/") + "/basketball/match/".Length;
                    var mid = item.Substring(star);
                    star = mid.IndexOf("/trends/");
                    mid = mid.Substring(0, star);

                    if (_Match_Ok_HG.Keys.Contains(matchId))
                    {
                        _Match_Ok_HG.Remove(matchId);
                        _Match_Ok_HG.Add(matchId, string.Format("{0}^{1}", mid, leagueId));
                    }
                    else
                    {
                        _Match_Ok_HG.Add(matchId, string.Format("{0}^{1}", mid, leagueId));
                    }
                }
            }
            catch (Exception ex)
            {
                this.WriteLog("解析数据错误 " + ex.ToString());
            }
        }

        public string CaculateWeekDay_C(int y, int m, int d, string week_c)
        {
            if (m == 1 || m == 2)
            {
                m += 12;
                y--;         //把一月和二月看成是上一年的十三月和十四月，例：如果是2004-1-10则换算成：2003-13-10来代入公式计算。
            }
            int week = (d + 2 * m + 3 * (m + 1) / 5 + y + y / 4 - y / 100 + y / 400) % 7;
            string weekstr = "";
            switch (week)
            {
                case 0: weekstr = "周一"; break;
                case 1: weekstr = "周二"; break;
                case 2: weekstr = "周三"; break;
                case 3: weekstr = "周四"; break;
                case 4: weekstr = "周五"; break;
                case 5: weekstr = "周六"; break;
                case 6: weekstr = "周日"; break;
            }
            if (weekstr.Equals(week_c))
                return string.Format("{0}-{1}-{2}", y, m, d);
            else
                return CaculateWeekDay_C(y, m, d - 1, week_c);
        }

        private List<JCLQ_MatchInfo> AddZHMPrivilegesType(List<JCLQ_MatchInfo> matchList)
        {
            try
            {
                var code = "016";
                var xml = string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?><msg><head transcode=\"{0}\" partnerid=\"{1}\" version=\"1.0\" time=\"{2}\"></head><body>", code, zhm_PartnerId, DateTime.Now.ToString("yyyyMMddHHmmss"));
                xml += string.Format("<querySchedule type=\"{0}\" />", "jclq");
                xml += "</body></msg>";

                string requestString = string.Format("transcode={0}&msg={1}&key={2}&partnerid={3}", new object[] { code, xml, MessageHelper.GetMd5Body(code + xml + zhm_Key), zhm_PartnerId });
                var resultXml = GetXMl(PostManager.PostCustomer(zhm_ServerUrl, requestString, Encoding.UTF8, (request) =>
                {
                    request.ContentType = "text/xml";
                    if (ServiceHelper.IsUseProxy("JCZQ"))
                    {
                        var proxy = ServiceHelper.GetProxyUrl();
                        if (!string.IsNullOrEmpty(proxy))
                        {
                            request.Proxy = new System.Net.WebProxy(proxy);
                        }
                    }
                }));

                //spfMatch = new List<JCZQ_Match>();
                //brqspfMatch = new List<JCZQ_Match>();

                var doc = new XmlDocument();
                doc.LoadXml(resultXml);
                var root = doc.SelectSingleNode("//body/jcgames");
                if (root == null)
                    return matchList;

                var list = new List<JCLQ_MatchInfo>();
                foreach (XmlNode item in root.ChildNodes)
                {
                    var matchId = item.Attributes["matchID"].Value;
                    var current = matchList.FirstOrDefault(p => p.MatchIdName == matchId);
                    if (current == null) continue;

                    current.PrivilegesType = item.Attributes["PrivilegesType"].Value;
                    list.Add(current);
                }

                return list;
            }
            catch (Exception)
            {
                return matchList;
            }
        }
        private string GetXMl(string xmlstr)
        {
            var match = new Regex("&msg=(?<value0>.*?)&key=").Match(xmlstr);
            if (match.Groups[0].Length > 0)
            {
                return match.Groups[1].Value;
            }
            return "";
        }

        private List<JCLQ_MatchInfo> MergerMatch(List<JCLQ_MatchInfo> one, List<JCLQ_MatchInfo> other)
        {
            var result = new List<JCLQ_MatchInfo>();
            result.AddRange(one);
            foreach (var item in other)
            {
                if (result.FirstOrDefault(p => p.MatchId == item.MatchId) == null)
                    result.Add(item);
            }
            return result;
        }

        private List<JCLQ_MatchInfo> FomartMatch(XmlNode averageRoot, string xmlContent)
        {
            //XmlNode lcRoot = null;
            //if (ServiceHelper.Get_JCLQ_TeamInfo())
            //{
            //    var lcUrl = "http://odds.iiioo.com/sdata/jclqxml.php";
            //    var lcDoc = new XmlDocument();
            //    lcDoc.LoadXml(PostManager.Get(lcUrl, Encoding.GetEncoding("gb2312")));
            //    lcRoot = lcDoc.SelectSingleNode("xml");
            //}

            var list = new List<JCLQ_MatchInfo>();
            var doc = new XmlDocument();
            doc.LoadXml(xmlContent);
            var root = doc.SelectSingleNode("Resp/matches");

            //var sfFXDic = ServiceHelper.GetFXIdSource() == "okooo" ? GetJCLQ_FX_OKOOO() : GetJCLQ_FX("SF");
            //var rfsfFXDic = ServiceHelper.GetFXIdSource() == "okooo" ? GetJCLQ_FX_OKOOO() : GetJCLQ_FX("RFSF");
            var sfFXDic = GetJCLQ_FX("SF");
            var rfsfFXDic = GetJCLQ_FX("RFSF");

            foreach (XmlNode item in root.ChildNodes)
            {
                var matchId = item.Attributes["expectitemid"].Value;
                var averageNode = averageRoot.SelectSingleNode(string.Format("//row[@xid='{0}']", matchId));
                var averageWin = averageNode == null ? 0 : averageNode.Attributes["oh"].Value.GetDecimal();
                var averageLose = averageNode == null ? 0 : averageNode.Attributes["oa"].Value.GetDecimal();
                var color = averageNode == null ? "FF0000" : averageNode.Attributes["cl"].Value;

                //var team = lcRoot == null ? null : lcRoot.SelectSingleNode(string.Format("//row[@xid='{0}']", matchId));

                var homeTeamName = item.Attributes["hostteam"].Value;
                var guestTeamName = item.Attributes["visitteam"].Value;
                var leagueName = item.Attributes["leaguename"].Value;
                //var homeTeam = manager.QueryTeamBasketballEntity(homeTeamName);
                //var guestTeam = manager.QueryTeamBasketballEntity(guestTeamName);
                //var league = manager.QueryLeagueEntity(leagueName);
                //var leagueNameBK = manager.QueryLeagueBasketballEntity(leagueName);
                var fxId = sfFXDic.ContainsKey(matchId)
                       ? int.Parse(string.IsNullOrEmpty(sfFXDic[matchId]) ? "0" : sfFXDic[matchId])
                       : (rfsfFXDic.ContainsKey(matchId) ? int.Parse(string.IsNullOrEmpty(rfsfFXDic[matchId]) ? "0" : rfsfFXDic[matchId]) : 0);
                list.Add(new JCLQ_MatchInfo
                {
                    MatchId = matchId,
                    MatchIdName = item.Attributes["itemname"].Value,
                    MatchData = item.Attributes["expect"].Value,
                    MatchNumber = item.Attributes["itemid"].Value,
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    LeagueId = 0,//leagueNameBK == null ? 0 : leagueNameBK.sclassID, //team == null ? item.Attributes["leagueid"].Value.GetInt32() : team.Attributes["lid"].Value.GetInt32(),
                    LeagueName = leagueName,
                    LeagueColor = color,
                    HomeTeamName = homeTeamName,
                    GuestTeamName = guestTeamName,
                    MatchState = item.Attributes["state"].Value.GetInt32(),
                    StartDateTime = item.Attributes["matchtime"].Value.GetDateTime().ToString("yyyy-MM-dd HH:mm:ss"),
                    DSStopBettingTime = item.Attributes["dsendtime"].Value.GetDateTime().AddMinutes(JCLQ_advanceMinutes).ToString("yyyy-MM-dd HH:mm:ss"),
                    FSStopBettingTime = item.Attributes["fsendtime"].Value.GetDateTime().AddMinutes(JCLQ_advanceMinutes).ToString("yyyy-MM-dd HH:mm:ss"),
                    AverageWin = averageWin,
                    AverageLose = averageLose,
                    HomeTeamId = 0,// homeTeam == null ? 0 : int.Parse(homeTeam.ID),// team == null ? 0 : team.Attributes["htid"].Value.GetInt32(),
                    GuestTeamId = 0,// guestTeam == null ? 0 : int.Parse(guestTeam.ID),// team == null ? 0 : team.Attributes["gtid"].Value.GetInt32(),
                    Mid = 0,//ServiceHelper.GetLeagueCategoryID(leagueNameBK),//team == null ? 0 : team.Attributes["mid"].Value.GetInt32(),

                    FXId = fxId,
                });
            }
            return list;
        }

        public List<JCLQ_MatchResult> GetMatchResultList()
        {
            var list = new List<JCLQ_MatchResult>();
            var url = "http://trade.cpdyj.com/jclq/getopeninfo.go";
            var page = 1;
            var totalPage = 1;
            var pagesize = 50;
            var totalCount = 50;
            var startDate = DateTime.Now.AddDays(-JCLQ_Result_Day).ToString("yyyy-MM-dd");
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
                    var matchId = item.Attributes["expectItemID"].Value;
                    var matchData = matchId.Substring(0, 6);
                    var matchNumber = matchId.Replace(matchData, "");
                    var state = item.Attributes["state"].Value;
                    if (state != "2")
                    {
                        //异常的比赛
                        //3 是取消的比赛   todo 其它
                        list.Add(new JCLQ_MatchResult
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            MatchId = matchId,
                            MatchData = matchData,
                            MatchNumber = matchNumber,
                            MatchState = state,
                            HomeScore = 0,
                            GuestScore = 0,
                            RFSF_Trend = item.Attributes["rfsfGdReslut"].Value,
                            DXF_Trend = item.Attributes["dxfGdReslut"].Value,
                            SF_Result = string.Empty,
                            SF_SP = 1.0M,
                            RFSF_Result = string.Empty,
                            RFSF_SP = 1.0M,
                            SFC_Result = string.Empty,
                            SFC_SP = 1.0M,
                            DXF_Result = string.Empty,
                            DXF_SP = 1.0M,
                        });
                        continue;
                    }
                    var bf = item.Attributes["qcScore"].Value;
                    var bfArray = new string[] { "", "" };
                    if (!string.IsNullOrEmpty(bf))
                        bfArray = bf.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    var homeScore = bfArray.Length == 2 ? bfArray[1].GetInt32() : 0;
                    var guestScore = bfArray.Length == 2 ? bfArray[0].GetInt32() : 0;
                    var sfResult = item.Attributes["sfReslut"].Value;
                    while (true)
                    {
                        if (homeScore > guestScore)
                        {
                            sfResult = "3";
                            break;
                        }
                        if (homeScore == guestScore)
                        {
                            sfResult = "1";
                            break;
                        }
                        sfResult = "0";
                        break;
                    }
                    var sf_sp = item.Attributes["sfMoney"].Value.GetDecimal();
                    if (sf_sp == 0M)
                        sf_sp = 1M;
                    list.Add(new JCLQ_MatchResult
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        MatchId = matchId,
                        MatchData = matchData,
                        MatchNumber = matchNumber,
                        MatchState = state,
                        HomeScore = homeScore,
                        GuestScore = guestScore,
                        RFSF_Trend = item.Attributes["rfsfGdReslut"].Value,
                        DXF_Trend = item.Attributes["dxfGdReslut"].Value,
                        SF_Result = sfResult,
                        SF_SP = sf_sp,
                        RFSF_Result = item.Attributes["rfsfReslut"].Value,
                        RFSF_SP = item.Attributes["rfsfMoney"].Value.GetDecimal(),
                        SFC_Result = item.Attributes["sfcReslut"].Value,
                        SFC_SP = item.Attributes["sfcMoney"].Value.GetDecimal(),
                        DXF_Result = item.Attributes["dxfReslut"].Value,//0为小，3为大
                        DXF_SP = item.Attributes["dxfMoney"].Value.GetDecimal(),
                    });
                }

                page++;
            }
            return list;
        }

        public List<JCLQ_MatchResult> GetMatchResultListFromAiCai()
        {
            var list = new List<JCLQ_MatchResult>();

            var url = "http://www.aicai.com/lottery/jcReport!lcMatchResult.jhtml";
            var content = PostManager.Get(url, Encoding.UTF8, 0, (request) =>
            {
                if (ServiceHelper.IsUseProxy("JCLQ"))
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

            var rows = content.Split(new string[] { "<tr id='tra" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var row in rows)
            {
                #region 变量定义

                var matchIdName = string.Empty;
                var matchDate = string.Empty;
                var matchNumber = string.Empty;
                var matchId = string.Empty;
                //2 是正常；3 是取消的比赛；4 是延期
                var matchState = "2";
                var fullHomeTeamScore = 0;
                var fullGuestTeamScore = 0;

                var rf_Pan = 0M;
                var dxf_Pan = 0M;
                var rfsf_Trend = "-1";
                var dxf_Trend = "-1";

                var sf_Result = "-1";
                var sf_SP = 1.0M;
                var rfsf_Result = "-1";
                var rfsf_SP = 1.0M;
                var sfc_Result = "-1";
                var sfc_SP = 1.0M;
                var dxf_Result = "-1";
                var dxf_SP = 1.0M;

                #endregion

                #region 比赛编号

                flag = "onmouseover=";
                flagIndex = row.IndexOf(flag);
                if (flagIndex < 0) continue;

                var rowContent = row.Substring(flagIndex);
                flag = "class=\"xx_bg";
                flagIndex = rowContent.IndexOf(flag);
                rowContent = rowContent.Substring(flagIndex);
                rowContent = rowContent.Replace(flag, "");
                flag = ">";
                flagIndex = rowContent.IndexOf(flag);
                rowContent = rowContent.Substring(flagIndex);
                rowContent = rowContent.Replace(flag, "");

                flag = "</td";
                flagIndex = rowContent.IndexOf(flag);
                matchIdName = rowContent.Substring(0, flagIndex).Replace("\r\n", "").Replace("\t", "").Replace(" ", "");
                matchDate = GetMatchDate(matchIdName, false);
                matchNumber = matchIdName.Substring(2);
                matchId = matchDate + matchNumber;

                #endregion

                #region 比赛状态 和 比分

                flag = "strong class=\"red\"";
                flagIndex = rowContent.IndexOf(flag);
                rowContent = rowContent.Substring(flagIndex);
                rowContent = rowContent.Replace(flag, "");
                flag = "</strong";
                flagIndex = rowContent.IndexOf(flag);
                var bf = rowContent.Substring(0, flagIndex);
                if (string.IsNullOrEmpty(bf) || string.IsNullOrWhiteSpace(bf))
                {
                    matchState = "4";
                }
                if (bf.Trim() == "*")
                {
                    matchState = "3";
                }
                var bfArray = bf.Split(':');
                if (bfArray.Length == 2)
                {
                    fullHomeTeamScore = int.Parse(bfArray[1]);
                    fullGuestTeamScore = int.Parse(bfArray[0]);
                }

                #endregion

                #region 盘口信息

                if (matchState == "2")
                {
                    flag = "<div class=\"text_line\"";
                    flagIndex = rowContent.IndexOf(flag);
                    rowContent = rowContent.Substring(flagIndex);

                    flag = "过关固定奖金";
                    flagIndex = rowContent.IndexOf(flag);
                    rowContent = rowContent.Substring(0, flagIndex);
                    //盘口 和 sp
                    var resultRows = rowContent.Split(new string[] { "</tr" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < resultRows.Length; i++)
                    {
                        var r = resultRows[i];
                        switch (i)
                        {
                            case 0:
                                //取盘口信息
                                flag = "<div class=\"text_line\"";
                                var pArray = r.Split(new string[] { "</div" }, StringSplitOptions.RemoveEmptyEntries);
                                rfsf_Trend = pArray[0].Replace(flag, "").Replace("\r\n", "").Replace("\t", "").Replace(" ", "");
                                rf_Pan = decimal.Parse(rfsf_Trend.Substring(4));
                                rfsf_Trend = rfsf_Trend.Insert(4, "|") + ";";

                                flagIndex = pArray[1].IndexOf(flag);
                                dxf_Trend = pArray[1].Substring(flagIndex).Replace(flag, "").Replace("\r\n", "").Replace("\t", "").Replace(" ", "");
                                dxf_Pan = decimal.Parse(dxf_Trend.Substring(1));
                                dxf_Trend = dxf_Trend.Insert(1, "|") + ";";

                                break;
                            case 1:
                                //取sp值
                                var spArray = r.Split(new string[] { "</td" }, StringSplitOptions.RemoveEmptyEntries);
                                flag = "<td";
                                for (int j = 0; j < spArray.Length; j++)
                                {
                                    var sp = spArray[j].Replace("\r\n", "").Replace("\t", "").Replace(" ", "").Trim();
                                    if (string.IsNullOrEmpty(sp)) continue;
                                    if (!sp.StartsWith(flag)) continue;
                                    sp = sp.Replace(flag, "");
                                    if (string.IsNullOrWhiteSpace(sp) || string.IsNullOrEmpty(sp)) continue;
                                    switch (j)
                                    {
                                        case 0:
                                            break;
                                        case 1:
                                            sf_SP = decimal.Parse(sp);
                                            break;
                                        case 2:
                                            rfsf_SP = decimal.Parse(sp);
                                            break;
                                        case 3:
                                            sfc_SP = decimal.Parse(sp);
                                            break;
                                        case 4:
                                            dxf_SP = decimal.Parse(sp);
                                            break;
                                        case 5:
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            case 2:
                                break;
                            default:
                                break;
                        }
                    }

                    sf_Result = fullHomeTeamScore > fullGuestTeamScore ? "3" : "0";
                    rfsf_Result = fullHomeTeamScore + rf_Pan > fullGuestTeamScore ? "3" : "0";
                    dxf_Result = fullHomeTeamScore + fullGuestTeamScore > dxf_Pan ? "3" : "0";
                    var sfc = fullHomeTeamScore - fullGuestTeamScore;
                    while (true)
                    {
                        if (sfc >= 1 && sfc <= 5)
                        {
                            sfc_Result = "01";
                            break;
                        }
                        if (sfc >= 6 && sfc <= 10)
                        {
                            sfc_Result = "02";
                            break;
                        }
                        if (sfc >= 11 && sfc <= 15)
                        {
                            sfc_Result = "03";
                            break;
                        }
                        if (sfc >= 16 && sfc <= 20)
                        {
                            sfc_Result = "04";
                            break;
                        }
                        if (sfc >= 21 && sfc <= 25)
                        {
                            sfc_Result = "05";
                            break;
                        }
                        if (sfc >= 26)
                        {
                            sfc_Result = "06";
                            break;
                        }
                        if (sfc >= -5 && sfc <= -1)
                        {
                            sfc_Result = "11";
                            break;
                        }
                        if (sfc >= -10 && sfc <= -6)
                        {
                            sfc_Result = "12";
                            break;
                        }
                        if (sfc >= -15 && sfc <= -11)
                        {
                            sfc_Result = "13";
                            break;
                        }
                        if (sfc >= -20 && sfc <= -16)
                        {
                            sfc_Result = "14";
                            break;
                        }
                        if (sfc >= -25 && sfc <= -21)
                        {
                            sfc_Result = "15";
                            break;
                        }
                        if (sfc <= -26)
                        {
                            sfc_Result = "16";
                            break;
                        }
                    }
                }
                #endregion

                list.Add(new JCLQ_MatchResult
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    HomeScore = fullHomeTeamScore,
                    GuestScore = fullGuestTeamScore,
                    MatchId = matchId,
                    MatchData = matchDate,
                    MatchNumber = matchNumber,
                    MatchState = matchState,
                    DXF_Trend = dxf_Trend,
                    RFSF_Trend = rfsf_Trend,
                    SF_Result = sf_Result,
                    SF_SP = sf_SP,
                    RFSF_Result = rfsf_Result,
                    RFSF_SP = rfsf_SP,
                    SFC_Result = sfc_Result,
                    SFC_SP = sfc_SP,
                    DXF_Result = dxf_Result,
                    DXF_SP = dxf_SP,
                });
            }

            return list;
        }

        public List<JCLQ_MatchResult> GetMatchResultListFrom310Win()
        {
            var list = new List<JCLQ_MatchResult>();
            //过关数据
            //var url = "http://www.310win.com/jingcailanqiu/kaijiang_jclq_all_2.html";
            //单关数据
            //var url = "http://www.310win.com/jingcailanqiu/kaijiang_jclq_all.html";
            var url = "http://www.310win.com/jingcailanqiu/kaijiang_jclq_all_2.html";
            //var content = PostManager.Post(url, string.Format("txtStartDate={0}&txtEndDate={1}", "2012-10-27", "	2012-10-31"), Encoding.UTF8);
            var content = PostManager.Get(url, Encoding.UTF8, 0, (request) =>
            {
                if (ServiceHelper.IsUseProxy("JCLQ"))
                {
                    var proxy = ServiceHelper.GetProxyUrl();
                    if (!string.IsNullOrEmpty(proxy))
                    {
                        request.Proxy = new System.Net.WebProxy(proxy);
                    }
                }
            });
            if (string.IsNullOrEmpty(content) || content == "404")
                return list;
            //Console.WriteLine(content);
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
            //content = content.Substring(0, index + 5);
            content = content.Substring(0, index);
            //Console.WriteLine(content);

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
                    var fullHomeTeamScore = 0;
                    var fullGuestTeamScore = 0;

                    var rfsf_Trend = string.Empty;
                    var dxf_Trend = string.Empty;
                    var sf_Result = string.Empty;
                    var sf_SP = 1.0M;
                    var rfsf_Result = string.Empty;
                    var rfsf_SP = 1.0M;
                    var sfc_Result = string.Empty;
                    var sfc_SP = 1.0M;
                    var dxf_Result = string.Empty;
                    var dxf_SP = 1.0M;

                    var tds = row.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                    #region 解析每行数据
                    for (int i = 0; i < tds.Length; i++)
                    {
                        var td = tds[i].Trim();
                        if (!td.Contains("<td")) continue;
                        //Console.WriteLine(td);

                        index = td.IndexOf(">");
                        var tdContent = td.Substring(index + 1).Replace("\r\n", string.Empty);
                        //Console.WriteLine(tdContent);

                        switch (i)
                        {
                            case 0:
                                //比赛名称
                                //周一009 
                                matchIdName = tdContent;
                                matchDate = GetMatchDate(tdContent, false);
                                matchNumber = tdContent.Substring(2);
                                matchId = matchDate + matchNumber;
                                break;
                            case 1:
                                //联赛名称
                                //NBA
                                leagueName = tdContent;
                                break;
                            case 2:
                                //比赛时间
                                //10月31日10:30 
                                break;
                            case 3:
                                //主队名称
                                //埃尔夫斯堡
                                homeTeamName = tdContent;
                                break;
                            case 4:
                                //全场比分
                                //<b>2-0</b> 或 中文
                                //2 是正常；3 是取消的比赛；4 是延期
                                switch (tdContent)
                                {
                                    case "推迟":
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
                            case 5:
                                //客队名称
                                //耶夫勒
                                guestTeamName = tdContent;
                                break;
                            case 6:
                                //SF
                                tempArray = tdContent.Split(new string[] { "<br />" }, StringSplitOptions.RemoveEmptyEntries);
                                if (matchState == "2" && tempArray.Length == 2)
                                {
                                    index = tempArray[0].IndexOf(">");
                                    tempArray[0] = tempArray[0].Substring(index + 1);
                                    sf_Result = FormatRFSFResult(tempArray[0].Replace("</span>", ""));
                                    //sf_Result = fullHomeTeamScore > fullGuestTeamScore ? "3" : "0";
                                    if (string.IsNullOrEmpty(sf_Result))
                                    {
                                        sf_Result = "-1";
                                    }
                                    index = tempArray[1].IndexOf("<u>");
                                    tempArray[1] = tempArray[1].Substring(index + 3);
                                    tempArray[1] = tempArray[1].Replace("</u></a>", "");
                                    if (string.IsNullOrEmpty(tempArray[1]))
                                        sf_SP = 0M;
                                    else
                                        decimal.TryParse(tempArray[1], out sf_SP);
                                }
                                if (matchState == "3")
                                {
                                    sf_Result = "-1";
                                    sf_SP = 1M;
                                }
                                break;
                            case 7:
                                //让分盘口
                                //<b>-5.5</u></a></b>
                                // 让分主负|-8.5;
                                var rf = 0M;
                                decimal.TryParse(tdContent.Replace("<b>", "").Replace("</u></a></b>", ""), out rf);
                                rfsf_Trend = fullHomeTeamScore + rf > fullGuestTeamScore ? string.Format("让分主胜|{0};", rf) : string.Format("让分主负|{0};", rf);

                                break;
                            case 8:
                                //RFSF
                                //<span style='color:#f00;'>2:0</span><br /><a href="/sp/zuqiu/48654_102.html" target="_blank"><u>6.5</u></a>

                                tempArray = tdContent.Split(new string[] { "<br />" }, StringSplitOptions.RemoveEmptyEntries);
                                if (matchState == "2" && tempArray.Length == 2)
                                {
                                    index = tempArray[0].IndexOf(">");
                                    tempArray[0] = tempArray[0].Substring(index + 1);
                                    rfsf_Result = FormatRFSFResult(tempArray[0].Replace("</span>", ""));
                                    if (string.IsNullOrEmpty(rfsf_Result))
                                    {
                                        rfsf_Result = "-1";
                                    }

                                    index = tempArray[1].IndexOf("<u>");
                                    tempArray[1] = tempArray[1].Substring(index + 3);
                                    tempArray[1] = tempArray[1].Replace("</u></a>", "");
                                    if (string.IsNullOrEmpty(tempArray[1]))
                                        rfsf_SP = 0M;
                                    else
                                        decimal.TryParse(tempArray[1], out rfsf_SP);
                                }
                                if (matchState == "3")
                                {
                                    rfsf_Result = "-1";
                                    rfsf_SP = 1M;
                                }
                                break;
                            case 9:
                                //SFC
                                //<span style='color:#f00;'>负6-10分</span><br /><a href="/sp/lanqiu/9156_113.html" target="_blank"><u>9.5</u></a>
                                tempArray = tdContent.Split(new string[] { "<br />" }, StringSplitOptions.RemoveEmptyEntries);
                                if (matchState == "2" && tempArray.Length == 2)
                                {
                                    index = tempArray[0].IndexOf(">");
                                    tempArray[0] = tempArray[0].Substring(index + 1);
                                    sfc_Result = FormatSFCResult(tempArray[0].Replace("</span>", ""));
                                    if (string.IsNullOrEmpty(sfc_Result))
                                    {
                                        sfc_Result = "-1";
                                    }

                                    index = tempArray[1].IndexOf("<u>");
                                    tempArray[1] = tempArray[1].Substring(index + 3);
                                    tempArray[1] = tempArray[1].Replace("</u></a>", "");
                                    if (string.IsNullOrEmpty(tempArray[1]))
                                        sfc_SP = 0M;
                                    else
                                        decimal.TryParse(tempArray[1], out sfc_SP);
                                }
                                if (matchState == "3")
                                {
                                    sfc_Result = "-1";
                                    sfc_SP = 1M;
                                }
                                break;
                            case 10:
                                //总分盘口
                                //"<b>189.5</b>"
                                //大 | 186.5;
                                var dxf = 0M;
                                decimal.TryParse(tdContent.Replace("<b>", "").Replace("</b>", ""), out dxf);
                                dxf_Trend = fullHomeTeamScore + fullGuestTeamScore > dxf ? string.Format("大|{0};", dxf) : string.Format("小|{0};", dxf);
                                break;
                            case 11:
                                //DXF
                                // <td bgcolor="#f0f0d0"><span style='color:#f00;'>大</span><br /><a href="/sp/lanqiu/9156_114.html" target="_blank"><u>2.54</u></a></td>
                                tempArray = tdContent.Split(new string[] { "<br />" }, StringSplitOptions.RemoveEmptyEntries);
                                if (matchState == "2" && tempArray.Length == 2)
                                {
                                    index = tempArray[0].IndexOf(">");
                                    tempArray[0] = tempArray[0].Substring(index + 1);
                                    dxf_Result = FormatDXFResult(tempArray[0].Replace("</span>", ""));
                                    if (string.IsNullOrEmpty(dxf_Result))
                                    {
                                        dxf_Result = "-1";
                                    }

                                    index = tempArray[1].IndexOf("<u>");
                                    tempArray[1] = tempArray[1].Substring(index + 3);
                                    tempArray[1] = tempArray[1].Replace("</u></a>", "");
                                    if (string.IsNullOrEmpty(tempArray[1]))
                                        dxf_SP = 0M;
                                    else
                                        decimal.TryParse(tempArray[1], out dxf_SP);
                                }
                                if (matchState == "3")
                                {
                                    dxf_Result = "-1";
                                    dxf_SP = 1M;
                                }
                                break;
                            case 12:
                                break;
                            default:
                                break;
                        }
                    }
                    #endregion

                    if (string.IsNullOrEmpty(matchState)) continue;
                    if (matchState == "2" && (sf_SP == 1M && rfsf_SP == 1M && sfc_SP == 1M && dxf_SP == 1M)) continue;
                    if (sf_Result == "" && sfc_Result == "" && rfsf_Result == "" && dxf_Result == "") continue;

                    list.Add(new JCLQ_MatchResult
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        HomeScore = fullHomeTeamScore,
                        GuestScore = fullGuestTeamScore,
                        MatchId = matchId,
                        MatchData = matchDate,
                        MatchNumber = matchNumber,
                        MatchState = matchState,
                        DXF_Trend = dxf_Trend,
                        RFSF_Trend = rfsf_Trend,
                        SF_Result = sf_Result,
                        SF_SP = sf_SP,
                        RFSF_Result = rfsf_Result,
                        RFSF_SP = rfsf_SP,
                        SFC_Result = sfc_Result,
                        SFC_SP = sfc_SP,
                        DXF_Result = dxf_Result,
                        DXF_SP = dxf_SP,
                    });
                }
                catch (Exception ex)
                {
                    this.WriteLog("解析表格数据失败：" + ex.ToString());
                }
            }
            return list;
        }

        public List<JCLQ_MatchResult> GetMatchResultListFromZZJCW()
        {
            var list = new List<JCLQ_MatchResult>();
            var url = "http://info.sporttery.cn/basketball/match_result.php";
            var encoding = Encoding.GetEncoding("gb2312");
            var content = PostManager.Get(url, encoding, 0, null);
            if (string.IsNullOrEmpty(content) || content == "404")
                return list;
            //Console.WriteLine(content);
            var tempArray = new string[] { };
            //step 1 得到div内容
            var index = content.IndexOf("<tr class='tr1'><td class=\"txc first\">");
            content = content.Substring(index);
            index = content.IndexOf("</table></div>");
            content = content.Substring(0, index);

            var rows = content.Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in rows)
            {
                try
                {
                    var row = item.Trim();
                    if (!row.Contains("<tr")) continue;

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
                    var fullHomeTeamScore = 0;
                    var fullGuestTeamScore = 0;

                    var rfsf_Trend = string.Empty;
                    var dxf_Trend = string.Empty;
                    var sf_Result = string.Empty;
                    var sf_SP = 1.0M;
                    var rfsf_Result = string.Empty;
                    var rfsf_SP = 1.0M;
                    var sfc_Result = string.Empty;
                    var sfc_SP = 1.0M;
                    var dxf_Result = string.Empty;
                    var dxf_SP = 1.0M;

                    var tds = row.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                    var bf = string.Empty;

                    #region 解析每行数据
                    for (int i = 0; i < tds.Length; i++)
                    {
                        var td = tds[i].Trim();
                        if (!td.Contains("<td")) continue;
                        //Console.WriteLine(td);

                        index = td.IndexOf(">");
                        var tdContent = td.Substring(index + 1).Replace("\r\n", string.Empty);
                        //Console.WriteLine(tdContent);

                        switch (i)
                        {
                            case 1:
                                //比赛名称
                                //周一009 
                                matchIdName = tdContent;
                                matchDate = GetMatchDate(tdContent, false);
                                matchNumber = tdContent.Substring(2);
                                matchId = matchDate + matchNumber;
                                break;
                            case 8:
                                bf = tdContent;
                                break;
                            case 9:
                                //全场比分
                                //<b>2-0</b> 或 中文
                                //2 是正常；3 是取消的比赛；4 是延期
                                switch (tdContent)
                                {
                                    case "推迟":
                                        matchState = "4";
                                        break;
                                    case "取消":
                                        matchState = "3";
                                        break;
                                    case "已完成":
                                        matchState = "2";
                                        break;
                                }
                                tempArray = bf.Split(':');
                                if (matchState == "2" && tempArray.Length == 2)
                                {
                                    fullHomeTeamScore = int.Parse(tempArray[0].Replace("<b>", "").Trim());
                                    fullGuestTeamScore = int.Parse(tempArray[1].Replace("</b>", "").Trim());
                                }
                                if (matchState == "3")
                                {
                                    fullHomeTeamScore = -1;
                                    fullGuestTeamScore = -1;
                                    sf_Result = "-1";
                                    rfsf_Result = "-1";
                                    sfc_Result = "-1";
                                    dxf_Result = "-1";
                                }
                                break;
                            case 10:
                                if (string.IsNullOrEmpty(tdContent) || tdContent == "取消") continue;
                                var start = tdContent.IndexOf("id=");
                                var end = tdContent.IndexOf("target=");
                                tdContent = tdContent.Substring(start, end - start).Replace("id=", "").Replace("\"", "").Trim();
                                GetResultAndSP_ZZJCW(tdContent, out rfsf_Trend, out dxf_Trend, out sf_Result, out rfsf_Result, out sfc_Result, out dxf_Result, out sf_SP, out rfsf_SP, out sfc_SP, out dxf_SP);
                                break;
                            default:
                                break;
                        }
                    }
                    #endregion

                    if (string.IsNullOrEmpty(matchState)) continue;
                    list.Add(new JCLQ_MatchResult
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        HomeScore = fullHomeTeamScore,
                        GuestScore = fullGuestTeamScore,
                        MatchId = matchId,
                        MatchData = matchDate,
                        MatchNumber = matchNumber,
                        MatchState = matchState,
                        DXF_Trend = dxf_Trend,
                        RFSF_Trend = rfsf_Trend,
                        SF_Result = sf_Result,
                        SF_SP = sf_SP,
                        RFSF_Result = rfsf_Result,
                        RFSF_SP = rfsf_SP,
                        SFC_Result = sfc_Result,
                        SFC_SP = sfc_SP,
                        DXF_Result = dxf_Result,
                        DXF_SP = dxf_SP,
                    });
                }
                catch (Exception ex)
                {
                    this.WriteLog("解析表格数据失败：" + ex.ToString());
                }
            }
            return list;
        }
        public void GetResultAndSP_ZZJCW(string id, out string rfsf_Trend, out string dxf_Trend, out string sf_Result, out string rfsf_Result, out string sfc_Result, out string dxf_Result
            , out decimal sf_SP, out decimal rfsf_SP, out decimal sfc_SP, out decimal dxf_SP)
        {
            rfsf_Trend = string.Empty;
            dxf_Trend = string.Empty;
            sf_Result = string.Empty;
            sf_SP = 1.0M;
            rfsf_Result = string.Empty;
            rfsf_SP = 1.0M;
            sfc_Result = string.Empty;
            sfc_SP = 1.0M;
            dxf_Result = string.Empty;
            dxf_SP = 1.0M;

            var url = string.Format("http://info.sporttery.cn/basketball/pool_result.php?id={0}", id);
            var encoding = Encoding.GetEncoding("gb2312");
            var content = PostManager.Get(url, encoding, 0, null);
            var tempArray = new string[] { };
            //step 1 得到div内容
            var index = content.IndexOf("<table");
            content = content.Substring(index);
            index = content.LastIndexOf("</table>");
            content = content.Substring(0, index + "</table>".Length);

            var tables = content.Split(new string[] { "</table>" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < tables.Length; i++)
            {
                var table = tables[i];
                var trsL = table.Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);
                switch (i)
                {
                    case 0:
                        #region 胜负
                        var sfresult = trsL[1];
                        index = sfresult.LastIndexOf("</font>");
                        sfresult = sfresult.Substring(0, index);
                        index = sfresult.LastIndexOf(">");
                        sfresult = sfresult.Substring(index + 1);
                        sf_Result = FormatRFSFResult(sfresult.Replace("主负", "客胜"));
                        var sp = trsL[trsL.Length - 2];
                        var tds = sp.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                        if (sf_Result == "0")
                            decimal.TryParse(tds[1].Substring(tds[1].IndexOf(">") + 1).Replace("\r\n", string.Empty).Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowDown.gif\" />", "").Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowUp.gif\" />", ""), out sf_SP);
                        if (sf_Result == "3")
                            decimal.TryParse(tds[2].Substring(tds[2].IndexOf(">") + 1).Replace("\r\n", string.Empty).Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowDown.gif\" />", "").Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowUp.gif\" />", ""), out sf_SP);
                        #endregion
                        break;
                    case 1:
                        #region 让分胜负
                        sp = trsL[trsL.Length - 2];
                        tds = sp.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                        var rfsfresult = tds[4];
                        index = rfsfresult.LastIndexOf("</font>");
                        rfsfresult = rfsfresult.Substring(0, index);
                        index = rfsfresult.LastIndexOf(">");
                        rfsfresult = rfsfresult.Substring(index + 1);
                        rfsf_Result = FormatRFSFResult(rfsfresult.Replace("让分", "").Replace("主负", "客胜"));
                        var rfsftrend = tds[2].Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowDown.gif\" />", "").Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowUp.gif\" />", "");
                        index = rfsftrend.LastIndexOf("</font>");
                        rfsftrend = rfsftrend.Substring(0, index);
                        index = rfsftrend.LastIndexOf(">");
                        rfsftrend = rfsftrend.Substring(index + 1);
                        rfsf_Trend = string.Format("{0}|{1}", rfsfresult, rfsftrend.Replace("+", ""));
                        if (rfsf_Result == "0")
                            decimal.TryParse(tds[1].Substring(tds[1].IndexOf(">") + 1).Replace("\r\n", string.Empty).Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowDown.gif\" />", "").Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowUp.gif\" />", ""), out rfsf_SP);
                        if (rfsf_Result == "3")
                            decimal.TryParse(tds[3].Substring(tds[3].IndexOf(">") + 1).Replace("\r\n", string.Empty).Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowDown.gif\" />", "").Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowUp.gif\" />", ""), out rfsf_SP);
                        #endregion
                        break;
                    case 2:
                        #region 大小分
                        sp = trsL[trsL.Length - 2];
                        tds = sp.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                        var dxfresult = tds[4];
                        index = dxfresult.LastIndexOf("</font>");
                        dxfresult = dxfresult.Substring(0, index);
                        index = dxfresult.LastIndexOf(">");
                        dxfresult = dxfresult.Substring(index + 1);
                        dxf_Result = FormatDXFResult(dxfresult);
                        var dxftrend = tds[2].Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowDown.gif\" />", "").Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowUp.gif\" />", "");
                        index = dxftrend.LastIndexOf("</font>");
                        dxftrend = dxftrend.Substring(0, index);
                        index = dxftrend.LastIndexOf(">");
                        dxftrend = dxftrend.Substring(index + 1);
                        dxf_Trend = string.Format("{0}|{1}", dxfresult, dxftrend.Replace("+", ""));
                        if (dxf_Result == "3")
                            decimal.TryParse(tds[1].Substring(tds[1].IndexOf(">") + 1).Replace("\r\n", string.Empty).Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowDown.gif\" />", "").Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowUp.gif\" />", ""), out dxf_SP);
                        if (dxf_Result == "0")
                            decimal.TryParse(tds[3].Substring(tds[3].IndexOf(">") + 1).Replace("\r\n", string.Empty).Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowDown.gif\" />", "").Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowUp.gif\" />", ""), out dxf_SP);
                        #endregion
                        break;
                    case 3:
                        #region 胜分差
                        var sfcresult = trsL[1];
                        index = sfcresult.LastIndexOf("</font>");
                        sfcresult = sfcresult.Substring(0, index);
                        index = sfcresult.LastIndexOf(">");
                        sfcresult = sfcresult.Substring(index + 1);
                        sfc_Result = FormatSFCResult_ZZJCW(sfcresult);
                        sp = trsL[trsL.Length - 2];
                        tds = sp.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                        if (sfc_Result == "11")
                            decimal.TryParse(tds[1].Substring(tds[1].IndexOf(">") + 1).Replace("\r\n", string.Empty).Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowDown.gif\" />", "").Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowUp.gif\" />", ""), out sfc_SP);
                        if (sfc_Result == "12")
                            decimal.TryParse(tds[2].Substring(tds[2].IndexOf(">") + 1).Replace("\r\n", string.Empty).Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowDown.gif\" />", "").Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowUp.gif\" />", ""), out sfc_SP);
                        if (sfc_Result == "13")
                            decimal.TryParse(tds[3].Substring(tds[3].IndexOf(">") + 1).Replace("\r\n", string.Empty).Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowDown.gif\" />", "").Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowUp.gif\" />", ""), out sfc_SP);
                        if (sfc_Result == "14")
                            decimal.TryParse(tds[4].Substring(tds[4].IndexOf(">") + 1).Replace("\r\n", string.Empty).Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowDown.gif\" />", "").Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowUp.gif\" />", ""), out sfc_SP);
                        if (sfc_Result == "15")
                            decimal.TryParse(tds[5].Substring(tds[5].IndexOf(">") + 1).Replace("\r\n", string.Empty).Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowDown.gif\" />", "").Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowUp.gif\" />", ""), out sfc_SP);
                        if (sfc_Result == "16")
                            decimal.TryParse(tds[6].Substring(tds[6].IndexOf(">") + 1).Replace("\r\n", string.Empty).Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowDown.gif\" />", "").Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowUp.gif\" />", ""), out sfc_SP);
                        if (sfc_Result == "01")
                            decimal.TryParse(tds[7].Substring(tds[7].IndexOf(">") + 1).Replace("\r\n", string.Empty).Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowDown.gif\" />", "").Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowUp.gif\" />", ""), out sfc_SP);
                        if (sfc_Result == "02")
                            decimal.TryParse(tds[8].Substring(tds[8].IndexOf(">") + 1).Replace("\r\n", string.Empty).Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowDown.gif\" />", "").Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowUp.gif\" />", ""), out sfc_SP);
                        if (sfc_Result == "03")
                            decimal.TryParse(tds[9].Substring(tds[9].IndexOf(">") + 1).Replace("\r\n", string.Empty).Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowDown.gif\" />", "").Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowUp.gif\" />", ""), out sfc_SP);
                        if (sfc_Result == "04")
                            decimal.TryParse(tds[10].Substring(tds[10].IndexOf(">") + 1).Replace("\r\n", string.Empty).Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowDown.gif\" />", "").Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowUp.gif\" />", ""), out sfc_SP);
                        if (sfc_Result == "05")
                            decimal.TryParse(tds[11].Substring(tds[11].IndexOf(">") + 1).Replace("\r\n", string.Empty).Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowDown.gif\" />", "").Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowUp.gif\" />", ""), out sfc_SP);
                        if (sfc_Result == "06")
                            decimal.TryParse(tds[12].Substring(tds[12].IndexOf(">") + 1).Replace("\r\n", string.Empty).Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowDown.gif\" />", "").Replace("<img src=\"http://static.sporttery.cn/info/football/info/images2/ArrowUp.gif\" />", ""), out sfc_SP);

                        #endregion
                        break;
                    default:
                        break;
                }
            }
        }


        public List<JCLQ_MatchResult> GetMatchResultListFromOk()
        {
            var list = new List<JCLQ_MatchResult>();
            var url = string.Format("http://www.okooo.com/jingcailanqiu/kaijiang/?StartDate={0}&EndDate={1}", DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd"));
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

            foreach (var item in rows)
            {
                if (!item.Contains("<tr align=\"center\"")) continue;
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
                    var homeTeamName = string.Empty;
                    var guestTeamName = string.Empty;
                    //2 是正常；3 是取消的比赛；4 是延期
                    var matchState = string.Empty;
                    var fullHomeTeamScore = 0;
                    var fullGuestTeamScore = 0;
                    var Score = string.Empty;

                    var rfsf_Trend = string.Empty;
                    var dxf_Trend = string.Empty;
                    var sf_Result = string.Empty;
                    var sf_SP = 1.0M;
                    var rfsf_Result = string.Empty;
                    var rfsf_SP = 1.0M;
                    var sfc_Result = string.Empty;
                    var sfc_SP = 1.0M;
                    var dxf_Result = string.Empty;
                    var dxf_SP = 1.0M;

                    var tds = row.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                    #region 解析每行数据
                    for (int i = 0; i < tds.Length; i++)
                    {
                        var td = tds[i].Trim();
                        if (!td.Contains("<td")) continue;
                        //Console.WriteLine(td);

                        //index = td.IndexOf(">");
                        //var tdContent = td.Substring(index + 1).Replace("\r\n", string.Empty);
                        //Console.WriteLine(tdContent);
                        var tdContent = CutHtml(td);
                        switch (i)
                        {
                            case 0:
                                //比赛名称
                                //周一009 
                                matchIdName = tdContent;
                                matchDate = GetMatchDate(tdContent, false);
                                matchNumber = tdContent.Substring(2);
                                matchId = matchDate + matchNumber;
                                break;
                            case 1:
                                //联赛名称
                                //NBA
                                leagueName = tdContent;
                                break;
                            case 2:
                                //比赛时间
                                //10月31日10:30 
                                break;
                            case 3:
                                //主队名称
                                //埃尔夫斯堡
                                homeTeamName = tdContent;
                                break;
                            case 4:
                                //客队名称
                                //耶夫勒
                                guestTeamName = tdContent;
                                break;
                            case 5:
                                //全场比分
                                //<b>2-0</b> 或 中文
                                if (tdContent == "-") continue;
                                Score = tdContent.Replace("-", ":");
                                matchState = "2";
                                fullHomeTeamScore = int.Parse(Score.Split(':')[1]);
                                fullGuestTeamScore = int.Parse(Score.Split(':')[0]);
                                break;
                            case 7:
                                if (string.IsNullOrEmpty(Score)) continue;
                                if (string.IsNullOrEmpty(tdContent))
                                {
                                    rfsf_Result = "-1";
                                    rfsf_SP = 1M;
                                }
                                else
                                {
                                    rfsf_Result = tdContent.Replace("让分主负(", "").Replace("让分主胜(", "").Replace(")", "");
                                }
                                break;
                            case 8:
                                if (string.IsNullOrEmpty(Score)) continue;
                                if (rfsf_Result == "-1")
                                {
                                    if (string.IsNullOrEmpty(tdContent))
                                        rfsf_SP = 0M;
                                    continue;
                                }
                                rfsf_SP = decimal.Parse(tdContent);
                                break;
                            case 10:
                                if (string.IsNullOrEmpty(Score)) continue;
                                if (string.IsNullOrEmpty(tdContent))
                                {
                                    dxf_Result = "-1";
                                    dxf_SP = 1M;
                                }
                                else
                                {
                                    dxf_Result = tdContent.Replace("大分", "3").Replace("小分", "0");
                                }
                                break;
                            case 11:
                                if (string.IsNullOrEmpty(Score)) continue;
                                if (dxf_Result == "-1")
                                {
                                    if (string.IsNullOrEmpty(tdContent))
                                        dxf_SP = 0M;
                                    continue;
                                }
                                dxf_SP = decimal.Parse(tdContent);
                                break;
                            case 12:
                                if (string.IsNullOrEmpty(Score)) continue;
                                if (string.IsNullOrEmpty(tdContent))
                                {
                                    sf_Result = "-1";
                                    sf_SP = 1M;
                                }
                                else
                                {
                                    sf_Result = tdContent;
                                }
                                break;
                            case 13:
                                if (string.IsNullOrEmpty(Score)) continue;
                                if (sf_Result == "-1")
                                {
                                    if (string.IsNullOrEmpty(tdContent))
                                        sf_SP = 0M;
                                    continue;
                                }
                                sf_SP = decimal.Parse(tdContent);
                                break;
                            case 14:
                                if (string.IsNullOrEmpty(Score)) continue;
                                if (string.IsNullOrEmpty(tdContent))
                                {
                                    sfc_Result = "-1";
                                    sfc_SP = 1M;
                                }
                                else
                                {
                                    sfc_Result = FormatSFCResult_ZZJCW(tdContent.Replace("主", "主胜").Replace("客", "客胜"));
                                }
                                break;
                            case 15:
                                if (string.IsNullOrEmpty(Score)) continue;
                                if (sfc_Result == "-1")
                                {
                                    if (string.IsNullOrEmpty(tdContent))
                                        sfc_SP = 0M;
                                    continue;
                                }
                                sfc_SP = decimal.Parse(tdContent);
                                break;
                            default:
                                break;
                        }
                    }
                    #endregion

                    if (string.IsNullOrEmpty(matchState)) continue;
                    if (matchState == "2" && (sf_SP == 1M && rfsf_SP == 1M && sfc_SP == 1M && dxf_SP == 1M)) continue;
                    if (sf_Result == "" && sfc_Result == "" && rfsf_Result == "" && dxf_Result == "") continue;

                    list.Add(new JCLQ_MatchResult
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        HomeScore = fullHomeTeamScore,
                        GuestScore = fullGuestTeamScore,
                        MatchId = matchId,
                        MatchData = matchDate,
                        MatchNumber = matchNumber,
                        MatchState = matchState,
                        DXF_Trend = dxf_Trend,
                        RFSF_Trend = rfsf_Trend,
                        SF_Result = sf_Result,
                        SF_SP = sf_SP,
                        RFSF_Result = rfsf_Result,
                        RFSF_SP = rfsf_SP,
                        SFC_Result = sfc_Result,
                        SFC_SP = sfc_SP,
                        DXF_Result = dxf_Result,
                        DXF_SP = dxf_SP,
                    });
                }
                catch (Exception ex)
                {
                    this.WriteLog("解析表格数据失败：" + ex.ToString());
                }
            }
            return list;
        }

        public List<JCLQ_MatchResult> GetMatchResultListFrom500wan()
        {
            var list = new List<JCLQ_MatchResult>();
            for (int i = 0; i < 3; i++)
            {

                var date = DateTime.Now.AddDays(-i).ToString("yyyy-MM-dd");
                var url = string.Format("http://zx.500.com/jclq/kaijiang.php?playid=0&d={0}", date);
                var content = PostManager.Get(url, Encoding.Default, 0, (request) =>
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
                    //if (!item.Contains("<tr align=\"center\"")) continue;
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
                        var homeTeamName = string.Empty;
                        var guestTeamName = string.Empty;
                        //2 是正常；3 是取消的比赛；4 是延期
                        var matchState = string.Empty;
                        var fullHomeTeamScore = 0;
                        var fullGuestTeamScore = 0;
                        var Score = string.Empty;

                        var rfsf_Trend = string.Empty;
                        var dxf_Trend = string.Empty;
                        var sf_Result = string.Empty;
                        var sf_SP = 1.0M;
                        var rfsf_Result = string.Empty;
                        var rfsf_SP = 1.0M;
                        var sfc_Result = string.Empty;
                        var sfc_SP = 1.0M;
                        var dxf_Result = string.Empty;
                        var dxf_SP = 1.0M;

                        var tds = row.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                        #region 解析每行数据
                        for (int j = 0; j < tds.Length; j++)
                        {
                            var td = tds[j].Trim();
                            if (!td.Contains("<td")) continue;
                            //Console.WriteLine(td);

                            //index = td.IndexOf(">");
                            //var tdContent = td.Substring(index + 1).Replace("\r\n", string.Empty);
                            //Console.WriteLine(tdContent);
                            var tdContent = CutHtml(td);
                            switch (j)
                            {
                                case 0:
                                    //比赛名称
                                    //周一009 
                                    matchIdName = tdContent;
                                    matchDate = GetMatchDate(tdContent, false);
                                    matchNumber = tdContent.Substring(2);
                                    matchId = matchDate + matchNumber;
                                    break;
                                case 1:
                                    //联赛名称
                                    //NBA
                                    leagueName = tdContent;
                                    break;
                                case 2:
                                    //比赛时间
                                    //10月31日10:30 
                                    break;
                                case 3:
                                    guestTeamName = tdContent;
                                    break;
                                case 5:
                                    homeTeamName = tdContent;
                                    break;
                                case 6:
                                    //全场比分
                                    if (tdContent == "-") continue;
                                    Score = tdContent;
                                    matchState = "2";
                                    fullHomeTeamScore = int.Parse(Score.Split(':')[1]);
                                    fullGuestTeamScore = int.Parse(Score.Split(':')[0]);
                                    break;
                                case 7:
                                    if (string.IsNullOrEmpty(Score)) continue;
                                    if (string.IsNullOrEmpty(tdContent))
                                    {
                                        sf_Result = "-1";
                                        sf_SP = 1M;
                                    }
                                    else
                                    {
                                        sf_Result = FormatRFSFResult(tdContent.Replace("主负", "客胜"));
                                    }
                                    break;
                                case 8:
                                    //if (string.IsNullOrEmpty(Score)) continue;
                                    //if (sf_Result == "-1")
                                    //{
                                    //    if (string.IsNullOrEmpty(tdContent) || tdContent == "--")
                                    //        sf_SP = 1M;
                                    //    continue;
                                    //}
                                    //sf_SP = decimal.Parse(tdContent);
                                    break;
                                case 11:
                                    if (string.IsNullOrEmpty(Score)) continue;
                                    if (string.IsNullOrEmpty(tdContent))
                                    {
                                        rfsf_Result = "-1";
                                        rfsf_SP = 1M;
                                    }
                                    else
                                    {
                                        rfsf_Result = FormatRFSFResult(tdContent.Replace("主负", "客胜"));
                                    }
                                    break;
                                case 12:
                                    //if (string.IsNullOrEmpty(Score)) continue;
                                    //if (rfsf_Result == "-1")
                                    //{
                                    //    if (string.IsNullOrEmpty(tdContent) || tdContent == "--")
                                    //        rfsf_SP = 1M;
                                    //    continue;
                                    //}
                                    //rfsf_SP = decimal.Parse(tdContent);
                                    break;
                                case 14:
                                    if (string.IsNullOrEmpty(Score)) continue;
                                    if (string.IsNullOrEmpty(tdContent))
                                    {
                                        sfc_Result = "-1";
                                        sfc_SP = 1M;
                                    }
                                    else
                                    {
                                        sfc_Result = FormatSFCResult_ZZJCW(tdContent);
                                    }
                                    break;
                                case 15:
                                    //if (string.IsNullOrEmpty(Score)) continue;
                                    //if (sfc_Result == "-1")
                                    //{
                                    //    if (string.IsNullOrEmpty(tdContent) || tdContent == "--")
                                    //        sfc_SP = 1M;
                                    //    continue;
                                    //}
                                    //sfc_SP = decimal.Parse(tdContent);
                                    break;
                                case 18:
                                    if (string.IsNullOrEmpty(Score)) continue;
                                    if (string.IsNullOrEmpty(tdContent))
                                    {
                                        dxf_Result = "-1";
                                        dxf_SP = 1M;
                                    }
                                    else
                                    {
                                        dxf_Result = FormatDXFResult(tdContent);
                                    }
                                    break;
                                case 19:
                                    //if (string.IsNullOrEmpty(Score)) continue;
                                    //if (dxf_Result == "-1")
                                    //{
                                    //    if (string.IsNullOrEmpty(tdContent) || tdContent == "--")
                                    //        dxf_SP = 1M;
                                    //    continue;
                                    //}
                                    //dxf_SP = decimal.Parse(tdContent);
                                    break;
                                default:
                                    break;
                            }
                        }
                        #endregion

                        if (string.IsNullOrEmpty(matchState)) continue;
                        if (sf_Result == "" && sfc_Result == "" && rfsf_Result == "" && dxf_Result == "") continue;

                        list.Add(new JCLQ_MatchResult
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            HomeScore = fullHomeTeamScore,
                            GuestScore = fullGuestTeamScore,
                            MatchId = matchId,
                            MatchData = matchDate,
                            MatchNumber = matchNumber,
                            MatchState = matchState,
                            DXF_Trend = dxf_Trend,
                            RFSF_Trend = rfsf_Trend,
                            SF_Result = sf_Result,
                            SF_SP = sf_SP,
                            RFSF_Result = rfsf_Result,
                            RFSF_SP = rfsf_SP,
                            SFC_Result = sfc_Result,
                            SFC_SP = sfc_SP,
                            DXF_Result = dxf_Result,
                            DXF_SP = dxf_SP,
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

        private string FormatRFSFResult(string content)
        {
            if (content.Contains("无开售"))
                return "-1";
            switch (content)
            {
                case "主胜":
                    return "3";
                case "客胜":
                    return "0";
                default:
                    break;
            }
            return content;
        }
        private string FormatSFCResult(string content)
        {
            if (content.Contains("无开售"))
                return "-1";
            switch (content)
            {
                case "胜1-5分":
                    return "01";
                case "胜6-10分":
                    return "02";
                case "胜11-15分":
                    return "03";
                case "胜16-20分":
                    return "04";
                case "胜21-25分":
                    return "05";
                case "胜26分以上":
                    return "06";
                case "负1-5分":
                    return "11";
                case "负6-10分":
                    return "12";
                case "负11-15分":
                    return "13";
                case "负16-20分":
                    return "14";
                case "负21-25分":
                    return "15";
                case "负26分以上":
                    return "16";
                default:
                    break;
            }

            return content;
        }
        private string FormatSFCResult_ZZJCW(string content)
        {
            if (content.Contains("无开售"))
                return "-1";
            switch (content)
            {
                case "主胜1-5":
                    return "01";
                case "主胜6-10":
                    return "02";
                case "主胜11-15":
                    return "03";
                case "主胜16-20":
                    return "04";
                case "主胜21-25":
                    return "05";
                case "主胜26+":
                    return "06";
                case "客胜1-5":
                    return "11";
                case "客胜6-10":
                    return "12";
                case "客胜11-15":
                    return "13";
                case "客胜16-20":
                    return "14";
                case "客胜21-25":
                    return "15";
                case "客胜26+":
                    return "16";
                default:
                    break;
            }

            return content;
        }
        private string FormatDXFResult(string content)
        {
            if (content.Contains("无开售"))
                return "-1";
            switch (content)
            {
                case "大":
                    return "3";
                case "小":
                    return "0";
                default:
                    break;
            }
            return content;
        }

        public string GetMatchDate(string matchIdName, bool isFuture)
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

        private List<JCLQ_SF_SP> Get_SF_SP()
        {
            var list = new List<JCLQ_SF_SP>();
            var url = "http://trade.cpdyj.com/jclq/getMatchLqSp.go?playId=SF&ptype=1";
            var xmlContent = PostManager.Get(url, Encoding.GetEncoding("gb2312"), 0, (request) =>
            {
                if (ServiceHelper.IsUseProxy("JCLQ"))
                {
                    var proxy = ServiceHelper.GetProxyUrl();
                    if (!string.IsNullOrEmpty(proxy))
                    {
                        request.Proxy = new System.Net.WebProxy(proxy);
                    }
                }
            });
            var doc = new XmlDocument();
            doc.LoadXml(xmlContent);
            var root = doc.SelectSingleNode("Resp/matches");
            foreach (XmlNode item in root.ChildNodes)
            {
                var matchId = item.Attributes["expectitemid"].Value;
                var matchData = matchId.Substring(0, 6);
                var matchNumber = matchId.Replace(matchData, "");
                list.Add(new JCLQ_SF_SP
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    MatchId = matchId,
                    MatchData = matchData,
                    MatchNumber = matchNumber,
                    WinSP = item.Attributes["sf3"].Value.GetDecimal(),
                    LoseSP = item.Attributes["sf0"].Value.GetDecimal(),
                });
            }

            return list;
        }

        private List<JCLQ_SF_SP> Get_SF_SP_New(bool isDS)
        {
            var json = GetSPJsonContent("SF", isDS);
            var list = new List<JCLQ_SF_SP>();
            if (string.IsNullOrEmpty(json))
                return list;

            var array = JsonHelper.Decode(json);
            foreach (var item in array)
            {
                if (item.Length != 3) continue;
                if (string.IsNullOrEmpty(item[0])) continue;
                if (string.IsNullOrEmpty(item[1])) continue;
                if (string.IsNullOrEmpty(item[2])) continue;

                string matchId = item[0];
                var matchData = matchId.Substring(0, 6);
                var matchNumber = matchId.Replace(matchData, "");

                list.Add(new JCLQ_SF_SP
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    MatchData = matchData,
                    MatchId = matchId,
                    MatchNumber = matchNumber,
                    WinSP = decimal.Parse(item[1]),
                    LoseSP = decimal.Parse(item[2]),
                });

            }
            return list;
        }

        private List<JCLQ_RFSF_SP> Get_RFSF_SP()
        {
            var rfsfUrl = "http://trade.cpdyj.com/jclq/getmatch.go?playId=RFSF&ptype=1";
            var rfsfContent = PostManager.Get(rfsfUrl, Encoding.GetEncoding("gb2312"), 0, (request) =>
            {
                if (ServiceHelper.IsUseProxy("JCLQ"))
                {
                    var proxy = ServiceHelper.GetProxyUrl();
                    if (!string.IsNullOrEmpty(proxy))
                    {
                        request.Proxy = new System.Net.WebProxy(proxy);
                    }
                }
            });
            var rfsfDoc = new XmlDocument();
            rfsfDoc.LoadXml(rfsfContent);

            var list = new List<JCLQ_RFSF_SP>();
            var url = "http://trade.cpdyj.com/jclq/getMatchLqSp.go?playId=RFSF&ptype=1";
            var xmlContent = PostManager.Get(url, Encoding.GetEncoding("gb2312"), 0, (request) =>
            {
                if (ServiceHelper.IsUseProxy("JCLQ"))
                {
                    var proxy = ServiceHelper.GetProxyUrl();
                    if (!string.IsNullOrEmpty(proxy))
                    {
                        request.Proxy = new System.Net.WebProxy(proxy);
                    }
                }
            });
            var doc = new XmlDocument();
            doc.LoadXml(xmlContent);
            var root = doc.SelectSingleNode("Resp/matches");
            foreach (XmlNode item in root.ChildNodes)
            {
                var matchId = item.Attributes["expectitemid"].Value;
                var matchData = matchId.Substring(0, 6);
                var matchNumber = matchId.Replace(matchData, "");
                var rfsfNode = rfsfDoc.SelectSingleNode(string.Format("//match[@expectitemid='{0}']", matchId));
                var rf = rfsfNode == null ? 0 : rfsfNode.Attributes["rfs"].Value.GetDecimal();
                list.Add(new JCLQ_RFSF_SP
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    MatchId = matchId,
                    MatchData = matchData,
                    MatchNumber = matchNumber,
                    RF = rf,
                    WinSP = item.Attributes["rfsf3"].Value.GetDecimal(),
                    LoseSP = item.Attributes["rfsf0"].Value.GetDecimal(),
                });
            }
            return list;
        }

        private List<JCLQ_RFSF_SP> Get_RFSF_SP_New(bool isDS)
        {
            var json = GetSPJsonContent("RFSF", isDS);
            var list = new List<JCLQ_RFSF_SP>();
            if (string.IsNullOrEmpty(json))
                return list;

            var array = JsonHelper.Decode(json);
            foreach (var item in array)
            {
                if (item.Length != 4) continue;
                if (string.IsNullOrEmpty(item[0])) continue;
                if (string.IsNullOrEmpty(item[1])) continue;
                if (string.IsNullOrEmpty(item[2])) continue;
                if (string.IsNullOrEmpty(item[3])) continue;

                string matchId = item[0];
                var matchData = matchId.Substring(0, 6);
                var matchNumber = matchId.Replace(matchData, "");

                list.Add(new JCLQ_RFSF_SP
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    MatchData = matchData,
                    MatchId = matchId,
                    MatchNumber = matchNumber,
                    WinSP = decimal.Parse(item[1]),
                    LoseSP = decimal.Parse(item[2]),
                    RF = decimal.Parse(item[3]),
                });

            }
            return list;
        }

        private List<JCLQ_SFC_SP> Get_SFC_SP()
        {
            var list = new List<JCLQ_SFC_SP>();
            var url = "http://trade.cpdyj.com/jclq/getMatchLqSp.go?playId=SFC&ptype=1";
            var xmlContent = PostManager.Get(url, Encoding.GetEncoding("gb2312"), 0, (request) =>
            {
                if (ServiceHelper.IsUseProxy("JCLQ"))
                {
                    var proxy = ServiceHelper.GetProxyUrl();
                    if (!string.IsNullOrEmpty(proxy))
                    {
                        request.Proxy = new System.Net.WebProxy(proxy);
                    }
                }
            });
            var doc = new XmlDocument();
            doc.LoadXml(xmlContent);
            var root = doc.SelectSingleNode("Resp/matches");
            foreach (XmlNode item in root.ChildNodes)
            {
                var matchId = item.Attributes["expectitemid"].Value;
                var matchData = matchId.Substring(0, 6);
                var matchNumber = matchId.Replace(matchData, "");
                list.Add(new JCLQ_SFC_SP
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    MatchId = matchId,
                    MatchData = matchData,
                    MatchNumber = matchNumber,
                    HomeWin1_5 = item.Attributes["sfc1"].Value.GetDecimal(),
                    HomeWin6_10 = item.Attributes["sfc2"].Value.GetDecimal(),
                    HomeWin11_15 = item.Attributes["sfc3"].Value.GetDecimal(),
                    HomeWin16_20 = item.Attributes["sfc4"].Value.GetDecimal(),
                    HomeWin21_25 = item.Attributes["sfc5"].Value.GetDecimal(),
                    HomeWin26 = item.Attributes["sfc6"].Value.GetDecimal(),
                    GuestWin1_5 = item.Attributes["sfc11"].Value.GetDecimal(),
                    GuestWin6_10 = item.Attributes["sfc12"].Value.GetDecimal(),
                    GuestWin11_15 = item.Attributes["sfc13"].Value.GetDecimal(),
                    GuestWin16_20 = item.Attributes["sfc14"].Value.GetDecimal(),
                    GuestWin21_25 = item.Attributes["sfc15"].Value.GetDecimal(),
                    GuestWin26 = item.Attributes["sfc16"].Value.GetDecimal(),
                });
            }

            return list;
        }

        private List<JCLQ_SFC_SP> Get_SFC_SP_New(bool isDS)
        {
            var json = GetSPJsonContent("SFC", isDS);
            var list = new List<JCLQ_SFC_SP>();
            if (string.IsNullOrEmpty(json))
                return list;

            var array = JsonHelper.Decode(json);
            foreach (var item in array)
            {
                if (item.Length != 13) continue;
                if (string.IsNullOrEmpty(item[0])) continue;
                if (string.IsNullOrEmpty(item[1])) continue;
                if (string.IsNullOrEmpty(item[2])) continue;
                if (string.IsNullOrEmpty(item[3])) continue;
                if (string.IsNullOrEmpty(item[4])) continue;
                if (string.IsNullOrEmpty(item[5])) continue;
                if (string.IsNullOrEmpty(item[6])) continue;
                if (string.IsNullOrEmpty(item[7])) continue;
                if (string.IsNullOrEmpty(item[8])) continue;
                if (string.IsNullOrEmpty(item[9])) continue;
                if (string.IsNullOrEmpty(item[10])) continue;
                if (string.IsNullOrEmpty(item[11])) continue;
                if (string.IsNullOrEmpty(item[12])) continue;

                string matchId = item[0];
                var matchData = matchId.Substring(0, 6);
                var matchNumber = matchId.Replace(matchData, "");

                list.Add(new JCLQ_SFC_SP
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    MatchData = matchData,
                    MatchId = matchId,
                    MatchNumber = matchNumber,
                    HomeWin1_5 = decimal.Parse(item[1]),
                    HomeWin6_10 = decimal.Parse(item[2]),
                    HomeWin11_15 = decimal.Parse(item[3]),
                    HomeWin16_20 = decimal.Parse(item[4]),
                    HomeWin21_25 = decimal.Parse(item[5]),
                    HomeWin26 = decimal.Parse(item[6]),

                    GuestWin1_5 = decimal.Parse(item[7]),
                    GuestWin6_10 = decimal.Parse(item[8]),
                    GuestWin11_15 = decimal.Parse(item[9]),
                    GuestWin16_20 = decimal.Parse(item[10]),
                    GuestWin21_25 = decimal.Parse(item[11]),
                    GuestWin26 = decimal.Parse(item[12]),
                });

            }
            return list;
        }

        private List<JCLQ_DXF_SP> Get_DXF_SP()
        {
            var dxfUrl = "http://trade.cpdyj.com/jclq/getmatch.go?playId=DXF&ptype=1";
            var dxfContent = PostManager.Get(dxfUrl, Encoding.GetEncoding("gb2312"), 0, (request) =>
            {
                if (ServiceHelper.IsUseProxy("JCLQ"))
                {
                    var proxy = ServiceHelper.GetProxyUrl();
                    if (!string.IsNullOrEmpty(proxy))
                    {
                        request.Proxy = new System.Net.WebProxy(proxy);
                    }
                }
            });
            var dxfDoc = new XmlDocument();
            dxfDoc.LoadXml(dxfContent);

            var list = new List<JCLQ_DXF_SP>();
            var url = "http://trade.cpdyj.com/jclq/getMatchLqSp.go?playId=DXF&ptype=1";
            var xmlContent = PostManager.Get(url, Encoding.GetEncoding("gb2312"), 0, (request) =>
            {
                if (ServiceHelper.IsUseProxy("JCLQ"))
                {
                    var proxy = ServiceHelper.GetProxyUrl();
                    if (!string.IsNullOrEmpty(proxy))
                    {
                        request.Proxy = new System.Net.WebProxy(proxy);
                    }
                }
            });
            var doc = new XmlDocument();
            doc.LoadXml(xmlContent);
            var root = doc.SelectSingleNode("Resp/matches");
            foreach (XmlNode item in root.ChildNodes)
            {
                var matchId = item.Attributes["expectitemid"].Value;
                var matchData = matchId.Substring(0, 6);
                var matchNumber = matchId.Replace(matchData, "");
                var dxfNode = dxfDoc.SelectSingleNode(string.Format("//match[@expectitemid='{0}']", matchId));
                var yszf = dxfNode == null ? 0 : dxfNode.Attributes["yszf"].Value.GetDecimal();
                list.Add(new JCLQ_DXF_SP
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    MatchId = matchId,
                    MatchData = matchData,
                    MatchNumber = matchNumber,
                    YSZF = yszf,
                    //todo 确定具体数据
                    DF = item.Attributes["dxf3"].Value.GetDecimal(),
                    XF = item.Attributes["dxf0"].Value.GetDecimal(),
                });
            }

            return list;
        }

        private List<JCLQ_DXF_SP> Get_DXF_SP_New(bool isDS)
        {
            var json = GetSPJsonContent("DXF", isDS);
            var list = new List<JCLQ_DXF_SP>();
            if (string.IsNullOrEmpty(json))
                return list;

            var array = JsonHelper.Decode(json);
            foreach (var item in array)
            {
                if (item.Length != 4) continue;
                if (string.IsNullOrEmpty(item[0])) continue;
                if (string.IsNullOrEmpty(item[1])) continue;
                if (string.IsNullOrEmpty(item[2])) continue;
                if (string.IsNullOrEmpty(item[3])) continue;

                string matchId = item[0];
                var matchData = matchId.Substring(0, 6);
                var matchNumber = matchId.Replace(matchData, "");

                list.Add(new JCLQ_DXF_SP
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    MatchData = matchData,
                    MatchId = matchId,
                    MatchNumber = matchNumber,
                    DF = decimal.Parse(item[1]),
                    XF = decimal.Parse(item[2]),
                    YSZF = decimal.Parse(item[3]),
                });

            }
            return list;
        }

        private string GetSPJsonContent(string gameType, bool isDS)
        {
            var url = string.Empty;
            var replaceFlag = string.Empty;
            switch (gameType)
            {
                case "SF":
                    if (isDS)
                    {
                        url = "http://intf.cpdyj.com/data/jclq/sf_fd.js?callback=dyj_jc_spf_fd&_={0}";
                        replaceFlag = "dyj_jc_spf_fd(";
                    }
                    else
                    {
                        url = "http://intf.cpdyj.com/data/jclq/sf_gd.js?callback=dyj_jc_spf_gd&_={0}";
                        replaceFlag = "dyj_jc_spf_gd(";
                    }
                    break;
                case "RFSF":
                    if (isDS)
                    {
                        url = "http://intf.cpdyj.com/data/jclq/rfsf_fd.js?callback=dyj_jc_rfsf_fd&_={0}";
                        replaceFlag = "dyj_jc_rfsf_fd(";
                    }
                    else
                    {
                        url = "http://intf.cpdyj.com/data/jclq/rfsf_gd.js?callback=dyj_jc_rfsf_gd&_={0}";
                        replaceFlag = "dyj_jc_rfsf_gd(";
                    }
                    break;
                case "SFC":
                    if (isDS)
                    {
                        url = "http://intf.cpdyj.com/data/jclq/sfc_fd.js?callback=dyj_jc_sfc_fd&_={0}";
                        replaceFlag = "dyj_jc_sfc_fd(";
                    }
                    else
                    {
                        url = "http://intf.cpdyj.com/data/jclq/sfc_gd.js?callback=dyj_jc_sfc_gd&_={0}";
                        replaceFlag = "dyj_jc_sfc_gd(";
                    }
                    break;
                case "DXF":
                    if (isDS)
                    {
                        url = "http://intf.cpdyj.com/data/jclq/dxf_fd.js?callback=dyj_jc_dxf_fd&_={0}";
                        replaceFlag = "dyj_jc_dxf_fd(";
                    }
                    else
                    {
                        url = "http://intf.cpdyj.com/data/jclq/dxf_gd.js?callback=dyj_jc_dxf_gd&_={0}";
                        replaceFlag = "dyj_jc_dxf_gd(";
                    }
                    break;
            }

            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long tt = (DateTime.Now.Ticks - startTime.Ticks) / 10000;
            url = string.Format(url, tt);
            var encoding = Encoding.GetEncoding("gb2312");
            var content = PostManager.Get(url, encoding, 0, (request) =>
            {
                request.Host = "intf.cpdyj.com";
                request.Referer = "	http://jclq.cpdyj.com/";
                if (ServiceHelper.IsUseProxy("JCLQ"))
                {
                    var proxy = ServiceHelper.GetProxyUrl();
                    if (!string.IsNullOrEmpty(proxy))
                    {
                        request.Proxy = new System.Net.WebProxy(proxy);
                    }
                }
            });
            content = content.Replace(replaceFlag, "").Replace(");", "");

            return content;
        }


        private List<JCLQ_SF_SP> Get_SF_SP_ZZJCW()
        {
            var json = GetSPJsonContent_ZZJCW("SF");
            var list = new List<JCLQ_SF_SP>();
            if (string.IsNullOrEmpty(json))
                return list;

            var array = JsonHelper.Decode(json);

            foreach (var item in array)
            {
                //if (item.Key != "data")
                //    continue;
                var matchlist = item.Value;
                foreach (var match in matchlist)
                {
                    var match_D = match.Value;
                    string b_date = "";
                    try
                    {
                        b_date = match_D.b_date;
                        if (string.IsNullOrEmpty(b_date))
                        {
                            continue;
                        }
                    }
                    catch (Exception)
                    {

                        continue;
                    }
                    string num = match_D.num;
                    string matchId = b_date.Substring(2).Replace("-", "") +num.Substring(2);
                    var matchData = matchId.Substring(0, 6);
                    var matchNumber = matchId.Substring(6);

                    list.Add(new JCLQ_SF_SP
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        MatchData = matchData,
                        MatchId = matchId,
                        MatchNumber = matchNumber,
                        WinSP = string.IsNullOrEmpty(match_D.mnl.h.Value) ? 0M : decimal.Parse(match_D.mnl.h.Value),
                        LoseSP = string.IsNullOrEmpty(match_D.mnl.a.Value) ? 0M : decimal.Parse(match_D.mnl.a.Value),
                    });
                }
            }
            return list;
        }

        private List<JCLQ_RFSF_SP> Get_RFSF_SP_ZZJCW()
        {
            var json = GetSPJsonContent_ZZJCW("RFSF");
            var list = new List<JCLQ_RFSF_SP>();
            if (string.IsNullOrEmpty(json))
                return list;

            var array = JsonHelper.Decode(json);

            foreach (var item in array)
            {
                //if (item.Key != "data")
                //    continue;
                var matchlist = item.Value;
                foreach (var match in matchlist)
                {
                    var match_D = match.Value;

                    string b_date = "";
                    try
                    {
                        b_date = match_D.b_date;
                        if (string.IsNullOrEmpty(b_date))
                        {
                            continue;
                        }
                    }
                    catch (Exception)
                    {

                        continue;
                    }
                    string num = match_D.num;
                    string matchId = b_date.Substring(2).Replace("-", "") + num.Substring(2);
                    var matchData = matchId.Substring(0, 6);
                    var matchNumber = matchId.Substring(6);
                    list.Add(new JCLQ_RFSF_SP
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        MatchData = matchData,
                        MatchId = matchId,
                        MatchNumber = matchNumber,
                        WinSP = string.IsNullOrEmpty(match_D.hdc.h.Value) ? 0M : decimal.Parse(match_D.hdc.h.Value),
                        LoseSP = string.IsNullOrEmpty(match_D.hdc.a.Value) ? 0M : decimal.Parse(match_D.hdc.a.Value),
                        RF = string.IsNullOrEmpty(match_D.hdc.fixedodds.Value) ? 0M : decimal.Parse(match_D.hdc.fixedodds.Value),
                    });
                }
            }
            return list;
        }

        private List<JCLQ_SFC_SP> Get_SFC_SP_ZZJCW()
        {
            var json = GetSPJsonContent_ZZJCW("SFC");
            var list = new List<JCLQ_SFC_SP>();
            if (string.IsNullOrEmpty(json))
                return list;

            var array = JsonHelper.Decode(json);
            foreach (var item in array)
            {
              
                var matchlist = item.Value;
                foreach (var match in matchlist)
                {
                    var match_D = match.Value;
                    string b_date = "";
                    try
                    {
                        b_date = match_D.b_date;
                        if (string.IsNullOrEmpty(b_date))
                        {
                            continue;
                        }
                    }
                    catch (Exception)
                    {

                        continue;
                    }
                    string num = match_D.num;

                    string matchId = b_date.Substring(2).Replace("-", "") + num.Substring(2);
                    var matchData = matchId.Substring(0, 6);
                    var matchNumber = matchId.Substring(6);

                    list.Add(new JCLQ_SFC_SP
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        MatchData = matchData,
                        MatchId = matchId,
                        MatchNumber = matchNumber,

                        HomeWin1_5 = string.IsNullOrEmpty(match_D.wnm.w1.Value) ? 0M : decimal.Parse(match_D.wnm.w1.Value),
                        HomeWin6_10 = string.IsNullOrEmpty(match_D.wnm.w2.Value) ? 0M : decimal.Parse(match_D.wnm.w2.Value),
                        HomeWin11_15 = string.IsNullOrEmpty(match_D.wnm.w3.Value) ? 0M : decimal.Parse(match_D.wnm.w3.Value),
                        HomeWin16_20 = string.IsNullOrEmpty(match_D.wnm.w4.Value) ? 0M : decimal.Parse(match_D.wnm.w4.Value),
                        HomeWin21_25 = string.IsNullOrEmpty(match_D.wnm.w5.Value) ? 0M : decimal.Parse(match_D.wnm.w5.Value),
                        HomeWin26 = string.IsNullOrEmpty(match_D.wnm.w6.Value) ? 0M : decimal.Parse(match_D.wnm.w6.Value),

                        GuestWin1_5 = string.IsNullOrEmpty(match_D.wnm.l1.Value) ? 0M : decimal.Parse(match_D.wnm.l1.Value),
                        GuestWin6_10 = string.IsNullOrEmpty(match_D.wnm.l2.Value) ? 0M : decimal.Parse(match_D.wnm.l2.Value),
                        GuestWin11_15 = string.IsNullOrEmpty(match_D.wnm.l3.Value) ? 0M : decimal.Parse(match_D.wnm.l3.Value),
                        GuestWin16_20 = string.IsNullOrEmpty(match_D.wnm.l4.Value) ? 0M : decimal.Parse(match_D.wnm.l4.Value),
                        GuestWin21_25 = string.IsNullOrEmpty(match_D.wnm.l5.Value) ? 0M : decimal.Parse(match_D.wnm.l5.Value),
                        GuestWin26 = string.IsNullOrEmpty(match_D.wnm.l6.Value) ? 0M : decimal.Parse(match_D.wnm.l6.Value),
                    });
                }
            }
            return list;
        }

        private List<JCLQ_DXF_SP> Get_DXF_SP_ZZJCW()
        {
            var json = GetSPJsonContent_ZZJCW("DXF");
            var list = new List<JCLQ_DXF_SP>();
            if (string.IsNullOrEmpty(json))
                return list;

            var array = JsonHelper.Decode(json);
            foreach (var item in array)
            {
              
                var matchlist = item.Value;
                foreach (var match in matchlist)
                {
                    var match_D = match.Value;
                    string b_date = "";
                    try
                    {
                        b_date = match_D.b_date;
                        if (string.IsNullOrEmpty(b_date))
                        {
                            continue;
                        }
                    }
                    catch (Exception)
                    {

                        continue;
                    }
                    string num = match_D.num;
                    string matchId =b_date.Substring(2).Replace("-", "") +num.Substring(2);
                    var matchData = matchId.Substring(0, 6);
                    var matchNumber = matchId.Substring(6);

                    list.Add(new JCLQ_DXF_SP
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        MatchData = matchData,
                        MatchId = matchId,
                        MatchNumber = matchNumber,
                        DF = string.IsNullOrEmpty(match_D.hilo.h.Value) ? 0M : decimal.Parse(match_D.hilo.h.Value),
                        XF = string.IsNullOrEmpty(match_D.hilo.l.Value) ? 0M : decimal.Parse(match_D.hilo.l.Value),
                        YSZF = string.IsNullOrEmpty(match_D.hilo.fixedodds.Value) ? 0M : decimal.Parse(match_D.hilo.fixedodds.Value),
                    });
                }
            }
            return list;
        }

        /// <summary>
        /// 各玩法SP-中国竞彩网
        /// </summary>
        private string GetSPJsonContent_ZZJCW(string gameType)
        {
            var url = string.Empty;
            var replaceFlag = string.Empty;
            switch (gameType)
            {
                case "SF":
                    url = "http://i.sporttery.cn/odds_calculator/get_odds?i_format=json&i_callback=getData&poolcode[]=mnl&_={0}";
                    replaceFlag = "getData(";
                    break;
                case "RFSF":
                    url = "http://i.sporttery.cn/odds_calculator/get_odds?i_format=json&i_callback=getData&poolcode[]=hdc&_={0}";
                    replaceFlag = "getData(";
                    break;
                case "SFC":
                    url = "http://i.sporttery.cn/odds_calculator/get_odds?i_format=json&i_callback=getData&poolcode[]=wnm&_={0}";
                    replaceFlag = "getData(";
                    break;
                case "DXF":
                    url = "http://i.sporttery.cn/odds_calculator/get_odds?i_format=json&i_callback=getData&poolcode[]=hilo&_={0}";
                    replaceFlag = "getData(";
                    break;
            }

            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long tt = (DateTime.Now.Ticks - startTime.Ticks) / 10000;
            url = string.Format(url, tt);
            var encoding = Encoding.GetEncoding("gb2312");
            var content = PostManager.Get(url, encoding, 0, null);
            content = content.Replace(replaceFlag, "").Replace(");", "");

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
            //    _logWriter.Write(logErrorCategory, logErrorSource, LogType.Error, "自动采集竞彩篮球队伍数据", log);
        }

        public void WriteLog(string log)
        {
            Console.WriteLine(log);
            //if (_logWriter != null)
            //    _logWriter.Write(logCategory, logInfoSource, LogType.Information, "自动采集竞彩篮球队伍数据", log);
        }

        /// <summary>
        /// 采集310win的FXId
        /// </summary>
        private Dictionary<string, string> GetJCLQ_FX(string gameType)
        {
            string url = GetUrl(gameType);
            string html = PostManager.PostCustomer(url, string.Empty, Encoding.UTF8, (request) =>
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
            if (html == "404")
                return new Dictionary<string, string>();

            switch (gameType)
            {
                case "RFSF":
                    return JCLQ_RFSF_FX(html, gameType);
                case "SF":
                    return JCLQ_SF_FX(html, gameType);
            }
            return new Dictionary<string, string>();
        }
        private string GetUrl(string gameType)
        {
            switch (gameType)
            {
                case "RFSF":
                    return "http://www.310win.com/buy/JingCaiBasket.aspx?typeID=112";
                case "SF":
                    return "http://www.310win.com/buy/JingCaiBasket.aspx?typeID=111";
            }
            return string.Empty;
        }
        private Dictionary<string, string> JCLQ_RFSF_FX(string html, string gameType)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            int tableIndex = html.IndexOf("<tr class='niDate'>");
            html = html.Substring(tableIndex, html.Length - tableIndex);

            int endTableIndex = html.IndexOf("<div id=\"divDaohang\"");
            html = html.Substring(0, endTableIndex);

            var rows = html.Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);
            var zhou = string.Empty;
            var winNumber = string.Empty;
            foreach (var row in rows)
            {
                if (row.IndexOf("class='niDate'") != -1)
                {
                    var subfrist = row.IndexOf("日 星期") + 4;
                    zhou = "周" + row.Substring(subfrist, 1);
                    continue;
                }
                if (row.IndexOf("HomeOrder_") != -1)
                {
                    var matchId = row.Substring(row.IndexOf("absmiddle\" />") + ("absmiddle\" />").Length, 3);
                    var matchIdName = GetMatchDate(zhou, true);
                    var match = matchIdName + matchId;

                    winNumber = row.Substring(row.IndexOf("<span id=\"HomeOrder_") + ("<span id=\"HomeOrder_").Length, 6);
                    dic.Add(match, winNumber);
                    continue;
                }
            }
            return dic;
        }
        private Dictionary<string, string> JCLQ_SF_FX(string html, string gameType)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            int tableIndex = html.IndexOf("<tr class='niDate'>");
            html = html.Substring(tableIndex, html.Length - tableIndex);

            int endTableIndex = html.IndexOf("<div id=\"divDaohang\"");
            html = html.Substring(0, endTableIndex);

            var rows = html.Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries);
            var zhou = string.Empty;
            var winNumber = string.Empty;
            foreach (var row in rows)
            {

                if (row.IndexOf("class='niDate'") != -1)
                {
                    var subfrist = row.IndexOf("日 星期") + 4;
                    zhou = "周" + row.Substring(subfrist, 1);
                    continue;
                }
                if (row.IndexOf("HomeOrder_") != -1)
                {
                    var matchId = row.Substring(row.IndexOf("absmiddle\" />") + ("absmiddle\" />").Length, 3);
                    var matchIdName = GetMatchDate(zhou, true);
                    var match = matchIdName + matchId;

                    winNumber = row.Substring(row.IndexOf("<span id=\"HomeOrder_") + ("<span id=\"HomeOrder_").Length, 6);
                    dic.Add(match, winNumber);
                    continue;
                }
            }
            return dic;
        }

        /// <summary>
        /// 采集OKOOO的FXId
        /// </summary>
        private Dictionary<string, string> GetJCLQ_FX_OKOOO()
        {
            var url = "http://www.okooo.com/jingcailanqiu/rangfen/";
            var dic = new Dictionary<string, string>();
            var html = PostManager.Get(url, Encoding.GetEncoding("GBK"));
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

                var matchDate = string.Empty;
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
                        target = "MatchTitle";
                        index = td.IndexOf(target);
                        if (index >= 0)
                        {
                            //取日期
                            var tempArray = td.Split(new string[] { "</span>" }, StringSplitOptions.RemoveEmptyEntries);
                            if (tempArray.Length > 0)
                            {
                                index = tempArray[0].LastIndexOf(">");
                                var array2 = tempArray[0].Substring(index + 1).Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                if (array2.Length > 0)
                                {
                                    matchDate = DateTime.Parse(array2[1]).ToString("yyyyMMdd").Substring(2);
                                }
                            }
                        }

                        target = "<i>";
                        index = td.IndexOf(target);
                        if (index >= 0)
                        {
                            //取编号
                            temp = td.Substring(index + target.Length);
                            target = "</i>";
                            index = temp.IndexOf(target);
                            temp = temp.Substring(0, index);

                            matchId = string.Format("{0}{1}", matchDate, temp);
                        }
                        target = "href=\"/basketball/match/";
                        index = td.IndexOf(target);
                        if (index >= 0)
                        {
                            //取FXId
                            temp = td.Substring(index + target.Length);
                            target = "/ah/";
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

    }


    public class JCLQMatchResultCache
    {
        public string Id { get; set; }
        public int CacheTimes { get; set; }
        public JCLQ_MatchResult MatchResult { get; set; }
    }
}
