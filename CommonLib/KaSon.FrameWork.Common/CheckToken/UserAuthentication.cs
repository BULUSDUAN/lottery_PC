using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.Common.CheckToken
{
    public class UserAuthentication
    {
        /// <summary>
        /// 获取用户口令
        /// </summary>
        /// <param name="userId">帐号</param>
        /// <returns>口令</returns>
        public static string GetUserToken(string userId, string loginName)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("LI", userId);
            dic.Add("LN", loginName);
            dic.Add("LD", DateTime.Now.ToString());
            return UserTokenHelper.GetUserToken(dic);
        }

        public static string GetUserToken(Dictionary<string, string> dic)
        {
            return UserTokenHelper.GetUserToken(dic);
        }
        /// <summary>
        /// 验证token
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public static string ValidateAuthentication(string userToken)
        {
            try
            {
                var rlt = UserTokenHelper.AnalyzeUserToken(userToken);
                if (!rlt.ContainsKey("LI"))
                {
                    throw new Exception("用户身份验证失败");
                }
                //if (!rlt.ContainsKey("LN"))
                //{
                //    throw new Exception("用户身份验证失败");
                //}
                string userid = rlt["LI"];
                if (string.IsNullOrEmpty(userid))
                    throw new Exception("用户身份验证失败");
                return userid;
            }
            catch (Exception ex)
            {
                throw new Exception("用户身份验证失败，请检查是否已登录", ex);
            }
        }
    }
}
