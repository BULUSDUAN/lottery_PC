using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.Administration;

namespace Common.IISHelper
{
    public class SiteInfo
    {
        public SiteInfo()
        {
            Port = "80";
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string Path { get; set; }
        public override string ToString()
        {
            return string.Format("{0}({1}:{2})", Name, Host, Port);
        }
    }
    public class ApplicationPoolInfo
    {
        public bool AutoStart { get; set; }
        public bool Enable32BitAppOnWin64 { get; set; }
        public string ManagedRuntimeVersion { get; set; }
        public string Name { get; set; }
        public long QueueLength { get; set; }
        public ObjectState State { get; set; }
    }

    public abstract class IISOperator
    {
        public static IISOperator GetIISOperator(IISVersion iisVersion)
        {
            IISOperator iisOperator;

            switch (iisVersion)
            {
                case IISVersion.IIS5:
                case IISVersion.IIS6:
                    iisOperator = new IISOperator_V6();
                    break;
                case IISVersion.IIS7:
                case IISVersion.IIS8:
                    iisOperator = new IISOperator_V7();
                    break;
                default:
                    throw new IISException("不支持当前的IIS版本。", iisVersion);
            }
            iisOperator._iisVersion = iisVersion;
            return iisOperator;
        }

        private IISVersion _iisVersion;
        public IISVersion IISVersion { get { return _iisVersion; } }

        public abstract bool SetAuthenticationConfig(string locationPath, bool windowsAuthenticationEnabled);

        #region 站点操作

        /// <summary>
        /// 检查指定的站点是否已经存在
        /// </summary>
        /// <param name="serverName">服务器名称，如果IsNullOrEmpty，则默认为localhost。</param>
        /// <param name="websiteName">站点名称</param>
        /// <returns>如果站点在指定服务器已存在，则返回true，否则返回false。</returns>
        public abstract bool IsExistedWebSite(string serverName, string websiteName);

        /// <summary>
        /// 验证指定的主机名是否已经存在
        /// </summary>
        /// <param name="serverName">服务器名称，如果IsNullOrEmpty，则默认为localhost。</param>
        /// <param name="hostHeader">主机名</param>
        /// <param name="port">端口号</param>
        /// <returns>如果主机名已存在，则返回true，否则返回false。</returns>
        public abstract bool IsExistedHostHeader(string serverName, string hostHeader, string port);

        /// <summary>
        /// 得到默认的站点。判断标准：端口为80，header为空。
        /// </summary>
        /// <param name="serverName">服务器名称，如果IsNullOrEmpty，则默认为localhost。</param>
        /// <returns>默认站点的站点Id。如果不存在则返回-1</returns>
        public abstract string GetDefaultWebSiteID(string serverName, out string siteName);

        /// <summary>
        /// 获取当前服务器所有的站点列表
        /// </summary>
        /// <param name="serverName">服务器名称，如果IsNullOrEmpty，则默认为localhost。</param>
        /// <returns>所有的站点列表。如果不存在，则返回长度为0的列表。</returns>
        public abstract IList<SiteInfo> GetAllSiteList(string serverName);

        public abstract SiteInfo GetSiteByName(string serverName, string siteName);

        public abstract SiteInfo GetSiteById(string serverName, string siteName);

        /// <summary>
        /// 创建一个站点
        /// </summary>
        /// <param name="serverName">服务器名称，如果IsNullOrEmpty，则默认为localhost。</param>
        /// <param name="webSiteName">站点名称。如果为空，则返回Empty。</param>
        /// <param name="IPAddress">IP地址</param>
        /// <param name="TCPPort">端口号</param>
        /// <param name="hostHeader"></param>
        /// <param name="localPath">本地映射路径。</param>
        /// <returns>如果创建站点成功，返回站点Id。</returns>
        /// <exception cref="IISException"></exception>
        public abstract string CreateWebSite(string serverName, string webSiteName, string IPAddress, string TCPPort, string hostHeader, string localPath);

