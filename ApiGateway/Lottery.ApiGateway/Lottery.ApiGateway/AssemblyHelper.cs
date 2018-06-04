using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Lottery.ApiGateway
{

    public  class AssemblyHelper
    {

        //public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        //{
        //    HashSet<TKey> seenKeys = new HashSet<TKey>();
        //    foreach (TSource element in source)
        //    {
        //        if (seenKeys.Add(keySelector(element)))
        //        {
        //            yield return element;
        //        }
        //    }
        //}
        public static IList<Assembly> LoadAssembly()
        {

            string runpath = Directory.GetCurrentDirectory();
            DirectoryInfo dir = new DirectoryInfo(runpath);
            IList<Assembly> alist = new List<Assembly>();
            IList<string> blist = new List<string>();
            var controllerList = dir.GetFiles("*Controllers.dll", SearchOption.AllDirectories);

          var list=  controllerList.Distinct();

          

            foreach (FileInfo file in list)//第二个参数表示搜索包含子目录中的文件；
            {
                
                if (file.Name.Contains(".Controllers"))
                {

                    if (!blist.Contains(file.Name))
                    {
                        blist.Add(file.Name);
                        Console.WriteLine(file.Name);
                        alist.Add(Assembly.Load(file.Name.Replace(".dll", "")));
                        //yield return element;
                    }
                    // string assemblyPath = runpath +"\\"+ file.fi;
                    // alist.Add();
                   

                }
                // MessageBox.Show(file.Name);
            }
            return alist;


        }
    }
}
