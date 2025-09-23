using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NovaSec.Compiler.Compilers;
using NovaSec.Compiler.Resolvers;
using NovaSec.Exceptions;
using NovaSec.Parser.AbstractSyntaxTree;

namespace NovaSec.Compiler
{
    /// <summary>
    /// This class can generate linq lambda function from security language expressions. This function can be executed
    /// to evaluate the expression when checking security
    /// </summary>
    public class ExpressionCompiler<TIdentifierCompiler, TFunctionCompiler, TIdentifierInstanceResolver> 
        where TIdentifierInstanceResolver : IIdentifierResolver 
        where TIdentifierCompiler : IIdentifierCompiler<TIdentifierInstanceResolver>
        where TFunctionCompiler : IFunctionCompiler<TIdentifierInstanceResolver, TIdentifierCompiler>
    {
        public Func<TIdentifierInstanceResolver, bool> Compile
        ( 
            IExpression expression,
            TIdentifierCompiler identifierCompiler,
            TFunctionCompiler functionCompiler)
        {
            try
            {
                return CompileExpression(expression, identifierCompiler, functionCompiler);
            }
            catch (ArgumentException e)
            {
                throw new SecurityExpressionEvaluationException(
                    "Failed to evaluate expression. Some cast could not convert properly, check the inner exception",
                    e);
            }
            catch (InvalidOperationException e)
            {
                throw new SecurityExpressionEvaluationException(
                    "Failed to evaluate expression. Some operation could not be executed, check the inner exception.",
                    e);
            }
        }

        private static Func<TIdentifierInstanceResolver, bool> CompileExpression(
            IExpression expression,
            TIdentifierCompiler identifierCompiler,
            TFunctionCompiler functionCompiler)
        {
            var identifierResolverParam = Expression.Parameter(typeof(TIdentifierInstanceResolver));
            var body = ConvertExpression(expression, identifierResolverParam, identifierCompiler, functionCompiler);
            var lambda = Expression.Lambda<Func<TIdentifierInstanceResolver, bool>>(
                    body,false,new List<ParameterExpression>() { identifierResolverParam}
                );
            return lambda.Compile();
        }

        private static Expression ConvertExpression(
            IExpression expression, 
            ParameterExpression identifierResolverParam, 
            TIdentifierCompiler identifierCompiler,
            TFunctionCompiler functionCompiler)
        {
            if (expression is BooleanValue b)
            {
                return Expression.Constant(b.Value);
            }
            else if (expression is Comparator c)
            {
                var left = ConvertExpression(c.Left, identifierResolverParam, identifierCompiler);
                var right = ConvertExpression(c.Right, identifierResolverParam, identifierCompiler);

                return c.Operator switch
                {
                    ComparatorOperator.Eq => Expression.Equal(left, right),
                    ComparatorOperator.Neq => Expression.NotEqual(left, right),
                    ComparatorOperator.Gt => Expression.GreaterThan(left, right),
                    ComparatorOperator.Geq => Expression.GreaterThanOrEqual(left, right),
                    ComparatorOperator.Lt => Expression.LessThan(left, right),
                    ComparatorOperator.Leq => Expression.LessThanOrEqual(left, right),
                    _ => throw new SecurityExpressionEvaluationException(
                        $"Evaluation of comparator operator {c.Operator} not implemented"),
                };
            }
            else if (expression is Function f)
            {
                return functionCompiler.Compile(f, identifierCompiler, identifierResolverParam);
            }
            else if (expression is Identifier i)
            {
                return identifierCompiler.Compile(i, identifierResolverParam);
            }
            else if (expression is Logical l)
            {
                return l.Operator switch
                {
                    LogicalOperator.Or => Expression.OrElse(
                        ConvertExpression(l.Left, identifierResolverParam, identifierCompiler, functionCompiler), 
                        ConvertExpression(l.Right, identifierResolverParam, identifierCompiler, functionCompiler)),
                    LogicalOperator.And => Expression.AndAlso(
                        ConvertExpression(l.Left, identifierResolverParam, identifierCompiler, functionCompiler), 
                        ConvertExpression(l.Right, identifierResolverParam, identifierCompiler, functionCompiler)),
                    _ => throw new SecurityExpressionEvaluationException(
                        $"Evaluation of logical operator {l.Operator} not implemented"),
                };
            }
            else if (expression is Not n)
            {
                return Expression.Not(ConvertExpression(n.Expression, identifierResolverParam, identifierCompiler, functionCompiler));
            }

            throw new SecurityExpressionEvaluationException(
                $"Evaluation IExpression for class {expression.GetType()} not implemented");
        }

        private static Expression ConvertExpression(
            IComparatorInput expression, 
            ParameterExpression identifierResolverParam, 
            TIdentifierCompiler identifierCompiler)
        {
            return expression switch
            {
                StringArgument sa => Expression.Constant(sa.Value),
                DecimalValue dv => Expression.Constant(dv.Value),
                Identifier i => identifierCompiler.Compile(i, identifierResolverParam),
                _ => throw new SecurityExpressionEvaluationException(
                    $"Evaluation comparator input for class {expression.GetType()} not implemented"),
            };
        }
    }
}