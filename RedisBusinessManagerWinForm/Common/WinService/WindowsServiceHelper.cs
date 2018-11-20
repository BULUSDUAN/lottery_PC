using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace Common.WinService
{
    /// <summary>
    /// Windows服务辅助类
    /// </summary>
    public static class WindowsServiceHelper
    {
        private static string installerPath;
        private static string runtimePath;
        static WindowsServiceHelper()
        {
            runtimePath = RuntimeEnvironment.GetRuntimeDirectory();
            runtimePath = runtimePath.Replace(@"\Framework64\", @"\Framework\");
            installerPath = Path.Combine(runtimePath, "installutil.exe");
        }
        /// <summary>
        /// 查看指定名称的服务
        /// </summary>
        public static ServiceController GetServiceByName(string serviceName)
        {
            return ServiceController.GetServices().FirstOrDefault(s => s.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
        }

        public static List<ServiceController> GetServices(string[] serviceNameArray)
        {
            return ServiceController.GetServices().Where(s => serviceNameArray.Contains(s.ServiceName)).ToList<ServiceController>();
        }

        /// <summary>
        /// 检查指定名称的服务是否存在
        /// </summary>
        public static bool IsServiceExisted(string serviceName)
        {
            return ServiceController.GetServices().Where(s => s.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase)).Count() == 1;
        }
        /// <summary>
        /// 查看指定名称的服务的状态
        /// </summary>
        public static ServiceControllerStatus CheckState(string serviceName)
        {
            return ServiceController.GetServices().FirstOrDefault(s => s.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase)).Status;
        }
        /// <summary>
        /// 获取指定名称的服务的文件路径
        /// </summary>
        public static string GetServiceFilePath(string serviceName)
        {
            string regKeyPath = string.Format(@"SYSTEM\ControlSet001\Services\{0}", serviceName);
            RegistryKey _Key = Registry.LocalMachine.OpenSubKey(regKeyPath);
            if (_Key != null)
            {
                object _ObjPath = _Key.GetValue("ImagePath");
                if (_ObjPath != null)
                {
                    return _ObjPath.ToString().Trim('"');
                }
            }
            return "";
        }
        /// <summary>
        /// 获取指定名称的服务的描述
        /// </summary>
        public static string GetServiceDescription(string serviceName)
        {
            string regKeyPath = string.Format(@"SYSTEM\ControlSet001\Services\{0}", serviceName);
            RegistryKey _Key = Registry.LocalMachine.OpenSubKey(regKeyPath);
            if (_Key != null)
            {
                object _ObjPath = _Key.GetValue("Description");
                if (_ObjPath != null)
                {
                    return _ObjPath.ToString().Trim('"');
                }
            }
            return "";
        }
        /// <summary>
        /// 安装服务
        /// </summary>
        public static void InstallService(string fileName)
        {
            Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "install.bat"), "\"" + runtimePath + "\"  \"" + fileName + "\"");
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        public static void StartService(string serviceName)
        {
            using (var service = GetServiceByName(serviceName))
            {
                if (service == null) throw new Exception("指定服务\"" + serviceName + "\"不存在。");

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running);
                service.Close();
            }
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        public static void StopService(string serviceName)
        {
            using (var service = GetServiceByName(serviceName))
            {
                if (service == null) throw new Exception("指定服务\"" + serviceName + "\"不存在。");

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped);
                service.Close();
            }
        }
        /// <summary>
        /// 卸载服务
        /// </summary>
        public static void UninstallService(string fileName)
        {
            Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "install.bat"), "\"" + runtimePath + "\"  \"" + fileName + "\" -u");
        }
    }
}
