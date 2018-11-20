using Common.Compression;
using Common.Cryptography;

namespace Common.Communication
{
    /// <summary>
    /// Wcf通信过程中的字节流处理。包括加解密及压缩解压
    /// </summary>
    public static class WcfByteHandler
    {
        #region 加密解密

        private static readonly byte[] Key = new byte[] { 14, 56, 245, 196, 151, 241, 144, 162, 77, 219, 32, 246, 229, 124, 143, 36, 70, 128, 14, 59, 160, 128, 35, 156, 67, 236, 116, 93, 97, 37, 36, 63 };
        private static readonly byte[] Iv = new byte[] { 101, 72, 82, 180, 118, 145, 45, 173, 116, 20, 74, 30, 184, 208, 72, 19 };
        /// <summary>
        /// 加密数据
        /// </summary>
        /// <param name="buffer">要进行加密的数据</param>
        /// <returns>加密后的数据</returns>
        internal static byte[] EncryptData(byte[] buffer)
        {
            using (var sc = new SymmetricCrypt(CryptType.Aes))
            {
                return sc.Encrypt(buffer, Key, Iv);
            }
        }
        /// <summary>
        /// 解密数据
        /// </summary>
        /// <param name="buffer">要解密的数据</param>
        /// <returns>解密后的数据</returns>
        internal static byte[] DecryptData(byte[] buffer)
        {
            using (var sc = new SymmetricCrypt(CryptType.Aes))
            {
                return sc.Decrypt(buffer, Key, Iv);
            }
        }

        #endregion

        #region 压缩解压

        /// <summary>
        /// 压缩数据
        /// </summary>
        /// <param name="buffer">待压缩的数据</param>
        /// <returns>压缩后的数据</returns>
        internal static byte[] CompressData(byte[] buffer)
        {
            return ByteCompresser.Compress(buffer);
        }
        /// <summary>
        /// 解压数据
        /// </summary>
        /// <param name="buffer">待解压的数据</param>
        /// <returns>解压后的数据</returns>
        internal static byte[] DecompressData(byte[] buffer)
        {
            return ByteCompresser.Decompress(buffer);
        }

        #endregion
    }
}