        /// <summary>
        /// 设置站点的应用程序池
        /// </summary>
        /// <param name="serverName">服务器名称，如果IsNullOrEmpty，则默认为localhost。</param>
        /// <param name="websiteID"></param>
        /// <param name="applicationPool"></param>
        /// <returns></returns>
        public abstract bool SetWebSiteApplicationPool(string serverName, string websiteID, string applicationPool);

        /// <summary>
        /// 编辑站点信息
        /// </summary>
        /// <param name="serverName">服务器名称，如果IsNullOrEmpty，则默认为localhost。</param>
        /// <param name="webSiteID">站点Id</param>
        /// <param name="IPAddress">IP地址</param>
        /// <param name="TCPPort">端口号</param>
        /// <param name="hostHeader"></param>
        /// <param name="webSiteName">站点名称。如果IsNullOrEmpty，则不修改此项。</param>
        /// <param name="localPath">本地映射路径。如果IsNullOrEmpty，则不修改此项。</param>
        /// <returns>如果编辑站点成功，则返回true，否则返回false。</returns>
        /// <exception cref="IISException"></exception>
        public abstract bool UpdateWebSite(string serverName, string webSiteID, string IPAddress, string TCPPort, string hostHeader, string webSiteName, string localPath);

        /// <summary>
        /// 删除一个站点
        /// </summary>
        /// <param name="serverName">服务器名称，如果IsNullOrEmpty，则默认为localhost。</param>
        /// <param name="websiteID">站点Id</param>
        /// <returns>如果站点删除成功，则返回true，否则返回false。</returns>
        /// <exception cref="IISException"></exception>
        public abstract bool DeleteWebSite(string serverName, string websiteID);

        #endregion

        #region 虚拟目录操作

        /// <summary>
        /// 检查指定名称的虚拟目录是否已经存在
        /// </summary>
        /// <param name="serverName">服务器名称，如果IsNullOrEmpty，则默认为localhost。</param>
        /// <param name="websiteID">站点Id</param>
        /// <param name="virtualDirName">虚拟目录名称</param>
        /// <returns>如果虚拟目录已经在服务器中存在，则返回true，否则返回false。</returns>
        /// <exception cref="IISException"></exception>
        public abstract bool IsExistedApplication(string serverName, string websiteID, string virtualDirName);

        /// <summary>
        /// 创建虚拟目录
        /// </summary>
        /// <param name="serverName">服务器名称，如果IsNullOrEmpty，则默认为localhost。</param>
        /// <param name="virtualDirName">虚拟目录名</param>
        /// <param name="physicalDir">物理路径</param>
        /// <param name="defaultPage">默认起始页</param>
        /// <param name="websiteID">站点Id</param>
        /// <param name="appPoolName">应用程序池名称。为空时则设置为默认池</param>
        /// <returns>如果虚拟目录创建成功，则返回true，否则返回false。</returns>
        /// <exception cref="IISException"></exception>
        public abstract bool CreateApplication(string serverName, string virtualDirName, string physicalDir, string defaultPage, string websiteID, string appPoolName);

        public abstract bool CreateSubApplication(string serverName, string virtualDirName, string appName, string appPoolName, string physicalDir, string websiteID);

        public abstract bool CreateRootVirtualDir(string serverName, string virtualDirName, string physicalDir, string websiteID);

        public abstract bool ModifyRootVirtualDir(string serverName, string oldVirDirName, string newVirDirName, string websiteID);

        public abstract bool DeleteRootVirtualDir(string serverName, string virtualDirName, string websiteID);

        public abstract bool DeleteSubApplication(string serverName, string websiteID, string virtualDirName, string appName);
        /// <summary>
        /// 删除虚拟目录
        /// </summary>
        /// <param name="serverName">服务器名称，如果IsNullOrEmpty，则默认为localhost。</param>
        /// <param name="websiteID">站点Id</param>
        /// <param name="virtualDirName">虚拟目录名称</param>
        /// <returns>如果虚拟目录删除成功，则返回true，否则返回false。</returns>
        /// <exception cref="IISException"></exception>
        public abstract bool DeleteApplication(string server, string websiteID, string virtualDirName);

