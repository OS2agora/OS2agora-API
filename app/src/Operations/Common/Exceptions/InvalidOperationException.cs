using System;

namespace BallerupKommune.Operations.Common.Exceptions
{
    public class InvalidOperationException : Exception
    {
        public InvalidOperationException() : base("The attempted operation was invalid.")
        {

        }

        public InvalidOperationException(string message) : base(message)
        {

        }
    }
}