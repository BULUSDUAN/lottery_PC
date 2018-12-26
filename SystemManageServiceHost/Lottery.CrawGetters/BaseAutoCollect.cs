using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.CrawGetters
{

   
    [BsonIgnoreExtraElements]
    public class LogModel{
        [BsonId]

        public ObjectId _id { get; set; }
        public string LogLevel { get; set; } = "Info";
        public string Content { get; set; } = "";
        public DateTime CreateTime { get; set; } = DateTime.Now;

    }
   public class BaseAutoCollect
    {
        ILogger<BaseAutoCollect> _Log;
        string tableName;
       
        private IMongoDatabase mDB;
        List<string> list = new List<string>();
        public bool isError = false;
        private bool IsStartLog = true;
        public BaseAutoCollect(string Catetory, IMongoDatabase _mDB)
        {
         string temp=   Lottery.CrawGetters.InitConfigInfo.MongoSettings["IsStartLog"].ToString();
            _Log = InitConfigInfo.logFactory.CreateLogger<BaseAutoCollect>();
            IsStartLog = bool.Parse(temp);
            tableName = "LOG_"+ Catetory + "_LOG";
            mDB = _mDB;
        }
        public  void WriteLog( string log, Exception ex = null)
        {
            //tableName = Catetory + "_LOG";
            Console.WriteLine(log);
            list.Add(log);
            if (ex != null)
            {
                isError = true;
                list.Add(ex.ToString());
            }
            //
            // _Log.LogInformation(log);
            //if (_logWriter != null)
            //    _logWriter.Write(logCategory, logInfoSource, LogType.Information, "自动采集北京单场数据", log);
        }

        public  void WriteError( string log,Exception ex=null)
        {
            //tableName = Catetory + "_LOG";
            Console.WriteLine(log);
    
               isError = true;
            list.Add(log);

            if (ex != null)
            {
                isError = true;
                list.Add(ex.ToString());
            }
            //if (_logWriter != null)
            //    _logWriter.Write(logErrorCategory, logErrorSource, LogType.Error, "自动采集北京单场数据异常", log);
        }
        public void WriteLogAll()
        {
            if (list.Count<=0)
            {
                return;
            }
            Console.WriteLine("采集完成"+ tableName);
            //tableName = Catetory + "_LOG";
            // Console.WriteLine(log);
            list.Add(DateTime.Now.ToShortTimeString());
           string Content = string.Join(Environment.NewLine, list.ToArray()); ;
            this._Log.LogInformation(Content);


            if (!IsStartLog)
            {
                return;
            }
            try
            {
                var coll = mDB.GetCollection<LogModel>(tableName);

                LogModel mlog = new LogModel();
                if (isError)
                {
                    mlog.LogLevel = "Error";
                    isError = false;
                }
                mlog.Content = string.Join(Environment.NewLine, list.ToArray()); ;
                coll.InsertOne(mlog);


                list.Clear();
            }
            catch 
            {

                Console.WriteLine("mogodb 日志存储错误");
            }
         
            //if (_logWriter != null)
            //    _logWriter.Write(logErrorCategory, logErrorSource, LogType.Error, "自动采集北京单场数据异常", log);
        }
    }
}
