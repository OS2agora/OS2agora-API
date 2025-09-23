using System;

namespace NovaSec.Exceptions
{
    public class SecurityExpressionException : Exception
    {
        public SecurityExpressionException(string message) : base(message)
        {
        }

        public SecurityExpressionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}