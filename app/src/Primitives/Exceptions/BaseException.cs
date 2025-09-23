using System;

namespace BallerupKommune.Primitives.Exceptions
{
    public class BaseException : Exception
    {
        public int ErrorCode { get; set; }

        public BaseException(int errorCode)
        {
            ErrorCode = errorCode;
        }

        public BaseException(string message, int errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}