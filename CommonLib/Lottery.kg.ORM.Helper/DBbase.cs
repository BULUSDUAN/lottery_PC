using KaSon.FrameWork.Helper;
using KaSon.FrameWork.ORM.Provider;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace Lottery.Kg.ORM.Helper
{
    public class DBbase
    {

        private static readonly string  _DBType= "SqlServer";

        IList<SQLModel> sqlmodel = null;
        private static string jsonText = "";

        static SQLModule sqlModule = null;
        static DBbase() {

           string path = Directory.GetCurrentDirectory();
            if (_DBType == "MySql")
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), @"SQL_JSON\SQLServer_SQL.json");
                jsonText = FileHelper.txtReader(path);
               
            }
            else {
                path = Path.Combine(Directory.GetCurrentDirectory(), @"SQL_JSON\MyServer_SQL.json");
                jsonText = FileHelper.txtReader(path );
            }
            // JObject jo = (JObject)JsonConvert.DeserializeObject(jsonText);'System.Data.SqlClient
            sqlModule = (SQLModule)  JsonHelper.Deserialize<SQLModule>(jsonText);



        }

        private DbProvider db = null;
        public DbProvider DB
        {
            get
            {
                if (db == null)
                {
                    db = new DbProvider();
                    //// db.Init("Default");
                    if (_DBType == "MySql")
                    {
                        db.Init("MySql.Default");
                    }
                    else {
                        db.Init("SqlServer.Default");
                    }
                 

                }
                return db;
            }

        }
        /// <summary>
        /// SQL 语言模块
        /// </summary>
        public SQLModule SqlModule
        {
            get
            {
                if (sqlModule == null)
                {
                    sqlModule = (SQLModule)JsonHelper.Deserialize<SQLModule>(jsonText);
                }
                return sqlModule;
            }

        }



    }
}
