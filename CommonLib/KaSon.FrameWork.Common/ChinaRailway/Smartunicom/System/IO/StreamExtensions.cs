using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaSon.FrameWork.Common.ChinaRailway
{
    public static class StreamExtensions
    {
        public static string ReadAsString(this Stream body)
        {
            using (var reader = new StreamReader(body, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        public static byte[] ReadAsBytes(this Stream body)
        {
            using (var ms = new MemoryStream())
            {
                body.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
