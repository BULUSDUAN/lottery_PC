using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.Helper
{
   public class UsefullHelper
    {
        public static string UUID(int length = 10)
        {
            return DateTime.Now.ToString("MMddHHmmssmmm") + GetUUID(length);
        }

        public static string GetUUID(int length)
        {
            var buffer = Guid.NewGuid().ToByteArray();
            var code = System.Math.Abs(BitConverter.ToInt32(buffer, 0)).ToString();
            if (code.Length >= length)
                return code.Substring(0, length);

            var diffLength = length - code.Length;
            var diffCode = GetUUID(diffLength);
            return string.Format("{0}{1}", diffCode, code);
        }
    }
}
