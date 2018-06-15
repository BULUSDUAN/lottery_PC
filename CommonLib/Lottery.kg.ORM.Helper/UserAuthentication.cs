using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;
using KaSon.FrameWork.Helper;

namespace Lottery.Kg.ORM.Helper
{
   public class UserAuthentication:DBbase
    {
        private static List<C_Auth_MethodFunction_List> _allMethodFunctionList = new List<C_Auth_MethodFunction_List>();
        /// <summary>
        /// 验证用户是否具有该方法的权限
        /// </summary>
        public  string ValidateUserAuthentication(string userToken)
        {
            if (_allMethodFunctionList == null || _allMethodFunctionList.Count == 0)
            {                

                _allMethodFunctionList = DB.CreateQuery<C_Auth_MethodFunction_List>().ToList();
            }

            var method = new System.Diagnostics.StackFrame(1).GetMethod();
            var currentFullName = string.Format("{0}.{1}", method.ReflectedType.FullName, method.Name);
            var config = _allMethodFunctionList.FirstOrDefault(p => p.MethodFullName == currentFullName);
            if (config == null)
                throw new Exception(string.Format("没有配置方法 {0} 的调用权限数据", currentFullName));

            var userId = string.Empty;
            ValidateAuthentication(userToken, config.Mode, config.FunctionId, out userId);
            return userId;
        }
        /// <summary>
        /// 验证用户身份及权限，并返回用户名称
        /// </summary>
        public static void ValidateAuthentication(string userToken, string needRight, string functionId, out string userId)
        {
            try
            {
                var rlt = ValidateAuthentication(userToken, needRight, functionId, "LI");
                if (!rlt.ContainsKey("LI"))
                {
                    throw new Exception("UserToken不完整，缺少UserId信息");
                }
                userId = rlt["LI"];
            }
            catch (Exception ex)
            {
                throw new Exception("用户身份验证失败，请检查是否已登录", ex);
            }
        }
        /// <summary>
        /// 验证用户身份及权限，并返回用户名称
        /// </summary>
        public static Dictionary<string, string> ValidateAuthentication(string userToken, string needRight, string functionId, params string[] keys)
        {
            var dic = UserTokenHandler.AnalyzeUserToken(userToken);
            if (!dic.ContainsKey("IA") || dic["IA"] != "IA")
            {
                if (!dic[functionId].Contains(needRight))
                {
                    throw new Exception("权限不足");
                }
            }
            var rlt = new Dictionary<string, string>(keys.Length);
            foreach (var key in keys)
            {
                rlt.Add(key, dic[key]);
            }
            return rlt;
        }
       
    }
    public static class UserTokenHandler
    {
        private const CryptType _type = CryptType.Rijndael;
        private const string _key = "S312ifM8gM5LnWS3nK+6MI+33nqTJUPu+EONiDVZ4oY=";
        private const string _iv = "r+bveoBETg6ENOitGw+4IQ==";
        private const char _spliter_outer = (char)20;
        private const char _spliter_inner = (char)21;
        public static string GetUserToken(Dictionary<string, string> acl)
        {
            List<string> tokenList = new List<string>();
            foreach (var key in acl.Keys)
            {
                tokenList.Add(string.Format("{1}{0}{2}", _spliter_inner, key, acl[key]));
            }
            var userToken = string.Join(_spliter_outer.ToString(), tokenList.ToArray());
            SymmetricCrypt crypt = new SymmetricCrypt(_type);
            return crypt.Encrypt(userToken, _key, _iv);
        }
        public static bool IsAdmin(string userToken)
        {
            var dic = AnalyzeUserToken(userToken);
            return dic.ContainsKey("IA") && dic["IA"] == "IA";
        }
        /// <summary>
        /// 验证用户身份及权限，并返回用户名称
        /// </summary>
        public static Dictionary<string, string> ValidateAuthentication(string userToken, string needRight, string functionId, params string[] keys)
        {
            var dic = AnalyzeUserToken(userToken);
            if (!dic.ContainsKey("IA") || dic["IA"] != "IA")
            {
                if (!dic[functionId].Contains(needRight))
                {
                    throw new Exception("权限不足");
                }
            }
            var rlt = new Dictionary<string, string>(keys.Length);
            foreach (var key in keys)
            {
                rlt.Add(key, dic[key]);
            }
            return rlt;
        }
        public static Dictionary<string, string> AnalyzeUserToken(string userToken)
        {
            SymmetricCrypt crypt = new SymmetricCrypt(_type);
            userToken = crypt.Decrypt(userToken, _key, _iv);
            var tokenList = userToken.Split(_spliter_outer);

            var dic = new Dictionary<string, string>();
            foreach (var item in tokenList)
            {
                var tmp = item.Split(_spliter_inner);
                if (tmp.Length == 2)
                {
                    dic.Add(tmp[0], tmp[1]);
                }
                else
                {
                    throw new ArgumentException("口令格式错误 - " + item);
                }
            }
            return dic;
        }
        private static int GetRandom(int min, int max)
        {
            var r = new Random(Guid.NewGuid().GetHashCode());
            return r.Next(min, max);
        }
    }
}
