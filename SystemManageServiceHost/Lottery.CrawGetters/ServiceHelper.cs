using EntityModel.Enum;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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
        /// <summary>
        /// 上传文件
        /// </summary>
        public static void PostFileToServer(string key,string filePath, string[] customerPath, Action<string> writeLog)
        {
            var urlArray = key.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in urlArray)
            {
                var currentUrl = string.Format("{0}/CoreNotice/ReviceFile", item);
                PostFileToServer(currentUrl, filePath, customerPath, 0, 3, writeLog);
            }
        }

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
