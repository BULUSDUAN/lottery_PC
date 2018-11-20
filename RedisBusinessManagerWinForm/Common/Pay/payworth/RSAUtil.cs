using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;


namespace Common.Pay.payworth
{
    /**
     * <b>Rsa加解密工具</b><br>
     * <br>
     * 公钥采用X509,Cer格式<br>
     * 私钥采用PKCS12加密方式的PFX私钥文件<br>
     * 加密算法为1024位的RSA\工作模式ECB\填充算法为PKCS1Padding<br>
     */
  public  class RSAUtil
    {
        public static string EncryptRSAByCer(string src, string path)
        {
            try
            {
                if(String.IsNullOrEmpty(src)){
                    throw new Exception("加密值为空[src]");
                }
                AsymmetricKeyParameter PublicKey = RsaReadUtil.getPublicKeyFromFile(path);//读取公钥
                byte[] StrSrc = System.Text.Encoding.GetEncoding("GBK").GetBytes(src);
                byte[] EncrStr = RSAEDCore(StrSrc, PublicKey, true);//加密
                //byte[] HEX = System.Text.Encoding.GetEncoding("GBK").GetBytes(Hex.ToHexString(EncrStr));//加密并转成十六进制
                byte[] Base64Str = Base64.Encode(EncrStr);//编码
                return System.Text.Encoding.Default.GetString(Base64Str);
            }
            catch (Exception e) {
                e.GetBaseException();
                throw new Exception("异常:"+e.Message);
            
            }
        }


        public static string DecryptRSAByPfx(string src, string path, string passwd) 
        {
            try
            {
                if (String.IsNullOrEmpty(src))
                {
                    throw new Exception("加密值为空[src]");
                }
                byte[] StrSrc = System.Text.Encoding.GetEncoding("GBK").GetBytes(src);
                byte[] Base64Str = Base64.Decode(StrSrc);
                AsymmetricKeyParameter PrivteKey = RsaReadUtil.getPrivateKeyFromFile(path, passwd);//读取私钥            
                byte[] DecryString = RSAEDCore(Base64Str, PrivteKey, false);
                return System.Text.Encoding.Default.GetString(DecryString);
            }
            catch (Exception e) {
                e.GetBaseException();
                throw new Exception("异常:" + e.Message);
            }
        }

        /*加密核心
         * Src加密内容
         * 
         * PFXorCER  证书
         * 
         * Mode  加密或解密  true  OR  false
         */
        private static byte[] RSAEDCore(byte[] Src,AsymmetricKeyParameter PFXorCER,bool Mode) 
        {
            IAsymmetricBlockCipher engine = new RsaEngine();
            IBufferedCipher Cipher = CipherUtilities.GetCipher("RSA/ECB/PKCS1Padding");//加密标准

            Cipher.Init(Mode, PFXorCER);//初始加密程序

            byte[] EDString = null;

            int blockSize = Cipher.GetBlockSize();
            for (int i = 0; i < Src.Length; i += blockSize)
            {
                byte[] outBytes = Cipher.DoFinal(Subarray(Src, i, i + blockSize));//数据加密
                EDString = AddAll(EDString, outBytes);
			}
            return EDString;
        }

        /*
         * 数据分块算法
         * 
         * array：源长度
         * 
         * startIndexInclusive:开始长度
         * 
         * endIndexExclusive：结束长度
         * 
         */
        private static byte[] Subarray(byte[] array, int startIndexInclusive, int endIndexExclusive)
        {
            if (array == null)
            {
                throw new IOException("byte[] array内容为空！异常：subarray(byte[],int,int)");
            }
            if (endIndexExclusive > array.Length)
            {
                endIndexExclusive = array.Length;
            }
            int newSize = endIndexExclusive - startIndexInclusive;
            if (newSize <= 0)
            {
                return new byte[0];
            }
            byte[] subarray = new byte[newSize];
            Array.Copy(array, startIndexInclusive, subarray, 0, newSize);
            return subarray;
        }

        /*
         * 数据拼接
         * 
         * array1   
         * 
         * array2
         * 
         * AddAll=array1+array2
         */
        private static byte[] AddAll(byte[] array1, byte[] array2)
        {
            if (array1 == null)
            {
                return Clone(array2);
            }
            else if (array2 == null)
            {
                return Clone(array1);
            }
            byte[] joinedArray = new byte[array1.Length + array2.Length];
            Array.Copy(array1, 0, joinedArray, 0, array1.Length);
            Array.Copy(array2, 0, joinedArray, array1.Length, array2.Length);
            return joinedArray;
        }

        /*
         *=======================================
         */

        private static byte[] Clone(byte[] array)
        {
            if (array == null)
            {
                return null;
            }
            return (byte[])array.Clone() ;
        }

    }
}
