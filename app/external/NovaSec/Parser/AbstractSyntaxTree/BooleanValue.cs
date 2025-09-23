namespace NovaSec.Parser.AbstractSyntaxTree
{
    public class BooleanValue : IExpression
    {
        public bool Value { get; }

        public BooleanValue(bool value)
        {
            Value = value;
        }
    }
}
