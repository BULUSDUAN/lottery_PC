using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaSon.FrameWork.Common.Cryptography;

namespace KaSon.FrameWork.Common.Database
{
    public interface ICryptConnection
    {
        string EncryptConnString(string connStr);
        string DecryptConnString(string connStr);
    }
    public static class Consts
    {
        public static CryptType GET_CONN_CRYPT_TYPE()
        {
            return CryptType.Rijndael;
        }
        public static string GET_CONN_CRYPT_KEY()
        {
            return "vhOrfTst0anwgoBwfzBU38XAUgrFtmOsSezay2+xYkI=";
        }
        public static string GET_CONN_CRYPT_IV()
        {
            return "FvMEdFZLwyd/jafaVGFMqw==";
        }
    }
}
