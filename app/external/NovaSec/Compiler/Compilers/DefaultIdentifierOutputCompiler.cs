using System.Linq;
using System.Linq.Expressions;
using NovaSec.Compiler.Resolvers;
using NovaSec.Exceptions;
using NovaSec.Parser.AbstractSyntaxTree;

namespace NovaSec.Compiler.Compilers
{
    class DefaultIdentifierOutputCompiler<TResultType, TIdentifierCompiler, TIdentifierInstanceResolver> 
        : IIdentifierCompiler<ResultObjectResolver<TResultType>>
        where TIdentifierInstanceResolver : IIdentifierResolver
        where TIdentifierCompiler : IIdentifierCompiler<TIdentifierInstanceResolver>
    {
        private TIdentifierCompiler _inputCompiler;

        public DefaultIdentifierOutputCompiler(TIdentifierCompiler inputCompiler)
        {
            _inputCompiler = inputCompiler;
        }
        
        public Expression Compile(Identifier identifier, ParameterExpression identifierInstanceResolver)
        {
            var path = identifier.Name.Split('.').ToArray();
            // resultObject is the object that should be filtered or redacted
            if (path[0] == "resultObject")
            {
                Expression result =
                    Expression.Property(Expression.Convert(identifierInstanceResolver, typeof(ResultObjectResolver<TResultType>)),
                        "ResultObject");
                for (var i = 1; i < path.Length; i++)
                {
                    result = Expression.PropertyOrField(result, path[i]);
                }

                return result;
            }
            // fallback to resolving via the secondary resolver provided
            else if (_inputCompiler != null)
            {
                return _inputCompiler.Compile(identifier, identifierInstanceResolver);
            }

            throw new SecurityExpressionResolveException(
                $"{this.GetType()} unable to resolve variable for identifier with name {identifier.Name}");
        }
    }
}