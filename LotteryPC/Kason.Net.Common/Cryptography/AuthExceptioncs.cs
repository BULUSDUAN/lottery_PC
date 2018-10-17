using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Kason.Net.Common.Cryptography
{
    public class AuthException : Exception
    {
        public AuthException(string message)
            : base(message)
        {
        }
        public AuthException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
