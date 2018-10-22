using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.Common.FileOperate
{
   public class FileHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileFullName"></param>
        /// <param name="content"></param>
        /// <param name="writeLog"></param>
        /// <returns></returns>
        public static bool CreateOrAppend(string fileFullName, string content, Action<string> writeLog)
        {
            bool bol = true;
            try
            {
                content = string.Format("{0}{1}{2}", "var data=", content, ";");
                StreamWriter sw = new StreamWriter(fileFullName, false);
                sw.Write(content);
                sw.Close();
            }
            catch (Exception ex)
            {
                bol = false;
                writeLog(ex.ToString());
            }
            return bol;
        }
       
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        /// <param name="customerPath"></param>
        /// <param name="currentTimes"></param>
        /// <param name="maxTimes"></param>
        /// <param name="writeLog"></param>
        /// <returns></returns>
        public static bool PostFileToServer(string url, string filePath, string[] customerPath, int currentTimes)
        {
            bool bol = true;
            try
            {

                //if (currentTimes >= maxTimes)
                //    return false;
                //currentTimes++;

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
                var r = Net.PostManager.UploadFile(url, filePath, dic);
                if (r != "1")
                    throw new Exception(string.Format("上传文件失败", currentTimes));
               // writeLog();
            }
            catch (Exception ex)
            {
                bol = false;
                // writeLog(ex.ToString());
               // PostFileToServer(url, filePath, customerPath, currentTimes, maxTimes, writeLog);
            }
            return bol;
        }

        public static bool txtWriter(string path,string content) {

           try
           {
               StreamWriter sW2 = new StreamWriter(path, true, Encoding.UTF8);

               sW2.Write(content);
               sW2.Close();
           }
           catch 
           {

               return false;
           }

           return true;
       }
       public static string txtReader(string path)
       {
           string all = "";

           try
           {
               StreamReader sR2 = new StreamReader(path, Encoding.UTF8);
               all= sR2.ReadToEnd();
               sR2.Close();
           }
           catch
           {

               return "";
           }

           return all;
       }

       public static void WriteString(string path, string content)
       {


           using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
           {
               sw.Write(content);
               sw.Flush();

               sw.Close();
           }
       }
       public static string ReadString(string path)
       {
           string html = "";

           if (File.Exists(path))
           {
               using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
               {
                   html = sr.ReadToEnd();
                   sr.Close();
               }


           }


           return html;
       }
    }
}
