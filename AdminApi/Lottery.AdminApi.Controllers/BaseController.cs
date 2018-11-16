using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using EntityModel.CoreModel;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Net;
using KaSon.FrameWork.ORM.Helper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Lottery.AdminApi.Controllers
{

    // [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class BaseController : Controller
    {
        public static Dictionary<string, ShowUserBindInfo> LoginUser { get; set; }
        IKgLog log = null;
        public BaseController() {
            log = new Log4Log();
        }
        protected JsonResult JsonEx(object data)
        {
            JsonResult result = new JsonResult(data);
            //result.Data = data;
            //result.ContentType = contentType;
            //result.ContentEncoding = contentEncoding;
            //result.JsonRequestBehavior = behavior;
            return result;
        }
        /// <summary>
        /// 记录框架的异常信息
        /// </summary>
        /// <param name="ex">异常对象</param>
        public  void Log(string name,Exception ex)
        {
            log.Log(name,ex);
        }

        /// <summary>
        /// 记录框架的调试信息
        /// </summary>
        /// <param name="msg">调试信息字符串</param>
        public  void Log(string msg)
        {
            log.Log(msg);
        }
        public static string GetJsonData(string url)
        {
            try
            {
                var domain = ConfigHelper.AllConfigInfo["SelfDomain"] ?? "";
                url = domain + url;
                if (string.IsNullOrEmpty(url))
                    return string.Empty;
                var result = PostManager.Get(url, Encoding.UTF8);
                if (result == "404") return string.Empty;
                return result;
            }
            catch
            {
                return string.Empty;
            }
        }

        #region 各属性
        /// <summary>
        /// 当前用户
        /// </summary>
        public ShowUserBindInfo CurrentUser
        {
            get
            {
                var param = HttpContext.Request.Form["param"];
                var p = JsonHelper.Decode(param);
                string userToken = p.userToken;
                if (string.IsNullOrEmpty(userToken)) return new ShowUserBindInfo();
                var userId = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication_Admin(userToken);
                return GetUserInfo(userId);
                // var userInfo = HttpContext.Session.GetObj<LoginInfo>("CurrentUser");
                //return new LoginInfo()
                //{
                //    UserId = "10032",
                //    DisplayName = "laogan",
                //    IsAdmin = true,
                //    FunctionList=new List<string>()
                //};
            }
        }
        /// <summary>
        /// 默认页面索引
        /// </summary>
        public int PageIndex
        {
            get
            {
                return 0;
            }
        }
        /// <summary>
        /// 默认页面条数
        /// </summary>
        public int PageSize
        {
            get
            {
                return 30;
            }
        }
        #endregion

        #region 系统配置信息

        //public string AdminAgentToken
        //{
        //    get { return ConfigHelper.AllConfigInfo.GetString("AdminAgentToken"]; }
        //}
        //public string GatewayAdminToken
        //{
        //    get { return ConfigHelper.AllConfigInfo.GetString("GatewayAdminToken"]; }
        //}

        public string SiteName
        {
            get { return ConfigHelper.AllConfigInfo.GetString("SiteName"); }
        }
        public string WebSiteName
        {
            get { return ConfigHelper.AllConfigInfo.GetString("WebSiteName"); }
        }
        //public string WithdrawRemarkConfigFile
        //{
        //    get { return Server.MapPath("~/Configurations/" + SiteName + "/xmls/WithdrawRemark.Config.xml"); }
        //}
        public bool IsTest
        {
            get
            {
                if (string.IsNullOrEmpty(ConfigHelper.AllConfigInfo.GetString("IsTest")))
                { return false; }

                return bool.Parse(ConfigHelper.AllConfigInfo.GetString("IsTest"));
            }
        }

        #endregion

        //#region 获取接口配置信息
        ///// <summary>
        ///// 解析接口配置文件，并返回配置信息
        ///// </summary>
        ///// <param name="gatewayType">接口类型，必须与Config.xml里的子项名称一致</param>
        ///// <returns>配置信息</returns>
        //protected Dictionary<string, string> GetGatewayConfig(string gatewayType)
        //{
        //    var ret = new Dictionary<string, string>();
        //    XmlDocument doc = new XmlDocument();
        //    doc.Load(Server.MapPath("~/Mappings/Account.Config.xml"));
        //    var mappings = doc.GetElementsByTagName(gatewayType);
        //    foreach (XmlElement e in mappings)
        //    {
        //        ret.Add(e.Attributes["key"].Value, e.Attributes["value"].Value);
        //    }
        //    return ret;
        //}
        //#endregion

        //#region 导出excel
        //public void ExportExcelFromDataSet(DataSet ds, string fileName, string typeid = "1")
        //{
        //    var resp = Response;
        //    resp.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
        //    resp.ContentType = "application/ms-excel";

        //    resp.AddHeader("Content-Disposition", "attachment; filename=" + System.Web.HttpUtility.UrlEncode(fileName, Encoding.UTF8) + ".xls");

        //    string colHeaders = "", Is_item = "";
        //    int i = 0;

        //    //定义表对象与行对象，同时使用DataSet对其值进行初始化
        //    DataTable dt = ds.Tables[0];
        //    DataRow[] myRow = dt.Select("");
        //    //typeid=="1"时导出为Excel格式文件;typeid=="2"时导出为XML文件
        //    if (typeid == "1")
        //    {
        //        //取得数据表各列标题，标题之间以\t分割，最后一个列标题后加回车符
        //        for (i = 0; i < dt.Columns.Count; i++)
        //        {
        //            colHeaders += dt.Columns[i].Caption.ToString() + "\t";
        //        }
        //        colHeaders += "\n";

        //        resp.Write(colHeaders);
        //        //逐行处理数据
        //        foreach (DataRow row in myRow)
        //        {
        //            //在当前行中，逐列取得数据，数据之间以\t分割，结束时加回车符\n
        //            for (i = 0; i < dt.Columns.Count; i++)
        //            {
        //                Is_item += row[i].ToString() + "\t";
        //            }
        //            Is_item += "\n";
        //            resp.Write(Is_item);
        //            Is_item = "";
        //        }
        //    }
        //    else
        //    {
        //        if (typeid == "2")
        //        {
        //            //从DataSet中直接导出XML数据并且写到HTTP输出流中
        //            resp.Write(ds.GetXml());
        //        }
        //    }
        //    //写缓冲区中的数据到HTTP头文件中
        //    resp.End();
        //}
        //#endregion


        public string LoadImageFile(IFormFile file, string uploadfile, string imgName = "")
        {
            try
            {
                string nameImg = string.Empty;
                if (string.IsNullOrEmpty(imgName))
                    nameImg = DateTime.Now.ToString("yyyyMMddHHmmssff");
                else nameImg = imgName;

                string resourceSiteUrl = ConfigHelper.AllConfigInfo.GetString("ResourceSiteUrl");
                string resourceSitePostUrl = ConfigHelper.AllConfigInfo.GetString("ResourceSitePostUrl");

                string upLoadFile = uploadfile;  // "/images/add/";
                string upLoadPostPath = ConfigHelper.AllConfigInfo.GetString("UpLoadPostPath");

                nameImg += file.FileName.Substring(file.FileName.LastIndexOf(".")).ToLower();
                string url = string.Format("{0}{1}{2}", resourceSiteUrl, upLoadFile, nameImg);

                upLoadFile = "/" + ConfigHelper.AllConfigInfo.GetString("WebSiteEName") + upLoadFile;

                string postUrl = string.Format("{0}{1}?filename={2}&upLoadFile={3}", resourceSitePostUrl, upLoadPostPath, nameImg, upLoadFile);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postUrl);
                request.Method = "POST";
                request.AllowAutoRedirect = false;
                request.ContentType = "multipart/form-data";
                byte[] bytes = new byte[file.Length];
                var st = new MemoryStream();
                file.CopyTo(st);
                st.Read(bytes, 0, (int)file.Length);
                request.ContentLength = bytes.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }
                HttpWebResponse respon = (HttpWebResponse)request.GetResponse();
                //Hashtable hash = new Hashtable();
                //hash["error"] = 0;
                //hash["url"] = url;
                //return Content(System.Web.Helpers.Json.Encode(hash), "text/html; charset=UTF-8");
                return url;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 彩富上传图片
        /// </summary>
        /// <param name="file"></param>
        /// <param name="uploadfile"></param>
        /// <returns></returns>
        public string LoadImageFile_Tgbank(IFormFile file, string uploadfile)
        {
            try
            {
                string nameImg = DateTime.Now.ToString("yyyyMMddHHmmssff");

                string resourceSiteUrl = ConfigHelper.AllConfigInfo.GetString("ResourceSiteUrl_Tgbank");
                string resourceSitePostUrl = ConfigHelper.AllConfigInfo.GetString("ResourceSitePostUrl_Tgbank");

                string upLoadFile = uploadfile;  // "/images/add/";
                string upLoadPostPath = ConfigHelper.AllConfigInfo.GetString("UpLoadPostPath");

                nameImg += file.FileName.Substring(file.FileName.LastIndexOf(".")).ToLower();
                string url = string.Format("{0}{1}{2}", resourceSiteUrl, upLoadFile, nameImg);

                upLoadFile = "/" + ConfigHelper.AllConfigInfo.GetString("WebSiteEName_Tgbank") + upLoadFile;

                string postUrl = string.Format("{0}{1}?filename={2}&upLoadFile={3}", resourceSitePostUrl, upLoadPostPath, nameImg, upLoadFile);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postUrl);
                request.Method = "POST";
                request.AllowAutoRedirect = false;
                request.ContentType = "multipart/form-data";
                byte[] bytes = new byte[file.Length];
                var st = new MemoryStream();
                file.CopyTo(st);
                st.Read(bytes, 0, (int)file.Length);
                request.ContentLength = bytes.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }
                HttpWebResponse respon = (HttpWebResponse)request.GetResponse();
                //Hashtable hash = new Hashtable();
                //hash["error"] = 0;
                //hash["url"] = url;
                //return Content(System.Web.Helpers.Json.Encode(hash), "text/html; charset=UTF-8");
                return url;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="msgContent"></param>
        /// <returns></returns>
        protected string SendMessage(string mobile, string msgContent)
        {
            //var agentName = this.GameClient.QueryCoreConfigByKey("SMSAgent.Name").ConfigValue;
            //var attach = this.GameClient.QueryCoreConfigByKey("SMSAgent.Attach").ConfigValue;
            //var password = this.GameClient.QueryCoreConfigByKey("SMSAgent.Password").ConfigValue;
            //var userName = this.GameClient.QueryCoreConfigByKey("SMSAgent.UserId").ConfigValue;
            //var returnted = SMSSenderFactory.GetSMSSenderInstance(new SMSConfigInfo
            //{
            //    AgentName = agentName,
            //    Attach = attach,
            //    Password = password,
            //    UserName = userName
            //}).SendSMS(mobile, msgContent, attach);
            //return returnted;
            return string.Empty;
        }
        public string GetGameCodeName(string strGameCode)
        {
            switch (strGameCode)
            {
                case "CQSSC": return "重庆时时彩";
                case "DLT": return "大乐透";
                case "FC3D": return "福彩3D";
                case "GD11X5": return "广东十一选五";
                case "GDKLSF": return "广东快乐十分";
                case "GXKLSF": return "广西快乐十分";
                case "JSKS": return "江苏快3";
                case "JX11X5": return "江西十一选五";
                case "JXSSC": return "江西时时彩";
                case "PL3": return "排列三";
                case "QLC": return "七乐彩";
                case "QXC": return "七星彩";
                case "SD11X5": return "山东十一选五";
                case "SDQYH": return "山东群英会";
                case "SSQ": return "双色球";
                case "CQ11X5": return "重庆十一选五";
                case "CQKLSF": return "重庆快乐十分";
                case "DF6J1": return "东方6+1";
                case "HBK3": return "湖北快3";
                case "HC1": return "好彩一";
                case "HD15X5": return "华东十五选五";
                case "HNKLSF": return "湖南快乐十分";
                case "JLK3": return "新快3";
                case "JSK3": return "江苏快3";
                case "LN11X5": return "辽宁十一选五";
                case "PL5": return "排列五";
                case "BJDC": return "北京单场";
                case "JCZQ": return "竞彩足球";
                case "JCLQ": return "竞彩篮球";
                case "CTZQ": return "传统足球";
                default: return string.Empty;
            }
        }

        /// <summary>
        /// 检查是否有权限
        /// </summary>
        /// <param name="needFunction"></param>
        /// <returns></returns>
        public bool CheckRights(string needFunction)
        {
            return true;
            try
            {
                if (CurrentUser != null)
                {
                    if (CurrentUser.FunctionList.Contains(needFunction) || CurrentUser.IsAdmin)
                        return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 发送生成静态数据通知
        /// </summary>
        public void SendBuildStaticDataNotice(string pageType, string key)
        {
            var urlArray = ConfigHelper.AllConfigInfo.GetString("BuildStaticFileSendUrl").Split('|');
            foreach (var url in urlArray)
            {
                if (string.IsNullOrEmpty(url))
                    continue;
                var code = Encipherment.MD5(string.Format("Home_BuildSpecificPage_{0}", pageType), Encoding.UTF8);
                var webSiteUrl = string.Format("{0}/{1}?pageType={2}&code={3}&key={4}", url, "StaticHtml/BuildSpecificPage", pageType, code, key);
                var result = PostManager.Get(webSiteUrl, Encoding.UTF8, timeoutSeconds: 60);
            }

        }

        /// <summary>
        /// 充值回调域名
        /// </summary>
        public string FillMoneyCallBackDomain
        {
            get
            {
                string defalutValue = "";
                try
                {
                    var business = new CacheDataBusiness();
                    var v = business.QueryCoreConfigByKey("FillMoney.CallBackDomain").ConfigValue;
                    if (string.IsNullOrEmpty(v))
                    {
                        return defalutValue;
                    }
                    else
                    {
                        return v;
                    }
                }
                catch (Exception)
                {
                    return defalutValue;
                }
            }
        }

        /// <summary>
        /// 登陆时间超过两小时则清除
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public static bool UserIsLogin(string UserId)
        {
            try
            {
                var info = LoginUser[UserId];
                if (info == null) return false;
                var now = DateTime.Now;
                if (info.LoginTime < now.AddHours(-2))
                {
                    LoginUser.Remove(UserId);
                    return false;
                }
                else
                {
                    info.LoginTime = now;
                    LoginUser[UserId] = info;
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        public static ShowUserBindInfo GetUserInfo(string UserId)
        {
            var info = LoginUser[UserId];
            return info;
        }

        public static void SetUser(LoginInfo info)
        {
            ShowUserBindInfo model = new ShowUserBindInfo()
            {
                IsSuccess = true,
                Message = "登录成功",
                CreateTime = info.CreateTime,
                LoginFrom = "ADMIN",
                RegType = info.RegType,
                Referrer = info.Referrer,
                UserId = info.UserId,
                VipLevel = info.VipLevel,
                LoginName = info.LoginName,
                DisplayName = info.LoginName,
                UserToken = info.UserToken,
                FunctionList = info.FunctionList,
                IsAdmin = info.IsAdmin,
                LoginTime=DateTime.Now
            };
            if (LoginUser == null) LoginUser = new Dictionary<string, ShowUserBindInfo>();
             LoginUser[info.UserId] = model;
        }
    }
}
