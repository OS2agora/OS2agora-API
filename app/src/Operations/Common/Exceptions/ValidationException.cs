using System;
using FluentValidation.Results;
using System.Collections.Generic;
using System.Linq;

namespace Agora.Operations.Common.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException() : base("One or more validation failures have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IEnumerable<ValidationFailure> failures) : this()
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }

        public ValidationException(string message) : base(message) { }

        public ValidationException(string message, Exception innerException) : base(message, innerException) { }

        public ValidationException(IDictionary<string, string[]> failures) : this()
        {
            Errors = failures;
        }

        public IDictionary<string, string[]> Errors { get; }
    }

    public class InvalidCprException : ValidationException
    {
        public InvalidCprException() : base("Cpr is invalid") { }

        public InvalidCprException(string message) : base(message) { }
    }

    public class InvalidCvrException : ValidationException
    {
        public InvalidCvrException() : base("Cvr is invalid") { }

        public InvalidCvrException(string message) : base(message) { }
    }

    public class InvalidEmailException : ValidationException
    {
        public InvalidEmailException() : base("Email is invalid") { }

        public InvalidEmailException(string message) : base(message) { }

        public InvalidEmailException(string message, Exception innerException) : base(message, innerException) { }
    }
}