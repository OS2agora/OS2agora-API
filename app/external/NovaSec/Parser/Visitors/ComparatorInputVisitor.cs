using NovaSec.Exceptions;
using NovaSec.Lang;
using NovaSec.Parser.AbstractSyntaxTree;

namespace NovaSec.Parser.Visitors
{
    internal class ComparatorInputVisitor : SelParserBaseVisitor<IComparatorInput>
    {
        public override IComparatorInput VisitComparatorinput(SelParser.ComparatorinputContext ctx)
        {
            if (ctx.IDENTIFIER() != null)
            {
                return new Identifier(ctx.IDENTIFIER().GetText());
            }
            else if (ctx.DECIMAL() != null)
            {
                return new DecimalValue(ctx.DECIMAL().GetText());
            }
            else if (ctx.STRING() != null)
            {
                return new StringArgument(ctx.STRING().GetText());
            }
            else
            {
                throw new SecurityExpressionParseException($"Cannot recognize comparator input '{ctx.GetText()}'");
            }
        }
    }
}