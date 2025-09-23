using System;
using System.Collections.Generic;

namespace BallerupKommune.Operations.Common.Exceptions
{
    public class EmailException : Exception
    {
        public EmailException() : base("One or more errors occurred while sending an e-mail")
        {
            Errors = new List<string>();
        }

        public EmailException(IEnumerable<string> failures) : this()
        {
            Errors = failures;
        }

        public IEnumerable<string> Errors { get; }
    }
}