using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Common.Net
{
    /// <summary>
    /// Url辅助函数
    /// </summary>
    public static class UrlHelper
    {
        /// <summary>
        /// 获取指定链接的根目录
        /// </summary>
        public static string GetRootUrl(string url)
        {
            var uriOld = new Uri(url);
            return uriOld.GetLeftPart(UriPartial.Authority);
        }
        /// <summary>
        /// 获取指定链接的上级目录
        /// </summary>
        public static string GetParentUrl(string url)
        {
            var uriOld = new Uri(url);
            var dir = uriOld.GetLeftPart(UriPartial.Path);
            if (dir.EndsWith("/"))  // 是目录
            {
                dir = dir.TrimEnd('/');
            }
            if (GetRootUrl(dir).Equals(dir, StringComparison.OrdinalIgnoreCase))
            {
                return dir + "/";
            }
            var tmp = uriOld.GetLeftPart(UriPartial.Path);
            dir = dir.Substring(0, dir.LastIndexOf("/"));
            return dir + "/";

            //if (uriOld.AbsolutePath.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
            //{
            //    var tmp = uriOld.GetLeftPart(UriPartial.Path);
            //    dir = tmp.Substring(0, uriOld.GetLeftPart(UriPartial.Path).LastIndexOf("/") + 1);
            //}
            //else
            //{
            //    if (!uriOld.AbsolutePath.EndsWith("/"))
            //    {
            //        dir = uriOld.GetLeftPart(UriPartial.Path) + "/";
            //    }
            //}
            //return dir;
        }
        /// <summary>
        /// 获取指定链接的完整链接
        /// </summary>
        /// <param name="currentUrl">当前链接</param>
        /// <param name="url">指定链接。可为相对路径货绝对路径</param>
        public static string GetFullUrl(string currentUrl, string url)
        {
            if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            {
                return url;
            }
            else if (url.StartsWith("/"))
            {
                return GetRootUrl(currentUrl) + url;
            }
            else
            {
                return GetParentUrl(currentUrl) + url;
            }
        }
        public static string GetSiteRoot(HttpRequestBase request)
        {
            string UrlAuthority = request.Url.GetLeftPart(UriPartial.Authority);
            if (request.ApplicationPath == null || request.ApplicationPath == "/")
            {
                //直接安装在Web站点
                return UrlAuthority;
            }
            else
            {
                //安装在虚拟子目录下
                return UrlAuthority + request.ApplicationPath;
            }
        }
    }
}
