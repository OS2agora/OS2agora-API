using Antlr4.Runtime;
using NovaSec.Exceptions;
using NovaSec.Lang;
using NovaSec.Parser.AbstractSyntaxTree;
using NovaSec.Parser.Visitors;

namespace NovaSec.Parser
{
    public class SecurityExpressionParser
    {
        private readonly ParseVisitor _visitor = new ParseVisitor();

        public IExpression Parse(string securityExpression)
        {
            var charStream = new AntlrInputStream(securityExpression);
            var lexer = new SelLexer(charStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new SelParser(tokenStream);
            var errorListener = new SecurityExpressionErrorListener();
            parser.AddErrorListener(errorListener);
            var tree = parser.parse();
            if (errorListener.ErrorOccured)
            {
                throw new SecurityExpressionParseException($"Failed to parse '{securityExpression}'\n{string.Join("\n", errorListener.ErrorMessages)}");
            }
            var expression = tree.Accept(_visitor);
            return expression;
        }
    }
}
