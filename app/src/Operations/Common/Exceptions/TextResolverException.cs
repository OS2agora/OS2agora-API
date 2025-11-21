using System;

namespace Agora.Operations.Common.Exceptions
{
    public class TextResolverException : Exception
    {
        public TextResolverException(string message)
            : base(message)
        {
        }

        public TextResolverException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}