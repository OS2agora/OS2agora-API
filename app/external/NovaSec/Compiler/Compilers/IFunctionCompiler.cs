using System.Linq.Expressions;
using NovaSec.Compiler.Resolvers;
using NovaSec.Parser.AbstractSyntaxTree;

namespace NovaSec.Compiler.Compilers
{
    public interface IFunctionCompiler<TIdentifierInstanceResolver, in TIdentifierCompiler>
        where TIdentifierInstanceResolver : IIdentifierResolver
        where TIdentifierCompiler : IIdentifierCompiler<TIdentifierInstanceResolver>
    {
        Expression Compile(
            Function function, 
            TIdentifierCompiler identifierCompiler, 
            ParameterExpression identifierResolverInstance);
    }
}