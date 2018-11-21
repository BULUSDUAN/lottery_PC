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
using EntityModel.Domain.Entities;
using EntityModel.Interface;
using KaSon.FrameWork.Common.Gateway;
using System.Text.RegularExpressions;

namespace Lottery.CrawGetters.MatchBizGetter
{
   
   
    // <summary>
    /// 采集竞猜足球赛事数据
    /// </summary>
    public class JCZQMatch_AutoCollect : BaseAutoCollect, IAutoCollect
    {
        //  private ILogWriter _logWriter = null;
        private const string logCategory = "Services.Info";
        private string logInfoSource = "Auto_Collect_JCZQMatch_Info_";
        private const string logErrorCategory = "Services.Error";
        private const string logErrorSource = "Auto_Collect_JCZQMatch_Error";

        private long BeStop = 0;
        private System.Timers.Timer timer = null;
        private int CTZQ_advanceMinutes = 0;
        private string SavePath = string.Empty;
        private ILogger<JCZQMatch_AutoCollect> _logWriter = null;
        private Dictionary<string, string> _Match_Ok_HG = new Dictionary<string, string>();
       
        public string Category { get; set; }
        public string Key { get; set; }
        private Task thread = null;

      
      
        private int JCZQ_advanceMinutes = 0;
    
        private string SavePath_New = string.Empty;
        private string zhm_ServerUrl = "";
        private string zhm_Key = "";
        private string zhm_PartnerId = "";
        private Dictionary<string, string> _MatchStatus = new Dictionary<string, string>();
        private List<string> _ozbList = new List<string>();
        private List<string> _sjbList = new List<string>();
        private IMongoDatabase mDB;
        private string gameCode { get; set; }
        private int sleepSecond = 5;
        public JCZQMatch_AutoCollect(IMongoDatabase _mDB, string _gameName, int _sleepSecond = 5) : base(_gameName + "Match", _mDB)
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
                   WriteLogAll();
                    try
                    {


                        CollectJCZQMatchCore(gameCode);

                    }
                    catch 
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
        DisableMatchConfigInfoCollection DisableMatchConfigList;
        private int OkNumber = -1;
        public void CollectJCZQMatchCore(string gameCode)
        {
            this.WriteLog("进入DoWork  开始采集数据");

            try
            {
                JCZQ_advanceMinutes = int.Parse(ServiceHelper.GetSystemConfig("JCZQ_AdvanceMinutes"));
                //  DisableMatchConfigList = ServiceHelper.QueryDisableMatchConfigList("JCZQ");
                 DisableMatchConfigList = new TicketGatewayAdmin().QueryDisableMatchConfigList("JCZQ");
                //zhm_ServerUrl = ServiceHelper.GetZHM_ServerUrl();
                //zhm_Key = ServiceHelper.GetZHM_Key();
                //zhm_PartnerId = ServiceHelper.GetZHM_PartnerId();

                this.WriteLog("开始采集竞彩足球队伍数据");
                var matchFun = ServiceHelper.GetSystemConfig("JCZQ_Match_Fun");
                var tempMatchList = new List<JCZQ_MatchInfo>();
                var tempMatchList_FB = new List<JCZQ_MatchInfo>();
                var tempMatchList_BF = new List<JCZQ_MatchInfo>();
                var tempMatchList_BQC = new List<JCZQ_MatchInfo>();
                var tempMatchList_ZJQ = new List<JCZQ_MatchInfo>();
                var match_HH = new List<C_JCZQ_Match_HH>();
                var match_spf = new List<C_JCZQ_Match_SPF>();
                //var match_zjq = new List<JCZQ_Match_ZJQ>();
                //var match_bqc = new List<JCZQ_Match_BQC>();
                var ozSPList = new List<JCZQ_SPF_OZ_SPInfo>();

                var spfList = new List<C_JCZQ_SPF_SP>();
                var brqspfList = new List<C_JCZQ_SPF_SP>();
                var bfList = new List<C_JCZQ_BF_SP>();
                var zjqList = new List<C_JCZQ_ZJQ_SP>();
                var bqcList = new List<C_JCZQ_BQC_SP>();
                var spList = new List<JCZQ_SP>();
                var m_spList = new List<JCZQ_Match_SPF_SP>();
                var sp2x1List = new List<JCZQ_2X1SP>();
                //if (matchFun == "old")
                //    tempMatchList = GetMatchList();
                GetMatchList_OP_ZGJCW();
                if (OkNumber >= 10 || OkNumber == -1)
                {
                    OkNumber = 0;
                    GetMatchList_okooo_HG();
                }
                OkNumber++;
                if (matchFun == "new")
                {
                    try
                    {
                        //Thread.Sleep(500);
                        tempMatchList = GetMatchList_ZGJCW_SPF(out ozSPList); //1
                        //Thread.Sleep(500);
                        tempMatchList_FB = GetMatchList_ZGJCW_SPF_FB();
                       // Thread.Sleep(500);
                        tempMatchList_BF = GetMatchList_ZGJCW_BF();
                       // Thread.Sleep(500);
                        tempMatchList_ZJQ = GetMatchList_ZGJCW_ZJQ();
                       // Thread.Sleep(500);
                        tempMatchList_BQC = GetMatchList_ZGJCW_BQC();
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                  
                }

                if (matchFun == "ok")
                {
                    tempMatchList = GetMatchList_okooo(out ozSPList, out tempMatchList_FB, out tempMatchList_BF, out tempMatchList_ZJQ, out tempMatchList_BQC,
                        out spfList, out brqspfList, out bfList, out zjqList, out bqcList);
                }
                //if (matchFun == "zhm")
                //    tempMatchList = GetMatchList_ZHM();
                this.WriteLog(string.Format("采集竞彩足球队伍数据完成,记录{0}条", tempMatchList.Count));

                if (tempMatchList.Count == 0 || tempMatchList_FB.Count == 0 || tempMatchList_BF.Count == 0 || tempMatchList_ZJQ.Count == 0 || tempMatchList_BQC.Count == 0)
                    return;

                foreach (var item in tempMatchList_FB)
                {
                    var hRank = string.Empty;
                    var gRank = string.Empty;
                    var hLg = string.Empty;
                    var gLg = string.Empty;
                    var leagueMatchId = 0;
                    var mid = 0;
                    var hg = _Match_Ok_HG.Where(p => p.Key == item.MatchId).FirstOrDefault();
                    if (!string.IsNullOrEmpty(hg.Key))
                    {
                        var strL = hg.Value.Split('^');
                        if (strL.Length == 6)
                        {
                            hRank = strL[0];
                            gRank = strL[1];
                            hLg = strL[2];
                            gLg = strL[3];
                            mid = int.Parse(strL[4]);
                            leagueMatchId = int.Parse(strL[5]);
                        }
                    }

                    #region HH
                    match_HH.Add(new C_JCZQ_Match_HH
                    {
                        CreateTime = item.CreateTime,
                        DSStopBettingTime = item.DSStopBettingTime,
                        MatchData = item.MatchData,
                        StartDateTime = item.StartDateTime,
                        FlatOdds = item.FlatOdds,
                        FSStopBettingTime = item.FSStopBettingTime,
                        FXId = item.FXId,
                        Gi = item.Gi,
                        GuestTeamId = item.GuestTeamId,
                        GuestTeamName = item.GuestTeamName,
                        Hi = item.Hi,
                        HomeTeamId = item.HomeTeamId,
                        HomeTeamName = item.HomeTeamName,
                        mId = item.mId,
                        LeagueColor = item.LeagueColor,
                        LeagueId = item.LeagueId,
                        LeagueName = item.LeagueName,
                        LetBall = item.LetBall,
                        LoseOdds = item.LoseOdds,
                        MatchId = item.MatchId,
                        MatchIdName = item.MatchIdName,
                        Mid = item.Mid,
                        MatchNumber = item.MatchNumber,
                        PrivilegesType = item.PrivilegesType,
                        ShortGuestTeamName = item.ShortGuestTeamName,
                        ShortHomeTeamName = item.ShortHomeTeamName,
                        ShortLeagueName = item.ShortLeagueName,
                        State = _MatchStatus.Where(p => p.Key == item.MatchId).FirstOrDefault().Key != null ? _MatchStatus.Where(p => p.Key == item.MatchId).FirstOrDefault().Value : "0",
                        WinOdds = item.WinOdds,
                        MatchStopDesc = item.MatchStopDesc,
                        GLg = gLg,
                        GRank = gRank,
                        HLg = hLg,
                        HRank = hRank,
                    });
                    #endregion

                    #region SPF-BRQSPF
                    match_spf.Add(new C_JCZQ_Match_SPF
                    {
                        CreateTime = item.CreateTime,
                        DSStopBettingTime = item.DSStopBettingTime,
                        MatchData = item.MatchData,
                        StartDateTime = item.StartDateTime,
                        FlatOdds = item.FlatOdds,
                        FSStopBettingTime = item.FSStopBettingTime,
                        FXId = item.FXId,
                        Gi = item.Gi,
                        GuestTeamId = item.GuestTeamId,
                        GuestTeamName = item.GuestTeamName,
                        Hi = item.Hi,
                        HomeTeamId = item.HomeTeamId,
                        HomeTeamName = item.HomeTeamName,
                        mId = item.mId,
                        LeagueColor = item.LeagueColor,
                        LeagueId = item.LeagueId,
                        LeagueName = item.LeagueName,
                        LetBall = item.LetBall,
                        LoseOdds = item.LoseOdds,
                        MatchId = item.MatchId,
                        MatchIdName = item.MatchIdName,
                        Mid = item.Mid,
                        MatchNumber = item.MatchNumber,
                        PrivilegesType = item.PrivilegesType,
                        ShortGuestTeamName = item.ShortGuestTeamName,
                        ShortHomeTeamName = item.ShortHomeTeamName,
                        ShortLeagueName = item.ShortLeagueName,
                        State = _MatchStatus.Where(p => p.Key == item.MatchId).FirstOrDefault().Key != null ? _MatchStatus.Where(p => p.Key == item.MatchId).FirstOrDefault().Value : "0",
                        WinOdds = item.WinOdds,
                        GLg = gLg,
                        GRank = gRank,
                        HLg = hLg,
                        HRank = hRank,
                    });
                    #endregion

                    #region ZJQ
                    //match_zjq.Add(new JCZQ_Match_ZJQ
                    //{
                    //    CreateTime = item.CreateTime,
                    //    DSStopBettingTime = item.DSStopBettingTime,
                    //    MatchData = item.MatchData,
                    //    StartDateTime = item.StartDateTime,
                    //    FlatOdds = item.FlatOdds,
                    //    FSStopBettingTime = item.FSStopBettingTime,
                    //    FXId = item.FXId,
                    //    Gi = item.Gi,
                    //    GuestTeamId = item.GuestTeamId,
                    //    GuestTeamName = item.GuestTeamName,
                    //    Hi = item.Hi,
                    //    HomeTeamId = item.HomeTeamId,
                    //    HomeTeamName = item.HomeTeamName,
                    //    Id = item.Id,
                    //    LeagueColor = item.LeagueColor,
                    //    LeagueId = leagueMatchId,
                    //    LeagueName = item.LeagueName,
                    //    LetBall = item.LetBall,
                    //    LoseOdds = item.LoseOdds,
                    //    MatchId = item.MatchId,
                    //    MatchIdName = item.MatchIdName,
                    //    Mid = mid,
                    //    MatchNumber = item.MatchNumber,
                    //    PrivilegesType = item.PrivilegesType,
                    //    ShortGuestTeamName = item.ShortGuestTeamName,
                    //    ShortHomeTeamName = item.ShortHomeTeamName,
                    //    ShortLeagueName = item.ShortLeagueName,
                    //    State = _MatchStatus.Where(p => p.Key == item.MatchId).FirstOrDefault().Key != null ? _MatchStatus.Where(p => p.Key == item.MatchId).FirstOrDefault().Value : "0",
                    //    WinOdds = item.WinOdds,
                    //    GLg = gLg,
                    //    GRank = gRank,
                    //    HLg = hLg,
                    //    HRank = hRank,
                    //});
                    #endregion

                    #region BQC
                    //match_bqc.Add(new JCZQ_Match_BQC
                    //{
                    //    CreateTime = item.CreateTime,
                    //    DSStopBettingTime = item.DSStopBettingTime,
                    //    MatchData = item.MatchData,
                    //    StartDateTime = item.StartDateTime,
                    //    FlatOdds = item.FlatOdds,
                    //    FSStopBettingTime = item.FSStopBettingTime,
                    //    FXId = item.FXId,
                    //    Gi = item.Gi,
                    //    GuestTeamId = item.GuestTeamId,
                    //    GuestTeamName = item.GuestTeamName,
                    //    Hi = item.Hi,
                    //    HomeTeamId = item.HomeTeamId,
                    //    HomeTeamName = item.HomeTeamName,
                    //    Id = item.Id,
                    //    LeagueColor = item.LeagueColor,
                    //    LeagueId = leagueMatchId,
                    //    LeagueName = item.LeagueName,
                    //    LetBall = item.LetBall,
                    //    LoseOdds = item.LoseOdds,
                    //    MatchId = item.MatchId,
                    //    MatchIdName = item.MatchIdName,
                    //    Mid = mid,
                    //    MatchNumber = item.MatchNumber,
                    //    PrivilegesType = item.PrivilegesType,
                    //    ShortGuestTeamName = item.ShortGuestTeamName,
                    //    ShortHomeTeamName = item.ShortHomeTeamName,
                    //    ShortLeagueName = item.ShortLeagueName,
                    //    State = _MatchStatus.Where(p => p.Key == item.MatchId).FirstOrDefault().Key != null ? _MatchStatus.Where(p => p.Key == item.MatchId).FirstOrDefault().Value : "0",
                    //    WinOdds = item.WinOdds,
                    //    GLg = gLg,
                    //    GRank = gRank,
                    //    HLg = hLg,
                    //    HRank = hRank,
                    //});
                    #endregion
                }



                this.WriteLog("开始采集竞彩足球SP数据");
                var matchList = new List<JCZQ_MatchInfo>();
                var matchList_bf = new List<JCZQ_MatchInfo>();
                var matchList_zjq = new List<JCZQ_MatchInfo>();
                var matchList_bqc = new List<JCZQ_MatchInfo>();


                //var spfList_DS = new List<C_JCZQ_SPF_SP>();
                //var brqspfList_DS = new List<C_JCZQ_SPF_SP>();
                //var bfList_DS = new List<C_JCZQ_BF_SP>();
                //var zjqList_DS = new List<C_JCZQ_ZJQ_SP>();
                //var bqcList_DS = new List<C_JCZQ_BQC_SP>();
                //var spList_DS = new List<JCZQ_SP>();
                //if (matchFun == "old")
                //{
                //    spfList = Get_SPF_SP(tempMatchList, out matchList);
                //    bfList = Get_BF_SP();
                //    zjqList = Get_ZJQ_SP();
                //    bqcList = Get_BQC_SP();
                //}
                if (matchFun == "new")
                {
                    //matchList = AddZHMPrivilegesType(tempMatchList);
                    //matchList_bf = AddZHMPrivilegesType(tempMatchList_BF);
                    //matchList_zjq = AddZHMPrivilegesType(tempMatchList_ZJQ);
                    //matchList_bqc = AddZHMPrivilegesType(tempMatchList_BQC);
                  //  Thread.Sleep(500);
                    spfList = Get_SPF_SP_ZZJCW(out brqspfList);
                  //  Thread.Sleep(500);
                    bfList = Get_BF_SP_ZZJCW();
                  //  Thread.Sleep(500);
                    zjqList = Get_ZJQ_SP_ZZJCW();
                  //  Thread.Sleep(500);
                    bqcList = Get_BQC_SP_ZZJCW();

                    if (spfList.Count < tempMatchList.Count || brqspfList.Count < tempMatchList.Count || bfList.Count < tempMatchList_BF.Count || zjqList.Count < tempMatchList_ZJQ.Count || bqcList.Count < tempMatchList_BQC.Count)
                        return;

                    ////过关SP
                    //spfList = Get_SPF_SP_New(false, out brqspfList);
                    //bfList = Get_BF_SP_New(false);
                    //zjqList = Get_ZJQ_SP_New(false);
                    //bqcList = Get_BQC_SP_New(false);

                    ////单关SP
                    //spfList_DS = Get_SPF_SP_New(true, out brqspfList_DS);
                    //bfList_DS = Get_BF_SP_New(true);
                    //zjqList_DS = Get_ZJQ_SP_New(true);
                    //bqcList_DS = Get_BQC_SP_New(true);
                    #region 循环写入SP
                    var il = tempMatchList.Count;
                    var str = string.Empty;
                    string[] propertyNames = new[] { "Id", "MatchData", "MatchId", "MatchNumber", "CreateTime" };

                    for (int i = 0; i < il; i++)
                    {
                        var matchid = tempMatchList[i].MatchId;
                        var sps = new JCZQ_SP();
                        var msps = new JCZQ_Match_SPF_SP();
                        var sp2x1 = new JCZQ_2X1SP();
                        //var sps_DS = new JCZQ_SP();
                        sps.MatchId = tempMatchList[i].MatchId;
                        sps.MatchData = tempMatchList[i].MatchData;
                        sps.MatchNumber = tempMatchList[i].MatchNumber;
                        msps.MatchId = tempMatchList[i].MatchId;
                        msps.MatchData = tempMatchList[i].MatchData;
                        msps.MatchNumber = tempMatchList[i].MatchNumber;
                        sp2x1.MatchId = tempMatchList[i].MatchId;
                        sp2x1.MatchData = tempMatchList[i].MatchData;
                        sp2x1.MatchNumber = tempMatchList[i].MatchNumber;
                        var hh = match_HH.Where(p => p.MatchId == tempMatchList[i].MatchId).FirstOrDefault();
                        //var m_spf = match_spf.Where(p => p.MatchId == tempMatchList[i].MatchId).FirstOrDefault();
                        //var m_zjq = match_zjq.Where(p => p.MatchId == tempMatchList[i].MatchId).FirstOrDefault();
                        //var m_bqc = match_bqc.Where(p => p.MatchId == tempMatchList[i].MatchId).FirstOrDefault();


                        //sps_DS.MatchId = matchList[i].MatchId;
                        //sps_DS.MatchData = matchList[i].MatchData;
                        //sps_DS.MatchNumber = matchList[i].MatchNumber;

                        C_JCZQ_SPF_SP brqspf = brqspfList.FirstOrDefault(o => o.MatchId == matchid);
                        if (brqspf != null)
                        {
                            if (brqspf.WinOdds == 0M && brqspf.LoseOdds == 0M && brqspf.FlatOdds == 0M)
                                brqspf.NoSaleState = "1";
                            else
                                brqspf.NoSaleState = "0";
                            str = ServiceHelper.getProperties(brqspf, propertyNames);
                            sps.BRQSPF = str;
                            sp2x1.BRQSPF = str;
                            if (hh != null)
                                hh.BRQSPF = str;
                            //if (m_spf != null)
                            //    m_spf.BRQSPF = str;
                        }
                        //C_JCZQ_SPF_SP brqspf_DS = brqspfList_DS.FirstOrDefault(o => o.MatchId == matchid);
                        //if (brqspf_DS != null)
                        //{
                        //    str = ServiceHelper.getProperties(brqspf_DS, propertyNames);
                        //    sps_DS.BRQSPF = str;
                        //}
                        C_JCZQ_SPF_SP spf = spfList.FirstOrDefault(o => o.MatchId == matchid);
                        if (spf != null)
                        {
                            if (spf.WinOdds == 0M && spf.LoseOdds == 0M && spf.FlatOdds == 0M)
                                spf.NoSaleState = "1";
                            else
                                spf.NoSaleState = "0";
                            str = ServiceHelper.getProperties(spf, propertyNames);
                            sps.SPF = str;
                            sp2x1.SPF = str;
                            if (hh != null)
                                hh.SPF = str;
                            //if (m_spf != null)
                            //    m_spf.SPF = str;

                        }
                        //C_JCZQ_SPF_SP spf_DS = spfList_DS.FirstOrDefault(o => o.MatchId == matchid);
                        //if (spf_DS != null)
                        //{
                        //    str = ServiceHelper.getProperties(spf_DS, propertyNames);
                        //    sps_DS.SPF = str;
                        //}
                        C_JCZQ_BF_SP bf = bfList.FirstOrDefault(o => o.MatchId == matchid);
                        if (bf != null)
                        {
                            if (bf.S_QT == 0M && bf.S_52 == 0M && bf.S_51 == 0M && bf.S_50 == 0M && bf.S_42 == 0M && bf.S_41 == 0M
                                  && bf.S_40 == 0M && bf.S_32 == 0M && bf.S_31 == 0M && bf.S_30 == 0M && bf.S_21 == 0M && bf.S_20 == 0M
                                  && bf.S_10 == 0M && bf.P_QT == 0M && bf.P_33 == 0M && bf.P_22 == 0M && bf.P_11 == 0M && bf.P_00 == 0M
                                  && bf.F_QT == 0M && bf.F_25 == 0M && bf.F_24 == 0M && bf.F_23 == 0M && bf.F_15 == 0M && bf.F_14 == 0M
                                  && bf.F_13 == 0M && bf.F_12 == 0M && bf.F_05 == 0M && bf.F_04 == 0M && bf.F_03 == 0M && bf.F_02 == 0M
                                  && bf.F_01 == 0M)
                                bf.NoSaleState = "1";
                            else
                                bf.NoSaleState = "0";
                            str = ServiceHelper.getProperties(bf, propertyNames);
                            sps.BF = str;
                            msps.BF = str;
                            if (hh != null)
                                hh.BF = str;
                        }
                        //C_JCZQ_BF_SP bf_DS = bfList_DS.FirstOrDefault(o => o.MatchId == matchid);
                        //if (bf_DS != null)
                        //{
                        //    str = ServiceHelper.getProperties(bf_DS, propertyNames);
                        //    sps_DS.BF = str;
                        //}
                        C_JCZQ_ZJQ_SP zjq = zjqList.FirstOrDefault(o => o.MatchId == matchid);
                        if (zjq != null)
                        {
                            if (zjq.JinQiu_7_Odds == 0M && zjq.JinQiu_6_Odds == 0M && zjq.JinQiu_5_Odds == 0M && zjq.JinQiu_4_Odds == 0M && zjq.JinQiu_3_Odds == 0M && zjq.JinQiu_2_Odds == 0M && zjq.JinQiu_1_Odds == 0M && zjq.JinQiu_0_Odds == 0M)
                                zjq.NoSaleState = "1";
                            else
                                zjq.NoSaleState = "0";
                            str = ServiceHelper.getProperties(zjq, propertyNames);
                            sps.ZJQ = str;
                            msps.ZJQ = str;
                            if (hh != null)
                                hh.ZJQ = str;
                            //if (m_zjq != null)
                            //    m_zjq.ZJQ = str;
                        }
                        //C_JCZQ_ZJQ_SP zjq_DS = zjqList_DS.FirstOrDefault(o => o.MatchId == matchid);
                        //if (zjq_DS != null)
                        //{
                        //    str = ServiceHelper.getProperties(zjq_DS, propertyNames);
                        //    sps_DS.ZJQ = str;
                        //}
                        C_JCZQ_BQC_SP bqc = bqcList.FirstOrDefault(o => o.MatchId == matchid);
                        if (bqc != null)
                        {
                            if (bqc.F_F_Odds == 0M && bqc.F_P_Odds == 0M && bqc.F_SH_Odds == 0M && bqc.P_F_Odds == 0M && bqc.P_P_Odds == 0M && bqc.P_SH_Odds == 0M && bqc.SH_F_Odds == 0M && bqc.SH_P_Odds == 0M && bqc.SH_SH_Odds == 0M)
                                bqc.NoSaleState = "1";
                            else
                                bqc.NoSaleState = "0";
                            str = ServiceHelper.getProperties(bqc, propertyNames);
                            sps.BQC = str;
                            msps.BQC = str;
                            if (hh != null)
                                hh.BQC = str;
                            //if (m_bqc != null)
                            //    m_bqc.BQC = str;
                        }
                        //C_JCZQ_BQC_SP bqc_DS = bqcList_DS.FirstOrDefault(o => o.MatchId == matchid);
                        //if (bqc_DS != null)
                        //{
                        //    str = ServiceHelper.getProperties(bqc_DS, propertyNames);
                        //    sps_DS.BQC = str;
                        //}
                        spList.Add(sps);
                        m_spList.Add(msps);
                        sp2x1List.Add(sp2x1);
                        //spList_DS.Add(sps_DS);
                    }
                    #endregion
                }
                if (matchFun == "ok")
                {
                    #region 循环写入SP
                    var il = tempMatchList.Count;
                    var str = string.Empty;
                    string[] propertyNames = new[] { "Id", "MatchData", "MatchId", "MatchNumber", "CreateTime" };

                    for (int i = 0; i < il; i++)
                    {
                        var matchid = tempMatchList[i].MatchId;
                        var sps = new JCZQ_SP();
                        sps.MatchId = tempMatchList[i].MatchId;
                        sps.MatchData = tempMatchList[i].MatchData;
                        sps.MatchNumber = tempMatchList[i].MatchNumber;
                        var hh = match_HH.Where(p => p.MatchId == tempMatchList[i].MatchId).FirstOrDefault();

                        C_JCZQ_SPF_SP brqspf = brqspfList.FirstOrDefault(o => o.MatchId == matchid);
                        if (brqspf != null)
                        {
                            str = ServiceHelper.getProperties(brqspf, propertyNames);
                            sps.BRQSPF = str;
                            if (hh != null)
                                hh.BRQSPF = str;
                        }
                        C_JCZQ_SPF_SP spf = spfList.FirstOrDefault(o => o.MatchId == matchid);
                        if (spf != null)
                        {
                            str = ServiceHelper.getProperties(spf, propertyNames);
                            sps.SPF = str;
                            if (hh != null)
                                hh.SPF = str;
                        }
                        C_JCZQ_BF_SP bf = bfList.FirstOrDefault(o => o.MatchId == matchid);
                        if (bf != null)
                        {
                            str = ServiceHelper.getProperties(bf, propertyNames);
                            sps.BF = str;
                            if (hh != null)
                                hh.BF = str;
                        }
                        C_JCZQ_ZJQ_SP zjq = zjqList.FirstOrDefault(o => o.MatchId == matchid);
                        if (zjq != null)
                        {
                            str = ServiceHelper.getProperties(zjq, propertyNames);
                            sps.ZJQ = str;
                            if (hh != null)
                                hh.ZJQ = str;
                        }
                        C_JCZQ_BQC_SP bqc = bqcList.FirstOrDefault(o => o.MatchId == matchid);
                        if (bqc != null)
                        {
                            str = ServiceHelper.getProperties(bqc, propertyNames);
                            sps.BQC = str;
                            if (hh != null)
                                hh.BQC = str;
                        }
                        spList.Add(sps);
                    }
                    #endregion
                }
                if (matchFun == "zhm")
                {
                    //matchList = tempMatchList;
                    //spfList = Get_SPF_SP_ZHM("JCSPF", matchList);
                    //brqspfList = Get_SPF_SP_ZHM("JCBRQSPF", matchList);
                    //bfList = Get_BF_SP_New();
                    //zjqList = Get_ZJQ_SP_New();
                    //bqcList = Get_BQC_SP_New();
                }
                this.WriteLog(string.Format("采集竞彩足球SP数据完成,SPF:{0}条；BF:{1}条；ZJQ:{2}条；BQC:{3}条", spfList.Count, bfList.Count, zjqList.Count, bqcList.Count));

                #region 采集其它赔率数据

                GetNewJCZQList<JCZQ_SPF_OZ_SPInfo>(ozSPList, "JCZQ_OZ_SP");

                #endregion


                #region 历史对阵
                //采集队伍对阵信息
                //SaveTeamHistoryMatch(matchList);
                #endregion

              

                this.WriteLog("开始对比竞彩足球SP数据");
                //var newMatch_List = GetNewJCZQList<JCZQ_MatchInfo>(matchList, "Match_List.json");
                //GetNewJCZQList<JCZQ_MatchInfo>(matchList_bf, "Match_List_BF.json");
                //GetNewJCZQList<JCZQ_MatchInfo>(matchList_zjq, "Match_List_ZJQ.json");
                //GetNewJCZQList<JCZQ_MatchInfo>(matchList_bqc, "Match_List_BQC.json");
                //mDB.DropCollection("JCZQ_Match_List");
                var newMatch_List = ServiceHelper.BuildNewMatchList<JCZQ_MatchInfo>(mDB, "JCZQ_Match_List", tempMatchList, null, CompareNewJCZQList);


                ServiceHelper.BuildNewMatchList<JCZQ_MatchInfo>(mDB, "JCZQ_Match_List_FB", tempMatchList_FB, null, CompareNewJCZQList);
                ServiceHelper.BuildNewMatchList<JCZQ_MatchInfo>(mDB, "JCZQ_Match_List_BF", tempMatchList_BF, null, CompareNewJCZQList);
                ServiceHelper.BuildNewMatchList<JCZQ_MatchInfo>(mDB, "JCZQ_Match_List_ZJQ", tempMatchList_ZJQ, null, CompareNewJCZQList);
                ServiceHelper.BuildNewMatchList<JCZQ_MatchInfo>(mDB, "JCZQ_Match_List_BQC", tempMatchList_BQC, null, CompareNewJCZQList);

                GetNewJCZQList_HH<C_JCZQ_Match_HH>(match_HH, "JCZQ_Match_List_HH");  //这个有期号
                ServiceHelper.BuildNewMatchList<C_JCZQ_Match_SPF>(mDB, "JCZQ_Match_List_SPF", match_spf, null, CompareNewJCZQList);
                //  ServiceHelper.BuildNewMatchList<JCZQ_MatchInfo>(mDB, "Match_List", tempMatchList, null, CompareNewJCZQList);
                //过关SP
                var newSPF_SP_List = ServiceHelper.BuildNewMatchList<C_JCZQ_SPF_SP>(mDB, "JCZQ_SPF_SP", spfList, null, CompareNewJCZQList);
                var newBRQSPF_SP_List = ServiceHelper.BuildNewMatchList<C_JCZQ_SPF_SP>(mDB, "JCZQ_BRQSPF_SP", brqspfList, null, CompareNewJCZQList);
                var newBF_SP_List = ServiceHelper.BuildNewMatchList<C_JCZQ_BF_SP>(mDB, "JCZQ_BF_SP", bfList, null, CompareNewJCZQList);
                var newZJQ_SP_List = ServiceHelper.BuildNewMatchList<C_JCZQ_ZJQ_SP>(mDB, "JCZQ_ZJQ_SP", zjqList, null, CompareNewJCZQList);
                var newBQC_SP_List = ServiceHelper.BuildNewMatchList<C_JCZQ_BQC_SP>(mDB, "JCZQ_BQC_SP", bqcList, null, CompareNewJCZQList);

                //var newSPF_SP_List = GetNewJCZQList<C_JCZQ_SPF_SP>(spfList, "SPF_SP.json");
                //var newBRQSPF_SP_List = GetNewJCZQList<C_JCZQ_SPF_SP>(brqspfList, "BRQSPF_SP.json");
                //var newBF_SP_List = GetNewJCZQList<C_JCZQ_BF_SP>(bfList, "BF_SP.json");
                //var newZJQ_SP_List = GetNewJCZQList<C_JCZQ_ZJQ_SP>(zjqList, "ZJQ_SP.json");
                //var newBQC_SP_List = GetNewJCZQList<C_JCZQ_BQC_SP>(bqcList, "BQC_SP.json");

                //C_JCZQ_Match_SPF  C_JCZQ_SPF_SP C_JCZQ_SPF_SP C_JCZQ_ZJQ_SP C_JCZQ_BQC_SP  JCZQ_SP JCZQ_2X1SP  JCZQ_Match_SPF_SP

                //  var newMatch_List = GetNewJCZQList<JCZQ_MatchInfo>(tempMatchList, "Match_List.json");


                //GetNewJCZQList<JCZQ_MatchInfo>(tempMatchList_FB, "Match_List_FB.json");
                //GetNewJCZQList<JCZQ_MatchInfo>(tempMatchList_BF, "Match_List_BF.json");
                //GetNewJCZQList<JCZQ_MatchInfo>(tempMatchList_ZJQ, "Match_List_ZJQ.json");
                //GetNewJCZQList<JCZQ_MatchInfo>(tempMatchList_BQC, "Match_List_BQC.json");
                //GetNewJCZQList_HH<C_JCZQ_Match_HH>(match_HH, "Match_List_HH.json");
                //GetNewJCZQList<C_JCZQ_Match_SPF>(match_spf, "Match_List_SPF.json");

                //GetNewJCZQList_New<JCZQ_Match_ZJQ>(match_zjq, "Match_List_ZJQ.json");
                //GetNewJCZQList_New<JCZQ_Match_BQC>(match_bqc, "Match_List_BQC.json");
                //GetNewJCZQList_New<JCZQ_MatchInfo>(tempMatchList_BF, "Match_List_BF.json");
                //过关SP
                //var newSPF_SP_List = GetNewJCZQList<C_JCZQ_SPF_SP>(spfList, "SPF_SP.json");
                //var newBRQSPF_SP_List = GetNewJCZQList<C_JCZQ_SPF_SP>(brqspfList, "BRQSPF_SP.json");
                //var newBF_SP_List = GetNewJCZQList<C_JCZQ_BF_SP>(bfList, "BF_SP.json");
                //var newZJQ_SP_List = GetNewJCZQList<C_JCZQ_ZJQ_SP>(zjqList, "ZJQ_SP.json");
                //var newBQC_SP_List = GetNewJCZQList<C_JCZQ_BQC_SP>(bqcList, "BQC_SP.json");
                //把所以的SP集中
                 ServiceHelper.BuildNewMatchList<JCZQ_SP>(mDB, "JCZQ_SP", spList, null, CompareNewJCZQList);
                ServiceHelper.BuildNewMatchList<JCZQ_2X1SP>(mDB, "JCZQ_EXY_SP", sp2x1List, null, CompareNewJCZQList);
               ServiceHelper.BuildNewMatchList<JCZQ_Match_SPF_SP>(mDB, "JCZQ_HH_SP", m_spList, null, CompareNewJCZQList);

                //GetNewJCZQList<JCZQ_SP>(spList, "SP.json");
                //GetNewJCZQList<JCZQ_2X1SP>(sp2x1List, "EXY_SP.json");
                //GetNewJCZQList<JCZQ_Match_SPF_SP>(m_spList, "HH_SP.json");
                //GetNewJCZQList_New<C_JCZQ_BF_SP>(bfList, "BF_SP.json");


                ////单关SP
                //var newSPF_SP_List_DS = GetNewJCZQList<C_JCZQ_SPF_SP>(spfList_DS, "SPF_SP_DS.json");
                //var newBRQSPF_SP_List_DS = GetNewJCZQList<C_JCZQ_SPF_SP>(brqspfList_DS, "BRQSPF_SP_DS.json");
                //var newBF_SP_List_DS = GetNewJCZQList<C_JCZQ_BF_SP>(bfList_DS, "BF_SP_DS.json");
                //var newZJQ_SP_List_DS = GetNewJCZQList<C_JCZQ_ZJQ_SP>(zjqList_DS, "ZJQ_SP_DS.json");
                //var newBQC_SP_List_DS = GetNewJCZQList<C_JCZQ_BQC_SP>(bqcList_DS, "BQC_SP_DS.json");
                ////把所以的SP集中
                //GetNewJCZQList<JCZQ_SP>(spList_DS, "SP_DS.json");

                this.WriteLog(string.Format("对比竞彩足球SP数据完成,变化的数据 队伍：{0}条；SPF:{1}条；BF:{2}条；ZJQ:{3}条；BQC:{4}条； "
                    , newMatch_List.Count, newSPF_SP_List.Count, newBF_SP_List.Count, newZJQ_SP_List.Count, newBQC_SP_List.Count));

                this.WriteLog("开始对变化的数据发送通知");


                #region 发送 队伍数据通知

                this.WriteLog("1、开始=>发送队伍数据通知");
                var addMatchList = new List<JCZQ_MatchInfo>();
                var updateMatchList = new List<JCZQ_MatchInfo>();
                foreach (var r in newMatch_List)
                {
                    try
                    {
                        if (r.Key == DBChangeState.Add)
                        {
                            addMatchList.Add(r.Value);
                            //manager.AddJCZQ_Match(r.Value);
                        }
                        else
                        {
                            updateMatchList.Add(r.Value);
                            //manager.ModifyJCZQ_Match(r.Value);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(string.Format("向数据库写入 竞彩足球 数据异常 编号：{0}，异常：{1}", r.Value.MatchId, ex.ToString()));
                    }
                }
                if (addMatchList.Count > 0)
                {
                    var category = (int)NoticeCategory.JCZQ_Match;
                    var state = (int)DBChangeState.Add;
                    var param = string.Join("_", (from l in addMatchList select l.MatchId).ToArray());
                    var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                    var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                    //发送 竞彩足球队伍 添加 通知
                    var innerKey = string.Format("{0}_{1}", "JCZQ_MatchInfo", "Add");
                   // ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.JCZQ_Match);
                    new Sports_Business(this.mDB).UpdateLocalData(param, "", NoticeType.JCZQ_Match, innerKey);
                    //ServiceHelper.SendNotice(issuse_Request, (log) =>
                    //{
                    //    this.WriteLog(log);
                    //});
                }
                if (updateMatchList.Count > 0)
                {
                    var category = (int)NoticeCategory.JCZQ_Match;
                    var state = (int)DBChangeState.Update;
                    var param = string.Join("_", (from l in updateMatchList select l.MatchId).ToArray());
                    var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                    var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                    //发送 竞彩足球队伍 修改 通知
                    var innerKey = string.Format("{0}_{1}", "JCZQ_MatchInfo", "Update");
                   // ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.JCZQ_Match);
                    new Sports_Business(this.mDB).UpdateLocalData(param, "", NoticeType.JCZQ_Match, innerKey);

                    //ServiceHelper.SendNotice(issuse_Request, (log) =>
                    //{
                    //    this.WriteLog(log);
                    //});
                }

                this.WriteLog("1、发送队伍数据通知 完成");

                #endregion

                #region 发送SPF SP数据通知

                this.WriteLog("2、开始=>发送SPF SP数据通知");
                var addSPF_SP_List = new List<C_JCZQ_SPF_SP>();
                foreach (var r in newSPF_SP_List)
                {
                    try
                    {
                        addSPF_SP_List.Add(r.Value);
                        //manager.AddJCZQ_SPF_SP(r.Value);
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(string.Format("向数据库写入 竞彩足球 数据异常 编号：{0}，异常：{1}", r.Value.MatchId, ex.ToString()));
                    }
                }
                if (addSPF_SP_List.Count > 0)
                {
                    var category = (int)NoticeCategory.JCZQ_SPF_SP;
                    var state = (int)DBChangeState.Add;
                    var param = string.Join("_", (from l in addSPF_SP_List select l.MatchId).ToArray());
                    var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                    var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                    //发送 竞彩足球队伍SPF SP 添加 通知
                    var innerKey = string.Format("{0}_{1}", "C_JCZQ_SPF_SP", "Add");
                   // ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.JCZQ_SPF_SP);
                    new Sports_Business(this.mDB).UpdateLocalData(param, "", NoticeType.JCZQ_SPF_SP, innerKey);
                    //ServiceHelper.SendNotice(issuse_Request, (log) =>
                    //{
                    //    this.WriteLog(log);
                    //});
                }
                //单关sp
                //var addSPF_SP_List_DS = new List<C_JCZQ_SPF_SP>();
                //foreach (var r in newSPF_SP_List_DS)
                //{
                //    try
                //    {
                //        addSPF_SP_List_DS.Add(r.Value);
                //        //manager.AddJCZQ_SPF_SP(r.Value);
                //    }
                //    catch (Exception ex)
                //    {
                //        this.WriteLog(string.Format("向数据库写入 竞彩足球 数据异常 编号：{0}，异常：{1}", r.Value.MatchId, ex.ToString()));
                //    }
                //}
                //if (addSPF_SP_List_DS.Count > 0)
                //{
                //    var category = (int)NoticeCategory.JCZQ_SPF_SP_DS;
                //    var state = (int)DBChangeState.Add;
                //    var param = string.Join("_", (from l in addSPF_SP_List_DS select l.MatchId).ToArray());
                //    var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                //    var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                //    //发送 竞彩足球队伍SPF SP 添加 通知
                //    var innerKey = string.Format("{0}_{1}", "JCZQ_SPF_SP_DS", "Add");
                //    ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.JCZQ_SPF_SP_DS);

                //    //ServiceHelper.SendNotice(issuse_Request, (log) =>
                //    //{
                //    //    this.WriteLog(log);
                //    //});
                //}
                this.WriteLog("2、发送SPF SP数据通知 完成");

                #endregion

                #region 发送BRQSPF SP数据通知

                this.WriteLog("2、开始=>发送BRQSPF SP数据通知");
                var addBRQSPF_SP_List = new List<C_JCZQ_SPF_SP>();
                foreach (var r in newBRQSPF_SP_List)
                {
                    try
                    {
                        addBRQSPF_SP_List.Add(r.Value);
                        //manager.AddJCZQ_SPF_SP(r.Value);
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(string.Format("向数据库写入 竞彩足球 数据异常 编号：{0}，异常：{1}", r.Value.MatchId, ex.ToString()));
                    }
                }
                if (addBRQSPF_SP_List.Count > 0)
                {
                    var category = (int)NoticeCategory.JCZQ_BRQSPF_SP;
                    var state = (int)DBChangeState.Add;
                    var param = string.Join("_", (from l in addBRQSPF_SP_List select l.MatchId).ToArray());
                    var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                    var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                    //发送 竞彩足球队伍SPF SP 添加 通知
                    var innerKey = string.Format("{0}_{1}", "JCZQ_BRQSPF_SP", "Add");
                    //ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.JCZQ_BRQSPF_SP);
                    new Sports_Business(this.mDB).UpdateLocalData(param, "", NoticeType.JCZQ_BRQSPF_SP, innerKey);
                    //ServiceHelper.SendNotice(issuse_Request, (log) =>
                    //{
                    //    this.WriteLog(log);
                    //});
                }
                //单关sp
                //var addBRQSPF_SP_List_DS = new List<C_JCZQ_SPF_SP>();
                //foreach (var r in newBRQSPF_SP_List_DS)
                //{
                //    try
                //    {
                //        addBRQSPF_SP_List_DS.Add(r.Value);
                //        //manager.AddJCZQ_SPF_SP(r.Value);
                //    }
                //    catch (Exception ex)
                //    {
                //        this.WriteLog(string.Format("向数据库写入 竞彩足球 数据异常 编号：{0}，异常：{1}", r.Value.MatchId, ex.ToString()));
                //    }
                //}
                //if (addBRQSPF_SP_List_DS.Count > 0)
                //{
                //    var category = (int)NoticeCategory.JCZQ_BRQSPF_SP_DS;
                //    var state = (int)DBChangeState.Add;
                //    var param = string.Join("_", (from l in addBRQSPF_SP_List_DS select l.MatchId).ToArray());
                //    var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                //    var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                //    //发送 竞彩足球队伍SPF SP 添加 通知
                //    var innerKey = string.Format("{0}_{1}", "JCZQ_BRQSPF_SP_DS", "Add");
                //    ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.JCZQ_BRQSPF_SP_DS);

                //    //ServiceHelper.SendNotice(issuse_Request, (log) =>
                //    //{
                //    //    this.WriteLog(log);
                //    //});
                //}
                this.WriteLog("2、发送BRQSPF SP数据通知 完成");

                #endregion

                #region 发送BF SP数据通知

                this.WriteLog("3、开始=>发送BF SP数据通知");
                var addBF_SP_List = new List<C_JCZQ_BF_SP>();
                foreach (var r in newBF_SP_List)
                {
                    try
                    {
                        addBF_SP_List.Add(r.Value);
                        //manager.AddJCZQ_BF_SP(r.Value);
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(string.Format("向数据库写入 竞彩足球 数据异常 编号：{0}，异常：{1}", r.Value.MatchId, ex.ToString()));
                    }
                }
                if (addBF_SP_List.Count > 0)
                {
                    var category = (int)NoticeCategory.JCZQ_BF_SP;
                    var state = (int)DBChangeState.Add;
                    var param = string.Join("_", (from l in addBF_SP_List select l.MatchId).ToArray());
                    var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                    var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                    //发送 竞彩足球队伍SPF SP 添加 通知
                    var innerKey = string.Format("{0}_{1}", "C_JCZQ_BF_SP", "Add");
                   // ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.JCZQ_BF_SP);
                    new Sports_Business(this.mDB).UpdateLocalData(param, "", NoticeType.JCZQ_BF_SP, innerKey);
                    //ServiceHelper.SendNotice(issuse_Request, (log) =>
                    //{
                    //    this.WriteLog(log);
                    //});
                }
                //单关sp
                //var addBF_SP_List_DS = new List<C_JCZQ_BF_SP>();
                //foreach (var r in newBF_SP_List_DS)
                //{
                //    try
                //    {
                //        addBF_SP_List_DS.Add(r.Value);
                //        //manager.AddJCZQ_BF_SP(r.Value);
                //    }
                //    catch (Exception ex)
                //    {
                //        this.WriteLog(string.Format("向数据库写入 竞彩足球 数据异常 编号：{0}，异常：{1}", r.Value.MatchId, ex.ToString()));
                //    }
                //}
                //if (addBF_SP_List_DS.Count > 0)
                //{
                //    var category = (int)NoticeCategory.JCZQ_BF_SP_DS;
                //    var state = (int)DBChangeState.Add;
                //    var param = string.Join("_", (from l in addBF_SP_List_DS select l.MatchId).ToArray());
                //    var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                //    var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                //    //发送 竞彩足球队伍SPF SP 添加 通知
                //    var innerKey = string.Format("{0}_{1}", "JCZQ_BF_SP_DS", "Add");
                //    ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.JCZQ_BF_SP_DS);

                //    //ServiceHelper.SendNotice(issuse_Request, (log) =>
                //    //{
                //    //    this.WriteLog(log);
                //    //});
                //}
                this.WriteLog("3、发送BF SP数据通知 完成");

                #endregion

                #region 发送ZJQ SP数据通知

                this.WriteLog("4、开始=>发送ZJQ SP数据通知");
                var addZJQ_SP_List = new List<C_JCZQ_ZJQ_SP>();
                foreach (var r in newZJQ_SP_List)
                {
                    try
                    {
                        addZJQ_SP_List.Add(r.Value);
                        //manager.AddJCZQ_ZJQ_SP(r.Value);
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(string.Format("向数据库写入 竞彩足球 数据异常 编号：{0}，异常：{1}", r.Value.MatchId, ex.ToString()));
                    }
                }
                if (addZJQ_SP_List.Count > 0)
                {
                    var category = (int)NoticeCategory.JCZQ_ZJQ_SP;
                    var state = (int)DBChangeState.Add;
                    var param = string.Join("_", (from l in addZJQ_SP_List select l.MatchId).ToArray());
                    var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                    var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                    //发送 竞彩足球队伍SPF SP 添加 通知
                    var innerKey = string.Format("{0}_{1}", "C_JCZQ_ZJQ_SP", "Add");
                  //  ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.JCZQ_ZJQ_SP);
                    new Sports_Business(this.mDB).UpdateLocalData(param, "", NoticeType.JCZQ_ZJQ_SP, innerKey);
                    //ServiceHelper.SendNotice(issuse_Request, (log) =>
                    //{
                    //    this.WriteLog(log);
                    //});
                }
                //单关sp
                //var addZJQ_SP_List_DS = new List<C_JCZQ_ZJQ_SP>();
                //foreach (var r in newZJQ_SP_List_DS)
                //{
                //    try
                //    {
                //        addZJQ_SP_List_DS.Add(r.Value);
                //        //manager.AddJCZQ_ZJQ_SP(r.Value);
                //    }
                //    catch (Exception ex)
                //    {
                //        this.WriteLog(string.Format("向数据库写入 竞彩足球 数据异常 编号：{0}，异常：{1}", r.Value.MatchId, ex.ToString()));
                //    }
                //}
                //if (addZJQ_SP_List_DS.Count > 0)
                //{
                //    var category = (int)NoticeCategory.JCZQ_ZJQ_SP_DS;
                //    var state = (int)DBChangeState.Add;
                //    var param = string.Join("_", (from l in addZJQ_SP_List_DS select l.MatchId).ToArray());
                //    var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                //    var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                //    //发送 竞彩足球队伍SPF SP 添加 通知
                //    var innerKey = string.Format("{0}_{1}", "JCZQ_ZJQ_SP_DS", "Add");
                //    ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.JCZQ_ZJQ_SP_DS);

                //    //ServiceHelper.SendNotice(issuse_Request, (log) =>
                //    //{
                //    //    this.WriteLog(log);
                //    //});
                //}
                this.WriteLog("4、发送ZJQ SP数据通知 完成");

                #endregion

                #region 发送BQC SP数据通知

                this.WriteLog("5、开始=>发送BQC SP数据通知");
                var addBQC_SP_List = new List<C_JCZQ_BQC_SP>();
                foreach (var r in newBQC_SP_List)
                {
                    try
                    {
                        addBQC_SP_List.Add(r.Value);
                        //manager.AddJCZQ_BQC_SP(r.Value);
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(string.Format("向数据库写入 竞彩足球 数据异常 编号：{0}，异常：{1}", r.Value.MatchId, ex.ToString()));
                    }
                }
                if (addBQC_SP_List.Count > 0)
                {
                    var category = (int)NoticeCategory.JCZQ_BQC_SP;
                    var state = (int)DBChangeState.Add;
                    var param = string.Join("_", (from l in addBQC_SP_List select l.MatchId).ToArray());
                    var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                    var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                    //发送 竞彩足球队伍SPF SP 添加 通知
                    var innerKey = string.Format("{0}_{1}", "C_JCZQ_BQC_SP", "Add");
                  //  ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.JCZQ_BQC_SP);
                    new Sports_Business(this.mDB).UpdateLocalData(param, "", NoticeType.JCZQ_BQC_SP, innerKey);
                    //ServiceHelper.SendNotice(issuse_Request, (log) =>
                    //{
                    //    this.WriteLog(log);
                    //});
                }
                //单关sp
                //var addBQC_SP_List_DS = new List<C_JCZQ_BQC_SP>();
                //foreach (var r in newBQC_SP_List_DS)
                //{
                //    try
                //    {
                //        addBQC_SP_List_DS.Add(r.Value);
                //        //manager.AddJCZQ_BQC_SP(r.Value);
                //    }
                //    catch (Exception ex)
                //    {
                //        this.WriteLog(string.Format("向数据库写入 竞彩足球 数据异常 编号：{0}，异常：{1}", r.Value.MatchId, ex.ToString()));
                //    }
                //}
                //if (addBQC_SP_List_DS.Count > 0)
                //{
                //    var category = (int)NoticeCategory.JCZQ_BQC_SP_DS;
                //    var state = (int)DBChangeState.Add;
                //    var param = string.Join("_", (from l in addBQC_SP_List_DS select l.MatchId).ToArray());
                //    var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                //    var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                //    //发送 竞彩足球队伍SPF SP 添加 通知
                //    var innerKey = string.Format("{0}_{1}", "JCZQ_BQC_SP_DS", "Add");
                //    ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.JCZQ_BQC_SP_DS);

                //    //ServiceHelper.SendNotice(issuse_Request, (log) =>
                //    //{
                //    //    this.WriteLog(log);
                //    //});
                //}
                this.WriteLog("5、发送BQC SP数据通知 完成");

                #endregion

                this.WriteLog("本次采集所有通知发送完成");
            }
            catch (Exception ex)
            {
                this.WriteLog(ex.Message);
                throw ex;
            }
            this.WriteLog("DoWork  完成");
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
        /// 只写发
        /// </summary>
        private List<KeyValuePair<DBChangeState, T>> GetNewJCZQList_HH<T>(List<T> currentList, string tablename)
            where T : C_JCZQ_Match_HH
        {
            var result = new List<KeyValuePair<DBChangeState, T>>();
            if (currentList.Count == 0)
                return result;

            foreach (var item in currentList.GroupBy(p => p.MatchData))
            {
                var issuseNumber = item.Key;
             
              //  var customerSavePath = new string[] { "JCZQ", issuseNumber };
                try
                {
                    var coll = mDB.GetCollection<T>(tablename);
                    var mFilter = MongoDB.Driver.Builders<T>.Filter.Eq(b=>b.MatchData, issuseNumber);// & Builders<BsonDocument>.Filter.Eq("IssuseNumber", issuseNumber);
                    coll.DeleteMany(mFilter);
                
                    coll.InsertMany(item.ToList());
                    //if (!Directory.Exists(path))
                    //    Directory.CreateDirectory(path);
                    //var fullFileName = Path.Combine(path, fileName);
                    //ServiceHelper.CreateOrAppend_JSONFile(fullFileName, JsonSerializer.Serialize(currentList.Where(p => p.MatchData == issuseNumber).ToList()), (log) =>
                    //{
                    //    this.WriteLog(log);
                    //});

                    ////上传文件
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

            //SaveHistory_HH<T>(currentList, fileName);

            //var fileFullName = BuildFileFullName(fileName);
            //var customerSavePath = new string[] { "JCZQ" };
            //if (File.Exists(fileFullName))
            //{
            //    var text = File.ReadAllText(fileFullName).Trim().Replace("var data=", "").Replace("];", "]");
            //    var oldList = string.IsNullOrEmpty(text) ? new List<T>() : JsonSerializer.Deserialize<List<T>>(text);
            //    ServiceHelper.CreateOrAppend_JSONFile(fileFullName, JsonSerializer.Serialize(currentList), (log) =>
            //    {
            //        this.WriteLog(log);
            //    });

            //    //上传文件
            //    ServiceHelper.PostFileToServer(fileFullName, customerSavePath, (log) =>
            //    {
            //        this.WriteLog(log);
            //    });
            //    return result;
            //}
            //try
            //{
            //    ServiceHelper.CreateOrAppend_JSONFile(fileFullName, JsonSerializer.Serialize(currentList), (log) =>
            //    {
            //        this.WriteLog(log);
            //    });

            //    //上传文件
            //    ServiceHelper.PostFileToServer(fileFullName, customerSavePath, (log) =>
            //    {
            //        this.WriteLog(log);
            //    });
            //}
            //catch (Exception ex)
            //{
            //    this.WriteLog(string.Format("第一次写入JCZQ数据文件 {0} 失败：{1}", fileFullName, ex.ToString()));
            //}
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="currentList"></param>
        /// <param name="tablename"></param>
        /// <returns></returns>
        private List<KeyValuePair<DBChangeState, T>> GetNewJCZQList<T>(List<T> currentList, string tablename)
           where T : JCZQ_SPF_OZ_SPInfo
        {
            var result = new List<KeyValuePair<DBChangeState, T>>();
            if (currentList.Count == 0)
                return result;

           
         //   mDB.DropCollection(tablename);
            var coll = mDB.GetCollection<T>(tablename);
            coll.DeleteMany(Builders<T>.Filter.Empty);// mFilter = Builders<T>.Filter.Empty;
            coll.InsertMany(currentList);
        
            
            return result;
        }

        #region New



        //private void SaveHistory_New<T>(List<T> currentList, string fileName)
        //   where T : EntityModel.LotteryJsonInfo.JCZQBase
        //{
        //    if (string.IsNullOrEmpty(SavePath_New))
        //        SavePath_New = ServiceHelper.Get_JCZQ_SavePath_New();
        //    foreach (var item in currentList.GroupBy(p => p.MatchData))
        //    {
        //        var issuseNumber = item.Key;
        //        var path = Path.Combine(SavePath_New, issuseNumber);
        //        var customerSavePath = new string[] { "JCZQ", "New", issuseNumber };
        //        try
        //        {
        //            if (!Directory.Exists(path))
        //                Directory.CreateDirectory(path);
        //            var fullFileName = Path.Combine(path, fileName);
        //            ServiceHelper.CreateOrAppend_JSONFile(fullFileName, JsonSerializer.Serialize(currentList.Where(p => p.MatchData == issuseNumber).ToList()), (log) =>
        //            {
        //                this.WriteLog(log);
        //            });

        //            //上传文件
        //            ServiceHelper.PostFileToServer(fullFileName, customerSavePath, (log) =>
        //            {
        //                this.WriteLog(log);
        //            });
        //        }
        //        catch (Exception ex)
        //        {
        //            this.WriteLog("保存历史数据失败：" + ex.ToString());
        //        }
        //    }
        //}

      

        #endregion

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

        public List<JCZQ_MatchInfo> GetZS_MatchList()
        {
            var list = new List<JCZQ_MatchInfo>();
            //var clUrl = "http://live.iqucai.com/apps?lotyid=6";
            var clUrl = "http://live.wancai.com/apps?lotyid=6";

            var clDoc = new XmlDocument();
            var clContent = PostManager.Get(clUrl, Encoding.UTF8, 0, (request) =>
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
            if (clContent == "404")
                return list;

            var dic = JsonHelper.Deserialize(clContent) as Dictionary<string, object>;
            foreach (var item in dic)
            {
                //htid  atid lid mid
                var matchId = item.Key;
                var v = item.Value as Dictionary<string, object>;
                var htid = v.ContainsKey("htid") ? v["htid"].ToString() : "0";
                var atid = v.ContainsKey("atid") ? v["atid"].ToString() : "0";
                var lid = v.ContainsKey("lid") ? v["lid"].ToString() : "0";
                var mid = v.ContainsKey("mid") ? v["mid"].ToString() : "0";
                var cl = v.ContainsKey("cl") ? v["cl"].ToString() : "";
                var hname = v.ContainsKey("home") ? v["home"].ToString() : "";
                var gname = v.ContainsKey("away") ? v["away"].ToString() : "";

                list.Add(new JCZQ_MatchInfo
                {
                    MatchId = matchId,
                    HomeTeamId = int.Parse(htid),
                    GuestTeamId = int.Parse(atid),
                    LeagueId = int.Parse(lid),
                    Mid = int.Parse(mid),
                    LeagueColor = cl,
                    HomeTeamName = hname,
                    GuestTeamName = gname
                    //FXId = int.Parse(sid),
                });
            }
            return list;
        }

        //                   _ooOoo_
        //                  o8888888o
        //                  88" . "88
        //                  (| -_- |)
        //                  O\  =  /O
        //               ____/`---'\____
        //             .'  \\|     |//  `.
        //            /  \\|||  :  |||//  \
        //           /  _||||| -:- |||||-  \
        //           |   | \\\  -  /// |   |
        //           | \_|  ''\---/''  |   |
        //           \  .-\__  `-`  ___/-. /
        //         ___`. .'  /--.--\  `. . __
        //      ."" '<  `.___\_<|>_/___.'  >'"".
        //     | | :  `- \`.;`\ _ /`;.`/ - ` : | |
        //     \  \ `-.   \_ __\ /__ _/   .-` /  /
        //======`-.____`-.___\_____/___.-`____.-'======
        //                   `=---='
        //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
        //         佛祖保佑       永无BUG


        private Dictionary<string, string> _MatchIdList = new Dictionary<string, string>();

        private Dictionary<string, string> _StopTimeList = new Dictionary<string, string>();

        private Dictionary<string, string> _OPList = new Dictionary<string, string>();

        private List<IndexMatchInfo> _indexMatchInfoList = new List<IndexMatchInfo>();

        public List<JCZQ_MatchInfo> GetMatchList_ZGJCW_SPF(out List<JCZQ_SPF_OZ_SPInfo> spOZ)
        {
            this.WriteLog("进入方法 GetMatchList_ZGJCW_SPF");
            var matchList = new List<JCZQ_MatchInfo>();
            spOZ = new List<JCZQ_SPF_OZ_SPInfo>();
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long tt = (DateTime.Now.Ticks - startTime.Ticks) / 10000;
            var url = string.Format("http://i.sporttery.cn/odds_calculator/get_odds?i_format=json&i_callback=getData&poolcode%5B%5D=hhad&poolcode%5B%5D=had&_={0}", tt);
            var encoding = Encoding.GetEncoding("gb2312");
            var content = PostManager.Get(url, encoding, 0, null);
            content = content.Replace("getData(", "").Replace(");", "").Trim();
            if (string.IsNullOrEmpty(content) || content == "404")
                return matchList;
            var array = JsonHelper.Decode(content);

            var zsMathList = GetZS_MatchList();

            var indexMatchInfoList = new List<IndexMatchInfo>();
            var indexMatchList = new IndexMatch_Collection();
            foreach (var item in zsMathList)
            {
                //if (item.GuestTeamId.ToString() != "281") continue;
                if (_indexMatchInfoList.Where(p => p.MatchId == item.HomeTeamId.ToString()).Count() == 0)
                {
                    indexMatchInfoList.Add(new IndexMatchInfo
                    {
                        CreateTime = DateTime.Now,
                        ImgPath = string.Empty,
                        MatchId = item.HomeTeamId.ToString(),
                        MatchName = item.HomeTeamName
                    });
                }

                if (_indexMatchInfoList.Where(p => p.MatchId == item.GuestTeamId.ToString()).Count() == 0)
                {
                    indexMatchInfoList.Add(new IndexMatchInfo
                    {
                        CreateTime = DateTime.Now,
                        ImgPath = string.Empty,
                        MatchId = item.GuestTeamId.ToString(),
                        MatchName = item.GuestTeamName
                    });
                }
            }

            if (indexMatchInfoList.Count > 0)
            {
                try
                {
                    indexMatchList = new Sports_Business().AddIndexMatch(JsonHelper.Serialize(indexMatchInfoList));
                  //  indexMatchList = ServiceHelper.QueryIndexMatch(JsonSerializer.Serialize<List<IndexMatchInfo>>(indexMatchInfoList));
                }
                catch
                {
                }
            }

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
                        string id = match_D.id;
                        object crs = match_D.crs;

                        string time = match_D.time;
                        string date = match_D.date;
                        string l_background_color = match_D.l_background_color;

                        string matchId = b_date.Substring(2).Replace("-", "") + num.Substring(2);
                        var color = string.Format("#{0}", l_background_color);
                        var matchData = matchId.Substring(0, 6);
                        var zsMatch = zsMathList.FirstOrDefault(p => p.MatchId == matchId);
                        var matchNumber = matchId.Substring(6);
                        var matchConfig = DisableMatchConfigList.FirstOrDefault(p => p.MatchId.Trim() == matchId);
                        var state = "0";
                        var matchStopDesc = string.Empty;
                        object hhad = match_D.hhad;
                        object had = match_D.had;
                        if (!(hhad == null))
                        {
                            int single = match_D.hhad.single;
                            if (single == 1)
                                state = "R";
                        }
                        if (!(had == null))
                        {
                            int single = match_D.had.single;
                            if (single == 1)
                                state = "NR";
                        }
                        var op = string.IsNullOrEmpty(_OPList.FirstOrDefault(p => p.Key == id).Value) ? new string[] { } : _OPList.FirstOrDefault(p => p.Key == id).Value.Split('|');

                        var fsStopBettingTime = DateTime.Now;
                        var dateTime = string.Format("{0}|{1} {2}", b_date, date, time).Split('|');
                        var hh = Convert.ToDateTime(dateTime[1]).Hour;
                        var hhm = string.Format("{0}.{1}", Convert.ToDateTime(dateTime[1]).Hour, Convert.ToDateTime(dateTime[1]).Minute);
                        var st = Convert.ToDateTime(dateTime[0]);
                        var et = Convert.ToDateTime(dateTime[1]);
                        TimeSpan ts = et - st;
                        if (ts.Days >= 2)
                        {
                            dateTime[0] = st.AddDays(1).ToString("yyyy-MM-dd");
                        }
                        var dateWeek = Convert.ToDateTime(dateTime[0]);
                        var week = CaculateWeekDay(dateWeek.Year, dateWeek.Month, dateWeek.Day);

                        var homeTeamName = match_D.h_cn;
                        var guestTeamName = match_D.a_cn;
                        string leagueName = match_D.l_cn;

                        var isTrue = false;
                        if (leagueName == "欧洲杯")
                        {
                            if (!_ozbList.Contains(b_date.Substring(2).Replace("-", "")))
                                _ozbList.Add(b_date.Substring(2).Replace("-", ""));
                        }

                        var isOzb = false;
                        if (_ozbList.Contains(b_date.Substring(2).Replace("-", "")))
                            isOzb = true;

                        if (leagueName == "世界杯")
                        {
                            if (!_sjbList.Contains(b_date.Substring(2).Replace("-", "")))
                                _sjbList.Add(b_date.Substring(2).Replace("-", ""));
                        }
                        var isSjb = false;
                        if (_sjbList.Contains(b_date.Substring(2).Replace("-", "")))
                            isSjb = true;


                        while (true)
                        {
                            if (isTrue)
                            {
                                fsStopBettingTime = fsStopBettingTime.AddMinutes(JCZQ_advanceMinutes);
                            }
                            else
                            {
                                if (hh >= 0 && decimal.Parse(hhm) < 9.30M)
                                    fsStopBettingTime = isOzb ?
                                        ((hh >= 3 && decimal.Parse(hhm) < 9.30M) ? Convert.ToDateTime(string.Format("{0} {1}", Convert.ToDateTime(dateTime[1]).ToString("yyyy-MM-dd"), "03:00:00")).AddMinutes(JCZQ_advanceMinutes) : Convert.ToDateTime(dateTime[1]).AddMinutes(JCZQ_advanceMinutes))
                                        : (week == "6" || week == "7") ?
                                        ((hh >= 1 && decimal.Parse(hhm) < 9.30M) ? Convert.ToDateTime(string.Format("{0} {1}", Convert.ToDateTime(dateTime[1]).ToString("yyyy-MM-dd"), "01:00:00")).AddMinutes(JCZQ_advanceMinutes) : Convert.ToDateTime(dateTime[1]).AddMinutes(JCZQ_advanceMinutes))
                                        : Convert.ToDateTime(string.Format("{0} {1}", dateTime[0], "23:59:00")).AddMinutes(JCZQ_advanceMinutes).AddMinutes(1);
                                else
                                    fsStopBettingTime = Convert.ToDateTime(dateTime[1]).AddMinutes(JCZQ_advanceMinutes);
                                isTrue = true;
                            }

                            if ((fsStopBettingTime.Hour >= (isOzb ? 3 : (week == "6" || week == "7") ? 1 : 0) && fsStopBettingTime.Minute >= 0)
                                && (fsStopBettingTime.Hour <= 9))
                            {
                                if (fsStopBettingTime.Minute >= 0 && fsStopBettingTime.Hour == 9)
                                    break;
                                hh = fsStopBettingTime.Hour;
                                hhm = string.Format("{0}.{1}", fsStopBettingTime.Hour, fsStopBettingTime.Minute.ToString("D2"));
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (isSjb)
                        {
                            fsStopBettingTime = Convert.ToDateTime(dateTime[1]).AddMinutes(JCZQ_advanceMinutes);
                        }

                        var leagueId = zsMatch != null ? zsMatch.LeagueId : 0;
                        var homeTeamId = zsMatch != null ? zsMatch.HomeTeamId : 0;
                        var guestTeamId = zsMatch != null ? zsMatch.GuestTeamId : 0;

                        var hIndexMatch = indexMatchList.IndexMatchList.FirstOrDefault(p => p.MatchId == homeTeamId.ToString());
                        var gIndexMatch = indexMatchList.IndexMatchList.FirstOrDefault(p => p.MatchId == guestTeamId.ToString());



                        if (hIndexMatch != null && !string.IsNullOrEmpty(hIndexMatch.ImgPath))
                        {
                            _indexMatchInfoList.Add(hIndexMatch);
                        }
                        else if (hIndexMatch == null)
                        {
                            hIndexMatch = _indexMatchInfoList.FirstOrDefault(p => p.MatchId == homeTeamId.ToString());
                        }
                        if (gIndexMatch != null && !string.IsNullOrEmpty(gIndexMatch.ImgPath))
                        {
                            _indexMatchInfoList.Add(gIndexMatch);
                        }
                        else if (gIndexMatch == null)
                        {
                            gIndexMatch = _indexMatchInfoList.FirstOrDefault(p => p.MatchId == guestTeamId.ToString());
                        }

                        matchList.Add(new JCZQ_MatchInfo
                        {
                            MatchId = matchId,
                            MatchData = matchData,
                            MatchNumber = matchNumber,
                            MatchIdName = match_D.num,
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            LeagueId = leagueId,  //int.Parse(item.li),
                            LeagueName = leagueName,
                            ShortLeagueName = match_D.l_cn_abbr,
                            LeagueColor = zsMatch != null ? zsMatch.LeagueColor : color,
                            HomeTeamId = homeTeamId,// team == null ? 0 : team.Attributes["htid"].Value.GetInt32(),
                            HomeTeamName = homeTeamName,
                            ShortHomeTeamName = match_D.h_cn_abbr,
                            GuestTeamId = guestTeamId,// team == null ? 0 : team.Attributes["gtid"].Value.GetInt32(),
                            GuestTeamName = guestTeamName,
                            ShortGuestTeamName = match_D.a_cn_abbr,
                            LetBall = match_D.hhad.fixedodds,
                            WinOdds = op.Length >= 1 ? op[0].GetDecimal() : 0M,
                            FlatOdds = op.Length >= 1 ? op[1].GetDecimal() : 0M,
                            LoseOdds = op.Length >= 1 ? op[2].GetDecimal() : 0M,
                            StartDateTime = DateTime.Parse(string.Format("{0} {1}", match_D.date, match_D.time)).ToString("yyyy-MM-dd HH:mm:ss"),
                            DSStopBettingTime = fsStopBettingTime.AddMinutes(-5).ToString("yyyy-MM-dd HH:mm:ss"),
                            FSStopBettingTime = fsStopBettingTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            Mid = zsMatch != null ? zsMatch.Mid : 0,
                            FXId = zsMatch != null ? zsMatch.FXId : 0,
                            //为了配置队伍对阵历史                   
                            Hi = match_D.h_id,
                            Gi = match_D.a_id,
                            State = state,
                            MatchStopDesc = "1",
                            HomeImgPath = hIndexMatch != null ? hIndexMatch.ImgPath : string.Empty,
                            GuestImgPath = gIndexMatch != null ? gIndexMatch.ImgPath : string.Empty,
                            PrivilegesType = matchConfig == null ? string.Empty : matchConfig.PrivilegesType,
                        });

                        spOZ.Add(new JCZQ_SPF_OZ_SPInfo
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            MatchId = matchId,
                            MatchData = matchData,
                            MatchNumber = matchNumber,
                            WinOdds = op.Length >= 1 ? op[0].GetDecimal() : 0M,
                            FlatOdds = op.Length >= 1 ? op[1].GetDecimal() : 0M,
                            LoseOdds = op.Length >= 1 ? op[2].GetDecimal() : 0M,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                this.WriteLog("解析array错误 " + ex.ToString());
            }
            return matchList;
        }
        public List<JCZQ_MatchInfo> GetMatchList_ZGJCW_SPF_FB()
        {
            _MatchStatus = new Dictionary<string, string>();
            this.WriteLog("进入方法 GetMatchList_ZGJCW_SPF");
            var matchList = new List<JCZQ_MatchInfo>();
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long tt = (DateTime.Now.Ticks - startTime.Ticks) / 10000;
            var url = string.Format("http://i.sporttery.cn/odds_calculator/get_odds?i_format=json&i_callback=getData&poolcode%5B%5D=hhad&poolcode%5B%5D=had&_={0}", tt);
            var encoding = Encoding.GetEncoding("gb2312");
            var content = PostManager.Get(url, encoding, 0, null);
            content = content.Replace("getData(", "").Replace(");", "").Trim();
            if (string.IsNullOrEmpty(content) || content == "404")
                return matchList;
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
                        string date = match_D.date;
                        string id = match_D.id;
                        string time = match_D.time;
                        string num = match_D.num;
                        string l_background_color = match_D.l_background_color;

                        string matchId = b_date.Substring(2).Replace("-", "") + num.Substring(2);

                        var color = string.Format("#{0}", l_background_color);
                        var matchData = matchId.Substring(0, 6);
                        var zsMatch = zsMathList.FirstOrDefault(p => p.MatchId == matchId);
                        var matchNumber = matchId.Substring(6);
                        var matchConfig = DisableMatchConfigList.FirstOrDefault(p => p.MatchId.Trim() == matchId);
                        var state = "0";
                        string matchStopDesc = string.Empty;
                         
                        if (!(match_D.hhad == null))
                        {
                            int single = match_D.hhad.single;
                            if (single == 1)
                            {
                                state += "R";
                                state = state.Replace("0", "");
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
                            matchStopDesc = match_D.hhad.cbt;
                            //if (!string.IsNullOrEmpty(match_D.hhad.cbt))
                            //    matchStopDesc = match_D.hhad.cbt;
                        }
                        if (!(match_D.had == null))
                        {
                            int single = match_D.had.single;
                            if (single == 1)
                            {
                                state += "NR";
                                state = state.Replace("0", "");
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
                        var op = string.IsNullOrEmpty(_OPList.FirstOrDefault(p => p.Key == id).Value) ? new string[] { } : _OPList.FirstOrDefault(p => p.Key ==id).Value.Split('|');

                        var fsStopBettingTime = DateTime.Now;
                        var dateTime = string.Format("{0}|{1} {2}", b_date , date, time).Split('|');
                        var hh = Convert.ToDateTime(dateTime[1]).Hour;
                        var hhm = string.Format("{0}.{1}", Convert.ToDateTime(dateTime[1]).Hour, Convert.ToDateTime(dateTime[1]).Minute);
                        var st = Convert.ToDateTime(dateTime[0]);
                        var et = Convert.ToDateTime(dateTime[1]);
                        TimeSpan ts = et - st;
                        if (ts.Days >= 2)
                        {
                            dateTime[0] = st.AddDays(1).ToString("yyyy-MM-dd");
                        }
                        var dateWeek = Convert.ToDateTime(dateTime[0]);
                        var week = CaculateWeekDay(dateWeek.Year, dateWeek.Month, dateWeek.Day);

                        string homeTeamName = match_D.h_cn;
                        string guestTeamName = match_D.a_cn;
                        string leagueName = match_D.l_cn;

                        var isTrue = false;
                        if (leagueName == "欧洲杯")
                        {
                            if (!_ozbList.Contains(b_date.Substring(2).Replace("-", "")))
                                _ozbList.Add(b_date.Substring(2).Replace("-", ""));
                        }
                        var isOzb = false;
                        if (_ozbList.Contains(b_date.Substring(2).Replace("-", "")))
                            isOzb = true;

                        if (leagueName == "世界杯")
                        {
                            if (!_sjbList.Contains(b_date.Substring(2).Replace("-", "")))
                                _sjbList.Add(b_date.Substring(2).Replace("-", ""));
                        }
                        var isSjb = false;
                        if (_sjbList.Contains(b_date.Substring(2).Replace("-", "")))
                            isSjb = true;

                        while (true)
                        {
                            if (isTrue)
                            {
                                fsStopBettingTime = fsStopBettingTime.AddMinutes(JCZQ_advanceMinutes);
                            }
                            else
                            {
                                if (hh >= 0 && decimal.Parse(hhm) < 9.30M)
                                    fsStopBettingTime = isOzb ?
                                        ((hh >= 3 && decimal.Parse(hhm) < 9.30M) ? Convert.ToDateTime(string.Format("{0} {1}", Convert.ToDateTime(dateTime[1]).ToString("yyyy-MM-dd"), "03:00:00")).AddMinutes(JCZQ_advanceMinutes) : Convert.ToDateTime(dateTime[1]).AddMinutes(JCZQ_advanceMinutes))
                                        : (week == "6" || week == "7") ?
                                        ((hh >= 1 && decimal.Parse(hhm) < 9.30M) ? Convert.ToDateTime(string.Format("{0} {1}", Convert.ToDateTime(dateTime[1]).ToString("yyyy-MM-dd"), "01:00:00")).AddMinutes(JCZQ_advanceMinutes) : Convert.ToDateTime(dateTime[1]).AddMinutes(JCZQ_advanceMinutes))
                                        : Convert.ToDateTime(string.Format("{0} {1}", dateTime[0], "23:59:00")).AddMinutes(JCZQ_advanceMinutes).AddMinutes(1);
                                else
                                    fsStopBettingTime = Convert.ToDateTime(dateTime[1]).AddMinutes(JCZQ_advanceMinutes);
                                isTrue = true;
                            }

                            if ((fsStopBettingTime.Hour >= (isOzb ? 3 : (week == "6" || week == "7") ? 1 : 0) && fsStopBettingTime.Minute >= 0)
                                && (fsStopBettingTime.Hour <= 9))
                            {
                                if (fsStopBettingTime.Minute >= 0 && fsStopBettingTime.Hour == 9)
                                    break;
                                hh = fsStopBettingTime.Hour;
                                hhm = string.Format("{0}.{1}", fsStopBettingTime.Hour, fsStopBettingTime.Minute.ToString("D2"));
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (isSjb)
                        {
                            fsStopBettingTime = Convert.ToDateTime(dateTime[1]).AddMinutes(JCZQ_advanceMinutes);
                        }

                        var leagueId = zsMatch != null ? zsMatch.LeagueId : 0;
                        var homeTeamId = zsMatch != null ? zsMatch.HomeTeamId : 0;
                        var guestTeamId = zsMatch != null ? zsMatch.GuestTeamId : 0;

                         

                        matchList.Add(new JCZQ_MatchInfo
                        {
                            MatchId = matchId,
                            MatchData = matchData,
                            MatchNumber = matchNumber,
                            MatchIdName = match_D.num,
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            LeagueId = leagueId,  //int.Parse(item.li),
                            LeagueName = leagueName,
                            ShortLeagueName = match_D.l_cn_abbr,
                            LeagueColor = zsMatch != null ? zsMatch.LeagueColor : color,
                            HomeTeamId = homeTeamId,// team == null ? 0 : team.Attributes["htid"].Value.GetInt32(),
                            HomeTeamName = homeTeamName,
                            ShortHomeTeamName = match_D.h_cn_abbr,
                            GuestTeamId = guestTeamId,// team == null ? 0 : team.Attributes["gtid"].Value.GetInt32(),
                            GuestTeamName = guestTeamName,
                            ShortGuestTeamName = match_D.a_cn_abbr,
                            LetBall = match_D.hhad.fixedodds,
                            WinOdds = op.Length >= 1 ? op[0].GetDecimal() : 0M,
                            FlatOdds = op.Length >= 1 ? op[1].GetDecimal() : 0M,
                            LoseOdds = op.Length >= 1 ? op[2].GetDecimal() : 0M,
                            StartDateTime = DateTime.Parse(string.Format("{0} {1}",date,time)).ToString("yyyy-MM-dd HH:mm:ss"),
                            DSStopBettingTime = fsStopBettingTime.AddMinutes(-5).ToString("yyyy-MM-dd HH:mm:ss"),
                            FSStopBettingTime = fsStopBettingTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            Mid = zsMatch != null ? zsMatch.Mid : 0,
                            FXId = zsMatch != null ? zsMatch.FXId : 0,
                            //为了配置队伍对阵历史                   
                            Hi = match_D.h_id,
                            Gi = match_D.a_id,
                            PrivilegesType = matchConfig == null ? string.Empty : matchConfig.PrivilegesType,
                            State = state,
                            MatchStopDesc = "1",
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                this.WriteLog("解析array错误 " + ex.ToString());
            }
            return matchList;
        }

        public List<JCZQ_MatchInfo> GetMatchList_ZGJCW_BF()
        {
            this.WriteLog("进入方法 GetMatchList_ZGJCW_BF");
            var matchList = new List<JCZQ_MatchInfo>();
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long tt = (DateTime.Now.Ticks - startTime.Ticks) / 10000;
            var url = string.Format("http://i.sporttery.cn/odds_calculator/get_odds?i_format=json&i_callback=getData&poolcode%5B%5D=crs&_={0}", tt);
            var encoding = Encoding.GetEncoding("gb2312");
            var content = PostManager.Get(url, encoding, 0, null);
            content = content.Replace("getData(", "").Replace(");", "").Trim();
            if (string.IsNullOrEmpty(content) || content == "404")
                return matchList;
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
                        string id = match_D.id;
                        object crs = match_D.crs;

                        string time = match_D.time;
                        string date = match_D.date;

                        string l_background_color = match_D.l_background_color;
                        string matchId =b_date.Substring(2).Replace("-", "") + num.Substring(2);
                        var color = string.Format("#{0}", l_background_color);
                        var matchData = matchId.Substring(0, 6);
                        var zsMatch = zsMathList.FirstOrDefault(p => p.MatchId == matchId);
                        var matchNumber = matchId.Substring(6);
                        var matchConfig = DisableMatchConfigList.FirstOrDefault(p => p.MatchId.Trim() == matchId);
                        var state = "0";
                        var matchStopDesc = string.Empty;

                      

                        if (!(crs == null))
                        {
                            int single = match_D.crs.single;

                            if (single == 1)
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
                          //  if (!string.IsNullOrEmpty(match_D.crs.cbt))
                                matchStopDesc = match_D.crs.cbt;
                        }
                        //var op = _OPList.FirstOrDefault(p => p.Key == match_D.id).Value.Split('|');
                        var op = string.IsNullOrEmpty(_OPList.FirstOrDefault(p => p.Key == id).Value) ? new string[] { } : _OPList.FirstOrDefault(p => p.Key == id).Value.Split('|');
                        var fsStopBettingTime = DateTime.Now;
                        var dateTime = string.Format("{0}|{1} {2}", b_date, date,time).Split('|');
                        var hh = Convert.ToDateTime(dateTime[1]).Hour;
                        var hhm = string.Format("{0}.{1}", Convert.ToDateTime(dateTime[1]).Hour, Convert.ToDateTime(dateTime[1]).Minute);
                        var st = Convert.ToDateTime(dateTime[0]);
                        var et = Convert.ToDateTime(dateTime[1]);
                        TimeSpan ts = et - st;
                        if (ts.Days >= 2)
                        {
                            dateTime[0] = st.AddDays(1).ToString("yyyy-MM-dd");
                        }
                        var dateWeek = Convert.ToDateTime(dateTime[0]);
                        var week = CaculateWeekDay(dateWeek.Year, dateWeek.Month, dateWeek.Day);
                        string homeTeamName = match_D.h_cn;
                        string guestTeamName = match_D.a_cn;
                        string leagueName = match_D.l_cn;

                        var isTrue = false;
                        if (leagueName == "欧洲杯")
                        {
                            if (!_ozbList.Contains(b_date.Substring(2).Replace("-", "")))
                                _ozbList.Add(b_date.Substring(2).Replace("-", ""));
                        }
                        var isOzb = false;
                        if (_ozbList.Contains(b_date.Substring(2).Replace("-", "")))
                            isOzb = true;

                        if (leagueName == "世界杯")
                        {
                            if (!_sjbList.Contains(b_date.Substring(2).Replace("-", "")))
                                _sjbList.Add(b_date.Substring(2).Replace("-", ""));
                        }
                        var isSjb = false;
                        if (_sjbList.Contains(b_date.Substring(2).Replace("-", "")))
                            isSjb = true;

                        while (true)
                        {
                            if (isTrue)
                            {
                                fsStopBettingTime = fsStopBettingTime.AddMinutes(JCZQ_advanceMinutes);
                            }
                            else
                            {
                                if (hh >= 0 && decimal.Parse(hhm) < 9.30M)
                                    fsStopBettingTime = isOzb ?
                                        ((hh >= 3 && decimal.Parse(hhm) < 9.30M) ? Convert.ToDateTime(string.Format("{0} {1}", Convert.ToDateTime(dateTime[1]).ToString("yyyy-MM-dd"), "03:00:00")).AddMinutes(JCZQ_advanceMinutes) : Convert.ToDateTime(dateTime[1]).AddMinutes(JCZQ_advanceMinutes))
                                        : (week == "6" || week == "7") ?
                                        ((hh >= 1 && decimal.Parse(hhm) < 9.30M) ? Convert.ToDateTime(string.Format("{0} {1}", Convert.ToDateTime(dateTime[1]).ToString("yyyy-MM-dd"), "01:00:00")).AddMinutes(JCZQ_advanceMinutes) : Convert.ToDateTime(dateTime[1]).AddMinutes(JCZQ_advanceMinutes))
                                        : Convert.ToDateTime(string.Format("{0} {1}", dateTime[0], "23:59:00")).AddMinutes(JCZQ_advanceMinutes).AddMinutes(1);
                                else
                                    fsStopBettingTime = Convert.ToDateTime(dateTime[1]).AddMinutes(JCZQ_advanceMinutes);
                                isTrue = true;
                            }

                            if ((fsStopBettingTime.Hour >= (isOzb ? 3 : (week == "6" || week == "7") ? 1 : 0) && fsStopBettingTime.Minute >= 0)
                                && (fsStopBettingTime.Hour <= 9))
                            {
                                if (fsStopBettingTime.Minute >= 0 && fsStopBettingTime.Hour == 9)
                                    break;
                                hh = fsStopBettingTime.Hour;
                                hhm = string.Format("{0}.{1}", fsStopBettingTime.Hour, fsStopBettingTime.Minute.ToString("D2"));
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (isSjb)
                        {
                            fsStopBettingTime = Convert.ToDateTime(dateTime[1]).AddMinutes(JCZQ_advanceMinutes);
                        }

                        var leagueId = zsMatch != null ? zsMatch.LeagueId : 0;
                        var homeTeamId = zsMatch != null ? zsMatch.HomeTeamId : 0;
                        var guestTeamId = zsMatch != null ? zsMatch.GuestTeamId : 0;

                        matchList.Add(new JCZQ_MatchInfo
                        {
                            MatchId = matchId,
                            MatchData = matchData,
                            MatchNumber = matchNumber,
                            MatchIdName = match_D.num,
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            LeagueId = leagueId,  //int.Parse(item.li),
                            LeagueName = leagueName,
                            ShortLeagueName = match_D.l_cn_abbr,
                            LeagueColor = zsMatch != null ? zsMatch.LeagueColor : color,
                            HomeTeamId = homeTeamId,// team == null ? 0 : team.Attributes["htid"].Value.GetInt32(),
                            HomeTeamName = homeTeamName,
                            ShortHomeTeamName = match_D.h_cn_abbr,
                            GuestTeamId = guestTeamId,// team == null ? 0 : team.Attributes["gtid"].Value.GetInt32(),
                            GuestTeamName = guestTeamName,
                            ShortGuestTeamName = match_D.a_cn_abbr,
                            LetBall = 0,
                            WinOdds = op.Length >= 1 ? op[0].GetDecimal() : 0M,
                            FlatOdds = op.Length >= 1 ? op[1].GetDecimal() : 0M,
                            LoseOdds = op.Length >= 1 ? op[2].GetDecimal() : 0M,
                            StartDateTime = DateTime.Parse(string.Format("{0} {1}",date, time)).ToString("yyyy-MM-dd HH:mm:ss"),
                            DSStopBettingTime = fsStopBettingTime.AddMinutes(-5).ToString("yyyy-MM-dd HH:mm:ss"),
                            FSStopBettingTime = fsStopBettingTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            Mid = zsMatch != null ? zsMatch.Mid : 0,
                            FXId = zsMatch != null ? zsMatch.FXId : 0,
                            //为了配置队伍对阵历史                   
                            Hi = match_D.h_id,
                            Gi = match_D.a_id,
                            PrivilegesType = matchConfig == null ? string.Empty : matchConfig.PrivilegesType,
                            State = state,
                            MatchStopDesc = "1",
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                this.WriteLog("解析array错误 " + ex.ToString());
            }
            return matchList;
        }

        public List<JCZQ_MatchInfo> GetMatchList_ZGJCW_ZJQ()
        {
            this.WriteLog("进入方法 GetMatchList_ZGJCW_ZJQ");
            var matchList = new List<JCZQ_MatchInfo>();
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long tt = (DateTime.Now.Ticks - startTime.Ticks) / 10000;
            var url = string.Format("http://i.sporttery.cn/odds_calculator/get_odds?i_format=json&i_callback=getData&poolcode%5B%5D=ttg&_={0}", tt);
            var encoding = Encoding.GetEncoding("gb2312");
            var content = PostManager.Get(url, encoding, 0, null);
            content = content.Replace("getData(", "").Replace(");", "").Trim();
            if (string.IsNullOrEmpty(content) || content == "404")
                return matchList;
            var array =JsonHelper.Decode(content);

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
                        string id = match_D.id;
                        object crs = match_D.crs;

                        string time = match_D.time;
                        string date = match_D.date;
                        string l_background_color = match_D.l_background_color;

                        string matchId =b_date.Substring(2).Replace("-", "") +num.Substring(2);
                        var color = string.Format("#{0}", l_background_color);
                        var matchData = matchId.Substring(0, 6);
                        var zsMatch = zsMathList.FirstOrDefault(p => p.MatchId == matchId);
                        var matchNumber = matchId.Substring(6);
                        var matchConfig = DisableMatchConfigList.FirstOrDefault(p => p.MatchId.Trim() == matchId);
                        var state = "0";
                        var matchStopDesc = string.Empty;
                        object ttg=match_D.ttg;
                        if (!(ttg == null))
                        {

                            int single = match_D.ttg.single;
                            if (single == 1)
                                state = "1";
                            var ms = _MatchStatus.Where(p => p.Key == matchId).FirstOrDefault();
                            if (ms.Key != null)
                            {
                                _MatchStatus.Remove(matchId);
                                _MatchStatus.Add(matchId, ms.Value + "5");
                            }
                            else
                            {
                                _MatchStatus.Add(matchId, "5");
                            }
                           // if (!string.IsNullOrEmpty(match_D.ttg.cbt))
                                matchStopDesc = match_D.ttg.cbt;
                        }
                        //var op = _OPList.FirstOrDefault(p => p.Key == match_D.id).Value.Split('|');
                        var op = string.IsNullOrEmpty(_OPList.FirstOrDefault(p => p.Key == id).Value) ? new string[] { } : _OPList.FirstOrDefault(p => p.Key ==id).Value.Split('|');
                        var fsStopBettingTime = DateTime.Now;
                        var dateTime = string.Format("{0}|{1} {2}",b_date,date, time).Split('|');
                        var hh = Convert.ToDateTime(dateTime[1]).Hour;
                        var hhm = string.Format("{0}.{1}", Convert.ToDateTime(dateTime[1]).Hour, Convert.ToDateTime(dateTime[1]).Minute);
                        var st = Convert.ToDateTime(dateTime[0]);
                        var et = Convert.ToDateTime(dateTime[1]);
                        TimeSpan ts = et - st;
                        if (ts.Days >= 2)
                        {
                            dateTime[0] = st.AddDays(1).ToString("yyyy-MM-dd");
                        }
                        var dateWeek = Convert.ToDateTime(dateTime[0]);
                        var week = CaculateWeekDay(dateWeek.Year, dateWeek.Month, dateWeek.Day);
                        var homeTeamName = match_D.h_cn;
                        var guestTeamName = match_D.a_cn;
                        string leagueName = match_D.l_cn;

                        var isTrue = false;
                        if (leagueName == "欧洲杯")
                        {
                            if (!_ozbList.Contains(b_date.Substring(2).Replace("-", "")))
                                _ozbList.Add(b_date.Substring(2).Replace("-", ""));
                        }
                        var isOzb = false;
                        if (_ozbList.Contains(b_date.Substring(2).Replace("-", "")))
                            isOzb = true;

                        if (leagueName == "世界杯")
                        {
                            if (!_sjbList.Contains(b_date.Substring(2).Replace("-", "")))
                                _sjbList.Add(b_date.Substring(2).Replace("-", ""));
                        }
                        var isSjb = false;
                        if (_sjbList.Contains(b_date.Substring(2).Replace("-", "")))
                            isSjb = true;

                        while (true)
                        {
                            if (isTrue)
                            {
                                fsStopBettingTime = fsStopBettingTime.AddMinutes(JCZQ_advanceMinutes);
                            }
                            else
                            {
                                if (hh >= 0 && decimal.Parse(hhm) < 9.30M)
                                    fsStopBettingTime = isOzb ?
                                        ((hh >= 3 && decimal.Parse(hhm) < 9.30M) ? Convert.ToDateTime(string.Format("{0} {1}", Convert.ToDateTime(dateTime[1]).ToString("yyyy-MM-dd"), "03:00:00")).AddMinutes(JCZQ_advanceMinutes) : Convert.ToDateTime(dateTime[1]).AddMinutes(JCZQ_advanceMinutes))
                                        : (week == "6" || week == "7") ?
                                        ((hh >= 1 && decimal.Parse(hhm) < 9.30M) ? Convert.ToDateTime(string.Format("{0} {1}", Convert.ToDateTime(dateTime[1]).ToString("yyyy-MM-dd"), "01:00:00")).AddMinutes(JCZQ_advanceMinutes) : Convert.ToDateTime(dateTime[1]).AddMinutes(JCZQ_advanceMinutes))
                                        : Convert.ToDateTime(string.Format("{0} {1}", dateTime[0], "23:59:00")).AddMinutes(JCZQ_advanceMinutes).AddMinutes(1);
                                else
                                    fsStopBettingTime = Convert.ToDateTime(dateTime[1]).AddMinutes(JCZQ_advanceMinutes);
                                isTrue = true;
                            }

                            if ((fsStopBettingTime.Hour >= (isOzb ? 3 : (week == "6" || week == "7") ? 1 : 0) && fsStopBettingTime.Minute >= 0)
                                && (fsStopBettingTime.Hour <= 9))
                            {
                                if (fsStopBettingTime.Minute >= 0 && fsStopBettingTime.Hour == 9)
                                    break;
                                hh = fsStopBettingTime.Hour;
                                hhm = string.Format("{0}.{1}", fsStopBettingTime.Hour, fsStopBettingTime.Minute.ToString("D2"));
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (isSjb)
                        {
                            fsStopBettingTime = Convert.ToDateTime(dateTime[1]).AddMinutes(JCZQ_advanceMinutes);
                        }
                        var leagueId = zsMatch != null ? zsMatch.LeagueId : 0;
                        var homeTeamId = zsMatch != null ? zsMatch.HomeTeamId : 0;
                        var guestTeamId = zsMatch != null ? zsMatch.GuestTeamId : 0;


                        matchList.Add(new JCZQ_MatchInfo
                        {
                            MatchId = matchId,
                            MatchData = matchData,
                            MatchNumber = matchNumber,
                            MatchIdName = match_D.num,
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            LeagueId = leagueId,  //int.Parse(item.li),
                            LeagueName = leagueName,
                            ShortLeagueName = match_D.l_cn_abbr,
                            LeagueColor = zsMatch != null ? zsMatch.LeagueColor : color,
                            HomeTeamId = homeTeamId,// team == null ? 0 : team.Attributes["htid"].Value.GetInt32(),
                            HomeTeamName = homeTeamName,
                            ShortHomeTeamName = match_D.h_cn_abbr,
                            GuestTeamId = guestTeamId,// team == null ? 0 : team.Attributes["gtid"].Value.GetInt32(),
                            GuestTeamName = guestTeamName,
                            ShortGuestTeamName = match_D.a_cn_abbr,
                            LetBall = 0,
                            WinOdds = op.Length >= 1 ? op[0].GetDecimal() : 0M,
                            FlatOdds = op.Length >= 1 ? op[1].GetDecimal() : 0M,
                            LoseOdds = op.Length >= 1 ? op[2].GetDecimal() : 0M,
                            StartDateTime = DateTime.Parse(string.Format("{0} {1}", match_D.date, match_D.time)).ToString("yyyy-MM-dd HH:mm:ss"),
                            DSStopBettingTime = fsStopBettingTime.AddMinutes(-5).ToString("yyyy-MM-dd HH:mm:ss"),
                            FSStopBettingTime = fsStopBettingTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            Mid = zsMatch != null ? zsMatch.Mid : 0,
                            FXId = zsMatch != null ? zsMatch.FXId : 0,
                            //为了配置队伍对阵历史                   
                            Hi = match_D.h_id,
                            Gi = match_D.a_id,
                            PrivilegesType = matchConfig == null ? string.Empty : matchConfig.PrivilegesType,
                            State = state,
                            MatchStopDesc = "1",
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                this.WriteLog("解析array错误 " + ex.ToString());
            }
            return matchList;
        }

        public List<JCZQ_MatchInfo> GetMatchList_ZGJCW_BQC()
        {
            this.WriteLog("进入方法 GetMatchList_ZGJCW_BQC");
            var matchList = new List<JCZQ_MatchInfo>();
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long tt = (DateTime.Now.Ticks - startTime.Ticks) / 10000;
            var url = string.Format("http://i.sporttery.cn/odds_calculator/get_odds?i_format=json&i_callback=getData&poolcode%5B%5D=hafu&_={0}", tt);
            var encoding = Encoding.GetEncoding("gb2312");
            var content = PostManager.Get(url, encoding, 0, null);
            content = content.Replace("getData(", "").Replace(");", "").Trim();
            if (string.IsNullOrEmpty(content) || content == "404")
                return matchList;
            var array =JsonHelper.Decode(content);

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
                      //  var match_D = match.Value;

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
                        string id = match_D.id;
                        object crs = match_D.crs;

                        string time = match_D.time;
                        string date = match_D.date;
                        string l_background_color = match_D.l_background_color;


                        string matchId =b_date.Substring(2).Replace("-", "") +num.Substring(2);
                        var color = string.Format("#{0}", l_background_color);
                        var matchData = matchId.Substring(0, 6);
                        var zsMatch = zsMathList.FirstOrDefault(p => p.MatchId == matchId);
                        var matchNumber = matchId.Substring(6);
                        var matchConfig = DisableMatchConfigList.FirstOrDefault(p => p.MatchId.Trim() == matchId);
                        var state = "0";
                        var matchStopDesc = string.Empty;
                        object hafu= match_D.hafu;
                        if (!(hafu == null))
                        {   int single= match_D.hafu.single;
                            if (single == 1)
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
                         //   if (!string.IsNullOrEmpty(match_D.hafu.cbt))
                                matchStopDesc = match_D.hafu.cbt;
                        }
                        //var op = _OPList.FirstOrDefault(p => p.Key == match_D.id).Value.Split('|');
                        var op = string.IsNullOrEmpty(_OPList.FirstOrDefault(p => p.Key == id).Value) ? new string[] { } : _OPList.FirstOrDefault(p => p.Key == id).Value.Split('|');
                        var fsStopBettingTime = DateTime.Now;
                        var dateTime = string.Format("{0}|{1} {2}", match_D.b_date, match_D.date, match_D.time).Split('|');
                        var hh = Convert.ToDateTime(dateTime[1]).Hour;
                        var hhm = string.Format("{0}.{1}", Convert.ToDateTime(dateTime[1]).Hour, Convert.ToDateTime(dateTime[1]).Minute);
                        var st = Convert.ToDateTime(dateTime[0]);
                        var et = Convert.ToDateTime(dateTime[1]);
                        TimeSpan ts = et - st;
                        if (ts.Days >= 2)
                        {
                            dateTime[0] = st.AddDays(1).ToString("yyyy-MM-dd");
                        }
                        var dateWeek = Convert.ToDateTime(dateTime[0]);
                        var week = CaculateWeekDay(dateWeek.Year, dateWeek.Month, dateWeek.Day);
                        string homeTeamName = match_D.h_cn;
                        string guestTeamName = match_D.a_cn;
                        string leagueName = match_D.l_cn;

                        var isTrue = false;
                        if (leagueName == "欧洲杯")
                        {
                            if (!_ozbList.Contains(b_date.Substring(2).Replace("-", "")))
                                _ozbList.Add(b_date.Substring(2).Replace("-", ""));
                        }
                        var isOzb = false;
                        if (_ozbList.Contains(b_date.Substring(2).Replace("-", "")))
                            isOzb = true;

                        if (leagueName == "世界杯")
                        {
                            if (!_sjbList.Contains(b_date.Substring(2).Replace("-", "")))
                                _sjbList.Add(b_date.Substring(2).Replace("-", ""));
                        }
                        var isSjb = false;
                        if (_sjbList.Contains(b_date.Substring(2).Replace("-", "")))
                            isSjb = true;

                        while (true)
                        {
                            if (isTrue)
                            {
                                fsStopBettingTime = fsStopBettingTime.AddMinutes(JCZQ_advanceMinutes);
                            }
                            else
                            {
                                if (hh >= 0 && decimal.Parse(hhm) < 9.30M)
                                    fsStopBettingTime = isOzb ?
                                        ((hh >= 3 && decimal.Parse(hhm) < 9.30M) ? Convert.ToDateTime(string.Format("{0} {1}", Convert.ToDateTime(dateTime[1]).ToString("yyyy-MM-dd"), "03:00:00")).AddMinutes(JCZQ_advanceMinutes) : Convert.ToDateTime(dateTime[1]).AddMinutes(JCZQ_advanceMinutes))
                                        : (week == "6" || week == "7") ?
                                        ((hh >= 1 && decimal.Parse(hhm) < 9.30M) ? Convert.ToDateTime(string.Format("{0} {1}", Convert.ToDateTime(dateTime[1]).ToString("yyyy-MM-dd"), "01:00:00")).AddMinutes(JCZQ_advanceMinutes) : Convert.ToDateTime(dateTime[1]).AddMinutes(JCZQ_advanceMinutes))
                                        : Convert.ToDateTime(string.Format("{0} {1}", dateTime[0], "23:59:00")).AddMinutes(JCZQ_advanceMinutes).AddMinutes(1);
                                else
                                    fsStopBettingTime = Convert.ToDateTime(dateTime[1]).AddMinutes(JCZQ_advanceMinutes);
                                isTrue = true;
                            }

                            if ((fsStopBettingTime.Hour >= (isOzb ? 3 : (week == "6" || week == "7") ? 1 : 0) && fsStopBettingTime.Minute >= 0)
                                && (fsStopBettingTime.Hour <= 9))
                            {
                                if (fsStopBettingTime.Minute >= 0 && fsStopBettingTime.Hour == 9)
                                    break;
                                hh = fsStopBettingTime.Hour;
                                hhm = string.Format("{0}.{1}", fsStopBettingTime.Hour, fsStopBettingTime.Minute.ToString("D2"));
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (isSjb)
                        {
                            fsStopBettingTime = Convert.ToDateTime(dateTime[1]).AddMinutes(JCZQ_advanceMinutes);
                        }

                        var leagueId = zsMatch != null ? zsMatch.LeagueId : 0;
                        var homeTeamId = zsMatch != null ? zsMatch.HomeTeamId : 0;
                        var guestTeamId = zsMatch != null ? zsMatch.GuestTeamId : 0;


                        matchList.Add(new JCZQ_MatchInfo
                        {
                            MatchId = matchId,
                            MatchData = matchData,
                            MatchNumber = matchNumber,
                            MatchIdName = match_D.num,
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            LeagueId = leagueId,  //int.Parse(item.li),
                            LeagueName = leagueName,
                            ShortLeagueName = match_D.l_cn_abbr,
                            LeagueColor = zsMatch != null ? zsMatch.LeagueColor : color,
                            HomeTeamId = homeTeamId,// team == null ? 0 : team.Attributes["htid"].Value.GetInt32(),
                            HomeTeamName = homeTeamName,
                            ShortHomeTeamName = match_D.h_cn_abbr,
                            GuestTeamId = guestTeamId,// team == null ? 0 : team.Attributes["gtid"].Value.GetInt32(),
                            GuestTeamName = guestTeamName,
                            ShortGuestTeamName = match_D.a_cn_abbr,
                            LetBall = 0,
                            WinOdds = op.Length >= 1 ? op[0].GetDecimal() : 0M,
                            FlatOdds = op.Length >= 1 ? op[1].GetDecimal() : 0M,
                            LoseOdds = op.Length >= 1 ? op[2].GetDecimal() : 0M,
                            StartDateTime = DateTime.Parse(string.Format("{0} {1}", match_D.date, match_D.time)).ToString("yyyy-MM-dd HH:mm:ss"),
                            DSStopBettingTime = fsStopBettingTime.AddMinutes(-5).ToString("yyyy-MM-dd HH:mm:ss"),
                            FSStopBettingTime = fsStopBettingTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            Mid = zsMatch != null ? zsMatch.Mid : 0,
                            FXId = zsMatch != null ? zsMatch.FXId : 0,
                            //为了配置队伍对阵历史                   
                            Hi = match_D.h_id,
                            Gi = match_D.a_id,
                            PrivilegesType = matchConfig == null ? string.Empty : matchConfig.PrivilegesType,
                            State = state,
                            MatchStopDesc = "1",
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                this.WriteLog("解析array错误 " + ex.ToString());
            }
            return matchList;
        }

        public List<JCZQ_MatchInfo> GetMatchList_okooo(out List<JCZQ_SPF_OZ_SPInfo> spOZ, out List<JCZQ_MatchInfo> tempMatchList_FB, out List<JCZQ_MatchInfo> tempMatchList_BF, out List<JCZQ_MatchInfo> tempMatchList_ZJQ, out List<JCZQ_MatchInfo> tempMatchList_BQC,
            out List<C_JCZQ_SPF_SP> spfList, out List<C_JCZQ_SPF_SP> brqspfList, out List<C_JCZQ_BF_SP> bfList, out List<C_JCZQ_ZJQ_SP> zjqList, out List<C_JCZQ_BQC_SP> bqcList)
        {
            _MatchStatus = new Dictionary<string, string>();
            this.WriteLog("进入方法 GetMatchList_okooo");
            var matchList = new List<JCZQ_MatchInfo>();
            spOZ = new List<JCZQ_SPF_OZ_SPInfo>();
            tempMatchList_FB = new List<JCZQ_MatchInfo>();
            tempMatchList_BF = new List<JCZQ_MatchInfo>();
            tempMatchList_ZJQ = new List<JCZQ_MatchInfo>();
            tempMatchList_BQC = new List<JCZQ_MatchInfo>();
            spfList = new List<C_JCZQ_SPF_SP>();
            brqspfList = new List<C_JCZQ_SPF_SP>();
            bfList = new List<C_JCZQ_BF_SP>();
            zjqList = new List<C_JCZQ_ZJQ_SP>();
            bqcList = new List<C_JCZQ_BQC_SP>();

            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long tt = (DateTime.Now.Ticks - startTime.Ticks) / 10000;
            //var url = string.Format("http://i.sporttery.cn/odds_calculator/get_odds?i_format=json&i_callback=getData&poolcode%5B%5D=hhad&poolcode%5B%5D=had&_={0}", tt);
            //var url = string.Format("http://www.okooo.com/jingcai/?={0}", tt);
            var url = "http://www.okooo.com/jingcai/?={0}";
            var encoding = Encoding.GetEncoding("gb2312");
            var content = PostManager.Get(url, encoding, 0, null);
            this.WriteLog("采集到数据：" + content == "404" ? content : "");
            if (content == "404")
                return matchList;
            //step 1 得到div内容
            var index = content.IndexOf("<div id=\"content\" class=\"box\">");
            content = content.Substring(index);
            //index = content.IndexOf("<div class=\"touzhu\">");
            //content = content.Substring(index);
            index = content.IndexOf("<div class=\"footer\" id=\"touzhulan\">");
            content = content.Substring(0, index).Replace("<div class=\"clear\"></div>", "");

            var zsMathList = GetZS_MatchList();

            this.WriteLog("开始解析数据：");
            var rows = content.Split(new string[] { "<div class=\"cont \" >" }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                foreach (var item in rows)
                {
                    var row = item.Trim();
                    if (row.Contains("<div class=\"cont endday\" style=\"display:none\">")) continue;

                    index = row.IndexOf("<span class=\"float_l\">") + "<span class=\"float_l\">".Length;
                    var b_date = row.Substring(index, 10);
                    index = row.IndexOf("<div class=\"touzhu\">");
                    row = row.Substring(index);

                    GetokoooAvgSp(b_date);

                    var divL = row.Split(new string[] { "<div class=\"touzhu_1\"" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var div in divL)
                    {
                        var cont = div.Trim();
                        if (!cont.Contains("hover=\"hover2\"")) continue;
                        var date = DateTime.Parse(b_date);
                        var star = cont.IndexOf("hover=\"xulie_h\" title=\"") + "hover=\"xulie_h\" title=\"".Length;
                        var match_D = cont.Substring(star, 5);
                        var T_date = DateTime.Parse(CaculateWeekDay_C(date.Year, date.Month, date.Day, match_D.Substring(0, 2))).ToString("yyyy-MM-dd");
                        string matchId = T_date.Substring(2).Replace("-", "") + match_D.Substring(2);

                        this.WriteLog("获取到比赛：" + matchId);
                        star = cont.IndexOf("data-mid=\"") + "data-mid=\"".Length;
                        var id = cont.Substring(star, cont.IndexOf("\" data-morder=\"") - star);
                        star = cont.IndexOf("data-morder=\"") + "data-morder=\"".Length;
                        var tid = cont.Substring(star, cont.IndexOf("\" data-ordercn=") - star);
                        star = cont.IndexOf("<a class=\"saiming aochao\"  style=\"background-color:") + "<a class=\"saiming aochao\"  style=\"background-color:".Length;
                        var color = cont.Substring(star, 7);

                        var matchData = matchId.Substring(0, 6);
                        var zsMatch = zsMathList.FirstOrDefault(p => p.MatchId == matchId);
                        var matchNumber = matchId.Substring(6);
                        var spf = cont.IndexOf("<div class=\"shenpf");
                        var rqspf = cont.IndexOf("<div class=\"rangqiuspf");
                        var state = "0";
                        var state_fb = "0";
                        var state_bf = "0";
                        var state_zjq = "0";
                        var state_bqc = "0";

                        var fsStopBettingTime = DateTime.Now;

                        #region 获取投注时间并处理

                        star = cont.IndexOf("title=\"比赛时间:") + "title=\"比赛时间:".Length;
                        var startDateTime = cont.Substring(star, 19);
                        var dateTime = string.Format("{0}|{1}", T_date, startDateTime).Split('|');
                        var hh = Convert.ToDateTime(dateTime[1]).Hour;
                        var hhm = string.Format("{0}.{1}", Convert.ToDateTime(dateTime[1]).Hour, Convert.ToDateTime(dateTime[1]).Minute);
                        var dateWeek = Convert.ToDateTime(dateTime[0]);
                        var week = CaculateWeekDay(dateWeek.Year, dateWeek.Month, dateWeek.Day);

                        var isTrue = false;
                        while (true)
                        {
                            if (isTrue)
                            {
                                if (hh >= 0 && decimal.Parse(hhm) <= 9.00M)
                                    fsStopBettingTime = (week == "6" || week == "7") ?
                                        ((hh >= 1 && decimal.Parse(hhm) <= 9.00M) ? fsStopBettingTime.AddMinutes(JCZQ_advanceMinutes) : fsStopBettingTime.AddMinutes(JCZQ_advanceMinutes))
                                        : fsStopBettingTime.AddMinutes(JCZQ_advanceMinutes);
                                else
                                    fsStopBettingTime = fsStopBettingTime.AddMinutes(JCZQ_advanceMinutes);
                            }
                            else
                            {
                                if (hh >= 0 && decimal.Parse(hhm) <= 9.00M)
                                    fsStopBettingTime = (week == "6" || week == "7") ?
                                        ((hh >= 1 && decimal.Parse(hhm) <= 9.00M) ? Convert.ToDateTime(string.Format("{0} {1}", Convert.ToDateTime(dateTime[1]).ToString("yyyy-MM-dd"), "01:00:00")).AddMinutes(JCZQ_advanceMinutes) : Convert.ToDateTime(dateTime[1]).AddMinutes(JCZQ_advanceMinutes))
                                        : Convert.ToDateTime(string.Format("{0} {1}", dateTime[0], "23:59:00")).AddMinutes(JCZQ_advanceMinutes);
                                else
                                    fsStopBettingTime = Convert.ToDateTime(dateTime[1]).AddMinutes(JCZQ_advanceMinutes);
                                isTrue = true;
                            }

                            if ((fsStopBettingTime.Hour >= ((week == "6" || week == "7") ? 1 : 0) && fsStopBettingTime.Minute >= 0)
                                && (fsStopBettingTime.Hour <= 9))
                            {
                                if (fsStopBettingTime.Minute >= 1)
                                    break;
                                hh = fsStopBettingTime.Hour;
                                hhm = string.Format("{0}.{1}", fsStopBettingTime.Hour, fsStopBettingTime.Minute);
                            }
                            else
                            {
                                break;
                            }

                        }
                        #endregion

                        #region 胜平负

                        var content_spf = cont.Substring(spf, rqspf - spf);


                        star = content_spf.IndexOf("<div class=\"zhum fff hui_colo");
                        var htn = content_spf.Substring(star);
                        star = htn.IndexOf(">") + 1;
                        htn = htn.Substring(star, htn.IndexOf("</div>") - star);
                        var homeTeamName = htn;
                        star = content_spf.LastIndexOf("<div class=\"zhum fff hui_colo");
                        var gtn = content_spf.Substring(star);
                        star = gtn.IndexOf(">") + 1;
                        gtn = gtn.Substring(star, gtn.IndexOf("</div>") - star);
                        var guestTeamName = gtn;

                        star = content_spf.IndexOf("<div class=\"peilv fff hui_colo red_colo\">") + "<div class=\"peilv fff hui_colo red_colo\">".Length;
                        var brqspf_sp = content_spf.Substring(star);
                        star = brqspf_sp.IndexOf("</div>");
                        var brqspf_sp_s = brqspf_sp.Substring(0, star).Replace("\r\n", "").Replace("\t", "").Trim();

                        star = brqspf_sp.IndexOf("<div class=\"peilv fff hui_colo red_colo\">") + "<div class=\"peilv fff hui_colo red_colo\">".Length;
                        brqspf_sp = brqspf_sp.Substring(star);
                        star = brqspf_sp.IndexOf("</div>");
                        var brqspf_sp_p = brqspf_sp.Substring(0, star).Replace("\r\n", "").Replace("\t", "").Trim();

                        star = brqspf_sp.IndexOf("<div class=\"peilv fff hui_colo red_colo\">") + "<div class=\"peilv fff hui_colo red_colo\">".Length;
                        brqspf_sp = brqspf_sp.Substring(star);
                        star = brqspf_sp.IndexOf("</div>");
                        var brqspf_sp_f = brqspf_sp.Substring(0, star).Replace("\r\n", "").Replace("\t", "").Trim();

                        brqspfList.Add(new C_JCZQ_SPF_SP
                        {
                            MatchId = matchId,
                            MatchData = matchData,
                            MatchNumber = matchNumber,
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            WinOdds = decimal.Parse(brqspf_sp_s),
                            FlatOdds = decimal.Parse(brqspf_sp_p),
                            LoseOdds = decimal.Parse(brqspf_sp_f)
                        });
                        var hRank = string.Empty;
                        var gRank = string.Empty;
                        var hLg = string.Empty;
                        var gLg = string.Empty;
                        star = content_spf.IndexOf("<div class=\"paim paim_sel\">") + "<div class=\"paim paim_sel\">".Length;
                        var rank = content_spf.Substring(star);
                        star = rank.IndexOf("</div>");
                        var rankAndLg = rank.Substring(0, star).Replace("<p class=\"p2\">", "").Replace("<p class=\"p1\">", "").Replace("<p >", "").Replace("</p>", "").Replace("\r\n", "").Replace("\t", "").Replace(" ", "").Trim();
                        if (!string.IsNullOrEmpty(rankAndLg))
                        {
                            if (rankAndLg.IndexOf("]") != -1)
                            {
                                var arry = rankAndLg.Split(']');
                                if (arry.Length == 2)
                                {
                                    hRank = string.Format("{0}]", arry[0]);
                                    hLg = arry[1];
                                }
                            }
                        }
                        star = rank.IndexOf("<div class=\"paim paim_sel\">") + "<div class=\"paim paim_sel\">".Length;
                        rank = rank.Substring(star);
                        star = rank.IndexOf("</div>");
                        rankAndLg = rank.Substring(0, star).Replace("<p class=\"p2\">", "").Replace("<p class=\"p1\">", "").Replace("<p >", "").Replace("</p>", "").Replace("\r\n", "").Replace("\t", "").Replace(" ", "").Trim();
                        if (!string.IsNullOrEmpty(rankAndLg))
                        {
                            if (rankAndLg.IndexOf("]") != -1)
                            {
                                var arry = rankAndLg.Split(']');
                                if (arry.Length == 2)
                                {
                                    gRank = string.Format("{0}]", arry[0]);
                                    gLg = arry[1];
                                }
                            }
                        }
                        #endregion

                        #region 让球胜平负

                        var content_rqspf = cont.Substring(rqspf);
                        if (content_rqspf.IndexOf("<div class=\"danguan\"") != -1)
                        {
                            state = "R";
                            state_fb += "R";
                            state_fb = state_fb.Replace("0", "");
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
                        if (content_spf.IndexOf("<div class=\"danguan\"") != -1)
                        {
                            state = "NR";
                            state_fb += "NR";
                            state_fb = state_fb.Replace("0", "");
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
                        star = content_rqspf.IndexOf("<span class=\"rangqiu\">") != -1 ? content_rqspf.IndexOf("<span class=\"rangqiu\">") + "<span class=\"rangqiu\">".Length
                            : content_rqspf.IndexOf("<span class=\"rangqiuzhen\">") + "<span class=\"rangqiuzhen\">".Length;
                        var rangqiu = content_rqspf.Substring(star);
                        var letBall = rangqiu.Substring(0, rangqiu.IndexOf("</span>")).Replace("\r", "").Replace("\n", "").Replace("\t", "").Trim();

                        star = content_rqspf.IndexOf("<div class=\"peilv fff hui_colo red_colo\">") + "<div class=\"peilv fff hui_colo red_colo\">".Length;
                        var spf_sp = content_rqspf.Substring(star);
                        star = spf_sp.IndexOf("</div>");
                        var spf_sp_s = spf_sp.Substring(0, star).Replace("\r\n", "").Replace("\t", "").Trim();

                        star = spf_sp.IndexOf("<div class=\"peilv fff hui_colo red_colo\">") + "<div class=\"peilv fff hui_colo red_colo\">".Length;
                        spf_sp = spf_sp.Substring(star);
                        star = spf_sp.IndexOf("</div>");
                        var spf_sp_p = spf_sp.Substring(0, star).Replace("\r\n", "").Replace("\t", "").Trim();

                        star = spf_sp.IndexOf("<div class=\"peilv fff hui_colo red_colo\">") + "<div class=\"peilv fff hui_colo red_colo\">".Length;
                        spf_sp = spf_sp.Substring(star);
                        star = spf_sp.IndexOf("</div>");
                        var spf_sp_f = spf_sp.Substring(0, star).Replace("\r\n", "").Replace("\t", "").Trim();

                        spfList.Add(new C_JCZQ_SPF_SP
                        {
                            MatchId = matchId,
                            MatchData = matchData,
                            MatchNumber = matchNumber,
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            WinOdds = decimal.Parse(spf_sp_s),
                            FlatOdds = decimal.Parse(spf_sp_p),
                            LoseOdds = decimal.Parse(spf_sp_f)
                        });

                        #endregion

                        var url_f = string.Format("http://www.okooo.com/jingcai/?action=more&LotteryNo={0}&MatchOrder={1}", T_date, tid);
                        var content_f = PostManager.Get(url_f, Encoding.GetEncoding("utf-8"), 0, null);

                        #region 比分

                        star = content_f.IndexOf("<div class=\"jnm\">");
                        var content_bf = content_f.Substring(0, star);
                        if (content_bf.Contains("可投单关"))
                        {
                            state_bf = "1";
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
                        star = content_bf.IndexOf("<div class=\"float_l\">");
                        content_bf = content_bf.Substring(star);
                        var bf_10 = 0M;
                        var bf_20 = 0M;
                        var bf_21 = 0M;
                        var bf_30 = 0M;
                        var bf_31 = 0M;
                        var bf_32 = 0M;
                        var bf_40 = 0M;
                        var bf_41 = 0M;
                        var bf_42 = 0M;
                        var bf_50 = 0M;
                        var bf_51 = 0M;
                        var bf_52 = 0M;
                        var bf_X0 = 0M;
                        var bf_00 = 0M;
                        var bf_11 = 0M;
                        var bf_22 = 0M;
                        var bf_33 = 0M;
                        var bf_XX = 0M;
                        var bf_01 = 0M;
                        var bf_02 = 0M;
                        var bf_12 = 0M;
                        var bf_03 = 0M;
                        var bf_13 = 0M;
                        var bf_23 = 0M;
                        var bf_04 = 0M;
                        var bf_14 = 0M;
                        var bf_24 = 0M;
                        var bf_05 = 0M;
                        var bf_15 = 0M;
                        var bf_25 = 0M;
                        var bf_0X = 0M;
                        var bf_sp_list = content_bf.Split(new string[] { "<div choose=\"" }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < bf_sp_list.Length; i++)
                        {
                            if (!bf_sp_list[i].Contains("hover=\"hover11\"")) continue;
                            star = bf_sp_list[i].IndexOf("<div class=\"peilv_1 fff hui_colo red_colo\">") != -1 ? bf_sp_list[i].IndexOf("<div class=\"peilv_1 fff hui_colo red_colo\">") :
                                bf_sp_list[i].IndexOf("<div class=\"peilv_1 p_1 fff hui_colo red_colo\">");
                            var str = bf_sp_list[i].Substring(star);
                            star = str.IndexOf(">") + 1;
                            str = str.Substring(star, str.IndexOf("</div>") - star);
                            switch (i)
                            {
                                case 1:
                                    bf_10 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 2:
                                    bf_20 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 3:
                                    bf_21 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 4:
                                    bf_30 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 5:
                                    bf_31 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 6:
                                    bf_32 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 7:
                                    bf_40 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 8:
                                    bf_41 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 9:
                                    bf_42 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 10:
                                    bf_50 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 11:
                                    bf_51 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 12:
                                    bf_52 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 13:
                                    bf_X0 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 14:
                                    bf_00 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 15:
                                    bf_11 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 16:
                                    bf_22 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 17:
                                    bf_33 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 18:
                                    bf_XX = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 19:
                                    bf_01 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 20:
                                    bf_02 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 21:
                                    bf_12 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 22:
                                    bf_03 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 23:
                                    bf_13 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 24:
                                    bf_23 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 25:
                                    bf_04 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 26:
                                    bf_14 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 27:
                                    bf_24 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 28:
                                    bf_05 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 29:
                                    bf_15 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 30:
                                    bf_25 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 31:
                                    bf_0X = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                default:
                                    break;
                            }
                        }
                        bfList.Add(new C_JCZQ_BF_SP
                        {
                            MatchId = matchId,
                            MatchData = matchData,
                            MatchNumber = matchNumber,
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            S_QT = bf_X0,
                            P_QT = bf_XX,
                            F_QT = bf_0X,
                            S_52 = bf_52,
                            S_51 = bf_51,
                            S_50 = bf_50,
                            S_42 = bf_42,
                            S_41 = bf_41,
                            S_40 = bf_40,
                            S_32 = bf_32,
                            S_31 = bf_31,
                            S_30 = bf_30,
                            S_21 = bf_21,
                            S_20 = bf_20,
                            S_10 = bf_10,
                            P_00 = bf_00,
                            P_11 = bf_11,
                            P_22 = bf_22,
                            P_33 = bf_33,
                            F_01 = bf_01,
                            F_02 = bf_02,
                            F_03 = bf_03,
                            F_04 = bf_04,
                            F_05 = bf_05,
                            F_12 = bf_12,
                            F_13 = bf_13,
                            F_14 = bf_14,
                            F_15 = bf_15,
                            F_23 = bf_23,
                            F_24 = bf_24,
                            F_25 = bf_25
                        });
                        #endregion

                        #region 总进球

                        star = content_f.IndexOf("<div class=\"jnm\">");
                        var end = content_f.LastIndexOf("<div class=\"jnm\">");
                        var content_zjq = content_f.Substring(star, end - star);
                        if (content_zjq.Contains("可投单关"))
                        {
                            state_zjq = "1";
                            var ms = _MatchStatus.Where(p => p.Key == matchId).FirstOrDefault();
                            if (ms.Key != null)
                            {
                                _MatchStatus.Remove(matchId);
                                _MatchStatus.Add(matchId, ms.Value + "5");
                            }
                            else
                            {
                                _MatchStatus.Add(matchId, "5");
                            }
                        }
                        star = content_zjq.IndexOf("<div class=\"zk_1 float_l\">");
                        content_zjq = content_zjq.Substring(star);

                        var zjq_0 = 0M;
                        var zjq_1 = 0M;
                        var zjq_2 = 0M;
                        var zjq_3 = 0M;
                        var zjq_4 = 0M;
                        var zjq_5 = 0M;
                        var zjq_6 = 0M;
                        var zjq_7 = 0M;

                        var zjq_sp_list = content_zjq.Split(new string[] { "<div choose=\"" }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < zjq_sp_list.Length; i++)
                        {
                            if (!zjq_sp_list[i].Contains("hover=\"hover11\"")) continue;
                            star = zjq_sp_list[i].IndexOf("<div class=\"peilv_1 fff hui_colo red_colo\">");
                            var str = zjq_sp_list[i].Substring(star);
                            star = str.IndexOf(">") + 1;
                            str = str.Substring(star, str.IndexOf("</div>") - star);
                            switch (i)
                            {
                                case 1:
                                    zjq_0 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 2:
                                    zjq_1 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 3:
                                    zjq_2 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 4:
                                    zjq_3 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 5:
                                    zjq_4 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 6:
                                    zjq_5 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 7:
                                    zjq_6 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 8:
                                    zjq_7 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                default:
                                    break;
                            }
                        }
                        zjqList.Add(new C_JCZQ_ZJQ_SP
                        {
                            MatchId = matchId,
                            MatchData = matchData,
                            MatchNumber = matchNumber,
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            JinQiu_0_Odds = zjq_0,
                            JinQiu_1_Odds = zjq_1,
                            JinQiu_2_Odds = zjq_2,
                            JinQiu_3_Odds = zjq_3,
                            JinQiu_4_Odds = zjq_4,
                            JinQiu_5_Odds = zjq_5,
                            JinQiu_6_Odds = zjq_6,
                            JinQiu_7_Odds = zjq_7,
                        });
                        #endregion

                        #region 半全场

                        star = content_f.LastIndexOf("<div class=\"jnm\">");
                        var content_bqc = content_f.Substring(star);
                        if (content_bqc.Contains("可投单关"))
                        {
                            state_bqc = "1";
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
                        star = content_bqc.IndexOf("<div class=\"zk_1 float_l\">");
                        content_bqc = content_bqc.Substring(star);

                        var bqc_33 = 0M;
                        var bqc_31 = 0M;
                        var bqc_30 = 0M;
                        var bqc_13 = 0M;
                        var bqc_11 = 0M;
                        var bqc_10 = 0M;
                        var bqc_03 = 0M;
                        var bqc_01 = 0M;
                        var bqc_00 = 0M;

                        var bqc_sp_list = content_bqc.Split(new string[] { "<div choose=\"" }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < bqc_sp_list.Length; i++)
                        {
                            if (!bqc_sp_list[i].Contains("hover=\"hover11\"")) continue;
                            star = bqc_sp_list[i].IndexOf("<div class=\"peilv_1 fff hui_colo red_colo\">");
                            var str = bqc_sp_list[i].Substring(star);
                            star = str.IndexOf(">") + 1;
                            str = str.Substring(star, str.IndexOf("</div>") - star);
                            switch (i)
                            {
                                case 1:
                                    bqc_33 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 2:
                                    bqc_31 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 3:
                                    bqc_30 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 4:
                                    bqc_13 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 5:
                                    bqc_11 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 6:
                                    bqc_10 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 7:
                                    bqc_03 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 8:
                                    bqc_01 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                case 9:
                                    bqc_00 = string.IsNullOrEmpty(str) ? 0 : decimal.Parse(str);
                                    break;
                                default:
                                    break;
                            }
                        }
                        bqcList.Add(new C_JCZQ_BQC_SP
                        {
                            MatchId = matchId,
                            MatchData = matchData,
                            MatchNumber = matchNumber,
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            SH_SH_Odds = bqc_33,
                            SH_P_Odds = bqc_31,
                            SH_F_Odds = bqc_30,
                            P_SH_Odds = bqc_13,
                            P_P_Odds = bqc_11,
                            P_F_Odds = bqc_10,
                            F_SH_Odds = bqc_03,
                            F_P_Odds = bqc_01,
                            F_F_Odds = bqc_00,
                        });

                        #endregion

                        star = cont.IndexOf("target=\"_blank\">") + "target=\"_blank\">".Length;
                        var leagueName = cont.Substring(star, cont.IndexOf("</a>") - star);
                        star = cont.IndexOf("data-hname=\"") + "data-hname=\"".Length;
                        var shortHomeTeamName = cont.Substring(star, cont.IndexOf("\" data-aname") - star);
                        star = cont.IndexOf("data-aname=\"") + "data-aname=\"".Length;
                        var shortGuestTeamName = cont.Substring(star, cont.IndexOf("\" data-rev=") - star);

                        var leagueId = zsMatch != null ? zsMatch.LeagueId : 0;
                        var homeTeamId = zsMatch != null ? zsMatch.HomeTeamId : 0;
                        var guestTeamId = zsMatch != null ? zsMatch.GuestTeamId : 0;

                        var op = _AvgSp.ContainsKey(id) ? _AvgSp.Where(p => p.Key == id).FirstOrDefault().Value.Split(',') : null;

                        #region 生成比赛数据
                        matchList.Add(new JCZQ_MatchInfo
                        {
                            MatchId = matchId,
                            MatchData = matchData,
                            MatchNumber = matchNumber,
                            MatchIdName = match_D,
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            LeagueId = leagueId,  //int.Parse(item.li),
                            LeagueName = leagueName,
                            ShortLeagueName = leagueName,
                            LeagueColor = zsMatch != null ? zsMatch.LeagueColor : color,
                            HomeTeamId = homeTeamId,// team == null ? 0 : team.Attributes["htid"].Value.GetInt32(),
                            HomeTeamName = homeTeamName,
                            ShortHomeTeamName = shortHomeTeamName,
                            GuestTeamId = guestTeamId,// team == null ? 0 : team.Attributes["gtid"].Value.GetInt32(),
                            GuestTeamName = guestTeamName,
                            ShortGuestTeamName = shortGuestTeamName,
                            LetBall = int.Parse(letBall),
                            WinOdds = op.Length >= 1 ? op[0].GetDecimal() : 0M,
                            FlatOdds = op.Length >= 1 ? op[1].GetDecimal() : 0M,
                            LoseOdds = op.Length >= 1 ? op[2].GetDecimal() : 0M,
                            StartDateTime = DateTime.Parse(startDateTime).ToString("yyyy-MM-dd HH:mm:ss"),
                            DSStopBettingTime = fsStopBettingTime.AddMinutes(-15).ToString("yyyy-MM-dd HH:mm:ss"),
                            FSStopBettingTime = fsStopBettingTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            Mid = zsMatch != null ? zsMatch.Mid : 0,
                            FXId = zsMatch != null ? zsMatch.FXId : 0,
                            //为了配置队伍对阵历史                   
                            Hi = "0",
                            Gi = "0",
                            PrivilegesType = string.Empty,
                            State = state,
                            MatchStopDesc = "1",
                            HRank = hRank,
                            GRank = gRank,
                            HLg = hLg,
                            GLg = gLg
                        });

                        tempMatchList_FB.Add(new JCZQ_MatchInfo
                        {
                            MatchId = matchId,
                            MatchData = matchData,
                            MatchNumber = matchNumber,
                            MatchIdName = match_D,
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            LeagueId = leagueId,  //int.Parse(item.li),
                            LeagueName = leagueName,
                            ShortLeagueName = leagueName,
                            LeagueColor = zsMatch != null ? zsMatch.LeagueColor : color,
                            HomeTeamId = homeTeamId,// team == null ? 0 : team.Attributes["htid"].Value.GetInt32(),
                            HomeTeamName = homeTeamName,
                            ShortHomeTeamName = shortHomeTeamName,
                            GuestTeamId = guestTeamId,// team == null ? 0 : team.Attributes["gtid"].Value.GetInt32(),
                            GuestTeamName = guestTeamName,
                            ShortGuestTeamName = shortGuestTeamName,
                            LetBall = int.Parse(letBall),
                            WinOdds = op.Length >= 1 ? op[0].GetDecimal() : 0M,
                            FlatOdds = op.Length >= 1 ? op[1].GetDecimal() : 0M,
                            LoseOdds = op.Length >= 1 ? op[2].GetDecimal() : 0M,
                            StartDateTime = DateTime.Parse(startDateTime).ToString("yyyy-MM-dd HH:mm:ss"),
                            DSStopBettingTime = fsStopBettingTime.AddMinutes(-15).ToString("yyyy-MM-dd HH:mm:ss"),
                            FSStopBettingTime = fsStopBettingTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            Mid = zsMatch != null ? zsMatch.Mid : 0,
                            FXId = zsMatch != null ? zsMatch.FXId : 0,
                            //为了配置队伍对阵历史                   
                            Hi = "0",
                            Gi = "0",
                            PrivilegesType = string.Empty,
                            State = state_fb,
                            MatchStopDesc = "1",
                            HRank = hRank,
                            GRank = gRank,
                            HLg = hLg,
                            GLg = gLg
                        });

                        tempMatchList_BF.Add(new JCZQ_MatchInfo
                        {
                            MatchId = matchId,
                            MatchData = matchData,
                            MatchNumber = matchNumber,
                            MatchIdName = match_D,
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            LeagueId = leagueId,  //int.Parse(item.li),
                            LeagueName = leagueName,
                            ShortLeagueName = leagueName,
                            LeagueColor = zsMatch != null ? zsMatch.LeagueColor : color,
                            HomeTeamId = homeTeamId,// team == null ? 0 : team.Attributes["htid"].Value.GetInt32(),
                            HomeTeamName = homeTeamName,
                            ShortHomeTeamName = shortHomeTeamName,
                            GuestTeamId = guestTeamId,// team == null ? 0 : team.Attributes["gtid"].Value.GetInt32(),
                            GuestTeamName = guestTeamName,
                            ShortGuestTeamName = shortGuestTeamName,
                            LetBall = int.Parse(letBall),
                            WinOdds = op.Length >= 1 ? op[0].GetDecimal() : 0M,
                            FlatOdds = op.Length >= 1 ? op[1].GetDecimal() : 0M,
                            LoseOdds = op.Length >= 1 ? op[2].GetDecimal() : 0M,
                            StartDateTime = DateTime.Parse(startDateTime).ToString("yyyy-MM-dd HH:mm:ss"),
                            DSStopBettingTime = fsStopBettingTime.AddMinutes(-15).ToString("yyyy-MM-dd HH:mm:ss"),
                            FSStopBettingTime = fsStopBettingTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            Mid = zsMatch != null ? zsMatch.Mid : 0,
                            FXId = zsMatch != null ? zsMatch.FXId : 0,
                            //为了配置队伍对阵历史                   
                            Hi = "0",
                            Gi = "0",
                            PrivilegesType = string.Empty,
                            State = state_bf,
                            MatchStopDesc = "1",
                            HRank = hRank,
                            GRank = gRank,
                            HLg = hLg,
                            GLg = gLg
                        });

                        tempMatchList_ZJQ.Add(new JCZQ_MatchInfo
                        {
                            MatchId = matchId,
                            MatchData = matchData,
                            MatchNumber = matchNumber,
                            MatchIdName = match_D,
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            LeagueId = leagueId,  //int.Parse(item.li),
                            LeagueName = leagueName,
                            ShortLeagueName = leagueName,
                            LeagueColor = zsMatch != null ? zsMatch.LeagueColor : color,
                            HomeTeamId = homeTeamId,// team == null ? 0 : team.Attributes["htid"].Value.GetInt32(),
                            HomeTeamName = homeTeamName,
                            ShortHomeTeamName = shortHomeTeamName,
                            GuestTeamId = guestTeamId,// team == null ? 0 : team.Attributes["gtid"].Value.GetInt32(),
                            GuestTeamName = guestTeamName,
                            ShortGuestTeamName = shortGuestTeamName,
                            LetBall = int.Parse(letBall),
                            WinOdds = op.Length >= 1 ? op[0].GetDecimal() : 0M,
                            FlatOdds = op.Length >= 1 ? op[1].GetDecimal() : 0M,
                            LoseOdds = op.Length >= 1 ? op[2].GetDecimal() : 0M,
                            StartDateTime = DateTime.Parse(startDateTime).ToString("yyyy-MM-dd HH:mm:ss"),
                            DSStopBettingTime = fsStopBettingTime.AddMinutes(-15).ToString("yyyy-MM-dd HH:mm:ss"),
                            FSStopBettingTime = fsStopBettingTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            Mid = zsMatch != null ? zsMatch.Mid : 0,
                            FXId = zsMatch != null ? zsMatch.FXId : 0,
                            //为了配置队伍对阵历史                   
                            Hi = "0",
                            Gi = "0",
                            PrivilegesType = string.Empty,
                            State = state_zjq,
                            MatchStopDesc = "1",
                            HRank = hRank,
                            GRank = gRank,
                            HLg = hLg,
                            GLg = gLg
                        });

                        tempMatchList_BQC.Add(new JCZQ_MatchInfo
                        {
                            MatchId = matchId,
                            MatchData = matchData,
                            MatchNumber = matchNumber,
                            MatchIdName = match_D,
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            LeagueId = leagueId,  //int.Parse(item.li),
                            LeagueName = leagueName,
                            ShortLeagueName = leagueName,
                            LeagueColor = zsMatch != null ? zsMatch.LeagueColor : color,
                            HomeTeamId = homeTeamId,// team == null ? 0 : team.Attributes["htid"].Value.GetInt32(),
                            HomeTeamName = homeTeamName,
                            ShortHomeTeamName = shortHomeTeamName,
                            GuestTeamId = guestTeamId,// team == null ? 0 : team.Attributes["gtid"].Value.GetInt32(),
                            GuestTeamName = guestTeamName,
                            ShortGuestTeamName = shortGuestTeamName,
                            LetBall = int.Parse(letBall),
                            WinOdds = op.Length >= 1 ? op[0].GetDecimal() : 0M,
                            FlatOdds = op.Length >= 1 ? op[1].GetDecimal() : 0M,
                            LoseOdds = op.Length >= 1 ? op[2].GetDecimal() : 0M,
                            StartDateTime = DateTime.Parse(startDateTime).ToString("yyyy-MM-dd HH:mm:ss"),
                            DSStopBettingTime = fsStopBettingTime.AddMinutes(-15).ToString("yyyy-MM-dd HH:mm:ss"),
                            FSStopBettingTime = fsStopBettingTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            Mid = zsMatch != null ? zsMatch.Mid : 0,
                            FXId = zsMatch != null ? zsMatch.FXId : 0,
                            //为了配置队伍对阵历史                   
                            Hi = "0",
                            Gi = "0",
                            PrivilegesType = string.Empty,
                            State = state_bqc,
                            MatchStopDesc = "1",
                            HRank = hRank,
                            GRank = gRank,
                            HLg = hLg,
                            GLg = gLg
                        });

                        #endregion
                        spOZ.Add(new JCZQ_SPF_OZ_SPInfo
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            MatchId = matchId,
                            MatchData = matchData,
                            MatchNumber = matchNumber,
                            WinOdds = op.Length >= 1 ? op[0].GetDecimal() : 0M,
                            FlatOdds = op.Length >= 1 ? op[1].GetDecimal() : 0M,
                            LoseOdds = op.Length >= 1 ? op[2].GetDecimal() : 0M,
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                this.WriteLog("解析数据错误 " + ex.ToString());
            }
            return matchList;
        }


        public void GetMatchList_okooo_HG()
        {
            if (_Match_Ok_HG.Count > 200)
                _Match_Ok_HG = new Dictionary<string, string>();
            this.WriteLog("进入方法 GetMatchList_okooo");
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long tt = (DateTime.Now.Ticks - startTime.Ticks) / 10000;
            var url = "http://www.okooo.com/jingcai/";
            var encoding = Encoding.GetEncoding("gb2312");
            var content = PostManager.Get(url, encoding, 0, null);
            this.WriteLog("采集到数据：" + content == "404" ? content : "");
            if (content == "404" || string.IsNullOrEmpty(content))
                return;
            //step 1 得到div内容
            var index = content.IndexOf("<div id=\"content\" class=\"box\">");
            content = content.Substring(index);
            index = content.IndexOf("<div class=\"footer\" id=\"touzhulan\">");
            content = content.Substring(0, index).Replace("<div class=\"clear\"></div>", "");

            this.WriteLog("开始解析数据：");
            var rows = content.Split(new string[] { "<div class=\"cont \" >" }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                foreach (var item in rows)
                {
                    var row = item.Trim();
                    if (row.Contains("<div class=\"cont endday\" style=\"display:none\">")) continue;
                    if (row.Contains("<div class=\"head\">")) continue;//表头里面没有队伍数据

                    index = row.IndexOf("<span class=\"float_l\">") + "<span class=\"float_l\">".Length;
                    var b_date = row.Substring(index, 10);
                    index = row.IndexOf("<div class=\"touzhu\">");
                    row = row.Substring(index);

                    var divL = row.Split(new string[] { "<div class=\"touzhu_1\"" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var div in divL)
                    {
                        var cont = div.Trim();
                        if (!cont.Contains("hover=\"hover2\"")) continue;
                        var date = DateTime.Parse(b_date);
                        var star = cont.IndexOf("hover=\"xulie_h\" title=\"") + "hover=\"xulie_h\" title=\"".Length;
                        var match_D = cont.Substring(star, 5);
                        var T_date = DateTime.Parse(CaculateWeekDay_C(date.Year, date.Month, date.Day, match_D.Substring(0, 2))).ToString("yyyy-MM-dd");
                        string matchId = T_date.Substring(2).Replace("-", "") + match_D.Substring(2);

                        star = cont.IndexOf("data-mid=\"") + "data-mid=\"".Length;
                        var mid = cont.Substring(star, cont.IndexOf("\" data-morder=\"") - star);

                        star = cont.IndexOf("href=\"/soccer/league/") + "href=\"/soccer/league/".Length;
                        var leagueId = cont.Substring(star);
                        leagueId = leagueId.Substring(0, leagueId.IndexOf("/"));

                        this.WriteLog("获取到比赛：" + matchId);
                        var matchData = matchId.Substring(0, 6);
                        var matchNumber = matchId.Substring(6);
                        var spf = cont.IndexOf("<div class=\"shenpf");
                        var rqspf = cont.IndexOf("<div class=\"rangqiuspf");

                        if (spf == -1 || rqspf == -1) continue;
                        #region 胜平负

                        var content_spf = cont.Substring(spf, rqspf - spf);
                        var hRank = string.Empty;
                        var gRank = string.Empty;
                        var hLg = string.Empty;
                        var gLg = string.Empty;
                        star = content_spf.IndexOf("<div class=\"paim paim_sel\">") + "<div class=\"paim paim_sel\">".Length;
                        var rank = content_spf.Substring(star);
                        star = rank.IndexOf("</div>");
                        var rankAndLg = rank.Substring(0, star).Replace("<p class=\"p2\">", "").Replace("<p class=\"p1\">", "").Replace("<p >", "").Replace("</p>", "").Replace("\r\n", "").Replace("\t", "").Replace(" ", "").Trim();
                        if (!string.IsNullOrEmpty(rankAndLg))
                        {
                            if (rankAndLg.IndexOf("]") != -1)
                            {
                                var arry = rankAndLg.Split(']');
                                if (arry.Length == 2)
                                {
                                    hRank = string.Format("{0}]", arry[0]);
                                    hLg = arry[1];
                                }
                            }
                        }
                        star = rank.IndexOf("<div class=\"paim paim_sel\">") + "<div class=\"paim paim_sel\">".Length;
                        rank = rank.Substring(star);
                        star = rank.IndexOf("</div>");
                        rankAndLg = rank.Substring(0, star).Replace("<p class=\"p2\">", "").Replace("<p class=\"p1\">", "").Replace("<p >", "").Replace("</p>", "").Replace("\r\n", "").Replace("\t", "").Replace(" ", "").Trim();
                        if (!string.IsNullOrEmpty(rankAndLg))
                        {
                            if (rankAndLg.IndexOf("]") != -1)
                            {
                                var arry = rankAndLg.Split(']');
                                if (arry.Length == 2)
                                {
                                    gRank = string.Format("{0}]", arry[0]);
                                    gLg = arry[1];
                                }
                            }
                        }
                        #endregion

                        if (_Match_Ok_HG.Keys.Contains(matchId))
                        {
                            _Match_Ok_HG.Remove(matchId);
                            _Match_Ok_HG.Add(matchId, string.Format("{0}^{1}^{2}^{3}^{4}^{5}", hRank, gRank, hLg, gLg, mid, leagueId));
                        }
                        else
                        {
                            _Match_Ok_HG.Add(matchId, string.Format("{0}^{1}^{2}^{3}^{4}^{5}", hRank, gRank, hLg, gLg, mid, leagueId));
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                this.WriteLog("解析数据错误 " + ex.ToString());
            }
        }

        private Dictionary<string, string> _AvgSp = new Dictionary<string, string>();

        public void GetokoooAvgSp(string date)
        {
            _AvgSp = new Dictionary<string, string>();
            var time = DateTime.Parse(string.Format("{0} 00:00:00", date));
            var url = string.Format("http://www.okooo.com/I/?method=lottery.match.custom&typeid=1&pid=24&lotteryType=SportterySoccerMix&lotteryNo={0}%2C{1}%2C{2}&typeId=1&format=json", time.AddDays(-1).ToString("yyyy-MM-dd"), time.ToString("yyyy-MM-dd"), time.AddDays(1).ToString("yyyy-MM-dd"));
            var encoding = Encoding.GetEncoding("gb2312");
            var content = PostManager.Get(url, encoding, 0, null);

            content = content.Replace("getData(", "").Replace(");", "").Trim();
            var array =JsonHelper.Decode(content);
            try
            {
                foreach (var item in array)
                {
                    var matchlist = item.Value;
                    foreach (var match in matchlist)
                    {
                        var id = match.Key;
                        var vlues = match.Value;
                        var h = vlues.home;
                        var d = vlues.draw;
                        var a = vlues.away;
                        if (_AvgSp.ContainsKey(id))
                        {
                            _AvgSp.Remove(id);
                            _AvgSp.Add(id, string.Format("{0},{1},{2}", h, d, a));
                        }
                        else
                        {
                            _AvgSp.Add(id, string.Format("{0},{1},{2}", h, d, a));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.WriteLog("解析array错误 " + ex.ToString());
            }
        }


        private List<JCZQ_MatchInfo> GetMatchList_New(out List<JCZQ_SPF_OZ_SPInfo> spOZ)
        {
            this.WriteLog("进入方法 GetMatchList_New");
            var matchList = new List<JCZQ_MatchInfo>();
            spOZ = new List<JCZQ_SPF_OZ_SPInfo>();

            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long tt = (DateTime.Now.Ticks - startTime.Ticks) / 10000;
            var url = string.Format("http://intf.cpdyj.com/data/jc/match.js?callback=dyj_jc_match&_={0}", tt);
            var encoding = Encoding.GetEncoding("gb2312");
            var content = PostManager.Get(url, encoding, 0, (request) =>
            {
                request.Host = "intf.cpdyj.com";
                request.Referer = "http://jc.cpdyj.com/";
                if (ServiceHelper.IsUseProxy("JCZQ"))
                {
                    var proxy = ServiceHelper.GetProxyUrl();
                    if (!string.IsNullOrEmpty(proxy))
                    {
                        request.Proxy = new System.Net.WebProxy(proxy);
                    }
                }
            });
            content = content.Replace("dyj_jc_match(", "").Replace(");", "").Trim();
            if (string.IsNullOrEmpty(content) || content == "404")
                return matchList;

            var array =JsonHelper.Decode(content);
            //var fxDic = ServiceHelper.GetFXIdSource() == "okooo" ? GetJCZQ_FX_OKOOO() : GetJCZQ_FX();
            this.WriteLog("调用GetJCZQ_FX完成");

            var zsMathList = GetZS_MatchList();
            GetMatchList_ZGJCW();
            try
            {
                this.WriteLog("开始解析队伍 " + array.Length);
                foreach (var item in array)
                {
                    string matchId = item.id;

                    var zsMatch = zsMathList.FirstOrDefault(p => p.MatchId == matchId);

                    //var team = lcRoot == null ? null : lcRoot.SelectSingleNode(string.Format("//row[@xid='{0}']", matchId));

                    try
                    {
                        #region 解析一条数据
                        var color = item.cl;
                        var matchData = matchId.Substring(0, 6);
                        var matchNumber = matchId.Substring(6);
                        string state = item.st;
                        //确认单关是胜平负还是不让球胜平负。
                        var matchId_ROrNR = _MatchIdList.FirstOrDefault(p => p.Key == matchId).Value;
                        if (!string.IsNullOrEmpty(matchId_ROrNR))
                            state = string.Format("{0}-{1}", state, matchId_ROrNR);

                        //确定停售时间（与500万同步）
                        var fsStopBettingTime = DateTime.Now;
                        var dateTime = _StopTimeList.FirstOrDefault(p => p.Key == matchId).Value.Split('|');
                        var hh = Convert.ToDateTime(dateTime[1]).Hour;
                        var dateWeek = Convert.ToDateTime(dateTime[0]);
                        var week = CaculateWeekDay(dateWeek.Year, dateWeek.Month, dateWeek.Day);
                        var strL = new string[] { "6", "7" };
                        if (hh >= 0 && hh < 9)
                            fsStopBettingTime = strL.Contains(week) ? Convert.ToDateTime(string.Format("{0} {1}", Convert.ToDateTime(dateTime[1]).ToString("yyyy-MM-dd"), "00:50:00")) : Convert.ToDateTime(string.Format("{0} {1}", dateTime[0], "23:50:00"));
                        else
                            fsStopBettingTime = Convert.ToDateTime(dateTime[1]).AddMinutes(-9);


                        string vs = item.vs;
                        var vsArray = vs.Split(',');
                        string vss = item.vss;
                        var vssArray = vss.Split(',');
                        string oz = item.oz;
                        var ozArray = string.IsNullOrEmpty(oz) ? new string[] { "0", "0", "0" } : oz.Split(',');
                        var homeTeamName = vsArray[1];
                        var guestTeamName = vsArray[2];
                        var leagueName = vsArray[0];
                        //var homeTeam = manager.QueryTeamEntity(homeTeamName);
                        //var guestTeam = manager.QueryTeamEntity(guestTeamName);
                        //var league = manager.QueryLeagueEntity(leagueName);

                        var leagueId = zsMatch != null ? zsMatch.LeagueId : 0;
                        var homeTeamId = zsMatch != null ? zsMatch.HomeTeamId : 0;
                        var guestTeamId = zsMatch != null ? zsMatch.GuestTeamId : 0;

                        matchList.Add(new JCZQ_MatchInfo
                        {
                            MatchId = matchId,
                            MatchData = matchData,
                            MatchNumber = matchNumber,
                            MatchIdName = item.zi,
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            LeagueId = leagueId,  //int.Parse(item.li),
                            LeagueName = leagueName,
                            ShortLeagueName = vssArray[0],
                            LeagueColor = zsMatch != null ? zsMatch.LeagueColor : color,
                            HomeTeamId = homeTeamId,// team == null ? 0 : team.Attributes["htid"].Value.GetInt32(),
                            HomeTeamName = vsArray[1],
                            ShortHomeTeamName = vssArray[1],
                            GuestTeamId = guestTeamId,// team == null ? 0 : team.Attributes["gtid"].Value.GetInt32(),
                            GuestTeamName = vsArray[2],
                            ShortGuestTeamName = vssArray[2],
                            LetBall = int.Parse(item.lb),
                            WinOdds = ozArray[0].GetDecimal(),
                            FlatOdds = ozArray[1].GetDecimal(),
                            LoseOdds = ozArray[2].GetDecimal(),
                            StartDateTime = DateTime.Parse(item.mt).ToString("yyyy-MM-dd HH:mm:ss"),
                            DSStopBettingTime = DateTime.Parse(item.de).ToString("yyyy-MM-dd HH:mm:ss"),
                            FSStopBettingTime = fsStopBettingTime.ToString("yyyy-MM-dd HH:mm:ss"),// DateTime.Parse(item.fe).ToString("yyyy-MM-dd HH:mm:ss"),
                            Mid = zsMatch != null ? zsMatch.Mid : 0,
                            FXId = zsMatch != null ? zsMatch.FXId : 0,
                            //为了配置队伍对阵历史                   
                            Hi = item.hi,
                            Gi = item.gi,
                            PrivilegesType = string.Empty,
                            State = state,
                        });

                        spOZ.Add(new JCZQ_SPF_OZ_SPInfo
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            MatchId = matchId,
                            MatchData = matchData,
                            MatchNumber = matchNumber,
                            WinOdds = ozArray[0].GetDecimal(),
                            FlatOdds = ozArray[1].GetDecimal(),
                            LoseOdds = ozArray[2].GetDecimal(),
                        });

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(string.Format("采集队伍 ：{0}  出错 ：{1}", matchId, ex.ToString()));
                    }
                }

            }
            catch (Exception ex)
            {
                this.WriteLog("解析array错误 " + ex.ToString());
            }
            return matchList;
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
            {
                if (d == 0)
                    return DateTime.Parse(string.Format("{0}-{1}-{2}", y, m, d + 1)).AddDays(-1).ToString("yyyy-MM-dd");
                else
                    return string.Format("{0}-{1}-{2}", y, m, d);
            }
            else
                return CaculateWeekDay_C(y, m, d - 1, week_c);
        }

        private void GetMatchList_ZGJCW()
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long tt = (DateTime.Now.Ticks - startTime.Ticks) / 10000;
            var url = string.Format("http://i.sporttery.cn/odds_calculator/get_odds?i_format=json&i_callback=getData&poolcode[]=hhad&poolcode[]=had&_={0}", tt);
            var encoding = Encoding.GetEncoding("gb2312");
            var content = PostManager.Get(url, encoding, 0, null);
            content = content.Replace("getData(", "").Replace(");", "").Trim();
            if (string.IsNullOrEmpty(content) || content == "404")
                return;
            _MatchIdList = new Dictionary<string, string>();
            _StopTimeList = new Dictionary<string, string>();
            var array =JsonHelper.Decode(content);
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
                        string id = b_date.Substring(2).Replace("-", "") +num.Substring(2);
                        _StopTimeList.Add(id, string.Format("{0}|{1} {2}", match_D.b_date, match_D.date, match_D.time));
                        if (!(match_D.hhad == null))
                        {
                            if (int.Parse(match_D.hhad.single) == 1)
                            {
                                if (!_MatchIdList.Keys.Contains(id))
                                {
                                    _MatchIdList.Add(id, "R");
                                    continue;
                                }
                            }
                        }
                        if (!(match_D.had == null))
                        {
                            if (int.Parse(match_D.had.single) == 1)
                            {
                                if (!_MatchIdList.Keys.Contains(id))
                                {
                                    _MatchIdList.Add(id, "NR");
                                    continue;
                                }
                            }
                        }
                        if (!_MatchIdList.Keys.Contains(id))
                            _MatchIdList.Add(id, "");
                    }
                }

            }
            catch (Exception ex)
            {
                this.WriteLog("解析array错误 " + ex.ToString());
            }
        }

        private void GetMatchList_OP_ZGJCW()
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long tt = (DateTime.Now.Ticks - startTime.Ticks) / 10000;
            var url = string.Format("http://i.sporttery.cn/odds_calculator/get_bookmarker_odds?i_format=json&sportscode=FB&i_callback=getReferData&_={0}", tt);
            var encoding = Encoding.GetEncoding("gb2312");
            var content = PostManager.Get(url, encoding, 0, null);
            content = content.Replace("getReferData(", "").Replace(");", "").Trim();
            if (string.IsNullOrEmpty(content) || content == "404")
                return;
            _OPList = new Dictionary<string, string>();
            var array =JsonHelper.Decode(content);
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
                        string sportteryid= match_D.sportteryid;
                        _OPList.Add(sportteryid, string.Format("{0}|{1}|{2}", match_D.o1, match_D.o2
                           , match_D.o3));
                    }
                }
            }
            catch (Exception ex)
            {
                this.WriteLog("解析array错误 " + ex.ToString());
            }
        }


        private List<JCZQ_MatchInfo> GetMatchList()
        {
            //备用 http://www.9188.com/data/jincai/jc_spf.xml
            var url = "http://trade.cpdyj.com/staticdata/lotteryinfo/odds/jc/jc.xml";
            var xmlContent = PostManager.Get(url, Encoding.GetEncoding("gb2312"), 0, (request) =>
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
            //var xmlContent = PostManager.Get(url, Encoding.UTF8);
            var doc = new XmlDocument();
            doc.LoadXml(xmlContent);
            var root = doc.SelectSingleNode("xml");
            if (root == null)
                throw new Exception("从xml中查询节点错误  - " + xmlContent);

            //XmlNode lcRoot = null;
            //if (ServiceHelper.Get_JCZQ_TeamInfo())
            //{
            //    var lcUrl = "http://odds.iiioo.com/sdata/getzcxml.php?lotyid=6";
            //    var lcDoc = new XmlDocument();
            //    lcDoc.LoadXml(PostManager.Get(lcUrl, Encoding.GetEncoding("gb2312")));
            //    lcRoot = lcDoc.SelectSingleNode("xml");
            //}
            var fxDic = ServiceHelper.GetFXIdSource() == "okooo" ? GetJCZQ_FX_OKOOO() : GetJCZQ_FX();
            var list = new List<JCZQ_MatchInfo>();
            foreach (XmlNode item in root.ChildNodes)
            {
                var matchId = item.Attributes["xid"].Value;
                var date = matchId.Substring(0, 6);
                var number = matchId.Replace(date, "");
                var dateTime = new DateTime(int.Parse("20" + date.Substring(0, 2)), int.Parse(date.Substring(2, 2)), int.Parse(date.Substring(4)));
                var matchIdName = string.Empty;
                switch (dateTime.DayOfWeek)
                {
                    case DayOfWeek.Friday:
                        matchIdName = "周五" + number;
                        break;
                    case DayOfWeek.Monday:
                        matchIdName = "周一" + number;
                        break;
                    case DayOfWeek.Saturday:
                        matchIdName = "周六" + number;
                        break;
                    case DayOfWeek.Sunday:
                        matchIdName = "周日" + number;
                        break;
                    case DayOfWeek.Thursday:
                        matchIdName = "周四" + number;
                        break;
                    case DayOfWeek.Tuesday:
                        matchIdName = "周二" + number;
                        break;
                    case DayOfWeek.Wednesday:
                        matchIdName = "周三" + number;
                        break;
                    default:
                        break;
                }

                var homeTeamName = item.Attributes["hn"].Value;
                var guestTeamName = item.Attributes["gn"].Value;
                var leagueName = item.Attributes["ln"].Value;
                //var homeTeam = manager.QueryTeamEntity(homeTeamName);
                //var guestTeam = manager.QueryTeamEntity(guestTeamName);
                //var league = manager.QueryLeagueEntity(leagueName);

                //var team = lcRoot == null ? null : lcRoot.SelectSingleNode(string.Format("//row[@xid='{0}']", matchId));
                var fxId = fxDic.ContainsKey(matchId) ? int.Parse(string.IsNullOrEmpty(fxDic[matchId]) ? "0" : fxDic[matchId]) : 0;
                list.Add(new JCZQ_MatchInfo
                {
                    MatchId = matchId,
                    MatchData = date,
                    MatchNumber = number,
                    MatchIdName = matchIdName,
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    LeagueId = 0,//league == null ? 0 : league.sclassID,  //item.Attributes["lid"].Value.GetInt32(),
                    LeagueName = leagueName,
                    LeagueColor = item.Attributes["cl"].Value,
                    HomeTeamId = 0,// homeTeam == null ? 0 : int.Parse(homeTeam.TeamID),// team == null ? item.Attributes["htid"].Value.GetInt32() : team.Attributes["htid"].Value.GetInt32(),
                    HomeTeamName = homeTeamName,
                    GuestTeamId = 0,//guestTeam == null ? 0 : int.Parse(guestTeam.TeamID),// team == null ? item.Attributes["gtid"].Value.GetInt32() : team.Attributes["gtid"].Value.GetInt32(),
                    GuestTeamName = guestTeamName,
                    LetBall = item.Attributes["rq"].Value.GetInt32(),
                    WinOdds = item.Attributes["oh"].Value.GetDecimal(),
                    FlatOdds = item.Attributes["od"].Value.GetDecimal(),
                    LoseOdds = item.Attributes["oa"].Value.GetDecimal(),
                    StartDateTime = item.Attributes["mtime"].Value.GetDateTime().ToString("yyyy-MM-dd HH:mm:ss"),
                    DSStopBettingTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    FSStopBettingTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Mid = 0,// ServiceHelper.GetLeagueCategory(league),//team == null ? 0 : team.Attributes["oddsmid"].Value.GetInt32(),
                    FXId = fxId,

                });
            }
            return list;
        }

        private List<JCZQ_MatchInfo> AddZHMPrivilegesType(List<JCZQ_MatchInfo> matchList)
        {
            try
            {
                var code = "016";
                var xml = string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?><msg><head transcode=\"{0}\" partnerid=\"{1}\" version=\"1.0\" time=\"{2}\"></head><body>", code, zhm_PartnerId, DateTime.Now.ToString("yyyyMMddHHmmss"));
                xml += string.Format("<querySchedule type=\"{0}\" />", "jczq");
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

                //spfMatch = new List<JCZQ_MatchInfo>();
                //brqspfMatch = new List<JCZQ_MatchInfo>();

                var doc = new XmlDocument();
                doc.LoadXml(resultXml);
                var root = doc.SelectSingleNode("//body/jcgames");
                if (root == null)
                    return matchList;

                var list = new List<JCZQ_MatchInfo>();
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

        public List<JCZQ_MatchInfo> GetMatchList_ZHM()
        {
            var code = "016";
            var xml = string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?><msg><head transcode=\"{0}\" partnerid=\"{1}\" version=\"1.0\" time=\"{2}\"></head><body>", code, zhm_PartnerId, DateTime.Now.ToString("yyyyMMddHHmmss"));
            xml += string.Format("<querySchedule type=\"{0}\" />", "jczq");
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

            //spfMatch = new List<JCZQ_MatchInfo>();
            //brqspfMatch = new List<JCZQ_MatchInfo>();

            var doc = new XmlDocument();
            doc.LoadXml(resultXml);
            var root = doc.SelectSingleNode("//body/jcgames");
            if (root == null)
                throw new Exception("从xml中查询节点错误  - " + resultXml);

            //var spf_SP_Dic = GetSP_ZHM("JCSPF");
            //var brqspf_SP_Dic = GetSP_ZHM("JCBRQSPF");
            var tempList = GetMatchList();
            var list = new List<JCZQ_MatchInfo>();
            foreach (XmlNode item in root.ChildNodes)
            {
                var name = item.Attributes["name"].Value;
                var matchId = item.Attributes["matchID"].Value;
                var hometeam = item.Attributes["hometeam"].Value;
                var guestteam = item.Attributes["guestteam"].Value;
                var matchstate = item.Attributes["matchstate"].Value;
                var matchtime = item.Attributes["matchtime"].Value;
                var sellouttime = item.Attributes["sellouttime"].Value;
                var letBall = item.Attributes["polygoal"].Value;

                var zhou = matchId.Substring(0, 2);
                var matchNumber = matchId.Substring(2);
                var matchDate = GetMatchDate(zhou, true);
                var endTime = DateTime.Parse(sellouttime);

                var temp = tempList.FirstOrDefault(p => p.MatchIdName == matchId);
                if (temp == null) continue;

                temp.FSStopBettingTime = endTime.ToString("yyyy-MM-dd HH:mm:ss");
                temp.DSStopBettingTime = endTime.AddMinutes(-15).ToString("yyyy-MM-dd HH:mm:ss");
                temp.LetBall = letBall.GetInt32();
                list.Add(temp);



                //var match = new JCZQ_MatchInfo
                //{
                //    CreateTime = DateTime.Now,
                //    DSStopBettingTime = endTime.AddMinutes(-15),
                //    FSStopBettingTime = endTime,
                //    HomeTeamName = hometeam,
                //    GuestTeamName = guestteam,
                //    MatchData = matchDate,
                //    MatchNumber = matchNumber,
                //    //MatchId = temp,
                //    MatchIdName = matchId,
                //    StartDateTime = DateTime.Parse(matchtime),
                //    LetBall = int.Parse(letBall),
                //    LeagueId =  temp.LeagueId,
                //    LeagueColor = temp.LeagueColor,
                //    LeagueName =  temp.LeagueName,
                //    Hi = temp.Hi,
                //    Gi =  temp.Gi,
                //    GuestTeamId =   temp.GuestTeamId,
                //    HomeTeamId =  temp.HomeTeamId,
                //    FXId =   temp.FXId,
                //    Mid =  temp.Mid,
                //    WinOdds =  temp.WinOdds,
                //    FlatOdds = temp.FlatOdds,
                //    LoseOdds =  temp.LoseOdds,
                //};
                //list.Add(match);

                //var spf_SP = GetSP(matchId, spf_SP_Dic);
                //var brqspf_SP = GetSP(matchId, brqspf_SP_Dic);
                //match.WinOdds = spf_SP[0];
                //match.FlatOdds = spf_SP[1];
                //match.LoseOdds = spf_SP[2];
                //spfMatch.Add(match);

                //match.WinOdds = brqspf_SP[0];
                //match.FlatOdds = brqspf_SP[1];
                //match.LoseOdds = brqspf_SP[2];
                //brqspfMatch.Add(match);
            }
            return list;
        }

        private decimal[] GetSP(string matchId, Dictionary<string, string> dic)
        {
            var t = new decimal[] { 0, 0, 0 };
            var item = dic.FirstOrDefault(p => p.Key == matchId);
            if (string.IsNullOrEmpty(item.Value))
                return t;
            var array = item.Value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length != 3)
                return t;
            for (int i = 0; i < array.Length; i++)
            {
                t[i] = decimal.Parse(array[i]);
            }
            return t;
        }

        private List<C_JCZQ_SPF_SP> Get_SPF_SP_ZHM(string gameType, List<JCZQ_MatchInfo> matchList)
        {
            var list = new List<C_JCZQ_SPF_SP>();
            var spf = GetSP_ZHM(gameType);
            foreach (var item in spf)
            {
                var match = matchList.FirstOrDefault(p => p.MatchIdName == item.Key);
                if (match == null) continue;
                //var zhou = item.Key.Substring(0, 2);
                //var matchNumber = item.Key.Substring(2);
                //var matchDate = GetMatchDate(zhou, true);
                //var matchId = matchDate + matchNumber;

                if (string.IsNullOrEmpty(item.Value))
                    continue;
                var array = item.Value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (array.Length != 3)
                    continue;

                list.Add(new C_JCZQ_SPF_SP
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    MatchId = match.MatchId,
                    MatchData = match.MatchData,
                    MatchNumber = match.MatchNumber,
                    WinOdds = decimal.Parse(array[0]),
                    FlatOdds = decimal.Parse(array[1]),
                    LoseOdds = decimal.Parse(array[2]),
                });
            }
            return list;
        }

        private Dictionary<string, string> GetSP_ZHM(string gameType)
        {
            var code = "013";
            var xml = string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?><msg><head transcode=\"{0}\" partnerid=\"{1}\" version=\"1.0\" time=\"{2}\"></head><body>", code, zhm_PartnerId, DateTime.Now.ToString("yyyyMMddHHmmss"));
            xml += string.Format("<querygamesp type=\"jczqgg\" lotteryId =\"{0}\" macthdate =\"{1}\" />", gameType, DateTime.Now.ToString("yyyy-MM-dd"));
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

            var doc = new XmlDocument();
            doc.LoadXml(resultXml);
            var root = doc.SelectSingleNode("//body/spInfos");
            if (root == null)
                throw new Exception("从xml中查询节点错误  - " + resultXml);

            var dic = new Dictionary<string, string>();
            foreach (XmlNode item in root.ChildNodes)
            {
                var matchId = item.Attributes["matchId"].Value;
                var sp = item.Attributes["sp"].Value;
                dic.Add(matchId, sp);
            }
            return dic;
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

        #region 采集SP

        private List<C_JCZQ_SPF_SP> Get_SPF_SP(List<JCZQ_MatchInfo> tempMatchList, out List<JCZQ_MatchInfo> matchList)
        {
            matchList = new List<JCZQ_MatchInfo>();
            var spf_xml = GetSPXmlContent("SPF");
            var list = new List<C_JCZQ_SPF_SP>();
            if (string.IsNullOrEmpty(spf_xml))
                return list;
            var doc = new XmlDocument();
            doc.LoadXml(spf_xml);
            var root = doc.SelectSingleNode("Resp/matches");
            if (root == null)
                throw new Exception("从xml中查询节点错误  - " + spf_xml);
            foreach (XmlNode item in root.ChildNodes)
            {
                var matchId = item.Attributes["expectitemid"].Value;
                if (string.IsNullOrEmpty(matchId))
                    continue;
                var temp = tempMatchList.FirstOrDefault(s => s.MatchId == matchId);
                if (temp == null)
                    continue;
                temp.MatchIdName = item.Attributes["itemname"].Value;
                temp.DSStopBettingTime = item.Attributes["dsendtime"].Value.GetDateTime().AddMinutes(JCZQ_advanceMinutes).ToString("yyyy-MM-dd HH:mm:ss");
                temp.FSStopBettingTime = item.Attributes["fsendtime"].Value.GetDateTime().AddMinutes(JCZQ_advanceMinutes).ToString("yyyy-MM-dd HH:mm:ss");
                temp.StartDateTime = item.Attributes["matchtime"].Value.GetDateTime().ToString("yyyy-MM-dd HH:mm:ss");
                temp.HomeTeamName = item.Attributes["hostteam"].Value;
                temp.GuestTeamName = item.Attributes["visitteam"].Value;
                matchList.Add(temp);

                list.Add(new C_JCZQ_SPF_SP
                {
                    MatchId = matchId,
                    MatchData = item.Attributes["expect"].Value,
                    MatchNumber = item.Attributes["itemid"].Value.PadLeft(3, '0'),
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    WinOdds = item.Attributes["spf3"].Value.GetDecimal(),
                    FlatOdds = item.Attributes["spf1"].Value.GetDecimal(),
                    LoseOdds = item.Attributes["spf0"].Value.GetDecimal(),
                });
            }
            return list;
        }

        private List<C_JCZQ_SPF_SP> Get_SPF_SP()
        {
            var spf_xml = GetSPXmlContent("SPF");
            var list = new List<C_JCZQ_SPF_SP>();
            if (string.IsNullOrEmpty(spf_xml))
                return list;
            var doc = new XmlDocument();
            doc.LoadXml(spf_xml);
            var root = doc.SelectSingleNode("Resp/matches");
            if (root == null)
                throw new Exception("从xml中查询节点错误  - " + spf_xml);
            foreach (XmlNode item in root.ChildNodes)
            {
                var matchId = item.Attributes["expectitemid"].Value;
                if (string.IsNullOrEmpty(matchId))
                    continue;

                list.Add(new C_JCZQ_SPF_SP
                {
                    MatchId = matchId,
                    MatchData = item.Attributes["expect"].Value,
                    MatchNumber = item.Attributes["itemid"].Value.PadLeft(3, '0'),
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    WinOdds = item.Attributes["spf3"].Value.GetDecimal(),
                    FlatOdds = item.Attributes["spf1"].Value.GetDecimal(),
                    LoseOdds = item.Attributes["spf0"].Value.GetDecimal(),
                });
            }
            return list;
        }

        private List<C_JCZQ_SPF_SP> Get_SPF_SP_New(bool isDS, out List<C_JCZQ_SPF_SP> brqSPF_SP)
        {
            var json = GetSPJsonContent("SPF", isDS);
            var list = new List<C_JCZQ_SPF_SP>();
            brqSPF_SP = new List<C_JCZQ_SPF_SP>();
            if (string.IsNullOrEmpty(json) || json == "404")
                return list;

            var array =JsonHelper.Decode(json);
            foreach (var item in array)
            {
                var matchId = item.id;
                if (string.IsNullOrEmpty(matchId))
                    continue;

                var matchData = matchId.Substring(0, 6);
                var matchNumber = matchId.Substring(6);
                list.Add(new C_JCZQ_SPF_SP
                {
                    MatchId = matchId,
                    MatchData = matchData,
                    MatchNumber = matchNumber,
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    WinOdds = string.IsNullOrEmpty(item.R3) ? 0 : decimal.Parse(item.R3),
                    FlatOdds = string.IsNullOrEmpty(item.R1) ? 0 : decimal.Parse(item.R1),
                    LoseOdds = string.IsNullOrEmpty(item.R0) ? 0 : decimal.Parse(item.R0),
                });

                brqSPF_SP.Add(new C_JCZQ_SPF_SP
                {
                    MatchId = matchId,
                    MatchData = matchData,
                    MatchNumber = matchNumber,
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    WinOdds = string.IsNullOrEmpty(item.s3) ? 0 : decimal.Parse(item.s3),
                    FlatOdds = string.IsNullOrEmpty(item.s1) ? 0 : decimal.Parse(item.s1),
                    LoseOdds = string.IsNullOrEmpty(item.s0) ? 0 : decimal.Parse(item.s0),
                });
            }
            return list;
        }

        private List<C_JCZQ_BF_SP> Get_BF_SP()
        {
            var bf_xml = GetSPXmlContent("BF");
            var list = new List<C_JCZQ_BF_SP>();
            if (string.IsNullOrEmpty(bf_xml))
                return list;
            var doc = new XmlDocument();
            doc.LoadXml(bf_xml);
            var root = doc.SelectSingleNode("Resp/matches");
            if (root == null)
                throw new Exception("从xml中查询节点错误  - " + bf_xml);
            foreach (XmlNode item in root.ChildNodes)
            {
                list.Add(new C_JCZQ_BF_SP
                {
                    MatchId = item.Attributes["expectitemid"].Value,
                    MatchData = item.Attributes["expect"].Value,
                    MatchNumber = item.Attributes["itemid"].Value.PadLeft(3, '0'),
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    S_QT = item.Attributes["bf90"].Value.GetDecimal(),
                    P_QT = item.Attributes["bf99"].Value.GetDecimal(),
                    F_QT = item.Attributes["bf09"].Value.GetDecimal(),
                    S_52 = item.Attributes["bf52"].Value.GetDecimal(),
                    S_51 = item.Attributes["bf51"].Value.GetDecimal(),
                    S_50 = item.Attributes["bf50"].Value.GetDecimal(),
                    S_42 = item.Attributes["bf42"].Value.GetDecimal(),
                    S_41 = item.Attributes["bf41"].Value.GetDecimal(),
                    S_40 = item.Attributes["bf40"].Value.GetDecimal(),
                    S_32 = item.Attributes["bf32"].Value.GetDecimal(),
                    S_31 = item.Attributes["bf31"].Value.GetDecimal(),
                    S_30 = item.Attributes["bf30"].Value.GetDecimal(),
                    S_21 = item.Attributes["bf21"].Value.GetDecimal(),
                    S_20 = item.Attributes["bf20"].Value.GetDecimal(),
                    S_10 = item.Attributes["bf10"].Value.GetDecimal(),
                    P_00 = item.Attributes["bf00"].Value.GetDecimal(),
                    P_11 = item.Attributes["bf11"].Value.GetDecimal(),
                    P_22 = item.Attributes["bf22"].Value.GetDecimal(),
                    P_33 = item.Attributes["bf33"].Value.GetDecimal(),
                    F_01 = item.Attributes["bf01"].Value.GetDecimal(),
                    F_02 = item.Attributes["bf02"].Value.GetDecimal(),
                    F_03 = item.Attributes["bf03"].Value.GetDecimal(),
                    F_04 = item.Attributes["bf04"].Value.GetDecimal(),
                    F_05 = item.Attributes["bf05"].Value.GetDecimal(),
                    F_12 = item.Attributes["bf12"].Value.GetDecimal(),
                    F_13 = item.Attributes["bf13"].Value.GetDecimal(),
                    F_14 = item.Attributes["bf14"].Value.GetDecimal(),
                    F_15 = item.Attributes["bf15"].Value.GetDecimal(),
                    F_23 = item.Attributes["bf23"].Value.GetDecimal(),
                    F_24 = item.Attributes["bf24"].Value.GetDecimal(),
                    F_25 = item.Attributes["bf25"].Value.GetDecimal(),
                });
            }
            return list;
        }

        private List<C_JCZQ_BF_SP> Get_BF_SP_New(bool isDS)
        {
            var json = GetSPJsonContent("BF", isDS);
            var list = new List<C_JCZQ_BF_SP>();
            if (string.IsNullOrEmpty(json) || json == "404")
                return list;

            var array =JsonHelper.Decode(json);
            foreach (var item in array)
            {
                var matchId = item.id;
                if (string.IsNullOrEmpty(matchId))
                    continue;

                var matchData = matchId.Substring(0, 6);
                var matchNumber = matchId.Substring(6);

                list.Add(new C_JCZQ_BF_SP
                {
                    MatchId = matchId,
                    MatchData = matchData,
                    MatchNumber = matchNumber,
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    S_QT = string.IsNullOrEmpty(item.s90) ? 0 : decimal.Parse(item.s90),
                    P_QT = string.IsNullOrEmpty(item.s99) ? 0 : decimal.Parse(item.s99),
                    F_QT = string.IsNullOrEmpty(item.s09) ? 0 : decimal.Parse(item.s09),
                    S_52 = string.IsNullOrEmpty(item.s52) ? 0 : decimal.Parse(item.s52),
                    S_51 = string.IsNullOrEmpty(item.s51) ? 0 : decimal.Parse(item.s51),
                    S_50 = string.IsNullOrEmpty(item.s50) ? 0 : decimal.Parse(item.s50),
                    S_42 = string.IsNullOrEmpty(item.s42) ? 0 : decimal.Parse(item.s42),
                    S_41 = string.IsNullOrEmpty(item.s41) ? 0 : decimal.Parse(item.s41),
                    S_40 = string.IsNullOrEmpty(item.s40) ? 0 : decimal.Parse(item.s40),
                    S_32 = string.IsNullOrEmpty(item.s32) ? 0 : decimal.Parse(item.s32),
                    S_31 = string.IsNullOrEmpty(item.s31) ? 0 : decimal.Parse(item.s31),
                    S_30 = string.IsNullOrEmpty(item.s30) ? 0 : decimal.Parse(item.s30),
                    S_21 = string.IsNullOrEmpty(item.s21) ? 0 : decimal.Parse(item.s21),
                    S_20 = string.IsNullOrEmpty(item.s20) ? 0 : decimal.Parse(item.s20),
                    S_10 = string.IsNullOrEmpty(item.s10) ? 0 : decimal.Parse(item.s10),
                    P_00 = string.IsNullOrEmpty(item.s00) ? 0 : decimal.Parse(item.s00),
                    P_11 = string.IsNullOrEmpty(item.s11) ? 0 : decimal.Parse(item.s11),
                    P_22 = string.IsNullOrEmpty(item.s22) ? 0 : decimal.Parse(item.s22),
                    P_33 = string.IsNullOrEmpty(item.s33) ? 0 : decimal.Parse(item.s33),
                    F_01 = string.IsNullOrEmpty(item.s01) ? 0 : decimal.Parse(item.s01),
                    F_02 = string.IsNullOrEmpty(item.s02) ? 0 : decimal.Parse(item.s02),
                    F_03 = string.IsNullOrEmpty(item.s03) ? 0 : decimal.Parse(item.s03),
                    F_04 = string.IsNullOrEmpty(item.s04) ? 0 : decimal.Parse(item.s04),
                    F_05 = string.IsNullOrEmpty(item.s05) ? 0 : decimal.Parse(item.s05),
                    F_12 = string.IsNullOrEmpty(item.s12) ? 0 : decimal.Parse(item.s12),
                    F_13 = string.IsNullOrEmpty(item.s13) ? 0 : decimal.Parse(item.s13),
                    F_14 = string.IsNullOrEmpty(item.s14) ? 0 : decimal.Parse(item.s14),
                    F_15 = string.IsNullOrEmpty(item.s15) ? 0 : decimal.Parse(item.s15),
                    F_23 = string.IsNullOrEmpty(item.s23) ? 0 : decimal.Parse(item.s23),
                    F_24 = string.IsNullOrEmpty(item.s24) ? 0 : decimal.Parse(item.s24),
                    F_25 = string.IsNullOrEmpty(item.s25) ? 0 : decimal.Parse(item.s25),
                });
            }
            return list;
        }

        private List<C_JCZQ_ZJQ_SP> Get_ZJQ_SP()
        {
            var zjq_xml = GetSPXmlContent("ZJQ");
            var list = new List<C_JCZQ_ZJQ_SP>();
            if (string.IsNullOrEmpty(zjq_xml))
                return list;
            var doc = new XmlDocument();
            doc.LoadXml(zjq_xml);
            var root = doc.SelectSingleNode("Resp/matches");
            if (root == null)
                throw new Exception("从xml中查询节点错误  - " + zjq_xml);
            foreach (XmlNode item in root.ChildNodes)
            {
                list.Add(new C_JCZQ_ZJQ_SP
                {
                    MatchId = item.Attributes["expectitemid"].Value,
                    MatchData = item.Attributes["expect"].Value,
                    MatchNumber = item.Attributes["itemid"].Value.PadLeft(3, '0'),
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    JinQiu_0_Odds = item.Attributes["jqs0"].Value.GetDecimal(),
                    JinQiu_1_Odds = item.Attributes["jqs1"].Value.GetDecimal(),
                    JinQiu_2_Odds = item.Attributes["jqs2"].Value.GetDecimal(),
                    JinQiu_3_Odds = item.Attributes["jqs3"].Value.GetDecimal(),
                    JinQiu_4_Odds = item.Attributes["jqs4"].Value.GetDecimal(),
                    JinQiu_5_Odds = item.Attributes["jqs5"].Value.GetDecimal(),
                    JinQiu_6_Odds = item.Attributes["jqs6"].Value.GetDecimal(),
                    JinQiu_7_Odds = item.Attributes["jqs7"].Value.GetDecimal(),
                });
            }

            return list;
        }

        private List<C_JCZQ_ZJQ_SP> Get_ZJQ_SP_New(bool isDS)
        {
            var json = GetSPJsonContent("ZJQ", isDS);
            var list = new List<C_JCZQ_ZJQ_SP>();
            if (string.IsNullOrEmpty(json) || json == "404")
                return list;

            var array =JsonHelper.Decode(json);
            foreach (var item in array)
            {
                var matchId = item.id;
                if (string.IsNullOrEmpty(matchId))
                    continue;

                var matchData = matchId.Substring(0, 6);
                var matchNumber = matchId.Substring(6);
                list.Add(new C_JCZQ_ZJQ_SP
                {
                    MatchId = matchId,
                    MatchData = matchData,
                    MatchNumber = matchNumber,
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    JinQiu_0_Odds = string.IsNullOrEmpty(item.s0) ? 0 : decimal.Parse(item.s0),
                    JinQiu_1_Odds = string.IsNullOrEmpty(item.s1) ? 0 : decimal.Parse(item.s1),
                    JinQiu_2_Odds = string.IsNullOrEmpty(item.s2) ? 0 : decimal.Parse(item.s2),
                    JinQiu_3_Odds = string.IsNullOrEmpty(item.s3) ? 0 : decimal.Parse(item.s3),
                    JinQiu_4_Odds = string.IsNullOrEmpty(item.s4) ? 0 : decimal.Parse(item.s4),
                    JinQiu_5_Odds = string.IsNullOrEmpty(item.s5) ? 0 : decimal.Parse(item.s5),
                    JinQiu_6_Odds = string.IsNullOrEmpty(item.s6) ? 0 : decimal.Parse(item.s6),
                    JinQiu_7_Odds = string.IsNullOrEmpty(item.s7) ? 0 : decimal.Parse(item.s7),
                });
            }
            return list;
        }

        private List<C_JCZQ_BQC_SP> Get_BQC_SP()
        {
            var bqc_xml = GetSPXmlContent("BQC");
            var list = new List<C_JCZQ_BQC_SP>();
            if (string.IsNullOrEmpty(bqc_xml))
                return list;
            var doc = new XmlDocument();
            doc.LoadXml(bqc_xml);
            var root = doc.SelectSingleNode("Resp/matches");
            if (root == null)
                throw new Exception("从xml中查询节点错误  - " + bqc_xml);
            foreach (XmlNode item in root.ChildNodes)
            {
                list.Add(new C_JCZQ_BQC_SP
                {
                    MatchId = item.Attributes["expectitemid"].Value,
                    MatchData = item.Attributes["expect"].Value,
                    MatchNumber = item.Attributes["itemid"].Value.PadLeft(3, '0'),
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    SH_SH_Odds = item.Attributes["bqc33"].Value.GetDecimal(),
                    SH_P_Odds = item.Attributes["bqc31"].Value.GetDecimal(),
                    SH_F_Odds = item.Attributes["bqc30"].Value.GetDecimal(),
                    P_SH_Odds = item.Attributes["bqc13"].Value.GetDecimal(),
                    P_P_Odds = item.Attributes["bqc11"].Value.GetDecimal(),
                    P_F_Odds = item.Attributes["bqc10"].Value.GetDecimal(),
                    F_SH_Odds = item.Attributes["bqc03"].Value.GetDecimal(),
                    F_P_Odds = item.Attributes["bqc01"].Value.GetDecimal(),
                    F_F_Odds = item.Attributes["bqc00"].Value.GetDecimal(),
                });
            }
            return list;
        }

        private List<C_JCZQ_BQC_SP> Get_BQC_SP_New(bool isDS)
        {
            var json = GetSPJsonContent("BQC", isDS);
            var list = new List<C_JCZQ_BQC_SP>();
            if (string.IsNullOrEmpty(json) || json == "404")
                return list;

            var array =JsonHelper.Decode(json);
            foreach (var item in array)
            {
                var matchId = item.id;
                if (string.IsNullOrEmpty(matchId))
                    continue;

                var matchData = matchId.Substring(0, 6);
                var matchNumber = matchId.Substring(6);
                list.Add(new C_JCZQ_BQC_SP
                {
                    MatchId = matchId,
                    MatchData = matchData,
                    MatchNumber = matchNumber,
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    SH_SH_Odds = string.IsNullOrEmpty(item.s33) ? 0 : decimal.Parse(item.s33),
                    SH_P_Odds = string.IsNullOrEmpty(item.s31) ? 0 : decimal.Parse(item.s31),
                    SH_F_Odds = string.IsNullOrEmpty(item.s30) ? 0 : decimal.Parse(item.s30),
                    P_SH_Odds = string.IsNullOrEmpty(item.s13) ? 0 : decimal.Parse(item.s13),
                    P_P_Odds = string.IsNullOrEmpty(item.s11) ? 0 : decimal.Parse(item.s11),
                    P_F_Odds = string.IsNullOrEmpty(item.s10) ? 0 : decimal.Parse(item.s10),
                    F_SH_Odds = string.IsNullOrEmpty(item.s03) ? 0 : decimal.Parse(item.s03),
                    F_P_Odds = string.IsNullOrEmpty(item.s01) ? 0 : decimal.Parse(item.s01),
                    F_F_Odds = string.IsNullOrEmpty(item.s00) ? 0 : decimal.Parse(item.s00),
                });
            }
            return list;
        }

        private List<C_JCZQ_SPF_SP> Get_SPF_SP_ZZJCW(out List<C_JCZQ_SPF_SP> brqSPF_SP)
        {
            var json = GetSPJsonContent_ZZJCW("SPF");
            var list = new List<C_JCZQ_SPF_SP>();
            brqSPF_SP = new List<C_JCZQ_SPF_SP>();
            if (string.IsNullOrEmpty(json) || json == "404")
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
                    //string b_date = match_D.b_date;
                    //string b_date = match_D.b_date;

                    string matchId =b_date.Substring(2).Replace("-", "") +num.Substring(2);
                    var matchData = matchId.Substring(0, 6);
                    var matchNumber = matchId.Substring(6);
                    var winOdds = 0M;
                    var flatOdds = 0M;
                    var loseOdds = 0M;
                    if (!(match_D.hhad == null))
                    {
                        string h = match_D.hhad.h;
                        string d = match_D.hhad.d;
                        string a = match_D.hhad.a;

                        winOdds = decimal.Parse(h);
                        flatOdds = decimal.Parse(d);
                        loseOdds = decimal.Parse(a);
                    }
                    var winOdds_brq = 0M;
                    var flatOdds_brq = 0M;
                    var loseOdds_brq = 0M;
                    if (!(match_D.had == null))
                    {
                        string h = match_D.had.h;
                        string d = match_D.had.d;
                        string a = match_D.had.a;
                        winOdds_brq = decimal.Parse(h);
                        flatOdds_brq = decimal.Parse(d);
                        loseOdds_brq = decimal.Parse(a);
                    }
                    list.Add(new C_JCZQ_SPF_SP
                    {
                        MatchId = matchId,
                        MatchData = matchData,
                        MatchNumber = matchNumber,
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        WinOdds = winOdds,
                        FlatOdds = flatOdds,
                        LoseOdds = loseOdds,
                    });

                    brqSPF_SP.Add(new C_JCZQ_SPF_SP
                    {
                        MatchId = matchId,
                        MatchData = matchData,
                        MatchNumber = matchNumber,
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        WinOdds = winOdds_brq,
                        FlatOdds = flatOdds_brq,
                        LoseOdds = loseOdds_brq,
                    });
                }
            }
            return list;
        }

        private List<C_JCZQ_BF_SP> Get_BF_SP_ZZJCW()
        {
            //var json = GetSPJsonContent_ZZJCW("BF").Replace("-1-h", "S_QT").Replace("-1-d", "P_QT").Replace("-1-a", "F_QT").Replace("0502", "S_52").Replace("0501", "S_51").Replace("0500", "S_50").Replace("0402", "S_42")
            //    .Replace("0401", "S_41").Replace("0400", "S_40").Replace("0302", "S_32").Replace("0301", "S_31").Replace("0300", "S_30").Replace("0201", "S_21").Replace("0200", "S_20").Replace("0100", "S_10")
            //    .Replace("0000", "P_00").Replace("0101", "P_11").Replace("0202", "P_22").Replace("0303", "P_33").Replace("0001", "F_01").Replace("0002", "F_02").Replace("0003", "F_03").Replace("0004", "F_04")
            //    .Replace("0005", "F_05").Replace("0102", "F_12").Replace("0103", "F_13").Replace("0104", "F_14").Replace("0105", "F_15").Replace("0203", "F_23").Replace("0204", "F_24").Replace("0205", "F_25");
            var json = GetSPJsonContent_ZZJCW("BF").Replace("\"-1-h\"", "\"S_QT\"").Replace("\"-1-d\"", "\"P_QT\"").Replace("\"-1-a\"", "\"F_QT\"").Replace("\"0502\"", "\"S_52\"").Replace("\"0501\"", "\"S_51\"").Replace("\"0500\"", "\"S_50\"").Replace("\"0402\"", "\"S_42\"")
                .Replace("\"0401\"", "\"S_41\"").Replace("\"0400\"", "\"S_40\"").Replace("\"0302\"", "\"S_32\"").Replace("\"0301\"", "\"S_31\"").Replace("\"0300\"", "\"S_30\"").Replace("\"0201\"", "\"S_21\"").Replace("\"0200\"", "\"S_20\"").Replace("\"0100\"", "\"S_10\"")
                .Replace("\"0000\"", "\"P_00\"").Replace("\"0101\"", "\"P_11\"").Replace("\"0202\"", "\"P_22\"").Replace("\"0303\"", "\"P_33\"").Replace("\"0001\"", "\"F_01\"").Replace("\"0002\"", "\"F_02\"").Replace("\"0003\"", "\"F_03\"").Replace("\"0004\"", "\"F_04\"")
                .Replace("\"0005\"", "\"F_05\"").Replace("\"0102\"", "\"F_12\"").Replace("\"0103\"", "\"F_13\"").Replace("\"0104\"", "\"F_14\"").Replace("\"0105\"", "\"F_15\"").Replace("\"0203\"", "\"F_23\"").Replace("\"0204\"", "\"F_24\"").Replace("\"0205\"", "\"F_25\""); ;
            var list = new List<C_JCZQ_BF_SP>();
            if (string.IsNullOrEmpty(json) || json == "404")
                return list;

            var array =JsonHelper.Decode(json);
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

                    string matchId =b_date.Substring(2).Replace("-", "") +num.Substring(2);
                    var matchData = matchId.Substring(0, 6);
                    var matchNumber = matchId.Substring(6);

                    list.Add(new C_JCZQ_BF_SP
                    {
                        MatchId = matchId,
                        MatchData = matchData,
                        MatchNumber = matchNumber,
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        S_QT = string.IsNullOrEmpty(match_D.crs.S_QT.Value) ? 0 : decimal.Parse(match_D.crs.S_QT.Value),
                        P_QT = string.IsNullOrEmpty(match_D.crs.P_QT.Value) ? 0 : decimal.Parse(match_D.crs.P_QT.Value),
                        F_QT = string.IsNullOrEmpty(match_D.crs.F_QT.Value) ? 0 : decimal.Parse(match_D.crs.F_QT.Value),
                        S_52 = string.IsNullOrEmpty(match_D.crs.S_52.Value) ? 0 : decimal.Parse(match_D.crs.S_52.Value),
                        S_51 = string.IsNullOrEmpty(match_D.crs.S_51.Value) ? 0 : decimal.Parse(match_D.crs.S_51.Value),
                        S_50 = string.IsNullOrEmpty(match_D.crs.S_50.Value) ? 0 : decimal.Parse(match_D.crs.S_50.Value),
                        S_42 = string.IsNullOrEmpty(match_D.crs.S_42.Value) ? 0 : decimal.Parse(match_D.crs.S_42.Value),
                        S_41 = string.IsNullOrEmpty(match_D.crs.S_41.Value) ? 0 : decimal.Parse(match_D.crs.S_41.Value),
                        S_40 = string.IsNullOrEmpty(match_D.crs.S_40.Value) ? 0 : decimal.Parse(match_D.crs.S_40.Value),
                        S_32 = string.IsNullOrEmpty(match_D.crs.S_32.Value) ? 0 : decimal.Parse(match_D.crs.S_32.Value),
                        S_31 = string.IsNullOrEmpty(match_D.crs.S_31.Value) ? 0 : decimal.Parse(match_D.crs.S_31.Value),
                        S_30 = string.IsNullOrEmpty(match_D.crs.S_30.Value) ? 0 : decimal.Parse(match_D.crs.S_30.Value),
                        S_21 = string.IsNullOrEmpty(match_D.crs.S_21.Value) ? 0 : decimal.Parse(match_D.crs.S_21.Value),
                        S_20 = string.IsNullOrEmpty(match_D.crs.S_20.Value) ? 0 : decimal.Parse(match_D.crs.S_20.Value),
                        S_10 = string.IsNullOrEmpty(match_D.crs.S_10.Value) ? 0 : decimal.Parse(match_D.crs.S_10.Value),
                        P_00 = string.IsNullOrEmpty(match_D.crs.P_00.Value) ? 0 : decimal.Parse(match_D.crs.P_00.Value),
                        P_11 = string.IsNullOrEmpty(match_D.crs.P_11.Value) ? 0 : decimal.Parse(match_D.crs.P_11.Value),
                        P_22 = string.IsNullOrEmpty(match_D.crs.P_22.Value) ? 0 : decimal.Parse(match_D.crs.P_22.Value),
                        P_33 = string.IsNullOrEmpty(match_D.crs.P_33.Value) ? 0 : decimal.Parse(match_D.crs.P_33.Value),
                        F_01 = string.IsNullOrEmpty(match_D.crs.F_01.Value) ? 0 : decimal.Parse(match_D.crs.F_01.Value),
                        F_02 = string.IsNullOrEmpty(match_D.crs.F_02.Value) ? 0 : decimal.Parse(match_D.crs.F_02.Value),
                        F_03 = string.IsNullOrEmpty(match_D.crs.F_03.Value) ? 0 : decimal.Parse(match_D.crs.F_03.Value),
                        F_04 = string.IsNullOrEmpty(match_D.crs.F_04.Value) ? 0 : decimal.Parse(match_D.crs.F_04.Value),
                        F_05 = string.IsNullOrEmpty(match_D.crs.F_05.Value) ? 0 : decimal.Parse(match_D.crs.F_05.Value),
                        F_12 = string.IsNullOrEmpty(match_D.crs.F_12.Value) ? 0 : decimal.Parse(match_D.crs.F_12.Value),
                        F_13 = string.IsNullOrEmpty(match_D.crs.F_13.Value) ? 0 : decimal.Parse(match_D.crs.F_13.Value),
                        F_14 = string.IsNullOrEmpty(match_D.crs.F_14.Value) ? 0 : decimal.Parse(match_D.crs.F_14.Value),
                        F_15 = string.IsNullOrEmpty(match_D.crs.F_15.Value) ? 0 : decimal.Parse(match_D.crs.F_15.Value),
                        F_23 = string.IsNullOrEmpty(match_D.crs.F_23.Value) ? 0 : decimal.Parse(match_D.crs.F_23.Value),
                        F_24 = string.IsNullOrEmpty(match_D.crs.F_24.Value) ? 0 : decimal.Parse(match_D.crs.F_24.Value),
                        F_25 = string.IsNullOrEmpty(match_D.crs.F_25.Value) ? 0 : decimal.Parse(match_D.crs.F_25.Value),
                    });                                                                               
                }                                                                                           
            }                                                                                               
            return list;
        }

        private List<C_JCZQ_ZJQ_SP> Get_ZJQ_SP_ZZJCW()
        {
            var json = GetSPJsonContent_ZZJCW("ZJQ");
            var list = new List<C_JCZQ_ZJQ_SP>();
            if (string.IsNullOrEmpty(json) || json == "404")
                return list;

            var array =JsonHelper.Decode(json);
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
                    list.Add(new C_JCZQ_ZJQ_SP
                    {
                        MatchId = matchId,
                        MatchData = matchData,
                        MatchNumber = matchNumber,
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        JinQiu_0_Odds = string.IsNullOrEmpty(match_D.ttg.s0.Value) ? 0 : decimal.Parse(match_D.ttg.s0.Value),
                        JinQiu_1_Odds = string.IsNullOrEmpty(match_D.ttg.s1.Value) ? 0 : decimal.Parse(match_D.ttg.s1.Value),
                        JinQiu_2_Odds = string.IsNullOrEmpty(match_D.ttg.s2.Value) ? 0 : decimal.Parse(match_D.ttg.s2.Value),
                        JinQiu_3_Odds = string.IsNullOrEmpty(match_D.ttg.s3.Value) ? 0 : decimal.Parse(match_D.ttg.s3.Value),
                        JinQiu_4_Odds = string.IsNullOrEmpty(match_D.ttg.s4.Value) ? 0 : decimal.Parse(match_D.ttg.s4.Value),
                        JinQiu_5_Odds = string.IsNullOrEmpty(match_D.ttg.s5.Value) ? 0 : decimal.Parse(match_D.ttg.s5.Value),
                        JinQiu_6_Odds = string.IsNullOrEmpty(match_D.ttg.s6.Value) ? 0 : decimal.Parse(match_D.ttg.s6.Value),
                        JinQiu_7_Odds = string.IsNullOrEmpty(match_D.ttg.s7.Value) ? 0 : decimal.Parse(match_D.ttg.s7.Value),
                    });
                }
            }
            return list;
        }

        private List<C_JCZQ_BQC_SP> Get_BQC_SP_ZZJCW()
        {
            var json = GetSPJsonContent_ZZJCW("BQC");
            var list = new List<C_JCZQ_BQC_SP>();
            if (string.IsNullOrEmpty(json) || json=="404")
                return list;

            var array =JsonHelper.Decode(json);
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
                    list.Add(new C_JCZQ_BQC_SP
                    {
                        MatchId = matchId,
                        MatchData = matchData,
                        MatchNumber = matchNumber,
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        SH_SH_Odds = string.IsNullOrEmpty(match_D.hafu.hh.Value) ? 0 : decimal.Parse(match_D.hafu.hh .Value),
                        SH_P_Odds = string.IsNullOrEmpty(match_D.hafu.hd .Value) ? 0 : decimal.Parse(match_D.hafu.hd .Value),
                        SH_F_Odds = string.IsNullOrEmpty(match_D.hafu.ha .Value) ? 0 : decimal.Parse(match_D.hafu.ha .Value),
                        P_SH_Odds = string.IsNullOrEmpty(match_D.hafu.dh .Value) ? 0 : decimal.Parse(match_D.hafu.dh .Value),
                        P_P_Odds = string.IsNullOrEmpty(match_D.hafu.dd .Value) ? 0 : decimal.Parse(match_D.hafu.dd .Value),
                        P_F_Odds = string.IsNullOrEmpty(match_D.hafu.da .Value) ? 0 : decimal.Parse(match_D.hafu.da .Value),
                        F_SH_Odds = string.IsNullOrEmpty(match_D.hafu.ah .Value) ? 0 : decimal.Parse(match_D.hafu.ah .Value),
                        F_P_Odds = string.IsNullOrEmpty(match_D.hafu.ad .Value) ? 0 : decimal.Parse(match_D.hafu.ad .Value),
                        F_F_Odds = string.IsNullOrEmpty(match_D.hafu.aa .Value) ? 0 : decimal.Parse(match_D.hafu.aa .Value),
                    });
                }
            }
            return list;
        }
        #endregion

        #region 采集欧赔SP

        /// <summary>
        /// 保存OZ SP
        /// </summary>
        //private void Save_OZ_SPInfo<T>(List<T> list, string fileName) where T : JCZQ_SPF_OZ_SPInfo
        //{
        //    var fileFullName = BuildFileFullName(fileName);
        //    if (list.Count == 0) return;
        //    var customerSavePath = new string[] { "JCZQ" };

        //    try
        //    {
        //        ServiceHelper.CreateOrAppend_JSONFile(fileFullName, JsonSerializer.Serialize(list), (log) =>
        //        {
        //            this.WriteLog(log);
        //        });

        //        //上传文件
        //        ServiceHelper.PostFileToServer(fileFullName, customerSavePath, (log) =>
        //        {
        //            this.WriteLog(log);
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        this.WriteLog(string.Format("写入 OZ SP 数据文件 {0} 失败：{1}", fileFullName, ex.ToString()));
        //    }
        //}

        #endregion

        #region 队伍对阵历史

        //private void SaveTeamHistoryMatch(List<JCZQ_MatchInfo> list)
        //{
        //    var encoding = Encoding.GetEncoding("gb2312");

        //    if (string.IsNullOrEmpty(SavePath))
        //        SavePath = ServiceHelper.Get_JCZQ_SavePath();
        //    foreach (var item in list)
        //    {
        //        try
        //        {
        //            var path = Path.Combine(SavePath, item.MatchData);
        //            if (!Directory.Exists(path))
        //                Directory.CreateDirectory(path);
        //            var customerSavePath = new string[] { "JCZQ", item.MatchData };

        //            var hFullFileName = Path.Combine(path, string.Format("TeamHistory_{0}.json", item.Hi));
        //            //if (!File.Exists(hFullFileName))
        //            //{
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
        //            //}
        //            var gFullFileName = Path.Combine(path, string.Format("TeamHistory_{0}.json", item.Gi));
        //            //if (!File.Exists(gFullFileName))
        //            //{
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
        //            //}
        //        }
        //        catch (Exception ex)
        //        {
        //            this.WriteLog(string.Format("写入 队伍历史 数据文件   失败：{0}", ex.ToString()));
        //        }
        //    }
        //}

        private List<JCZQ_Team_History> GetHistory(string id)
        {
            if (string.IsNullOrEmpty(id)) return new List<JCZQ_Team_History>();
            var baseTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));//.AddSeconds(1366558200)
            var encoding = Encoding.UTF8;
            var url = string.Format("http://www.9188.com/data/static/odds/historymatch/{0}.xml", id);
            var hxml = PostManager.Get(url, encoding, 0, (request) =>
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
            var historyList = new List<JCZQ_Team_History>();
            if (!string.IsNullOrEmpty(hxml) && hxml != "404")
            {
                var doc = new XmlDocument();
                doc.LoadXml(hxml);
                var root = doc.SelectSingleNode("xml");
                if (root != null)
                {
                    foreach (XmlNode t in root.ChildNodes)
                    {
                        //<r ln="日乙" hteam="长崎成功丸" ateam="千叶市原" mtime="1369540800" hscore="2" ascore="1" bc="0:1" bet="-0.25" binfo="赢" htid="10210" atid="372" cl="6FC809"/>
                        historyList.Add(new JCZQ_Team_History
                        {
                            Ln = t.Attributes["ln"].Value,
                            HTeam = t.Attributes["hteam"].Value,
                            ATeam = t.Attributes["ateam"].Value,
                            MTime = baseTime.AddSeconds(double.Parse(t.Attributes["mtime"].Value)).ToString("yyyy-MM-dd HH:mm:ss"),
                            HScore = t.Attributes["hscore"].Value.GetInt32(),
                            AScore = t.Attributes["ascore"].Value.GetInt32(),
                            Bc = t.Attributes["bc"].Value,
                            Bet = t.Attributes["bet"].Value,
                            BInfo = t.Attributes["binfo"].Value,
                            HTId = t.Attributes["htid"].Value,
                            ATId = t.Attributes["atid"].Value,
                            Cl = t.Attributes["cl"].Value,
                        });
                    }
                }
            }
            return historyList;
        }

        #endregion

        private string GetSPXmlContent(string gameCode)
        {
            var url = string.Empty;
            switch (gameCode)
            {
                case "SPF":
                    url = "http://trade.cpdyj.com/jc/getmatch.go?playId=SPF&ptype=1";
                    break;
                case "BF":
                    url = "http://trade.cpdyj.com/jc/getmatch.go?playId=CBF&ptype=1";
                    break;
                case "ZJQ":
                    url = "http://trade.cpdyj.com/jc/getmatch.go?playId=JQS&ptype=1";
                    break;
                case "BQC":
                    url = "http://trade.cpdyj.com/jc/getmatch.go?playId=BQC&ptype=1";
                    break;
            }
            return PostManager.Get(url, Encoding.GetEncoding("gb2312"), 0, (request) =>
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
        }

        /// <summary>
        /// 各玩法SP
        /// </summary>
        private string GetSPJsonContent(string gameType, bool isDS)
        {
            var url = string.Empty;
            var replaceFlag = string.Empty;
            switch (gameType)
            {
                case "SPF":
                    if (isDS)
                    {
                        url = "http://intf.cpdyj.com/data/jc/spf_fd.js?callback=dyj_jc_spf_fd&_={0}";
                        replaceFlag = "dyj_jc_spf_fd(";
                    }
                    else
                    {
                        url = "http://intf.cpdyj.com/data/jc/spf_gd.js?callback=dyj_jc_spf_gd&_={0}";
                        replaceFlag = "dyj_jc_spf_gd(";
                    }
                    break;
                case "BF":
                    if (isDS)
                    {
                        url = "http://intf.cpdyj.com/data/jc/cbf_fd.js?callback=dyj_jc_cbf_fd&_={0}";
                        replaceFlag = "dyj_jc_cbf_fd(";
                    }
                    else
                    {
                        url = "http://intf.cpdyj.com/data/jc/cbf_gd.js?callback=dyj_jc_cbf_gd&_={0}";
                        replaceFlag = "dyj_jc_cbf_gd(";
                    }
                    break;
                case "ZJQ":
                    if (isDS)
                    {
                        url = "http://intf.cpdyj.com/data/jc/jqs_fd.js?callback=dyj_jc_jqs_fd&_={0}";
                        replaceFlag = "dyj_jc_jqs_fd(";
                    }
                    else
                    {
                        url = "http://intf.cpdyj.com/data/jc/jqs_gd.js?callback=dyj_jc_jqs_gd&_={0}";
                        replaceFlag = "dyj_jc_jqs_gd(";
                    }
                    break;
                case "BQC":
                    if (isDS)
                    {
                        url = "http://intf.cpdyj.com/data/jc/bqc_fd.js?callback=dyj_jc_bqc_fd&_={0}";
                        replaceFlag = "dyj_jc_bqc_fd(";
                    }
                    else
                    {
                        url = "http://intf.cpdyj.com/data/jc/bqc_gd.js?callback=dyj_jc_bqc_gd&_={0}";
                        replaceFlag = "dyj_jc_bqc_gd(";
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
                request.Referer = "http://jc.cpdyj.com/";
                if (ServiceHelper.IsUseProxy("JCZQ"))
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

        /// <summary>
        /// 各玩法SP-中国竞彩网
        /// </summary>
        private string GetSPJsonContent_ZZJCW(string gameType)
        {
            var url = string.Empty;
            var replaceFlag = string.Empty;
            switch (gameType)
            {
                case "SPF":
                    url = "http://i.sporttery.cn/odds_calculator/get_odds?i_format=json&i_callback=getData&poolcode%5B%5D=hhad&poolcode%5B%5D=had&_={0}";
                    replaceFlag = "getData(";
                    break;
                case "BF":
                    url = "http://i.sporttery.cn/odds_calculator/get_odds?i_format=json&i_callback=getData&poolcode%5B%5D=crs&_={0}";
                    replaceFlag = "getData(";
                    break;
                case "ZJQ":
                    url = "http://i.sporttery.cn/odds_calculator/get_odds?i_format=json&i_callback=getData&poolcode%5B%5D=ttg&_={0}";
                    replaceFlag = "getData(";
                    break;
                case "BQC":
                    url = "http://i.sporttery.cn/odds_calculator/get_odds?i_format=json&i_callback=getData&poolcode%5B%5D=hafu&_={0}";
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
            BeStop = 0;
            //if (timer != null)
            //    timer.Stop();
        }

       

        /// <summary>
        /// 采集310win的FXId
        /// </summary>
        private Dictionary<string, string> GetJCZQ_FX()
        {
            string url = @"http://www.310win.com/buy/jingcai.aspx?typeID=101&oddstype=2";
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
            {
                return new Dictionary<string, string>();
            }
            return GetJCZQ_SPF_FX(html);
        }

        private Dictionary<string, string> GetJCZQ_SPF_FX(string html)
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
                    if (!dic.Keys.Contains(match))
                        dic.Add(match, winNumber);
                    continue;
                }
            }
            return dic;
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

        /// <summary>
        /// 采集OKOOO的FXId
        /// </summary>
        private Dictionary<string, string> GetJCZQ_FX_OKOOO()
        {
            //var url = "http://www.okooo.com/jingcai/";
            var url = "http://www.okooo.com/jingcai/shengpingfu/";
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
                        target = "href=\"/soccer/match/";
                        index = td.IndexOf(target);
                        if (index >= 0)
                        {
                            //取FXId
                            temp = td.Substring(index + target.Length);
                            target = "/trends/";
                            index = temp.IndexOf(target);
                            temp = temp.Substring(0, index);
                            fxId = temp;
                            break;
                        }
                    }
                    if (!string.IsNullOrEmpty(matchId) && !string.IsNullOrEmpty(fxId))
                    {
                        if (!dic.ContainsKey(matchId))
                            dic.Add(matchId, fxId);
                    }
                }
            }

            return dic;
        }
    }
}
