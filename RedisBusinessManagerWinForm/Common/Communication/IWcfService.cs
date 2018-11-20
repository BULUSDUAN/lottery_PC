using System.ServiceModel;

namespace Common.Communication
{
    /// <summary>
    /// Wcf服务接口协议
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IWcfService
    {
        /// <summary>
        /// 调用一个有返回值的函数。会阻塞等待服务器端返回一个值
        /// </summary>
        /// <param name="buffer">参数序列化的二进制</param>
        /// <returns>返回值的二进制格式</returns>
        [OperationContract]
        byte[] Process(byte[] buffer);
        /// <summary>
        /// 调用一个单向方法。会开启新线程调用服务器端方法，不会等待
        /// </summary>
        /// <param name="buffer">参数序列化的二进制</param>
        [OperationContract(IsOneWay = true)]
        void ProcessViaOneWay(byte[] buffer);
    }
}
