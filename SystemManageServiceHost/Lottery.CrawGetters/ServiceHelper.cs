using EntityModel;
using EntityModel.Enum;
using EntityModel.Interface;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Net;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lottery.CrawGetters
{
    public class ServiceHelper
    {
        public static JToken MatchSettings { get; set; }
        public static void AddAndSendNotification(string text, string param, string innerKey, NoticeType noticeType)
        {
            var urlArray = GetSystemConfig("Wcf.GameBiz.Core.Many").Split('|');
            foreach (var item in urlArray)
            {
                //var client = new GameBizWcfClient_Core(string.Format("{0}/Wcf_GameBiz_Core.svc", item));
                //client.HandleNotification(text, param, innerKey, noticeType);
            }
        }
        public static int Get_JCZQ_Result_Interval()
        {
            string key = "JCZQ_Result_Interval";
            var value = GetSystemConfig(key);
            if (string.IsNullOrEmpty(value))
                throw new Exception("没有配置 - " + key);
            return int.Parse(value);
        }
        /// <summary>
        /// 获取分析编号来源
        /// </summary>
        public static string GetFXIdSource()
        {
            string key = "FXId_Source";
            return GetSystemConfig(key);
        }

        public static List<KeyValuePair<DBChangeState, T>> BuildNewMatchList<T>(IMongoDatabase mDB,string tablename, List<T> currentList,
            MongoDB.Driver.FilterDefinition<T> mFilter,Func<List<T>, List<T>, List<T>> action,Action<List<T> ,string> action1=null )
              where T : new()
        {
            var result = new List<KeyValuePair<DBChangeState, T>>();
            if (currentList.Count == 0) return result;
            //  var issuseFileFullName = BuildFileFullName(string.Format("Match_{0}_List.json", gameCode), issuseNumber);
            // var customerSavePath = new string[] { "CTZQ", issuseNumber };
            if (action1 != null) action1(currentList, tablename);
            try
            {
                string currentListStr = KaSon.FrameWork.Common.JSON.JsonHelper.Serialize(currentList);

                // string tablename = Lottery.CrawGetters.InitConfigInfo.MongoTableSettings["CTZQMatch"].ToString();IssuseNumber  IssuseNumber
                //   var mFilter = MongoDB.Driver.Builders<CTZQ_MatchInfo>.Filter.Eq(b => b.GameType, GameType) & Builders<CTZQ_MatchInfo>.Filter.Eq(b => b.IssuseNumber, issuseNumber);
                List<T> rn = new List<T>();
                if (mFilter==null)
                {
                    mFilter = Builders<T>.Filter.Empty;
                }

                var coll = mDB.GetCollection<T>(tablename);
                // var count = coll.Find(mFilter).CountDocuments();
                var document = coll.Find(mFilter).ToList<T>();
                

                // BsonDocument one = coll.fincou;
                // var updated = Builders<BsonDocument>.Update.Set("Content", currentListStr);
                if (document.Count > 0)
                {//更新
                 //Thread.Sleep(2000);
                 //  var text = document["Content"].ToString().Trim();//.Replace("var data=", "").Replace("];", "]");
                 //  var oldList = string.IsNullOrEmpty(text) ? new List<CTZQ_MatchInfo>() : KaSon.FrameWork.Common.JSON.JsonHelper.Deserialize<List<CTZQ_MatchInfo>>(text);
                    var newList = action(document, currentList);
                    coll.DeleteMany(mFilter);
                    //UpdateResult uresult = coll.UpdateOne(mFilter, updated);
                    //if (uresult.ModifiedCount > 0)
                    //{
                    //    //成功修改一行以上
                    //}
                    foreach (var item in newList)
                    {
                        result.Add(new KeyValuePair<DBChangeState, T>(DBChangeState.Update, item));
                    }

                }
                else
                {
                    foreach (var item in currentList)
                    {
                        result.Add(new KeyValuePair<DBChangeState, T>(DBChangeState.Add, item));
                    }
                    //BsonDocument bson = new BsonDocument();
                    //bson.Add("GameCode", gameCode);
                    //bson.Add("IssuseNumber", issuseNumber);
                    //bson.Add("Content", currentListStr);

                }
                coll.InsertMany(currentList);
            }
            catch (Exception EX)
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

        public static List<T> Save_And_GetDiffer<T>(IMongoDatabase mDB, string tablename, List<T> currentList,
       MongoDB.Driver.FilterDefinition<T> mFilter, Func<List<T>, List<T>, List<T>> action, Action<List<T>, string> action1 = null)
         where T : new()
        {
            if (currentList.Count == 0)
                return currentList;
            List<T> result = new List<T>();
            //  var issuseFileFullName = BuildFileFullName(string.Format("Match_{0}_List.json", gameCode), issuseNumber);
            // var customerSavePath = new string[] { "CTZQ", issuseNumber };
            if (action1 != null) action1(currentList, tablename);
            try
            {
                string currentListStr = KaSon.FrameWork.Common.JSON.JsonHelper.Serialize(currentList);

                // string tablename = Lottery.CrawGetters.InitConfigInfo.MongoTableSettings["CTZQMatch"].ToString();IssuseNumber  IssuseNumber
                //   var mFilter = MongoDB.Driver.Builders<CTZQ_MatchInfo>.Filter.Eq(b => b.GameType, GameType) & Builders<CTZQ_MatchInfo>.Filter.Eq(b => b.IssuseNumber, issuseNumber);

                var coll = mDB.GetCollection<T>(tablename);
                // var count = coll.Find(mFilter).CountDocuments();
                var document = coll.Find(mFilter).ToList<T>();
                // BsonDocument one = coll.fincou;
                // var updated = Builders<BsonDocument>.Update.Set("Content", currentListStr);
                if (document.Count > 0)
                {//更新
               

                    coll.DeleteMany(mFilter);

                    result = action(document, currentList);

                }
                else
                {
                    result = currentList;


                }
                coll.InsertMany(currentList);
               
            }
            catch (Exception)
            {

                throw;
            }
           
        
            return result;
        }
           public static void Write_Trend_JSON<T>(IMongoDatabase mDB, string tablename, List<T> collection, List<C_BJDC_Match> CurrentMatchList, MongoDB.Driver.FilterDefinition<T> mFilter=null)
            where T: IBJDCBallBaseInfo
        {
            var coll = mDB.GetCollection<T>(tablename);
            foreach (var item in collection)
            {
                try
                {
                   // v
                    //   ServiceHelper.BuildList_GN(mDB, "BJDC_ZJQ_SpInfo", collection, null, mFilter);

                    //var fileName = string.Format("ZJQ_SP_Trend_{0}.json", item.MatchOrderId);
                    //var fileFullName = BuildFileFullName(fileName, item.IssuseNumber);
                    var league = CurrentMatchList.FirstOrDefault(p => p.IssuseNumber == item.IssuseNumber && p.MatchOrderId == item.MatchOrderId);
                    if (league == null || league.MatchState == (int)BJDCMatchState.Stop)
                    {

                    }
                    else
                    {
                      //  var dlist = coll.Find<T>(mFilter).ToList();
                        coll.InsertOne(item);
                    }




                }
                catch (Exception ex)
                {
                    //this.WriteLog(string.Format("写入第{0}期，赛事{1} SP值走势失败:{2}", item.IssuseNumber, item.MatchOrderId, ex.ToString()));
                    continue;
                }
            }
        }
     
        public static string getProperties<T>(T t, string[] propertyNames)
        {
            var list = new List<string>();
            if (t == null)
            {
                return string.Empty;
            }
            System.Reflection.PropertyInfo[] properties = t.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            if (properties.Length <= 0)
            {
                return string.Empty;
            }
            foreach (System.Reflection.PropertyInfo item in properties)
            {
                string name = item.Name; //名称
                if (propertyNames.Contains(name)) continue;

                object value = item.GetValue(t, null);  //值

                if (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String"))
                {
                    list.Add("\"" + name + "\":" + (value == null ? "" : value.ToString()));
                }
            }
            return "{" + string.Join(",", list) + "}";
        }

        /// <summary>
        /// 指定彩种相关采集是否使用代理
        /// </summary>
        public static bool IsUseProxy(string gameCode)
        {
            string key = gameCode + "_UseProxy";
            string value = MatchSettings[key].ToString();
            if (string.IsNullOrEmpty(value))
                return false;
            return bool.Parse(value);
        }
        public static string GetSystemConfig(string paramKey)
        {
            var t = MatchSettings[paramKey];
            if (t==null)
                return string.Empty;
            return t.ToString();
        }
        /// <summary>
        /// 代理地址
        /// </summary>
        public static string GetProxyUrl()
        {
            string key = "ProxyUrl";
            var value = MatchSettings[key].ToString();
            if (string.IsNullOrEmpty(value))
                throw new Exception("没有配置 - " + key);
            return value;
        }
        public static void CreateOrAppend_JSONFile(string fileFullName, string content, Action<string> writeLog)
        {
            try
            {
                content = string.Format("{0}{1}{2}", "var data=", content, ";");
                StreamWriter sw = new StreamWriter(fileFullName, false);
                sw.Write(content);
                sw.Close();
            }
            catch (Exception ex)
            {
                writeLog(ex.ToString());
            }
        }
        ///// <summary>
        ///// 上传文件
        ///// </summary>
        //public static void PostFileToServer(string key,string filePath, string[] customerPath, Action<string> writeLog)
        //{
        //    var urlArray = key.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
        //    foreach (var item in urlArray)
        //    {
        //        var currentUrl = string.Format("{0}/CoreNotice/ReviceFile", item);
        //        PostFileToServer(currentUrl, filePath, customerPath, 0, 3, writeLog);
        //    }
        //}

        private static void PostFileToServer(string url, string filePath, string[] customerPath, int currentTimes, int maxTimes, Action<string> writeLog)
        {
            try
            {
                if (currentTimes >= maxTimes)
                    return;
                currentTimes++;

                var file = new FileInfo(filePath);
                if (file == null)
                    throw new Exception("文件对象为空");
                if (!file.Exists)
                    throw new Exception(string.Format("文件{0}不存在", filePath));

                var dic = new Dictionary<string, string>();
                //路径以|分隔，接收端拆分
                dic.Add("CustomerFilePath", string.Join("|", customerPath));
                dic.Add("CustomerFileName", file.Name);
                dic.Add("Sign", Encipherment.MD5(string.Format("XT{0}{1}", file.Name, string.Join("|", customerPath)), Encoding.UTF8));
                var r = PostManager.UploadFile(url, filePath, dic);
                if (r != "1")
                    throw new Exception(string.Format("第{0}次上传文件失败", currentTimes));
                writeLog(string.Format("第{0}次上传文件成功", currentTimes));
            }
            catch (Exception ex)
            {
                writeLog(ex.ToString());
                PostFileToServer(url, filePath, customerPath, currentTimes, maxTimes, writeLog);
            }
        }

    }
}
