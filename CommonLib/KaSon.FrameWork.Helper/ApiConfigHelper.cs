using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KaSon.FrameWork.Helper
{
    /// <summary>
    /// 配置文件帮助类
    /// </summary>
    public static class ApiConfigHelper
    {
        static JObject ConfigInfo;
        static ApiConfigHelper()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), @"Config\ConfigSettings.json");
            string jsonText = FileHelper.txtReader(path);
            ConfigInfo = (JObject)JsonConvert.DeserializeObject(jsonText);
        }
    }
}
