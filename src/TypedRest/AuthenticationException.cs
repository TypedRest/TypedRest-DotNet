using System;

#if NETFRAMEWORK
using System.Runtime.Serialization;
#endif

namespace TypedRest
{
    /// <summary>
    /// A problem occured while authenticating with a remote endpoint. Perhaps your credentials are incorrect.
    /// </summary>
    public class AuthenticationException : Exception
    {
        public AuthenticationException()
        {}

        public AuthenticationException(string message)
            : base(message)
        {}

        public AuthenticationException(string message, Exception innerException)
            : base(message, innerException)
        {}

#if NETFRAMEWORK
        protected AuthenticationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {}
#endif
    }
}
