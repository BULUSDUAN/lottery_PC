using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.Common.Cryptography
{
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
                    throw new AuthException("权限不足");
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
