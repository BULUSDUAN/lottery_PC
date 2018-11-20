using System;
using System.Runtime.Serialization;

namespace Common.SharpZipLib.GZip
{
    /// <summary>
    /// GZipException represents a Gzip specific exception	
    /// </summary>
    [Serializable]
    public class GZipException : SharpZipBaseException
    {
        /// <summary>
        /// Deserialization constructor 
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/> for this constructor</param>
        /// <param name="context"><see cref="StreamingContext"/> for this constructor</param>
        protected GZipException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        /// <summary>
        /// Initialise a new instance of GZipException
        /// </summary>
        public GZipException()
        {
        }
        /// <summary>
        /// Initialise a new instance of GZipException with its message string.
        /// </summary>
        /// <param name="message">A <see cref="string"/> that describes the error.</param>
        public GZipException(string message)
            : base(message)
        {
        }
        /// <summary>
        /// Initialise a new instance of <see cref="GZipException"></see>.
        /// </summary>
        /// <param name="message">A <see cref="string"/> that describes the error.</param>
        /// <param name="innerException">The <see cref="Exception"/> that caused this exception.</param>
        public GZipException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
