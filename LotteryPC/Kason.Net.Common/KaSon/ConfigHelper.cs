using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kason.Net.Common
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
            //string npath = @"E:\wokerFiler\Net FramWork SG\WebApp\WebApp\Config\AllConfig.json";
            string dic = Directory.GetCurrentDirectory();
#if DEBUG
            dic= @"E:\wokerFiler\Lettery\Lottery_PC\Lottery_02\90.Apps\app.lottery.site\";
#endif

            string path = Path.Combine(dic, @"Config\AllConfig.json");
           // path = @"E:\wokerFiler\Net FramWork SG\WebApp\WebApp\Config\AllConfig.json";
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
