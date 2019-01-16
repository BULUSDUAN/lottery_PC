using KaSon.FrameWork.Common.FileOperate;
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
      //public  static JObject ConfigInfo;
        public static JObject AllConfigInfo;
        static ConfigHelper()
        {
            //string path = Path.Combine(Directory.GetCurrentDirectory(), @"Config\ConfigSettings.json");
            //string jsonText = FileHelper.txtReader(path);
            //ConfigInfo = (JObject)JsonConvert.DeserializeObject(jsonText);
            string path = Path.Combine(Directory.GetCurrentDirectory(), @"Config\AllConfig.json");
            string jsonText = FileHelper.txtReader(path);
            AllConfigInfo = (JObject)JsonConvert.DeserializeObject(jsonText);
        }

        /// <summary>
        /// 获取Json 配置
        /// </summary>
        public static void GetServiceAllJsonCfg() {

          
        }


    }
}
