using System;

namespace Agora.Operations.Common.Exceptions
{
    public class PaginationException : Exception
    {
        public PaginationException(): base("An error occured while paginating content") { }

        public PaginationException(string message) : base(message) { }
    }
}
