using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Common.Communication
{
    /// <summary>
    /// Wcf传输结果对象
    /// </summary>
    [KnownType("GetKnownTypes")]
    [DataContract]
    public class WcfInvokeResult
    {
        /// <summary>
        /// 初始化对象
        /// </summary>
        public WcfInvokeResult()
        {
            IsSuccess = false;
        }
        /// <summary>
        /// 结果是否成功
        /// </summary>
        [DataMember]
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
        [DataMember]
        public string ErrorMessage { get; set; }
        /// <summary>
        /// 错误明细
        /// </summary>
        [DataMember]
        public string ErrorDetail { get; set; }
        /// <summary>
        /// 错误分类
        /// </summary>
        [DataMember]
        public string ErrorCategory { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        [DataMember]
        public object Data { get; set; }
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
