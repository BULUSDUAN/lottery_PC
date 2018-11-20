using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Common.SharpZipLib
{
    [Serializable]
    public class SharpZipBaseException : ApplicationException
    {
        protected SharpZipBaseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public SharpZipBaseException() { }
        public SharpZipBaseException(string message) : base(message) { }
        public SharpZipBaseException(string message, Exception innerException) : base(message, innerException) { }
    }
}
