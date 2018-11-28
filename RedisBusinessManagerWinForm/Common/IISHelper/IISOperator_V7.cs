using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.Administration;
using System.IO;

namespace Common.IISHelper
{
    public class IISOperator_V7 : IISOperator
    {
        #region WebSite IsExisted

        public override bool IsExistedWebSite(string serverName, string websiteName)
        {
            using (ServerManager iisManager = new ServerManager())
            {
                foreach (Site site in iisManager.Sites)
                {
                    if (site.Id.ToString() == websiteName || site.Name == websiteName)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        #endregion

        #region HostHeader IsExisted

        public override bool IsExistedHostHeader(string serverName, string hostHeader, string port)
        {
            using (ServerManager iisManager = new ServerManager())
            {
                foreach (Site site in iisManager.Sites)
                {
                    foreach (Binding binding in site.Bindings)
                    {
                        if (binding.Host.Equals(hostHeader, StringComparison.OrdinalIgnoreCase)
                            && binding.Protocol.Equals("http", StringComparison.OrdinalIgnoreCase)
                            && binding.EndPoint != null
                            && binding.EndPoint.Port.ToString().Equals(port, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        #endregion

        #region WebSite GetDefult

        public override string GetDefaultWebSiteID(string serverName, out string siteName)
        {
            using (ServerManager iisManager = new ServerManager())
            {
                foreach (Site site in iisManager.Sites)
                {
                    if (site.Bindings[0].BindingInformation == "*:80:")
                    {
                        siteName = site.Name;
                        return site.Id.ToString();
                    }
                }
                siteName = null;
                return "-1";
            }
        }

        #endregion

        #region WebSite GetAllSiteList

        public override IList<SiteInfo> GetAllSiteList(string serverName)
        {
            using (ServerManager iisManager = new ServerManager())
            {
                IList<SiteInfo> list = new List<SiteInfo>();
                foreach (Site site in iisManager.Sites)
                {
                    SiteInfo info = new SiteInfo();
                    info.Id = site.Id;
                    info.Name = site.Name;
                    info.Path = GetSitePath(site);
                    foreach (Binding binding in site.Bindings)
                    {
                        if (binding.Protocol.Equals("http", StringComparison.OrdinalIgnoreCase))
                        {
                            string[] items = binding.BindingInformation.Split(':');
                            info.Host = items[2];
                            info.Port = items[1];
                        }
                    }
                    list.Add(info);
                }
                return list;
            }
        }
        private string GetSitePath(Site site)
        {
            foreach (var app in site.Applications)
            {
                if (app.Path == "/")
                {
                    return app.VirtualDirectories[0].PhysicalPath;
                }
            }
            return "";
        }

        public override SiteInfo GetSiteByName(string serverName, string siteName)
        {
            using (ServerManager iisManager = new ServerManager())
            {
                IList<SiteInfo> list = new List<SiteInfo>();
                foreach (Site site in iisManager.Sites)
                {
                    if (site.Name.Equals(siteName, StringComparison.OrdinalIgnoreCase))
                    {
                        SiteInfo info = new SiteInfo();
                        info.Id = site.Id;
                        info.Name = site.Name;
                        info.Path = GetSitePath(site);
                        foreach (Binding binding in site.Bindings)
                        {
                            if (binding.Protocol.Equals("http", StringComparison.OrdinalIgnoreCase))
                            {
                                string[] items = binding.BindingInformation.Split(':');
                                info.Host = items[2];
                                info.Port = items[1];
                            }
                        }
                        return info;
                    }
                }
            }
            return null;
        }

        #endregion

        #region WebSite Create

        public override string CreateWebSite(string serverName, string webSiteName, string IPAddress, string TCPPort, string hostHeader, string localPath)
        {
            try
            {
                using (ServerManager iisManager = new ServerManager())
                {
                    Site site = iisManager.Sites.Add(webSiteName, localPath, int.Parse(TCPPort));
                    site.Bindings.Clear();
                    // 添加站点绑定信息
                    site.Bindings.Add(string.Format("*:{0}:{1}", TCPPort, hostHeader ?? ""), "http");
                    iisManager.CommitChanges();
                    return site.Id.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new IISException("Create WebSite error - " + ex.Message, ex, IISVersion);
            }
        }

        public override bool SetWebSiteApplicationPool(string serverName, string websiteID, string applicationPool)
        {
            try
            {
                using (ServerManager iisManager = new ServerManager())
                {
                    var site = GetSiteById(iisManager, websiteID);
                    if (site != null)
                    {
                        site.Applications["/"].ApplicationPoolName = applicationPool;
                    }
                    iisManager.CommitChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new IISException("Set WebSite's ApplicationPool error - " + ex.Message, ex, IISVersion);
            }
        }

        #endregion

        #region WebSite Update

        public override bool UpdateWebSite(string serverName, string webSiteID, string IPAddress, string TCPPort, string hostHeader, string webSiteName, string localPath)
        {
            try
            {
                using (ServerManager iisManager = new ServerManager())
                {
                    Site site = GetSiteById(iisManager, webSiteID);
                    if (site != null)
                    {
                        bool isChanged = false;
                        if (site.Name != webSiteName)
                        {
                            site.Name = webSiteName;
                            isChanged = true;
                        }
                        // 添加站点绑定信息
                        foreach (Binding binding in site.Bindings)
                        {
                            if (binding.Protocol.Equals("http", StringComparison.OrdinalIgnoreCase))
                            {
                                string bindingInfo = string.Format("*:{0}:{1}", TCPPort, hostHeader ?? "");
                                if (binding.BindingInformation != bindingInfo)
                                {
                                    binding.BindingInformation = bindingInfo;
                                    isChanged = true;
                                }
                            }
                        }
                        if (isChanged)
                        {
                            iisManager.CommitChanges();
                        }
                        return true;
                    }
                    else
                    {
                        throw new ArgumentNullException("WebSite is not exsited.");
                    }
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Update WebSite error - " + ex.Message);
                sb.AppendLine("serverName: " + serverName);
                sb.AppendLine("webSiteID: " + webSiteID);
                sb.AppendLine("IPAddress: " + IPAddress);
                sb.AppendLine("TCPPort: " + TCPPort);
                sb.AppendLine("hostHeader: " + hostHeader);
                sb.AppendLine("webSiteName: " + webSiteName);
                sb.AppendLine("localPath: " + localPath);
                throw new IISException(sb.ToString(), ex, IISVersion);
            }
        }

        #endregion

        #region WebSite Delete

        public override bool DeleteWebSite(string serverName, string websiteID)
        {
            try
            {
                using (ServerManager iisManager = new ServerManager())
                {
                    Site site = GetSiteById(iisManager, websiteID);
                    if (site != null)
                    {
                        iisManager.Sites.Remove(site);
                        iisManager.CommitChanges();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new IISException("Delete WebSite error - " + ex.Message, ex, IISVersion);
            }
        }

        #endregion

        #region VirtualDir IsExisted

        public override bool IsExistedApplication(string serverName, string websiteID, string virtualDirName)
        {
            using (ServerManager iisManager = new ServerManager())
            {
                Site site = GetSiteById(iisManager, websiteID);
                if (site != null)
                {
                    foreach (Application app in site.Applications)
                    {
                        if (app.Path == "/" + virtualDirName)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        #endregion

        #region VirtualDir Create

        public override bool SetAuthenticationConfig(string locationPath, bool windowsAuthenticationEnabled)
        {
            try
            {
                using (ServerManager serverManager = new ServerManager())
                {
                    Configuration config = serverManager.GetApplicationHostConfiguration();

                    ConfigurationSection windowsAuthenticationSection = config.GetSection("system.webServer/security/authentication/windowsAuthentication", locationPath);
                    windowsAuthenticationSection["enabled"] = windowsAuthenticationEnabled;

                    serverManager.CommitChanges();
                    return true;
                }

            }
            catch (Exception ex)
            {
                throw new IISException("Set authentication config error - " + ex.Message, ex, IISVersion);
            }
        }

        public override bool ModifySubApplicationIdentity(string serverName, string websiteID, string virtualDirName, string appName, string userName, string password)
        {
            try
            {
                using (ServerManager iisManager = new ServerManager())
                {
                    Site site = GetSiteById(iisManager, websiteID);
                    if (site != null)
                    {
                        var coll = site.GetCollection();
                        ConfigurationElement ap = coll.Where((e) =>
                        {
                            return e != null && string.Compare(e.ElementTagName, "application", true) == 0 && e["path"].ToString() == @"/" + virtualDirName + @"/" + appName;
                        }).FirstOrDefault();

                        if (ap != null)
                        {
                            coll = ap.GetCollection();
                            var vdap = coll.Where((e) =>
                            {
                                return e != null && string.Compare(e.ElementTagName, "virtualDirectory", true) == 0 && e["path"].ToString() == @"/";
                            }).FirstOrDefault();
                            if (vdap != null)
                            {
                                vdap["userName"] = userName;
                                vdap["password"] = password;
                            }
                        }
                        iisManager.CommitChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new IISException("Modify Sub Application Identity error - " + ex.Message, ex, IISVersion);
            }
        }

        public override bool CreateSubApplication(string serverName, string virtualDirName, string appName, string appPoolName, string physicalDir, string websiteID)
        {
            try
            {
                using (ServerManager iisManager = new ServerManager())
                {
                    Site site = GetSiteById(iisManager, websiteID);
                    if (site != null)
                    {
                        var coll = site.GetCollection();
                        ConfigurationElement app = coll.Where((e) =>
                        {
                            return e != null && string.Compare(e.ElementTagName, "application", true) == 0;
                        }).FirstOrDefault();

                        coll = app.GetCollection();
                        ConfigurationElement vir = coll.Where((e) =>
                        {
                            return e != null && string.Compare(e.ElementTagName, "virtualDirectory", true) == 0 && e["path"].ToString() == @"/" + virtualDirName;
                        }).FirstOrDefault();

                        coll = site.GetCollection();
                        ConfigurationElement ap = coll.Where((e) =>
                        {
                            return e != null && string.Compare(e.ElementTagName, "application", true) == 0 && e["path"].ToString() == @"/" + virtualDirName + @"/" + appName;
                        }).FirstOrDefault();

                        if (ap == null)
                        {
                            ap = coll.CreateElement("application");
                            ap["path"] = @"/" + virtualDirName + @"/" + appName;
                            coll.Add(ap);

                            coll = ap.GetCollection();
                            var vdap = coll.CreateElement("virtualDirectory");
                            vdap["path"] = @"/";
                            vdap["physicalPath"] = physicalDir;
                            coll.Add(vdap);
                        }
                        if (!string.IsNullOrEmpty(appPoolName))
                        {
                            ap["applicationPool"] = appPoolName;
                        }
                        iisManager.CommitChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new IISException("Create Application error - " + ex.Message, ex, IISVersion);
            }
        }

        public override bool DeleteRootVirtualDir(string serverName, string virtualDirName, string websiteID)
        {
            try
            {
                using (ServerManager iisManager = new ServerManager())
                {
                    Site site = GetSiteById(iisManager, websiteID);
                    if (site != null)
                    {
                        var coll = site.GetCollection();
                        ConfigurationElement app = coll.Where((e) =>
                        {
                            return e != null && string.Compare(e.ElementTagName, "application", true) == 0 && e["path"].ToString() == @"/";
                        }).FirstOrDefault();
                        if (app != null)
                        {
                            coll = app.GetCollection();
                            ConfigurationElement vd = coll.Where((e) =>
                            {
                                return e != null && string.Compare(e.ElementTagName, "virtualDirectory", true) == 0 && e["path"].ToString() == @"/" + virtualDirName;
                            }).FirstOrDefault();
                            if (vd != null)
                            {
                                coll.Remove(vd);
                            }
                        }
                        iisManager.CommitChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new IISException("Create Virtual Directory error - " + ex.Message, ex, IISVersion);
            }
        }

        public override bool CreateRootVirtualDir(string serverName, string virtualDirName, string physicalDir, string websiteID)
        {
            try
            {
                using (ServerManager iisManager = new ServerManager())
                {
                    Site site = GetSiteById(iisManager, websiteID);
                    if (site != null)
                    {
                        var coll = site.GetCollection();
                        ConfigurationElement app = coll.Where((e) =>
                        {
                            return e != null && string.Compare(e.ElementTagName, "application", true) == 0 && e["path"].ToString() == @"/";
                        }).FirstOrDefault();
                        if (app == null)
                        {
                            app = coll.CreateElement("application");
                            app["path"] = @"/";
                            coll.Add(app);
                        }
                        coll = app.GetCollection();
                        var vd = coll.CreateElement("virtualDirectory");
                        vd["path"] = @"/" + virtualDirName;
                        vd["physicalPath"] = physicalDir;
                        coll.Add(vd);
                        iisManager.CommitChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new IISException("Create Virtual Directory error - " + ex.Message, ex, IISVersion);
            }
        }

        public override bool ModifyRootVirtualDir(string serverName, string oldVirDirName, string newVirDirName, string websiteID)
        {
            try
            {
                using (ServerManager iisManager = new ServerManager())
                {
                    Site site = GetSiteById(iisManager, websiteID);
                    if (site != null)
                    {
                        var coll = site.GetCollection();
                        ConfigurationElement app = coll.Where((e) =>
                        {
                            return e != null && string.Compare(e.ElementTagName, "application", true) == 0 && e["path"].ToString() == @"/";
                        }).FirstOrDefault();
                        var subAppColl = coll.Where((e) =>
                        {
                            return e != null && string.Compare(e.ElementTagName, "application", true) == 0 && e["path"].ToString().StartsWith(@"/" + oldVirDirName);
                        }).ToArray();

                        if (subAppColl != null)
                        {
                            foreach (ConfigurationElement subApp in subAppColl)
                            {
                                subApp["path"] = subApp["path"].ToString().Replace("/" + oldVirDirName + "/", "/" + newVirDirName + "/"); ;
                            }
                        }
                        if (app != null)
                        {
                            coll = app.GetCollection();
                            ConfigurationElement vd = coll.Where((e) =>
                            {
                                return e != null && string.Compare(e.ElementTagName, "virtualDirectory", true) == 0 && e["path"].ToString() == @"/" + oldVirDirName;

                            }).FirstOrDefault();
                            if (vd != null)
                            {
                                vd["path"] = @"/" + newVirDirName;
                            }
                        }
                        iisManager.CommitChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new IISException("Modify Virtual Directory error - " + ex.Message, ex, IISVersion);
            }
        }

        public override bool CreateApplication(string serverName, string virtualDirName, string physicalDir, string defaultPage, string websiteID, string appPoolName)
        {
            try
            {
                using (ServerManager iisManager = new ServerManager())
                {
                    Site site = GetSiteById(iisManager, websiteID);
                    if (site != null)
                    {
                        Application app = site.Applications.Add("/" + virtualDirName, physicalDir);
                        // 设置应用池
                        if (!string.IsNullOrEmpty(appPoolName))
                        {
                            app.ApplicationPoolName = appPoolName;
                        }
                        // 启用命名管道协议
                        app.EnabledProtocols = "http";

                        iisManager.CommitChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new IISException("Create Application error - " + ex.Message, ex, IISVersion);
            }
        }

        #endregion

        #region VirtualDir Delete

        public override bool DeleteSubApplication(string serverName, string websiteID, string virtualDirName, string appName)
        {
            try
            {
                using (ServerManager iisManager = new ServerManager())
                {
                    Site site = GetSiteById(iisManager, websiteID);
                    if (site != null)
                    {
                        var coll = site.GetCollection();
                        ConfigurationElement ap = coll.Where((e) =>
                        {
                            return e != null && string.Compare(e.ElementTagName, "application", true) == 0 && e["path"].ToString() == @"/" + virtualDirName + @"/" + appName;
                        }).FirstOrDefault();

                        if (ap != null)
                        {
                            coll.Remove(ap);
                        }
                        ConfigurationElement app = coll.Where((e) =>
                        {
                            return e != null && string.Compare(e.ElementTagName, "application", true) == 0;
                        }).FirstOrDefault();

                        coll = app.GetCollection();
                        ConfigurationElement vir = coll.Where((e) =>
                        {
                            return e != null && string.Compare(e.ElementTagName, "virtualDirectory", true) == 0 && e["path"].ToString() == @"/" + virtualDirName;
                        }).FirstOrDefault();

                        if (vir != null)
                        {
                            coll.Remove(vir);
                        }

                        iisManager.CommitChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new IISException("Delete Application error - " + ex.Message, ex, IISVersion);
            }
        }

        public override bool DeleteApplication(string serverName, string websiteID, string virtualDirName)
        {
            try
            {
                using (ServerManager iisManager = new ServerManager())
                {
                    Site site = GetSiteById(iisManager, websiteID);
                    if (site != null)
                    {
                        Application app = site.Applications["/" + virtualDirName];
                        if (app != null)
                        {
                            site.Applications.Remove(app);
                            iisManager.CommitChanges();
                        }
                        return true;
                    }
                    else
                    {
                        throw new ArgumentNullException("WebSite is not existed.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new IISException("Delete Virtual Directory error. - " + ex.Message, ex, IISVersion);
            }
        }

        #endregion

        #region ApplicationPool IsExisted

        public override bool IsExistedApplicationPool(string serverName, string appPoolName)
        {
            using (ServerManager iisManager = new ServerManager())
            {
                foreach (ApplicationPool pool in iisManager.ApplicationPools)
                {
                    if (pool.Name == appPoolName)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        #endregion

        #region ApplicationPool Create

        public override IList<ApplicationPoolInfo> GetAllApplicationPoolList(string serverName)
        {
            using (ServerManager iisManager = new ServerManager())
            {
                var list = new List<ApplicationPoolInfo>();
                foreach (ApplicationPool pool in iisManager.ApplicationPools)
                {
                    var info = new ApplicationPoolInfo()
                    {
                        Name = pool.Name,
                        AutoStart = pool.AutoStart,
                        Enable32BitAppOnWin64 = pool.Enable32BitAppOnWin64,
                        ManagedRuntimeVersion = pool.ManagedRuntimeVersion,
                        QueueLength = pool.QueueLength,
                        State = pool.State,
                    };
                    list.Add(info);
                }
                return list;
            }
        }

        public override bool CreateApplicationPool(string serverName, string appPoolName)
        {
            try
            {
                using (ServerManager iisManager = new ServerManager())
                {
                    ApplicationPool pool = iisManager.ApplicationPools.Add(appPoolName);
                    pool.ProcessModel.IdentityType = ProcessModelIdentityType.ApplicationPoolIdentity;
                    iisManager.CommitChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new IISException("Create Application pool error - " + ex.Message, ex, IISVersion);
            }
        }

        public override bool ModifyApplicationPoolRuntimeVersion(string serverName, string appPoolName, string runtimeVersion)
        {
            using (ServerManager iisManager = new ServerManager())
            {
                foreach (ApplicationPool pool in iisManager.ApplicationPools)
                {
                    if (pool.Name == appPoolName)
                    {
                        pool.ManagedRuntimeVersion = runtimeVersion;
                        iisManager.CommitChanges();
                        return true;
                    }
                }
                return false;
            }
        }

        public override bool ModifyApplicationPoolIdentity(string serverName, string appPoolName, bool isCustom, int identityType, string userName, string password)
        {
            using (ServerManager iisManager = new ServerManager())
            {
                foreach (ApplicationPool pool in iisManager.ApplicationPools)
                {
                    if (pool.Name == appPoolName)
                    {
                        if (isCustom)
                        {
                            pool.ProcessModel.IdentityType = ProcessModelIdentityType.SpecificUser;
                            pool.ProcessModel.UserName = userName;
                            pool.ProcessModel.Password = password;
                        }
                        else
                        {
                            pool.ProcessModel.IdentityType = (ProcessModelIdentityType)identityType;
                        }
                        iisManager.CommitChanges();
                        return true;
                    }
                }
                return false;
            }
        }

        public override bool ModifyApplicationIdentity(string serverName, string websiteID, string virtualDirName, string userName, string password)
        {
            using (ServerManager iisManager = new ServerManager())
            {
                Site site = GetSiteById(iisManager, websiteID);
                if (site != null)
                {
                    foreach (Application app in site.Applications)
                    {
                        if (app.Path == "/" + virtualDirName)
                        {
                            foreach (VirtualDirectory vDir in app.VirtualDirectories)
                            {
                                vDir.UserName = userName;
                                vDir.Password = password;
                            }
                            iisManager.CommitChanges();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        #endregion

        #region ApplicationPool Delete

        public override bool DeleteApplicationPool(string serverName, string appPoolName)
        {
            try
            {
                using (ServerManager iisManager = new ServerManager())
                {
                    ApplicationPool pool = iisManager.ApplicationPools[appPoolName];
                    if (pool != null)
                    {
                        iisManager.ApplicationPools.Remove(pool);
                        iisManager.CommitChanges();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new IISException("Delete application pool error - " + ex.Message, ex, IISVersion);
            }
        }

        #endregion

        public override SiteInfo GetSiteById(string serverName, string siteId)
        {
            using (ServerManager iisManager = new ServerManager())
            {
                Site site = GetSiteById(iisManager, siteId);
                if (site != null)
                {
                    SiteInfo info = new SiteInfo();
                    info.Id = site.Id;
                    info.Name = site.Name;
                    info.Path = GetSitePath(site);
                    foreach (Binding binding in site.Bindings)
                    {
                        if (binding.Protocol.Equals("http", StringComparison.OrdinalIgnoreCase))
                        {
                            string[] items = binding.BindingInformation.Split(':');
                            info.Host = items[2];
                            info.Port = items[1];
                        }
                    }
                    return info;
                }
                return null;
            }
        }

        #region Helper Methods

        private Site GetSiteById(ServerManager iisManager, string siteId)
        {
            foreach (Site site in iisManager.Sites)
            {
                if (site.Id.ToString() == siteId)
                {
                    return site;
                }
            }
            return null;
        }
        private ConfigurationElement FindElement(ConfigurationElementCollection collection, string elementTagName, params string[] keyValues)
        {
            foreach (ConfigurationElement element in collection)
            {
                if (String.Equals(element.ElementTagName, elementTagName, StringComparison.OrdinalIgnoreCase))
                {
                    bool matches = true;
                    for (int i = 0; i < keyValues.Length; i += 2)
                    {
                        object o = element.GetAttributeValue(keyValues[i]);
                        string value = null;
                        if (o != null)
                        {

                            value = o.ToString();
                        }
                        if (!String.Equals(value, keyValues[i + 1], StringComparison.OrdinalIgnoreCase))
                        {
                            matches = false;
                            break;
                        }
                    }
                    if (matches)
                    {
                        return element;
                    }
                }
            }
            return null;
        }


        #endregion
    }
}
