using System.Collections.Generic;

namespace NovaSec.Parser.AbstractSyntaxTree
{
    public class Function : IExpression
    {
        public string Name { get; }
        public List<IFunctionArgument> Arguments { get; }

        public Function(string name, List<IFunctionArgument> arguments)
        {
            Name = name;
            Arguments = arguments;
        }
    }
}