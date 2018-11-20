using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Common.Communication;
using System.Threading;
using Common.Log;
using Common.Cryptography;
using System.Reflection;

namespace Common.CommonWCF
{
    /// <summary>
    /// Wcf服务基类
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall)]
    public abstract class WcfService_JSON : IWcfService_JSON
    {
        public string Process(string buffer)
        {
            //buffer = WcfByteHandler.DecryptData(buffer);
            //buffer = WcfByteHandler.DecompressData(buffer);
            buffer = DoProcess(buffer);
            //buffer = WcfByteHandler.CompressData(buffer);
            //buffer = WcfByteHandler.EncryptData(buffer);
            return buffer;
        }
        public void ProcessViaOneWay(string buffer)
        {
            var thread = new Thread(() => Process(buffer));
            thread.Start();
        }
        private string DoProcess(string buffer)
        {
            var invoke = System.Web.Helpers.Json.Decode<WcfInvokeInfo>(buffer);
            var result = DoWork(invoke);
            try
            {
                buffer = System.Web.Helpers.Json.Encode(result);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.IsSuccess = false;
                result.ErrorMessage = "序列号错误：" + ex.Message;
                result.ErrorDetail = "序列号错误 - " + ex;
                buffer = System.Web.Helpers.Json.Encode(result);
            }
            return buffer;
        }
        private WcfInvokeResult DoWork(WcfInvokeInfo invokeInfo)
        {
            var rst = new WcfInvokeResult();
            try
            {
                try
                {
                    var start = DateTime.Now;
                    if (string.IsNullOrWhiteSpace(invokeInfo.MethodName))
                    {
                        throw new LogicException("传入函数名为空");
                    }
                    var mi = GetType().GetMethod(invokeInfo.MethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                    if (mi == null)
                    {
                        throw new LogicException("传入函数名不存在 - " + invokeInfo.MethodName);
                    }
                    var result = mi.Invoke(this, invokeInfo.Parameters);  //所有被调用方法,必须返回WcfInvokeResult对象
                    var end = DateTime.Now;
                    var span = end - start;
                    if (span.Seconds >= 5)
                    {
                        WriteLog(invokeInfo, (int)span.TotalMilliseconds);
                    }
                    rst.IsSuccess = true;
                    rst.Data = result;
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
                catch (Exception ex)
                {
                    WriteException(invokeInfo, ex);

                    rst.ErrorMessage = ex.Message;
                    rst.ErrorDetail = "调用函数异常 - " + invokeInfo.MethodName + "  " + ex;
                }
            }
            catch (WcfException ex)
            {
                WriteException(invokeInfo, ex);

                rst.ErrorCategory = "WCF";
                rst.ErrorMessage = ex.Message;
                rst.ErrorDetail = ex.ToString();
            }
            catch (LogicException ex)   // 逻辑错误，不需要写日志
            {
                rst.ErrorCategory = "LOGIC";
                rst.ErrorMessage = ex.Message;
                rst.ErrorDetail = ex.ToString();
            }
            catch (AuthException ex)    // 身份验证失败，不需要写日志
            {
                rst.ErrorCategory = "AUTH";
                rst.ErrorMessage = ex.Message;
                rst.ErrorDetail = ex.ToString();
            }
            catch (Exception ex)
            {
                WriteException(invokeInfo, ex);

                rst.ErrorCategory = ex.GetType().FullName;
                rst.ErrorMessage = ex.Message;
                rst.ErrorDetail = "调用函数异常 - " + invokeInfo.MethodName + "  " + ex;
            }
            return rst;
        }
        private static void WriteException(WcfInvokeInfo invokeInfo, Exception ex)
        {
            var logWriter = LogWriterGetter.GetLogWriter();
            if (logWriter != null)
            {
                for (int i = 0; i < invokeInfo.Parameters.Length; i++)
                {
                    if (invokeInfo.Parameters[i] != null)
                    {
                        ex.Data.Add("[" + i + "]" + invokeInfo.Parameters[i].GetType().Name, "值：" + invokeInfo.Parameters[i]);
                    }
                }
                logWriter.Write("Error", invokeInfo.MethodName, ex);
            }
        }
        private static void WriteLog(WcfInvokeInfo invokeInfo, int time)
        {
            var logWriter = LogWriterGetter.GetLogWriter();
            if (logWriter != null)
            {
                var sb = new StringBuilder();
                if (invokeInfo != null)
                {
                    sb.AppendLine("执行函数：" + invokeInfo.MethodName);
                    foreach (var p in invokeInfo.Parameters)
                    {
                        if (p != null)
                        {
                            sb.AppendLine(string.Format("  参数类型：{0}；值：{1}", p.GetType().Name, p));
                        }
                    }
                }
                logWriter.Write("Timeout\\Info-", invokeInfo.MethodName + "_" + time, LogType.Information, invokeInfo.MethodName + "_" + time, sb.ToString());
            }
        }
        private static string GetSpace(int count)
        {
            string s = "";
            for (int i = 0; i < count; i++)
            {
                s += "  ";
            }
            return s;
        }

        #region IDisposable 成员

        /// <summary>
        /// 释放资源，关闭通信通道
        /// </summary>
        public void Dispose()
        {
        }

        #endregion
    }
}
