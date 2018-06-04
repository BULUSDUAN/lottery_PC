using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.Helper
{
   public class BashShell
    {


       /// <summary>
       /// linux 命令  
       /// </summary>
       /// <param name="filename"></param>
       /// <param name="cmdParm"></param>
       /// <returns></returns>
       public static string BashCmd(string filename,string cmdParm) {

           //  string cmdStr = "/bin/ls /";
           ///usr/bin/mono -V
        //   var output = Clib.InvokeBash(cmd);
         //  var output1 = Clib.InvokeBash("/usr/bin/mono -V");
           ProcessStartInfo procStartInfo = new ProcessStartInfo(filename, cmdParm);
           procStartInfo.RedirectStandardOutput = true;
           procStartInfo.UseShellExecute = false;
           procStartInfo.CreateNoWindow = true;

           System.Diagnostics.Process proc = new System.Diagnostics.Process();
           proc.StartInfo = procStartInfo;
           proc.Start();

           String result = proc.StandardOutput.ReadToEnd();

           return result;

       }

    }
}
