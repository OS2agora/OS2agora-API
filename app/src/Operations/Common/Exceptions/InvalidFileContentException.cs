using System;

namespace Agora.Operations.Common.Exceptions
{
    public class InvalidFileContentException : Exception
    {
        public InvalidFileContentException() : base("Failed to read file - invalid content") { }

        public InvalidFileContentException(string message) : base(message) { }

        public InvalidFileContentException(string message, Exception innerException)
        : base(message, innerException)
        {
        }
}
}