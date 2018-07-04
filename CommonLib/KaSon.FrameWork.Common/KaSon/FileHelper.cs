using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.Common
{
   public class FileHelper
    {

       
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
