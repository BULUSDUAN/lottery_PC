using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Data;
using System.ComponentModel;
using Kason.Net.Common.Business;

namespace Kason.Net.Common.Communication
{
    /// <summary>
    /// 将一个类、枚举、结构标识为一个Wcf通信对象
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, AllowMultiple = false, Inherited = true)]
    public class CommunicationObjectAttribute : Attribute
    {
    }
    /// <summary>
    /// 获取组建中的通信对象类型列表
    /// </summary>
    public static class CommunicationObjectGetter
    {
        private static Type[] _dataContractTypes = null;
        /// <summary>
        /// 获取组建中的通信对象类型列表
        /// </summary>
        /// <param name="assemblies">组建列表</param>
        /// <returns>通信对象列表</returns>
        public static Type[] GetCommunicationObjectTypes(params Assembly[] assemblies)
        {
            if (_dataContractTypes == null || _dataContractTypes.Count() == 0)
            {
                if (assemblies == null || assemblies.Length == 0)
                {
                    assemblies = GetCurrentAssemblyList();
                }
                try
                {
                    var list =
                        (from assem in assemblies
                         from type in assem.GetTypes()
                         where type.IsDefined(typeof(CommunicationObjectAttribute), true) || type.IsEnum
                         select type);

                    var allList = new List<Type>(list);

                    //var allList = new List<Type>();
                    //foreach (var item in assemblies)
                    //{
                    //    try
                    //    {
                    //        var types = item.GetTypes();
                    //        foreach (var t in types)
                    //        {
                    //            if (t.IsDefined(typeof(CommunicationObjectAttribute), true) || t.IsEnum)
                    //                allList.Add(t);
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bin");
                    //        if (!Directory.Exists(dir))
                    //        {
                    //            dir = AppDomain.CurrentDomain.BaseDirectory;
                    //        }
                    //        File.AppendAllText(Path.Combine(dir, "error.txt"), "反射程序集" + item.FullName + "==>错误：" + ex.ToString() + " \r\n");
                    //    }
                    //}

                    //allList.Add(typeof(EnableStatus));
                    //allList.Add(typeof(ResultStatus));
                    //allList.Add(typeof(MappingType));
                    //allList.Add(typeof(XmlObjectType));
                    //allList.Add(typeof(PayType));

                    //allList.Add(typeof(CommonActionResult));

                    //allList.Add(typeof(OpenDataInfo));
                    //allList.Add(typeof(OpenDataInfoCollection));
                    //allList.Add(typeof(OpenGradeInfo));
                    //allList.Add(typeof(OpenGradeInfoCollection));
                    //allList.Add(typeof(Kason.Net.Common.XmlAnalyzer.XmlMappingObject));

                    allList.Add(typeof(DataTable));
                    allList.Add(typeof(DataSet));
                    allList.Add(typeof(List<object>));

                    _dataContractTypes = allList.ToArray();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return _dataContractTypes;
        }
        public static Assembly[] GetCurrentAssemblyList()
        {
            //return AppDomain.CurrentDomain.GetAssemblies(); 
            var list = new List<Assembly>();
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bin");
            if (!Directory.Exists(dir))
            {
                dir = AppDomain.CurrentDomain.BaseDirectory;
            }

            foreach (var file in Directory.GetFiles(dir, "*.Core.dll"))
            {
                list.Add(Assembly.LoadFrom(file));
            }
            foreach (var file in Directory.GetFiles(dir, "Kason.Net.Common.dll"))
            {
                list.Add(Assembly.LoadFrom(file));
            }
            foreach (var file in Directory.GetFiles(dir, "Kason.Net.Common.*.dll"))
            {
                list.Add(Assembly.LoadFrom(file));
            }
            return list.ToArray();
        }
    }
}
