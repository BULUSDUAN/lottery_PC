using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common.Log;
using DS.Web.UCenter;

namespace app.lottery.site
{
    /// <summary>
    /// uc 的摘要说明
    /// </summary>
    public class uc : DS.Web.UCenter.Api.UcApiBase, IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            //todo:接收到UCenter通知，用数据做登录等操作

            var postlogList = new List<string>();
            foreach (string key in context.Request.Form.Keys)
            {
                postlogList.Add(string.Format("{0}=>{1}", key, context.Request.Form[key]));
            }
            this.WriteLog("POST参数:" + Environment.NewLine + string.Join(Environment.NewLine, postlogList.ToArray()));


            var getlogList = new List<string>();
            foreach (string key in context.Request.QueryString.Keys)
            {
                getlogList.Add(string.Format("{0}=>{1}", key, context.Request.QueryString[key]));
            }
            this.WriteLog("GET参数:" + Environment.NewLine + string.Join(Environment.NewLine, getlogList.ToArray()));

            if (context.Request.QueryString.AllKeys.Contains("code"))
            {
                try
                {
                    var code = context.Request.QueryString["code"];
                    var str = UcUtility.AuthCodeDecode(code);
                    this.WriteLog("解密后参数：" + str);
                }
                catch (Exception ex)
                {
                    this.WriteLog("解密后error：" + ex.ToString());
                }
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write("1");

            //return Content("1");
        }

        private static ILogWriter log = Common.Log.LogWriterGetter.GetLogWriter();
        private void WriteLog(string txt)
        {
            log = Common.Log.LogWriterGetter.GetLogWriter();
            log.Write("userController.", DateTime.Now.ToString("yyyyMMddHHmmssffffff"), LogType.Information, "BBS通信日志", txt);
        }

        //public bool IsReusable
        //{
        //    get
        //    {
        //        return false;
        //    }
        //}



        public override DS.Web.UCenter.Api.ApiReturn DeleteUser(IEnumerable<int> ids)
        {
            this.WriteLog("DeleteUser " + string.Join(",", ids.ToArray()));
            return DS.Web.UCenter.Api.ApiReturn.Success;
        }

        public override DS.Web.UCenter.Api.ApiReturn GetCredit(int uid, int credit)
        {
            this.WriteLog(string.Format("GetCredit uid:{0},credit:{1}", uid, credit));
            return DS.Web.UCenter.Api.ApiReturn.Success;
        }

        public override UcCreditSettingReturns GetCreditSettings()
        {
            this.WriteLog("GetCreditSettings");
            return new UcCreditSettingReturns();
        }

        public override UcTagReturns GetTag(string tagName)
        {
            this.WriteLog(string.Format("GetTag tagName:{0}", tagName));
            return new UcTagReturns(tagName);
        }

        public override DS.Web.UCenter.Api.ApiReturn RenameUser(int uid, string oldUserName, string newUserName)
        {
            this.WriteLog(string.Format("RenameUser uid:{0},oldUserName:{1},newUserName:{2}", uid, oldUserName, newUserName));
            return DS.Web.UCenter.Api.ApiReturn.Success;
        }

        public override DS.Web.UCenter.Api.ApiReturn SynLogin(int uid)
        {
            this.WriteLog(string.Format("SynLogin uid:{0}", uid));
            return DS.Web.UCenter.Api.ApiReturn.Success;
        }

        public override DS.Web.UCenter.Api.ApiReturn SynLogout()
        {
            this.WriteLog("SynLogout");
            return DS.Web.UCenter.Api.ApiReturn.Success;
        }

        public override DS.Web.UCenter.Api.ApiReturn UpdateApps(UcApps apps)
        {
            this.WriteLog("UpdateApps");
            return DS.Web.UCenter.Api.ApiReturn.Success;
        }

        public override DS.Web.UCenter.Api.ApiReturn UpdateBadWords(UcBadWords badWords)
        {
            return DS.Web.UCenter.Api.ApiReturn.Success;
        }

        public override DS.Web.UCenter.Api.ApiReturn UpdateClient(UcClientSetting client)
        {
            return DS.Web.UCenter.Api.ApiReturn.Success;
        }

        public override DS.Web.UCenter.Api.ApiReturn UpdateCredit(int uid, int credit, int amount)
        {
            return DS.Web.UCenter.Api.ApiReturn.Success;
        }

        public override DS.Web.UCenter.Api.ApiReturn UpdateCreditSettings(UcCreditSettings creditSettings)
        {
            return DS.Web.UCenter.Api.ApiReturn.Success;
        }

        public override DS.Web.UCenter.Api.ApiReturn UpdateHosts(UcHosts hosts)
        {
            return DS.Web.UCenter.Api.ApiReturn.Success;
        }

        public override DS.Web.UCenter.Api.ApiReturn UpdatePw(string userName, string passWord)
        {
            this.WriteLog(string.Format("UpdatePw userName:{0},passWord:{1}", userName, passWord));
            return DS.Web.UCenter.Api.ApiReturn.Success;
        }
    }
}