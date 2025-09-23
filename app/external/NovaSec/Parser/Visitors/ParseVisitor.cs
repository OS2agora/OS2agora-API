using NovaSec.Lang;
using NovaSec.Parser.AbstractSyntaxTree;

namespace NovaSec.Parser.Visitors
{
    internal class ParseVisitor : SelParserBaseVisitor<IExpression>
    {
        private readonly ExpressionVisitor _visitor = new ExpressionVisitor();

        public override IExpression VisitParse(SelParser.ParseContext ctx)
        {
            return ctx.expression().Accept(_visitor);
        }
    }
}