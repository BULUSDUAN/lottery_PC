using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Org.BouncyCastle.Utilities.Encoders;

namespace KaSon.FrameWork.Common.ChinaRailway
{
    public class HS256
    {
        protected readonly System.Security.Cryptography.HashAlgorithm _digest;

        public HS256(byte[] key)
        {
            _digest = new System.Security.Cryptography.HMACSHA256(key);
        }

        public string Hash(string data)
        {
            return Hash(Encoding.UTF8.GetBytes(data));
        }

        public string Hash(byte[] data)
        {
            return Hex.ToHexString(ComputeHash(data));
        }

        protected byte[] ComputeHash(byte[] data)
        {
            return _digest.ComputeHash(data);
        }
    }
}
