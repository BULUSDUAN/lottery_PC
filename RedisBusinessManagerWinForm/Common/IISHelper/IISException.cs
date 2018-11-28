using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.IISHelper
{
    public class IISException : Exception
    {
        public IISException(IISVersion iisVersion)
            : base()
        {
            _iisVersion = iisVersion;
        }

        public IISException(string message, IISVersion iisVersion)
            : base(message)
        {
            _iisVersion = iisVersion;
        }

        public IISException(string message, Exception innerException, IISVersion iisVersion)
            : base(message, innerException)
        {
            _iisVersion = iisVersion;
        }

        private IISVersion _iisVersion;
        public IISVersion IISVersion { get { return _iisVersion; } }
    }
}
