using System;

namespace NovaSec.Exceptions
{
    class SecurityExpressionEvaluationException : SecurityExpressionException
    {
        public SecurityExpressionEvaluationException(string message) : base(message)
        {
        }

        public SecurityExpressionEvaluationException(string message, Exception innerException) : base(message,
            innerException)
        {
        }
    }
}