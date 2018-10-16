using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kason.Net.Common.Communication
{
    /// <summary>
    /// 已知类型注册机
    /// </summary>
    public static class KnownTypeRegister
    {
        private static readonly Dictionary<string, Type> KnownTypes = new Dictionary<string, Type>();
        private static readonly object SycTypes = new object();
        /// <summary>
        /// 注册类型
        /// </summary>
        /// <param name="types">类型清单</param>
        public static void RegisterKnownTypes(Type[] types)
        {
            foreach (Type t in types.Where(t => !KnownTypes.ContainsKey(t.FullName)))
            {
                lock (SycTypes)
                {
                    if (!KnownTypes.ContainsKey(t.FullName))
                    {
                        KnownTypes.Add(t.FullName, t);
                    }
                }
            }
        }
        /// <summary>
        /// 获取已注册类型
        /// </summary>
        /// <returns>类型清单</returns>
        public static Type[] GetKnownTypes()
        {
            return KnownTypes.Values.ToArray();
        }
    }
}
