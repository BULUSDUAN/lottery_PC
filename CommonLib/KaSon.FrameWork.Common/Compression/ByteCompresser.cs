using System.IO;
using System.IO.Compression;

namespace KaSon.FrameWork.Common.Compression
{
    /// <summary>
    /// 字节流压缩器
    /// </summary>
    public static class ByteCompresser
    {
        private const int CSIZE = 1024;
        /// <summary>
        /// 提供内部使用，压缩字流的方法
        /// </summary>
        public static byte[] Compress(byte[] data)
        {
            using (var ms = new MemoryStream())
            {
                using (var zip = new DeflateStream(ms, CompressionMode.Compress, true))
                {
                    zip.Write(data, 0, data.Length);
                    zip.Close();
                }
                return ms.ToArray();
            }
        }
        /// <summary>
        /// 提供内部使用，解压缩字流的方法
        /// </summary>
        public static byte[] Decompress(byte[] data)
        {
            using (var ms = new MemoryStream())
            {
                ms.Write(data, 0, data.Length);
                ms.Flush();
                ms.Position = 0;
                using (var zip = new DeflateStream(ms, CompressionMode.Decompress, true))
                {
                    using (var os = new MemoryStream())
                    {
                        var buf = new byte[CSIZE];
                        while (true)
                        {
                            var l = zip.Read(buf, 0, CSIZE);
                            if (l == 0) l = zip.Read(buf, 0, CSIZE);
                            os.Write(buf, 0, l);
                        
                            if (l == 0) break;
                        }
                        return os.ToArray();
                    }
                }
            }
        }
    }
}