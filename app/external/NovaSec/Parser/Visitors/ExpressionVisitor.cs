using NovaSec.Exceptions;
using NovaSec.Lang;
using NovaSec.Parser.AbstractSyntaxTree;

namespace NovaSec.Parser.Visitors
{
    internal class ExpressionVisitor : SelParserBaseVisitor<IExpression>
    {
        private FunctionVisitor _functionVisitor = new FunctionVisitor();
        private ComparatorInputVisitor _comparatorInputVisitor = new ComparatorInputVisitor();

        public override IExpression VisitParenExpression(SelParser.ParenExpressionContext ctx)
        {
            return ctx.expression().Accept(this);
        }

        public override IExpression VisitNotExpression(SelParser.NotExpressionContext ctx)
        {
            return new Not(ctx.expression().Accept(this));
        }

        public override IExpression VisitComparatorExpression(SelParser.ComparatorExpressionContext ctx)
        {

            // | left=expression op=comparator right=expression #comparatorExpression
            ComparatorOperator op;
            if (ctx.op.EQ() != null)
            {
                op = ComparatorOperator.Eq;
            }
            else if (ctx.op.LEQ() != null)
            {
                op = ComparatorOperator.Leq;
            }
            else if (ctx.op.LT() != null)
            {
                op = ComparatorOperator.Lt;
            }
            else if (ctx.op.GT() != null)
            {
                op = ComparatorOperator.Gt;
            }
            else if (ctx.op.GEQ() != null)
            {
                op = ComparatorOperator.Geq;
            }
            else if (ctx.op.NEQ() != null)
            {
                op = ComparatorOperator.Neq;
            }
            else
            {
                throw new SecurityExpressionParseException($"Unrecognized comparator operator '{ctx.op.GetText()}'");
            }

            var left = ctx.left.Accept(_comparatorInputVisitor);
            var right = ctx.right.Accept(_comparatorInputVisitor);

            return new Comparator(left, op, right);
        }

        public override IExpression VisitLogicalAndExpression(SelParser.LogicalAndExpressionContext ctx)
        {
            var left = ctx.left.Accept(this);
            var right = ctx.right.Accept(this);

            return new Logical(left, LogicalOperator.And, right);
        }

        public override IExpression VisitLogicalOrExpression(SelParser.LogicalOrExpressionContext ctx)
        {
            var left = ctx.left.Accept(this);
            var right = ctx.right.Accept(this);

            return new Logical(left, LogicalOperator.Or, right);
        }

        public override IExpression VisitFunctionExpression(SelParser.FunctionExpressionContext ctx)
        {
            return ctx.function().Accept(_functionVisitor);
        }

        public override IExpression VisitBoolExpression(SelParser.BoolExpressionContext ctx)
        {
            bool value;
            if (ctx.@bool().FALSE() != null)
            {
                value = false;
            }
            else if (ctx.@bool().TRUE() != null)
            {
                value = true;
            }
            else
            {
                throw new SecurityExpressionParseException($"Unrecognized boolean value '{ctx.GetText()}'");
            }
            return new BooleanValue(value);
        }

        public override IExpression VisitIdentifierExpression(SelParser.IdentifierExpressionContext ctx)
        {
            return new Identifier(ctx.IDENTIFIER().GetText());
        }
    }
}
