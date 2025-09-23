using NovaSec.Exceptions;
using System;
using System.Globalization;

namespace NovaSec.Parser.AbstractSyntaxTree
{
    public class DecimalValue : IComparatorInput
    {
        public decimal? Value { get; }

        public DecimalValue(string decimalAsString)
        {
            try
            {
                Value = decimal.Parse(decimalAsString, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                throw new SecurityExpressionParseException($"Cannot parse decimal '{decimalAsString}'");
            }
        }
    }
}