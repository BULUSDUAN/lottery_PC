using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace KaSon.FrameWork.Common
{
    [Serializable]
    public class PluginConfigInfo
    {

        /// <summary>
        /// 是否调试模式
        /// </summary>
        public string DicPath { get; set; }
    }
    /// <summary>
    /// 利用反射动态加载dll
    /// </summary>
    public class AssemblyRefHelper
    {

        /// <summary>
        /// 通过目录加载dll to Assembly
        /// </summary>
        /// <param name="path">目录</param>
        /// <returns></returns>
      //  static IList<Assembly> dllList = new List<Assembly>();
        static IDictionary<string, Assembly> diclist=new Dictionary<string, Assembly>();
         static AssemblyRefHelper()
        {
            // s

            string pathdic = Path.Combine(Directory.GetCurrentDirectory(), @"AssemblyRef.xml");
            if (!File.Exists(pathdic))
            {
                pathdic = Path.Combine(Directory.GetCurrentDirectory(), @"Config\AssemblyRef.xml");
            }
            var list = xmlHelper.SerializerList<PluginConfigInfo>(pathdic);
            string path = "";
            if (list.Count > 0)
            {

                path = list[0].DicPath;

            }
            if (string.IsNullOrEmpty(path)) throw new Exception("插件目录不能为空，请检查配置文件");
            //加载路径
            //获取指定文件夹的所有文件  
            string[] paths = Directory.GetFiles(path);
          
            foreach (var item in paths)
            {
                //获取文件后缀名  
                string extension = Path.GetExtension(item).ToLower();
                if (extension == ".dll")
                {
                   // dllList.Add(item);//添加到图片list中  
                    Assembly asm = Assembly.LoadFrom(item);
                    //  if(dllList.Contains(base=)
                    diclist[asm.GetName().Name] =asm;
                }
            }
            //return diclist;
        }
       /// <summary>
       ///  获取类型
       /// </summary>
       /// <param name="n_namespace"></param>
       /// <param name="classname"></param>
       /// <returns></returns>
        public static Type GetType(string n_namespace,string classname) {

            var value = diclist[n_namespace];
            string path = n_namespace + "." + classname;
            return  value.GetType(path);
        }

    }
}
