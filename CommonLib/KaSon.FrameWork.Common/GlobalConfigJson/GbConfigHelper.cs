using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KaSon.FrameWork.Common.GlobalConfigJson
{
    public class GbConfigHelper
    {
        public static JObject GlobalConfig = new JObject();
        static GbConfigHelper()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), @"GlobalConfig\GlobalConfig.json");
            string jsonText = FileHelper.txtReader(path);
              jsonText = FileHelper.txtReader(path);

            GlobalConfig = (JObject)JsonConvert.DeserializeObject(jsonText); //'System.Data.SqlClient
        }
    }
}
