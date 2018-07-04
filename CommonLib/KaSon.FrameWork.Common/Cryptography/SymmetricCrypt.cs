using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace KaSon.FrameWork.Common.Cryptography
{
    /// <summary>
    /// 提供.Net自身的对称加密算法的包装
    /// </summary>
    [Serializable]
    public sealed class SymmetricCrypt : IDisposable
    {
        private SymmetricAlgorithm _symmetricAlgorithm;
        /// <summary>
        /// 构造函数，确定用哪一种对称加密算法来初始化加密对象。
        /// </summary>
        /// <param name="cryptType">加密算法类型,CryptType枚举</param>
        public SymmetricCrypt(CryptType cryptType)
        {
            switch (cryptType)
            {
                case CryptType.Rc2:
                    _symmetricAlgorithm = new RC2CryptoServiceProvider();
                    break;
                case CryptType.Rijndael:
                    _symmetricAlgorithm = new RijndaelManaged();
                    break;
                case CryptType.TripleDes:
                    _symmetricAlgorithm = new TripleDESCryptoServiceProvider();
                    break; 
                case CryptType.Aes:
                    _symmetricAlgorithm = new AesCryptoServiceProvider();
                    break;
                case CryptType.Des:
                    _symmetricAlgorithm = new DESCryptoServiceProvider();
                    break;
            }
        }
        /// <summary>
        /// 生成用于SymmetricCrypt对象的随机密钥 (Key)与IV值。
        /// </summary>
        /// <param name="key">以out方式返回SymmetricCrypt对象生成的随机密钥key值</param>
        /// <param name="iv">以out方式返回SymmetricCrypt对象生成的随机IV值</param>
        public void GenKey(out byte[] key, out byte[] iv)
        {
            _symmetricAlgorithm.GenerateKey();
            _symmetricAlgorithm.GenerateIV();
            key = _symmetricAlgorithm.Key;
            iv = _symmetricAlgorithm.IV;
        }
        /// <summary>
        /// 生成用于SymmetricCrypt对象的随机密钥 (Key)与IV值。
        /// </summary>
        /// <param name="key">以out方式返回SymmetricCrypt对象生成的随机密钥key(base64格式字符串)值</param>
        /// <param name="iv">以out方式返回SymmetricCrypt对象生成的随机IV(base64格式字符串)值</param>
        /// <remarks>
        /// 返回的密钥与IV值应当成对使用，否则不能产生正确的结果
        /// </remarks>
        public void GenKey(out string key, out string iv)
        {
            byte[] bkey, biv;
            GenKey(out bkey, out biv);
            key = Convert.ToBase64String(bkey);
            iv = Convert.ToBase64String(biv);
        }
        /// <summary>
        /// 用对称加密算法加密数据
        /// </summary>
        /// <param name="buffer">要加密的明文数据,不能为空！</param>
        /// <param name="akey">对称算法的机密密钥（Key），不能为空。</param>
        /// <param name="aiv">对称算法的初始化向量 (IV)，不能为空。</param>
        /// <returns>返回加密后的密文数据</returns>
        public byte[] Encrypt(byte[] buffer, byte[] akey, byte[] aiv)
        {
            var ct = _symmetricAlgorithm.CreateEncryptor(akey, aiv);
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, ct, CryptoStreamMode.Write))
                {
                    cs.Write(buffer, 0, buffer.Length);
                }
                return ms.ToArray();
            }
        }
        /// <summary>
        /// 用对称加密算法加密数据
        /// </summary>
        /// <param name="str">要加密的明文数据,不能为空！</param>
        /// <param name="akey">对称算法的机密密钥（Key,Base64格式字符串），不能为空。</param>
        /// <param name="aiv">对称算法的初始化向量 (IV,Base64格式字符串)，不能为空。</param>
        /// <returns>返回加密后的密文数据(Base64格式字符串)</returns>
        /// <remarks>
        /// 输入值akey,aiv,必须采用调用GenKey方法获取密钥与IV值，否则不能保证加密的密文在解密的时候得到正确的结果。
        /// </remarks>
        public string Encrypt(string str, string akey, string aiv)
        {
            string result = string.Empty;
            byte[] key = null, iv = null, data = null, edata = null;
            key = Convert.FromBase64String(akey);
            iv = Convert.FromBase64String(aiv);
            data = Encoding.UTF8.GetBytes(str);
            edata = Encrypt(data, key, iv);
            result = Convert.ToBase64String(edata);

            return result;
        }
        /// <summary>
        /// 用对称加密算法解密数据
        /// </summary>
        /// <param name="bts">要解密的密文数据</param>
        /// <param name="akey">加密时所用到的对称算法的机密密钥（Key），不能为空。</param>
        /// <param name="aiv">加密时所用到的对称算法的初始化向量 (IV)，不能为空。</param>
        /// <returns>返回解密后的明文数据</returns>
        public byte[] Decrypt(byte[] bts, byte[] akey, byte[] aiv)
        {
            var ct = _symmetricAlgorithm.CreateDecryptor(akey, aiv);
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, ct, CryptoStreamMode.Write))
                {
                    cs.Write(bts, 0, bts.Length);
                }
                return ms.ToArray();
            }
        }
        /// <summary>
        /// 用对称加密算法解密数据
        /// </summary>
        /// <param name="str">要解密的密文数据(Base64格式字符串)</param>
        /// <param name="akey">加密时所用到的对称算法的机密密钥（Key,Bas64格式字符串），不能为空。</param>
        /// <param name="aiv">加密时所用到的对称算法的初始化向量 (IV,Bas64格式字符串)，不能为空。</param>
        /// <returns>返回解密后的明文数据(UTF8格式字符串)</returns>
        /// <remarks>
        /// 输入值akey,aiv,必须为加密时调用GenKey方法所产生的密钥Key值与IV值，否则不能保证得到正确的结果。
        /// </remarks>
        public string Decrypt(string str, string akey, string aiv)
        {
            byte[] key = Convert.FromBase64String(akey);
            byte[] iv = Convert.FromBase64String(aiv);
            byte[] data = Convert.FromBase64String(str);

            byte[] result = Decrypt(data, key, iv);
            return Encoding.UTF8.GetString(result);
        }

        #region IDisposable 接口成员

        private bool _isDisposed;
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                try
                {
                    _symmetricAlgorithm.Clear();
                    _symmetricAlgorithm = null;
                }
                finally
                {
                    _isDisposed = true;
                }
            }
        }

        #endregion
    }
    /// <summary>
    /// 对称加密算法类型
    /// </summary>
    public enum CryptType
    {
        /// <summary>
        /// AES加密方式
        /// </summary>
        Aes = 0,
        /// <summary>
        /// DES加密方式
        /// </summary>
        Des = 1,
        /// <summary>
        /// RC2加密方式
        /// </summary>
        Rc2 = 2,
        /// <summary>
        /// Rijndael加密方式
        /// </summary>
        Rijndael = 3,
        /// <summary>
        /// TripleDES加密方式
        /// </summary>
        TripleDes = 4
    }
}
