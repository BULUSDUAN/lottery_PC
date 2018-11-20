using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System;
using System.Configuration;

namespace Common.Pay.hfb
{
    public class CertUtil
    {
        /// <summary>
        /// 获取签名证书私钥
        /// </summary>
        /// <returns></returns>
        public static RSACryptoServiceProvider GetSignProviderFromPfx()
        {

            string SignCertPath = hfbpay.GetInstance().SignCertPath;
                //System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificate", ConfigurationManager.AppSettings["hfb.signCert.path"].ToString());
            string SignCertPwd = hfbpay.GetInstance().SignCertPwd;
                //ConfigurationManager.AppSettings["hfb.signCert.pwd"].ToString();
            //X509Certificate2 pc = new X509Certificate2(SignCertPath, SignCertPwd, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);
            X509Certificate2 pc = new X509Certificate2(SignCertPath, SignCertPwd);
            return (RSACryptoServiceProvider)pc.PrivateKey;
        }


        /// <summary>
        /// 通过证书id，获取验证签名的证书
        /// </summary>
        /// <param name="certId"></param>
        /// <returns></returns>
        public static RSACryptoServiceProvider GetValidateProviderFromPath()
        {
            string PublicCertPath = hfbpay.GetInstance().PublicCertPath;
                //System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificate", ConfigurationManager.AppSettings["hfb.publicCert.path"].ToString());
            X509Certificate2 pc = new X509Certificate2(PublicCertPath);
            return (RSACryptoServiceProvider)pc.PublicKey.Key;
        }
    }
}
