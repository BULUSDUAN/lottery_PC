using Common.Compression;
using Common.Cryptography;

namespace Common.Communication
{
    /// <summary>
    /// Wcfͨ�Ź����е��ֽ������������ӽ��ܼ�ѹ����ѹ
    /// </summary>
    public static class WcfByteHandler
    {
        #region ���ܽ���

        private static readonly byte[] Key = new byte[] { 14, 56, 245, 196, 151, 241, 144, 162, 77, 219, 32, 246, 229, 124, 143, 36, 70, 128, 14, 59, 160, 128, 35, 156, 67, 236, 116, 93, 97, 37, 36, 63 };
        private static readonly byte[] Iv = new byte[] { 101, 72, 82, 180, 118, 145, 45, 173, 116, 20, 74, 30, 184, 208, 72, 19 };
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="buffer">Ҫ���м��ܵ�����</param>
        /// <returns>���ܺ������</returns>
        internal static byte[] EncryptData(byte[] buffer)
        {
            using (var sc = new SymmetricCrypt(CryptType.Aes))
            {
                return sc.Encrypt(buffer, Key, Iv);
            }
        }
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="buffer">Ҫ���ܵ�����</param>
        /// <returns>���ܺ������</returns>
        internal static byte[] DecryptData(byte[] buffer)
        {
            using (var sc = new SymmetricCrypt(CryptType.Aes))
            {
                return sc.Decrypt(buffer, Key, Iv);
            }
        }

        #endregion

        #region ѹ����ѹ

        /// <summary>
        /// ѹ������
        /// </summary>
        /// <param name="buffer">��ѹ��������</param>
        /// <returns>ѹ���������</returns>
        internal static byte[] CompressData(byte[] buffer)
        {
            return ByteCompresser.Compress(buffer);
        }
        /// <summary>
        /// ��ѹ����
        /// </summary>
        /// <param name="buffer">����ѹ������</param>
        /// <returns>��ѹ�������</returns>
        internal static byte[] DecompressData(byte[] buffer)
        {
            return ByteCompresser.Decompress(buffer);
        }

        #endregion
    }
}