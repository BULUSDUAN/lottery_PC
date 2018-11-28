using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.DirectoryServices;

namespace Common.IISHelper
{
    public class IISOperator_V6 : IISOperator
    {
        public override bool SetAuthenticationConfig(string locationPath, bool windowsAuthenticationEnabled)
        {
            return false;
        }
        public override bool CreateRootVirtualDir(string serverName, string virtualDirName, string physicalDir, string websiteID)
        {
            return false;
        }
        public override bool CreateSubApplication(string serverName, string virtualDirName, string appName, string appPoolName, string physicalDir, string websiteID)
        {
            return false;
        }
        public override bool ModifyRootVirtualDir(string serverName, string oldVirDirName, string newVirDirName, string websiteID)
        {
            return false;
        }
        public override bool DeleteRootVirtualDir(string serverName, string virtualDirName, string websiteID)
        {
            return false;
        }
        public override IList<ApplicationPoolInfo> GetAllApplicationPoolList(string serverName)
        {
            return null;
        }
        public override bool ModifyApplicationPoolRuntimeVersion(string serverName, string appPoolName, string runtimeVersion)
        {
            return false;
        }
        public override bool SetWebSiteApplicationPool(string serverName, string websiteID, string applicationPool)
        {
            return false;
        }
        #region WebSite IsExisted

