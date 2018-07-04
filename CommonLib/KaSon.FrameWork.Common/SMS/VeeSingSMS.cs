
using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.Common.SMS
{
    /// <summary>
    /// 维信互动短信平台
    /// </summary>
    public class VeeSingSMS : ISMSSend
    {
        //GET短信接口地址
        //private static string msgUrl_Get = "http://121.199.16.178/webservice/sms.php?method=Submit&account=用户名&password=密码&mobile=手机号码&content=您的验证码是：4526。请不要把验证码泄露给其他人。";
        //POST短信提交接口
        private static string msgUrl_Post = "http://121.199.16.178/webservice/sms.php?method=Submit";//Submit:发送短信；
        //查询短信剩余条数
        private static string getBalanceUrl = "http://121.199.16.178/webservice/sms.php?method=GetNum";//GetNum:查询余额(string account,string password)
        //WebService短信提交接口
        //private static string msgUrl_wbService = "http://121.199.16.178/webservice/sms.php?WSDL";

        //用户账号
        private string account = "";
        //用户密码
        private string pwd = "";
        //短信地址
        private string url = "";
        public VeeSingSMS(string account, string pwd, string url)
        {
            this.account = account;
            this.pwd = pwd;
            if (!string.IsNullOrEmpty(url))
            {
                msgUrl_Post = string.Format("{0}/webservice/sms.php?method=Submit", url);
                getBalanceUrl = string.Format("{0}/webservice/sms.php?method=GetNum", url);
            }
        }
        /// <summary>
        /// 发送短信
        /// </summary>
        public string SendSMS(string mobile, string content, string attach)
        {
            string postData = string.Format("account={0}&password={1}&mobile={2}&content={3}", account, pwd, mobile, content);
            string result = PostManager.PostCustomer(msgUrl_Post, postData, Encoding.UTF8);
            return result;
        }
        /// <summary>
        /// 查询短信剩余条数
        /// </summary>
        public string GetBalance()
        {
            string postData = string.Format("account={0}&password={1}", account, pwd);
            string result = PostManager.PostCustomer(getBalanceUrl, postData, Encoding.UTF8);
            return result;
        }
        /// <summary>
        /// 暂时未用
        /// </summary>
        string ISMSSend.SendSMS_Batch(string mobileList, string content)
        {
            throw new NotImplementedException();
        }
    }
}
