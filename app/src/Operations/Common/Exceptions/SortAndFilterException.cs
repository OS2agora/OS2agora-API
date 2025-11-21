using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace Agora.Operations.Common.Exceptions
{
    public class SortAndFilterException : Exception
    {
        public IDictionary<string, string[]> Errors { get; }
        public SortAndFilterException() : base("Error encountered when performing sorting and filtering")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public SortAndFilterException(IEnumerable<ValidationFailure> failures) : this()
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }

        public SortAndFilterException(IDictionary<string, string[]> failures) : this()
        {
            Errors = failures;
        }
    }

    public static class SortAndFilterExceptionTypes
    {
        public static readonly string SortingError = "Sorting Error";
        public static readonly string FilterError = "Filter Error";
        public static readonly string ProcessingError = "Processing Error";

    }
}
