using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KaSon.FrameWork.Common
{
    /// <summary>
    /// 配置文件帮助类
    /// </summary>
    public static class ConfigHelper
    {
      public  static JObject ConfigInfo;
        static ConfigHelper()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), @"Config\ConfigSettings.json");
            string jsonText = FileHelper.txtReader(path);
            ConfigInfo = (JObject)JsonConvert.DeserializeObject(jsonText);
        }



    }
}
