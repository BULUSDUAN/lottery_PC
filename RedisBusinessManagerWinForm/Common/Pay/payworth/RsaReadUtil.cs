using System;
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;

namespace Common.Pay.payworth
{
    public class RsaReadUtil
    {
        /*
         * 
        ==================读取私钥==========================
         * path   证书路径
         * 
         * pwd  证书密码
         * 
         */
         public static AsymmetricKeyParameter getPrivateKeyFromFile( string path,string pwd)
        {
            FileStream fs = File.OpenRead(path);      //path路径下证书           
            char[] passwd = pwd.ToCharArray();
            
            Pkcs12Store store = new Pkcs12StoreBuilder().Build();  
            store.Load(fs, passwd); //加载证书  
            string alias = null;  
            foreach (string str in store.Aliases)  
            {  
                if (store.IsKeyEntry(str))  
                    alias = str;  
            }
            AsymmetricKeyEntry keyEntry = store.GetKey(alias);
            return keyEntry.Key;

        }

         /*
          ==================读取公钥==========================
          * 
          * path   证书路径
          * 
          */

         public static AsymmetricKeyParameter getPublicKeyFromFile(string path) 
         {
             FileStream fs = File.OpenRead(path);
             X509CertificateParser X509RC = new X509CertificateParser();
             X509Certificate Temp = X509RC.ReadCertificate(fs);
             return Temp.GetPublicKey();
      
         }

    }
}
