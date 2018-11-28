using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Common.CommonWCF
{
    /// <summary>
    /// Wcf服务接口协议
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IWcfService_JSON
    {
        /// <summary>
        /// 调用一个有返回值的函数。会阻塞等待服务器端返回一个值
        /// </summary>
        [OperationContract]
        string Process(string p);
        /// <summary>
        /// 调用一个单向方法。会开启新线程调用服务器端方法，不会等待
        /// </summary>
        /// <param name="buffer">参数序列化的二进制</param>
        [OperationContract(IsOneWay = true)]
        void ProcessViaOneWay(string p);
    }
}
