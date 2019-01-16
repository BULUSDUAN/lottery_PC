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
using KaSon.FrameWork.ORM.Helper.WinNumber;
using EntityModel.Communication;

namespace Lottery.CrawGetters.MatchBizGetter
{
 
    // <summary>
    /// 采集传统足球赛事数据
    /// </summary>
    public class CTZQPool_AutoCollect : BaseAutoCollect,IAutoCollect
    {
        //  private ILogWriter _logWri,ter = null;
        private const string logCategory = "Services.Info";
        private string logInfoSource = "Auto_Collect_CTZQMatchPool_Info_";
        private const string logErrorCategory = "Services.Error";
        private const string logErrorSource = "Auto_Collect_CTZQMatchPool_Error";

        private long BeStop = 0;
        private System.Timers.Timer timer = null;
        private int CTZQ_advanceMinutes = 0;
        private string SavePath = string.Empty;
        private ILogger<CTZQPool_AutoCollect> _logWriter = null;
      
        public string Category { get; set; }
        public string Key { get; set; }
        private Task thread = null;

        private IMongoDatabase mDB;
        private string gameCode { get; set; }
        private int sleepSecond = 5;
        public CTZQPool_AutoCollect(IMongoDatabase _mDB, string _gameName, int _sleepSecond = 5) : base(_gameName + "Pool", _mDB)
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
                    WriteLogAll();
                    ////TODO：销售期间，暂停采集
                    try
                    {


                        CollectCTZQPoolCore(gameCode);

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

        
     
        public void CollectCTZQPoolCore(string gameCode)
        {
            this.WriteLog("开始采集奖池信息");
            try
            {
                var list = new List<CTZQ_BonusLevelInfo>();
                var maxIssuseCount = 5;
                var resultSource = ServiceHelper.GetSystemConfig("CTZQ_Result_Source");
                if (resultSource == "cpdyj")
                {
                    list = GetBonusLevel_CPDYJ(gameCode, maxIssuseCount);
                }
                if (resultSource == "310win")
                {
                    list = GetBonusLevel_310Win(gameCode, maxIssuseCount);
                }
                if (resultSource == "aicai")
                {
                    list = GetBonusLevel_AiCai(gameCode, maxIssuseCount);
                }

                this.WriteLog("解析数据完成");

                #region 写入数据

                try
                {
                    this.WriteLog("开始导入奖池数据");
                    //var p = new ObjectPersistence(DbAccess_Match_Helper.DbAccess);

                    foreach (var g in list.GroupBy(l => l.IssuseNumber))
                    {
                        var issuseNumber = g.Key;
                        var newList = GetNewBonusLevel(list.Where(t => t.IssuseNumber == issuseNumber).ToList(), gameCode, issuseNumber);
                        var addList = new List<CTZQ_BonusLevelInfo>();
                        var updateList = new List<CTZQ_BonusLevelInfo>();
                        //var updateMatchSql = new List<string>();
                        #region 添加数据
                        foreach (var item in newList)
                        {
                            try
                            {
                                if (item.Key == DBChangeState.Add)
                                {
                                    addList.Add(item.Value);
                                    //p.Add(item.Value);
                                }
                                else
                                {
                                    updateList.Add(item.Value);
                                    //p.Modify(item.Value);
                                }
                                //修改队伍表中的比赛结果
                                var resultArray = item.Value.MatchResult.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                //switch (gameCode)
                                //{
                                //    case "T6BQC":
                                //        //分上下半场 特殊处理
                                //        for (int i = 0; i < 6; i++)
                                //        {
                                //            var id = string.Format("{0}|{1}|{2}|{3}", item.Value.GameCode, item.Value.GameType, item.Value.IssuseNumber, i + 1);
                                //            updateMatchSql.Add(string.Format("update C_CTZQ_Match set MatchResult='{0}' where Id='{1}'", string.Join(",", resultArray[i * 2], resultArray[i * 2 + 1]), id));
                                //        }
                                //        break;
                                //    case "T4CJQ":
                                //        //分上下半场 特殊处理
                                //        for (int i = 0; i < 4; i++)
                                //        {
                                //            var id = string.Format("{0}|{1}|{2}|{3}", item.Value.GameCode, item.Value.GameType, item.Value.IssuseNumber, i + 1);
                                //            updateMatchSql.Add(string.Format("update C_CTZQ_Match set MatchResult='{0}' where Id='{1}'", string.Join(",", resultArray[i * 2], resultArray[i * 2 + 1]), id));
                                //        }
                                //        break;
                                //    default:
                                //        for (int i = 0; i < resultArray.Length; i++)
                                //        {
                                //            var id = string.Format("{0}|{1}|{2}|{3}", item.Value.GameCode, item.Value.GameType, item.Value.IssuseNumber, i + 1);
                                //            updateMatchSql.Add(string.Format("update C_CTZQ_Match set MatchResult='{0}' where Id='{1}'", resultArray[i], id));
                                //        }
                                //        break;
                                //}
                            }
                            catch (Exception ex)
                            {
                                this.WriteLog(string.Format("向数据库写入 传统足球奖池 数据异常 编号：{0}，异常：{1}", item.Value.Id, ex.ToString()));
                            }
                        }
                        #endregion
                        //if (updateMatchSql.Count != 0)
                        //{
                        //    try
                        //    {
                        //        DbAccess_Match_Helper.DbAccess.ExecSQL(string.Join(Environment.NewLine, updateMatchSql.ToArray()));
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        this.WriteLog("更新队伍结果失败：" + ex.ToString());
                        //    }
                        //}
                        if (addList.Count > 0)
                        {
                            var category = (int)NoticeCategory.CTZQ_MatchPoolLevel;
                            var state = (int)DBChangeState.Add;
                            var paramT = gameCode + "^" + issuseNumber + "^" + string.Join("_", (from l in addList select l.Id).ToArray());
                            var param = string.Join("_", (from l in addList select l.Id).ToArray());
                            var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                            var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, paramT, sign);

                            //发送 奖池 添加 通知
                            var innerKey = string.Format("{0}_{1}", "CTZQ_MatchPool", "Add");
                           // ServiceHelper.AddAndSendNotification(param, paramT, innerKey, NoticeType.CTZQ_MatchPool);
                            new Sports_Business(this.mDB).UpdateLocalData(param, paramT, NoticeType.CTZQ_MatchPool, innerKey);
                            try
                            {
                                var winNumber = addList.FirstOrDefault(p => p.IssuseNumber == issuseNumber);
                                if (winNumber != null && winNumber.BonusCount > 0)
                                {
                                    //step 1 导入开奖号
                                    this.WriteLog(string.Format("开始对{0}期导入开奖号码", issuseNumber));
                                    var mssage = this.ImportWinNumber(string.Format("{0}_{1}", "CTZQ", gameCode), issuseNumber, winNumber.MatchResult);
                                    this.WriteLog(string.Format("{0}期导入开奖号结果：{1}", issuseNumber, mssage.Message));
                                }
                            }
                            catch (Exception ex)
                            {
                                this.WriteError(string.Format("{0}期导入开奖号码异常：{1}", issuseNumber, ex.Message));
                            }


                            try
                            {
                                //this.WriteLog("开始生成静态相关数据.");

                                //this.WriteLog("1.生成开奖结果首页");
                                //var log = ServiceHelper.SendBuildStaticFileNotice("301");
                                //this.WriteLog("1.生成开奖结果首页结果：" + log);

                                //this.WriteLog("2.生成彩种开奖历史");
                                //log = ServiceHelper.SendBuildStaticFileNotice("302", "CTZQ");
                                //this.WriteLog("2.生成彩种开奖历史结果：" + log);

                                //this.WriteLog("3.生成彩种开奖详细");
                                //log = ServiceHelper.SendBuildStaticFileNotice("303", "CTZQ");
                                //this.WriteLog("3.生成彩种开奖详细结果：" + log);

                                //this.WriteLog("4.生成网站首页");
                                //log = ServiceHelper.SendBuildStaticFileNotice("10");
                                //this.WriteLog("4.生成网站首页结果：" + log);
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
                        if (updateList.Count > 0)
                        {
                            var category = (int)NoticeCategory.CTZQ_MatchPoolLevel;
                            var state = (int)DBChangeState.Update;
                            var paramT = gameCode + "^" + issuseNumber + "^" + string.Join("_", (from l in updateList select l.Id).ToArray());
                            var param = string.Join("_", (from l in updateList select l.Id).ToArray());
                            var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                            var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, paramT, sign);

                            //发送 奖池 修改 通知
                            var innerKey = string.Format("{0}_{1}", "CTZQ_MatchPool", "Update");
                          //  ServiceHelper.AddAndSendNotification(param, paramT, innerKey, NoticeType.CTZQ_MatchPool);
                            new Sports_Business(this.mDB).UpdateLocalData(param, paramT, NoticeType.CTZQ_MatchPool, innerKey);

                            try
                            {
                                var winNumber = updateList.FirstOrDefault(p => p.IssuseNumber == issuseNumber);
                                if (winNumber != null && winNumber.BonusCount > 0)
                                {
                                    //step 1 导入开奖号
                                    this.WriteLog(string.Format("开始对{0}期导入开奖号码", issuseNumber));
                                    var mssage = this.ImportWinNumber(string.Format("{0}_{1}", "CTZQ", gameCode), issuseNumber, winNumber.MatchResult);
                                    this.WriteLog(string.Format("{0}期导入开奖号结果：{1}", issuseNumber, mssage));
                                }
                            }
                            catch (Exception ex)
                            {
                                this.WriteLog(string.Format("{0}期导入开奖号码异常：{1}", issuseNumber, ex.Message));
                            }


                            try
                            {
                                //this.WriteLog("开始生成静态相关数据.");

                                //this.WriteLog("1.生成开奖结果首页");
                                //var log = ServiceHelper.SendBuildStaticFileNotice("301");
                                //this.WriteLog("1.生成开奖结果首页结果：" + log);

                                //this.WriteLog("2.生成彩种开奖历史");
                                //log = ServiceHelper.SendBuildStaticFileNotice("302", "CTZQ");
                                //this.WriteLog("2.生成彩种开奖历史结果：" + log);

                                //this.WriteLog("3.生成彩种开奖详细");
                                //log = ServiceHelper.SendBuildStaticFileNotice("303", "CTZQ");
                                //this.WriteLog("3.生成彩种开奖详细结果：" + log);

                                //this.WriteLog("4.生成网站首页");
                                //log = ServiceHelper.SendBuildStaticFileNotice("10");
                                //this.WriteLog("4.生成网站首页结果：" + log);
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

                    this.WriteLog("导入奖池数据成功。");
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static object importLocker = new object();
        public CommonActionResult ImportWinNumber(string gameCode, string issuseNumber, string winNumber)
        {
            try
            {
                lock (importLocker)
                {
                    ILotteryDataBusiness biz = null;
                    switch (gameCode)
                    {
                        case "SSQ":
                            biz = new LotteryDataBusiness_SSQ();
                            break;
                        case "DLT":
                            biz = new LotteryDataBusiness_DLT();
                            break;
                        case "FC3D":
                            biz = new LotteryDataBusiness_FC3D();
                            break;
                        case "PL3":
                            biz = new LotteryDataBusiness_PL3();
                            break;
                        case "CQSSC":
                            biz = new LotteryDataBusiness_CQSSC();
                            break;
                        case "JX11X5":
                            biz = new LotteryDataBusiness_JX11X5();
                            break;
                        case "CQ11X5":
                            biz = new LotteryDataBusiness_CQ11X5();
                            break;
                        case "CQKLSF":
                            biz = new LotteryDataBusiness_CQKLSF();
                            break;
                        case "DF6J1":
                            biz = new LotteryDataBusiness_DF6_1();
                            break;
                        case "GD11X5":
                            biz = new LotteryDataBusiness_GD11X5();
                            break;
                        case "GDKLSF":
                            biz = new LotteryDataBusiness_GDKLSF();
                            break;
                        case "HBK3":
                            biz = new LotteryDataBusiness_HBK3();
                            break;
                        case "HC1":
                            biz = new LotteryDataBusiness_HC1();
                            break;
                        case "HD15X5":
                            biz = new LotteryDataBusiness_HD15X5();
                            break;
                        case "HNKLSF":
                            biz = new LotteryDataBusiness_HNKLSF();
                            break;
                        case "JLK3":
                            biz = new LotteryDataBusiness_JLK3();
                            break;
                        case "JSKS":
                            biz = new LotteryDataBusiness_JSK3();
                            break;
                        case "JXSSC":
                            biz = new LotteryDataBusiness_JXSSC();
                            break;
                        case "LN11X5":
                            biz = new LotteryDataBusiness_LN11X5();
                            break;
                        case "PL5":
                            biz = new LotteryDataBusiness_PL5();
                            break;
                        case "QLC":
                            biz = new LotteryDataBusiness_QLC();
                            break;
                        case "QXC":
                            biz = new LotteryDataBusiness_QXC();
                            break;
                        case "SDQYH":
                            biz = new LotteryDataBusiness_SDQYH();
                            break;
                        case "SD11X5":
                            biz = new LotteryDataBusiness_YDJ11();
                            break;
                        case "SDKLPK3":
                            biz = new LotteryDataBusiness_SDKLPK3();
                            break;
                        case "CTZQ_T14C":
                        case "CTZQ_TR9":
                        case "CTZQ_T6BQC":
                        case "CTZQ_T4CJQ":
                            biz = new LotteryDataBusiness_CTZQ(gameCode);
                            break;
                        default:
                            throw new Exception(string.Format("未找到匹配的接口：{0}", gameCode));
                    }
                    biz.ImportWinNumber(issuseNumber, winNumber);
                }
                return new CommonActionResult(true, "导入成功");
            }
            catch (Exception ex)
            {
                return new CommonActionResult(false, "导入失败 " + ex.ToString());
            }
        }

        public List<CTZQ_BonusLevelInfo> GetBonusLevel_CPDYJ(string gameCode, int maxIssuseCount)
        {
            var list = new List<CTZQ_BonusLevelInfo>();
            var url = GetCGMatchUrl(gameCode);
            if (string.IsNullOrEmpty(url))
                throw new Exception(string.Format("{0} 没有指定的url地址", gameCode));

            var xml = PostManager.Get(url, Encoding.GetEncoding("gb2312"), 0, (request) =>
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
            if (string.IsNullOrEmpty(xml))
            {
                this.WriteLog("采集页面返回XML为空");
                return list;
            }

            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var root = doc.SelectSingleNode("xml");
            var index = 0;
            foreach (XmlNode node in root.ChildNodes)
            {
                index++;
                if (index > maxIssuseCount)
                    break;

                var issuseNumber = node.Attributes["lotissue"].Value;
                if (string.IsNullOrEmpty(issuseNumber))
                    break;
                issuseNumber = issuseNumber.Substring(2);
                var level = 1;
                var level_name = "一等奖";
                list.Add(new CTZQ_BonusLevelInfo
                {
                    Id = string.Format("{0}|{1}|{2}|{3}", "CTZQ", gameCode, issuseNumber, level),
                    GameCode = "CTZQ",
                    GameType = gameCode,
                    BonusLevel = level,
                    BonusLevelDisplayName = level_name,
                    IssuseNumber = issuseNumber,
                    BonusCount = node.Attributes["Count1"] == null ? 0 : int.Parse(node.Attributes["Count1"].Value),
                    BonusMoney = node.Attributes["Bonus1"] == null ? 0M : decimal.Parse(node.Attributes["Bonus1"].Value),
                    MatchResult = string.Join(",", node.Attributes["BaseCode"].Value.ToArray()),
                    BonusBalance = decimal.Parse(node.Attributes["BonusBalance"].Value),
                    TotalSaleMoney = decimal.Parse(node.Attributes["sale"].Value),
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                });
                if (gameCode == "T14C")
                {
                    level = 2;
                    level_name = "二等奖";
                    list.Add(new CTZQ_BonusLevelInfo
                    {
                        Id = string.Format("{0}|{1}|{2}|{3}", "CTZQ", gameCode, issuseNumber, level),
                        GameCode = "CTZQ",
                        GameType = gameCode,
                        BonusLevel = level,
                        BonusLevelDisplayName = level_name,
                        IssuseNumber = issuseNumber,
                        BonusCount = node.Attributes["Count2"] == null ? 0 : int.Parse(node.Attributes["Count2"].Value),
                        BonusMoney = node.Attributes["Bonus2"] == null ? 0M : decimal.Parse(node.Attributes["Bonus2"].Value),
                        MatchResult = string.Join(",", node.Attributes["BaseCode"].Value.ToArray()),
                        BonusBalance = decimal.Parse(node.Attributes["BonusBalance"].Value),
                        TotalSaleMoney = decimal.Parse(node.Attributes["sale"].Value),
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    });
                }
            }
            return list;
        }

        public List<CTZQ_BonusLevelInfo> GetBonusLevel_310Win(string gameCode, int maxIssuseCount)
        {
            var list = new List<CTZQ_BonusLevelInfo>();

            //获取期号和对应的310win编号
            var issuseList = GetIssuseNumberFrom310Win(gameCode, maxIssuseCount);
            foreach (var item in issuseList)
            {
                try
                {
                    if (string.IsNullOrEmpty(item.Key) || string.IsNullOrEmpty(item.Value)) continue;
                    var url = Get310WinUrl(gameCode, item.Value);
                    if (string.IsNullOrEmpty(url)) continue;
                    var json = PostManager.Get(url, Encoding.UTF8, 0, (request) =>
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
                    var entity =JsonHelper.Decode(json);
                    //{"IssueNum":"2012143","AwardTime":"2012-11-01","CashInStopTime":"2012-12-31","Result":"13311333333311",
                    //"Table":[{"MatchID":"1","Sclass":"德国杯","MatchTime":"2012-11-01","HomeTeam":"比勒费","HalfScore":"1-1","Score":"2-2","GuestTeam":"勒 沃","Result":"1"},{"MatchID":"2","Sclass":"德国杯","MatchTime":"2012-11-01","HomeTeam":"斯图加","HalfScore":"3-0","Score":"3-0","GuestTeam":"圣保利","Result":"3"},{"MatchID":"3","Sclass":"德国杯","MatchTime":"2012-11-01","HomeTeam":"拜 仁","HalfScore":"1-0","Score":"4-0","GuestTeam":"凯 泽","Result":"3"},{"MatchID":"4","Sclass":"德国杯","MatchTime":"2012-11-01","HomeTeam":"杜塞尔","HalfScore":"0-0","Score":"0-0","GuestTeam":"门 兴","Result":"1"},{"MatchID":"5","Sclass":"德国杯","MatchTime":"2012-11-01","HomeTeam":"汉诺威","HalfScore":"1-1","Score":"1-1","GuestTeam":"德累斯","Result":"1"},{"MatchID":"6","Sclass":"德国杯","MatchTime":"2012-11-01","HomeTeam":"沃尔夫","HalfScore":"0-0","Score":"2-0","GuestTeam":"FS法兰","Result":"3"},{"MatchID":"7","Sclass":"意甲","MatchTime":"2012-11-01","HomeTeam":"切 沃","HalfScore":"0-0","Score":"2-0","GuestTeam":"佩斯卡","Result":"3"},{"MatchID":"8","Sclass":"意甲","MatchTime":"2012-11-01","HomeTeam":"亚特兰","HalfScore":"1-0","Score":"1-0","GuestTeam":"那不勒","Result":"3"},{"MatchID":"9","Sclass":"意甲","MatchTime":"2012-11-01","HomeTeam":"卡利亚","HalfScore":"3-1","Score":"4-2","GuestTeam":"锡耶纳","Result":"3"},{"MatchID":"10","Sclass":"意甲","MatchTime":"2012-11-01","HomeTeam":"帕尔马","HalfScore":"2-1","Score":"3-2","GuestTeam":"罗 马","Result":"3"},{"MatchID":"11","Sclass":"意甲","MatchTime":"2012-11-01","HomeTeam":"国 米","HalfScore":"0-1","Score":"3-2","GuestTeam":"桑普多","Result":"3"},{"MatchID":"12","Sclass":"意甲","MatchTime":"2012-11-01","HomeTeam":"尤 文","HalfScore":"0-0","Score":"2-1","GuestTeam":"博洛尼","Result":"3"},{"MatchID":"13","Sclass":"意甲","MatchTime":"2012-11-01","HomeTeam":"拉齐奥","HalfScore":"0-1","Score":"1-1","GuestTeam":"都 灵","Result":"1"},{"MatchID":"14","Sclass":"意甲","MatchTime":"2012-11-01","HomeTeam":"乌迪内","HalfScore":"1-0","Score":"2-2","GuestTeam":"卡塔尼","Result":"1"}],
                    //"Bonus":[{"Grade":"一等奖","BasicStakes":"22","BasicBonus":"<span class='mm'>498,613</span>"},
                    //{"Grade":"二等奖","BasicStakes":"662","BasicBonus":"<span class='mm'>7,101</span>"},
                    //{"Grade":"任九场","BasicStakes":"6890","BasicBonus":"<span class='mm'>1,478</span>"}],
                    //"Bottom":"14场本期销售：<span class='mm'>24,485,506元 </span> ，任九本期销售：<span class='mm'>15,914,288元</span> ，14场奖池：<span class='mm'>0元</span>"}

                    var level = 1;
                    var level_name = "一等奖";
                    string issueNum = entity.IssueNum;
                    var issuseNumber = issueNum.Substring(2);
                    var winNumber = (string)entity.Result;
                    var totalSaleMoney = 0M;
                    var bonusBalance = 0M;
                    string Bottom = entity.Bottom;
                   var bottomArray = Bottom.Split(new string[] { "，" }, StringSplitOptions.RemoveEmptyEntries);
                    switch (gameCode)
                    {
                        case "T14C":
                            if (bottomArray.Length != 3) break;
                            totalSaleMoney = FormatBonusMoney(bottomArray[0]);
                            bonusBalance = FormatBonusMoney(bottomArray[2]);
                            break;
                        case "TR9":
                            if (bottomArray.Length != 3) break;
                            totalSaleMoney = FormatBonusMoney(bottomArray[1]);
                            break;
                        case "T6BQC":
                        case "T4CJQ":
                            if (bottomArray.Length != 2) break;
                            totalSaleMoney = FormatBonusMoney(bottomArray[0]);
                            bonusBalance = FormatBonusMoney(bottomArray[1]);
                            break;
                        default:
                            break;
                    }


                    foreach (var b in entity.Bonus)
                    {
                        if (gameCode == "TR9")
                        {
                            if (b.Grade == "任九场")
                            {
                                 
                                list.Add(new CTZQ_BonusLevelInfo
                                {
                                    GameCode = "CTZQ",
                                    GameType = gameCode,
                                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    MatchResult = string.Join(",", winNumber.ToArray()),
                                    Id = string.Format("{0}|{1}|{2}|{3}", "CTZQ", gameCode, issuseNumber, level),
                                    IssuseNumber = issuseNumber,
                                    BonusLevel = level,
                                    BonusLevelDisplayName = level_name,
                                    BonusCount = FormatBonusCount(b.BasicStakes.Value),
                                    BonusMoney = FormatBonusMoney(b.BasicBonus.Value),
                                    TotalSaleMoney = totalSaleMoney,
                                    BonusBalance = bonusBalance,
                                });
                                break;
                            }
                            continue;
                        }
                        if (b.Grade == "一等奖")
                        {
                            level = 1;
                            level_name = "一等奖";
                        }
                        else
                        {
                            level = 2;
                            level_name = "二等奖";
                        }

                        list.Add(new CTZQ_BonusLevelInfo
                        {
                            GameCode = "CTZQ",
                            GameType = gameCode,
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            MatchResult = string.Join(",", winNumber.ToArray()),
                            Id = string.Format("{0}|{1}|{2}|{3}", "CTZQ", gameCode, issuseNumber, level),
                            IssuseNumber = issuseNumber,
                            BonusLevel = level,
                            BonusLevelDisplayName = level_name,
                            BonusCount = FormatBonusCount((string)b.BasicStakes.Value),
                            BonusMoney = FormatBonusMoney((string)b.BasicBonus.Value),
                            TotalSaleMoney = totalSaleMoney,
                            BonusBalance = bonusBalance,
                        });
                        if (gameCode != "T14C") break;
                        if (level == 2) break;
                    }

                }
                catch (Exception ex)
                {
                    this.WriteLog(ex.ToString());
                }
            }
            return list;
        }

        public List<CTZQ_BonusLevelInfo> GetBonusLevel_AiCai(string gameCode, int maxIssuseCount)
        {
            var list = new List<CTZQ_BonusLevelInfo>();

            var baseUrl = "http://kaijiang.aicai.com/allopenprized/historyprizedetail/arenicHistory/{0}/{1}.html";
            var url = string.Empty;
            int levelCount = 1;
            switch (gameCode.ToUpper())
            {
                case "T14C":
                    url = string.Format(baseUrl, "401", "-1");
                    levelCount = 2;
                    break;
                case "TR9":
                    url = string.Format(baseUrl, "402", "-1");
                    break;
                case "T4CJQ":
                    url = string.Format(baseUrl, "403", "-1");
                    break;
                case "T6BQC":
                    url = string.Format(baseUrl, "404", "-1");
                    break;
                default:
                    break;
            }
            if (string.IsNullOrEmpty(url))
                return list;

            var baseContent = PostManager.Get(url, Encoding.UTF8, 0, (request) =>
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
            if (baseContent == "404")
                return list;
            var baseJson = JsonHelper.Decode(baseContent);
            foreach (var item in baseJson.result.issueList)
            {
                //Console.WriteLine(item[0] + "-" + item[1]);
                if (list.Count >= maxIssuseCount * levelCount)
                    break;
                var currentUrl = string.Empty;
                currentUrl = string.Format("http://kaijiang.aicai.com/allopenprized/historyprizedetail/arenicHistory/401/{0}.html", item[0]);
                switch (gameCode.ToUpper())
                {
                    case "T14C":
                        currentUrl = string.Format(baseUrl, "401", item[0]);
                        break;
                    case "TR9":
                        currentUrl = string.Format(baseUrl, "402", item[0]);
                        break;
                    case "T4CJQ":
                        currentUrl = string.Format(baseUrl, "403", item[0]);
                        break;
                    case "T6BQC":
                        currentUrl = string.Format(baseUrl, "404", item[0]);
                        break;
                    default:
                        break;
                }
                if (string.IsNullOrEmpty(currentUrl))
                    continue;

                var content = PostManager.Get(currentUrl, Encoding.UTF8, 0, (request) =>
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
                if (content == "404")
                    continue;
                var json = JsonHelper.Decode(content);
                string pool = json.result.pool;
                string sales = json.result.sales;
                string issue = json.result.lotteryIssue;
                string his = json.result.historyCodes;
                his = his.Substring(his.IndexOf("<strong class='red'>")).Replace("</td></tr></tfoot>", "");
                var codeList = new List<string>();
                var codeArray = his.Split(new string[] { "</td><td>" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var code in codeArray)
                {
                    var t = code.Replace("<strong class='red'>", "").Replace("</strong>", "");
                    codeList.Add(t);
                }
                Func<string, decimal> formartMoney = (str) =>
                {
                    return decimal.Parse(str.Replace("￥", "").Replace(",", ""));
                };
                foreach (var p in json.result.prize)
                {
                    string money = p[0];
                    int bonusCount = int.Parse(p[1]);
                    string levelName = p[3];
                    var level = 0;
                    switch (levelName)
                    {
                        case "一等奖":
                            level = 1;
                            break;
                        case "二等奖":
                            level = 2;
                            break;
                        default:
                            break;
                    }
                    if (level == 0)
                        break;
                    list.Add(new CTZQ_BonusLevelInfo
                    {
                        Id = string.Format("{0}|{1}|{2}|{3}", "CTZQ", gameCode, issue, level),
                        GameCode = "CTZQ",
                        GameType = gameCode,
                        BonusLevel = level,
                        BonusLevelDisplayName = levelName,
                        IssuseNumber = issue,
                        BonusCount = bonusCount,
                        BonusMoney = formartMoney(money),
                        MatchResult = string.Join(",", codeList.ToArray()),
                        BonusBalance = formartMoney(pool),
                        TotalSaleMoney = formartMoney(sales),
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    });
                }
            }

            return list;
        }

        private int FormatBonusCount(string content)
        {
            var money = content.Replace("--", "").Replace(",", "");
            if (string.IsNullOrEmpty(money)) return 0;
            int retult = 0;
            try
            {
                retult = int.Parse(money);
            }
            catch (Exception ex)
            {
                Console.WriteLine("FormatBonusCount:" + ex.ToString());
                Console.WriteLine("FormatBonusCount:"+ content);
              //  throw;
            }
            return retult;
        }
        private decimal FormatBonusMoney(string content)
        {
            //<span class='mm'>7,101</span>
            var index = content.IndexOf(">");
            var money = content.Substring(index + 1).Replace(",", "").Replace("</span>", "").Replace("元", "").Replace("--", "").Trim(); ;
            if (string.IsNullOrEmpty(money) || money == "无人中奖") return 0M;
            return decimal.Parse(money);
        }

        private Dictionary<string, string> GetIssuseNumberFrom310Win(string gameType, int maxIssuseCount)
        {
            var list = new Dictionary<string, string>();
            var url = string.Empty;
            switch (gameType)
            {
                case "T14C":
                case "TR9":
                    url = "http://www.310win.com/zucai/14changshengfucai/kaijiang_zc_1.html";
                    break;
                case "T6BQC":
                    url = "http://www.310win.com/zucai/6changbanquanchang/kaijiang_zc_3.html";
                    break;
                case "T4CJQ":
                    url = "http://www.310win.com/zucai/4changjinqiucai/kaijiang_zc_4.html";
                    break;
                default:
                    break;
            }
            if (string.IsNullOrEmpty(url))
                return list;
            var html = PostManager.Get(url, Encoding.UTF8, 0, (request) =>
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
            var startStr = "<select name=\"dropIssueNum\" id=\"dropIssueNum\" onchange=\"LoadPrize()\">";
            var index = html.IndexOf(startStr);
            html = html.Substring(index);
            index = html.IndexOf("</select>");
            html = html.Substring(0, index).Replace(startStr, "");
            foreach (var option in html.Split(new string[] { "\r\n\t", "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                //<option selected=\"selected\" value=\"351853\">2012174</option>
                var tempArray = option.Split(new string[] { ">" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempArray.Length != 2) continue;
                var key = tempArray[0];
                index = key.LastIndexOf("=");
                key = key.Substring(index + 1, key.Length - index - 1).Replace("\"", "");

                var issuseNumber = tempArray[1].Replace("</option", "");
                list.Add(issuseNumber, key);
                if (list.Count >= maxIssuseCount)
                    break;
            }
            return list;
        }

        private string Get310WinUrl(string gameType, string key)
        {
            switch (gameType)
            {
                case "T14C":
                case "TR9":
                    return string.Format("http://www.310win.com/Info/Result/Soccer.aspx?load=ajax&typeID=1&IssueID={0}&randomT-_-=0.11246344269790631", key);
                case "T6BQC":
                    return string.Format("http://www.310win.com/Info/Result/Soccer.aspx?load=ajax&typeID=3&IssueID={0}&randomT-_-=0.9525663779635821", key);
                case "T4CJQ":
                    return string.Format("http://www.310win.com/Info/Result/Soccer.aspx?load=ajax&typeID=4&IssueID={0}&randomT-_-=0.6765524493303026", key);
                default:
                    break;
            }
            return string.Empty;
        }

        private void GetMatchPool_500wan()
        {

            #region 原500W采集老代码
            //var response = PostManager.Get(url, Encoding.GetEncoding("gb2312")).ToLower();
            //if (response == "404")
            //{
            //    this.WriteLog("采集页面返回404错误");
            //    return;
            //}
            //this.WriteLog("已获取到数据，开始解析 ");
            //var divIndex = response.IndexOf("<div class=\"nr\">");
            //var divText = response.Substring(divIndex);
            //var endDivIndex = divText.IndexOf("<div class=\"kong10\"></div>");
            //divText = divText.Substring(0, endDivIndex);

            //var table_1 = divText.Substring(0, divText.IndexOf("</table>"));
            //table_1 = table_1.Substring(table_1.IndexOf("<div class=\"zj_jieguo\">"));
            //table_1 = table_1.Substring(0, table_1.IndexOf("</div>"));
            //table_1 = table_1.Substring(table_1.IndexOf(">") + 1);
            ////比赛结果
            //var matchArray = table_1.Trim().Split('-');
            //this.WriteLog("比赛结果 ：" + string.Join(",", matchArray));

            //divText = divText.Substring(divText.IndexOf("</table>"));
            //divText = divText.Substring(divText.IndexOf("<span class=\"span_left\">"));
            //var issuseSpan = divText.Substring(0, divText.IndexOf("</span>"));
            //issuseSpan = issuseSpan.Substring(issuseSpan.IndexOf(">") + 1).Trim();
            ////期号
            ////var issuseNumber = string.Format("{0}-{1}", DateTime.Now.ToString("yyyy"), issuseSpan.Substring(2, 3));
            //var issuseNumber = issuseSpan.Substring(0, 5);
            //this.WriteLog("期号： " + issuseNumber);
            //divText = divText.Substring(divText.IndexOf("<tr>") + 4, divText.LastIndexOf("</tr>") - divText.IndexOf("<tr>") - 4);

            //while (divText.IndexOf("<tr>") > 0)
            //{
            //    divText = divText.Substring(divText.IndexOf("<tr>"));
            //    divText = divText.Substring(divText.IndexOf("<td"));
            //    //奖级名称
            //    var level_name = divText.Substring(divText.IndexOf(">") + 1, divText.IndexOf("</td>") - divText.IndexOf(">") - 1);
            //    var level = 1;
            //    switch (level_name)
            //    {
            //        case "一等奖":
            //            level = 1;
            //            break;
            //        case "二等奖":
            //            level = 2;
            //            break;
            //    }
            //    divText = divText.Substring(divText.IndexOf("</td>"));
            //    divText = divText.Substring(divText.IndexOf("<td"));
            //    //中奖注数
            //    var level_count = divText.Substring(divText.IndexOf(">") + 1, divText.IndexOf("</td>") - divText.IndexOf(">") - 1);
            //    this.WriteLog("中奖注数: " + level_count);
            //    divText = divText.Substring(divText.IndexOf("</td>"));
            //    divText = divText.Substring(divText.IndexOf("<td"));
            //    //单注奖金
            //    var level_Money = divText.Substring(divText.IndexOf(">") + 1, divText.IndexOf("</td>") - divText.IndexOf(">") - 1);
            //    this.WriteLog("单注奖金: " + level_Money);

            //    divText = divText.Substring(divText.IndexOf("</td>"));
            //    divText = divText.Substring(divText.IndexOf("<td"));
            //    //总奖金
            //    var level_TotalMoney = divText.Substring(divText.IndexOf(">") + 1, divText.IndexOf("</td>") - divText.IndexOf(">") - 1);
            //    this.WriteLog("总奖金: " + level_TotalMoney);

            //    list.Add(new CTZQ_BonusLevel
            //    {
            //        Id = string.Format("{0}|{1}|{2}|{3}", "CTZQ", gameCode, issuseNumber, level),
            //        GameCode = "CTZQ",
            //        GameType = gameCode,
            //        BonusLevelDisplayName = level_name,
            //        BonusCount = string.IsNullOrEmpty(level_count) ? 0 : int.Parse(level_count),
            //        BonusMoney = string.IsNullOrEmpty(level_Money) ? 0 : int.Parse(level_Money),
            //        BonusLevel = level,
            //        IssuseNumber = issuseNumber,
            //        MatchResult = string.Join(",", matchArray),
            //        CreateTime = DateTime.Now,
            //    });
            //} 
            #endregion

        }

        ///// <summary>
        ///// 创建文件全路径
        ///// </summary>
        //private string BuildFileFullName(string fileName, string issuseNumber)
        //{
        //    if (string.IsNullOrEmpty(SavePath))
        //        SavePath = ServiceHelper.Get_CTZQ_SavePath();
        //    var path = Path.Combine(SavePath, issuseNumber);
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

        private List<KeyValuePair<DBChangeState, CTZQ_BonusLevelInfo>> GetNewBonusLevel(List<CTZQ_BonusLevelInfo> list, string GameType, string issuseNumber)
        {
            var result = new List<KeyValuePair<DBChangeState, CTZQ_BonusLevelInfo>>();
            if (list.Count == 0) return result;
         //   string fileName = string.Format("CTZQ_{0}_BonusLevel.json", gameCode);
           // var fileFullName = BuildFileFullName(fileName, issuseNumber);
            var customerSavePath = new string[] { "CTZQ", issuseNumber };
            var mFilter = MongoDB.Driver.Builders<CTZQ_BonusLevelInfo>.Filter.Eq(b => b.GameType, GameType) & Builders<CTZQ_BonusLevelInfo>.Filter.Eq(b => b.IssuseNumber, issuseNumber);
            var coll = mDB.GetCollection<CTZQ_BonusLevelInfo>("CTZQ_BonusPoolInfo");
            try
            {
                var document = coll.Find(mFilter).ToList();
                if (document.Count() > 0)
                {
                    var oldList = document;// document ==null ? new List<C_CTZQ_GameIssuse>() : JSONHelper.Deserialize<List<C_CTZQ_GameIssuse>>(text);
                    var newList = GetNewBonus(oldList, list);
                    coll.DeleteMany(mFilter);
                    foreach (var item in newList)
                    {
                        result.Add(new KeyValuePair<DBChangeState, CTZQ_BonusLevelInfo>(DBChangeState.Update, item));
                    }
                }
                else {
                    foreach (var item in list)
                    {
                        result.Add(new KeyValuePair<DBChangeState, CTZQ_BonusLevelInfo>(DBChangeState.Add, item));
                    }
                }
                coll.InsertMany(list);
            }
            catch (Exception)
            {

                throw;
            }

            //if (File.Exists(fileFullName))
            //{
            //    var text = File.ReadAllText(fileFullName).Trim().Replace("var data=", "").Replace("];", "]");
            //    var oldList = string.IsNullOrEmpty(text) ? new List<CTZQ_BonusLevelInfo>() : JsonSerializer.Deserialize<List<CTZQ_BonusLevelInfo>>(text);
            //    var newList = GetNewBonus(oldList, list);
            //    ServiceHelper.CreateOrAppend_JSONFile(fileFullName, JsonSerializer.Serialize(list), (log) =>
            //    {
            //        this.WriteLog(log);
            //    });
            //    foreach (var item in newList)
            //    {
            //        result.Add(new KeyValuePair<DBChangeState, CTZQ_BonusLevelInfo>(DBChangeState.Update, item));
            //    }

            //    //上传文件
            //    ServiceHelper.PostFileToServer(fileFullName, customerSavePath, (log) =>
            //    {
            //        this.WriteLog(log);
            //    });
            //    return result;
            //}
            //try
            //{
            //    ServiceHelper.CreateOrAppend_JSONFile(fileFullName, JsonSerializer.Serialize(list), (log) =>
            //    {
            //        this.WriteLog(log);
            //    });
            //}
            //catch (Exception ex)
            //{
            //    this.WriteLog(string.Format("第一次写入 {0}  文件失败：{1}", fileFullName, ex.ToString()));
            //}

            ////上传文件
            //ServiceHelper.PostFileToServer(fileFullName, customerSavePath, (log) =>
            //{
            //    this.WriteLog(log);
            //});
            //foreach (var item in list)
            //{
            //    result.Add(new KeyValuePair<DBChangeState, CTZQ_BonusLevelInfo>(DBChangeState.Add, item));
            //}
            return result;
        }

        private List<CTZQ_BonusLevelInfo> GetNewBonus(List<CTZQ_BonusLevelInfo> oldList, List<CTZQ_BonusLevelInfo> newList)
        {
            var list = new List<CTZQ_BonusLevelInfo>();
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

        private string GetCGMatchUrl(string gameCode)
        {
            //switch (gameCode)
            //{
            //    case "T14C":
            //        return "http://www.500wan.com/pages/info/zhongjiang/index.php";
            //    case "TR9":
            //        return "http://www.500wan.com/pages/info/zhongjiang/rj.php";
            //    case "T6BQC":
            //        return "http://www.500wan.com/pages/info/zhongjiang/6cbq.php";
            //    case "T4CJQ":
            //        return "http://www.500wan.com/pages/info/zhongjiang/jq4.php";
            //}
            switch (gameCode)
            {
                case "T14C":
                    return "http://sfc.cpdyj.com/staticdata/lotteryinfo/opencode/11.xml";
                case "TR9":
                    return "http://rj.cpdyj.com/staticdata/lotteryinfo/opencode/19.xml";
                case "T6BQC":
                    return "http://bq.cpdyj.com/staticdata/lotteryinfo/opencode/16.xml";
                case "T4CJQ":
                    return "http://jq.cpdyj.com/staticdata/lotteryinfo/opencode/18.xml";
            }
            return string.Empty;
        }
        public void Stop()
        {
            BeStop = 1;
            if (timer != null)
                timer.Stop();
        }

    

   
    }
}
