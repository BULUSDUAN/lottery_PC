using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kason.Net.Common.KaSon
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
                    Task.Delay(1000 * 20);
                    Console.Clear();
                    //Console.WriteLine("1231");
                }

            });
        }
    }
}
