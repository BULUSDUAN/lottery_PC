using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Common.Communication
{
    /// <summary>
    /// Wcf传输参数对象
    /// </summary>
    [KnownType("GetKnownTypes")]
    [DataContract]
    public class WcfInvokeInfo
    {
        /// <summary>
        /// 调用函数名
        /// </summary>
        [DataMember]
        public string MethodName { get; set; }
        /// <summary>
        /// 函数实参
        /// </summary>
        [DataMember]
        public object[] Parameters { get; set; }
        ///<summary>
        /// 获取已知的类型
        ///</summary>
        ///<returns>类型列表</returns>
        public static Type[] GetKnownTypes()
        {
            return KnownTypeRegister.GetKnownTypes();
        }
    }
}
