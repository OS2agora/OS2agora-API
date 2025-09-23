using System;

namespace NovaSec.Exceptions
{
    public class SecurityExpressionRedactionException : SecurityExpressionException
    {
        public SecurityExpressionRedactionException(string message) : base(message)
        {
        }

        public SecurityExpressionRedactionException(string message, Exception innerException) : base(message,
            innerException)
        {
        }
    }
}