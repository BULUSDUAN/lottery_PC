using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Common.Cryptography
{
    /// <summary>
    /// by 邓自强
    /// 非对称加密、解密；签名、验证签名
    /// 证书的生成：
    /// 第一：vs2010 命令提示行 运行：makecert -r -pe -n "cn=XT" -$ commercial -a sha1 -cy authority -ss my -sr currentuser
    /// 第二：运行CMD-->MMC, 然后 “文件”-->“添加/删除管理单元”-->“证书”-->“添加”-->“我的用户帐户”，在“个人”-->“证书”里面可以看到生成的证书
    /// 第三：右键 证书-->“所有任务”-->“导出”，公钥是.cer，私钥是.pfx
    /// 第四：证书导出公钥和私钥:
    /// X509Certificate2 pc = new X509Certificate2("E:\\xinticai.pfx", "xinticai", X509KeyStorageFlags.Exportable);
    /// var key_public = pc.PublicKey.Key.ToXmlString(false);
    /// var key_private = pc.PrivateKey.ToXmlString(true);
    /// </summary>
    public class RSACrypt
    {
        public RSACrypt()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="cerPath">证书路径</param>
        /// <param name="cerPwd">证书密码</param>
        public RSACrypt(string cerPath, string cerPwd)
        {
            SetKeys(cerPath, cerPwd);
        }

        /// <summary>
        /// 公钥xml
        /// </summary>
        public string KeyPublicXml { get; set; }
        /// <summary>
        /// 私钥xml
        /// </summary>
        public string KeyPrivateXml { get; set; }


        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="xmlPublicKey">公钥xml</param>
        /// <param name="sourceStr">源字符串</param>
        /// <returns></returns>
        public string RSAEncrypt(string xmlPublicKey, string sourceStr)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(xmlPublicKey);
            byte[] bytes = new UnicodeEncoding().GetBytes(sourceStr);
            return Convert.ToBase64String(provider.Encrypt(bytes, false));
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="cerPath">证书路径</param>
        /// <param name="cerPwd">证书密码</param>
        /// <param name="sourceStr">源字符串</param>
        /// <returns></returns>
        public string RSAEncrypt(string cerPath, string cerPwd, string sourceStr)
        {
            SetKeys(cerPath, cerPwd);

            return RSAEncrypt(this.KeyPublicXml, sourceStr);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="xmlPrivateKey">私钥xml</param>
        /// <param name="code">待解密字符串（加密后的字符串）</param>
        /// <returns></returns>
        public string RSADecrypt(string xmlPrivateKey, string code)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(xmlPrivateKey);
            byte[] rgb = Convert.FromBase64String(code);
            byte[] bytes = provider.Decrypt(rgb, false);
            return new UnicodeEncoding().GetString(bytes);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="cerPath">证书路径</param>
        /// <param name="cerPwd">证书密码</param>
        /// <param name="code">待解密字符串（加密后的字符串）</param>
        /// <returns></returns>
        public string RSADecrypt(string cerPath, string cerPwd, string code)
        {
            SetKeys(cerPath, cerPwd);

            return RSADecrypt(this.KeyPrivateXml, code);
        }

        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="xmlPrivateKey">私钥xml</param>
        /// <param name="sourceStr">源字符串</param>
        /// <param name="type">签名类型，0或是其它</param>
        /// <returns></returns>
        public string SignData(string xmlPrivateKey, string sourceStr, int type = 0)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(xmlPrivateKey);

            byte[] data = Encoding.UTF8.GetBytes(sourceStr);
            byte[] endata = provider.SignData(data, type == 0 ? "SHA1" : "MD5");
            return Convert.ToBase64String(endata);
        }

        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="cerPath">证书路径</param>
        /// <param name="cerPwd">证书密码</param>
        /// <param name="sourceStr">源字符串</param>
        /// <param name="type">签名类型，0或是其它</param>
        /// <returns></returns>
        public string SignData(string cerPath, string cerPwd, string sourceStr, int type = 0)
        {
            SetKeys(cerPath, cerPwd);

            return SignData(this.KeyPrivateXml, sourceStr, type);
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="xmlPublicKey">公钥xml</param>
        /// <param name="code">签名后的字符串</param>
        /// <param name="source">源字符串</param>
        /// <returns></returns>
        public bool VerifySignature(string xmlPublicKey, string code, string source, int type = 0)
        {
            string endata = Convert.ToBase64String(Encoding.UTF8.GetBytes(source));
            byte[] sign = Convert.FromBase64String(code);
            return VerifySignature(xmlPublicKey, sign, endata, type);
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="cerPath">证书路径</param>
        /// <param name="cerPwd">证书密码</param>
        /// <param name="code">签名后的字符串</param>
        /// <param name="source">源字符串</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool VerifySignature(string cerPath, string cerPwd, string code, string source, int type = 0)
        {
            SetKeys(cerPath, cerPwd);

            return VerifySignature(this.KeyPublicXml, code, source, type);
        }

        private bool VerifySignature(string xmlPublicKey, byte[] signature, string signedData, int type = 0)
        {
            try
            {
                RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
                provider.FromXmlString(xmlPublicKey);

                byte[] hash = Convert.FromBase64String(signedData);
                return provider.VerifyData(hash, type == 0 ? "SHA1" : "MD5", signature);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private void SetKeys(string cerPath, string cerPwd)
        {
            X509Certificate2 pc = new X509Certificate2(cerPath, cerPwd, X509KeyStorageFlags.Exportable);
            this.KeyPublicXml = pc.PublicKey.Key.ToXmlString(false);
            if (!string.IsNullOrEmpty(cerPwd))
                this.KeyPrivateXml = pc.PrivateKey.ToXmlString(true);
        }
    }
}