        public override bool IsExistedWebSite(string serverName, string websiteName)
        {
            if (string.IsNullOrEmpty(serverName)) serverName = "localhost";
            string entPath = String.Format("IIS://{0}/w3svc", serverName);
            DirectoryEntry ent = new DirectoryEntry(entPath);
            foreach (DirectoryEntry child in ent.Children)
            {
                if (child.SchemaClassName == "IIsWebServer")
                {
                    string id = child.Name;
                    string name = (string)child.InvokeGet("ServerComment");
                    if (websiteName == id || websiteName == name)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        #region HostHeader IsExisted

        public override bool IsExistedHostHeader(string serverName, string hostHeader, string port)
        {
            if (string.IsNullOrEmpty(serverName)) serverName = "localhost";
            string entPath = String.Format("IIS://{0}/w3svc", serverName);
            DirectoryEntry ent = new DirectoryEntry(entPath);
            foreach (DirectoryEntry child in ent.Children)
            {
                if (child.SchemaClassName == "IIsWebServer")
                {
                    object[] bindings = (object[])child.InvokeGet("ServerBindings");
                    if (bindings != null && bindings.Length > 0)
                    {
                        string name = (string)bindings[0];
                        string check = ":" + port + ":" + hostHeader;
                        if (name.Equals(check, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        #endregion

        #region WebSite GetDefult

        public override string GetDefaultWebSiteID(string serverName, out string siteName)
        {
            if (string.IsNullOrEmpty(serverName)) serverName = "localhost";
            string result = "-1";

            result = GetWebSiteID(serverName, "", "80", "");
            if ("" == result)
                result = "-1";

            siteName = "";
            return result;
        }

        // 获取一个网站的编号。根据网站的ServerBindings确定网站编号
        private string GetWebSiteID(string serverName, string IPAddress, string TCPPort, string hostHeader)
        {
            string bindStr = IPAddress + ":" + TCPPort + ":" + hostHeader;
            string entPath = String.Format("IIS://{0}/w3svc", serverName);
            DirectoryEntry ent = new DirectoryEntry(entPath);
            foreach (DirectoryEntry child in ent.Children)
            {
                if (child.SchemaClassName == "IIsWebServer")
                {
                    if (child.Properties["ServerBindings"].Value != null)
                    {
                        if (child.Properties["ServerBindings"].Value.ToString() == bindStr)
                        {
                            return child.Name;
                        }
                    }
                }
            }
            return "";
        }

        #endregion

        #region WebSite GetAllSiteList

        public override IList<SiteInfo> GetAllSiteList(string serverName)
        {
            IList<SiteInfo> list = new List<SiteInfo>();
            return list;
        }

        public override SiteInfo GetSiteByName(string serverName, string siteName)
        {
            return null;
        }

        public override SiteInfo GetSiteById(string serverName, string siteId)
        {
            return null;
        }

        #endregion

        #region WebSite Create

        public override string CreateWebSite(string serverName, string webSiteName, string IPAddress, string TCPPort, string hostHeader, string localPath)
        {
            if (string.IsNullOrEmpty(serverName)) serverName = "localhost";
            int siteID = GetNewWebSiteID(serverName);
            try
            {
                DirectoryEntry root = new DirectoryEntry("IIS://" + serverName + "/W3SVC");
                DirectoryEntry site = (DirectoryEntry)root.Invoke("Create", "IIsWebServer", siteID);
                site.Invoke("Put", "ServerComment", webSiteName);
                site.Invoke("Put", "KeyType", "IIsWebServer");
                site.Invoke("Put", "ServerBindings", IPAddress + ":" + TCPPort + ":" + hostHeader);
                site.Invoke("Put", "ServerState", 2);
                site.Invoke("Put", "FrontPageWeb", 1);
                site.Invoke("Put", "DefaultDoc", "index.aspx,index.html,index.html,default.aspx,default.htm,default.html");
                site.Invoke("Put", "ServerAutoStart", 1);
                site.Invoke("Put", "ServerSize", 1);
                site.Invoke("SetInfo");

                DirectoryEntry siteVDir = site.Children.Add("Root", "IISWebVirtualDir");
                siteVDir.Properties["AppIsolated"][0] = 2;
                siteVDir.Properties["Path"][0] = localPath;
                siteVDir.Properties["AccessFlags"][0] = 513;
                siteVDir.Properties["FrontPageWeb"][0] = 1;
                siteVDir.Properties["AppRoot"][0] = "LM/W3SVC/" + siteID + "/Root";
                siteVDir.Properties["AppFriendlyName"][0] = "ROOT";
                siteVDir.CommitChanges();
                site.CommitChanges();

                SetCustomHTTPheaders("IIS://" + serverName + "/W3SVC/" + siteID + "/ROOT", "X-UA-Compatible", "IE=EmulateIE7");

                return siteID.ToString();
            }
            catch (Exception ex)
            {
                throw new IISException("Create WebSite error - " + ex.Message, ex, IISVersion);
            }
        }

        /// <summary>
        /// 得到能建站点的最小编号
        /// </summary>
        private int GetNewWebSiteID(string serverName)
        {
            ArrayList list = new ArrayList();
            string tmpStr;
            string entPath = String.Format("IIS://{0}/w3svc", serverName);
            DirectoryEntry ent = new DirectoryEntry(entPath);
            foreach (DirectoryEntry child in ent.Children)
            {
                if (child.SchemaClassName == "IIsWebServer")
                {
                    tmpStr = child.Name.ToString();
                    list.Add(Convert.ToInt32(tmpStr));
                }
            }
            list.Sort();
            int i = 1;
            foreach (int j in list)
            {
                if (i == j)
                {
                    i++;
                }
            }
            return i;
        }

        #endregion

        #region WebSite Update

        public override bool UpdateWebSite(string serverName, string webSiteID, string IPAddress, string TCPPort, string hostHeader, string webSiteName, string localPath)
        {
            try
            {
                DirectoryEntry siteEntry = new DirectoryEntry("IIS://localhost/W3SVC/" + webSiteID);
                if (webSiteName.Trim() != "")
                {
                    siteEntry.Invoke("Put", "ServerComment", webSiteName);
                }
                siteEntry.Invoke("Put", "ServerBindings", IPAddress + ":" + TCPPort + ":" + hostHeader);

                if (localPath.Trim() != "")
                {
                    DirectoryEntry siteVDir = siteEntry.Children.Find("Root", "IISWebVirtualDir");
                    siteVDir.Properties["Path"][0] = localPath;
                    siteVDir.CommitChanges();
                }

                siteEntry.CommitChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw new IISException("Update WebSite error - " + ex.Message, ex, IISVersion);
            }
        }

        #endregion

        #region WebSite Delete

        public override bool DeleteWebSite(string serverName, string websiteID)
        {
            if (string.IsNullOrEmpty(serverName)) serverName = "localhost";
            try
            {
                string siteEntPath = String.Format("IIS://{0}/w3svc/{1}", serverName, websiteID);
                DirectoryEntry siteEntry = new DirectoryEntry(siteEntPath);
                string rootPath = String.Format("IIS://{0}/w3svc", serverName);
                DirectoryEntry rootEntry = new DirectoryEntry(rootPath);
                rootEntry.Children.Remove(siteEntry);
                rootEntry.CommitChanges();
                return true;
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
            if (string.IsNullOrEmpty(serverName)) serverName = "localhost";
            bool exited = false;
            DirectoryEntry _entry = new DirectoryEntry("IIS://" + serverName + "/W3SVC/" + websiteID + "/ROOT");
            DirectoryEntries _entries = _entry.Children;
            foreach (DirectoryEntry _cen in _entries)
            {
                if (_cen.Name == virtualDirName)
                    exited = true;
            }
            return exited;
        }

        #endregion

        #region VirtualDir Create

        public override bool CreateApplication(string serverName, string virtualDirName, string physicalDir, string defaultPage, string websiteID, string appPoolName)
        {
            if (string.IsNullOrEmpty(serverName)) serverName = "localhost";
            try
            {
                string constIISWebSiteRoot = "IIS://" + serverName + "/W3SVC/" + websiteID + "/ROOT";
                DirectoryEntry root = new DirectoryEntry(constIISWebSiteRoot);

                DirectoryEntry tbEntry = root.Children.Add(virtualDirName, root.SchemaClassName);
                tbEntry.Properties["Path"][0] = physicalDir;//设置物理地址
                tbEntry.Properties["DefaultDoc"][0] = defaultPage;//设置起始页
                tbEntry.Invoke("AppCreate", true);
                if (appPoolName.Trim() == "")
                    appPoolName = "DefaultAppPool";
                object[] poolParm = { 0, appPoolName, true };
                tbEntry.Invoke("Appcreate3", poolParm);

                tbEntry.CommitChanges();

                bool result = true;
                if (!SetMimeTypeProperty(constIISWebSiteRoot + "/" + virtualDirName, ".xaml", "application/xaml+xml"))
                {
                    result = false;
                }
                if (!SetMimeTypeProperty(constIISWebSiteRoot + "/" + virtualDirName, ".xap", "application/x-silverlight-app"))
                {
                    result = false;
                }
                if (!SetCustomHTTPheaders(constIISWebSiteRoot, "X-UA-Compatible", "IE=EmulateIE7"))
                {
                    result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new IISException("Create Virtual directory error - " + ex.Message, ex, IISVersion);
            }
        }

        #endregion

        #region VirtualDir Delete

        public override bool DeleteSubApplication(string serverName, string websiteID, string virtualDirName, string appName)
        {
            return false;
        }

        public override bool DeleteApplication(string serverName, string websiteID, string virtualDirName)
        {
            if (string.IsNullOrEmpty(serverName)) serverName = "localhost";
            try
            {
                string constIISWebSiteRoot = "IIS://" + serverName + "/W3SVC/" + websiteID + "/ROOT";
                DirectoryEntry root = new DirectoryEntry(constIISWebSiteRoot);

                object[] paras = new object[2];
                paras[0] = "IIsVirtualDir";
                paras[1] = virtualDirName;
                root.Invoke("Delete", paras);

                root.CommitChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new IISException("Delete Virtual directory error - " + ex.Message, ex, IISVersion);
            }
        }

        #endregion

        #region ApplicationPool IsExisted

        public override bool IsExistedApplicationPool(string serverName, string appPoolName)
        {
            if (string.IsNullOrEmpty(serverName)) serverName = "localhost";
            string constIISAppPoolsRoot = "IIS://" + serverName + "/W3SVC/AppPools";
            DirectoryEntry appPools = new DirectoryEntry(constIISAppPoolsRoot);

            try
            {
                foreach (DirectoryEntry eachPool in appPools.Children)
                {
                    if (eachPool.Name == appPoolName)
                    {
                        return true;
                    }
                }
            }
            catch { return false; }

            return false;
        }

        #endregion

        #region ApplicationPool Create

        public override bool CreateApplicationPool(string serverName, string appPoolName)
        {
            if (string.IsNullOrEmpty(serverName)) serverName = "localhost";
            try
            {
                string constIISAppPoolsRoot = "IIS://" + serverName + "/W3SVC/AppPools";
                DirectoryEntry appPools = new DirectoryEntry(constIISAppPoolsRoot);

                // 不存在同名应用程序池就添加
                if (!IsExistedApplicationPool(serverName, appPoolName))
                {
                    DirectoryEntry newPool = appPools.Children.Add(appPoolName, "IIsApplicationPool");
                    newPool.CommitChanges();
                    newPool.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new IISException("Create application pool error - " + ex.Message, ex, IISVersion);
            }
        }

        public override bool ModifyApplicationPoolIdentity(string serverName, string appPoolName, bool isCustom, int identityType, string userName, string password)
        {
            // IIS6 暂未实现设置运行标识的功能
            return false;
        }

        public override bool ModifyApplicationIdentity(string serverName, string websiteID, string virtualDirName, string userName, string password)
        {
            // IIS6 暂未实现设置运行标识的功能
            return false;
        }

        public override bool ModifySubApplicationIdentity(string serverName, string websiteID, string virtualDirName, string appName, string userName, string password)
        {
            // IIS6 暂未实现设置运行标识的功能
            return false;
        }

        #endregion

        #region ApplicationPool Delete

        public override bool DeleteApplicationPool(string serverName, string appPoolName)
        {
            if (string.IsNullOrEmpty(serverName)) serverName = "localhost";
            string constIISAppPoolsRoot = "IIS://" + serverName + "/W3SVC/AppPools/" + appPoolName;
            try
            {
                DirectoryEntry tree = new DirectoryEntry(constIISAppPoolsRoot);
                string rootPath = String.Format("IIS://{0}/w3svc/AppPools", serverName);
                DirectoryEntry rootEntry = new DirectoryEntry(rootPath);
                rootEntry.Children.Remove(tree);
                rootEntry.CommitChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw new IISException("Delete application pool error - " + ex.Message, ex, IISVersion);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// 设置Customer HTTP header
        /// </summary>
        private bool SetCustomHTTPheaders(string metabasePath, string customHeaderName, string customHeaderNameValue)
        {
            try
            {
                DirectoryEntry path = new DirectoryEntry(metabasePath);
                PropertyValueCollection propValues = path.Properties["HttpCustomHeaders"];

                object exists = null;
                foreach (object value in propValues)
                {
                    string[] a;
                    a = value.ToString().Split(':');
                    if (a[0] == customHeaderName.Trim())
                        exists = value;
                }

                if (null != exists)
                {
                    propValues.Remove(exists);
                }

                string saveValue = customHeaderName + ":" + customHeaderNameValue;
                propValues.Add((object)saveValue);
                path.CommitChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw new IISException("Set Customer HTTP header error - " + ex.Message, ex, IISVersion);
            }
        }

        private bool SetMimeTypeProperty(string metabasePath, string newExtension, string newMimeType)
        {
            try
            {
                DirectoryEntry path = new DirectoryEntry(metabasePath);
                PropertyValueCollection propValues = path.Properties["MimeMap"];

                path.CommitChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw new IISException("Set Mime Type Property error - " + ex.Message, ex, IISVersion);
            }
        }

        #endregion
    }
}
