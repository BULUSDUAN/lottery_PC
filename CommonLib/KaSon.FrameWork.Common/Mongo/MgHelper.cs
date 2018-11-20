using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.Common
{
  public  class MgHelper
    {

        /// 数据库连接
        /// </summary>
        private const string connUrl = "mongodb://127.0.0.1:27017";
        /// <summary>
        /// 指定的数据库
        /// </summary>
        private const string dbName = "testdb";

        protected static IMongoClient _client;
        protected static IMongoDatabase _database;

        public  static IMongoDatabase MgDB {

            get {
                return _database;
            }
        }

         
    }
}
