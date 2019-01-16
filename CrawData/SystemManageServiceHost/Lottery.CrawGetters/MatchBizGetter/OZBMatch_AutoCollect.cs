using EntityModel.CoreModel;
using EntityModel.Enum;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.JSON;
using KaSon.FrameWork.Common.Net;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.CrawGetters.MatchBizGetter
{
    
    /// <summary>
    /// 欧洲杯
    /// </summary>
    public class OZBMatch_AutoCollect 
    {
      
        private const string logCategory = "Services.Info";
        private string logInfoSource = "Auto_Collect_OZBMatch_Info_";
        private const string logErrorCategory = "Services.Error";
        private const string logErrorSource = "Auto_Collect_OZBMatch_Error";
        private string SavePath = string.Empty;

        private bool BeStop = true;
        private System.Timers.Timer timer = null;
        private IMongoDatabase mDB;
        public OZBMatch_AutoCollect(IMongoDatabase _mDB)
        {
            mDB = _mDB;
        }

        public void DoWork()
        {
            //取期号
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long tt = (DateTime.Now.Ticks - startTime.Ticks) / 10000;
            var url = string.Format("http://i.sporttery.cn/rank_calculator/get_list?tid[]=77535&&pcode[]=chp&pcode[]=fnl&i_callback=getDataCallBack&_={0}", tt);
            var encoding = Encoding.GetEncoding("gb2312");
            var content = PostManager.Get(url, encoding, 0);
            content = content.Replace("getDataCallBack({\"data\":", "").Replace(");", "").Replace("}]}", "}]").Trim();

            var Ozb_GJList = KaSon.FrameWork.Common.JSON.JsonHelper.Deserialize<List<CupInfo>>(content);

            var ozb_GJMatchList = new List<CupGJMatchInfo>();
            var ozb_GYJMatchList = new List<CupGYJMatchInfo>();
            if (Ozb_GJList.Count < 2)
                return;
            ozb_GJMatchList = Get_OZB_GJMatchList(Ozb_GJList[0]);
            ozb_GYJMatchList = Get_OZB_GYJMatchList(Ozb_GJList[1]);

            var newOzb_GJ_List = GetNewSJBList<CupGJMatchInfo>(ozb_GJMatchList, "OZB_GJ");
            var newOzb_GYJ_List = GetNewSJBList<CupGYJMatchInfo>(ozb_GYJMatchList, "OZB_GYJ");

            #region 发送 欧洲杯冠军数据通知

            this.WriteLog("1、开始=>发送欧洲杯冠军数据通知");
            var addCupGJMatchList = new List<CupGJMatchInfo>();
            var updateCupGJMatchList = new List<CupGJMatchInfo>();
            foreach (var r in newOzb_GJ_List)
            {
                try
                {
                    if (r.Key == DBChangeState.Add)
                    {
                        addCupGJMatchList.Add(r.Value);
                    }
                    else
                    {
                        updateCupGJMatchList.Add(r.Value);
                    }
                }
                catch (Exception ex)
                {
                    this.WriteLog(string.Format("向数据库写入 竞彩欧洲杯冠军 数据异常 编号：{0}，异常：{1}", r.Value.MatchId, ex.ToString()));
                }
            }
            if (addCupGJMatchList.Count > 0)
            {
                var category = (int)NoticeType.JCOZB_GJ;
                var state = (int)DBChangeState.Add;
                var param = string.Join("_", (from l in addCupGJMatchList select l.MatchId).ToArray());
                var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                //发送 竞彩足球队伍 添加 通知
                var innerKey = string.Format("{0}_{1}", "OZB_GJ", "Add");
                ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.JCOZB_GJ);

                //ServiceHelper.SendNotice(issuse_Request, (log) =>
                //{
                //    this.WriteLog(log);
                //});
            }
            if (updateCupGJMatchList.Count > 0)
            {
                var category = (int)NoticeType.JCOZB_GJ;
                var state = (int)DBChangeState.Update;
                var param = string.Join("_", (from l in updateCupGJMatchList select l.MatchId).ToArray());
                var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                //发送 竞彩足球队伍 修改 通知
                var innerKey = string.Format("{0}_{1}", "OZB_GJ", "Update");
                ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.JCOZB_GJ);

                //ServiceHelper.SendNotice(issuse_Request, (log) =>
                //{
                //    this.WriteLog(log);
                //});
            }

            this.WriteLog("1、发送欧洲杯冠军数据通知 完成");

            #endregion

            #region 发送 欧洲杯冠军数据通知

            this.WriteLog("2、开始=>发送欧洲杯冠亚军数据通知");
            var addCupGYJMatchList = new List<CupGYJMatchInfo>();
            var updateCupGYJMatchList = new List<CupGYJMatchInfo>();
            foreach (var r in newOzb_GYJ_List)
            {
                try
                {
                    if (r.Key == DBChangeState.Add)
                    {
                        addCupGYJMatchList.Add(r.Value);
                    }
                    else
                    {
                        updateCupGYJMatchList.Add(r.Value);
                    }
                }
                catch (Exception ex)
                {
                    this.WriteLog(string.Format("向数据库写入 竞彩欧洲杯冠军 数据异常 编号：{0}，异常：{1}", r.Value.MatchId, ex.ToString()));
                }
            }
            if (addCupGYJMatchList.Count > 0)
            {
                var category = (int)NoticeType.JCOZB_GYJ;
                var state = (int)DBChangeState.Add;
                var param = string.Join("_", (from l in addCupGYJMatchList select l.MatchId).ToArray());
                var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                //发送 竞彩足球队伍 添加 通知
                var innerKey = string.Format("{0}_{1}", "OZB_GYJ", "Add");
                ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.JCOZB_GYJ);

                //ServiceHelper.SendNotice(issuse_Request, (log) =>
                //{
                //    this.WriteLog(log);
                //});
            }
            if (updateCupGYJMatchList.Count > 0)
            {
                var category = (int)NoticeType.JCOZB_GYJ;
                var state = (int)DBChangeState.Update;
                var param = string.Join("_", (from l in updateCupGYJMatchList select l.MatchId).ToArray());
                var sign = Encipherment.MD5(string.Format("IIIOO{0}{1}{2}", category, state, param));
                var issuse_Request = string.Format("NoticeCategory={0}&ChangeState={1}&Param={2}&Sign={3}", category, state, param, sign);

                //发送 竞彩足球队伍 修改 通知
                var innerKey = string.Format("{0}_{1}", "OZB_GYJ", "Update");
                ServiceHelper.AddAndSendNotification(param, "", innerKey, NoticeType.JCOZB_GYJ);

                //ServiceHelper.SendNotice(issuse_Request, (log) =>
                //{
                //    this.WriteLog(log);
                //});
            }

            this.WriteLog("2、发送欧洲杯冠亚军数据通知 完成");

            #endregion
        }

        private List<CupGJMatchInfo> Get_OZB_GJMatchList(CupInfo worldCupInfo)
        {
            var list = new List<CupGJMatchInfo>();
            //foreach (var item in worldCupInfo)
            //{
            var GJmatch = worldCupInfo.data;
            var GJarray = GJmatch.Split('|');//.ToArray();
            foreach (var arr in GJarray)
            {
                var arrlist = arr.Split('-');
                list.Add(new CupGJMatchInfo
                {
                    MatchId = arrlist[0].PadLeft(2, '0'),
                    Team = arrlist[1].ToString(),
                    BetState = arrlist[2].ToString(),
                    BonusMoney = decimal.Parse(arrlist[3]),
                    SupportRate = decimal.Parse(arrlist[4].Replace("%", "").ToString()),
                    Probadbility = decimal.Parse(arrlist[5].Replace("%", "").ToString()),
                    IssuseNumber = DateTime.Now.Year.ToString(),
                    GameType = "GJ",
                    GameCode = "OZB",
                });
            }
            //}
            return list;
        }
        private List<CupGYJMatchInfo> Get_OZB_GYJMatchList(CupInfo worldCupInfo)
        {
            var list = new List<CupGYJMatchInfo>();
            //foreach (var item in worldCupInfo)
            //{
            var GYJmatch = worldCupInfo.data;
            var GYJarray = GYJmatch.Split('|');//.ToArray();
            foreach (var arr in GYJarray)
            {
                var arrlist = arr.Split('-');
                list.Add(new CupGYJMatchInfo
                {
                    MatchId = arrlist[0].PadLeft(2, '0'),
                    Team = arrlist[1].ToString(),
                    BetState = arrlist[2].ToString(),
                    BonusMoney = decimal.Parse(arrlist[3]),
                    SupportRate = decimal.Parse(arrlist[4].Replace("%", "").ToString()),
                    Probadbility = decimal.Parse(arrlist[5].Replace("%", "").ToString()),
                    IssuseNumber = DateTime.Now.Year.ToString(),
                    GameType = "GYJ",
                    GameCode = "OZB",
                });
            }
            //}
            return list;
        }
        ///// <summary>
        ///// 创建文件全路径
        ///// </summary>
        //private string BuildFileFullName(string fileName)
        //{
        //    if (string.IsNullOrEmpty(SavePath))
        //        SavePath = ServiceHelper.Get_OZB_SavePath();
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
        private List<KeyValuePair<DBChangeState, T>> GetNewSJBList<T>(List<T> currentList, string tableName)
            where T : Cup_Base
        {
            var result = new List<KeyValuePair<DBChangeState, T>>();
            if (currentList.Count == 0)
                return result;

            //SaveHistory<T>(currentList, fileName);

            var coll = mDB.GetCollection<T>(tableName);


            var mlist = coll.Find<T>(null).ToList();
            if (mlist.Count>0)
            {
                var newList = CompareNewJCZQList(currentList, mlist);
                foreach (var item in newList)
                {
                    result.Add(new KeyValuePair<DBChangeState, T>(DBChangeState.Update, item));
                }
                // return result;
                coll.DeleteMany(null);
            }
            else
            {
                foreach (var item in currentList)
                {
                    result.Add(new KeyValuePair<DBChangeState, T>(DBChangeState.Add, item));
                }
            }

            //return result;
            //coll.DeleteMany(null);
            coll.InsertMany(mlist);

            //var fileFullName = BuildFileFullName(fileName);
            //var customerSavePath = new string[] { "OZB" };
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

            //    var newList = CompareNewJCZQList(currentList, oldList);
            //    foreach (var item in newList)
            //    {
            //        result.Add(new KeyValuePair<DBChangeState, T>(DBChangeState.Update, item));
            //    }
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
            //    this.WriteLog(string.Format("第一次写入欧洲杯冠军数据文件 {0} 失败：{1}", fileFullName, ex.ToString()));
            //}
            //foreach (var item in currentList)
            //{
            //    result.Add(new KeyValuePair<DBChangeState, T>(DBChangeState.Add, item));
            //}
            return result;
        }

        public void WriteError(string log)
        {
            //if (_logWriter != null)
            //    _logWriter.Write(logErrorCategory, logErrorSource, LogType.Error, "自动采集竞彩欧洲杯数据", log);
        }

        public void WriteLog(string log)
        {
            //if (_logWriter != null)
            //    _logWriter.Write(logCategory, logInfoSource, LogType.Information, "自动采集竞彩欧洲杯数据", log);
        }
        private List<T> CompareNewJCZQList<T>(List<T> newList, List<T> oldList)
            where T : Cup_Base
        {
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
        public void Stop()
        {
            BeStop = true;
            if (timer != null)
                timer.Stop();
        }
    }
}
