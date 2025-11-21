using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Agora.Entities.Common;
using Agora.Models.Common;

namespace Agora.DAOs.Mappings
{
    public static class ExpressionMapper
    {
        /// <summary>
        /// Maps filter expression for a model to a filter expression for the affiliated entity.
        /// </summary>
        /// <param name="expression">The expression to map. Not all expression node types are supported.</param>
        /// <typeparam name="TModel">The model from the model filter expression.</typeparam>
        /// <typeparam name="TEntity">The entity from the entity filter expression.</typeparam>
        /// <exception cref="NotSupportedException">
        /// The <paramref name="expression"/> contains an unsupported expression node.
        /// </exception>
        /// <returns>Entity filter expression.</returns>
        public static Expression<Func<TEntity, bool>> MapToEntityExpression<TModel, TEntity>(
            this Expression<Func<TModel, bool>> expression) where TModel : BaseModel where TEntity : BaseEntity
        {
            ParameterExpression modelParameter = expression.Parameters[0];
            ParameterExpression entityParameter = Expression.Parameter(typeof(TEntity), modelParameter.Name);
            var substitutes = new Dictionary<Expression, Expression> {{modelParameter, entityParameter}};
            Expression entityExpressionBody = MapExpressionNode(expression.Body, substitutes);
            return Expression.Lambda<Func<TEntity, bool>>(entityExpressionBody, entityParameter);
        }

        private static Expression MapExpressionNode(Expression expressionNode,
            IDictionary<Expression, Expression> substitutes)
        {
            if (expressionNode == null) return null;
            if (substitutes.TryGetValue(expressionNode, out Expression node)) return node;

            switch (expressionNode.NodeType)
            {
                case ExpressionType.Constant:
                    return expressionNode;
                case ExpressionType.Convert:
                case ExpressionType.Not:
                    return MapUnaryExpressionNode((UnaryExpression)expressionNode, substitutes);
                case ExpressionType.MemberAccess:
                    return MapMemberExpressionNode((MemberExpression)expressionNode, substitutes);
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    return MapBinaryExpressionNode((BinaryExpression)expressionNode, substitutes);
                case ExpressionType.Call:
                    return MapMethodCallExpressionNode((MethodCallExpression)expressionNode, substitutes);
                default:
                    throw new NotSupportedException(
                        $"Mapping of '{expressionNode.NodeType}' expression node type is not supported.");
            }
        }

        private static Expression MapUnaryExpressionNode(UnaryExpression unaryExpressionNode, 
            IDictionary<Expression, Expression> substitutes)
        {
            Expression newOperand = MapExpressionNode(unaryExpressionNode.Operand, substitutes);
            return Expression.MakeUnary(unaryExpressionNode.NodeType, newOperand, unaryExpressionNode.Type, 
                unaryExpressionNode.Method);
        }

        private static Expression MapMemberExpressionNode(MemberExpression memberExpressionNode,
            IDictionary<Expression, Expression> substitutes)
        {
            Expression newExpressionNode = MapExpressionNode(memberExpressionNode.Expression, substitutes);
            MemberInfo newExpressionNodeMemberInfo =
                newExpressionNode.Type.GetMember(memberExpressionNode.Member.Name).Single();
            Expression newMemberAccess = Expression.MakeMemberAccess(newExpressionNode, newExpressionNodeMemberInfo);
            
            bool nullableInSourceButNotNullableInTarget = 
                Nullable.GetUnderlyingType(memberExpressionNode.Type) == newMemberAccess.Type;
            if (nullableInSourceButNotNullableInTarget)
            {
                newMemberAccess = Expression.Convert(newMemberAccess, memberExpressionNode.Type);
            }

            return newMemberAccess;
        }

        private static Expression MapBinaryExpressionNode(BinaryExpression binaryExpressionNode,
            IDictionary<Expression, Expression> substitutes)
        {
            return Expression.MakeBinary(binaryExpressionNode.NodeType,
                MapExpressionNode(binaryExpressionNode.Left, substitutes),
                MapExpressionNode(binaryExpressionNode.Right, substitutes), binaryExpressionNode.IsLiftedToNull,
                binaryExpressionNode.Method);
        }

        private static Expression MapMethodCallExpressionNode(MethodCallExpression methodCallExpressionNode,
            IDictionary<Expression, Expression> substitutes)
        {
            Expression newObject = MapExpressionNode(methodCallExpressionNode.Object, substitutes);

            var newArguments = methodCallExpressionNode.Arguments
                .Select(arg => MapExpressionNode(arg, substitutes))
                .ToArray();

            return Expression.Call(newObject, methodCallExpressionNode.Method, newArguments);
        }
    }
}