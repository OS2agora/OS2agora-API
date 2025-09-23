namespace NovaSec.Parser.AbstractSyntaxTree
{
    public enum LogicalOperator
    {
        And,
        Or
    }

    public class Logical : IExpression
    {
        public IExpression Left { get; }
        public IExpression Right { get; }
        public LogicalOperator Operator { get; }

        public Logical(IExpression left, LogicalOperator op, IExpression right)
        {
            Left = left;
            Right = right;
            Operator = op;
        }
    }
}