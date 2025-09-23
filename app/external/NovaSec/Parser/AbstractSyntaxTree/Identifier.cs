namespace NovaSec.Parser.AbstractSyntaxTree
{
    public class Identifier : IExpression, IComparatorInput, IFunctionArgument, IListItem
    {
        public string Name { get; }

        public Identifier(string name)
        {
            Name = name;
        }
    }
}