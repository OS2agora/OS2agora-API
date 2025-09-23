using NovaSec.Exceptions;
using NovaSec.Lang;
using NovaSec.Parser.AbstractSyntaxTree;
using System.Linq;

namespace NovaSec.Parser.Visitors
{
    internal class FunctionVisitor : SelParserBaseVisitor<IExpression>
    {
        public override IExpression VisitFunction(SelParser.FunctionContext context)
        {
            var name = context.IDENTIFIER().GetText();
            var arguments = context.arguments().argument().Select(x =>
            {
                if (x.IDENTIFIER() != null)
                {
                    // argument is a single identifier
                    return (IFunctionArgument) new Identifier(x.IDENTIFIER().GetText());
                }
                else if (x.list() != null)
                {
                    // argument is a list of identifiers or strings

                    return (IFunctionArgument) new ListArgument(x.list().listitems().listitem().Select(y =>
                    {
                        if (y.IDENTIFIER() != null)
                        {
                            return (IListItem) new Identifier(y.IDENTIFIER().GetText());
                        }
                        else if (y.STRING() != null)
                        {
                            return (IListItem) new StringArgument(y.STRING().GetText());
                        }
                        else
                        {
                            throw new SecurityExpressionParseException($"Unrecognized list item '{y.GetText()}'");
                        }
                    }).ToList());
                }
                else if (x.STRING() != null)
                {
                    return (IFunctionArgument) new StringArgument(x.STRING().GetText());
                }
                else
                {
                    throw new SecurityExpressionParseException($"Unrecognized function argument '{x.GetText()}'");
                }
            }).ToList();

            return new Function(name, arguments);
        }
    }
}