using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.Common
{
  public  class MgHelper
    {

      

        protected static IMongoClient _client;
        protected static IMongoDatabase _database=null;

        public  static IMongoDatabase MgDB {

            get {
                if (_database == null)
                {

                    string ipaddr = ConfigHelper.MongoSettings["IpAddr"].ToString();
                    string Post = ConfigHelper.MongoSettings["Post"].ToString();
                    string dbName = ConfigHelper.MongoSettings["DBName"].ToString();
                    string MaxConnectionPoolSize = ConfigHelper.MongoSettings["MaxConnectionPoolSize"].ToString();
                    string MinConnectionPoolSize = ConfigHelper.MongoSettings["MinConnectionPoolSize"].ToString();

                    MongoClientSettings mongoSetting = new MongoClientSettings();
                    //设置连接超时时间  
                    mongoSetting.ConnectTimeout = new TimeSpan(15 * TimeSpan.TicksPerSecond);
                    mongoSetting.MaxConnectionPoolSize =int.Parse( MaxConnectionPoolSize);
                    mongoSetting.MinConnectionPoolSize = int.Parse(MinConnectionPoolSize);
                    //设置数据库服务器  
                    mongoSetting.Server = new MongoServerAddress(ipaddr, int.Parse(Post));
                    //创建Mongo的客户端  
                    _client  = new MongoClient(mongoSetting);
                    //得到服务器端并且生成数据库实例  
                    _database = _client.GetDatabase(dbName);


                }
                else {
                  
                }
                

                return _database;
            }
        }

         
    }
}
