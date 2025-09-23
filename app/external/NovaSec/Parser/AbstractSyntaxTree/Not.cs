namespace NovaSec.Parser.AbstractSyntaxTree
{
    public class Not : IExpression
    {
        public IExpression Expression { get; }

        public Not(IExpression expression)
        {
            Expression = expression;
        }
    }
}
