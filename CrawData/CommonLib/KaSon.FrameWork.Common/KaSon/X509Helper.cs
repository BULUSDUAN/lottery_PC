namespace KaSon.FrameWork.Common
{
    using System;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;

    public static class X509Helper
    {
        public static X509Certificate2 InitCertificate(byte[] certdata)
        {
            return new X509Certificate2(certdata);
        }

        public static X509Certificate2 InitCertificate(string certfile)
        {
            X509Certificate2 certificate = new X509Certificate2();
            certificate.Import(certfile);
            return certificate;
        }

        public static bool VerifyCertificate(X509Certificate2 cert)
        {
            if (cert.NotAfter <= DateTime.Now)
            {
                throw new ApplicationException(" 用户证书已经过期！");
            }
            X509Chain chain = new X509Chain {
                ChainPolicy = { RevocationMode = X509RevocationMode.Online }
            };
            chain.Build(cert);
            if (chain.ChainStatus.Length > 0)
            {
                string message = " 证书检查错误：\r\n";
                foreach (X509ChainStatus status in chain.ChainStatus)
                {
                    message = message + string.Format("{0}={1}\r\n", status.Status, status.StatusInformation);
                }
                throw new ApplicationException(message);
            }
            return true;
        }

        public static bool VerifyData(X509Certificate2 cert, byte[] content, byte[] signature)
        {
            RSACryptoServiceProvider key = (RSACryptoServiceProvider) cert.PublicKey.Key;
            SHA1 halg = SHA1.Create();
            return key.VerifyData(content, halg, signature);
        }
    }
}

