using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KaSon.FrameWork.Common.KaSon
{
    public class ConsoleHelper
    {

        /// <summary>
        /// 屏幕输出有时卡住，清屏幕
        /// </summary>
        public static void Clear(){
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000 * 2);
                    //Task.Delay(1000 * 20);
                    Console.Clear();
                    //Console.WriteLine("1231");
                }

            });
        }
    }
}
