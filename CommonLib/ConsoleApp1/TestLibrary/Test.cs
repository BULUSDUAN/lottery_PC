using ITestLibrary;
using KaSon.FrameWork.ORM.Provider;
using System;
using System.Linq;
namespace TestLibrary
{
    public class Test: ITest
    {

        public void Fn1()
        {

            var evadb = new DbProvider();
            // db.Init("Default");
            evadb.Init("Default");
            var LIST = evadb.CreateQuery<Dmodel>().ToList();
            foreach (var item in LIST)
            {
                Console.WriteLine(item.name);
            }
        }
    }
}
