using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kason.Net.Common
{
    /// <summary>
    /// 控制台调试消息跟踪器
    /// </summary>
    public static class DebugTracker
    {
        /// <summary>
        /// 输出通用起始标签到控制台
        /// </summary>
        public static void PrintCommonStartTag(string msg, params object[] arg)
        {
#if DEBUG
            Console.ForegroundColor = ConsoleColor.Cyan;
            string format = "";
            int i = 0;
            for (; i < arg.Length - 1; i++)
            {
                format += ", {" + i + "}";
            }
            Console.WriteLine("---- Start:{0} ----", string.Format(msg + format, arg));
#endif
        }
        /// <summary>
        /// 输出通用起始标签到控制台
        /// </summary>
        public static void PrintCommonStartTag_AllParams(string msg, params object[] arg)
        {
#if DEBUG
            Console.ForegroundColor = ConsoleColor.Cyan;
            string format = "";
            int i = 0;
            for (; i < arg.Length; i++)
            {
                format += ", {" + i + "}";
            }
            Console.WriteLine("---- Start:{0} ----", string.Format(msg + format, arg));
#endif
        }
        /// <summary>
        /// 输出起始标签到控制台
        /// </summary>
        public static void PrintStartTag(string msg, params object[] arg)
        {
#if DEBUG
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("---- Start:{0} ----", string.Format(msg, arg));
#endif
        }
        /// <summary>
        /// 输出结束标签到控制台
        /// </summary>
        public static void PrintEndTag()
        {
#if DEBUG
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("-- End ----------------------------------------------------");
#endif
        }
        /// <summary>
        /// 输出标签。白字不换行
        /// </summary>
        public static void PrintLabel(string msg, params object[] arg)
        {
#if DEBUG
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(msg, arg);
#endif
        }
        /// <summary>
        /// 输出消息。白字
        /// </summary>
        public static void PrintMessage(string msg, params object[] arg)
        {
#if DEBUG
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(msg, arg);
#endif
        }
        /// <summary>
        /// 输出成功消息。绿色
        /// </summary>
        public static void PrintSuccess(string msg, params object[] arg)
        {
#if DEBUG
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(msg, arg);
            Console.ForegroundColor = ConsoleColor.White;
#endif
        }
        /// <summary>
        /// 输出失败消息。红色
        /// </summary>
        public static void PrintFail(string msg, params object[] arg)
        {
#if DEBUG
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg, arg);
            Console.ForegroundColor = ConsoleColor.White;
#endif
        }
        /// <summary>
        /// 输出异常消息。红色
        /// </summary>
        public static void PrintException(Exception ex)
        {
#if DEBUG
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("异常：", ex.Message);
            Console.WriteLine(ex.ToString());
            Console.ForegroundColor = ConsoleColor.White;
#endif
        }
    }
}
