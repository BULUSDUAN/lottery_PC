﻿using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Security;

namespace Common.Pay.sandpay
{
    /// <summary>
    /// 
    /// </summary>
    public class CryptUtils
    {
        public static byte[] AESEncrypt(byte[] Data, string Key)
        {
            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(Key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rm.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(Data, 0, Data.Length);

            return resultArray;
        }

        public static byte[] AESDecrypt(byte[] Data, string Key)
        {

            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(Key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rm.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(Data, 0, Data.Length);

            return resultArray;
        }


        public static string GuidTo16String()
        {
            string dic = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int keylen = dic.Length;
            long x = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
                x *= ((int)b + 1);
            string value = string.Empty;
            Random ra = new Random((int)(x & 0xffffffffL) | (int)(x >> 32));
            for (int i = 0; i < 16; i++)
            {
                value += dic[ra.Next() % keylen];
            }
            return value;
        }

        public static string getStringFromBytes(byte[] hexbytes, Encoding enc)
        {
            return enc.GetString(hexbytes);
        }

        public static byte[] getBytesFromString(string str, Encoding enc)
        {
            return enc.GetBytes(str);
        }

        public static byte[] asc2hex(string hexString)
        {

            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        public static string hex2asc(byte[] hexbytes)
        {
            return BitConverter.ToString(hexbytes).Replace("-", string.Empty);

        }
        /// <summary>   
        /// RSA解密   要加密较长的数据，则可以采用分段加解密的方式
        /// </summary>   
        /// <param name="xmlPrivateKey"></param>   
        /// <param name="EncryptedBytes"></param>   
        /// <returns></returns>   
        public static byte[] RSADecrypt(string xmlPrivateKey, byte[] EncryptedBytes)
        {

            using (RSACryptoServiceProvider RSACryptography = new RSACryptoServiceProvider())
            {
                RSACryptography.FromXmlString(xmlPrivateKey);


                int MaxBlockSize = RSACryptography.KeySize / 8;    //解密块最大长度限制

                if (EncryptedBytes.Length <= MaxBlockSize)
                    return RSACryptography.Decrypt(EncryptedBytes, false);

                using (MemoryStream CrypStream = new MemoryStream(EncryptedBytes))
                using (MemoryStream PlaiStream = new MemoryStream())
                {
                    Byte[] Buffer = new Byte[MaxBlockSize];
                    int BlockSize = CrypStream.Read(Buffer, 0, MaxBlockSize);

                    while (BlockSize > 0)
                    {
                        Byte[] ToDecrypt = new Byte[BlockSize];
                        Array.Copy(Buffer, 0, ToDecrypt, 0, BlockSize);

                        Byte[] Plaintext = RSACryptography.Decrypt(ToDecrypt, false);
                        PlaiStream.Write(Plaintext, 0, Plaintext.Length);

                        BlockSize = CrypStream.Read(Buffer, 0, MaxBlockSize);
                    }

                    return PlaiStream.ToArray();
                }
            }
        }


        /// <summary>   
        /// RSA加密   要加密较长的数据，则可以采用分段加解密的方式
        /// </summary>   
        /// <param name="xmlPublicKey"></param>   
        /// <param name="SourceBytes"></param>   
        /// <returns></returns>   
        public static byte[] RSAEncrypt(string xmlPublicKey, byte[] SourceBytes)
        {
            using (RSACryptoServiceProvider RSACryptography = new RSACryptoServiceProvider())
            {
                RSACryptography.FromXmlString(xmlPublicKey);

                int MaxBlockSize = RSACryptography.KeySize / 8 - 11;    //加密块最大长度限制

                if (SourceBytes.Length <= MaxBlockSize)
                    return RSACryptography.Encrypt(SourceBytes, false);

                using (MemoryStream PlaiStream = new MemoryStream(SourceBytes))
                using (MemoryStream CrypStream = new MemoryStream())
                {
                    Byte[] Buffer = new Byte[MaxBlockSize];
                    int BlockSize = PlaiStream.Read(Buffer, 0, MaxBlockSize);

                    while (BlockSize > 0)
                    {
                        Byte[] ToEncrypt = new Byte[BlockSize];
                        Array.Copy(Buffer, 0, ToEncrypt, 0, BlockSize);

                        Byte[] Cryptograph = RSACryptography.Encrypt(ToEncrypt, false);
                        CrypStream.Write(Cryptograph, 0, Cryptograph.Length);

                        BlockSize = PlaiStream.Read(Buffer, 0, MaxBlockSize);
                    }

                    return CrypStream.ToArray();
                }
            }
        }


        public static X509Certificate2 getPrivateKeyXmlFromPFX(string PFX_file, string password)
        {
            return new X509Certificate2(PFX_file, password, X509KeyStorageFlags.Exportable);
        }

        public static X509Certificate2 getPublicKeyXmlFromCer(string Cer_file)
        {
            return new X509Certificate2(Cer_file);
        }


        public static byte[] CreateSignWithPrivateKey(byte[] msgin, X509Certificate2 pfx)
        {
            HashAlgorithm SHA1 = HashAlgorithm.Create("sha1");
            byte[] hashdata = SHA1.ComputeHash(msgin);//求数字指纹

            try
            {
                AsymmetricAlgorithm prikey = pfx.PrivateKey;
            }
            catch (Exception er)
            {
                Console.WriteLine(er.ToString());
            }

            RSAPKCS1SignatureFormatter signCrt = new RSAPKCS1SignatureFormatter();
            signCrt.SetKey(pfx.PrivateKey);
            signCrt.SetHashAlgorithm("sha1");
            return signCrt.CreateSignature(hashdata);
        }

        public static bool VerifySignWithPublicKey(byte[] msgin, X509Certificate2 cer, byte[] sign)
        {
            HashAlgorithm SHA1 = HashAlgorithm.Create("sha1");
            byte[] hashdata = SHA1.ComputeHash(msgin);//求数字指纹

            RSACryptoServiceProvider signV = new RSACryptoServiceProvider();
            signV.FromXmlString(cer.PublicKey.Key.ToXmlString(false));
            return signV.VerifyData(msgin, CryptoConfig.MapNameToOID("SHA1"), sign);
        }

        public static string Base64Encoder(byte[] b)
        {
            return Convert.ToBase64String(b);
        }

        public static byte[] Base64Decoder(string base64String)
        {
            return Convert.FromBase64String(base64String);
        }

    }
}
