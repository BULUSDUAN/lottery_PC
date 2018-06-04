using KaSon.FrameWork.Helper;
using Lottery.Kg.ORM.Helper;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static IKgLog log = null;
        static void Main(string[] args)
        {
            log = new Log4Log();

            log.Log("调试信息调试信息调试信息调试信息");
            log.Log("调试信息调试信息调试信息调试信息",new Exception("1231"));

            LoginHelper lh = new LoginHelper();

           lh.QueryUserName();

            Console.ReadKey(true);

            Console.WriteLine("Hello World!");
        }
    }
}