        #endregion

        #region 应用程序池操作

        /// <summary>
        /// 获取所有应用程序池列表
        /// </summary>
        /// <param name="serverName">服务器名称，如果IsNullOrEmpty，则默认为localhost。</param>
        /// <returns></returns>
        public abstract IList<ApplicationPoolInfo> GetAllApplicationPoolList(string serverName);

        /// <summary>
        /// 检查指定名称的应用程序池是否已经存在
        /// </summary>
        /// <param name="serverName">服务器名称，如果IsNullOrEmpty，则默认为localhost。</param>
        /// <param name="appPoolName">应用程序池名称。</param>
        /// <returns>如果应用程序池已经在服务器中存在，则返回true，否则返回false。</returns>
        /// <exception cref="IISException"></exception>
        public abstract bool IsExistedApplicationPool(string serverName, string appPoolName);

        /// <summary>
        /// 创建应用程序池
        /// </summary>
        /// <param name="serverName">服务器名称，如果IsNullOrEmpty，则默认为localhost。</param>
        /// <param name="appPoolName">应用程序池名称。如果已经存在同名应用程序池，则返回false。</param>
        /// <returns>如果应用程序池创建成功，则返回true，否则返回false。</returns>
        /// <exception cref="IISException"></exception>
        public abstract bool CreateApplicationPool(string serverName, string appPoolName);

        /// <summary>
        /// 修改应用程序池运行标识
        /// </summary>
        /// <param name="serverName">服务器名称，如果IsNullOrEmpty，则默认为localhost。</param>
        /// <param name="appPoolName">应用程序池名称。如果已经存在同名应用程序池，则返回false。</param>
        /// <param name="userName">运行用户名</param>
        /// <param name="password">运行密码</param>
        /// <returns>如果修改应用程序池运行标识成功，则返回true，否则返回false。</returns>
        public abstract bool ModifyApplicationPoolIdentity(string serverName, string appPoolName, bool isCustom, int identityType, string userName, string password);

        /// <summary>
        /// 修改应用程序池运行版本
        /// </summary>
        /// <param name="serverName">服务器名称，如果IsNullOrEmpty，则默认为localhost。</param>
        /// <param name="appPoolName">应用程序池名称。如果已经存在同名应用程序池，则返回false。</param>
        /// <param name="runtimeVersion"></param>
        /// <returns></returns>
        public abstract bool ModifyApplicationPoolRuntimeVersion(string serverName, string appPoolName, string runtimeVersion);

        /// <summary>
        /// 修改应用程序运行用户
        /// </summary>
        /// <param name="serverName">服务器名称，如果IsNullOrEmpty，则默认为localhost。</param>
        /// <param name="websiteID">站点ID</param>
        /// <param name="virtualDirName">应用程序名称</param>
        /// <param name="userName">运行用户名</param>
        /// <param name="password">运行密码</param>
        /// <returns>如果修改应用程序运行用户成功，则返回true，否则返回false。</returns>
        public abstract bool ModifyApplicationIdentity(string serverName, string websiteID, string virtualDirName, string userName, string password);

        public abstract bool ModifySubApplicationIdentity(string serverName, string websiteID, string virtualDirName, string appName, string userName, string password);

        /// <summary>
        /// 删除应用程序池
        /// </summary>
        /// <param name="serverName">服务器名称，如果IsNullOrEmpty，则默认为localhost。</param>
        /// <param name="appPoolName">应用程序池名称。如果不存在指定应用程序池，则返回false。</param>
        /// <returns>如果应用程序池删除成功，则返回true，否则返回false。</returns>
        /// <exception cref="IISException"></exception>
        public abstract bool DeleteApplicationPool(string serverName, string appPoolName);

        #endregion
    }
}
