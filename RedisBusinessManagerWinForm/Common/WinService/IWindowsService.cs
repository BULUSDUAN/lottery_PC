using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Log;

namespace Common.WinService
{
    /// <summary>
    /// 提供由ServiceAgent服务程序启动和停止的通用服务程序接口。
    /// </summary>    
    public interface IWindowsService
    {
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="logWriter">日志记录器. 当在服务内部需要进行日志记录时, 调用此记录器进行日志的记录。</param>   
        /// <param name="gameName">彩种编码</param>
        void Start(ILogWriter logWriter, string gameName);
        /// <summary>
        /// 停止
        /// </summary>
        void Stop();
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="log">日志内容</param>
        void WriteLog(string log);
        /// <summary>
        /// 写错误日志
        /// </summary>
        /// <param name="log">日志内容</param>
        void WriteError(string log);
    }
}
