using NovaSec.Exceptions;

namespace NovaSec.Parser.AbstractSyntaxTree
{
    public enum ComparatorOperator
    {
        Gt,
        Geq,
        Lt,
        Leq,
        Eq,
        Neq
    }

    public class Comparator : IExpression
    {
        public IComparatorInput Left { get; }
        public IComparatorInput Right { get; }
        public ComparatorOperator Operator { get; }

        public Comparator(IComparatorInput left, ComparatorOperator op, IComparatorInput right)
        {
            Left = left;
            Right = right;
            Operator = op;

            VerifyInputs();
        }

        private void VerifyInputs()
        {
            var hasString = Left is StringArgument || Right is StringArgument;
            var hasDecimal = Left is DecimalValue || Right is DecimalValue;

            if (Operator != ComparatorOperator.Eq && Operator != ComparatorOperator.Neq)
            {
                if (hasString)
                {
                    throw new SecurityExpressionParseException(
                        "Cannot use other comparison operators than == or != when using string as operand");
                }
            }
            // is == or !=
            else
            {
                if (hasDecimal && hasString)
                {
                    throw new SecurityExpressionParseException(
                        "Cannot compare string and decimal");
                }
            }
        }
    }
}