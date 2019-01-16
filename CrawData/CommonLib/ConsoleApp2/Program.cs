using ITestLibrary;
using KaSon.FrameWork.Helper;
using System;
using System.Reflection;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //System.Data.SqlClient
            //  string path = @"E:\GitLab9\Lottery_02\CommonLib\ConsoleApp1\TestLibrary\bin\Debug\netcoreapp2.1";

            var t =AssemblyRefHelper.GetType("TestLibrary", "Test");
            //实例化类型
            var  o =(ITest) Activator.CreateInstance(t);

            //得到要调用的某类型的方法
            o.Fn1();//functionname:方法名字

//            object[] obj =
//            {
//     Parameters[0].TaxpayerName,
//     Parameters[0].TaxpayerTaxCode,
//     Parameters[0].CAPassword
//};
            //对方法进行调用
           // var keyData = method.Invoke(o,null);//param为方法参数object数组

            Console.ReadKey(true);
        }
    }
}
