using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.ExceptionExtend
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
